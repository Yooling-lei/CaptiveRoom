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
        private TaskEntity _takePillTask;
        private TaskEntity _fixCupTask;
        private int _count = 0;


        private TaskEntity GetTask(string title)
        {
            switch (title)
            {
                case "TakePill":
                    return _takePillTask ??= TaskManager.Instance.GetTask(title);
                case "FixCup":
                    return _fixCupTask ??= TaskManager.Instance.GetTask(title);
            }

            return null;
        }

        public override void OnInteract()
        {
            _count++;

            var pillTask = GetTask("TakePill");
            if (pillTask == null) return;

            if (pillTask.Status != ETaskStatus.Done)
            {
                AlertNotTakePill(_count);
            }
            else
            {
                var fixCupTask = GetTask("FixCup");
                if (fixCupTask == null) return;

                if (fixCupTask.Status == ETaskStatus.Done)
                {
                    Debug.Log("前往下一个场景");
                    return;
                }

                AlertFixCup(_count);
            }
        }

        private void AlertNotTakePill(int count)
        {
            if (count == 1)
            {
                var subtitle = new SubtitleEntity()
                {
                    Key = "TakePillFirst",
                    SubtitleText = "I need to take the pill first.",
                    Duration = 8.0f
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
                GameManager.Instance.AddSubtitleToPlay(subtitle, true);
            }
        }

        private void AlertFixCup(int count)
        {
            var subtitle = new SubtitleEntity()
            {
                Key = "FixCup",
                SubtitleText = "Come on, I  need to fix the mass.",
                Duration = 3.0f
            };
            GameManager.Instance.AddSubtitleToPlay(subtitle);
        }
    }
}