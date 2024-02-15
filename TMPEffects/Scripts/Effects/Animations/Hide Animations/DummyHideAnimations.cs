using System.Collections.Generic;

namespace TMPEffects.Animations
{
    public class DummyHideAnimation : TMPShowAnimation
    {
        public override void Animate(ref CharData cData, IAnimationContext context)
        {
            for (int i = 0; i < 4; i++)
            {
                EffectUtility.SetVertexRaw(i, cData.info.initialPosition, ref cData, ref context);
            }

            cData.SetVisibilityState(CharData.VisibilityState.Hidden, context.animatorContext.passedTime);
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
