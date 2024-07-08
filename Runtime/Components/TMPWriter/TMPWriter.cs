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
using TMPEffects.CharacterData;
using TMPEffects.Tags;

namespace TMPEffects.Components
{
    /// <summary>
    /// Shows / hides the characters of a <see cref="TMP_Text"/> component over time,<br/>
    /// and allows you to raise events and commands at specific indices.
    /// </summary>
    /// <remarks>
    /// One of the two main components of TMPEffects, along with <see cref="TMPAnimator"/>.<br/>
    /// Using command tags, you can call specific methods. There are two types of Commands:
    /// <list type="table">
    /// <item><see cref="TMPCommand"/>: These are defined by the <see cref="TMPCommandDatabase"/> object on the component. As they derive from <see cref="ScriptableObject"/>, they are stored on disk. All built-in commands of this type serve to control the TMPWriter component.</item>
    /// <item><see cref="TMPSceneCommand"/>: These are defined as a property on the TMPWriter component. You can use them to reference specific methods on objects in the scene.</item>
    /// </list>
    /// In additon to command tags, TMPWriter also processes event tags:<br/><br/>
    /// <see cref="TMPEvent"/>: Using event tags, you can raise events from text, i.e. when a specific character is shown. You can subscribe to these events with OnTextEvent. 
    /// </remarks>    
    [HelpURL("https://tmpeffects.luca3317.dev/docs/tmpwriter.html")]
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
        public ITagCollection CommandTags => tags == null ? null : tags[commandCategory];
        /// <summary>
        /// All event tags parsed by the TMPWriter.
        /// </summary>
        public ITagCollection EventTags => tags == null ? null : tags[eventCategory];

        public bool WriteOnStart
        {
            get => writeOnStart;
            set => writeOnStart = value;
        }

        public bool WriteOnNewText
        {
            get => writeOnNewText;
            set => writeOnNewText = value;
        }

        public bool UseScaledTime
        {
            get => useScaledTime;
            set => useScaledTime = value;
        }

        public Delays DefaultDelays => delays;
        public Delays CurrentDelays => currentDelays;

        // Events
        /// <summary>
        /// Raised when the TMPWriter reaches an event tag.
        /// </summary>
        public TMPEvent OnTextEvent;
        /// <summary>
        /// Raised when the TMPWriter shows a new character.
        /// </summary>
        public UnityEvent<TMPWriter, CharData> OnCharacterShown;
        /// <summary>
        /// Raised when the TMPWriter starts / resumes writing.
        /// </summary>
        public UnityEvent<TMPWriter> OnStartWriter;
        /// <summary>
        /// Raised when the TMPWriter stops writing.
        /// </summary>
        public UnityEvent<TMPWriter> OnStopWriter;
        /// <summary>
        /// Raised when the TMPWriter is done writing the current text.
        /// </summary>
        public UnityEvent<TMPWriter> OnFinishWriter;
        /// <summary>
        /// Raised when the current (section of) text is skipped.
        /// </summary>
        public UnityEvent<TMPWriter, int> OnSkipWriter;
        /// <summary>
        /// Raised when the TMPWriter is reset.<br/>
        /// The integer parameter indicates the text index the TMPWriter was reset to.
        /// </summary>
        public UnityEvent<TMPWriter, int> OnResetWriter;

        /// <summary>
        /// The prefix used for command tags.
        /// </summary>
        public const char COMMAND_PREFIX = '!';
        /// <summary>
        /// The prefix used for event tags.
        /// </summary>
        public const char EVENT_PREFIX = '?';

        #region Fields
        // Settings
        [Tooltip("The database used to process command tags (e.g. <!delay=0.05>")]
        [SerializeField] TMPCommandDatabase database;

        //[Tooltip("The delay between new characters shown by the writer, i.e. the inverse of the speed of the writer.")]
        //[SerializeField] private float delay = 0.075f;

        [Tooltip("Whether the text may be skipped by default.")]
        [SerializeField] private bool maySkip = true;

        [Tooltip("If checked, the writer will begin writing when it is first enabled. If not checked, you will have to manually start the writer from your own code.")]
        [SerializeField] private bool writeOnStart = true;
        [Tooltip("If checked, the writer will automatically begin writing when the text on the associated TMP_Text component is modified. If not checked, you will have to manually start the writer from your own code.")]
        [SerializeField] private bool writeOnNewText = true;
        [Tooltip("Whether the writer should use scaled time to wait for delays and wait commands.")]
        [SerializeField] private bool useScaledTime = true;

        [SerializeField] private Delays delays = new Delays();

        [Tooltip("Commands that may reference scene objects.\nNOT raised in preview mode.")]
        [SerializeField, SerializedDictionary(keyName: "Name", valueName: "Command")]
        private SerializedDictionary<string, TMPSceneCommand> sceneCommands;

        [System.NonSerialized] private TagProcessorManager processors;
        [System.NonSerialized] private TagCollectionManager<TMPEffectCategory> tags;

        [System.NonSerialized] private TMPCommandCategory commandCategory;
        [System.NonSerialized] private TMPEventCategory eventCategory;

        [System.NonSerialized] private CommandDatabase commandDatabase;

        [System.NonSerialized] private CachedCollection<CachedCommand> commands;
        [System.NonSerialized] private CachedCollection<CachedEvent> events;

        [System.NonSerialized] private Coroutine writerCoroutine = null;
        [System.NonSerialized] private bool currentMaySkip;
        [System.NonSerialized] private Delays currentDelays;

        [System.NonSerialized] private bool shouldWait = false;
        [System.NonSerialized] private float waitAmount = 0f;
        [System.NonSerialized] private Func<bool> continueConditions;

        [System.NonSerialized] private bool writing = false;
        [System.NonSerialized] private int currentIndex = -1;
        #endregion

        #region Public methods

        #region Writer Controlling
        /// <summary>
        /// Start (or resume) writing.
        /// </summary>
        public void StartWriter()
        {
            if (Mediator == null/*!isActiveAndEnabled || !gameObject.activeInHierarchy*/)
            {
                Debug.LogWarning($"The TMPWriter component on {gameObject.name} is not enabled");
                return;
            }

            if (!writing)
            {
                RaiseStartWriterEvent();
                StartWriterCoroutine();
            }
        }

        /// <summary>
        /// Stop writing.<br/>
        /// Note that this does not reset the shown text.
        /// </summary>
        public void StopWriter()
        {
            if (Mediator == null/*!isActiveAndEnabled || !gameObject.activeInHierarchy*/)
            {
                Debug.LogWarning($"The TMPWriter component on {gameObject.name} is not enabled");
                return;
            }

            if (writing)
            {
                StopWriterCoroutine();
                RaiseStopWriterEvent();
            }

        }

        /// <summary>
        /// Reset the writer to the initial state for the current text.<br/>
        /// This also stops the writing process.
        /// </summary>
        public void ResetWriter()
        {
            if (Mediator == null/*!isActiveAndEnabled || !gameObject.activeInHierarchy*/)
            {
                Debug.LogWarning($"The TMPWriter component on {gameObject.name} is not enabled");
                return;
            }

            if (writing)
            {
                StopWriterCoroutine();
            }

            // reset
            currentIndex = -1;
            ResetInvokables(Mediator.CharData.Count);
            Hide(0, Mediator.CharData.Count, true);

            ResetData();

            RaiseResetWriterEvent(0);
        }

        /// <summary>
        /// Reset the writer to the given index of the current text.<br/>
        /// Does not allow you to skip text; the passed index must be smaller than
        /// the current index.
        /// </summary>
        /// <param name="index">The index to reset the writer to.</param>
        public void ResetWriter(int index)
        {
            if (Mediator == null/*!isActiveAndEnabled || !gameObject.activeInHierarchy*/)
            {
                Debug.LogWarning($"The TMPWriter component on {gameObject.name} is not enabled");
                return;
            }

            if (index >= currentIndex)
            {
                Debug.LogWarning($"Can't reset the TMPWriter on {gameObject.name} to index {index}; current index is only {currentIndex}");
                return;
            }

            if (writing)
            {
                StopWriterCoroutine();
            }

            // reset
            ResetData();

            Hide(0, Mediator.CharData.Count, true);
            Show(0, index, true);

            ResetInvokables(currentIndex);
            for (int i = -1; i < index; i++)
            {
                RaiseInvokables(i);
            }

            currentIndex = index;

            RaiseResetWriterEvent(index);
        }

        /// <summary>
        /// Skip the current section of the text.<br/>
        /// If the current section may not be skipped, this will do nothing.<br/>
        /// Otherwise, the writing process is skipped to either the end of the current text, or the next unskippable section of the current text.
        /// </summary>
        public void SkipWriter(bool skipShowAnimation = true)
        {
            if (Mediator == null/*!isActiveAndEnabled || !gameObject.activeInHierarchy*/)
            {
                Debug.LogWarning($"The TMPWriter component on {gameObject.name} is not enabled");
                return;
            }

            if (!currentMaySkip)
            {
                Debug.LogWarning($"The TMPWriter component on {gameObject.name} may not skip at the current index");
                return;
            }

            int skipTo;
            CachedCommand cc = commands.FirstOrDefault(x => x.Indices.StartIndex >= currentIndex && x.Tag.Name == "skippable" && x.Tag.Parameters != null && x.Tag.Parameters[""] == "false");

            if (cc == default) skipTo = Mediator.CharData.Count;
            else skipTo = cc.Indices.StartIndex;

            RaiseSkipWriterEvent(skipTo);

            for (int i = currentIndex; i < skipTo; i++)
            {
                RaiseInvokables(i, true);
            }

            currentIndex = skipTo;
            Show(0, skipTo, skipShowAnimation);

            if (skipTo == Mediator.CharData.Count)
            {
                if (writing) StopWriterCoroutine();
                RaiseFinishWriterEvent();
            }
        }

        /// <summary>
        /// Restart the writer.<br/>
        /// This will reset the writer and start the writing process.
        /// </summary>
        public void RestartWriter()
        {
            if (Mediator == null)
            {
                Debug.LogWarning($"The TMPWriter component on {gameObject.name} is not enabled");
                return;
            }

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

        public void ResetWaitPeriod()
        {
            shouldWait = false;
            waitAmount = 0f;
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

        public void ResetWaitConditions()
        {
            continueConditions = null;
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


        #endregion

        #region Initialization
        private void OnEnable()
        {
            UpdateMediator();

            SubscribeToMediator();

            PrepareForProcessing();

            if (database != null)
            {
                database.ObjectChanged += ReprocessOnDatabaseChange;
            }

            Mediator.ForceReprocess();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorApplication.update -= EditorUpdate;
                EditorApplication.update += EditorUpdate;
            }
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

            commandDatabase?.Dispose();

            UnsubscribeFromMediator();

            //#if UNITY_EDITOR
            //            if (!Application.isPlaying)
            //            {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
            StopWriterCoroutine();
            currentIndex = -1;
            Show(0, Mediator.CharData.Count, true);
            writing = false;
            //            }
            //#endif

            var textComponent = Mediator.Text;
            FreeMediator();
            if (textComponent != null) textComponent.ForceMeshUpdate(false, true);

#if UNITY_EDITOR
            // Queue a player loop update to instantly update scene view
            EditorApplication.delayCall += EditorApplication.QueuePlayerLoopUpdate;
#endif
        }

        private void PrepareForProcessing()
        {
            // Reset database wrappers
            commandDatabase?.Dispose();
            commandDatabase = new CommandDatabase(database == null ? null : database, sceneCommands);
            commandDatabase.ObjectChanged += ReprocessOnDatabaseChange;

            // Reset categories
            commandCategory = new TMPCommandCategory(COMMAND_PREFIX, commandDatabase);
            eventCategory = new TMPEventCategory(EVENT_PREFIX);

            // Reset tagcollection & cachedcollection
            //ReadOnlyCollection<CharData> ro = new ReadOnlyCollection<CharData>(Mediator.CharData);
            //tags = new();
            //commands = new CachedCollection<CachedCommand>(new CommandCacher(ro, this, commandCategory), tags.AddKey(commandCategory));
            //events = new CachedCollection<CachedEvent>(new EventCacher(this, OnTextEvent), tags.AddKey(eventCategory));

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
            Mediator.TextChanged_Late += OnTextChanged_Late;
            Mediator.TextChanged_Early += OnTextChanged_Early;
        }

        private void UnsubscribeFromMediator()
        {
            Mediator.TextChanged_Late -= OnTextChanged_Late;
            Mediator.TextChanged_Early -= OnTextChanged_Early;
        }

        private void OnDatabaseChanged()
        {
            PrepareForProcessing();
            Mediator.ForceReprocess();
        }

        private void ReprocessOnDatabaseChange(object sender)
        {
            PrepareForProcessing();
            Mediator.ForceReprocess();
        }
        #endregion

        #region Event Callbacks and Wrappers
        [System.NonSerialized] private bool tagsChanged = false;
        private void OnTextChanged_Early(bool textContentChanged, ReadOnlyCollection<CharData> oldCharData)
        {
            tagsChanged = tags == null;
            if (!textContentChanged && tags != null)
            {
                tagsChanged = true;

                var oldTags = tags.ContainsKey(commandCategory) ? CommandTags : Enumerable.Empty<TMPEffectTagTuple>();
                var newTags = processors.TagProcessors[commandCategory.Prefix].SelectMany(processed => processed.ProcessedTags).Select(tag => new TMPEffectTagTuple(tag.Value, tag.Key));

                if (oldTags.SequenceEqual(newTags))
                {
                    oldTags = tags.ContainsKey(eventCategory) ? EventTags : Enumerable.Empty<TMPEffectTagTuple>();
                    newTags = processors.TagProcessors[eventCategory.Prefix].SelectMany(processed => processed.ProcessedTags).Select(tag => new TMPEffectTagTuple(tag.Value, tag.Key));

                    if (oldTags.SequenceEqual(newTags))
                    {
                        tagsChanged = false;
                    }
                }
            }

            PostProcessTags();
        }

        private void OnTextChanged_Late(bool textContentChanged, ReadOnlyCollection<CharData> oldCharData, ReadOnlyCollection<VisibilityState> oldVisibilities)
        {
            if (!textContentChanged && !tagsChanged)
            {
                return;
            }

#if UNITY_EDITOR
            // If is preview 
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

            bool waswriting = writing;
            ResetWriter();
            if (waswriting || writeOnNewText) StartWriter();
            return;
        }

        private void RaiseCharacterShownEvent(CharData cData)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                OnCharacterShownPreview?.Invoke(this, cData);
                return;
            }
#endif

            OnCharacterShown?.Invoke(this, cData);
        }

        private void RaiseResetWriterEvent(int index)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                OnResetWriterPreview?.Invoke(this, index);
                return;
            }
#endif

            OnResetWriter?.Invoke(this, index);
        }

        private void RaiseFinishWriterEvent()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                OnFinishWriterPreview?.Invoke(this);
                return;
            }
#endif

            OnFinishWriter?.Invoke(this);
        }

        private void RaiseStartWriterEvent()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                OnStartWriterPreview?.Invoke(this);
                return;
            }
#endif

            OnStartWriter?.Invoke(this);
        }

        private void RaiseStopWriterEvent()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                OnStopWriterPreview?.Invoke(this);
                return;
            }
#endif

            OnStopWriter?.Invoke(this);
        }

        private void RaiseSkipWriterEvent(int index)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                OnSkipWriterPreview?.Invoke(this, index);
                return;
            }
#endif

            OnSkipWriter?.Invoke(this, index);
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

            // The accumulated excess waiting time (due to WaitForSeconds and frame-timing)
            float excessWaitedTime = 0f;
            float prevTime, tempTime;
            bool prevScaled = useScaledTime;

            // This indicates the text is fully shown already
            if (currentIndex >= Mediator.CharData.Count)
            {
                OnStopWriting();
                yield break;
            }

            // If you begin writing from the very start of the text,
            // reset all relevant variables
            if (currentIndex <= 0) ResetData();

            if (currentIndex == -1)
                HideAllCharacters(true);

            // Execute instant commands
            var invokables = GetInvokables(-1);
            foreach (var invokable in invokables)
            {
                waitAmount = 0f;
                shouldWait = false;
                continueConditions = null;

                invokable.Trigger();

                prevTime = useScaledTime ? Time.time : Time.unscaledTime;
                if (shouldWait && waitAmount > 0) yield return useScaledTime ? new WaitForSeconds(waitAmount) : new WaitForSecondsRealtime(waitAmount);
                FixTimePost(waitAmount);

                if (Mediator == null) yield break;
                if (continueConditions != null) yield return HandleWaitConditions();
                if (Mediator == null) yield break;
            }

            yield return null;

            // Iterate over all characters / indices in the text
            CharData cData;
            for (int i = Mathf.Max(currentIndex, 0); i < Mediator?.CharData.Count; i++) // .? and other null checks because coroutines are not instantly cancelled (so disable writer => NRE)
            {
                currentIndex = i;
                cData = Mediator.CharData[i];

                // Execute the commands & events for the current index
                invokables = GetInvokables(i);
                foreach (var invokable in invokables)
                {
                    waitAmount = 0f;
                    shouldWait = false;
                    continueConditions = null;

                    invokable.Trigger();

                    // Wait for the given amount of time, and accomodate for excess wait time (frame-timing)
                    FixTimePre(ref waitAmount);
                    if (shouldWait && waitAmount > 0) yield return useScaledTime ? new WaitForSeconds(waitAmount) : new WaitForSecondsRealtime(waitAmount);
                    FixTimePost(waitAmount);
                    if (Mediator == null) yield break;

                    // Wait until all wait conditions are true
                    if (continueConditions != null) yield return HandleWaitConditions();
                    if (Mediator == null) yield break;
                }

                // Calculate and wait for the delay for the current index, and accomodate for excess wait time (frame-timing)
                float delay = CalculateDelay(i);
                FixTimePre(ref delay);
                if (delay > 0)
                {
                    yield return useScaledTime ? new WaitForSeconds(delay) : new WaitForSecondsRealtime(delay);
                    if (Mediator == null) yield break;
                }
                FixTimePost(delay);

                // Show the current character, if it is not already shown
                VisibilityState vState = Mediator.VisibilityStates[i];
                if (vState == VisibilityState.Hidden || vState == VisibilityState.Hiding)
                {
                    RaiseCharacterShownEvent(cData);
                    Show(i, 1, false);
                }
            }

            if (Mediator == null)
            {
                RaiseFinishWriterEvent();
                OnStopWriting();
                yield break;
            }

            // TODO 
            // This originally was required to raise commands/events at the very end
            // of a text, e.g. "Lorem ipsum<?event>"
            // Currently the preprocessor adds a space character to the end of every text
            // (originally to fix a bug related to the TMP_Text not updating correctly for empty texts)
            // If that changes, this will be required again!
            //invokables = GetInvokables(Mediator.CharData.Count);
            //foreach (var invokable in invokables)
            //{
            //    waitAmount = 0f;
            //    shouldWait = false;
            //    continueConditions = null;

            //    invokable.Trigger();

            //    FixTimePre(ref waitAmount);
            //    if (shouldWait && waitAmount > 0) yield return useScaledTime ? new WaitForSeconds(waitAmount) : new WaitForSecondsRealtime(waitAmount);

            //    if (Mediator == null) yield break;
            //    if (continueConditions != null) yield return HandleWaitConditions();
            //    if (Mediator == null) yield break;
            //}

            RaiseFinishWriterEvent();
            OnStopWriting();

            // Fix excess time and passed in wait amount so both take each other into account
            void FixTimePre(ref float time)
            {
                tempTime = time;
                time = Mathf.Max(0f, time - excessWaitedTime);
                excessWaitedTime = Mathf.Max(0f, excessWaitedTime - tempTime);
                prevScaled = useScaledTime;
                prevTime = useScaledTime ? Time.time : Time.unscaledTime;
            }

            // Fix excess time to account for the actual waited time
            void FixTimePost(float time)
            {
                excessWaitedTime += (prevScaled ? Time.time : Time.unscaledTime) - prevTime - time;
            }
        }

        private void ResetData()
        {
            currentDelays = new Delays();
            currentDelays.delay = delays.delay;
            currentDelays.whitespaceDelay = delays.whitespaceDelay;
            currentDelays.whitespaceDelayType = delays.whitespaceDelayType;
            currentDelays.linebreakDelay = delays.linebreakDelay;
            currentDelays.linebreakDelayType = delays.linebreakDelayType;
            currentDelays.punctuationDelay = delays.punctuationDelay;
            currentDelays.punctuationDelayType = delays.punctuationDelayType;
            currentDelays.visibleDelay = delays.visibleDelay;
            currentDelays.visibleDelayType = delays.visibleDelayType;

            currentMaySkip = maySkip;
        }

        private void ResetInvokables(int maxIndex)
        {
            foreach (var eventt in events)
            {
                if ((eventt.Indices.StartIndex <= maxIndex || eventt.ExecuteInstantly) && eventt.ExecuteRepeatable)
                {
                    eventt.Reset();
                }
            }

            foreach (var command in commands)
            {
                if ((command.Indices.StartIndex <= maxIndex || command.ExecuteInstantly) && command.ExecuteRepeatable)
                {
                    command.Reset();
                }
            }
        }

        private float CalculateDelay(int index)
        {
            CharData cData = Mediator.CharData[index];

            // If character is invisible (=> is whitespace)
            if (!cData.info.isVisible) // Alternatively check char.IsWhiteSpace
            {
                // If line break
                if (cData.info.character == '\n')
                {
                    return Mathf.Max(currentDelays.CalculatedLinebreakDelay, 0);
                }

                return Mathf.Max(currentDelays.CalculatedWhiteSpaceDelay, 0);
            }

            // If character is already shown (e.g. through using the !show command)
            VisibilityState vState = Mediator.GetVisibilityState(cData);
            if (vState == VisibilityState.Shown || vState == VisibilityState.Showing)
            {
                return Mathf.Max(currentDelays.CalculatedVisibleDelay, 0);
            }

            // If character is punctuation, and not directly followed by another punctuation (to multiple extended delays for e.g. "..." or "?!?"
            if (char.IsPunctuation(cData.info.character) && (index == Mediator.CharData.Count - 1 || !char.IsPunctuation(Mediator.CharData[index + 1].info.character)))
            {
                return Mathf.Max(currentDelays.CalculatedPunctuationDelay, 0);
            }

            return currentDelays.delay;
        }

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

                if (!allMet) yield return null;

            } while (!allMet && continueConditions != null);

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
            waitAmount = 0f;
            shouldWait = false;
            continueConditions = null;

            foreach (var invokable in GetInvokables(index, skipped))
            {
                invokable.Trigger();

                if (block)
                {
                    if (shouldWait) yield return new WaitForSeconds(waitAmount);
                    if (continueConditions != null) yield return HandleWaitConditions();
                }

                waitAmount = 0f;
                shouldWait = false;
                continueConditions = null;
            }
        }

        private IEnumerator RaiseInvokablesCoroutine(IEnumerable<ICachedInvokable> invokables, bool block = true)
        {
            waitAmount = 0f;
            shouldWait = false;
            continueConditions = null;

            foreach (var invokable in invokables)
            {
                invokable.Trigger();

                if (block)
                {
                    if (shouldWait) yield return new WaitForSeconds(waitAmount);
                    if (continueConditions != null) yield return HandleWaitConditions();
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
#pragma warning disable CS0414
        //[System.NonSerialized] bool reprocessFlag = false;
        [SerializeField, HideInInspector] bool useDefaultDatabase = true;
        [SerializeField, HideInInspector] bool initDatabase = false;
        [Tooltip("Raise text events in preview mode?")]
        [SerializeField, HideInInspector] bool eventsEnabled = false;
        [Tooltip("Raise commands in preview mode?")]
        [SerializeField, HideInInspector] bool commandsEnabled = true;
#pragma warning restore CS0414

        internal delegate void CharDataHandler(TMPWriter writer, CharData cData);
        internal delegate void IntHandler(TMPWriter writer, int index);
        internal delegate void VoidHandler(TMPWriter writer);
        internal delegate void ResetHandler();
        internal event CharDataHandler OnCharacterShownPreview;
        internal event IntHandler OnResetWriterPreview;
        internal event IntHandler OnSkipWriterPreview;
        internal event VoidHandler OnFinishWriterPreview;
        internal event VoidHandler OnStartWriterPreview;
        internal event VoidHandler OnStopWriterPreview;
        internal event ResetHandler OnResetComponent;


        private void EditorUpdate()
        {
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

        internal void OnChangedDatabase()
        {
            if (Mediator == null) return;
            OnDatabaseChanged();
        }

        private void OnValidate()
        {
            delays.delay = Mathf.Max(delays.delay, 0);
            delays.linebreakDelay = Mathf.Max(delays.linebreakDelay, 0);
            delays.whitespaceDelay = Mathf.Max(delays.whitespaceDelay, 0);
            delays.visibleDelay = Mathf.Max(delays.visibleDelay, 0);
            delays.punctuationDelay = Mathf.Max(delays.punctuationDelay, 0);
        }

        private void Reset()
        {
            ResetWriter();
            ShowAll(true);

            if (enabled)
            {
                enabled = false;
                EditorApplication.delayCall += () => this.enabled = true;
                EditorApplication.delayCall += () => EditorApplication.delayCall += EditorApplication.QueuePlayerLoopUpdate;
            }

            OnResetComponent?.Invoke();
        }

        internal void SkipPlayer()
        {
            int skipTo = Mediator.CharData.Count;

            for (int i = currentIndex; i < skipTo; i++)
            {
                RaiseInvokables(i, true);
            }

            currentIndex = skipTo;
            Show(0, skipTo, true);

            if (skipTo == Mediator.CharData.Count)
            {
                if (writing) StopWriterCoroutine();
            }
        }

        internal void SetWriter(int index)
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

            ResetInvokables(currentIndex);
            for (int i = -1; i < index; i++)
            {
                RaiseInvokables(i);
            }

            currentIndex = index;

            RaiseResetWriterEvent(index);
        }
#endif
        #endregion

        /// <summary>
        /// The type of delay.<br/>
        /// Either a percentage of the normal delay (<see cref="Delays.delay"/>) or a raw value in seconds.
        /// </summary>
        public enum DelayType
        {
            Percentage,
            Raw
        }

        /// <summary>
        /// Stores the various delays along with their <see cref="DelayType"/> of a TMPWriter.
        /// </summary>
        [System.Serializable]
        public class Delays
        {
            /// <summary>
            /// The calculated delay after "showing" a whitespace character, using <see cref="whitespaceDelay"/> and <see cref="whitespaceDelayType"/>.
            /// </summary>
            public float CalculatedWhiteSpaceDelay => whitespaceDelayType == DelayType.Raw ? whitespaceDelay : delay * whitespaceDelay;
            /// <summary>
            /// The calculated delay after showing a punctuation character, using <see cref="punctuationDelay"/> and <see cref="punctuationDelayType"/>.
            /// </summary>
            public float CalculatedPunctuationDelay => punctuationDelayType == DelayType.Raw ? punctuationDelay : delay * punctuationDelay;
            /// <summary>
            /// The calculated delay after "showing" an already visible character, using <see cref="visibleDelay"/> and <see cref="visibleDelayType"/>.
            /// </summary>
            public float CalculatedVisibleDelay => visibleDelayType == DelayType.Raw ? visibleDelay : delay * visibleDelay;
            /// <summary>
            /// The calculated delay after "showing" a linebreak character, using <see cref="linebreakDelay"/> and <see cref="linebreakDelayType"/>.
            /// </summary>
            public float CalculatedLinebreakDelay => linebreakDelayType == DelayType.Raw ? linebreakDelay : delay * linebreakDelay;

            /// <summary>
            /// The delay after showing a character.
            /// </summary>
            [Tooltip("The delay between new characters shown by the writer, i.e. the inverse of the speed of the writer.")]
            public float delay = 0.035f;

           /// <summary>
           /// The delay after "showing" a whitespace character.
           /// </summary>
            [Tooltip("The delay after whitespace characters, either as percentage of the general delay or in seconds")]
            public float whitespaceDelay;
            /// <summary>
            /// The <see cref="DelayType"/> of <see cref="whitespaceDelay"/>.
            /// </summary>
            public DelayType whitespaceDelayType;

            /// <summary>
            /// The delay after "showing" a linebreak character.
            /// </summary>
            [Tooltip("The delay after linebreaks, either as percentage of the general delay or in seconds")]
            public float linebreakDelay;
            /// <summary>
            /// The <see cref="DelayType"/> of <see cref="linebreakDelay"/>.
            /// </summary>
            public DelayType linebreakDelayType;

            /// <summary>
            /// The delay after showing a punctuation character.
            /// </summary>
            [Tooltip("The delay after punctuation characters, either as percentage of the general delay or in seconds")]
            public float punctuationDelay;
            /// <summary>
            /// The <see cref="DelayType"/> of <see cref="punctuationDelay"/>.
            /// </summary>
            public DelayType punctuationDelayType;

            /// <summary>
            /// The delay after "showing" an already visible character.
            /// </summary>
            [Tooltip("The delay after already visible characters, either as percentage of the general delay or in seconds")]
            public float visibleDelay;
            /// <summary>
            /// The <see cref="DelayType"/> of <see cref="visibleDelay"/>.
            /// </summary>
            public DelayType visibleDelayType;

            /// <summary>
            /// Set the delay of the writer.
            /// </summary>
            /// <param name="delay">The delay after showing a character.</param>
            public void SetDelay(float delay)
            {
                this.delay = delay;
            }

            /// <summary>
            /// Set the whitespace delay of the writer.
            /// </summary>
            /// <param name="delay">The delay after "showing" a whitespace character.</param>
            public void SetWhitespaceDelay(float delay, DelayType? type = null)
            {
                whitespaceDelay = delay;
                if (type != null) whitespaceDelayType = type.Value;
            }

            /// <summary>
            /// Set the linebreak delay of the writer.
            /// </summary>
            /// <param name="delay">The delay after "showing" a linebreak character.</param>
            public void SetLinebreakDelay(float delay, DelayType? type = null)
            {
                linebreakDelay = delay;
                if (type != null) linebreakDelayType = type.Value;
            }

            /// <summary>
            /// Set the visible delay of the writer.
            /// </summary>
            /// <param name="delay">The delay after "showing" an already visible character.</param>
            public void SetVisibleDelay(float delay, DelayType? type = null)
            {
                visibleDelay = delay;
                if (type != null) visibleDelayType = type.Value;
            }

            /// <summary>
            /// Set the punctuation delay of the writer.
            /// </summary>
            /// <param name="delay">The delay after "showing" a punctuation character.</param>
            public void SetPunctuationDelay(float delay, DelayType? type = null)
            {
                punctuationDelay = delay;
                if (type != null) punctuationDelayType = type.Value;
            }
        }

        private void HideAllCharacters(bool skipAnimations = false)
        {
            Hide(0, Mediator.CharData.Count, skipAnimations);
        }

        private void PostProcessTags()
        {
            var kvpCommands = new KeyValuePair<TMPEffectCategory, IEnumerable<KeyValuePair<TMPEffectTagIndices, TMPEffectTag>>>(commandCategory, processors.TagProcessors[commandCategory.Prefix][0].ProcessedTags);
            var kvpEvents = new KeyValuePair<TMPEffectCategory, IEnumerable<KeyValuePair<TMPEffectTagIndices, TMPEffectTag>>>(eventCategory, processors.TagProcessors[eventCategory.Prefix][0].ProcessedTags);

            tags = new TagCollectionManager<TMPEffectCategory>(kvpCommands, kvpEvents);

            var commandCacher = new CommandCacher(Mediator.CharData, this, commandCategory);
            var eventCacher = new EventCacher(this, OnTextEvent);

            commands = new CachedCollection<CachedCommand>(commandCacher, tags[commandCategory]);
            events = new CachedCollection<CachedEvent>(eventCacher, tags[eventCategory]);
        }
    }
}
