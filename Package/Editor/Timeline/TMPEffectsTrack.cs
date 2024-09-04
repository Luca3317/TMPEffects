using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TMPEffects.Editor.Timeline
{
    [TrackBindingType(typeof(TMPAnimation))]
    [TrackClipType(typeof(TMPEffectsClip))]
    public class TMPEffectsTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<TMPEffectsTrackMixer>.Create(graph, inputCount);
        } 
    }
}