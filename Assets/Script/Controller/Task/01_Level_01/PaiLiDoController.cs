using System;
using System.Collections;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Task._01_Level_01
{
    public class PaiLiDoController : MonoBehaviour
    {
        Camera mainCamera;
        PlayerInputReceiver _input;


        private bool _picked;

        public void PickUpPaiLiDo()
        {
            transform.SetParent(Camera.main.transform);
            transform.localPosition = new Vector3(0.3f, -0.2f, 0.7f);
            _input = GameManager.Instance.playerInputReceiver;
            _picked = true;
            // TODO: 此时应该开始响应鼠标右键输入事件, 使得给物体一个拉进的效果
        }


        private void OnEnable()
        {
            mainCamera = Camera.main;
            StartCoroutine(TestGet());
        }


        private float _focusDuration = 0f;

        private void Update()
        {
            return;
            if (!_picked) return;

            // TODO: 平常状态是0, 举起相机状态是100,通过按住右键的行为来过渡
            if (_input.viewFocus)
            {
                var count = 100 * Time.deltaTime;
                _focusDuration += count;
                if (_focusDuration > 100) _focusDuration = 100;
            }
            else
            {
                var count = 100 * Time.deltaTime;
                _focusDuration -= count;
                if (_focusDuration < 0) _focusDuration = 0;
            }
            
            // TODO: 根据_focusDuration来调整物体的位置

        }

        private IEnumerator TestGet()
        {
            yield return new WaitForSeconds(3);
            PickUpPaiLiDo();
        }
    }
}