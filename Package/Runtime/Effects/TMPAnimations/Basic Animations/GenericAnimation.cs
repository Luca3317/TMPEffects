using System;
using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using UnityEngine;

namespace TMPEffects.TMPAnimations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new GenericAnimation",
        menuName = "TMPEffects/Animations/Basic Animations/Generic Animation")]
    public partial class GenericAnimation : TMPAnimation
    {
        public List<AnimationStep> AnimationSteps => animationSteps;
        public bool Repeat => repeat;
        public float Duration => duration;

        [SerializeReference] private List<AnimationStep> animationSteps = new List<AnimationStep>()
        {
            new AnimationStep()
        };

        [AutoParameter("repeat", "rp"), SerializeField]
        private bool repeat;

        [AutoParameter("duration", "dur"), SerializeField]
        private float duration;

        private CharDataModifiers modifiersStorage;

        [System.Serializable]
        public class GenericAnimationModifiers
        {
            [System.Serializable]
            public class RotationsStruct
            {
                public Vector3 eulerAngles = Vector3.zero;

                public ParameterTypes.TypedVector3 pivot =
                    new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);
            }

            public ParameterTypes.TypedVector3 Position =
                new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

            public Vector3 Scale = Vector3.one;

            public List<RotationsStruct> Rotations =
                new List<RotationsStruct>();

            public ParameterTypes.TypedVector3 BL_Position =
                new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

            public ParameterTypes.TypedVector3 TL_Position =
                new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

            public ParameterTypes.TypedVector3 TR_Position =
                new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

            public ParameterTypes.TypedVector3 BR_Position =
                new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

            public ColorOverride BL_Color = new ColorOverride();
            public ColorOverride TL_Color = new ColorOverride();
            public ColorOverride TR_Color = new ColorOverride();
            public ColorOverride BR_Color = new ColorOverride();

            public ParameterTypes.TypedVector3 BL_UV0 =
                new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

            public ParameterTypes.TypedVector3 TL_UV0 =
                new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

            public ParameterTypes.TypedVector3 TR_UV0 =
                new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

            public ParameterTypes.TypedVector3 BR_UV0 =
                new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);


            public CharDataModifiers ToCharDataModifiers(CharData cData, IAnimationContext ctx)
            {
                CharDataModifiers modifiers = new CharDataModifiers();

                // Position(Delta)
                if (Position.type == ParameterTypes.VectorType.Position)
                {
                    Vector3 position = Position.ToPosition(cData);
                    modifiers.CharacterModifiers.Position = new Vector3Override(position);
                }
                else if (Position.vector != Vector3.zero)
                {
                    Vector3 posDelta = Position.ToDelta(cData);
                    if (posDelta != Vector3.zero) modifiers.CharacterModifiers.PositionDelta = posDelta;
                }

                // Scale
                if (Scale != Vector3.one)
                {
                    modifiers.CharacterModifiers.ScaleDelta = Matrix4x4.Scale(Scale);
                }

                // Rotations
                if (Rotations.Count > 0)
                {
                    for (int i = 0; i < Rotations.Count; i++)
                    {
                        var rot = Rotations[i];
                        if (rot.eulerAngles != Vector3.zero)
                        {
                            Vector3 pivot = Vector3.zero;
                            if (rot.pivot.type == ParameterTypes.VectorType.Position)
                            {
                                Vector3 position = rot.pivot.IgnoreScaling(cData, ctx)
                                    .ToDelta(cData);
                                pivot = position;
                            }
                            else
                            {
                                Vector3 posDelta = rot.pivot.ToPosition(cData);
                                pivot = posDelta;
                            }
                            
                            Debug.LogWarning("Calced pivot " +pivot);
                
                            modifiers.CharacterModifiers.AddRotation(new Rotation(rot.eulerAngles, pivot));
                        }
                    }
                }
                
                // Vertex colors
                modifiers.MeshModifiers.BL_Color = BL_Color;
                modifiers.MeshModifiers.TL_Color = TL_Color;
                modifiers.MeshModifiers.TR_Color = TR_Color;
                modifiers.MeshModifiers.BR_Color = BR_Color;
                
                // Vertex UVs
                // if (BL_UV0.type == ParameterTypes.VectorType.Position)
                // {
                //     Vector3 position = BL_UV0.IgnoreScaling(cData, ctx)
                //         .ToPosition(cData, cData.InitialMesh.BL_UV0);
                //     modifiers.MeshModifiers.BL_UV0 = new Vector3Override(position);
                // }
                // else
                // {
                //     Vector3 posDelta = BL_UV0.ToPosition(cData, cData.InitialMesh.BL_UV0);
                //     if (posDelta != Vector3.zero) 
                //     modifiers.MeshModifiers.BL_UV0 = new Vector3Override(posDelta);
                // }
                
                // Vertex deltas
                if (BL_Position.type == ParameterTypes.VectorType.Position)
                {
                    Vector3 position = BL_Position.IgnoreScaling(cData, ctx)
                        .ToDelta(cData, cData.InitialMesh.BL_Position);
                    modifiers.MeshModifiers.BL_Delta = position;
                }
                else
                {
                    Vector3 posDelta = BL_Position.ToDelta(cData, cData.InitialMesh.BL_Position);
                    if (posDelta != Vector3.zero) modifiers.MeshModifiers.BL_Delta = posDelta;
                }
                
                if (TL_Position.type == ParameterTypes.VectorType.Position)
                {
                    Vector3 position = TL_Position.IgnoreScaling(cData, ctx)
                        .ToDelta(cData, cData.InitialMesh.TL_Position);
                    modifiers.MeshModifiers.TL_Delta = position;
                }
                else
                {
                    Vector3 posDelta = TL_Position.ToDelta(cData, cData.InitialMesh.TL_Position);
                    if (posDelta != Vector3.zero) modifiers.MeshModifiers.TL_Delta = posDelta;
                }
                
                if (TR_Position.type == ParameterTypes.VectorType.Position)
                {
                    Vector3 position = TR_Position.IgnoreScaling(cData, ctx)
                        .ToDelta(cData, cData.InitialMesh.TR_Position);
                    modifiers.MeshModifiers.TR_Delta = position;
                }
                else
                {
                    Vector3 posDelta = TR_Position.ToDelta(cData, cData.InitialMesh.TR_Position);
                    if (posDelta != Vector3.zero) modifiers.MeshModifiers.TR_Delta = posDelta;
                }
                
                if (BR_Position.type == ParameterTypes.VectorType.Position)
                {
                    Vector3 position = BR_Position.IgnoreScaling(cData, ctx)
                        .ToDelta(cData, cData.InitialMesh.BR_Position);
                    modifiers.MeshModifiers.BR_Delta = position;
                }
                else
                {
                    Vector3 posDelta = BR_Position.ToDelta(cData, cData.InitialMesh.BR_Position);
                    modifiers.MeshModifiers.BR_Delta = posDelta;
                }
                
                // Debug.LogWarning("Returning: POsdelta: " +modifiers.CharacterModifiers.PositionDelta + " Scale " + modifiers.CharacterModifiers.ScaleDelta + " pos " + modifiers.CharacterModifiers.Position.GetValue(cData.InitialPosition));
                return modifiers;
            }
        }

        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
            modifiersStorage ??= new CharDataModifiers();

            float timeValue =
                data.repeat ? context.AnimatorContext.PassedTime % data.duration : context.AnimatorContext.PassedTime;

            CharDataModifiers accModifier = new CharDataModifiers();
            accModifier.Reset();
            modifiersStorage.Reset();

            // TODO Probably should use an IntervalTree or smth for this
            int count = 0;
            foreach (var step in animationSteps)
            {
                if (step == null) continue;
                if (!step.animate) continue;
                if (step.startTime > timeValue) continue;
                if (step.EndTime < timeValue) continue;

                CharDataModifiers toUse = step.genModifiers.ToCharDataModifiers(cData, context);
                CharDataModifiers result;

                if (step.useWave)
                {
                    result =
                        CharDataModifiers.LerpUnclamped(cData, toUse,
                            step.wave.Evaluate(timeValue,
                                AnimationUtility.GetWaveOffset(cData, context, step.waveOffsetType)).Value,
                            modifiersStorage);
                }
                else
                {
                    result = toUse;
                }

                float entry = timeValue - step.startTime;
                if (entry <= step.entryDuration)
                {
                    result = CharDataModifiers.LerpUnclamped(cData, result,
                        step.entryCurve.Evaluate(entry / step.entryDuration), modifiersStorage);
                }

                float exit = step.EndTime - timeValue;
                if (exit <= step.exitDuration)
                {
                    result = CharDataModifiers.LerpUnclamped(cData, result,
                        step.exitCurve.Evaluate(exit / step.exitDuration), modifiersStorage);
                }

                accModifier.MeshModifiers.Combine(result.MeshModifiers);
                accModifier.CharacterModifiers.Combine(result.CharacterModifiers);
            }

            cData.MeshModifiers.Combine(accModifier.MeshModifiers);
            cData.CharacterModifiers.Combine(accModifier.CharacterModifiers);
        }
    }
}