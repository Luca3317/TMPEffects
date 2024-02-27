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
using TMPEffects.TMPAnimations.ShowAnimations;
using TMPEffects.TMPAnimations.HideAnimations;
using TMPEffects.Components.CharacterData;

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
        [SerializeField] private TMPAnimationDatabase database;
        [SerializeField] private AnimatorContext context;

        [SerializeField] private UpdateFrom updateFrom;
        [SerializeField] private bool animateOnStart = true;

        [SerializeField] private bool animationsOverride = false;
        [SerializeField] private string defaultShowString;
        [SerializeField] private string defaultHideString;

        [SerializeField] private string excludedCharacters = "";
        [SerializeField] private string excludedCharactersShow = "";
        [SerializeField] private string excludedCharactersHide = "";

        [SerializeField] private bool excludePunctuation = false;
        [SerializeField] private bool excludePunctuationShow = false;
        [SerializeField] private bool excludePunctuationHide = false;

        [SerializeField, SerializedDictionary(keyName: "Name", valueName: "Animation")]
        private SerializedDictionary<string, TMPSceneAnimation> sceneAnimations;
        [SerializeField, SerializedDictionary(keyName: "Name", valueName: "Show Animation")]
        private SerializedDictionary<string, TMPSceneShowAnimation> sceneShowAnimations;
        [SerializeField, SerializedDictionary(keyName: "Name", valueName: "Hide Animation")]
        private SerializedDictionary<string, TMPSceneHideAnimation> sceneHideAnimations;

        [System.NonSerialized] private TagProcessorManager processors;
        [System.NonSerialized] private TagCollectionManager<TMPAnimationCategory> tags;
        [System.NonSerialized] private bool isAnimating = false;

        [System.NonSerialized] private TMPAnimationCategory basicCategory;
        [System.NonSerialized] private TMPAnimationCategory showCategory;
        [System.NonSerialized] private TMPAnimationCategory hideCategory;

        [System.NonSerialized] private AnimationDatabase<TMPSceneAnimation> basicDatabase;
        [System.NonSerialized] private AnimationDatabase<TMPSceneShowAnimation> showDatabase;
        [System.NonSerialized] private AnimationDatabase<TMPSceneHideAnimation> hideDatabase;

        [System.NonSerialized] private CachedCollection<CachedAnimation> basic;
        [System.NonSerialized] private CachedCollection<CachedAnimation> show;
        [System.NonSerialized] private CachedCollection<CachedAnimation> hide;

        [System.NonSerialized] private CachedAnimation dummyShow;
        [System.NonSerialized] private CachedAnimation dummyHide;
        [System.NonSerialized] private CachedAnimation defaultShow;
        [System.NonSerialized] private CachedAnimation defaultHide;

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

            SetDummyShow();
            SetDummyHide();

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

            Mediator.ForceReprocess();

#if UNITY_EDITOR
            StopPreview();
#endif

            UnsubscribeFromMediator();
        }

        private void SubscribeToMediator()
        {
            Mediator.OnVisibilityStateUpdated += EnsureCorrectTiming; // Ensure visibility time of object is persistent with context.UseScaledTime; TODO: likely move this into mediator or TMPEffectComponent
            Mediator.CharDataPopulated += PostProcessTags; // 
            Mediator.TextChanged += OnTextChanged; // Will update animations once; otherwise, depending on timing of text change, there'll be a frame of unanimated text
            Mediator.ForcedUpdate += OnForcedUpdate; // Will update animations at indices once; otherwise, depending on timing of required update, there'll be a frame of unanimated text
        }

        private void UnsubscribeFromMediator()
        {
            Mediator.OnVisibilityStateUpdated -= EnsureCorrectTiming;
            Mediator.CharDataPopulated -= PostProcessTags;
            Mediator.TextChanged -= OnTextChanged;
            Mediator.ForcedUpdate -= OnForcedUpdate;

            FreeMediator();
        }

        private void PrepareForProcessing()
        {
            // Reset database wrappers
            basicDatabase = new AnimationDatabase<TMPSceneAnimation>(database == null ? null : database.BasicAnimationDatabase, sceneAnimations);
            showDatabase = new AnimationDatabase<TMPSceneShowAnimation>(database == null ? null : database.ShowAnimationDatabase, sceneShowAnimations);
            hideDatabase = new AnimationDatabase<TMPSceneHideAnimation>(database == null ? null : database.HideAnimationDatabase, sceneHideAnimations);

            // Reset categories
            basicCategory = new TMPAnimationCategory(ANIMATION_PREFIX, basicDatabase);
            showCategory = new TMPAnimationCategory(SHOW_ANIMATION_PREFIX, showDatabase);
            hideCategory = new TMPAnimationCategory(HIDE_ANIMATION_PREFIX, hideDatabase);

            // Reset tagcollection & cachedcollection
            tags = new();
            var roCData = new ReadOnlyCollection<CharData>(Mediator.CharData);
            basic = new CachedCollection<CachedAnimation>(new AnimationCacher(basicCategory, context, roCData, (x) => !IsExcludedBasic(x)), tags.AddKey(basicCategory));
            show = new CachedCollection<CachedAnimation>(new AnimationCacher(showCategory, context, roCData, (x) => !IsExcludedShow(x)), tags.AddKey(showCategory));
            hide = new CachedCollection<CachedAnimation>(new AnimationCacher(hideCategory, context, roCData, (x) => !IsExcludedHide(x)), tags.AddKey(hideCategory));

            // Reset processors
            processors ??= new();
            processors.UnregisterFrom(Mediator.Processor);
            processors.Clear();

            processors.AddProcessor(basicCategory.Prefix, new TagProcessor(basicCategory));
            processors.AddProcessor(showCategory.Prefix, new TagProcessor(showCategory));
            processors.AddProcessor(hideCategory.Prefix, new TagProcessor(hideCategory));

            processors.RegisterTo(Mediator.Processor);

            SetDefault(TMPAnimationType.Show);
            SetDefault(TMPAnimationType.Hide);
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
                    cacher = new AnimationCacher(database?.ShowAnimationDatabase, context, new ReadOnlyCollection<CharData>(Mediator.CharData), x => !IsExcludedShow(x));
                    defaultShow = cacher.CacheTag(new EffectTag(tagInfo.name, tagInfo.prefix, tagParams), new EffectTagIndices());
                    break;

                case TMPAnimationType.Hide:
                    cacher = new AnimationCacher(database?.HideAnimationDatabase, context, new ReadOnlyCollection<CharData>(Mediator.CharData), x => !IsExcludedHide(x));
                    defaultHide = cacher.CacheTag(new EffectTag(tagInfo.name, tagInfo.prefix, tagParams), new EffectTagIndices());
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

        private void OnDatabaseChanged()
        {
            PrepareForProcessing();
        }

        private void SetDummyShow()
        {
            if (dummyShow != null) return;
            EffectTag tag = new EffectTag("Dummy Show Animation", ' ', null);

            DummyDatabase database = new DummyDatabase("Dummy Show Animation", ScriptableObject.CreateInstance<DummyShowAnimation>());
            AnimationCacher cacher = new AnimationCacher(database, context, new ReadOnlyCollection<CharData>(Mediator.CharData), (x) => !IsExcludedShow(x));
            dummyShow = cacher.CacheTag(tag, new EffectTagIndices());
        }

        private void SetDummyHide()
        {
            if (dummyHide != null) return;
            EffectTag tag = new EffectTag("Dummy Hide Animation", ' ', null);

            DummyDatabase database = new DummyDatabase("Dummy Hide Animation", ScriptableObject.CreateInstance<DummyHideAnimation>());
            AnimationCacher cacher = new AnimationCacher(database, context, new ReadOnlyCollection<CharData>(Mediator.CharData), (x) => !IsExcludedHide(x));
            dummyHide = cacher.CacheTag(tag, new EffectTagIndices());
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
            Mediator.ForceReprocess();
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

            RecalculateSegmentData(hide, x => !IsExcludedBasic(x));
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

            RecalculateSegmentData(hide, x => !IsExcludedShow(x));
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

            RecalculateSegmentData(hide, x => !IsExcludedHide(x));
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

            context.passedTime += deltaTime;

            for (int i = 0; i < Mediator.CharData.Count; i++)
            {
                CharData cData = Mediator.CharData[i];
                UpdateCharacterAnimation(ref cData, deltaTime, i, false);
            }

            if (Mediator.Text.mesh != null)
                Mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            sw.Stop();
        }

        private void UpdateCharacterAnimation(ref CharData cData, float deltaTime, int index, bool updateVertices = true)
        {
            if (!AnimateCharacter(index, ref cData)) return;

            context.deltaTime = deltaTime;

            UpdateCharacterAnimation_Impl(index);

            // TODO only set actually changed meshes; dirty flag on cdata & cehcking of uv vert color
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

        private bool AnimateCharacter(int index, ref CharData cData)
        {
            return cData.info.isVisible && // If not visible, e.g. whitespace, dont animate
                cData.visibilityState != VisibilityState.Hidden && // If hidden, dont animate
                (basic.HasAnyContaining(index) || cData.visibilityState != VisibilityState.Shown); // If has no animations, dont animate
        }

        private int UpdateCharacterAnimation_Impl(int index)
        {
            CharData cData = Mediator.CharData[index];
            if (!cData.info.isVisible || cData.visibilityState == VisibilityState.Hidden) return 0;

            int applied = 0;

            Vector3 positionDelta = Vector3.zero;
            Matrix4x4 scaleDelta = Matrix4x4.Scale(Vector3.one);
            Quaternion rotation = Quaternion.identity;
            Vector3 rotationPivot = cData.info.initialPosition;

            Vector3 TL = Vector3.zero;//Data.mesh.initial.vertex_TL.position;
            Vector3 TR = Vector3.zero;//Data.mesh.initial.vertex_TR.position;
            Vector3 BR = Vector3.zero;//Data.mesh.initial.vertex_BR.position;
            Vector3 BL = Vector3.zero;//cData.mesh.initial.vertex_BL.position;

            Vector3 TLMax = cData.mesh.initial.vertex_TL.position;
            Vector3 TRMax = cData.mesh.initial.vertex_TR.position;
            Vector3 BRMax = cData.mesh.initial.vertex_BR.position;
            Vector3 BLMax = cData.mesh.initial.vertex_BL.position;

            Vector3 TLMin = cData.mesh.initial.vertex_TL.position;
            Vector3 TRMin = cData.mesh.initial.vertex_TR.position;
            Vector3 BRMin = cData.mesh.initial.vertex_BR.position;
            Vector3 BLMin = cData.mesh.initial.vertex_BL.position;

            Vector2 TL_UV = cData.mesh.initial.vertex_TL.uv;
            Vector2 TR_UV = cData.mesh.initial.vertex_TR.uv;
            Vector2 BR_UV = cData.mesh.initial.vertex_BR.uv;
            Vector2 BL_UV = cData.mesh.initial.vertex_BL.uv;

            Vector2 TL_UV2 = cData.mesh.initial.vertex_TL.uv2;
            Vector2 TR_UV2 = cData.mesh.initial.vertex_TR.uv2;
            Vector2 BR_UV2 = cData.mesh.initial.vertex_BR.uv2;
            Vector2 BL_UV2 = cData.mesh.initial.vertex_BL.uv2;

            Color32 TL_Color = cData.mesh.initial.vertex_TL.color;
            Color32 TR_Color = cData.mesh.initial.vertex_TR.color;
            Color32 BR_Color = cData.mesh.initial.vertex_BR.color;
            Color32 BL_Color = cData.mesh.initial.vertex_BL.color;

            if (cData.visibilityState == VisibilityState.ShowAnimation)
            {
                if (!IsExcludedShow(cData.info.character))
                {
                    AnimateList(TMPAnimationType.Show);
                }
            }
            else if (cData.visibilityState == VisibilityState.HideAnimation)
            {
                if (!IsExcludedHide(cData.info.character))
                {
                    AnimateList(TMPAnimationType.Hide);
                }
            }

            if (cData.visibilityState == VisibilityState.Hidden || IsExcludedBasic(cData.info.character))
            {
                ApplyVertices();
                return 1;
            }

            AnimateList(TMPAnimationType.Basic);
            ApplyVertices();
            return applied;

            void Animate(CachedAnimation ca)
            {
                cData.Reset();
                cData.segmentIndex = index - ca.Indices.StartIndex;

                for (int i = 0; i < 4; i++)
                {
                    cData.SetVertex(i, cData.mesh.initial.GetPosition(i)); // cData.initialMesh.GetPosition(i));
                }

                ca.animation.ResetParameters();
                ca.animation.SetParameters(ca.Tag.Parameters);
                ca.animation.Animate(ref cData, ca.context);

                UpdateVertexOffsets();

                applied++;
            }

            void AnimateList(TMPAnimationType type)
            {
                CachedCollection<CachedAnimation> cc = basic;
                switch (type)
                {
                    case TMPAnimationType.Show:
                        if (show.Count == 0)
                        {
                            Animate(defaultShow);
                            return;
                        }
                        cc = show;
                        break;

                    case TMPAnimationType.Hide:
                        if (hide.Count == 0)
                        {
                            Animate(defaultHide);
                            return;
                        }
                        cc = hide;
                        break;
                }

                CachedCollection<CachedAnimation>.MinMax mm = cc.MinMaxAt(index);
                if (mm == null) return;

                if (animationsOverride)
                {
                    for (int i = mm.MaxIndex; i >= mm.MinIndex; i--)
                    {
                        CachedAnimation ca = cc[i];
                        if (ca.Indices.Contains(index))
                        {
                            Animate(ca);
                            if (!(ca.overrides != null && !ca.overrides.Value))
                                break;
                        }
                    }
                }
                else
                {
                    for (int i = mm.MaxIndex; i >= mm.MinIndex; i--)
                    {
                        CachedAnimation ca = cc[i];
                        if (ca.Indices.Contains(index))
                        {
                            Animate(ca);
                            if (ca.overrides != null && ca.overrides.Value)
                                break;
                        }
                    }
                }
            }

            void UpdateVertexOffsets()
            {
                if (cData.positionDirty) positionDelta += (cData.Position - cData.info.initialPosition);
                if (cData.scaleDirty)
                {
                    scaleDelta *= Matrix4x4.Scale(cData.Scale);
                }
                if (cData.rotationDirty)
                {
                    rotation = cData.Rotation * rotation;
                    rotationPivot += (cData.RotationPivot - cData.info.initialPosition) * (context.scaleAnimations ? cData.info.referenceScale : 1);
                }

                if (cData.verticesDirty)
                {
                    Vector3 deltaTL = (cData.mesh.vertex_TL.position - cData.mesh.initial.vertex_TL.position) * (context.scaleAnimations ? cData.info.referenceScale : 1);
                    Vector3 deltaTR = (cData.mesh.vertex_TR.position - cData.mesh.initial.vertex_TR.position) * (context.scaleAnimations ? cData.info.referenceScale : 1);
                    Vector3 deltaBR = (cData.mesh.vertex_BR.position - cData.mesh.initial.vertex_BR.position) * (context.scaleAnimations ? cData.info.referenceScale : 1);
                    Vector3 deltaBL = (cData.mesh.vertex_BL.position - cData.mesh.initial.vertex_BL.position) * (context.scaleAnimations ? cData.info.referenceScale : 1);

                    TL += deltaTL;// (cData.currentMesh.vertex_TL.position - cData.mesh.initial.vertex_TL.position);
                    TR += deltaTR;// (cData.currentMesh.vertex_TR.position - cData.mesh.initial.vertex_TR.position);
                    BR += deltaBR;// (cData.currentMesh.vertex_BR.position - cData.mesh.initial.vertex_BR.position);
                    BL += deltaBL;// (cData.currentMesh.vertex_BL.position - cData.mesh.initial.vertex_BL.position);

                    TLMax = new Vector3(Mathf.Max(cData.mesh.initial.vertex_TL.position.x + deltaTL.x, TLMax.x), Mathf.Max(cData.mesh.initial.vertex_TL.position.y + deltaTL.y, TLMax.y), Mathf.Max(cData.mesh.initial.vertex_TL.position.z + deltaTL.z, TLMax.z));
                    TRMax = new Vector3(Mathf.Max(cData.mesh.initial.vertex_TR.position.x + deltaTR.x, TRMax.x), Mathf.Max(cData.mesh.initial.vertex_TR.position.y + deltaTR.y, TRMax.y), Mathf.Max(cData.mesh.initial.vertex_TR.position.z + deltaTR.z, TRMax.z));
                    BRMax = new Vector3(Mathf.Max(cData.mesh.initial.vertex_BR.position.x + deltaBR.x, BRMax.x), Mathf.Max(cData.mesh.initial.vertex_BR.position.y + deltaBR.y, BRMax.y), Mathf.Max(cData.mesh.initial.vertex_BR.position.z + deltaBR.z, BRMax.z));
                    BLMax = new Vector3(Mathf.Max(cData.mesh.initial.vertex_BL.position.x + deltaBL.x, BLMax.x), Mathf.Max(cData.mesh.initial.vertex_BL.position.y + deltaBL.y, BLMax.y), Mathf.Max(cData.mesh.initial.vertex_BL.position.z + deltaBL.z, BLMax.z));

                    TLMin = new Vector3(Mathf.Min(cData.mesh.initial.vertex_TL.position.x + deltaTL.x, TLMin.x), Mathf.Min(cData.mesh.initial.vertex_TL.position.y + deltaTL.y, TLMin.y), Mathf.Min(cData.mesh.initial.vertex_TL.position.z + deltaTL.z, TLMin.z));
                    TRMin = new Vector3(Mathf.Min(cData.mesh.initial.vertex_TR.position.x + deltaTR.x, TRMin.x), Mathf.Min(cData.mesh.initial.vertex_TR.position.y + deltaTR.y, TRMin.y), Mathf.Min(cData.mesh.initial.vertex_TR.position.z + deltaTR.z, TRMin.z));
                    BRMin = new Vector3(Mathf.Min(cData.mesh.initial.vertex_BR.position.x + deltaBR.x, BRMin.x), Mathf.Min(cData.mesh.initial.vertex_BR.position.y + deltaBR.y, BRMin.y), Mathf.Min(cData.mesh.initial.vertex_BR.position.z + deltaBR.z, BRMin.z));
                    BLMin = new Vector3(Mathf.Min(cData.mesh.initial.vertex_BL.position.x + deltaBL.x, BLMin.x), Mathf.Min(cData.mesh.initial.vertex_BL.position.y + deltaBL.y, BLMin.y), Mathf.Min(cData.mesh.initial.vertex_BL.position.z + deltaBL.z, BLMin.z));
                }

                if (cData.colorsDirty)
                {
                    BL_Color = cData.mesh.GetColor(0);
                    TL_Color = cData.mesh.GetColor(1);
                    TR_Color = cData.mesh.GetColor(2);
                    BR_Color = cData.mesh.GetColor(3);
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

            void ApplyVertices()
            {
                // Apply vertex transformations
                Vector3 vtl = cData.mesh.initial.vertex_TL.position + TL;// * (context.scaleAnimations ? cData.info.referenceScale : 1);
                Vector3 vtr = cData.mesh.initial.vertex_TR.position + TR;// * (context.scaleAnimations ? cData.info.referenceScale : 1);
                Vector3 vbr = cData.mesh.initial.vertex_BR.position + BR;// * (context.scaleAnimations ? cData.info.referenceScale : 1);
                Vector3 vbl = cData.mesh.initial.vertex_BL.position + BL;// * (context.scaleAnimations ? cData.info.referenceScale : 1);

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


                cData.SetVertex(0, vbl);
                cData.SetVertex(1, vtl);
                cData.SetVertex(2, vtr);
                cData.SetVertex(3, vbr);


                //cData.mesh.SetPosition(0, vbl);
                //cData.mesh.SetPosition(1, vtl);
                //cData.mesh.SetPosition(2, vtr);
                //cData.mesh.SetPosition(3, vbr);

                cData.mesh.SetColor(0, BL_Color);
                cData.mesh.SetColor(1, TL_Color);
                cData.mesh.SetColor(2, TR_Color);
                cData.mesh.SetColor(3, BR_Color);

                cData.mesh.SetUV0(0, BL_UV);
                cData.mesh.SetUV0(1, TL_UV);
                cData.mesh.SetUV0(2, TR_UV);
                cData.mesh.SetUV0(3, BR_UV);

                cData.mesh.SetUV2(0, BL_UV2);
                cData.mesh.SetUV2(1, TL_UV2);
                cData.mesh.SetUV2(2, TR_UV2);
                cData.mesh.SetUV2(3, BR_UV2);

                Mediator.CharData[index] = cData;
            }
        }
        #endregion

        #region Event Callbacks
        private void OnTextChanged()
        {
            if (isAnimating) UpdateAnimations_Impl(0f);
        }

        private void OnForcedUpdate(int start, int length)
        {
            if (!IsAnimating) return;

            for (int i = 0; i < length; i++)
            {
                CharData cData = Mediator.CharData[start + i];
                UpdateCharacterAnimation(ref cData, 0f, start + i);
            }
        }

        private void EnsureCorrectTiming(int index, VisibilityState prev)
        {
            if (context == null) return;
            float passed = context.passedTime;
            CharData cData = Mediator.CharData[index];
            VisibilityState current = cData.visibilityState;
            cData.SetVisibilityState(prev, -1);
            cData.SetVisibilityState(current, passed);
            Mediator.CharData[index] = cData;
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
                if (!cInfo.isVisible || Mediator.CharData[i].visibilityState == VisibilityState.Hidden) continue;

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

                Mediator.CharData[i] = cData;
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

        private void RecalculateSegmentData(CachedCollection<CachedAnimation> coll, Predicate<char> animates)
        {
            ReadOnlyCollection<CharData> ro = new ReadOnlyCollection<CharData>(Mediator.CharData);
            foreach (var animation in coll)
            {
                animation.context.segmentData = new SegmentData(animation.Indices, ro, animates);
            }
        }
        #endregion

        #region Editor Only
#if UNITY_EDITOR
        [SerializeField, HideInInspector] bool preview = false;
        [SerializeField, HideInInspector] bool initDatabase = false;
        [SerializeField, HideInInspector] TMPAnimationDatabase prevDatabase = null;

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
            UpdateAnimations_Impl((float)EditorApplication.timeSinceStartup - context.passedTime);
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

        internal void UpdateProcessorsWrapper()
        {
            if (Mediator == null) return;
            OnDatabaseChanged();
        }

        internal string CheckDefaultString(TMPAnimationType type)
        {
            string str;
            ITMPEffectDatabase<ITMPAnimation> database;
            switch (type)
            {
                case TMPAnimationType.Show:
                    database = showDatabase;
                    defaultShow = dummyShow;
                    str = defaultShowString;
                    break;

                case TMPAnimationType.Hide:
                    database = hideDatabase;
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
            if (Mediator == null) return;

            if (prevDatabase != database ||
                    (database != null &&
                    (basicDatabase.Database != (ITMPEffectDatabase<ITMPAnimation>)database.BasicAnimationDatabase ||
                    showDatabase.Database != (ITMPEffectDatabase<ITMPAnimation>)database.ShowAnimationDatabase ||
                    hideDatabase.Database != (ITMPEffectDatabase<ITMPAnimation>)database.HideAnimationDatabase))
                )
            {
                prevDatabase = database;
                OnDatabaseChanged();
            }
        }
#endif
        #endregion

        // Wraps a database and sceneanimations dictionary
        private class AnimationDatabase<T> : ITMPEffectDatabase<ITMPAnimation> where T : TMPSceneAnimationBase
        {
            public ITMPEffectDatabase<ITMPAnimation> Database => database;
            public IDictionary<string, T> SceneAnimations => sceneAnimations;
            private readonly ITMPEffectDatabase<ITMPAnimation> database;
            private readonly IDictionary<string, T> sceneAnimations;
            public AnimationDatabase(ITMPEffectDatabase<ITMPAnimation> database, IDictionary<string, T> sceneAnimations)
            {
                this.database = database;
                this.sceneAnimations = sceneAnimations;
            }
            public bool ContainsEffect(string name)
            {
                bool contains = database != null && database.ContainsEffect(name);
                if (contains) return true;
                return sceneAnimations != null && sceneAnimations.ContainsKey(name) && sceneAnimations[name] != null;
            }
            public ITMPAnimation GetEffect(string name)
            {
                if (database != null && database.ContainsEffect(name)) return database.GetEffect(name);
                if (sceneAnimations != null && sceneAnimations.ContainsKey(name) && sceneAnimations[name] != null) return sceneAnimations[name];
                throw new KeyNotFoundException(name);
            }
        }

        // Handles logic for caching animations
        private class AnimationCacher : ITagCacher<CachedAnimation>
        {
            private readonly ITMPEffectDatabase<ITMPAnimation> database;
            private readonly IList<CharData> charData;
            private readonly AnimatorContext context;
            private readonly Predicate<char> animates;

            public AnimationCacher(ITMPEffectDatabase<ITMPAnimation> database, AnimatorContext context, IList<CharData> charData, Predicate<char> animates)
            {
                this.context = context;
                this.database = database;
                this.charData = charData;
                this.animates = animates;
            }

            public CachedAnimation CacheTag(EffectTag tag, EffectTagIndices indices)
            {
                CachedAnimation ca = new CachedAnimation(tag, new EffectTagIndices(indices.StartIndex, indices.IsOpen ? charData.Count : indices.EndIndex, indices.OrderAtIndex), database.GetEffect(tag.Name));
                ca.context.animatorContext = context;
                ca.context.segmentData = new SegmentData(ca.Indices, charData, animates);
                return ca;
            }
        }

        // Mock database used to use AnimationCacher with dummyshow/hide
        private struct DummyDatabase : ITMPEffectDatabase<ITMPAnimation>
        {
            private string name;
            private ITMPAnimation animation;

            public DummyDatabase(string name, ITMPAnimation animation)
            {
                this.name = name;
                this.animation = animation;
            }

            public bool ContainsEffect(string name)
            {
                return name == this.name;
            }

            public ITMPAnimation GetEffect(string name)
            {
                if (name == this.name) return this.animation;
                throw new KeyNotFoundException(name);
            }
        }
    }
}