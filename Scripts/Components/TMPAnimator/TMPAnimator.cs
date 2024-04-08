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
using TMPEffects.CharacterData;
using System.Collections.Specialized;
using TMPEffects.TMPAnimations.ShowAnimations;
using TMPEffects.TMPAnimations.HideAnimations;
using TMPEffects.TMPSceneAnimations;
using System.Diagnostics.Tracing;
using System.Linq;

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

        [SerializeField] private bool animationsUseAnimatorTime = true;

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

            SetDummies();
            
            PrepareForProcessing();

            SubscribeToMediator();

            Mediator.ForceReprocess();

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

#if UNITY_EDITOR
            StopPreview();
#endif

            basicDatabase?.Dispose();
            showDatabase?.Dispose();
            hideDatabase?.Dispose();

            UnsubscribeFromMediator();
        }

        private void SubscribeToMediator()
        {
            timesIdentifier = new object();
            if (!Mediator.RegisterVisibilityProcessor(timesIdentifier))
            {
                Debug.LogError("Could not register as visibility processor!");
            }

            Mediator.TextChanged_Late += OnTextChanged;
            Mediator.TextChanged_Early += PopulateTimes;
            Mediator.VisibilityStateUpdated += OnVisibilityStateUpdated; // Handle Visibility updates
        }

        private void UnsubscribeFromMediator()
        {
            Mediator.TextChanged_Late -= OnTextChanged;
            Mediator.TextChanged_Early -= PopulateTimes;
            Mediator.VisibilityStateUpdated -= OnVisibilityStateUpdated;

            if (!Mediator.UnregisterVisibilityProcessor(timesIdentifier))
            {
                Debug.LogError("Could not unregister as visibility processor!");
            }

            Mediator.ForceReprocess();
            FreeMediator();
        }

        private void CreateContext()
        {
            // TODO Should be no longer necessary to do 
            context._VisibleTime = (i) => visibleTimes[i];
            context._StateTime = (i) => stateTimes[i];
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

        // Reset all visible character when a tag is added / removed / replaced;
        // Mostly required when tag is removed or replaced so that previously animated text
        // is no longer animated; would remain in last animated state otherwise
        private void OnTagCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetAllVisible();
        }
        
        private void SetDefault(TMPAnimationType type)
        {
            if (Mediator == null)
            {
                SetToDummy();
                return;
            }

            string str;
            ITMPEffectDatabase<ITMPAnimation> database;


            switch (type)
            {

                case TMPAnimationType.Show:
                    defaultShow = dummyShow;
                    str = defaultShowString;
                    database = showDatabase;
                    break;

                case TMPAnimationType.Hide:
                    defaultHide = dummyHide;
                    str = defaultHideString;
                    database = hideDatabase;
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
            if (!ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo, ParsingUtility.TagType.Open) || !database.ContainsEffect(tagInfo.name))
            {
                SetToDummy();
                return;
            }

            if ((animation = database.GetEffect(tagInfo.name)) == null)
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
                    cacher = new AnimationCacher(database, state, context, new ReadOnlyCollection<CharData>(Mediator.CharData), x => !IsExcludedShow(x));
                    defaultShow = cacher.CacheTag(new EffectTag(tagInfo.name, tagInfo.prefix, tagParams), new EffectTagIndices(0, -1, 0));
                    break;

                case TMPAnimationType.Hide:
                    cacher = new AnimationCacher(database, state, context, new ReadOnlyCollection<CharData>(Mediator.CharData), x => !IsExcludedHide(x));
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

                if (dummyShow == null || dummyHide == null) Debug.LogWarning("SET DEFAULT WHILE DUMMY NULL");
            }
        }

        private void OnDatabaseChanged(TMPAnimationDatabase previousDatabase)
        {
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

            if (!isActiveAndEnabled)
            {
                throw new System.InvalidOperationException("Animator is not enabled!");
            }

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
            for (int i = 0; i < Mediator?.CharData.Count; i++)
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
            QueueCharacterReset();
        }

        private void RecalculateSegmentData(TMPAnimationType type)
        {
            // TODO Need to check this in case exclusions change while animator is
            // disabled (=> mediator is null).
            // At some point the editor-runtime code separation will need some level
            // of reworking
            if (!isActiveAndEnabled) return;

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
            if (updateFrom == UpdateFrom.Update && isAnimating) UpdateAnimations_Impl(context.UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (updateFrom == UpdateFrom.LateUpdate && isAnimating) UpdateAnimations_Impl(context.UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime);
        }

        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (updateFrom == UpdateFrom.FixedUpdate && isAnimating) UpdateAnimations_Impl(context.UseScaledTime ? Time.fixedDeltaTime : Time.fixedUnscaledDeltaTime);
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

            if (characterResetQueued)
            {
                ResetAllVisible();
                characterResetQueued = false;
            }

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
                verts[vIndex + j] = Mediator.CharData[index].mesh.GetPosition(j);
                colors[vIndex + j] = Mediator.CharData[index].mesh.GetColor(j);
                uvs0[vIndex + j] = Mediator.CharData[index].mesh.GetUV0(j);
                uvs2[vIndex + j] = Mediator.CharData[index].mesh.GetUV2(j);
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
        // TODO Own file
        public class CharDataState
        {
            public CharData cData;
            public AnimatorContext context;

            List<Vector3> pivots = new List<Vector3>();
            List<Quaternion> rotations = new List<Quaternion>();

            public Vector3 positionDelta;
            public Matrix4x4 scaleDelta;
            //public Quaternion rotation;
            //public Vector3 rotationPivot;

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
                //rotation = Quaternion.identity;
                //rotationPivot = cData.initialPosition;
                rotations.Clear();
                pivots.Clear();

                TL = Vector3.zero;
                TR = Vector3.zero;
                BR = Vector3.zero;
                BL = Vector3.zero;

                TLMax = cData.mesh.initial.TL_Position;
                TRMax = cData.mesh.initial.TR_Position;
                BRMax = cData.mesh.initial.BR_Position;
                BLMax = cData.mesh.initial.BL_Position;

                TLMin = cData.mesh.initial.TL_Position;
                TRMin = cData.mesh.initial.TR_Position;
                BRMin = cData.mesh.initial.BR_Position;
                BLMin = cData.mesh.initial.BL_Position;

                TL_UV = cData.mesh.initial.TL_UV0;
                TR_UV = cData.mesh.initial.TR_UV0;
                BR_UV = cData.mesh.initial.BR_UV0;
                BL_UV = cData.mesh.initial.BL_UV0;

                TL_UV2 = cData.mesh.initial.TL_UV2;
                TR_UV2 = cData.mesh.initial.TR_UV2;
                BR_UV2 = cData.mesh.initial.BR_UV2;
                BL_UV2 = cData.mesh.initial.BL_UV2;

                TL_Color = cData.mesh.initial.TL_Color;
                TR_Color = cData.mesh.initial.TR_Color;
                BR_Color = cData.mesh.initial.BR_Color;
                BL_Color = cData.mesh.initial.BL_Color;
            }

            public void CalculateVertexPositions()
            {
                // Apply vertex transformations
                Vector3 vtl = cData.initialMesh.TL_Position + TL;
                Vector3 vtr = cData.initialMesh.TR_Position + TR;
                Vector3 vbr = cData.initialMesh.BR_Position + BR;
                Vector3 vbl = cData.initialMesh.BL_Position + BL;

                // For now only the vertex offsets are clamped to min/max of each individual animation, as otherwise stacked animations are likely to deform the character
                vtl = new Vector3(Mathf.Clamp(vtl.x, TLMin.x, TLMax.x), Mathf.Clamp(vtl.y, TLMin.y, TLMax.y), Mathf.Clamp(vtl.z, TLMin.z, TLMax.z));
                vtr = new Vector3(Mathf.Clamp(vtr.x, TRMin.x, TRMax.x), Mathf.Clamp(vtr.y, TRMin.y, TRMax.y), Mathf.Clamp(vtr.z, TRMin.z, TRMax.z));
                vbr = new Vector3(Mathf.Clamp(vbr.x, BRMin.x, BRMax.x), Mathf.Clamp(vbr.y, BRMin.y, BRMax.y), Mathf.Clamp(vbr.z, BRMin.z, BRMax.z));
                vbl = new Vector3(Mathf.Clamp(vbl.x, BLMin.x, BLMax.x), Mathf.Clamp(vbl.y, BLMin.y, BLMax.y), Mathf.Clamp(vbl.z, BLMin.z, BLMax.z));

                // Apply scale
                vtl = scaleDelta.MultiplyPoint3x4(vtl - cData.InitialPosition) + cData.InitialPosition;
                vtr = scaleDelta.MultiplyPoint3x4(vtr - cData.InitialPosition) + cData.InitialPosition;
                vbr = scaleDelta.MultiplyPoint3x4(vbr - cData.InitialPosition) + cData.InitialPosition;
                vbl = scaleDelta.MultiplyPoint3x4(vbl - cData.InitialPosition) + cData.InitialPosition;

                // Apply rotation
                Vector3 pivot;
                Matrix4x4 matrix;
                for (int i = 0; i < rotations.Count; i++)
                {
                    pivot = pivots[i];
                    matrix = Matrix4x4.Rotate(rotations[i]);

                    vtl = matrix.MultiplyPoint3x4(vtl - pivot) + pivot;
                    vtr = matrix.MultiplyPoint3x4(vtr - pivot) + pivot;
                    vbr = matrix.MultiplyPoint3x4(vbr - pivot) + pivot;
                    vbl = matrix.MultiplyPoint3x4(vbl - pivot) + pivot;
                }

                // Apply transformation
                vtl += positionDelta * (context.ScaleAnimations ? cData.info.referenceScale : 1);
                vtr += positionDelta * (context.ScaleAnimations ? cData.info.referenceScale : 1);
                vbr += positionDelta * (context.ScaleAnimations ? cData.info.referenceScale : 1);
                vbl += positionDelta * (context.ScaleAnimations ? cData.info.referenceScale : 1);

                BL_Result = vbl;
                TL_Result = vtl;
                TR_Result = vtr;
                BR_Result = vbr;
            }

            public void UpdateVertexOffsets()
            {
                if (cData.positionDirty)
                {
                    positionDelta += (cData.Position - cData.InitialPosition);
                }

                if (cData.scaleDirty)
                {
                    scaleDelta *= Matrix4x4.Scale(cData.Scale);
                }

                if (cData.rotationDirty)
                {
                    if (cData.Rotation != Quaternion.identity || cData.Rotation.eulerAngles == Vector3.zero)
                    {
                        rotations.Add(cData.Rotation);
                        pivots.Add(cData.InitialPosition + (cData.RotationPivot - cData.InitialPosition) * (context.ScaleAnimations ? cData.info.referenceScale : 1));
                    }
                    //rotation = cData.Rotation * rotation;
                    //rotationPivot += (cData.RotationPivot - cData.initialPosition) * (context.scaleAnimations ? cData.info.referenceScale : 1);
                }

                if (cData.verticesDirty)
                {
                    Vector3 deltaTL = (cData.mesh.TL_Position - cData.mesh.initial.TL_Position) * (context.ScaleAnimations ? cData.info.referenceScale : 1);
                    Vector3 deltaTR = (cData.mesh.TR_Position - cData.mesh.initial.TR_Position) * (context.ScaleAnimations ? cData.info.referenceScale : 1);
                    Vector3 deltaBR = (cData.mesh.BR_Position - cData.mesh.initial.BR_Position) * (context.ScaleAnimations ? cData.info.referenceScale : 1);
                    Vector3 deltaBL = (cData.mesh.BL_Position - cData.mesh.initial.BL_Position) * (context.ScaleAnimations ? cData.info.referenceScale : 1);

                    TL += deltaTL;
                    TR += deltaTR;
                    BR += deltaBR;
                    BL += deltaBL;

                    TLMax = new Vector3(Mathf.Max(cData.mesh.initial.TL_Position.x + deltaTL.x, TLMax.x), Mathf.Max(cData.mesh.initial.TL_Position.y + deltaTL.y, TLMax.y), Mathf.Max(cData.mesh.initial.TL_Position.z + deltaTL.z, TLMax.z));
                    TRMax = new Vector3(Mathf.Max(cData.mesh.initial.TR_Position.x + deltaTR.x, TRMax.x), Mathf.Max(cData.mesh.initial.TR_Position.y + deltaTR.y, TRMax.y), Mathf.Max(cData.mesh.initial.TR_Position.z + deltaTR.z, TRMax.z));
                    BRMax = new Vector3(Mathf.Max(cData.mesh.initial.BR_Position.x + deltaBR.x, BRMax.x), Mathf.Max(cData.mesh.initial.BR_Position.y + deltaBR.y, BRMax.y), Mathf.Max(cData.mesh.initial.BR_Position.z + deltaBR.z, BRMax.z));
                    BLMax = new Vector3(Mathf.Max(cData.mesh.initial.BL_Position.x + deltaBL.x, BLMax.x), Mathf.Max(cData.mesh.initial.BL_Position.y + deltaBL.y, BLMax.y), Mathf.Max(cData.mesh.initial.BL_Position.z + deltaBL.z, BLMax.z));

                    TLMin = new Vector3(Mathf.Min(cData.mesh.initial.TL_Position.x + deltaTL.x, TLMin.x), Mathf.Min(cData.mesh.initial.TL_Position.y + deltaTL.y, TLMin.y), Mathf.Min(cData.mesh.initial.TL_Position.z + deltaTL.z, TLMin.z));
                    TRMin = new Vector3(Mathf.Min(cData.mesh.initial.TR_Position.x + deltaTR.x, TRMin.x), Mathf.Min(cData.mesh.initial.TR_Position.y + deltaTR.y, TRMin.y), Mathf.Min(cData.mesh.initial.TR_Position.z + deltaTR.z, TRMin.z));
                    BRMin = new Vector3(Mathf.Min(cData.mesh.initial.BR_Position.x + deltaBR.x, BRMin.x), Mathf.Min(cData.mesh.initial.BR_Position.y + deltaBR.y, BRMin.y), Mathf.Min(cData.mesh.initial.BR_Position.z + deltaBR.z, BRMin.z));
                    BLMin = new Vector3(Mathf.Min(cData.mesh.initial.BL_Position.x + deltaBL.x, BLMin.x), Mathf.Min(cData.mesh.initial.BL_Position.y + deltaBL.y, BLMin.y), Mathf.Min(cData.mesh.initial.BL_Position.z + deltaBL.z, BLMin.z));
                }

                if (cData.colorsDirty)
                {
                    if (cData.alphasDirty)
                    {
                        BL_Color = cData.mesh.GetColor(0);
                        TL_Color = cData.mesh.GetColor(1);
                        TR_Color = cData.mesh.GetColor(2);
                        BR_Color = cData.mesh.GetColor(3);
                    }
                    else
                    {
                        Color32 color = cData.mesh.GetColor(0);
                        color.a = BL_Color.a;
                        BL_Color = color;

                        color = cData.mesh.GetColor(1);
                        color.a = TL_Color.a;
                        TL_Color = color;

                        color = cData.mesh.GetColor(2);
                        color.a = TR_Color.a;
                        TR_Color = color;

                        color = cData.mesh.GetColor(3);
                        color.a = BR_Color.a;
                        BR_Color = color;
                    }
                }
                else if (cData.alphasDirty)
                {
                    BL_Color.a = cData.mesh.GetAlpha(0);
                    TL_Color.a = cData.mesh.GetAlpha(1);
                    TR_Color.a = cData.mesh.GetAlpha(2);
                    BR_Color.a = cData.mesh.GetAlpha(3);
                }

                if (cData.uvsDirty)
                {
                    BL_UV = cData.mesh.GetUV0(0);
                    TL_UV = cData.mesh.GetUV0(1);
                    TR_UV = cData.mesh.GetUV0(2);
                    BR_UV = cData.mesh.GetUV0(3);

                    BL_UV2 = cData.mesh.GetUV2(0);
                    TL_UV2 = cData.mesh.GetUV2(1);
                    TR_UV2 = cData.mesh.GetUV2(2);
                    BR_UV2 = cData.mesh.GetUV2(3);
                }
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
                bool isExcludedBasic = IsExcludedBasic(cData.info.character);

                if (IsExcludedShow(cData.info.character))
                {
                    Animate(dummyShow, false);
                    if (!isExcludedBasic)
                    {
                        AnimateBasic(false);
                        AnimateBasic(true);
                    }
                }
                else if (show.Count == 0)
                {
                    if (isExcludedBasic)
                    {
                        Animate(defaultShow, defaultShow.late);
                    }
                    else if (defaultShow.late)
                    {
                        AnimateBasic(false);
                        Animate(defaultShow, true);
                        AnimateBasic(true);
                    }
                    else
                    {
                        Animate(defaultShow, false);
                        AnimateBasic(false);
                        AnimateBasic(true);
                    }

                    allDone = defaultShow.Finished(cData.info.index);
                }
                else
                {
                    allDone = AnimateShowList(false);
                    if (!isExcludedBasic) AnimateBasic(false);
                    allDone &= AnimateShowList(true);
                    if (!isExcludedBasic) AnimateBasic(true);
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

                ApplyVertices();
                ignoreVisibilityChanges = prev;
                return;
            }

            else if (vState == VisibilityState.Hiding)
            {
                bool prev = ignoreVisibilityChanges;
                ignoreVisibilityChanges = true;

                bool done = true;
                bool isExcludedBasic = IsExcludedBasic(cData.info.character);

                if (IsExcludedHide(cData.info.character))
                {
                    Animate(dummyHide, false);
                    if (!isExcludedBasic)
                    {
                        AnimateBasic(false);
                        AnimateBasic(true);
                    }
                }
                else if (hide.Count == 0)
                {
                    if (isExcludedBasic)
                    {
                        Animate(defaultHide, defaultHide.late);
                    }
                    else if (defaultShow.late)
                    {
                        AnimateBasic(false);
                        Animate(defaultHide, true);
                        AnimateBasic(true);
                    }
                    else
                    {
                        Animate(defaultHide, false);
                        AnimateBasic(false);
                        AnimateBasic(true);
                    }

                    done = defaultHide.Finished(cData.info.index);
                }
                else
                {
                    done = AnimateHideList(false);
                    if (!isExcludedBasic) AnimateBasic(false);
                    if (!done) done = AnimateHideList(true);
                    if (!isExcludedBasic) AnimateBasic(true);
                }

                if (done)
                {
                    ignoreVisibilityChanges = false;
                    Mediator.SetVisibilityState(cData, VisibilityState.Hidden);
                    ignoreVisibilityChanges = prev;
                    return;
                }
                else
                {
                    ApplyVertices();
                }

                ignoreVisibilityChanges = prev;
                return;
            }

            if (vState != VisibilityState.Shown)
            { Debug.LogWarning("This should be unreachable! - BUG"); }

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

                //for (int i = 0; i < 4; i++)
                //{
                //    cData.SetVertex(i, cData.mesh.initial.GetVertex(i)); // cData.initialMesh.GetPosition(i));
                //}

                ca.animation.Animate(cData, ca.roContext);

                state.UpdateVertexOffsets();
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
        private void OnTextChanged(bool textContentChanged, ReadOnlyCollection<CharData> oldCharData, ReadOnlyCollection<VisibilityState> oldVisibilities)
        {
            if (textContentChanged)
            {
                SetDummies();
                PostProcessTags();
                SetDefault(TMPAnimationType.Show);
                SetDefault(TMPAnimationType.Hide);
                if (IsAnimating) UpdateAnimations_Impl(0f);
                return;
            }

            bool tagsChanged = true;
            var oldTags = BasicTags;
            var newTags = processors.TagProcessors[basicCategory.Prefix].SelectMany(processed => processed.ProcessedTags).Select(tag => new EffectTagTuple(tag.Value, tag.Key));

            if (oldTags.SequenceEqual(newTags))
            {
                oldTags = ShowTags;
                newTags = processors.TagProcessors[showCategory.Prefix].SelectMany(processed => processed.ProcessedTags).Select(tag => new EffectTagTuple(tag.Value, tag.Key));

                if (oldTags.SequenceEqual(newTags))
                {
                    oldTags = HideTags;
                    newTags = processors.TagProcessors[hideCategory.Prefix].SelectMany(processed => processed.ProcessedTags).Select(tag => new EffectTagTuple(tag.Value, tag.Key));
                    if (oldTags.SequenceEqual(newTags))
                    {
                        tagsChanged = false;
                    }
                }
            }

            if (tagsChanged)
            {
                SetDummies();
                PostProcessTags();
                SetDefault(TMPAnimationType.Show);
                SetDefault(TMPAnimationType.Hide);
                if (IsAnimating) UpdateAnimations_Impl(0f);
            }

            SetDefault(TMPAnimationType.Show);
            SetDefault(TMPAnimationType.Hide);
            if (IsAnimating) UpdateAnimations_Impl(0f);
        }

        private void PopulateTimes(bool textContentChanged, IList<CharData> oldCharData)
        {
            if (!textContentChanged && Mediator.CharData.Count == visibleTimes?.Count)
            {
                return;
            }

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

            //if (state == VisibilityState.Shown)
            //{
            //    cData.Reset();
            //    UpdateVisibility(true);
            //}
            //else if (state == VisibilityState.Hidden)
            //{
            //    UpdateVisibility(false);
            //}

            if (state == VisibilityState.Hidden)
            {
                UpdateVisibility(false);
            }
            else
            {
                if (state == VisibilityState.Shown)
                {
                    cData.Reset();
                    UpdateVisibility(true);
                }

                if (prev == VisibilityState.Hidden)
                    UpdateAnimationTimes(basic);

                if (state == VisibilityState.Showing)
                    UpdateAnimationTimes(show);

                if (state == VisibilityState.Hiding)
                    UpdateAnimationTimes(hide);
            }

            // Reset the "finished" status of the relevant animations
            if (state == VisibilityState.Showing)
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
                    verts[vIndex + j] = cData.mesh.GetPosition(j);
                    colors[vIndex + j] = cData.mesh.GetColor(j);
                    uvs0[vIndex + j] = cData.mesh.GetUV0(j);
                    uvs2[vIndex + j] = cData.mesh.GetUV2(j);
                }
            }

            void SetVerticesToZero()
            {
                // Set the current mesh's vertices all to the initial mesh values
                for (int j = 0; j < 4; j++)
                {
                    cData.SetVertex(j, cData.InitialPosition);// cData.initialPosition);
                }
            }

            void SetVerticesToDefault()
            {
                // Set the current mesh's vertices all to the initial mesh values
                for (int j = 0; j < 4; j++)
                {
                    cData.SetVertex(j, cData.mesh.initial.GetPosition(j));
                }
            }

            void UpdateAnimationTimes(CachedCollection<CachedAnimation> coll)
            {
                var mm = coll.MinMaxAt(index);
                if (mm == null) return;

                if (animationsUseAnimatorTime)
                {
                    CachedAnimation ca;
                    for (int i = mm.MinIndex; i <= mm.MaxIndex; i++)
                    {
                        ca = coll[i];
                        if (ca.Indices.Contains(index))
                        {
                            ca.context.AnimationTime = 0f;
                        }
                    }
                }
                else
                {
                    CachedAnimation ca;
                    for (int i = mm.MinIndex; i <= mm.MaxIndex; i++)
                    {
                        ca = coll[i];
                        if (ca.Indices.Contains(index))
                        {
                            ca.context.AnimationTime = context.passed;
                        }
                    }
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
            if (Mediator == null)
            {
                Debug.LogWarning("Early return in resetallvisible");
                return;
            }
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
                    verts[vIndex + j] = cData.mesh.initial.GetPosition(j);
                    colors[vIndex + j] = cData.mesh.initial.GetColor(j);
                    uvs0[vIndex + j] = cData.mesh.initial.GetUV0(j);
                    uvs2[vIndex + j] = cData.mesh.initial.GetUV2(j);
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
        [System.NonSerialized, HideInInspector] float lastPreviewUpdateTime = 0f;

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
            if (Application.isPlaying) return;

            if (lastPreviewUpdateTime <= 0)
            {
                UpdateAnimations(0f);
            }
            else
            {
                UpdateAnimations_Impl(Time.time - lastPreviewUpdateTime);
            }

            EditorApplication.QueuePlayerLoopUpdate();
            lastPreviewUpdateTime = Time.time;
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

        internal void ResetTime()
        {
            context.passed = 0f;
            for (int i = 0; i < stateTimes.Count; i++)
            {
                stateTimes[i] = 0;
            }
            for (int i = 0; i < visibleTimes.Count; i++)
            {
                visibleTimes[i] = 0;
            }
            lastPreviewUpdateTime = Time.time;
            Mediator.ForceReprocess();
        }

        internal string CheckDefaultString(TMPAnimationType type)
        {
            string str;
            ITMPEffectDatabase<ITMPAnimation> tempDatabase;
            switch (type)
            {
                case TMPAnimationType.Show:
                    tempDatabase = new AnimationDatabase<TMPShowAnimationDatabase, TMPSceneShowAnimation>(database == null ? null : database.ShowAnimationDatabase, sceneShowAnimations);
                    str = defaultShowString;
                    break;

                case TMPAnimationType.Hide:
                    tempDatabase = new AnimationDatabase<TMPHideAnimationDatabase, TMPSceneHideAnimation>(database == null ? null : database.HideAnimationDatabase, sceneHideAnimations);
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

            if (!tempDatabase.ContainsEffect(tagInfo.name))
            {
                return "Tag with name " + tagInfo.name + " not defined;";
            }

            if ((animation = tempDatabase.GetEffect(tagInfo.name)) == null)
            {
                return "Tag with name " + tagInfo.name + " is defined, but not assigned an animation";
            }

            try
            {
                tagParams = ParsingUtility.GetTagParametersDict(str);
                if (!animation.ValidateParameters(tagParams))
                {
                    return "Parameters are not valid for this tag";
                }
            }
            catch
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
                QueueCharacterReset();
            }

            if (prevExcludedShowCharacters != excludedCharactersShow || prevShowExcludePunctuation != excludePunctuationShow)
            {
                prevExcludedShowCharacters = excludedCharactersShow;
                prevShowExcludePunctuation = excludePunctuationShow;
                RecalculateSegmentData(TMPAnimationType.Show);
                QueueCharacterReset();
            }

            if (prevExcludedHideCharacters != excludedCharactersHide || prevHideExcludePunctuation != excludePunctuationHide)
            {
                prevExcludedHideCharacters = excludedCharactersHide;
                prevHideExcludePunctuation = excludePunctuationHide;
                RecalculateSegmentData(TMPAnimationType.Hide);
                QueueCharacterReset();
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


        private bool characterResetQueued = false;
        private void QueueCharacterReset()
        {
            characterResetQueued = true;
        }
    }
}