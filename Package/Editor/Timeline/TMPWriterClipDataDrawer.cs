using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPEffects.Components;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class ExportToGenericAnimationUtilityWindow : EditorWindow
{
    private string path;

    internal static void Init(string path)
    {
        ExportToGenericAnimationUtilityWindow window =
            EditorWindow.GetWindow<ExportToGenericAnimationUtilityWindow>(true, "Export animation");
        window.path = path;
        window.ShowModalUtility();
    }

    private void OnGUI()
    {
        var rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
        rect.x += 20;
        rect.width -= 40;

        EditorGUILayout.LabelField("You are exporting to a generic animation asset.");
        rect.y += rect.height;
        EditorGUILayout.LabelField("Select the path to save to:");
        rect.y += rect.height;
        EditorGUILayout.LabelField(path);
    }
}

[MenuEntry("Export TMPAnimation/Generic Animation SO")]
public class ExportToGenericAnimation : TimelineAction
{
    public override bool Execute(ActionContext context)
    {
        // Export to generic
        int result = TimelineToAnimationExporter.ExportAsGenericDialog(context);

        if (result == 1)
            return false;
        
        return TimelineToAnimationExporter.ExportAsGeneric(context, result);
    }

    public override ActionValidity Validate(ActionContext context)
    {
        return context.tracks.Any(track => track is TMPMeshModifierTrack)
            ? ActionValidity.Valid
            : ActionValidity.NotApplicable;
    }
}

[MenuEntry("Export TMPAnimation/TMPAnimation Script")]
public class ExportToTMPAnimationScript : TimelineAction
{
    public override bool Execute(ActionContext context)
    {
        // Export to script
        int result = TimelineToAnimationExporter.ExportAsScriptDialog(context);

        if (result == 1)
            return false;
        
        return TimelineToAnimationExporter.ExportAsScript(context, result);
    }

    public override ActionValidity Validate(ActionContext context)
    {
        return context.tracks.Any(track => track is TMPMeshModifierTrack)
            ? ActionValidity.Valid
            : ActionValidity.NotApplicable;
    }
}