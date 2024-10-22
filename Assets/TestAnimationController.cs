using System.Collections;
using System.Collections.Generic;
using Script.Manager;
using UnityEngine;
using UnityEngine.UI;

public class TestAnimationController : MonoBehaviour
{
    PlayerInputReceiver _input;
    private float _focusDuration = 0f;
    private bool init;
    private Animator animator;
    public Image filmingMarkImage;

    // 运行时调用
    void Start()
    {
    }

    void InitFunc()
    {
        _input = GameManager.Instance.playerInputReceiver;
        animator = GetComponent<Animator>();

        if (_input != null) init = true;
    }

    // 每帧更新时调用
    void Update()
    {
        if (!init)
        {
            InitFunc();
            return;
        }

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        var animationDone = false;
        if (_input.viewFocus)
        {
            // TODO: 1.Canvas上画一个摄像中的UI
            // TODO: 2.当拍立得动画播放完毕, 展示这个UI
            // TODO: 3.当松开右键,要播放收起动画时, 隐藏这个UI
            
            if (stateInfo.normalizedTime >= 1.0f)
            {
                // 播完了
                animator.SetFloat("Speed", 0f);
                animationDone = true;
                // 播完了, 显示摄像中的UI
                // filmingMarkImage.enabled = true;
            }
            else
            {
                animator.SetFloat("Speed", 1f);
                // filmingMarkImage.enabled = false;
            }
        }
        else
        {
            if (stateInfo.normalizedTime <= 0.01f)
            {
                // 播完了
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                animator.SetFloat("Speed", -1f);
            }
        }
        
        filmingMarkImage.enabled = animationDone;
        if (animationDone)
        {
            Debug.Log("111111111111111111111111111");
        }
        // 对于现在的需求, 动画完成, 应该进入新的视角(摄像头视角)
        // 进入新视角后, 松开右键, 返回原视角
    }
}