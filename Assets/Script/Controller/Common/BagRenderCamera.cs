using System;
using Script.Manager;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.Controller.Common
{
    [RequireComponent(typeof(Camera))]
    public class BagRenderCamera : MonoBehaviour
    {
        public Camera bagRenderCamera;


        // 一个像素等于多少单位长度
        public float unitPrePixel;
        public float bagItemOffset;
        public float bagSlotImageHeight = 145;

        // 正交size
        private float _bagCameraSize;

        // rawImage的宽高比
        private float _rawImageRatio;

        // 需要初始化:
        // 背包 场景锚点
        public GameObject anchorPoint;
        public GameObject bagRawImage;

        private void Awake()
        {
            Debug.Log("Camera Awake");
        }

        private void OnEnable()
        {
            Debug.Log("Camera Enable");
            InitBagSceneVariables();
        }

        private void Start()
        {
            BagManager.Instance.RegisterBagRenderCamera(this, bagRenderCamera);
        }

        private void InitBagSceneVariables()  
        {
            bagRenderCamera = GetComponent<Camera>();
            // 用代码获取正交相机的size
            _bagCameraSize = bagRenderCamera.orthographicSize;
            var aspectRatio = bagRenderCamera.aspect;

            // 代表在正交相机内, height个单位长度就可以填满整个屏幕
            var height = _bagCameraSize * 2;
            var rectTransform = bagRawImage.GetComponent<RectTransform>();
            var rect = rectTransform.rect;
            var rawImageHeight = rect.height;

            // 10 unit = RawImageHeight(px) = 600
            // 则每一个像素 = 10 / 600 个unit
            unitPrePixel = height / rawImageHeight;
            bagItemOffset = bagSlotImageHeight * unitPrePixel;

            Debug.Log("每个背包物体的偏移量: " + bagItemOffset);
            // 初始物体的坐标是 (size-1,size-1)
            // 向右,Z轴-2.083, 向下,X轴-2.083
        }
    }
}