using System.Collections.Generic;
using TMPEffects.CharacterData;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    public class DummyHideAnimation : TMPHideAnimation
    {
        public override void Animate(CharData cData, IAnimationContext context)
        {
            for (int i = 0; i < 4; i++)
            {
                AnimationUtility.SetVertexRaw(i, cData.info.initialPosition, cData, ref context);
            }

            context.FinishAnimation(cData);
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