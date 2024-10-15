using System;
using System.Collections.Generic;
using System.Linq;
using Script.Controller.Common;
using Script.Controller.Interactable;
using Script.Controller.UI;
using Script.Entity;
using Script.Extension;
using Script.Interface;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Manager
{
    public enum EBagBehavior
    {
        Normal,
        Select
    }

    public enum EBagSelectMode
    {
        Single,
        Multiple
    }

    /**
     * 设计逻辑:
     * BagManager负责背包矩阵存储的管理 (以及格子UI)  ,抛出 "拾取物体"  "删除物体" 的接口
     * 背包存储数据结构为BagMatrix, 用于在数据结构上管理
     * BagMatrix内的实体为ItemInPackage, 管理它代表的物体的所有UI和使用逻辑
     */
    public class BagManager : Singleton<BagManager>
    {
        public CanvasGroup bagCanvasGroup;

        // 背包UI 格子父物体
        public GameObject bagSlotParent;

        // 背包 格子预制件
        public GameObject selectSlotImage;

        // 背包 注册的背包内物品
        private readonly Dictionary<string, GameObject> _registeredPrefab = new();

        [Serializable]
        public struct KeyValuePair
        {
            public string key;
            public GameObject value;
        }

        // TODO: 有点麻烦 后面看是否用脚本控制模型 
        public KeyValuePair[] PrefabPairArray;

        // 背包的存储矩阵
        public readonly Matrix<ItemInPackage> BagMatrix = new(4, 3);

        // 背包场景摄像机控制器
        [HideInInspector] public BagRenderCamera bagRenderCameraController;

        // 背包 场景摄像机
        [HideInInspector] public Camera bagRenderCamera;

        // 背包格子按钮
        [HideInInspector] public List<BagSlotController> slotButtons = new();

        private bool _isShowingBag = false;

        // 单选时: 背包中选中的物体
        private ItemInPackage _selectedItem;

        // 单选时: 背包中选中的格子
        private BagSlotController _selectedSlot;

        // 多选时: 选中的物体
        private List<ItemInPackage> _selectedItems = new();

        // 多选时: 选中的格子
        private List<BagSlotController> _selectedSlots = new();


        // TODO: 拾取动画中不能打开背包 (不应该在这个类做这个事情,先记着)
        private bool _couldOpenBag;

        private EBagBehavior _bagBehavior = EBagBehavior.Normal;

        private Action<ItemInPackage> _onSelectItem;

        private Dictionary<string, bool> _bagItemArchive = new();

        // 背包选中模式: 单选/多选
        private EBagSelectMode _bagSelectMode = EBagSelectMode.Single;
        private bool isSingleSelect => _bagSelectMode == EBagSelectMode.Single;


        private void Start()
        {
            RegisterPrefabInBag();
            RegisterSlotButtons();
            ToggleBagVisible(false);
        }

        private void RegisterPrefabInBag()
        {
            foreach (var kvp in PrefabPairArray)
            {
                if (!_registeredPrefab.ContainsKey(kvp.key))
                {
                    _registeredPrefab.Add(kvp.key, kvp.value);
                }
            }
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
            buttons.Each((x, i) => { RegisterSlotButton(x, i, BagMatrix.ColumnCount); });
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

        public void SetBagSelectMode(EBagSelectMode mode) => _bagSelectMode = mode;

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
            Debug.Log("捕获点击事件");

            var item = BagMatrix.GetElement(row, col);
            if (isSingleSelect)
            {
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
            }
            else
            {
                if (item == null) return;

                if (_selectedItems.Contains(item))
                {
                    _selectedItems.Remove(item);
                    _selectedSlots.Remove(slotButtons[index]);
                }
                else
                {
                    _selectedItems.Add(item);
                    _selectedSlots.Add(slotButtons[index]);
                }
            }


            // 更新UI
            RefreshSlotColor();
        }

        /// <summary>
        /// 注册选中物品行为
        /// </summary>
        /// <param name="onSelectItem"></param>
        public void RegisterSelectItemAction(Action<ItemInPackage> onSelectItem) => _onSelectItem = onSelectItem;

        /// <summary>
        /// 背包内交互键触发
        /// </summary>
        public void OnInteractTrigger()
        {
            Debug.Log("Use Item In Bag");
            Debug.Log(_bagBehavior);
            if (isSingleSelect) OnSingleInteractTrigger();
            else OnMultipleInteractTrigger();
        }

        private void OnSingleInteractTrigger()
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

        private void OnMultipleInteractTrigger()
        {
            if (_selectedItems.Count == 0) return;
            Debug.Log("Merge Items");

            var mergeableItem = _selectedItems
                .Select(item => item.ModelInBag?.GetComponent<IMergeableItem>())
                .FirstOrDefault(mergeable => mergeable is not null);
            if (mergeableItem is null) return;

            var couldMerge = mergeableItem.MergeCheck(_selectedItems);
            Debug.Log("Could Merge: " + couldMerge);

            if (couldMerge) mergeableItem.OnMergeSuccess();
            else mergeableItem.OnMergeCheckFailed();
        }

        private void RemoveSelectedItem()
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
            var (item, _, _) = _FindElement(itemName, BagMatrix);
            return UseItemInBag(item);
        }

        private int UseItemInBag(ItemInPackage item)
        {
            var count = -1;
            item?.UseItem(ref count);
            return count;
        }

        /// <summary>
        /// 融合选中的物品
        /// </summary>
        public void MergeSelectItems()
        {
            // 1.判断能不能融合
            // 2.若可以,删除选中的物品,添加新的物品到背包
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
            switch (_bagSelectMode)
            {
                default:
                case EBagSelectMode.Single:
                    slotButtons.Each((x, _) => x.ChangeColor(_selectedSlot != null && x == _selectedSlot));
                    break;
                case EBagSelectMode.Multiple:
                    slotButtons.Each((x, _) => x.ChangeColor(_selectedSlots.Exists(y => x == y)));
                    break;
            }
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

            _bagItemArchive[itemName] = true;
            var (found, _, _) = _FindElement(itemName, BagMatrix);
            if (found != null)
            {
                found.CountPlus();
            }
            else
            {
                AddIntoBagMatrix(itemController);
            }
        }

        public bool HasItemPicked(string itemName) => _bagItemArchive.ContainsKey(itemName);


        private void AddIntoBagMatrix(PickupItemController itemController) =>
            AddIntoBagMatrix(itemController, BagMatrix);


        private void AddIntoBagMatrix(PickupItemController itemController, Matrix<ItemInPackage> matrix) =>
            AddIntoBagMatrix(itemController.itemName, matrix, itemController.scaleInBag);


        public void AddIntoBagMatrix(string itemName, Matrix<ItemInPackage> matrix,
            float scaleInBag = 1f)
        {
            var item = new ItemInPackage(itemName, 1, scaleInBag);
            var (row, col) = matrix.PushElement(item);
            var anchor = bagRenderCameraController.anchorPoint.transform;
            var offset = bagRenderCameraController.bagItemOffset;

            item.InitModelInBag(anchor, getPrefabInBag(itemName), row, col, offset);
        }


        /// <summary>
        /// 从背包中移除一个物体
        /// </summary>
        /// <param name="itemName"></param>
        public void RemoveItemFromPackage(string itemName) => RemoveItemFromPackage(itemName, BagMatrix);

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

        public GameObject getPrefabInBag(string itemName) => _registeredPrefab[itemName];

        #endregion
    }
}