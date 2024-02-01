using System.Collections.Generic;

namespace TMPEffects
{
    public interface ITMPAnimation
    {
        public void Animate(ref CharData charData, IAnimationContext context);

        public void SetParameters(IDictionary<string, string> parameters);
        public bool ValidateParameters(IDictionary<string, string> parameters);
        public void ResetParameters();

        public IAnimationContext GetNewContext();
    }
}