using System.Collections.Generic;
using TMPEffects.CharacterData;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    public class DummyHideAnimation : TMPHideAnimation
    {
        public override void Animate(CharData cData, IAnimationContext context)
        {
            context.FinishAnimation(cData);
        }

        public override object GetNewCustomData()
        {
            return null;
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