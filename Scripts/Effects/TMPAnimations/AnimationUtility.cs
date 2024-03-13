using UnityEngine;
using TMPEffects.Components.CharacterData;
using System;
using TMPEffects.Extensions;
using Unity.Collections;

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

        #region Waves
        [System.Serializable]
        public class Wave : ISerializationCallbackReceiver
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

            public WaveProperties Properties
            {
                get => properties;
            }


            public Wave() : this(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 1f, 1f, 1f, 1f, 0f, 0f, 1f)
            { }

            public Wave(AnimationCurve upwardCurve, AnimationCurve downwardCurve, float upPeriod, float downPeriod, float velocity, float amplitude, float uniformity = 1f)
            {
                if (upwardCurve == null) throw new System.ArgumentNullException(nameof(upwardCurve));
                if (downwardCurve == null) throw new System.ArgumentNullException(nameof(downwardCurve));
                if (upPeriod < 0) throw new System.ArgumentException(nameof(upPeriod) + " may not be negative");
                if (downPeriod < 0) throw new System.ArgumentException(nameof(downPeriod) + " may not be negative");
                if ((upPeriod + downPeriod) <= 0) throw new System.ArgumentException("The sum of " + nameof(upPeriod) + " and " + nameof(downPeriod) + " must be larger than zero");
                if (velocity < 0) throw new System.ArgumentException(nameof(velocity) + " may not be negative");

                this.uniformity = uniformity;
                properties = new WaveProperties(upPeriod, downPeriod, velocity, amplitude);
                UpwardCurve = upwardCurve;
                DownwardCurve = downwardCurve;
            }

            public Wave(AnimationCurve upwardCurve, AnimationCurve downwardCurve, float upPeriod, float downPeriod, float velocity, float amplitude, float crestWait, float troughWait, float uniformity = 1f)
                : this(upwardCurve, downwardCurve, upPeriod, downPeriod, velocity, amplitude, uniformity)
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
                float t = Mathf.Abs(CalculateT(time, offset, -1));

                if (deltaTime >= properties.EffectivePeriod)
                {
                    t %= properties.EffectivePeriod;
                    return t < properties.EffectiveUpPeriod ? -1 : 1;
                }

                float prevT = Mathf.Abs(CalculateT(time - deltaTime, offset, -1));

                if ((int)(t / properties.EffectivePeriod) > (int)(prevT / properties.EffectivePeriod))
                {
                    t %= properties.EffectivePeriod;
                    return t < properties.EffectiveUpPeriod ? -1 : 1;
                }

                prevT %= properties.EffectivePeriod;
                t %= properties.EffectivePeriod;

                if (prevT < properties.EffectiveUpPeriod && t >= properties.EffectiveUpPeriod)
                {
                    return 1;
                }

                return 0;
            }

            public int PassedPulseExtrema(float time, float deltaTime, float offset, bool realtimeWait = true, PulseExtrema extrema = PulseExtrema.Early)
            {
                float t = CalculateT(time, offset, -1);
                float interval = (TroughWait) * (realtimeWait ? properties.Velocity : 1f) + properties.EffectivePeriod;

                if (deltaTime >= interval)
                {
                    t %= interval;
                    if (t < properties.EffectiveUpPeriod) return -1;
                    if (t < properties.EffectivePeriod) return 1;
                    return -1;
                    //return t < EffectiveUpPeriod ? -1 : 1;
                }

                float prevT = CalculateT(time - deltaTime, offset, -1);

                if ((int)(t / interval) > (int)(prevT / interval))
                {
                    t %= interval;
                    if (t < properties.EffectiveUpPeriod)
                    {
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else return 1;
                }

                prevT %= interval;
                t %= interval;

                if (prevT < properties.EffectiveUpPeriod && t >= properties.EffectiveUpPeriod)
                {
                    return 1;
                }

                if (prevT < properties.EffectivePeriod && t >= properties.EffectivePeriod)
                {
                    if (extrema.HasFlag(PulseExtrema.Early)) return -1;
                }

                return 0;
            }

            public int PassedInvertedPulseExtrema(float time, float deltaTime, float offset, bool realtimeWait = true, PulseExtrema extrema = PulseExtrema.Early)
            {
                float t = CalculateT(time, offset, -1);
                float interval = (CrestWait) * (realtimeWait ? properties.Velocity : 1f) + properties.EffectivePeriod;

                if (deltaTime >= interval)
                {
                    t %= interval;
                    if (t < properties.EffectiveDownPeriod) return 1;
                    if (t < properties.EffectivePeriod) return -1;
                    return 1;
                }

                float prevT = CalculateT(time - deltaTime, offset, -1);

                if ((int)(t / interval) > (int)(prevT / interval))
                {
                    t %= interval;
                    if (t < properties.EffectiveDownPeriod)
                    {
                        if (extrema.HasFlag(PulseExtrema.Late)) return 1;
                        return 0;
                    }
                    else return -1;
                }

                prevT %= interval;
                t %= interval;

                if (prevT < properties.EffectiveDownPeriod && t >= properties.EffectiveDownPeriod)
                {
                    return -1;
                }

                if (prevT < properties.EffectivePeriod && t >= properties.EffectivePeriod)
                {
                    if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                }

                return 0;
            }

            public int PassedOneDirectionalPulseExtrema(float time, float deltaTime, float offset, bool realtimeWait = true, PulseExtrema extrema = PulseExtrema.Early)
            {
                float t = CalculateT(time, offset, -1);
                float upInterval = (CrestWait) * (realtimeWait ? properties.Velocity : 1f);
                float downInterval = (TroughWait) * (realtimeWait ? properties.Velocity : 1f);
                float interval = upInterval + downInterval + properties.EffectivePeriod;

                if (deltaTime >= interval)
                {
                    float ogt = t;

                    if (interval > 0)
                        t %= interval;

                    if (t <= properties.EffectiveUpPeriod)
                    {
                        return -1;
                    }
                    else if ((t -= properties.EffectiveUpPeriod) <= upInterval)
                    {
                        return 1;
                    }
                    else if ((t -= upInterval) <= properties.EffectiveDownPeriod)
                    {
                        return 1;
                    }
                    else if ((t -= properties.EffectiveDownPeriod) <= downInterval)
                    {
                        return -1;
                    }

                    throw new System.Exception("Should not be reachable with og t " + ogt + " and interval " + interval + "; final t  " + t);
                }

                float prevT = CalculateT(time - deltaTime, offset, -1);

                if ((int)(t / interval) > (int)(prevT / interval))
                {
                    t %= interval;
                    if (t < properties.EffectiveUpPeriod)
                    {
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else if ((t - properties.EffectiveUpPeriod) < upInterval)
                    {
                        if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else if ((t - upInterval) < properties.EffectiveDownPeriod)
                    {
                        if (extrema.HasFlag(PulseExtrema.Late)) return 1;
                        if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                        if (extrema.HasFlag(PulseExtrema.Late)) return -1;
                        return 0;
                    }
                    else if ((t - properties.EffectiveDownPeriod) < downInterval)
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

                interval -= properties.EffectiveDownPeriod;
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
                float t = CalculateT(time, offset, -1);
                t = Mathf.Abs(t);
                t %= properties.EffectivePeriod;

                if (t <= properties.EffectiveUpPeriod)
                {
                    t = Mathf.Lerp(0f, 1f, t / properties.EffectiveUpPeriod);
                    return (properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, t), 1);
                }
                else
                {
                    t = Mathf.Lerp(1f, 2f, (t - properties.EffectiveUpPeriod) / properties.EffectiveDownPeriod);
                    return (properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, t), -1);
                }
            }

            public (float, int) EvaluateAsPulse(float time, float offset, bool realTimeWait = false)
            {
                float t = CalculateT(time, offset, -1);
                float interval = (TroughWait) * (realTimeWait ? properties.Velocity : 1f) + properties.EffectivePeriod;

                if (interval > 0)
                    t %= interval;

                if (t <= 0) return (properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 0f), 1);
                if (t <= properties.EffectiveUpPeriod) return (properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / properties.EffectiveUpPeriod)), 1);
                if (t <= (properties.EffectivePeriod)) return (properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, (t - properties.EffectiveUpPeriod) / properties.EffectiveDownPeriod)), -1);
                return (properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 2f), -1);
            }

            public (float, int) EvaluateAsInvertedPulse(float time, float offset, bool realTimeWait = false)
            {
                float t = CalculateT(time, offset, -1);
                float interval = (CrestWait) * (realTimeWait ? properties.Velocity : 1f) + properties.EffectivePeriod;

                if (interval > 0)
                    t %= interval;

                if (t <= 0) return (properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 0f), -1);
                if (t <= properties.EffectiveDownPeriod) return (properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, t / properties.EffectiveDownPeriod)), -1);
                if (t <= properties.EffectivePeriod) return (properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, (t - properties.EffectiveDownPeriod) / properties.EffectiveUpPeriod)), 1);
                return (properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1f), 1);
            }

            public (float, int) EvaluateAsOneDirectionalPulse(float time, float offset, bool realTimeWait = false)
            {
                float t = CalculateT(time, offset, -1);
                float upInterval = (CrestWait) * (realTimeWait ? properties.Velocity : 1f);
                float downInterval = (TroughWait) * (realTimeWait ? properties.Velocity : 1f);
                float interval = upInterval + downInterval + properties.EffectivePeriod;

                if (interval > 0)
                    t %= interval;

                if (t <= properties.EffectiveUpPeriod)
                {
                    return (properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / properties.EffectiveUpPeriod)), 1);
                }

                t -= properties.EffectiveUpPeriod;
                if (t <= upInterval)
                {
                    return (properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1f), 1);
                }

                t -= upInterval;
                if (t <= properties.EffectiveDownPeriod)
                {
                    return (properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, t / properties.EffectiveDownPeriod)), -1);
                }

                t -= properties.EffectiveDownPeriod;
                if (t <= downInterval)
                {
                    return (properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, 1f)), -1);
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

            private float CalculateT(float time, float offset, int mult)
            {
                float t;
                if (properties.WaveLength == 0)
                {
                    t = (time * properties.Velocity);
                }
                else
                {
                    float v = properties.Velocity * properties.WaveLength;
                    t = (time * v) / properties.WaveLength + (offset / properties.WaveLength) * mult * uniformity;
                }

                return t;
            }

            public void OnBeforeSerialize()
            {
                if (upwardCurve == null || upwardCurve.keys.Length == 0) upwardCurve = AnimationCurveUtility.EaseInOutSine();
                if (downwardCurve == null || downwardCurve.keys.Length == 0) downwardCurve = AnimationCurveUtility.EaseInOutSine();

                crestWait = Mathf.Max(crestWait, 0f);
                troughWait = Mathf.Max(troughWait, 0f);
            }

            public void OnAfterDeserialize()
            {
                if (upwardCurve == null || upwardCurve.keys.Length == 0) upwardCurve = AnimationCurveUtility.EaseInOutSine();
                if (downwardCurve == null || downwardCurve.keys.Length == 0) downwardCurve = AnimationCurveUtility.EaseInOutSine();

                troughWait = Mathf.Max(troughWait, 0f);
                crestWait = Mathf.Max(crestWait, 0f);

                EvaluateCopy(0f, 0f, true);
            }

            public (float, int) EvaluateCopy(float time, float offset, bool realtimeWait = true)
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

            [SerializeField] private WaveProperties properties;
            [SerializeField] private AnimationCurve upwardCurve;
            [SerializeField] private AnimationCurve downwardCurve;
            [SerializeField] private float crestWait;
            [SerializeField] private float troughWait;
            [SerializeField] private float uniformity;
        }

        [System.Serializable]
        public class WaveProperties : ISerializationCallbackReceiver
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

            public WaveProperties() : this(1f, 1f, 1f, 1f)
            { }

            public WaveProperties(float upPeriod, float downPeriod, float velocity, float amplitude)
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


            [SerializeField] private float upPeriod;
            [SerializeField] private float downPeriod;
            [SerializeField] private float amplitude;
            [SerializeField] private float velocity;

            [System.NonSerialized] private float period;
            [System.NonSerialized] private float adjustedPeriod;
            [System.NonSerialized] private float adjustedUpPeriod;
            [System.NonSerialized] private float adjustedDownPeriod;
            [System.NonSerialized] private float frequency;
            [System.NonSerialized] public float wavelength;

            public void OnBeforeSerialize()
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

            public void OnAfterDeserialize()
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
















        //internal class WaveProperties
        //{
        //    public float WaveLength
        //    {
        //        get => wavelength;
        //        set
        //        {
        //            wavelength = value;
        //            frequency = velocity / wavelength;
        //            period = 1 / frequency;
        //        }
        //    }

        //    public float Period
        //    {
        //        get => period;
        //        set
        //        {
        //            period = value;
        //            frequency = 1f / period;
        //            wavelength = velocity * period;
        //        }
        //    }

        //    public float Frequency
        //    {
        //        get => frequency;
        //        set
        //        {
        //            frequency = value;
        //            period = 1f / frequency;
        //            wavelength = velocity * period;
        //        }
        //    }

        //    public float Velocity
        //    {
        //        get => velocity;
        //        set
        //        {
        //            velocity = value;
        //            wavelength = velocity / frequency;
        //            frequency = velocity / wavelength;
        //            period = 1 / frequency;
        //        }
        //    }

        //    public float Amplitude
        //    {
        //        get => amplitude;
        //        set => amplitude = value;
        //    }

        //    public WaveProperties()
        //    {
        //        WaveLength = 1f;
        //        Period = 1f;
        //        Frequency = 1f;
        //        Velocity = 1f;
        //        Amplitude = 1f;
        //    }

        //    public WaveProperties(float waveLength, float period, float frequency, float velocity, float amplitude) : this()
        //    {
        //        WaveLength = waveLength;
        //        Period = period;
        //        Frequency = frequency;
        //        Velocity = velocity;
        //        Amplitude = amplitude;
        //    }

        //    public WaveProperties(float period, float velocity, float amplitude) : this()
        //    {
        //        Period = period;
        //        Velocity = velocity;
        //        Amplitude = amplitude;
        //    }

        //    private float period;
        //    private float frequency;
        //    private float wavelength;
        //    private float velocity;
        //    private float amplitude;
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

