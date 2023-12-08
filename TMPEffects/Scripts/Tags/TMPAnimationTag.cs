using System.Collections.Generic;

namespace TMPEffects.Tags
{
    public class TMPAnimationTag : TMPEffectTag
    {
        public TMPAnimationTag(string name, int startIndex, Dictionary<string, string> parameters) : base(name, startIndex, parameters)
        { }
    }
}