using System.Collections.Generic;
using System.Linq;
using TMPEffects.Components;
using TMPEffects.Modifiers;
using UnityEngine;
using TMPro;

namespace TMPEffects.CharacterData
{
    /// <summary>
    /// A read-only wrapper around <see cref="CharData"/>.
    /// </summary>
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

    /// <summary>
    /// Represents a TextMeshPro character, allowing you to make modifications to its appearance.<br/>
    /// Primarily used by <see cref="TMPAnimator"/> and its animations to animate a character.<br/>
    /// When used in that manner, modifications made to it (e.g. by using <see cref="SetPosition"/>, or directly modifying the <see cref="CharacterModifiers"/>),
    /// will be applied by the animator to the actual mesh of the character.
    /// </summary>
    public partial class CharData
    {
        /// <summary>
        /// The character modifiers of this character.
        /// </summary>
        public TMPCharacterModifiers CharacterModifiers => characterModifiers;

        /// <summary>
        /// The mesh modifiers of this character.
        /// </summary>
        public TMPMeshModifiers MeshModifiers => mesh.Modifiers;

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

        /// <summary>
        /// The character's position.
        /// </summary>
        public Vector3 Position
        {
            get => characterModifiers.PositionDelta + InitialPosition;
            set => characterModifiers.PositionDelta = value - InitialPosition;
        }

        /// <summary>
        /// The character's position delta (offset from <see cref="InitialPosition"/>).
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
        /// The character's rotations.<br/>
        /// Modify using <see cref="AddRotation"/>, <see cref="RemoveRotation"/>, <see cref="ClearRotations"/>.
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
        
        private TMPCharacterModifiers characterModifiers;
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
        /// <param name="position"></param>
        public void SetPosition(Vector3 position)
        {
            characterModifiers.PositionDelta = position - InitialPosition;
        }

        #endregion

        #region PositionDelta

        /// <summary>
        /// Set the position delta of the character (offset from <see cref="InitialPosition"/>).
        /// </summary>
        /// <param name="delta"></param>
        public void SetPositionDelta(Vector3 delta)
        {
            characterModifiers.PositionDelta = delta;
        }

        /// <summary>
        /// Reset the position of the character back to <see cref="InitialPosition"/>.
        /// </summary>
        public void ClearPosition()
        {
            characterModifiers.ClearModifiers(TMPCharacterModifiers.ModifierFlags.PositionDelta);
        }

        #endregion

        #region Rotations

        /// <summary>
        /// Add a rotation to the character.
        /// </summary>
        /// <param name="eulerAngles">The euler angles to rotate by.</param>
        /// <param name="pivot">The pivot to rotate around.</param>
        public void AddRotation(Vector3 eulerAngles, Vector3 pivot)
        {
            characterModifiers.AddRotation(new Rotation(eulerAngles, pivot));
        }

        /// <summary>
        /// Remove a rotation from the character.
        /// </summary>
        /// <param name="index">The index of the rotation in <see cref="Rotations"/>.</param>
        public void RemoveRotation(int index)
        {
            characterModifiers.RemoveRotation(index);
        }

        /// <summary>
        /// Add a rotation to the character.
        /// </summary>
        /// <param name="index">The index in <see cref="Rotations"/> to insert into.</param>
        /// <param name="eulerAngles">The euler angles to rotate by.</param>
        /// <param name="pivot">The pivot to rotate around.</param>
        public void InsertRotation(int index, Vector3 eulerAngles, Vector3 pivot)
        {
            characterModifiers.InsertRotation(index, new Rotation(eulerAngles, pivot));
        }

        /// <summary>
        /// Clear all rotations of this character, effectively setting its rotation back to <see cref="InitialRotation"/>.
        /// </summary>
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

        /// <summary>
        /// Reset the scale of this character back to <see cref="InitialScale"/>.
        /// </summary>
        public void ClearScale()
        {
            characterModifiers.ClearModifiers(TMPCharacterModifiers.ModifierFlags.Scale);
        }

        #endregion

        /// <summary>
        /// Reset all changes made to the character (mesh, position, rotation, scale and pivot).
        /// </summary>
        public void Reset()
        {
            characterModifiers.ClearModifiers();
            MeshModifiers.ClearModifiers();
        }
    }
}