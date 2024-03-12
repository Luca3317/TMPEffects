using UnityEngine;
using TMPEffects.Components.CharacterData;
using System;
using static TMPEffects.ParameterUtility;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Utility methods for animations.
    /// </summary>
    public static class AnimationUtility
    {
        #region Waiting
        /// <summary>
        /// Begins a waiting process.
        /// </summary>
        /// <remarks>
        /// You need to manually store the new value for <paramref name="waitingSince"/>
        /// (for which you may use your custom IAnimationContext).
        /// </remarks>
        /// <param name="ctx">The animation context.</param>
        /// <param name="waitingSince">Will be set to the current time.</param>
        public static void BeginWaiting(in IAnimationContext ctx, out float waitingSince)
        {
            waitingSince = ctx.animatorContext.PassedTime;
        }

        /// <summary>
        /// Checks if the waiting process is done.<br/>
        /// This will also return false if not waiting (i.e. waitingSince is -1).
        /// </summary>
        /// <remarks>
        /// In addition to the return value, the value for <paramref name="waitingSince"/> also indicates whether
        /// waiting is done (it will be set to -1).
        /// </remarks>
        /// <param name="waitTime">The amount of time to wait, in seconds.</param>
        /// <param name="ctx">The animation context.</param>
        /// <param name="waitingSince">Since when you have been waiting.</param>
        /// <returns></returns>
        public static bool TryFinishWaiting(float waitTime, in IAnimationContext ctx, ref float waitingSince)
        {
            if (waitingSince < 0) return false;
            if ((ctx.animatorContext.PassedTime - waitingSince) >= waitTime)
            {
                waitingSince = -1;
                return true;
            }
            return false;
        }
        #endregion

        #region Raw Positions & Deltas
        /// <summary>
        /// Calculate the raw version of the passed in vertex position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="index">Index of the vertex.</param>
        /// <param name="position">The position to set the vertex to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawVertex(int index, Vector3 position, CharData cData, ref IAnimationContext ctx)
        {
            return GetRawPosition(position, cData.mesh.initial[index].position, cData.info.referenceScale, ref ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in character position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the character to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawPosition(Vector3 position, CharData cData, ref IAnimationContext ctx)
        {
            return GetRawPosition(position, cData.info.initialPosition, cData.info.referenceScale, ref ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in pivot position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the pivot to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawPivot(Vector3 position, CharData cData, ref IAnimationContext ctx)
        {
            return GetRawPosition(position, cData.info.initialPosition, cData.info.referenceScale, ref ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in delta, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawDelta(Vector3 delta, CharData cData, ref IAnimationContext ctx)
        {
            if (!ctx.animatorContext.ScaleAnimations) return delta;
            return delta / cData.info.referenceScale;
        }

        internal static Vector3 GetRawPosition(Vector3 position, Vector3 referencePosition, float scale, ref IAnimationContext ctx)
        {
            if (!ctx.animatorContext.ScaleAnimations) return position;
            return (position - referencePosition) / scale + referencePosition;
        }



        /// <summary>
        /// Set the raw position of the vertex at the given index. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="index">Index of the vertex.</param>
        /// <param name="position">The position to set the vertex to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void SetVertexRaw(int index, Vector3 position, CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.ScaleAnimations)
            {
                Vector3 ogPos = cData.mesh.initial.GetVertex(index);
                cData.SetVertex(index, (position - ogPos) / cData.info.referenceScale + ogPos);
            }
            else
            {
                cData.SetVertex(index, position);
            }
        }
        /// <summary>
        /// Set the raw position of the character. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the character to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void SetPositionRaw(Vector3 position, CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.ScaleAnimations)
            {
                Vector3 ogPos = cData.info.initialPosition;
                cData.SetPosition((position - ogPos) / cData.info.referenceScale + ogPos);
            }
            else
            {
                cData.SetPosition(position);
            }
        }
        /// <summary>
        /// Set the raw pivot of the character. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the pivot to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void SetPivotRaw(Vector3 pivot, CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.ScaleAnimations)
            {
                Vector3 ogPos = cData.info.initialPosition;
                cData.SetPivot(((pivot - ogPos) / cData.info.referenceScale) + ogPos);
            }
            else
            {
                cData.SetPivot(pivot);
            }
        }
        /// <summary>
        /// Add a raw delta to the vertex at the given index. This delta will ignore the animator's scaling.
        /// </summary>
        /// <param name="index">Index of the vertex.</param>
        /// <param name="delta">The delta to add to the vertex.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void AddVertexDeltaRaw(int index, Vector3 delta, CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.ScaleAnimations)
            {
                cData.AddVertexDelta(index, delta / cData.info.referenceScale);
            }
            else
            {
                cData.AddVertexDelta(index, delta);
            }
        }
        /// <summary>
        /// Add a raw delta to the position of the character. This delta will ignore the animator's scaling.
        /// </summary>
        /// <param name="delta">The delta to add to the position of the character.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void AddPositionDeltaRaw(Vector3 delta, CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.ScaleAnimations)
            {
                cData.AddPositionDelta(delta / cData.info.referenceScale);
            }
            else
            {
                cData.AddPositionDelta(delta);
            }
        }
        /// <summary>
        /// Add a raw delta to the pivot of the character. This delta will ignore the animator's scaling.
        /// </summary>
        /// <param name="delta">The delta to add to the pivot.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void AddPivotDeltaRaw(Vector3 delta, CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.ScaleAnimations)
            {
                cData.AddPivotDelta(delta / cData.info.referenceScale);
            }
            else
            {
                cData.AddPivotDelta(delta);
            }
        }
        #endregion

        #region General Math
        public static Vector3 ClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            var vVector1 = point - lineStart;
            var vVector2 = (lineEnd - lineStart).normalized;

            var d = Vector3.Distance(lineStart, lineEnd);
            var t = Vector3.Dot(vVector2, vVector1);

            if (t <= 0)
                return lineStart;

            if (t >= d)
                return lineEnd;

            var vVector3 = vVector2 * t;

            var vClosestPoint = lineStart + vVector3;

            return vClosestPoint;
        }
        #endregion

        public static Vector2 AnchorToPosition(Vector2 anchor, CharData cData)
        {
            if (anchor == Vector2.zero)
            {
                return cData.info.initialPosition;
            }

            Vector2 dist;
            Vector2 ret = cData.info.initialPosition;

            dist.x = (cData.mesh.initial.vertex_BL.position - cData.mesh.initial.vertex_BR.position).magnitude / 2f;
            dist.y = (cData.mesh.initial.vertex_BL.position - cData.mesh.initial.vertex_TL.position).magnitude / 2f;

            ret += Vector2.right * dist.x * anchor.x;
            ret += Vector2.up * dist.y * anchor.y;
            return ret;
        }

        public class Wave
        {
            public readonly WaveProperties Properties;
            public WaveType WaveType
            {
                get => waveType;
                set => waveType = value;
            }
            public AnimationCurve UpwardCurve
            {
                get => upwardCurve;
                set => upwardCurve = value;
            }
            public AnimationCurve DownwardCurve
            {
                get => downwardCurve;
                set => downwardCurve = value;
            }
            public float UpPeriod
            {
                get => upPeriod;
                set
                {
                    upPeriod = value;
                    Properties.Period = downPeriod + upPeriod;
                }
            }
            public float DownPeriod
            {
                get => downPeriod;
                set
                {
                    downPeriod = value;
                    Properties.Period = downPeriod + upPeriod;
                }
            }
            public float ImpulseInterval;

            public Wave(WaveType type, AnimationCurve upwardCurve, AnimationCurve downwardCurve, float upPeriod, float downPeriod, float velocity, float amplitude)
            {
                WaveType = type;
                Properties = new WaveProperties(upPeriod + downPeriod, velocity, amplitude);
                UpwardCurve = upwardCurve;
                DownwardCurve = downwardCurve;
                UpPeriod = upPeriod;
                DownPeriod = downPeriod;

                starttime = -1f;
            }

            private float starttime;

            public Wave(WaveType type, AnimationCurve upwardCurve, AnimationCurve downwardCurve, float upPeriod, float downPeriod, float velocity, float amplitude, float impulseinterval)
            {
                WaveType = type;
                Properties = new WaveProperties(upPeriod + downPeriod, velocity, amplitude);
                UpwardCurve = upwardCurve;
                DownwardCurve = downwardCurve;
                UpPeriod = upPeriod;
                DownPeriod = downPeriod;
                ImpulseInterval = impulseinterval;

                starttime = -1f;
            }

            [Flags]
            public enum PulseExtrema
            {
                Early = 0b01,
                Late = 0b10,
                Both = 0b11
            }

            public int PassedExtrema(float time, float deltaTime, float offset, bool realtimeInterval = false, PulseExtrema extrema = PulseExtrema.Early)
            {
                switch (waveType)
                {
                    case WaveType.Wave: return PassedWaveExtrema(time, deltaTime, offset);
                    case WaveType.Pulse: return PassedPulseExtrema(time, deltaTime, offset, realtimeInterval, extrema);
                    case WaveType.OneDirectionalPulse: return PassedOneDirectionalPulseExtrema(time, deltaTime, offset, realtimeInterval, extrema);
                }

                throw new System.InvalidOperationException();
            }

            public int PassedWaveExtrema(float time, float deltaTime, float offset)
            {
                float t = Mathf.Abs(CalculateT(time, offset, -1));

                if (deltaTime >= Properties.Period)
                {
                    t %= Properties.Period;
                    return t < upPeriod ? -1 : 1;
                }

                float prevT = Mathf.Abs(CalculateT(time - deltaTime, offset, -1));

                if ((int)(t / Properties.Period) > (int)(prevT / Properties.Period))
                {
                    t %= Properties.Period;
                    return t < upPeriod ? -1 : 1;
                }

                prevT %= Properties.Period;
                t %= Properties.Period;

                if (prevT < upPeriod && t >= upPeriod)
                {
                    return 1;
                }

                return 0;
            }

            public int PassedPulseExtrema(float time, float deltaTime, float offset, bool realtimeInterval = false, PulseExtrema extrema = PulseExtrema.Early)
            {
                float t = CalculateT(time, offset, -1);
                float interval = (ImpulseInterval) * (realtimeInterval ? Properties.Velocity : 1f) + Properties.Period;

                if (deltaTime >= interval)
                {
                    t %= interval;
                    return t < upPeriod ? -1 : 1;
                }

                float prevT = CalculateT(time - deltaTime, offset, -1);

                if ((int)(t / interval) > (int)(prevT / interval))
                {
                    t %= interval;
                    if (t < upPeriod)
                    {
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else return 1;
                }

                prevT %= interval;
                t %= interval;

                if (prevT < upPeriod && t >= upPeriod)
                {
                    return 1;
                }

                if (prevT < Properties.Period && t >= Properties.Period)
                {
                    if (extrema.HasFlag(PulseExtrema.Early)) return -1;
                }

                return 0;
            }

            public int PassedOneDirectionalPulseExtrema(float time, float deltaTime, float offset, bool realtimeInterval = false, PulseExtrema extrema = PulseExtrema.Early)
            {
                float t = CalculateT(time, offset, -1);
                float singleInterval = (ImpulseInterval) * (realtimeInterval ? Properties.Velocity : 1f);
                float interval = singleInterval * 2 + Properties.Period;

                if (deltaTime >= interval)
                {
                    t %= interval;

                    if (t < upPeriod)
                    {
                        return -1;
                    }
                    else if ((t -= upPeriod) < singleInterval)
                    {
                        return 1;
                    }
                    else if ((t -= singleInterval) < downPeriod)
                    {
                        return 1;
                    }
                    else if ((t -= downPeriod) < singleInterval)
                    {
                        return -1;
                    }

                    throw new System.Exception("Should not be reachable");
                }

                float prevT = CalculateT(time - deltaTime, offset, -1);

                if ((int)(t / interval) > (int)(prevT / interval))
                {
                    t %= interval;
                    if (t < upPeriod)
                    {
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else if ((t - upPeriod) < singleInterval)
                    {
                        if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else if ((t - singleInterval) < downPeriod)
                    {
                        if (extrema.HasFlag(PulseExtrema.Late)) return 1;
                        if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else if ((t - downPeriod) < singleInterval)
                    {
                        if (extrema.HasFlag(PulseExtrema.Early)) return -1;
                        if (extrema.HasFlag(PulseExtrema.Late)) return 1;
                        if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }

                    throw new System.Exception("Should not be reachable");
                }

                prevT %= interval;
                t %= interval;

                interval -= singleInterval;
                if (prevT < interval && t >= interval)
                {
                    if (extrema.HasFlag(PulseExtrema.Early)) return -1;
                }

                interval -= downPeriod;
                if (prevT < interval && t >= interval)
                {
                    if (extrema.HasFlag(PulseExtrema.Late)) return 1;
                }

                interval -= singleInterval;
                if (prevT < interval && t >= interval)
                {
                    if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                }

                return 0;
            }

            public float Evaluate(float time, float offset, bool realtimeInterval = false)
            {
                switch (waveType)
                {
                    case WaveType.Wave: return EvaluateAsWave(time, offset);
                    case WaveType.Pulse: return EvaluateAsPulse(time, offset, realtimeInterval);
                    case WaveType.OneDirectionalPulse: return EvaluateAsOneDirectionalPulse(time, offset, realtimeInterval);
                }

                throw new System.InvalidOperationException();
            }

            public float EvaluateAsWave(float time, float offset)
            {
                float t = CalculateT(time, offset, -1);
                t = Mathf.Abs(t);
                t %= Properties.Period;

                if (t <= UpPeriod)
                {
                    t = Mathf.Lerp(0f, 1f, t / UpPeriod);
                    return Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, t);
                }
                else
                {
                    t = Mathf.Lerp(1f, 2f, (t - UpPeriod) / DownPeriod);
                    return Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, t);
                }
            }

            public float EvaluateAsPulse(float time, float offset, bool realtimeInterval = false)
            {
                float t = CalculateT(time, offset, -1);
                float interval = (ImpulseInterval) * (realtimeInterval ? Properties.Velocity : 1f) + Properties.Period;

                if (interval > 0)
                    t %= interval;

                if (t <= 0) return Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 0f);
                if (t <= upPeriod) return Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / upPeriod));
                if (t <= Properties.Period) return Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, (t - upPeriod) / downPeriod));
                return Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 2);
            }

            public float EvaluateAsOneDirectionalPulse(float time, float offset, bool realtimeInterval = false)
            {
                float t = CalculateT(time, offset, -1);
                float singleInterval = (ImpulseInterval) * (realtimeInterval ? Properties.Velocity : 1f);
                float interval = singleInterval * 2 + Properties.Period;

                if (interval > 0)
                    t %= interval;

                if (t <= upPeriod)
                {
                    return Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / upPeriod));
                }

                t -= upPeriod;
                if (t <= singleInterval)
                {
                    return Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1f);
                }

                t -= singleInterval;
                if (t <= downPeriod)
                {
                    return Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, t / downPeriod));
                }

                t -= downPeriod;
                if (t <= singleInterval)
                {
                    return Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, 1f));
                }

                throw new System.Exception("Shouldnt be reachable");
            }

            private float CalculateT(float time, float offset, int mult)
            {
                float t;
                if (Properties.WaveLength == 0)
                {
                    t = (time * Properties.Velocity);
                }
                else
                {
                    float v = Properties.Velocity * Properties.WaveLength;
                    t = (time * v) / Properties.WaveLength + (offset / Properties.WaveLength) * mult;
                }

                return t;
            }

            private WaveType waveType;
            private AnimationCurve upwardCurve;
            private AnimationCurve downwardCurve;
            private float upPeriod;
            private float downPeriod;
        }

        public class WaveProperties
        {
            public float WaveLength
            {
                get => wavelength;
                set
                {
                    wavelength = value;
                    frequency = velocity / wavelength;
                    period = 1 / frequency;
                }
            }

            public float Period
            {
                get => period;
                set
                {
                    period = value;
                    frequency = 1f / period;
                    wavelength = velocity * period;
                }
            }

            public float Frequency
            {
                get => frequency;
                set
                {
                    frequency = value;
                    period = 1f / frequency;
                    wavelength = velocity * period;
                }
            }

            public float Velocity
            {
                get => velocity;
                set
                {
                    velocity = value;
                    wavelength = velocity / frequency;
                    frequency = velocity / wavelength;
                    period = 1 / frequency;
                }
            }

            public float Amplitude
            {
                get => amplitude;
                set => amplitude = value;
            }

            public WaveProperties()
            {
                WaveLength = 1f;
                Period = 1f;
                Frequency = 1f;
                Velocity = 1f;
                Amplitude = 1f;
            }

            public WaveProperties(float waveLength, float period, float frequency, float velocity, float amplitude) : this()
            {
                WaveLength = waveLength;
                Period = period;
                Frequency = frequency;
                Velocity = velocity;
                Amplitude = amplitude;
            }

            public WaveProperties(float period, float velocity, float amplitude) : this()
            {
                Period = period;
                Velocity = velocity;
                Amplitude = amplitude;
            }

            private float period;
            private float frequency;
            private float wavelength;
            private float velocity;
            private float amplitude;
        }



        //[System.Serializable]
        //public class Wave
        //{
        //    public WaveType WaveType => waveType;
        //    public float WaveLength => waveLength;
        //    public float DownMultiplier => downMultiplier;
        //    public float UpMultiplier => upMultiplier;
        //    public float WaveVelocity => waveVelocity;
        //    public float Amplitude => amplitude;
        //    public float ImpulseInterval => impulseInterval;
        //    public AnimationCurve UpwardCurve => upwardCurve;
        //    public AnimationCurve DownwardCurve => downwardCurve;

        //    [SerializeField] WaveType waveType;
        //    [SerializeField] float waveLength;
        //    [SerializeField] float downMultiplier;
        //    [SerializeField] float upMultiplier;
        //    [SerializeField] float waveVelocity;
        //    [SerializeField] float amplitude;
        //    [SerializeField] float impulseInterval;
        //    [SerializeField] AnimationCurve upwardCurve;
        //    [SerializeField] AnimationCurve downwardCurve;

        //    public Wave() : this(WaveType.Wave, AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 1f, 1f, 1f, 1f, 1f, 0f)
        //    { }

        //    public Wave(WaveType type) : this(type, AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 1f, 1f, 1f, 1f, 1f, 0f)
        //    { }

        //    public Wave(WaveType type, AnimationCurve upwardCurve, AnimationCurve downwardCurve) : this(type, upwardCurve, downwardCurve, 1f, 1f, 1f, 1f, 1f, 0f)
        //    { }

        //    public Wave(WaveType type, AnimationCurve upwardCurve, AnimationCurve downwardCurve, float wavelength, float wavevelocity, float amplitude) : this(type, upwardCurve, downwardCurve, wavelength, wavevelocity, amplitude, 1f, 1f, 0f)
        //    { }

        //    public Wave(WaveType type, AnimationCurve upwardCurve, AnimationCurve downwardCurve, float wavelength, float wavevelocity, float amplitude, float impulseinterval) : this(type, upwardCurve, downwardCurve, wavelength, wavevelocity, amplitude, 1f, 1f, impulseinterval)
        //    { }

        //    public Wave(WaveType type, AnimationCurve upwardCurve, AnimationCurve downwardCurve, float wavelength, float wavevelocity, float amplitude, float upMultiplier, float downMultiplier) : this(type, upwardCurve, downwardCurve, wavelength, wavevelocity, amplitude, upMultiplier, downMultiplier, 0f)
        //    { }

        //    public Wave(WaveType type, AnimationCurve upwardCurve, AnimationCurve downwardCurve, float wavelength, float wavevelocity, float amplitude, float upMultiplier, float downMultiplier, float impulseinterval)
        //    {
        //        if (upwardCurve == null) throw new System.ArgumentNullException(nameof(upwardCurve));
        //        if (downwardCurve == null) throw new System.ArgumentNullException(nameof(downwardCurve));

        //        this.waveType = type;
        //        this.waveLength = wavelength;
        //        this.downMultiplier = downMultiplier;
        //        this.upMultiplier = upMultiplier;
        //        this.waveVelocity = wavevelocity;
        //        this.amplitude = amplitude;
        //        this.impulseInterval = impulseinterval;
        //        this.upwardCurve = upwardCurve;
        //        this.downwardCurve = downwardCurve;
        //    }

        //    public float Evaluate(float time, CharData cData, bool realtimeInterval = false)
        //    {
        //        switch (WaveType)
        //        {
        //            case WaveType.Wave: return EvaluateAsWave(time, cData);
        //            case WaveType.Pulse: return EvaluateAsPulse(time, cData, realtimeInterval);
        //            case WaveType.OneDirectionalPulse: return EvaluateAsOneDirectionalPulse(time, cData, realtimeInterval);
        //        }

        //        throw new System.InvalidOperationException();
        //    }

        //    public float Evaluate(float time, int segmentIndex, bool realtimeInterval = false)
        //    {
        //        switch (WaveType)
        //        {
        //            case WaveType.Wave: return EvaluateAsWave(time, segmentIndex);
        //            case WaveType.Pulse: return EvaluateAsPulse(time, segmentIndex, realtimeInterval);
        //            case WaveType.OneDirectionalPulse: return EvaluateAsOneDirectionalPulse(time, segmentIndex, realtimeInterval);
        //        }

        //        throw new System.InvalidOperationException();
        //    }


        //    public float EvaluateAsWave(float time, CharData cData)
        //    {
        //        float t = CalculateT(time, cData, -1);

        //        if (t % 2 < 1)
        //        {
        //            return Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * upMultiplier, 0f, 1f));
        //        }
        //        else
        //        {
        //            return Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * downMultiplier + 1f, 0f, 2f));
        //        }
        //    }

        //    public float EvaluateAsWave(float time, int segmentIndex)
        //    {
        //        float t = CalculateT(time, segmentIndex, -1);

        //        if (t % 2 < 1)
        //        {
        //            return Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * upMultiplier, 0f, 1f));
        //        }
        //        else
        //        {
        //            return Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * downMultiplier + 1f, 0f, 2f));
        //        }
        //    }

        //    public float EvaluateAsPulse(float time, CharData cData, bool realtimeInterval = false)
        //    {
        //        float t = CalculateT(time, cData, -1);
        //        float interval = (ImpulseInterval + 2f) * (realtimeInterval ? WaveVelocity : 1f);

        //        t %= interval;

        //        if (t <= 0) return Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 0f);
        //        if (t <= 1) return Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * upMultiplier, 0f, 1f));
        //        if (t <= 2) return Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * downMultiplier + 1f, 0f, 2f));
        //        return Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 2);
        //    }

        //    public float EvaluateAsPulse(float time, int segmentIndex, bool realtimeInterval = false)
        //    {
        //        float t = CalculateT(time, segmentIndex, -1);
        //        float interval = (ImpulseInterval + 2f) * (realtimeInterval ? WaveVelocity : 1f);

        //        t %= interval;

        //        if (t <= 0) return Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 0f);
        //        if (t <= 1) return Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * upMultiplier, 0f, 1f));
        //        if (t <= 2) return Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * downMultiplier + 1f, 0f, 2f));
        //        return Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 2);
        //    }

        //    public float EvaluateAsOneDirectionalPulse(float time, CharData cData, bool realtimeInterval = false)
        //    {
        //        float t = CalculateT(time, cData, -1);
        //        float interval = (ImpulseInterval + 1f) * (realtimeInterval ? WaveVelocity : 1f);

        //        bool invert = (t / interval) % 2 <= 1;
        //        t %= interval;

        //        if (t <= 0) return Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1f);
        //        if (t <= 1)
        //        {
        //            if (invert)
        //            {
        //                return Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * downMultiplier + 1f, 0f, 2f));
        //            }
        //            else
        //            {
        //                return Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * upMultiplier, 0f, 1f));
        //            }
        //        }
        //        return Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, invert ? 0f : 1f);
        //    }

        //    public float EvaluateAsOneDirectionalPulse(float time, int segmentIndex, bool realtimeInterval = false)
        //    {
        //        float t = CalculateT(time, segmentIndex, -1);
        //        float interval = (ImpulseInterval + 1f) * (realtimeInterval ? WaveVelocity : 1f);

        //        bool invert = (t / interval) % 2 <= 1;
        //        t %= interval;

        //        if (t <= 0) return Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1f); if (t <= 1)
        //        {
        //            if (invert)
        //            {
        //                return Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * downMultiplier + 1f, 0f, 2f));
        //            }
        //            else
        //            {
        //                return Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Clamp((t - (int)t) * upMultiplier, 0f, 1f));
        //            }
        //        }
        //        return Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, invert ? 0f : 1f);
        //    }


        //    public float CalculateT(float time, CharData cData, int mult)
        //    {
        //        // Xpos is roughly approximated to look similar to index based T;
        //        // Couldnt figure out the exact logic behind referenceScale yet
        //        // so will stay rough approximation for now
        //        float xPos = cData.info.initialPosition.x;
        //        xPos /= (cData.info.referenceScale / 36f);

        //        float t;
        //        if (WaveLength == 0)
        //        {
        //            t = time * WaveVelocity;
        //        }
        //        else
        //        {
        //            float v = WaveVelocity * WaveLength;
        //            t = (time * v) / WaveLength + (xPos / 2000f / WaveLength) * mult;
        //        }

        //        return t;
        //    }

        //    public float CalculateT(float time, int segmentIndex, int mult)
        //    {
        //        float t;
        //        if (WaveLength == 0)
        //        {
        //            t = time * WaveVelocity;
        //        }
        //        else
        //        {
        //            float v = WaveVelocity * WaveLength;
        //            t = (time * v) / WaveLength + (segmentIndex / WaveLength) * mult;
        //        }

        //        return t;
        //    }

        //    #region Setters
        //    public void SetWaveType(WaveType waveType)
        //    {
        //        this.waveType = waveType;
        //    }

        //    public void SetWaveLength(float waveLength)
        //    {
        //        this.waveLength = waveLength;
        //    }

        //    public void SetWaveVelocity(float waveVelocity)
        //    {
        //        this.waveVelocity = waveVelocity;
        //    }

        //    public void SetAmplitude(float amplitude)
        //    {
        //        this.amplitude = amplitude;
        //    }

        //    public void SetImpulseInterval(float impulseInterval)
        //    {
        //        this.impulseInterval = Mathf.Max(impulseInterval, 0f);
        //    }

        //    public void SetUpwardCurve(AnimationCurve upwardCurve)
        //    {
        //        this.upwardCurve = upwardCurve;
        //    }

        //    public void SetDownwardCurve(AnimationCurve downwardCurve)
        //    {
        //        this.downwardCurve = downwardCurve;
        //    }
        //    #endregion
        //}

        public static float FrequencyToPeriod(float frequency)
        {
            return 1f / frequency;
        }

        public static float PeriodToFrequency(float period)
        {
            return 1f / period;
        }

        public static float WaveLengthVelocityToFrequency(float wavelength, float wavevelocity)
        {
            return wavevelocity / wavelength;
        }

        public static float WaveLengthFrequencyToVelocity(float wavelength, float frequency)
        {
            return frequency * wavelength;
        }

        public static float WaveVelocityFrequencyToLength(float wavevelocity, float frequency)
        {
            return wavevelocity / frequency;
        }

        public static void HandleWave(ParameterUtility.WaveType type, float frequency, float period, float amplitude, Func<float, float> evaluate, Action<float> action)
        {
            switch (type)
            {
                case ParameterUtility.WaveType.Wave: HandleWave_Impl(frequency, period, amplitude, evaluate, action); break;
                case ParameterUtility.WaveType.Pulse: HandlePulse(frequency, period, amplitude, evaluate, action); break;
                case ParameterUtility.WaveType.OneDirectionalPulse: HandleOneDirectionalPulse(frequency, period, amplitude, evaluate, action); break;
                default: throw new System.ArgumentException(nameof(type));
            }
        }

        private static void HandleWave_Impl(float frequency, float period, float amplitude, Func<float, float> evaluate, Action<float> action)
        {
        }

        private static void HandlePulse(float frequency, float period, float amplitude, Func<float, float> evaluate, Action<float> action)
        {

        }

        private static void HandleOneDirectionalPulse(float frequency, float period, float amplitude, Func<float, float> evaluate, Action<float> action)
        {

        }

        public static float GetValue(AnimationCurve curve, WrapMode wrapMode, float time)
        {
            float t;
            switch (wrapMode)
            {
                case WrapMode.Loop:
                    t = Mathf.Repeat(time, 1);
                    return curve.Evaluate(t);
                case WrapMode.PingPong:
                    t = Mathf.PingPong(time, 1);
                    return curve.Evaluate(t);
                case WrapMode.Once:
                    // TODO This absolutely does not belong here
                    //if (time > 1)
                    //{
                    //    context.FinishAnimation(cData);
                    //    return 1;
                    //}
                    return curve.Evaluate(time);

                default: throw new System.ArgumentException("WrapMode " + wrapMode.ToString() + " not supported");
            }
        }
    }
}

