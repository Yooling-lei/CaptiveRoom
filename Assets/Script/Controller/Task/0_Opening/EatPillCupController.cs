using Script.Controller.Interactable;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Task._0_Opening
{
    /// <summary>
    /// 吃药用的杯子控制器
    /// </summary>
    public class EatPillCupController : PuzzleSceneItemController
    {
        protected override void OnPuzzleSuccess()
        {
            base.OnPuzzleSuccess();
            TaskManager.Instance.FinishTask("TakePill");
            Destroy(GetComponent<BoxCollider>());

            // TODO: 播放摔碎杯子动画
            // 播放完后,生成碎玻璃GameObject,触发下个任务
        }

        private void NextTask()
        {
            TaskManager.Instance.AddTask("FixCup");
        }
    }
}