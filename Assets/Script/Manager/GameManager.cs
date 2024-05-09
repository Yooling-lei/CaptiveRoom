using System;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using Script.Controller.Interactable;
using Script.Entity;
using Script.Enums;
using Script.Tools;
using UnityEngine;

namespace Script.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        // 保存游戏实例:   Player, 
        // 不需要在编辑器初始化
        [HideInInspector] public GameObject player;

        // 控制游戏运行状态, 
        public EGameStatus gameStatus;

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
        private readonly BagMatrix<ItemInPackage> _bagMatrix = new(4, 3);

        public void AddItemToPackage(string itemName, PickupItemController itemController)
        {
            Debug.Log("Add Item To Package" + itemName);
            Debug.Log("Add Item To Package" + itemController);

            var found = _bagMatrix.FindElement(x => x != null && x.ItemName == itemName);
            if (found != null)
            {
                found.Count++;
                // TODO: 更新角标UI
            }
            else
            {
                AddIntoBag(itemController, _bagMatrix);
            }
        }

        private void AddIntoBag(string itemName, GameObject linkGameObject, BagMatrix<ItemInPackage> bagMatrix,
            float scaleInBag = 1f)
        {
            var item = new ItemInPackage() { ItemName = itemName, Count = 1, LinkGameObject = linkGameObject };
            var (row, col) = bagMatrix.PushElement(item);

            // 创建空物体
            var instance = new GameObject();
            instance.transform.parent = anchorPoint.transform;

            // 计算物体位置
            var itemPos = new Vector3(row * -2, 0, col * -2);
            instance.transform.localPosition = itemPos;
            instance.transform.localScale = new Vector3(scaleInBag, scaleInBag, scaleInBag);
            
            // 通过捡拾的物体,赋值空物体的mesh和material
            var meshFilter = instance.AddComponent<MeshFilter>();
            meshFilter.mesh = linkGameObject.GetComponent<MeshFilter>().mesh;
            
            var meshRenderer = instance.AddComponent<MeshRenderer>();
            meshRenderer.material = linkGameObject.GetComponent<MeshRenderer>().material;
            
            // 挂载旋转脚本
            instance.AddComponent<SelfRotation>();
        }

        private void AddIntoBag(PickupItemController itemController, BagMatrix<ItemInPackage> bagMatrix)
        {
            AddIntoBag(itemController.itemName, itemController.gameObject, bagMatrix, itemController.scaleInBag);
        }

        private void Start()
        {
            DebugLogConsole.AddCommand("AddItem", "Add Item In To Bag", TestAddToBag);
        }

        public GameObject testInBagItem;

        /// <summary>
        /// Console 测试代码
        /// </summary>
        private void TestAddToBag()
        {
            // 随机生成一个string
            var random = new System.Random();
            var str = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            AddIntoBag(str, testInBagItem, _bagMatrix);
        }


        public void RemoveItemFromPackage(ItemInPackage item)
        {
            // _itemsInPackage.Remove(item);
            throw new NotImplementedException();
        }

        #endregion
    }
}