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

            var (found, _, _) = _bagMatrix.FindElement(x => x != null && x.ItemName == itemName);
            if (found != null)
            {
                found.Count++;
                found.RefreshCountText();
            }
            else
            {
                AddIntoBag(itemController, _bagMatrix);
            }
        }

        public void AddIntoBag(string itemName, GameObject linkGameObject, BagMatrix<ItemInPackage> bagMatrix,
            float scaleInBag = 1f)
        {
            var item = new ItemInPackage() { ItemName = itemName, Count = 1, LinkGameObject = linkGameObject };
            var (row, col) = bagMatrix.PushElement(item);

            // TODO: 物体的位置需要单独出来
            // 创建空物体
            var instance = new GameObject();
            instance.transform.parent = anchorPoint.transform;

            // 计算物体位置
            var itemPos = CalculateItemInBagScenePosition(row, col);
            instance.transform.localPosition = itemPos;
            instance.transform.localScale = new Vector3(scaleInBag, scaleInBag, scaleInBag);

            // 通过捡拾的物体,赋值空物体的mesh和material
            var meshFilter = instance.AddComponent<MeshFilter>();
            meshFilter.mesh = linkGameObject.GetComponent<MeshFilter>().mesh;

            var meshRenderer = instance.AddComponent<MeshRenderer>();
            meshRenderer.material = linkGameObject.GetComponent<MeshRenderer>().material;

            // 挂载旋转展示脚本
            instance.AddComponent<SelfRotation>();
            item.ModelInBag = instance;
        }

        public void AddIntoBag(PickupItemController itemController, BagMatrix<ItemInPackage> bagMatrix)
        {
            AddIntoBag(itemController.itemName, itemController.gameObject, bagMatrix, itemController.scaleInBag);
        }

        /// <summary>
        /// 计算在背包的物体的位置(localPosition)
        /// </summary>
        /// <param name="row">行数</param>
        /// <param name="col">列数</param>
        /// <returns></returns>
        private static Vector3 CalculateItemInBagScenePosition(int row, int col)
        {
            return new Vector3(row * -2, 0, col * -2);
        }


        public void RemoveItemFromPackage(ItemInPackage item)
        {
            // _itemsInPackage.Remove(item);
            throw new NotImplementedException();
        }

        #endregion
    }
}