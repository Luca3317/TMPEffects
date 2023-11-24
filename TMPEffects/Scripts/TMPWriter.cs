using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static System.Net.WebRequestMethods;

/*
 * TODO
 * Strikethrough, underline and highlight will be instantly shown atm
 * For strikethrough (and most likely underline as well):
 *      Each strikethrough consists of 12 vertices
 *      You can get the strikethrough that is active at a character though CharacterInfo.strikeTrhoughVertexIndex
 *      => When iterating over the characters in the beginning of the writer loop, save unique strikethrougvertexIndeces as well and hide the strikethroughs (set to V3.zero)
 *      Update in loop based on the latest shown character
 *      
 * Highlight consist of vertices too, how to get index?
 */

public class TMPWriter : MonoBehaviour
{

    [Header("Settings"), Range(0, 10)]
    [SerializeField] float speed;

    private float currentSpeed;

    [Header("References")]
    [SerializeField] TMP_Text text;
    [SerializeField] TMPCommandDatabase database;

    private TMPMediator data;
    private CommandTagProcessor ctp;
    private EventTagProcessor etp;

    public void Awake()
    {
        etp = new EventTagProcessor();
        ctp = new CommandTagProcessor(database);

        // TODO Should this value only be initialized on Awake,
        // on Enable or every time StartWriterCoroutine is called?
        currentSpeed = speed;
    }

    private void OnEnable()
    {
        if ((data = GetComponent<TMPMediator>()) == null)
        {
            Debug.Log("TMPWriter initializes data");
            data = TMPMediator.Create(gameObject);
            data.Initialize();
        }

        data.Subscribe();
        data.Processor.RegisterProcessor('!', ctp);
        data.Processor.RegisterProcessor('#', etp);
        data.TextChanged += StartWriterCoroutine;

        data.ForceReprocess();
        StartWriterCoroutine();
    }

    private void OnDisable()
    {
        data.Unsubscribe();
        data.TextChanged -= StartWriterCoroutine;
    }

    Coroutine writerCoroutine = null;

    void StartWriterCoroutine()
    {
        if (writerCoroutine != null)
        {
            StopCoroutine(writerCoroutine);
            writerCoroutine = null;
        }

        writerCoroutine = StartCoroutine(WriterCoroutine2());
    }


    /*
     * The writer coroutine. Stop and restart when text is changed.
     * 
     * 1. Hide all characters. Update chardata accordingly.
     * 2. Iterate over all characters. This is the main loop.
     *      2.1 If wait flag set => wait for specified amount
     *      2.2 Show next character (if not whitespace)
     *      2.3 Update chardata accordingly
     *      2.4 Update Vertex data
     */
    IEnumerator WriterCoroutine2()
    {
        Debug.Log("Writer Coroutine");

        // Reset all relevant variables
        currentSpeed = speed;

        TMP_TextInfo info = data.Text.textInfo;
        TMP_CharacterInfo cInfo;
        CharData cData;
        Vector3[] verts;
        Color32[] colors;
        int vIndex, mIndex;

        HideAllCharacters();
        // ForceMeshUpdate?
        //yield return null;

        //if (ctp.ProcessedTags.Count == 0)
        //{
        //    Debug.Log("I aint got NO commands");
        //}
        //for (int i = 0; i < ctp.ProcessedTags.Count; i++)
        //{
        //    Debug.Log("Registered command:  " + ctp.ProcessedTags[i].name);
        //}

        Debug.Log(ctp.ProcessedTags.Count + " commands");
        Debug.Log(etp.ProcessedTags.Count + " events");

        for (int i = 0; i < info.characterCount; i++)
        {
            Debug.Log("Doing " + i + " out of " + info.characterCount);
            // TODO support scaled + unscaled time
            if (shouldWait)
            {
                shouldWait = false;
                yield return new WaitForSeconds(waitAmount);
            }

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
                    Debug.Log("Raising command " + ctp.ProcessedTags[j].name);
                    //database.GetCommand(ctp.ProcessedTags[j].name).ExecuteCommand(ctp.ProcessedTags[j], this);
                }
            }
            for (int j = 0; j < etp.ProcessedTags.Count; j++)
            {
                if (etp.ProcessedTags[j].index == i)
                {
                    Debug.Log("TODO Raise event " + etp.ProcessedTags[j].name);
                }
            }

            cInfo = info.characterInfo[i];
            //data.activeEndIndex = i;
            if (!cInfo.isVisible) continue;

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

            //data.activeEndIndex = i;
            if (currentSpeed > 0)
            {
                Debug.Log("The speed is calced with " + currentSpeed + "; the result is " + ((1f / currentSpeed) * 0.1f));
                yield return new WaitForSeconds((1f / currentSpeed) * 0.1f);
            }
        }

        Debug.Log("Broke out");
    }

    bool shouldWait = false;
    float waitAmount = 0f;
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


    //IEnumerator WriterCoroutine()
    //{
    //    TMP_TextInfo info = data.text.textInfo;
    //    TMP_CharacterInfo cInfo;
    //    CharData cData;
    //    Vector3[] verts;
    //    Color32[] colors;
    //    int vIndex, mIndex;

    //    HideAllCharacters();

    //    yield return null;

    //    for (int i = 0; i < data.charData.Count; i++)
    //    {
    //        cData = data.charData[i];
    //        cData.hidden = false;

    //        for (int j = 0; j < 4; j++)
    //        {
    //            cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
    //        }
    //        data.charData[i] = cData;

    //        cInfo = info.characterInfo[i];

    //        if (!cInfo.isVisible) continue;

    //        vIndex = cInfo.vertexIndex;
    //        mIndex = cInfo.materialReferenceIndex;

    //        colors = info.meshInfo[mIndex].colors32;
    //        verts = info.meshInfo[mIndex].vertices;

    //        for (int j = 0; j < 4; j++)
    //        {
    //            verts[vIndex + j] = data.charData[i].currentMesh[j].position;
    //            colors[vIndex + j] = data.charData[i].currentMesh[j].color;
    //        }

    //        data.text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

    //        data.activeEndIndex = i;
    //        yield return new WaitForSeconds(0.1f);
    //    }

    //    yield return null;
    //}    //IEnumerator WriterCoroutine()
    //{
    //    TMP_TextInfo info = data.text.textInfo;
    //    TMP_CharacterInfo cInfo;
    //    CharData cData;
    //    Vector3[] verts;
    //    Color32[] colors;
    //    int vIndex, mIndex;

    //    HideAllCharacters();

    //    yield return null;

    //    for (int i = 0; i < data.charData.Count; i++)
    //    {
    //        cData = data.charData[i];
    //        cData.hidden = false;

    //        for (int j = 0; j < 4; j++)
    //        {
    //            cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
    //        }
    //        data.charData[i] = cData;

    //        cInfo = info.characterInfo[i];

    //        if (!cInfo.isVisible) continue;

    //        vIndex = cInfo.vertexIndex;
    //        mIndex = cInfo.materialReferenceIndex;

    //        colors = info.meshInfo[mIndex].colors32;
    //        verts = info.meshInfo[mIndex].vertices;

    //        for (int j = 0; j < 4; j++)
    //        {
    //            verts[vIndex + j] = data.charData[i].currentMesh[j].position;
    //            colors[vIndex + j] = data.charData[i].currentMesh[j].color;
    //        }

    //        data.text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

    //        data.activeEndIndex = i;
    //        yield return new WaitForSeconds(0.1f);
    //    }

    //    yield return null;
    //}

    void HideAllCharacters()
    {
        TMP_TextInfo info = data.Text.textInfo;
        CharData cData;
        for (int i = 0; i < data.CharData.Count; i++)
        {
            cData = data.CharData[i];

            for (int j = 0; j < 4; j++)
            {
                cData.currentMesh.SetPosition(j, Vector3.zero);
            }

            data.CharData[i] = cData;
        }

        Vector3[] verts;
        Color32[] colors;
        int vIndex, mIndex;
        TMP_CharacterInfo cInfo;

        // Iterate over all characters and apply the new meshes
        for (int i = 0; i < info.characterCount; i++)
        {
            cInfo = info.characterInfo[i];

            if (!cInfo.isVisible) continue;

            vIndex = cInfo.vertexIndex;
            mIndex = cInfo.materialReferenceIndex;

            colors = info.meshInfo[mIndex].colors32;
            verts = info.meshInfo[mIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = data.CharData[i].currentMesh[j].position;
                colors[vIndex + j] = data.CharData[i].currentMesh[j].color;
            }
        }

        //data.activeStartIndex = 0;
        //data.activeEndIndex = 0;
        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    void OnChangedDatabase()
    {
        Debug.Log("Changed database!");
        //data.Processor.UnregisterProcessor('#');
        data.Processor.UnregisterProcessor('!');
        //etp = new EventTagProcessor();
        ctp = new CommandTagProcessor(database);
        //data.Processor.RegisterProcessor('#', etp);
        data.Processor.RegisterProcessor('!', ctp);
    }

    void ResetAllCharacters()
    {
        var info = data.Text.textInfo;
        Debug.Log("Resetting all character charactercount " + info.characterCount);

        Vector3[] verts;
        Color32[] colors;
        int vIndex, mIndex;
        TMP_CharacterInfo cInfo;

        // Iterate over all characters and apply the new meshes
        for (int i = 0; i < info.characterCount; i++)
        {
            cInfo = info.characterInfo[i];
            if (!cInfo.isVisible) continue;

            vIndex = cInfo.vertexIndex;
            mIndex = cInfo.materialReferenceIndex;

            colors = info.meshInfo[mIndex].colors32;
            verts = info.meshInfo[mIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                // TODO Also reset current mesh to initialMesh

                verts[vIndex + j] = data.CharData[i].initialMesh[j].position;
                colors[vIndex + j] = data.CharData[i].initialMesh[j].color;
            }
        }

        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    #region Editor
    private void Reset()
    {

    }
    #endregion
}


// BACK UP PRE EDITOR
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;

///*
// * TODO
// * Strikethrough, underline and highlight will be instantly shown atm
// * For strikethrough (and most likely underline as well):
// *      Each strikethrough consists of 12 vertices
// *      You can get the strikethrough that is active at a character though CharacterInfo.strikeTrhoughVertexIndex
// *      => When iterating over the characters in the beginning of the writer loop, save unique strikethrougvertexIndeces as well and hide the strikethroughs (set to V3.zero)
// *      Update in loop based on the latest shown character
// *      
// * Highlight consist of vertices too, how to get index?
// */

//public class TMPWriter : MonoBehaviour
//{
//    [Header("Settings"), Range(0, 10)]
//    [SerializeField] float speed;

//    private float currentSpeed;

//    [Header("References")]
//    [SerializeField] TMP_Text text;
//    [SerializeField] TMPCommandDatabase database;

//    private TMPEffectMediator data;
//    private CommandTagProcessor ctp;
//    private EventTagProcessor etp;

//    public void Awake()
//    {
//        etp = new EventTagProcessor();
//        ctp = new CommandTagProcessor(database);

//        // TODO Should this value only be initialized on Awake,
//        // on Enable or every time StartWriterCoroutine is called?
//        currentSpeed = speed;
//    }

//    private void OnEnable()
//    {
//        if (data == null) data = TMPEffectMediator.Create(gameObject);

//        data.Subscribe();
//        data.processor.RegisterProcessor('!', ctp);
//        data.processor.RegisterProcessor('#', etp);
//        data.TextChanged += StartWriterCoroutine;
//    }

//    private void OnDisable()
//    {
//        data.Unsubscribe();
//        data.TextChanged -= StartWriterCoroutine;
//    }

//    Coroutine writerCoroutine = null;

//    void StartWriterCoroutine()
//    {
//        if (writerCoroutine != null)
//        {
//            StopCoroutine(writerCoroutine);
//            writerCoroutine = null;
//        }

//        writerCoroutine = StartCoroutine(WriterCoroutine2());
//    }


//    /*
//     * The writer coroutine. Stop and restart when text is changed.
//     * 
//     * 1. Hide all characters. Update chardata accordingly.
//     * 2. Iterate over all characters. This is the main loop.
//     *      2.1 If wait flag set => wait for specified amount
//     *      2.2 Show next character (if not whitespace)
//     *      2.3 Update chardata accordingly
//     *      2.4 Update Vertex data
//     */
//    IEnumerator WriterCoroutine2()
//    {
//        TMP_TextInfo info = data.text.textInfo;
//        TMP_CharacterInfo cInfo;
//        CharData cData;
//        Vector3[] verts;
//        Color32[] colors;
//        int vIndex, mIndex;

//        HideAllCharacters();
//        // ForceMeshUpdate?
//        //yield return null;

//        //if (ctp.ProcessedTags.Count == 0)
//        //{
//        //    Debug.Log("I aint got NO commands");
//        //}
//        //for (int i = 0; i < ctp.ProcessedTags.Count; i++)
//        //{
//        //    Debug.Log("Registered command:  " + ctp.ProcessedTags[i].name);
//        //}

//        for (int i = 0; i < info.characterCount; i++)
//        {
//            Debug.Log("Doing " + i + " out of " + info.characterCount);
//            // TODO support scaled + unscaled time
//            if (shouldWait)
//            {
//                shouldWait = false;
//                yield return new WaitForSeconds(waitAmount);
//            }

//            cData = data.charData[i];
//            cData.hidden = false;

//            for (int j = 0; j < 4; j++)
//            {
//                cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
//            }
//            data.charData[i] = cData;


//            // Raise any events or comamnds associated with the current index
//            for (int j = 0; j < ctp.ProcessedTags.Count; j++)
//            {
//                if (ctp.ProcessedTags[j].index == i)
//                {
//                    Debug.Log("Raising command " + ctp.ProcessedTags[j].name);
//                    database.GetCommand(ctp.ProcessedTags[j].name).ExecuteCommand(ctp.ProcessedTags[j], this);
//                }
//            }
//            for (int j = 0; j < etp.ProcessedTags.Count; j++)
//            {
//                if (etp.ProcessedTags[j].index == i)
//                {
//                    Debug.Log("TODO Raise event " + etp.ProcessedTags[j].name);
//                }
//            }

//            cInfo = info.characterInfo[i];
//            data.activeEndIndex = i;
//            if (!cInfo.isVisible) continue;

//            vIndex = cInfo.vertexIndex;
//            mIndex = cInfo.materialReferenceIndex;

//            colors = info.meshInfo[mIndex].colors32;
//            verts = info.meshInfo[mIndex].vertices;

//            for (int j = 0; j < 4; j++)
//            {
//                verts[vIndex + j] = data.charData[i].currentMesh[j].position;
//                colors[vIndex + j] = data.charData[i].currentMesh[j].color;
//            }

//            data.text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

//            data.activeEndIndex = i;
//            if (currentSpeed > 0)
//            {
//                Debug.Log("The speed is calced with " + currentSpeed + "; the result is " + ((1f / currentSpeed) * 0.1f));
//                yield return new WaitForSeconds((1f / currentSpeed) * 0.1f);
//            }
//        }

//        Debug.Log("Broke out");
//    }

//    bool shouldWait = false;
//    float waitAmount = 0f;
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


//    IEnumerator WriterCoroutine()
//    {
//        TMP_TextInfo info = data.text.textInfo;
//        TMP_CharacterInfo cInfo;
//        CharData cData;
//        Vector3[] verts;
//        Color32[] colors;
//        int vIndex, mIndex;

//        HideAllCharacters();

//        yield return null;

//        for (int i = 0; i < data.charData.Count; i++)
//        {
//            cData = data.charData[i];
//            cData.hidden = false;

//            for (int j = 0; j < 4; j++)
//            {
//                cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
//            }
//            data.charData[i] = cData;

//            cInfo = info.characterInfo[i];

//            if (!cInfo.isVisible) continue;

//            vIndex = cInfo.vertexIndex;
//            mIndex = cInfo.materialReferenceIndex;

//            colors = info.meshInfo[mIndex].colors32;
//            verts = info.meshInfo[mIndex].vertices;

//            for (int j = 0; j < 4; j++)
//            {
//                verts[vIndex + j] = data.charData[i].currentMesh[j].position;
//                colors[vIndex + j] = data.charData[i].currentMesh[j].color;
//            }

//            data.text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

//            data.activeEndIndex = i;
//            yield return new WaitForSeconds(0.1f);
//        }

//        yield return null;
//    }

//    void HideAllCharacters()
//    {
//        TMP_TextInfo info = data.text.textInfo;
//        CharData cData;
//        for (int i = 0; i < data.charData.Count; i++)
//        {
//            cData = data.charData[i];

//            for (int j = 0; j < 4; j++)
//            {
//                cData.currentMesh.SetPosition(j, Vector3.zero);
//            }

//            data.charData[i] = cData;
//        }

//        Vector3[] verts;
//        Color32[] colors;
//        int vIndex, mIndex;
//        TMP_CharacterInfo cInfo;

//        // Iterate over all characters and apply the new meshes
//        for (int i = 0; i < info.characterCount; i++)
//        {
//            cInfo = info.characterInfo[i];

//            if (!cInfo.isVisible) continue;

//            vIndex = cInfo.vertexIndex;
//            mIndex = cInfo.materialReferenceIndex;

//            colors = info.meshInfo[mIndex].colors32;
//            verts = info.meshInfo[mIndex].vertices;

//            for (int j = 0; j < 4; j++)
//            {
//                verts[vIndex + j] = data.charData[i].currentMesh[j].position;
//                colors[vIndex + j] = data.charData[i].currentMesh[j].color;
//            }
//        }

//        data.activeStartIndex = 0;
//        data.activeEndIndex = 0;
//        data.text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
//    }


//    void ShowCharacter(ref CharData cData)
//    {

//    }




//    #region Editor
//    private void Reset()
//    {

//    }
//    #endregion
//}
