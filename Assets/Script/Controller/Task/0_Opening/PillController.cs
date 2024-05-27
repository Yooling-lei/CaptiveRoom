using System;
using Script.Interface;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Task.TaskTriggers
{
    public class PillController : MonoBehaviour
    {
        private void Start()
        {
            AddTakePillTask();
        }

        private string _takePillTaskName = "TakePill";

        private void AddTakePillTask()
        {
            TaskManager.Instance.AddTask(_takePillTaskName, OnTaskDone);
        }

        private void OnTaskDone()
        {
            Debug.Log("达成目标");
        }


        // public void OnItemUse()
        // {
        //     // 完成任务
        //     TaskManager.Instance.FinishTask("TakePill");
        // }
    }
}