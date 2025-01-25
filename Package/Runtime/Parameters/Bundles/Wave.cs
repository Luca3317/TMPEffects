using System;
using TMPEffects.Extensions;
using TMPEffects.Parameters.Attributes;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Parameters
{
    /// <summary>
    /// Allows you to easily create periodic animations.<br/>
    /// The class is explained in detail <a href="https://tmpeffects.luca3317.dev/manual/tmpanimator_animationutility_wave.html">here</a>.
    /// </summary>
    [System.Serializable]
    [TMPParameterBundle("Wave")]
    public partial class Wave : ISerializationCallbackReceiver
    {
        /// <summary>
        /// The up period of the wave; how long it takes to travel up the wave.<br/>
        /// <see cref="UpPeriod"/> does NOT contain either wait periods.<br/>
        /// Ignores the <see cref="Velocity"/> of the wave, if you want to know it'll actually
        /// take to travel up the wave, use <see cref="EffectiveUpPeriod"/>.
        /// </summary>
        public float UpPeriod
        {
            get => upPeriod;
            set
            {
                if (value < 0f) throw new System.ArgumentException(nameof(UpPeriod) + " may not be negative");
                if (value + downPeriod <= 0)
                    throw new System.ArgumentException("The sum of " + nameof(UpPeriod) + " and " +
                                                       nameof(DownPeriod) + " must be larger than zero");

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

        /// <summary> 
        /// The down period of the wave; how long it takes to travel down the wave.<br/>
        /// <see cref="DownPeriod"/> does NOT contain either wait periods.<br/>
        /// Ignores the <see cref="Velocity"/> of the wave, if you want to know it'll actually
        /// take to travel down the wave, use <see cref="EffectiveDownPeriod"/>.
        /// </summary>
        public float DownPeriod
        {
            get => downPeriod;
            set
            {
                if (value < 0f) throw new System.ArgumentException(nameof(DownPeriod) + " may not be negative");
                if (value + upPeriod <= 0)
                    throw new System.ArgumentException("The sum of " + nameof(UpPeriod) + " and " +
                                                       nameof(DownPeriod) + " must be larger than zero");

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

        /// <summary>
        /// The amplitude of the wave.
        /// </summary>
        public float Amplitude
        {
            get => amplitude;
            set => amplitude = value;
        }

        /// <summary>
        /// The velocity at which the wave travels.
        /// </summary>
        public float Velocity
        {
            get => velocity;
            private set
            {
                velocity = value;
                wavelength = velocity / frequency;
                frequency = velocity / wavelength;
                period = 1 / frequency;

                UpPeriod = upPeriod;
                DownPeriod = downPeriod;
            }
        }

        /// <summary>
        /// The period of the wave; how long it takes to travel up and down the wave.<br/>
        /// Sum of <see cref="UpPeriod"/> and <see cref="DownPeriod"/>.<br/>
        /// <see cref="Period"/> does NOT contain either wait periods.<br/>
        /// Ignores the <see cref="Velocity"/> of the wave, if you want to know it'll actually
        /// take to travel the wave, use <see cref="EffectivePeriod"/>.
        /// </summary>
        public float Period
        {
            get => period;
        }


        /// <summary>
        /// The wavelength of the wave.
        /// </summary>
        public float WaveLength
        {
            get => wavelength;
        }

        /// <summary>
        /// The amount of time it takes to travel up the wave.<br/>
        /// <see cref="EffectiveUpPeriod"/> does NOT contain either wait periods.
        /// </summary>
        public float EffectiveUpPeriod
        {
            get => adjustedUpPeriod;
        }

        /// <summary>
        /// The amount of time it takes to travel down the wave.<br/>
        /// <see cref="EffectiveDownPeriod"/> does NOT contain either wait periods.
        /// </summary>
        public float EffectiveDownPeriod
        {
            get => adjustedDownPeriod;
        }

        /// <summary>
        /// The amount of time it takes to travel the wave.<br/>
        /// Sum of <see cref="EffectiveUpPeriod"/> and <see cref="EffectiveDownPeriod"/>.<br/>
        /// <see cref="EffectivePeriod"/> does NOT contain either wait periods.
        /// </summary>
        public float EffectivePeriod
        {
            get => adjustedPeriod;
        }

        /// <summary>
        /// The frequency of the wave.
        /// </summary>
        public float Frequency
        {
            get => frequency;
        }

        /// <summary>
        /// The upward curve of the wave.
        /// </summary>
        public AnimationCurve UpwardCurve
        {
            get => upwardCurve;
            set => upwardCurve = value;
        }

        /// <summary>
        /// The downward curve of the wave.
        /// </summary>
        public AnimationCurve DownwardCurve
        {
            get => downwardCurve;
            set => downwardCurve = value;
        }

        /// <summary>
        /// How long to stay at the crest of the wave.
        /// </summary>
        public float CrestWait
        {
            get => crestWait;
            set => crestWait = value;
        }

        /// <summary>
        /// How long to stay at the trough of the wave.
        /// </summary>
        public float TroughWait
        {
            get => troughWait;
            set => troughWait = value;
        }

        public Wave() : this(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 1f, 1f,
            1f, 0f, 0f)
        {
        }

        public Wave(Wave original) : this(original.upwardCurve.Copy(), original.downwardCurve.Copy(),
            original.upPeriod, original.downPeriod, original.amplitude, original.crestWait, original.troughWait)
        {
        }

        private Wave(float upPeriod, float downPeriod, float amplitude)
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

        public Wave(AnimationCurve upwardCurve, AnimationCurve downwardCurve, float upPeriod, float downPeriod,
            float amplitude) : this(upPeriod, downPeriod, amplitude)
        {
            if (upwardCurve == null) throw new System.ArgumentNullException(nameof(upwardCurve));
            if (downwardCurve == null) throw new System.ArgumentNullException(nameof(downwardCurve));
            if (upPeriod < 0) throw new System.ArgumentException(nameof(upPeriod) + " may not be negative");
            if (downPeriod < 0) throw new System.ArgumentException(nameof(downPeriod) + " may not be negative");
            if ((upPeriod + downPeriod) <= 0)
                throw new System.ArgumentException("The sum of " + nameof(upPeriod) + " and " + nameof(downPeriod) +
                                                   " must be larger than zero");

            UpwardCurve = upwardCurve;
            DownwardCurve = downwardCurve;
        }

        public Wave(AnimationCurve upwardCurve, AnimationCurve downwardCurve, float upPeriod, float downPeriod,
            float amplitude, float crestWait, float troughWait)
            : this(upwardCurve, downwardCurve, upPeriod, downPeriod, amplitude)
        {
            if (crestWait < 0) throw new System.ArgumentException(nameof(crestWait) + " may not be negative");
            if (TroughWait < 0) throw new System.ArgumentException(nameof(TroughWait) + " may not be negative");

            CrestWait = crestWait;
            TroughWait = troughWait;
        }

        [Tooltip(
            "The time it takes for the wave to travel from trough to crest, or from its lowest to its highest point, in seconds.")]
        [SerializeField, TMPParameterBundleField("upperiod", "uppd")]
        private float upPeriod;

        [Tooltip(
            "The time it takes for the wave to travel from crest to trough, or from its highest to its lowest point, in seconds.")]
        [SerializeField, TMPParameterBundleField("downperiod", "downpd", "dnpd")]
        private float downPeriod;

        [Tooltip("The amplitude of the wave.")] [SerializeField, TMPParameterBundleField("amplitude", "amp")]
        private float amplitude;

        [Tooltip(
            "The \"up\" part of the wave. This is the curve that is used to travel from trough to crest, or from the wave's lowest to its highest point.")]
        [SerializeField, TMPParameterBundleField("upcurve", "upcrv", "up")]
        private AnimationCurve upwardCurve;

        [Tooltip(
            "The \"down\" part of the wave. This is the curve that is used to travel from crest to trough, or from the wave's highest to its lowest point.")]
        [SerializeField, TMPParameterBundleField("downcurve", "downcrv", "down", "dn")]
        private AnimationCurve downwardCurve;

        [Tooltip("The amount of time to remain at the crest before moving down again, in seconds.")]
        [SerializeField, TMPParameterBundleField("crestwait", "cwait", "cw")]
        private float crestWait;

        [Tooltip("The amount of time to remain at the trough before moving up again, in seconds.")]
        [SerializeField, TMPParameterBundleField("troughwait", "twait", "tw")]
        private float troughWait;

        [SerializeField, HideInInspector] private float velocity;

        [System.NonSerialized] private float period;
        [System.NonSerialized] private float adjustedPeriod;
        [System.NonSerialized] private float adjustedUpPeriod;
        [System.NonSerialized] private float adjustedDownPeriod;
        [System.NonSerialized] private float frequency;
        [System.NonSerialized] private float wavelength;


        /// <summary>
        /// Check whether an extrema was passed between (<paramref name="time"/> - <paramref name="deltaTime"/>) and <paramref name="time"/>.<br/>
        /// This will automatically choose the correct way to interpret the wave.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="deltaTime">The delta time value.</param>
        /// <param name="offset">The offset..</param>
        /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="TMPAnimationUtility.WaveBase.Period"/> or <see cref="TMPAnimationUtility.WaveBase.EffectivePeriod"/>).</param>
        /// <param name="extrema">If the wave has a <see cref="CrestWait"/> or <see cref="TroughWait"/>, this parameter defines whether an extremum is passed once the wait time begins, or once it ends.</param>
        /// <returns>1 if a maximum was passed, -1 if a minimum was passed, 0 if no extremum was passed.</returns>
        /// <exception cref="System.Exception"></exception>
        public int PassedExtrema(float time, float deltaTime, float offset, bool realtimeWait = true,
            PulseExtrema extrema = PulseExtrema.Early)
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

        /// <summary>
        /// Check whether an extrema was passed between (<paramref name="time"/> - <paramref name="deltaTime"/>) and <paramref name="time"/>.<br/>
        /// Explicitly interpret the wave as a normal wave, ignoring both <see cref="CrestWait"/> and <see cref="TroughWait"/>.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="deltaTime">The delta time value.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="TMPAnimationUtility.WaveBase.Period"/> or <see cref="TMPAnimationUtility.WaveBase.EffectivePeriod"/>).</param>
        /// <param name="extrema">If the wave has a <see cref="CrestWait"/> or <see cref="TroughWait"/>, this parameter defines whether an extremum is passed once the wait time begins, or once it ends.</param>
        /// <returns>1 if a maximum was passed, -1 if a minimum was passed, 0 if no extremum was passed.</returns>
        /// <exception cref="System.Exception"></exception>
        public int PassedWaveExtrema(float time, float deltaTime, float offset)
        {
            float t = CalculateT(EffectivePeriod, time, offset, -1);

            if (deltaTime >= EffectivePeriod)
            {
                t %= EffectivePeriod;
                return t < EffectiveUpPeriod ? -1 : 1;
            }

            float prevT = CalculateT(EffectivePeriod, time - deltaTime, offset, -1);

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

        /// <summary>
        /// Check whether an extrema was passed between (<paramref name="time"/> - <paramref name="deltaTime"/>) and <paramref name="time"/>.<br/>
        /// Explicitly interpret the wave as a pulse, ignoring the <see cref="CrestWait"/>.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="deltaTime">The delta time value.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="TMPAnimationUtility.WaveBase.Period"/> or <see cref="TMPAnimationUtility.WaveBase.EffectivePeriod"/>).</param>
        /// <param name="extrema">If the wave has a <see cref="CrestWait"/> or <see cref="TroughWait"/>, this parameter defines whether an extremum is passed once the wait time begins, or once it ends.</param>
        /// <returns>1 if a maximum was passed, -1 if a minimum was passed, 0 if no extremum was passed.</returns>
        /// <exception cref="System.Exception"></exception>
        public int PassedPulseExtrema(float time, float deltaTime, float offset, bool realtimeWait = true,
            PulseExtrema extrema = PulseExtrema.Early)
        {
            float interval = ((TroughWait) * (realtimeWait ? Velocity : 1f)) + EffectivePeriod;
            float t = CalculateT(interval, time, offset, -1);

            if (deltaTime >= interval)
            {
                t %= interval;
                if (t < EffectiveUpPeriod) return -1;
                if (t < EffectivePeriod) return 1;
                return -1;
                //return t < EffectiveUpPeriod ? -1 : 1;
            }

            float prevT = CalculateT(interval, time - deltaTime, offset, -1);

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

        /// <summary>
        /// Check whether an extrema was passed between (<paramref name="time"/> - <paramref name="deltaTime"/>) and <paramref name="time"/>.<br/>
        /// Explicitly interpret the wave as an inverted pulse, ignoring the <see cref="TroughWait"/>.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="deltaTime">The delta time value.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="TMPAnimationUtility.WaveBase.Period"/> or <see cref="TMPAnimationUtility.WaveBase.EffectivePeriod"/>).</param>
        /// <param name="extrema">If the wave has a <see cref="CrestWait"/> or <see cref="TroughWait"/>, this parameter defines whether an extremum is passed once the wait time begins, or once it ends.</param>
        /// <returns>1 if a maximum was passed, -1 if a minimum was passed, 0 if no extremum was passed.</returns>
        /// <exception cref="System.Exception"></exception>
        public int PassedInvertedPulseExtrema(float time, float deltaTime, float offset, bool realtimeWait = true,
            PulseExtrema extrema = PulseExtrema.Early)
        {
            float interval = ((CrestWait) * (realtimeWait ? Velocity : 1f)) + EffectivePeriod;
            float t = CalculateT(interval, time, offset, -1);

            if (deltaTime >= interval)
            {
                t %= interval;
                if (t < EffectiveDownPeriod) return 1;
                if (t < EffectivePeriod) return -1;
                return 1;
            }

            float prevT = CalculateT(interval, time - deltaTime, offset, -1);

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

        /// <summary>
        /// Check whether an extrema was passed between (<paramref name="time"/> - <paramref name="deltaTime"/>) and <paramref name="time"/>.
        /// Explicitly interpret the wave as a one-directional pulse.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="deltaTime">The delta time value.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="TMPAnimationUtility.WaveBase.Period"/> or <see cref="TMPAnimationUtility.WaveBase.EffectivePeriod"/>).</param>
        /// <param name="extrema">If the wave has a <see cref="CrestWait"/> or <see cref="TroughWait"/>, this parameter defines whether an extremum is passed once the wait time begins, or once it ends.</param>
        /// <returns>1 if a maximum was passed, -1 if a minimum was passed, 0 if no extremum was passed.</returns>
        /// <exception cref="System.Exception"></exception>
        public int PassedOneDirectionalPulseExtrema(float time, float deltaTime, float offset,
            bool realtimeWait = true, PulseExtrema extrema = PulseExtrema.Early)
        {
            float upInterval = (CrestWait) * (realtimeWait ? Velocity : 1f);
            float downInterval = (TroughWait) * (realtimeWait ? Velocity : 1f);
            float interval = upInterval + downInterval + EffectivePeriod;
            float t = CalculateT(interval, time, offset, -1);

            if (deltaTime >= interval)
            {
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

                throw new System.Exception("Should not be reachable");
            }

            float prevT = CalculateT(interval, time - deltaTime, offset, -1);

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

        /// <summary>
        /// Evaluate the wave.<br/>
        /// This will automatically choose the correct way to interpret the wave.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="TMPAnimationUtility.WaveBase.Period"/> or <see cref="TMPAnimationUtility.WaveBase.EffectivePeriod"/>).</param>
        /// <returns>Value: The value of the wave at the given time and offset.<br/>Direction: Whether youre currently travelling up the wave (=1) or down the wave (=-1).</returns>
        public (float Value, int Direction) Evaluate(float time, float offset, bool realtimeWait = true)
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

        public (float Value, int Direction) Evaluate(float time, float offset)
        {
            if (CrestWait <= 0)
            {
                if (TroughWait <= 0)
                {
                    return EvaluateAsWave(time, offset);
                }

                return EvaluateAsPulse(time, offset);
            }

            if (TroughWait <= 0) return EvaluateAsInvertedPulse(time, offset);

            return EvaluateAsOneDirectionalPulse(time, offset);
        }

        /// <summary>
        /// Evaluate the wave as a normal wave explicitly, ignoring both <see cref="TroughWait"/> and <see cref="CrestWait"/>.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Item1: The value of the wave at the given time and offset.<br/>Item2: Whether youre currently travelling up the wave (=1) or down the wave (=-1).</returns>
        /// <exception cref="System.Exception"></exception>
        public (float Value, int Direction) EvaluateAsWave(float time, float offset)
        {
            float t = CalculateT(EffectivePeriod, time, offset, -1);

            if (t <= EffectiveUpPeriod)
            {
                t = Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod);
                return (Amplitude * TMPAnimationUtility.GetValue(UpwardCurve, WrapMode.PingPong, t),
                    1);
            }

            t = Mathf.Lerp(1f, 2f, (t - EffectiveUpPeriod) / EffectiveDownPeriod);
            return (Amplitude * TMPAnimationUtility.GetValue(DownwardCurve, WrapMode.PingPong, t), -1);
        }

        /// <summary>
        /// Evaluate the wave as a pulse explicitly, ignoring the <see cref="CrestWait"/>.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="realTimeWait">Whether to use real time (i.e. whether to use <see cref="TMPAnimationUtility.WaveBase.Period"/> or <see cref="TMPAnimationUtility.WaveBase.EffectivePeriod"/>).</param>
        /// <returns>Item1: The value of the wave at the given time and offset.<br/>Item2: Whether youre currently travelling up the wave (=1) or down the wave (=-1).</returns>
        /// <exception cref="System.Exception"></exception>
        public (float Value, int Direction) EvaluateAsPulse(float time, float offset, bool realTimeWait = true)
        {
            float interval = ((TroughWait) * (realTimeWait ? Velocity : 1f)) + EffectivePeriod;
            float t = CalculateT(interval, time, offset, -1);

            // If 0, we are at start of up curve.
            if (t <= 0) return (Amplitude * TMPAnimationUtility.GetValue(UpwardCurve, WrapMode.PingPong, 0f), 1);

            // If smaller than effective up period, we are travelling up the curve
            if (t <= EffectiveUpPeriod)
                return (
                    Amplitude * TMPAnimationUtility.GetValue(UpwardCurve, WrapMode.PingPong,
                        Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod)),
                    1);

            // There is no crest wait, so if we are smaller than the effective period, we are travelling down the curve
            if (t <= (EffectivePeriod))
                return (
                    Amplitude * TMPAnimationUtility.GetValue(DownwardCurve, WrapMode.PingPong,
                        Mathf.Lerp(1f, 2f, (t - EffectiveUpPeriod) / EffectiveDownPeriod)), -1);

            // If larger than effective period, is trough waiting
            return (Amplitude * TMPAnimationUtility.GetValue(DownwardCurve, WrapMode.PingPong, 2f), -1);
        }

        /// <summary>
        /// Evaluate the wave as an inverted pulse explicitly, ignoring the <see cref="TroughWait"/>.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="realTimeWait">Whether to use real time (i.e. whether to use <see cref="TMPAnimationUtility.WaveBase.Period"/> or <see cref="TMPAnimationUtility.WaveBase.EffectivePeriod"/>).</param>
        /// <returns>Item1: The value of the wave at the given time and offset.<br/>Item2: Whether youre currently travelling up the wave (=1) or down the wave (=-1).</returns>
        /// <exception cref="System.Exception"></exception>
        public (float Value, int Direction) EvaluateAsInvertedPulse(float time, float offset, bool realTimeWait = true)
        {
            float wait = CrestWait * (realTimeWait ? Velocity : 1f);
            float interval = wait + EffectivePeriod;
            float t = CalculateT(interval, time, offset, -1);

            // If 0, we are at start of up curve.
            if (t <= 0) return (Amplitude * TMPAnimationUtility.GetValue(UpwardCurve, WrapMode.PingPong, 0f), 1);

            // If smaller than effective up period, we are travelling up the curve
            if (t <= EffectiveUpPeriod)
                return (
                    Amplitude * TMPAnimationUtility.GetValue(UpwardCurve, WrapMode.PingPong,
                        Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod)),
                    1);

            // If smaller than effective up period + wait, we are waiting
            if (t <= EffectiveUpPeriod + wait)
                return (
                    Amplitude * TMPAnimationUtility.GetValue(UpwardCurve, WrapMode.PingPong, 1), 1);

            // Otherwise we are travelling down the curve
            return (
                Amplitude * TMPAnimationUtility.GetValue(DownwardCurve, WrapMode.PingPong,
                    Mathf.Lerp(1f, 2f, (t - EffectiveUpPeriod - wait) / EffectiveDownPeriod)), -1);
        }

        /// <summary>
        /// Evaluate the wave as a one-directional pulse explicitly.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="realTimeWait">Whether to use real time (i.e. whether to use <see cref="TMPAnimationUtility.WaveBase.Period"/> or <see cref="TMPAnimationUtility.WaveBase.EffectivePeriod"/>).</param>
        /// <returns>Item1: The value of the wave at the given time and offset.<br/>Item2: Whether youre currently travelling up the wave (=1) or down the wave (=-1).</returns>
        /// <exception cref="System.Exception"></exception>
        public (float Value, int Direction) EvaluateAsOneDirectionalPulse(float time, float offset,
            bool realTimeWait = true)
        {
            float upInterval = (CrestWait) * (realTimeWait ? Velocity : 1f);
            float downInterval = (TroughWait) * (realTimeWait ? Velocity : 1f);
            float interval = upInterval + downInterval + EffectivePeriod;
            float t = CalculateT(interval, time, offset, -1);

            if (t <= EffectiveUpPeriod)
            {
                return (
                    Amplitude * TMPAnimationUtility.GetValue(UpwardCurve, WrapMode.PingPong,
                        Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod)),
                    1);
            }

            t -= EffectiveUpPeriod;
            if (t <= upInterval)
            {
                return (Amplitude * TMPAnimationUtility.GetValue(UpwardCurve, WrapMode.PingPong, 1f), 1);
            }

            t -= upInterval;
            if (t <= EffectiveDownPeriod)
            {
                return (
                    Amplitude * TMPAnimationUtility.GetValue(DownwardCurve, WrapMode.PingPong,
                        Mathf.Lerp(1f, 2f, t / EffectiveDownPeriod)), -1);
            }

            t -= EffectiveDownPeriod;
            if (t <= downInterval)
            {
                return (
                    Amplitude * TMPAnimationUtility.GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, 1f)),
                    -1);
            }

            throw new System.Exception("Shouldnt be reachable (interval = " + interval + ")");
        }

        /// <summary>
        /// If the wave has a <see cref="CrestWait"/> or <see cref="TroughWait"/>, PulseExtrema defines whether an extremum is passed once the wait time begins, or once it ends.
        /// </summary>
        [Flags]
        public enum PulseExtrema
        {
            Early = 0b01,
            Late = 0b10,
            Both = 0b11
        }

        // private float CalculateWaveT(float time, float offset, int mult)
        // {
        //     // return CalculateT(time, offset, mult);
        // }
        //
        // private float CalculatePulseT(float time, float offset, float interval, int mult)
        // {
        //     // float t = CalculateT(time, offset, mult);
        //     // if (t < 0)
        //     // {
        //     //     int times = (int)Math.Ceiling(Mathf.Abs(t / interval));
        //     //     t += times * interval;
        //     // }
        //     //
        //     // return t;
        // }

        private float CalculateT(float period, float time, float offset, int mult)
        {
            float t = time + (offset * mult);

            if (t < 0)
            {
                t = period - (-t % period);
            }
            else t %= period;

            return t;
        }


        public void OnBeforeSerialize()
        {
            UpdateFields();
        }

        public void OnAfterDeserialize()
        {
            UpdateFields();
        }

        private void UpdateFields()
        {
            upPeriod = Mathf.Max(upPeriod, 0f);
            downPeriod = Mathf.Max(downPeriod, 0f);
            if (downPeriod + upPeriod == 0) upPeriod = 0.1f;
            velocity = Mathf.Max(velocity, 0.001f);

            // Setting the velocity will set frequency and wavelength,
            // and also call the setter of DownPeriod and UpPeriod,
            // which will set all remaining values
            Velocity = velocity;

            if (upwardCurve == null || upwardCurve.keys.Length == 0)
                upwardCurve = AnimationCurveUtility.EaseInOutSine();
            if (downwardCurve == null || downwardCurve.keys.Length == 0)
                downwardCurve = AnimationCurveUtility.EaseInOutSine();

            troughWait = Mathf.Max(troughWait, 0f);
            crestWait = Mathf.Max(crestWait, 0f);
        }

        public override string ToString()
        {
            return @$"Wave {{
    upPeriod: {upPeriod},
    downPeriod: {downPeriod},
    amplitude: {amplitude},
    upwardCurve: {upwardCurve},
    downwardCurve: {downwardCurve},
    crestWait: {crestWait},
    troughWait: {troughWait},
    velocity: {velocity},
    period: {period},
    adjustedPeriod: {adjustedPeriod},
    adjustedUpPeriod: {adjustedUpPeriod},
    adjustedDownPeriod: {adjustedDownPeriod},
    frequency: {frequency},
    wavelength: {wavelength}
}}";
        }

        private static void Create_Hook(ref Wave newInstance, Wave originalInstance, WaveParameters parameters)
        {
            // Ensures all fields are correctly set
            // Since autoparameter bundles use the empty constructor and only set the fields
            // defined as parameter fields, this is necessary to update derived values
            // like adjustedUpPeriod, etc
            newInstance.UpdateFields();
        }
    }
}