using TMPEffects.Databases;
using TMPEffects.TextProcessing;
using TMPEffects.TMPAnimations;
using TMPEffects.Tags;
using System.Diagnostics;
using TMPEffects.Components.Animator;

namespace TMPEffects.EffectCategories
{
    /// <summary>
    /// Category for animations.
    /// </summary>
    public class TMPAnimationCategory : TMPEffectCategory<ITMPAnimation>
    {
        private ITMPEffectDatabase<ITMPAnimation> database;
        private IAnimatorContext context;

        public TMPAnimationCategory(char prefix, ITMPEffectDatabase<ITMPAnimation> database, IAnimatorContext context) : base(prefix)
        {
            this.database = database;
            this.context = context;
        }

        ///<inheritdoc/>
        public override bool ContainsEffect(string name) => database.ContainsEffect(name);

        ///<inheritdoc/>
        public override ITMPAnimation GetEffect(string name) => database.GetEffect(name);

        ///<inheritdoc/>
        public override bool ValidateOpenTag(ParsingUtility.TagInfo tagInfo, out TMPEffectTag data, out int endIndex)
        {
            data = null;
            endIndex = -1;
            if (tagInfo.type != ParsingUtility.TagType.Open) throw new System.ArgumentException(nameof(tagInfo.type));

            if (tagInfo.prefix != Prefix) return false;
            if (database == null || !database.ContainsEffect(tagInfo.name)) return false;

            var param = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);

            // TODO HUH! WTF TO DO ABOUT THIS
            if (!database.GetEffect(tagInfo.name).ValidateParameters(param, context)) return false;

            TMPEffectTag tag = new TMPEffectTag(tagInfo.name, tagInfo.prefix, param);
            data = tag;
            return true;
        }

        ///<inheritdoc/>
        public override bool ValidateTag(TMPEffectTag tag)
        {
            if (tag.Prefix != Prefix) return false;
            if (database == null || !database.ContainsEffect(tag.Name)) return false;

            // TODO HUH! WTF TO DO ABOUT THIS
            if (!database.GetEffect(tag.Name).ValidateParameters(tag.Parameters, context)) return false;

            return true;
        }

        ///<inheritdoc/>
        public override bool ValidateTag(ParsingUtility.TagInfo tagInfo)
        {
            if (tagInfo.prefix != Prefix) return false;
            if (database == null || !database.ContainsEffect(tagInfo.name)) return false;

            if (tagInfo.type == ParsingUtility.TagType.Open)
            {
                // TODO HUH! WTF TO DO ABOUT THIS
                var param = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
                if (!database.GetEffect(tagInfo.name).ValidateParameters(param, context)) return false;
            }

            return true;
        }
    }
}