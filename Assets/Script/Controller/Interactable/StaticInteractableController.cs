using System;
using HighlightPlus;
using UnityEngine;

namespace Script.Controller.Common
{
    /// <summary>
    /// 可交互物体控制器
    /// </summary>
    public class StaticInteractableController : BaseInteractableController
    {
        public override void InvokeInteractable()
        {
            base.InvokeInteractable();
            Debug.Log("StaticInteractableController InvokeInteract");
        }
    }
}