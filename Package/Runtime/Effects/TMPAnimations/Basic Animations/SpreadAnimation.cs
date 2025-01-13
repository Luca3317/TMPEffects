using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using UnityEngine;
using static TMPEffects.TMPAnimations.TMPAnimationUtility;
using static TMPEffects.Parameters.TMPParameterTypes;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new SpreadAnimation",
        menuName = "TMPEffects/Animations/Basic Animations/Built-in/Spread")]
    public partial class SpreadAnimation : TMPAnimation
    {
        [SerializeField, AutoParameterBundle("")]
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\n" +
                 "For more information about it, see the section on Waves in the documentation.")]
        Wave wave;

        [SerializeField, AutoParameterBundle("")]
        [Tooltip("The timing offsets used by this animation. No prefix.\n" +
                 "For more information about it, see the section on OffsetProviders in the documentation.")]
        OffsetBundle waveOffsetType;

        [SerializeField, AutoParameter("growanchor", "growanc", "ganc")]
        [Tooltip("The anchor used for growing.\nAliases: growanchor, growanc, ganc")]
        TypedVector3 growAnchor = new TypedVector3(VectorType.Anchor, Vector3.zero);

        [SerializeField, AutoParameter("growdirection", "growdir", "gdir")]
        [Tooltip("The direction used for growing.\nAliases: growdirection, growdir, gdir")]
        Vector3 growDirection = Vector3.up;

        [SerializeField, AutoParameter("shrinkanchor", "shrinkanc", "sanc")]
        [Tooltip("The anchor used for shrinking.\nAliases: shrinkanchor, shrinkanc, sanc")]
        TypedVector3 shrinkAnchor = new TypedVector3(VectorType.Anchor, Vector3.zero);

        [SerializeField, AutoParameter("shrinkdirection", "shrinkdir", "sdir")]
        [Tooltip("The direction used for shrinking.\nAliases: shrinkdirection, shrinkdir, sdir")]
        Vector3 shrinkDirection = Vector3.up;

        [SerializeField, AutoParameter("maxpercentage", "maxp", "max")]
        [Tooltip(
            "The maximum percentage to spread to, at 1 being completely shown.\nAliases: maxpercentage, maxp, max")]
        float maxPercentage = 1;

        [SerializeField, AutoParameter("minpercentage", "minp", "min")]
        [Tooltip(
            "The minimum percentage to unspread to, at 0 being completely hidden.\nAliases: minpercentage, minp, min")]
        float minPercentage = 0;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            // Evaluate the wave based on time and offset
            var result = d.wave.Evaluate(context.AnimatorContext.PassedTime,
                d.waveOffsetType.GetOffset(cData, context));

            // If the wave is moving up, grow the character
            if (result.Direction > 0)
            {
                Grow(cData, context, d, result.Item1);
            }
            // else, shrink the character
            else
            {
                Shrink(cData, context, d, result.Item1);
            }
        }

        private void Grow(CharData cData, IAnimationContext context, AutoParametersData d, float t)
        {
            float percentage = Mathf.LerpUnclamped(d.minPercentage, d.maxPercentage, t);

            Vector3 actualDir = new Vector3(-d.growDirection.y, d.growDirection.x, 0f);

            Vector3 lineStart, lineEnd;

            switch (d.growAnchor.type)
            {
                case VectorType.Offset:
                    lineStart = cData.InitialPosition + (d.growAnchor.vector - actualDir * 2);
                    lineEnd = cData.InitialPosition + (d.growAnchor.vector + actualDir * 2);
                    break;
                case VectorType.Anchor:
                    lineStart = AnchorToPosition(d.growAnchor.vector - actualDir * 2, cData);
                    lineEnd = AnchorToPosition(d.growAnchor.vector + actualDir * 2, cData);
                    break;
                case VectorType.Position:
                    lineStart = d.growAnchor.vector - actualDir * 2;
                    lineEnd = d.growAnchor.vector + actualDir * 2;
                    break;

                default: throw new System.NotImplementedException(nameof(growAnchor.type));
            }

            for (int i = 0; i < 4; i++)
            {
                Vector3 startPos = ClosestPointOnLine(lineStart, lineEnd, cData.mesh.initial.GetPosition(i));
                Vector3 pos = Vector3.LerpUnclamped(startPos, cData.mesh.initial.GetPosition(i), percentage);

                SetVertexRaw(i, pos, cData, context);
            }
        }

        private void Shrink(CharData cData, IAnimationContext context, AutoParametersData d, float t)
        {
            float percentage = Mathf.LerpUnclamped(d.minPercentage, d.maxPercentage, t);

            Vector3 actualDir = new Vector3(-d.shrinkDirection.y, d.shrinkDirection.x, 0f);

            Vector3 lineStart, lineEnd;

            switch (d.shrinkAnchor.type)
            {
                case VectorType.Offset:
                    lineStart = cData.InitialPosition + (d.shrinkAnchor.vector - actualDir * 2);
                    lineEnd = cData.InitialPosition + (d.shrinkAnchor.vector + actualDir * 2);
                    break;
                case VectorType.Anchor:
                    lineStart = AnchorToPosition(d.shrinkAnchor.vector - actualDir * 2, cData);
                    lineEnd = AnchorToPosition(d.shrinkAnchor.vector + actualDir * 2, cData);
                    break;
                case VectorType.Position:
                    lineStart = d.shrinkAnchor.vector - actualDir * 2;
                    lineEnd = d.shrinkAnchor.vector + actualDir * 2;
                    break;

                default: throw new System.NotImplementedException(nameof(shrinkAnchor.type));
            }

            for (int i = 0; i < 4; i++)
            {
                Vector3 startPos = ClosestPointOnLine(lineStart, lineEnd, cData.mesh.initial.GetPosition(i));
                Vector3 pos = Vector3.LerpUnclamped(startPos, cData.mesh.initial.GetPosition(i), percentage);

                SetVertexRaw(i, pos, cData, context);
            }
        }
    }
}