using System;
using System.Linq;
using IngameDebugConsole;
using Script.Controller.Common;
using Script.Controller.Interactable;
using Script.Controller.UI;
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
        [Header("玩家")] [Tooltip("是否允许玩家移动")] public bool enableMovement = true;

        // 默认是否锁定鼠标
        [Header("鼠标设置")] public bool cursorLocked = true;

        // 控制游戏运行状态
        public EGameStatus GameStatus { get; set; } = EGameStatus.InProgress;

        // 玩家实例
        [HideInInspector] public GameObject player;

        // 玩家输入接收器
        [HideInInspector] public PlayerInputReceiver playerInputReceiver;

        // 世界空间UI
        [HideInInspector] public GameObject worldSpaceCanvas;

        private void Start()
        {
            worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas");
        }

        public void RegisterPlayer(GameObject obj)
        {
            player = obj;
            playerInputReceiver = player.GetComponent<PlayerInputReceiver>();
        }

        public void LockCursor()
        {
            cursorLocked = true;
            SetCursorState(cursorLocked);
            if (playerInputReceiver != null) playerInputReceiver.cursorInputForLook = true;
        }

        public void UnlockCursor()
        {
            cursorLocked = false;
            SetCursorState(cursorLocked);
            if (playerInputReceiver != null) playerInputReceiver.cursorInputForLook = false;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private static void SetCursorState(bool newState)
        {
            UnityEngine.Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public void SwitchGameStatus(EGameStatus status)
        {
            // TODO: 暂停时PlayerInput应该被禁用
            GameStatus = status;
        }

        #region 字幕控制

        public SubtitleController subtitleController;

        public void AddSubtitleToPlay(SubtitleEntity subtitle, bool breakCurrent = false)
        {
            if (subtitleController == null)
            {
                Debug.LogError("SubtitleController is null");
                return;
            }

            subtitleController.AddSubtitleInSequence(subtitle, breakCurrent);
        }

        #endregion
    }
}