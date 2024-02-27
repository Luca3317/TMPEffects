using System.Collections.Generic;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    public class DummyHideAnimation : TMPShowAnimation
    {
        public override void Animate(ref CharData cData, IAnimationContext context)
        {
            for (int i = 0; i < 4; i++)
            {
                AnimationUtility.SetVertexRaw(i, cData.info.initialPosition, ref cData, ref context);
            }

            cData.SetVisibilityState(VisibilityState.Hidden, context.animatorContext.PassedTime);
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
