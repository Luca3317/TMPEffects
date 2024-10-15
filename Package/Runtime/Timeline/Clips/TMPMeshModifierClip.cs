using System.Collections;
using System.Collections.Generic;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TMPMeshModifierClip : PlayableAsset
{
    public ExposedReference<PlayableDirector> director;
    [SerializeField] private TimelineAnimationStep step;

    public TimelineAnimationStep Step => step;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TMPMeshModifierBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.Step = step;
        behaviour.Initialize(director.Resolve(graph.GetResolver()));
        return playable;
    }
}
