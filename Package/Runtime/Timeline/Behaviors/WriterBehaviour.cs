using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine;
using UnityEngine.Playables;

public class WriterBehaviour : PlayableBehaviour
{
    private TMPWriter writer;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        writer = info.output.GetUserData() as TMPWriter;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
    }
}
