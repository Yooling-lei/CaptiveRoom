using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    public Animator cutScenesAnimator;
    public Animator bedAnimator;
    public string[] cutScenesAnimationName;

    private void PlayCutScenesAnimation(int i)
    {
        if (i < 1 || i > cutScenesAnimationName.Length) return;
        cutScenesAnimator.Play(cutScenesAnimationName[i - 1]);
        bedAnimator.Play(cutScenesAnimationName[i - 1]);
    }
}