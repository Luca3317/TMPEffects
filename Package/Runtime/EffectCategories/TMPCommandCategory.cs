using TMPEffects.Components.Writer;
using TMPEffects.Databases;
using TMPEffects.TextProcessing;
using TMPEffects.TMPCommands;
using TMPEffects.Tags;

namespace TMPEffects.EffectCategories
{
    /// <summary>
    /// Category for commands.
    /// </summary>
    internal class TMPCommandCategory : TMPEffectCategory<ITMPCommand>
    {
        private ITMPEffectDatabase<ITMPCommand> database;
        private ITMPKeywordDatabase keywordDatabase;

        public TMPCommandCategory(char prefix, ITMPEffectDatabase<ITMPCommand> database, ITMPKeywordDatabase keywordDatabase) : base(prefix)
        {
            this.database = database;
            this.keywordDatabase = keywordDatabase;
        }

        ///<inheritdoc/>
        public override bool ContainsEffect(string name) => database.ContainsEffect(name);

        ///<inheritdoc/>
        public override ITMPCommand GetEffect(string name) => database.GetEffect(name);

        ///<inheritdoc/>
        public override bool ValidateOpenTag(ParsingUtility.TagInfo tagInfo, out TMPEffectTag data, out int endIndex)
        {
            data = null;
            endIndex = -1;
            if (tagInfo.type != ParsingUtility.TagType.Open) throw new System.ArgumentException(nameof(tagInfo.type));
            if (tagInfo.prefix != Prefix) return false;
            if (database == null || !database.ContainsEffect(tagInfo.name)) return false;

            var param = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
            var command = database.GetEffect(tagInfo.name);
            if (!command.ValidateParameters(param, keywordDatabase)) return false;

            TMPEffectTag tag = new TMPEffectTag(tagInfo.name, tagInfo.prefix, param);
            data = tag;

            endIndex = command.TagType == TagType.Index ? tagInfo.startIndex : -1;
            return true;
        }

        ///<inheritdoc/>
        public override bool ValidateTag(TMPEffectTag tag)
        {
            if (tag.Prefix != Prefix) return false;
            if (database == null || !database.ContainsEffect(tag.Name)) return false;
            if (!database.GetEffect(tag.Name).ValidateParameters(tag.Parameters, keywordDatabase)) return false;
            return true;
        }

        ///<inheritdoc/>
        public override bool ValidateTag(ParsingUtility.TagInfo tagInfo)
        {
            if (tagInfo.prefix != Prefix) return false;
            if (database == null || !database.ContainsEffect(tagInfo.name)) return false;

            if (tagInfo.type == ParsingUtility.TagType.Open)
            {
                var param = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
                if (!database.GetEffect(tagInfo.name).ValidateParameters(param, keywordDatabase)) return false;
            }

            return true;
        }
    }
}