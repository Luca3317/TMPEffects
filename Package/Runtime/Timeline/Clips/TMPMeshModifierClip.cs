using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[DisplayName("TMPEffects Clip/TMPMeshModifier Clip")]
public class TMPMeshModifierClip : TMPEffectsClip, ITimelineClipAsset
{
    [NonSerialized] public TimelineClip Clip = null;
    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Extrapolation; }
    }
 
    private ExposedReference<PlayableDirector> director;
    [SerializeField] private TimelineAnimationStep step;
    
    public TimelineAnimationStep Step => step;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TMPMeshModifierBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.GetDuration();
        behaviour.Step = step;
        behaviour.Clip = Clip;
        
        PlayableDirector director = (PlayableDirector)graph.GetResolver();
        playable.GetGraph().GetResolver().SetReferenceValue(this.director.exposedName, director);
        // behaviour.Initialize(this.director.Resolve(graph.GetResolver()));
        
        return playable;
    }

}
