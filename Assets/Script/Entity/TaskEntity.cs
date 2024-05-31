using System;
using Script.Enums;

namespace Script.Entity
{
    public class TaskEntity
    {
        public string Title { get; set; }

        public ETaskStatus Status { get; set; }
    }
}