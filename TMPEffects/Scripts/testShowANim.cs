using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "testshowanim", menuName ="TMPEffects/testshowanim")]
public class testShowANim : TMPShowAnimation
{
    public override void Animate(ref CharData cData, AnimationContext context)
    {
        float t = (Time.time - cData.stateTime) * 10f;
        Vector3 center = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            center += cData.initialMesh.GetPosition(i);
        }
        center /= 4;

        for (int i = 0; i < 4; i++)
        {
            Vector3 pos = Vector3.Lerp(center, cData.initialMesh.GetPosition(i), t);
            cData.currentMesh.SetPosition(i, pos);

            cData.currentMesh.SetColor(i, Color.red);
        }

        if (t >= 1)
        {
            cData.visibilityState = CharData.VisibilityState.Shown;
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

