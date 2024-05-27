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
        private int _count = 0;

        private void Start()
        {
            takePillTask = TaskManager.Instance.GetTask("TakePill");
        }

        public override void OnInteract()
        {
            _count++;
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
                if (_count == 1)
                {
                    var subtitle = new SubtitleEntity()
                    {
                        Key = "TakePillFirst",
                        SubtitleText = "I need to take the pill first.",
                        Duration = 3.0f
                    };
                    GameManager.Instance.AddSubtitleToPlay(subtitle);
                }
                else
                {
                    var subtitle = new SubtitleEntity()
                    {
                        Key = "TakePillFirst2",
                        SubtitleText = "NO, I really need to take the pill first.",
                        Duration = 3.0f
                    };
                    GameManager.Instance.AddSubtitleToPlay(subtitle);
                }
            }
        }
    }
}