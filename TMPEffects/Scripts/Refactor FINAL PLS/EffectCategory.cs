using JetBrains.Annotations;
using System.Collections.Generic;
using TMPEffects;
using TMPEffects.Commands;
using TMPEffects.Databases;
using TMPEffects.Tags;
using TMPEffects.TextProcessing;


/*
 * TODO the ValidateTag(TagInfo, out EffectTag) methods need to handle open / close tags differently
 * 
 */


namespace TMPEffects
{
    public class TMPAnimationCategory : TMPEffectCategory<ITMPAnimation>
    {
        private ITMPEffectDatabase<ITMPAnimation> database;

        // TODO Added this to get rid of error in onvalidate in TMPAnimator
        // Might remove
        public ITMPEffectDatabase<ITMPAnimation> Database => database;

        public TMPAnimationCategory(char prefix, ITMPEffectDatabase<ITMPAnimation> database) : base(prefix)
        {
            this.database = database;
        }

        public override bool ContainsEffect(string name) => database.ContainsEffect(name);
        public override ITMPAnimation GetEffect(string name) => database.GetEffect(name);

        public override bool ValidateTag(ParsingUtility.TagInfo tagInfo, out EffectTag data)
        {
            data = null;
            if (tagInfo.prefix != Prefix) return false;
            if (database == null || !database.ContainsEffect(tagInfo.name)) return false;

            var param = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
            if (!database.GetEffect(tagInfo.name).ValidateParameters(param)) return false;

            EffectTag tag = new EffectTag(tagInfo.name, tagInfo.prefix, param);
            data = tag;
            return true;
        }

        public override bool ValidateTag(EffectTag tag)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TMPEventCategory : TMPEffectCategory
    {
        public TMPEventCategory(char prefix) : base(prefix)
        { }

        public override bool ValidateTag(ParsingUtility.TagInfo tagInfo, out EffectTag data)
        {
            data = null;
            if (tagInfo.prefix != Prefix) return false;
            EffectTag tagData = new(tagInfo.name, tagInfo.prefix, ParsingUtility.GetTagParametersDict(tagInfo.parameterString));
            data = tagData;
            return true;
        }

        public override bool ValidateTag(EffectTag tag)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TMPCommandCategory : TMPEffectCategory<ITMPCommand>
    {
        private ITMPEffectDatabase<ITMPCommand> database;

        public TMPCommandCategory(char prefix, ITMPEffectDatabase<ITMPCommand> database) : base(prefix)
        {
            this.database = database;
        }

        public override bool ContainsEffect(string name) => database.ContainsEffect(name);
        public override ITMPCommand GetEffect(string name) => database.GetEffect(name);

        public override bool ValidateTag(ParsingUtility.TagInfo tagInfo, out EffectTag data)
        {
            data = null;
            if (tagInfo.prefix != Prefix) return false;
            if (!database.ContainsEffect(tagInfo.name)) return false;

            var param = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
            EffectTag tag = new EffectTag(tagInfo.name, tagInfo.prefix, param);
            data = tag;
            return true;
        }

        public override bool ValidateTag(EffectTag tag)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TMPSceneCommandCategory : TMPEffectCategory<ITMPCommand>
    {
        private Dictionary<string, SceneCommand> tags;

        public TMPSceneCommandCategory(char prefix, Dictionary<string, SceneCommand> tags) : base(prefix)
        {
            this.tags = tags;
        }

        public override bool ContainsEffect(string name) => tags.ContainsKey(name);
        public override ITMPCommand GetEffect(string name) => tags[name];

        public override bool ValidateTag(ParsingUtility.TagInfo tagInfo, out EffectTag data)
        {
            data = null;
            if (!tags.ContainsKey(tagInfo.name)) return false;
            if (tagInfo.type == ParsingUtility.TagType.Open || tags[tagInfo.name].CommandType != CommandType.Index)
            {
                var param = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
                EffectTag tag = new EffectTag(tagInfo.name, tagInfo.prefix, param);
                data = tag;
                return true;
            }

            return false;
        }

        public override bool ValidateTag(EffectTag tag)
        {
            throw new System.NotImplementedException();
        }
    }

    public abstract class TMPEffectCategory<TEffect> : TMPEffectCategory, ITMPEffectDatabase<TEffect>
    {
        public TMPEffectCategory(char prefix) : base(prefix)
        { }

        public abstract bool ContainsEffect(string name);
        public abstract TEffect GetEffect(string name);
    }

    public abstract class TMPEffectCategory : ITMPTagValidator, ITMPPrefixSupplier
    {
        public char Prefix => prefix;

        protected readonly char prefix;

        public TMPEffectCategory(char prefix)
        {
            this.prefix = prefix;
        }

        public abstract bool ValidateTag(ParsingUtility.TagInfo tagInfo, out EffectTag data);
        public abstract bool ValidateTag(EffectTag tag);
        public bool ValidateTag(ParsingUtility.TagInfo tagInfo)
        {
            return ValidateTag(tagInfo, out _);
        }
    }

    public interface ITMPPrefixSupplier
    {
        public char Prefix { get; }
    }

    public interface ITMPTagValidator
    {
        public bool ValidateTag(ParsingUtility.TagInfo tagInfo, out EffectTag data);
        public bool ValidateTag(ParsingUtility.TagInfo tagInfo);
        public bool ValidateTag(EffectTag tag);
    }
}
