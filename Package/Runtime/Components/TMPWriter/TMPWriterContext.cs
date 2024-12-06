using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Components.Writer;
using TMPEffects.Databases;
using TMPEffects.Tags;
using UnityEngine;

namespace TMPEffects.TMPCommands
{
    public interface ICommandContext
    {
        public TMPWriter Writer { get; }
        
        public TMPEffectTagIndices Indices { get; }

        public object CustomData { get; }
    }

    public class CommandContext : ICommandContext
    {
        public TMPWriter Writer { get; set; }
        public TMPEffectTagIndices Indices { get; set; }
        
        public object CustomData { get; set; }

        public CommandContext(TMPWriter writer, TMPEffectTagIndices indices)
        {
            this.Writer = writer;
            this.Indices = indices;
        }
    }

    public class ReadOnlyCommandContext : ICommandContext
    {
        public TMPWriter Writer => context.Writer;
        public TMPEffectTagIndices Indices => context.Indices;
        public object CustomData => context.CustomData;

        public ReadOnlyCommandContext(ICommandContext context)
        {
            this.context = context;
        }

        private ICommandContext context;
    }
}