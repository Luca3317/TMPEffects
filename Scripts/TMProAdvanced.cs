using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// TODO new name

/*
 * The class that manages the tmpro components preprocessor
 */

public class TMProAdvanced : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMPEffectDatabase database;
    TMProEffectPreprocessor preprocessor;

    private List<CharData> charData;

    private void Awake()
    {
        preprocessor = new TMProEffectPreprocessor(database);
        SetTextPreprocessor();

        charData = new List<CharData>();
        //StartCoroutine(ChangeText());
    }


    // TODO
    // When changing text need to call forcemeshupdate and Updatetext (otherwise only gets applied on second frame)
    IEnumerator ChangeText()
    {
        yield return new WaitForSeconds(6);

        Debug.Break();
        text.text = "Hello, this is a new string. Is it <test>preprocessed</test>";
        text.ForceMeshUpdate();
        UpdateText();
        yield return null;
    }

    private void OnEnable()
    {
        text.ForceMeshUpdate();

        SetCharData();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    private void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        var info = text.textInfo;

        for (int i = 0; i < preprocessor.Tags.Count; i++)
        {
            TMPEffectTag tag = preprocessor.Tags[i];
            ITMProEffect effect = database.GetEffect(tag.name);
            if (effect == null) Debug.LogError("Tags contained tag that did not have a registered effect; sdhould not be possible");

            effect.ResetVariables();
            effect.SetParameters(tag.parameters);

            for (int j = 0; j < tag.length; j++)
            {
                CharData data = charData[tag.startIndex + j];

                // Set segment-dependent data here?
                data.segmentIndex = j;
                data.segmentLength = tag.length;

                effect.Apply(ref data);
                charData[tag.startIndex + j] = data;
            }
        }

        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();

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

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = charData[i].currentMesh[j].position;
                colors[vIndex + j] = charData[i].currentMesh[j].color;
            }

            //if (!cInfo.isVisible) continue;

            ////Debug.Log("Updating " + i + " with vindex " + vIndex);
            ////Debug.Log("Meshinfo count " + info.meshInfo.Length);
            //for (int j = 0; j < 4; j++)
            //{
            //    verts[vIndex + j] = charData[i].currentMesh[j].position;
            //    colors[vIndex + j] = charData[i].currentMesh[j].color;
            //}
        }

        for (int i = 0; i < info.characterCount; i++)
        {
            cInfo = info.characterInfo[i];
            cInfo.lineNumber += 1;
            info.characterInfo[i] = cInfo;
        }



        text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        //sw.Stop();
        //Debug.Log("Updaitn took " + sw.Elapsed.TotalMilliseconds);
    }

    void SetTextPreprocessor()
    {
        text.textPreprocessor = preprocessor;
    }

    void OnTextChanged(Object obj)
    {
        if (obj == text)
        {
            SetCharData();
        }
    }

    void SetCharData()
    {
        charData.Clear();

        TMP_TextInfo info = text.textInfo;
        CharData data;
        TMP_WordInfo? wordInfo = null;
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


        //// Is the new text longer than the old one
        //bool longer = charData.Count < info.characterCount;

        //if (longer)
        //{
        //    for (int i = 0; i < charData.Count; i++)
        //    {
        //        data = new CharData(info.characterInfo[i]);
        //        charData[i] = data;
        //    }

        //    for (int i = charData.Count; i < text.textInfo.characterCount; i++)
        //    {
        //        data = new CharData(info.characterInfo[i]);
        //        charData.Add(data);
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < text.textInfo.characterCount; i++)
        //    {
        //        data = new CharData(info.characterInfo[i]);
        //        charData[i] = data;
        //    }
        //}
    }

    void SetText(string text)
    {
        this.text.text = text;
        this.text.ForceMeshUpdate();
        UpdateText();
    }
}