using System;
using Script.Controller.Common;
using Script.Controller.Interactable;
using Script.Entity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Script.Manager
{
    public class BagManager : Singleton<BagManager>
    {
        public Button firstButton;

        private void Start()
        {
            Debug.Log("注册了");
            // firstButton.onClick.AddListener(() => { GameManager.Instance.OnItemClick(0, 0); });
        }

        public void ClickLog()
        {
            Debug.Log("aaaaaaaaaaaaaaaaaa");
        }

        public void RegisterBagRenderCamera(BagRenderCamera bagCameraController, Camera bagCamera)
        {
            bagRenderCameraController = bagCameraController;
            bagRenderCamera = bagCamera;
        }

        #region 背包系统

        // TODO: 拾取动画中不能打开背包
        private bool _couldOpenBag;

        // TODO: 控制展示背包UI
        private bool _isShowBag;

        // 背包UI Canvas
        public GameObject bagCanvas;

        // 背包 格子UI
        public GameObject selectSlotImage;

        // 背包中选中的物体
        private ItemInPackage _selectedItem;

        // 背包的存储矩阵
        public readonly BagMatrix<ItemInPackage> GameBagMatrix = new(4, 3);

        // 局部变量 上一帧是否显示背包
        private bool _isPressTabPreFrame = false;
        private bool _isShowingBag = false;


        // TODO: 使用物品 => 格子物品count--

        public void OnItemPickup(string localName, PickupItemController controller) =>
            AddItemToPackage(localName, controller);

        public void OnItemClick(int row, int col)
        {
            Debug.Log("点击了!!!");
            var item = GameBagMatrix.GetElement(row, col);
            if (item == null) return;
            // _selectedItem = item;
            // UpdateBagSelectSlotImage(item);
        }


        private float _bagCameraSize,
            _bagCameraHeight,
            _bagCameraWidth;


        // 背包场景摄像机控制器
        [HideInInspector] public BagRenderCamera bagRenderCameraController;

        // 背包 场景摄像机
        [HideInInspector] public Camera bagRenderCamera;


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

        // FIXME: 改成切换选中UI
        private void UpdateBagSelectSlotImage(ItemInPackage itemInPackage)
        {
            if (itemInPackage == null)
            {
                selectSlotImage.SetActive(false);
                return;
            }

            if (itemInPackage == _selectedItem) return;
            var row = itemInPackage.BagRow;
            var col = itemInPackage.BagCol;
            UpdateBagSelectSlotImage(row, col);
        }

        public void UpdateBagSelectSlotImage(int row, int col)
        {
            const int offset = 180;
            const int initX = -350;
            const int initY = 185;
            selectSlotImage.SetActive(true);
            selectSlotImage.transform.localPosition = new Vector3(initX + col * offset, initY - row * offset, 0);
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
            AddIntoBagMatrix(itemController.itemName, itemController.gameObject, bagMatrix, itemController.scaleInBag);

        public void AddIntoBagMatrix(string itemName, GameObject linkGameObject, BagMatrix<ItemInPackage> bagMatrix,
            float scaleInBag = 1f)
        {
            var item = new ItemInPackage()
                { ItemName = itemName, Count = 1, LinkGameObject = linkGameObject, ScaleInBag = scaleInBag };
            Debug.Log("??111============" + bagMatrix);
            var (row, col) = bagMatrix.PushElement(item);
            Debug.Log("??2222===========" + item);
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

        #endregion
    }
}