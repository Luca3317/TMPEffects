using System.Collections.Generic;
using TMPEffects.TMPCommands;
using TMPEffects.Databases;
using TMPEffects.TextProcessing;
using TMPEffects.Tags;

/*
 * TODO the ValidateTag(TagInfo, out EffectTag) methods need to handle open / close tags differently
 * 
 */
namespace TMPEffects.EffectCategories
{
    ///<inheritdoc/>
    public abstract class TMPEffectCategory<TEffect> : TMPEffectCategory, ITMPEffectDatabase<TEffect>
    {
        public TMPEffectCategory(char prefix) : base(prefix)
        { }

        ///<inheritdoc/>
        public abstract bool ContainsEffect(string name);

        ///<inheritdoc/>
        public abstract TEffect GetEffect(string name);
    }

    /// <summary>
    /// Base class for all effect categories.
    /// </summary>
    public abstract class TMPEffectCategory : ITMPTagValidator, ITMPPrefixSupplier
    {
        /// <summary>
        /// The prefix associated with this category.
        /// </summary>
        public char Prefix => prefix;

        protected readonly char prefix;

        public TMPEffectCategory(char prefix)
        {
            this.prefix = prefix;
        }

        ///<inheritdoc/>
        public abstract bool ValidateTag(ParsingUtility.TagInfo tagInfo, out EffectTag data);

        ///<inheritdoc/>
        public abstract bool ValidateTag(EffectTag tag);

        ///<inheritdoc/>
        public bool ValidateTag(ParsingUtility.TagInfo tagInfo)
        {
            return ValidateTag(tagInfo, out _);
        }
    }
}
