using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using TMPEffects.TMPAnimations;
using TMPEffects.Databases;
using TMPEffects.TextProcessing;
using System;
using System.Collections.ObjectModel;
using TMPEffects.Extensions;
using TMPEffects.SerializedCollections;
using TMPEffects.EffectCategories;
using TMPEffects.Components.Animator;
using TMPEffects.Databases.AnimationDatabase;
using TMPEffects.Tags.Collections;
using TMPEffects.Tags;
using TMPEffects.Components.CharacterData;
using System.Collections.Specialized;
using TMPEffects.TMPAnimations.ShowAnimations;
using TMPEffects.TMPAnimations.HideAnimations;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace TMPEffects.Components
{
    /// <summary>
    /// Animates the character of a <see cref="TMP_Text"/> component.
    /// </summary>
    /// <remarks>
    /// One of the two main components of TMPEffects, along with <see cref="TMPWriter"/>.<br/>
    /// TMPAnimator allows you to apply animations to the characters of a <see cref="TMP_Text"/> component.<br/>
    /// There are three types of animations:
    /// <list type="table">
    /// <item><see cref="TMPAnimation"/>: The "basic" type of animation. Will animate the effected text continuously.</item>
    /// <item><see cref="TMPShowAnimation"/>: Will animate the effected text when it begins to be shown. Show animations are only applied if there is also a <see cref="TMPWriter"/> component on the same GameObject.</item>
    /// <item><see cref="TMPHideAnimation"/>: Will animate the effected text when it begins to be hidden. Hide animations are only applied if there is also a <see cref="TMPWriter"/> component on the same GameObject.</item>
    /// </list>    
    /// <br/>
    /// You may control when the animations are updated by setting <see cref="UpdateFrom"/> to <see cref="UpdateFrom.Script"/> and calling <see cref="UpdateAnimations(float)"/>.<br/>
    /// </remarks>
    [ExecuteAlways, DisallowMultipleComponent, RequireComponent(typeof(TMP_Text))]
    public class TMPAnimator : TMPEffectComponent
    {
        /// <summary>
        /// Whether the text is currently being animated.<br/>
        /// If <see cref="UpdateFrom"/> is set to <see cref="UpdateFrom.Script"/>, this will always evaluate to true.
        /// </summary>
        public bool IsAnimating => updateFrom == UpdateFrom.Script || isAnimating;

        /// <summary>
        /// The database used to parse animation tags.
        /// </summary>
        public TMPAnimationDatabase Database => database;

        /// <summary>
        /// Where the animations are currently being updated from.
        /// </summary>
        public UpdateFrom UpdateFrom => updateFrom;

        /// <summary>
        /// All tags parsed by the TMPAnimator.
        /// </summary>
        public ITagCollection Tags => tags;
        /// <summary>
        /// All basic animation tags parsed by the TMPAnimator.
        /// </summary>
        public ITagCollection BasicTags => tags[basicCategory];
        /// <summary>
        /// All show animation tags parsed by the TMPAnimator.
        /// </summary>
        public ITagCollection ShowTags => tags[showCategory];
        /// <summary>
        /// All hide animation tags parsed by the TMPAnimator.
        /// </summary>
        public ITagCollection HideTags => tags[hideCategory];

        /// <summary>
        /// Whether the TMPAnimator should automatically begin animating on <see cref="Start"/>.
        /// </summary>
        public bool AnimateOnStart { get => animateOnStart; set => animateOnStart = value; }
        /// <summary>
        /// Whether animations will override each other by default.
        /// </summary>
        public bool AnimationsOverride { get => animationsOverride; set => animationsOverride = value; }

        /// <summary>
        /// The prefix used for basic animation tags.
        /// </summary>
        public const char ANIMATION_PREFIX = '\0';
        /// <summary>
        /// The prefix used for show animation tags.
        /// </summary>
        public const char SHOW_ANIMATION_PREFIX = '+';
        /// <summary>
        /// The prefix used for hide animation tags.
        /// </summary>
        public const char HIDE_ANIMATION_PREFIX = '-';

        private float GetVisibleTime(int index) => visibleTimes[index];

        #region Fields
        [SerializeField] private TMPAnimationDatabase database = null;
        [SerializeField] private AnimatorContext context = new AnimatorContext();

        [SerializeField] private UpdateFrom updateFrom = UpdateFrom.Update;
        [SerializeField] private bool animateOnStart = true;

        [SerializeField] private bool animationsOverride = false;
        [SerializeField] private string defaultShowString = "";
        [SerializeField] private string defaultHideString = "";

        [SerializeField] private string excludedCharacters = "";
        [SerializeField] private string excludedCharactersShow = "";
        [SerializeField] private string excludedCharactersHide = "";

        [SerializeField] private bool excludePunctuation = false;
        [SerializeField] private bool excludePunctuationShow = false;
        [SerializeField] private bool excludePunctuationHide = false;

        [SerializeField, SerializedDictionary(keyName: "Name", valueName: "Animation")]
        private SerializedObservableDictionary<string, TMPSceneAnimation> sceneAnimations;
        [SerializeField, SerializedDictionary(keyName: "Name", valueName: "Show Animation")]
        private SerializedObservableDictionary<string, TMPSceneShowAnimation> sceneShowAnimations;
        [SerializeField, SerializedDictionary(keyName: "Name", valueName: "Hide Animation")]
        private SerializedObservableDictionary<string, TMPSceneHideAnimation> sceneHideAnimations;

        [System.NonSerialized] private TagProcessorManager processors;
        [System.NonSerialized] private TagCollectionManager<TMPAnimationCategory> tags;
        [System.NonSerialized] private bool isAnimating = false;
        [System.NonSerialized] private bool ignoreVisibilityChanges = false;

        [System.NonSerialized] private TMPAnimationCategory basicCategory;
        [System.NonSerialized] private TMPAnimationCategory showCategory;
        [System.NonSerialized] private TMPAnimationCategory hideCategory;

        [System.NonSerialized] private AnimationDatabase<TMPBasicAnimationDatabase, TMPSceneAnimation> basicDatabase;
        [System.NonSerialized] private AnimationDatabase<TMPShowAnimationDatabase, TMPSceneShowAnimation> showDatabase;
        [System.NonSerialized] private AnimationDatabase<TMPHideAnimationDatabase, TMPSceneHideAnimation> hideDatabase;

        [System.NonSerialized] private CachedCollection<CachedAnimation> basic;
        [System.NonSerialized] private CachedCollection<CachedAnimation> show;
        [System.NonSerialized] private CachedCollection<CachedAnimation> hide;

        [System.NonSerialized] private CachedAnimation dummyShow;
        [System.NonSerialized] private CachedAnimation dummyHide;

        [System.NonSerialized] private CachedAnimation defaultShow;
        [System.NonSerialized] private CachedAnimation defaultHide;

        [System.NonSerialized] private List<float> visibleTimes = new List<float>();
        [System.NonSerialized] private List<float> stateTimes = new List<float>();
        [System.NonSerialized] private object timesIdentifier;

        private const string falseUpdateAnimationsCallWarning = "Called UpdateAnimations while TMPAnimator {0} is set to automatically update from {1}; " +
            "If you want to manually control the animation updates, set its UpdateFrom property to \"Script\", " +
            "either through the inspector or through a script using the SetUpdateFrom method.";
        private const string falseStartStopAnimatingCallWarning = "Called {0} while TMPAnimator {1} is set to manually update from script; " +
            "If you want the TMPAnimator to automatically update and to use the Start / StopAnimating methods, set its UpdateFrom property to \"Update\", \"LateUpdate\" or \"FixedUpdate\", " +
            "either through the inspector or through a script using the SetUpdateFrom method.";
        #endregion

        #region Initialization
        private void OnEnable()
        {
            UpdateMediator();

            CreateContext();

            PrepareForProcessing();

            SubscribeToMediator();

            Mediator.ForceReprocess();

            SetDummies();
            SetDefault(TMPAnimationType.Show);
            SetDefault(TMPAnimationType.Hide);

#if UNITY_EDITOR
            if (preview && !Application.isPlaying) StartPreview();
#endif
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            if (animateOnStart && updateFrom != UpdateFrom.Script) StartAnimating();
        }

        private void OnDisable()
        {
            processors.UnregisterFrom(Mediator.Processor);

            Mediator.ForceReprocess();

#if UNITY_EDITOR
            StopPreview();
#endif

            UnsubscribeFromMediator();
        }

        private void OnDestroy()
        {
            basicDatabase?.Dispose();
            showDatabase?.Dispose();
            hideDatabase?.Dispose();
        }

        private void SubscribeToMediator()
        {
            timesIdentifier = new object();
            if (!Mediator.RegisterVisibilityProcessor(timesIdentifier))
            {
                Debug.LogError("Could not register as visibility processor!");
            }

            Mediator.CharDataPopulated += SetDummies; // Set the dummy show / hide animation 
            Mediator.CharDataPopulated += PopulateTimes; // Populate the timing datas
            Mediator.VisibilityStateUpdated += OnVisibilityStateUpdated; // Handle Visibility updates
            Mediator.CharDataPopulated += PostProcessTags; //
            Mediator.TextChanged += OnTextChanged; // Will update animations once; otherwise, depending on timing of text change, there'll be a frame of unanimated text
        }

        private void UnsubscribeFromMediator()
        {
            Mediator.CharDataPopulated -= SetDummies;
            Mediator.CharDataPopulated -= PopulateTimes;
            Mediator.VisibilityStateUpdated -= OnVisibilityStateUpdated;
            Mediator.CharDataPopulated -= PostProcessTags;
            Mediator.TextChanged -= OnTextChanged;

            if (!Mediator.UnregisterVisibilityProcessor(timesIdentifier))
            {
                Debug.LogError("Could not unregister as visibility processor!");
            }

            FreeMediator();
        }

        private void CreateContext()
        {
            context.VisibleTime = (i) => visibleTimes[i];
            context.StateTime = (i) => stateTimes[i];
        }

        private void PrepareForProcessing()
        {
            // Reset database wrappers
            basicDatabase?.Dispose();
            showDatabase?.Dispose();
            hideDatabase?.Dispose();
            basicDatabase = new AnimationDatabase<TMPBasicAnimationDatabase, TMPSceneAnimation>(database == null ? null : database.BasicAnimationDatabase, sceneAnimations);
            showDatabase = new AnimationDatabase<TMPShowAnimationDatabase, TMPSceneShowAnimation>(database == null ? null : database.ShowAnimationDatabase, sceneShowAnimations);
            hideDatabase = new AnimationDatabase<TMPHideAnimationDatabase, TMPSceneHideAnimation>(database == null ? null : database.HideAnimationDatabase, sceneHideAnimations);
            basicDatabase.ObjectChanged += ReprocessOnDatabaseChange;
            showDatabase.ObjectChanged += ReprocessOnDatabaseChange;
            hideDatabase.ObjectChanged += ReprocessOnDatabaseChange;

            // Reset categories
            basicCategory = new TMPAnimationCategory(ANIMATION_PREFIX, basicDatabase);
            showCategory = new TMPAnimationCategory(SHOW_ANIMATION_PREFIX, showDatabase);
            hideCategory = new TMPAnimationCategory(HIDE_ANIMATION_PREFIX, hideDatabase);

            // Reset tagcollection & cachedcollection
            if (tags != null) tags.CollectionChanged -= OnTagCollectionChanged;
            tags = new TagCollectionManager<TMPAnimationCategory>();
            tags.CollectionChanged += OnTagCollectionChanged;

            var roCData = new ReadOnlyCollection<CharData>(Mediator.CharData);
            basic = new CachedCollection<CachedAnimation>(new AnimationCacher(basicCategory, state, context, roCData, (x) => !IsExcludedBasic(x)), tags.AddKey(basicCategory));
            show = new CachedCollection<CachedAnimation>(new AnimationCacher(showCategory, state, context, roCData, (x) => !IsExcludedShow(x)), tags.AddKey(showCategory));
            hide = new CachedCollection<CachedAnimation>(new AnimationCacher(hideCategory, state, context, roCData, (x) => !IsExcludedHide(x)), tags.AddKey(hideCategory));

            // Reset processors
            processors ??= new();
            processors.UnregisterFrom(Mediator.Processor);
            processors.Clear();

            processors.AddProcessor(basicCategory.Prefix, new TagProcessor(basicCategory));
            processors.AddProcessor(showCategory.Prefix, new TagProcessor(showCategory));
            processors.AddProcessor(hideCategory.Prefix, new TagProcessor(hideCategory));

            processors.RegisterTo(Mediator.Processor);
        }

        private void OnTagCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    int min = Mediator.CharData.Count;
                    int max = 0;

                    EffectTagTuple tuple;
                    foreach (var ad in e.OldItems)
                    {
                        tuple = (EffectTagTuple)ad;

                        if (tuple.Indices.StartIndex < min) min = tuple.Indices.StartIndex;
                        if (tuple.Indices.EndIndex > max) max = tuple.Indices.EndIndex;
                    }

                    for (int i = min; i <= max; i++)
                    {
                        UpdateCharacterAnimation(Mediator.CharData[i], 0f, i, false, true);
                    }

                    if (Mediator.Text.mesh != null)
                        Mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

                    break;
            }
        }

        private void SetDefault(TMPAnimationType type)
        {
            string str;

            switch (type)
            {
                case TMPAnimationType.Show:
                    defaultShow = dummyShow;
                    str = defaultShowString;
                    break;

                case TMPAnimationType.Hide:
                    defaultHide = dummyHide;
                    str = defaultHideString;
                    break;

                default:
                    throw new System.ArgumentException(nameof(type));
            }

            ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();
            Dictionary<string, string> tagParams;
            ITMPAnimation animation;

            if (string.IsNullOrWhiteSpace(str))
            {
                SetToDummy();
                return;
            }
            str = (str.Trim()[0] == '<' ? str : "<" + str + ">");
            if (!ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo, ParsingUtility.TagType.Open) || !database.ContainsEffect(tagInfo.name, type))
            {
                SetToDummy();
                return;
            }

            if ((animation = database.GetEffect(tagInfo.name, type)) == null)
            {
                SetToDummy();
                return;
            }

            tagParams = ParsingUtility.GetTagParametersDict(str);
            if (!animation.ValidateParameters(tagParams))
            {
                SetToDummy();
                return;
            }

            AnimationCacher cacher;
            switch (type)
            {
                case TMPAnimationType.Show:
                    cacher = new AnimationCacher(database?.ShowAnimationDatabase, state, context, new ReadOnlyCollection<CharData>(Mediator.CharData), x => !IsExcludedShow(x));
                    defaultShow = cacher.CacheTag(new EffectTag(tagInfo.name, tagInfo.prefix, tagParams), new EffectTagIndices(0, -1, 0));
                    break;

                case TMPAnimationType.Hide:
                    cacher = new AnimationCacher(database?.HideAnimationDatabase, state, context, new ReadOnlyCollection<CharData>(Mediator.CharData), x => !IsExcludedHide(x));
                    defaultHide = cacher.CacheTag(new EffectTag(tagInfo.name, tagInfo.prefix, tagParams), new EffectTagIndices(0, -1, 0));
                    break;
            }


            void SetToDummy()
            {
                switch (type)
                {
                    case TMPAnimationType.Show: defaultShow = dummyShow; break;
                    case TMPAnimationType.Hide: defaultHide = dummyHide; break;
                }
            }
        }

        private void OnDatabaseChanged(TMPAnimationDatabase previousDatabase)
        {
            //if (previousDatabase != null)
            //{
            //    previousDatabase.StopListenForChanges(ReprocessOnDatabaseChange);
            //}

            //if (database != null)
            //{
            //    database.StopListenForChanges(ReprocessOnDatabaseChange);
            //    database.ListenForChanges(ReprocessOnDatabaseChange);
            //}

            PrepareForProcessing();
            Mediator.ForceReprocess();
        }

        private void ReprocessOnDatabaseChange(object sender)
        {
            PrepareForProcessing();
            Mediator.ForceReprocess();
        }

        private void SetDummies()
        {
            SetDummyShow();
            SetDummyHide();
        }

        private void SetDummyShow()
        {
            //if (dummyShow != null) return;
            EffectTag tag = new EffectTag("Dummy Show Animation", ' ', null);

            DummyDatabase database = new DummyDatabase("Dummy Show Animation", ScriptableObject.CreateInstance<DummyShowAnimation>());
            AnimationCacher cacher = new AnimationCacher(database, state, context, new ReadOnlyCollection<CharData>(Mediator.CharData), (x) => !IsExcludedShow(x));
            dummyShow = cacher.CacheTag(tag, new EffectTagIndices(0, -1, 0));
        }

        private void SetDummyHide()
        {
            //if (dummyHide != null) return; 
            EffectTag tag = new EffectTag("Dummy Hide Animation", ' ', null);

            DummyDatabase database = new DummyDatabase("Dummy Hide Animation", ScriptableObject.CreateInstance<DummyHideAnimation>());
            AnimationCacher cacher = new AnimationCacher(database, state, context, new ReadOnlyCollection<CharData>(Mediator.CharData), (x) => !IsExcludedHide(x));
            dummyHide = cacher.CacheTag(tag, new EffectTagIndices(0, -1, 0));
        }
        #endregion

        #region Animation Controlling
        /// <summary>
        /// Update the current animations.<br/>
        /// You should only call this if <see cref="UpdateFrom"/> is set to <see cref="UpdateFrom.Script"/>,
        /// otherwise this will output a warning and return.
        /// </summary>
        public void UpdateAnimations(float deltaTime)
        {
#if UNITY_EDITOR
            if (Application.isPlaying && updateFrom != UpdateFrom.Script)
            {
                Debug.LogWarning(string.Format(falseUpdateAnimationsCallWarning, name, updateFrom.ToString()));
                return;
            }
#else
            if (updateFrom != UpdateFrom.Script) 
            {
                Debug.LogWarning(string.Format(falseUpdateAnimationsCallWarning, name, updateFrom.ToString()));
                return;
            }
#endif

            UpdateAnimations_Impl(deltaTime);
        }

        /// <summary>
        /// Start animating.<br/>
        /// You should only call this if <see cref="UpdateFrom"/> is NOT set to <see cref="UpdateFrom.Script"/>,
        /// otherwise this will output a warning and return.
        /// </summary>
        public void StartAnimating()
        {
#if UNITY_EDITOR
            if (Application.isPlaying && updateFrom == UpdateFrom.Script)
            {
                Debug.LogWarning(string.Format(falseStartStopAnimatingCallWarning, "StartAnimating", name));
                return;
            }
#else
            if (updateFrom == UpdateFrom.Script)
            {
                Debug.LogWarning(string.Format(falseStartStopAnimatingCallWarning, "StartAnimating", name));
                return;
            }
#endif

            isAnimating = true;
        }

        /// <summary>
        /// Stop animating.<br/>
        /// You should only call this if <see cref="UpdateFrom"/> is NOT set to <see cref="UpdateFrom.Script"/>,
        /// otherwise this will output a warning and return.
        /// </summary>
        public void StopAnimating()
        {
#if UNITY_EDITOR
            if (Application.isPlaying && updateFrom == UpdateFrom.Script)
            {
                Debug.LogWarning(string.Format(falseStartStopAnimatingCallWarning, "StopAnimating", name));
                return;
            }
#else
            if (updateFrom == UpdateFrom.Script)
            {
                Debug.LogWarning(string.Format(falseStartStopAnimatingCallWarning, "StopAnimating", name));
                return;
            }
#endif
            isAnimating = false;

            VisibilityState visibility;
            for (int i = 0; i < Mediator.CharData.Count; i++)
            {
                visibility = Mediator.VisibilityStates[i];

                if (visibility == VisibilityState.Showing)
                {
                    Mediator.SetVisibilityState(i, VisibilityState.Shown);
                }
                else if (visibility == VisibilityState.Hiding)
                {
                    Mediator.SetVisibilityState(i, VisibilityState.Hidden);
                }
            }
        }

        /// <summary>
        /// Reset all visible characters to their initial, unanimated state.
        /// </summary>
        public void ResetAnimations() => ResetAllVisible();
        #endregion

        #region Setters
        /// <summary>
        /// Set where the animations should be updated from.
        /// </summary>
        /// <param name="updateFrom">Where the animations are updated from.</param>
        public void SetUpdateFrom(UpdateFrom updateFrom)
        {
            if (isAnimating)
            {
                StopAnimating();
            }

            this.updateFrom = updateFrom;
        }

        /// <summary>
        /// Set the database that will be used to parse animation tags.
        /// </summary>
        /// <param name="database">The database that will be used to parse animation tags.</param>
        public void SetDatabase(TMPAnimationDatabase database)
        {
            TMPAnimationDatabase previous = this.database;
            this.database = database;
            OnDatabaseChanged(previous);
        }

        /// <summary>
        /// Set the default show animation.
        /// </summary>
        /// <param name="str">The default show animation as a string, e.g. "<+fade>".</param>
        public void SetDefaultShowString(string str)
        {
            defaultShowString = str;
            SetDefault(TMPAnimationType.Show);
        }

        /// <summary>
        /// Set the default hide animation.
        /// </summary>
        /// <param name="str">The default hide animation as a string, e.g. "<-fade>".</param>
        public void SetDefaultHideString(string str)
        {
            defaultHideString = str;
            SetDefault(TMPAnimationType.Hide);
        }

        /// <summary>
        /// Set the excluded character for animations of the given type, meaning characters that will not be animated by that type of animations.
        /// </summary>
        /// <param name="str">The excluded characters, as string. The string will be evaluated character-wise.</param>
        /// <param name="excludePunctuation">Whether punctuation is excluded.</param>
        /// <exception cref="System.ArgumentException">If an invalid <see cref="TMPAnimationType"/> is passed in.</exception>
        public void SetExcludedCharacters(TMPAnimationType type, string str, bool? excludePunctuation = null)
        {
            switch (type)
            {
                case TMPAnimationType.Basic: SetExcludedBasicCharacters(str, excludePunctuation); break;
                case TMPAnimationType.Show: SetExcludedShowCharacters(str, excludePunctuation); break;
                case TMPAnimationType.Hide: SetExcludedHideCharacters(str, excludePunctuation); break;
                default: throw new System.ArgumentException();
            }

            RecalculateSegmentData(type);
        }

        private void RecalculateSegmentData(TMPAnimationType type)
        {
            switch (type)
            {
                case TMPAnimationType.Basic:
                    foreach (var anim in basic)
                        anim.context.segmentData = new SegmentData(anim.Indices, Mediator.CharData, (c) => !IsExcludedBasic(c));
                    break;

                case TMPAnimationType.Show:
                    foreach (var anim in show)
                        anim.context.segmentData = new SegmentData(anim.Indices, Mediator.CharData, (c) => !IsExcludedShow(c));
                    break;

                case TMPAnimationType.Hide:
                    foreach (var anim in hide)
                        anim.context.segmentData = new SegmentData(anim.Indices, Mediator.CharData, (c) => !IsExcludedHide(c));
                    break;

                default: throw new System.ArgumentException();
            }
        }

        /// <summary>
        /// Set the excluded character for basic animations, meaning characters that will not be animated by basic animations.
        /// </summary>
        /// <param name="str">The excluded characters, as string. The string will be evaluated character-wise.</param>
        /// <param name="excludePunctuation">Whether punctuation is excluded.</param>
        public void SetExcludedBasicCharacters(string str, bool? excludePunctuation = null)
        {
            excludedCharacters = str;
            if (excludePunctuation != null)
            {
                this.excludePunctuation = excludePunctuation.Value;
            }

            RecalculateSegmentData(TMPAnimationType.Basic);
        }

        /// <summary>
        /// Set the excluded character for show animations, meaning characters that will not be animated by show animations.
        /// </summary>
        /// <param name="str">The excluded characters, as string. The string will be evaluated character-wise.</param>
        /// <param name="excludePunctuation">Whether punctuation is excluded.</param>
        public void SetExcludedShowCharacters(string str, bool? excludePunctuation = null)
        {
            excludedCharactersShow = str;
            if (excludePunctuation != null)
            {
                excludePunctuationShow = excludePunctuation.Value;
            }

            RecalculateSegmentData(TMPAnimationType.Show);
        }

        /// <summary>
        /// Set the excluded character for hide animations, meaning characters that will not be animated by hide animations.
        /// </summary>
        /// <param name="str">The excluded characters, as string. The string will be evaluated character-wise.</param>
        /// <param name="excludePunctuation">Whether punctuation is excluded.</param>
        public void SetExcludedHideCharacters(string str, bool? excludePunctuation = null)
        {
            excludedCharactersHide = str;
            if (excludePunctuation != null)
            {
                excludePunctuationHide = excludePunctuation.Value;
            }

            RecalculateSegmentData(TMPAnimationType.Hide);
        }
        #endregion

        #region Animations 
        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (updateFrom == UpdateFrom.Update && isAnimating) UpdateAnimations_Impl(context.useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (updateFrom == UpdateFrom.LateUpdate && isAnimating) UpdateAnimations_Impl(context.useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime);
        }

        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (updateFrom == UpdateFrom.FixedUpdate && isAnimating) UpdateAnimations_Impl(context.useScaledTime ? Time.fixedDeltaTime : Time.fixedUnscaledDeltaTime);
        }

        [System.NonSerialized] System.Diagnostics.Stopwatch sw = null;
        [System.NonSerialized] int count = 0;

        private void UpdateAnimations_Impl(float deltaTime)
        {
            if (sw == null) sw = new();
            else if (count == 100000)
            {
                Debug.Log("MEasurement aftert 100000 iterations: " + sw.Elapsed.TotalMilliseconds);
            }
            //else if (count % 100 == 0) Debug.Log(count);
            count++;
            sw.Start();


            context.passed += deltaTime;

            for (int i = 0; i < Mediator.CharData.Count; i++)
            {
                CharData cData = Mediator.CharData[i];
                UpdateCharacterAnimation(cData, deltaTime, i, false);
            }

            if (Mediator.Text.mesh != null)
                Mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            sw.Stop();
        }

        private void UpdateCharacterAnimation(CharData cData, float deltaTime, int index, bool updateVertices = true, bool forced = false)
        {
            if (!cData.info.isVisible || (!forced && !AnimateCharacter(index, cData))) return;

            context.deltaTime = deltaTime;

            UpdateCharacterAnimation_Impl(index);

            var info = Mediator.Text.textInfo;
            TMP_CharacterInfo cInfo = info.characterInfo[index];
            int vIndex = cInfo.vertexIndex, mIndex = cInfo.materialReferenceIndex;
            Color32[] colors = info.meshInfo[mIndex].colors32;
            Vector3[] verts = info.meshInfo[mIndex].vertices;
            Vector2[] uvs0 = info.meshInfo[mIndex].uvs0;
            Vector2[] uvs2 = info.meshInfo[mIndex].uvs2;
            //Vector2[] uvs4 = info.meshInfo[mIndex].uvs2;

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = Mediator.CharData[index].mesh[j].position;
                colors[vIndex + j] = Mediator.CharData[index].mesh[j].color;
                uvs0[vIndex + j] = Mediator.CharData[index].mesh[j].uv;
                uvs2[vIndex + j] = Mediator.CharData[index].mesh[j].uv2;
                //uvs4[vIndex + j] = mediator.CharData[index].currentMesh[j].uv4;
            }

            if (updateVertices && Mediator.Text.mesh != null)
                Mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

        private bool AnimateCharacter(int index, CharData cData)
        {
            VisibilityState vState = Mediator.VisibilityStates[index];
            return cData.info.isVisible && // If not visible, e.g. whitespace, dont animate
               vState != VisibilityState.Hidden && // If hidden, dont animate
                (basic.HasAnyContaining(index) || vState != VisibilityState.Shown); // If has no animations, dont animate
        }

        [System.NonSerialized]
        private CharDataState state = new CharDataState();
        public class CharDataState
        {
            public CharData cData;
            public AnimatorContext context;

            public Vector3 positionDelta;
            public Matrix4x4 scaleDelta;
            public Quaternion rotation;
            public Vector3 rotationPivot;

            public Vector3 TL;
            public Vector3 TR;
            public Vector3 BR;
            public Vector3 BL;

            public Vector3 TLMax;
            public Vector3 TRMax;
            public Vector3 BRMax;
            public Vector3 BLMax;

            public Vector3 TLMin;
            public Vector3 TRMin;
            public Vector3 BRMin;
            public Vector3 BLMin;

            public Vector2 TL_UV;
            public Vector2 TR_UV;
            public Vector2 BR_UV;
            public Vector2 BL_UV;

            public Vector2 TL_UV2;
            public Vector2 TR_UV2;
            public Vector2 BR_UV2;
            public Vector2 BL_UV2;

            public Color32 TL_Color;
            public Color32 TR_Color;
            public Color32 BR_Color;
            public Color32 BL_Color;

            public Vector3 TL_Result;
            public Vector3 TR_Result;
            public Vector3 BR_Result;
            public Vector3 BL_Result;

            public void Reset(AnimatorContext context, CharData cData)
            {
                this.cData = cData;
                this.context = context;

                positionDelta = Vector3.zero;
                scaleDelta = Matrix4x4.Scale(Vector3.one);
                rotation = Quaternion.identity;
                rotationPivot = cData.info.initialPosition;

                TL = Vector3.zero;
                TR = Vector3.zero;
                BR = Vector3.zero;
                BL = Vector3.zero;

                TLMax = cData.mesh.initial.vertex_TL.position;
                TRMax = cData.mesh.initial.vertex_TR.position;
                BRMax = cData.mesh.initial.vertex_BR.position;
                BLMax = cData.mesh.initial.vertex_BL.position;

                TLMin = cData.mesh.initial.vertex_TL.position;
                TRMin = cData.mesh.initial.vertex_TR.position;
                BRMin = cData.mesh.initial.vertex_BR.position;
                BLMin = cData.mesh.initial.vertex_BL.position;

                TL_UV = cData.mesh.initial.vertex_TL.uv;
                TR_UV = cData.mesh.initial.vertex_TR.uv;
                BR_UV = cData.mesh.initial.vertex_BR.uv;
                BL_UV = cData.mesh.initial.vertex_BL.uv;

                TL_UV2 = cData.mesh.initial.vertex_TL.uv2;
                TR_UV2 = cData.mesh.initial.vertex_TR.uv2;
                BR_UV2 = cData.mesh.initial.vertex_BR.uv2;
                BL_UV2 = cData.mesh.initial.vertex_BL.uv2;

                TL_Color = cData.mesh.initial.vertex_TL.color;
                TR_Color = cData.mesh.initial.vertex_TR.color;
                BR_Color = cData.mesh.initial.vertex_BR.color;
                BL_Color = cData.mesh.initial.vertex_BL.color;
            }

            public void CalculateVertexPositions()
            {
                // Apply vertex transformations
                Vector3 vtl = cData.mesh.initial.vertex_TL.position + TL;
                Vector3 vtr = cData.mesh.initial.vertex_TR.position + TR;
                Vector3 vbr = cData.mesh.initial.vertex_BR.position + BR;
                Vector3 vbl = cData.mesh.initial.vertex_BL.position + BL;

                // For now only the vertex offsets are clamped to min/max of each individual animation, as otherwise stacked animations are likely to deform the character
                vtl = new Vector3(Mathf.Clamp(vtl.x, TLMin.x, TLMax.x), Mathf.Clamp(vtl.y, TLMin.y, TLMax.y), Mathf.Clamp(vtl.z, TLMin.z, TLMax.z));
                vtr = new Vector3(Mathf.Clamp(vtr.x, TRMin.x, TRMax.x), Mathf.Clamp(vtr.y, TRMin.y, TRMax.y), Mathf.Clamp(vtr.z, TRMin.z, TRMax.z));
                vbr = new Vector3(Mathf.Clamp(vbr.x, BRMin.x, BRMax.x), Mathf.Clamp(vbr.y, BRMin.y, BRMax.y), Mathf.Clamp(vbr.z, BRMin.z, BRMax.z));
                vbl = new Vector3(Mathf.Clamp(vbl.x, BLMin.x, BLMax.x), Mathf.Clamp(vbl.y, BLMin.y, BLMax.y), Mathf.Clamp(vbl.z, BLMin.z, BLMax.z));

                // Apply scale
                vtl = scaleDelta.MultiplyPoint3x4(vtl - cData.info.initialPosition) + cData.info.initialPosition;
                vtr = scaleDelta.MultiplyPoint3x4(vtr - cData.info.initialPosition) + cData.info.initialPosition;
                vbr = scaleDelta.MultiplyPoint3x4(vbr - cData.info.initialPosition) + cData.info.initialPosition;
                vbl = scaleDelta.MultiplyPoint3x4(vbl - cData.info.initialPosition) + cData.info.initialPosition;

                // Apply rotation
                Matrix4x4 matrix = Matrix4x4.Rotate(rotation);
                vtl = matrix.MultiplyPoint3x4(vtl - rotationPivot) + rotationPivot;
                vtr = matrix.MultiplyPoint3x4(vtr - rotationPivot) + rotationPivot;
                vbr = matrix.MultiplyPoint3x4(vbr - rotationPivot) + rotationPivot;
                vbl = matrix.MultiplyPoint3x4(vbl - rotationPivot) + rotationPivot;

                // Apply transformation
                vtl += positionDelta * (context.scaleAnimations ? cData.info.referenceScale : 1);
                vtr += positionDelta * (context.scaleAnimations ? cData.info.referenceScale : 1);
                vbr += positionDelta * (context.scaleAnimations ? cData.info.referenceScale : 1);
                vbl += positionDelta * (context.scaleAnimations ? cData.info.referenceScale : 1);

                BL_Result = vbl;
                TL_Result = vtl;
                TR_Result = vtr;
                BR_Result = vbr;
            }
        }

        private void UpdateCharacterAnimation_Impl(int index)
        {
            CharData cData = Mediator.CharData[index];
            VisibilityState vState = Mediator.VisibilityStates[index];
            if (!cData.info.isVisible || vState == VisibilityState.Hidden) return;

            state.Reset(context, cData);

            if (vState == VisibilityState.Showing)
            {
                bool prev = ignoreVisibilityChanges;
                ignoreVisibilityChanges = true;

                bool allDone = true;

                if (IsExcludedShow(cData.info.character))
                {
                    Animate(dummyShow, false);
                }
                else if (show.Count == 0)
                {
                    Animate(defaultShow, defaultShow.late);
                    allDone = defaultShow.Finished(cData.info.index);
                }
                else
                {
                    allDone = AnimateShowList(false) && AnimateShowList(true);
                }

                if (allDone)
                {
                    if (!basic.HasAnyContaining(index) || IsExcludedBasic(cData.info.character))
                    {
                        ignoreVisibilityChanges = false;
                        Mediator.SetVisibilityState(cData, VisibilityState.Shown);
                        ignoreVisibilityChanges = prev;
                        return;
                    }

                    Mediator.SetVisibilityState(cData, VisibilityState.Shown);
                }

                ignoreVisibilityChanges = prev;
            }

            else if (vState == VisibilityState.Hiding)
            {
                bool prev = ignoreVisibilityChanges;
                ignoreVisibilityChanges = true;

                bool done = true;

                if (IsExcludedHide(cData.info.character))
                {
                    Animate(dummyHide, false);
                }
                else if (hide.Count == 0)
                {
                    Animate(defaultHide, defaultHide.late);
                    done = defaultHide.Finished(cData.info.index);
                }
                else
                {
                    done = AnimateHideList(false) || AnimateHideList(true);
                }

                if (done)
                {
                    ignoreVisibilityChanges = false;
                    Mediator.SetVisibilityState(cData, VisibilityState.Hidden);
                    ignoreVisibilityChanges = prev;
                    return;
                }

                ignoreVisibilityChanges = prev;
            }


            // TODO test if is excluded basic, are the show / hide anims still applied?
            if (IsExcludedBasic(cData.info.character))
            {
                return;
            }

            AnimateBasic(false);
            AnimateBasic(true);
            ApplyVertices();
            return;

            void Animate(CachedAnimation ca, bool late)
            {
                if (ca.Finished(index)) return;
                if (ca.late != late) return;

                cData.Reset();

                for (int i = 0; i < 4; i++)
                {
                    cData.SetVertex(i, cData.mesh.initial.GetVertex(i)); // cData.initialMesh.GetPosition(i));
                }

                ca.animation.Animate(cData, ca.roContext);

                UpdateVertexOffsets();
            }

            void AnimateBasic(bool late)
            {
                CachedCollection<CachedAnimation>.MinMax mm = basic.MinMaxAt(index);
                if (mm == null) return;

                if (animationsOverride)
                {
                    int start = mm.MinIndex;

                    for (int i = mm.MaxIndex; i >= mm.MinIndex; i--)
                    {
                        CachedAnimation ca = basic[i];
                        if (ca.Indices.Contains(index))
                        {
                            if (!(ca.overrides != null && !ca.overrides.Value))
                            {
                                start = i;
                                break;
                            }
                        }
                    }

                    for (int i = start; i <= mm.MaxIndex; i++)
                    {
                        CachedAnimation ca = basic[i];
                        if (ca.Indices.Contains(index))
                        {
                            Animate(ca, late);
                        }
                    }
                }
                else
                {
                    int start = mm.MinIndex;

                    for (int i = mm.MaxIndex; i >= mm.MinIndex; i--)
                    {
                        CachedAnimation ca = basic[i];
                        if (ca.Indices.Contains(index))
                        {
                            if (ca.overrides != null && ca.overrides.Value)
                            {
                                start = i;
                                break;
                            }
                        }
                    }

                    for (int i = start; i <= mm.MaxIndex; i++)
                    {
                        CachedAnimation ca = basic[i];
                        if (ca.Indices.Contains(index))
                        {
                            Animate(ca, late);
                        }
                    }
                }
            }

            bool AnimateShowList(bool late)
            {
                CachedCollection<CachedAnimation>.MinMax mm = show.MinMaxAt(index);
                if (mm == null) return true;

                bool allDone = true;
                if (animationsOverride)
                {
                    int start = mm.MinIndex;

                    for (int i = mm.MaxIndex; i >= mm.MinIndex; i--)
                    {
                        CachedAnimation ca = show[i];
                        if (ca.Indices.Contains(index))
                        {
                            if (!(ca.overrides != null && !ca.overrides.Value))
                            {
                                start = i;
                                break;
                            }
                        }
                    }

                    for (int i = start; i <= mm.MaxIndex; i++)
                    {
                        CachedAnimation ca = show[i];
                        if (ca.Indices.Contains(index))
                        {
                            Animate(ca, late);

                            if (!ca.Finished(index))
                            {
                                allDone = false;
                            }

                            if (!(ca.overrides != null && !ca.overrides.Value))
                                break;
                        }
                    }
                }
                else
                {
                    int start = mm.MinIndex;

                    for (int i = mm.MaxIndex; i >= mm.MinIndex; i--)
                    {
                        CachedAnimation ca = show[i];
                        if (ca.Indices.Contains(index))
                        {
                            if (ca.overrides != null && ca.overrides.Value)
                            {
                                start = i;
                                break;
                            }
                        }
                    }

                    for (int i = start; i <= mm.MaxIndex; i++)
                    {
                        CachedAnimation ca = show[i];
                        if (ca.Indices.Contains(index))
                        {
                            Animate(ca, late);

                            if (!ca.Finished(index))
                            {
                                allDone = false;
                            }

                            if (ca.overrides != null && ca.overrides.Value)
                                break;
                        }
                    }
                }

                return allDone;
            }

            bool AnimateHideList(bool late)
            {
                CachedCollection<CachedAnimation>.MinMax mm = hide.MinMaxAt(index);
                if (mm == null) return true;

                bool done = false;
                if (animationsOverride)
                {
                    int start = mm.MinIndex;

                    for (int i = mm.MaxIndex; i >= mm.MinIndex; i--)
                    {
                        CachedAnimation ca = hide[i];
                        if (ca.Indices.Contains(index))
                        {
                            if (!(ca.overrides != null && !ca.overrides.Value))
                            {
                                start = i;
                                break;
                            }
                        }
                    }

                    for (int i = start; i <= mm.MaxIndex; i++)
                    {
                        CachedAnimation ca = hide[i];
                        if (ca.Indices.Contains(index))
                        {
                            Animate(ca, late);

                            if (ca.Finished(index)) return true;

                            if (!(ca.overrides != null && !ca.overrides.Value))
                                break;
                        }
                    }
                }
                else
                {
                    int start = mm.MinIndex;

                    for (int i = mm.MaxIndex; i >= mm.MinIndex; i--)
                    {
                        CachedAnimation ca = hide[i];
                        if (ca.Indices.Contains(index))
                        {
                            if (ca.overrides != null && ca.overrides.Value)
                            {
                                start = i;
                                break;
                            }
                        }
                    }

                    for (int i = start; i <= mm.MaxIndex; i++)
                    {
                        CachedAnimation ca = hide[i];
                        if (ca.Indices.Contains(index))
                        {
                            Animate(ca, late);

                            if (ca.Finished(index)) return true;

                            if (ca.overrides != null && ca.overrides.Value)
                                break;
                        }
                    }
                }

                return done;
            }

            void UpdateVertexOffsets()
            {
                if (cData.positionDirty)
                {
                    state.positionDelta += (cData.Position - cData.info.initialPosition);
                }
                if (cData.scaleDirty)
                {
                    state.scaleDelta *= Matrix4x4.Scale(cData.Scale);
                }
                if (cData.rotationDirty)
                {
                    state.rotation = cData.Rotation * state.rotation;
                    state.rotationPivot += (cData.RotationPivot - cData.info.initialPosition) * (context.scaleAnimations ? cData.info.referenceScale : 1);
                }

                if (cData.verticesDirty)
                {
                    Vector3 deltaTL = (cData.mesh.vertex_TL.position - cData.mesh.initial.vertex_TL.position) * (context.scaleAnimations ? cData.info.referenceScale : 1);
                    Vector3 deltaTR = (cData.mesh.vertex_TR.position - cData.mesh.initial.vertex_TR.position) * (context.scaleAnimations ? cData.info.referenceScale : 1);
                    Vector3 deltaBR = (cData.mesh.vertex_BR.position - cData.mesh.initial.vertex_BR.position) * (context.scaleAnimations ? cData.info.referenceScale : 1);
                    Vector3 deltaBL = (cData.mesh.vertex_BL.position - cData.mesh.initial.vertex_BL.position) * (context.scaleAnimations ? cData.info.referenceScale : 1);

                    state.TL += deltaTL;
                    state.TR += deltaTR;
                    state.BR += deltaBR;
                    state.BL += deltaBL;

                    state.TLMax = new Vector3(Mathf.Max(cData.mesh.initial.vertex_TL.position.x + deltaTL.x, state.TLMax.x), Mathf.Max(cData.mesh.initial.vertex_TL.position.y + deltaTL.y, state.TLMax.y), Mathf.Max(cData.mesh.initial.vertex_TL.position.z + deltaTL.z, state.TLMax.z));
                    state.TRMax = new Vector3(Mathf.Max(cData.mesh.initial.vertex_TR.position.x + deltaTR.x, state.TRMax.x), Mathf.Max(cData.mesh.initial.vertex_TR.position.y + deltaTR.y, state.TRMax.y), Mathf.Max(cData.mesh.initial.vertex_TR.position.z + deltaTR.z, state.TRMax.z));
                    state.BRMax = new Vector3(Mathf.Max(cData.mesh.initial.vertex_BR.position.x + deltaBR.x, state.BRMax.x), Mathf.Max(cData.mesh.initial.vertex_BR.position.y + deltaBR.y, state.BRMax.y), Mathf.Max(cData.mesh.initial.vertex_BR.position.z + deltaBR.z, state.BRMax.z));
                    state.BLMax = new Vector3(Mathf.Max(cData.mesh.initial.vertex_BL.position.x + deltaBL.x, state.BLMax.x), Mathf.Max(cData.mesh.initial.vertex_BL.position.y + deltaBL.y, state.BLMax.y), Mathf.Max(cData.mesh.initial.vertex_BL.position.z + deltaBL.z, state.BLMax.z));

                    state.TLMin = new Vector3(Mathf.Min(cData.mesh.initial.vertex_TL.position.x + deltaTL.x, state.TLMin.x), Mathf.Min(cData.mesh.initial.vertex_TL.position.y + deltaTL.y, state.TLMin.y), Mathf.Min(cData.mesh.initial.vertex_TL.position.z + deltaTL.z, state.TLMin.z));
                    state.TRMin = new Vector3(Mathf.Min(cData.mesh.initial.vertex_TR.position.x + deltaTR.x, state.TRMin.x), Mathf.Min(cData.mesh.initial.vertex_TR.position.y + deltaTR.y, state.TRMin.y), Mathf.Min(cData.mesh.initial.vertex_TR.position.z + deltaTR.z, state.TRMin.z));
                    state.BRMin = new Vector3(Mathf.Min(cData.mesh.initial.vertex_BR.position.x + deltaBR.x, state.BRMin.x), Mathf.Min(cData.mesh.initial.vertex_BR.position.y + deltaBR.y, state.BRMin.y), Mathf.Min(cData.mesh.initial.vertex_BR.position.z + deltaBR.z, state.BRMin.z));
                    state.BLMin = new Vector3(Mathf.Min(cData.mesh.initial.vertex_BL.position.x + deltaBL.x, state.BLMin.x), Mathf.Min(cData.mesh.initial.vertex_BL.position.y + deltaBL.y, state.BLMin.y), Mathf.Min(cData.mesh.initial.vertex_BL.position.z + deltaBL.z, state.BLMin.z));
                }

                if (cData.colorsDirty)
                {
                    state.BL_Color = cData.mesh.GetColor(0);
                    state.TL_Color = cData.mesh.GetColor(1);
                    state.TR_Color = cData.mesh.GetColor(2);
                    state.BR_Color = cData.mesh.GetColor(3);
                }

                if (cData.uvsDirty)
                {
                    state.BL_UV = cData.mesh.GetUV0(0);
                    state.TL_UV = cData.mesh.GetUV0(1);
                    state.TR_UV = cData.mesh.GetUV0(2);
                    state.BR_UV = cData.mesh.GetUV0(3);

                    state.BL_UV2 = cData.mesh.GetUV2(0);
                    state.TL_UV2 = cData.mesh.GetUV2(1);
                    state.TR_UV2 = cData.mesh.GetUV2(2);
                    state.BR_UV2 = cData.mesh.GetUV2(3);
                }
            }

            void ApplyVertices()
            {
                state.CalculateVertexPositions();

                cData.SetVertex(0, state.BL_Result);
                cData.SetVertex(1, state.TL_Result);
                cData.SetVertex(2, state.TR_Result);
                cData.SetVertex(3, state.BR_Result);

                cData.mesh.SetColor(0, state.BL_Color);
                cData.mesh.SetColor(1, state.TL_Color);
                cData.mesh.SetColor(2, state.TR_Color);
                cData.mesh.SetColor(3, state.BR_Color);

                cData.mesh.SetUV0(0, state.BL_UV);
                cData.mesh.SetUV0(1, state.TL_UV);
                cData.mesh.SetUV0(2, state.TR_UV);
                cData.mesh.SetUV0(3, state.BR_UV);

                cData.mesh.SetUV2(0, state.BL_UV2);
                cData.mesh.SetUV2(1, state.TL_UV2);
                cData.mesh.SetUV2(2, state.TR_UV2);
                cData.mesh.SetUV2(3, state.BR_UV2);
            }
        }
        #endregion

        #region Event Callbacks
        private void OnTextChanged()
        {
            if (IsAnimating) UpdateAnimations_Impl(0f);
        }

        private void PopulateTimes()
        {
            visibleTimes = new List<float>();
            stateTimes = new List<float>();

            for (int i = 0; i < Mediator.CharData.Count; i++)
            {
                visibleTimes.Add(0f);
                stateTimes.Add(0f);
            }
        }

        private void OnVisibilityStateUpdated(int index, VisibilityState prev)
        {
            if (ignoreVisibilityChanges) return;

            CharData cData = Mediator.CharData[index];
            if (!cData.info.isVisible) return;

            // Check if visibility actually changed
            VisibilityState state = Mediator.VisibilityStates[index];

            if (!IsAnimating)
            {
                if (state == VisibilityState.Showing)
                {
                    Mediator.SetVisibilityState(index, VisibilityState.Shown);
                    return;
                }
                if (state == VisibilityState.Hiding)
                {
                    Mediator.SetVisibilityState(index, VisibilityState.Hidden);
                    return;
                }
            }

            if (prev == state)
            {
                Debug.LogError("Character didnt change but the update event was raised?");
                return;
            }

            // Update timings of the character
            stateTimes[index] = context.passed;
            if (state == VisibilityState.Hidden || prev == VisibilityState.Hidden) visibleTimes[index] = context.passed;

            if (state == VisibilityState.Shown)
            {
                cData.Reset();
                UpdateVisibility(true);
            }
            else if (state == VisibilityState.Hidden)
            {
                UpdateVisibility(false);
            }

            // Reset the "finished" status of the relevant animations
            else if (state == VisibilityState.Showing)
            {
                defaultShow.context.ResetFinishAnimation(index);
                dummyShow.context.ResetFinishAnimation(index);

                var mm = show.MinMaxAt(index);
                if (mm != null)
                {
                    for (int i = mm.MinIndex; i <= mm.MaxIndex; i++)
                    {
                        CachedAnimation ca = show[i];
                        if (ca.Indices.Contains(index))
                        {
                            ca.context.ResetFinishAnimation(index);
                        }
                    }
                }
            }
            else if (state == VisibilityState.Hiding)
            {
                defaultHide.context.ResetFinishAnimation(index);
                dummyHide.context.ResetFinishAnimation(index);

                var mm = hide.MinMaxAt(index);
                if (mm != null)
                {
                    for (int i = mm.MinIndex; i <= mm.MaxIndex; i++)
                    {
                        CachedAnimation ca = hide[i];
                        if (ca.Indices.Contains(index))
                        {
                            ca.context.ResetFinishAnimation(index);
                        }
                    }
                }
            }

            // Update character animations
            if (IsAnimating)
            {
                ignoreVisibilityChanges = true;
                UpdateCharacterAnimation(Mediator.CharData[index], 0f, index, false);
                ignoreVisibilityChanges = false;

                if (state != Mediator.VisibilityStates[index])
                {
                    OnVisibilityStateUpdated(index, state);
                    return;
                }
            }

            if (Mediator.Text.mesh != null)
                Mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            void UpdateVisibility(bool show)
            {
                TMP_TextInfo info = Mediator.Text.textInfo;
                TMP_CharacterInfo cInfo;
                Vector3[] verts;
                Color32[] colors;
                Vector2[] uvs0;
                Vector2[] uvs2;
                int vIndex, mIndex;

                // Set the current mesh's vertices all to the initial mesh values
                if (show) SetVerticesToDefault();
                else SetVerticesToZero();

                // Apply the new vertices to the vertex array
                cInfo = info.characterInfo[index];
                vIndex = cInfo.vertexIndex;
                mIndex = cInfo.materialReferenceIndex;

                colors = info.meshInfo[mIndex].colors32;
                verts = info.meshInfo[mIndex].vertices;
                uvs0 = info.meshInfo[mIndex].uvs0;
                uvs2 = info.meshInfo[mIndex].uvs2;

                for (int j = 0; j < 4; j++)
                {
                    verts[vIndex + j] = cData.mesh[j].position;
                    colors[vIndex + j] = cData.mesh[j].color;
                    uvs0[vIndex + j] = cData.mesh[j].uv;
                    uvs2[vIndex + j] = cData.mesh[j].uv2;
                }
            }

            void SetVerticesToZero()
            {
                // Set the current mesh's vertices all to the initial mesh values
                for (int j = 0; j < 4; j++)
                {
                    cData.SetVertex(j, cData.info.initialPosition);// cData.info.initialPosition);
                }
            }

            void SetVerticesToDefault()
            {
                // Set the current mesh's vertices all to the initial mesh values
                for (int j = 0; j < 4; j++)
                {
                    cData.SetVertex(j, cData.mesh.initial.GetVertex(j));
                }
            }
        }

        private void PostProcessTags()
        {
            tags.Clear();

            foreach (var processor in processors.TagProcessors[basicCategory.Prefix])
            {
                foreach (var tag in processor.ProcessedTags)
                {
                    tags[basicCategory].TryAdd(tag.Value, tag.Key);
                }
            }

            foreach (var processor in processors.TagProcessors[showCategory.Prefix])
            {
                foreach (var tag in processor.ProcessedTags)
                    tags[showCategory].TryAdd(tag.Value, tag.Key);
            }

            foreach (var processor in processors.TagProcessors[hideCategory.Prefix])
            {
                foreach (var tag in processor.ProcessedTags)
                    tags[hideCategory].TryAdd(tag.Value, tag.Key);
            }
        }
        #endregion

        #region Utility
        private void ResetAllVisible()
        {
            var info = Mediator.Text.textInfo;

            Vector3[] verts;
            Color32[] colors;
            Vector2[] uvs0;
            Vector2[] uvs2;
            int vIndex, mIndex;
            TMP_CharacterInfo cInfo;

            // Iterate over all characters and apply the new meshes
            for (int i = 0; i < Mediator.CharData.Count; i++)
            {
                cInfo = info.characterInfo[i];
                if (!cInfo.isVisible || Mediator.VisibilityStates[i] == VisibilityState.Hidden) continue;

                vIndex = cInfo.vertexIndex;
                mIndex = cInfo.materialReferenceIndex;

                colors = info.meshInfo[mIndex].colors32;
                verts = info.meshInfo[mIndex].vertices;

                uvs0 = info.meshInfo[mIndex].uvs0;
                uvs2 = info.meshInfo[mIndex].uvs2;

                CharData cData = Mediator.CharData[i];
                cData.Reset();

                for (int j = 0; j < 4; j++)
                {
                    verts[vIndex + j] = cData.mesh.initial[j].position;
                    colors[vIndex + j] = cData.mesh.initial[j].color;
                    uvs0[vIndex + j] = cData.mesh.initial[j].uv;
                    uvs2[vIndex + j] = cData.mesh.initial[j].uv2;
                }

                //Mediator.CharData[i] = cData;
            }

            if (Mediator.Text.mesh != null)
                Mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }


        /// <summary>
        /// Whether the character is excluded from animations of the given type.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <param name="type">The type of animation to check against.</param>
        /// <returns>Whether the character is excluded from animations of the given type.</returns>
        /// <exception cref="System.ArgumentException">If an invalid <see cref="TMPAnimationType"/> is passed in.</exception>
        public bool IsExcluded(char c, TMPAnimationType type)
        {
            switch (type)
            {
                case TMPAnimationType.Basic: return IsExcludedBasic(c);
                case TMPAnimationType.Show: return IsExcludedShow(c);
                case TMPAnimationType.Hide: return IsExcludedHide(c);
                default: throw new System.ArgumentException();
            }
        }
        /// <summary>
        /// Check whether the given character is excluded from basic animations.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>Whether the character is excluded from basic animations.</returns>
        public bool IsExcludedBasic(char c) => (excludePunctuation && char.IsPunctuation(c)) || excludedCharacters.Contains(c);
        /// <summary>
        /// Check whether the given character is excluded from show animations.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>Whether the character is excluded from show animations.</returns>
        public bool IsExcludedShow(char c) => (excludePunctuationShow && char.IsPunctuation(c)) || excludedCharactersShow.Contains(c);
        /// <summary>
        /// Check whether the given character is excluded from hide animations.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>Whether the character is excluded from hide animations.</returns>
        public bool IsExcludedHide(char c) => (excludePunctuationHide && char.IsPunctuation(c)) || excludedCharactersHide.Contains(c);
        #endregion

        #region Editor Only
#if UNITY_EDITOR
        [SerializeField, HideInInspector] bool preview = false;
        [SerializeField, HideInInspector] bool initValidate = false;
        [SerializeField, HideInInspector] TMPAnimationDatabase prevDatabase = null;
        [SerializeField, HideInInspector] string prevExcludedBasicCharacters = null;
        [SerializeField, HideInInspector] string prevExcludedShowCharacters = null;
        [SerializeField, HideInInspector] string prevExcludedHideCharacters = null;
        [SerializeField, HideInInspector] bool prevBasicExcludePunctuation = false;
        [SerializeField, HideInInspector] bool prevShowExcludePunctuation = false;
        [SerializeField, HideInInspector] bool prevHideExcludePunctuation = false;


        internal void StartPreview()
        {
            //preview = true;
            StartAnimating();

            EditorApplication.update -= UpdatePreview;
            EditorApplication.update += UpdatePreview;
        }

        [MenuItem("CONTEXT/TMP_Text/Add animator")]
        static void AddAnimator(MenuCommand command)
        {
            TMP_Text text = command.context as TMP_Text;
            if (text == null)
            {
                Debug.LogWarning("Could not add animator to " + command.context.name);
                return;
            }

            text.gameObject.GetOrAddComponent<TMPAnimator>();
        }

        internal void StopPreview()
        {
            //preview = false;
            EditorApplication.update -= UpdatePreview;
            if (updateFrom != UpdateFrom.Script) StopAnimating();
            ResetAnimations();
        }

        private void UpdatePreview()
        {
            UpdateAnimations_Impl((float)EditorApplication.timeSinceStartup - context.passed);
            EditorApplication.QueuePlayerLoopUpdate();
        }

        internal void ForceReprocess()
        {
            if (Mediator != null) Mediator.ForceReprocess();
        }

        internal void ForcePostProcess()
        {
            if (Mediator != null) PostProcessTags();
        }

        internal void PrepareForProcessingWrapper()
        {
            if (Mediator == null) return;
            PrepareForProcessing();
        }

        internal string CheckDefaultString(TMPAnimationType type)
        {
            string str;
            ITMPEffectDatabase<ITMPAnimation> database;
            switch (type)
            {
                case TMPAnimationType.Show:
                    database = showDatabase;
                    str = defaultShowString;
                    break;

                case TMPAnimationType.Hide:
                    database = hideDatabase;
                    str = defaultHideString;
                    break;

                default:
                    throw new System.ArgumentException(nameof(type));
            }

            ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();
            Dictionary<string, string> tagParams;
            ITMPAnimation animation;

            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            str = (str.Trim()[0] == '<' ? str : "<" + str + ">");
            if (!ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo, ParsingUtility.TagType.Open))
            {
                return "Not a well-formed tag";
            }

            if (!database.ContainsEffect(tagInfo.name))
            {
                return "Tag with name " + tagInfo.name + " not defined";
            }

            if ((animation = database.GetEffect(tagInfo.name)) == null)
            {
                return "Tag with name " + tagInfo.name + " is defined, but not assigned an animation";
            }

            tagParams = ParsingUtility.GetTagParametersDict(str);
            if (!animation.ValidateParameters(tagParams))
            {

                return "Parameters are not valid for this tag";
            }

            return "";
        }

        internal void UpdateDefaultStrings()
        {
            SetDefaultShowString(defaultShowString);
            SetDefaultHideString(defaultHideString);
        }

        private void OnValidate()
        {
            if (!initValidate)
            {
                prevDatabase = database;
                prevExcludedBasicCharacters = excludedCharacters;
                prevExcludedShowCharacters = excludedCharactersShow;
                prevExcludedHideCharacters = excludedCharactersHide;
                prevBasicExcludePunctuation = excludePunctuation;
                prevShowExcludePunctuation = excludePunctuationHide;
                prevHideExcludePunctuation = excludePunctuationShow;
                initValidate = true;
            }

            if (prevExcludedBasicCharacters != excludedCharacters || prevBasicExcludePunctuation != excludePunctuation)
            {
                prevExcludedBasicCharacters = excludedCharacters;
                prevBasicExcludePunctuation = excludePunctuation;
                RecalculateSegmentData(TMPAnimationType.Basic);
            }

            if (prevExcludedShowCharacters != excludedCharactersShow || prevShowExcludePunctuation != excludePunctuationHide)
            {
                prevExcludedShowCharacters = excludedCharactersShow;
                prevShowExcludePunctuation = excludePunctuationShow;
                RecalculateSegmentData(TMPAnimationType.Show);
            }

            if (prevExcludedHideCharacters != excludedCharactersHide || prevHideExcludePunctuation != excludePunctuationHide)
            {
                prevExcludedHideCharacters = excludedCharactersHide;
                prevHideExcludePunctuation = excludePunctuationHide;
                RecalculateSegmentData(TMPAnimationType.Hide);
            }

            if (Mediator != null &&
                    (prevDatabase != database ||
                    (database != null &&
                    (basicDatabase.Database != (ITMPEffectDatabase<ITMPAnimation>)database.BasicAnimationDatabase ||
                    showDatabase.Database != (ITMPEffectDatabase<ITMPAnimation>)database.ShowAnimationDatabase ||
                    hideDatabase.Database != (ITMPEffectDatabase<ITMPAnimation>)database.HideAnimationDatabase)))
                )
            {
                OnDatabaseChanged(prevDatabase);
                prevDatabase = database;
            }
        }
#endif
        #endregion
    }
}