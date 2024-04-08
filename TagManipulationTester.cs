using System.Reflection;
using TMPEffects.Components;
using TMPEffects.TextProcessing;
using TMPro;
using UnityEngine;
using TMPEffects.Tags;

public class TagManipulationTester : MonoBehaviour
{
    [SerializeField] TMP_InputField tagField;
    [SerializeField] TMP_InputField startField;
    [SerializeField] TMP_InputField endField;
    [SerializeField] TMP_InputField orderField;
    [SerializeField] TMP_Dropdown componentDropdown;
    [SerializeField] TMP_Dropdown actionDropdown;

    [SerializeField] TMPAnimator animator;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    if (Time.timeScale == 0)
        //    {
        //        Time.timeScale = 1.0f;
        //    }
        //    else
        //    {
        //        Time.timeScale = 0.0f;
        //    }
        //}
    }

    public void Submit()
    {
        string tag = tagField.text;
        int start, end, orderT;
        int? order;
        if (!ParsingUtility.StringToInt(startField.text, out start)) throw new System.ArgumentException("Start not integer");
        if (!ParsingUtility.StringToInt(endField.text, out end)) throw new System.ArgumentException("End not integer");
        if (!ParsingUtility.StringToInt(orderField.text, out orderT))
        {
            order = null;
        }
        else order = orderT;

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
            if (order == null)
            {
                var t = new TMPEffectTag(info.name, info.prefix, dict);
                var ti = new TMPEffectTagIndices(start, end, 0);
                if (!animator.Tags.TryAdd(t, ti))
                {
                    throw new System.ArgumentException("Failed to add");
                }
                else
                    Debug.Log("Successfully added tag! Name: " + t.Name + "; Start: " + ti.StartIndex + " Endindex: " + ti.EndIndex + " Order: " + ti.OrderAtIndex);
            }
            else
            {
                var t = new TMPEffectTag(info.name, info.prefix, dict);
                var ti = new TMPEffectTagIndices(start, end, order.Value);
                if (!animator.Tags.TryAdd(t, ti))
                {
                    throw new System.ArgumentException("Failed to add");
                }
                else
                    Debug.Log("Successfully added tag! Name: " + t.Name + "; Start: " + ti.StartIndex + " Endindex: " + ti.EndIndex + " Order: " + ti.OrderAtIndex);
            }

        }
        else if (action == Action.Remove)
        {
            if (order == null)
            {
                if (!animator.Tags.RemoveAt(start))
                {
                    throw new System.ArgumentException("Failed to remove");
                }
                else
                    Debug.Log("Successful!");
            }
            else
            {
                if (!animator.Tags.RemoveAt(start, order))
                {
                    throw new System.ArgumentException("Failed to remove");
                }
                else
                    Debug.Log("Successful!");
            }
        }
    }

    public void PrintTags()
    {
        //ClearLog();

        foreach (var tag in animator.Tags)
        {
            Debug.Log(tag.Tag.Name + ": " + tag.Indices.StartIndex + " - " + tag.Indices.OrderAtIndex + " : " + tag.Indices.EndIndex);
        }

        Debug.Break();
    }

    public void ClearLog()
    {
#if UNITY_EDITOR
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
#endif
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
