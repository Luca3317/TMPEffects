using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using UnityEngine;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    public class DummyShowAnimation : TMPShowAnimation
    {
        public override void Animate(CharData cData, IAnimationContext context)
        {
            for (int i = 0; i < 4; i++)
            {
                cData.SetVertex(i, cData.mesh.initial.GetPosition(i));
            }

            context.FinishAnimation(cData.info.index);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            return true;
        }
    }
}