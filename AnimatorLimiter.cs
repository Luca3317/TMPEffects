using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.Tags;

public class AnimatorLimiter : MonoBehaviour
{
    [SerializeField] TMPAnimator animator;
    [SerializeField] int updatesPerSecond;

    private float timer = 0.0f;
    private float updateInterval;

    // Start is called before the first frame update
    void Start()
    {
        if (animator == null) return;
        updateInterval = 1.0f / updatesPerSecond;
        animator.SetUpdateFrom(UpdateFrom.Script);
    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null) return;

        timer += Time.deltaTime;

        if (timer > updateInterval)
        {
            int amount = (int)(timer / updateInterval);

            if (animator.isActiveAndEnabled)
                animator.UpdateAnimations(amount * updateInterval);

            timer %= updateInterval;
        }

    }

    private void OnValidate()
    {
        updateInterval = 1.0f / updatesPerSecond;
    } 
}
