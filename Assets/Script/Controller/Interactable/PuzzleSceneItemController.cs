using Script.Entity;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Interactable
{
    public class PuzzleSceneItemController : BaseInteractableController
    {
        // 解密需要的物品名称
        public string puzzleNeedItemName;
        private bool puzzleDone = false;

        public override void OnInteract()
        {
            if (puzzleDone) return;
            // 1. 打开物品栏
            // 2. 选择需要选择的物品 (name)
            // 3. 按E键进行选择
            BagManager.Instance.ToggleBagVisible(OnItemSelect);
        }

        protected virtual void OnPuzzleSuccess()
        {
            Debug.Log("解密成功");
            puzzleDone = true;
        }

        private void OnItemSelect(ItemInPackage item)
        {
            if (item.ItemName != puzzleNeedItemName) return;

            // 解密成功,物品栏删除物品
            BagManager.Instance.RemoveItemFromPackage(item.ItemName);
            // 关闭背包
            BagManager.Instance.ToggleBagVisible(false);
            OnPuzzleSuccess();
        }
    }
}