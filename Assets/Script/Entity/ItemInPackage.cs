using System;
using Script.Manager;
using TMPro;
using UnityEngine;

namespace Script.Entity
{
    public class ItemInPackage
    {
        public string ItemName { get; set; }

        public int Count { get; set; }

        // 关联的GameObject
        public GameObject LinkGameObject { get; set; }

        public GameObject ModelInBag { get; set; }

        private TextMeshProUGUI CountText { get; set; }

        private Vector3 Offset { get; set; } = new Vector3(-0.3f, 1, -0.3f);


        public void RefreshCountText()
        {
            var linkTransform = ModelInBag.transform;
            if (CountText is null)
            {
                var prefab = PrefabManager.Instance.footNotePrefab;
                var parent = GameManager.Instance.WorldSpaceCanvas.transform;
                CountText = UnityEngine.Object.Instantiate(prefab, parent);
                CountText.transform.position = linkTransform.position + Offset;
                CountText.text = Count.ToString();
            }
            else
            {
                CountText.text = Count < 2 ? "" : Count.ToString();
            }
        }
    }
}