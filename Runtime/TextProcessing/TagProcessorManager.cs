using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TMPEffects.TextProcessing
{
    /// <summary>
    /// Interface for manager of <see cref="TagProcessor"/>.
    /// </summary>
    public interface ITagProcessorManager : IEnumerable<TagProcessor>
    {
        /// <summary>
        /// Mapping of prefix to list of <see cref="TagProcessor"/>.
        /// </summary>
        public ReadOnlyDictionary<char, ReadOnlyCollection<TagProcessor>> TagProcessors { get; }

        /// <summary>
        /// Add a processor.
        /// </summary>
        /// <param name="prefix">Prefix of tags processed by <paramref name="processor"/>.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="priority">
        /// Priority of this processor. Only one processors can process any given tag, 
        /// so the order that processors are invoked in matters.<br/>
        /// Less than zero => last processed<br/>
        /// zero => first processed (highest priority)<br/>
        /// Greater than zero => priority-th processed<br/>
        /// Greater than current amount of processors => last processed
        /// </param>
        public void AddProcessor(char prefix, TagProcessor processor, int priority = 0);
        /// <summary>
        /// Remove a processor.
        /// </summary>
        /// <param name="prefix">Prefix of tags processed by <paramref name="processor"/>.</param>
        /// <param name="processor">The processor.</param>
        /// <returns>true if the processors was removed; otherwise false.</returns>
        public bool RemoveProcessor(char prefix, TagProcessor processor);
    }

    /// <summary>
    /// Manages multiple <see cref="TagProcessor"/>.
    /// </summary>
    public class TagProcessorManager : ITagProcessorManager
    {
        ///<inheritdoc/>
        public ReadOnlyDictionary<char, ReadOnlyCollection<TagProcessor>> TagProcessors { get; private set; }

        //public event NotifyProcessorsChangedEventHandler ProcessorsChanged;

        public TagProcessorManager()
        {
            tagProcessors = new();
            tagProcessorsRO = new();
            TagProcessors = new(tagProcessorsRO);
        }

        ///<inheritdoc/>
        public void AddProcessor(char prefix, TagProcessor processor, int priority = 0)
        {
            if (processor == null) throw new System.ArgumentNullException(nameof(processor));

            List<TagProcessor> processors;
            if (tagProcessors.TryGetValue(prefix, out processors))
            {
                if (priority > processors.Count || priority < 0)
                {
                    processors.Add(processor);
                }
                else
                {
                    processors.Insert(priority, processor);
                }
            }
            else
            {
                processors = new List<TagProcessor>() { processor };
                tagProcessors.Add(prefix, processors);
                tagProcessorsRO.Add(prefix, new ReadOnlyCollection<TagProcessor>(processors));
            }
        }

        ///<inheritdoc/>
        public bool RemoveProcessor(char prefix, TagProcessor processor)
        {
            if (processor == null) throw new System.ArgumentNullException(nameof(processor));

            List<TagProcessor> processors;
            if (!tagProcessors.TryGetValue(prefix, out processors)) return false;

            int index = processors.IndexOf(processor);
            if (!processors.Remove(processor)) return false;

            if (processors.Count == 0)
            {
                tagProcessors.Remove(prefix);
                tagProcessorsRO.Remove(prefix);
            }

            return true;
        }

        /// <summary>
        /// Remove all processors.
        /// </summary>
        public void Clear()
        {
            tagProcessors.Clear();
            tagProcessorsRO.Clear();
        }

        /// <summary>
        /// Register all <see cref="TagProcessor"/> to the given <paramref name="textProcessor"/>.
        /// </summary>
        /// <param name="textProcessor">The <see cref="TMPTextProcessor"/> to register to.</param>
        public void RegisterTo(TMPTextProcessor textProcessor)
        {
            foreach (var kvp in tagProcessors)
            {
                foreach (var processor in kvp.Value)
                {
                    textProcessor.AddProcessor(kvp.Key, processor);
                }
            }
        }

        /// <summary>
        /// Unregister all <see cref="TagProcessor"/> from the given <paramref name="textProcessor"/>.
        /// </summary>
        /// <param name="textProcessor">The <see cref="TMPTextProcessor"/> to unregister from.</param>
        public void UnregisterFrom(TMPTextProcessor textProcessor)
        {
            foreach (var kvp in tagProcessors)
            {
                foreach (var processor in kvp.Value)
                {
                    textProcessor.RemoveProcessor(kvp.Key, processor);
                }
            }
        }

        /// <summary>
        /// Get all <see cref="TagProcessor"/> managed by this instance.
        /// </summary>
        /// <returns>All <see cref="TagProcessor"/> managed by this instance.</returns>
        public IEnumerator<TagProcessor> GetEnumerator()
        {
            foreach (var list in tagProcessors.Values)
                foreach (var processor in list)
                    yield return processor;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Dictionary<char, List<TagProcessor>> tagProcessors;
        private Dictionary<char, ReadOnlyCollection<TagProcessor>> tagProcessorsRO;
    }
}