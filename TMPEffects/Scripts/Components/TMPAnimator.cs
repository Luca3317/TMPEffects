using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using TMPEffects.Animations;
using TMPEffects.Databases;
using TMPEffects.Tags;
using TMPEffects.TextProcessing;
using TMPEffects.TextProcessing.TagProcessors;

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
    /// </remarks>
    [ExecuteAlways, DisallowMultipleComponent]
    public class TMPAnimator : TMPEffectComponent
    {
        /// <summary>
        /// Is the text currently being animated?
        /// </summary>
        public bool IsAnimating => isAnimating;
        /// <summary>
        /// The database used to process the text's animation tags.
        /// </summary>
        public TMPAnimationDatabase Database => database;
        /// <summary>
        /// The animation tags parsed by the TMPAnimator.
        /// </summary>
        public IEnumerable<TMPAnimationTag> AnimationTags
        {
            get
            {
                for (int i = 0; i < atp.ProcessedTags.Count; i++) yield return atp.ProcessedTags[i];
                for (int i = 0; i < satp.ProcessedTags.Count; i++) yield return satp.ProcessedTags[i];
                for (int i = 0; i < hatp.ProcessedTags.Count; i++) yield return hatp.ProcessedTags[i];
            }
        }

        #region Fields
        [SerializeField] TMPAnimationDatabase database;
        [SerializeField] AnimatorContext context;

        [SerializeField] UpdateFrom updateFrom;
        [SerializeField] bool animateOnStart;

        [SerializeField] bool animationsOverride = false;
        [SerializeField] string defaultShowString;
        [SerializeField] string defaultHideString;

        [SerializeField] string excludedCharacters = "";
        [SerializeField] string excludedCharactersShow = "";
        [SerializeField] string excludedCharactersHide = "";

        [SerializeField] bool excludePunctuation = false;
        [SerializeField] bool excludePunctuationShow = false;
        [SerializeField] bool excludePunctuationHide = false;

        [System.NonSerialized] private AnimationTagProcessor<TMPAnimation> atp = null;
        [System.NonSerialized] private AnimationTagProcessor<TMPShowAnimation> satp = null;
        [System.NonSerialized] private AnimationTagProcessor<TMPHideAnimation> hatp = null;
        [System.NonSerialized] private bool isAnimating = false;
        #endregion 

        #region Initialization
        private void OnEnable()
        {
            // Set up the mediator and processor, and subscribe to relevant events

            UpdateMediator(); // Create / get the mediator; Initialize it if not initialized already
            UpdateProcessors(); // Set up the animation tag processor and add it to the mediator

            mediator.OnVisibilityStateUpdated += EnsureCorrectTiming;

            mediator.Subscribe(this); // Subscribe to the mediator; This makes the mediator persistent at least until this component is destroyed

            mediator.CharDataPopulated += PostProcessTags; // Close still open tags once tag processing is done

            mediator.TextChanged += OnTextChanged; // Subscribe to the relevant events
            mediator.ForcedUpdate += OnForcedUpdate;

            mediator.ForceReprocess(); // Force a reprocess of the text now that processor is added and mediator events are handled

#if UNITY_EDITOR
            if (preview && !Application.isPlaying) StartPreview();
#endif
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            if (animateOnStart) StartAnimating();
        }
        #endregion

        #region CleanUp
        private void OnDisable()
        {
            // Unsubscribe from all events
            mediator.OnVisibilityStateUpdated -= EnsureCorrectTiming;
            mediator.CharDataPopulated -= PostProcessTags;
            mediator.TextChanged -= OnTextChanged;
            mediator.ForcedUpdate -= OnForcedUpdate;

            mediator.Processor.UnregisterProcessor(ParsingUtility.NO_PREFIX); // Remove animation tag processor from mediator
            mediator.Processor.UnregisterProcessor(ParsingUtility.SHOW_ANIMATION_PREFIX);
            mediator.Processor.UnregisterProcessor(ParsingUtility.HIDE_ANIMATION_PREFIX);
            atp.Reset(); // Reset animation tag processor itself

            mediator.ForceReprocess(); // Force a reprocess of the text

#if UNITY_EDITOR
            StopPreview();
            //if (!Application.isPlaying) EditorApplication.delayCall += StopPreview;
#endif
        }

        private void OnDestroy()
        {
            if (mediator != null) mediator.Unsubscribe(this); // Unsubscribe from the mediator; if this was the last subscriber, mediator will be destroyed
        }
        #endregion

        #region Animation Controlling
        /// <summary>
        /// Update the current animations.
        /// </summary>
        /// TODO Enforce calling StartAnimating when UpdateFrom.Script?
        /// TODO Allow calling this when not updating from Script?
        public void UpdateAnimations(float deltaTime)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                string callingFuncName = new System.Diagnostics.StackFrame(1).GetMethod().Name;
                switch (updateFrom)
                {
                    case UpdateFrom.Update: if (callingFuncName != "Update") Debug.LogWarningFormat(this, falseCallerMethodWarning, name, callingFuncName, "Update"); break;
                    case UpdateFrom.LateUpdate: if (callingFuncName != "LateUpdate") Debug.LogWarningFormat(this, falseCallerMethodWarning, name, callingFuncName, "LateUpdate"); break;
                    case UpdateFrom.FixedUpdate: if (callingFuncName != "FixedUpdate") Debug.LogWarningFormat(this, falseCallerMethodWarning, name, callingFuncName, "FixedUpdate"); break;
                }
            }
#endif

            UpdateAnimations_Impl(deltaTime);
        }

        /// <summary>
        /// Start animating.
        /// </summary>
        public void StartAnimating()
        {
            isAnimating = true;
        }

        /// <summary>
        /// Stop animating.
        /// </summary>
        public void StopAnimating()
        {
            isAnimating = false;
        }

        /// <summary>
        /// Reset all visible characters to their initial state.
        /// </summary>
        public void ResetAnimations() => ResetAllVisible();
        #endregion

        #region Setters
        /// <summary>
        /// Set where the animations should be updated from.
        /// </summary>
        /// <param name="updateFrom"></param>
        public void SetUpdateFrom(UpdateFrom updateFrom)
        {
            this.updateFrom = updateFrom;
        }

        /// <summary>
        /// Set the database the animator should use to parse the text's animation tags.
        /// </summary>
        /// <param name="database"></param>
        public void SetDatabase(TMPAnimationDatabase database)
        {
            this.database = database;
            UpdateProcessors();
            mediator.ForceReprocess();
        }
        #endregion

        #region Animations
        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (updateFrom == UpdateFrom.Update && isAnimating) UpdateAnimations_Impl(Time.deltaTime);
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (updateFrom == UpdateFrom.LateUpdate && isAnimating) UpdateAnimations_Impl(Time.deltaTime);
        }

        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (updateFrom == UpdateFrom.FixedUpdate && isAnimating) UpdateAnimations_Impl(Time.fixedDeltaTime);
        }

        private void UpdateAnimations_Impl(float deltaTime)
        {
            context.passedTime += deltaTime;

            for (int i = 0; i < mediator.CharData.Count; i++)
            {
                UpdateCharacterAnimation(deltaTime, i, false);
            }

            if (mediator.Text.mesh != null)
                mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

        private void UpdateCharacterAnimation(float deltaTime, int index, bool updateVertices = true)
        {
            CharData cData = mediator.CharData[index];
            if (!AnimateCharacter(ref cData)) return;

            context.deltaTime = deltaTime;


            UpdateCharacterAnimation_Impl(index);

            // TODO only set actually changed meshes; dirty flag on cdata & cehcking of uv vert color
            var info = mediator.Text.textInfo;
            TMP_CharacterInfo cInfo = info.characterInfo[index];
            int vIndex = cInfo.vertexIndex, mIndex = cInfo.materialReferenceIndex;
            Color32[] colors = info.meshInfo[mIndex].colors32;
            Vector3[] verts = info.meshInfo[mIndex].vertices;
            Vector2[] uvs0 = info.meshInfo[mIndex].uvs0;
            Vector2[] uvs2 = info.meshInfo[mIndex].uvs2;
            //Vector2[] uvs4 = info.meshInfo[mIndex].uvs2;

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = mediator.CharData[index].mesh[j].position;
                colors[vIndex + j] = mediator.CharData[index].mesh[j].color;
                uvs0[vIndex + j] = mediator.CharData[index].mesh[j].uv;
                uvs2[vIndex + j] = mediator.CharData[index].mesh[j].uv2;
                //uvs4[vIndex + j] = mediator.CharData[index].currentMesh[j].uv4;
            }

            if (updateVertices && mediator.Text.mesh != null)
                mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

        private bool AnimateCharacter(ref CharData cData) => cData.info.isVisible && cData.visibilityState != CharData.VisibilityState.Hidden;

        private int UpdateCharacterAnimation_Impl(int index)
        {
            CharData cData = mediator.CharData[index];
            if (!cData.info.isVisible || cData.visibilityState == CharData.VisibilityState.Hidden) return 0;

            int applied = 0;

            Vector3 positionDelta = Vector3.zero;
            Matrix4x4 scaleDelta = Matrix4x4.Scale(Vector3.one);
            Quaternion rotation = Quaternion.identity;
            Vector3 rotationPivot = cData.info.initialPosition;

            Vector3 TL = Vector3.zero;
            Vector3 TR = Vector3.zero;
            Vector3 BR = Vector3.zero;
            Vector3 BL = Vector3.zero;

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

            if (cData.visibilityState == CharData.VisibilityState.ShowAnimation)
            {
                if (!IsExcludedShow(cData.info.character))
                    Animate(showCached[GetActiveIndex(index, showCached)]);
            }
            else if (cData.visibilityState == CharData.VisibilityState.HideAnimation)
            {
                if (!IsExcludedHide(cData.info.character))
                    Animate(hideCached[GetActiveIndex(index, hideCached)]);
            }

            if (cData.visibilityState == CharData.VisibilityState.Hidden || IsExcluded(cData.info.character))
            {
                ApplyVertices();
                return 1;
            }

            if (animationsOverride)
            {
                int animIndex = GetActiveIndex(index, basicCached);
                if (animIndex >= 0) Animate(basicCached[animIndex]);
            }
            else
            {
                // Get all basic animations active here and apply
                CachedAnimation ca;
                for (int i = 0; i < basicCached.Count; i++)
                {
                    ca = basicCached[i];
                    if (ca.tag.startIndex <= index && ca.tag.startIndex + ca.tag.length > index)
                    {
                        Animate(ca);
                    }
                }
            }

            ApplyVertices();
            return applied;

            void Animate(CachedAnimation ca)
            {
                cData.Reset();
                cData.segmentIndex = index - ca.tag.startIndex;
                if (cData.segmentIndex == 0)
                {
                    // cdata is first
                }
                else
                {

                }

                for (int i = 0; i < 4; i++)
                {
                    cData.mesh.SetPosition(i, cData.mesh.initial.GetPosition(i)); // cData.initialMesh.GetPosition(i));
                }

                ca.animation.ResetParameters();
                ca.animation.SetParameters(ca.tag.parameters);
                ca.animation.Animate(ref cData, ref ca.context);

                UpdateVertexOffsets();

                applied++;
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
                    rotationPivot += (cData.RotationPivot - cData.info.initialPosition);
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

                // TODO Does this make sense?
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

                cData.mesh.SetPosition(0, vbl);
                cData.mesh.SetPosition(1, vtl);
                cData.mesh.SetPosition(2, vtr);
                cData.mesh.SetPosition(3, vbr);

                cData.mesh.SetColor(0, BL_Color);
                cData.mesh.SetColor(1, TL_Color);
                cData.mesh.SetColor(2, TR_Color);
                cData.mesh.SetColor(3, BR_Color);

                cData.mesh.SetUV(0, BL_UV);
                cData.mesh.SetUV(1, TL_UV);
                cData.mesh.SetUV(2, TR_UV);
                cData.mesh.SetUV(3, BR_UV);

                cData.mesh.SetUV2(0, BL_UV2);
                cData.mesh.SetUV2(1, TL_UV2);
                cData.mesh.SetUV2(2, TR_UV2);
                cData.mesh.SetUV2(3, BR_UV2);

                mediator.CharData[index] = cData;
            }
        }

        private int GetActiveIndex(int charIndex, IList<CachedAnimation> animations)
        {
            int maxStart = -1; int maxIndex = -1;
            for (int i = 0; i < animations.Count; i++)
            {
                if (animations[i].tag.startIndex >= maxStart && animations[i].tag.startIndex <= charIndex && animations[i].tag.startIndex + animations[i].tag.length > charIndex)
                {
                    maxStart = animations[i].tag.startIndex;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }
        #endregion

        #region Event Callbacks
        private void OnTextChanged()
        {
            if (isAnimating) UpdateAnimations_Impl(0f);
        }

        private void OnForcedUpdate(int start, int length)
        {
            if (!isAnimating) return;

            for (int i = 0; i < length; i++)
            {
                UpdateCharacterAnimation(0f, i + start);
            }
        }
        #endregion

        #region Editor Only
#if UNITY_EDITOR
        [SerializeField, HideInInspector] bool preview = false;
        [SerializeField, HideInInspector] bool initDatabase = false;
        [SerializeField, HideInInspector] bool startedEditorApplication = false;
        [SerializeField, HideInInspector] TMPAnimationDatabase prevDatabase = null;
        const string falseCallerMethodWarning = "The animations of the TMPAnimator on {0} were incorrectly updated " +
            "from method \"{1}\" instead of {0}'s \"{2}\" method; If you want to manually control the animation updates, set {0}'s UpdateFrom property to \"Script\", " +
            "either through the inspector or through a script using the SetUpdateFrom method.";

        public void StartPreview()
        {
            //preview = true;
            StartAnimating();

            EditorApplication.update -= UpdatePreview;
            EditorApplication.update += UpdatePreview;
        }

        public void StopPreview()
        {
            //preview = false;
            EditorApplication.update -= UpdatePreview;
            StopAnimating();
            ResetAnimations();
        }

        [System.NonSerialized, HideInInspector] float lastTimeSinceStartUp = 0;
        private void UpdatePreview()
        {

            UpdateAnimations_Impl((float)EditorApplication.timeSinceStartup - context.passedTime);
            EditorApplication.QueuePlayerLoopUpdate();
        }

        public void ForceReprocess()
        {
            if (mediator != null) mediator.ForceReprocess();
        }

        public void ForcePostProcess()
        {
            if (mediator != null) PostProcessTags();
        }

        public void UpdateProcessorsWrapper()
        {
            if (mediator == null) return;
            UpdateProcessors();
        }

        public string CheckDefaultShowString()
        {
            if (database == null) return "No show database assigned on animation database";

            ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();
            Dictionary<string, string> tagParams = null;
            ITMPAnimation animation;

            if (string.IsNullOrWhiteSpace(defaultShowString)) return "";

            string str = defaultShowString;
            str = (str.Trim()[0] == '<' ? str : "<" + str + ">");
            if (!ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo, ParsingUtility.TagType.Open)) return "Not a wellformed tag";
            if (!database.showAnimationDatabase.Contains(tagInfo.name)) return "Tag not contained within show database";

            if ((animation = database.showAnimationDatabase.GetEffect(tagInfo.name)) == null) return "Tag is valid but is not assigned an animation object within the show database";

            tagParams = ParsingUtility.GetTagParametersDict(str);
            if (!animation.ValidateParameters(tagParams)) return "Parameters are not valid for this tag";

            return "";
        }

        public string CheckDefaultHideString()
        {
            if (database == null) return "No hide database assigned on animation database";

            ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();
            Dictionary<string, string> tagParams = null;
            ITMPAnimation animation;

            if (string.IsNullOrWhiteSpace(defaultHideString)) return "";

            string str = defaultHideString;
            str = (str.Trim()[0] == '<' ? str : "<" + str + ">");
            if (!ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo, ParsingUtility.TagType.Open)) return "Not a wellformed tag";
            if (!database.showAnimationDatabase.Contains(tagInfo.name)) return "Tag not contained within hide database";

            if ((animation = database.showAnimationDatabase.GetEffect(tagInfo.name)) == null) return "Tag is valid but is not assigned an animation object within the hide database";

            tagParams = ParsingUtility.GetTagParametersDict(str);
            if (!animation.ValidateParameters(tagParams)) return "Parameters are not valid for this tag";

            return "";
        }

        private void OnValidate()
        {
            if (mediator == null) return;

            if (prevDatabase != database || (database != null && (atp.Database != database.basicAnimationDatabase || satp.Database != database.showAnimationDatabase || hatp.Database != database.hideAnimationDatabase)))
            {
                prevDatabase = database;
                UpdateProcessors();
            }
        }
#endif
        #endregion

        #region Various Public Methods
        public bool IsExcluded(char c) => (excludePunctuation && char.IsPunctuation(c)) || excludedCharacters.Contains(c);
        public bool IsExcludedShow(char c) => (excludePunctuationShow && char.IsPunctuation(c)) || excludedCharactersShow.Contains(c);
        public bool IsExcludedHide(char c) => (excludePunctuationHide && char.IsPunctuation(c)) || excludedCharactersHide.Contains(c);
        #endregion

        private void UpdateProcessors()
        {
            mediator.Processor.UnregisterProcessor(ParsingUtility.NO_PREFIX);
            mediator.Processor.UnregisterProcessor(ParsingUtility.SHOW_ANIMATION_PREFIX);
            mediator.Processor.UnregisterProcessor(ParsingUtility.HIDE_ANIMATION_PREFIX);
            atp = new(database == null ? null : database.basicAnimationDatabase);
            satp = new(database == null ? null : database.showAnimationDatabase);
            hatp = new(database == null ? null : database.hideAnimationDatabase);
            mediator.Processor.RegisterProcessor(ParsingUtility.NO_PREFIX, atp);
            mediator.Processor.RegisterProcessor(ParsingUtility.SHOW_ANIMATION_PREFIX, satp);
            mediator.Processor.RegisterProcessor(ParsingUtility.HIDE_ANIMATION_PREFIX, hatp);
        }

        private struct CachedAnimation
        {
            public TMPAnimationTag tag;
            public ITMPAnimation animation;
            public IAnimationContext context;

            public CachedAnimation(TMPAnimator animator, TMPAnimationTag tag, ITMPAnimation animation, AnimatorContext animatorContext, TMPMediator mediator)
            {
                this.tag = tag;
                this.animation = animation;
                this.context = animation.GetNewContext();
                if (context != null) context.animatorContext = animatorContext;

                context.segmentData = new SegmentData(animator, tag, mediator.CharData);
            }
        }

        private List<CachedAnimation> basicCached;
        private List<CachedAnimation> showCached;
        private List<CachedAnimation> hideCached;

        private void PostProcessTags(/*string text*/)
        {
            // Close any unclosed animation tags
            int endIndex = mediator.CharData.Count - 1;
            foreach (var tag in AnimationTags)
            {
                if (tag.IsOpen) tag.Close(endIndex);
            }

            // Cache the corresponding animation for each tag
            basicCached = new List<CachedAnimation>();
            showCached = new List<CachedAnimation>();
            hideCached = new List<CachedAnimation>();

            foreach (var tag in atp.ProcessedTags) basicCached.Add(new CachedAnimation(this, tag, database.basicAnimationDatabase.GetEffect(tag.name), context, mediator));
            foreach (var tag in satp.ProcessedTags) showCached.Add(new CachedAnimation(this, tag, database.showAnimationDatabase.GetEffect(tag.name), context, mediator));
            foreach (var tag in hatp.ProcessedTags) hideCached.Add(new CachedAnimation(this, tag, database.hideAnimationDatabase.GetEffect(tag.name), context, mediator));

            // Add default show / hide animation
            if (database == null || !AddDefault(defaultShowString, database.showAnimationDatabase, showCached))
            {
                TMPAnimationTag tag = new TMPAnimationTag("Dummy Show Animation", 0, null);
                tag.Close(mediator.CharData.Count - 1);
                var cached = new CachedAnimation(this, tag, ScriptableObject.CreateInstance<DummyShowAnimation>(), context, mediator);

                //var cached = new CachedAnimation(new TMPAnimationTag("Dummy Show Animation", 0, null), ScriptableObject.CreateInstance<DummyShowAnimation>(), context, mediator);
                //cached.tag.Close(text.Length - 1);
                showCached.Insert(0, cached);
            }

            if (database == null || !AddDefault(defaultHideString, database.hideAnimationDatabase, hideCached))
            {
                TMPAnimationTag tag = new TMPAnimationTag("Dummy Hide Animation", 0, null);
                tag.Close(mediator.CharData.Count - 1);
                var cached = new CachedAnimation(this, tag, ScriptableObject.CreateInstance<DummyHideAnimation>(), context, mediator);

                hideCached.Insert(0, cached);
            }
        }

        private bool AddDefault<T>(string str, TMPAnimationDatabaseBase<T> database, List<CachedAnimation> anims) where T : ITMPAnimation
        {
            if (database == null) return false;

            ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();
            Dictionary<string, string> tagParams = null;
            ITMPAnimation animation;

            if (string.IsNullOrWhiteSpace(str)) return false;

            str = (str.Trim()[0] == '<' ? str : "<" + str + ">");
            if (!ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo, ParsingUtility.TagType.Open) || !database.Contains(tagInfo.name)) return false;

            if ((animation = database.GetEffect(tagInfo.name)) == null)
            {
                return false;
            }

            tagParams = ParsingUtility.GetTagParametersDict(str);
            if (!animation.ValidateParameters(tagParams)) return false;

            var cached = new CachedAnimation(this, new TMPAnimationTag(tagInfo.name, 0, tagParams), animation, context, mediator);
            cached.tag.Close(mediator.CharData.Count - 1);
            anims.Insert(0, cached);

            return true;
        }

        #region Dummy Animations
        private class DummyShowAnimation : TMPShowAnimation
        {
            public override void Animate(ref CharData cData, ref IAnimationContext context)
            {
                for (int i = 0; i < 4; i++)
                {
                    cData.mesh.SetPosition(i, cData.mesh.initial.GetPosition(i));
                }

                cData.SetVisibilityState(CharData.VisibilityState.Shown, context.animatorContext.passedTime);
            }

            public override void ResetParameters()
            {
            }

            public override void SetParameters(Dictionary<string, string> parameters)
            {
            }

            public override bool ValidateParameters(Dictionary<string, string> parameters)
            {
                return true;
            }
        }
        private class DummyHideAnimation : TMPShowAnimation
        {
            public override void Animate(ref CharData cData, ref IAnimationContext context)
            {
                for (int i = 0; i < 4; i++)
                {
                    EffectUtility.SetVertexRaw(i, cData.info.initialPosition, ref cData, ref context);
                }

                cData.SetVisibilityState(CharData.VisibilityState.Hidden, context.animatorContext.passedTime);
            }

            public override void ResetParameters()
            {
            }

            public override void SetParameters(Dictionary<string, string> parameters)
            {
            }

            public override bool ValidateParameters(Dictionary<string, string> parameters)
            {
                return true;
            }
        }
        #endregion

        private void ResetAllVisible()
        {
            var info = mediator.Text.textInfo;

            Vector3[] verts;
            Color32[] colors;
            int vIndex, mIndex;
            TMP_CharacterInfo cInfo;

            // Iterate over all characters and apply the new meshes
            for (int i = 0; i < info.characterCount; i++)
            {
                cInfo = info.characterInfo[i];
                if (!cInfo.isVisible || mediator.CharData[i].visibilityState == CharData.VisibilityState.Hidden/*hidden*/) continue;

                vIndex = cInfo.vertexIndex;
                mIndex = cInfo.materialReferenceIndex;

                colors = info.meshInfo[mIndex].colors32;
                verts = info.meshInfo[mIndex].vertices;

                CharData cData = mediator.CharData[i];

                for (int j = 0; j < 4; j++)
                {
                    cData.mesh.SetPosition(j, cData.mesh.initial.GetPosition(j));
                    cData.mesh.SetColor(j, cData.mesh.initial.GetColor(j));

                    verts[vIndex + j] = mediator.CharData[i].mesh.initial[j].position;
                    colors[vIndex + j] = mediator.CharData[i].mesh.initial[j].color;
                }

                mediator.CharData[i] = cData;
            }

            if (mediator.Text.mesh != null)
                mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

        private void EnsureCorrectTiming(int index, CharData.VisibilityState prev)
        {
            if (context == null) return;
            float passed = context.passedTime;
            CharData cData = mediator.CharData[index];
            CharData.VisibilityState current = cData.visibilityState;
            //Debug.Log("1");
            cData.SetVisibilityState(prev, -1);
            //Debug.Log("2");
            cData.SetVisibilityState(current, passed);
            mediator.CharData[index] = cData;
            //Debug.Log("Ensuring correct timing; setting from " + current + " to " + prev + " and back again; time: " + mediator.CharData[index].visibleTime + " and passed: " + context.passedTime + " cached passed: " + passed);
        }
    }
}
