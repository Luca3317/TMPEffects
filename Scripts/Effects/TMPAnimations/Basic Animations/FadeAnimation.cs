using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new FadeAnimation", menuName = "TMPEffects/Animations/Fade")]
    public class FadeAnimation : TMPAnimation
    {
        [SerializeField] float maxOpacity = 255;
        [SerializeField] AnimationCurve fadeInCurve = AnimationCurveUtility.EaseInSine();
        [SerializeField] Vector2 fadeInAnchor = Vector2.zero;
        [SerializeField] float fadeInDuration = 1;
        [SerializeField] float afterFadeInWaitDuration = 1;

        [SerializeField] float minOpacity = 0;
        [SerializeField] AnimationCurve fadeOutCurve = AnimationCurveUtility.EaseOutSine();
        [SerializeField] Vector2 fadeOutAnchor = Vector2.zero;
        [SerializeField] float fadeOutDuration = 1;
        [SerializeField] float afterFadeOutWaitDuration = 1;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            if (d.cycleTime == -1) d.cycleTime = context.animatorContext.PassedTime;
            Color32 c;

            // Check if done fading in / out => should wait
            float fadeDuration = d.fadeIn ? d.fadeInDuration : d.fadeOutDuration;
            if (d.waitingSince == -1 && context.animatorContext.PassedTime - d.cycleTime >= fadeDuration)
            {
                d.waitingSince = context.animatorContext.PassedTime;
            }

            // If waiting
            float waitDuration = d.fadeIn ? d.afterFadeInWaitDuration : d.afterFadeOutWaitDuration;
            if (d.waitingSince != -1)
            {
                if (context.animatorContext.PassedTime - d.waitingSince >= waitDuration)
                {
                    d.cycleTime = context.animatorContext.PassedTime;
                    d.waitingSince = -1f;
                    d.fadeIn = !d.fadeIn;
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        c = cData.mesh.initial.GetColor(i);
                        c.a = (byte)(((d.fadeIn ? d.maxOpacity : d.minOpacity) / 255f) * c.a);
                        cData.mesh.SetColor(i, c);
                    }
                    return;
                }
            }

            // Get values to use (just to prevent a bunch of if / else branches)
            Vector2 anchor = d.fadeIn ? d.fadeInAnchor : d.fadeOutAnchor;
            FixAnchor(ref anchor);
            float targetOpacity = d.fadeIn ? d.maxOpacity : d.minOpacity;
            AnimationCurve curve = d.fadeIn ? d.fadeInCurve : d.fadeOutCurve;
            float startOpacity = d.fadeIn ? d.minOpacity : d.maxOpacity;

            float t = GetValue(curve, WrapMode.PingPong, context, (context.animatorContext.PassedTime - d.cycleTime) / fadeDuration, cData);
            float opacity = Mathf.Lerp(startOpacity, targetOpacity, t);

            if (anchor == Vector2.zero)
            {
                for (int i = 0; i < 4; i++)
                {
                    c = cData.mesh.initial.GetColor(i);
                    c.a = (byte)((opacity / 255f) * c.a);
                    cData.mesh.SetColor(i, c);
                }

                return;
            }

            float blDist = (new Vector2(-1, -1) - anchor).magnitude / d.sqrt2;
            float tlDist = (new Vector2(-1, 1) - anchor).magnitude / d.sqrt2;
            float trDist = (new Vector2(1, 1) - anchor).magnitude / d.sqrt2;
            float brDist = (new Vector2(1, -1) - anchor).magnitude / d.sqrt2;

            d.dists[0] = blDist;
            d.dists[1] = tlDist;
            d.dists[2] = trDist;
            d.dists[3] = brDist;

            for (int i = 0; i < 4; i++)
            {
                float op = Mathf.Lerp(startOpacity, targetOpacity, t * (10 - d.dists[i] * 9));
                c = cData.mesh.initial.GetColor(i);
                c.a = (byte)((op / 255f) * c.a);
                cData.mesh.SetColor(i, c);

                //if (d.fadeIn)
                //{
                //    Debug.Log("FADEIN For " + i + " with dist " + d.dists[i] + " and t " + (t * (2 - d.dists[i])) + " => alpha = " + c.a);
                //}
                //else
                //    Debug.Log("FADEOUT For " + i + " with dist " + d.dists[i] + " and t " + (t * (2 - d.dists[i])) + " => alpha = " + c.a);
            }
        }

        private void FixAnchor(ref Vector2 v)
        {
            if (v.x != 0)
            {
                if (v.x > 0) v.x = 1;
                else v.x = -1;
            }
            if (v.y != 0)
            {
                if (v.y > 0) v.y = 1;
                else v.y = -1;
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = (Data)customData;
            if (TryGetAnimCurveParameter(out var c, parameters, "fadeInCurve", fadeInCurveAliases)) d.fadeInCurve = c;
            if (TryGetFloatParameter(out var f, parameters, "maxOpacity", maxOpAliases)) d.maxOpacity = f;
            if (TryGetFloatParameter(out f, parameters, "fadeInDuration", fadeInDurationAliases)) d.fadeInDuration = f;
            if (TryGetFloatParameter(out f, parameters, "fadeInWait", fadeInWaitAliases)) d.afterFadeInWaitDuration = f;
            if (TryGetVector2Parameter(out var v2, parameters, "fadeInAnchor", fadeInAnchorAliases)) d.fadeInAnchor = v2;

            if (TryGetAnimCurveParameter(out c, parameters, "fadeOutCurve", fadeOutCurveAliases)) d.fadeOutCurve = c;
            if (TryGetFloatParameter(out f, parameters, "minOpacity", minOpAliases)) d.minOpacity = f;
            if (TryGetFloatParameter(out f, parameters, "fadeOutDuration", fadeOutDurationAliases)) d.fadeOutDuration = f;
            if (TryGetFloatParameter(out f, parameters, "fadeOutWait", fadeOutWaitAliases)) d.afterFadeOutWaitDuration = f;
            if (TryGetVector2Parameter(out v2, parameters, "fadeOutAnchor", fadeOutAnchorAliases)) d.fadeOutAnchor = v2;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "maxOpacity", maxOpAliases)) return false;
            if (HasNonFloatParameter(parameters, "fadeInWait", fadeInWaitAliases)) return false;
            if (HasNonFloatParameter(parameters, "fadeInDuration", fadeInDurationAliases)) return false;
            if (HasNonAnimCurveParameter(parameters, "fadeInCurve", fadeInCurveAliases)) return false;
            if (HasNonVector2Parameter(parameters, "fadeInAnchor", fadeInAnchorAliases)) return false;

            if (HasNonFloatParameter(parameters, "minOpacity", minOpAliases)) return false;
            if (HasNonFloatParameter(parameters, "fadeOutWait", fadeOutWaitAliases)) return false;
            if (HasNonFloatParameter(parameters, "fadeOutDuration", fadeOutDurationAliases)) return false;
            if (HasNonAnimCurveParameter(parameters, "fadeOutCurve", fadeOutCurveAliases)) return false;
            if (HasNonVector2Parameter(parameters, "fadeOutAnchor", fadeOutAnchorAliases)) return false;

            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                maxOpacity = this.maxOpacity,
                fadeInCurve = this.fadeInCurve,
                afterFadeInWaitDuration = this.afterFadeInWaitDuration,
                fadeInAnchor = this.fadeInAnchor,
                fadeInDuration = this.fadeInDuration,

                minOpacity = this.minOpacity,
                fadeOutCurve = this.fadeOutCurve,
                afterFadeOutWaitDuration = this.afterFadeOutWaitDuration,
                fadeOutAnchor = this.fadeOutAnchor,
                fadeOutDuration = this.fadeOutDuration,

                waitingSince = -1,
                cycleTime = -1,
                lastT = 0
            };
        }

        private readonly string[] maxOpAliases = new string[] { "max", "maxOp" };
        private readonly string[] fadeInCurveAliases = new string[] { "fiCurve", "fiCrv", "fiC" };
        private readonly string[] fadeInWaitAliases = new string[] { "fiWait", "fiW" };
        private readonly string[] fadeInDurationAliases = new string[] { "fiDuration", "fiDur", "fiD" };
        private readonly string[] fadeInAnchorAliases = new string[] { "fiAnchor", "fiAnc", "fiA" };

        private readonly string[] minOpAliases = new string[] { "min", "minOp" };
        private readonly string[] fadeOutCurveAliases = new string[] { "foCurve", "foCrv", "foC" };
        private readonly string[] fadeOutWaitAliases = new string[] { "foWait", "foWait", "foW" };
        private readonly string[] fadeOutDurationAliases = new string[] { "foDuration", "foDur", "foD" };
        private readonly string[] fadeOutAnchorAliases = new string[] { "foAnchor", "foAnc", "foA" };

        private class Data
        {
            public float maxOpacity;
            public AnimationCurve fadeInCurve;
            public float fadeInDuration;
            public Vector2 fadeInAnchor;
            public float afterFadeInWaitDuration;

            public float minOpacity;
            public AnimationCurve fadeOutCurve;
            public float fadeOutDuration;
            public Vector2 fadeOutAnchor;
            public float afterFadeOutWaitDuration;

            public int lastT;
            public float waitingSince;
            public bool fadeIn;

            public float cycleTime;

            public readonly float sqrt2 = Mathf.Sqrt(8);
            public readonly float[] dists = new float[4];
        }
    }
}