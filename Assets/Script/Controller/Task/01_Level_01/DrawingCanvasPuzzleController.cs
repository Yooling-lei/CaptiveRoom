using System;
using System.Collections.Generic;
using Script.Controller.Interactable;
using Script.Entity;
using Script.Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Controller.Task._01_Level_01
{
    public class DrawingCanvasPuzzleController : PuzzleSceneItemController
    {
        private bool _matched;
        private DrawingPuzzleTotalController _totalController;
        public GameObject drawingPanel;
        public Material redPainting;
        public Material bluePainting;
        public Material yellowPainting;
        public Material emptyPainting;

        private void Start()
        {
            _totalController = FindObjectOfType<DrawingPuzzleTotalController>();
        }


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
            _matched = item.ItemName == puzzleNeedItemName;
            _totalController.SetMatched(item.ItemName, _matched, this);

            // 关闭背包
            BagManager.Instance.ToggleBagVisible(false);
        }

        public void Success()
        {
            OnPuzzleSuccess();
        }


        // 设置当前GameObject的材质
        private void SetMaterial(Material material)
        {
            drawingPanel.GetComponent<Renderer>().material = material;
        }

        // 根据获取名字为材质
        private Material GetRedMaterial(string colorName)
        {
            return colorName switch
            {
                "RedBrush" => redPainting,
                "BlueBrush" => bluePainting,
                "YellowBrush" => yellowPainting,
                _ => emptyPainting
            };
        }
    }
}