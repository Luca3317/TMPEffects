using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.Tags;
using System.Data.Common;

public class ComponentController : MonoBehaviour
{
    [SerializeField] TMPAnimator animator;
    [SerializeField] TMPWriter writer;

    [SerializeField]
    string[] texts = new string[]
    {
        "This is one damn short text.",
        "This one is just the tiniest bit longer.",
        "Bye."
    };

    int i = 0;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            HandleAnimatorInput();
        else
            HandleWriterInput();
    }

    private void OnEnable()
    {
        writer.OnFinishWriter.AddListener(StartCoroutine);
    }

    private void StartCoroutine()
    {
        StartCoroutine(ListenWriter());
    }

    private IEnumerator ListenWriter()
    {
        yield return null;
        //yield return new WaitForSeconds(1.5f);

        //writer.Hide(0, writer.TextComponent.textInfo.characterCount, false);

        //yield return new WaitForSeconds(1.5f);

        //writer.SetText(texts[i]);
        //i++;
        //i %= texts.Length;
    }

    void HandleAnimatorInput()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            animator.SetUpdateFrom(UpdateFrom.Update);
            return;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            animator.SetUpdateFrom(UpdateFrom.LateUpdate);
            return;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetUpdateFrom(UpdateFrom.FixedUpdate);
            return;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetUpdateFrom(UpdateFrom.Script);
            return;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            animator.UpdateAnimations(Time.deltaTime);
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.ResetAnimations();
            return;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (animator.IsAnimating) animator.StopAnimating();
            else animator.StartAnimating();
            return;
        }
    }

    void HandleWriterInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            writer.SkipWriter();
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            writer.RestartWriter();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            writer.ResetWriter();
            return;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            EffectTag tag = new EffectTag("wait", '!', new Dictionary<string, string>() { { "", "2" } });
            EffectTagIndices indices = new EffectTagIndices(writer.CurrentIndex + 1, writer.CurrentIndex + 2, 0);
            writer.CommandTags.TryAdd(tag, indices);
            return;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (writer.IsWriting) writer.StopWriter();
            else writer.StartWriter();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            writer.ResetWriter(writer.TextComponent.textInfo.characterCount * 0);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            writer.ResetWriter((int)(writer.TextComponent.textInfo.characterCount * 0.1f));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            writer.ResetWriter((int)(writer.TextComponent.textInfo.characterCount * 0.2f));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            writer.ResetWriter((int)(writer.TextComponent.textInfo.characterCount * 0.3f));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            writer.ResetWriter((int)(writer.TextComponent.textInfo.characterCount * 0.4f));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            writer.ResetWriter((int)(writer.TextComponent.textInfo.characterCount * 0.5f));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            writer.ResetWriter((int)(writer.TextComponent.textInfo.characterCount * 0.6f));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            writer.ResetWriter((int)(writer.TextComponent.textInfo.characterCount * 0.7f));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            writer.ResetWriter((int)(writer.TextComponent.textInfo.characterCount * 0.8f));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            writer.ResetWriter((int)(writer.TextComponent.textInfo.characterCount * 0.9f));
            return;
        }
    }
}
