using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
using static TMPEffects.ParameterUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new CharAnimation", menuName = "TMPEffects/Animations/Char")]
    public class CharAnimation : TMPAnimation
    {
        [SerializeField] string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        [SerializeField] float probability = 0.15f;
        [SerializeField] float minWait = 0.5f;
        [SerializeField] float maxWait = 2.5f;
        [SerializeField] bool autoCase = true;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            if (string.IsNullOrWhiteSpace(d.characters)) return;

            int segmentIndex = context.segmentData.SegmentIndexOf(cData);
            TMP_Character c;

            if (d.waitingSince == null)
            {
                Init(cData, d, context);
            }

            if (!d.positions.ContainsKey(segmentIndex))
            {
                if (!cData.info.fontAsset.characterLookupTable.TryGetValue(cData.info.character, out c))
                    Debug.LogError("Failed to get character from lookup table");

                d.positions[segmentIndex] = c.glyph.glyphRect;
                d.originalPositions[segmentIndex] = c.glyph.glyphRect;
            }

            float h = cData.info.fontAsset.atlasHeight;
            float w = cData.info.fontAsset.atlasWidth;

            // If waiting
            if (d.waitingSince[segmentIndex] != -1)
            {
                // If done waiting
                if (context.animatorContext.PassedTime - d.waitingSince[segmentIndex] >= d.waitDuration[segmentIndex])
                {
                    d.waitingSince[segmentIndex] = -1;
                }
                else
                {
                    GlyphRect rect = d.positions[segmentIndex];
                    GlyphRect ogRect = d.originalPositions[segmentIndex];
                    cData.mesh.SetUV0(0, new Vector2(rect.x / w, rect.y / h));
                    cData.mesh.SetUV0(1, new Vector2(rect.x / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(2, new Vector2((rect.x + rect.width) / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(3, new Vector2((rect.x + rect.width) / w, rect.y / h));

                    if (rect.width != ogRect.width || rect.height != ogRect.height)
                    {
                        float wProp = (float)rect.width / ogRect.width;
                        float hProp = (float)rect.height / ogRect.height;
                        float height = cData.mesh.initial.GetVertex(1).y - cData.mesh.initial.GetVertex(0).y;
                        height /= hProp;
                        cData.SetScale(new Vector3(wProp, hProp, 1f));
                        cData.SetPosition(cData.info.initialPosition - Vector3.up * height / 4);
                    }

                    return;
                }
            }

            // Set to original character
            if (d.random.NextDouble() > d.probability)
            {
                cData.info.fontAsset.characterLookupTable.TryGetValue(cData.info.character, out c);
                GlyphRect rect = c.glyph.glyphRect;
                cData.mesh.SetUV0(0, new Vector2(rect.x / w, rect.y / h));
                cData.mesh.SetUV0(1, new Vector2(rect.x / w, (rect.y + rect.height) / h));
                cData.mesh.SetUV0(2, new Vector2((rect.x + rect.width) / w, (rect.y + rect.height) / h));
                cData.mesh.SetUV0(3, new Vector2((rect.x + rect.width) / w, rect.y / h));
                d.positions[segmentIndex] = rect;
            }

            // Set to new random character
            else
            {
                int index = d.random.Next(0, d.characters.Length);
                char character = d.characters[index];
                if (d.autoCase && char.IsLetter(cData.info.character) && char.IsLetter(character))
                {
                    if (char.IsUpper(cData.info.character))
                        character = char.ToUpper(character);
                    else if (char.IsLower(cData.info.character))
                        character = char.ToLower(character);
                }

                bool t = cData.info.fontAsset.characterLookupTable.TryGetValue(character, out c);

                if (t)
                {
                    GlyphRect rect = c.glyph.glyphRect;
                    GlyphRect ogRect = d.originalPositions[segmentIndex];
                    cData.mesh.SetUV0(0, new Vector2(rect.x / w, rect.y / h));
                    cData.mesh.SetUV0(1, new Vector2(rect.x / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(2, new Vector2((rect.x + rect.width) / w, (rect.y + rect.height) / h));
                    cData.mesh.SetUV0(3, new Vector2((rect.x + rect.width) / w, rect.y / h));
                    d.positions[segmentIndex] = rect;

                    if (rect.width != ogRect.width || rect.height != ogRect.height)
                    {
                        float wProp = (float)rect.width / ogRect.width;
                        float hProp = (float)rect.height / ogRect.height;
                        float height = cData.mesh.initial.GetVertex(1).y - cData.mesh.initial.GetVertex(0).y;
                        height /= hProp;

                        cData.SetScale(new Vector3(wProp, hProp, 1f));
                        cData.SetPosition(cData.info.initialPosition - Vector3.up * height / 4);
                    }
                }
                else
                    Debug.LogError("Failed to get character from lookup table");
            }

            d.waitingSince[segmentIndex] = context.animatorContext.PassedTime;
            d.waitDuration[segmentIndex] = Mathf.Lerp(d.minWait, d.maxWait, (float)d.random.NextDouble());
        }


        private void Init(CharData cData, Data d, IAnimationContext context)
        {
            d.random = new System.Random((int)(context.animatorContext.PassedTime * 1000));

            if (cData.info.fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic)
                cData.info.fontAsset.TryAddCharacters(d.characters);

            //d.vertices = new(context.segmentData.length);
            d.originalPositions = new(context.segmentData.length);
            d.waitingSince = new(context.segmentData.length);
            d.waitDuration = new(context.segmentData.length);
            d.positions = new(context.segmentData.length);

            for (int i = 0; i < context.segmentData.length; i++)
            {
                d.waitDuration[i] = -1;
                d.waitingSince[i] = -1;
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            Data d = (Data)customData;

            if (TryGetFloatParameter(out float f, parameters, "probability", "prob", "p")) d.probability = f;
            if (TryGetFloatParameter(out f, parameters, "minWait", "minW")) d.minWait = f;
            if (TryGetFloatParameter(out f, parameters, "maxWait", "maxW")) d.maxWait = f;
            if (TryGetDefinedParameter(out string s, parameters, "characters", "char", "c")) d.characters = parameters[s];
            if (TryGetBoolParameter(out bool b, parameters, "autoCase", "case")) d.autoCase = b;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "probability", "prob", "p")) return false;
            if (HasNonFloatParameter(parameters, "minWait", "minW")) return false;
            if (HasNonFloatParameter(parameters, "maxWait", "maxW")) return false;
            if (HasNonBoolParameter(parameters, "autoCase", "case")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                characters = this.characters,
                probability = this.probability,
                minWait = this.minWait,
                maxWait = this.maxWait,
                autoCase = this.autoCase,
            };
        }

        private class Data
        {
            public Dictionary<int, float> waitingSince = null;
            public Dictionary<int, float> waitDuration = null;
            public Dictionary<int, GlyphRect> positions = null;
            public Dictionary<int, GlyphRect> originalPositions = null;
            public System.Random random = null;

            public string characters;
            public float probability;
            public float minWait;
            public float maxWait;
            public bool autoCase;
        }
    }
}