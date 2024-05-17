using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ShakeManager : MonoBehaviour
{
    public string animationName = "Entrance";
    public Animator shakeAnimator;

    private Animator animator;
    private bool didTrigger = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isPlayingAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);

        if (!isPlayingAnimation && !didTrigger)
        {
            shakeAnimator.SetTrigger("Shake");
            didTrigger = true;
        }
    }
}
