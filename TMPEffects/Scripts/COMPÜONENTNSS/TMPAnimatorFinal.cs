using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class TMPAnimatorFinal : MonoBehaviour
{
    [SerializeField] TMPEffectsDatabase database;
    [SerializeField] AnimationContext context;

    [SerializeField] UpdateFrom updateFrom;
    [SerializeField] bool animateOnStart;

    private TMPMediatorFinal data = null;
    private AnimationTagProcessor atp = null;

    public bool IsAnimating => isAnimating;
    private bool isAnimating;

    private void OnEnable()
    {
        Debug.LogWarning("OnEnable");
        UpdateData();
        UpdateDatabase();

        data.Subscribe(this);

        data.Processor.FinishProcessTags += CloseOpenTags;

        data.TextChanged += OnTextChanged;
        data.ForcedUpdate += OnForcedUpdate;

        data.ForceReprocess();

#if UNITY_EDITOR
        if (preview) StartPreview();
#endif
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if (animateOnStart) StartAnimating();
    }

    private void OnDisable()
    {
        Debug.LogWarning("OnDisable");
        data.Processor.FinishProcessTags -= CloseOpenTags;
        data.Processor.UnregisterProcessor(ParsingUtility.NO_PREFIX);

        data.TextChanged -= OnTextChanged;
        data.ForcedUpdate -= OnForcedUpdate;

        // TODO Reassigned in OnEnable anyway;
        // Either change class to reuse instances or dont reset (atm, resetting is necessary for some editor functionality though)
        atp.Reset();

        data.ForceReprocess();

#if UNITY_EDITOR
        StopPreview();
#endif
    }

    private void OnDestroy()
    {
        Debug.LogWarning("OnDestrory");
        if (data != null) data.Unsubscribe(this);
    }

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

    public void StartAnimating()
    {
        isAnimating = true;
    }

    public void StopAnimating()
    {
        isAnimating = false;
    }

    public void ResetAnimations()
    {
        ResetAllVisible();
    }

    public void SetUpdateFrom(UpdateFrom updateFrom)
    {
        this.updateFrom = updateFrom;
    }

    public void UpdateAnimations()
    {
#if UNITY_EDITOR
        // TODO Need to handle textchanged case
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

        if (!isAnimating)
        {
            Debug.LogWarning("Before updating the animations, you must call StartAnimating on the TMPAnimator component (and, for best practice, call StopAnimating once you will no longer update the animations).");
            return;
        }

        UpdateAnimations_Impl();
    }

    void UpdateAnimations_Impl()
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
                //Debug.Log("Pointsize of " + (tag.startIndex + j) + " ( " + data.CharData[tag.startIndex + j].character + " ) is " + data.CharData[tag.startIndex + j].pointSize);
                //if (tag.startIndex + j > data.activeEndIndex) break;

                if (tag.startIndex + j < 0 || tag.startIndex + j >= data.CharData.Count)
                    Debug.Log("im doing w/ " + tag.startIndex + " and " + j + " (tag len: " + tag.length + "; char len: " + data.CharData.Count);

                CharData cData = data.CharData[tag.startIndex + j];
                if (!cData.isVisible || cData.hidden) continue;

                // Set segment-dependent data here?
                cData.segmentIndex = j;
                cData.segmentLength = tag.length;

                effect.Animate(ref cData, context);
                data.CharData[tag.startIndex + j] = cData;
            }
        }

        var info = data.Text.textInfo;
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

            // TODO Ctrl+z for tmp text during runtime cause oob exception here
            if (data.CharData.Count == 0)
            {
                Debug.LogWarning("Uninitialized chardata");
            }

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = data.CharData[i].currentMesh[j].position;
                colors[vIndex + j] = data.CharData[i].currentMesh[j].color;
            }
        }

        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    void UpdateCharacterAnimation(int index)
    {
        CharData cData = data.CharData[index];

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
            data.CharData[index] = cData;
        }
    }

    void ApplyUpdateMeshOnCharacter(int index)
    {
        var info = data.Text.textInfo;
        TMP_CharacterInfo cInfo = info.characterInfo[index];
        int vIndex = cInfo.vertexIndex, mIndex = cInfo.materialReferenceIndex;
        Color32[] colors = info.meshInfo[mIndex].colors32;
        Vector3[] verts = info.meshInfo[mIndex].vertices;

        for (int j = 0; j < 4; j++)
        {
            verts[vIndex + j] = data.CharData[index].currentMesh[j].position;
            colors[vIndex + j] = data.CharData[index].currentMesh[j].color;
        }

        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    void CloseOpenTags(string text)
    {
        int endIndex = text.Length - 1;
        foreach (var tag in atp.ProcessedTags)
        {
            if (tag.IsOpen/* || (tag.length + tag.startIndex) > endIndex + 1*/)
                tag.Close(endIndex);
            //if (tag.IsOpen) tag.Close(endIndex);
        }
    }

    void OnTextChanged()
    {
        if (isAnimating) UpdateAnimations_Impl();
    }

    void OnForcedUpdate(int start, int length)
    {
        if (!isAnimating) return;

        for (int i = 0; i < length; i++)
        {
            UpdateCharacterAnimation(i + start);
            ApplyUpdateMeshOnCharacter(i + start);
        }
    }

    void UpdateData()
    {
        if (data == null && (data = GetComponent<TMPMediatorFinal>()) == null)
        {
            data = gameObject.AddComponent<TMPMediatorFinal>();
        }

        data.Initialize();
    }

    void UpdateDatabase()
    {
        Debug.Log("Updating database with " + database == null);
        data.Processor.UnregisterProcessor(ParsingUtility.NO_PREFIX);
        atp = new AnimationTagProcessor(database);
        data.Processor.RegisterProcessor(ParsingUtility.NO_PREFIX, atp);
    }

    void ResetAllVisible()
    {
        Debug.Log("Reset all visible");
        var info = data.Text.textInfo;

        Vector3[] verts;
        Color32[] colors;
        int vIndex, mIndex;
        TMP_CharacterInfo cInfo;

        // Iterate over all characters and apply the new meshes
        for (int i = 0; i < info.characterCount; i++)
        {
            cInfo = info.characterInfo[i];
            if (!cInfo.isVisible || data.CharData[i].hidden) continue;

            vIndex = cInfo.vertexIndex;
            mIndex = cInfo.materialReferenceIndex;

            colors = info.meshInfo[mIndex].colors32;
            verts = info.meshInfo[mIndex].vertices;

            CharData cData = data.CharData[i]; 

            for (int j = 0; j < 4; j++)
            {
                cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
                cData.currentMesh.SetColor(j, cData.initialMesh.GetColor(j));

                verts[vIndex + j] = data.CharData[i].initialMesh[j].position;
                colors[vIndex + j] = data.CharData[i].initialMesh[j].color;
            }

            data.CharData[i] = cData;
        }

        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

#if UNITY_EDITOR
    [SerializeField, HideInInspector] bool preview;
    Coroutine previewCoroutine;
    TMPEffectsDatabase prevDatabase = null;
    bool reprocessFlag = false;
    const string falseCallerMethodWarning = "The animations of the TMPAnimator on {0} were incorrectly updated " +
        "from method \"{1}\" instead of {0}'s \"{2}\" method; If you want to manually control the animation updates, set {0}'s UpdateFrom property to \"Script\", " +
        "either through the inspector or through a script using the SetUpdateFrom method.";

    private void OnValidate()
    {
        //Debug.Log("Onvalidate with data == null " + (data == null) + " and initialized " + (data == null ? "false" : data.isInitialized));
        // Ensure data is set - OnValidate often called before OnEnable
        if (data == null || !data.isInitialized) return;

        if (database != prevDatabase)
        {
            prevDatabase = database;
            UpdateDatabase();

            reprocessFlag = true;
            //data.ForceReprocess();
        }
    }

    public void StartPreview()
    {
        StartAnimating();
        if (previewCoroutine == null)
            previewCoroutine = StartCoroutine(Preview());
    }

    public void StopPreview()
    {
        if (previewCoroutine != null)
        {
            StopCoroutine(previewCoroutine);
            previewCoroutine = null;
        }

        StopAnimating();
        ResetAnimations();
    }

    IEnumerator Preview()
    {
        while (true)
        {
            UpdateAnimations_Impl();
            EditorApplication.QueuePlayerLoopUpdate();
            yield return null;
        }
    }

    public void ForceReprocess()
    {
        data.ForceReprocess();
    }
#endif
}