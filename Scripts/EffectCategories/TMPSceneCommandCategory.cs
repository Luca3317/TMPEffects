using System.Collections.Generic;
using TMPEffects.TMPCommands;
using TMPEffects.TextProcessing;
using TMPEffects.Tags;

namespace TMPEffects.EffectCategories
{
    // TODO Needed? COuld just merge with tmpcommandcategory
    /// <summary>
    /// Category for scene commands.
    /// </summary>
    public class TMPSceneCommandCategory : TMPEffectCategory<ITMPCommand>
    {
        private Dictionary<string, SceneCommand> tags;

        public TMPSceneCommandCategory(char prefix, Dictionary<string, SceneCommand> tags) : base(prefix)
        {
            this.tags = tags;
        }

        ///<inheritdoc/>
        public override bool ContainsEffect(string name) => tags.ContainsKey(name);
        ///<inheritdoc/>
        public override ITMPCommand GetEffect(string name) => tags[name];

        ///<inheritdoc/>
        public override bool ValidateTag(ParsingUtility.TagInfo tagInfo, out EffectTag data)
        {
            data = null;
            if (!tags.ContainsKey(tagInfo.name)) return false;
            if (tagInfo.type == ParsingUtility.TagType.Open || tags[tagInfo.name].TagType != TagType.Empty)
            {
                var param = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
                EffectTag tag = new EffectTag(tagInfo.name, tagInfo.prefix, param);
                data = tag;
                return true;
            }

            return false;
        }

        ///<inheritdoc/>
        public override bool ValidateTag(EffectTag tag)
        {
            throw new System.NotImplementedException();
        }
    }
}
