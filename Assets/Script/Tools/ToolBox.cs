using System.Collections;
using UnityEngine;

namespace Script.Tools
{
    /// <summary>
    /// 管理一些static工具函数
    /// </summary>
    public static class ToolBox
    {
        public static void DeductTimer(ref float timer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) timer = 0f;
        }
        
        public static IEnumerator SmoothMoveToPosition(Transform transform, Vector3 targetPosition, float duration = 1f)
        {
            var elapsedTime = 0.0f;
            var startingPos = transform.position;
            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
        }
    }
}