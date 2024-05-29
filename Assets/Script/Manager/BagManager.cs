using System;
using System.Collections.Generic;
using Script.Controller.Common;
using Script.Controller.Interactable;
using Script.Controller.UI;
using Script.Entity;
using Script.Extension;
using Script.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Script.Manager
{
    public enum EBagBehavior
    {
        Normal,
        Select
    }

    /**
     * 设计逻辑:
     * BagManager负责背包矩阵存储的管理 (以及格子UI)  ,抛出 "拾取物体"  "删除物体" 的接口
     * 背包存储数据结构为BagMatrix, 用于在数据结构上管理
     * BagMatrix内的实体为ItemInPackage, 管理它代表的物体的所有UI和使用逻辑
     */
    public class BagManager : Singleton<BagManager>
    {
        // 背包UI Canvas
        // public GameObject bagCanvas;

        public CanvasGroup bagCanvasGroup;

        // 背包UI 格子父物体
        public GameObject bagSlotParent;

        // 背包 格子预制件
        public GameObject selectSlotImage;

        // 背包的存储矩阵
        public readonly Matrix<ItemInPackage> GameMatrix = new(4, 3);

        // 背包场景摄像机控制器
        [HideInInspector] public BagRenderCamera bagRenderCameraController;

        // 背包 场景摄像机
        [HideInInspector] public Camera bagRenderCamera;

        // 背包格子按钮
        [HideInInspector] public List<BagSlotController> slotButtons = new();

        private bool _isShowingBag = false;

        // 背包中选中的物体
        private ItemInPackage _selectedItem;

        private BagSlotController _selectedSlot;

        // TODO: 拾取动画中不能打开背包 (不应该在这个类做这个事情,先记着)
        private bool _couldOpenBag;

        private EBagBehavior _bagBehavior = EBagBehavior.Normal;

        private Action<ItemInPackage> _onSelectItem;
        public void RegisterSelectItemAction(Action<ItemInPackage> onSelectItem) => _onSelectItem = onSelectItem;


        private void Start()
        {
            // Debug.Log("设置背包隐藏");
            RegisterSlotButtons();
            ToggleBagVisible(false);
        }


        #region 初始化

        public void RegisterBagRenderCamera(BagRenderCamera bagCameraController, Camera bagCamera)
        {
            bagRenderCameraController = bagCameraController;
            bagRenderCamera = bagCamera;
        }


        // 获取bagSlotParent下的所有Button, 并注册点击事件
        private void RegisterSlotButtons()
        {
            var buttons = bagSlotParent.GetComponentsInChildren<BagSlotController>();
            buttons.Each((x, i) => { RegisterSlotButton(x, i, GameMatrix.ColumnCount); });
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
            var item = GameMatrix.GetElement(row, col);
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


        /// <summary>
        /// 背包内交互键触发
        /// </summary>
        public void OnInteractTrigger()
        {
            if (_selectedItem == null) return;

            // 普通模式下,使用物品
            if (_bagBehavior == EBagBehavior.Normal)
            {
                if (!_selectedItem.IsUsable) return;
                var count = UseItemInBag(_selectedItem);
                if (count >= 1) return;
                RemoveSelectedItem();
            }
            else if (_bagBehavior == EBagBehavior.Select)
            {
                _onSelectItem?.Invoke(_selectedItem);
            }
        }

        public void RemoveSelectedItem()
        {
            if (_selectedItem == null) return;
            RemoveItemFromPackage(_selectedItem.ItemName);
            _selectedItem = null;
            _selectedSlot = null;
            RefreshSlotColor();
        }


        /// <summary>
        /// 使用物品
        /// </summary>
        /// <returns></returns>
        public int UseItemInBag(string itemName)
        {
            var (item, _, _) = _FindElement(itemName, GameMatrix);
            return UseItemInBag(item);
        }

        private int UseItemInBag(ItemInPackage item)
        {
            var count = -1;
            item?.UseItem(ref count);
            return count;
        }

        #endregion


        #region 2D UI

        public bool IsShowingBag() => _isShowingBag;

        // 切换背包显示状态
        public void ToggleBagVisible()
        {
            ToggleBagVisible(!_isShowingBag);
        }


        // 打开背包,并设置选中物体行为
        public void ToggleBagVisible(Action<ItemInPackage> onSelectItem)
        {
            RegisterSelectItemAction(onSelectItem);
            ToggleBagVisible(true, EBagBehavior.Select);
        }

        // 设置背包显示状态
        public void ToggleBagVisible(bool visible, EBagBehavior behavior = EBagBehavior.Normal)
        {
            _bagBehavior = behavior;
            _isShowingBag = visible;

            // 设置bagCanvas的透明度
            bagCanvasGroup.alpha = _isShowingBag ? 1 : 0;
            // 设置游戏流速
            Time.timeScale = _isShowingBag ? 0.1f : 1f;
            // 设置鼠标锁定
            if (_isShowingBag) GameManager.Instance.UnlockCursor();
            else GameManager.Instance.LockCursor();
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

            var (found, _, _) = _FindElement(itemName, GameMatrix);
            if (found != null)
            {
                found.CountPlus();
            }
            else
            {
                AddIntoBagMatrix(itemController);
            }
        }

        private void AddIntoBagMatrix(PickupItemController itemController) =>
            AddIntoBagMatrix(itemController, GameMatrix);


        private void AddIntoBagMatrix(PickupItemController itemController, Matrix<ItemInPackage> matrix) =>
            AddIntoBagMatrix(itemController.itemName, itemController.gameObject, matrix,
                itemController.scaleInBag);


        public void AddIntoBagMatrix(string itemName, GameObject linkGameObject, Matrix<ItemInPackage> matrix,
            float scaleInBag = 1f)
        {
            var item = new ItemInPackage(itemName, 1, scaleInBag, linkGameObject);
            var (row, col) = matrix.PushElement(item);
            item.InitModelInBag(bagRenderCameraController.anchorPoint.transform, row, col,
                bagRenderCameraController.bagItemOffset);
        }


        /// <summary>
        /// 从背包中移除一个物体
        /// </summary>
        /// <param name="itemName"></param>
        public void RemoveItemFromPackage(string itemName) => RemoveItemFromPackage(itemName, GameMatrix);

        private static void RemoveItemFromPackage(string itemName, Matrix<ItemInPackage> matrix)
        {
            var (found, row, col) = _FindElement(itemName, matrix);
            if (found == null) return;

            found.DestroyModelInBag();
            matrix.RemoveElement(row, col);
            matrix.TraverseElement((item, x, y) =>
            {
                // 更新视图层
                if (item == null) return true;
                item.UpdatePosition(x, y);
                return false;
            });
        }

        public static (ItemInPackage data, int x, int y) _FindElement(string itemName,
            Matrix<ItemInPackage> matrix) =>
            matrix.FindElement(x => x != null && x.ItemName == itemName);

        #endregion
    }
}