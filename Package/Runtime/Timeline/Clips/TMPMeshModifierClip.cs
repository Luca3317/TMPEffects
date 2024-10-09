using System.Collections;
using System.Collections.Generic;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;

public class TMPMeshModifierClip : PlayableAsset
{
    [SerializeField] private GenericAnimation.AnimationStep step;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TMPMeshModifierBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.Step = step;
        return playable;
    }
}
