using System.Linq;
using UnityEditor.Timeline.Actions;


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
        return context.clips.Any(clip => clip.asset is TMPAnimationClip)
            ? ActionValidity.Valid
            : ActionValidity.NotApplicable;
    }
}

[MenuEntry("Export TMPAnimation/Generic Animation SO")]
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

[MenuEntry("Export TMPAnimation/TMPAnimation Script")]
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