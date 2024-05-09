using System;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using Script.Entity;
using Script.Enums;
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
        private bool _isShowBag;
        private readonly List<ItemInPackage> _itemsInPackage = new();

        public void AddItemToPackage(string itemName, GameObject itemObj)
        {
            var found = _itemsInPackage.FirstOrDefault(x => x.ItemName == itemName);
            if (found != null)
            {
                found.Count++;
            }
            else
            {
                var item = new ItemInPackage() { ItemName = itemName, Count = 1, LinkGameObject = itemObj };
                _itemsInPackage.Add(item);
            }

            // LOG
            Debug.Log("目前的背包物品有: ");
            foreach (var item in _itemsInPackage)
            {
                Debug.Log(item.ItemName + " " + item.Count);
            }
        }


        // TODO: 添加到背包场景
        // 添加时需要考虑背包的位置,行数排列等
        // 应该通过存储的list动态算背包格子及排序
        public GameObject testInBagItem;

        public GameObject anchorPoint;
        private BagMatrix _bagMatrix = new BagMatrix(4, 4);

        private void Start()
        {
            DebugLogConsole.AddCommand("testAdd", "testAdd", TestAddToBag);
        }

        public void TestAddToBag()
        {
            // TODO: 改成真实数据
            var testObj = new CustomClass();
            var (row, col) = _bagMatrix.PushElement(testObj);

            // 根据矩阵坐标计算位置
            // 向右 z轴-2 ,向下 x轴-2
            var itemPos = new Vector3(row * -2, 0, col * -2);
            var item = Instantiate(testInBagItem, anchorPoint.transform);
            item.transform.localPosition = itemPos;

            // TODO: 给物体挂载自旋转脚本
            

            Debug.Log("Row:" + row);
            Debug.Log("Col:" + col);
        }


        public void RemoveItemFromPackage(ItemInPackage item)
        {
            _itemsInPackage.Remove(item);
        }

        #endregion
    }
}