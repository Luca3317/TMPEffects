using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;

/// <summary>
/// One of the two main components of TMPEffects, along with TMPWriter.
/// TMPAnimator allows you to animate characters.
/// </summary>
[ExecuteAlways]
public class TMPAnimator : TMPEffectComponent
{
    public bool IsAnimating => isAnimating;
    public TMPAnimationDatabase Database => database;
    public List<TMPAnimationTag> Animations => atp.ProcessedTags;

    #region Fields
    [SerializeField] TMPAnimationDatabase database;
    [SerializeField] AnimationContext context;

    [SerializeField] UpdateFrom updateFrom;
    [SerializeField] bool animateOnStart;

    [SerializeField] TMPShowAnimation defaultShowAnimation;
    [SerializeField] TMPHideAnimation defaultHideAnimation;

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

        mediator.Processor.FinishProcessTags += CloseOpenTags; // Close still open tags once tag processing is done

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
        mediator.Processor.FinishProcessTags -= CloseOpenTags;
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
    public void UpdateAnimations()
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

        UpdateAnimations_Impl();
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
        if (updateFrom == UpdateFrom.Update && isAnimating) UpdateAnimations_Impl();
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if (updateFrom == UpdateFrom.LateUpdate && isAnimating) UpdateAnimations_Impl();
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if (updateFrom == UpdateFrom.FixedUpdate && isAnimating) UpdateAnimations_Impl();
    }

    private void UpdateAnimations_Impl()
    {
        System.Diagnostics.Stopwatch sw = new();
        sw.Start();

        for (int i = 0; i < mediator.CharData.Count; i++)
        {
            UpdateCharacterAnimation(i, false);
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

    public void UpdateCharacterAnimation(int index, bool updateVertices = true)
    {
        CharData cData = mediator.CharData[index];
        if (!cData.isVisible || cData.visibilityState == CharData.VisibilityState.Hidden/*hidden*/) return;

        switch (cData.visibilityState)
        {
            case CharData.VisibilityState.ShowAnimation:
                if (UpdateCharacterAnimation_Impl(index, showCached /*satp.ProcessedTags*/) == 0)
                {
                    Debug.LogWarning("There are 0 show animations applied to the current character, which is in ShowAnimation state. This should be impossible");
                    //GetComponent<TMPWriter>().Show(index, 1, true);
                    //mediator.ForceUpdate(index, 1);
                }

                if (mediator.CharData[index].visibilityState == CharData.VisibilityState.Shown)
                {
                    Debug.Log("Set to shown " + cData.character);
                    UpdateCharacterAnimation(index, updateVertices);
                }
                // TODO Some indication whether to update normal animation as well
                //UpdateCharacterAnimation_Impl(index, basicCached /*atp.ProcessedTags*/);
                break;
            case CharData.VisibilityState.HideAnimation:
                UpdateCharacterAnimation_Impl(index, hideCached /*hatp.ProcessedTags*/);
                break;
            case CharData.VisibilityState.Shown:
                UpdateCharacterAnimation_Impl(index, basicCached /*atp.ProcessedTags*/); break;
        }

        // TODO only set actually changed meshes
        var info = mediator.Text.textInfo;
        TMP_CharacterInfo cInfo = info.characterInfo[index];
        int vIndex = cInfo.vertexIndex, mIndex = cInfo.materialReferenceIndex;
        Color32[] colors = info.meshInfo[mIndex].colors32;
        Vector3[] verts = info.meshInfo[mIndex].vertices;

        for (int j = 0; j < 4; j++)
        {
            verts[vIndex + j] = mediator.CharData[index].currentMesh[j].position;
            colors[vIndex + j] = mediator.CharData[index].currentMesh[j].color;
        }

        if (updateVertices && mediator.Text.mesh != null)
            mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    private int UpdateCharacterAnimation_Impl(int index, List<CachedAnimation> animations) //List<TMPAnimationTag> tags)
    {
        int applied = 0;
        for (int i = 0; i < animations.Count; i++)
        {
            CachedAnimation cachedAnimation = animations[i];
            if (cachedAnimation.tag.startIndex > index || cachedAnimation.tag.startIndex + cachedAnimation.tag.length <= index)
            {
                continue;
            }

            applied++;

            cachedAnimation.animation.ResetVariables();
            cachedAnimation.animation.SetParameters(cachedAnimation.tag.parameters);

            CharData cData = mediator.CharData[index];
            cData.segmentIndex = index - cachedAnimation.tag.startIndex;
            cData.segmentLength = cachedAnimation.tag.length;

            cachedAnimation.animation.Animate(ref cData, context);
            mediator.CharData[index] = cData;
        }

        return applied;
    }
    #endregion

    #region Event Callbacks
    private void OnTextChanged()
    {
        if (isAnimating) UpdateAnimations_Impl();
    }

    private void OnForcedUpdate(int start, int length)
    {
        if (!isAnimating) return;

        for (int i = 0; i < length; i++)
        {
            UpdateCharacterAnimation(i + start);
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
        StartAnimating();

        EditorApplication.update -= UpdatePreview;
        EditorApplication.update += UpdatePreview;
    }

    public void StopPreview()
    {
        EditorApplication.update -= UpdatePreview;
        StopAnimating();
        ResetAnimations();
    }

    private void UpdatePreview()
    {
        UpdateAnimations_Impl();
    }

    public void ForceReprocess()
    {
        if (mediator != null) mediator.ForceReprocess();
    }

    public void UpdateProcessorsWrapper()
    {
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

    private void CloseOpenTags(string text)
    {
        int endIndex = text.Length - 1;
        foreach (var tag in atp.ProcessedTags)
        {
            if (tag.IsOpen)
                tag.Close(endIndex);
        }

        basicCached = new List<CachedAnimation>();
        showCached = new List<CachedAnimation>();
        hideCached = new List<CachedAnimation>();

        foreach (var tag in atp.ProcessedTags)
        {
            basicCached.Add(new CachedAnimation(tag, database.basicAnimationDatabase.GetEffect(tag.name)));
        }

        foreach (var tag in satp.ProcessedTags)
        {
            showCached.Add(new CachedAnimation(tag, database.showAnimationDatabase.GetEffect(tag.name)));
        }

        foreach (var tag in hatp.ProcessedTags)
        {
            hideCached.Add(new CachedAnimation(tag, database.hideAnimationDatabase.GetEffect(tag.name)));
        }

        bool handled = false;
        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();
        if (ParsingUtility.TryParseTag(defaultShowString, 0, defaultShowString.Length - 1, ref tagInfo, ParsingUtility.TagType.Open))
        {
            Debug.Log("Success parse show with tag");
            var showParams = ParsingUtility.GetTagParametersDict(defaultShowString);
            bool success = database.showAnimationDatabase.Contains(tagInfo.name) && database.showAnimationDatabase.GetEffect(tagInfo.name).ValidateParameters(showParams);

            if (success)
            {
                Debug.Log("FULLY PARSED SHOW");
                // Add to cached
                var cached = new CachedAnimation(new TMPAnimationTag(tagInfo.name, 0, showParams), database.showAnimationDatabase.GetEffect(tagInfo.name));
                cached.tag.Close(text.Length - 1);
                showCached.Add(cached);
                handled = true;
            }
        }
        if (!handled)
        {
            // Use a built in dummy one
            var cached = new CachedAnimation(new TMPAnimationTag("dummy show", 0, null), ScriptableObject.CreateInstance<DummyShowAnimation>());
            cached.tag.Close(text.Length - 1);
            showCached.Add(cached);
            Debug.Log("DUMMY SHOW");
        }


        handled = false;
        if (ParsingUtility.TryParseTag(defaultHideString, 0, defaultHideString.Length - 1, ref tagInfo, ParsingUtility.TagType.Open))
        {
            var hideParams = ParsingUtility.GetTagParametersDict(defaultHideString);
            bool success = database.hideAnimationDatabase.Contains(tagInfo.name) && database.hideAnimationDatabase.GetEffect(tagInfo.name).ValidateParameters(hideParams);
            Debug.Log("Success parse HIDE");
            if (success)
            {
                // Add to cached
                var cached = new CachedAnimation(new TMPAnimationTag(tagInfo.name, 0, hideParams), database.hideAnimationDatabase.GetEffect(tagInfo.name));
                cached.tag.Close(text.Length - 1);
                hideCached.Add(cached);
                Debug.Log("FULLY PARSED HIDE");
                handled = true;
            }
        }
        if (!handled)
        {
            // Use a built in dummy one
            var cached = new CachedAnimation(new TMPAnimationTag("dummy hide", 0, null), ScriptableObject.CreateInstance<DummyHideAnimation>());
            cached.tag.Close(text.Length - 1);
            hideCached.Add(cached);
            Debug.Log("DUMMY HIDE");
        }
    }

    private class DummyShowAnimation : TMPShowAnimation
    {
        public override void Animate(ref CharData charData, AnimationContext context)
        {
            float t = (Time.time - charData.stateTime) * 4;
            Vector3 center = Vector3.zero;
            for (int i = 0; i < 4; i++)
            {
                center += charData.initialMesh.GetPosition(i);
            }
            center /= 4;

            for (int i = 0; i < 4; i++)
            {
                Vector3 pos = Vector3.Lerp(center, charData.initialMesh.GetPosition(i), t);
                charData.currentMesh.SetPosition(i, pos);
            }

            if (t >= 1)
            {
                charData.visibilityState = CharData.VisibilityState.Shown;
            }

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

            charData.visibilityState = CharData.VisibilityState.Shown;
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