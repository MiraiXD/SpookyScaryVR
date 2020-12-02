using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardAnimatorMove : MonoBehaviour
{
    private Animator animator;    
    public System.Action<Animator> onAnimatorMove;
    public System.Action<string> onAnimationEvent;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnAnimatorMove()
    {
        onAnimatorMove?.Invoke(animator);
    }    
    private void AnimationEvent(string animationEvent)
    {
        onAnimationEvent?.Invoke(animationEvent);
    }
}
