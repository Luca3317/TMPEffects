using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.CharacterData
{
    /// <summary>
    /// Exposes vertex state of a <see cref="CharData"/> to allow iterative modifications.<br/>
    /// Used in <see cref="CharDataState"/> and <see cref="ReadOnlyCharDataState"/>.
    /// </summary>
    public interface ICharDataState
    {
        /// <summary>
        /// The accumulated position delta of all modifications.
        /// </summary>
        public Vector3 positionDelta { get; }
        /// <summary>
        /// The accumulated scale delta of all modifications.
        /// </summary>
        public Matrix4x4 scaleDelta { get; }
        /// <summary>
        /// All rotations and the pivot they will be applied around.
        /// </summary>
        public IEnumerable<ValueTuple<Quaternion, Vector3>> Rotations { get; }

        /// <summary>
        /// The position of the topleft vertex.
        /// </summary>
        public Vector3 TL { get; }
        /// <summary>
        /// The position of the topright vertex.
        /// </summary>
        public Vector3 TR { get; }
        /// <summary>
        /// The position of the bottomright vertex.
        /// </summary>
        public Vector3 BR { get; }
        /// <summary>
        /// The position of the bottomleft vertex.
        /// </summary>
        public Vector3 BL { get; }

        /// <summary>
        /// The maximum position modification of the topleft vertex.<br/>
        /// This is used in calculating the scaling of all vertex position modifications in <see cref="CalculateVertexPositions"/>; you can most likely ignore this.
        /// </summary>
        public Vector3 TLMax { get; }
        /// <summary>
        /// The maximum position modification of the topright vertex.<br/>
        /// This is used in calculating the scaling of all vertex position modifications in <see cref="CalculateVertexPositions"/>; you can most likely ignore this.
        /// </summary>
        public Vector3 TRMax { get; }
        /// <summary>
        /// The maximum position modification of the bottomright vertex.<br/>
        /// This is used in calculating the scaling of all vertex position modifications in <see cref="CalculateVertexPositions"/>; you can most likely ignore this.
        /// </summary>
        public Vector3 BRMax { get; }
        /// <summary>
        /// The maximum position modification of the bottomleft vertex.<br/>
        /// This is used in calculating the scaling of all vertex position modifications in <see cref="CalculateVertexPositions"/>; you can most likely ignore this.
        /// </summary>
        public Vector3 BLMax { get; }

        /// <summary>
        /// The minimum position modification of the topleft vertex.<br/>
        /// This is used in calculating the scaling of all vertex position modifications in <see cref="CalculateVertexPositions"/>; you can most likely ignore this.
        /// </summary>
        public Vector3 TLMin { get; }
        /// <summary>
        /// The minimum position modification of the topright vertex.<br/>
        /// This is used in calculating the scaling of all vertex position modifications in <see cref="CalculateVertexPositions"/>; you can most likely ignore this.
        /// </summary>
        public Vector3 TRMin { get; }
        /// <summary>
        /// The minimum position modification of the bottomleft vertex.<br/>
        /// This is used in calculating the scaling of all vertex position modifications in <see cref="CalculateVertexPositions"/>; you can most likely ignore this.
        /// </summary>
        public Vector3 BRMin { get; }
        /// <summary>
        /// The minimum position modification of the bottomleft vertex.<br/>
        /// This is used in calculating the scaling of all vertex position modifications in <see cref="CalculateVertexPositions"/>; you can most likely ignore this.
        /// </summary>
        public Vector3 BLMin { get; }

        /// <summary>
        /// The UV value of the topleft vertex.
        /// </summary>
        public Vector2 TL_UV { get; }
        /// <summary>
        /// The UV value of the topright vertex.
        /// </summary>
        public Vector2 TR_UV { get; }
        /// <summary>
        /// The UV value of the bottomright vertex.
        /// </summary>
        public Vector2 BR_UV { get; }
        /// <summary>
        /// The UV value of the bottomleft vertex.
        /// </summary>
        public Vector2 BL_UV { get; }

        /// <summary>
        /// The UV2 value of the topleft vertex.
        /// </summary>
        public Vector2 TL_UV2 { get; }
        /// <summary>
        /// The UV2 value of the topright vertex.
        /// </summary>
        public Vector2 TR_UV2 { get; }
        /// <summary>
        /// The UV2 value of the bottomright vertex.
        /// </summary>
        public Vector2 BR_UV2 { get; }
        /// <summary>
        /// The UV2 value of the bottomleft vertex.
        /// </summary>
        public Vector2 BL_UV2 { get; }

        /// <summary>
        /// The color value of the topleft vertex.
        /// </summary>
        public Color32 TL_Color { get; }
        /// <summary>
        /// The color value of the topright vertex.
        /// </summary>
        public Color32 TR_Color { get; }
        /// <summary>
        /// The color value of the bottomright vertex.
        /// </summary>
        public Color32 BR_Color { get; }
        /// <summary>
        /// The color value of the bottomleft vertex.
        /// </summary>
        public Color32 BL_Color { get; }

        /// <summary>
        /// Contains the calculated position of the topleft vertex after calling <see cref="CalculateVertexPositions"/>.
        /// </summary>
        public Vector3 TL_Result { get; }
        /// <summary>
        /// Contains the calculated position of the topright vertex after calling <see cref="CalculateVertexPositions"/>.
        /// </summary>
        public Vector3 TR_Result { get; }
        /// <summary>
        /// Contains the calculated position of the bottomright vertex after calling <see cref="CalculateVertexPositions"/>.
        /// </summary>
        public Vector3 BR_Result { get; }
        /// <summary>
        /// Contains the calculated position of the bottomleft vertex after calling <see cref="CalculateVertexPositions"/>.
        /// </summary>
        public Vector3 BL_Result { get; }

        /// <summary>
        /// Calculate the vertex positions, applying all rotations, transformations and scale operations.<br/>
        /// Will set <see cref="BL_Result"/>, <see cref="TL_Result"/>, <see cref="TR_Result"/> and <see cref="BR_Result"/>.
        /// </summary>
        public void CalculateVertexPositions();
    }
}