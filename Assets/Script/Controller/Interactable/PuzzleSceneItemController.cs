using Script.Entity;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Interactable
{
    public class PuzzleSceneItemController : BaseInteractableController
    {
        // 解密需要的物品名称
        public string puzzleNeedItemName;

        public override void OnInteract()
        {
            Debug.Log("解密物体OnInteract");
            // 1. 打开物品栏
            // 2. 选择需要选择的物品 (name)
            // 3. 按E键进行解密

            BagManager.Instance.ToggleBagVisible(OnItemSelect);
        }

        private void OnItemSelect(ItemInPackage item)
        {
            if (item.ItemName != puzzleNeedItemName) return;
            
            // 解密成功,物品栏删除物品
            BagManager.Instance.RemoveItemFromPackage(item.ItemName);
            // 关闭背包
            BagManager.Instance.ToggleBagVisible(false);
        }
    }
}