using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPro;
using UnityEngine;
using static TMPEffects.Parameters.TMPParameterUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new CharAnimation", menuName = "TMPEffects/Animations/Basic Animations/Built-in/Char")]
    public partial class CharAnimation : TMPAnimation
    {
        [SerializeField, AutoParameter("characters", "chars", "char", "c")]
        [Tooltip("The pool of characters to change to.\nAliases: characters, chars, char, c")]
        string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        [SerializeField, AutoParameter("probability", "prob", "p")]
        [Tooltip("The probability to change to a character different from the original.\n" +
                 "Aliases: probability, prob, p")]
        float probability = 0.15f;

        [SerializeField, AutoParameter("minwait", "minw", "min")]
        [Tooltip(
            "The minimum amount of time to wait once a character changed (or did not change).\nAliases: minwait, minw, min")]
        float minWait = 0.5f;

        [SerializeField, AutoParameter("maxwait", "maxw", "max")]
        [Tooltip(
            "The maximum amount of time to wait once a character changed (or did not change).\nAliases: maxwait, maxw, max")]
        float maxWait = 2.5f;

        [SerializeField, AutoParameter("autocase", "case")]
        [Tooltip(
            "Whether to ensure capitalized characters are only changed to other capitalized characters, and vice versa.\nautocase, case")]
        bool autoCase = true;

        private partial void Animate(CharData cData, Data data, IAnimationContext context)
        {
            if (string.IsNullOrWhiteSpace(data.characters)) return;
            if (cData.info.elementType != TMP_TextElementType.Character) return;

            int segmentIndex = context.SegmentData.SegmentIndexOf(cData);

            if (data.waitingSince == null)
            {
                Init(cData, data, context);
            }

            if (!data.originalCharacterCache.ContainsKey(segmentIndex))
            {
                if (cData.info.fontAsset.characterLookupTable.TryGetValue(cData.info.character,
                        out TMP_Character original))
                {
                    data.originalCharacterCache[segmentIndex] = original;
                    data.currentCharacterCache[segmentIndex] = original;
                }
                else
                {
                    return;
                }
            }

            // If waiting
            if (data.waitingSince[segmentIndex] != -1)
            {
                // If done waiting
                if (context.AnimatorContext.PassedTime - data.waitingSince[segmentIndex] >= data.waitDuration[segmentIndex])
                {
                    data.waitingSince[segmentIndex] = -1;
                }
                else
                {
                    TMP_Character current = data.currentCharacterCache[segmentIndex];
                    TMP_Character original = data.originalCharacterCache[segmentIndex];
                    TMPAnimationUtility.SetToCharacter(current, original, cData, context);
                    return;
                }
            }

            // Set to original character
            if (data.random.NextDouble() > data.probability)
            {
                data.currentCharacterCache[segmentIndex] = data.originalCharacterCache[segmentIndex];
                TMP_Character current = data.currentCharacterCache[segmentIndex];
                TMP_Character original = data.originalCharacterCache[segmentIndex];
                TMPAnimationUtility.SetToCharacter(current, original, cData, context);
            }

            // Set to new random character
            else
            {
                int index = data.random.Next(0, data.characters.Length);
                char character = data.characters[index];
                if (data.autoCase && char.IsLetter(cData.info.character) && char.IsLetter(character))
                {
                    if (char.IsUpper(cData.info.character))
                        character = char.ToUpper(character);
                    else if (char.IsLower(cData.info.character))
                        character = char.ToLower(character);
                }

                bool succ = cData.info.fontAsset.characterLookupTable.TryGetValue(character,
                    out TMP_Character newCharacter);

                if (succ)
                {
                    data.currentCharacterCache[segmentIndex] = newCharacter;
                    TMPAnimationUtility.SetToCharacter(newCharacter, data.originalCharacterCache[segmentIndex], cData,
                        context);
                }
                else
                    Debug.LogError($"Failed to get character {character} from lookup table");
            }

            data.waitingSince[segmentIndex] = context.AnimatorContext.PassedTime;
            data.waitDuration[segmentIndex] = Mathf.Lerp(data.minWait, data.maxWait, (float)data.random.NextDouble());
        }
        
        private void Init(CharData cData, Data d, IAnimationContext context)
        {
            d.random = new System.Random((int)(context.AnimatorContext.PassedTime * 1000));

            if (cData.info.fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic)
                cData.info.fontAsset.TryAddCharacters(d.characters);

            //d.vertices = new(context.segmentData.length);
            d.waitingSince = new(context.SegmentData.Length);
            d.waitDuration = new(context.SegmentData.Length);
            d.originalCharacterCache = new(context.SegmentData.Length);
            d.currentCharacterCache = new(context.SegmentData.Length);

            for (int i = 0; i < context.SegmentData.Length; i++)
            {
                d.waitDuration[i] = -1;
                d.waitingSince[i] = -1;
            }
        }

        [AutoParametersStorage]
        private partial class Data
        {
            public Dictionary<int, float> waitingSince = null;
            public Dictionary<int, float> waitDuration = null;
            public Dictionary<int, TMP_Character> currentCharacterCache = null;
            public Dictionary<int, TMP_Character> originalCharacterCache = null;
            public System.Random random = null;
        }
    }
}