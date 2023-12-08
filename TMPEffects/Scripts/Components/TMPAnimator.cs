using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// One of the two main components of TMPEffects, along with TMPWriter.
/// TMPAnimator allows you to animate characters.
/// </summary>
[ExecuteAlways]
public class TMPAnimator : TMPEffectComponent
{
    public bool IsAnimating => isAnimating;
    public TMPAnimationDatabase Database => database;
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
    [SerializeField] AnimationContext context;

    [SerializeField] UpdateFrom updateFrom;
    [SerializeField] bool animateOnStart;

    [SerializeField] string defaultShowString;
    [SerializeField] string defaultHideString;

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

        mediator.Subscribe(this); // Subscribe to the mediator; This makes the mediator persistent at least until this component is destroyed

        mediator.Processor.FinishProcessTags += PostProcessTags; // Close still open tags once tag processing is done

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
        mediator.Processor.FinishProcessTags -= PostProcessTags;
        mediator.TextChanged -= OnTextChanged;
        mediator.ForcedUpdate -= OnForcedUpdate;

        mediator.Processor.UnregisterProcessor(ParsingUtility.NO_PREFIX); // Remove animation tag processor from mediator
        mediator.Processor.UnregisterProcessor(ParsingUtility.SHOW_ANIMATION_PREFIX);
        mediator.Processor.UnregisterProcessor(ParsingUtility.HIDE_ANIMATION_PREFIX);
        atp.Reset(); // Reset animation tag processor itself

        mediator.ForceReprocess(); // Force a reprocess of the text

#if UNITY_EDITOR
        if (!Application.isPlaying) EditorApplication.delayCall += StopPreview;
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
    /// Set the database the animator should use.
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
        System.Diagnostics.Stopwatch sw = new();
        sw.Start();

        context.passedTime += deltaTime;

        for (int i = 0; i < mediator.CharData.Count; i++)
        {
            UpdateCharacterAnimation(deltaTime, i, false);
        }

        if (mediator.Text.mesh != null)
            mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        sw.Stop();
        total += sw.Elapsed.TotalMilliseconds;
        count++;
        //Debug.Log("Time: " + total + " Count: " + count);//ElapsedMilliseconds); 
    }

    [System.NonSerialized] private double total;
    [System.NonSerialized] private int count;

    void UpdateCharacterAnimation(float deltaTime, int index, bool updateVertices = true)
    {
        CharData cData = mediator.CharData[index];
        if (!cData.isVisible || cData.visibilityState == CharData.VisibilityState.Hidden/*hidden*/) return;

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
            verts[vIndex + j] = mediator.CharData[index].currentMesh[j].position;
            colors[vIndex + j] = mediator.CharData[index].currentMesh[j].color;
            uvs0[vIndex + j] = mediator.CharData[index].currentMesh[j].uv;
            uvs2[vIndex + j] = mediator.CharData[index].currentMesh[j].uv2;
            //uvs4[vIndex + j] = mediator.CharData[index].currentMesh[j].uv4;
        }

        if (updateVertices && mediator.Text.mesh != null)
            mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    [SerializeField] bool animationsOverride = false;
    private int UpdateCharacterAnimation_Impl(int index)
    {
        CharData cData = mediator.CharData[index];
        //Debug.Log("Updating wth " + cData.visibilityState.ToString());
        if (!cData.isVisible || cData.visibilityState == CharData.VisibilityState.Hidden) return 0;

        int applied = 0;

        Vector3 TL = Vector3.zero;
        Vector3 TR = Vector3.zero;
        Vector3 BR = Vector3.zero;
        Vector3 BL = Vector3.zero;

        if (cData.visibilityState == CharData.VisibilityState.ShowAnimation) Animate(showCached[GetActiveIndex(index, showCached)]);
        else if (cData.visibilityState == CharData.VisibilityState.HideAnimation) Animate(hideCached[GetActiveIndex(index, hideCached)]);

        if (animationsOverride) Animate(basicCached[GetActiveIndex(index, basicCached)]);
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

        cData.currentMesh.SetPosition(0, cData.initialMesh.vertex_BL.position + BL);
        cData.currentMesh.SetPosition(1, cData.initialMesh.vertex_TL.position + TL);
        cData.currentMesh.SetPosition(2, cData.initialMesh.vertex_TR.position + TR);
        cData.currentMesh.SetPosition(3, cData.initialMesh.vertex_BR.position + BR);
        mediator.CharData[index] = cData;

        return applied;

        void Animate(CachedAnimation ca)
        {
            // TODO Reset CData mesh before every animation?
            //cData.currentMesh.SetPosition(0, cData.initialMesh.GetPosition(0));
            //cData.currentMesh.SetPosition(1, cData.initialMesh.GetPosition(1));
            //cData.currentMesh.SetPosition(2, cData.initialMesh.GetPosition(2));
            //cData.currentMesh.SetPosition(3, cData.initialMesh.GetPosition(3));

            cData.segmentIndex = index - ca.tag.startIndex;
            cData.segmentLength = ca.tag.length;
            ca.animation.ResetVariables();
            ca.animation.SetParameters(ca.tag.parameters);
            ca.animation.Animate(ref cData, context);

            UpdateVertexOffsets();
            applied++;
        }

        void UpdateVertexOffsets()
        {
            TL += (cData.currentMesh.vertex_TL.position - cData.initialMesh.vertex_TL.position);
            TR += (cData.currentMesh.vertex_TR.position - cData.initialMesh.vertex_TR.position);
            BR += (cData.currentMesh.vertex_BR.position - cData.initialMesh.vertex_BR.position);
            BL += (cData.currentMesh.vertex_BL.position - cData.initialMesh.vertex_BL.position);
        }
    }

    private int GetActiveIndex(int charIndex, IList<CachedAnimation> animations)
    {
        // TODO Better way to do this; Does intervaltree work here; Issue: two tags can have startIndex 0 but its still defined which one takes precedence (the one w/ higher index in the list)
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
        if (isAnimating) UpdateAnimations_Impl(0f); // TODO what delta value to use here?
    }

    private void OnForcedUpdate(int start, int length)
    {
        if (!isAnimating) return;

        for (int i = 0; i < length; i++)
        {
            UpdateCharacterAnimation(0f, i + start); // TODO what delta value to use here?
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
        preview = true;
        StartAnimating();

        EditorApplication.update -= UpdatePreview;
        EditorApplication.update += UpdatePreview;
    }

    public void StopPreview()
    {
        preview = false;
        EditorApplication.update -= UpdatePreview;
        StopAnimating();
        ResetAnimations();
    }

    private void UpdatePreview()
    {
        UpdateAnimations_Impl((float)EditorApplication.timeSinceStartup - context.passedTime);
    }

    public void ForceReprocess()
    {
        if (mediator != null) mediator.ForceReprocess();
    }

    public void ForcePostProcess()
    {
        if (mediator != null) PostProcessTags(mediator.Text.GetParsedText());
    }

    public void UpdateProcessorsWrapper()
    {
        if (mediator == null) return;
        UpdateProcessors();
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
        public TMPAnimation animation;

        public CachedAnimation(TMPAnimationTag tag, TMPAnimation animation)
        {
            this.tag = tag;
            this.animation = animation;
        }
    }

    private List<CachedAnimation> basicCached;
    private List<CachedAnimation> showCached;
    private List<CachedAnimation> hideCached;

    private void PostProcessTags(string text)
    {
        Debug.Log("YE " + defaultShowString);

        // Close any unclosed animation tags
        int endIndex = text.Length - 1;
        foreach (var tag in AnimationTags)
        {
            if (tag.IsOpen) tag.Close(endIndex);
        }

        // Cache the corresponding animation for each tag
        basicCached = new List<CachedAnimation>();
        showCached = new List<CachedAnimation>();
        hideCached = new List<CachedAnimation>();

        foreach (var tag in atp.ProcessedTags) basicCached.Add(new CachedAnimation(tag, database.basicAnimationDatabase.GetEffect(tag.name)));
        foreach (var tag in satp.ProcessedTags) showCached.Add(new CachedAnimation(tag, database.showAnimationDatabase.GetEffect(tag.name)));
        foreach (var tag in hatp.ProcessedTags) hideCached.Add(new CachedAnimation(tag, database.hideAnimationDatabase.GetEffect(tag.name)));

        // Add default show / hide animation
        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();
        Dictionary<string, string> tagParams = null;

        if (!AddDefault(defaultShowString, database.showAnimationDatabase, showCached))
        {
            Debug.Log("DUMMY SHOW");
            var cached = new CachedAnimation(new TMPAnimationTag("Dummy Show Animation", 0, null), ScriptableObject.CreateInstance<DummyShowAnimation>());
            cached.tag.Close(text.Length - 1);
            showCached.Insert(0, cached);
        }

        if (!AddDefault(defaultHideString, database.hideAnimationDatabase, hideCached))
        {
            Debug.Log("DUMMY HIDE");
            var cached = new CachedAnimation(new TMPAnimationTag("Dummy Hide Animation", 0, null), ScriptableObject.CreateInstance<DummyHideAnimation>());
            cached.tag.Close(text.Length - 1);
            hideCached.Insert(0, cached);
        }

        bool AddDefault<T>(string str, TMPAnimationDatabaseBase<T> database, List<CachedAnimation> anims) where T : TMPAnimation
        {
            TMPAnimation animation;

            if (string.IsNullOrWhiteSpace(str)) return false;

            str = (str.Trim()[0] == '<' ? str : "<" + str + ">");

            if (!ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo, ParsingUtility.TagType.Open)) return false;

            tagParams = ParsingUtility.GetTagParametersDict(str);
            if (!(database.Contains(tagInfo.name) && (animation = database.GetEffect(tagInfo.name)).ValidateParameters(tagParams))) return false;

            var cached = new CachedAnimation(new TMPAnimationTag(tagInfo.name, 0, tagParams), animation);
            cached.tag.Close(text.Length - 1);
            anims.Insert(0, cached);

            return true;
        }
    }

    #region Dummy Animations
    private class DummyShowAnimation : TMPShowAnimation
    {
        public override void Animate(ref CharData cData, AnimationContext context)
        {
            for (int i = 0; i < 4; i++)
            {
                cData.currentMesh.SetPosition(i, cData.initialMesh.GetPosition(i));
            }

            cData.visibilityState = CharData.VisibilityState.Shown;

            // Accidental kinda cool effect
            //float t = (Time.time - charData.stateTime) / 1000;
            //for (int i = 0; i < 4; i++)
            //{
            //    Vector3 pos = Vector3.Lerp(charData.currentMesh.GetPosition(i), charData.initialMesh.GetPosition(i), t);
            //    charData.currentMesh.SetPosition(i, pos);
            //}

            //if (t >= 1)
            //    charData.visibilityState = CharData.VisibilityState.Shown;
        }

        public override void ResetVariables()
        {
        }

        public override void SetParameter<T>(string name, T value)
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
        public override void Animate(ref CharData charData, AnimationContext context)
        {
            for (int i = 0; i < 4; i++)
            {
                charData.currentMesh.SetPosition(i, Vector3.zero);
            }

            charData.visibilityState = CharData.VisibilityState.Hidden;
        }

        public override void ResetVariables()
        {
        }

        public override void SetParameter<T>(string name, T value)
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
                cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
                cData.currentMesh.SetColor(j, cData.initialMesh.GetColor(j));

                verts[vIndex + j] = mediator.CharData[i].initialMesh[j].position;
                colors[vIndex + j] = mediator.CharData[i].initialMesh[j].color;
            }

            mediator.CharData[i] = cData;
        }

        if (mediator.Text.mesh != null)
            mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
}