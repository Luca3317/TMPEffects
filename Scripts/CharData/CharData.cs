using UnityEngine;
using TMPro;
using TMPEffects.Components.Mediator;

namespace TMPEffects.CharacterData
{
    /// <summary>
    /// Holds information about a character.<br/>
    /// In addition to holding a selection of data supplied by the respective
    /// <see cref="TMP_CharacterInfo"/>, accessible through the <see cref="info"/> field,
    /// also holds <see cref="TMPEffects"/> specific data and methods to manipulate said data.
    /// </summary>
    public class CharData
    {
        /// <summary>
        /// Whether the position has been manipulated from the character's initial position.
        /// </summary>
        public bool positionDirty => position != initialPosition;
        /// <summary>
        /// Whether the rotation has been manipulated from the character's initial rotation.
        /// </summary>
        public bool rotationDirty => rotation != defaultRotation;
        /// <summary>
        /// Whether the scale has been manipulated from the character's initial scale.
        /// </summary>
        public bool scaleDirty => scale != defaultScale;
        /// <summary>
        /// Whether the vertices have been manipulated.
        /// </summary>
        public bool verticesDirty => mesh.verticesDirty;
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
        public Vector3 Position => position;
        /// <summary>
        /// The character's original position.
        /// </summary>
        public Vector3 OriginalPosition => initialPosition;
        /// <summary>
        /// The character's scale.
        /// </summary>
        public Vector3 Scale => scale;
        /// <summary>
        /// The character's rotation.
        /// </summary>
        public Quaternion Rotation => rotation;
        /// <summary>
        /// The character's rotation pivot.
        /// </summary>
        public Vector3 RotationPivot => pivot;

        #region Fields
        /// <summary>
        /// Holds a selection of <see cref="TMP_CharacterInfo"/> data.
        /// </summary>
        public readonly Info info;
        /// <summary>
        /// The mesh of the character.
        /// </summary>
        // TODO Should this not be readonly? making it readonly seems to break all animations, somehow
        public VertexData mesh;

        public readonly ReadOnlyVertexData initialMesh;

        private Vector3 position;
        private Vector3 scale;
        private Quaternion rotation;
        private Vector3 pivot;

        // TODO Keep these?

        /// <summary>
        /// The default scale.
        /// </summary>
        public static readonly Vector3 defaultScale = new Vector3(1, 1, 1);
        public static readonly Quaternion defaultRotation = Quaternion.identity;

        /// <summary>
        /// The initial position of this character.
        /// </summary>
        public readonly Vector3 initialPosition;
        /// <summary>
        /// The initial rotation of this character.
        /// </summary>
        public readonly Quaternion originalRotation;
        #endregion

        public CharData(int index, TMP_CharacterInfo cInfo)
        {
            VertexData vData = new VertexData(cInfo);
            mesh = vData;
            info = new Info(index, cInfo, mesh);

            initialPosition = default;
            originalRotation = Quaternion.identity;
            initialPosition = GetCenter(in mesh.initial);

            position = initialPosition;
            rotation = defaultRotation;
            scale = defaultScale;
            pivot = initialPosition;
            //this.mediator = mediator;
        }
        public CharData(int index, TMP_CharacterInfo cInfo, TMP_WordInfo? wInfo = null)
        {
            VertexData vData = new VertexData(cInfo);
            mesh = vData;
            info = wInfo == null ? new Info(index, cInfo, mesh) : new Info(index, cInfo, wInfo.Value, mesh);

            initialPosition = default;
            originalRotation = Quaternion.identity;
            initialPosition = GetCenter(in mesh.initial);

            position = initialPosition;
            rotation = defaultRotation;
            scale = defaultScale;
            pivot = initialPosition;
        }


        /// <summary>
        /// Set the position of the vertex at the given index.
        /// </summary>
        /// <param name="index">The index of the vertex.</param>
        /// <param name="position">The new position of the vertex.</param>
        public void SetVertex(int index, Vector3 position)
        {
            mesh.SetVertex(index, position);
        }

        /// <summary>
        /// Add a positon delta to the vertex at the given index.
        /// </summary>
        /// <param name="index">The index of the vertex.</param>
        /// <param name="delta">The delta to add to the position of the vertex.</param> 
        public void AddVertexDelta(int index, Vector3 delta)
        {
            mesh.SetVertex(index, mesh.GetVertex(index) + delta);
        }

        /// <summary>
        /// Set the position of the character.
        /// </summary>
        /// <param name="position">The new position of the character.</param>
        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        /// <summary>
        /// Add a delta to the position of the character.
        /// </summary>
        /// <param name="delta">The delta to add to the position of the character.</param>
        public void AddPositionDelta(Vector3 delta)
        {
            position += delta;
        }

        /// <summary>
        /// Set the pivot of this character.<br/>
        /// Note that the pivot is independent of the character's position, scale and rotation.
        /// </summary>
        /// <param name="pivot">The new position of the pivot.</param>
        public void SetPivot(Vector3 pivot)
        {
            this.pivot = pivot;
        }
        /// <summary>
        /// Add a delta to the pivot of the character.<br/>
        /// Note that the pivot is independent of the character's position, rotation and scale.
        /// </summary>
        /// <param name="delta">The delta to add to the position of the pivot.</param>
        public void AddPivotDelta(Vector3 delta)
        {
            pivot += delta;
        }

        /// <summary>
        /// Set the rotation of this character.
        /// </summary>
        /// <param name="rotation">The new rotation of this character.</param>
        public void SetRotation(Quaternion rotation)
        {
            this.rotation = rotation;
            //this.pivot = initialPosition;
        }

        /// <summary>
        /// Set the scale of this character. 
        /// </summary>
        /// <param name="scale">The new scale of this character.</param>
        public void SetScale(Vector3 scale)
        {
            this.scale = scale;
        }

        /// <summary>
        /// Reset changes made to the character's mesh, position, rotation, scale and pivot.
        /// </summary>
        public void Reset()
        {
            ResetPosition();
            ResetRotation();
            ResetScale();
            ResetPivot();
            mesh.Reset();
        }

        /// <summary>
        /// Reset the character's scale.
        /// </summary>
        public void ResetScale() => this.scale = defaultScale;
        /// <summary>
        /// Reset the character's position.
        /// </summary>
        public void ResetPosition() => this.position = initialPosition;
        /// <summary>
        /// Reset the character's rotation.
        /// </summary>
        public void ResetRotation() => this.rotation = defaultRotation;
        /// <summary>
        /// Reset the character's pivot.
        /// </summary>
        public void ResetPivot() => this.pivot = initialPosition;
        /// <summary>
        /// Reset the character's vertices.
        /// </summary>
        public void ResetVertices() => mesh.ResetVertices();
        /// <summary>
        /// Reset the character's UVs.
        /// </summary>
        public void ResetUVs() => mesh.ResetUVs();
        /// <summary>
        /// Reset the character's vertex colors.
        /// </summary>
        public void ResetColors() => mesh.ResetColors();


        private Vector3 GetCenter(in ReadOnlyVertexData data)
        {
            Vector3 center = Vector3.zero;
            for (int i = 0; i < 4; i++)
            {
                center += data.GetVertex(i);
            }
            return center / 4;
        }
    }
}