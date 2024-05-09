using System.Collections.Generic;
using System.Linq;
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


        // 背包系统
        // public List<>

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


        public void RemoveItemFromPackage(ItemInPackage item)
        {
            _itemsInPackage.Remove(item);
        }

        #endregion
    }
}