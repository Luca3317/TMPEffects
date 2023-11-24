using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static log4net.Appender.ColoredConsoleAppender;

/*
 * Handles Animation tags.
 * 
 * Goal now: Allow previewing animations from editor.
 * Issues with that:
 *      For some reason mediator is not destroyed in edit mode on ondisable (even using DestroyImmediate not working)
 *      At least assuming that => data != null on reload
 *      
 *      TMPMediator is not serialized either, so all fields are reset
 *      => Either: Have to figure out the destruction of the mediator; or have separate initialization from runtime
 * 
 * 
 * Previewing
 *      1.  OnEnable: Create / get Mediator; register everything to mediator
 *      2.  OnDisable: Unregister everything from mediator
 *      3.  OnValidate: Update database if changed
 */

[ExecuteAlways]
public class TMPAnimatorNew : MonoBehaviour
{
    [SerializeField] TMPEffectsDatabase database;

    private TMPMediator data;
    private AnimationTagProcessor atp;

    private void OnEnable()
    {
#if UNITY_EDITOR
        prevDatabase = database;
        preview = false;
        prevPreview = false;
        EditorApplication.update += UpdateAnimations;
#endif

        if ((data = GetComponent<TMPMediator>()) == null)
        {
            data = TMPMediator.Create(gameObject);
            data.Initialize();
        }

#if UNITY_EDITOR
        // Manually call awake/onenable/start
        // Only for now; not compatible when having both writer and animator
        //data.Initialize();
#endif

        Debug.Log("Processor is null: " + (data.Processor == null));

        if (database == null) Debug.LogWarning("Database on " + name + " unassigned; Wont parse any animation tags");

        OnChangedDatabase();

        data.Subscribe();
        data.Processor.RegisterProcessor(ParsingUtility.NO_PREFIX, atp);
        data.Processor.FinishProcessTags += CloseOpenTags;
        data.TextChanged += UpdateAnimations;
        //data.ForceUpdateTriggered += UpdateText;

        data.ForceReprocess();
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= UpdateAnimations;
#endif
        data.Processor.FinishPreProcess -= CloseOpenTags;
        data.TextChanged -= UpdateAnimations;
        //data.ForceUpdateTriggered -= UpdateText;
        data.Processor.UnregisterProcessor(ParsingUtility.NO_PREFIX);
        data.ForceReprocess();
        data.Unsubscribe();
    }

    private void Update()
    {
        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        var info = data.Text.textInfo;

#if UNITY_EDITOR
        if (!Application.isPlaying && !preview) return;
#endif

        for (int i = 0; i < atp.ProcessedTags.Count; i++)
        {
            TMPEffectTag tag = atp.ProcessedTags[i];
            ITMPEffect effect = database.GetEffect(tag.name);
            if (effect == null) Debug.LogError("Tags contained tag that did not have a registered effect; sdhould not be possible");

            effect.ResetVariables();
            effect.SetParameters(tag.parameters);

            for (int j = 0; j < tag.length; j++)
            {
                //if (tag.startIndex + j > data.activeEndIndex) break;

                CharData cData = data.CharData[tag.startIndex + j];

                Debug.Log((tag.startIndex + j) + " Is hidden: " + cData.hidden);
                if (!cData.isVisible || cData.hidden) continue;

                // Set segment-dependent data here?
                cData.segmentIndex = j;
                cData.segmentLength = tag.length;

                effect.Apply(ref cData);
                data.CharData[tag.startIndex + j] = cData;
            }
        }

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

    void CloseOpenTags(string text)
    {
        int endIndex = text.Length - 1;
        foreach (var tag in atp.ProcessedTags)
        {
            if (tag.IsOpen) tag.Close(endIndex);
        }
    }

#if UNITY_EDITOR
    private TMPEffectsDatabase prevDatabase;
    [SerializeField] private bool preview;
    private bool prevPreview;
    //private bool prevPreview;
    private void OnValidate()
    {
        if (prevPreview != preview)
        {
            Debug.Log("yeah");
            prevPreview = preview;
            if (!preview)
                ResetAllCharacters();
        }

        if (prevDatabase != database)
        {
            prevDatabase = database;
            if (enabled)
            {
                OnChangedDatabase();
                data.ForceReprocess();
            }
        }
    }
#endif

    void OnChangedDatabase()
    {
        if (!enabled) return;
        Debug.Log("Changed database!");

        data.Processor.UnregisterProcessor(ParsingUtility.NO_PREFIX);
        atp = new AnimationTagProcessor(database);
        data.Processor.RegisterProcessor(ParsingUtility.NO_PREFIX, atp);
    }

    void ResetAllCharacters()
    {
        if (!enabled) return;
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
}


// BACKUP PRE-EDITOR PREVIEW
//public class TMPAnimatorNew : MonoBehaviour
//{
//    [SerializeField] TMPEffectsDatabase database;

//    private TMPMediator data;
//    private AnimationTagProcessor atp;

//    private void OnEnable()
//    {
//        if ((data = GetComponent<TMPMediator>()) == null)
//        {
//            data = TMPMediator.Create(gameObject);
//        }

//        atp = new AnimationTagProcessor(database);

//        data.Subscribe();
//        data.Processor.RegisterProcessor(ParsingUtility.NO_PREFIX, atp);
//        data.Processor.FinishProcessTags += CloseOpenTags;
//        data.TextChanged += UpdateAnimations;
//        //data.ForceUpdateTriggered += UpdateText;

//        data.ForceReprocess();
//    }

//    private void OnDisable()
//    {
//        data.Processor.FinishPreProcess -= CloseOpenTags;
//        data.TextChanged -= UpdateAnimations;
//        //data.ForceUpdateTriggered -= UpdateText;
//        data.Unsubscribe();

//        data.ForceReprocess();
//    }

//    private void Update()
//    {
//        UpdateAnimations();
//    }

//    void UpdateAnimations()
//    {
//        var info = data.Text.textInfo;

//        for (int i = 0; i < atp.ProcessedTags.Count; i++)
//        {
//            TMPEffectTag tag = atp.ProcessedTags[i];
//            ITMPEffect effect = database.GetEffect(tag.name);
//            if (effect == null) Debug.LogError("Tags contained tag that did not have a registered effect; sdhould not be possible");

//            effect.ResetVariables();
//            effect.SetParameters(tag.parameters);

//            for (int j = 0; j < tag.length; j++)
//            {
//                //if (tag.startIndex + j > data.activeEndIndex) break;

//                CharData cData = data.CharData[tag.startIndex + j];

//                if (!cData.isVisible || cData.hidden) continue;

//                // Set segment-dependent data here?
//                cData.segmentIndex = j;
//                cData.segmentLength = tag.length;

//                effect.Apply(ref cData);
//                data.CharData[tag.startIndex + j] = cData;
//            }
//        }

//        Vector3[] verts;
//        Color32[] colors;
//        int vIndex, mIndex;
//        TMP_CharacterInfo cInfo;

//        // TODO only update affected regions

//        // Iterate over all characters and apply the new meshes
//        for (int i = 0; i < info.characterCount; i++)
//        {
//            cInfo = info.characterInfo[i];

//            if (!cInfo.isVisible) continue;

//            vIndex = cInfo.vertexIndex;
//            mIndex = cInfo.materialReferenceIndex;

//            colors = info.meshInfo[mIndex].colors32;
//            verts = info.meshInfo[mIndex].vertices;

//            // TODO Ctrl+z for tmp text during runtime cause oob exception here
//            if (data.CharData.Count == 0)
//            {
//                Debug.LogWarning("Uninitialized chardata");
//            }

//            for (int j = 0; j < 4; j++)
//            {
//                verts[vIndex + j] = data.CharData[i].currentMesh[j].position;
//                colors[vIndex + j] = data.CharData[i].currentMesh[j].color;
//            }
//        }

//        data.Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
//    }

//    void CloseOpenTags(string text)
//    {
//        int endIndex = text.Length - 1;
//        foreach (var tag in atp.ProcessedTags)
//        {
//            if (tag.IsOpen) tag.Close(endIndex);
//        }
//    }
//}