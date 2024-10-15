using Script.Controller.Interactable;
using UnityEngine;

namespace Script.Controller.Task._01_Level_01
{
    public class DrawingPuzzleTotalController : MonoBehaviour
    {
        private bool _redMatched = false;
        private bool _blueMatched = false;
        private bool _yellowMatched = false;

        private DrawingCanvasPuzzleController _redController;
        private DrawingCanvasPuzzleController _blueController;
        private DrawingCanvasPuzzleController _yellowController;

        public void SetMatched(string colorName, bool value, DrawingCanvasPuzzleController instance)
        {
            switch (colorName)
            {
                case "RedBrush":
                    _redMatched = value;
                    _redController = instance;
                    break;
                case "BlueBrush":
                    _blueMatched = value;
                    _blueController = instance;
                    break;
                case "YellowBrush":
                    _yellowMatched = value;
                    _yellowController = instance;
                    break;
            }

            // 是否全部匹配
            if (!_redMatched || !_blueMatched || !_yellowMatched) return;
            NextPuzzle();
        }

        private void NextPuzzle()
        {
            _redController.Success();
            _blueController.Success();
            _yellowController.Success();

            // TODO: 当三个面板全部匹配时, 触发后续
            // 1. 播放类似老师鼓励动画
            // 2. 刷出一个拍立得相机在地上
            // 3.画板变的无法互动
        }
    }
}