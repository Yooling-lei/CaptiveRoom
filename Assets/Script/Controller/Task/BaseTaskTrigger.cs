using Script.Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Controller.Task
{
    public class BaseTaskTrigger : MonoBehaviour
    {
        protected virtual bool Verify(string taskName)
        {
            if (TaskManager.Instance.GetTask(taskName) == null) return true;
            return false;
        }
    }
}