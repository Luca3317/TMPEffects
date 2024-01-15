using System.Collections.Generic;

namespace TMPEffects.Tags
{
    public class TMPAnimationTag : TMPEffectTag
    {
        public TMPAnimationTag(string name, int startIndex, int order, Dictionary<string, string> parameters) : base(name, startIndex, order, parameters)
        { }
    }
}