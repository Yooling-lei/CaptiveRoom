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
        Camera mainCamera;
        PlayerInputReceiver _input;
        private Animator animator;
        public Image filmingMarkImage;
        private bool init;

        //物体的meshRender
        private MeshRenderer _meshRenderer;
        private bool _picked;

        private float _takePhotoCD;

        public void PickUpPaiLiDo()
        {
            transform.SetParent(Camera.main.transform);
            transform.localPosition = new Vector3(0.3f, -0.2f, 0.7f);
            _input = GameManager.Instance.playerInputReceiver;
            _picked = true;
            // TODO: 此时应该开始响应鼠标右键输入事件, 使得给物体一个拉进的效果
        }

        void InitFunc()
        {
            mainCamera = Camera.main;
            _input = GameManager.Instance?.playerInputReceiver;
            _meshRenderer = GetComponent<MeshRenderer>();
            animator = GetComponent<Animator>();

            if (_input != null && mainCamera != null) init = true;
        }


        private void OnEnable()
        {
            InitFunc();
            // mainCamera = Camera.main;
            // StartCoroutine(TestGet());
        }


        private float _focusDuration = 0f;

        private void Update()
        {
            if (!init)
            {
                InitFunc();
                return;
            }

            ControlAnimation();
            ControlTakePhoto();
            ToolBox.DeductTimer(ref _takePhotoCD);

            // TODO: 根据_focusDuration来调整物体的位置
        }

        private bool _animationDone;

        private void ControlAnimation()
        {
            _animationDone = false;
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (_input.viewFocus)
            {
                // TODO: 1.Canvas上画一个摄像中的UI
                // TODO: 2.当拍立得动画播放完毕, 展示这个UI
                // TODO: 3.当松开右键,要播放收起动画时, 隐藏这个UI

                if (stateInfo.normalizedTime >= 1.0f)
                {
                    // 播完了
                    animator.SetFloat("Speed", 0f);
                    _animationDone = true;
                    // 播完了, 显示摄像中的UI
                    // filmingMarkImage.enabled = true;
                }
                else
                {
                    animator.SetFloat("Speed", 1f);
                    // filmingMarkImage.enabled = false;
                }
            }
            else
            {
                if (stateInfo.normalizedTime <= 0.01f)
                {
                    // 播完了
                    animator.SetFloat("Speed", 0f);
                }
                else
                {
                    animator.SetFloat("Speed", -1f);
                }
            }

            filmingMarkImage.enabled = _animationDone;
            _meshRenderer.enabled = !_animationDone;
        }

        private void ControlTakePhoto()
        {
            if (!_input.fire) return;
            _input.fire = false;
            
            if (!_animationDone) return;
            if(_takePhotoCD > 0)
            {
                Debug.Log("CD中");
                return;
            }
            // TODO: 后面还需要防抖等
            Debug.Log("拍照");
            // 拍照后应该有三秒钟CD
            _takePhotoCD = 3f;
        }


        private IEnumerator TestGet()
        {
            yield return new WaitForSeconds(1);
            PickUpPaiLiDo();
        }
    }
}