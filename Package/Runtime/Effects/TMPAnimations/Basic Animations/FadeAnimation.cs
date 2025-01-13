using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using UnityEngine;
using static TMPEffects.TMPAnimations.TMPAnimationUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new FadeAnimation", menuName = "TMPEffects/Animations/Basic Animations/Built-in/Fade")]
    public partial class FadeAnimation : TMPAnimation
    {
        [SerializeField, AutoParameterBundle("")]
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\n" +
                 "For more information about it, see the section on Waves in the documentation.")]
        Wave wave;

        [SerializeField, AutoParameterBundle("")]
        [Tooltip("The timing offsets used by this animation. No prefix.\n" +
                 "For more information about it, see the section on OffsetProviders in the documentation.")]
        OffsetBundle waveOffset;

        [SerializeField, AutoParameter("maxopacity", "maxop", "max")]
        [Tooltip("The maximum opacity that is reached.\nAliases: maxopacity, maxop, max")]
        [Range(0, 255)] float maxOpacity = 255;

        [SerializeField, AutoParameter("fadeinanchor", "fianchor", "fianc", "fia")]
        [Tooltip("The anchor used for fading in.\nAliases: fadeinanchor, fianchor, fianc, fia")]
        Vector3 fadeInAnchor = Vector3.zero;

        [SerializeField, AutoParameter("fadeindirection", "fidirection", "fidir", "fid")]
        [Tooltip("The direction to fade in in.\nAliases: fadeindirection, fidirection, fidir, fid")]
        Vector3 fadeInDirection = Vector3.up;

        [SerializeField, AutoParameter("minopacity", "minop", "min")]
        [Tooltip("The minimum opacity that is reached.\nAliases: minopacity, minop, min")]
        [Range(0, 255)] float minOpacity = 0;

        [SerializeField, AutoParameter("fadeoutanchor", "foanchor", "foanc", "foa")]
        [Tooltip("The anchor used for fading out.\nAliases: fadeoutanchor, foanchor, foanc, foa")]
        Vector3 fadeOutAnchor = Vector3Int.zero;

        [SerializeField, AutoParameter("fadeoutdirection", "fodirection", "fodir", "fod")]
        [Tooltip("The direction to fade out in.\nAliases: fadeoutdirection, fodirection, fodir, fod")]
        Vector3 fadeOutDirection = Vector3.up;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            (float, int) result = d.wave.Evaluate(context.AnimatorContext.PassedTime,
                d.waveOffset.GetOffset(cData, context));

            if (result.Item2 > 0)
            {
                FadeIn(cData, context, d, result.Item1);
            }
            else
            {
                FadeOut(cData, context, d, result.Item1);
            }
        }


        private void FadeIn(CharData cData, IAnimationContext context, AutoParametersData d, float t)
        {
            Vector2 anchor = d.fadeInAnchor;
            FixAnchor(ref anchor);

            if (anchor == Vector2.zero)
            {
                for (int i = 0; i < 4; i++)
                {
                    float eval = Mathf.Lerp(d.minOpacity, d.maxOpacity, t);

                    Color32 c;
                    c = cData.mesh.initial.GetColor(i);
                    c.a = (byte)((eval / 255f) * c.a);

                    cData.mesh.SetAlpha(i, (byte)((eval / 255f) * c.a));
                }

                return;
            }

            Vector2 direction = new Vector2(-anchor.x, -anchor.y);
            Vector2 opposite = direction;

            Vector3 anchorPosition = AnchorToPosition(anchor, cData);
            Vector3 oppositePosition = AnchorToPosition(opposite, cData);
            float dist = (anchorPosition - oppositePosition).magnitude;

            for (int i = 0; i < 4; i++)
            {
                Vector3 vec = cData.mesh.initial.GetPosition(i) - anchorPosition;
                vec.x *= direction.x;
                vec.y *= direction.y;

                float currDist = (vec.magnitude / dist);

                float eval = Mathf.Lerp(d.minOpacity, d.maxOpacity, t * (2 - currDist));

                Color32 c;
                c = cData.mesh.initial.GetColor(i);
                c.a = (byte)((eval / 255f) * c.a);

                cData.mesh.SetAlpha(i, (byte)((eval / 255f) * c.a));
            }
        }

        private void FadeOut(CharData cData, IAnimationContext context, AutoParametersData d, float t)
        {
            Vector2 anchor = d.fadeOutAnchor;
            FixAnchor(ref anchor);

            if (anchor == Vector2.zero)
            {
                for (int i = 0; i < 4; i++)
                {
                    float eval = Mathf.Lerp(d.minOpacity, d.maxOpacity, t);

                    Color32 c;
                    c = cData.mesh.initial.GetColor(i);
                    c.a = (byte)((eval / 255f) * c.a);

                    cData.mesh.SetAlpha(i, (byte)((eval / 255f) * c.a));
                }

                return;
            }

            Vector2 direction = new Vector2(-anchor.x, -anchor.y);
            Vector2 opposite = direction;

            Vector3 anchorPosition = AnchorToPosition(anchor, cData);
            Vector3 oppositePosition = AnchorToPosition(opposite, cData);
            float dist = (anchorPosition - oppositePosition).magnitude;

            for (int i = 0; i < 4; i++)
            {
                Vector3 vec = cData.mesh.initial.GetPosition(i) - anchorPosition;
                vec.x *= direction.x;
                vec.y *= direction.y;

                float currDist = (vec.magnitude / dist);

                float eval = Mathf.Lerp(d.minOpacity, d.maxOpacity, t * (2 - currDist));

                Color32 c;
                c = cData.mesh.initial.GetColor(i);
                c.a = (byte)((eval / 255f) * c.a);
                cData.mesh.SetAlpha(i, (byte)((eval / 255f) * c.a));
            }
        }

        private void FixAnchor(ref Vector2 v)
        {
            if (v.x != 0)
            {
                if (v.x > 0) v.x = 1;
                else v.x = -1;
            }

            if (v.y != 0)
            {
                if (v.y > 0) v.y = 1;
                else v.y = -1;
            }
        }

        [AutoParametersStorage]
        private partial class AutoParametersData
        {
            public readonly float sqrt2 = Mathf.Sqrt(8);
            public readonly float[] dists = new float[4];
        }
    }
}