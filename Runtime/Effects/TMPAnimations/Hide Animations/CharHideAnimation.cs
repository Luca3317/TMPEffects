using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
using static TMPEffects.ParameterUtility;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [CreateAssetMenu(fileName = "new CharHideAnimation", menuName = "TMPEffects/Hide Animations/Char")]
    public class CharHideAnimation : TMPHideAnimation
    {
        [Tooltip("How long the animation will take to fully hide the character.\nAliases: duration, dur, d")]
        [SerializeField] float duration = 1f;

        [Tooltip("The pool of characters to change to.\nAliases: characters, chars, char, c")]
        [SerializeField] string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        [Tooltip("The probability to change to a character different from the original.\nAliases: probability, prob, p")]
        [SerializeField] float probability = 0.95f;
        [Tooltip("The minimum amount of time to wait once a character changed (or did not change).\nAliases: minwait, minw, min")]
        [SerializeField] float minWait = 0.1f;
        [Tooltip("The maximum amount of time to wait once a character changed (or did not change).\nAliases: maxwait, maxw, max")]
        [SerializeField] float minDelay = 0.1f;
        [Tooltip("Whether to ensure capitalized characters are only changed to other capitalized characters, and vice versa.\nautocase, case")]
        [SerializeField] bool autoCase = true;

        [Tooltip("The curve that defines the falloff of the wait between each change.\nAliases: waitcurve, waitcrv, waitc")]
        [SerializeField] AnimationCurve waitCurve = AnimationCurveUtility.Linear();
        [Tooltip("The curve that defines the falloff of the probability of changing to a character other than the original.\nAliases: probabilitycurve, probabilitycrv, probabilityc, probcurve, probcrv, probc")]
        [SerializeField] AnimationCurve probabilityCurve = AnimationCurveUtility.Invert(AnimationCurveUtility.Linear());

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;


            if (string.IsNullOrWhiteSpace(d.characters) || cData.info.elementType != TMP_TextElementType.Character)
            {
                context.FinishAnimation(cData);
                return;
            }

            if (!d.init)
            {
                d.init = true;

                InitRNGDict(context);
                InitLastUpdatedDict(context);
                InitDelayDict(context);
                InitCharactersDict(context);
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
            float probMult = d.probCurve.Evaluate(1 - t);

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
                            AnimationUtility.SetToCharacter(newCharacter, d.originalCharacterCache[segmentIndex], cData, context);
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
                AnimationUtility.SetToCharacter(current, original, cData, context);
            }
        }



        private void InitRNGDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            int seed = (int)(context.AnimatorContext.PassedTime * 1000);
            d.rngDict = new Dictionary<int, System.Random>(context.SegmentData.length);
            for (int i = 0; i < context.SegmentData.length; i++)
            {
                d.rngDict.Add(i, new System.Random(seed + i));
            }
        }

        private void InitLastUpdatedDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            d.lastUpdatedDict = new Dictionary<int, float>(context.SegmentData.length);
            for (int i = 0; i < context.SegmentData.length; i++)
            {
                d.lastUpdatedDict.Add(i, context.AnimatorContext.PassedTime);
            }
        }

        private void InitDelayDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            d.delayDict = new Dictionary<int, float>(context.SegmentData.length);

            for (int i = 0; i < context.SegmentData.length; i++)
            {
                d.delayDict.Add(i, 0);
            }
        }

        private void InitCharactersDict(IAnimationContext context)
        {
            Data d = context.CustomData as Data;
            d.currentCharacterCache = new Dictionary<int, TMP_Character>(context.SegmentData.length);
            d.originalCharacterCache = new Dictionary<int, TMP_Character>(context.SegmentData.length);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;
            Data d = (Data)customData;

            if (TryGetFloatParameter(out float f, parameters, "probability", "prob", "p")) d.probability = f;
            if (TryGetFloatParameter(out f, parameters, "duration", "dur", "d")) d.duration = f;
            if (TryGetFloatParameter(out f, parameters, "minwait", "minw", "min")) d.minWait = f;
            if (TryGetFloatParameter(out f, parameters, "maxwait", "maxw", "max")) d.maxWait = f;
            if (TryGetDefinedParameter(out string s, parameters, "characters", "char", "c")) d.characters = parameters[s];
            if (TryGetBoolParameter(out bool b, parameters, "autocase", "case")) d.autoCase = b;
            if (TryGetAnimCurveParameter(out var crv, parameters, "waitcurve", "waitcrv", "waitc")) d.waitCurve = crv;
            if (TryGetAnimCurveParameter(out crv, parameters, "probabilitycurve", "probabilitycrv", "probabilityc", "probcurve", "probcrv", "probc")) d.probCurve = crv;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "probability", "prob", "p")) return false;
            if (HasNonFloatParameter(parameters, "duration", "dur", "d")) return false;
            if (HasNonFloatParameter(parameters, "minwait", "minw", "min")) return false;
            if (HasNonFloatParameter(parameters, "maxwait", "maxw", "max")) return false;
            if (HasNonBoolParameter(parameters, "autocase", "case")) return false;
            if (HasNonAnimCurveParameter(parameters, "waitcurve", "waitcrv", "waitc")) return false;
            if (HasNonAnimCurveParameter(parameters, "probabilitycurve", "probabilitycrv", "probabilityc", "probcurve", "probcrv", "probc")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                init = false,
                duration = this.duration,
                characters = this.characters,
                probability = this.probability,
                minWait = this.minWait,
                maxWait = this.minDelay,
                autoCase = this.autoCase,

                waitCurve = this.waitCurve,
                probCurve = this.probabilityCurve,

                lastUpdatedDict = null,
                delayDict = null,
                rngDict = null,
            };
        }

        private class Data
        {
            public bool init;

            public float duration;
            public string characters;
            public float probability;
            public float minWait;
            public float maxWait;
            public bool autoCase;

            public AnimationCurve waitCurve;
            public AnimationCurve probCurve;

            public System.Random random = null;
            public Dictionary<int, TMP_Character> currentCharacterCache = null;
            public Dictionary<int, TMP_Character> originalCharacterCache = null;
            public Dictionary<int, float> lastUpdatedDict = null;
            public Dictionary<int, float> delayDict = null;
            public Dictionary<int, System.Random> rngDict = null;
        }
    }
}