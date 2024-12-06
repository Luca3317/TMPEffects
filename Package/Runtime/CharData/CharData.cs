using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine;
using TMPro;

namespace TMPEffects.CharacterData
{
    public class ReadOnlyCharData
    {
        private CharData cData;

        public Vector3 Position => cData.Position;
        public Vector3 PositionDelta => cData.PositionDelta;

        public Vector3 Scale => cData.Scale;
        public IReadOnlyList<Rotation> Rotation => cData.Rotations;

        public CharData.Info info => cData.info;

        public ReadOnlyVertexData InitialMesh => cData.InitialMesh;

        public Vector3 InitialPosition => cData.InitialPosition;

        public Quaternion InitialRotation => cData.InitialRotation;

        public Vector3 InitialScale => cData.InitialScale;

        public ReadOnlyCharData(CharData cData)
        {
            this.cData = cData;
        }
    }

    public partial class CharData
    {
        public TMPCharacterModifiers CharacterModifiers => characterModifiers;
        public TMPMeshModifiers MeshModifiers => mesh.Modifiers;

        private TMPCharacterModifiers characterModifiers;

        /// <summary>
        /// Whether the position has been manipulated from the character's initial position.
        /// </summary>
        public bool positionDirty => characterModifiers.PositionDelta != Vector3.zero;

        /// <summary>
        /// Whether the rotation has been manipulated from the character's initial rotation.
        /// </summary>
        public bool rotationDirty => characterModifiers.Rotations.Any();

        /// <summary>
        /// Whether the scale has been manipulated from the character's initial scale.
        /// </summary>
        public bool scaleDirty => characterModifiers.ScaleDelta != Matrix4x4.identity;

        /// <summary>
        /// Whether the vertices have been manipulated.
        /// </summary>
        public bool verticesDirty => mesh.positionsDirty;

        /// <summary>
        /// Whether the vertex colors have been manipulated.
        /// </summary>
        public bool colorsDirty => mesh.colorsDirty;

        /// <summary>
        /// Whether the vertex alphas have been manipulated.
        /// </summary>
        public bool alphasDirty => mesh.alphasDirty;

        /// <summary>
        /// Whether the UVs have been manipulated.
        /// </summary>
        public bool uvsDirty => mesh.uvsDirty;

        public Vector3 Position
        {
            get => characterModifiers.PositionDelta + InitialPosition;
            set => characterModifiers.PositionDelta = value - InitialPosition;
        }

        /// <summary>
        /// The character's position.
        /// </summary>
        public Vector3 PositionDelta
        {
            get => characterModifiers.PositionDelta;
            set => characterModifiers.PositionDelta = value;
        }

        /// <summary>
        /// The character's scale.
        /// </summary>
        public Vector3 Scale
        {
            get => characterModifiers.ScaleDelta.lossyScale;
            set => characterModifiers.ScaleDelta = Matrix4x4.Scale(value);
        }

        /// <summary>
        /// The character's rotations.
        /// </summary>
        public IReadOnlyList<Rotation> Rotations => characterModifiers.Rotations;

        #region Fields

        /// <summary>
        /// The default scale of any CharData.
        /// </summary>
        public static readonly Vector3 defaultScale = new Vector3(1, 1, 1);

        /// <summary>
        /// The default rotation of any CharData.
        /// </summary>
        public static readonly Quaternion defaultRotation = Quaternion.identity;

        /// <summary>
        /// Holds a selection of <see cref="TMP_CharacterInfo"/> data.
        /// </summary>
        public readonly Info info;

        /// <summary>
        /// The mesh of the character.
        /// </summary>
        public readonly VertexData mesh;

        /// <summary>
        /// The initial position of this character.
        /// </summary>
        public Vector3 InitialPosition => info.InitialPosition;

        /// <summary>
        /// The initial rotation of this character.
        /// </summary>
        public Quaternion InitialRotation => info.InitialRotation;

        /// <summary>
        /// The initial scale of this character.
        /// </summary>
        public Vector3 InitialScale => info.InitialScale;
        
        /// <summary>
        /// The initial mesh of this character.
        /// </summary>
        public ReadOnlyVertexData InitialMesh => info.initialMesh;

        #endregion

        public CharData(int index, TMP_CharacterInfo cInfo, int wordIndex)
        {
            info = new CharData.Info(index, cInfo, wordIndex);

            mesh = new VertexData(cInfo);
            characterModifiers = new TMPCharacterModifiers();
        }

        public CharData(int index, TMP_CharacterInfo cInfo, int wordIndex, TMP_WordInfo? wInfo = null)
        {
            info = wInfo == null
                ? new CharData.Info(index, cInfo, wordIndex)
                : new CharData.Info(index, cInfo, wordIndex, wInfo.Value);

            mesh = new VertexData(cInfo);
            characterModifiers = new TMPCharacterModifiers();
        }

        #region Position

        /// <summary>
        /// Set the position of the character.
        /// </summary>
        /// <param name="position">The new position of the character.</param>
        public void SetPosition(Vector3 position)
        {
            characterModifiers.PositionDelta = position - InitialPosition;
        }

        // public void ClearPosition()
        // {
        //     CharacterModifiers.ClearModifierFlags(TMPCharacterModifiers.ModifierFlags.Position);
        // }

        #endregion

        #region PositionDelta

        public void SetPositionDelta(Vector3 delta)
        {
            characterModifiers.PositionDelta = delta;
        }

        public void ClearPosition()
        {
            characterModifiers.ClearModifiers(TMPCharacterModifiers.ModifierFlags.PositionDelta);
        }

        #endregion

        #region Rotations

        public void AddRotation(Vector3 eulerAngles, Vector3 pivot)
        {
            characterModifiers.AddRotation(new Rotation(eulerAngles, pivot));
        }

        public void RemoveRotation(int index)
        {
            characterModifiers.RemoveRotation(index);
        }

        public void InsertRotation(int index, Vector3 eulerAngles, Vector3 pivot)
        {
            characterModifiers.InsertRotation(index, new Rotation(eulerAngles, pivot));
        }

        public void ClearRotations()
        {
            characterModifiers.ClearModifiers(TMPCharacterModifiers.ModifierFlags.Rotations);
        }

        #endregion

        #region Scale

        /// <summary>
        /// Set the scale of this character. 
        /// </summary>
        /// <param name="scale">The new scale of this character.</param>
        public void SetScale(Vector3 scale)
        {
            characterModifiers.ScaleDelta = Matrix4x4.Scale(scale);
        }

        public void ClearScale()
        {
            characterModifiers.ClearModifiers(TMPCharacterModifiers.ModifierFlags.Scale);
        }

        #endregion

        /// <summary>
        /// Reset changes made to the character's mesh, position, rotation, scale and pivot.
        /// </summary>
        public void Reset()
        {
            characterModifiers.ClearModifiers();
            MeshModifiers.ClearModifiers();
        }
    }
}