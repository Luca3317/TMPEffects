using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TMPEffectsDatabase Database => database;
    public List<TMPAnimationTag> Animations => atp.ProcessedTags;

    #region Fields
    [SerializeField] TMPEffectsDatabase database;
    [SerializeField] AnimationContext context;

    [SerializeField] UpdateFrom updateFrom;
    [SerializeField] bool animateOnStart;

    [System.NonSerialized] private AnimationTagProcessor atp = null;
    [System.NonSerialized] private bool isAnimating = false;
    #endregion

    #region Initialization
    private void OnEnable()
    {
        // Set up the mediator and processor, and subscribe to relevant events

        UpdateMediator(); // Create / get the mediator; Initialize it if not initialized already
        UpdateProcessor(); // Set up the animation tag processor and add it to the mediator

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
    public void SetDatabase(TMPEffectsDatabase database)
    {
        this.database = database;
        UpdateProcessor();
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
        for (int i = 0; i < atp.ProcessedTags.Count; i++)
        {
            TMPEffectTag tag = atp.ProcessedTags[i];
            ITMPAnimation effect = database.GetEffect(tag.name);
            if (effect == null) Debug.LogError("Tags contained tag that did not have a registered effect; sdhould not be possible");

            effect.ResetVariables();
            effect.SetParameters(tag.parameters);

            for (int j = 0; j < tag.length; j++)
            {
                CharData cData = mediator.CharData[tag.startIndex + j];
                if (!cData.isVisible || cData.hidden) continue;

                // Set segment-dependent data here?
                cData.segmentIndex = j;
                cData.segmentLength = tag.length;

                effect.Animate(ref cData, context);
                mediator.CharData[tag.startIndex + j] = cData;
            }
        }

        var info = mediator.Text.textInfo;
        Vector3[] verts;
        Color32[] colors;
        int vIndex, mIndex;
        TMP_CharacterInfo cInfo;

        // TODO only update affected regions

        // Iterate over all characters and apply the new meshes
        for (int i = 0; i < info.characterCount; i++)
        {
            cInfo = info.characterInfo[i];

            if (!cInfo.isVisible) continue;

            vIndex = cInfo.vertexIndex;
            mIndex = cInfo.materialReferenceIndex;

            colors = info.meshInfo[mIndex].colors32;
            verts = info.meshInfo[mIndex].vertices;

            //if (mediator.CharData.Count == 0)
            //{
            //    Debug.LogWarning("Uninitialized chardata");
            //}

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = mediator.CharData[i].currentMesh[j].position;
                colors[vIndex + j] = mediator.CharData[i].currentMesh[j].color;
            }
        }

        if (mediator.Text.mesh != null)
            mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    private void UpdateCharacterAnimation(int index)
    {
        CharData cData = mediator.CharData[index];

        if (!cData.isVisible || cData.hidden) return;

        for (int i = 0; i < atp.ProcessedTags.Count; i++)
        {
            TMPEffectTag tag = atp.ProcessedTags[i];

            if (tag.startIndex > index || tag.startIndex + tag.length <= index)
            {
                continue;
            }

            ITMPAnimation effect = database.GetEffect(tag.name);
            if (effect == null) Debug.LogError("Tags contained tag that did not have a registered effect; sdhould not be possible");

            effect.ResetVariables();
            effect.SetParameters(tag.parameters);

            cData.segmentIndex = index - tag.startIndex;
            cData.segmentLength = tag.length;

            effect.Animate(ref cData, context);
            mediator.CharData[index] = cData;
        }
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

        mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

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
    [SerializeField, HideInInspector] TMPEffectsDatabase prevDatabase = null;
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

    private void OnValidate()
    {
        if (mediator == null) return;

        if (prevDatabase != database)
        {
            prevDatabase = database;
            UpdateProcessor();
        }
    }
#endif
    #endregion

    private void UpdateProcessor()
    {
        mediator.Processor.UnregisterProcessor(ParsingUtility.NO_PREFIX);
        atp = new AnimationTagProcessor(database);
        mediator.Processor.RegisterProcessor(ParsingUtility.NO_PREFIX, atp);
    }

    private void CloseOpenTags(string text)
    {
        int endIndex = text.Length - 1;
        foreach (var tag in atp.ProcessedTags)
        {
            if (tag.IsOpen)
                tag.Close(endIndex);
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
            if (!cInfo.isVisible || mediator.CharData[i].hidden) continue;

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