using TMPro;
using UnityEngine;

public class TMPAnimator : MonoBehaviour
{
    [SerializeField] TMPEffectsDatabase database;

    private TMPEffectMediator data;
    private AnimationTagProcessor atp;

    private void Awake()
    {
        atp = new AnimationTagProcessor(database);
    }

    private void OnEnable()
    {
        if (data == null) data = TMPEffectMediator.Create(gameObject);

        data.Subscribe();
        data.processor.RegisterPreprocessor(ParsingUtility.NO_PREFIX, atp);
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
    }

    private void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        var info = data.text.textInfo;

        for (int i = 0; i < atp.ProcessedTags.Count; i++)
        {
            TMPEffectTag tag = atp.ProcessedTags[i];
            ITMPEffect effect = database.GetEffect(tag.name);
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

                effect.Apply(ref cData);
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
