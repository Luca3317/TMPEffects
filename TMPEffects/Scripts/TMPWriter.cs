using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class TMPWriter : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMPCommandDatabase database;

    [Header("Testing out Writer functionality")]
    public float lettersPerSecond = 3;

    private TMPEffectMediator data;
    private CommandTagProcessor ctp;
    private EventTagProcessor etp;

    public void Awake()
    {
        etp = new EventTagProcessor();
        ctp = new CommandTagProcessor(database);
    }

    private void OnEnable()
    {
        if (data == null) data = TMPEffectMediator.Create(gameObject);

        data.Subscribe();
        data.processor.RegisterPreprocessor('!', ctp);
        data.processor.RegisterPreprocessor('#', etp);
        data.TextChanged += StartWriterCoroutine;
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

        StartCoroutine(WriterCoroutine());
    }

    IEnumerator WriterCoroutine()
    {
        Debug.Log("Writer Coroutine Start");
        CharData cData;
        for (int i = 0; i < data.charData.Count; i++)
        {
            cData = data.charData[i];
            //cData.hidden = true;

            for (int j = 0; j < 4; j++)
            {
                cData.currentMesh.SetPosition(j, Vector3.zero);
            }
            data.charData[i] = cData;
        }

        Vector3[] verts;
        Color32[] colors;
        int vIndex, mIndex;
        TMP_CharacterInfo cInfo;

        // TODO only update affected regions

        TMP_TextInfo info = data.text.textInfo;
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
                verts[vIndex + j] = data.charData[i].currentMesh[j].position;
                colors[vIndex + j] = data.charData[i].currentMesh[j].color;
            }
        }

        data.activeEndIndex = 0;
        data.text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        yield return null;

        Debug.Log("Will show again now");
        for (int i = 0; i < data.charData.Count; i++)
        {
            cData = data.charData[i];
            cData.hidden = false;

            for (int j = 0; j < 4; j++)
            {
                cData.currentMesh.SetPosition(j, cData.initialMesh.GetPosition(j));
            }
            data.charData[i] = cData;



            cInfo = info.characterInfo[i];

            if (!cInfo.isVisible) continue;

            vIndex = cInfo.vertexIndex;
            mIndex = cInfo.materialReferenceIndex;

            colors = info.meshInfo[mIndex].colors32;
            verts = info.meshInfo[mIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = data.charData[i].currentMesh[j].position;
                colors[vIndex + j] = data.charData[i].currentMesh[j].color;
            }

            data.text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            Debug.Break();

            data.activeEndIndex = i;
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

    public void ResetWriter()
    {

    }

    public void Break()
    {

    }


    //IEnumerator WriterCoroutine()
    //{

    //}

    #region Editor
    private void Reset()
    {

    }
    #endregion
}
