using System.Linq;
using TMPEffects.Timeline;
using TMPEffects.TMPAnimations;
using UnityEditor.Timeline.Actions;
using UnityEngine;


[MenuEntry("Unpack Generic Animation")]
internal class UnpackGenericAnimation : TimelineAction
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
        return context.clips.Any(clip => clip.asset is TMPAnimationClip animationClip && animationClip.animation is IGenericAnimation)
            ? ActionValidity.Valid
            : ActionValidity.NotApplicable;
    }
}

[MenuEntry("Export TMPAnimation/Asset")]
internal class ExportToGenericAnimation : TimelineAction
{
    public override bool Execute(ActionContext context)
    {
        ExportToGenericUtilityWindow.ShowWindow(context);
        return true;
    }

    public override ActionValidity Validate(ActionContext context)
    {
        return context.tracks.Any(track => track is TMPMeshModifierTrack)
            ? ActionValidity.Valid
            : ActionValidity.NotApplicable;
    }
}

[MenuEntry("Export TMPAnimation/Script")]
internal class ExportToTMPAnimationScript : TimelineAction
{
    public override bool Execute(ActionContext context)
    {
        ExportToTMPAnimationScriptUtilityWindow.ShowWindow(context);
        return true;
    }

    public override ActionValidity Validate(ActionContext context)
    {
        return context.tracks.Any(track => track is TMPMeshModifierTrack)
            ? ActionValidity.Valid
            : ActionValidity.NotApplicable;
    }
}
