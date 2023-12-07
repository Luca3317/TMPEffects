using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "testhideanim", menuName = "TMPEffects/testhideanim")]
public class testHideAnim : TMPHideAnimation
{
    public override void Animate(ref CharData cData, AnimationContext context)
    {
        float t = (Time.time - cData.stateTime) * 2.5f;
        Vector3 center = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            center += cData.initialMesh.GetPosition(i);
        }
        center /= 4;

        for (int i = 0; i < 4; i++)
        {
            Vector3 pos = Vector3.Lerp(cData.initialMesh.GetPosition(i), center, t);
            cData.currentMesh.SetPosition(i, pos);
        }

        if (t >= 1)
        {
            cData.visibilityState = CharData.VisibilityState.Hidden;
        }
    }

    public override void ResetVariables()
    {
    }

    public override void SetParameter<T>(string name, T value)
    {

    }

    public override void SetParameters(Dictionary<string, string> parameters)
    {

    }

    public override bool ValidateParameters(Dictionary<string, string> parameters)
    {
        return true;
    }
}
