using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * A mediator class for TMPAnimator and TMPWriter (and potential additions, if any).
 * Handles the pre- and postprocessing of the text, as well as maintaining information
 * about it in the form of a CharData collection.
 */
public class TMPMediatorFinal : MonoBehaviour
{
    public bool isInitialized => initialized;

    //public int Subscribers { get; private set; }
    public List<CharData> CharData { get; private set; }
    public TMPTextProcessor Processor { get; private set; }
    public TMP_Text Text { get; private set; }

    public delegate void EmptyEventHandler();
    public delegate void RangeEventHandler(int start, int lenght);
    public event EmptyEventHandler TextChanged;
    public event RangeEventHandler ForcedUpdate;

    [System.NonSerialized] private bool initialized = false; 

    // TODO
    // For now im using an initialize method instead of Awake/OnEnable/Start
    // to simplify using this with [ExecuteAlways] on TMPAnimator/TMPWriter
    // Issue: Initialized for some reason retains its value after recompile => nre after recompiling
    public void Initialize()
    {
        Debug.Log("Init data w/ " + initialized);
        if (initialized) return;

        initialized = true;

        subscribers = new List<object>();
        Text = GetComponent<TMP_Text>();
        CharData = new List<CharData>();
        Processor = new TMPTextProcessor();

        SetPreprocessor();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT.Add(TestingEvents);
        //ForceReprocess();
    }

    public void ForceReprocess() 
    {
        Debug.LogWarning("Force reprocess; Preprocessor set: " + (Text.textPreprocessor == Processor));
        //string tmpText = Text.text;
        //Text.SetText(" ");
        //Text.SetText(tmpText);
        Text.ForceMeshUpdate(true, true);
    } 

    public void ForceUpdate(int start, int length)
    {
        ForcedUpdate?.Invoke(start, length);
    }

    void TestingEvents(bool b, Object obj)
    {
        if ((obj as TMP_Text) == Text)
        {
            Debug.LogWarning("Called with " + b);
        }
    }

    void OnTextChanged(Object obj)
    {
        if ((obj as TMP_Text) == Text)
        {
            Debug.LogWarning("Text changed! to " + (obj as TMP_Text).text);
            TextChangedProcedure();
        }
    }

    void TextChangedProcedure()
    {
        Debug.Log("Triggered text change procedure");
        Processor.ProcessTags(Text.text, Text.GetParsedText());
        PopulateCharData();
        Debug.Log("Charactercount " + Text.textInfo.characterCount);
        TextChanged?.Invoke();
    }

    void SetPreprocessor()
    {
        Text.textPreprocessor = Processor;
    }

    void UnsetPreprocessor()
    {
        if (Text.textPreprocessor == Processor)
            Text.textPreprocessor = null;
    }

    void PopulateCharData()
    {
        CharData.Clear();

        TMP_TextInfo info = Text.textInfo;
        CharData data;
        TMP_WordInfo? wordInfo;
        for (int i = 0; i < info.characterCount; i++)
        {
            var cInfo = info.characterInfo[i];
            wordInfo = null;

            if (cInfo.isVisible)
            {
                for (int j = 0; j < info.wordCount; j++)
                {
                    wordInfo = info.wordInfo[j];
                    if (wordInfo.Value.firstCharacterIndex <= i && wordInfo.Value.lastCharacterIndex >= i)
                    {
                        break;
                    }
                }
            }

            data = wordInfo == null ? new CharData(cInfo) : new CharData(cInfo, wordInfo.Value);
            CharData.Add(data);
        }
    }

    List<object> subscribers = new List<object>();

    public void Subscribe(object obj)
    {
        if (subscribers.Contains(obj)) return;
        subscribers.Add(obj);        
        Debug.Log("Subscriber count " + subscribers.Count);
    }

    public void Unsubscribe(object obj)
    {
        if (!subscribers.Contains(obj)) return;
        subscribers.Remove(obj);

        if (subscribers.Count == 0)
        {
            // TODO Is this the right way to handle this?
            // This check is meant to prevent doubly calling destroy
            // on this component if the gameobject is destroyed
            // (it being destroyed will cause the other objects to call Unsubscribe)
            if (gameObject.activeInHierarchy)
            {
                UnsetPreprocessor();
                TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);

#if UNITY_EDITOR
                if (Application.isPlaying) Destroy(this);
                else DestroyImmediate(this);
#else
                Destroy(this);
#endif
            }
        }
    }

//    public void Subscribe() => Subscribers++;
//    public void Unsubscribe()
//    {
//        Subscribers--;
//        if (Subscribers <= 0)
//        {
//            // TODO Is this the right way to handle this?
//            // This check is meant to prevent doubly calling destroy
//            // on this component if the gameobject is destroyed
//            // (it being destroyed will cause the other objects to call Unsubscribe)
//            if (gameObject.activeInHierarchy)
//            {
//                UnsetPreprocessor();
//                TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);

//#if UNITY_EDITOR
//                if (Application.isPlaying) Destroy(this);
//                else DestroyImmediate(this);
//#else
//                Destroy(this);
//#endif
//            }
//        }
//    }
}
