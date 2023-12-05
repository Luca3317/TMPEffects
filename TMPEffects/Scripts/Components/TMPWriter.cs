using AYellowpaper.SerializedCollections;
using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static log4net.Appender.ColoredConsoleAppender;
using static System.Net.WebRequestMethods;

/// <summary>
/// One of the two main components of TMPEffects, along with TMPAnimator.
/// TMPWriter allows you to show text over time.
/// Using command tags, you can control how the text appears. For example, you can set the speed at which new characters are shown, or you can wait for a given amount of time.
/// Using event tags, you can raise events from text, i.e. when a specific character is shown. You can subscribe to these events with OnTextEvent. 
/// </summary>
[ExecuteAlways]
public class TMPWriter : TMPEffectComponent
{
    public bool IsWriting => writing;
    public int TotalCharacterCount => mediator.Text.textInfo.characterCount;
    public TMPCommandDatabase CommandDatabase => database;
    public List<TMPCommandTag> Commands => ctps.ProcessedTags;
    public List<TMPEventArgs> Events => etp.ProcessedTags;

    public UnityEvent<TMPEventArgs> OnTextEvent;
    public UnityEvent<CharData> OnShowCharacter;
    public UnityEvent OnStartWriter;
    public UnityEvent OnStopWriter;
    public UnityEvent OnFinishWriter;
    public UnityEvent<int> OnResetWriter;

    #region Fields
    [SerializeField] TMPCommandDatabase database;

    [SerializeField] float speed = 1;
    [Tooltip("If checked, the writer will begin writing when it is first enabled. If not checked, you will have to manually start the writer from your own code.")]
    [SerializeField] bool writeOnStart = true;
    [SerializeField] bool autoWriteNewText = true;
    [SerializedDictionary("Tag Name", "Command")]
    [SerializeField] SerializedDictionary<string, SceneCommand> sceneCommands;

    [System.NonSerialized] private CommandTagProcessor ctp;
    [System.NonSerialized] private SceneCommandTagProcessor sctp;
    [System.NonSerialized] private EventTagProcessor etp;
    [System.NonSerialized] private TagProcessorStack<TMPCommandTag> ctps;

    [System.NonSerialized] private Coroutine writerCoroutine = null;
    [System.NonSerialized] private float currentSpeed;

    [System.NonSerialized] private bool shouldWait = false;
    [System.NonSerialized] private float waitAmount = 0f;

    [System.NonSerialized] private bool writing = false;
    [System.NonSerialized] private int currentIndex = -1;

    [System.NonSerialized] Dictionary<int, List<TMPEventArgs>> events;
    [System.NonSerialized] Dictionary<int, List<TMPCommandTag>> commands;
    #endregion

    #region Initialization
    private void OnEnable()
    {
        UpdateMediator();
        UpdateProcessor(forceReprocess: false);

        etp = new EventTagProcessor(); // Not dependent on any database
        mediator.Processor.RegisterProcessor('#', etp);

        mediator.Subscribe(this);
        //data.Processor.FinishProcessTags += CloseOpenTags; TODO Commands can have closing tags?
        //data.TextChanged += UpdateAnimations;
        mediator.TextChanged -= OnTextChanged;
        mediator.TextChanged += OnTextChanged;
        mediator.Processor.FinishProcessTags += CloseOpenTags;

        mediator.ForceReprocess();

#if UNITY_EDITOR
        if (!Application.isPlaying) EditorApplication.update += EditorUpdate;
#endif
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if (writeOnStart) StartWriter();
    }
    #endregion

    #region CleanUp
    private void OnDisable()
    {
        mediator.TextChanged -= OnTextChanged;
        mediator.Processor.FinishProcessTags -= CloseOpenTags;
        mediator.Processor.UnregisterProcessor('!');
        mediator.Processor.UnregisterProcessor('#');
        mediator.Processor.UnregisterProcessor('+');

        // TODO Each reassigned in OnEnable anyway;
        // Either change class to reuse instances or dont reset (atm, resetting is necessary for some editor functionality though)
        etp.Reset();
        ctps.Reset();

        ResetWriter();

        mediator.ForceReprocess();

#if UNITY_EDITOR
        if (!Application.isPlaying) EditorApplication.delayCall += () => { EditorApplication.update -= EditorUpdate; };
#endif
    }

    private void OnDestroy()
    {
        if (mediator != null) mediator.Unsubscribe(this);
    }
    #endregion

    #region Writer Controlling
    /// <summary>
    /// Start (or resume) writing.
    /// </summary>
    public void StartWriter()
    {
        if (!enabled || !gameObject.activeInHierarchy) return;

        if (!writing)
        {
            StartWriterCoroutine();
        }

        OnStartWriter.Invoke();
    }

    /// <summary>
    /// Stop writing. 
    /// Note that this does not reset the shown text.
    /// </summary>
    public void StopWriter()
    {
        if (!enabled || !gameObject.activeInHierarchy) return;

        if (writing)
        {
            StopWriterCoroutine();
        }

        OnStopWriter.Invoke();
    }

    /// <summary>
    /// Reset the writer.
    /// </summary>
    /// TODO rn this shows all characters; should hide them but need to fix editor accordingly
    public void ResetWriter()
    {
        if (!enabled || !gameObject.activeInHierarchy) return;

        if (writing)
        {
            StopWriterCoroutine();
        }

        // reset
        currentIndex = -1;
        Show(0, mediator.Text.textInfo.characterCount, true);

        OnResetWriter?.Invoke(0);
    }

    /// <summary>
    /// Reset the writer to the given index.
    /// </summary>
    /// <param name="index">The index to reset the writer to.</param>
    public void ResetWriter(int index)
    {
        if (!enabled || !gameObject.activeInHierarchy) return;

        if (writing)
        {
            StopWriterCoroutine();
        }

        // reset
        currentIndex = index;
        Hide(index, mediator.Text.textInfo.characterCount - index, true);
        Show(0, index, true);

        OnResetWriter.Invoke(index);
    }

    /// <summary>
    /// Finish the writing process. 
    /// This will instantly show all characters, raise all not-yet raised events and execute commands that are tagged with Raise-On-Skip. TODO Implement as defiend here
    /// </summary>
    public void FinishWriter()
    {
        if (!enabled || !gameObject.activeInHierarchy) return;

        // skip to end
        if (writing) StopWriterCoroutine();
        currentIndex = mediator.Text.textInfo.characterCount;
        Show(0, mediator.Text.textInfo.characterCount, true);

        OnFinishWriter.Invoke();
    }

    /// <summary>
    /// Restart the writer.
    /// This will reset the writer and start the writing process.
    /// </summary>
    public void RestartWriter()
    {
        if (!enabled) return;

        ResetWriter();
        RestartWriter();
    }
    #endregion

    #region Writer Commands
    /// <summary>
    /// Pause the writer for the given amount of seconds.
    /// </summary>
    /// <param name="seconds">The amount of time to wait.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">Throws if <paramref name="seconds"/> is less than zero.</exception>
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

    /// <summary>
    /// Set the current speed of the writer.
    /// </summary>
    /// <param name="speed">The speed to set the writer to.</param>
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }
    #endregion

    #region Setters
    /// <summary>
    /// Set the database that will be used to parse command tags.
    /// </summary>
    /// <param name="database">The database that will be used to parse command tags.</param>
    public void SetDatabase(TMPCommandDatabase database)
    {
        this.database = database;
        UpdateProcessor();
    }
    #endregion

    #region Event Callbacks
    private void OnTextChanged()
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
    #endregion

    #region Writer Coroutine
    private void StartWriterCoroutine()
    {
        StopWriterCoroutine();
        writerCoroutine = StartCoroutine(WriterCoroutine());
    }

    private void StopWriterCoroutine()
    {
        if (writerCoroutine != null)
        {
            StopCoroutine(writerCoroutine);
            writerCoroutine = null;
        }

        OnStopWriting();
    }

    IEnumerator WriterCoroutine()
    {
        OnStartWriting();

        // TODO This indicates the text is fully shown already
        if (currentIndex >= mediator.Text.textInfo.characterCount)
        {
            OnStopWriting();
            yield break;
        }

        // Reset all relevant variables
        currentSpeed = speed;

        TMP_TextInfo info = mediator.Text.textInfo;
        TMP_CharacterInfo cInfo;
        CharData cData;
        Vector3[] verts;
        Color32[] colors;
        int vIndex, mIndex;

        if (currentIndex == -1)
            HideAllCharacters(true);

        // Execute all commands tagged as ExecuteInstantly
        TMPCommand command;
        for (int j = 0; j < ctp.ProcessedTags.Count; j++)
        {
            if ((command = database.GetEffect(ctp.ProcessedTags[j].name)).ExecuteInstantly)
            {
                command.ExecuteCommand(ctp.ProcessedTags[j], this);
            }
        }

        for (int i = Mathf.Max(currentIndex, 0); i < info.characterCount; i++)
        {
            // Update current index; needed for when continuing where it left off
            currentIndex = i;

            // TODO preliminary wait implementation
            if (shouldWait)
            {
                shouldWait = false;
                yield return new WaitForSeconds(waitAmount);
            }

            cData = mediator.CharData[i];

            //for (int j = 0; j < 4; j++)
            //{
            //    cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
            //}
            //mediator.CharData[i] = cData;


            // Raise any events or comamnds associated with the current index
            for (int j = 0; j < ctp.ProcessedTags.Count; j++)
            {
                if (ctp.ProcessedTags[j].startIndex == i)
                {
#if UNITY_EDITOR
                    if (!commandsEnabled && !Application.isPlaying) continue;
#endif

                    database.GetEffect(ctp.ProcessedTags[j].name).ExecuteCommand(ctp.ProcessedTags[j], this);
                }
            }
            for (int j = 0; j < sctp.ProcessedTags.Count; j++)
            {
                if (sctp.ProcessedTags[j].startIndex == i)
                {
#if UNITY_EDITOR
                    if (!commandsEnabled && !Application.isPlaying) continue;
#endif
                    sceneCommands[sctp.ProcessedTags[j].name].command?.Invoke(sctp.ProcessedTags[j].parameters);
                    //database.GetCommand(sctp.ProcessedTags[j].name).ExecuteCommand(sctp.ProcessedTags[j], this);
                }
            }
            for (int j = 0; j < etp.ProcessedTags.Count; j++)
            {
                if (etp.ProcessedTags[j].index == i)
                {
#if UNITY_EDITOR
                    if (!eventsEnabled && !Application.isPlaying) continue;
#endif

                    OnTextEvent?.Invoke(etp.ProcessedTags[j]);
                }
            }

            cInfo = info.characterInfo[i];

            OnShowCharacter?.Invoke(cData);

            // Show next character
            if (!cInfo.isVisible || cData.visibilityState != CharData.VisibilityState.Hidden) continue;

            //vIndex = cInfo.vertexIndex;
            //mIndex = cInfo.materialReferenceIndex;

            //colors = info.meshInfo[mIndex].colors32;
            //verts = info.meshInfo[mIndex].vertices;

            //for (int j = 0; j < 4; j++)
            //{
            //    verts[vIndex + j] = mediator.CharData[i].currentMesh[j].position;
            //    colors[vIndex + j] = mediator.CharData[i].currentMesh[j].color;
            //}

            //mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            //mediator.ForceUpdate(i, 1);

            Show(i, 1, false);
            mediator.ForceUpdate(i, 1);

            //data.activeEndIndex = i;
            if (currentSpeed > 0)
            {
                yield return new WaitForSeconds((1f / currentSpeed) * 0.1f);
            }
        }

        //currentIndex = -1;
        OnStopWriting();
    }

    private void OnStopWriting()
    {
        writing = false;
    }

    private void OnStartWriting()
    {
        writing = true;
    }
    #endregion

    #region Editor
#if UNITY_EDITOR
    [System.NonSerialized] TMPCommandDatabase prevDatabase = null;
    [System.NonSerialized] bool reprocessFlag = false;
    [SerializeField, HideInInspector] bool eventsEnabled = false;
    [SerializeField, HideInInspector] bool commandsEnabled = false;

    private void EditorUpdate()
    {
        if (reprocessFlag)
        {
            mediator.ForceReprocess();
            reprocessFlag = false;
        }
        EditorApplication.QueuePlayerLoopUpdate();
    }

    private void OnValidate()
    {
        // Ensure data is set - OnValidate often called before OnEnable
        if (mediator == null || !mediator.isInitialized) return;

        if (database != prevDatabase)
        {
            prevDatabase = database;
            UpdateProcessor();

            reprocessFlag = true;
        }
    }

    public void ForceReprocess()
    {
        mediator.ForceReprocess();
    }
#endif
    #endregion

    private void HideAllCharacters(bool skipAnimations = false)
    {
        Hide(0, mediator.Text.textInfo.characterCount, skipAnimations);
    }

    private void UpdateProcessor(bool forceReprocess = true)
    {
        mediator.Processor.UnregisterProcessor('!');
        ctps = new TagProcessorStack<TMPCommandTag>();
        ctps.AddProcessor(ctp = new CommandTagProcessor(database));
        ctps.AddProcessor(sctp = new SceneCommandTagProcessor(sceneCommands));
        mediator.Processor.RegisterProcessor('!', ctps);

        if (forceReprocess) mediator.ForceReprocess();
    }

    private void CloseOpenTags(string text)
    {
        int endIndex = text.Length - 1;
        foreach (var tag in ctps.ProcessedTags)
        {
            if (tag.IsOpen)
                tag.Close(endIndex);
        }
    }

    /// <summary>
    /// Show the specified characters.
    /// </summary>
    /// <param name="start">The startindex of the characters to be shown.</param>
    /// <param name="length">The amount of characters to be shown, starting from <paramref name="start"/></param>
    /// <exception cref="System.ArgumentException">Throws if either <paramref name="start"/> or <paramref name="length"/> is out of bounds.</exception>
    public void Show(int start, int length, bool skipAnimation = false)
    {
        TMP_TextInfo info = mediator.Text.textInfo;
        if (start < 0 || length < 0 || start + length > info.characterCount)
        {
            throw new System.ArgumentException("Invalid input: Start = " + start + "; Length = " + length + "; Length of string: " + info.characterCount);
        }

        CharData cData;
        TMP_CharacterInfo cInfo;
        if (skipAnimation)
        {
            Vector3[] verts;
            Color32[] colors;
            int vIndex, mIndex;

            for (int i = start; i < start + length; i++)
            {
                cInfo = info.characterInfo[i];
                cData = mediator.CharData[i];
                if (!cData.isVisible) continue;

                // Set the current mesh's vertices all to the initial mesh values
                for (int j = 0; j < 4; j++)
                {
                    cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
                }

                cData.visibilityState = CharData.VisibilityState.Shown;

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
                mediator.CharData[i] = cData;
            }

            if (mediator.Text.mesh != null)
                mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
        else
        {
            for (int i = start; i < start + length; i++)
            {
                cData = mediator.CharData[i];
                if (!cData.isVisible || cData.visibilityState == CharData.VisibilityState.ShowAnimation || cData.visibilityState == CharData.VisibilityState.ShowAnimation) continue;

                cData.visibilityState = CharData.VisibilityState.ShowAnimation;
                mediator.CharData[i] = cData;
            }
        }
    }

    /// <summary>
    /// Hide the specified characters.
    /// </summary>
    /// <param name="start">The startindex of the characters to be hidden.</param>
    /// <param name="length">The amount of characters to be hidden, starting from <paramref name="start"/></param>
    /// <exception cref="System.ArgumentException">Throws if either <paramref name="start"/> or <paramref name="length"/> is out of bounds.</exception>
    public void Hide(int start, int length, bool skipAnimation = false)
    {
        TMP_TextInfo info = mediator.Text.textInfo;
        if (start < 0 || length < 0 || start + length > info.characterCount)
        {
            throw new System.ArgumentException("Invalid input: Start = " + start + "; Length = " + length + "; Length of string: " + info.characterCount);
        }

        CharData cData;
        TMP_CharacterInfo cInfo;
        if (skipAnimation)
        {
            Vector3[] verts;
            Color32[] colors;
            int vIndex, mIndex;

            for (int i = start; i < start + length; i++)
            {
                cInfo = info.characterInfo[i];
                if (!cInfo.isVisible) continue;

                cData = mediator.CharData[i]; 

                // Set the current mesh's vertices all to the initial mesh values
                for (int j = 0; j < 4; j++)
                {
                    cData.currentMesh.SetPosition(j, Vector3.zero);
                }

                cData.visibilityState = CharData.VisibilityState.Hidden;

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
                mediator.CharData[i] = cData;
            }

            if (mediator.Text.mesh != null)
                mediator.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
        else
        {
            for (int i = start; i < start + length; i++)
            {
                cData = mediator.CharData[i];
                if (!cData.isVisible || cData.visibilityState == CharData.VisibilityState.HideAnimation || cData.visibilityState == CharData.VisibilityState.Hidden) continue;

                cData.visibilityState = CharData.VisibilityState.HideAnimation;
                mediator.CharData[i] = cData;
            }
        }
    }
}