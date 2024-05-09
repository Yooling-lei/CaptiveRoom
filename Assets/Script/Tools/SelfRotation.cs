using UnityEngine;

namespace Script.Tools
{
    public class SelfRotation : MonoBehaviour
    {
        // 旋转轴方向,速率
        public Vector3 rotationSpeed = new Vector3(30, 30, 30);
        public bool isRotating = true;

        void Update()
        {
            if (!isRotating) return;
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}