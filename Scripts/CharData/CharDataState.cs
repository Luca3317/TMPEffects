
using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.CharacterData
{
    public class CharDataState
    {
        public CharData cData;
        public AnimatorContext context;

        public Vector3 positionDelta;
        public Matrix4x4 scaleDelta;
        //public Quaternion rotation;
        //public Vector3 rotationPivot;

        public Vector3 TL;
        public Vector3 TR;
        public Vector3 BR;
        public Vector3 BL;

        public Vector3 TLMax;
        public Vector3 TRMax;
        public Vector3 BRMax;
        public Vector3 BLMax;

        public Vector3 TLMin;
        public Vector3 TRMin;
        public Vector3 BRMin;
        public Vector3 BLMin;

        public Vector2 TL_UV;
        public Vector2 TR_UV;
        public Vector2 BR_UV;
        public Vector2 BL_UV;

        public Vector2 TL_UV2;
        public Vector2 TR_UV2;
        public Vector2 BR_UV2;
        public Vector2 BL_UV2;

        public Color32 TL_Color;
        public Color32 TR_Color;
        public Color32 BR_Color;
        public Color32 BL_Color;

        public Vector3 TL_Result;
        public Vector3 TR_Result;
        public Vector3 BR_Result;
        public Vector3 BL_Result;

        List<Vector3> pivots = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();

        public void Reset(AnimatorContext context, CharData cData)
        {
            this.cData = cData;
            this.context = context;

            positionDelta = Vector3.zero;
            scaleDelta = Matrix4x4.Scale(Vector3.one);
            //rotation = Quaternion.identity;
            //rotationPivot = cData.initialPosition;
            rotations.Clear();
            pivots.Clear();

            TL = Vector3.zero;
            TR = Vector3.zero;
            BR = Vector3.zero;
            BL = Vector3.zero;

            TLMax = cData.mesh.initial.TL_Position;
            TRMax = cData.mesh.initial.TR_Position;
            BRMax = cData.mesh.initial.BR_Position;
            BLMax = cData.mesh.initial.BL_Position;

            TLMin = cData.mesh.initial.TL_Position;
            TRMin = cData.mesh.initial.TR_Position;
            BRMin = cData.mesh.initial.BR_Position;
            BLMin = cData.mesh.initial.BL_Position;

            TL_UV = cData.mesh.initial.TL_UV0;
            TR_UV = cData.mesh.initial.TR_UV0;
            BR_UV = cData.mesh.initial.BR_UV0;
            BL_UV = cData.mesh.initial.BL_UV0;

            TL_UV2 = cData.mesh.initial.TL_UV2;
            TR_UV2 = cData.mesh.initial.TR_UV2;
            BR_UV2 = cData.mesh.initial.BR_UV2;
            BL_UV2 = cData.mesh.initial.BL_UV2;

            TL_Color = cData.mesh.initial.TL_Color;
            TR_Color = cData.mesh.initial.TR_Color;
            BR_Color = cData.mesh.initial.BR_Color;
            BL_Color = cData.mesh.initial.BL_Color;
        }

        public void CalculateVertexPositions()
        {
            // Apply vertex transformations
            Vector3 vtl = cData.initialMesh.TL_Position + TL;
            Vector3 vtr = cData.initialMesh.TR_Position + TR;
            Vector3 vbr = cData.initialMesh.BR_Position + BR;
            Vector3 vbl = cData.initialMesh.BL_Position + BL;

            // For now only the vertex offsets are clamped to min/max of each individual animation, as otherwise stacked animations are likely to deform the character
            vtl = new Vector3(Mathf.Clamp(vtl.x, TLMin.x, TLMax.x), Mathf.Clamp(vtl.y, TLMin.y, TLMax.y), Mathf.Clamp(vtl.z, TLMin.z, TLMax.z));
            vtr = new Vector3(Mathf.Clamp(vtr.x, TRMin.x, TRMax.x), Mathf.Clamp(vtr.y, TRMin.y, TRMax.y), Mathf.Clamp(vtr.z, TRMin.z, TRMax.z));
            vbr = new Vector3(Mathf.Clamp(vbr.x, BRMin.x, BRMax.x), Mathf.Clamp(vbr.y, BRMin.y, BRMax.y), Mathf.Clamp(vbr.z, BRMin.z, BRMax.z));
            vbl = new Vector3(Mathf.Clamp(vbl.x, BLMin.x, BLMax.x), Mathf.Clamp(vbl.y, BLMin.y, BLMax.y), Mathf.Clamp(vbl.z, BLMin.z, BLMax.z));

            // Apply scale
            vtl = scaleDelta.MultiplyPoint3x4(vtl - cData.InitialPosition) + cData.InitialPosition;
            vtr = scaleDelta.MultiplyPoint3x4(vtr - cData.InitialPosition) + cData.InitialPosition;
            vbr = scaleDelta.MultiplyPoint3x4(vbr - cData.InitialPosition) + cData.InitialPosition;
            vbl = scaleDelta.MultiplyPoint3x4(vbl - cData.InitialPosition) + cData.InitialPosition;

            // Apply rotation
            Vector3 pivot;
            Matrix4x4 matrix;
            for (int i = 0; i < rotations.Count; i++)
            {
                pivot = pivots[i];
                matrix = Matrix4x4.Rotate(rotations[i]);

                vtl = matrix.MultiplyPoint3x4(vtl - pivot) + pivot;
                vtr = matrix.MultiplyPoint3x4(vtr - pivot) + pivot;
                vbr = matrix.MultiplyPoint3x4(vbr - pivot) + pivot;
                vbl = matrix.MultiplyPoint3x4(vbl - pivot) + pivot;
            }

            // Apply transformation
            var scaled = AnimationUtility.ScaleVector(positionDelta, cData, context);
            vtl += scaled;
            vtr += scaled;
            vbr += scaled;
            vbl += scaled;

            BL_Result = vbl;
            TL_Result = vtl;
            TR_Result = vtr;
            BR_Result = vbr;
        }

        public void UpdateVertexOffsets()
        {
            if (cData.positionDirty)
            {
                positionDelta += (cData.Position - cData.InitialPosition);
            }

            if (cData.scaleDirty)
            {
                scaleDelta *= Matrix4x4.Scale(cData.Scale);
            }

            if (cData.rotationDirty)
            {
                if (cData.Rotation != Quaternion.identity || cData.Rotation.eulerAngles == Vector3.zero)
                {
                    rotations.Add(cData.Rotation);
                    var scaled = cData.InitialPosition + AnimationUtility.ScaleVector((cData.RotationPivot - cData.InitialPosition), cData, context);
                    pivots.Add(scaled);
                }
            }

            if (cData.verticesDirty)
            {
                Vector3 deltaTL;
                Vector3 deltaTR;
                Vector3 deltaBR;
                Vector3 deltaBL;

                deltaTL = AnimationUtility.ScaleVector(cData.mesh.TL_Position - cData.mesh.initial.TL_Position, cData, context);
                deltaTR = AnimationUtility.ScaleVector(cData.mesh.TR_Position - cData.mesh.initial.TR_Position, cData, context);
                deltaBR = AnimationUtility.ScaleVector(cData.mesh.BR_Position - cData.mesh.initial.BR_Position, cData, context);
                deltaBL = AnimationUtility.ScaleVector(cData.mesh.BL_Position - cData.mesh.initial.BL_Position, cData, context);

                TL += deltaTL;
                TR += deltaTR;
                BR += deltaBR;
                BL += deltaBL;

                TLMax = new Vector3(Mathf.Max(cData.mesh.initial.TL_Position.x + deltaTL.x, TLMax.x), Mathf.Max(cData.mesh.initial.TL_Position.y + deltaTL.y, TLMax.y), Mathf.Max(cData.mesh.initial.TL_Position.z + deltaTL.z, TLMax.z));
                TRMax = new Vector3(Mathf.Max(cData.mesh.initial.TR_Position.x + deltaTR.x, TRMax.x), Mathf.Max(cData.mesh.initial.TR_Position.y + deltaTR.y, TRMax.y), Mathf.Max(cData.mesh.initial.TR_Position.z + deltaTR.z, TRMax.z));
                BRMax = new Vector3(Mathf.Max(cData.mesh.initial.BR_Position.x + deltaBR.x, BRMax.x), Mathf.Max(cData.mesh.initial.BR_Position.y + deltaBR.y, BRMax.y), Mathf.Max(cData.mesh.initial.BR_Position.z + deltaBR.z, BRMax.z));
                BLMax = new Vector3(Mathf.Max(cData.mesh.initial.BL_Position.x + deltaBL.x, BLMax.x), Mathf.Max(cData.mesh.initial.BL_Position.y + deltaBL.y, BLMax.y), Mathf.Max(cData.mesh.initial.BL_Position.z + deltaBL.z, BLMax.z));

                TLMin = new Vector3(Mathf.Min(cData.mesh.initial.TL_Position.x + deltaTL.x, TLMin.x), Mathf.Min(cData.mesh.initial.TL_Position.y + deltaTL.y, TLMin.y), Mathf.Min(cData.mesh.initial.TL_Position.z + deltaTL.z, TLMin.z));
                TRMin = new Vector3(Mathf.Min(cData.mesh.initial.TR_Position.x + deltaTR.x, TRMin.x), Mathf.Min(cData.mesh.initial.TR_Position.y + deltaTR.y, TRMin.y), Mathf.Min(cData.mesh.initial.TR_Position.z + deltaTR.z, TRMin.z));
                BRMin = new Vector3(Mathf.Min(cData.mesh.initial.BR_Position.x + deltaBR.x, BRMin.x), Mathf.Min(cData.mesh.initial.BR_Position.y + deltaBR.y, BRMin.y), Mathf.Min(cData.mesh.initial.BR_Position.z + deltaBR.z, BRMin.z));
                BLMin = new Vector3(Mathf.Min(cData.mesh.initial.BL_Position.x + deltaBL.x, BLMin.x), Mathf.Min(cData.mesh.initial.BL_Position.y + deltaBL.y, BLMin.y), Mathf.Min(cData.mesh.initial.BL_Position.z + deltaBL.z, BLMin.z));
            }

            if (cData.colorsDirty)
            {
                if (cData.alphasDirty)
                {
                    BL_Color = cData.mesh.GetColor(0);
                    TL_Color = cData.mesh.GetColor(1);
                    TR_Color = cData.mesh.GetColor(2);
                    BR_Color = cData.mesh.GetColor(3);
                }
                else
                {
                    Color32 color = cData.mesh.GetColor(0);
                    color.a = BL_Color.a;
                    BL_Color = color;

                    color = cData.mesh.GetColor(1);
                    color.a = TL_Color.a;
                    TL_Color = color;

                    color = cData.mesh.GetColor(2);
                    color.a = TR_Color.a;
                    TR_Color = color;

                    color = cData.mesh.GetColor(3);
                    color.a = BR_Color.a;
                    BR_Color = color;
                }
            }
            else if (cData.alphasDirty)
            {
                BL_Color.a = cData.mesh.GetAlpha(0);
                TL_Color.a = cData.mesh.GetAlpha(1);
                TR_Color.a = cData.mesh.GetAlpha(2);
                BR_Color.a = cData.mesh.GetAlpha(3);
            }

            if (cData.uvsDirty)
            {
                BL_UV = cData.mesh.GetUV0(0);
                TL_UV = cData.mesh.GetUV0(1);
                TR_UV = cData.mesh.GetUV0(2);
                BR_UV = cData.mesh.GetUV0(3);

                BL_UV2 = cData.mesh.GetUV2(0);
                TL_UV2 = cData.mesh.GetUV2(1);
                TR_UV2 = cData.mesh.GetUV2(2);
                BR_UV2 = cData.mesh.GetUV2(3);
            }
        }
    }
}