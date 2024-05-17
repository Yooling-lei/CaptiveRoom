using System;
using System.Linq;
using IngameDebugConsole;
using Script.Controller.Common;
using Script.Controller.Interactable;
using Script.Entity;
using Script.Enums;
using Script.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
            UpdateBagVisible();

            if (Mouse.current.leftButton.isPressed)
            {
                // 获取鼠标位置
                var mousePos = Mouse.current.position.ReadValue();
                // 世界坐标转换为屏幕坐标
                // var screenPos = Camera.main.WorldToScreenPoint(mousePos);

                EventSystem eventSystem = EventSystem.current;
                PointerEventData eventData = new PointerEventData(eventSystem);
                eventData.position = mousePos;

                // 检测是否点击到UI
                if (eventSystem.IsPointerOverGameObject())
                {
                    Debug.Log("点击到UI");
                }
                else
                {
                    Debug.Log("点击到场景");
                }


                Debug.Log("screen Pos" + mousePos);
            }

            // 计算鼠标与玩家的距离
            // var distance = Vector2.Distance(mousePos, screenPos);
            // // 如果距离小于一定值,则显示交互UI
            // if (distance < 100)
            // {
            //     // 显示交互UI
            //     WorldSpaceCanvas.SetActive(true);
            // }
            // else
            // {
            //     // 隐藏交互UI
            //     WorldSpaceCanvas.SetActive(false);
            // }
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

        // 背包UI Canvas
        public GameObject bagCanvas;

        // 背包 格子UI
        public GameObject selectSlotImage;

        // 背包中选中的物体
        private ItemInPackage selectedItem;

        // 背包的存储矩阵
        private readonly BagMatrix<ItemInPackage> _bagMatrix = new(4, 3);

        // 局部变量 上一帧是否显示背包
        private bool _isPressTabPreFrame = false;
        private bool _isShowingBag = false;

        // TODO: 背包重构:
        // 1.获取正交相机的size
        // 2.通过size可以计算相机投影在背包实体panel上3d场景坐标的范围
        // (目前把Camera挂载在了锚点物体下,可以把Camera的位置看为锚点(x,z), y为深度,默认2就行 )
        // 3.根据可视的size,计算背包的行列数, (默认一个格子是多少Unity单位,默认格子的间距等)
        // 4.根据行列数,计算格子的位置
        // 5.根据size计算出来的范围大小,以及展示的Raw Image的宽高,可以计算格子在屏幕坐标上的范围
        // 6.鼠标点击时,

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

        private void UpdateBagSelectSlotImage(ItemInPackage itemInPackage)
        {
            if (itemInPackage == null)
            {
                selectSlotImage.SetActive(false);
                return;
            }

            if (itemInPackage == selectedItem) return;
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

        private float _bagCameraSize;
        private float _bagCameraHeight;
        private float _bagCameraWidth;

        // 背包场景摄像机控制器
        [DoNotSerialize] public BagRenderCamera bagRenderCameraController;

        // 背包 场景摄像机
        [DoNotSerialize] public Camera bagRenderCamera;

        public void RegisterBagRenderCamera(BagRenderCamera bagCameraController, Camera bagCamera)
        {
            bagRenderCameraController = bagCameraController;
            bagRenderCamera = bagCamera;
        }


        public void UseItemInPackage(string itemName, int row, int col)
        {
            var item = _bagMatrix.GetElement(row, col);
            if (item.ItemName != itemName)
            {
                Debug.LogWarning("物品名称不匹配");
            }

            // 1.GamePlay逻辑
            // 1.回复血量等
            // 2.触发事件
        }


        /// <summary>
        /// 背包中添加道具
        /// </summary>
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
            item.InitModelInBag(bagRenderCameraController.anchorPoint.transform, row, col, bagRenderCameraController.bagItemOffset);
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

        public static (ItemInPackage data, int x, int y) _FindElement(string itemName,
            BagMatrix<ItemInPackage> bagMatrix) =>
            bagMatrix.FindElement(x => x != null && x.ItemName == itemName);

        #endregion

        #endregion
    }
}