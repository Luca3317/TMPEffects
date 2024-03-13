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
        public class Wave
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

            public float UpPeriod
            {
                get => upPeriod;
                set
                {
                    if (value < 0f) throw new System.ArgumentException(nameof(UpPeriod) + " may not be negative");
                    if (value + downPeriod <= 0) throw new System.ArgumentException("The sum of " + nameof(UpPeriod) + " and " + nameof(DownPeriod) + " must be larger than zero");

                    upPeriod = value;
                    Properties.Period = downPeriod + upPeriod;

                    if (Velocity == 0)
                    {
                        EffectivePeriod = Period;
                        EffectiveUpPeriod = upPeriod;
                    }
                    else
                    {
                        EffectivePeriod = Period * Velocity;
                        EffectiveUpPeriod = upPeriod * Velocity;
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
                    Properties.Period = downPeriod + upPeriod;

                    if (Velocity == 0)
                    {
                        EffectivePeriod = Period;
                        EffectiveDownPeriod = downPeriod;
                    }
                    else
                    {
                        EffectivePeriod = Period * Velocity;
                        EffectiveDownPeriod = downPeriod * Velocity;
                    }
                }
            }
            public float EffectivePeriod { get; private set; }
            public float EffectiveUpPeriod { get; private set; }
            public float EffectiveDownPeriod { get; private set; }
            public float Period
            {
                get => Properties.Period;
            }
            public float Velocity
            {
                get => Properties.Velocity;
                set
                {
                    Properties.Velocity = value;
                    UpPeriod = upPeriod;
                    DownPeriod = downPeriod;
                }
            }
            public float Amplitude
            {
                get => Properties.Amplitude;
                set => Properties.Amplitude = value;
            }
            public float CrestWait { get; set; }
            public float TroughWait { get; set; }

            public Wave(AnimationCurve upwardCurve, AnimationCurve downwardCurve, float upPeriod, float downPeriod, float velocity, float amplitude)
            {
                if (upwardCurve == null) throw new System.ArgumentNullException(nameof(upwardCurve));
                if (downwardCurve == null) throw new System.ArgumentNullException(nameof(downwardCurve));
                if (upPeriod < 0) throw new System.ArgumentException(nameof(upPeriod) + " may not be negative");
                if (downPeriod < 0) throw new System.ArgumentException(nameof(downPeriod) + " may not be negative");
                if ((upPeriod + downPeriod) <= 0) throw new System.ArgumentException("The sum of " + nameof(upPeriod) + " and " + nameof(downPeriod) + " must be larger than zero");
                if (velocity < 0) throw new System.ArgumentException(nameof(velocity) + " may not be negative");

                Properties = new WaveProperties(upPeriod + downPeriod, velocity, amplitude);
                UpwardCurve = upwardCurve;
                DownwardCurve = downwardCurve;
                this.upPeriod = upPeriod;
                this.downPeriod = downPeriod;

                UpPeriod = upPeriod;
                DownPeriod = downPeriod;
            }

            public Wave(AnimationCurve upwardCurve, AnimationCurve downwardCurve, float upPeriod, float downPeriod, float velocity, float amplitude, float crestWait, float troughWait)
                : this(upwardCurve, downwardCurve, upPeriod, downPeriod, velocity, amplitude)
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

                if (deltaTime >= EffectivePeriod)
                {
                    t %= EffectivePeriod;
                    return t < EffectiveUpPeriod ? -1 : 1;
                }

                float prevT = Mathf.Abs(CalculateT(time - deltaTime, offset, -1));

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
                float t = CalculateT(time, offset, -1);
                float interval = (TroughWait) * (realtimeWait ? Properties.Velocity : 1f) + EffectivePeriod;

                if (deltaTime >= interval)
                {
                    t %= interval;
                    if (t < EffectiveUpPeriod) return -1;
                    if (t < EffectivePeriod) return 1;
                    return -1;
                    //return t < EffectiveUpPeriod ? -1 : 1;
                }

                float prevT = CalculateT(time - deltaTime, offset, -1);

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
                float t = CalculateT(time, offset, -1);
                float interval = (CrestWait) * (realtimeWait ? Properties.Velocity : 1f) + EffectivePeriod;

                if (deltaTime >= interval)
                {
                    t %= interval;
                    if (t < EffectiveDownPeriod) return 1;
                    if (t < EffectivePeriod) return -1;
                    return 1;
                }

                float prevT = CalculateT(time - deltaTime, offset, -1);

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
                float t = CalculateT(time, offset, -1);
                float upInterval = (CrestWait) * (realtimeWait ? Properties.Velocity : 1f);
                float downInterval = (TroughWait) * (realtimeWait ? Properties.Velocity : 1f);
                float interval = upInterval + downInterval + EffectivePeriod;

                if (deltaTime >= interval)
                {
                    //if (offset == 0)
                    //    Debug.LogWarning("Case 0!");
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

                float prevT = CalculateT(time - deltaTime, offset, -1);

                if ((int)(t / interval) > (int)(prevT / interval))
                {
                    //if (offset == 0)
                    //    Debug.LogWarning("Case 2");
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
                    //if (offset == 0)
                    //    Debug.Log("Case 5");
                    if (extrema.HasFlag(PulseExtrema.Early)) return -1;
                }

                interval -= EffectiveDownPeriod;
                if (prevT < interval && t >= interval)
                {
                    //if (offset == 0)
                    //    Debug.Log("Case 6");
                    if (extrema.HasFlag(PulseExtrema.Late)) return 1;
                }

                interval -= downInterval;
                if (prevT < interval && t >= interval)
                {
                    //if (offset == 0)
                    //    Debug.Log("Case 7");
                    if (extrema.HasFlag(PulseExtrema.Early)) return 1;
                }

                //interval -= downInterval;
                //if (prevT < interval && t >= interval)
                //{
                //    Debug.Log("Case 5");
                //    if (extrema.HasFlag(PulseExtrema.Early))
                //    {
                //        Debug.Log("Forreal");
                //        return -1;
                //    }
                //}

                //interval -= EffectiveDownPeriod;
                //if (prevT < interval && t >= interval)
                //{
                //    Debug.Log("Case 6");
                //    if (extrema.HasFlag(PulseExtrema.Late))
                //    {
                //        Debug.Log("Forreal");
                //        return 1;
                //    }
                //}

                //interval -= upInterval;
                //if (prevT < interval && t >= interval)
                //{
                //    Debug.Log("Case 7");
                //    if (extrema.HasFlag(PulseExtrema.Early))
                //    {
                //        Debug.Log("Forreal");
                //        return 1;
                //    }
                //}

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
                t %= EffectivePeriod;

                if (t <= EffectiveUpPeriod)
                {
                    t = Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod);
                    return (Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, t), 1);
                }
                else
                {
                    t = Mathf.Lerp(1f, 2f, (t - EffectiveUpPeriod) / EffectiveDownPeriod);
                    return (Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, t), -1);
                }
            }

            public (float, int) EvaluateAsPulse(float time, float offset, bool realTimeWait = false)
            {
                float t = CalculateT(time, offset, -1);
                float interval = (TroughWait) * (realTimeWait ? Properties.Velocity : 1f) + EffectivePeriod;

                if (interval > 0)
                    t %= interval;

                if (t <= 0) return (Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 0f), 1);
                if (t <= EffectiveUpPeriod) return (Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod)), 1);
                if (t <= (EffectivePeriod)) return (Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, (t - EffectiveUpPeriod) / EffectiveDownPeriod)), -1);
                return (Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 2f), -1);
            }

            public (float, int) EvaluateAsInvertedPulse(float time, float offset, bool realTimeWait = false)
            {
                float t = CalculateT(time, offset, -1);
                float interval = (CrestWait) * (realTimeWait ? Properties.Velocity : 1f) + EffectivePeriod;

                if (interval > 0)
                    t %= interval;

                if (t <= 0) return (Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 0f), -1);
                if (t <= EffectiveDownPeriod) return (Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, t / EffectiveDownPeriod)), -1);
                if (t <= EffectivePeriod) return (Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, (t - EffectiveDownPeriod) / EffectiveUpPeriod)), 1);
                return (Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1f), 1);
            }

            public (float, int) EvaluateAsOneDirectionalPulse(float time, float offset, bool realTimeWait = false)
            {
                float t = CalculateT(time, offset, -1);
                float upInterval = (CrestWait) * (realTimeWait ? Properties.Velocity : 1f);
                float downInterval = (TroughWait) * (realTimeWait ? Properties.Velocity : 1f);
                float interval = upInterval + downInterval + EffectivePeriod;

                if (interval > 0)
                    t %= interval;

                if (t <= EffectiveUpPeriod)
                {
                    return (Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod)), 1);
                }

                t -= EffectiveUpPeriod;
                if (t <= upInterval)
                {
                    return (Properties.Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1f), 1);
                }

                t -= upInterval;
                if (t <= EffectiveDownPeriod)
                {
                    return (Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, t / EffectiveDownPeriod)), -1);
                }

                t -= EffectiveDownPeriod;
                if (t <= downInterval)
                {
                    return (Properties.Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, 1f)), -1);
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
                if (Properties.WaveLength == 0) return 0f;

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

            private readonly WaveProperties Properties;
            private AnimationCurve upwardCurve;
            private AnimationCurve downwardCurve;
            private float upPeriod;
            private float downPeriod;
        }

        internal class WaveProperties
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

