using System.Collections.Generic;
using TMPEffects;
using TMPEffects.TextProcessing;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.TextCore;

[CreateAssetMenu(fileName = "new CharAnimation", menuName = "TMPEffects/Animations/Char")]
public class CharAnimation : TMPAnimation
{
    [SerializeField] string characters;
    [SerializeField] int maxUnitSize;
    [SerializeField] int minUnitSize;
    [SerializeField] float maxWait;
    [SerializeField] float minWait;
    [SerializeField] float originalChance;

    [System.NonSerialized] string currentCharacters;
    [System.NonSerialized] int currentMaxUnitSize;
    [System.NonSerialized] int currentMinUnitSize;
    [System.NonSerialized] float currentMaxWait;
    [System.NonSerialized] float currentMinWait;
    [System.NonSerialized] float currentOriginalChance;

    // Potential TODO
    // Use min / max unit size
    // order in which characters are read: random, loop, pingpong
    // if using units: allow for multi chars in current characters    

    public override void Animate(ref CharData cData, ref IAnimationContext context)
    {
        if (string.IsNullOrWhiteSpace(currentCharacters)) return;

        TMP_Character c;
        CharAnimContext ctx = context as CharAnimContext;
        if (!ctx.waitTimeRemaining.ContainsKey(cData.segmentIndex)) ctx.waitTimeRemaining[cData.segmentIndex] = 0;
        if (!ctx.positions.ContainsKey(cData.segmentIndex))
        {
            cData.info.fontAsset.characterLookupTable.TryGetValue(cData.info.character, out c);
            ctx.positions[cData.segmentIndex] = c.glyph.glyphRect;
        }

        if (!ctx.init)
        {
            if (cData.info.fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic)
                cData.info.fontAsset.TryAddCharacters(currentCharacters);
            ctx.init = true;
        }

        ctx.waitTimeRemaining[cData.segmentIndex] = ctx.waitTimeRemaining[cData.segmentIndex] - context.animatorContext.deltaTime;

        float h = cData.info.fontAsset.atlasHeight;
        float w = cData.info.fontAsset.atlasWidth;

        // If still waiting
        if (ctx.waitTimeRemaining[cData.segmentIndex] > 0)
        {
            GlyphRect rect = ctx.positions[cData.segmentIndex];
            cData.mesh.SetUV(0, new Vector2(rect.x / w, rect.y / h));
            cData.mesh.SetUV(1, new Vector2(rect.x / w, (rect.y + rect.height) / h));
            cData.mesh.SetUV(2, new Vector2((rect.x + rect.width) / w, (rect.y + rect.height) / h));
            cData.mesh.SetUV(3, new Vector2((rect.x + rect.width) / w, rect.y / h));
            return;
        }

        if (ctx.random.NextDouble() < currentOriginalChance)
        {
            cData.info.fontAsset.characterLookupTable.TryGetValue(cData.info.character, out c);
            GlyphRect rect = c.glyph.glyphRect;
            cData.mesh.SetUV(0, new Vector2(rect.x / w, rect.y / h));
            cData.mesh.SetUV(1, new Vector2(rect.x / w, (rect.y + rect.height) / h));
            cData.mesh.SetUV(2, new Vector2((rect.x + rect.width) / w, (rect.y + rect.height) / h));
            cData.mesh.SetUV(3, new Vector2((rect.x + rect.width) / w, rect.y / h));
            ctx.positions[cData.segmentIndex] = rect;
        }
        else
        {
            int index = ctx.random.Next(0, currentCharacters.Length);
            bool t = cData.info.fontAsset.characterLookupTable.TryGetValue(currentCharacters[index], out c);

            if (t)
            {
                GlyphRect rect = c.glyph.glyphRect;
                cData.mesh.SetUV(0, new Vector2(rect.x / w, rect.y / h));
                cData.mesh.SetUV(1, new Vector2(rect.x / w, (rect.y + rect.height) / h));
                cData.mesh.SetUV(2, new Vector2((rect.x + rect.width) / w, (rect.y + rect.height) / h));
                cData.mesh.SetUV(3, new Vector2((rect.x + rect.width) / w, rect.y / h));
                ctx.positions[cData.segmentIndex] = rect;
            }
        }

        ctx.waitTimeRemaining[cData.segmentIndex] = Mathf.Lerp(currentMinWait, currentMaxWait, (float)ctx.random.NextDouble());
    }

    public override void ResetParameters()
    {
        currentOriginalChance = originalChance;
        currentCharacters = characters;
        currentMaxUnitSize = maxUnitSize;
        currentMinUnitSize = minUnitSize;
        currentMaxWait = maxWait;
        currentMinWait = minWait;
    }

    public override void SetParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null) return;

        foreach (KeyValuePair<string, string> pair in parameters)
        {
            switch (pair.Key)
            {
                case "maxunitsize":
                case "maxus":
                case "maxu":
                    ParsingUtility.StringToInt(pair.Value, out currentMaxUnitSize);
                    break;
                case "minunitsize":
                case "minus":
                case "minu":
                    ParsingUtility.StringToInt(pair.Value, out currentMinUnitSize);
                    break;

                case "maxwait":
                case "maxw":
                    ParsingUtility.StringToFloat(pair.Value, out currentMaxWait);
                    break;

                case "minwait":
                case "minw":
                    ParsingUtility.StringToFloat(pair.Value, out currentMinWait);
                    break;

                case "cs":
                case "chars":
                case "string:":
                case "str":
                    currentCharacters = pair.Value;
                    break;

                case "original":
                case "chance":
                case "originalchance":
                case "oc":
                case "ogc":
                case "ogchance":
                    ParsingUtility.StringToFloat(pair.Value, out currentOriginalChance);
                    Mathf.Clamp(currentOriginalChance, 0, 1);
                    break;
            }
        }
    }

    public override bool ValidateParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null) return true;

        foreach (KeyValuePair<string, string> pair in parameters)
        {
            switch (pair.Key)
            {
                case "maxunitsize":
                case "maxus":
                case "maxu":
                case "minunitsize":
                case "minus":
                case "minu":
                    if (!ParsingUtility.StringToInt(pair.Value, out _)) return false;
                    break;

                case "maxwait":
                case "maxw":
                case "minwait":
                case "minw":
                case "original":
                case "chance":
                case "originalchance":
                case "oc":
                case "ogc":
                case "ogchance":
                    if (!ParsingUtility.StringToFloat(pair.Value, out _)) return false;
                    break;
            }
        }

        return true;
    }

    public override IAnimationContext GetNewContext()
    {
        return new CharAnimContext() { waitTimeRemaining = new Dictionary<int, float>(), random = new System.Random(), init = false, positions = new Dictionary<int, GlyphRect>() };
    }

    private class CharAnimContext : IAnimationContext
    {
        public Dictionary<int, float> waitTimeRemaining;
        public Dictionary<int, GlyphRect> positions;
        public System.Random random;
        public bool init;

        public AnimatorContext animatorContext { get; set; }
        public SegmentData segmentData { get; set; }
    }
}
