using UnityEngine;
using TMPEffects.CharacterData;
using System;
using TMPEffects.Extensions;
using TMPEffects.Components.Animator;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEngine.TextCore;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Utility methods for animations.
    /// </summary>
    public static class AnimationUtility
    {
        public static Vector3 ScaleVector(Vector3 vector, CharData cData, IAnimationContext context) => ScaleVector(vector, cData, context.AnimatorContext);
        public static Vector3 ScaleVector(Vector3 vector, CharData cData, IAnimatorContext context)
        {
            if (!context.ScaleAnimations) return vector;
            if (!context.ScaleUniformly) return vector * (cData.info.pointSize / 36f);
            return vector * (context.Animator.TextComponent.fontSize / 36f);
        }


        public static Vector3 InverseScaleVector(Vector3 vector, CharData cData, IAnimationContext context) => InverseScaleVector(vector, cData, context.AnimatorContext);
        public static Vector3 InverseScaleVector(Vector3 vector, CharData cData, IAnimatorContext context)
        {
            if (!context.ScaleAnimations) return vector;
            if (!context.ScaleUniformly) return vector / (cData.info.pointSize / 36f);
            return vector / (context.Animator.TextComponent.fontSize / 36f);
        }


        #region Raw Positions & Deltas
        /// <summary>
        /// Convert an anchor to its actual position.
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

            Vector2 up = (cData.initialMesh.TL_Position - cData.initialMesh.BL_Position) / 2f;
            Vector2 right = (cData.initialMesh.BR_Position - cData.initialMesh.BL_Position) / 2f;

            dist.x = (cData.mesh.initial.BL_Position - cData.mesh.initial.BR_Position).magnitude / 2f;
            dist.y = (cData.mesh.initial.BL_Position - cData.mesh.initial.TL_Position).magnitude / 2f;

            ret += right * anchor.x;
            ret += up * anchor.y;
            return ret;
        }


        /// <summary>
        /// Calculate the raw version of the passed in vertex position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="index">Index of the vertex.</param>
        /// <param name="position">The position to set the vertex to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawVertex(int index, Vector3 position, CharData cData, IAnimationContext ctx) => GetRawVertex(index, position, cData, ctx.AnimatorContext);
        public static Vector3 GetRawVertex(int index, Vector3 position, CharData cData, IAnimatorContext ctx)
        {
            return GetRawPosition(position, cData.initialMesh.GetPosition(index), cData, ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in character position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the character to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawPosition(Vector3 position, CharData cData, IAnimationContext ctx) => GetRawPosition(position, cData, ctx.AnimatorContext);
        public static Vector3 GetRawPosition(Vector3 position, CharData cData, IAnimatorContext ctx)
        {
            return GetRawPosition(position, cData.InitialPosition, cData, ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in pivot position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the pivot to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawPivot(Vector3 position, CharData cData, IAnimationContext ctx) => GetRawPivot(position, cData, ctx.AnimatorContext);
        public static Vector3 GetRawPivot(Vector3 position, CharData cData, IAnimatorContext ctx)
        {
            return GetRawPosition(position, cData.InitialPosition, cData, ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in delta, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static Vector3 GetRawDelta(Vector3 delta, CharData cData, IAnimationContext ctx) => GetRawDelta(delta, cData, ctx.AnimatorContext);
        public static Vector3 GetRawDelta(Vector3 delta, CharData cData, IAnimatorContext ctx)
        {
            return InverseScaleVector(delta, cData, ctx);
        }

        internal static Vector3 GetRawPosition(Vector3 position, Vector3 referencePosition, CharData cData, IAnimatorContext ctx)
        {
            return InverseScaleVector(position - referencePosition, cData, ctx) + referencePosition;
        }



        /// <summary>
        /// Set the raw position of the vertex at the given index. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="index">Index of the vertex.</param>
        /// <param name="position">The position to set the vertex to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void SetVertexRaw(int index, Vector3 position, CharData cData, IAnimationContext ctx) => SetVertexRaw(index, position, cData, ctx.AnimatorContext);
        public static void SetVertexRaw(int index, Vector3 position, CharData cData, IAnimatorContext ctx)
        {
            Vector3 ogPos = cData.initialMesh.GetPosition(index);
            cData.SetVertex(index, GetRawPosition(position, ogPos, cData, ctx));
        }
        /// <summary>
        /// Set the raw position of the character. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the character to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void SetPositionRaw(Vector3 position, CharData cData, IAnimationContext ctx) => SetPositionRaw(position, cData, ctx.AnimatorContext);
        public static void SetPositionRaw(Vector3 position, CharData cData, IAnimatorContext ctx)
        {
            Vector3 ogPos = cData.InitialPosition;
            cData.SetPosition(GetRawPosition(position, ogPos, cData, ctx));
        }
        /// <summary>
        /// Set the raw pivot of the character. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the pivot to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void SetPivotRaw(Vector3 pivot, CharData cData, IAnimationContext ctx) => SetPivotRaw(pivot, cData, ctx.AnimatorContext);
        public static void SetPivotRaw(Vector3 pivot, CharData cData, IAnimatorContext ctx)
        {
            Vector3 ogPos = cData.InitialPosition;
            cData.SetPivot(GetRawPosition(pivot, ogPos, cData, ctx));
        }
        /// <summary>
        /// Add a raw delta to the vertex at the given index. This delta will ignore the animator's scaling.
        /// </summary>
        /// <param name="index">Index of the vertex.</param>
        /// <param name="delta">The delta to add to the vertex.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void AddVertexDeltaRaw(int index, Vector3 delta, CharData cData, IAnimationContext ctx) => AddVertexDeltaRaw(index, delta, cData, ctx.AnimatorContext);
        public static void AddVertexDeltaRaw(int index, Vector3 delta, CharData cData, IAnimatorContext ctx)
        {
            cData.AddVertexDelta(index, GetRawDelta(delta, cData, ctx));
        }
        /// <summary>
        /// Add a raw delta to the position of the character. This delta will ignore the animator's scaling.
        /// </summary>
        /// <param name="delta">The delta to add to the position of the character.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void AddPositionDeltaRaw(Vector3 delta, CharData cData, IAnimationContext ctx) => AddPositionDeltaRaw(delta, cData, ctx.AnimatorContext);
        public static void AddPositionDeltaRaw(Vector3 delta, CharData cData, IAnimatorContext ctx)
        {
            cData.AddPositionDelta(GetRawDelta(delta, cData, ctx)); 
        }
        /// <summary>
        /// Add a raw delta to the pivot of the character. This delta will ignore the animator's scaling.
        /// </summary>
        /// <param name="delta">The delta to add to the pivot.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The animation context.</param>
        public static void AddPivotDeltaRaw(Vector3 delta, CharData cData, IAnimationContext ctx) => AddPivotDeltaRaw(delta, cData, ctx.AnimatorContext);
        public static void AddPivotDeltaRaw(Vector3 delta, CharData cData, IAnimatorContext ctx)
        {
            cData.AddPivotDelta(GetRawDelta(delta, cData, ctx));
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
                float interval = (TroughWait) * (realtimeWait ? Velocity : 1f) + EffectivePeriod;
                float t = CalculatePulseT(time, offset, interval, -1);

                if (deltaTime >= interval)
                {
                    t %= interval;
                    if (t < EffectiveUpPeriod) return -1;
                    if (t < EffectivePeriod) return 1;
                    return -1;
                    //return t < EffectiveUpPeriod ? -1 : 1;
                }

                float prevT = CalculatePulseT(time - deltaTime, offset, interval, -1);

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
                float interval = (CrestWait) * (realtimeWait ? Velocity : 1f) + EffectivePeriod;
                float t = CalculatePulseT(time, offset, interval, -1);

                if (deltaTime >= interval)
                {
                    t %= interval;
                    if (t < EffectiveDownPeriod) return 1;
                    if (t < EffectivePeriod) return -1;
                    return 1;
                }

                float prevT = CalculatePulseT(time - deltaTime, offset, interval, -1);

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
                float upInterval = (CrestWait) * (realtimeWait ? Velocity : 1f);
                float downInterval = (TroughWait) * (realtimeWait ? Velocity : 1f);
                float interval = upInterval + downInterval + EffectivePeriod;
                float t = CalculatePulseT(time, offset, interval, -1);

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

                float prevT = CalculatePulseT(time - deltaTime, offset, interval, -1);

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
                float interval = (TroughWait) * (realTimeWait ? Velocity : 1f) + EffectivePeriod;
                float t = CalculatePulseT(time, offset, interval, -1);

                if (interval > 0)
                    t %= interval;

                if (t <= 0) return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 0f), 1);
                if (t <= EffectiveUpPeriod) return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod)), 1);
                if (t <= (EffectivePeriod)) return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, (t - EffectiveUpPeriod) / EffectiveDownPeriod)), -1);
                return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 2f), -1);
            }

            public (float, int) EvaluateAsInvertedPulse(float time, float offset, bool realTimeWait = true)
            {
                float interval = (CrestWait) * (realTimeWait ? Velocity : 1f) + EffectivePeriod;
                float t = CalculatePulseT(time, offset, interval, -1);

                if (interval > 0)
                    t %= interval;

                if (t <= 0) return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 1f), -1);
                if (t <= EffectiveDownPeriod) return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, t / EffectiveDownPeriod)), -1);
                if (t <= EffectivePeriod) return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, (t - EffectiveDownPeriod) / EffectiveUpPeriod)), 1);
                return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1f), 1);
            }

            public (float, int) EvaluateAsOneDirectionalPulse(float time, float offset, bool realTimeWait = true)
            {
                float upInterval = (CrestWait) * (realTimeWait ? Velocity : 1f);
                float downInterval = (TroughWait) * (realTimeWait ? Velocity : 1f);
                float interval = upInterval + downInterval + EffectivePeriod;
                float t = CalculatePulseT(time, offset, interval, -1);

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

                throw new System.Exception("Shouldnt be reachable (interval = " + interval + ")");
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

            private float CalculatePulseT(float time, float offset, float interval, int mult)
            {
                float t = CalculateT(time, offset, mult);
                if (t < 0)
                {
                    int times = (int)Math.Ceiling(Mathf.Abs(t / interval));
                    t += times * interval;
                }
                return t;
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
            YPos = 15,


            // new
            Word,
            Line,
            Baseline
        }

        public static float GetWaveOffset(CharData cData, IAnimationContext context, WaveOffsetType type)
        {
            switch (type)
            {
                case WaveOffsetType.SegmentIndex: return context.SegmentData.SegmentIndexOf(cData);
                case WaveOffsetType.Index: return cData.info.index;

                case WaveOffsetType.Line: return cData.info.lineNumber;
                case WaveOffsetType.Baseline: return cData.info.baseLine;
                case WaveOffsetType.Word: return (cData.info.wordFirstIndex + cData.info.wordLastIndex) / 2f;

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

        public static void SetToCharacter(TMP_Character newCharacter, TMP_Character originalCharacter, CharData cData, IAnimationContext context)
        {
            float baseSpriteScale = originalCharacter.scale * originalCharacter.glyph.scale;
            Vector2 origin = new Vector2(cData.info.origin, cData.info.baseLine);
            float spriteScale = cData.info.referenceScale / baseSpriteScale * newCharacter.scale * newCharacter.glyph.scale;

            Vector3 bl = new Vector3(origin.x + newCharacter.glyph.metrics.horizontalBearingX * spriteScale, origin.y + (newCharacter.glyph.metrics.horizontalBearingY - newCharacter.glyph.metrics.height) * spriteScale);
            Vector3 tl = new Vector3(bl.x, origin.y + newCharacter.glyph.metrics.horizontalBearingY * spriteScale);
            Vector3 tr = new Vector3(origin.x + (newCharacter.glyph.metrics.horizontalBearingX + newCharacter.glyph.metrics.width) * spriteScale, tl.y);
            Vector3 br = new Vector3(tr.x, bl.y);

            var fontAsset = cData.info.fontAsset;

            Vector2 uv0 = new Vector2((float)newCharacter.glyph.glyphRect.x / fontAsset.atlasWidth, (float)newCharacter.glyph.glyphRect.y / fontAsset.atlasHeight);
            Vector2 uv1 = new Vector2(uv0.x, (float)(newCharacter.glyph.glyphRect.y + newCharacter.glyph.glyphRect.height) / fontAsset.atlasHeight);
            Vector2 uv2 = new Vector2((float)(newCharacter.glyph.glyphRect.x + newCharacter.glyph.glyphRect.width) / fontAsset.atlasWidth, uv1.y);
            Vector2 uv3 = new Vector2(uv2.x, uv0.y);

            SetVertexRaw(0, bl, cData, context);
            SetVertexRaw(1, tl, cData, context);
            SetVertexRaw(2, tr, cData, context);
            SetVertexRaw(3, br, cData, context);

            cData.mesh.SetUV0(0, uv0);
            cData.mesh.SetUV0(1, uv1);
            cData.mesh.SetUV0(2, uv2);
            cData.mesh.SetUV0(3, uv3);
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
                    return curve.Evaluate(time);

                default: throw new System.ArgumentException("WrapMode " + wrapMode.ToString() + " not supported");
            }
        }
    }
}

