using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(TMPAnimator))]
[TrackClipType(typeof(TMPMeshModifierClip))]
public class TMPMeshModifierTrack : TMPEffectsTrack
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<TMPMeshModifierTrackMixer>.Create(graph, inputCount);
    }
}