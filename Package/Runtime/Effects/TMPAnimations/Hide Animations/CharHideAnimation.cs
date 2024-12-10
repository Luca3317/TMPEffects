using System.Collections;
using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
using static TMPEffects.Parameters.TMPParameterUtility;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [AutoParameters] 
    [CreateAssetMenu(fileName = "new CharHideAnimation", menuName = "TMPEffects/Animations/Hide Animations/Built-in/Char")]
    public partial class CharHideAnimation : TMPHideAnimation
    {
        [SerializeField, AutoParameter("duration", "dur", "d")]
        [Tooltip("How long the animation will take to fully hide the character.\nAliases: duration, dur, d")]
        float duration = 1f;

        [SerializeField, AutoParameter("characters", "chars", "char", "c")]
        [Tooltip("The pool of characters to change to.\nAliases: characters, chars, char, c")]
        string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        [SerializeField, AutoParameter("probability", "prob", "p")]
        [Tooltip("The probability to change to a character different from the original.\nAliases: probability, prob, p")]
        float probability = 0.95f;
        
        [SerializeField, AutoParameter("minwait", "minw", "min")]
        [Tooltip("The minimum amount of time to wait once a character changed (or did not change).\nAliases: minwait, minw, min")]
        float minWait = 0.1f;
        
        [SerializeField, AutoParameter("maxwait", "maxw", "max")]
        [Tooltip("The maximum amount of time to wait once a character changed (or did not change).\nAliases: maxwait, maxw, max")]
        float maxWait = 0.1f;
        
        [SerializeField, AutoParameter("autocase", "case")]
        [Tooltip("Whether to ensure capitalized characters are only changed to other capitalized characters, and vice versa.\nautocase, case")]
        bool autoCase = true;

        [SerializeField, AutoParameter("waitcurve", "waitcrv", "wait")]
        [Tooltip("The curve that defines the falloff of the wait between each change.\nAliases: waitcurve, waitcrv, waitc")]
        AnimationCurve waitCurve = AnimationCurveUtility.Linear();
        
        [SerializeField, AutoParameter("probabilitycurve", "probabilitycrv", "probabilityc", "probcurve", "probcrv", "probc")]
        [Tooltip("The curve that defines the falloff of the probability of changing to a character other than the original.\nAliases: probabilitycurve, probabilitycrv, probabilityc, probcurve, probcrv, probc")]
        AnimationCurve probabilityCurve = AnimationCurveUtility.Invert(AnimationCurveUtility.Linear());

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            if (string.IsNullOrWhiteSpace(d.characters) || cData.info.elementType != TMP_TextElementType.Character)
            {
                context.FinishAnimation(cData);
                return;
            }

            if (!d.init)
            {
                d.init = true;

                InitRNGDict(d, context);
                InitLastUpdatedDict(d, context);
                InitDelayDict(d, context);
                InitCharactersDict(d, context);
            }

            int segmentIndex = context.SegmentData.SegmentIndexOf(cData);
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

            float t = Mathf.Lerp(0, 1, (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData)) / d.duration);

            float delayMult = d.waitCurve.Evaluate(1 - t);
            float probMult = d.probabilityCurve.Evaluate(1 - t);

            float remaining = d.duration - (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData));

            if (t == 1)
            {
                d.delayDict[segmentIndex] = 0f;
                d.lastUpdatedDict[segmentIndex] = 0f;
                context.FinishAnimation(cData);
                return;
            }

            float h = cData.info.fontAsset.atlasHeight;
            float w = cData.info.fontAsset.atlasWidth;

            // If waiting done
            if (context.AnimatorContext.PassedTime - d.lastUpdatedDict[segmentIndex] >= d.delayDict[segmentIndex])
            {
                // If time for another
                if (remaining >= d.minWait * delayMult)
                {
                    bool original = d.rngDict[segmentIndex].NextDouble() * probMult > d.probability;

                    // Set delay
                    float delay = d.maxWait == d.minWait ? d.maxWait : Mathf.Lerp(d.minWait, d.maxWait, (float)d.rngDict[segmentIndex].NextDouble());
                    delay *= delayMult;
                    delay = Mathf.Clamp(delay, 0f, remaining);
                    d.delayDict[segmentIndex] = delay;

                    d.lastUpdatedDict[segmentIndex] = context.AnimatorContext.PassedTime;

                    // Set to original
                    if (original)
                    {
                        d.currentCharacterCache[segmentIndex] = d.originalCharacterCache[segmentIndex];
                    }
                    else
                    {
                        int index = d.rngDict[segmentIndex].Next(0, d.characters.Length);
                        char character = d.characters[index];
                        if (d.autoCase && char.IsLetter(cData.info.character) && char.IsLetter(character))
                        {
                            if (char.IsUpper(cData.info.character))
                                character = char.ToUpper(character);
                            else if (char.IsLower(cData.info.character))
                                character = char.ToLower(character);
                        }

                        bool success = cData.info.fontAsset.characterLookupTable.TryGetValue(character, out TMP_Character newCharacter);

                        if (success)
                        {
                            d.currentCharacterCache[segmentIndex] = newCharacter;
                            TMPAnimationUtility.SetToCharacter(newCharacter, d.originalCharacterCache[segmentIndex], cData, context);
                        }
                        else
                            Debug.LogError($"Failed to get character {character} from lookup table");
                    }
                }
                // If not, set to original
                else
                {
                    d.currentCharacterCache[segmentIndex] = d.originalCharacterCache[segmentIndex];
                }
            }
            else
            {
                TMP_Character current = d.currentCharacterCache[segmentIndex];
                TMP_Character original = d.originalCharacterCache[segmentIndex];
                TMPAnimationUtility.SetToCharacter(current, original, cData, context);
            }
        }



        private void InitRNGDict(AutoParametersData d, IAnimationContext context)
        {
            int seed = (int)(context.AnimatorContext.PassedTime * 1000);
            d.rngDict = new Dictionary<int, System.Random>(context.SegmentData.Length);
            for (int i = 0; i < context.SegmentData.Length; i++)
            {
                d.rngDict.Add(i, new System.Random(seed + i));
            }
        }

        private void InitLastUpdatedDict(AutoParametersData d, IAnimationContext context)
        {
            d.lastUpdatedDict = new Dictionary<int, float>(context.SegmentData.Length);
            for (int i = 0; i < context.SegmentData.Length; i++)
            {
                d.lastUpdatedDict.Add(i, context.AnimatorContext.PassedTime);
            }
        }

        private void InitDelayDict(AutoParametersData d, IAnimationContext context)
        {
            d.delayDict = new Dictionary<int, float>(context.SegmentData.Length);

            for (int i = 0; i < context.SegmentData.Length; i++)
            {
                d.delayDict.Add(i, 0);
            }
        }

        private void InitCharactersDict(AutoParametersData d, IAnimationContext context)
        {
            d.currentCharacterCache = new Dictionary<int, TMP_Character>(context.SegmentData.Length);
            d.originalCharacterCache = new Dictionary<int, TMP_Character>(context.SegmentData.Length);
        }
        
        [AutoParametersStorage]
        private partial class AutoParametersData
        {
            public bool init = false;
            public System.Random random = null;
            public Dictionary<int, TMP_Character> currentCharacterCache = null;
            public Dictionary<int, TMP_Character> originalCharacterCache = null;
            public Dictionary<int, float> lastUpdatedDict = null;
            public Dictionary<int, float> delayDict = null;
            public Dictionary<int, System.Random> rngDict = null;
        }
    }
}