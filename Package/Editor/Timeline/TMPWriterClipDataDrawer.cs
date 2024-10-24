using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPEffects.Components;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;


[MenuEntry("Unpack Generic Animation")]
public class UnpackGenericAnimation : TimelineAction
{
    public override bool Execute(ActionContext context)
    {
        bool result = TimelineUtility.UnpackGenericDialog(context);
        
        if (!result)
            return false;
        
        return TimelineUtility.UnpackGeneric(context);
    }

    public override ActionValidity Validate(ActionContext context)
    {
        return context.clips.Any(clip => clip.asset is TMPAnimationClip)
            ? ActionValidity.Valid
            : ActionValidity.NotApplicable;
    }
}

[MenuEntry("Export TMPAnimation/Generic Animation SO")]
public class ExportToGenericAnimation : TimelineAction
{
    public override bool Execute(ActionContext context)
    {
        // Export to generic
        int result = TimelineUtility.ExportAsGenericDialog(context);

        if (result == 1)
            return false;
        
        return TimelineUtility.ExportAsGeneric(context, result);
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
        int result = TimelineUtility.ExportAsScriptDialog(context);

        if (result == 1)
            return false;
        
        return TimelineUtility.ExportAsScript(context, result);
    }

    public override ActionValidity Validate(ActionContext context)
    {
        return context.tracks.Any(track => track is TMPMeshModifierTrack)
            ? ActionValidity.Valid
            : ActionValidity.NotApplicable;
    }
}