using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Parameters
{
    /// <summary>
    /// Some types needed for specific parameters.
    /// </summary>
    public static class TMPParameterTypes
    {
        /// <summary>
        /// Different built-in offset types, to be used with <see cref="GetOffset(CharData, IAnimationContext, OffsetType)"/>.
        /// </summary>
        public enum OffsetType
        {
            SegmentIndex = 0,
            Index = 5,
            XPos = 10,
            YPos = 15,

            WorldXPos = 20,
            WorldYPos = 25,
            WorldZPos = 30,

            Word = 35,
            Line = 40,
            Baseline = 45
        }

        /// <summary>
        /// The different types of vectors.
        /// </summary>
        public enum VectorType
        {
            /// <summary>
            /// Pure position (normal vector).
            /// </summary>
            Position,

            /// <summary>
            /// Offset vector from the original position.
            /// </summary>
            Offset,

            /// <summary>
            /// Anchor vector, in "character space"; (0,0) is the center, (1,1) top-right, (-1,0) center-left etc.
            /// </summary>
            Anchor
        }

        /// <summary>
        /// A <see cref="Vector2"/> with a <see cref="VectorType"/>.
        /// </summary>
        [System.Serializable]
        public struct TypedVector2
        {
            /// <summary>
            /// The raw vector.
            /// </summary>
            public Vector2 vector;

            /// <summary>
            /// The type of this vector.
            /// </summary>
            public VectorType type;

            public TypedVector2(VectorType type, Vector2 vector)
            {
                this.type = type;
                this.vector = vector;
            }

            public static implicit operator TypedVector2(TypedVector3 v)
            {
                return new TypedVector2() { vector = v.vector, type = v.type };
            }

            private TypedVector2 IgnoreScaling(CharData cData, IAnimationContext context)
                => IgnoreScaling(cData, context.AnimatorContext);

            private TypedVector2 IgnoreScaling(CharData cData, IAnimatorDataProvider context)
            {
                return type switch
                {
                    VectorType.Position => new TypedVector2(type,
                        TMPAnimationUtility.GetRawPosition(vector, cData, context)),
                    VectorType.Anchor =>
                        new TypedVector2(type, vector),
                    VectorType.Offset => new TypedVector2(type,
                        TMPAnimationUtility.GetRawDelta(vector, cData, context)),
                    _ => throw new System.NotImplementedException(nameof(type))
                };
            }

            /// <summary>
            /// Get the position defined by this vector, using the <see cref="cData"/>'s initial position as reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="context"></param>
            /// <returns>The position defined by this vector and the <see cref="cData"/>'s initial position.</returns>
            public Vector2 ToPosition(CharData cData, IAnimationContext context) =>
                ToPosition(cData, context.AnimatorContext, cData.InitialPosition);

            /// <summary>
            /// Get the position defined by this vector and the reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="context"></param>
            /// <param name="referencePos"></param>
            /// <returns>The position defined by this vector and the reference position.</returns>
            public Vector2 ToPosition(CharData cData, IAnimationContext context, Vector2 referencePos) =>
                ToPosition(cData, context.AnimatorContext, referencePos);

            /// <summary>
            /// Get the position defined by this vector, using the <see cref="cData"/>'s initial position as reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="animatorData"></param>
            /// <returns>The position defined by this vector and the reference position.</returns>
            public Vector2 ToPosition(CharData cData, IAnimatorDataProvider animatorData) =>
                ToPosition(cData, animatorData, cData.InitialPosition);

            /// <summary>
            /// Get the position defined by this vector and the reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="animatorData"></param>
            /// <param name="referencePos"></param>
            /// <returns>The position defined by this vector and the reference position.</returns>
            public Vector2 ToPosition(CharData cData, IAnimatorDataProvider animatorData, Vector2 referencePos)
            {
                TypedVector2 vec = this;
                switch (type)
                {
                    case VectorType.Position:
                        return vec.IgnoreScaling(cData, animatorData).vector;
                    case VectorType.Anchor:
                        TypedVector2 vec2 = new TypedVector2(VectorType.Position,
                            TMPAnimationUtility.AnchorToPosition(vector, cData));
                        return vec2.ToPosition(cData, animatorData);
                    case VectorType.Offset:
                        return referencePos + vector;

                    default: throw new System.NotImplementedException(nameof(type));
                }
            }

            /// <summary>
            /// Get the delta defined by this vector, using the <see cref="cData"/>'s initial position as reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="context"></param>
            /// <returns>The delta defined by this vector and the <see cref="cData"/>'s initial position.</returns>
            public Vector2 ToDelta(CharData cData, IAnimationContext context) =>
                ToDelta(cData, context.AnimatorContext, cData.InitialPosition);

            /// <summary>
            /// Get the delta defined by this vector and the reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="context"></param>
            /// <returns>The delta defined by this vector and the reference position.</returns>
            public Vector2 ToDelta(CharData cData, IAnimationContext context, Vector2 referencePos) =>
                ToDelta(cData, context.AnimatorContext, referencePos);

            /// <summary>
            /// Get the delta defined by this vector, using the <see cref="cData"/>'s initial position as reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="animatorData"></param>
            /// <returns>The delta defined by this vector and the <see cref="cData"/>'s initial position.</returns>
            public Vector2 ToDelta(CharData cData, IAnimatorDataProvider animatorData) =>
                ToDelta(cData, animatorData, cData.InitialPosition);

            /// <summary>
            /// Get the delta defined by this vector and the reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="animatorData"></param>
            /// <param name="referencePos"></param>
            /// <returns>The delta defined by this vector and the <see cref="cData"/>'s initial position.</returns>
            public Vector2 ToDelta(CharData cData, IAnimatorDataProvider animatorData, Vector2 referencePos)
            {
                switch (type)
                {
                    case VectorType.Position:
                        TypedVector2 vec2 = this;
                        return vec2.IgnoreScaling(cData, animatorData).vector - referencePos;

                    case VectorType.Anchor:
                        vec2 = new TypedVector2(VectorType.Position,
                            TMPAnimationUtility.AnchorToPosition(vector, cData));
                        return vec2.ToDelta(cData, animatorData, referencePos);
                    case VectorType.Offset:
                        return vector;

                    default: throw new System.NotImplementedException(nameof(type));
                }
            }

            public override string ToString()
            {
                return "{ " + vector + ", " + type + " }";
            }
        }

        /// <summary>
        /// A <see cref="Vector3"/> with a <see cref="VectorType"/>.
        /// </summary>
        [System.Serializable]
        public struct TypedVector3
        {
            /// <summary>
            /// The raw vector.
            /// </summary>
            public Vector3 vector;

            /// <summary>
            /// The type of this vector.
            /// </summary>
            public VectorType type;

            public TypedVector3(VectorType type, Vector3 vector)
            {
                this.type = type;
                this.vector = vector;
            }

            public bool Equals(TypedVector3 other)
            {
                return vector == other.vector && type == other.type;
            }

            public static implicit operator TypedVector3(TypedVector2 v)
            {
                return new TypedVector3() { vector = v.vector, type = v.type };
            }

            private TypedVector3 IgnoreScaling(CharData cData, IAnimationContext context)
                => IgnoreScaling(cData, context.AnimatorContext);

            private TypedVector3 IgnoreScaling(CharData cData, IAnimatorDataProvider context)
            {
                return type switch
                {
                    VectorType.Position => new TypedVector3(type,
                        TMPAnimationUtility.GetRawPosition(vector, cData, context)),
                    VectorType.Offset => new TypedVector3(type,
                        TMPAnimationUtility.GetRawDelta(vector, cData, context)),
                    VectorType.Anchor => this,
                    _ => throw new System.NotImplementedException(nameof(type))
                };
            }

            /// <summary>
            /// Get the position defined by this vector, using the <see cref="cData"/>'s initial position as reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="context"></param>
            /// <returns>The position defined by this vector and the <see cref="cData"/>'s initial position.</returns>
            public Vector3 ToPosition(CharData cData, IAnimationContext context) =>
                ToPosition(cData, context.AnimatorContext, cData.InitialPosition);

            /// <summary>
            /// Get the position defined by this vector and the reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="context"></param>
            /// <param name="referencePos"></param>
            /// <returns>The position defined by this vector and the reference position.</returns>
            public Vector3 ToPosition(CharData cData, IAnimationContext context, Vector3 referencePos) =>
                ToPosition(cData, context.AnimatorContext, referencePos);

            /// <summary>
            /// Get the position defined by this vector, using the <see cref="cData"/>'s initial position as reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="animatorData"></param>
            /// <returns>The position defined by this vector and the reference position.</returns>
            public Vector3 ToPosition(CharData cData, IAnimatorDataProvider animatorData) =>
                ToPosition(cData, animatorData, cData.InitialPosition);

            /// <summary>
            /// Get the position defined by this vector and the reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="animatorData"></param>
            /// <param name="referencePos"></param>
            /// <returns>The position defined by this vector and the reference position.</returns>
            public Vector3 ToPosition(CharData cData, IAnimatorDataProvider animatorData, Vector3 referencePos)
            {
                TypedVector3 vec = this;
                switch (type)
                {
                    case VectorType.Position:
                        return vec.IgnoreScaling(cData, animatorData).vector;
                    case VectorType.Anchor:
                        TypedVector3 vec2 = new TypedVector3(VectorType.Position,
                            TMPAnimationUtility.AnchorToPosition(vector, cData));
                        return vec2.ToPosition(cData, animatorData);
                    case VectorType.Offset:
                        return referencePos + vector;

                    default: throw new System.NotImplementedException(nameof(type));
                }
            }

            /// <summary>
            /// Get the delta defined by this vector, using the <see cref="cData"/>'s initial position as reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="context"></param>
            /// <returns>The delta defined by this vector and the <see cref="cData"/>'s initial position.</returns>
            public Vector3 ToDelta(CharData cData, IAnimationContext context) =>
                ToDelta(cData, context.AnimatorContext, cData.InitialPosition);

            /// <summary>
            /// Get the delta defined by this vector and the reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="context"></param>
            /// <returns>The delta defined by this vector and the reference position.</returns>
            public Vector3 ToDelta(CharData cData, IAnimationContext context, Vector3 referencePos) =>
                ToDelta(cData, context.AnimatorContext, referencePos);

            /// <summary>
            /// Get the delta defined by this vector, using the <see cref="cData"/>'s initial position as reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="animatorData"></param>
            /// <returns>The delta defined by this vector and the <see cref="cData"/>'s initial position.</returns>
            public Vector3 ToDelta(CharData cData, IAnimatorDataProvider animatorData) =>
                ToDelta(cData, animatorData, cData.InitialPosition);

            /// <summary>
            /// Get the delta defined by this vector and the reference position.
            /// </summary>
            /// <param name="cData"></param>
            /// <param name="animatorData"></param>
            /// <param name="referencePos"></param>
            /// <returns>The delta defined by this vector and the <see cref="cData"/>'s initial position.</returns>
            public Vector3 ToDelta(CharData cData, IAnimatorDataProvider animatorData, Vector3 referencePos)
            {
                switch (type)
                {
                    case VectorType.Position:
                        TypedVector3 vec2 = this;
                        return vec2.IgnoreScaling(cData, animatorData).vector - referencePos;

                    case VectorType.Anchor:
                        vec2 = new TypedVector3(VectorType.Position,
                            TMPAnimationUtility.AnchorToPosition(vector, cData));
                        return vec2.ToDelta(cData, animatorData, referencePos);

                    case VectorType.Offset:
                        return vector;

                    default: throw new System.NotImplementedException(nameof(type));
                }
            }

            public override string ToString()
            {
                return "{ " + vector + ", " + type + " }";
            }
        }
    }
}