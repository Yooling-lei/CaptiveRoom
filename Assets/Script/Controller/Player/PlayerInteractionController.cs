using System;
using Script.Controller.Common;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Player
{
    /// <summary>
    /// 控制玩家的交互行为
    /// </summary>
    public class PlayerInteractionController : MonoBehaviour
    {
        public float interactionRange = 1.5f;

        private GameObject _mainCamera;
        private BaseInteractableController _interactableObject;
        private bool _hasInteractableObject;

        private PlayerInputReceiver _input;

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _input = GetComponent<PlayerInputReceiver>();
            GameManager.Instance.RegisterPlayer(this.gameObject);
        }

        private void Update()
        {
            // 玩家朝向且在范围内的可交互物体: 高亮、提示
            InteractionEvaluation();
            // 是否按下交互键
            if (_input.interact)
            {
                // TODO: 判断是否调用 (如果在菜单页面或者暂停等,则不调用)
                if (_hasInteractableObject) _interactableObject.OnInteract();
                _input.interact = false;
            }
        }


        /// <summary>
        /// 判断朝向的物体是否可交互
        /// </summary>
        private void InteractionEvaluation()
        {
            if (!Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out var hit,
                    3f)) return;
            if (hit.collider.CompareTag("Interactable"))
            {
                var interactableController = hit.collider.GetComponent<BaseInteractableController>();
                if (interactableController is null)
                {
                    CancelCurrentInteract();
                    return;
                }

                // 如果之前有交互物体
                if (_hasInteractableObject)
                {
                    // 交互物体没变 不用处理
                    if (interactableController.id == _interactableObject.id) return;
                    // 交互物体变了 取消之前的高亮和提示
                    CancelCurrentInteract();
                }

                _hasInteractableObject = true;
                _interactableObject = interactableController;
                interactableController.InvokeInteractable();
            }
            else
            {
                CancelCurrentInteract();
            }
        }

        private void CancelCurrentInteract()
        {
            if (!_hasInteractableObject) return;
            _interactableObject.CancelInteractable();
            _hasInteractableObject = false;
        }
    }
}