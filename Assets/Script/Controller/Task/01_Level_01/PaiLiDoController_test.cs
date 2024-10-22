using System;
using System.Collections;
using Script.Manager;
using Script.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Controller.Task._01_Level_01
{
    public class PaiLiDoController_test : MonoBehaviour
    {
        // 初始化参数
        public Image filmingMarkImage;
        Camera _mainCamera;
        PlayerInputReceiver _input;
        private Animator animator;
        private bool init;
        private GameObject _player;
        // 物体的meshRender
        private MeshRenderer _meshRenderer;

        
        // 是否被主角捡起
        private bool _picked;
        // 拍照CD
        private float _takePhotoCd;
        private static readonly int Speed = Animator.StringToHash("Speed");

        public void PickUpPaiLiDo()
        {
            // FIXME: 捡起拍立得
            transform.SetParent(Camera.main.transform);
            transform.localPosition = new Vector3(0.3f, -0.2f, 0.7f);
            _input = GameManager.Instance.playerInputReceiver;
            _picked = true;
        }

        void InitFunc()
        {
            _mainCamera = Camera.main;
            _player = GameManager.Instance?.player;
            _input = GameManager.Instance?.playerInputReceiver;
            _meshRenderer = GetComponent<MeshRenderer>();
            animator = GetComponent<Animator>();

            if (_input != null && _mainCamera != null) init = true;
        }


        private void OnEnable()
        {
            InitFunc();
        }


        private void Update()
        {
            if (!init)
            {
                InitFunc();
                return;
            }
            ControlAnimation();
            ControlTakePhoto();
            ToolBox.DeductTimer(ref _takePhotoCd);
        }

        private bool _animationDone;
        /// <summary>
        /// 拍照动画控制
        /// </summary>
        private void ControlAnimation()
        {
            _animationDone = false;
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (_input.viewFocus)
            {
                if (stateInfo.normalizedTime >= 1.0f)
                {
                    StopAnimation();
                    _animationDone = true;
                }
                else StartAnimation();
            }
            else
            {
                if (stateInfo.normalizedTime <= 0.01f) StopAnimation();
                else StartAnimation(true);
            }

            filmingMarkImage.enabled = _animationDone;
            _meshRenderer.enabled = !_animationDone;
            return;

            void StopAnimation()
            {
                animator.SetFloat(Speed, 0f);
            }

            void StartAnimation(bool back = false)
            {
                animator.SetFloat(Speed, back ? -2f : 2f);
            }
        }

        /// <summary>
        /// 拍照和拍照时检验
        /// </summary>
        private void ControlTakePhoto()
        {
            // 按下左键
            if (!_input.fire) return;
            _input.fire = false;

            if (!_animationDone) return;
            if (_takePhotoCd > 0)
            {
                Debug.Log("CD中");
                return;
            }

            // TODO: 后面还需要防抖等
            _takePhotoCd = 1f;
            var isChecked = CheckStandPoint();
            Debug.Log("拍照" + isChecked);
        }

        // 用于辅助校验拍照是否正确的 站立点位 和 视角点位
        public GameObject standPoint;
        public GameObject viewPoint;

        // 校验主角是否站立在站立点位附近, 且视角朝向视角点位
        private bool CheckStandPoint()
        {
            // 柑橘主角站立点位和视角朝向点位判断
            var currentPosition = _player.transform.position;
            var distance = Vector3.Distance(standPoint.transform.position, currentPosition);
            if (distance < 1f) return false;
            var screenPoint = _mainCamera.WorldToViewportPoint(viewPoint.transform.position);
            return screenPoint is { x: >= 0.45f and <= 0.55f, y: >= 0.45f and <= 0.55f };
        }

        private IEnumerator TestGet()
        {
            yield return new WaitForSeconds(1);
            PickUpPaiLiDo();
        }
    }
}