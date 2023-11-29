using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// One of the two main components of TMPEffects, along with TMPAnimator.
/// TMPWriter allows you to show text over time.
/// Using command tags, you can control how the text appears. For example, you can set the speed at which new characters are shown, or you can wait for a given amount of time.
/// Using event tags, you can raise events from text, i.e. when a specific character is shown. You can subscribe to these events with OnTextEvent. 
/// </summary>
[ExecuteAlways]
public class TMPWriterFinal : MonoBehaviour
{
    [SerializeField] TMPCommandDatabase database;

    [SerializeField] float speed = 1;
    [Tooltip("If checked, the writer will begin writing when it is first enabled. If not checked, you will have to manually start the writer from your own code.")]
    [SerializeField] bool writeOnStart = true;
    [SerializeField] bool autoWriteNewText = true;

    public int TotalCharacterCount => data.Text.textInfo.characterCount;
    public TMPCommandDatabase CommandDatabase => database;

    public UnityEvent<TMPEventArgs> OnTextEvent;
    public UnityEvent<CharData> OnShowCharacter;
    public UnityEvent OnStartWriter;
    public UnityEvent OnStopWriter;
    public UnityEvent OnFinishWriter;
    public UnityEvent<int> OnResetWriter;
    //public delegate void CharacterEventHandler(CharData cData);
    //public event CharacterEventHandler OnShowCharacter; 

    private TMPMediatorFinal data;
    private CommandTagProcessor ctp;
    private EventTagProcessor etp;

    private Coroutine writerCoroutine = null;
    private float currentSpeed;

    bool shouldWait = false;
    float waitAmount = 0f;

    public bool IsWriting => writing;
    bool writing = false;
    int currentIndex = -1;

    private void OnEnable()
    {
        Debug.LogWarning("OnEnable in " + (Application.isPlaying ? "play" : "edit") + " mode");

#if UNITY_EDITOR
        if (!Application.isPlaying) EditorApplication.update += QueueUpdate;
#endif

        UpdateData();
        UpdateDatabase();
        etp = new EventTagProcessor(); // Not dependent on any database
        data.Processor.RegisterProcessor('#', etp);

        data.Subscribe(this);
        //data.Processor.FinishProcessTags += CloseOpenTags; TODO Commands can have closing tags?
        //data.TextChanged += UpdateAnimations;
        data.ForceReprocess();
        data.TextChanged += OnTextChanged;
    }

    private void OnDisable()
    {
        Debug.LogWarning("OnDisable in " + (Application.isPlaying ? "play" : "edit") + " mode");

#if UNITY_EDITOR
        if (!Application.isPlaying) EditorApplication.update -= QueueUpdate;
#endif

        data.TextChanged -= OnTextChanged;
        //data.TextChanged -= StartWriterCoroutine;
        data.Processor.UnregisterProcessor('!');
        data.Processor.UnregisterProcessor('#');
        //data.Processor.FinishProcessTags -= CloseOpenTags;
        //data.TextChanged -= UpdateAnimations;

        // TODO Each reassigned in OnEnable anyway;
        // Either change class to reuse instances or dont reset (atm, resetting is necessary for some editor functionality though)
        etp.Reset();
        ctp.Reset();

        ResetWriter();

        data.ForceReprocess();
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if (writeOnStart) StartWriterCoroutine();
    }

    private void OnDestroy()
    {
        if (data != null) data.Unsubscribe(this);
    }

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

    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }

    public void Show(int start, int length)
    {
        TMP_TextInfo info = data.Text.textInfo;
        if (start < 0 || length < 0 || start + length > info.characterCount)
        {
            throw new System.ArgumentException("Invalid input: Start = " + start + "; Length = " + length + "; Length of string: " + info.characterCount);
        }

        CharData cData;
        Vector3[] verts;
        Color32[] colors;
        int vIndex, mIndex;
        TMP_CharacterInfo cInfo;

        for (int i = start; i < start + length; i++)
        {
            cInfo = info.characterInfo[i];
            if (!cInfo.isVisible) continue;

            cData = data.CharData[i];
            if (!cData.hidden) continue;

            // Set the current mesh's vertices all to the initial mesh values
            // TODO Play show animations here?
            for (int j = 0; j < 4; j++)
            {
                cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
            }

            cData.hidden = false;

            // Apply the new vertices to the vertex array
            vIndex = cInfo.vertexIndex;
            mIndex = cInfo.materialReferenceIndex;

            colors = info.meshInfo[mIndex].colors32;
            verts = info.meshInfo[mIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = cData.currentMesh[j].position;
                colors[vIndex + j] = cData.currentMesh[j].color;
            }

            // Apply the new vertices to the char data array
            data.CharData[i] = cData;
        }

        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    public void Hide(int start, int length)
    {
        TMP_TextInfo info = data.Text.textInfo;
        if (start < 0 || length < 0 || start + length > info.characterCount)
        {
            throw new System.ArgumentException("Invalid input: Start = " + start + "; Length = " + length + "; Length of string: " + info.characterCount);
        }

        CharData cData;
        Vector3[] verts;
        Color32[] colors;
        int vIndex, mIndex;
        TMP_CharacterInfo cInfo;

        for (int i = start; i < start + length; i++)
        {
            cInfo = info.characterInfo[i];
            if (!cInfo.isVisible) continue;

            cData = data.CharData[i];

            // Set the current mesh's vertices all to the initial mesh values
            // TODO Play show animations here?
            for (int j = 0; j < 4; j++)
            {
                cData.currentMesh.SetPosition(j, Vector3.zero);
            }

            cData.hidden = true;

            // Apply the new vertices to the vertex array
            vIndex = cInfo.vertexIndex;
            mIndex = cInfo.materialReferenceIndex;

            colors = info.meshInfo[mIndex].colors32;
            verts = info.meshInfo[mIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = cData.currentMesh[j].position;
                colors[vIndex + j] = cData.currentMesh[j].color;
            }

            // Apply the new vertices to the char data array
            data.CharData[i] = cData;
        }

        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    public void StartWriter()
    {
        if (!writing)
        {
            Debug.LogWarning("Start writer");
            StartWriterCoroutine();
        }
        else Debug.Log("already running");

        OnStartWriter.Invoke();
    }

    public void StopWriter()
    {
        if (writing)
        {
            Debug.LogWarning("Stop writer");
            StopWriterCoroutine();
        }

        OnStopWriter.Invoke();
    }

    // TODO Right now resetting writer shows all characters;
    // Should hide all characters; need to fix editor accordingly
    public void ResetWriter()
    {
        if (writing)
        {
            Debug.LogWarning("ResetWriter");
            StopWriterCoroutine();
        }

        // reset
        currentIndex = -1;
        Show(0, data.Text.textInfo.characterCount);

        OnResetWriter.Invoke(0);
    }

    public void ResetWriter(int index)
    {
        if (writing)
        {
            Debug.LogWarning("ResetWriter");
            StopWriterCoroutine();
        }

        // reset
        currentIndex = index;
        Hide(index, data.Text.textInfo.characterCount - index);
        Show(0, index);

        OnResetWriter.Invoke(index);
    }

    public void FinishWriter()
    {
        // skip to end
        if (writing) StopWriterCoroutine();
        currentIndex = data.Text.textInfo.characterCount;
        Show(0, data.Text.textInfo.characterCount);

        OnFinishWriter.Invoke();
    }

    public void RestartWriter()
    {
        ResetWriter();
        RestartWriter();
    }

    public void SetDatabase(TMPCommandDatabase database)
    {
        this.database = database;
        UpdateDatabase();
    }

    void OnStopWriting()
    {
        writing = false;
    }

    void OnStartWriting()
    {
        writing = true;
    }

    void StartWriterCoroutine()
    {
        StopWriterCoroutine();
        writerCoroutine = StartCoroutine(WriterCoroutine2());
    }

    void StopWriterCoroutine()
    {
        if (writerCoroutine != null)
        {
            StopCoroutine(writerCoroutine);
            writerCoroutine = null;
        }

        OnStopWriting();
    }

    void OnTextChanged()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            bool wasWriting = writing;
            ResetWriter();
            if (wasWriting)
            {
                StartWriter();
            }
            return;
        }
#endif

        ResetWriter();
        if (autoWriteNewText) StartWriter();
    }

    public void MessageReciver(TMPEventArgs args)
    {
        Debug.Log("Received event with " + args.name + " and " + (args.parameters == null ? "0" : args.parameters.Count) + " parameters");
    }

    IEnumerator WriterCoroutine2()
    {
        OnStartWriting();

        // TODO This indicates the text is fully shown already
        if (currentIndex >= data.Text.textInfo.characterCount)
        {
            OnStopWriting();
            yield break;
        }

        // Reset all relevant variables
        currentSpeed = speed;

        TMP_TextInfo info = data.Text.textInfo;
        TMP_CharacterInfo cInfo;
        CharData cData;
        Vector3[] verts;
        Color32[] colors;
        int vIndex, mIndex;

        if (currentIndex == -1)
            HideAllCharacters();

        //Debug.Log(ctp.ProcessedTags.Count + " commands");
        //Debug.Log(etp.ProcessedTags.Count + " events");
        if (data.CharData.Count == 0) Debug.Log("Chardata not initialized yet!");

        for (int i = Mathf.Max(currentIndex, 0); i < info.characterCount; i++)
        {
            // TODO preliminary wait implementation
            if (shouldWait)
            {
                shouldWait = false;
                yield return new WaitForSeconds(waitAmount);
            }

            // Update current index; needed for when continuing where it left off
            currentIndex = i;

            cData = data.CharData[i];
            cData.hidden = false;

            for (int j = 0; j < 4; j++)
            {
                cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
            }
            data.CharData[i] = cData;


            // Raise any events or comamnds associated with the current index
            for (int j = 0; j < ctp.ProcessedTags.Count; j++)
            {
                if (ctp.ProcessedTags[j].index == i)
                {
#if UNITY_EDITOR
                    if (!commandsEnabled && !Application.isPlaying) continue;
#endif

                    Debug.Log("Raising command " + ctp.ProcessedTags[j].name);
                    database.GetCommand(ctp.ProcessedTags[j].name).ExecuteCommand(ctp.ProcessedTags[j], this);
                }
            }
            for (int j = 0; j < etp.ProcessedTags.Count; j++)
            {
                if (etp.ProcessedTags[j].index == i)
                {
#if UNITY_EDITOR
                    if (!eventsEnabled && !Application.isPlaying) continue;
#endif

                    Debug.Log("Raising event");
                    OnTextEvent?.Invoke(etp.ProcessedTags[j]);
                }
            }

            cInfo = info.characterInfo[i];

            OnShowCharacter?.Invoke(cData);

            //data.activeEndIndex = i;
            if (!cInfo.isVisible /*|| !cData.hidden*/) continue;

            vIndex = cInfo.vertexIndex;
            mIndex = cInfo.materialReferenceIndex;

            colors = info.meshInfo[mIndex].colors32;
            verts = info.meshInfo[mIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = data.CharData[i].currentMesh[j].position;
                colors[vIndex + j] = data.CharData[i].currentMesh[j].color;
            }

            data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            data.ForceUpdate(i, 1);

            //data.activeEndIndex = i;
            if (currentSpeed > 0)
            {
                yield return new WaitForSeconds((1f / currentSpeed) * 0.1f);
            }
        }

        //currentIndex = -1;
        OnStopWriting();
    }

    void HideAllCharacters()
    {
        TMP_TextInfo info = data.Text.textInfo;
        CharData cData;

        Vector3[] verts;
        Color32[] colors;
        int vIndex, mIndex;
        TMP_CharacterInfo cInfo;

        for (int i = 0; i < data.CharData.Count; i++)
        {
            cInfo = info.characterInfo[i];
            if (!cInfo.isVisible) continue;

            cData = data.CharData[i];

            // Set the current mesh's vertices all to V3.zero
            for (int j = 0; j < 4; j++)
            {
                cData.currentMesh.SetPosition(j, Vector3.zero);
            }

            cData.hidden = true;

            // Apply the new vertices to the vertex array
            vIndex = cInfo.vertexIndex;
            mIndex = cInfo.materialReferenceIndex;

            colors = info.meshInfo[mIndex].colors32;
            verts = info.meshInfo[mIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = cData.currentMesh[j].position;
                colors[vIndex + j] = cData.currentMesh[j].color;
            }

            // Apply the new vertices to the char data array
            data.CharData[i] = cData;
        }

        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    void UpdateData()
    {
        Debug.Log("update data");
        if (data == null && (data = GetComponent<TMPMediatorFinal>()) == null)
        {
            data = gameObject.AddComponent<TMPMediatorFinal>();
        }

        data.Initialize();
    }

    void UpdateDatabase()
    {
        data.Processor.UnregisterProcessor('!');
        //data.Processor.UnregisterProcessor('#');
        ctp = new CommandTagProcessor(database);
        //etp = new EventTagProcessor();
        data.Processor.RegisterProcessor('!', ctp);
        //data.Processor.RegisterProcessor('#', etp);
    }

#if UNITY_EDITOR
    #region Editor
    TMPCommandDatabase prevDatabase = null;
    bool reprocessFlag = false;
    [HideInInspector] public bool eventsEnabled = false;
    [HideInInspector] public bool commandsEnabled = false;

    void QueueUpdate()
    {
        // TODO
        // Either change method name to smth like. WriterUpdate
        // or move reprocess flag somewhere else; not conceptually related
        // to queueing updates
        if (reprocessFlag)
        {
            data.ForceReprocess();
            reprocessFlag = false;
        }
        /*if (writing)*/
        EditorApplication.QueuePlayerLoopUpdate();
    }

    private void OnValidate()
    {
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

    public void ManualInit()
    {
        Debug.LogWarning("Manual init");
        OnEnable();
    }
    #endregion
#endif
}



//using System.Collections;
//using TMPro;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.Events;

///// <summary>
///// One of the two main components of TMPEffects, along with TMPAnimator.
///// TMPWriter allows you to show text over time.
///// Using command tags, you can control how the text appears. For example, you can set the speed at which new characters are shown, or you can wait for a given amount of time.
///// Using event tags, you can raise events from text, i.e. when a specific character is shown. You can subscribe to these events with OnTextEvent. 
///// </summary>
//[ExecuteAlways]
//public class TMPWriterFinal : MonoBehaviour
//{
//    [SerializeField] TMPCommandDatabase database;

//    [SerializeField] float speed = 1;
//    [Tooltip("If checked, the writer will begin writing when it is first enabled. If not checked, you will have to manually start the writer from your own code.")]
//    [SerializeField] bool writeOnStart = true;

//    public int TotalCharacterCount => data.Text.textInfo.characterCount;
//    public TMPCommandDatabase CommandDatabase => database;

//    public UnityEvent<TMPEventArgs> OnTextEvent;
//    public UnityEvent<CharData> OnShowCharacter;
//    public UnityEvent OnStartWriter;
//    public UnityEvent OnStopWriter;
//    public UnityEvent OnFinishWriter;
//    public UnityEvent<int> OnResetWriter;
//    //public delegate void CharacterEventHandler(CharData cData);
//    //public event CharacterEventHandler OnShowCharacter; 

//    private TMPMediatorFinal data;
//    private CommandTagProcessor ctp;
//    private EventTagProcessor etp;

//    private Coroutine writerCoroutine = null;
//    private float currentSpeed;

//    bool shouldWait = false;
//    float waitAmount = 0f;

//    public bool IsWriting => writing;
//    bool writing = false;
//    int currentIndex = -1;

//    private void OnEnable()
//    {
//        Debug.LogWarning("OnEnable in " + (Application.isPlaying ? "play" : "edit") + " mode");

//#if UNITY_EDITOR
//        if (!Application.isPlaying) EditorApplication.update += QueueUpdate;
//#endif

//        UpdateData();
//        UpdateDatabase();
//        etp = new EventTagProcessor(); // Not dependent on any database
//        data.Processor.RegisterProcessor('#', etp);

//        data.Subscribe(this);
//        //data.Processor.FinishProcessTags += CloseOpenTags; TODO Commands can have closing tags?
//        //data.TextChanged += UpdateAnimations;
//        data.ForceReprocess();
//        data.TextChanged += OnTextChanged;
//    }

//    private void OnDisable()
//    {
//        Debug.LogWarning("OnDisable in " + (Application.isPlaying ? "play" : "edit") + " mode");

//#if UNITY_EDITOR
//        if (!Application.isPlaying) EditorApplication.update -= QueueUpdate;
//#endif

//        data.TextChanged -= OnTextChanged;
//        //data.TextChanged -= StartWriterCoroutine;
//        data.Processor.UnregisterProcessor('!');
//        data.Processor.UnregisterProcessor('#');
//        //data.Processor.FinishProcessTags -= CloseOpenTags;
//        //data.TextChanged -= UpdateAnimations;

//        // TODO Each reassigned in OnEnable anyway;
//        // Either change class to reuse instances or dont reset (atm, resetting is necessary for some editor functionality though)
//        etp.Reset();
//        ctp.Reset();

//        ResetWriter();

//        data.ForceReprocess();
//    }

//    private void Start()
//    {
//#if UNITY_EDITOR
//        if (!Application.isPlaying) return;
//#endif
//        if (writeOnStart) StartWriterCoroutine();
//    }

//    private void OnDestroy()
//    {
//        if (data != null) data.Unsubscribe(this);
//    }

//    public void Wait(float seconds)
//    {
//        if (seconds < 0f)
//        {
//            throw new System.ArgumentOutOfRangeException("Seconds was negative");
//        }

//        if (shouldWait)
//        {
//            waitAmount = Mathf.Max(waitAmount, seconds);
//            return;
//        }

//        shouldWait = true;
//        waitAmount = seconds;
//    }

//    public void SetSpeed(float speed)
//    {
//        currentSpeed = speed;
//    }

//    public void Show(int start, int length)
//    {
//        TMP_TextInfo info = data.Text.textInfo;
//        if (start < 0 || length < 0 || start + length > info.characterCount)
//        {
//            throw new System.ArgumentException("Invalid input: Start = " + start + "; Length = " + length + "; Length of string: " + info.characterCount);
//        }

//        CharData cData;
//        Vector3[] verts;
//        Color32[] colors;
//        int vIndex, mIndex;
//        TMP_CharacterInfo cInfo;

//        for (int i = start; i < start + length; i++)
//        {
//            cInfo = info.characterInfo[i];
//            if (!cInfo.isVisible) continue;

//            cData = data.CharData[i];
//            if (!cData.hidden) continue;

//            // Set the current mesh's vertices all to the initial mesh values
//            // TODO Play show animations here?
//            for (int j = 0; j < 4; j++)
//            {
//                cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
//            }

//            cData.hidden = false;

//            // Apply the new vertices to the vertex array
//            vIndex = cInfo.vertexIndex;
//            mIndex = cInfo.materialReferenceIndex;

//            colors = info.meshInfo[mIndex].colors32;
//            verts = info.meshInfo[mIndex].vertices;

//            for (int j = 0; j < 4; j++)
//            {
//                verts[vIndex + j] = cData.currentMesh[j].position;
//                colors[vIndex + j] = cData.currentMesh[j].color;
//            }

//            // Apply the new vertices to the char data array
//            data.CharData[i] = cData;
//        }

//        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
//    }

//    public void Hide(int start, int length)
//    {
//        TMP_TextInfo info = data.Text.textInfo;
//        if (start < 0 || length < 0 || start + length > info.characterCount)
//        {
//            throw new System.ArgumentException("Invalid input: Start = " + start + "; Length = " + length + "; Length of string: " + info.characterCount);
//        }

//        CharData cData;
//        Vector3[] verts;
//        Color32[] colors;
//        int vIndex, mIndex;
//        TMP_CharacterInfo cInfo;

//        for (int i = start; i < start + length; i++)
//        {
//            cInfo = info.characterInfo[i];
//            if (!cInfo.isVisible) continue;

//            cData = data.CharData[i];

//            // Set the current mesh's vertices all to the initial mesh values
//            // TODO Play show animations here?
//            for (int j = 0; j < 4; j++)
//            {
//                cData.currentMesh.SetPosition(j, Vector3.zero);
//            }

//            cData.hidden = true;

//            // Apply the new vertices to the vertex array
//            vIndex = cInfo.vertexIndex;
//            mIndex = cInfo.materialReferenceIndex;

//            colors = info.meshInfo[mIndex].colors32;
//            verts = info.meshInfo[mIndex].vertices;

//            for (int j = 0; j < 4; j++)
//            {
//                verts[vIndex + j] = cData.currentMesh[j].position;
//                colors[vIndex + j] = cData.currentMesh[j].color;
//            }

//            // Apply the new vertices to the char data array
//            data.CharData[i] = cData;
//        }

//        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
//    }

//    /* Below: methods such as start/pause/stop player for editor previewing
//     * Want to implement essentially the same functionality for runtime code too,
//     * to allow controlling the writer via script.
//     * 
//     * Then also remove these editor methods an replace with runtime versions.
//    */

//    public void StartWriter()
//    {
//        if (!writing)
//        {
//            Debug.LogWarning("Start writer");
//            StartWriterCoroutine();
//        }
//        else Debug.Log("already running");

//        OnStartWriter.Invoke();
//    }

//    public void StopWriter()
//    {
//        if (writing)
//        {
//            Debug.LogWarning("Stop writer");
//            StopWriterCoroutine();
//        }

//        OnStopWriter.Invoke();
//    }

//    // TODO Allow resetting to specific index?
//    // ResetWriter(12): StopWriterCoroutine, Hide(12, len-12), Show(0, 12)
//    // Can be used for skipping to specific points in text 
//    public void ResetWriter()
//    {
//        if (writing)
//        {
//            Debug.LogWarning("ResetWriter");
//            StopWriterCoroutine();
//        }

//        // reset
//        currentIndex = -1;
//        Show(0, data.Text.textInfo.characterCount);

//        OnResetWriter.Invoke(0);
//    }

//    public void ResetWriter(int index)
//    {
//        if (writing)
//        {
//            Debug.LogWarning("ResetWriter");
//            StopWriterCoroutine();
//        }

//        // reset
//        currentIndex = index;
//        Hide(index, data.Text.textInfo.characterCount - index);
//        Show(0, index);

//        OnResetWriter.Invoke(index);
//    }

//    public void FinishWriter()
//    {
//        // skip to end
//        if (writing) StopWriterCoroutine();
//        currentIndex = data.Text.textInfo.characterCount;
//        Show(0, data.Text.textInfo.characterCount);

//        OnFinishWriter.Invoke();
//    }

//    public void RestartWriter()
//    {
//        ResetWriter();
//        RestartWriter();
//    }

//    public void SetDatabase(TMPCommandDatabase database)
//    {
//        this.database = database;
//        UpdateDatabase();
//    }

//    void OnStopWriting()
//    {
//        writing = false;
//    }

//    void OnStartWriting()
//    {
//        writing = true;
//    }

//    void StartWriterCoroutine()
//    {
//        StopWriterCoroutine();
//        writerCoroutine = StartCoroutine(WriterCoroutine2());
//    }

//    void StopWriterCoroutine()
//    {
//        if (writerCoroutine != null)
//        {
//            StopCoroutine(writerCoroutine);
//            writerCoroutine = null;
//        }

//        OnStopWriting();
//    }

//    void OnTextChanged()
//    {
//        bool wasWriting = writing;
//        ResetWriter();
//        if (wasWriting) StartWriter();
//    }

//    public void MessageReciver(TMPEventArgs args)
//    {
//        Debug.Log("Received event with " + args.name + " and " + (args.parameters == null ? "0" : args.parameters.Count) + " parameters");
//    }

//    IEnumerator WriterCoroutine2()
//    {
//        OnStartWriting();

//        // TODO This indicates the text is fully shown already
//        if (currentIndex >= data.Text.textInfo.characterCount)
//        {
//            OnStopWriting();
//            yield break;
//        }

//        // Reset all relevant variables
//        currentSpeed = speed;

//        TMP_TextInfo info = data.Text.textInfo;
//        TMP_CharacterInfo cInfo;
//        CharData cData;
//        Vector3[] verts;
//        Color32[] colors;
//        int vIndex, mIndex;

//        if (currentIndex == -1)
//            HideAllCharacters();

//        //Debug.Log(ctp.ProcessedTags.Count + " commands");
//        //Debug.Log(etp.ProcessedTags.Count + " events");
//        if (data.CharData.Count == 0) Debug.Log("Chardata not initialized yet!");

//        for (int i = Mathf.Max(currentIndex, 0); i < info.characterCount; i++)
//        {
//            // TODO preliminary wait implementation
//            if (shouldWait)
//            {
//                shouldWait = false;
//                yield return new WaitForSeconds(waitAmount);
//            }

//            // Update current index; needed for when continuing where it left off
//            currentIndex = i;

//            cData = data.CharData[i];
//            cData.hidden = false;

//            for (int j = 0; j < 4; j++)
//            {
//                cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
//            }
//            data.CharData[i] = cData;


//            // Raise any events or comamnds associated with the current index
//            for (int j = 0; j < ctp.ProcessedTags.Count; j++)
//            {
//                if (ctp.ProcessedTags[j].index == i)
//                {
//#if UNITY_EDITOR
//                    if (!commandsEnabled && !Application.isPlaying) continue;
//#endif

//                    Debug.Log("Raising command " + ctp.ProcessedTags[j].name);
//                    database.GetCommand(ctp.ProcessedTags[j].name).ExecuteCommand(ctp.ProcessedTags[j], this);
//                }
//            }
//            for (int j = 0; j < etp.ProcessedTags.Count; j++)
//            {
//                if (etp.ProcessedTags[j].index == i)
//                {
//#if UNITY_EDITOR
//                    if (!eventsEnabled && !Application.isPlaying) continue;
//#endif

//                    Debug.Log("Raising event");
//                    OnTextEvent?.Invoke(etp.ProcessedTags[j]);
//                }
//            }

//            cInfo = info.characterInfo[i];

//            OnShowCharacter?.Invoke(cData);

//            //data.activeEndIndex = i;
//            if (!cInfo.isVisible /*|| !cData.hidden*/) continue;

//            vIndex = cInfo.vertexIndex;
//            mIndex = cInfo.materialReferenceIndex;

//            colors = info.meshInfo[mIndex].colors32;
//            verts = info.meshInfo[mIndex].vertices;

//            for (int j = 0; j < 4; j++)
//            {
//                verts[vIndex + j] = data.CharData[i].currentMesh[j].position;
//                colors[vIndex + j] = data.CharData[i].currentMesh[j].color;
//            }

//            data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

//            data.ForceUpdate(i, 1);

//            //data.activeEndIndex = i;
//            if (currentSpeed > 0)
//            {
//                yield return new WaitForSeconds((1f / currentSpeed) * 0.1f);
//            }
//        }

//        //currentIndex = -1;
//        OnStopWriting();
//    }

//    void HideAllCharacters()
//    {
//        TMP_TextInfo info = data.Text.textInfo;
//        CharData cData;

//        Vector3[] verts;
//        Color32[] colors;
//        int vIndex, mIndex;
//        TMP_CharacterInfo cInfo;

//        for (int i = 0; i < data.CharData.Count; i++)
//        {
//            cInfo = info.characterInfo[i];
//            if (!cInfo.isVisible) continue;

//            cData = data.CharData[i];

//            // Set the current mesh's vertices all to V3.zero
//            for (int j = 0; j < 4; j++)
//            {
//                cData.currentMesh.SetPosition(j, Vector3.zero);
//            }

//            cData.hidden = true;

//            // Apply the new vertices to the vertex array
//            vIndex = cInfo.vertexIndex;
//            mIndex = cInfo.materialReferenceIndex;

//            colors = info.meshInfo[mIndex].colors32;
//            verts = info.meshInfo[mIndex].vertices;

//            for (int j = 0; j < 4; j++)
//            {
//                verts[vIndex + j] = cData.currentMesh[j].position;
//                colors[vIndex + j] = cData.currentMesh[j].color;
//            }

//            // Apply the new vertices to the char data array
//            data.CharData[i] = cData;
//        }

//        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
//    }

//    void UpdateData()
//    {
//        Debug.Log("update data");
//        if (data == null && (data = GetComponent<TMPMediatorFinal>()) == null)
//        {
//            data = gameObject.AddComponent<TMPMediatorFinal>();
//        }

//        data.Initialize();
//    }

//    void UpdateDatabase()
//    {
//        data.Processor.UnregisterProcessor('!');
//        //data.Processor.UnregisterProcessor('#');
//        ctp = new CommandTagProcessor(database);
//        //etp = new EventTagProcessor();
//        data.Processor.RegisterProcessor('!', ctp);
//        //data.Processor.RegisterProcessor('#', etp);
//    }

//#if UNITY_EDITOR
//    #region Editor
//    TMPCommandDatabase prevDatabase = null;
//    bool reprocessFlag = false;
//    [HideInInspector] public bool eventsEnabled = false;
//    [HideInInspector] public bool commandsEnabled = false;

//    void QueueUpdate()
//    {
//        // TODO
//        // Either change method name to smth like. WriterUpdate
//        // or move reprocess flag somewhere else; not conceptually related
//        // to queueing updates
//        if (reprocessFlag)
//        {
//            data.ForceReprocess();
//            reprocessFlag = false;
//        }
//        /*if (writing)*/
//        EditorApplication.QueuePlayerLoopUpdate();
//    }

//    private void OnValidate()
//    {
//        // Ensure data is set - OnValidate often called before OnEnable
//        if (data == null || !data.isInitialized) return;

//        if (database != prevDatabase)
//        {
//            prevDatabase = database;
//            UpdateDatabase();

//            reprocessFlag = true;
//            //data.ForceReprocess();
//        }
//    }

//    public void ManualInit()
//    {
//        Debug.LogWarning("Manual init");
//        OnEnable();
//    }
//    #endregion
//#endif
//}
