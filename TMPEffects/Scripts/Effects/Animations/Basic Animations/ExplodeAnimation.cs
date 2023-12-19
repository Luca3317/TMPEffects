using System.Collections;
using System.Collections.Generic;
using TMPEffects;
using UnityEngine;

[CreateAssetMenu(fileName ="new ExplodeAnimation", menuName ="TMPEffects/Animations/Explode")]
public class ExplodeAnimation : TMPAnimation
{
    public override void Animate(ref CharData cData, ref IAnimationContext context)
    {
        Vector3 center = (context.segmentData.max + context.segmentData.min) / 2;

        float t =  Mathf.Sin(context.animatorContext.passedTime) / 2 + 0.5f;
        Vector3 TL = Vector3.Lerp(center, cData.info.initialMesh.vertex_TL.position, t);
        Vector3 TR = Vector3.Lerp(center, cData.info.initialMesh.vertex_TR.position, t);
        Vector3 BL = Vector3.Lerp(center, cData.info.initialMesh.vertex_BL.position, t);
        Vector3 BR = Vector3.Lerp(center, cData.info.initialMesh.vertex_BR.position, t);

        EffectUtility.SetVertexRaw(0, BL, ref cData, ref context);
        EffectUtility.SetVertexRaw(1, TL, ref cData, ref context);
        EffectUtility.SetVertexRaw(2, TR, ref cData, ref context);
        EffectUtility.SetVertexRaw(3, BR, ref cData, ref context);
    }

    public override void ResetParameters()
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
