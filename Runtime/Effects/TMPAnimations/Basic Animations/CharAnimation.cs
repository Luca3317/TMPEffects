using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.TextCore;
using static TMPEffects.ParameterUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new CharAnimation", menuName = "TMPEffects/Animations/Char")]
    public class CharAnimation : TMPAnimation
    {
        [Tooltip("The pool of characters to change to.\nAliases: characters, chars, char, c")]
        [SerializeField] string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        [Tooltip("The probability to change to a character different from the original.\nAliases: probability, prob, p")]
        [SerializeField] float probability = 0.15f;
        [Tooltip("The minimum amount of time to wait once a character changed (or did not change).\nAliases: minwait, minw, min")]
        [SerializeField] float minWait = 0.5f;
        [Tooltip("The maximum amount of time to wait once a character changed (or did not change).\nAliases: maxwait, maxw, max")]
        [SerializeField] float maxWait = 2.5f;
        [Tooltip("Whether to ensure capitalized characters are only changed to other capitalized characters, and vice versa.\nautocase, case")]
        [SerializeField] bool autoCase = true;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;

            if (string.IsNullOrWhiteSpace(d.characters)) return;
            if (cData.info.elementType != TMP_TextElementType.Character) return;

            int segmentIndex = context.SegmentData.SegmentIndexOf(cData);

            if (d.waitingSince == null)
            {
                Init(cData, d, context);
            }

            if (!d.originalCharacterCache.ContainsKey(segmentIndex))
            {
                if (cData.info.fontAsset.characterLookupTable.TryGetValue(cData.info.character, out TMP_Character original))
                {
                    d.originalCharacterCache[segmentIndex] = original;
                    d.currentCharacterCache[segmentIndex] = original;
                }
                else
                {
                    return;
                }
            }

            // If waiting
            if (d.waitingSince[segmentIndex] != -1)
            {
                // If done waiting
                if (context.AnimatorContext.PassedTime - d.waitingSince[segmentIndex] >= d.waitDuration[segmentIndex])
                {
                    d.waitingSince[segmentIndex] = -1;
                }
                else
                {
                    TMP_Character current = d.currentCharacterCache[segmentIndex];
                    TMP_Character original = d.originalCharacterCache[segmentIndex];
                    AnimationUtility.SetToCharacter(current, original, cData, context);
                    return;
                }
            }

            // Set to original character
            if (d.random.NextDouble() > d.probability)
            {
                d.currentCharacterCache[segmentIndex] = d.originalCharacterCache[segmentIndex];
                TMP_Character current = d.currentCharacterCache[segmentIndex];
                TMP_Character original = d.originalCharacterCache[segmentIndex];
                AnimationUtility.SetToCharacter(current, original, cData, context);
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

                bool succ = cData.info.fontAsset.characterLookupTable.TryGetValue(character, out TMP_Character newCharacter);

                if (succ)
                {
                    d.currentCharacterCache[segmentIndex] = newCharacter;
                    AnimationUtility.SetToCharacter(newCharacter, d.originalCharacterCache[segmentIndex], cData, context);
                }
                else
                    Debug.LogError($"Failed to get character {character} from lookup table");
            }

            d.waitingSince[segmentIndex] = context.AnimatorContext.PassedTime;
            d.waitDuration[segmentIndex] = Mathf.Lerp(d.minWait, d.maxWait, (float)d.random.NextDouble());
        }


        private void Init(CharData cData, Data d, IAnimationContext context)
        {
            d.random = new System.Random((int)(context.AnimatorContext.PassedTime * 1000));

            if (cData.info.fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic)
                cData.info.fontAsset.TryAddCharacters(d.characters);

            //d.vertices = new(context.segmentData.length);
            d.waitingSince = new(context.SegmentData.length);
            d.waitDuration = new(context.SegmentData.length);
            d.originalCharacterCache = new(context.SegmentData.length);
            d.currentCharacterCache = new(context.SegmentData.length);

            for (int i = 0; i < context.SegmentData.length; i++)
            {
                d.waitDuration[i] = -1;
                d.waitingSince[i] = -1;
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            Data d = (Data)customData;

            if (TryGetFloatParameter(out float f, parameters, "probability", "prob", "p")) d.probability = f;
            if (TryGetFloatParameter(out f, parameters, "minwait", "minw", "min")) d.minWait = f;
            if (TryGetFloatParameter(out f, parameters, "maxwait", "maxw", "max")) d.maxWait = f;
            if (TryGetDefinedParameter(out string s, parameters, "characters", "char", "chars", "c")) d.characters = parameters[s];
            if (TryGetBoolParameter(out bool b, parameters, "autocase", "case")) d.autoCase = b;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "probability", "prob", "p")) return false;
            if (HasNonFloatParameter(parameters, "minwait", "minw", "min")) return false;
            if (HasNonFloatParameter(parameters, "maxwait", "maxw", "max")) return false;
            if (HasNonBoolParameter(parameters, "autocase", "case")) return false;
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
            public Dictionary<int, TMP_Character> currentCharacterCache = null;
            public Dictionary<int, TMP_Character> originalCharacterCache = null;
            public System.Random random = null;

            public string characters;
            public float probability;
            public float minWait;
            public float maxWait;
            public bool autoCase;
        }
    }
}