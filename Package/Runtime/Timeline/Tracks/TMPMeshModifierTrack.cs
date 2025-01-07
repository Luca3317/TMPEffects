using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(TMPAnimator))]
[TrackClipType(typeof(TMPMeshModifierClip))]
[DisplayName("TMPEffects/TMPMeshModifier Track")]
public class TMPMeshModifierTrack : TMPEffectsTrack
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var clips = GetClips();
        foreach (var clip in clips)
        {
            var currClip = clip.asset as TMPMeshModifierClip;
            currClip.Clip = clip;
        }
        
        return ScriptPlayable<TMPMeshModifierTrackMixer>.Create(graph, inputCount);
    }
}