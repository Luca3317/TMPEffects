using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Components.Writer;
using TMPEffects.Databases;
using TMPEffects.Tags;
using UnityEngine;

namespace TMPEffects.Components.Writer
{
    public interface IWriterContext
    {
        public ITMPKeywordDatabase KeywordDatabase { get; }
        public TMPWriter Writer { get; }
    }

    [System.Serializable]
    public class WriterContext : IWriterContext
    {
        public ITMPKeywordDatabase KeywordDatabase => keywordDatabase;

        public TMPWriter Writer { get; set; }

        [SerializeField] private TMPSceneKeywordDatabase keywordDatabase;
    }

    public class ReadOnlyWriterContext : IWriterContext
    {
        public ITMPKeywordDatabase KeywordDatabase => context.KeywordDatabase;
        public TMPWriter Writer => context.Writer;

        private IWriterContext context;

        public ReadOnlyWriterContext(IWriterContext context)
        {
            this.context = context;
        }
    }
}

namespace TMPEffects.TMPCommands
{
    public interface ICommandContext
    {
        public IWriterContext WriterContext { get; }
        public TMPEffectTagIndices Indices { get; }
    }

    public class CommandContext : ICommandContext
    {
        public IWriterContext WriterContext { get; set; }
        public TMPEffectTagIndices Indices { get; set; }

        public CommandContext(IWriterContext writerContext, TMPEffectTagIndices indices)
        {
            this.WriterContext = writerContext;
            this.Indices = indices;
        }
    }

    public class ReadOnlyCommandContext : ICommandContext
    {
        public IWriterContext WriterContext => context.WriterContext;
        public TMPEffectTagIndices Indices => context.Indices;

        public ReadOnlyCommandContext(ICommandContext context)
        {
            this.context = context;
        }

        private ICommandContext context;
    }
}