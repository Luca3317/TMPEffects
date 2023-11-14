using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/*
 * Right now, handles animation, typewriter effect as well as events and commands.
 * 
 * Maybe split into multiple components:
 * TMPAnimator: Handles the animation tags
 * TMPWriter: Handles typewriter effect
 * (Something to specifically handle events and command calls? Most commands will only make sense in the context of the TMPWriter though; maybe just integrate there)
 */

public class TMPEffects : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMPEffectsDatabase database;
    TMPEffectPreProcessor preprocessor;

    private List<CharData> charData;

    public TMPEvent onEvent;

    private void Awake()
    {
        preprocessor = new TMPEffectPreProcessor(database);
        SetTextPreprocessor();

        charData = new List<CharData>();
    }

    void OnEvent()
    {
        
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

        for (int i = 0; i < preprocessor.tags.Count; i++)
        {
            TMPEffectTag tag = preprocessor.tags[i];
            ITMPEffect effect = database.GetEffect(tag.name);
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

    void OnTextChanged(UnityEngine.Object obj)
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

        foreach (var args in preprocessor.events)
        {
            Debug.Log("Event! " + args.name);
            onEvent.Invoke(args);
        }
    }

    void SetText(string text)
    {
        this.text.text = text;
        this.text.ForceMeshUpdate();
        UpdateText();
    }
}