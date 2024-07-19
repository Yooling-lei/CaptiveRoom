using System.Collections.Generic;
using System.Linq;
using Script.Entity;
using Script.Interface;
using Script.Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Controller.Interactable
{
    // TODO: 改成CUP Merge
    public class MergeableController : MonoBehaviour, IMergeableItem
    {
        // 新生成的物品
        public GameObject generateItemPrefab;
        public Vector3 generatePosition;

        public bool MergeCheck(List<ItemInPackage> items)
        {
            if (items.Count != 2) return false;
            var names = items.Select(x => x.ItemName);
            return names.Contains("BrokenCup") && names.Contains("Glue");
        }


        public void OnMergeCheckFailed()
        {
            var subtitle = new SubtitleEntity()
            {
                Key = "MergeCheckFailed",
                SubtitleText = "It can't work like this. I need to find the right way to fix it.",
                Duration = 3.0f
            };
            GameManager.Instance.AddSubtitleToPlay(subtitle);
        }

        public void OnMergeSuccess()
        {
            var subtitle = new SubtitleEntity()
            {
                Key = "MergeSuccess",
                SubtitleText = "Right",
                Duration = 3.0f
            };
            GameManager.Instance.AddSubtitleToPlay(subtitle);
            Debug.Log("UI 动画: 合成成功！");
            Debug.Log("删除当前物品, 其他行为...");

            BagManager.Instance.RemoveItemFromPackage("BrokenCup");
            BagManager.Instance.RemoveItemFromPackage("Glue");

            // 把这个物品放到游戏场景
            Instantiate(generateItemPrefab, generatePosition, Quaternion.identity);
        }
    }
}