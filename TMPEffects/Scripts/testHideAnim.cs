using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "testhideanim", menuName = "TMPEffects/testhideanim")]
public class testHideAnim : TMPHideAnimation
{
    public override void Animate(ref CharData cData, AnimationContext context)
    {
        Debug.Log("TestHideAnim");
        for (int i = 0; i < 4; i++)
        {

        }

        if (Time.time - cData.stateTime > 1000)
        {
            Debug.Log("Done w/ hide anim after " + (Time.time - cData.stateTime));
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
