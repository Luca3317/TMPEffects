using System.Collections.Generic;
using TMPEffects;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new ExplodeAnimation", menuName = "TMPEffects/Animations/Explode")]
    public class ExplodeAnimation : TMPAnimation
    {
        public override void Animate(ref CharData cData, IAnimationContext context)
        {
            Vector3 center = (context.segmentData.max + context.segmentData.min) / 2;

            float t = Mathf.Sin(context.animatorContext.PassedTime) / 2 + 0.5f;
            Vector3 TL = Vector3.Lerp(center, cData.mesh.initial.vertex_TL.position, t);
            Vector3 TR = Vector3.Lerp(center, cData.mesh.initial.vertex_TR.position, t);
            Vector3 BL = Vector3.Lerp(center, cData.mesh.initial.vertex_BL.position, t);
            Vector3 BR = Vector3.Lerp(center, cData.mesh.initial.vertex_BR.position, t);

            SetVertexRaw(0, BL, ref cData, ref context);
            SetVertexRaw(1, TL, ref cData, ref context);
            SetVertexRaw(2, TR, ref cData, ref context);
            SetVertexRaw(3, BR, ref cData, ref context);
        }

        public override void ResetParameters()
        {
        }

        public override void SetParameters(IDictionary<string, string> parameters)
        {
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            return true;
        }
    }
}

