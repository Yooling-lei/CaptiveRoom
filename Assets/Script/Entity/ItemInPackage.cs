using System;
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
        public string ItemName { get; set; }

        public int Count { get; set; }

        public float ScaleInBag { get; set; } = 1;

        // public Vector2 BagIndex { get; set; }

        public int BagRow { get; set; }

        public int BagCol { get; set; }

        public float ItemOffset { get; set; }

        // 关联的GameObject
        public GameObject LinkGameObject { get; set; }

        public PickupItemController PickupItemController { get; set; }

        // 在背包场景的GameObject
        public GameObject ModelInBag { get; set; }

        private Action onUse;
        private bool isUsable = false;
        private TextMeshProUGUI CountText { get; set; }

        private Vector3 Offset { get; set; } = new Vector3(-0.3f, 1, -0.3f);

        private bool hasModelInBag = false;

        public ItemInPackage(string itemName, int count, float scaleInBag, GameObject linkGameObject)
        {
            ItemName = itemName;
            Count = count;
            LinkGameObject = linkGameObject;
            ScaleInBag = scaleInBag;
            PickupItemController = linkGameObject.GetComponent<PickupItemController>();
            var usable = linkGameObject.GetComponent<IUsableItem>();
            isUsable = usable != null;
            if (usable != null)
            {
                onUse += usable.OnItemUse;
            }
        }

        #region 视图层更新 (场景 + 角标)

        /// <summary>
        /// 在背包场景生成旋转的物体模型 
        /// </summary>
        public void InitModelInBag(Transform anchor, int row, int col, float offset)
        {
            if (hasModelInBag) return;

            // var instance = new GameObject()
            // {
            //     transform = { parent = anchor }
            // };
            var initTarget = PickupItemController?.itemModel ?? LinkGameObject;
            var instance = Object.Instantiate(initTarget, anchor, true);
            // 复制LinkGameObject


            // 计算物体位置
            ItemOffset = offset;
            var itemPos = CalculateItemInBagScenePosition(row, col, ItemOffset);
            instance.transform.localPosition = itemPos;
            instance.transform.localScale = new Vector3(ScaleInBag, ScaleInBag, ScaleInBag);

            // 通过捡拾的物体,赋值空物体的mesh和material
            // var meshFilter = instance.AddComponent<MeshFilter>();
            // meshFilter.mesh = LinkGameObject.GetComponent<MeshFilter>().mesh;
            // var meshRenderer = instance.AddComponent<MeshRenderer>();
            // meshRenderer.material = LinkGameObject.GetComponent<MeshRenderer>().material;


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

            // TODO: 是不是太简单了?
            onUse.Invoke();
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