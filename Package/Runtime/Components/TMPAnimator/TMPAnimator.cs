using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using TMPEffects.TMPAnimations;
using TMPEffects.Databases;
using TMPEffects.TextProcessing;
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
using TMPEffects.TMPAnimations.Animations;

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
    [HelpURL("https://tmpeffects.luca3317.dev/docs/tmpanimator.html")]
    [ExecuteAlways, DisallowMultipleComponent, RequireComponent(typeof(TMP_Text))]
    public class TMPAnimator : TMPEffectComponent
    {
        /// <summary>
        /// Whether the text is currently being animated.<br/>
        /// If <see cref="UpdateFrom"/> is set to <see cref="UpdateFrom.Script"/>, this will always evaluate to true.
        /// </summary>
#if UNITY_EDITOR
        public bool IsAnimating => updateFrom == UpdateFrom.Script || isAnimating || preview;
#else
        public bool IsAnimating => updateFrom == UpdateFrom.Script || isAnimating;
#endif

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
        public ITagCollection BasicTags => tags == null ? null : tags[basicCategory];
        /// <summary>
        /// All show animation tags parsed by the TMPAnimator.
        /// </summary>
        public ITagCollection ShowTags => tags == null ? null : tags[showCategory];
        /// <summary>
        /// All hide animation tags parsed by the TMPAnimator.
        /// </summary>
        public ITagCollection HideTags => tags == null ? null : tags[hideCategory];

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
        [Tooltip("The database used for processing animation tags (e.g. <wave>, <+fade>)")]
        [SerializeField] private TMPAnimationDatabase database = null;
        [SerializeField] private AnimatorContext context = new AnimatorContext();

        [Tooltip("Where to update the animations from. If set to Script, you will have to manually update animations from your own script")]
        [SerializeField] private UpdateFrom updateFrom = UpdateFrom.Update;
        [Tooltip("Whether to automatically start animating when entering playmode. Ignored if UpdateFrom is set to Script")]
        [SerializeField] private bool animateOnStart = true;

        [Tooltip("Whether animation tags override each other by default. You can set this individually on a per-tag basis by adding the override=true/false parameter to them")]
        [SerializeField] private bool animationsOverride = false;
        //[Tooltip("The default show animation to use, if a TMPWriter component is present")] Set in editor
        [SerializeField] private string defaultShowString = "";
        //[Tooltip("The default hide animation to use, if a TMPWriter component is present")]
        [SerializeField] private string defaultHideString = "";

        [Tooltip("Characters that are excluded from basic animations")]
        [SerializeField] private string excludedCharacters = "";
        [Tooltip("Characters that are excluded from show animations")]
        [SerializeField] private string excludedCharactersShow = "";
        [Tooltip("Characters that are excluded from hide animations")]
        [SerializeField] private string excludedCharactersHide = "";

        [Tooltip("Whether to exclude punctuation characters from basic animations")]
        [SerializeField] private bool excludePunctuation = false;
        [Tooltip("Whether to exclude punctuation characters from show animations")]
        [SerializeField] private bool excludePunctuationShow = false;
        [Tooltip("Whether to exclude punctuation characters from hide animations")]
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
        [System.NonSerialized] private AnimationDatabase<TMPAnimationDatabase, TMPSceneAnimation> mainDatabaseWrapper;

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

        [System.NonSerialized] private CharDataState state = new CharDataState();

        private const string falseUpdateAnimationsCallWarning = "Called UpdateAnimations while TMPAnimator {0} is set to automatically update from {1}; " +
            "If you want to manually control the animation updates, set its UpdateFrom property to \"Script\", " +
            "either through the inspector or through a script using the SetUpdateFrom method.";
        private const string falseStartStopAnimatingCallWarning = "Called {0} while TMPAnimator {1} is set to manually update from script; " +
            "If you want the TMPAnimator to automatically update and to use the Start / StopAnimating methods, set its UpdateFrom property to \"Update\", \"LateUpdate\" or \"FixedUpdate\", " +
            "either through the inspector or through a script using the SetUpdateFrom method.";
        #endregion

        #region Public methods

        #region Animation Controlling
        /// <summary>
        /// Update the current animations.<br/>
        /// You should only call this if <see cref="UpdateFrom"/> is set to <see cref="UpdateFrom.Script"/>,
        /// otherwise this will output a warning and return.
        /// </summary>
        public void UpdateAnimations(float deltaTime)
        {
            if (Mediator == null)
            {
                throw new System.InvalidOperationException("Animator is not enabled!");
            }

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
            if (Mediator == null)
            {
                throw new System.InvalidOperationException("Animator is not enabled!");
            }

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
            if (Mediator == null)
            {
                throw new System.InvalidOperationException("Animator is not enabled!");
            }

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

            ResetAllVisible();
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
            this.database = database;
            OnDatabaseChanged();
        }

        /// <summary>
        /// Set the default show animation.
        /// </summary>
        /// <param name="str">The default show animation as a string, e.g. "<+fade>".</param>
        public void SetDefaultShowString(string str)
        {
            defaultShowString = str;

            if (Mediator == null) return;
            SetDefault(TMPAnimationType.Show);
        }

        /// <summary>
        /// Set the default hide animation.
        /// </summary>
        /// <param name="str">The default hide animation as a string, e.g. "<-fade>".</param>
        public void SetDefaultHideString(string str)
        {
            defaultHideString = str;

            if (Mediator == null) return;
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

            if (Mediator != null)
            {
                RecalculateSegmentData(TMPAnimationType.Basic);
                QueueCharacterReset();
            }
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

            if (Mediator != null)
            {
                RecalculateSegmentData(TMPAnimationType.Show);
                QueueCharacterReset();
            }
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

            if (Mediator != null)
            {
                RecalculateSegmentData(TMPAnimationType.Hide);
                QueueCharacterReset();
            }
        }
        #endregion

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

        /// <summary>
        /// Reset the time of the animator.
        /// </summary>
        public void ResetTime()
        {
            if (Mediator == null) return;

            context.passed = 0f;
            for (int i = 0; i < stateTimes.Count; i++)
            {
                stateTimes[i] = 0f;
            }
            for (int i = 0; i < visibleTimes.Count; i++)
            {
                visibleTimes[i] = 0f;
            }

#if UNITY_EDITOR
            lastPreviewUpdateTime = Time.time;
#endif
        }

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
            if (preview && !Application.isPlaying) StopPreviewWithoutSet();
#endif

            basicDatabase?.Dispose();
            showDatabase?.Dispose();
            hideDatabase?.Dispose();
            mainDatabaseWrapper?.Dispose();

            UnsubscribeFromMediator();

#if UNITY_EDITOR
            // Queue a player loop update to instantly update scene view
            EditorApplication.delayCall += EditorApplication.QueuePlayerLoopUpdate;
#endif 
        }

        private void SubscribeToMediator()
        {
            timesIdentifier = new object();
            if (!Mediator.RegisterVisibilityProcessor(timesIdentifier))
            {
                Debug.LogError("Could not register as visibility processor!");
            }

            Mediator.TextChanged_Late += OnTextChanged_Late;
            Mediator.TextChanged_Early += OnTextChanged_Early;
            Mediator.VisibilityStateUpdated += OnVisibilityStateUpdated; // Handle Visibility updates
        }

        private void UnsubscribeFromMediator()
        {
            Mediator.TextChanged_Late -= OnTextChanged_Late;
            Mediator.TextChanged_Early -= OnTextChanged_Early;
            Mediator.VisibilityStateUpdated -= OnVisibilityStateUpdated;

            if (!Mediator.UnregisterVisibilityProcessor(timesIdentifier))
            {
                Debug.LogError("Could not unregister as visibility processor!");
            }

            var textComponent = Mediator.Text;
            FreeMediator();
            if (textComponent != null) textComponent.ForceMeshUpdate(false, true);
        }

        private void CreateContext()
        {
            context._VisibleTime = (i) => visibleTimes[i];
            context._StateTime = (i) => stateTimes[i];
            context.Animator = this;
            ResetTime();
        }

        private void PrepareForProcessing()
        {
            // Reset database wrappers
            basicDatabase?.Dispose();
            showDatabase?.Dispose();
            hideDatabase?.Dispose();
            mainDatabaseWrapper?.Dispose();


            basicDatabase = new AnimationDatabase<TMPBasicAnimationDatabase, TMPSceneAnimation>(database == null ? null : database.BasicAnimationDatabase == null ? null : database.BasicAnimationDatabase, sceneAnimations);
            showDatabase = new AnimationDatabase<TMPShowAnimationDatabase, TMPSceneShowAnimation>(database == null ? null : database.ShowAnimationDatabase == null ? null : database.ShowAnimationDatabase, sceneShowAnimations);
            hideDatabase = new AnimationDatabase<TMPHideAnimationDatabase, TMPSceneHideAnimation>(database == null ? null : database.HideAnimationDatabase == null ? null : database.HideAnimationDatabase, sceneHideAnimations);
            mainDatabaseWrapper = new AnimationDatabase<TMPAnimationDatabase, TMPSceneAnimation>(database == null ? null : database, null);

            basicDatabase.AddAnimation("sprite", new SpriteAnimation());


            basicDatabase.ObjectChanged += ReprocessOnDatabaseChange;
            showDatabase.ObjectChanged += ReprocessOnDatabaseChange;
            hideDatabase.ObjectChanged += ReprocessOnDatabaseChange;
            mainDatabaseWrapper.ObjectChanged += ReprocessOnDatabaseChange;

            // Reset categories
            basicCategory = new TMPAnimationCategory(ANIMATION_PREFIX, basicDatabase);
            showCategory = new TMPAnimationCategory(SHOW_ANIMATION_PREFIX, showDatabase);
            hideCategory = new TMPAnimationCategory(HIDE_ANIMATION_PREFIX, hideDatabase);


            // Reset tagcollection & cachedcollection => moved to postprocesstags
            //if (tags != null) tags.CollectionChanged -= OnTagCollectionChanged;
            //tags = new TagCollectionManager<TMPAnimationCategory>();
            //tags.CollectionChanged += OnTagCollectionChanged;

            //var roCData = new ReadOnlyCollection<CharData>(Mediator.CharData);
            //var roCDataState = new ReadOnlyCharDataState(state);
            //basic = new CachedCollection<CachedAnimation>(new AnimationCacher(basicCategory, roCDataState, context, roCData, (x) => !IsExcludedBasic(x)), tags.AddKey(basicCategory));
            //show = new CachedCollection<CachedAnimation>(new AnimationCacher(showCategory, roCDataState, context, roCData, (x) => !IsExcludedShow(x)), tags.AddKey(showCategory));
            //hide = new CachedCollection<CachedAnimation>(new AnimationCacher(hideCategory, roCDataState, context, roCData, (x) => !IsExcludedHide(x)), tags.AddKey(hideCategory));

            // Reset processors
            processors ??= new();
            processors.UnregisterFrom(Mediator.Processor);
            processors.Clear();

            processors.AddProcessor(basicCategory.Prefix, new TagProcessor(basicCategory));
            processors.AddProcessor(showCategory.Prefix, new TagProcessor(showCategory));
            processors.AddProcessor(hideCategory.Prefix, new TagProcessor(hideCategory));

            processors.RegisterTo(Mediator.Processor);
        }

        private void MainDatabaseWrapper_ObjectChanged(object sender)
        {
            throw new System.NotImplementedException();
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
            //if (Mediator == null)
            //{
            //    SetToDummy();
            //    return;
            //}

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
            var roCDataState = new ReadOnlyCharDataState(state);
            switch (type)
            {
                case TMPAnimationType.Show:
                    cacher = new AnimationCacher(database, roCDataState, context, Mediator.CharData, x => !IsExcludedShow(x));
                    defaultShow = cacher.CacheTag(new TMPEffectTag(tagInfo.name, tagInfo.prefix, tagParams), new TMPEffectTagIndices(0, -1, 0));
                    break;

                case TMPAnimationType.Hide:
                    cacher = new AnimationCacher(database, roCDataState, context, Mediator.CharData, x => !IsExcludedHide(x));
                    defaultHide = cacher.CacheTag(new TMPEffectTag(tagInfo.name, tagInfo.prefix, tagParams), new TMPEffectTagIndices(0, -1, 0));
                    break;
            }


            void SetToDummy()
            {
                switch (type)
                {
                    case TMPAnimationType.Show: defaultShow = dummyShow; break;
                    case TMPAnimationType.Hide: defaultHide = dummyHide; break;
                }

                if (dummyShow == null || dummyHide == null) Debug.LogWarning("Set default while dummy null - bug!");
            }
        }

        private void OnDatabaseChanged()
        {
            if (Mediator == null) return;
            PrepareForProcessing();
            Mediator.ForceReprocess();
        }

        private void ReprocessOnDatabaseChange(object sender)
        {
            OnDatabaseChanged();
        }

        private void SetDummies()
        {
            SetDummyShow();
            SetDummyHide();
        }

        private void SetDummyShow()
        {
            //if (dummyShow != null) return;
            TMPEffectTag tag = new TMPEffectTag("Dummy Show Animation", ' ', null);

            DummyDatabase database = new DummyDatabase("Dummy Show Animation", ScriptableObject.CreateInstance<DummyShowAnimation>());
            AnimationCacher cacher = new AnimationCacher(database, new ReadOnlyCharDataState(state), context, Mediator.CharData, (x) => !IsExcludedShow(x));
            dummyShow = cacher.CacheTag(tag, new TMPEffectTagIndices(0, -1, 0));
        }

        private void SetDummyHide()
        {
            //if (dummyHide != null) return; 
            TMPEffectTag tag = new TMPEffectTag("Dummy Hide Animation", ' ', null);

            DummyDatabase database = new DummyDatabase("Dummy Hide Animation", ScriptableObject.CreateInstance<DummyHideAnimation>());
            AnimationCacher cacher = new AnimationCacher(database, new ReadOnlyCharDataState(state), context, Mediator.CharData, (x) => !IsExcludedHide(x));
            dummyHide = cacher.CacheTag(tag, new TMPEffectTagIndices(0, -1, 0));
        }
        #endregion

        private void RecalculateSegmentData(TMPAnimationType type)
        {
            if (Mediator == null) return;

            switch (type)
            {
                case TMPAnimationType.Basic:
                    foreach (var anim in basic)
                        anim.context.SegmentData = new SegmentData(anim.Indices, Mediator.CharData, (c) => !IsExcludedBasic(c));
                    break;

                case TMPAnimationType.Show:
                    foreach (var anim in show)
                        anim.context.SegmentData = new SegmentData(anim.Indices, Mediator.CharData, (c) => !IsExcludedShow(c));
                    break;

                case TMPAnimationType.Hide:
                    foreach (var anim in hide)
                        anim.context.SegmentData = new SegmentData(anim.Indices, Mediator.CharData, (c) => !IsExcludedHide(c));
                    break;

                default: throw new System.ArgumentException();
            }
        }

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

        private void UpdateAnimations_Impl(float deltaTime)
        {
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
        }

        private void UpdateCharacterAnimation(CharData cData, float deltaTime, int index, bool updateVertices = true, bool forced = false)
        {
            if (!cData.info.isVisible || (!forced && !AnimateCharacter(index, cData))) return;

            context.deltaTime = deltaTime;

            UpdateCharacterAnimation_Impl(index);

            Mediator.ApplyMesh(cData);

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
                if (ca.late != late) return;
                if (ca.Finished(index)) return;

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

        private void OnTextChanged_Early(bool textContentChanged, ReadOnlyCollection<CharData> oldCharData)
        {
            PopulateTimes(textContentChanged, oldCharData);
            SetDummies();
            PostProcessTags();
            SetDefault(TMPAnimationType.Show);
            SetDefault(TMPAnimationType.Hide);
        }

        private void OnTextChanged_Late(bool textContentChanged, ReadOnlyCollection<CharData> oldCharData, ReadOnlyCollection<VisibilityState> oldVisibilities)
        {
            QueueCharacterReset();
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
                //Debug.LogError("Character did not change visbility but event was raised?");
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

                //if (prev == VisibilityState.Hidden)
                //    UpdateAnimationTimes(basic);

                //if (state == VisibilityState.Showing)
                //    UpdateAnimationTimes(show);

                //if (state == VisibilityState.Hiding)
                //    UpdateAnimationTimes(hide);
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
                // Set the current mesh's vertices all to the initial mesh values
                if (show) SetVerticesToDefault();
                else SetVerticesToZero();

                Mediator.ApplyMesh(cData);
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
        }

        private void PostProcessTags()
        {
            var roCData = new ReadOnlyCollection<CharData>(Mediator.CharData);
            var roCDataState = new ReadOnlyCharDataState(state);

            var kvpBasic = new KeyValuePair<TMPAnimationCategory, IEnumerable<KeyValuePair<TMPEffectTagIndices, TMPEffectTag>>>(basicCategory, processors.TagProcessors[basicCategory.Prefix][0].ProcessedTags);
            var kvpShow = new KeyValuePair<TMPAnimationCategory, IEnumerable<KeyValuePair<TMPEffectTagIndices, TMPEffectTag>>>(showCategory, processors.TagProcessors[showCategory.Prefix][0].ProcessedTags);
            var kvpHide = new KeyValuePair<TMPAnimationCategory, IEnumerable<KeyValuePair<TMPEffectTagIndices, TMPEffectTag>>>(hideCategory, processors.TagProcessors[hideCategory.Prefix][0].ProcessedTags);

            if (tags != null) tags.CollectionChanged -= OnTagCollectionChanged;
            tags = new TagCollectionManager<TMPAnimationCategory>(kvpBasic, kvpShow, kvpHide);
            tags.CollectionChanged += OnTagCollectionChanged;

            var basicCacher = new AnimationCacher(basicCategory, roCDataState, context, roCData, (x) => !IsExcludedBasic(x));
            var showCacher = new AnimationCacher(showCategory, roCDataState, context, roCData, (x) => !IsExcludedShow(x));
            var hideCacher = new AnimationCacher(hideCategory, roCDataState, context, roCData, (x) => !IsExcludedHide(x));

            basic = new CachedCollection<CachedAnimation>(basicCacher, tags[basicCategory]);
            show = new CachedCollection<CachedAnimation>(showCacher, tags[showCategory]);
            hide = new CachedCollection<CachedAnimation>(hideCacher, tags[hideCategory]);
        }
        #endregion

        #region Utility
        private void ResetAllVisible()
        {
            if (Mediator == null)
            {
                return;
            }

            var info = Mediator.Text.textInfo;

            TMP_CharacterInfo cInfo;

            // Iterate over all characters and apply the new meshes
            for (int i = 0; i < Mediator.CharData.Count; i++)
            {
                cInfo = info.characterInfo[i];
                if (!cInfo.isVisible || Mediator.VisibilityStates[i] == VisibilityState.Hidden) continue;

                CharData cData = Mediator.CharData[i];
                cData.Reset();

                Mediator.ApplyMesh(cData);
            }

            if (Mediator.Text.mesh != null)
                Mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
        #endregion

        #region Editor Only
#if UNITY_EDITOR
#pragma warning disable CS0414
        [SerializeField, HideInInspector] private bool preview = false;
        [SerializeField, HideInInspector] private bool useDefaultDatabase = true;
        [SerializeField, HideInInspector] private bool initDatabase = false;
        [System.NonSerialized, HideInInspector] private float lastPreviewUpdateTime = 0f;
#pragma warning restore CS0414

        internal delegate void VoidHandler();
        internal event VoidHandler OnResetComponent;

        internal void StartPreview()
        {
            if (Mediator == null) return;

            preview = true;
            EditorApplication.update -= UpdatePreview;
            EditorApplication.update += UpdatePreview;
        }

        internal void StopPreviewWithoutSet()
        {
            if (Mediator == null) return;
            EditorApplication.update -= UpdatePreview;
            ResetAnimations();
        }

        internal void StopPreview()
        {
            if (Mediator == null) return;
            preview = false;
            EditorApplication.update -= UpdatePreview;
            ResetAnimations();
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

        private void UpdatePreview()
        {
            if (Mediator == null) return;
            if (Application.isPlaying) return;

            if (lastPreviewUpdateTime <= 0)
            {
                UpdateAnimations_Impl(0f);
            }
            else
            {
                UpdateAnimations_Impl(Time.time - lastPreviewUpdateTime);
            }

            EditorApplication.QueuePlayerLoopUpdate();
            lastPreviewUpdateTime = Time.time;
        }

        private void Reset()
        {
            StopPreview();
            if (enabled)
            {
                enabled = false;
                EditorApplication.delayCall += () => this.enabled = true;
                EditorApplication.delayCall += () => EditorApplication.delayCall += EditorApplication.QueuePlayerLoopUpdate;
            }

            OnResetComponent?.Invoke();
        }

        internal string CheckDefaultString(TMPAnimationType type)
        {
            string str;
            ITMPEffectDatabase<ITMPAnimation> tempDatabase;
            switch (type)
            {
                case TMPAnimationType.Show:
                    tempDatabase = new AnimationDatabase<TMPShowAnimationDatabase, TMPSceneShowAnimation>(database == null ? null : database.ShowAnimationDatabase == null ? null : database.ShowAnimationDatabase, sceneShowAnimations);
                    str = defaultShowString;
                    break;

                case TMPAnimationType.Hide:
                    tempDatabase = new AnimationDatabase<TMPHideAnimationDatabase, TMPSceneHideAnimation>(database == null ? null : database.HideAnimationDatabase == null ? null : database.HideAnimationDatabase, sceneHideAnimations);
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

        internal void OnChangedBasicExclusion()
        {
            if (Mediator == null) return;
            RecalculateSegmentData(TMPAnimationType.Basic);
            QueueCharacterReset();
        }
        internal void OnChangedShowExclusion()
        {
            if (Mediator == null) return;
            RecalculateSegmentData(TMPAnimationType.Show);
            QueueCharacterReset();
        }
        internal void OnChangedHideExclusion()
        {
            if (Mediator == null) return;
            RecalculateSegmentData(TMPAnimationType.Hide);
            QueueCharacterReset();
        }

        internal void OnChangedDatabase()
        {
            if (Mediator == null) return;
            OnDatabaseChanged();
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