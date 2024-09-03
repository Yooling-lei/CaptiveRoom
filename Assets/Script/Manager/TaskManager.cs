using System;
using System.Collections.Generic;
using Script.Entity;
using Script.Enums;
using UnityEngine;

namespace Script.Manager
{
    public class TaskManager : Singleton<TaskManager>
    {
        // public List<TaskEntity> taskEntities = new List<TaskEntity>();
        public Dictionary<string, TaskEntity> taskEntities = new Dictionary<string, TaskEntity>();
        public Dictionary<string, Action> taskActions = new Dictionary<string, Action>();

        public void AddTask(string title)
        {
            taskEntities.Add(title, new TaskEntity()
            {
                Title = title,
                Status = ETaskStatus.UnStart,
            });
        }

        /// <summary>
        /// 订阅任务完成事件
        /// </summary>
        /// <param name="title">任务主键</param>
        /// <param name="action">任务完成时触发委托</param>
        public void ListenTask(string title, Action action)
        {
            if (!taskEntities.ContainsKey(title)) return;
            if (taskActions.ContainsKey(title))
                taskActions[title] += action;
            else
                taskActions.Add(title, action);
        }

        public void FinishTask(string title)
        {
            if (!taskEntities.ContainsKey(title)) return;
            Debug.Log("Finish Task: " + title);
            
            taskEntities[title].Status = ETaskStatus.Done;
            if (taskActions.ContainsKey(title))
            {
                taskActions[title]?.Invoke();
            }
        }

        public TaskEntity GetTask(string title)
        {
            return taskEntities.GetValueOrDefault(title);
        }
    }
}