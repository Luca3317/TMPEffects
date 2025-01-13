using System;
using System.Diagnostics;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TMPEffects.Modifiers
{
    /// <summary>
    /// Modifiers applied to a TextMeshPro character.<br/>
    /// Used by <see cref="CharData"/> to store modifications.
    /// </summary> 
    [Serializable]
    public class CharDataModifiers
    {
        /// <summary>
        /// The mesh modifiers, applied to individual vertices.
        /// </summary>
        public TMPMeshModifiers MeshModifiers => meshModifiers;

        /// <summary>
        /// The character modifiers, applied uniformly to all vertices of the character.
        /// </summary>
        public TMPCharacterModifiers CharacterModifiers=> characterModifiers;

        [SerializeField] private TMPMeshModifiers meshModifiers;
        [SerializeField] private TMPCharacterModifiers characterModifiers;

        /// <summary>
        /// Stores the position of the bottom-left vertex after calling <see cref="CalculateVertexPositions"/>.
        /// </summary>
        public Vector3 BL_Position { get; private set; }
    
        /// <summary>
        /// Stores the position of the top-left vertex after calling <see cref="CalculateVertexPositions"/>.
        /// </summary>
        public Vector3 TL_Position { get; private set; }
    
        /// <summary>
        /// Stores the position of the top-right vertex after calling <see cref="CalculateVertexPositions"/>.
        /// </summary>
        public Vector3 TR_Position { get; private set; }
    
        /// <summary>
        /// Stores the position of the bottom-right vertex after calling <see cref="CalculateVertexPositions"/>.
        /// </summary>
        public Vector3 BR_Position { get; private set; }
        
        /// <summary>
        /// Stores the color of the bottom-left vertex after calling <see cref="CalculateVertexColors"/>.
        /// </summary>
        public Color32 BL_Color { get; private set; }
        
        /// <summary>
        /// Stores the color of the top-left vertex after calling <see cref="CalculateVertexColors"/>.
        /// </summary>
        public Color32 TL_Color { get; private set; }
        
        /// <summary>
        /// Stores the color of the top-right vertex after calling <see cref="CalculateVertexColors"/>.
        /// </summary>
        public Color32 TR_Color { get; private set; }
        
        /// <summary>
        /// Stores the color of the bottom-right vertex after calling <see cref="CalculateVertexColors"/>.
        /// </summary>
        public Color32 BR_Color { get; private set; }

        public CharDataModifiers()
        {
            meshModifiers = new TMPMeshModifiers();
            characterModifiers = new TMPCharacterModifiers();
        }

        public CharDataModifiers(CharDataModifiers original)
        {
            meshModifiers = new TMPMeshModifiers(original.meshModifiers);
            characterModifiers = new TMPCharacterModifiers(original.characterModifiers);
        }

        /// <summary>
        /// Combine this <see cref="CharDataModifiers"/> with another.<br/>
        /// </summary>
        /// <param name="other">The other <see cref="CharacterModifiers"/> to combine with.</param>
        public void Combine(CharDataModifiers other)
        {
            meshModifiers.Combine(other.meshModifiers);
            characterModifiers.Combine(other.characterModifiers);
        }

        /// <summary>
        /// Calculate the vertex colors, applying all relevant modifiers to the passed in <see cref="CharData"/>.
        /// The results are stored in <see cref="BL_Color"/>, <see cref="TL_Color"/>, <see cref="TR_Color"/>, <see cref="BR_Color"/>.
        /// </summary>
        /// <param name="cData">The CharData to apply the modifiers to.</param>
        /// <param name="context"></param>
        public void CalculateVertexColors(CharData cData, IAnimatorDataProvider context)
        {
            BL_Color = context.Modifiers.MeshModifiers.BL_Color.GetValue(cData.InitialMesh.GetColor(0));
            TL_Color = context.Modifiers.MeshModifiers.TL_Color.GetValue(cData.InitialMesh.GetColor(1));
            TR_Color = context.Modifiers.MeshModifiers.TR_Color.GetValue(cData.InitialMesh.GetColor(2));
            BR_Color = context.Modifiers.MeshModifiers.BR_Color.GetValue(cData.InitialMesh.GetColor(3));
        }

        /// <summary>
        /// Calculate the vertex positions, applying all relevant modifiers to the passed in <see cref="CharData"/>.<br/>
        /// The results are stored in <see cref="BL_Position"/>, <see cref="TL_Position"/>, <see cref="TR_Position"/>, <see cref="BR_Position"/>.
        /// </summary>
        /// <param name="cData">The CharData to apply the modifiers to.</param>
        /// <param name="context"></param>
        public void CalculateVertexPositions(CharData cData, IAnimatorDataProvider context)
        {
            // Apply vertex transformations
            Vector3 vbl = cData.InitialMesh.BL_Position;
            Vector3 vtl = cData.InitialMesh.TL_Position;
            Vector3 vtr = cData.InitialMesh.TR_Position;
            Vector3 vbr = cData.InitialMesh.BR_Position;

            // Apply deltas
            if (meshModifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Deltas))
            {
                vbl += TMPAnimationUtility.ScaleVector(meshModifiers.BL_Delta, cData, context);
                vtl += TMPAnimationUtility.ScaleVector(meshModifiers.TL_Delta, cData, context);
                vtr += TMPAnimationUtility.ScaleVector(meshModifiers.TR_Delta, cData, context);
                vbr += TMPAnimationUtility.ScaleVector(meshModifiers.BR_Delta, cData, context);
            }

            // Apply scale
            if (characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Scale))
            {
                vbl = characterModifiers.ScaleDelta.MultiplyPoint3x4(vbl - cData.InitialPosition) +
                      cData.InitialPosition;
                vtl = characterModifiers.ScaleDelta.MultiplyPoint3x4(vtl - cData.InitialPosition) +
                      cData.InitialPosition;
                vtr = characterModifiers.ScaleDelta.MultiplyPoint3x4(vtr - cData.InitialPosition) +
                      cData.InitialPosition;
                vbr = characterModifiers.ScaleDelta.MultiplyPoint3x4(vbr - cData.InitialPosition) +
                      cData.InitialPosition;
            }

            // Apply rotation
            if (characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Rotations))
            {
                Vector3 pivot;
                Matrix4x4 matrix;

                for (int i = 0; i < characterModifiers.Rotations.Count; i++)
                {
                    var rot = characterModifiers.Rotations[i];

                    if (rot.eulerAngles == Vector3.zero) continue;

                    pivot = cData.InitialPosition +
                            TMPAnimationUtility.ScaleVector(rot.pivot - cData.InitialPosition, cData, context);

                    matrix = Matrix4x4.Rotate(Quaternion.Euler(rot.eulerAngles));

                    vbl = matrix.MultiplyPoint3x4(vbl - pivot) + pivot;
                    vtl = matrix.MultiplyPoint3x4(vtl - pivot) + pivot;
                    vtr = matrix.MultiplyPoint3x4(vtr - pivot) + pivot;
                    vbr = matrix.MultiplyPoint3x4(vbr - pivot) + pivot;
                }
            }

            if (characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.PositionDelta))
            {
                var scaled = TMPAnimationUtility.ScaleVector(characterModifiers.PositionDelta, cData, context);
                vbl += scaled;
                vtl += scaled;
                vtr += scaled;
                vbr += scaled;
            }

            BL_Position = vbl;
            TL_Position = vtl;
            TR_Position = vtr;
            BR_Position = vbr;
        }

        /// <summary>
        /// Get the vertex position. You have to call <see cref="CalculateVertexPositions"/> before this
        /// to populate the values.
        /// </summary>
        /// <param name="i">0: BL, 1: TL, 2: TR, 3: BR</param>
        /// <returns></returns>
        public Vector3 VertexPosition(int i)
        {
            return i switch
            {
                0 => BL_Position,
                1 => TL_Position,
                2 => TR_Position,
                3 => BR_Position,
                _ => throw new IndexOutOfRangeException()
            };
        }
        
        /// <summary>
        /// Get the vertex color. You have to call <see cref="CalculateVertexColors"/> before this
        /// to populate the values.
        /// </summary>
        /// <param name="i">0: BL, 1: TL, 2: TR, 3: BR</param>
        /// <returns></returns>
        public Color32 VertexColor(int i)
        {
            return i switch
            {
                0 => BL_Color,
                1 => TL_Color,
                2 => TR_Color,
                3 => BR_Color,
                _ => throw new IndexOutOfRangeException()
            };
        }

        static Vector3 GetPreciseScale(Matrix4x4 matrix)
        {
            return new Vector3(
                matrix.GetColumn(0).magnitude * Mathf.Sign(matrix.m00),
                matrix.GetColumn(1).magnitude * Mathf.Sign(matrix.m11),
                matrix.GetColumn(2).magnitude * Mathf.Sign(matrix.m22)
            );
        }

        /// <summary>
        /// Linearly interpolate between two <see cref="CharDataModifiers"/>.
        /// The result is stored in <see cref="result"/>, <see cref="start"/> and <see cref="end"/> are not modified.
        /// </summary>
        /// <param name="cData"></param>
        /// <param name="ctx"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t">The </param>
        /// <param name="result">Stores the result.</param>
        public static void LerpUnclamped(CharData cData, IAnimatorDataProvider ctx, CharDataModifiers start,
            CharDataModifiers end, float t,
            CharDataModifiers result)
        {
            LerpCharacterModifiersUnclamped(cData, start.CharacterModifiers, end.CharacterModifiers, t,
                result.CharacterModifiers);
            LerpMeshModifiersUnclamped(cData, ctx, start.MeshModifiers, end.MeshModifiers, t, result.MeshModifiers);
        }

        /// <summary>
        /// Linearly interpolate between a <see cref="CharData"/> and a <see cref="CharDataModifiers"/>.<br/>
        /// This essentially interpolates between "no modifiers" (the <see cref="CharData"/>'s default values, and the <see cref="CharDataModifiers"/>.<br/>
        /// The result is stored in <see cref="result"/>.
        /// </summary>
        /// <param name="cData"></param>
        /// <param name="modifiers"></param>
        /// <param name="t"></param>
        /// <param name="result">Stores the result.</param>
        public static void LerpUnclamped(CharData cData, CharDataModifiers modifiers, float t,
            CharDataModifiers result)
        {
            LerpCharacterModifiersUnclamped(cData, modifiers.CharacterModifiers, t, result.CharacterModifiers);
            LerpMeshModifiersUnclamped(cData, modifiers.MeshModifiers, t, result.MeshModifiers);
        }

        /// <summary>
        /// Linearly interpolate between two <see cref="TMPCharacterModifiers"/>.
        /// The result is stored in <see cref="result"/>, <see cref="start"/> and <see cref="end"/> are not modified.
        /// </summary>
        /// <param name="cData"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <param name="result">Stores the result.</param>
        public static void LerpCharacterModifiersUnclamped(CharData cData, TMPCharacterModifiers start,
            TMPCharacterModifiers end, float t, TMPCharacterModifiers result)
        {
            result.ClearModifiers();

            var combinedFlags = end.Modifier | start.Modifier;

            if (combinedFlags.HasFlag(TMPCharacterModifiers.ModifierFlags.PositionDelta))
            {
                result.PositionDelta =
                    Vector3.LerpUnclamped(start.PositionDelta, end.PositionDelta, t);
            }

            if (combinedFlags.HasFlag(TMPCharacterModifiers.ModifierFlags.Scale))
            {
                Vector3 endScale = GetPreciseScale(end.ScaleDelta);
                Vector3 startScale = GetPreciseScale(start.ScaleDelta);
                Vector3 lerpedScale = Vector3.LerpUnclamped(startScale, endScale, t);

                result.ScaleDelta = Matrix4x4.Scale(lerpedScale);
            }

            if (combinedFlags.HasFlag(TMPCharacterModifiers.ModifierFlags.Rotations))
            {
                try
                {
                    for (int i = 0; i < start.Rotations.Count; i++)
                    {
                        var rot = start.Rotations[i];
                        result.AddRotation(
                            new Rotation(Vector3.LerpUnclamped(rot.eulerAngles, cData.InitialRotation.eulerAngles, t),
                                rot.pivot));
                    }

                    for (int i = 0; i < end.Rotations.Count; i++)
                    {
                        var rot = end.Rotations[i];
                        result.AddRotation(
                            new Rotation(Vector3.LerpUnclamped(cData.InitialRotation.eulerAngles, rot.eulerAngles, t),
                                rot.pivot));
                    }
                }
                catch
                {
                    StackTrace st = new StackTrace();
                    TMPEffectsBugReport.BugReportPrompt("Tried to add to many with " + end.Rotations.Count + ": " +
                                                        st.ToString());
                }
            }
        }

        /// <summary>
        /// Linearly interpolate between a <see cref="CharData"/> and a <see cref="TMPCharacterModifiers"/>.<br/>
        /// This essentially interpolates between "no modifiers" (the <see cref="CharData"/>'s default values, and the <see cref="TMPCharacterModifiers"/>.<br/>
        /// The result is stored in <see cref="result"/>.
        /// </summary>
        /// <param name="cData"></param>
        /// <param name="modifiers"></param>
        /// <param name="t"></param>
        /// <param name="result"></param>
        public static void LerpCharacterModifiersUnclamped(CharData cData, TMPCharacterModifiers modifiers, float t,
            TMPCharacterModifiers result)
        {
            if (modifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.PositionDelta))
            {
                result.PositionDelta =
                    modifiers.PositionDelta * t;
            }
        
            if (modifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Rotations))
            {
                try
                {
                    for (int i = 0; i < modifiers.Rotations.Count; i++)
                    {
                        var rot = modifiers.Rotations[i];
                        result.AddRotation(
                            new Rotation(Vector3.LerpUnclamped(cData.InitialRotation.eulerAngles, rot.eulerAngles, t),
                                rot.pivot));
                    }
                }
                catch (System.Exception exception)
                {
                    TMPEffectsBugReport.BugReportPrompt("Tried to add too many with " + modifiers.Rotations.Count + "; " + result.Rotations.Count + ":\n" + exception);
                } 
            }

            if (modifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Scale))
            {
                // LossyScale is more lossy than I thought, so this is not an option
                // For example: Matrix4x4.Scale(1, -1.5, 1).lossyScale = (-1, 1.5, 0)
                // Vector3 endScale = modifiers.characterModifiers.ScaleDelta.lossyScale;

                // For now; GetPreciseScale. Probably fuggin slow though
                // Alternatively; store list of vector3s that is combined when needed?
                // Could cache that result. Hm

                Vector3 endScale = GetPreciseScale(modifiers.ScaleDelta);
                Vector3 startScale = cData.InitialScale;
                Vector3 lerpedScale = Vector3.LerpUnclamped(startScale, endScale, t);

                result.ScaleDelta = Matrix4x4.Scale(lerpedScale);
            }
        }

        /// <summary>
        /// Linearly interpolate between two <see cref="TMPMeshModifiers"/>.
        /// The result is stored in <see cref="result"/>, <see cref="start"/> and <see cref="end"/> are not modified.
        /// </summary>
        /// <param name="cData"></param>
        /// <param name="ctx"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <param name="result">Stores the result.</param>
        public static void LerpMeshModifiersUnclamped(CharData cData, IAnimatorDataProvider ctx, TMPMeshModifiers start,
            TMPMeshModifiers end,
            float t,
            TMPMeshModifiers result)
        {
            result.ClearModifiers();

            var combinedFlags = start.Modifier | end.Modifier;

            if (combinedFlags.HasFlag(TMPMeshModifiers.ModifierFlags.Deltas))
            {
                result.BL_Delta = Vector3.LerpUnclamped(start.BL_Delta, end.BL_Delta, t);
                result.TL_Delta = Vector3.LerpUnclamped(start.TL_Delta, end.TL_Delta, t);
                result.TR_Delta = Vector3.LerpUnclamped(start.TR_Delta, end.TR_Delta, t);
                result.BR_Delta = Vector3.LerpUnclamped(start.BR_Delta, end.BR_Delta, t);
            }

            if (end.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Colors))
            {
                if (start.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Colors))
                {
                    if (cData.info.index == 2) Debug.Log("option 0; end: " + end.Modifier + " start: " + start.Modifier);
                    result.BL_Color = ColorOverride.LerpUnclamped(start.BL_Color, end.BL_Color, t);
                    result.TL_Color = ColorOverride.LerpUnclamped(start.TL_Color, end.TL_Color, t);
                    result.TR_Color = ColorOverride.LerpUnclamped(start.TR_Color, end.TR_Color, t);
                    result.BR_Color = ColorOverride.LerpUnclamped(start.BR_Color, end.BR_Color, t);
                }
                else if (cData.MeshModifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Colors))
                {
                    if (cData.info.index == 2) Debug.Log("option 1");
                    result.BL_Color = ColorOverride.LerpUnclamped(cData.MeshModifiers.BL_Color, end.BL_Color, t);
                    result.TL_Color = ColorOverride.LerpUnclamped(cData.MeshModifiers.TL_Color, end.TL_Color, t);
                    result.TR_Color = ColorOverride.LerpUnclamped(cData.MeshModifiers.TR_Color, end.TR_Color, t);
                    result.BR_Color = ColorOverride.LerpUnclamped(cData.MeshModifiers.BR_Color, end.BR_Color, t);
                }
                else if (ctx.Modifiers.MeshModifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Colors))
                {
                    if (cData.info.index == 2) Debug.Log("option 2");
                    result.BL_Color = ColorOverride.LerpUnclamped(ctx.Modifiers.MeshModifiers.BL_Color, end.BL_Color, t);
                    result.TL_Color = ColorOverride.LerpUnclamped(ctx.Modifiers.MeshModifiers.TL_Color, end.TL_Color, t);
                    result.TR_Color = ColorOverride.LerpUnclamped(ctx.Modifiers.MeshModifiers.TR_Color, end.TR_Color, t);
                    result.BR_Color = ColorOverride.LerpUnclamped(ctx.Modifiers.MeshModifiers.BR_Color, end.BR_Color, t);
                }
                else
                {
                    if (cData.info.index == 2) Debug.Log("option 3");
                    result.BL_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.BL_Color, end.BL_Color, t);
                    result.TL_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.TL_Color, end.TL_Color, t);
                    result.TR_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.TR_Color, end.TR_Color, t);
                    result.BR_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.BR_Color, end.BR_Color, t);
                }
            }

            if (combinedFlags.HasFlag(TMPMeshModifiers.ModifierFlags.UVs))
            {
                var startVector = start.BL_UV0.GetValue(cData.InitialMesh.BL_UV0);
                var endVector = end.BL_UV0.GetValue(cData.InitialMesh.BL_UV0);
                result.BL_UV0 =
                    new Vector3Override(Vector3.LerpUnclamped(startVector, endVector, t));

                startVector = start.TL_UV0.GetValue(cData.InitialMesh.TL_UV0);
                endVector = end.TL_UV0.GetValue(cData.InitialMesh.TL_UV0);
                result.TL_UV0 =
                    new Vector3Override(Vector3.LerpUnclamped(startVector, endVector, t));

                startVector = start.TR_UV0.GetValue(cData.InitialMesh.TR_UV0);
                endVector = end.TR_UV0.GetValue(cData.InitialMesh.TR_UV0);
                result.TR_UV0 =
                    new Vector3Override(Vector3.LerpUnclamped(startVector, endVector, t));

                startVector = start.BR_UV0.GetValue(cData.InitialMesh.BR_UV0);
                endVector = end.BR_UV0.GetValue(cData.InitialMesh.BR_UV0);
                result.BR_UV0 =
                    new Vector3Override(Vector3.LerpUnclamped(startVector, endVector, t));
            }
        }

        /// <summary>
        /// Linearly interpolate between a <see cref="CharData"/> and a <see cref="TMPMeshModifiers"/>.<br/>
        /// This essentially interpolates between "no modifiers" (the <see cref="CharData"/>'s default values, and the <see cref="TMPMeshModifiers"/>.<br/>
        /// The result is stored in <see cref="result"/>.
        /// </summary>
        /// <param name="cData"></param>
        /// <param name="modifiers"></param>
        /// <param name="t"></param>
        /// <param name="result">Stores the result.</param>
        public static void LerpMeshModifiersUnclamped(CharData cData, TMPMeshModifiers modifiers, float t,
            TMPMeshModifiers result)
        {
            // result.ClearModifiers();

            if (modifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Deltas))
            {
                result.BL_Delta = modifiers.BL_Delta * t;
                result.TL_Delta = modifiers.TL_Delta * t;
                result.TR_Delta = modifiers.TR_Delta * t;
                result.BR_Delta = modifiers.BR_Delta * t;
            }

            if (modifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Colors))
            {
                result.BL_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.BL_Color, modifiers.BL_Color, t);
                result.TL_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.TL_Color, modifiers.TL_Color, t);
                result.TR_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.TR_Color, modifiers.TR_Color, t);
                result.BR_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.BR_Color, modifiers.BR_Color, t);
            }

            if (modifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.UVs))
            {
                var vector = modifiers.BL_UV0.GetValue(cData.InitialMesh.BL_UV0);
                result.BL_UV0 =
                    new Vector3Override(Vector3.LerpUnclamped(cData.InitialMesh.BL_UV0, vector, t));

                vector = modifiers.TL_UV0.GetValue(cData.InitialMesh.TL_UV0);
                result.TL_UV0 =
                    new Vector3Override(Vector3.LerpUnclamped(cData.InitialMesh.TL_UV0, vector, t));

                vector = modifiers.TR_UV0.GetValue(cData.InitialMesh.TR_UV0);
                result.TR_UV0 =
                    new Vector3Override(Vector3.LerpUnclamped(cData.InitialMesh.TR_UV0, vector, t));

                vector = modifiers.BR_UV0.GetValue(cData.InitialMesh.BR_UV0);
                result.BR_UV0 =
                    new Vector3Override(Vector3.LerpUnclamped(cData.InitialMesh.BR_UV0, vector, t));
            }
        }

        /// <summary>
        /// Reset the modifiers.
        /// </summary>
        public void Reset()
        {
            meshModifiers.ClearModifiers();
            characterModifiers.ClearModifiers();
        }
    }
}