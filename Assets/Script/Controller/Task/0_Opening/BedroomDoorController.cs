using System;
using System.Threading.Tasks;
using Script.Controller.Interactable;
using Script.Entity;
using Script.Enums;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Task.TaskTriggers
{
    public class BedroomDoorController : BaseInteractableController
    {
        private TaskEntity takePillTask;

        private void Start()
        {
            takePillTask = TaskManager.Instance.GetTask("TakePill");
        }

        public override void OnInteract()
        {
            if (takePillTask == null)
            {
                takePillTask = TaskManager.Instance.GetTask("TakePill");
                if (takePillTask == null)
                {
                    Debug.LogError("任务不存在: TakePill");
                    return;
                }
            }

            if (takePillTask.Status == ETaskStatus.Done)
            {
                Debug.Log("打开门,到下一个场景");
            }
            else
            {
                Debug.Log("还没有吃药");
            }
        }
    }
}