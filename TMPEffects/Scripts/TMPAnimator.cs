using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class TMPAnimator : MonoBehaviour
{
    [SerializeField] TMPEffectsDatabase database;
    [SerializeField] AnimationContext context;

#if UNITY_EDITOR
    [SerializeField] bool previewAnimations = false;
    bool firstCall = true;
#endif

    private TMPEffectMediator data;
    private AnimationTagProcessor atp;

    private void Awake()
    {
        Debug.Log("Awake animator");
        atp = new AnimationTagProcessor(database);
    }

    private void OnEnable()
    {
        Debug.Log("Onenable animator");
        if (data == null) data = TMPEffectMediator.Create(gameObject);


#if UNITY_EDITOR
        prevPreviewAnimations = previewAnimations;
        EditorApplication.update += UpdateText;
        if (atp == null) atp = new AnimationTagProcessor(database);
        data.AwakeMediator();
#endif

        data.Subscribe();
        data.processor.RegisterProcessor(ParsingUtility.NO_PREFIX, atp);
        data.processor.FinishPreProcess += CloseOpenTags;
        data.TextChanged += UpdateText;
        data.ForceUpdateTriggered += UpdateText;
    }

    private void OnDisable()
    {
        data.processor.FinishPreProcess -= CloseOpenTags;
        data.TextChanged -= UpdateText;
        data.ForceUpdateTriggered -= UpdateText;
        data.Unsubscribe();

#if UNITY_EDITOR
        EditorApplication.update -= UpdateText;
        if (atp == null) atp = new AnimationTagProcessor(database);
        data.AwakeMediator();
        previewAnimations = false;
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
        //if (!Application.isPlaying && previewAnimations)
        //{
        //    if (firstCall)
        //    {
        //        firstCall = false;
        //        Awake();
        //        OnEnable();
        //        data.AwakeMediator();
        //        EditorApplication.update += UpdatePreview;
        //    }
        //}
#endif

        UpdateText();
    }

#if UNITY_EDITOR 
    bool prevPreviewAnimations = false;
    private void OnValidate()
    {
        if (prevPreviewAnimations != previewAnimations)
        {
            Debug.LogWarning("VALUE CHANGED");
            if (previewAnimations)
            {
                Debug.LogWarning("SUB");
                data.ForceUpdate();
            }
            else
            {
                Debug.LogWarning("UNSUB");
                var info = data.text.textInfo;

                for (int i = 0; i < info.characterCount; i++)
                {
                    var cInfo = info.characterInfo[i];

                    if (!cInfo.isVisible) continue;

                    int vIndex = cInfo.vertexIndex;
                    int mIndex = cInfo.materialReferenceIndex;

                    var colors = info.meshInfo[mIndex].colors32;
                    var verts = info.meshInfo[mIndex].vertices;

                    // TODO Ctrl+z for tmp text during runtime cause oob exception here
                    if (data.charData.Count == 0)
                    {
                        Debug.LogWarning("Uninitialized chardata");
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        verts[vIndex + j] = data.charData[i].initialMesh[j].position;
                        colors[vIndex + j] = data.charData[i].initialMesh[j].color;
                    }
                }

                data.text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }

            prevPreviewAnimations = previewAnimations;
        }
    }
#endif

    void UpdateText()
    {

#if UNITY_EDITOR
        Debug.Log("Called updatepreview");
        if (!Application.isPlaying && !previewAnimations) return;
        Debug.Log("WILL DO  updatepreview");
#endif

        var info = data.text.textInfo;

        for (int i = 0; i < atp.ProcessedTags.Count; i++)
        {
            TMPEffectTag tag = atp.ProcessedTags[i];
            ITMPAnimation effect = database.GetEffect(tag.name);
            if (effect == null) Debug.LogError("Tags contained tag that did not have a registered effect; sdhould not be possible");

            effect.ResetVariables();
            effect.SetParameters(tag.parameters);

            for (int j = 0; j < tag.length; j++)
            {
                if (tag.startIndex + j > data.activeEndIndex) break;

                CharData cData = data.charData[tag.startIndex + j];

                if (!cData.isVisible || cData.hidden) continue;

                // Set segment-dependent data here?
                cData.segmentIndex = j;
                cData.segmentLength = tag.length;

                effect.Animate(ref cData, context);
                data.charData[tag.startIndex + j] = cData;
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
            if (data.charData.Count == 0)
            {
                Debug.LogWarning("Uninitialized chardata");
            }

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = data.charData[i].currentMesh[j].position;
                colors[vIndex + j] = data.charData[i].currentMesh[j].color;
            }
        }

        data.text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    void CloseOpenTags(string text)
    {
        int endIndex = text.Length - 1;
        foreach (var tag in atp.ProcessedTags)
        {
            if (tag.IsOpen) tag.Close(endIndex);
        }
    }

    #region Editor
    private void Reset()
    {

    }
    #endregion
}
