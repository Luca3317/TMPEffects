using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Parameters
{
    public static class ParameterTypes
    {
        /// <summary>
        /// Different built-in offset types, to be used with <see cref="GetWaveOffset(CharData, IAnimationContext, WaveOffsetType)"/>.
        /// </summary>
        public enum WaveOffsetType
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
            public Vector2 vector;
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

            public static TypedVector2 operator +(TypedVector2 a, Vector2 b)
            {
                a.vector += b;
                return a;
            }

            public static TypedVector2 operator -(TypedVector2 a, Vector2 b)
            {
                a.vector -= b;
                return a;
            }

            public Vector2 ToPosition(CharData cData)
            {
                return type switch
                {
                    VectorType.Position => vector,
                    VectorType.Anchor => AnimationUtility.AnchorToPosition(vector, cData),
                    VectorType.Offset => (Vector3)vector + cData.InitialPosition,
                    _ => throw new System.NotImplementedException()
                };
            }

            public Vector2 ToDelta(CharData cData)
            {
                return type switch
                {
                    VectorType.Position => vector - (Vector2)cData.InitialPosition,
                    VectorType.Anchor => AnimationUtility.AnchorToPosition(vector, cData) -
                                         (Vector2)cData.InitialPosition,
                    VectorType.Offset => vector,
                    _ => throw new System.NotImplementedException(nameof(type))
                };
            }
            
            public Vector2 ToPosition(CharData cData, Vector2 referencePos)
            {
                return type switch
                {
                    VectorType.Position => vector,
                    VectorType.Anchor => AnimationUtility.AnchorToPosition(vector, cData),
                    VectorType.Offset => vector + referencePos,
                    _ => throw new System.NotImplementedException(nameof(type))
                };
            }
            
            public Vector2 ToDelta(CharData cData, Vector2 referencePos)
            {
                return type switch
                {
                    VectorType.Position => vector - referencePos,
                    VectorType.Anchor => AnimationUtility.AnchorToPosition(vector, cData) - referencePos,
                    VectorType.Offset => vector,
                    _ => throw new System.NotImplementedException(nameof(type))
                };
            }
        }

        /// <summary>
        /// A <see cref="Vector3"/> with a <see cref="VectorType"/>.
        /// </summary>
        [System.Serializable]
        public struct TypedVector3
        {
            public Vector3 vector;
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

            public static TypedVector3 operator +(TypedVector3 a, Vector3 b)
            {
                a.vector += b;
                return a;
            }

            public static TypedVector3 operator -(TypedVector3 a, Vector3 b)
            {
                a.vector -= b;
                return a;
            }

            public TypedVector3 IgnoreScaling(CharData cData, IAnimationContext context)
                => IgnoreScaling(cData, context.AnimatorContext);

            public TypedVector3 IgnoreScaling(CharData cData, IAnimatorContext context)
            {
                switch (type)
                {
                    case VectorType.Position:
                        vector = AnimationUtility.GetRawPosition(vector, cData, context);
                        break;
                    case VectorType.Anchor:
                        // TODO Dont have to do anything i think. Since based on anchors of character,
                        // inherently ignores scaling
                        // vector = AnimationUtility.GetRawPosition(AnimationUtility.AnchorToPosition(vector, cData),
                        //     cData, context);
                        break;
                    case VectorType.Offset:
                        vector = AnimationUtility.GetRawDelta(vector, cData, context);
                        break;
                }
                return this;
            }

            public Vector3 ToPosition(CharData cData)
            {
                return type switch
                {
                    VectorType.Position => vector,
                    VectorType.Anchor => AnimationUtility.AnchorToPosition(vector, cData),
                    VectorType.Offset => vector + cData.InitialPosition,
                    _ => throw new System.NotImplementedException(nameof(type))
                };
            }

            public Vector3 ToDelta(CharData cData)
            {
                return type switch
                {
                    VectorType.Position => vector - cData.InitialPosition,
                    VectorType.Anchor => AnimationUtility.AnchorToPosition(vector, cData) -
                                         (Vector2)cData.InitialPosition,
                    VectorType.Offset => vector,
                    _ => throw new System.NotImplementedException(nameof(type))
                };
            }

            public Vector3 ToPosition(CharData cData, Vector3 referencePos)
            {
                return type switch
                {
                    VectorType.Position => vector,
                    VectorType.Anchor => AnimationUtility.AnchorToPosition(vector, cData),
                    VectorType.Offset => vector + referencePos,
                    _ => throw new System.NotImplementedException(nameof(type))
                };
            }
            
            public Vector3 ToDelta(CharData cData, Vector3 referencePos)
            {
                return type switch
                {
                    VectorType.Position => vector - referencePos,
                    VectorType.Anchor => (Vector3)AnimationUtility.AnchorToPosition(vector, cData) - referencePos,
                    VectorType.Offset => vector,
                    _ => throw new System.NotImplementedException(nameof(type))
                };
            }
        }
    }
}