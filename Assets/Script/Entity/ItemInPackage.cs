using System;
using Newtonsoft.Json;
using Script.Controller.Interactable;
using Script.Interface;
using Script.Manager;
using Script.Tools;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Script.Entity
{
    public class ItemInPackage
    {
        // 道具名称
        public string ItemName { get; set; }

        // 道具数量
        public int Count { get; set; }

        // 在背包的缩放比例
        public float ScaleInBag { get; set; } = 1;

        // 在背包中的行
        public int BagRow { get; set; }

        // 在背包中的列
        public int BagCol { get; set; }

        // 物体在背包中的偏移量
        public float ItemOffset { get; set; }


        // 在背包场景的 GameObject
        [JsonIgnore] public GameObject ModelInBag { get; set; }

        [JsonIgnore] private TextMeshProUGUI CountText { get; set; }

        // 是否可用
        private bool isUsable = false;

        // 角标的偏移量
        private Vector3 Offset { get; set; } = new Vector3(-0.3f, 1, -0.3f);

        // 是否已经在背包中生成了模型
        private bool hasModelInBag = false;


        public ItemInPackage()
        {
        }

        // 构造函数
        public ItemInPackage(string itemName, int count, float scaleInBag)
        {
            InitProperty(itemName, count, scaleInBag);
        }


        public ItemInPackage(string itemName, int count, float scaleInBag, Transform anchor,
            GameObject prefabInBag,
            int row, int col, float offset)
        {
            InitProperty(itemName, count, scaleInBag);
            InitModelInBag(anchor, prefabInBag, row, col, offset);
        }

        // 初始化属性
        private void InitProperty(string itemName, int count, float scaleInBag)
        {
            ItemName = itemName;
            Count = count;
            ScaleInBag = scaleInBag;
        }


        #region 视图层更新 (场景 + 角标)

        /// <summary>
        /// 在背包场景生成旋转的物体模型 
        /// </summary>
        public void InitModelInBag(Transform anchor, GameObject prefabInBag, int row, int col, float offset)
        {
            if (hasModelInBag) return;

            var instance = Object.Instantiate(prefabInBag, anchor, true);

            // 计算物体位置
            ItemOffset = offset;
            var itemPos = CalculateItemInBagScenePosition(row, col, ItemOffset);
            instance.transform.localPosition = itemPos;
            instance.transform.localScale = new Vector3(ScaleInBag, ScaleInBag, ScaleInBag);

            // 挂载旋转展示脚本
            instance.AddComponent<SelfRotation>();
            SetBagRowAndCol(row, col);
            ModelInBag = instance;
            hasModelInBag = true;
        }

        public void CountPlus()
        {
            Count++;
            RefreshCountText();
        }

        public void CountMinus()
        {
            Count--;
            RefreshCountText();
        }


        /// <summary>
        /// TODO:验证 更新背包中物体的位置
        /// </summary>
        public void UpdatePosition(int row, int col)
        {
            if (ModelInBag is null) return;
            if (row == BagRow && col == BagCol) return;

            SetBagRowAndCol(row, col);
            var itemPos = CalculateItemInBagScenePosition(row, col, ItemOffset);
            ModelInBag.transform.localPosition = itemPos;

            // 更新角标位置
            if (CountText is null) return;
            CountText.transform.position = ModelInBag.transform.position + Offset;
        }

        private void SetBagRowAndCol(int row, int col)
        {
            BagRow = row;
            BagCol = col;
        }

        /// <summary>
        /// 刷新CountText
        /// </summary>
        public void RefreshCountText()
        {
            var linkTransform = ModelInBag.transform;

            if (CountText is null)
            {
                var prefab = PrefabManager.Instance.footNotePrefab;
                var parent = GameManager.Instance.worldSpaceCanvas.transform;
                CountText = UnityEngine.Object.Instantiate(prefab, parent);
                CountText.transform.position = linkTransform.position + Offset;
                CountText.text = Count < 2 ? "" : Count.ToString();
            }
            else
            {
                CountText.text = Count < 2 ? "" : Count.ToString();
            }
        }

        /// <summary>
        /// 计算在背包场景下物体的位置(localPosition)
        /// </summary>
        private static Vector3 CalculateItemInBagScenePosition(int row, int col, float offset)
        {
            return new Vector3(row * -offset, 0, col * -offset);
        }

        #endregion

        #region 物品使用逻辑

        // 物体是否可以直接使用
        public bool IsUsable => isUsable;

        // 使用物体
        public bool UseItem(ref int count)
        {
            if (!isUsable) return false;

            // 若挂载的物体实现了IUsableItem接口,则调用OnItemUse方法
            // var usable = LinkGameObject.GetComponent<IUsableItem>();
            // usable?.OnItemUse();
            CountMinus();
            count = Count;
            return true;
        }

        public void DestroyModelInBag()
        {
            if (ModelInBag is null) return;
            UnityEngine.Object.Destroy(ModelInBag);
            ModelInBag = null;
            hasModelInBag = false;
            if (CountText != null)
            {
                UnityEngine.Object.Destroy(CountText);
            }
        }

        #endregion
    }
}