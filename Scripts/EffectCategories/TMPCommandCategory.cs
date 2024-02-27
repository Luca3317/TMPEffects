using TMPEffects.Databases;
using TMPEffects.TextProcessing;
using TMPEffects.TMPCommands;
using TMPEffects.Tags;

namespace TMPEffects.EffectCategories
{
    /// <summary>
    /// Category for commands.
    /// </summary>
    public class TMPCommandCategory : TMPEffectCategory<ITMPCommand>
    {
        private ITMPEffectDatabase<ITMPCommand> database;

        public TMPCommandCategory(char prefix, ITMPEffectDatabase<ITMPCommand> database) : base(prefix)
        {
            this.database = database;
        }

        ///<inheritdoc/>
        public override bool ContainsEffect(string name) => database.ContainsEffect(name);

        ///<inheritdoc/>
        public override ITMPCommand GetEffect(string name) => database.GetEffect(name);

        ///<inheritdoc/>
        public override bool ValidateOpenTag(ParsingUtility.TagInfo tagInfo, out EffectTag data)
        {
            data = null;
            if (tagInfo.prefix != Prefix) return false;
            if (!database.ContainsEffect(tagInfo.name)) return false;

            var param = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
            EffectTag tag = new EffectTag(tagInfo.name, tagInfo.prefix, param);
            data = tag;
            return true;
        }

        ///<inheritdoc/>
        public override bool ValidateTag(EffectTag tag)
        {
            if (tag.Prefix != Prefix) return false;
            if (database == null || !database.ContainsEffect(tag.Name)) return false;
            return true;
        }

        ///<inheritdoc/>
        public override bool ValidateTag(ParsingUtility.TagInfo tagInfo)
        {
            if (tagInfo.prefix != Prefix) return false;
            if (database == null || !database.ContainsEffect(tagInfo.name)) return false;
            return true;
        }
    }
}