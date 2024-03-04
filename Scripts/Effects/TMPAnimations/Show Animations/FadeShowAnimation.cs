using Codice.Client.Common.FsNodeReaders;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using TMPEffects.TextProcessing;
using UnityEngine;
using static TMPEffects.EffectUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new FadeShowAnimation", menuName = "TMPEffects/Show Animations/Fade")]
    public class FadeShowAnimation : TMPShowAnimation
    {
        [SerializeField] private float duration;
        [SerializeField] private Vector2 anchor;
        [SerializeField] private AnimationCurve easeInSine = AnimationCurveUtility.EaseInSine();
        [SerializeField] private AnimationCurve easeOutSine = AnimationCurveUtility.EaseOutSine();
        [SerializeField] private AnimationCurve easeInOutSine = AnimationCurveUtility.EaseInOutSine(); 
        [SerializeField] private AnimationCurve easeInQuad = AnimationCurveUtility.EaseInQuad();
        [SerializeField] private AnimationCurve easeOutQuad = AnimationCurveUtility.EaseOutQuad();
        [SerializeField] private AnimationCurve easeInOutQuad = AnimationCurveUtility.EaseInOutQuad();
        [SerializeField] private AnimationCurve easeInQuart = AnimationCurveUtility.EaseInQuart();
        [SerializeField] private AnimationCurve easeOutQuart = AnimationCurveUtility.EaseOutQuart();
        [SerializeField] private AnimationCurve easeInOutQuart = AnimationCurveUtility.EaseInOutQuart();
        [SerializeField] private AnimationCurve easeInCubic = AnimationCurveUtility.EaseInCubic();
        [SerializeField] private AnimationCurve easeOutCubic = AnimationCurveUtility.EaseOutCubic();
        [SerializeField] private AnimationCurve easeInOutCubic = AnimationCurveUtility.EaseInOutCubic();
        [SerializeField] private AnimationCurve easeInCirc = AnimationCurveUtility.EaseInCirc();
        [SerializeField] private AnimationCurve easeOutCirc = AnimationCurveUtility.EaseOutCirc();
        [SerializeField] private AnimationCurve easeInOutCirc = AnimationCurveUtility.EaseInOutCirc();
        [SerializeField] private AnimationCurve easeInElastic = AnimationCurveUtility.EaseInElastic();
        [SerializeField] private AnimationCurve easeOutElastic = AnimationCurveUtility.EaseOutElastic();
        [SerializeField] private AnimationCurve easeInOutElastic = AnimationCurveUtility.EaseInOutElastic();
        [SerializeField] private AnimationCurve easeInBounce = AnimationCurveUtility.EaseInBounce();
        [SerializeField] private AnimationCurve easeOutBounce = AnimationCurveUtility.EaseOutBounce();
        [SerializeField] private AnimationCurve easeInOutBounce = AnimationCurveUtility.EaseInOutBounce();
        [SerializeField] private AnimationCurve easeInBack = AnimationCurveUtility.EaseInBack();
        [SerializeField] private AnimationCurve easeOutBack = AnimationCurveUtility.EaseOutBack();
        [SerializeField] private AnimationCurve easeInOutBack = AnimationCurveUtility.EaseInOutBack();
        [SerializeField] private AnimationCurve quadraticcurve = AnimationCurveUtility.QuadraticBezier(new(0, 0), new(0.0001f, 1), new(1, 1));

        public override void Animate(CharData cData, IAnimationContext context)
        {
            var ac = context.animatorContext;
            var data = context.customData as Data;

            float t = data.duration > 0 ? (ac.PassedTime - ac.StateTime(cData)) / data.duration : 1f;
            float value = Mathf.Lerp(0f, 1f, t);

            value = AnimationCurveUtility.LinearBezier(new(0,0), new(1,1)).Evaluate(t);

            for (int i = 0; i < 4; i++)
            {
                Color32 color = cData.mesh.GetColor(i);
                color.a = (byte)(value * color.a);
                cData.mesh.SetColor(i, color);
            }

            if (t == 1)
            {
                context.FinishAnimation(cData);
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float val, parameters, "d", durationAlias)) d.duration = val;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "d", durationAlias)) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                duration = this.duration,
                anchor = this.anchor,
                //curve = this.curve
            };
        }

        private readonly string[] durationAlias = new string[] { "d", "dur" };
        private readonly string[] anchorAlias = new string[] { "a", "anc" };
        private readonly string[] curveAlias = new string[] { "c" };

        private class Data
        {
            public float duration;
            public Vector2 anchor;
            public AnimationCurve curve;
        }
    }
}
