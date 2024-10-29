using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    public class DummyHideAnimation : TMPHideAnimation
    {
        public override void Animate(CharData cData, IAnimationContext context)
        {
            context.FinishAnimation(cData);
        }

        public override object GetNewCustomData(IAnimationContext context)
        {
            return null;
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters,
            IAnimationContext context)
        {
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters, IAnimatorContext context)
        {
            return true;
        }
    }
}