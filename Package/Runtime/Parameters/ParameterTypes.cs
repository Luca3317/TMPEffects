using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.ParameterUtilityGenerator.Attributes;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Parameters
{
    [TMPParameterType("OffsetProvider", typeof(OffsetTypePowerEnum), typeof(SceneOffsetTypePowerEnum), true)]
    public partial interface ITMPOffsetProvider
    {
        // TODO Split up context interfaces 
        public float GetOffset(CharData cData, IAnimationContext context);
        public float GetOffset(CharData cData, IAnimatorContext context);

        public static partial bool StringToOffsetProvider(string str, out ITMPOffsetProvider result,
            TMPEffects.Databases.ITMPKeywordDatabase keywords)
        {
            result = null;
            
            if (ParameterParsing.GlobalKeywordDatabase.TryGetOffsetProvider(str, out result)) return true;
            if (keywords != null && keywords.TryGetOffsetProvider(str, out result)) return true;
            
            switch (str)
            {
                case "sidx":
                case "sindex":
                case "segmentindex":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.SegmentIndex);
                    return true;

                case "idx":
                case "index":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.Index);
                    return true;

                case "word":
                case "wordidx":
                case "wordindex":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.Word);
                    return true;

                case "line":
                case "lineno":
                case "linenumber":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.Line);
                    return true;

                case "base":
                case "baseline":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.Baseline);
                    return true;

                case "x":
                case "xpos":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.XPos);
                    return true;

                case "y":
                case "ypos":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.YPos);
                    return true;

                case "wordly":
                case "worldypos":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.WorldYPos);
                    return true;

                case "wordlx":
                case "worldxpos":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.WorldXPos);
                    return true;

                case "wordlz":
                case "worldzpos":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.WorldZPos);
                    return true;
            }

            return false;
        }
    }

    public abstract class TMPOffsetProvider : ScriptableObject, ITMPOffsetProvider
    {
        public abstract float GetOffset(CharData cData, IAnimationContext context);
        public abstract float GetOffset(CharData cData, IAnimatorContext context);
    }

    public abstract class TMPSceneOffsetProvider : MonoBehaviour, ITMPOffsetProvider
    {
        public abstract float GetOffset(CharData cData, IAnimationContext context);
        public abstract float GetOffset(CharData cData, IAnimatorContext context);
    }

    public static class ParameterTypes
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

            public TypedVector2 IgnoreScaling(CharData cData, IAnimationContext context)
                => IgnoreScaling(cData, context.AnimatorContext);

            public TypedVector2 IgnoreScaling(CharData cData, IAnimatorContext context)
            {
                return type switch
                {
                    VectorType.Position => new TypedVector2(type,
                        AnimationUtility.GetRawPosition(vector, cData, context)),
                    VectorType.Anchor =>
                        // TODO Dont have to do anything i think. Since based on anchors of character,
                        // inherently ignores scaling
                        // vector = AnimationUtility.GetRawPosition(AnimationUtility.AnchorToPosition(vector, cData),
                        //     cData, context);
                        new TypedVector2(type, vector),
                    VectorType.Offset => new TypedVector2(type, AnimationUtility.GetRawDelta(vector, cData, context)),
                    _ => throw new System.NotImplementedException(nameof(type))
                };
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
                return type switch
                {
                    VectorType.Position => new TypedVector3(type,
                        AnimationUtility.GetRawPosition(vector, cData, context)),
                    VectorType.Offset => new TypedVector3(type, AnimationUtility.GetRawDelta(vector, cData, context)),
                    VectorType.Anchor => this,
                    _ => throw new System.NotImplementedException(nameof(type))
                };
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

            public override string ToString()
            {
                return "{ " + vector + ", " + type + " }";
            }
        }
    }
}