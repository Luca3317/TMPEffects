using System.Collections.Generic;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    public class DummyHideAnimation : TMPShowAnimation
    {
        public override void Animate(CharData cData, IAnimationContext context)
        {
            for (int i = 0; i < 4; i++)
            {
                AnimationUtility.SetVertexRaw(i, cData.info.initialPosition, cData, ref context);
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
