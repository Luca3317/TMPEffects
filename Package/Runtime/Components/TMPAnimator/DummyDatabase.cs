using System.Collections.Generic;
using TMPEffects.Databases;
using TMPEffects.TMPAnimations;

namespace TMPEffects.Components.Animator
{
    // Mock database used to use AnimationCacher with dummyshow/hide
    internal struct DummyDatabase : ITMPEffectDatabase<ITMPAnimation>
    {
        private string name;
        private ITMPAnimation animation;

        public DummyDatabase(string name, ITMPAnimation animation)
        {
            this.name = name;
            this.animation = animation;
        }

        public bool ContainsEffect(string name)
        {
            return name == this.name;
        }

        public ITMPAnimation GetEffect(string name)
        {
            if (name == this.name) return this.animation;
            throw new KeyNotFoundException(name);
        }
    }
}
