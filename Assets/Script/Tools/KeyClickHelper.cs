using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Script.Tools
{
    public static class KeyClickHelper
    {
        /// <summary>
        /// 测试用: 便于按键点击测试
        /// </summary>
        public static IEnumerator OnClick(KeyControl key, Action doSomething)
        {
            var isPressPreFrame = false;
            while (true)
            {
                var isPressed = key.isPressed;
                if (isPressed == isPressPreFrame)
                {
                    yield return null;
                }
                else
                {
                    if (isPressed)
                    {
                        doSomething.Invoke();
                    }

                    isPressPreFrame = isPressed;
                    yield return null;
                }
            }
        }
    }
}