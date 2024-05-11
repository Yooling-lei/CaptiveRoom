using System;
using System.Linq;
using IngameDebugConsole;
using Script.Controller.Interactable;
using Script.Entity;
using Script.Enums;
using Script.Tools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        // 保存游戏实例:   Player, 
        // 不需要在编辑器初始化
        [HideInInspector] public GameObject player;

        // 控制游戏运行状态, 
        public EGameStatus gameStatus;

        public GameObject WorldSpaceCanvas;

        private void Start()
        {
            WorldSpaceCanvas = GameObject.Find("WorldSpaceCanvas");
        }

        private void Update()
        {
            // FIXME: UI总体控制
            var isPressed = Keyboard.current.tabKey.isPressed;
            if (isPressed == _isShowBagPreFrame) return;

            bagCanvas.SetActive(isPressed);
            _isShowBagPreFrame = isPressed;
        }


        public void RegisterPlayer(GameObject obj)
        {
            player = obj;
        }

        public void SwitchGameStatus(EGameStatus status)
        {
            gameStatus = status;
            // TODO: 暂停时PlayerInput应该被禁用
        }

        #region 背包系统

        // TODO: 拾取动画中不能打开背包
        private bool _couldOpenBag;

        // TODO: 控制展示背包UI
        private bool _isShowBag;
        public GameObject anchorPoint;
        public GameObject bagCanvas;
        private readonly BagMatrix<ItemInPackage> _bagMatrix = new(4, 3);
        private bool _isShowBagPreFrame = false;


        public void AddItemToPackage(string itemName, PickupItemController itemController)
        {
            Debug.Log("Add Item To Package" + itemName);
            Debug.Log("Add Item To Package" + itemController);


            var (found, _, _) = _FindElement(itemName, _bagMatrix);
            if (found != null)
            {
                found.Count++;
                found.RefreshCountText();
            }
            else
            {
                AddIntoBagMatrix(itemController);
            }
        }

        public void AddIntoBagMatrix(PickupItemController itemController) =>
            AddIntoBagMatrix(itemController, _bagMatrix);

        public void AddIntoBagMatrix(PickupItemController itemController, BagMatrix<ItemInPackage> bagMatrix) =>
            AddIntoBagMatrix(itemController.itemName, itemController.gameObject, bagMatrix, itemController.scaleInBag);
        
        public void AddIntoBagMatrix(string itemName, GameObject linkGameObject, BagMatrix<ItemInPackage> bagMatrix,
            float scaleInBag = 1f)
        {
            var item = new ItemInPackage()
                { ItemName = itemName, Count = 1, LinkGameObject = linkGameObject, ScaleInBag = scaleInBag };
            var (row, col) = bagMatrix.PushElement(item);
            item.InitModelInBag(anchorPoint.transform, row, col);
        }
        
        /// <summary>
        /// 从背包中移除一个物体
        /// </summary>
        /// <param name="itemName"></param>
        public void RemoveItemFromPackage(string itemName) => RemoveItemFromPackage(itemName, _bagMatrix);

        public void RemoveItemFromPackage(string itemName, BagMatrix<ItemInPackage> bagMatrix)
        {
            var (found, row, col) = _FindElement(itemName, bagMatrix);
            if (found == null) return;

            // 数量 --
            if (found.Count > 1)
            {
                found.Count--;
                found.RefreshCountText();
                return;
            }

            // 数量为1时,移除元素
            bagMatrix.RemoveElement(row, col);
            bagMatrix.TraverseElement((item, x, y) =>
            {
                // 更新视图层
                if (item == null) return true;
                item.UpdatePositionOfModelInBag(x, y);
                return false;
            });
        }

        private static (ItemInPackage data, int x, int y) _FindElement(string itemName,
            BagMatrix<ItemInPackage> bagMatrix) =>
            bagMatrix.FindElement(x => x != null && x.ItemName == itemName);

        #endregion
    }
}