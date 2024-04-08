using UnityEngine;
using TMPEffects.CharacterData;
using System;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Utility methods for animations.
    /// </summary>
    public static class AnimationUtility
    {
        #region Raw Positions & Deltas
        /// <summary>
        /// Convert an anchor to its actual position.<br/>
        /// Since it is inherently based on the character's vertex positions,
        /// the resulting position automatically ignores <see cref="TMPAnimator's"/> scaling;
        /// no need to call <see cref="GetRawPosition(Vector3, CharData, IAnimationContext)"/> on it.
        /// </summary>
        /// <param name="anchor"></param>
        /// <param name="cData"></param>
        /// <returns></returns>
        public static Vector2 AnchorToPosition(Vector2 anchor, CharData cData)
        {
            if (anchor == Vector2.zero)
            {
                return cData.InitialPosition;
            }

            Vector2 dist;
            Vector2 ret = cData.InitialPosition;

            dist.x = (cData.mesh.initial.BL_Position - cData.mesh.initial.BR_Position).magnitude / 2f;
            dist.y = (cData.mesh.initial.BL_Position - cData.mesh.initial.TL_Position).magnitude / 2f;

            ret += Vector2.right * dist.x * anchor.x;
            ret += Vector2.up * dist.y * anchor.y;
            return ret;
        }


        /// <summary>
        /// Calculate the raw version of the passed in vertex position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="index">Index of the vertex.</param>
        /// <param name="position">The position to set the vertex to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawVertex(int index, Vector3 position, CharData cData, IAnimationContext ctx)
        {
            return GetRawPosition(position, cData.initialMesh.GetPosition(index), cData.info.referenceScale, ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in character position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the character to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawPosition(Vector3 position, CharData cData, IAnimationContext ctx)
        {
            return GetRawPosition(position, cData.InitialPosition, cData.info.referenceScale, ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in pivot position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the pivot to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawPivot(Vector3 position, CharData cData, IAnimationContext ctx)
        {
            return GetRawPosition(position, cData.InitialPosition, cData.info.referenceScale, ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in delta, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawDelta(Vector3 delta, CharData cData, IAnimationContext ctx)
        {
            if (!ctx.AnimatorContext.ScaleAnimations) return delta;
            return delta / cData.info.referenceScale;
        }

        internal static Vector3 GetRawPosition(Vector3 position, Vector3 referencePosition, float scale, IAnimationContext ctx)
        {
            if (!ctx.AnimatorContext.ScaleAnimations) return position;
            return (position - referencePosition) / scale + referencePosition;
        }



        /// <summary>
        /// Set the raw position of the vertex at the given index. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="index">Index of the vertex.</param>
        /// <param name="position">The position to set the vertex to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void SetVertexRaw(int index, Vector3 position, CharData cData, IAnimationContext ctx)
        {
            if (ctx.AnimatorContext.ScaleAnimations)
            {
                Vector3 ogPos = cData.initialMesh.GetPosition(index);
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
        public static void SetPositionRaw(Vector3 position, CharData cData, IAnimationContext ctx)
        {
            if (ctx.AnimatorContext.ScaleAnimations)
            {
                Vector3 ogPos = cData.InitialPosition;
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
        public static void SetPivotRaw(Vector3 pivot, CharData cData, IAnimationContext ctx)
        {
            if (ctx.AnimatorContext.ScaleAnimations)
            {
                Vector3 ogPos = cData.InitialPosition;
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
        public static void AddVertexDeltaRaw(int index, Vector3 delta, CharData cData, IAnimationContext ctx)
        {
            if (ctx.AnimatorContext.ScaleAnimations)
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
        public static void AddPositionDeltaRaw(Vector3 delta, CharData cData, IAnimationContext ctx)
        {
            if (ctx.AnimatorContext.ScaleAnimations)
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
        public static void AddPivotDeltaRaw(Vector3 delta, CharData cData, IAnimationContext ctx)
        {
            if (ctx.AnimatorContext.ScaleAnimations)
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

        #region Waves
        [System.Serializable]
        public abstract class WaveBase : ISerializationCallbackReceiver
        {
            public float UpPeriod
            {
                get => upPeriod;
                set
                {
                    if (value < 0f) throw new System.ArgumentException(nameof(UpPeriod) + " may not be negative");
                    if (value + downPeriod <= 0) throw new System.ArgumentException("The sum of " + nameof(UpPeriod) + " and " + nameof(DownPeriod) + " must be larger than zero");

                    upPeriod = value;
                    period = upPeriod + downPeriod;
                    frequency = 1f / period;
                    wavelength = velocity * period;

                    if (Velocity == 0)
                    {
                        adjustedPeriod = period;
                        adjustedUpPeriod = upPeriod;
                    }
                    else
                    {
                        adjustedPeriod = period * Velocity;
                        adjustedUpPeriod = upPeriod * Velocity;
                    }
                }
            }

            public float DownPeriod
            {
                get => downPeriod;
                set
                {
                    if (value < 0f) throw new System.ArgumentException(nameof(DownPeriod) + " may not be negative");
                    if (value + upPeriod <= 0) throw new System.ArgumentException("The sum of " + nameof(UpPeriod) + " and " + nameof(DownPeriod) + " must be larger than zero");

                    downPeriod = value;
                    period = upPeriod + downPeriod;
                    frequency = 1f / period;
                    wavelength = velocity * period;

                    if (Velocity == 0)
                    {
                        adjustedPeriod = period;
                        adjustedDownPeriod = downPeriod;
                    }
                    else
                    {
                        adjustedPeriod = period * Velocity;
                        adjustedDownPeriod = downPeriod * Velocity;
                    }
                }
            }

            public float Amplitude
            {
                get => amplitude;
                set => amplitude = value;
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

                    UpPeriod = upPeriod;
                    DownPeriod = downPeriod;
                }
            }

            public float Period
            {
                get => period;
            }

            public float WaveLength
            {
                get => wavelength;
            }

            public float EffectiveUpPeriod
            {
                get => adjustedUpPeriod;
            }

            public float EffectiveDownPeriod
            {
                get => adjustedDownPeriod;
            }

            public float EffectivePeriod
            {
                get => adjustedPeriod;
            }

            public float Frequency
            {
                get => frequency;
            }

            public WaveBase() : this(1f, 1f, 1f, 1f)
            { }

            public WaveBase(float upPeriod, float downPeriod, float velocity, float amplitude)
            {
                this.upPeriod = 1f;
                this.downPeriod = 1f;
                this.amplitude = 1f;
                this.velocity = 1f;

                period = 1f;
                adjustedPeriod = 1f;
                adjustedUpPeriod = 1f;
                adjustedDownPeriod = 1f;

                frequency = 1f;
                wavelength = 1f;

                UpPeriod = upPeriod;
                DownPeriod = downPeriod;
                Velocity = velocity;
                Amplitude = amplitude;
            }

            [Tooltip("The time it takes for the wave to travel from trough to crest, or from its lowest to its highest point, in seconds")]
            [SerializeField] private float upPeriod;
            [Tooltip("The time it takes for the wave to travel from crest to trough, or from its highest to its lowest point, in seconds")]
            [SerializeField] private float downPeriod;
            [Tooltip("The amplitude of the wave")]
            [SerializeField] private float amplitude;
            [SerializeField, HideInInspector] private float velocity;

            [System.NonSerialized] private float period;
            [System.NonSerialized] private float adjustedPeriod;
            [System.NonSerialized] private float adjustedUpPeriod; 
            [System.NonSerialized] private float adjustedDownPeriod;
            [System.NonSerialized] private float frequency;
            [System.NonSerialized] public float wavelength;

            public virtual void OnBeforeSerialize()
            {
                upPeriod = Mathf.Max(upPeriod, 0f);
                downPeriod = Mathf.Max(downPeriod, 0f);
                if (downPeriod + upPeriod == 0) upPeriod = 0.1f;
                velocity = Mathf.Max(velocity, 0.001f);

                // Setting the velocity will set frequency and wavelength,
                // and also call the setter of DownPeriod and UpPeriod,
                // which will set all remaining values
                Velocity = velocity;
            }

            public virtual void OnAfterDeserialize()
            {
                upPeriod = Mathf.Max(upPeriod, 0f);
                downPeriod = Mathf.Max(downPeriod, 0f);
                if (downPeriod + upPeriod == 0) upPeriod = 0.1f;
                velocity = Mathf.Max(velocity, 0.001f);

                // Setting the velocity will set frequency and wavelength,
                // and also call the setter of DownPeriod and UpPeriod,
                // which will set all remaining values
                Velocity = velocity;
            }
        }

        
        [System.Serializable]
        public class Wave : WaveBase, ISerializationCallbackReceiver
        {
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

            public float CrestWait
            {
                get => crestWait;
                set => crestWait = value;
            }
            public float TroughWait
            {
                get => troughWait;
                set => troughWait = value;
            }

            public float Uniformity
            {
                get => uniformity;
                set => uniformity = value;
            }

            public Wave() : this(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 1f, 1f, 1f, 0f, 0f, 1f)
            { }


            public Wave(AnimationCurve upwardCurve, AnimationCurve downwardCurve, float upPeriod, float downPeriod, float amplitude, float uniformity = 1f) : base(upPeriod, downPeriod, 1f, amplitude)
            {
                if (upwardCurve == null) throw new System.ArgumentNullException(nameof(upwardCurve));
                if (downwardCurve == null) throw new System.ArgumentNullException(nameof(downwardCurve));
                if (upPeriod < 0) throw new System.ArgumentException(nameof(upPeriod) + " may not be negative");
                if (downPeriod < 0) throw new System.ArgumentException(nameof(downPeriod) + " may not be negative");
                if ((upPeriod + downPeriod) <= 0) throw new System.ArgumentException("The sum of " + nameof(upPeriod) + " and " + nameof(downPeriod) + " must be larger than zero");

                this.uniformity = uniformity;
                UpwardCurve = upwardCurve;
                DownwardCurve = downwardCurve;
            }

            public Wave(AnimationCurve upwardCurve, AnimationCurve downwardCurve, float upPeriod, float downPeriod, float amplitude, float crestWait, float troughWait, float uniformity = 1f)
                : this(upwardCurve, downwardCurve, upPeriod, downPeriod, amplitude, uniformity)
            {
                if (crestWait < 0) throw new System.ArgumentException(nameof(crestWait) + " may not be negative");
                if (TroughWait < 0) throw new System.ArgumentException(nameof(TroughWait) + " may not be negative");

                CrestWait = crestWait;
                TroughWait = troughWait;
            }


            public int PassedExtrema(float time, float deltaTime, float offset, bool realtimeWait = true, PulseExtrema extrema = PulseExtrema.Early)
            {
                if (CrestWait <= 0)
                {
                    if (TroughWait <= 0)
                    {
                        return PassedWaveExtrema(time, deltaTime, offset);
                    }

                    return PassedPulseExtrema(time, deltaTime, offset, realtimeWait, extrema);
                }

                if (TroughWait <= 0)
                    return PassedInvertedPulseExtrema(time, deltaTime, offset, realtimeWait, extrema);

                return PassedOneDirectionalPulseExtrema(time, deltaTime, offset, realtimeWait, extrema);
            }

            public int PassedWaveExtrema(float time, float deltaTime, float offset)
            {
                float t = CalculateWaveT(time, offset, -1);

                if (deltaTime >= EffectivePeriod)
                {
                    t %= EffectivePeriod;
                    return t < EffectiveUpPeriod ? -1 : 1;
                }

                float prevT = CalculateWaveT(time - deltaTime, offset, -1);

                if ((int)(t / EffectivePeriod) > (int)(prevT / EffectivePeriod))
                {
                    t %= EffectivePeriod;
                    return t < EffectiveUpPeriod ? -1 : 1;
                }

                prevT %= EffectivePeriod;
                t %= EffectivePeriod;

                if (prevT < EffectiveUpPeriod && t >= EffectiveUpPeriod)
                {
                    return 1;
                }

                return 0;
            }

            public int PassedPulseExtrema(float time, float deltaTime, float offset, bool realtimeWait = true, PulseExtrema extrema = PulseExtrema.Early)
            {
                float t = CalculatePulseT(time, offset, -1);
                float interval = (TroughWait) * (realtimeWait ? Velocity : 1f) + EffectivePeriod;

                if (deltaTime >= interval)
                {
                    t %= interval;
                    if (t < EffectiveUpPeriod) return -1;
                    if (t < EffectivePeriod) return 1;
                    return -1;
                    //return t < EffectiveUpPeriod ? -1 : 1;
                }

                float prevT = CalculatePulseT(time - deltaTime, offset, -1);

                if ((int)(t / interval) > (int)(prevT / interval))
                {
                    t %= interval;
                    if (t < EffectiveUpPeriod)
                    {
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else return 1;
                }

                prevT %= interval;
                t %= interval;

                if (prevT < EffectiveUpPeriod && t >= EffectiveUpPeriod)
                {
                    return 1;
                }

                if (prevT < EffectivePeriod && t >= EffectivePeriod)
                {
                    if (extrema.HasFlag(PulseExtrema.Early)) return -1;
                }

                return 0;
            }

            public int PassedInvertedPulseExtrema(float time, float deltaTime, float offset, bool realtimeWait = true, PulseExtrema extrema = PulseExtrema.Early)
            {
                float t = CalculatePulseT(time, offset, -1);
                float interval = (CrestWait) * (realtimeWait ? Velocity : 1f) + EffectivePeriod;

                if (deltaTime >= interval)
                {
                    t %= interval;
                    if (t < EffectiveDownPeriod) return 1;
                    if (t < EffectivePeriod) return -1;
                    return 1;
                }

                float prevT = CalculatePulseT(time - deltaTime, offset, -1);

                if ((int)(t / interval) > (int)(prevT / interval))
                {
                    t %= interval;
                    if (t < EffectiveDownPeriod)
                    {
                        if (extrema.HasFlag(PulseExtrema.Late)) return 1;
                        return 0;
                    }
                    else return -1;
                }

                prevT %= interval;
                t %= interval;

                if (prevT < EffectiveDownPeriod && t >= EffectiveDownPeriod)
                {
                    return -1;
                }

                if (prevT < EffectivePeriod && t >= EffectivePeriod)
                {
                    if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                }

                return 0;
            }

            public int PassedOneDirectionalPulseExtrema(float time, float deltaTime, float offset, bool realtimeWait = true, PulseExtrema extrema = PulseExtrema.Early)
            {
                float t = CalculatePulseT(time, offset, -1);
                float upInterval = (CrestWait) * (realtimeWait ? Velocity : 1f);
                float downInterval = (TroughWait) * (realtimeWait ? Velocity : 1f);
                float interval = upInterval + downInterval + EffectivePeriod;

                if (deltaTime >= interval)
                {
                    float ogt = t;

                    if (interval > 0)
                        t %= interval;

                    if (t <= EffectiveUpPeriod)
                    {
                        return -1;
                    }
                    else if ((t -= EffectiveUpPeriod) <= upInterval)
                    {
                        return 1;
                    }
                    else if ((t -= upInterval) <= EffectiveDownPeriod)
                    {
                        return 1;
                    }
                    else if ((t -= EffectiveDownPeriod) <= downInterval)
                    {
                        return -1;
                    }

                    throw new System.Exception("Should not be reachable with og t " + ogt + " and interval " + interval + "; final t  " + t);
                }

                float prevT = CalculatePulseT(time - deltaTime, offset, -1);

                if ((int)(t / interval) > (int)(prevT / interval))
                {
                    t %= interval;
                    if (t < EffectiveUpPeriod)
                    {
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else if ((t - EffectiveUpPeriod) < upInterval)
                    {
                        if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else if ((t - upInterval) < EffectiveDownPeriod)
                    {
                        if (extrema.HasFlag(PulseExtrema.Late)) return 1;
                        if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else if ((t - EffectiveDownPeriod) < downInterval)
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

                interval -= upInterval;
                if (prevT < interval && t >= interval)
                {
                    if (extrema.HasFlag(PulseExtrema.Early)) return -1;
                }

                interval -= EffectiveDownPeriod;
                if (prevT < interval && t >= interval)
                {
                    if (extrema.HasFlag(PulseExtrema.Late)) return 1;
                }

                interval -= downInterval;
                if (prevT < interval && t >= interval)
                {
                    if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                }

                return 0;
            }


            public (float, int) Evaluate(float time, float offset, bool realtimeWait = true)
            {
                if (CrestWait <= 0)
                {
                    if (TroughWait <= 0)
                    {
                        return EvaluateAsWave(time, offset);
                    }

                    return EvaluateAsPulse(time, offset, realtimeWait);
                }

                if (TroughWait <= 0) return EvaluateAsInvertedPulse(time, offset, realtimeWait);

                return EvaluateAsOneDirectionalPulse(time, offset, realtimeWait);
            }

            public (float, int) EvaluateAsWave(float time, float offset)
            {
                float t = CalculateWaveT(time, offset, -1);
                t = Mathf.Abs(t);
                t %= EffectivePeriod;

                if (t <= EffectiveUpPeriod)
                {
                    t = Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod);
                    return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, t), 1);
                }
                else
                {
                    t = Mathf.Lerp(1f, 2f, (t - EffectiveUpPeriod) / EffectiveDownPeriod);
                    return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, t), -1);
                }
            }

            public (float, int) EvaluateAsPulse(float time, float offset, bool realTimeWait = true)
            {
                float t = CalculatePulseT(time, offset, -1);
                float interval = (TroughWait) * (realTimeWait ? Velocity : 1f) + EffectivePeriod;

                if (interval > 0)
                    t %= interval;

                if (t <= 0) return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 0f), 1);
                if (t <= EffectiveUpPeriod) return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod)), 1);
                if (t <= (EffectivePeriod)) return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, (t - EffectiveUpPeriod) / EffectiveDownPeriod)), -1);
                return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 2f), -1);
            }

            public (float, int) EvaluateAsInvertedPulse(float time, float offset, bool realTimeWait = true)
            {
                float t = CalculatePulseT(time, offset, -1);
                float interval = (CrestWait) * (realTimeWait ? Velocity : 1f) + EffectivePeriod;

                if (interval > 0)
                    t %= interval;

                if (t <= 0) return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 1f), -1);
                if (t <= EffectiveDownPeriod) return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, t / EffectiveDownPeriod)), -1);
                if (t <= EffectivePeriod) return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, (t - EffectiveDownPeriod) / EffectiveUpPeriod)), 1);
                return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1f), 1);
            }

            public (float, int) EvaluateAsOneDirectionalPulse(float time, float offset, bool realTimeWait = true)
            {
                float t = CalculatePulseT(time, offset, -1);
                float upInterval = (CrestWait) * (realTimeWait ? Velocity : 1f);
                float downInterval = (TroughWait) * (realTimeWait ? Velocity : 1f);
                float interval = upInterval + downInterval + EffectivePeriod;

                if (interval > 0)
                    t %= interval;

                if (t <= EffectiveUpPeriod)
                {
                    return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod)), 1);
                }

                t -= EffectiveUpPeriod;
                if (t <= upInterval)
                {
                    return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1f), 1);
                }

                t -= upInterval;
                if (t <= EffectiveDownPeriod)
                {
                    return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, t / EffectiveDownPeriod)), -1);
                }

                t -= EffectiveDownPeriod;
                if (t <= downInterval)
                {
                    return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, 1f)), -1);
                }

                throw new System.Exception("Shouldnt be reachable");
            }

            [Flags]
            public enum PulseExtrema
            {
                Early = 0b01,
                Late = 0b10,
                Both = 0b11
            }

            private float CalculateWaveT(float time, float offset, int mult)
            {
                return Mathf.Abs(CalculateT(time, offset, mult));
            }

            private float CalculatePulseT(float time, float offset, int mult)
            {
                float t = CalculateT(time, offset, mult);
                return Mathf.Max(t, 0f);
            }

            private float CalculateT(float time, float offset, int mult)
            {
                float t;
                if (WaveLength == 0)
                {
                    t = (time * Velocity);
                }
                else
                {
                    float v = Velocity * WaveLength;
                    t = (time * v) / WaveLength + (offset / WaveLength) * mult * uniformity;
                }

                return t;
            }

            public override void OnBeforeSerialize()
            {
                base.OnBeforeSerialize();

                if (upwardCurve == null || upwardCurve.keys.Length == 0) upwardCurve = AnimationCurveUtility.EaseInOutSine();
                if (downwardCurve == null || downwardCurve.keys.Length == 0) downwardCurve = AnimationCurveUtility.EaseInOutSine();

                crestWait = Mathf.Max(crestWait, 0f);
                troughWait = Mathf.Max(troughWait, 0f);
            }

            public override void OnAfterDeserialize()
            {
                base.OnAfterDeserialize();

                if (upwardCurve == null || upwardCurve.keys.Length == 0) upwardCurve = AnimationCurveUtility.EaseInOutSine();
                if (downwardCurve == null || downwardCurve.keys.Length == 0) downwardCurve = AnimationCurveUtility.EaseInOutSine();

                troughWait = Mathf.Max(troughWait, 0f);
                crestWait = Mathf.Max(crestWait, 0f);

                //EvaluateCopy(0f, 0f, true);
            }

            //public (float, int) EvaluateCopy(float time, float offset, bool realtimeWait = true)
            //{
            //    if (CrestWait <= 0)
            //    {
            //        if (TroughWait <= 0)
            //        {
            //            return EvaluateAsWave(time, offset);
            //        }

            //        return EvaluateAsPulse(time, offset, realtimeWait);
            //    }

            //    if (TroughWait <= 0) return EvaluateAsInvertedPulse(time, offset, realtimeWait);

            //    return EvaluateAsOneDirectionalPulse(time, offset, realtimeWait);
            //}

            [Tooltip("The \"up\" part of the wave. This is the curve that is used to travel from trough to crest, or from the wave's lowest to its highest point.")]
            [SerializeField] private AnimationCurve upwardCurve;
            [Tooltip("The \"down\" part of the wave. This is the curve that is used to travel from crest to trough, or from the wave's highest to its lowest point.")]
            [SerializeField] private AnimationCurve downwardCurve;
            [Tooltip("The amount of time to remain at the crest before moving down again, in seconds.")]
            [SerializeField] private float crestWait;
            [Tooltip("The amount of time to remain at the trough before moving up again, in seconds.")]
            [SerializeField] private float troughWait;
            [Tooltip("The uniformity of the wave. The closer to zero, the more uniform the wave is applied to the effected characters.")]
            [SerializeField] private float uniformity;
        }


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

        public enum WaveOffsetType
        {
            SegmentIndex = 0,
            Index = 5,
            XPos = 10,
            YPos = 15
        }

        public static float GetWaveOffset(CharData cData, IAnimationContext context, WaveOffsetType type)
        {
            switch (type)
            {
                case WaveOffsetType.SegmentIndex: return context.SegmentData.SegmentIndexOf(cData);
                case WaveOffsetType.Index: return cData.info.index;
                case WaveOffsetType.XPos:
                    float pos = cData.InitialPosition.x;
                    pos /= (cData.info.referenceScale / 36f);
                    pos /= 2000f; 
                    return pos;
                case WaveOffsetType.YPos:
                    pos = cData.InitialPosition.y;
                    pos /= (cData.info.referenceScale / 36f);
                    pos /= 2000f;
                    return pos;
            }

            throw new System.ArgumentException(nameof(type));
        }
        #endregion

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
                    return curve.Evaluate(time);

                default: throw new System.ArgumentException("WrapMode " + wrapMode.ToString() + " not supported");
            }
        }
    }
}

