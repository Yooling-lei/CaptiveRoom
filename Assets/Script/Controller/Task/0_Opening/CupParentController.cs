﻿using System;
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


        private void OnCollisionEnter(Collision other)
        {
            if (!_responseCollision) return;

            // TODO: 替换成碎玻璃
            Destroy(cupRigidbody);
            SyncBrokenCup();
            normalCup.SetActive(false);
            brokenCup.SetActive(true);
        }

        protected override void OnPuzzleSuccess()
        {
            base.OnPuzzleSuccess();
            // 完成TakePill任务,开始播放动画
            // TaskManager.Instance.FinishTask("TakePill");
            cupAnimator.Play(_eatPillAnim);
            _isPlayingEating = true;
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

        private void NextTask()
        {
            TaskManager.Instance.AddTask("FixCup");
        }
    }
}