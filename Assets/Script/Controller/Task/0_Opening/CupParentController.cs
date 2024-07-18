using System;
using IngameDebugConsole;
using Script.Controller.Interactable;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Task._0_Opening
{
    public class CupParentController : PuzzleSceneItemController
    {
        [Header("完好杯子")] public Animator cupAnimator;
        public GameObject normalCup;
        private readonly string _eatPillAnim = "EatPill";

        [Header("破碎杯子")] public GameObject brokenCup;

        private Rigidbody cupRigidbody;
        private bool _isPlayingEating;
        private bool _responseCollision;

        private void Start()
        {
            cupRigidbody = GetComponent<Rigidbody>();
            if (cupRigidbody != null) cupRigidbody.useGravity = false;
            DebugLogConsole.AddCommand("EatPill", "EatPill", OnPuzzleSuccess);
        }

        private void Update()
        {
            // 监测吃药动画结束
            WatchEatPillAnimation();
        }

        private void WatchEatPillAnimation()
        {
            if (!_isPlayingEating) return;
            var cut = cupAnimator.GetCurrentAnimatorStateInfo(0);

            if (!cut.IsName(_eatPillAnim) || !(cut.normalizedTime >= 1)) return;
            _isPlayingEating = false;
            OnEatPillAnimationEnd();
        }

        private void SyncBrokenCup()
        {
            brokenCup.transform.position = normalCup.transform.position;
        }


        // 杯子摔倒地上 (生成碎玻璃和下个任务)
        private void OnCollisionEnter(Collision other)
        {
            if (!_responseCollision) return;
            // 替换成碎玻璃杯
            Destroy(cupRigidbody);
            SyncBrokenCup();
            normalCup.SetActive(false);
            brokenCup.SetActive(true);
            NextTask();
        }

        protected override void OnPuzzleSuccess()
        {
            base.OnPuzzleSuccess();
            // 完成TakePill任务,开始播放动画
            TaskManager.Instance.FinishTask("TakePill");
            // TODO: 播放音效
            cupAnimator.Play(_eatPillAnim);
            _isPlayingEating = true;
        }

        private void OnEatPillAnimationEnd()
        {
            cupRigidbody.useGravity = true;
            _responseCollision = true;
        }

        private void NextTask()
        {
            TaskManager.Instance.AddTask("FixCup");
        }
    }
}