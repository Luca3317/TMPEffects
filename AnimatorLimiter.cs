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
    private float updateInterval; // 144 times per second

    bool added = false;

    // Start is called before the first frame update
    void Start()
    {
        updateInterval = 1.0f / updatesPerSecond;
        animator.SetUpdateFrom(UpdateFrom.Script);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("UPDATE");
        if (!added)
        {
            TMPEffectTag tag = new TMPEffectTag("wave", '\0', new Dictionary<string, string>());
            TMPEffectTagIndices indices = new TMPEffectTagIndices(0, -1, 0);

            if (animator.BasicTags.TryAdd(tag, indices))
            {
                Debug.Log("Added!");
            }
            else Debug.Log("Failed to add!");

            foreach (var t in animator.BasicTags)
            {
                Debug.Log(t.Tag.Name + " Start " + t.Indices.StartIndex + " End " + t.Indices.EndIndex + " Order " + t.Indices.OrderAtIndex);
            }

            added = true;



            tag = new TMPEffectTag("fade", '+', new Dictionary<string, string>());
            indices = new TMPEffectTagIndices(0, 10, 0);

            if (animator.ShowTags.TryAdd(tag, indices))
            {
                Debug.Log("added show!");
            }
            else Debug.Log("Failed to add show!");

            foreach (var t in animator.ShowTags)
            {
                Debug.Log(t.Tag.Name + " Start " + t.Indices.StartIndex + " End " + t.Indices.EndIndex + " Order " + t.Indices.OrderAtIndex);
            }


        }

        timer += Time.deltaTime;

        if (timer > updateInterval)
        {
            int amount = (int)(timer / updateInterval);

            if (animator.isActiveAndEnabled)
                animator.UpdateAnimations(amount * updateInterval);

            timer %= updateInterval;
        }

        foreach (var t in animator.BasicTags)
        {
            Debug.Log("BASIC "  + t.Tag.Name + " Start " + t.Indices.StartIndex + " End " + t.Indices.EndIndex + " Order " + t.Indices.OrderAtIndex);
        }

        foreach (var t in animator.BasicTags)
        {
            Debug.Log("SHOW " + t.Tag.Name + " Start " + t.Indices.StartIndex + " End " + t.Indices.EndIndex + " Order " + t.Indices.OrderAtIndex);
        }

    }

    private void OnValidate()
    {
        updateInterval = 1.0f / updatesPerSecond;
    } 
}
