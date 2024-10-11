using TMPEffects.TMPAnimations;
using UnityEngine.Playables;

public class TMPMeshModifierBehaviour : PlayableBehaviour
{
    public GenericAnimation.AnimationStep Step;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        var meshModifier = info.output.GetUserData() as TMPMeshModifier;
        meshModifier.SetModifiers( Step.charModifiers );
    }
}
