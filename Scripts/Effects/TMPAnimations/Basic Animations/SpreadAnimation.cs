using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.ParameterUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new SpreadAnimation", menuName = "TMPEffects/Animations/Spread")]
    public class SpreadAnimation : TMPAnimation
    {
        [SerializeField] float growDuration = 1;
        [SerializeField] Vector3 growAnchor = new Vector3(0, -1, 0);
        [SerializeField] Vector3 growDirection = Vector3.up;
        [SerializeField] AnimationCurve growCurve = AnimationCurveUtility.EaseOutElastic();

        [SerializeField] float shrinkDuration = 1;
        [SerializeField] Vector3 shrinkAnchor = new Vector3(0, -1, 0);
        [SerializeField] Vector3 shrinkDirection = Vector3.up;
        [SerializeField] AnimationCurve shrinkCurve = AnimationCurveUtility.EaseOutElastic();

        [SerializeField] float maxPercentage = 1;
        [SerializeField] float minPercentage = 1;

        [Header("Wave - ignored if wavelength is 0")]
        [SerializeField] WaveType waveType;
        [SerializeField] float wavelength;
        [SerializeField] float crestWait;
        [SerializeField] float throughWait;
        [SerializeField] float velocity;
        [SerializeField] float amplitude;


        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            if (d.playingSince == -1) d.playingSince = context.animatorContext.PassedTime;
            if (!d.growingDict.ContainsKey(cData.info.index)) d.growingDict[cData.info.index] = true;

            if (wave == null)
            {
                wave = new Wave(growCurve, shrinkCurve, growDuration, shrinkDuration, velocity, amplitude, crestWait, throughWait);
            }

            int val = wave.PassedExtrema(context.animatorContext.PassedTime, context.animatorContext.DeltaTime, context.segmentData.SegmentIndexOf(cData));
            if (val == 1) d.growingDict[cData.info.index] = false;
            if (val == -1) d.growingDict[cData.info.index] = true;

            if (d.growingDict[cData.info.index])
            {
                Grow(cData, context, d);
            }
            else
            {
                Shrink(cData, context, d);
            }
        }

        [System.NonSerialized] Wave wave = null;

        private void Grow(CharData cData, IAnimationContext context, Data d)
        {
            float t = (context.animatorContext.PassedTime);
            float t2 = wave.Evaluate(t, context.segmentData.SegmentIndexOf(cData));
            float percentage = Mathf.LerpUnclamped(d.minPercentage, d.maxPercentage, t2);

            Vector3 actualDir = new Vector3(d.growDirection.y, d.growDirection.x, 0f);

            Vector3 lineStart = AnchorToPosition(d.growAnchor - actualDir * 2, cData);
            Vector3 lineEnd = AnchorToPosition(d.growAnchor + actualDir * 2, cData);

            for (int i = 0; i < 4; i++)
            {
                Vector3 startPos = ClosestPointOnLine(lineStart, lineEnd, cData.mesh.initial.GetVertex(i));
                Vector3 pos = Vector3.LerpUnclamped(startPos, cData.mesh.initial.GetVertex(i), percentage);

                SetVertexRaw(i, pos, cData, ref context);
            }
        }

        private void Shrink(CharData cData, IAnimationContext context, Data d)
        {
            float t = (context.animatorContext.PassedTime);
            float t2 = wave.Evaluate(t, context.segmentData.SegmentIndexOf(cData));
            float percentage = Mathf.LerpUnclamped(d.minPercentage, d.maxPercentage, t2);

            Vector3 actualDir = new Vector3(d.shrinkDirection.y, d.shrinkDirection.x, 0f);

            Vector3 lineStart = AnchorToPosition(d.shrinkAnchor - actualDir * 2, cData);
            Vector3 lineEnd = AnchorToPosition(d.shrinkAnchor + actualDir * 2, cData);

            for (int i = 0; i < 4; i++)
            {
                Vector3 startPos = ClosestPointOnLine(lineStart, lineEnd, cData.mesh.initial.GetVertex(i));
                Vector3 pos = Vector3.LerpUnclamped(startPos, cData.mesh.initial.GetVertex(i), percentage);

                SetVertexRaw(i, pos, cData, ref context);
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "growDuration", "growDur", "growD", "gD")) d.growDuration = f;
            if (TryGetVector3Parameter(out Vector3 v, parameters, "growDirection", "growDir", "gDir")) d.growDirection = v;
            if (TryGetVector3Parameter(out v, parameters, "growAnchor", "growAnc", "gAnc") || TryGetAnchorParameter(out v, parameters, "growAnchor", "growAnc", "gAnc")) d.growAnchor = v;
            if (TryGetAnimCurveParameter(out AnimationCurve crv, parameters, "growCurve", "growCrv", "growC", "gC")) d.growCurve = crv;

            if (TryGetFloatParameter(out f, parameters, "shrinkDuration", "shrinkDur", "shrinkD", "sD")) d.shrinkDuration = f;
            if (TryGetVector3Parameter(out v, parameters, "shrinkDirection", "shrinkDir", "sDir")) d.shrinkDirection = v;
            if (TryGetVector3Parameter(out v, parameters, "shrinkAnchor", "shrinkAnc", "sAnc") || TryGetAnchorParameter(out v, parameters, "shrinkAnchor", "shrinkAnc", "gAnc")) d.shrinkAnchor = v;
            if (TryGetAnimCurveParameter(out crv, parameters, "shrinkCurve", "shrinkCrv", "shrinkC", "sC")) d.shrinkCurve = crv;

            if (TryGetFloatParameter(out f, parameters, "maxPercentage", "maxP")) d.maxPercentage = f;
            if (TryGetFloatParameter(out f, parameters, "minPercentage", "minP")) d.minPercentage = f;

            if (TryGetWaveTypeParameter(out WaveType type, parameters, "wavetype", "wt")) d.waveType = type;
            if (TryGetFloatParameter(out f, parameters, "wavelength", "wavelen", "wl")) d.wavelength = f;
            if (TryGetFloatParameter(out f, parameters, "pulseinterval", "pulseinr", "plsinr", "pi")) d.interval = f;

            wave = null;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (!ValidateWaveParameters(parameters)) return false;
            if (HasNonFloatParameter(parameters, "growDuration", "growDur", "growD", "gD")) return false;
            if (HasNonVector3Parameter(parameters, "growDirection", "growDir", "gDir")) return false;
            if (HasNonVector3Parameter(parameters, "growAnchor", "growAnc", "gAnc") && HasNonAnchorParameter(parameters, "growAnchor", "growAnc", "gAnc")) return false;
            if (HasNonAnimCurveParameter(parameters, "growCurve", "growCrv", "growC", "gC")) return false;

            if (HasNonFloatParameter(parameters, "shrinkDuration", "shrinkDur", "shrinkD", "sD")) return false;
            if (HasNonVector3Parameter(parameters, "shrinkDirection", "shrinkDir", "sDir")) return false;
            if (HasNonVector3Parameter(parameters, "shrinkAnchor", "shrinkAnc", "sAnc") && HasNonAnchorParameter(parameters, "shrinkAnchor", "shrinkAnc", "gAnc")) return false;
            if (HasNonAnimCurveParameter(parameters, "shrinkCurve", "shrinkCrv", "shrinkC", "sC")) return false;

            if (HasNonFloatParameter(parameters, "maxPercentage", "maxP")) return false;
            if (HasNonFloatParameter(parameters, "minPercentage", "minP")) return false;

            if (HasWaveTypeParameter(parameters, "wavetype", "wt")) return false;
            if (HasNonFloatParameter(parameters, "wavelength", "wavelen", "wl")) return false;
            if (HasNonFloatParameter(parameters, "pulseinterval", "pulseinr", "plsinr", "pi")) return false;

            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                growDuration = this.growDuration,
                growAnchor = this.growAnchor,
                growDirection = this.growDirection,
                growCurve = this.growCurve,

                shrinkDuration = this.shrinkDuration,
                shrinkAnchor = this.shrinkAnchor,
                shrinkDirection = this.shrinkDirection,
                shrinkCurve = this.shrinkCurve,

                maxPercentage = this.maxPercentage,
                minPercentage = this.minPercentage,

                waitingSince = -1f,
                playingSince = -1f,
                growing = true,
                growingDict = new Dictionary<int, bool>()
            };
        }

        private class Data
        {
            //public Wave wave;

            public float growDuration = 1;
            public Vector3 growAnchor = new Vector3(0, -1, 0);
            public Vector3 growDirection = Vector3.up;
            public AnimationCurve growCurve = AnimationCurveUtility.EaseOutElastic();

            public float shrinkDuration = 1;
            public Vector3 shrinkAnchor = new Vector3(0, -1, 0);
            public Vector3 shrinkDirection = Vector3.up;
            public AnimationCurve shrinkCurve = AnimationCurveUtility.EaseOutElastic();

            public float maxPercentage = 1;
            public float minPercentage = 1;

            public WaveType waveType;
            public float wavelength;
            public float interval;

            public float waitingSince;
            public float playingSince;
            public bool growing;


            public Dictionary<int, bool> growingDict;
        }
    }
}




/*
 * pre wave
 * 
 * 
 * using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.ParameterUtility;
using TMPEffects.Extensions;
using Codice.CM.Common;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new SpreadAnimation", menuName = "TMPEffects/Animations/Spread")]
    public class SpreadAnimation : TMPAnimation
    {
        [SerializeField] float growDuration = 1;
        [SerializeField] Vector3 growAnchor = new Vector3(0, -1, 0);
        [SerializeField] Vector3 growDirection = Vector3.up;
        [SerializeField] AnimationCurve growCurve = AnimationCurveUtility.EaseOutElastic();

        [SerializeField] float shrinkDuration = 1;
        [SerializeField] Vector3 shrinkAnchor = new Vector3(0, -1, 0);
        [SerializeField] Vector3 shrinkDirection = Vector3.up;
        [SerializeField] AnimationCurve shrinkCurve = AnimationCurveUtility.EaseOutElastic();

        [SerializeField] float maxPercentage = 1;
        [SerializeField] float minPercentage = 1;

        public override void Animate(CharData cData, IAnimationContext context)
        {

        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "growDuration", "growDur", "growD", "gD")) d.growDuration = f;
            if (TryGetVector3Parameter(out Vector3 v, parameters, "growDirection", "growDir", "gDir")) d.growDirection = v;
            if (TryGetVector3Parameter(out v, parameters, "growAnchor", "growAnc", "gAnc") || TryGetAnchorParameter(out v, parameters, "growAnchor", "growAnc", "gAnc")) d.growAnchor = v;
            if (TryGetAnimCurveParameter(out AnimationCurve crv, parameters, "growCurve", "growCrv", "growC", "gC")) d.growCurve = crv;

            if (TryGetFloatParameter(out f, parameters, "shrinkDuration", "shrinkDur", "shrinkD", "sD")) d.shrinkDuration = f;
            if (TryGetVector3Parameter(out v, parameters, "shrinkDirection", "shrinkDir", "sDir")) d.shrinkDirection = v;
            if (TryGetVector3Parameter(out v, parameters, "shrinkAnchor", "shrinkAnc", "sAnc") || TryGetAnchorParameter(out v, parameters, "shrinkAnchor", "shrinkAnc", "gAnc")) d.shrinkAnchor = v;
            if (TryGetAnimCurveParameter(out crv, parameters, "shrinkCurve", "shrinkCrv", "shrinkC", "sC")) d.shrinkCurve = crv;

            if (TryGetFloatParameter(out f, parameters, "maxPercentage", "maxP")) d.maxPercentage = f;
            if (TryGetFloatParameter(out f, parameters, "minPercentage", "minP")) d.minPercentage = f;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "growDuration", "growDur", "growD", "gD")) return false;
            if (HasNonVector3Parameter(parameters, "growDirection", "growDir", "gDir")) return false;
            if (HasNonVector3Parameter(parameters, "growAnchor", "growAnc", "gAnc") && HasNonAnchorParameter(parameters, "growAnchor", "growAnc", "gAnc")) return false;
            if (HasNonAnimCurveParameter(parameters, "growCurve", "growCrv", "growC", "gC")) return false;

            if (HasNonFloatParameter(parameters, "shrinkDuration", "shrinkDur", "shrinkD", "sD")) return false;
            if (HasNonVector3Parameter(parameters, "shrinkDirection", "shrinkDir", "sDir")) return false;
            if (HasNonVector3Parameter(parameters, "shrinkAnchor", "shrinkAnc", "sAnc") && HasNonAnchorParameter(parameters, "shrinkAnchor", "shrinkAnc", "gAnc")) return false;
            if (HasNonAnimCurveParameter(parameters, "shrinkCurve", "shrinkCrv", "shrinkC", "sC")) return false;

            if (HasNonFloatParameter(parameters, "maxPercentage", "maxP")) return false;
            if (HasNonFloatParameter(parameters, "minPercentage", "minP")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                growDuration = this.growDuration,
                growAnchor = this.growAnchor,
                growDirection = this.growDirection,
                growCurve = this.growCurve,

                shrinkDuration = this.shrinkDuration,
                shrinkAnchor = this.shrinkAnchor,
                shrinkDirection = this.shrinkDirection,
                shrinkCurve = this.shrinkCurve,

                maxPercentage = this.maxPercentage,
                minPercentage = this.minPercentage,

                waitingSince = -1f
            };
        }

        private class Data
        {
            public float growDuration = 1;
            public Vector3 growAnchor = new Vector3(0, -1, 0);
            public Vector3 growDirection = Vector3.up;
            public AnimationCurve growCurve = AnimationCurveUtility.EaseOutElastic();

            public float shrinkDuration = 1;
            public Vector3 shrinkAnchor = new Vector3(0, -1, 0);
            public Vector3 shrinkDirection = Vector3.up;
            public AnimationCurve shrinkCurve = AnimationCurveUtility.EaseOutElastic();

            public float maxPercentage = 1;
            public float minPercentage = 1;


            public float waitingSince;
            public float playingSince;
            public bool growing;
        }
    }
}


 */