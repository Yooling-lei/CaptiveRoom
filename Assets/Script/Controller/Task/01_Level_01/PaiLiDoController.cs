using System.Collections;
using Script.Controller.Interactable;
using Script.Manager;
using Script.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Controller.Task._01_Level_01
{
    public class PaiLiDoController : BaseInteractableController
    {
        // 初始化参数
        public Image filmingMarkImage;
        private Camera _mainCamera;
        private PlayerInputReceiver _input;
        private Animator _animator;

        private GameObject _player;

        // 物体的meshRender
        private MeshRenderer _meshRenderer;
        private Collider _collider;
        private bool init;


        // 是否被主角捡起
        private bool _picked;

        // 拍照CD
        private float _takePhotoCd;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int isPlaying = Animator.StringToHash("isPlaying");

        // 捡起时的动画控制
        public float flyTime = 1f;

        void InitFunc()
        {
            _collider = GetComponent<Collider>();
            _mainCamera = Camera.main;
            _player = GameManager.Instance?.player;
            _input = GameManager.Instance?.playerInputReceiver;
            _meshRenderer = GetComponent<MeshRenderer>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!_picked) return;
            ControlAnimation();
            ControlTakePhoto();
            ToolBox.DeductTimer(ref _takePhotoCd);
        }

        #region 捡起前

        public override void OnInteract()
        {
            InitFunc();
            if (_collider is not null) _collider.enabled = false;
            StartCoroutine(PickUpItem());
        }

        private IEnumerator PickUpItem()
        {
            var destiny = GameManager.Instance.player.transform.position;
            var moveObject = transform.parent;
            yield return StartCoroutine(ToolBox.SmoothMoveToPosition(moveObject, destiny, flyTime));
            PickUpPaiLiDo();
        }

        private void PickUpPaiLiDo()
        {
            transform.SetParent(_mainCamera.transform);
            transform.localPosition = new Vector3(0.3f, -0.2f, 0.7f);

            _input = GameManager.Instance.playerInputReceiver;
            _picked = true;
            _animator.SetBool(isPlaying, true);
        }

        #endregion

        #region 拾取后拍照等

        private bool _animationDone;

        /// <summary>
        /// 拍照动画控制
        /// </summary>
        private void ControlAnimation()
        {
            _animationDone = false;
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

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
                _animator.SetFloat(Speed, 0f);
            }

            void StartAnimation(bool back = false)
            {
                _animator.SetFloat(Speed, back ? -2f : 2f);
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

        #endregion
    }
}