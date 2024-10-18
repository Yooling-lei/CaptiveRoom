using System.Collections;
using System.Collections.Generic;
using Script.Manager;
using UnityEngine;

public class TestAnimationController : MonoBehaviour
{
    PlayerInputReceiver _input;
    private float _focusDuration = 0f;
    private bool init;
    private Animator animator;

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
        if (_input.viewFocus)
        {
            if (stateInfo.normalizedTime >= 1.0f)
            {
                // 播完了
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                animator.SetFloat("Speed", 1f);
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
    }
}