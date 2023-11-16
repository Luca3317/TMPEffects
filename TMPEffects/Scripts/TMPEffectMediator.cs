using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * TODO Better name
 * 
 * This class manages the processing of the TMP_Text
 * 
 *  -Set / unset processor
 *  -Reprocess when text changed
 *  -Maintain list of chardata
 *  -ForceMeshUpdate / UpdateVertexData when necessary
 */

[DisallowMultipleComponent]
public class TMPEffectMediator : MonoBehaviour
{
    public List<CharData> charData { get; private set; }

    public TMPTextProcessor processor = new TMPTextProcessor();
    //public TMPEffectPreProcessor preProcessor { get; private set; }



    public TMP_Text text { get; private set; }

    public delegate void EmptyEventHandler();
    public event EmptyEventHandler TextChanged;
    public event EmptyEventHandler ForceUpdateTriggered;

    public delegate void CharDataEventHandler(ref CharData data);
    public event CharDataEventHandler CharacterShown;

    public int activeStartIndex;
    public int activeEndIndex;


    // Subscribers are objects that rely on this object (atm: TMPWriter and TMPAnimator)
    // If this value reaches 0 destroy yourself
    public int Subscribers { get; private set; }

    private void Awake()
    {
        text = GetComponent<TMP_Text>();

        charData = new List<CharData>();
        processor = new TMPTextProcessor();
        hideFlags = HideFlags.HideInInspector;

        StartCoroutine(ChangeText());
    }

    private void OnEnable()
    {
        SetPreprocessor();
        text.ForceMeshUpdate();
        SetCharData();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    private void OnDisable()
    {
        UnsetPreprocessor();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }




    /*
     * How i will likely handle this:
     *      The preprocessor only removes; this should only ever by necessary for editor stuff; normally you set the text without depending on the previous input
     *      so it doesnt matter if the tag strings are discarded after
     *      
     *      Then: postprocessor
     *      
     * 
     * 
     */

    void OnTextChanged(UnityEngine.Object obj)
    {
        if ((obj as TMP_Text) == text)
        {

            // TODO
            // Right now the concept is:
            // Processor contains a PreProcessor and a (Post)Processor
            // Preprocessor removes all custom tags
            // Processor takes parsed and raw text, and calculates the custom
            // tags' indeces within the parsed text.
            //
            // Much easier solution would be to get rid of the preprocessor
            // and directly calculate the indeces in the parsed text,
            // which would now also contain the custom tags.
            // Advantages:
            //      Much easier to implement;
            //      (slightly) more performant;
            //      <noparse> implementation more stable
            // Disadvantages:
            //      Will have to set text twice every time (not a big deal);
            //      Custom tags will be stripped from the raw text - cant modify an applied text
            //
            // Maybe: Dual solution; use the current method for editor work and the other one during runtime
            processor.Process(text.text, text.GetParsedText());

            activeStartIndex = 0;
            activeEndIndex = text.textInfo.characterCount - 1;
            SetCharData();
            TextChanged?.Invoke();

        }
    }

    public void SetText(string text)
    {
    }

    // Test for ensuring texteffects are immediately updated
    IEnumerator ChangeText()
    {
        string newText = "A NEW Text! <wave><color=green><test>Wooo</test></wave>A NEW Text! <wave><test>Wooo</test></wave>A NEW Text! <wave><test>Wooo</test></wave>";
        yield return new WaitForSeconds(0.1f);

        text.text = newText;
        text.ForceMeshUpdate();
        //Debug.Break();
        yield return null;
    }

    public void SetCharData()
    {
        charData.Clear();

        TMP_TextInfo info = text.textInfo;
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
            charData.Add(data);
        }
    }

    void SetPreprocessor()
    {
        TextPreprocessorStack stack;

        if (text.textPreprocessor == processor) return;
        if (text.textPreprocessor == null)
        {
            stack = new TextPreprocessorStack();
            stack.AddPreprocessor(processor);
            text.textPreprocessor = stack;
            return;
        }
        if (text.textPreprocessor is TextPreprocessorStack)
        {
            (text.textPreprocessor as TextPreprocessorStack).AddPreprocessor(processor);
            return;
        }

        ITextPreprocessor old = text.textPreprocessor;
        stack = new TextPreprocessorStack();
        stack.AddPreprocessor(old);
        stack.AddPreprocessor(processor);
        text.textPreprocessor = stack;
    }

    void UnsetPreprocessor()
    {
        if (text.textPreprocessor == null) return;
        if (text.textPreprocessor is TextPreprocessorStack)
        {
            (text.textPreprocessor as TextPreprocessorStack).RemovePreprocessor(processor);
            return;
        }
        if (text.textPreprocessor == processor)
        {
            text.textPreprocessor = null;
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

    public void ForceUpdate()
    {
        ForceUpdateTriggered?.Invoke();
    }

    public static TMPEffectMediator Create(GameObject go)
    {
        TMPEffectMediator tem = go.GetOrAddComponent<TMPEffectMediator>();
        tem.hideFlags = HideFlags.HideInInspector;
        return tem;
    }
}



///*
// * TODO Better name
// * 
// * This class manages the processing of the TMP_Text
// * 
// *  -Set / unset processor
// *  -Reprocess when text changed
// *  -Maintain list of chardata
// *  -ForceMeshUpdate / UpdateVertexData when necessary
// */

//[DisallowMultipleComponent]
//public class TMPEffectMediator : MonoBehaviour
//{
//    public List<CharData> charData { get; private set; }
//    public TMPEffectPreProcessor preProcessor { get; private set; }
//    public TMP_Text text { get; private set; }

//    public delegate void EmptyEventHandler();
//    public event EmptyEventHandler TextChanged;
//    public event EmptyEventHandler ForceUpdateTriggered;

//    public delegate void CharDataEventHandler(ref CharData data);
//    public event CharDataEventHandler CharacterShown;

//    public int activeStartIndex;
//    public int activeEndIndex;


//    // Subscribers are objects that rely on this object (atm: TMPWriter and TMPAnimator)
//    // If this value reaches 0 destroy yourself
//    public int Subscribers { get; private set; }

//    private void Awake()
//    {
//        text = GetComponent<TMP_Text>();

//        charData = new List<CharData>();
//        preProcessor = new TMPEffectPreProcessor();
//        hideFlags = HideFlags.HideInInspector;

//        StartCoroutine(ChangeText());
//    }

//    private void OnEnable()
//    {
//        SetPreprocessor();
//        text.ForceMeshUpdate();
//        SetCharData();
//        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
//    }

//    private void OnDisable()
//    {
//        UnsetPreprocessor();
//        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
//    }




//    /*
//     * How i will likely handle this:
//     *      The preprocessor only removes; this should only ever by necessary for editor stuff; normally you set the text without depending on the previous input
//     *      so it doesnt matter if the tag strings are discarded after
//     *      
//     *      Then: postprocessor
//     *      
//     * 
//     * 
//     */


//    void OnTextChanged(UnityEngine.Object obj)
//    {
//        if ((obj as TMP_Text) == text)
//        {
//            int parsedIndex = 0;
//            int rawIndex = 0;

//            Debug.Log("Text changed");
//            Debug.Log("Parsed: " +  text.GetParsedText());
//            Debug.Log("Parsed: " +  text.text);



//            for (; rawIndex < text.text.Length; rawIndex++)
//            {
//                if (text.text[rawIndex] == '<')
//                {
//                    // If is not present in parsed version, this is a native tag; continue
//                    // Else test for custom tag
//                }

//            }




//            activeStartIndex = 0;
//            activeEndIndex = text.textInfo.characterCount - 1;
//            SetCharData();
//            TextChanged?.Invoke();
//        }
//    }

//    // Test for ensuring texteffects are immediately updated
//    IEnumerator ChangeText()
//    {
//        string newText = "A NEW Text! <wave><color=green><test>Wooo</test></wave>A NEW Text! <wave><test>Wooo</test></wave>A NEW Text! <wave><test>Wooo</test></wave>";
//        yield return new WaitForSeconds(4);
//        text.text = newText;
//        text.ForceMeshUpdate();
//        //Debug.Break();
//        yield return null;
//    }

//    public void SetCharData()
//    {
//        charData.Clear();

//        TMP_TextInfo info = text.textInfo;
//        CharData data;
//        TMP_WordInfo? wordInfo;
//        for (int i = 0; i < info.characterCount; i++)
//        {
//            var cInfo = info.characterInfo[i];
//            wordInfo = null;

//            if (cInfo.isVisible)
//            {
//                for (int j = 0; j < info.wordCount; j++)
//                {
//                    wordInfo = info.wordInfo[j];
//                    if (wordInfo.Value.firstCharacterIndex <= i && wordInfo.Value.lastCharacterIndex >= i)
//                    {
//                        break;
//                    }
//                }
//            }

//            data = wordInfo == null ? new CharData(cInfo) : new CharData(cInfo, wordInfo.Value);
//            charData.Add(data);
//        }
//    }

//    void SetPreprocessor()
//    {
//        TextPreprocessorStack stack;

//        if (text.textPreprocessor == preProcessor) return;
//        if (text.textPreprocessor == null)
//        {
//            stack = new TextPreprocessorStack();
//            stack.AddPreprocessor(preProcessor);
//            text.textPreprocessor = stack;
//            return;
//        }
//        if (text.textPreprocessor is TextPreprocessorStack)
//        {
//            (text.textPreprocessor as TextPreprocessorStack).AddPreprocessor(preProcessor);
//            return;
//        }

//        ITextPreprocessor old = text.textPreprocessor;
//        stack = new TextPreprocessorStack();
//        stack.AddPreprocessor(old);
//        stack.AddPreprocessor(preProcessor);
//        text.textPreprocessor = stack;
//    }

//    void UnsetPreprocessor()
//    {
//        if (text.textPreprocessor == null) return;
//        if (text.textPreprocessor is TextPreprocessorStack)
//        {
//            (text.textPreprocessor as TextPreprocessorStack).RemovePreprocessor(preProcessor);
//            return;
//        }
//        if (text.textPreprocessor == preProcessor)
//        {
//            text.textPreprocessor = null;
//        }
//    }

//    public void Subscribe() => Subscribers++;
//    public void Unsubscribe()
//    {
//        Subscribers--;
//        if (Subscribers <= 0)
//        {
//            Debug.Log("KMS");
//            Destroy(this);
//        }
//    }

//    public void ForceUpdate()
//    {
//        ForceUpdateTriggered?.Invoke();
//    }

//    public static TMPEffectMediator Create(GameObject go)
//    {
//        TMPEffectMediator tem = go.GetOrAddComponent<TMPEffectMediator>();
//        tem.hideFlags = HideFlags.HideInInspector;
//        return tem;
//    }
//}
