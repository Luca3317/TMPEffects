using System.Collections.Generic;

namespace TMPEffects
{
    public interface ITMPAnimation
    {
        public void Animate(ref CharData charData, AnimationContext context);

        public void SetParameters(Dictionary<string, string> parameters);
        public bool ValidateParameters(Dictionary<string, string> parameters);
        public void ResetParameters();
    }
}