using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "testshowanim", menuName ="TMPEffects/testshowanim")]
public class testShowANim : TMPShowAnimation
{
    public override void Animate(ref CharData cData, AnimationContext context)
    {
        Debug.Log("TestShowAnim");
        for (int i = 0; i < 4; i++)
        {

        }

        if (Time.time - cData.stateTime > 1000)
        {
            Debug.Log("Done w/ show anim after " + (Time.time - cData.stateTime));
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

