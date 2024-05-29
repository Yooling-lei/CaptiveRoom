using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Script.Entity;
using Script.Enums;

namespace Script.Manager
{
    public class TaskManager : Singleton<TaskManager>
    {
        // public List<TaskEntity> taskEntities = new List<TaskEntity>();
        public Dictionary<string, TaskEntity> taskEntities = new Dictionary<string, TaskEntity>();
        
        private void OnEnable()
        {
        }

        public void AddTask(string title, Action action = null)
        {
            taskEntities.Add(title, new TaskEntity()
            {
                Title = title,
                Status = ETaskStatus.UnStart,
                Action = action
            });
        }

        public void FinishTask(string title)
        {
            if (!taskEntities.ContainsKey(title)) return;
            taskEntities[title].Status = ETaskStatus.Done;
            taskEntities[title].Action?.Invoke();
        }

        public TaskEntity GetTask(string title)
        {
            if (!taskEntities.ContainsKey(title)) return null;
            return taskEntities[title];
        }
        
        
        // 游戏进程:
        // 1-1, 
        // 读取存档,发现是1-1
        
        
    }
}