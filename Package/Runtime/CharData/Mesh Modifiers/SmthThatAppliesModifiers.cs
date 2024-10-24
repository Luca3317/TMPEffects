   // using TMPEffects.CharacterData;
   // using TMPEffects.TMPAnimations;
   // using UnityEngine;
   //
   // public class SmthThatAppliesModifiers
   //  {
   //      private Vector3 BLMin, BLMax;
   //      private Vector3 TLMin, TLMax;
   //      private Vector3 TRMin, TRMax;
   //      private Vector3 BRMin, BRMax;
   //
   //      private (Vector3 BL, Vector3 TL, Vector3 TR, Vector3 BR)? prevResult = null;
   //
   //      public SmthThatAppliesModifiers()
   //      {
   //      }
   //
   //      public void MakeModifierDeltasRaw(CharData cData, IAnimationContext ctx, TMPMeshModifiers modifiers)
   //      {
   //          var textCmp = ctx.AnimatorContext.Animator.TextComponent;
   //          var pointSize = cData.info.pointSize;
   //          var fontSize = ctx.AnimatorContext.Animator.TextComponent.fontSize;
   //
   //          if (!modifiers.PositionDeltaIsRaw && modifiers.PositionDelta != Vector3.zero)
   //          {
   //              modifiers.PositionDeltaIsRaw = true;
   //              modifiers.PositionDelta = AnimationUtility.InverseScaleVector(modifiers.PositionDelta, textCmp,
   //                  true, true, pointSize, fontSize);
   //          }
   //
   //          if (!modifiers.BL_DeltaIsRaw && modifiers.BL_Delta != Vector3.zero)
   //          {
   //              modifiers.BL_DeltaIsRaw = true;
   //              modifiers.BL_Delta = AnimationUtility.InverseScaleVector(modifiers.BL_Delta, textCmp,
   //                  true, true, pointSize, fontSize);
   //          }
   //
   //          if (!modifiers.TL_DeltaIsRaw && modifiers.TL_Delta != Vector3.zero)
   //          {
   //              modifiers.TL_DeltaIsRaw = true;
   //              modifiers.TL_Delta = AnimationUtility.InverseScaleVector(modifiers.TL_Delta, textCmp,
   //                  true, true, pointSize, fontSize);
   //          }
   //
   //          if (!modifiers.TR_DeltaIsRaw && modifiers.TR_Delta != Vector3.zero)
   //          {
   //              modifiers.TR_DeltaIsRaw = true;
   //              modifiers.TR_Delta = AnimationUtility.InverseScaleVector(modifiers.TR_Delta, textCmp,
   //                  true, true, pointSize, fontSize);
   //          }
   //
   //          if (!modifiers.BR_DeltaIsRaw && modifiers.BR_Delta != Vector3.zero)
   //          {
   //              modifiers.BR_DeltaIsRaw = true;
   //              modifiers.BR_Delta = AnimationUtility.InverseScaleVector(modifiers.BR_Delta, textCmp,
   //                  true, true, pointSize, fontSize);
   //          }
   //      }
   //
   //      // TODO The applier (TMPMeshModifierApplier or so) should be reused within the actual animator
   //      // (/ in the chardatastate). Does this method still make sense then?
   //      public void ApplyToCharData(CharData cData, TMPMeshModifiers modifiers)
   //      {
   //          if (modifiers.PositionDelta != Vector3.zero)
   //              cData.SetPosition(cData.InitialPosition + modifiers.PositionDelta);
   //
   //          if (modifiers.RotationDelta != Quaternion.identity)
   //              cData.SetRotation(cData.InitialRotation * modifiers.RotationDelta);
   //
   //          // TODO I want to be able to use simple V3 scale -- why didnt i originally? some shit with order of applying?
   //          // if (step.Modifiers.ScaleDelta != Vector3.one)
   //
   //          if (modifiers.BL_Delta != Vector3.zero)
   //              cData.SetVertex(0, cData.initialMesh.BL_Position + modifiers.BL_Delta);
   //
   //          if (modifiers.TL_Delta != Vector3.zero)
   //              cData.SetVertex(1, cData.initialMesh.TL_Position + modifiers.TL_Delta);
   //
   //          if (modifiers.TR_Delta != Vector3.zero)
   //              cData.SetVertex(2, cData.initialMesh.TR_Position + modifiers.TR_Delta);
   //
   //          if (modifiers.BR_Delta != Vector3.zero)
   //              cData.SetVertex(3, cData.initialMesh.BR_Position + modifiers.BR_Delta);
   //
   //
   //          if (modifiers.BL_Color.OverrideColor)
   //              cData.mesh.SetColor(0, modifiers.BL_Color.Color, !modifiers.BL_Color.OverrideAlpha);
   //          else if (modifiers.BL_Color.OverrideAlpha)
   //              cData.mesh.SetAlpha(0, modifiers.BL_Color.Color.a);
   //
   //          if (modifiers.TL_Color.OverrideColor)
   //              cData.mesh.SetColor(1, modifiers.TL_Color.Color, !modifiers.TL_Color.OverrideAlpha);
   //          else if (modifiers.TL_Color.OverrideAlpha)
   //              cData.mesh.SetAlpha(1, modifiers.TL_Color.Color.a);
   //
   //          if (modifiers.TR_Color.OverrideColor)
   //              cData.mesh.SetColor(2, modifiers.TR_Color.Color, !modifiers.TR_Color.OverrideAlpha);
   //          else if (modifiers.TR_Color.OverrideAlpha)
   //              cData.mesh.SetAlpha(2, modifiers.TR_Color.Color.a);
   //
   //          if (modifiers.BR_Color.OverrideColor)
   //              cData.mesh.SetColor(3, modifiers.BR_Color.Color, !modifiers.BR_Color.OverrideAlpha);
   //          else if (modifiers.BR_Color.OverrideAlpha)
   //              cData.mesh.SetAlpha(3, modifiers.TL_Color.Color.a);
   //
   //          if (modifiers.BL_UV0 != Vector3.zero)
   //              cData.mesh.SetUV0(0, modifiers.BL_UV0);
   //
   //          if (modifiers.TL_UV0 != Vector3.zero)
   //              cData.mesh.SetUV0(1, modifiers.TL_UV0);
   //
   //          if (modifiers.TR_UV0 != Vector3.zero)
   //              cData.mesh.SetUV0(2, modifiers.TR_UV0);
   //
   //          if (modifiers.BR_UV0 != Vector3.zero)
   //              cData.mesh.SetUV0(3, modifiers.BR_UV0);
   //      }
   //
   //      // TODO qucik test method to see if TMPMEshmodifiers works in concept
   //      public void ApplyToCurrentCharData(CharData cData, TMPMeshModifiers modifiers)
   //      {
   //          if (modifiers.PositionDelta != Vector3.zero)
   //              cData.SetPosition(cData.Position + modifiers.PositionDelta);
   //
   //          if (modifiers.RotationDelta != Quaternion.identity)
   //              cData.SetRotation(cData.Rotation * modifiers.RotationDelta);
   //
   //          // TODO I want to be able to use simple V3 scale -- why didnt i originally? some shit with order of applying?
   //          if (modifiers.ScaleDelta != Matrix4x4.identity)
   //              cData.SetScale(modifiers.ScaleDelta.MultiplyPoint3x4(Vector3.one));
   //
   //          if (modifiers.BL_Delta != Vector3.zero)
   //              cData.AddVertexDelta(0, modifiers.BL_Delta);
   //
   //          if (modifiers.TL_Delta != Vector3.zero)
   //              cData.AddVertexDelta(1, modifiers.TL_Delta);
   //
   //          if (modifiers.TR_Delta != Vector3.zero)
   //              cData.AddVertexDelta(2, modifiers.TR_Delta);
   //
   //          if (modifiers.BR_Delta != Vector3.zero)
   //              cData.AddVertexDelta(3, modifiers.BR_Delta);
   //
   //
   //          if (modifiers.BL_Color.OverrideColor)
   //              cData.mesh.SetColor(0, modifiers.BL_Color.Color, !modifiers.BL_Color.OverrideAlpha);
   //          else if (modifiers.BL_Color.OverrideAlpha)
   //              cData.mesh.SetAlpha(0, modifiers.BL_Color.Color.a);
   //
   //          if (modifiers.TL_Color.OverrideColor)
   //              cData.mesh.SetColor(1, modifiers.TL_Color.Color, !modifiers.TL_Color.OverrideAlpha);
   //          else if (modifiers.TL_Color.OverrideAlpha)
   //              cData.mesh.SetAlpha(1, modifiers.TL_Color.Color.a);
   //
   //          if (modifiers.TR_Color.OverrideColor)
   //              cData.mesh.SetColor(2, modifiers.TR_Color.Color, !modifiers.TR_Color.OverrideAlpha);
   //          else if (modifiers.TR_Color.OverrideAlpha)
   //              cData.mesh.SetAlpha(2, modifiers.TR_Color.Color.a);
   //
   //          if (modifiers.BR_Color.OverrideColor)
   //              cData.mesh.SetColor(3, modifiers.BR_Color.Color, !modifiers.BR_Color.OverrideAlpha);
   //          else if (modifiers.BR_Color.OverrideAlpha)
   //              cData.mesh.SetAlpha(3, modifiers.TL_Color.Color.a);
   //
   //          if (modifiers.BL_UV0 != Vector3.zero)
   //              cData.mesh.SetUV0(0, modifiers.BL_UV0);
   //
   //          if (modifiers.TL_UV0 != Vector3.zero)
   //              cData.mesh.SetUV0(1, modifiers.TL_UV0);
   //
   //          if (modifiers.TR_UV0 != Vector3.zero)
   //              cData.mesh.SetUV0(2, modifiers.TR_UV0);
   //
   //          if (modifiers.BR_UV0 != Vector3.zero)
   //              cData.mesh.SetUV0(3, modifiers.BR_UV0);
   //      }
   //
   //      // TODO shouldnt have to pass in cdata; make some interface or w/e
   //      public (Vector3 BL, Vector3 TL, Vector3 TR, Vector3 BR) CalculateVertexPositions(CharData cData,
   //          TMPMeshModifiers modifiers)
   //      {
   //          if (modifiers.Dirty == TMPMeshModifiers.DirtyFlags.None && prevResult.HasValue)
   //          {
   //              return prevResult.Value;
   //          }
   //
   //          Vector3 vbl = cData.initialMesh.BL_Position; //+ modifiers.BL_Delta;
   //          Vector3 vtl = cData.initialMesh.TL_Position; // + modifiers.TL_Delta;
   //          Vector3 vtr = cData.initialMesh.TR_Position; // + modifiers.TR_Delta;
   //          Vector3 vbr = cData.initialMesh.BR_Position; // + modifiers.BR_Delta;
   //
   //          // Apply scale
   //          vbl = modifiers.ScaleDelta.MultiplyPoint3x4(vbl - cData.InitialPosition) + cData.InitialPosition;
   //          vtl = modifiers.ScaleDelta.MultiplyPoint3x4(vtl - cData.InitialPosition) + cData.InitialPosition;
   //          vtr = modifiers.ScaleDelta.MultiplyPoint3x4(vtr - cData.InitialPosition) + cData.InitialPosition;
   //          vbr = modifiers.ScaleDelta.MultiplyPoint3x4(vbr - cData.InitialPosition) + cData.InitialPosition;
   //
   //          // Apply rotation TODO
   //          // Vector3 pivot;
   //          Matrix4x4 matrix = Matrix4x4.Rotate(modifiers.RotationDelta);
   //          vtl = matrix.MultiplyPoint3x4(vtl);
   //          vtr = matrix.MultiplyPoint3x4(vtr);
   //          vbr = matrix.MultiplyPoint3x4(vbr);
   //          vbl = matrix.MultiplyPoint3x4(vbl);
   //
   //          // TODO Moved those after scaling; since its, well a transformation too. Might have to do the same in CHarDataState
   //          // TODO Actually it might not be that simple; this does mess with the appearance more than i assumed; maybe separate pre/post modifications? idk
   //          // Apply vertex transformations
   //          vbl += modifiers.BL_Delta;
   //          vtl += modifiers.TL_Delta;
   //          vtr += modifiers.TR_Delta;
   //          vbr += modifiers.BR_Delta;
   //
   //          // Apply transformation
   //          var scaled = modifiers.PositionDelta;
   //          vbl += scaled;
   //          vtl += scaled;
   //          vtr += scaled;
   //          vbr += scaled;
   //
   //          modifiers.ClearDirtyFlags();
   //          prevResult = (vbl, vtl, vtr, vbr);
   //          return prevResult.Value;
   //      }
   //
   //      public (Color32 BL, Color32 TL, Color32 TR, Color32 BR) CalculateVertexColors(CharData cData,
   //          TMPMeshModifiers modifiers)
   //      {
   //          throw new System.NotImplementedException();
   //          // return
   //          // (
   //          //     modifiers.BL_Color.HasValue ? modifiers.BL_Color.Value : cData.initialMesh.BL_Color,
   //          //     modifiers.TL_Color.HasValue ? modifiers.TL_Color.Value : cData.initialMesh.TL_Color,
   //          //     modifiers.TR_Color.HasValue ? modifiers.TR_Color.Value : cData.initialMesh.TR_Color,
   //          //     modifiers.BR_Color.HasValue ? modifiers.BR_Color.Value : cData.initialMesh.BR_Color
   //          // );
   //      }
   //
   //      // TODO same as above + need to have some default value for colors (=> not set); probably nullable
   //      public (Vector3 BL, Vector3 TL, Vector3 TR, Vector3 BR) CalculateVertexUVs(CharData cData,
   //          TMPMeshModifiers modifiers)
   //      {
   //          return
   //          (
   //              cData.initialMesh.BL_UV0,
   //              cData.initialMesh.TL_UV0,
   //              cData.initialMesh.TR_UV0,
   //              cData.initialMesh.BR_UV0
   //          );
   //      }
   //  }