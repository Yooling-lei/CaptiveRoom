using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Script.Entity;
using UnityEngine;

namespace Script.Manager
{
    public class SaveDataEntity
    {
        public Dictionary<string, TaskEntity> tasks;
        public Matrix<ItemInPackage> bag;
    }

    public static class SaveManager
    {
        public const string SavePath = "F:\\CreatingProjects\\CaptiveDemo\\SaveData\\";

        public static void SaveData()
        {
            var saveDataEntity = new SaveDataEntity()
            {
                tasks = TaskManager.Instance.taskEntities,
                bag = BagManager.Instance.BagMatrix
            };


            var output = JsonConvert.SerializeObject(saveDataEntity);
            File.WriteAllText(SavePath + "data.json", output);
        }


        public static void LoadData()
        {
            // 1.task进度
            var path = SavePath + "data.json";
            var saveData = File.ReadAllText(path);
            var saveDataEntity = JsonConvert.DeserializeObject<SaveDataEntity>(saveData);


            Debug.Log("read.........." + saveDataEntity);
        }


        // 1.TaskManager.taskEntities
        //   统一序列化和反序列化
        //       AddTask时,脚本自行判断,Action改为订阅制 (订阅时判断任务状态)
        // 2.BagManager.BagMatrix (问题: 实例化时,需要PickupItemController?.itemModel)
        //       需要把itemModel通过一个Map来管理键值对, 读取时,通过键值对找到对应的itemModel
        // 场景中需要持久化状态的物体, (可以挂载脚本实现)  ISavable
    }
}