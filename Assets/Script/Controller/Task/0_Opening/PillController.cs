﻿using System;
using Script.Interface;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Task.TaskTriggers
{
    public class PillController : MonoBehaviour
    {
        // TODO: 这种物品初始化时基于任务系统
        private void Start()
        {
            AddTakePillTask();
        }

        private string _takePillTaskName = "TakePill";

        private void AddTakePillTask()
        {
            TaskManager.Instance.AddTask(_takePillTaskName, OnTaskDone);
        }

        private string test = "gaga";

        private void OnTaskDone()
        {
            Debug.Log("达成目标" + test);
        }
    }
}