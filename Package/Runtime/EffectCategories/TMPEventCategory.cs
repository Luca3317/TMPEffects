using TMPEffects.TextProcessing;
using TMPEffects.Tags;

namespace TMPEffects.EffectCategories
{
    /// <summary>
    /// Category for events.
    /// </summary>
    internal class TMPEventCategory : TMPEffectCategory
    {
        public TMPEventCategory(char prefix) : base(prefix)
        { }

        ///<inheritdoc/>
        public override bool ValidateOpenTag(ParsingUtility.TagInfo tagInfo, out TMPEffectTag data, out int endIndex)
        {
            data = null;
            endIndex = tagInfo.startIndex;
            if (tagInfo.type != ParsingUtility.TagType.Open) return false;
            if (tagInfo.prefix != Prefix) return false;
            TMPEffectTag tagData = new(tagInfo.name, tagInfo.prefix, ParsingUtility.GetTagParametersDict(tagInfo.parameterString));
            data = tagData;
            return true;
        }

        ///<inheritdoc/>
        public override bool ValidateTag(TMPEffectTag tag)
        {
            if (tag.Prefix != Prefix) return false;
            return true;
        }

        ///<inheritdoc/>
        public override bool ValidateTag(ParsingUtility.TagInfo tagInfo)
        {
            if (tagInfo.type != ParsingUtility.TagType.Open) return false;
            if (tagInfo.prefix != Prefix) return false;
            return true;
        }
    }
}