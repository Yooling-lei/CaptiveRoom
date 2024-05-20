using System;
using System.Collections.Generic;
using Script.Controller.Common;
using Script.Controller.Interactable;
using Script.Controller.UI;
using Script.Entity;
using Script.Extension;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Script.Manager
{
    public class BagManager : Singleton<BagManager>
    {
        // 背包UI Canvas
        public GameObject bagCanvas;

        // 背包UI 格子父物体
        public GameObject bagSlotParent;

        // 背包 格子预制件
        public GameObject selectSlotImage;

        // 背包的存储矩阵
        public readonly BagMatrix<ItemInPackage> GameBagMatrix = new(4, 3);

        // 背包场景摄像机控制器
        [HideInInspector] public BagRenderCamera bagRenderCameraController;

        // 背包 场景摄像机
        [HideInInspector] public Camera bagRenderCamera;

        // 背包格子按钮
        [HideInInspector] public List<BagSlotController> slotButtons = new();

        // 局部变量 上一帧是否显示背包
        private bool _isPressTabPreFrame = false;

        private bool _isShowingBag = false;

        // 背包中选中的物体
        private ItemInPackage _selectedItem;
        private BagSlotController _selectedSlot;

        // TODO: 拾取动画中不能打开背包
        private bool _couldOpenBag;

        // TODO: 控制展示背包UI
        private bool _isShowBag;

        #region 初始化

        private void Start()
        {
            RegisterSlotButtons();
        }

        public void RegisterBagRenderCamera(BagRenderCamera bagCameraController, Camera bagCamera)
        {
            bagRenderCameraController = bagCameraController;
            bagRenderCamera = bagCamera;
        }


        // 获取bagSlotParent下的所有Button, 并注册点击事件
        private void RegisterSlotButtons()
        {
            var buttons = bagSlotParent.GetComponentsInChildren<BagSlotController>();
            buttons.Each((x, i) => { RegisterSlotButton(x, i, GameBagMatrix.ColumnCount); });
        }


        private void RegisterSlotButton(BagSlotController item, int index, int matrixColumn)
        {
            var row = index / matrixColumn;
            var col = index % matrixColumn;
            UnityAction clickAction = () => { OnSlotClick(index, row, col); };
            item.slotButton.onClick.AddListener(clickAction);

            slotButtons.Add(item);
        }

        #endregion

        #region 抛出方法

        // TODO: 使用物品 => 格子物品count--
        /// <summary>
        /// 当可拾取物体触发拾取函数 
        /// </summary>
        public void OnItemPickup(string localName, PickupItemController controller) =>
            AddItemToPackage(localName, controller);

        /// <summary>
        /// 点击背包格子 
        /// </summary>
        private void OnSlotClick(int index, int row, int col)
        {
            var item = GameBagMatrix.GetElement(row, col);
            _selectedSlot = slotButtons[index];

            if (item == null)
            {
                _selectedItem = null;
            }
            else
            {
                var controller = slotButtons[index];
                controller.ChangeColor(true);
                _selectedItem = item;
            }

            // 更新UI
            RefreshSlotColor();
        }

        #endregion


        #region 2D UI

        private void UpdateBagVisible()
        {
            var isPressed = Keyboard.current.tabKey.isPressed;
            if (isPressed == _isPressTabPreFrame) return;

            if (isPressed)
            {
                // 按下Tab键位
                _isShowingBag = !_isShowingBag;
                bagCanvas.SetActive(_isShowingBag);
            }

            _isPressTabPreFrame = isPressed;
        }

        /// <summary>
        /// 根据选中的格子,刷新格子颜色
        /// </summary>
        private void RefreshSlotColor()
        {
            slotButtons.Each((x, _) => x.ChangeColor(_selectedSlot != null && x == _selectedSlot));
        }

        #endregion

        #region 3D场景

        /// <summary>
        /// 背包中添加道具
        /// </summary>
        public void AddItemToPackage(string itemName, PickupItemController itemController)
        {
            Debug.Log("Add Item To Package" + itemName);
            Debug.Log("Add Item To Package" + itemController);


            var (found, _, _) = _FindElement(itemName, GameBagMatrix);
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
            AddIntoBagMatrix(itemController, GameBagMatrix);

        public void AddIntoBagMatrix(PickupItemController itemController, BagMatrix<ItemInPackage> bagMatrix) =>
            AddIntoBagMatrix(itemController.itemName, itemController.gameObject, bagMatrix,
                itemController.scaleInBag);

        public void AddIntoBagMatrix(string itemName, GameObject linkGameObject, BagMatrix<ItemInPackage> bagMatrix,
            float scaleInBag = 1f)
        {
            var item = new ItemInPackage()
                { ItemName = itemName, Count = 1, LinkGameObject = linkGameObject, ScaleInBag = scaleInBag };
            var (row, col) = bagMatrix.PushElement(item);
            item.InitModelInBag(bagRenderCameraController.anchorPoint.transform, row, col,
                bagRenderCameraController.bagItemOffset);
        }


        /// <summary>
        /// 从背包中移除一个物体
        /// </summary>
        /// <param name="itemName"></param>
        public void RemoveItemFromPackage(string itemName) => RemoveItemFromPackage(itemName, GameBagMatrix);

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

        public static (ItemInPackage data, int x, int y) _FindElement(string itemName,
            BagMatrix<ItemInPackage> bagMatrix) =>
            bagMatrix.FindElement(x => x != null && x.ItemName == itemName);

        #endregion
    }
}