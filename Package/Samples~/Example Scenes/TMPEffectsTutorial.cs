using System;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TMPEffectsTutorial : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text textComp;
    private TMPAnimator animator;
    private TMPWriter writer;

    public Button next;
    public Button previous;

    int currentSequence;
    private Action[] sequence;

    private const string baseLink = "https://tmpeffects.luca3317.dev/manual";

    void Awake()
    {
        sequence = new Action[]
        {
            Sequence_0,
            Sequence_1,
            Sequence_2,
            Sequence_3,
            Sequence_4,
            Sequence_5,
            Sequence_6,
            Sequence_7,
            Sequence_8,
            Sequence_9,
            Sequence_10,
            Sequence_11,
            Sequence_12,
            Sequence_14,
            Sequence_15,
            Sequence_16,
            Sequence_17
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = textComp.GetComponent<TMPAnimator>();
        writer = textComp.GetComponent<TMPWriter>();

        animator.SetUpdateFrom(UpdateFrom.Script);
        animator.ResetAnimations();

        writer.WriteOnNewText = false;
        writer.WriteOnStart = false;

        currentSequence = 0;
        sequence[currentSequence].Invoke();
    }

    private void Sequence_0()
    {
        writer.enabled = true;
        animator.enabled = true;
        animator.enabled = false;
        animator.enabled = true;
        animator.enabled = false;
        animator.enabled = true;
        animator.enabled = false;
        animator.enabled = true;
        animator.enabled = false;

        writer.enabled = false;
        animator.enabled = true;
        animator.SetText("With TMPEffects, you can modify the way your TextMeshPro texts are displayed in a myriad of ways.\n\nSimply apply the tag corresponding to the effect you want, like you would any other TextMeshPro tag.\n\n" +
            "Example:\n<noparse><wave>waving text</wave></noparse>\n<wave>waving text</wave>");
    }

    private void Sequence_1()
    {
        writer.enabled = false;
        animator.enabled = false;
        animator.SetText("There are two main components of TMPEffects:\n\n1. The TMPAnimator\n\n2. The TMPWriter");

    }

    private void Sequence_2()
    {
        writer.enabled = false;
        animator.enabled = false;
        animator.SetText("<b>TMPAnimator</b>\n\nThe TMPAnimator component allows you to animate character over time using animation tags (for example, the <wave> tag on the previous page).");
    }

    private void Sequence_3()
    {
        writer.enabled = false;
        animator.enabled = true;
        animator.SetText("<b>TMPAnimator</b>\n\nYou can\napply tags to any section of the text:\n<alpha=#CC><noparse>Just some <pivot>example</></noparse> text\nJust some <pivot>example</> text");
    }

    private void Sequence_4()
    {
        writer.enabled = false;
        animator.enabled = true;
        animator.SetText("<b>TMPAnimator</b>\n\nYou can\nstack animation tags any way you want:\n<alpha=#CC><noparse><wave>Yet <pivot>another</wave> example</> text</noparse>\n<wave>Yet <pivot>another</wave> example</> text");
    }

    private void Sequence_5()
    {
        writer.enabled = false;
        animator.enabled = true;
        animator.SetText("<b>TMPAnimator</b>\n\nYou can\nmodify animations with parameters:\n<alpha=#CC><noparse><wave amplitude=5>Yet another example text</></noparse>\n<wave amplitude=5>Yet another example text</>");
    }

    private void Sequence_6()
    {
        writer.enabled = false;
        animator.enabled = true;
        animator.SetText("<b>TMPAnimator</b>\n\n<alpha=#CC><noparse>Yet another <pivot minangle=-180 maxangle=270>example</> text</noparse>\nYet another <pivot minangle=-180 maxangle=270>example</> text" +
            "\n\n<noparse>Yet another <spread growanchor=a:top shrinkanchor=a:right>example</> text</noparse>\nYet another <spread growanchor=a:top shrinkanchor=a:right>example</> text");
    }

    private void Sequence_7()
    {
        writer.enabled = false;
        animator.enabled = false;
        animator.SetText($"<b>TMPAnimator</b>\n\nFor a full overview of all (basic) animations and their parameters, see <color=lightblue><u><link=\"{baseLink}/tmpanimator_builtinbasicanimations.html\">this page of the docs</link>");
    }

    private void Sequence_8()
    {
        writer.enabled = false;
        animator.enabled = false;
        animator.SetText("<b>TMPAnimator</b>\n\nBesides these basic animations, there are also show and hide animations.\nMore about them after the TMPWriter section");
    }

    private void Sequence_9()
    {
        writer.enabled = true;
        animator.enabled = false;
        animator.SetText("<!show><b>TMPWriter</b>\n\n</!><!delay=0.075><!wait=0.1><alpha=#CC>The TMPWriter component allows you to show characters over time.");
        writer.StartWriter();
    }

    private void Sequence_10()
    {
        writer.enabled = false;
        animator.enabled = false;
        //writer.ResetWriter();
        animator.SetText("<b>TMPWriter</b>\n\nYou can modify the behavior of the writing process using command tags, prefixed with a '!'.");
    }

    private void Sequence_11()
    {
        writer.enabled = true;
        animator.enabled = false;
        writer.ResetWriter();
        writer.StartWriter();
        animator.SetText("<!show><b>TMPWriter</b>\n\n<noparse>For example, you can wait <!wait=2.25>for any given amount of time, or <!delay=0.025>change the speed at which the text is shown.</noparse></!>\n\n<alpha=#CC>For example, you can wait <!wait=2.25>for any given amount of time, or <!delay=0.025>change the speed at which the text is shown.</noparse>\n");
    }

    private void Sequence_12()
    {
        writer.enabled = false;
        animator.enabled = false;
        animator.SetText($"<b>TMPWriter</b>\n\nFor a full overview of all built-in commands and their parameters, see <color=lightblue><u><link=\"{baseLink}/tmpwriter_builtincommands.html\">this page of the docs</link>");
    }

    private void Sequence_14()
    {
        writer.enabled = false;
        animator.enabled = false;
        animator.SetText("<b>Combining them</b>\n\nWhen you have both a writer and an animator on your GameObject, you can animate the show and hide sequence as well.\nShow animation tags are prefixed with a '+',\nhide animation tags with a '-'.");
    }

    private void Sequence_15()
    {
        writer.enabled = true;
        animator.enabled = true;
        animator.SetText("<!show><b>Combining them</b>\n\n<noparse><+fade duration=0.2>First I will fade in, </+><+pivot>then pivot for a bit</+>, <+shake>and then shake for the rest of it.\n<!wait=1><+fade d=0.2><+pivot>And then I'll do all of it!</noparse></!>\n\n<!wait=1><+fade d=0.2>First I will fade in, </+><+pivot>then pivot for a bit</+>, <+shake>and then shake for the rest of it.\n<!wait=1><+fade d=0.2><+pivot>And then I'll do all of it!");
        writer.RestartWriter();
    }

    private void Sequence_16()
    {
        writer.enabled = false;
        animator.enabled = false;
        animator.SetText($"<b>Combining them</b>\n\nFor a full overview of all show / hide animations and their parameters, see <color=lightblue><u><link=\"{baseLink}/tmpanimator_builtinshowhideanimations.html\">this page of the docs</link>");
    }

    private void Sequence_17()
    {
        writer.enabled = false;
        animator.enabled = false;
        animator.SetText($"The full documentation of TMPEffects can be found <color=lightblue><u><link=\"{baseLink}/introduction.html\">here</link>");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComp, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = textComp.textInfo.linkInfo[linkIndex];
            string selectedLink = linkInfo.GetLinkID();
            if (!string.IsNullOrEmpty(selectedLink))
            {
                //Debug.LogFormat("Open link {0}", selectedLink);
                Application.OpenURL(selectedLink);
            }
        }
    }

    private void Update()
    {
        if (animator.isActiveAndEnabled)
        {
            animator.UpdateAnimations(Time.deltaTime);
        }
    }

    public void Previous()
    {
        if (currentSequence == 0) return;
        currentSequence--;
        sequence[currentSequence].Invoke();
    }

    public void Next()
    {
        if (currentSequence == sequence.Length - 1) return;
        currentSequence++;
        sequence[currentSequence].Invoke();
    }

    public void RestartWriter()
    {
        if (writer.isActiveAndEnabled)
        {
            writer.RestartWriter();
        }
    }
}
