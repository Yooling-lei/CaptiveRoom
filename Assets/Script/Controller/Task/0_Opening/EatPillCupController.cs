using System;
using IngameDebugConsole;
using Script.Controller.Interactable;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Task._0_Opening
{
    /// <summary>
    /// 吃药用的杯子控制器
    /// </summary>
    public class EatPillCupController : PuzzleSceneItemController
    {
        public Animator cupAnimator;
        public Rigidbody cupRigidbody;
        private bool _isPlaying;
        private readonly string _eatPillAnim = "EatPill";
        private bool _responseCollision;

        private void Start()
        {
            DebugLogConsole.AddCommand("RR", "Success Eat Pill", OnPuzzleSuccess);
            if (cupRigidbody != null) cupRigidbody.useGravity = false;
        }

        protected override void OnPuzzleSuccess()
        {
            base.OnPuzzleSuccess();
            TaskManager.Instance.FinishTask("TakePill");
            cupAnimator.Play(_eatPillAnim);
            _isPlaying = true;
            // Destroy(GetComponent<BoxCollider>());
        }

        private void OnEatPillAnimationEnd()
        {
            cupRigidbody.useGravity = true;
            _responseCollision = true;
            // TODO: 播放摔碎杯子动画
            // TODO: 播放恐怖音效
            // 播放完后,生成碎玻璃GameObject,触发下个任务
            NextTask();
        }

        // 当杯子的碰撞体碰撞到地面,Log
        public void OnParentCollisionEnter(Collision other)
        {
            if (!_responseCollision) return;
            Debug.Log("碰撞");
            // 替换成碎玻璃
            // if(_isPlaying)
        }

        private void Update()
        {
            if (!_isPlaying) return;
            var cut = cupAnimator.GetCurrentAnimatorStateInfo(0);

            if (!cut.IsName(_eatPillAnim) || !(cut.normalizedTime >= 1)) return;
            _isPlaying = false;
            OnEatPillAnimationEnd();
        }


        private void NextTask()
        {
            TaskManager.Instance.AddTask("FixCup");
        }
    }
}