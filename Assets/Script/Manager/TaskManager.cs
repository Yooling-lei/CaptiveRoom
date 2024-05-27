using System;
using System.Collections.Generic;
using Script.Entity;
using Script.Enums;

namespace Script.Manager
{
    public class TaskManager : Singleton<TaskManager>
    {
        // public List<TaskEntity> taskEntities = new List<TaskEntity>();

        public Dictionary<string, TaskEntity> taskEntities = new Dictionary<string, TaskEntity>();

        // 这个游戏的流程应该是比较简单的

        private void OnEnable()
        {
            //FIXME: FOR TEST  
            // taskEntities.Add(new TaskEntity()
            // {
            //     Title = "TakePill",
            //     Status = ETaskStatus.UnStart,
            // });
        }

        public void AddTask(string title, Action action)
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
    }
}