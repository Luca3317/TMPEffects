using UnityEngine;
using TMPEffects.CharacterData;
using System;
using System.Collections.Generic;
using TMPEffects.Extensions;
using TMPEffects.Components.Animator;
using TMPro;
using TMPEffects.Components;
using TMPEffects.Databases;
using TMPEffects.Parameters;
using static TMPEffects.Parameters.ParameterUtility;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Utility methods for animations.
    /// </summary>
    public static class AnimationUtility
    {
        #region Scaling

        /// <summary>
        /// Scale a given value to make it uniform between <see cref="TextMeshPro"/> and <see cref="TextMeshProUGUI"/> components. 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value">The value to scale.</param>
        /// <returns>The scaled value.</returns>
        public static float ScaleTextMesh(TMP_Text text, float value) =>
            ScaleTextMesh(text.canvas != null, value);

        public static float ScaleTextMesh(IAnimatorContext ctx, float value)
            => ScaleTextMesh(ctx.Animator.TextComponent.canvas != null, value);

        public static float ScaleTextMesh(IAnimationContext ctx, float value)
            => ScaleTextMesh(ctx.AnimatorContext.Animator.TextComponent.canvas != null, value);

        public static float ScaleTextMesh(bool isTMProUGUI, float value)
        {
            if (!isTMProUGUI) return value * 10;
            return value;
        }

        /// <summary>
        /// Scale a vector for an animation.<br/>
        /// Makes vectors uniform relative to the size of the text.<br/>
        /// Used by <see cref="TMPEffects.Components.TMPAnimator"/> to automatically scale animations.
        /// </summary>
        /// <param name="vector">The vector to scale.</param>
        /// <param name="cData">The <see cref="CharData"/> the vector will applied to.</param>
        /// <param name="context">The <see cref="IAnimatorContext"/> of the animation.</param>
        /// <returns>The scaled vector.</returns>
        public static Vector3 ScaleVector(Vector3 vector, CharData cData, IAnimationContext context) =>
            ScaleVector(vector, context.AnimatorContext.Animator.TextComponent.canvas != null,
                context.AnimatorContext.ScaleAnimations, context.AnimatorContext.ScaleUniformly,
                cData.info.pointSize, context.AnimatorContext.Animator.TextComponent.fontSize);

        /// <summary>
        /// Scale a vector for an animation.<br/>
        /// Used by <see cref="TMPEffects.Components.TMPAnimator"/> to automatically scale animations.
        /// </summary>
        /// <param name="vector">The vector to scale.</param>
        /// <param name="cData">The <see cref="CharData"/> the vector will applied to.</param>
        /// <param name="context">The <see cref="IAnimatorContext"/> of the animation.</param>
        /// <returns>The scaled vector.</returns>
        public static Vector3 ScaleVector(Vector3 vector, CharData cData, IAnimatorContext context) =>
            ScaleVector(vector, context.Animator.TextComponent.canvas != null, context.ScaleAnimations,
                context.ScaleUniformly,
                cData.info.pointSize, context.Animator.TextComponent.fontSize);


        public static Vector3 ScaleVector(Vector3 vector, bool isTMProUGUI, bool scaleAnimations, bool scaleUniformly,
            float pointSize, float fontSize)
        {
            vector /= ScaleTextMesh(isTMProUGUI, 1f);
            if (!scaleAnimations) return vector;
            if (!scaleUniformly) return vector * (pointSize / 36f);
            return vector * (fontSize / 36f);
        }

        public static Vector3 IgnoreScaling(Vector3 vector, CharData cData, IAnimationContext context) =>
            IgnoreScaling(vector, context.AnimatorContext.Animator.TextComponent.canvas != null,
                context.AnimatorContext.ScaleAnimations, context.AnimatorContext.ScaleUniformly,
                cData.info.pointSize, context.AnimatorContext.Animator.TextComponent.fontSize);

        public static Vector3 IgnoreScaling(Vector3 vector, CharData cData, IAnimatorContext context) =>
            IgnoreScaling(vector, context.Animator.TextComponent.canvas != null, context.ScaleAnimations,
                context.ScaleUniformly,
                cData.info.pointSize, context.Animator.TextComponent.fontSize);

        public static Vector3 IgnoreScaling(Vector3 vector, bool isTMProUGUI, bool scaleAnimations, bool scaleUniformly,
            float pointSize, float fontSize)
        {
            vector *= ScaleTextMesh(isTMProUGUI, 1f);
            if (!scaleAnimations) return vector;
            if (!scaleUniformly) return vector / (pointSize / 36f);
            return vector / (fontSize / 36f);
        }

        /// <summary>
        /// Scale a vector for an animation inversely.<br/>
        /// <see cref="TMPAnimator"/> automatically scales animations; using this method scales the vector in a way that makes it effectively ignore the <see cref="TMPAnimator"/>'s scaling.
        /// </summary>
        /// <param name="vector">The vector to scale inversely.</param>
        /// <param name="cData">The <see cref="CharData"/> the vector will be applied to.</param>
        /// <param name="context">The <see cref="IAnimationContext"/> of the animation.</param>
        /// <returns>The inversely scaled vector.</returns>
        public static Vector3 InverseScaleVector(Vector3 vector, CharData cData, IAnimationContext context) =>
            IgnoreScaling(vector, cData, context.AnimatorContext);

        /// <summary>
        /// Scale a vector for an animation inversely.<br/>
        /// <see cref="TMPAnimator"/> automatically scales animations; using this method scales the vector in a way that makes it effectively ignore the <see cref="TMPAnimator"/>'s scaling.
        /// </summary>
        /// <param name="vector">The vector to scale inversely.</param>
        /// <param name="cData">The <see cref="CharData"/> the vector will be applied to.</param>
        /// <param name="context">The <see cref="IAnimatorContext"/> of the animation.</param>
        /// <returns>The inversely scaled vector.</returns>
        public static Vector3 InverseScaleVector(Vector3 vector, CharData cData, IAnimatorContext context) =>
            IgnoreScaling(vector, cData, context);

        #endregion


        #region Raw Positions & Deltas

        /// <summary>
        /// Convert an anchor vector to its actual position vector.
        /// </summary>
        /// <param name="anchor">The anchor to convert.</param>
        /// <param name="cData">The <see cref="CharData"/> the anchor applies to.</param>
        /// <returns>The position vector.</returns>
        public static Vector2 AnchorToPosition(Vector2 anchor, CharData cData)
        {
            if (anchor == Vector2.zero)
            {
                return cData.InitialPosition;
            }

            Vector2 dist;
            Vector2 ret = cData.InitialPosition;

            Vector2 up = (cData.InitialMesh.TL_Position - cData.InitialMesh.BL_Position) / 2f;
            Vector2 right = (cData.InitialMesh.BR_Position - cData.InitialMesh.BL_Position) / 2f;

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
        /// <param name="ctx">The <see cref="IAnimationContext"/> of the animation.</param>
        /// <returns>The raw version of the passed in vertex position, i.e. the one that will ignore the <see cref="TMPEffects.Components.TMPAnimator"/>'s scaling.</returns>
        public static Vector3 GetRawVertex(int index, Vector3 position, CharData cData, IAnimationContext ctx) =>
            GetRawVertex(index, position, cData, ctx.AnimatorContext);

        /// <summary>
        /// Calculate the raw version of the passed in vertex position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="index">Index of the vertex.</param>
        /// <param name="position">The position to set the vertex to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The <see cref="IAnimatorContext"/> of the animation.</param>
        /// <returns>The raw version of the passed in vertex position, i.e. the one that will ignore the <see cref="TMPEffects.Components.TMPAnimator"/>'s scaling.</returns>
        public static Vector3 GetRawVertex(int index, Vector3 position, CharData cData, IAnimatorContext ctx)
        {
            return GetRawPosition(position, cData.InitialMesh.GetPosition(index), cData, ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in character position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the character to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The <see cref="IAnimationContext"/> of the animation.</param>
        /// <returns>The raw version of the passed in character position, i.e. the one that will ignore the <see cref="TMPEffects.Components.TMPAnimator"/>'s scaling.</returns>
        public static Vector3 GetRawPosition(Vector3 position, CharData cData, IAnimationContext ctx) =>
            GetRawPosition(position, cData, ctx.AnimatorContext);

        /// <summary>
        /// Calculate the raw version of the passed in character position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the character to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The <see cref="IAnimatorContext"/> of the animation.</param>
        /// <returns>The raw version of the passed in character position, i.e. the one that will ignore the <see cref="TMPEffects.Components.TMPAnimator"/>'s scaling.</returns>
        public static Vector3 GetRawPosition(Vector3 position, CharData cData, IAnimatorContext ctx)
        {
            return GetRawPosition(position, cData.InitialPosition, cData, ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in pivot position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the pivot to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The <see cref="IAnimationContext"/> of the animation.</param>
        /// <returns>The raw version of the passed in pivot position, i.e. the one that will ignore the <see cref="TMPEffects.Components.TMPAnimator"/>'s scaling.</returns>
        public static Vector3 GetRawPivot(Vector3 position, CharData cData, IAnimationContext ctx) =>
            GetRawPivot(position, cData, ctx.AnimatorContext);

        /// <summary>
        /// Calculate the raw version of the passed in pivot position, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the pivot to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The <see cref="IAnimatorContext"/> of the animation.</param>
        /// <returns>The raw version of the passed in pivot position, i.e. the one that will ignore the <see cref="TMPEffects.Components.TMPAnimator"/>'s scaling.</returns>
        public static Vector3 GetRawPivot(Vector3 position, CharData cData, IAnimatorContext ctx)
        {
            return GetRawPosition(position, cData.InitialPosition, cData, ctx);
        }

        /// <summary>
        /// Calculate the raw version of the passed in delta, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The <see cref="IAnimationContext"/> of the animation.</param>
        /// <returns>The raw version of the passed in delta, i.e. the one that will ignore the <see cref="TMPEffects.Components.TMPAnimator"/>'s scaling.</returns>
        public static Vector3 GetRawDelta(Vector3 delta, CharData cData, IAnimationContext ctx) =>
            GetRawDelta(delta, cData, ctx.AnimatorContext);

        /// <summary>
        /// Calculate the raw version of the passed in delta, i.e. the one that will ignore the animator's scaling.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The <see cref="IAnimatorContext"/> of the animation.</param>
        /// <returns>The raw version of the passed in delta, i.e. the one that will ignore the <see cref="TMPEffects.Components.TMPAnimator"/>'s scaling.</returns>
        public static Vector3 GetRawDelta(Vector3 delta, CharData cData, IAnimatorContext ctx)
        {
            return IgnoreScaling(delta, cData, ctx);
        }

        internal static Vector3 GetRawPosition(Vector3 position, Vector3 referencePosition, CharData cData,
            IAnimatorContext ctx)
        {
            return IgnoreScaling(position - referencePosition, cData, ctx) + referencePosition;
        }


        /// <summary>
        /// Set the raw position of the vertex at the given index. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="index">Index of the vertex.</param>
        /// <param name="position">The position to set the vertex to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The <see cref="IAnimationContext"/> of the animation.</param>
        public static void SetVertexRaw(int index, Vector3 position, CharData cData, IAnimationContext ctx) =>
            SetVertexRaw(index, position, cData, ctx.AnimatorContext);

        /// <summary>
        /// Set the raw position of the vertex at the given index. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="index">Index of the vertex.</param>
        /// <param name="position">The position to set the vertex to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The <see cref="IAnimatorContext"/> of the animation.</param>
        public static void SetVertexRaw(int index, Vector3 position, CharData cData, IAnimatorContext ctx)
        {
            Vector3 ogPos = cData.InitialMesh.GetPosition(index);
            cData.mesh.SetPosition(index, GetRawPosition(position, ogPos, cData, ctx));
        }

        /// <summary>
        /// Set the raw position of the character. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the character to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The <see cref="IAnimationContext"/> of the animation.</param>
        public static void SetPositionRaw(Vector3 position, CharData cData, IAnimationContext ctx) =>
            SetPositionRaw(position, cData, ctx.AnimatorContext);

        /// <summary>
        /// Set the raw position of the character. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="position">The position to set the character to.</param>
        /// <param name="cData">The <see cref="CharData"/> to act on.</param>
        /// <param name="ctx">The <see cref="IAnimatorContext"/> of the animation.</param>
        public static void SetPositionRaw(Vector3 position, CharData cData, IAnimatorContext ctx)
        {
            Vector3 ogPos = cData.InitialPosition;
            cData.SetPosition(GetRawPosition(position, ogPos, cData, ctx));
        }

        #endregion

        #region General Math

        /// <summary>
        /// Get the point on a line closest to the given point.
        /// </summary>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
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



        public static float GetOffset(CharData cData, IAnimationContext context, ITMPOffsetProvider provider,
            bool ignoreScaling = false, bool ignoreSegmentLenght = false)
        {
            float offset = provider.GetOffset(cData, context, ignoreScaling);

            if (!ignoreSegmentLenght)
                offset /= context.SegmentData.length == 0 ? 0.001f : context.SegmentData.length;

            return offset;
        }
        
        [System.Serializable]
        public class FancyAnimationCurve
        {
            public AnimationCurve Curve
            {
                get => curve;
                set => curve = value;
            }

            [SerializeField] private AnimationCurve curve;

            public FancyAnimationCurve()
            {
            }

            public FancyAnimationCurve(AnimationCurve curve)
            {
                this.curve = curve;
            }
            
            public FancyAnimationCurve(FancyAnimationCurve curve)
            {
                this.curve = curve.curve;
            }

            public float Evaluate(float time, float offset, float uniformity)
            {
                float t = CalculateT(time, offset,uniformity);
                return curve.Evaluate(time);
            }

            private float CalculateT(float time, float offset, float uniformity)
            {
                float t;
                t = (time) + offset * uniformity;
                return t;
            }
        }


        #region Waves

        /// <summary>
        /// Base class for <see cref="Wave"/>.<br/>
        /// Allows you to easily create periodic animations.<b/>
        /// You should take a look at the online documentation for this one:<b/>
        /// https://tmpeffects.luca3317.dev/docs/tmpanimator_animationutility_wave.html
        /// </summary>
        [System.Serializable]
        public abstract class WaveBase : ISerializationCallbackReceiver
        {
            /// <summary>
            /// The up period of the wave; how long it takes to travel up the wave.<br/>
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
            /// The amount of time it takes to travel up the wave.
            /// </summary>
            public float EffectiveUpPeriod
            {
                get => adjustedUpPeriod;
            }

            /// <summary>
            /// The amount of time it takes to travel down the wave.
            /// </summary>
            public float EffectiveDownPeriod
            {
                get => adjustedDownPeriod;
            }

            /// <summary>
            /// The amount of time it takes to travel the wave.<br/>
            /// Sum of <see cref="EffectiveUpPeriod"/> and <see cref="EffectiveDownPeriod"/>.
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

            public WaveBase() : this(1f, 1f, 1f)
            {
            }

            public WaveBase(float upPeriod, float downPeriod, float amplitude)
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

            [Tooltip(
                "The time it takes for the wave to travel from trough to crest, or from its lowest to its highest point, in seconds")]
            [SerializeField]
            private float upPeriod;

            [Tooltip(
                "The time it takes for the wave to travel from crest to trough, or from its highest to its lowest point, in seconds")]
            [SerializeField]
            private float downPeriod;

            [Tooltip("The amplitude of the wave")] [SerializeField]
            private float amplitude;

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

        /// <summary>
        /// A wave.
        /// Allows you to easily create periodic animations.<b/>
        /// You should take a look at the online documentation for this one:<b/>
        /// https://tmpeffects.luca3317.dev/docs/tmpanimator_animationutility_wave.html
        /// </summary>
        [System.Serializable]
        public class Wave : WaveBase, ISerializationCallbackReceiver
        {
            // !! TODO !!
            // Moved these in here temporarily so it compiles for now

            /// <summary>
            /// Get all parameter relevant to a wave.<br/>
            /// Important: this reserves the following parameter names:
            /// <list type="bullet">
            /// <item>upcurve, upcrv, upc</item>
            /// <item>downcurve, downcrv, downc, dcrv, dc</item>
            /// <item>upperiod, uppd</item>
            /// <item>downperiod, downpd, dpd</item>
            /// <item>crestwait, crestw, cwait</item>
            /// <item>troughwait, troughw, twait</item>
            /// <item>wavevelocity, wavevlc, wvelocity, wvlc</item>
            /// <item>wavelength, wavelen, wlength, wlen</item>
            /// <item>waveuniformity, waveuni, wuniformity, wuni</item>
            /// <item>waveamplitude, wamplitude, waveamp, wamp</item>
            /// </list>
            /// </summary>
            /// <param name="parameters">The dictionary containing the parameters.</param>
            /// <param name="upwardCurve">Whether to get the upwardCurve parameter.</param>
            /// <param name="downwardCurve">Whether to get the downwardCurve parameter.</param>
            /// <param name="upPeriod">Whether to get the upPeriod parameter.</param>
            /// <param name="downPeriod">Whether to get the downPeriod parameter.</param>
            /// <param name="crestWait">Whether to get the crestWait parameter.</param>
            /// <param name="troughWait">Whether to get the troughWait parameter.</param>
            /// <param name="waveVelocity">Whether to get the waveVelocity parameter.</param>
            /// <param name="waveLength">Whether to get the waveLength parameter.</param>
            /// <param name="waveUniformity">Whether to get the waveUniformity parameter.</param>
            /// <param name="amplitude">Whether to get the amplitude parameter.</param>
            /// <returns>A <see cref="ParameterUtility.WaveParameters"/> object containing the parsed fields.</returns>
            /// <exception cref="System.Exception">If conflicting parameters are specified</exception>
            public static ParameterUtility.WaveParameters GetParameters(IDictionary<string, string> parameters,
                ITMPKeywordDatabase keywords = null,
                string prefix = "",
                bool upwardCurve = true,
                bool downwardCurve = true,
                bool upPeriod = true,
                bool downPeriod = true,
                bool crestWait = true,
                bool troughWait = true,
                bool waveVelocity = true,
                bool waveLength = true,
                bool waveUniformity = true,
                bool amplitude = true
            )
            {
                ParameterUtility.WaveParameters wp = new ParameterUtility.WaveParameters();

                wp.upwardCurve = null;
                wp.downwardCurve = null;
                wp.upPeriod = null;
                wp.downPeriod = null;
                wp.crestWait = null;
                wp.troughWait = null;
                wp.wavevelocity = null;
                wp.amplitude = null;

                if (waveVelocity &&
                    TryGetFloatParameter(out float f, parameters, keywords, prefix + "velocity", prefix + "vlc"))
                    wp.wavevelocity = f;
                if (waveLength && TryGetFloatParameter(out f, parameters, keywords, prefix + "wavelength",
                        prefix + "wavelen",
                        prefix + "wlength", prefix + "wlen"))
                {
                    if (wp.wavevelocity != null)
                        throw new System.Exception(
                            "Must define either wave velocity, wave length or uniformity; not multiple");

                    wp.wavelength = f;
                }

                if (waveUniformity &&
                    TryGetFloatParameter(out f, parameters, keywords, prefix + "uniformity", prefix + "uni"))
                    wp.waveuniformity = f;
                if (upwardCurve && TryGetAnimCurveParameter(out AnimationCurve crv, parameters, keywords,
                        prefix + "upcurve",
                        prefix + "upcrv", prefix + "up")) wp.upwardCurve = crv;
                if (downwardCurve && TryGetAnimCurveParameter(out crv, parameters, keywords, prefix + "downcurve",
                        prefix + "downcrv",
                        prefix + "down", prefix + "dn")) wp.downwardCurve = crv;
                if (upPeriod && TryGetFloatParameter(out f, parameters, keywords, prefix + "upperiod", prefix + "uppd"))
                    wp.upPeriod = f;
                if (downPeriod && TryGetFloatParameter(out f, parameters, keywords, prefix + "downperiod",
                        prefix + "downpd",
                        prefix + "dnpd")) wp.downPeriod = f;
                if (crestWait && TryGetFloatParameter(out f, parameters, keywords, prefix + "crestwait",
                        prefix + "crestw",
                        prefix + "cwait", prefix + "cw")) wp.crestWait = f;
                if (troughWait && TryGetFloatParameter(out f, parameters, keywords, prefix + "troughwait",
                        prefix + "troughw",
                        prefix + "twait", prefix + "tw")) wp.troughWait = f;
                if (amplitude &&
                    TryGetFloatParameter(out f, parameters, keywords, prefix + "amplitude", prefix + "amp"))
                    wp.amplitude = f;

                return wp;
            }

            /// <summary>
            /// Validate all parameters relevant to a wave.<br/>
            /// Important: this reserves the following parameter names:
            /// <list type="bullet">
            /// <item>upcurve, upcrv, upc</item>
            /// <item>downcurve, downcrv, downc, dcrv, dc</item>
            /// <item>upperiod, uppd</item>
            /// <item>downperiod, downpd, dpd</item>
            /// <item>crestwait, crestw, cwait</item>
            /// <item>troughwait, troughw, twait</item>
            /// <item>wavevelocity, wavevlc, wvelocity, wvlc</item>
            /// <item>wavelength, wavelen, wlength, wlen</item>
            /// <item>waveuniformity, waveuni, wuniformity, wuni</item>
            /// <item>waveamplitude, wamplitude, waveamp, wamp</item>
            /// </list>
            /// <param name="parameters">The dictionary containing the parameters.</param>
            /// <param name="upwardCurve">Whether to validate the upwardCurve parameter.</param>
            /// <param name="downwardCurve">Whether to validate the downwardCurve parameter.</param>
            /// <param name="upPeriod">Whether to validate the upPeriod parameter.</param>
            /// <param name="downPeriod">Whether to validate the downPeriod parameter.</param>
            /// <param name="crestWait">Whether to validate the crestWait parameter.</param>
            /// <param name="troughWait">Whether to validate the troughWait parameter.</param>
            /// <param name="waveVelocity">Whether to validate the waveVelocity parameter.</param>
            /// <param name="waveLength">Whether to validate the waveLength parameter.</param>
            /// <param name="waveUniformity">Whether to validate the waveUniformity parameter.</param>
            /// <param name="amplitude">Whether to validate the amplitude parameter.</param>
            /// <returns>true if all specified fields were successfully validate; otherwise, false.</returns>
            public static bool ValidateParameters(IDictionary<string, string> parameters,
                ITMPKeywordDatabase keywords = null,
                string prefix = "",
                bool upwardCurve = true,
                bool downwardCurve = true,
                bool upPeriod = true,
                bool downPeriod = true,
                bool crestWait = true,
                bool troughWait = true,
                bool waveVelocity = true,
                bool waveLength = true,
                bool waveUniformity = true,
                bool amplitude = true
            )
            {
                bool contained = false;
                string defined;

                if (waveVelocity &&
                    TryGetDefinedParameter(out defined, parameters, prefix + "velocity", prefix + "vlc"))
                {
                    if (HasNonFloatParameter(parameters, keywords, defined)) return false;
                    contained = true;
                }

                if (waveLength && TryGetDefinedParameter(out defined, parameters, prefix + "wavelength",
                        prefix + "wavelen",
                        prefix + "wlength", prefix + "wlen"))
                {
                    if (contained) return false;
                    if (HasNonFloatParameter(parameters, keywords, defined)) return false;
                }

                if (waveUniformity && HasNonFloatParameter(parameters, keywords, prefix + "uniformity", prefix + "uni"))
                    return false;
                if (upwardCurve &&
                    HasNonAnimCurveParameter(parameters, keywords, prefix + "upcurve", prefix + "upcrv", prefix + "up"))
                    return false;
                if (downwardCurve && HasNonAnimCurveParameter(parameters, keywords, prefix + "downcurve",
                        prefix + "downcrv",
                        prefix + "down", prefix + "dn")) return false;
                if (upPeriod && HasNonFloatParameter(parameters, keywords, prefix + "upperiod", prefix + "uppd"))
                    return false;
                if (downPeriod &&
                    HasNonFloatParameter(parameters, keywords, prefix + "downperiod", prefix + "downpd",
                        prefix + "dnpd"))
                    return false;
                if (crestWait && HasNonFloatParameter(parameters, keywords, prefix + "crestwait", prefix + "crestw",
                        prefix + "cwait",
                        prefix + "cw")) return false;
                if (troughWait && HasNonFloatParameter(parameters, keywords, prefix + "troughwait", prefix + "troughw",
                        prefix + "twait", prefix + "tw")) return false;
                if (amplitude && HasNonFloatParameter(parameters, keywords, prefix + "amplitude", prefix + "amp"))
                    return false;

                return true;
            }


            /// <summary>
            /// Create a new <see cref="Wave"/> using the passed in one as a template, and replacing any of its properties that are defined in the passed in <see cref="ParameterUtility.WaveParameters"/>.<br/>
            /// This is not in-place. The passed in <see cref="Wave"> will not be modified.
            /// </summary>
            /// <param name="wave"></param>
            /// <param name="wp"></param>
            /// <returns>A new <see cref="Wave"/>, that combines the properties of the passed in objects.</returns>
            public static Wave Create(Wave wave, ParameterUtility.WaveParameters wp)
            {
                float upPeriod = wp.upPeriod == null ? wave.UpPeriod : wp.upPeriod.Value;
                float downPeriod = wp.downPeriod == null ? wave.DownPeriod : wp.downPeriod.Value;

                Wave newWave = new Wave
                (
                    wp.upwardCurve == null ? wave.UpwardCurve : wp.upwardCurve,
                    wp.downwardCurve == null ? wave.DownwardCurve : wp.downwardCurve,
                    upPeriod,
                    downPeriod,
                    wp.amplitude == null ? wave.Amplitude : wp.amplitude.Value,
                    wp.crestWait == null ? wave.CrestWait : wp.crestWait.Value,
                    wp.troughWait == null ? wave.TroughWait : wp.troughWait.Value
                    // wp.waveuniformity == null ? wave.Uniformity : wp.waveuniformity.Value
                );

                return newWave;
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


            public Wave(AnimationCurve upwardCurve, AnimationCurve downwardCurve, float upPeriod, float downPeriod,
                float amplitude) : base(upPeriod, downPeriod, amplitude)
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

            public Wave(Wave wave)
            {
                crestWait = wave.crestWait;
                troughWait = wave.troughWait;
                downwardCurve = new AnimationCurve(wave.downwardCurve.keys);
                upwardCurve = new AnimationCurve(wave.upwardCurve.keys);
                wavelength = wave.wavelength;
            }

            /// <summary>
            /// Check whether an extrema was passed between (<paramref name="time"/> - <paramref name="deltaTime"/>) and <paramref name="time"/>.<br/>
            /// This will automatically choose the correct way to interpret the wave.
            /// </summary>
            /// <param name="time">The time value.</param>
            /// <param name="deltaTime">The delta time value.</param>
            /// <param name="offset">The offset..</param>
            /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="WaveBase.Period"/> or <see cref="WaveBase.EffectivePeriod"/>).</param>
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
            /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="WaveBase.Period"/> or <see cref="WaveBase.EffectivePeriod"/>).</param>
            /// <param name="extrema">If the wave has a <see cref="CrestWait"/> or <see cref="TroughWait"/>, this parameter defines whether an extremum is passed once the wait time begins, or once it ends.</param>
            /// <returns>1 if a maximum was passed, -1 if a minimum was passed, 0 if no extremum was passed.</returns>
            /// <exception cref="System.Exception"></exception>
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

            /// <summary>
            /// Check whether an extrema was passed between (<paramref name="time"/> - <paramref name="deltaTime"/>) and <paramref name="time"/>.<br/>
            /// Explicitly interpret the wave as a pulse, ignoring the <see cref="CrestWait"/>.
            /// </summary>
            /// <param name="time">The time value.</param>
            /// <param name="deltaTime">The delta time value.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="WaveBase.Period"/> or <see cref="WaveBase.EffectivePeriod"/>).</param>
            /// <param name="extrema">If the wave has a <see cref="CrestWait"/> or <see cref="TroughWait"/>, this parameter defines whether an extremum is passed once the wait time begins, or once it ends.</param>
            /// <returns>1 if a maximum was passed, -1 if a minimum was passed, 0 if no extremum was passed.</returns>
            /// <exception cref="System.Exception"></exception>
            public int PassedPulseExtrema(float time, float deltaTime, float offset, bool realtimeWait = true,
                PulseExtrema extrema = PulseExtrema.Early)
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

            /// <summary>
            /// Check whether an extrema was passed between (<paramref name="time"/> - <paramref name="deltaTime"/>) and <paramref name="time"/>.<br/>
            /// Explicitly interpret the wave as an inverted pulse, ignoring the <see cref="TroughWait"/>.
            /// </summary>
            /// <param name="time">The time value.</param>
            /// <param name="deltaTime">The delta time value.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="WaveBase.Period"/> or <see cref="WaveBase.EffectivePeriod"/>).</param>
            /// <param name="extrema">If the wave has a <see cref="CrestWait"/> or <see cref="TroughWait"/>, this parameter defines whether an extremum is passed once the wait time begins, or once it ends.</param>
            /// <returns>1 if a maximum was passed, -1 if a minimum was passed, 0 if no extremum was passed.</returns>
            /// <exception cref="System.Exception"></exception>
            public int PassedInvertedPulseExtrema(float time, float deltaTime, float offset, bool realtimeWait = true,
                PulseExtrema extrema = PulseExtrema.Early)
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

            /// <summary>
            /// Check whether an extrema was passed between (<paramref name="time"/> - <paramref name="deltaTime"/>) and <paramref name="time"/>.
            /// Explicitly interpret the wave as a one-directional pulse.
            /// </summary>
            /// <param name="time">The time value.</param>
            /// <param name="deltaTime">The delta time value.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="WaveBase.Period"/> or <see cref="WaveBase.EffectivePeriod"/>).</param>
            /// <param name="extrema">If the wave has a <see cref="CrestWait"/> or <see cref="TroughWait"/>, this parameter defines whether an extremum is passed once the wait time begins, or once it ends.</param>
            /// <returns>1 if a maximum was passed, -1 if a minimum was passed, 0 if no extremum was passed.</returns>
            /// <exception cref="System.Exception"></exception>
            public int PassedOneDirectionalPulseExtrema(float time, float deltaTime, float offset,
                bool realtimeWait = true, PulseExtrema extrema = PulseExtrema.Early)
            {
                float upInterval = (CrestWait) * (realtimeWait ? Velocity : 1f);
                float downInterval = (TroughWait) * (realtimeWait ? Velocity : 1f);
                float interval = upInterval + downInterval + EffectivePeriod;
                float t = CalculatePulseT(time, offset, interval, -1);

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

            /// <summary>
            /// Evaluate the wave.<br/>
            /// This will automatically choose the correct way to interpret the wave.
            /// </summary>
            /// <param name="time">The time value.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="realtimeWait">Whether to use real time (i.e. whether to use <see cref="WaveBase.Period"/> or <see cref="WaveBase.EffectivePeriod"/>).</param>
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

            private float uniformity;

            public (float Value, int Direction) Evaluate(float time, float offset, float uniformity)
            {
                this.uniformity = uniformity;

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
            /// <param name="realTimeWait">Whether to use real time (i.e. whether to use <see cref="WaveBase.Period"/> or <see cref="WaveBase.EffectivePeriod"/>).</param>
            /// <returns>Item1: The value of the wave at the given time and offset.<br/>Item2: Whether youre currently travelling up the wave (=1) or down the wave (=-1).</returns>
            /// <exception cref="System.Exception"></exception>
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

            /// <summary>
            /// Evaluate the wave as a pulse explicitly, ignoring the <see cref="CrestWait"/>.
            /// </summary>
            /// <param name="time">The time value.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="realTimeWait">Whether to use real time (i.e. whether to use <see cref="WaveBase.Period"/> or <see cref="WaveBase.EffectivePeriod"/>).</param>
            /// <returns>Item1: The value of the wave at the given time and offset.<br/>Item2: Whether youre currently travelling up the wave (=1) or down the wave (=-1).</returns>
            /// <exception cref="System.Exception"></exception>
            public (float, int) EvaluateAsPulse(float time, float offset, bool realTimeWait = true)
            {
                float interval = (TroughWait) * (realTimeWait ? Velocity : 1f) + EffectivePeriod;
                float t = CalculatePulseT(time, offset, interval, -1);

                if (interval > 0)
                    t %= interval;

                // If 0, we are at start of up curve.
                if (t <= 0) return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 0f), 1);

                // If smaller than effective up period, we are travelling up the curve
                if (t <= EffectiveUpPeriod)
                    return (
                        Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod)),
                        1);

                // There is no crest wait, so if we are smaller than the effective period, we are travelling down the curve
                if (t <= (EffectivePeriod))
                    return (
                        Amplitude * GetValue(DownwardCurve, WrapMode.PingPong,
                            Mathf.Lerp(1f, 2f, (t - EffectiveUpPeriod) / EffectiveDownPeriod)), -1);

                // If larger than effective period, is trough waiting
                return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, 2f), -1);
            }

            /// <summary>
            /// Evaluate the wave as an inverted pulse explicitly, ignoring the <see cref="TroughWait"/>.
            /// </summary>
            /// <param name="time">The time value.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="realTimeWait">Whether to use real time (i.e. whether to use <see cref="WaveBase.Period"/> or <see cref="WaveBase.EffectivePeriod"/>).</param>
            /// <returns>Item1: The value of the wave at the given time and offset.<br/>Item2: Whether youre currently travelling up the wave (=1) or down the wave (=-1).</returns>
            /// <exception cref="System.Exception"></exception>
            public (float, int) EvaluateAsInvertedPulse(float time, float offset, bool realTimeWait = true)
            {
                float wait = CrestWait * (realTimeWait ? Velocity : 1f);
                float interval = wait + EffectivePeriod;
                float t = CalculatePulseT(time, offset, interval, -1);

                if (interval > 0)
                    t %= interval;

                // If 0, we are at start of up curve.
                if (t <= 0) return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 0f), 1);

                // If smaller than effective up period, we are travelling up the curve
                if (t <= EffectiveUpPeriod)
                    return (
                        Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod)),
                        1);

                // If smaller than effective up period + wait, we are waiting
                if (t <= EffectiveUpPeriod + wait)
                    return (
                        Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1), 1);

                // Otherwise we are travelling down the curve
                return (
                    Amplitude * GetValue(DownwardCurve, WrapMode.PingPong,
                        Mathf.Lerp(1f, 2f, (t - EffectiveUpPeriod - wait) / EffectiveDownPeriod)), -1);
            }

            /// <summary>
            /// Evaluate the wave as a one-directional pulse explicitly.
            /// </summary>
            /// <param name="time">The time value.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="realTimeWait">Whether to use real time (i.e. whether to use <see cref="WaveBase.Period"/> or <see cref="WaveBase.EffectivePeriod"/>).</param>
            /// <returns>Item1: The value of the wave at the given time and offset.<br/>Item2: Whether youre currently travelling up the wave (=1) or down the wave (=-1).</returns>
            /// <exception cref="System.Exception"></exception>
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
                    return (
                        Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, Mathf.Lerp(0f, 1f, t / EffectiveUpPeriod)),
                        1);
                }

                t -= EffectiveUpPeriod;
                if (t <= upInterval)
                {
                    return (Amplitude * GetValue(UpwardCurve, WrapMode.PingPong, 1f), 1);
                }

                t -= upInterval;
                if (t <= EffectiveDownPeriod)
                {
                    return (
                        Amplitude * GetValue(DownwardCurve, WrapMode.PingPong,
                            Mathf.Lerp(1f, 2f, t / EffectiveDownPeriod)), -1);
                }

                t -= EffectiveDownPeriod;
                if (t <= downInterval)
                {
                    return (Amplitude * GetValue(DownwardCurve, WrapMode.PingPong, Mathf.Lerp(1f, 2f, 1f)), -1);
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

                if (upwardCurve == null || upwardCurve.keys.Length == 0)
                    upwardCurve = AnimationCurveUtility.EaseInOutSine();
                if (downwardCurve == null || downwardCurve.keys.Length == 0)
                    downwardCurve = AnimationCurveUtility.EaseInOutSine();

                crestWait = Mathf.Max(crestWait, 0f);
                troughWait = Mathf.Max(troughWait, 0f);
            }

            public override void OnAfterDeserialize()
            {
                base.OnAfterDeserialize();

                if (upwardCurve == null || upwardCurve.keys.Length == 0)
                    upwardCurve = AnimationCurveUtility.EaseInOutSine();
                if (downwardCurve == null || downwardCurve.keys.Length == 0)
                    downwardCurve = AnimationCurveUtility.EaseInOutSine();

                troughWait = Mathf.Max(troughWait, 0f);
                crestWait = Mathf.Max(crestWait, 0f);

                //EvaluateCopy(0f, 0f, true);
            }


            [Tooltip(
                "The \"up\" part of the wave. This is the curve that is used to travel from trough to crest, or from the wave's lowest to its highest point.")]
            [SerializeField]
            private AnimationCurve upwardCurve;

            [Tooltip(
                "The \"down\" part of the wave. This is the curve that is used to travel from crest to trough, or from the wave's highest to its lowest point.")]
            [SerializeField]
            private AnimationCurve downwardCurve;

            [Tooltip("The amount of time to remain at the crest before moving down again, in seconds.")]
            [SerializeField]
            private float crestWait;

            [Tooltip("The amount of time to remain at the trough before moving up again, in seconds.")] [SerializeField]
            private float troughWait;
        }


        // TODO Quick solution for using this from TMPMeshModifier, where no AnimationContext is available
        // Could probably mock it instead
        public static float GetOffset(CharData cData, IAnimatorContext context, ParameterTypes.OffsetType type,
            int segmentIndex = 0,
            bool ignoreScaling = false)
        {
            switch (type)
            {
                case ParameterTypes.OffsetType.SegmentIndex: return segmentIndex;
                case ParameterTypes.OffsetType.Index: return cData.info.index;

                case ParameterTypes.OffsetType.Line: return cData.info.lineNumber;
                case ParameterTypes.OffsetType.Baseline: return cData.info.baseLine;
                case ParameterTypes.OffsetType.Word: return cData.info.wordNumber;

                case ParameterTypes.OffsetType.WorldXPos:
                    return ScalePos(context.Animator.transform.TransformPoint(cData.InitialPosition).x);
                case ParameterTypes.OffsetType.WorldYPos:
                    return ScalePos(context.Animator.transform.TransformPoint(cData.InitialPosition).y);
                case ParameterTypes.OffsetType.WorldZPos:
                    return ScalePos(context.Animator.transform.TransformPoint(cData.InitialPosition).z);
                case ParameterTypes.OffsetType.XPos: return ScalePos(cData.InitialPosition.x);
                case ParameterTypes.OffsetType.YPos: return ScalePos(cData.InitialPosition.y);
            }

            throw new System.NotImplementedException(nameof(type));

            float ScalePos(float pos)
            {
                if (ignoreScaling) return pos;

                // Rewrote ScaleVector with float here for performance
                // Ideally would reuse same code
                pos = ScaleTextMesh(context.Animator.TextComponent, pos);

                if (!context.ScaleAnimations)
                    return pos / 10f;

                if (context.ScaleUniformly)
                {
                    if (context.Animator.TextComponent.fontSize != 0)
                        pos /= (context.Animator.TextComponent.fontSize / 36f);
                    return pos / 10f;
                }
                else
                {
                    if (cData.info.pointSize != 0) pos /= (cData.info.pointSize / 36f);
                    return pos / 10f;
                }
            }
        }

        /// <summary>
        /// Get the wave offset to use based on the <paramref name="type"/>.<br/>
        /// To be used with <see cref="Wave.Evaluate(float, float, bool)"/> (and related methods).
        /// </summary>
        /// <param name="cData">The character to get the offset for.</param>
        /// <param name="context">The context of the animation.</param>
        /// <param name="type">The type of the offset.</param>
        /// <param name="ignoreScaling">Whether to ignore the scaling of the character, if applicable.<br/>
        /// For example, an <see cref="ParameterTypes.OffsetType.XPos"/> will return a different offset based on the size of the text,
        /// as it directly considers the x position of the character.</param>
        /// <returns>The offset for a wave.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static float GetOffset(CharData cData, IAnimationContext context, ParameterTypes.OffsetType type,
            bool ignoreScaling = false)
        {
            switch (type)
            {
                case ParameterTypes.OffsetType.SegmentIndex:
                    return context.SegmentData.SegmentIndexOf(cData);
                case ParameterTypes.OffsetType.Index: return cData.info.index;

                case ParameterTypes.OffsetType.Line: return cData.info.lineNumber;
                case ParameterTypes.OffsetType.Baseline: return cData.info.baseLine;
                case ParameterTypes.OffsetType.Word: return cData.info.wordNumber;

                case ParameterTypes.OffsetType.WorldXPos:
                    return ScalePos(context.AnimatorContext.Animator.transform.TransformPoint(cData.InitialPosition).x);
                case ParameterTypes.OffsetType.WorldYPos:
                    return ScalePos(context.AnimatorContext.Animator.transform.TransformPoint(cData.InitialPosition).y);
                case ParameterTypes.OffsetType.WorldZPos:
                    return ScalePos(context.AnimatorContext.Animator.transform.TransformPoint(cData.InitialPosition).z);
                case ParameterTypes.OffsetType.XPos: return ScalePos(cData.InitialPosition.x);
                case ParameterTypes.OffsetType.YPos: return ScalePos(cData.InitialPosition.y);
            }

            throw new System.NotImplementedException(nameof(type));

            float ScalePos(float pos)
            {
                if (ignoreScaling) return pos;

                // Rewrote ScaleVector with float here for performance
                // Ideally would reuse same code
                pos = ScaleTextMesh(context.AnimatorContext.Animator.TextComponent, pos);

                if (!context.AnimatorContext.ScaleAnimations)
                    return pos / 10f;

                if (context.AnimatorContext.ScaleUniformly)
                {
                    if (context.AnimatorContext.Animator.TextComponent.fontSize != 0)
                        pos /= (context.AnimatorContext.Animator.TextComponent.fontSize / 36f);
                    return pos / 10f;
                }
                else
                {
                    if (cData.info.pointSize != 0) pos /= (cData.info.pointSize / 36f);
                    return pos / 10f;
                }
            }
        }

        #endregion

        /// <summary>
        /// Set a character's UVs so it will look like another character.
        /// </summary>
        /// <param name="newCharacter">The character to change to.</param>
        /// <param name="originalCharacter">The original character of the <paramref name="cData"/>.</param>
        /// <param name="cData">The <see cref="CharData"/> of the character.</param>
        /// <param name="context">The context of the animation.</param>
        public static void SetToCharacter(TMP_Character newCharacter, TMP_Character originalCharacter, CharData cData,
            IAnimationContext context)
        {
            float baseSpriteScale = originalCharacter.scale * originalCharacter.glyph.scale;
            Vector2 origin = new Vector2(cData.info.origin, cData.info.baseLine);
            float spriteScale = cData.info.referenceScale / baseSpriteScale * newCharacter.scale *
                                newCharacter.glyph.scale;

            float horizontalBearingXDelta = newCharacter.glyph.metrics.horizontalBearingX -
                                            originalCharacter.glyph.metrics.horizontalBearingX;
            float horizontalBearingYDelta = newCharacter.glyph.metrics.horizontalBearingY -
                                            originalCharacter.glyph.metrics.horizontalBearingY;
            float heightDelta = newCharacter.glyph.metrics.height - originalCharacter.glyph.metrics.height;
            float widthDelta = newCharacter.glyph.metrics.width - originalCharacter.glyph.metrics.width;


            Vector3 bl = new Vector3(cData.InitialMesh.BL_Position.x + horizontalBearingXDelta * spriteScale,
                cData.InitialMesh.BL_Position.y + (horizontalBearingYDelta - heightDelta) * spriteScale);
            Vector3 tl = new Vector3(bl.x,
                cData.InitialMesh.TL_Position.y + horizontalBearingYDelta * spriteScale);
            Vector3 tr = new Vector3(
                cData.InitialMesh.TR_Position.x + (horizontalBearingXDelta + widthDelta) * spriteScale,
                tl.y);
            Vector3 br = new Vector3(tr.x, bl.y);

            var fontAsset = cData.info.fontAsset;

            Rect glyphRectDelta = new Rect(
                newCharacter.glyph.glyphRect.x - originalCharacter.glyph.glyphRect.x,
                newCharacter.glyph.glyphRect.y - originalCharacter.glyph.glyphRect.y,
                newCharacter.glyph.glyphRect.width - originalCharacter.glyph.glyphRect.width,
                newCharacter.glyph.glyphRect.height - originalCharacter.glyph.glyphRect.height
            );

            Vector2 uv0 = new Vector2(cData.InitialMesh.BL_UV0.x + (glyphRectDelta.x / fontAsset.atlasWidth),
                cData.InitialMesh.BL_UV0.y + (glyphRectDelta.y / fontAsset.atlasHeight));
            Vector2 uv1 = new Vector2(uv0.x,
                cData.InitialMesh.TL_UV0.y + (glyphRectDelta.y + glyphRectDelta.height) / fontAsset.atlasHeight);
            Vector2 uv2 = new Vector2(
                cData.InitialMesh.TR_UV0.x + (glyphRectDelta.x + glyphRectDelta.width) / fontAsset.atlasWidth,
                uv1.y);
            Vector2 uv3 = new Vector2(uv2.x, uv0.y);

            // TODO Now i again feel like the modifiers should be accessible
            // through the chardata directly and not the context
            context.AnimatorContext.Modifiers.MeshModifiers.BL_Delta = Vector3.zero;
            context.AnimatorContext.Modifiers.MeshModifiers.TL_Delta = Vector3.zero;
            context.AnimatorContext.Modifiers.MeshModifiers.TR_Delta = Vector3.zero;
            context.AnimatorContext.Modifiers.MeshModifiers.BR_Delta = Vector3.zero;

            // TODO
            // This'll fuck up when you got multiple
            // <char> chained, or more realistically
            // <char><+char>Lorem ipsum</></+>
            // Potential TODO
            // Add back concept of SetPosition for 
            // CharData + its indices that override
            // previous changes to stuff
            // Or: just dont chain those lol
            // 
            // For now: Do the reset on top
            // Might actually be fine to keep like that.
            SetVertexRaw(0, bl, cData, context);
            SetVertexRaw(1, tl, cData, context);
            SetVertexRaw(2, tr, cData, context);
            SetVertexRaw(3, br, cData, context);

            cData.mesh.SetUV0(0, uv0);
            cData.mesh.SetUV0(1, uv1);
            cData.mesh.SetUV0(2, uv2);
            cData.mesh.SetUV0(3, uv3);
        }

        /// <summary>
        /// Evaluate an <see cref="AnimationCurve"/> with different <see cref="WrapMode"/>s.
        /// </summary>
        /// <param name="curve">The curve to evaluate.</param>
        /// <param name="wrapMode">The <see cref="WrapMode"/> to use.</param>
        /// <param name="time">The time value.</param>
        /// <returns>The value of the curve at the given time value.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static float GetValue(AnimationCurve curve, WrapMode wrapMode, float time)
        {
            return GetValue(curve, wrapMode.ToTMPWrapMode(), time);
        }

        public static float GetValue(AnimationCurve curve, TMPWrapMode wrapMode, float time)
        {
            float t;
            switch (wrapMode)
            {
                case TMPWrapMode.Loop:
                    t = Mathf.Repeat(time, 1);
                    return curve.Evaluate(t);
                case TMPWrapMode.PingPong:
                    t = Mathf.PingPong(time, 1);
                    return curve.Evaluate(t);
                case TMPWrapMode.Clamp:
                    return curve.Evaluate(time);

                default: throw new System.ArgumentException("TMPWrapMode " + wrapMode.ToString() + " not supported");
            }
        }

        internal static TMPWrapMode ToTMPWrapMode(this WrapMode wrapMode)
        {
            switch (wrapMode)
            {
                case WrapMode.PingPong: return TMPWrapMode.PingPong;
                case WrapMode.Clamp: return TMPWrapMode.Clamp;
                case WrapMode.Loop: return TMPWrapMode.Loop;
                default:
                    throw new System.NotSupportedException("WrapMode " + wrapMode.ToString() +
                                                           " can not be converted to TMPWrapMode");
            }
        }

        public static WrapMode ToWrapMode(this TMPWrapMode wrapMode)
        {
            switch (wrapMode)
            {
                case TMPWrapMode.PingPong: return WrapMode.PingPong;
                case TMPWrapMode.Clamp: return WrapMode.Clamp;
                case TMPWrapMode.Loop: return WrapMode.Loop;
                default:
                    throw new System.NotSupportedException("TMPWrapMode " + wrapMode.ToString() +
                                                           " can not be converted to WrapMode");
            }
        }

        [Serializable]
        public enum TMPWrapMode
        {
            Clamp,
            Loop,
            PingPong,
        }
    }
}