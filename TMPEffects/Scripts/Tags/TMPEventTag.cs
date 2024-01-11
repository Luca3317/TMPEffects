using System;
using System.Collections.Generic;
using TMPEffects.TextProcessing;

namespace TMPEffects.Tags
{
    public class TMPEventTag : TMPEffectTag
    {
        public readonly bool invokeOnSkip = false;

        public TMPEventTag(int index, string name, Dictionary<string, string> parameters) : base(name, index, parameters)
        {
            // TODO Might move this into writer
            if (parameters != null)
            {
                foreach(var key in parameters.Keys)
                {
                    switch (key.ToLower())
                    {
                        case "onskip":
                        case "invokeonskip":
                            bool tmp = false;
                            if (ParsingUtility.StringToBool(parameters[key], out tmp)) invokeOnSkip = tmp;
                            break;
                    }
                }
            }
        }
    }
}
