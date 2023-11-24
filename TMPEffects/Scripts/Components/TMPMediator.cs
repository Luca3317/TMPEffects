using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMPMediator : MonoBehaviour
{
    public int Subscribers { get; private set; }
    public List<CharData> CharData { get; private set; }
    public TMPTextProcessor Processor { get; private set; }
    public TMP_Text Text { get; private set; }

    public delegate void EmptyEventHandler();
    public event EmptyEventHandler TextChanged;

    private void AwakeMANUAL()
    {
        Text = GetComponent<TMP_Text>();

        CharData = new List<CharData>();
        Processor = new TMPTextProcessor();
        //hideFlags = HideFlags.HideInInspector;
    }

    private void OnEnableMANUAL()
    {
        SetPreprocessor();
        Text.ForceMeshUpdate();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    private void StartMANUAL()
    {
        if (!string.IsNullOrWhiteSpace(Text.text))
        {
            //Text.SetText(Text.text + " ");
            //Text.ForceMeshUpdate();
            ForceReprocess();
        }
    }

    private void OnDisable()
    {
        UnsetPreprocessor();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    // TODO Force a reprocess of the text
    // Should this always immediately reprocess or just
    // set a flag that is read before the next update?
    public void ForceReprocess()
    {
        string tmpText = Text.text;
        Text.SetText(" ");
        Text.SetText(tmpText);
        Text.ForceMeshUpdate();
    }

    void OnTextChanged(Object obj)
    {
        if ((obj as TMP_Text) == Text)
        {
            TextChangedProcedure();
        }
    }

    void TextChangedProcedure()
    {
        Processor.ProcessTags(Text.text, Text.GetParsedText());
        PopulateCharData();
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

    public void Subscribe() => Subscribers++;
    public void Unsubscribe()
    {
        Subscribers--;
        if (Subscribers <= 0)
        {
            // TODO Is this the right way to handle this?
            // This check is meant to prevent doubly calling destroy
            // on this component if the gameobject is destroyed
            // (it being destroyed will cause the other objects to call Unsubscribe)
            if (gameObject.activeInHierarchy)
            {
#if UNITY_EDITOR
                if (Application.isPlaying) Destroy(this);
                else DestroyImmediate(this);
#else
                Destroy(this);
#endif
            }
        }
    }

    public static TMPMediator Create(GameObject go)
    {
        TMPMediator tem = go.GetOrAddComponent<TMPMediator>();
        //tem.hideFlags = HideFlags.HideInInspector;
        tem.Initialize();
        return tem;
    }

    public void Initialize()
    {
        AwakeMANUAL();
        OnEnableMANUAL();
        StartMANUAL();
    }
}

/* BACKU PRE EIDTOR PREVIEWE
 * 
 * 
 * public class TMPMediator : MonoBehaviour
{
    public int Subscribers { get; private set; }
    public List<CharData> CharData { get; private set; }
    public TMPTextProcessor Processor { get; private set; }
    public TMP_Text Text { get; private set; }

    public delegate void EmptyEventHandler();
    public event EmptyEventHandler TextChanged;

    private void Awake()
    {
        Text = GetComponent<TMP_Text>();

        CharData = new List<CharData>();
        Processor = new TMPTextProcessor();
        hideFlags = HideFlags.HideInInspector;
    }

    private void OnEnable()
    {
        SetPreprocessor();
        Text.ForceMeshUpdate();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    private void Start()
    {
        if (!string.IsNullOrWhiteSpace(Text.text))
        {
            Text.SetText(Text.text + " ");
            Text.ForceMeshUpdate();
        }
    }

    private void OnDisable()
    {
        UnsetPreprocessor();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    void OnTextChanged(Object obj)
    {
        if ((obj as TMP_Text) == Text)
        {
            TextChangedProcedure();
        }
    }

    void TextChangedProcedure()
    {
        Processor.ProcessTags(Text.text, Text.GetParsedText());
        PopulateCharData();
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

    public void Subscribe() => Subscribers++;
    public void Unsubscribe()
    {
        Subscribers--;
        if (Subscribers <= 0)
        {
            Debug.Log("KMS");
            Destroy(this);
        }
    }

    public static TMPMediator Create(GameObject go)
    {
        TMPMediator tem = go.GetOrAddComponent<TMPMediator>();
        tem.hideFlags = HideFlags.HideInInspector;
        return tem;
    }
}
*/
