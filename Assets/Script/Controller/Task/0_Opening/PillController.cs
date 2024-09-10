using Script.Manager;
using UnityEngine;

namespace Script.Controller.Task._0_Opening
{
    public class PillController : BaseTaskTrigger
    {
        private readonly string _takePillTaskName = "TakePill";

        // TODO: 这种物品初始化时基于任务系统
        private void Start()
        {
            if (base.Verify(_takePillTaskName))
            {
                AddTakePillTask();
            }
        }


        private void AddTakePillTask()
        {
            TaskManager.Instance.AddTask(_takePillTaskName);
            TaskManager.Instance.ListenTask(_takePillTaskName, OnTaskDone);
        }


        private void OnTaskDone()
        {
            Debug.Log("达成目标");
        }
    }
}