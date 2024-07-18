using System.Collections.Generic;
using Script.Entity;
using Script.Interface;
using UnityEngine;

namespace Script.Controller.Interactable
{
    public class MergeableController : MonoBehaviour, IMergeableItem
    {
        // 新生成的物品
        public GameObject generateItemPrefab;
        
        // TODO: 改成CUP Merge
        
        
        public bool MergeCheck(List<ItemInPackage> items)
        {
            return true;
        }

        public void OnMergeCheckFailed()
        {
            Debug.Log("UI 提示: 不太对...");
        }

        public void OnMergeSuccess()
        {
            Debug.Log("UI 动画: 合成成功！");
            Debug.Log("删除当前物品, 其他行为...");
        }
    }
}