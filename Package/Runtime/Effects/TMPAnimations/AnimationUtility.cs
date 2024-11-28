using UnityEngine;
using TMPEffects.CharacterData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPEffects.Extensions;
using TMPEffects.Components.Animator;
using TMPro;
using TMPEffects.Components;
using TMPEffects.Databases;
using TMPEffects.Parameters;
using TMPEffects.ParameterUtilityGenerator.Attributes;
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
        
        internal static ITMPSegmentData GetMockedSegment(int len, IEnumerable<CharData> cData)
        {
            return new TextSegment(len, cData);
        }

        private struct TextSegment : ITMPSegmentData
        {
            public int StartIndex => 0;

            public int Length { get; }

            public int EndIndex => StartIndex + Length;

            public IReadOnlyDictionary<int, CharData.Info> CharInfo { get; }

            public int IndexToSegmentIndex(int index)
            {
                return index;
            }

            public int SegmentIndexOf(CharData cData)
            {
                return cData.info.index;
            }

            public TextSegment(int len, IEnumerable<CharData> cData)
            {
                Length = len;
                var dict = new Dictionary<int, CharData.Info>();

                foreach (var cd in cData)
                {
                    dict[cd.info.index] = cd.info;
                }

                CharInfo = new ReadOnlyDictionary<int, CharData.Info>(dict);
            }
        }

        public static float GetOffset(CharData cData, IAnimationContext context, ITMPOffsetProvider provider,
            bool ignoreScaling = false, bool ignoreSegmentLenght = false)
        {
            float offset = provider.GetOffset(cData, context.SegmentData, context.AnimatorContext, ignoreScaling);

            if (!ignoreSegmentLenght)
                offset /= context.SegmentData.Length == 0 ? 0.001f : context.SegmentData.Length;

            return offset;
        }

        public static float GetOffset(CharData cData, IAnimatorContext context, ITMPOffsetProvider provider,
            bool ignoreScaling = false, bool ignoreSegmentLenght = false)
        {
            var segmentData = GetMockedSegment(context.Animator.TextComponent.GetParsedText().Length,
                context.Animator.CharData);
            float offset = provider.GetOffset(cData, segmentData, context, ignoreScaling);

            if (!ignoreSegmentLenght)
            {
                offset /= segmentData.Length == 0 ? 0.001f : segmentData.Length;
            }

            return offset;
        }

        public static void GetMinMaxOffset(out float min, out float max,
            ParameterTypes.OffsetType type, ITMPSegmentData segmentData, IAnimatorDataProvider animatorData,
            bool ignoreAnimatorScaling = false)
        {
            switch (type)
            {
                case ParameterTypes.OffsetType.SegmentIndex:
                    min = 0;
                    max = segmentData.Length - 1;
                    return;
                case ParameterTypes.OffsetType.Index:
                    min = segmentData.StartIndex;
                    max = segmentData.StartIndex + segmentData.Length - 1;
                    return;
                case ParameterTypes.OffsetType.XPos:
                {
                    // Expensive af (and so will other ones be)
                    // Should give some indication that you should cache
                    min = float.MaxValue;
                    max = float.MinValue;
                    foreach (var kvp in segmentData.CharInfo)
                    {
                        var pos = ScalePos(kvp.Value.pointSize, kvp.Value.InitialPosition.x);
                        min = Mathf.Min(min, pos);
                        max = Mathf.Max(max, pos);
                    }

                    return;
                }

                case ParameterTypes.OffsetType.YPos:
                {
                    // Expensive af (and so will other ones be)
                    // Should give some indication that you should cache
                    min = float.MaxValue;
                    max = float.MinValue;
                    foreach (var kvp in segmentData.CharInfo)
                    {
                        var pos = ScalePos(kvp.Value.pointSize, kvp.Value.InitialPosition.y);
                        min = Mathf.Min(min, pos);
                        max = Mathf.Max(max, pos);
                    }

                    return;
                }
                case ParameterTypes.OffsetType.Line:
                {
                    min = int.MaxValue;
                    max = 0; // cant have negative line indices
                    foreach (var kvp in segmentData.CharInfo)
                    {
                        int line = kvp.Value.lineNumber;
                        min = Math.Min(min, line);
                        max = Math.Max(max, line);
                    }

                    return;
                }
                case ParameterTypes.OffsetType.Baseline:
                {
                    min = float.MaxValue;
                    max = float.MinValue;
                    foreach (var kvp in segmentData.CharInfo)
                    {
                        float line = kvp.Value.baseLine;
                        min = Math.Min(min, line);
                        max = Math.Max(max, line);
                    }

                    return;
                }
                case ParameterTypes.OffsetType.Word:
                {
                    min = int.MaxValue;
                    max = int.MinValue;
                    foreach (var kvp in segmentData.CharInfo)
                    {
                        int wordIndex = kvp.Value.wordNumber;
                        // if (wordIndex == -1) continue;
                        min = Math.Min(min, wordIndex);
                        max = Math.Max(max, wordIndex);
                    }

                    return;
                }
                case ParameterTypes.OffsetType.WorldXPos:
                {
                    // Expensive af (and so will other ones be)
                    // Should give some indication that you should cache
                    min = float.MaxValue;
                    max = float.MinValue;
                    foreach (var kvp in segmentData.CharInfo)
                    {
                        var pos = ScalePos(kvp.Value.pointSize,
                            animatorData.Animator.transform.TransformPoint(kvp.Value.InitialPosition).x);
                        min = Mathf.Min(min, pos);
                        max = Mathf.Max(max, pos);
                    }

                    return;
                }

                case ParameterTypes.OffsetType.WorldYPos:
                {
                    // Expensive af (and so will other ones be)
                    // Should give some indication that you should cache
                    min = float.MaxValue;
                    max = float.MinValue;
                    foreach (var kvp in segmentData.CharInfo)
                    {
                        var pos = ScalePos(kvp.Value.pointSize,
                            animatorData.Animator.transform.TransformPoint(kvp.Value.InitialPosition).y);
                        min = Mathf.Min(min, pos);
                        max = Mathf.Max(max, pos);
                    }

                    return;
                }

                case ParameterTypes.OffsetType.WorldZPos:
                {
                    // Expensive af (and so will other ones be)
                    // Should give some indication that you should cache
                    min = float.MaxValue;
                    max = float.MinValue;
                    foreach (var kvp in segmentData.CharInfo)
                    {
                        // TODO Scaling should probably only be done once you have found the min max
                        // HOWEVER That will assume all charinfos have the same pointsize (which they should? idk)
                        var pos = ScalePos(kvp.Value.pointSize,
                            animatorData.Animator.transform.TransformPoint(kvp.Value.InitialPosition).z);
                        min = Mathf.Min(min, pos);
                        max = Mathf.Max(max, pos);
                    }

                    return;
                }
            }

            throw new System.NotImplementedException("NOT IMPLEMENTED");

            float ScalePos(float pointSize, float pos)
            {
                if (ignoreAnimatorScaling) return pos;

                // Rewrote ScaleVector with float here for performance
                // Ideally would reuse same code
                pos = ScaleTextMesh(animatorData.Animator.TextComponent, pos);

                if (!animatorData.ScaleAnimations)
                    return pos / 10f;

                if (animatorData.ScaleUniformly)
                {
                    if (animatorData.Animator.TextComponent.fontSize != 0)
                        pos /= (animatorData.Animator.TextComponent.fontSize / 36f);
                    return pos / 10f;
                }
                else
                {
                    if (pointSize != 0) pos /= (pointSize / 36f);
                    return pos / 10f;
                }
            }
        }

        public static float GetOffset(ParameterTypes.OffsetType type, CharData cData, ITMPSegmentData segmentData,
            IAnimatorDataProvider animatorData, bool ignoreAnimatorScaling = false)
        {
            switch (type)
            {
                case ParameterTypes.OffsetType.SegmentIndex: return segmentData.SegmentIndexOf(cData);
                case ParameterTypes.OffsetType.Index: return cData.info.index;

                case ParameterTypes.OffsetType.Line: return cData.info.lineNumber;
                case ParameterTypes.OffsetType.Baseline: return cData.info.baseLine;
                case ParameterTypes.OffsetType.Word: return cData.info.wordNumber;

                case ParameterTypes.OffsetType.WorldXPos:
                    return ScalePos(animatorData.Animator.transform.TransformPoint(cData.InitialPosition).x);
                case ParameterTypes.OffsetType.WorldYPos:
                    return ScalePos(animatorData.Animator.transform.TransformPoint(cData.InitialPosition).y);
                case ParameterTypes.OffsetType.WorldZPos:
                    return ScalePos(animatorData.Animator.transform.TransformPoint(cData.InitialPosition).z);
                case ParameterTypes.OffsetType.XPos: return ScalePos(cData.InitialPosition.x);
                case ParameterTypes.OffsetType.YPos: return ScalePos(cData.InitialPosition.y);
            }

            throw new System.NotImplementedException(nameof(type));

            float ScalePos(float pos)
            {
                if (ignoreAnimatorScaling) return pos;

                // Rewrote ScaleVector with float here for performance
                // Ideally would reuse same code
                pos = ScaleTextMesh(animatorData.Animator.TextComponent, pos);

                if (!animatorData.ScaleAnimations)
                    return pos / 10f;

                if (animatorData.ScaleUniformly)
                {
                    if (animatorData.Animator.TextComponent.fontSize != 0)
                        pos /= (animatorData.Animator.TextComponent.fontSize / 36f);
                    return pos / 10f;
                }
                else
                {
                    if (cData.info.pointSize != 0) pos /= (cData.info.pointSize / 36f);
                    return pos / 10f;
                }
            }
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
                float t = CalculateT(time, offset, uniformity);
                return curve.Evaluate(time);
            }

            private float CalculateT(float time, float offset, float uniformity)
            {
                float t;
                t = (time) + offset * uniformity;
                return t;
            }
        }
        
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

            context.AnimatorContext.Modifiers.MeshModifiers.BL_Delta = Vector3.zero;
            context.AnimatorContext.Modifiers.MeshModifiers.TL_Delta = Vector3.zero;
            context.AnimatorContext.Modifiers.MeshModifiers.TR_Delta = Vector3.zero;
            context.AnimatorContext.Modifiers.MeshModifiers.BR_Delta = Vector3.zero;

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