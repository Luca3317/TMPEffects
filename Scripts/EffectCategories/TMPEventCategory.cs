using TMPEffects.TextProcessing;
using TMPEffects.TMPEvents;
using TMPEffects.Tags;

namespace TMPEffects.EffectCategories
{
    /// <summary>
    /// Category for events.
    /// </summary>
    public class TMPEventCategory : TMPEffectCategory
    {
        public TMPEventCategory(char prefix) : base(prefix)
        { }

        ///<inheritdoc/>
        public override bool ValidateTag(ParsingUtility.TagInfo tagInfo, out EffectTag data)
        {
            data = null;
            if (tagInfo.prefix != Prefix) return false;
            EffectTag tagData = new(tagInfo.name, tagInfo.prefix, ParsingUtility.GetTagParametersDict(tagInfo.parameterString));
            data = tagData;
            return true;
        }

        ///<inheritdoc/>
        public override bool ValidateTag(EffectTag tag)
        {
            throw new System.NotImplementedException();
        }
    }
}