using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.Components
{
    public abstract class TMPSceneAnimationBase : MonoBehaviour, ITMPAnimation
    {
        public abstract void Animate(ref CharData charData, IAnimationContext context);
        public abstract IAnimationContext GetNewContext();

        public abstract void ResetParameters();

        public abstract void SetParameters(IDictionary<string, string> parameters);
        public abstract bool ValidateParameters(IDictionary<string, string> parameters);

        protected class DefaultSceneAnimationContext : IAnimationContext
        {
            public AnimatorContext animatorContext { get => settings; set => settings = value; }
            private AnimatorContext settings;

            public SegmentData segmentData { get; set; }

            public void ResetContext() { }
        }
    }
}
