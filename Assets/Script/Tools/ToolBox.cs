using UnityEngine;

namespace Script.Tools
{
    public static class ToolBox
    {
        public static void DeductTimer(ref float timer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) timer = 0f;
        }
    }
}