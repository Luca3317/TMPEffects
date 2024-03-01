using TMPEffects.SerializedCollections;
using System.Collections;
using TMPEffects.TMPCommands;
using TMPEffects.TextProcessing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEditor;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using TMPEffects.Extensions;
using TMPEffects.EffectCategories;
using TMPEffects.Components.Writer;
using TMPEffects.Tags.Collections;
using TMPEffects.TMPEvents;
using TMPEffects.Databases.CommandDatabase;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.Components
{
    /// <summary>
    /// Shows / hides the characters of a <see cref="TMP_Text"/> component over time.
    /// </summary>
    /// <remarks>
    /// One of the two main components of TMPEffects, along with <see cref="TMPAnimator"/>.<br/>
    /// TMPWriter allows you to show / hide text over time.<br/>  
    /// Using command tags, you can call specific methods. There are two types of Commands:
    /// <list type="table">
    /// <item><see cref="TMPCommand"/>: These are defined by the <see cref="TMPCommandDatabase"/> object on the component. As they derive from <see cref="ScriptableObject"/>, they are stored on disk. All built-in commands of this type serve to control the TMPWriter component.</item>
    /// <item><see cref="SceneCommand"/>: These are defined as a property on the TMPWriter component. You can use them to reference specific methods on objects in the scene.</item>
    /// </list>
    /// In additon to command tags, TMPWriter also processes event tags:<br/><br/>
    /// <see cref="TMPEvent"/>: Using event tags, you can raise events from text, i.e. when a specific character is shown. You can subscribe to these events with OnTextEvent. 
    /// </remarks>
    [ExecuteAlways, DisallowMultipleComponent, RequireComponent(typeof(TMP_Text))]
    public class TMPWriter : TMPEffectComponent
    {
        // State
        /// <summary>
        /// Whether the TMPWriter is currently writing text.
        /// </summary>
        public bool IsWriting => writing;
        /// <summary>
        /// Whether the TMPWriter may skip the current section of text.
        /// </summary>
        public bool MaySkip => currentMaySkip;
        /// <summary>
        /// The text index of the TMPWriter's current character.
        /// </summary>
        public int CurrentIndex => currentIndex;

        // Database
        /// <summary>
        /// The database used to parse command tags.
        /// </summary>
        public TMPCommandDatabase Database => database;

        // Tags
        /// <summary>
        /// All tags parsed by the TMPWriter.
        /// </summary>
        public ITagCollection Tags => tags;
        /// <summary>
        /// All command tags parsed by the TMPWriter.
        /// </summary>
        public ITagCollection CommandTags => tags[commandCategory];
        /// <summary>
        /// All event tags parsed by the TMPWriter.
        /// </summary>
        public ITagCollection EventTags => tags[eventCategory];

        // Events
        /// <summary>
        /// Raised when the TMPWriter reaches an event tag.
        /// </summary>
        public TMPEvent OnTextEvent;
        /// <summary>
        /// Raised when the TMPWriter shows a new character.
        /// </summary>
        public UnityEvent<CharData> OnShowCharacter;
        /// <summary>
        /// Raised when the TMPWriter starts / resumes writing.
        /// </summary>
        public UnityEvent OnStartWriter;
        /// <summary>
        /// Raised when the TMPWriter stops writing.
        /// </summary>
        public UnityEvent OnStopWriter;
        /// <summary>
        /// Raised when the TMPWriter is done writing the current text.
        /// </summary>
        public UnityEvent OnFinishWriter;
        /// <summary>
        /// Raised when the current (section of) text is skipped.
        /// </summary>
        public UnityEvent OnSkipWriter;
        /// <summary>
        /// Raised when the TMPWriter is reset.<br/>
        /// The integer parameter indicates the text index the TMPWriter was reset to.
        /// </summary>
        public UnityEvent<int> OnResetWriter;

        // TODO Needed? if not, private
        public float WhiteSpaceDelay => whiteSpaceDelayType == DelayType.Raw ? whiteSpaceDelay : currentDelay * whiteSpaceDelay;
        public float PunctuationDelay => punctuationDelayType == DelayType.Raw ? punctuationDelay : currentDelay * punctuationDelay;
        public float VisibleDelay => visibleDelayType == DelayType.Raw ? visibleDelay : currentDelay * visibleDelay;
        public float LinebreakDelay => linebreakDelayType == DelayType.Raw ? linebreakDelay : currentDelay * linebreakDelay;

        /// <summary>
        /// The prefix used for command tags.
        /// </summary>
        public const char COMMAND_PREFIX = '!';
        /// <summary>
        /// The prefix used for event tags.
        /// </summary>
        public const char EVENT_PREFIX = '#';

        #region Fields
        // Settings
        [SerializeField] TMPCommandDatabase database;

        [Tooltip("The delay between new characters shown by the writer, i.e. the inverse of the speed of the writer.")]
        [SerializeField] private float delay = 0.075f;

        [Tooltip("Whether the text may be skipped by default.")]
        [SerializeField] private bool maySkip = false;

        [Tooltip("If checked, the writer will begin writing when it is first enabled. If not checked, you will have to manually start the writer from your own code.")]
        [SerializeField] private bool writeOnStart = true;
        [Tooltip("If checked, the writer will automatically begin writing when the text on the associated TMP_Text component is modified. If not checked, you will have to manually start the writer from your own code.")]
        [SerializeField] private bool autoWriteNewText = true;

        [Tooltip("The delay after whitespace characters, as percentage of the general delay")]
        [SerializeField] private float whiteSpaceDelay;
        [SerializeField] private DelayType whiteSpaceDelayType;
        [Tooltip("The delay after punctuation characters, as percentage of the general delay")]
        [SerializeField] private float punctuationDelay;
        [SerializeField] private DelayType punctuationDelayType;
        [Tooltip("The delay after already visible characters, as percentage of the general delay")]
        [SerializeField] private float visibleDelay;
        [SerializeField] private DelayType visibleDelayType;
        [Tooltip("The delay after linebreaks, as percentage of the general delay")]
        [SerializeField] private float linebreakDelay;
        [SerializeField] private DelayType linebreakDelayType;

        public enum DelayType
        {
            Percentage,
            Raw
        }

        // Scene commands
        [Tooltip("Commands that may reference scene objects.\nNOT raised in preview mode.")]
        [SerializeField, SerializedDictionary("Tag Name", "Command")] private SerializedDictionary<string, SceneCommand> sceneCommands;

        [System.NonSerialized] private TagProcessorManager processors;
        [System.NonSerialized] private TagCollectionManager<TMPEffectCategory> tags;

        [System.NonSerialized] private TMPCommandCategory commandCategory;
        [System.NonSerialized] private TMPEventCategory eventCategory;

        [System.NonSerialized] private CachedCollection<CachedCommand> commands;
        [System.NonSerialized] private CachedCollection<CachedEvent> events;

        [System.NonSerialized] private Coroutine writerCoroutine = null;
        [System.NonSerialized] private float currentDelay;
        [System.NonSerialized] private bool currentMaySkip;

        [System.NonSerialized] private bool shouldWait = false;
        [System.NonSerialized] private float waitAmount = 0f;
        [System.NonSerialized] private Func<bool> continueConditions;

        [System.NonSerialized] private bool writing = false;
        [System.NonSerialized] private int currentIndex = -1;
        #endregion

        #region Initialization

        private void OnEnable()
        {
            UpdateMediator();

            SubscribeToMediator();

            PrepareForProcessing();

            Mediator.ForceReprocess();

#if UNITY_EDITOR
            if (!Application.isPlaying) EditorApplication.update += EditorUpdate;
#endif
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (writeOnStart) StartWriter();
        }

        private void OnDisable()
        {
            processors.UnregisterFrom(Mediator.Processor);

            Mediator.ForceReprocess();

            UnsubscribeFromMediator();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorApplication.update -= EditorUpdate;
                StopWriterCoroutine();
                currentIndex = -1;
                Show(0, Mediator.CharData.Count, true);
                writing = false;
            }
#endif

            FreeMediator();
        }

        private void PrepareForProcessing()
        {
            CommandDatabase db = new CommandDatabase(database, sceneCommands);

            // Reset categories
            commandCategory = new TMPCommandCategory(COMMAND_PREFIX, db);
            eventCategory = new TMPEventCategory(EVENT_PREFIX);

            // Reset tagcollection & cachedcollection
            ReadOnlyCollection<CharData> ro = new ReadOnlyCollection<CharData>(Mediator.CharData);
            tags = new();
            commands = new CachedCollection<CachedCommand>(new CommandCacher(ro, this, commandCategory), tags.AddKey(commandCategory));
            events = new CachedCollection<CachedEvent>(new EventCacher(/*ro,*/ OnTextEvent), tags.AddKey(eventCategory));

            // Reset processors
            processors ??= new();
            processors.UnregisterFrom(Mediator.Processor);
            processors.Clear();

            processors.AddProcessor(commandCategory.Prefix, new TagProcessor(commandCategory));
            processors.AddProcessor(eventCategory.Prefix, new TagProcessor(eventCategory));

            processors.RegisterTo(Mediator.Processor);
        }

        private void SubscribeToMediator()
        {
            Mediator.TextChanged += OnTextChanged;
            Mediator.Processor.FinishAdjustIndices += PostProcessTags;
        }

        private void UnsubscribeFromMediator()
        {
            Mediator.TextChanged -= OnTextChanged;
            Mediator.Processor.FinishAdjustIndices -= PostProcessTags;
        }

        private void OnDatabaseChanged()
        {
            PrepareForProcessing();
        }
        #endregion

        #region Writer Controlling
        /// <summary>
        /// Start (or resume) writing.
        /// </summary>
        public void StartWriter()
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy) return;

            if (!writing)
            {
                StartWriterCoroutine();
            }

            OnStartWriter?.Invoke();
        }

        /// <summary>
        /// Stop writing.<br/>
        /// Note that this does not reset the shown text.
        /// </summary>
        public void StopWriter()
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy) return;

            if (writing)
            {
                StopWriterCoroutine();
            }

            OnStopWriter?.Invoke();
        }

        /// <summary>
        /// Reset the writer to the initial state for the current text.
        /// </summary>
        public void ResetWriter()
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy) return;

            if (writing)
            {
                StopWriterCoroutine();
            }

            // reset
            currentIndex = -1;
            ResetInvokables(Mediator.CharData.Count);
            Hide(0, Mediator.CharData.Count, true);

            ResetData();

            OnResetWriter?.Invoke(0);
        }

        /// <summary>
        /// Reset the writer to the given index of the current text.
        /// </summary>
        /// <param name="index">The index to reset the writer to.</param>
        public void ResetWriter(int index)
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy) return;

            if (writing)
            {
                StopWriterCoroutine();
            }

            // reset
            ResetData();

            Hide(0, Mediator.CharData.Count, true);
            Show(0, index, true);
            //Hide(index, Mediator.CharData.Count - index, true);
            //Show(0, index, true);

            ResetInvokables(currentIndex);
            for (int i = -1; i < index; i++)
            {
                RaiseInvokables(i);
            }

            currentIndex = index;

            OnResetWriter?.Invoke(index);
        }

        /// <summary>
        /// Skip the current section of the text.<br/>
        /// If the current section may not be skipped, this will do nothing.<br/>
        /// Otherwise, the writing process is skipped to either the end of the current text, or the next unskippable section of the current text.
        /// </summary>
        public void SkipWriter()
        {
            if (!isActiveAndEnabled || gameObject.activeInHierarchy || !currentMaySkip) return;

            int skipTo;
            CachedCommand cc = commands.FirstOrDefault(x => x.Indices.StartIndex >= currentIndex && x.Tag.Name == "skippable" && x.Tag.Parameters != null && x.Tag.Parameters[""] == "false");

            if (cc == default) skipTo = Mediator.CharData.Count;
            else skipTo = cc.Indices.StartIndex;

            OnSkipWriter?.Invoke();

            for (int i = currentIndex; i < skipTo; i++)
            {
                RaiseInvokables(i, true);
            }

            currentIndex = skipTo;
            Show(0, skipTo, true); // TODO toggle for whether to skip show animations on skip

            if (skipTo == Mediator.CharData.Count)
            {
                if (writing) StopWriterCoroutine();
                OnFinishWriter?.Invoke();
            }
        }

        /// <summary>
        /// Restart the writer.<br/>
        /// This will reset the writer and start the writing process.
        /// </summary>
        public void RestartWriter()
        {
            if (!isActiveAndEnabled) return;

            ResetWriter();
            StartWriter();
        }
        #endregion

        #region Writer Commands
        /// <summary>
        /// Pause the writer for the given amount of seconds.
        /// </summary>
        /// <param name="seconds">The amount of time to wait.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Throws if <paramref name="seconds"/> is less than zero.</exception>
        public void Wait(float seconds)
        {
            if (seconds < 0f)
            {
                throw new System.ArgumentOutOfRangeException("Seconds was negative");
            }

            if (shouldWait)
            {
                waitAmount = Mathf.Max(waitAmount, seconds);
                return;
            }

            shouldWait = true;
            waitAmount = seconds;
        }

        /// <summary>
        /// Pause the writer until the given condition evaluates to true.
        /// </summary>
        /// <param name="condition">The condition to wait for.</param>
        public void WaitUntil(Func<bool> condition)
        {
            if (condition == null) return;

            continueConditions -= condition;
            continueConditions += condition;
        }

        /// <summary>
        /// Set the current delay of the writer.
        /// </summary>
        /// <param name="delay">The delay between showing two characters.</param>
        public void SetDelay(float delay)
        {
            currentDelay = delay;
        }

        /// <summary>
        /// Set whether the current text may be skipped.
        /// </summary>
        /// <param name="skippable">Whether the current text may be skipped.</param>
        public void SetSkippable(bool skippable)
        {
            currentMaySkip = skippable;
        }
        #endregion

        #region Setters
        /// <summary>
        /// Set the database that will be used to parse command tags.
        /// </summary>
        /// <param name="database">The database that will be used to parse command tags.</param>
        public void SetDatabase(TMPCommandDatabase database)
        {
            this.database = database;
            OnDatabaseChanged();
        }
        #endregion

        #region Event Callbacks
        private void OnTextChanged()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                bool wasWriting = writing;
                ResetWriter();
                if (wasWriting)
                {
                    StartWriter();
                }
                else Show(0, Mediator.CharData.Count, true);
                return;
            }
#endif

            ResetWriter();
            if (autoWriteNewText) StartWriter();
        }
        #endregion

        #region Writer Coroutine
        private void StartWriterCoroutine()
        {
            StopWriterCoroutine();
            writerCoroutine = StartCoroutine(WriterCoroutine());
        }

        private void StopWriterCoroutine()
        {
            if (writerCoroutine != null)
            {
                StopCoroutine(writerCoroutine);
                writerCoroutine = null;
            }

            OnStopWriting();
        }

        private IEnumerator WriterCoroutine()
        {
            OnStartWriting();

            // This indicates the text is fully shown already
            if (currentIndex >= Mediator.CharData.Count)
            {
                OnStopWriting();
                yield break;
            }

            // Reset all relevant variables
            if (currentIndex <= 0) ResetData();

            if (currentIndex == -1)
                HideAllCharacters(true);

            // Execute instant commands
            yield return RaiseInvokablesCoroutine(-1);

            CharData cData;
            for (int i = Mathf.Max(currentIndex, 0); i < Mediator.CharData.Count; i++)
            {
                currentIndex = i;
                cData = Mediator.CharData[i];

                // Execute the commands & events for the current index
                yield return RaiseInvokablesCoroutine(i);

                // Calculate delay; do here to use accurate visibilitystate
                float delay = CalculateDelay(i);

                // Show the current character, if it is not already shown
                VisibilityState vState = Mediator.VisibilityStates[i];
                if (vState == VisibilityState.Hidden || vState == VisibilityState.Hiding)
                {
                    // TODO should this be raised even if already shown?
                    OnShowCharacter?.Invoke(cData);
                    Show(i, 1, false);
                }

                // Calculate and wait for the delay for the current index
                if (delay > 0) yield return new WaitForSeconds(delay);
            }

            OnFinishWriter?.Invoke();
            OnStopWriting();
        }

        private void ResetData()
        {
            currentDelay = delay;
            currentMaySkip = maySkip;
        }

        private void ResetInvokables(int maxIndex)
        {
            foreach (var eventt in events)
            {
                if ((eventt.Indices.StartIndex < maxIndex || eventt.ExecuteInstantly) && eventt.ExecuteRepeatable) eventt.Reset();
            }

            foreach (var command in commands)
            {
                if ((command.Indices.StartIndex < maxIndex || command.ExecuteInstantly) && command.ExecuteRepeatable) command.Reset();
            }
        }

        private float CalculateDelay(int index)
        {
            if (currentDelay <= 0) return 0;

            CharData cData = Mediator.CharData[index];

            // If character is invisible (=> is whitespace)
            if (!cData.info.isVisible) // Alternatively check char.IsWhiteSpace
            {
                // If line break
                if (cData.info.character == '\n')
                {
                    return Mathf.Max(LinebreakDelay, 0);
                }
                //if (index < Mediator.CharData.Count - 1 && Mediator.CharData[index + 1].info.lineNumber > Mediator.CharData[index].info.lineNumber)
                //{
                //    return Mathf.Max(currentDelay * LinebreakDelay);
                //}

                return Mathf.Max(WhiteSpaceDelay, 0);
            }

            // If character is already shown (e.g. through using the !show command)
            VisibilityState vState = Mediator.GetVisibilityState(cData);
            if (vState == VisibilityState.Shown || vState == VisibilityState.Showing)
            {
                return Mathf.Max(VisibleDelay, 0);
            }

            // If character is punctuation, and not directly followed by another punctuation (to multiple extended delays for e.g. "..." or "?!?"
            if (char.IsPunctuation(cData.info.character) && (index == Mediator.CharData.Count - 1 || !char.IsPunctuation(Mediator.CharData[index + 1].info.character)))
            {
                return Mathf.Max(PunctuationDelay, 0);
            }

            return currentDelay;
        }

        // TODO Toggle for whether to wait 
        //      Until all conditions have been true at one point since starting the check
        //      Until all conditions are true at once
        private IEnumerator HandleWaitConditions()
        {
            if (continueConditions == null) yield break;

            bool allMet;
            Delegate[] delegates;
            do
            {
                allMet = true;
                delegates = continueConditions.GetInvocationList();

                for (int i = 0; i < delegates.Length; i++)
                {
                    if (!(delegates[i] as Func<bool>)()) allMet = false;
                    else continueConditions -= (delegates[i] as Func<bool>);
                }

                yield return null;

            } while (!allMet);

            continueConditions = null;
        }

        private IEnumerable<ICachedInvokable> GetInvokables(int index, bool skipped = false)
        {
            IEnumerable<ICachedInvokable> invokables;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!commandsEnabled && !eventsEnabled) return new List<ICachedInvokable>();


                if (skipped)
                {
                    if (commandsEnabled)
                    {
                        invokables = commands;
                        if (eventsEnabled)
                        {
                            invokables = invokables.Concat(events);
                            invokables = invokables.OrderBy(x => x.Indices.StartIndex).ThenBy(x => x.Indices.OrderAtIndex);
                        }
                    }
                    else { invokables = events; }

                    invokables = invokables.Where(x => x.Indices.StartIndex >= index && x.ExecuteOnSkip);
                }
                else if (index < 0)
                {
                    if (commandsEnabled)
                    {
                        invokables = commands;
                        invokables = invokables.Where(x => x.ExecuteInstantly);
                    }
                    else invokables = new List<ICachedInvokable>();

                }
                else
                {
                    if (commandsEnabled)
                    {
                        invokables = commands.GetAt(index);
                        if (eventsEnabled)
                        {
                            invokables = invokables.Concat(events.GetAt(index));
                            invokables = invokables.OrderBy(x => x.Indices.StartIndex).ThenBy(x => x.Indices.OrderAtIndex);
                        }
                    }
                    else invokables = events.GetAt(index);
                }

                return invokables.Where(x => x.ExecuteInPreview);
            }
#endif

            if (skipped)
            {
                invokables = commands;
                if (events.HasAny())
                {
                    invokables = invokables.Concat(events);
                    invokables = invokables.OrderBy(x => x.Indices.StartIndex).ThenBy(x => x.Indices.OrderAtIndex);
                }
                invokables = invokables.Where(x => x.Indices.StartIndex >= index && x.ExecuteOnSkip);
            }
            else if (index < 0)
            {
                invokables = commands;
                invokables = invokables.Where(x => x.ExecuteInstantly);
            }
            else
            {
                invokables = commands.GetAt(index);
                if (events.HasAnyAt(index))
                {
                    invokables = invokables.Concat(events.GetAt(index));
                    invokables = invokables.OrderBy(x => x.Indices.StartIndex).ThenBy(x => x.Indices.OrderAtIndex);
                }
            }

            return invokables;
        }

        private void RaiseInvokables(int index, bool skipped = false)
        {
            foreach (var invokable in GetInvokables(index, skipped))
            {
                invokable.Trigger();
                waitAmount = 0f;
                shouldWait = false;
                continueConditions = null;
            }
        }

        private IEnumerator RaiseInvokablesCoroutine(int index, bool skipped = false, bool block = true)
        {
            foreach (var invokable in GetInvokables(index, skipped))
            {
                invokable.Trigger();

                if (block)
                {
                    if (shouldWait) yield return new WaitForSeconds(waitAmount);
                    yield return HandleWaitConditions();
                }

                waitAmount = 0f;
                shouldWait = false;
                continueConditions = null;
            }
        }

        private void OnStopWriting()
        {
            writing = false;
        }

        private void OnStartWriting()
        {
            writing = true;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        [System.NonSerialized] bool reprocessFlag = false;
        [SerializeField, HideInInspector] TMPCommandDatabase prevDatabase = null;
        [SerializeField, HideInInspector] bool initValidate = false;
        [SerializeField, HideInInspector] bool eventsEnabled = false;
        [SerializeField, HideInInspector] bool commandsEnabled = false;

        private void EditorUpdate()
        {
            if (reprocessFlag)
            {
                Mediator.ForceReprocess();
                reprocessFlag = false;
            }
            if (IsWriting)
                EditorApplication.QueuePlayerLoopUpdate();
        }

        [MenuItem("CONTEXT/TMP_Text/Add writer")]
        static void AddWriter(MenuCommand command)
        {
            TMP_Text text = command.context as TMP_Text;
            if (text == null)
            {
                Debug.LogWarning("Could not add writer to " + command.context.name);
                return;
            }

            text.gameObject.GetOrAddComponent<TMPWriter>();
        }

        private void OnValidate()
        {
            if (!initValidate)
            {
                prevDatabase = database;
                initValidate = true;
            }

            // Ensure data is set - OnValidate called before OnEnable
            if (Mediator != null && database != prevDatabase)
            {
                prevDatabase = database;
                OnDatabaseChanged();

                reprocessFlag = true;
            }
        }

        internal void OnDatabaseChangedWrapper()
        {
            if (Mediator == null) return;
            OnDatabaseChanged();
        }

        internal void ForceReprocess()
        {
            Mediator.ForceReprocess();
        }

        internal void SkipPlayer()
        {
            int skipTo = Mediator.CharData.Count;

            for (int i = currentIndex; i < skipTo; i++)
            {
                RaiseInvokables(i, true);
            }

            currentIndex = skipTo;
            Show(0, skipTo, true); // TODO toggle for whether to skip show animations on skip

            if (skipTo == Mediator.CharData.Count)
            {
                if (writing) StopWriterCoroutine();
            }
        }
#endif
        #endregion

        private void HideAllCharacters(bool skipAnimations = false)
        {
            Hide(0, Mediator.CharData.Count, skipAnimations);
        }

        private void PostProcessTags(string text)
        {
            tags.Clear();

            foreach (var processor in processors.TagProcessors[commandCategory.Prefix])
            {
                foreach (var tag in processor.ProcessedTags)
                {
                    tags[commandCategory].TryAdd(tag.Value, tag.Key);
                }
            }

            foreach (var processor in processors.TagProcessors[eventCategory.Prefix])
            {
                foreach (var tag in processor.ProcessedTags)
                    tags[eventCategory].TryAdd(tag.Value, tag.Key);
            }
        }
    }
}