using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using UnityEngine;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    public class DummyShowAnimation : TMPShowAnimation
    {
        public override void Animate(CharData cData, IAnimationContext context)
        {
            Debug.Log("Dummy show");
            for (int i = 0; i < 4; i++)
            {
                cData.SetVertex(i, cData.mesh.initial.GetPosition(i));
            }

            context.FinishAnimation(cData.info.index);
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