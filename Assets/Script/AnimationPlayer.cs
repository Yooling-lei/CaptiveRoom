using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationPlayer : MonoBehaviour
{
    public Animator cutScenesAnimator;
    public Animator bedAnimator;

    public string[] cutScenesAnimationName;
    
    void Update()
    {
        // var keyboard = Keyboard.current;
        // if (keyboard.digit1Key.wasPressedThisFrame)
        // {
        //     PlayCutScenesAnimation(1);
        // }
    }


    private void PlayCutScenesAnimation(int i)
    {
        if (i < 1 || i > cutScenesAnimationName.Length) return;
        cutScenesAnimator.Play(cutScenesAnimationName[i - 1]);
        bedAnimator.Play(cutScenesAnimationName[i - 1]);
    }
}