using System.Collections.Generic;

namespace TMPEffects.Tags
{
    public class TMPCommandTag : TMPEffectTag
    {
        public TMPCommandTag(string name, int startIndex, Dictionary<string, string> parameters) : base(name, startIndex, parameters)
        { }
    }
}