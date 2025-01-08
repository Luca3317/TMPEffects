using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPEffects.Modifiers;
using TMPEffects.Timeline;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;

[MenuEntry("Copy Initial Modifiers From Next Clip Into Modifiers")]
public class CopyInitialModifiersOfNextClipIntoModifiersClipAction : ClipAction
{
    public override bool Execute(IEnumerable<TimelineClip> clips)
    {
        var clip = clips.FirstOrDefault();
        var meshModClip = clip.asset as TMPMeshModifierClip;

        var director = TimelineEditor.inspectedDirector;
        if (director == null)
            return false;

        var step = meshModClip.Step.Step;
        var closest = double.MaxValue;
        TimelineClip prevClip = null;
        var track = clip.GetParentTrack();
        foreach (var otherclip in track.GetClips())
        {
            if (clip == otherclip) continue;
            if (otherclip.start < clip.start) continue;
            if (otherclip.start > closest) continue;
            if (otherclip.asset is not TMPMeshModifierClip) continue;

            closest = otherclip.start;
            prevClip = otherclip;
        }
        
        if (prevClip == null) return false;
        
        var nextStep = (prevClip.asset as TMPMeshModifierClip).Step.Step;
        step.modifiers = new EditorFriendlyCharDataModifiers(nextStep.initModifiers);

        return true;
    }

    public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
    {
        var clipslist = clips.ToList();
        if (clipslist.Count != 1)
            return ActionValidity.NotApplicable;

        if (TimelineEditor.inspectedDirector == null)
            return ActionValidity.NotApplicable;

        var clip = clipslist.FirstOrDefault();
        if (clip.asset is not TMPMeshModifierClip)
            return ActionValidity.NotApplicable;

        var track = clip.GetParentTrack();
        foreach (var otherclip in track.GetClips())
        {
            if (clip == otherclip) continue;
            if (otherclip.start > clip.start)
                return ActionValidity.Valid;
        }

        return ActionValidity.Invalid;
    }
}