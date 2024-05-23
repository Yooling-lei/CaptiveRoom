using System;
using HighlightPlus;
using UnityEngine;

namespace Script.Controller.Interactable
{
    /// <summary>
    /// 可交互物体控制器
    /// </summary>
    public class BaseInteractableController : MonoBehaviour
    {
        public Guid id;
        HighlightEffect highlightEffect;
        bool hasHighlightEffect;
        public Action OnInteraction;


        protected virtual void Awake()
        {
            id = Guid.NewGuid();
        }


        private void OnEnable()
        {
            if (highlightEffect == null)
            {
                highlightEffect = GetComponent<HighlightEffect>();
                hasHighlightEffect = highlightEffect != null;
            }
        }

        /*
         * 游戏中 高亮的物体有三种:
         * 1.可拾取的物体
         * 2.有固定交互行为的物体(比如抽屉)
         * 3.可以触发应用背包道具的物体
         *
         * 相同:
         * 1. 高亮
         * 2. 触发交互时各自的行为
         */


        public virtual void InvokeInteractable()
        {
            InvokeInteractTip();
            // OnInteraction?.Invoke();
        }

        public virtual void CancelInteractable()
        {
            CancelInteractTip();
        }

        public virtual void OnInteract()
        {
            Debug.Log("OnInteract");
        }


        private void InvokeInteractTip()
        {
            Debug.Log("invokeInteract tip");
            if (hasHighlightEffect)
            {
                Debug.Log("hasHighlightEffect");
                highlightEffect.SetHighlighted(true);
            }
        }

        private void CancelInteractTip()
        {
            if (hasHighlightEffect)
            {
                highlightEffect.SetHighlighted(false);
            }
        }
    }
}