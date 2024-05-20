using System;
using System.Linq;
using IngameDebugConsole;
using Script.Controller.Common;
using Script.Controller.Interactable;
using Script.Entity;
using Script.Enums;
using Script.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Script.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        // 保存游戏实例:   Player, 
        // 不需要在编辑器初始化
        [HideInInspector] public GameObject player;

        // 世界空间UI
        [HideInInspector] public GameObject worldSpaceCanvas;

        // 控制游戏运行状态, 
        public EGameStatus gameStatus;

        private void Start()
        {
            worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas");
        }

        public void RegisterPlayer(GameObject obj)
        {
            player = obj;
        }

        public void SwitchGameStatus(EGameStatus status)
        {
            gameStatus = status;
            // TODO: 暂停时PlayerInput应该被禁用
        }

        private void Update()
        {
            // FIXME: UI总体控制
            // UpdateBagVisible();

            // if (Mouse.current.leftButton.isPressed)
            // {
            //     // 获取鼠标位置
            //     var mousePos = Mouse.current.position.ReadValue();
            //     // 世界坐标转换为屏幕坐标
            //     // var screenPos = Camera.main.WorldToScreenPoint(mousePos);
            //
            //     EventSystem eventSystem = EventSystem.current;
            //     PointerEventData eventData = new PointerEventData(eventSystem);
            //     eventData.position = mousePos;
            //
            //     // 检测是否点击到UI
            //     if (eventSystem.IsPointerOverGameObject())
            //     {
            //         Debug.Log("点击到UI");
            //     }
            //     else
            //     {
            //         Debug.Log("点击到场景");
            //     }
            //
            //
            //     Debug.Log("screen Pos" + mousePos);
            // }
        }
    }
}