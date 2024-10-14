using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;

public class TMPMeshModifierBehaviour : PlayableBehaviour
{
    public GenericAnimation.AnimationStep Step;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log("PLAY!");
        var meshModifier = info.output.GetUserData() as TMPMeshModifier;
        meshModifier.SetModifiers( Step.charModifiers );
        meshModifier.StartApplying();
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        var meshModifier = info.output.GetUserData() as TMPMeshModifier;
        meshModifier.StopApplying();
    }
}
