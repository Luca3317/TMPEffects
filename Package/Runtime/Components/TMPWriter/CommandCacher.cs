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
        private IWriterContext writerContext;
        private ITMPEffectDatabase<ITMPCommand> database;
        private TMPWriter writer;
        private IList<CharData> charData;

        public CommandCacher(IList<CharData> charData, TMPWriter writer, IWriterContext context, ITMPEffectDatabase<ITMPCommand> database)
        {
            this.charData = charData;
            this.writer = writer;
            this.database = database;
            this.writerContext = context;
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

            CommandContext cContext = new CommandContext(writerContext, fixedIndices);
            CachedCommand cc = new CachedCommand(tag, fixedIndices, cContext, command);
            return cc;
        }
    }
}