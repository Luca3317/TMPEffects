using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new WhatToCallTHisShowANimation", menuName = "TMPEffects/Show Animations/WhatToCallThis")]
    public class WhatToCallThisShowAnimation : TMPShowAnimation
    {
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseOutBack();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            float t = Mathf.Lerp(0, 1, context.animatorContext.PassedTime - context.animatorContext.StateTime(cData));
            float t2 = curve.Evaluate(t);

            if (t == 1) context.FinishAnimation(cData);

            Vector3 angle = Vector3.LerpUnclamped(new Vector3(90, 0, 0), Vector3.zero, t2);

            cData.SetRotation(Quaternion.Euler(angle));
            AddPivotDeltaRaw(Vector3.down * (cData.mesh.GetVertex(1) - cData.mesh.GetVertex(0)).magnitude / 2, cData, ref context);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            return;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            return true;
        }
    }
}
