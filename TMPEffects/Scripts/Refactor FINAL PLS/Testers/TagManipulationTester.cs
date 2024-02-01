using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPEffects.Components;
using TMPEffects.TextProcessing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TagManipulationTester : MonoBehaviour
{
    [SerializeField] TMP_InputField tagField;
    [SerializeField] TMP_InputField startField;
    [SerializeField] TMP_InputField endField;
    [SerializeField] TMP_InputField orderField;
    [SerializeField] TMP_Dropdown componentDropdown;
    [SerializeField] TMP_Dropdown actionDropdown;

    [SerializeField] TMPAnimator animator;

    public void Submit()
    {
        string tag = tagField.text;
        int start, end, order;
        if (!ParsingUtility.StringToInt(startField.text, out start)) throw new System.ArgumentException("Start not integer");
        if (!ParsingUtility.StringToInt(endField.text, out end)) throw new System.ArgumentException("End not integer");
        if (!ParsingUtility.StringToInt(orderField.text, out order)) throw new System.ArgumentException("Order not integer");

        Component comp;
        string compText = componentDropdown.options[componentDropdown.value].text;
        if (compText == "Writer") comp = Component.Writer;
        else if (compText == "Animator") comp = Component.Animator;
        else throw new System.ArgumentException("Component not valid");

        Action action;
        string actionText = actionDropdown.options[actionDropdown.value].text;
        if (actionText == "Insert") action = Action.Add;
        else if (actionText == "Remove") action = Action.Remove;
        else throw new System.ArgumentException("Action not valid");

        ParsingUtility.TagInfo info = new ParsingUtility.TagInfo();
        if (!ParsingUtility.TryParseTag(tag, 0, tag.Length - 1, ref info, ParsingUtility.TagType.Open)) throw new System.ArgumentException("Tag not well formed");

        var dict = ParsingUtility.GetTagParametersDict(tag);

        // TODO support writer
        if (action == Action.Add)
        {
            Debug.Log("Will add");
            if (!animator.Tags.TryAdd(new EffectTag(info.name, info.prefix, dict), new EffectTagIndices(start, end, order)))
            {
                throw new System.ArgumentException("Failed to add");
            }
            else
                Debug.Log("Successfull!");
        }
        else if (action == Action.Remove)
        {
            Debug.Log("Will remove");
            if (!animator.Tags.RemoveAt(start, order))
            {
                throw new System.ArgumentException("Failed to remove");
            }
            else
                Debug.Log("Successful!");
        }
    }

    public void PrintTags()
    {
        ClearLog();

        foreach (var tag in animator.Tags)
        {
            Debug.Log(tag.Tag.Name + ": " + tag.Indices.StartIndex + " - " + tag.Indices.OrderAtIndex + " : " + tag.Indices.EndIndex);
        }

        Debug.Break();
    }

    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    private enum Component
    {
        Writer = 0,
        Animator = 10
    }

    private enum Action
    {
        Add = 0,
        Remove = 10
    }
}
