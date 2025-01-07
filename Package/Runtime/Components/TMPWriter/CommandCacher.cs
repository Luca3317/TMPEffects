using System.Collections.Generic;
using System;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Databases;
using TMPEffects.Tags;
using TMPEffects.TMPCommands;

namespace TMPEffects.Components.Writer
{
    internal class CommandCacher : ITagCacher<CachedCommand>
    {
        private ITMPEffectDatabase<ITMPCommand> database;
        private TMPWriter writer;
        private IList<CharData> charData;
        private ITMPKeywordDatabase keywordDatabase;

        public CommandCacher(IList<CharData> charData, TMPWriter writer, ITMPEffectDatabase<ITMPCommand> database,
            ITMPKeywordDatabase keywordDatabase)
        {
            this.charData = charData;
            this.writer = writer;
            this.database = database;
            // Passed in to allow for parsing eventual default arguments (like AnimationCacher)
            this.keywordDatabase = keywordDatabase; 
        }

        public CachedCommand CacheTag(TMPEffectTag tag, TMPEffectTagIndices indices)
        {
            ITMPCommand command = database.GetEffect(tag.Name);
            int endIndex = indices.EndIndex;

            switch (command.TagType)
            {
                case TagType.Index: endIndex = indices.StartIndex + 1; break;
                case TagType.Either:
                case TagType.Block:
                    if (indices.IsOpen) endIndex = charData.Count;
                    break;
                default: throw new ArgumentException(nameof(command.TagType));
            }

            TMPEffectTagIndices fixedIndices = new TMPEffectTagIndices(indices.StartIndex, endIndex, indices.OrderAtIndex);

            CommandContext cContext = new CommandContext(writer, fixedIndices);
            
            object customCommandData = command.GetNewCustomData();
            cContext.CustomData = customCommandData;
            command.SetParameters(customCommandData, tag.Parameters, keywordDatabase);
            
            CachedCommand cc = new CachedCommand(tag, fixedIndices, cContext, command);
            return cc;
        }
    }
}