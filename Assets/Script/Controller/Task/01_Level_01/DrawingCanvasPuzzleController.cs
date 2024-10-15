using System;
using System.Collections.Generic;
using Script.Controller.Interactable;
using Script.Entity;
using Script.Manager;
using UnityEngine;

namespace Script.Controller.Task._01_Level_01
{
    public class DrawingCanvasPuzzleController : PuzzleSceneItemController
    {
        private bool _matched = false;

        public bool isMatched => _matched;

        public override void OnInteract()
        {
            if (puzzleDone) return;
            BagManager.Instance.ToggleBagVisible(OnItemSelect);
        }

        private readonly List<string> _puzzleNeedItems = new() { "RedBrush", "BlueBrush", "YellowBrush", };

        private void OnItemSelect(ItemInPackage item)
        {
            if (!_puzzleNeedItems.Contains(item.ItemName)) return;

            // 修改对应的贴图颜色
            SetMaterial(GetRedMaterial(item.ItemName));
            if (item.ItemName == puzzleNeedItemName)
            {
                _matched = true;
            }

            // 关闭背包
            BagManager.Instance.ToggleBagVisible(false);
        }

        // 设置当前GameObject的材质
        public void SetMaterial(Material material)
        {
            GetComponent<Renderer>().material = material;
        }

        // 获取名字为"RedMaterial"的材质
        public Material GetRedMaterial(string name)
        {
            var materialName = "";
            switch (name)
            {
                case "RedBrush":
                    materialName = "RedMaterial";
                    break;
                case "BlueBrush":
                    materialName = "BlueMaterial";
                    break;
                case "YellowBrush":
                    materialName = "YellowMaterial";
                    break;
            }

            return Resources.Load<Material>("Material/" + materialName);
        }

        // private void Start()
        // {
        //     SetMaterial(GetRedMaterial());
        // }
    }
}