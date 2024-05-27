using Script.Controller.Interactable;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Task._0_Opening
{
    public class EatPillCupController : PuzzleSceneItemController
    {
        protected override void OnPuzzleSuccess()
        {
            base.OnPuzzleSuccess();
            TaskManager.Instance.FinishTask("TakePill");
            Destroy(GetComponent<BoxCollider>());
        }
    }
}