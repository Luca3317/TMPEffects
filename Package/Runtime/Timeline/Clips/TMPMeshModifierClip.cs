using System.Collections;
using System.Collections.Generic;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TMPMeshModifierClip : PlayableAsset, ITimelineClipAsset
{
    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public ExposedReference<PlayableDirector> director;
    [SerializeField] private TimelineAnimationStep step;
    
    public TimelineAnimationStep Step => step;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TMPMeshModifierBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.GetDuration();
        behaviour.Step = step;
        
        PlayableDirector director = (PlayableDirector)graph.GetResolver();
        playable.GetGraph().GetResolver().SetReferenceValue(this.director.exposedName, director);
        behaviour.Initialize(this.director.Resolve(graph.GetResolver()));
        
        return playable;
    }

}
