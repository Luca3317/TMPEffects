using System.Collections.Generic;
using TMPEffects.TextProcessing;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new CharShowAnimation", menuName = "TMPEffects/Show Animations/Char")]
    public class CharShowAnimation : TMPShowAnimation
    {
        [SerializeField] string characters;
        [SerializeField] int maxUnitSize;
        [SerializeField] int minUnitSize;
        [SerializeField] float originalChance;
        [SerializeField] Order order;
        [SerializeField] float maxWait;
        [SerializeField] float minWait;


        [System.NonSerialized] float currentDuration;
        [System.NonSerialized] string currentCharacters;
        [System.NonSerialized] int currentMaxUnitSize;
        [System.NonSerialized] int currentMinUnitSize;
        [System.NonSerialized] float currentOriginalChance;
        [System.NonSerialized] Order currentOrder;
        [System.NonSerialized] float currentMaxWait;
        [System.NonSerialized] float currentMinWait;

        [System.Serializable]
        enum Order
        {
            Random = 0,
            Sequential = 5,
            SequentialReverse = 10,
        }

        public override void Animate(CharData cData, IAnimationContext context)
        {
            if (string.IsNullOrWhiteSpace(currentCharacters)) return;


            if (context.animatorContext.PassedTime - context.animatorContext.VisibleTime(cData) >= currentDuration)
            {
                context.FinishAnimation(cData.info.index);
                return;
            }


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
                cData.info.fontAsset.TryAddCharacters(currentCharacters);
                ctx.init = true;
            }




            ctx.waitTimeRemaining[cData.segmentIndex] = ctx.waitTimeRemaining[cData.segmentIndex] - context.animatorContext.DeltaTime;

            float h = cData.info.fontAsset.atlasHeight;
            float w = cData.info.fontAsset.atlasWidth;

            // If still waiting
            if (ctx.waitTimeRemaining[cData.segmentIndex] > 0)
            {
                GlyphRect rect = ctx.positions[cData.segmentIndex];
                cData.mesh.SetUV0(0, new Vector2(rect.x / w, rect.y / h));
                cData.mesh.SetUV0(1, new Vector2(rect.x / w, (rect.y + rect.height) / h));
                cData.mesh.SetUV0(2, new Vector2((rect.x + rect.width) / w, (rect.y + rect.height) / h));
                cData.mesh.SetUV0(3, new Vector2((rect.x + rect.width) / w, rect.y / h));
                return;
            }





            float t = (context.animatorContext.PassedTime - context.animatorContext.VisibleTime(cData)) / currentDuration;
            if (currentOrder == Order.Random)
            {
                float plus = Mathf.Lerp(0, 1 - currentOriginalChance, t);
                float newOriginalChance = currentOriginalChance + plus;

                if (ctx.random.NextDouble() < newOriginalChance)
                {
                    cData.info.fontAsset.characterLookupTable.TryGetValue(cData.info.character, out c);
                    GlyphRect rect = c.glyph.glyphRect;
                    cData.mesh.SetUV0(0, new Vector2(rect.x / w, rect.y / h));
                    cData.mesh.SetUV0(1, new Vector2(rect.x / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(2, new Vector2((rect.x + rect.width) / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(3, new Vector2((rect.x + rect.width) / w, rect.y / h));
                    ctx.positions[cData.segmentIndex] = rect;
                }
                else
                {

                    cData.info.fontAsset.characterLookupTable.TryGetValue(cData.info.character, out c);
                    GlyphRect rect = c.glyph.glyphRect;
                    rect = GetRectWithAspectRatio(rect, cData.info.fontAsset);
                    cData.mesh.SetUV0(0, new Vector2(rect.x / w, rect.y / h));
                    cData.mesh.SetUV0(1, new Vector2(rect.x / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(2, new Vector2((rect.x + rect.width) / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(3, new Vector2((rect.x + rect.width) / w, rect.y / h));
                    ctx.positions[cData.segmentIndex] = rect;


                }
            }
            else if (currentOrder == Order.Sequential)
            {
                float charIndex = Mathf.Lerp(-1, context.segmentData.length, t);
                if (charIndex >= cData.segmentIndex)
                {
                    cData.info.fontAsset.characterLookupTable.TryGetValue(cData.info.character, out c);
                    GlyphRect rect = c.glyph.glyphRect;
                    cData.mesh.SetUV0(0, new Vector2(rect.x / w, rect.y / h));
                    cData.mesh.SetUV0(1, new Vector2(rect.x / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(2, new Vector2((rect.x + rect.width) / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(3, new Vector2((rect.x + rect.width) / w, rect.y / h));
                    ctx.positions[cData.segmentIndex] = rect;
                }
                else
                {

                    cData.info.fontAsset.characterLookupTable.TryGetValue(cData.info.character, out c);
                    GlyphRect rect = c.glyph.glyphRect;
                    rect = GetRectWithAspectRatio(rect, cData.info.fontAsset);
                    cData.mesh.SetUV0(0, new Vector2(rect.x / w, rect.y / h));
                    cData.mesh.SetUV0(1, new Vector2(rect.x / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(2, new Vector2((rect.x + rect.width) / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(3, new Vector2((rect.x + rect.width) / w, rect.y / h));
                    ctx.positions[cData.segmentIndex] = rect;

                }
            }
            else if (currentOrder == Order.SequentialReverse)
            {

            }


            var d = Mathf.Lerp(currentMinWait, currentMaxWait, (float)ctx.random.NextDouble());

            ctx.waitTimeRemaining[cData.segmentIndex] = d;
            return;

            GlyphRect GetRectWithAspectRatio(GlyphRect rect, TMP_FontAsset fontAsset, bool considerSize = true)
            {
                int maxAttempts = 20;
                float maxAspectDiff = 0.1f;
                float maxSizeDiff = 10f;
                float aspect = (float)rect.width / rect.height;
                float size = rect.height + rect.width;

                for (int i = 0; i < maxAttempts; i++)
                {
                    int index = ctx.random.Next(0, currentCharacters.Length);
                    bool tr = fontAsset.characterLookupTable.TryGetValue(currentCharacters[index], out c);

                    if (tr)
                    {
                        GlyphRect rect2 = c.glyph.glyphRect;
                        float aspect2 = (float)rect2.width / rect2.height;
                        float size2 = (float)rect2.height + rect2.width;

                        if (Mathf.Abs(aspect - aspect2) < maxAspectDiff && (!considerSize || Mathf.Abs(size - size2) < maxSizeDiff))
                        {
                            return rect2;
                        }
                    }
                }

                return rect;
            }
        }

        public override void ResetParameters()
        {
            currentDuration = duration;
            currentOriginalChance = originalChance;
            currentCharacters = characters;
            currentMaxUnitSize = maxUnitSize;
            currentMinUnitSize = minUnitSize;
        }

        public override void SetParameters(IDictionary<string, string> parameters)
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

                    case "maxwait":
                    case "maxw":
                        ParsingUtility.StringToFloat(pair.Value, out currentMaxWait);
                        break;

                    case "minwait":
                    case "minw":
                        ParsingUtility.StringToFloat(pair.Value, out currentMinWait);
                        break;


                    case "d":
                    case "dur":
                    case "duration":
                        ParsingUtility.StringToFloat(pair.Value, out currentDuration);
                        break;


                    case "o":
                    case "order":
                        StringToOrder(pair.Value, out currentOrder);
                        break;
                }
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
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
                    case "d":
                    case "dur":
                    case "duration":
                        if (!ParsingUtility.StringToFloat(pair.Value, out _)) return false;
                        break;

                    case "o":
                    case "order":
                        if (!StringToOrder(pair.Value, out _)) return false;
                        break;
                }
            }

            return true;
        }

        public override object GetNewCustomData()
        {
            return new CharAnimContext() { waitTimeRemaining = new Dictionary<int, float>(), random = new System.Random(), init = false, positions = new Dictionary<int, GlyphRect>() };
        }

        private class CharAnimContext 
        {
            public Dictionary<int, float> waitTimeRemaining;
            public Dictionary<int, GlyphRect> positions;
            public System.Random random;
            public bool init;
        }

        bool StringToOrder(string str, out Order order)
        {
            switch (str)
            {
                case "random": order = Order.Random; return true;
                case "seq":
                case "sequential": order = Order.Sequential; return true;
                case "rev":
                case "reverse": order = Order.SequentialReverse; return true;
            }

            order = Order.Random;
            return false;
        }
    }
}

