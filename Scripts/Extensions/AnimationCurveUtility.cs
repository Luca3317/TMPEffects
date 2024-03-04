using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Security;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEngine;

namespace TMPEffects.Extensions
{
    public static class AnimationCurveUtility
    {
        // Need to make some really simple easy ways to define custom curves
        // For that
        //      1. Keywords that define complete, common curve
        //      2. Allow building curves like "(0.2,1),(1,2),(2,2)", where each () is a keyframe
        //      3. Allow building curves using keywords that defined some properties, e.g. ease-in
        //      4. Allow building curves with parameters, e.g. Spring(tension: 250, friction: 25), Spring(damp: 5), Bezier(...)

        // TODO
        //
        //  Curve(); string e.g. "(0.2,1),(1,2),(2,2)"
        //
        //  LinearBezier(); start,end | params points
        //  QuadraticBezier(); start,control,end | params points
        //  CubicBezier(); start,control0,control1,end | params points
        //  
        //  Penners easing functions (using the bezier methods?)
        //
        //  Maybe: Allow building curves using keywords that defined some properties, e.g. ease-in   


        public static readonly string[] AnimationCurveKeywords = new string[]
        {
            // Penner's easing functions
            // https://easings.net/
            "easeinsine",
            "easeoutsine",
            "easeinoutsine",

            "easeinquad",
            "easeoutquad",
            "easeinoutquad",

            "easeincubic",
            "easeoutcubic",
            "easeinoutcubic",

            "easeinquart",
            "easeoutquart",
            "easeinoutquart",

            "easeinquint",
            "easeoutquint",
            "easeinoutquint",

            "easeinexpo",
            "easeoutexpo",
            "easeinoutexpo",

            "easeincirc",
            "easeoutcirc",
            "easeinoutcirc",

            "easeinback",
            "easeoutback",
            "easeinoutback",

            "bouncein",
            "bounceout",
            "bounceinout",

            // Other
            "linear"
        };

        #region Spring
        #endregion

        #region Ease Sine
        private static readonly Vector2[] easeInSinePoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.360780f, -0.000436f),
            new Vector2(0.673486f, 0.486554f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeOutSinePoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.330931f, 0.520737f),
            new Vector2(0.641311f, 1.000333f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeInOutSinePoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.180390f, -0.000217f),
            new Vector2(0.336743f, 0.243277f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.665465f, 0.760338f),
            new Vector2(0.820656f, 1.000167f),
            new Vector2(1.0f, 1.0f)
        };

        public static readonly ReadOnlyCollection<Vector2> EaseInSinePoints = Array.AsReadOnly(easeInSinePoints);
        public static readonly ReadOnlyCollection<Vector2> EaseOutSinePoints = Array.AsReadOnly(easeOutSinePoints);
        public static readonly ReadOnlyCollection<Vector2> EaseInOutSinePoints = Array.AsReadOnly(easeInOutSinePoints);

        public static AnimationCurve EaseInSine() => CubicBezier(easeInSinePoints);
        public static AnimationCurve EaseOutSine() => CubicBezier(easeOutSinePoints);
        public static AnimationCurve EaseInOutSine() => CubicBezier(easeInOutSinePoints);
        #endregion

        #region Ease Quad
        private static readonly Vector2[] easeInQuadPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.333333f, 0.0f),
            new Vector2(0.666667f, 0.333333f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeOutQuadPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.333333f, 0.666667f),
            new Vector2(0.666667f, 1.0f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeInOutQuadPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.166667f, 0.0f),
            new Vector2(0.333333f, 0.166667f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.666667f, 0.833333f),
            new Vector2(0.833333f, 1.0f),
            new Vector2(1.0f, 1.0f)
        };

        public static readonly ReadOnlyCollection<Vector2> EaseInQuadPoints = Array.AsReadOnly(easeInQuadPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseOutQuadPoints = Array.AsReadOnly(easeOutQuadPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseInOutQuadPoints = Array.AsReadOnly(easeInOutQuadPoints);

        public static AnimationCurve EaseInQuad() => CubicBezier(easeInQuadPoints);
        public static AnimationCurve EaseOutQuad() => CubicBezier(easeOutQuadPoints);
        public static AnimationCurve EaseInOutQuad() => CubicBezier(easeInOutQuadPoints);
        #endregion

        #region Ease Cubic
        private static readonly Vector2[] easeInCubicPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.333333f, 0.0f),
            new Vector2(0.666667f, 0.0f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeOutCubicPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.333333f, 1.0f),
            new Vector2(0.666667f, 1.0f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeInOutCubicPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.166667f, 0.0f),
            new Vector2(0.333333f, 0.0f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.666667f, 1.0f),
            new Vector2(0.833333f, 1.0f),
            new Vector2(1.0f, 1.0f)
        };

        public static readonly ReadOnlyCollection<Vector2> EaseInCubicPoints = Array.AsReadOnly(easeInCubicPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseOutCubicPoints = Array.AsReadOnly(easeOutCubicPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseInOutCubicPoints = Array.AsReadOnly(easeInOutCubicPoints);

        public static AnimationCurve EaseInCubic() => CubicBezier(easeInCubicPoints);
        public static AnimationCurve EaseOutCubic() => CubicBezier(easeOutCubicPoints);
        public static AnimationCurve EaseInOutCubic() => CubicBezier(easeInOutCubicPoints);
        #endregion

        #region Ease Quart
        private static readonly Vector2[] easeInQuartPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.434789f, 0.006062f),
            new Vector2(0.730901f, -0.07258f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeOutQuartPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.269099f, 1.072581f),
            new Vector2(0.565211f, 0.993938f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeInOutQuartPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.217394f, 0.003031f),
            new Vector2(0.365451f, -0.036291f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.634549f, 1.036290f),
            new Vector2(0.782606f, 0.996969f),
            new Vector2(1.0f, 1.0f)
        };

        public static readonly ReadOnlyCollection<Vector2> EaseInQuartPoints = Array.AsReadOnly(easeInQuartPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseOutQuartPoints = Array.AsReadOnly(easeOutQuartPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseInOutQuartPoints = Array.AsReadOnly(easeInOutQuartPoints);

        public static AnimationCurve EaseInQuart() => CubicBezier(easeInQuartPoints);
        public static AnimationCurve EaseOutQuart() => CubicBezier(easeOutQuartPoints);
        public static AnimationCurve EaseInOutQuart() => CubicBezier(easeInOutQuartPoints);
        #endregion

        #region Ease Quint
        private static readonly Vector2[] easeInQuintPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.519568f, 0.012531f),
            new Vector2(0.774037f, -0.118927f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeOutQuintPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.225963f, 1.11926f),
            new Vector2(0.481099f, 0.987469f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeInOutQuintPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.259784f, 0.006266f),
            new Vector2(0.387018f, -0.059463f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.612982f, 1.059630f),
            new Vector2(0.740549f, 0.993734f),
            new Vector2(1.0f, 1.0f)
        };

        public static readonly ReadOnlyCollection<Vector2> EaseInQuintPoints = Array.AsReadOnly(easeInQuintPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseOutQuintPoints = Array.AsReadOnly(easeOutQuintPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseInOutQuintPoints = Array.AsReadOnly(easeInOutQuintPoints);

        public static AnimationCurve EaseInQuint() => CubicBezier(easeInQuintPoints);
        public static AnimationCurve EaseOutQuint() => CubicBezier(easeOutQuintPoints);
        public static AnimationCurve EaseInOutQuint() => CubicBezier(easeInOutQuintPoints);
        #endregion

        #region Ease Expo
        private static readonly Vector2[] easeInExpoPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.636963f, 0.0199012f),
            new Vector2(0.844333f, -0.0609379f),
            new Vector2(1.0f, 1.0f),
        };
        private static readonly Vector2[] easeOutExpoPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.155667f, 1.060938f),
            new Vector2(0.363037f, 0.980099f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeInOutExpoPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.318482f, 0.009951f),
            new Vector2(0.422167f, -0.030469f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.577833f, 1.0304689f),
            new Vector2(0.681518f, 0.9900494f),
            new Vector2(1.0f, 1.0f)
        };

        public static readonly ReadOnlyCollection<Vector2> EaseInExpoPoints = Array.AsReadOnly(easeInExpoPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseOutExpoPoints = Array.AsReadOnly(easeOutExpoPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseInOutExpoPoints = Array.AsReadOnly(easeInOutExpoPoints);

        public static AnimationCurve EaseInExpo() => CubicBezier(easeInExpoPoints);
        public static AnimationCurve EaseOutExpo() => CubicBezier(easeOutExpoPoints);
        public static AnimationCurve EaseInOutExpo() => CubicBezier(easeInOutExpoPoints);
        #endregion

        #region Ease Circ
        private static readonly Vector2[] easeInCircPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.55403f, 0.001198f),
            new Vector2(0.998802f, 0.449801f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeOutCircPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.001198f, 0.553198f),
            new Vector2(0.445976f, 0.998802f),
            new Vector2(1.0f, 1.0f)
        };
        private static readonly Vector2[] easeInOutCircPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.277013f, 0.000599f),
            new Vector2(0.499401f, 0.223401f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.500599f, 0.776599f),
            new Vector2(0.722987f, 0.999401f),
            new Vector2(1.0f, 1.0f)
        };

        public static readonly ReadOnlyCollection<Vector2> EaseInCircPoints = Array.AsReadOnly(easeInCircPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseOutCircPoints = Array.AsReadOnly(easeOutCircPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseInOutCircPoints = Array.AsReadOnly(easeInOutCircPoints);

        public static AnimationCurve EaseInCirc() => CubicBezier(easeInCircPoints);
        public static AnimationCurve EaseOutCirc() => CubicBezier(easeOutCircPoints);
        public static AnimationCurve EaseInOutCirc() => CubicBezier(easeInOutCircPoints);
        #endregion

        #region Ease Back
        private static readonly Vector2[] easeInBackPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.333333f, 0.0f),
            new Vector2(0.666667f, -0.567193f),
            new Vector2(1.0f, 1.0f),
        };
        private static readonly Vector2[] easeOutBackPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.333333f, 1.567193f),
            new Vector2(0.666667f, 1.0f),
            new Vector2(1.0f, 1.0f),
        };
        private static readonly Vector2[] easeInOutBackPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.166667f, 0.0f),
            new Vector2(0.333333f, -0.432485f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.666667f, 1.432485f),
            new Vector2(0.833333f, 1.0f),
            new Vector2(1.0f, 1.0f)
        };

        public static readonly ReadOnlyCollection<Vector2> EaseInBackPoints = Array.AsReadOnly(easeInBackPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseOutBackPoints = Array.AsReadOnly(easeOutBackPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseInOutBackPoints = Array.AsReadOnly(easeInOutBackPoints);

        public static AnimationCurve EaseInBack() => CubicBezier(easeInBackPoints);
        public static AnimationCurve EaseOutBack() => CubicBezier(easeOutBackPoints);
        public static AnimationCurve EaseInOutBack() => CubicBezier(easeInOutBackPoints);
        #endregion

        #region Ease Elastic
        private static readonly Vector2[] easeInElasticPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.175f, 0.00250747f),
            new Vector2(0.173542f, 0.0f),
            new Vector2(0.175f, 0.0f),

            new Vector2(0.4425f, -0.0184028f),
            new Vector2(0.3525f, 0.05f),
            new Vector2(0.475f, 0.0f),

            new Vector2(0.735f, -0.143095f),
            new Vector2(0.6575f, 0.383333f),
            new Vector2(0.775f, 0.0f),

            new Vector2(0.908125f, -0.586139f),
            new Vector2(0.866875f, -0.666667f),
            new Vector2(1.0f, 1.0f),
        };
        private static readonly Vector2[] easeOutElasticPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.133125f, 1.666667f),
            new Vector2(0.091875f, 1.586139f),
            new Vector2(0.225f, 1.0f),

            new Vector2(0.3425f, 0.616667f),
            new Vector2(0.265f, 1.143095f),
            new Vector2(0.525f, 1.0f),

            new Vector2(0.6475f, 0.95f),
            new Vector2(0.5575f, 1.0184028f),
            new Vector2(0.8250f, 1.0f),

            new Vector2(0.826458f, 1.0f),
            new Vector2(0.825f, 0.9974925f),
            new Vector2(1.0f, 1.0f),
        };
        private static readonly Vector2[] easeInOutElasticPoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.0875f, 0.001254f),
            new Vector2(0.086771f, 0.0f),
            new Vector2(0.0875f, 0.0f),

            new Vector2(0.22125f, -0.009201f),
            new Vector2(0.17625f, 0.025f),
            new Vector2(0.2375f, 0.0f),

            new Vector2(0.3675f, -0.071548f),
            new Vector2(0.32875f, 0.191667f),
            new Vector2(0.3875f, 0.0f),

            new Vector2(0.454063f, -0.293070f),
            new Vector2(0.433438f, -0.333334f),
            new Vector2(0.5f, 0.5f),

            new Vector2(0.5665625f, 1.333334f),
            new Vector2(0.5459375f, 1.293070f),
            new Vector2(0.6125f, 1.0f),

            new Vector2(0.67125f, 0.808334f),
            new Vector2(0.6325f, 1.071548f),
            new Vector2(0.7625f, 1.0f),

            new Vector2(0.82375f, 0.975f),
            new Vector2(0.77875f, 1.009201f),
            new Vector2(0.9125f, 1.0f),

            new Vector2(0.913229f, 1.0f),
            new Vector2(0.9125f, 0.9987463f),
            new Vector2(1.0f, 1.0f),
        };

        public static readonly ReadOnlyCollection<Vector2> EaseInElasticPoints = Array.AsReadOnly(easeInElasticPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseOutElasticPoints = Array.AsReadOnly(easeOutElasticPoints);
        public static readonly ReadOnlyCollection<Vector2> EaseInOutElasticPoints = Array.AsReadOnly(easeInOutElasticPoints);

        public static AnimationCurve EaseInElastic() => CubicBezier(easeInElasticPoints);
        public static AnimationCurve EaseOutElastic() => CubicBezier(easeOutElasticPoints);
        public static AnimationCurve EaseInOutElastic() => CubicBezier(easeInOutElasticPoints);
        #endregion

        #region Ease Bounce
        private static readonly Vector2[] easeInBouncePoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.030303f, 0.020833f),
            new Vector2(0.060606f, 0.020833f),
            new Vector2(0.0909f, 0.0f),

            new Vector2(0.151515f, 0.083333f),
            new Vector2(0.212121f, 0.083333f),
            new Vector2(0.2727f, 0.0f),

            new Vector2(0.393939f, 0.333333f),
            new Vector2(0.515152f, 0.333333f),
            new Vector2(0.6364f, 0.0f),

            new Vector2(0.757576f, 0.666667f),
            new Vector2(0.878788f, 1.0f),
            new Vector2(1.0f, 1.0f),
        };
        private static readonly Vector2[] easeOutBouncePoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.121212f, 0.0f),
            new Vector2(0.242424f, 0.333333f),
            new Vector2(0.3636f, 1.0f),

            new Vector2(0.484848f, 0.666667f),
            new Vector2(0.606060f, 0.666667f),
            new Vector2(0.7273f, 1.0f),

            new Vector2(0.787879f, 0.916667f),
            new Vector2(0.848485f, 0.916667f),
            new Vector2(0.9091f, 1.0f),

            new Vector2(0.939394f, 0.9791667f),
            new Vector2(0.969697f, 0.9791667f),
            new Vector2(1.0f, 1.0f),
        };
        private static readonly Vector2[] easeInOutBouncePoints = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.015152f, 0.010417f),
            new Vector2(0.030303f, 0.010417f),
            new Vector2(0.0455f, 0.0f),

            new Vector2(0.075758f, 0.041667f),
            new Vector2(0.106061f, 0.041667f),
            new Vector2(0.1364f, 0.0f),

            new Vector2(0.196970f, 0.166667f),
            new Vector2(0.257576f, 0.166667f),
            new Vector2(0.3182f, 0.0f),

            new Vector2(0.378788f, 0.333333f),
            new Vector2(0.439394f, 0.5f),
            new Vector2(0.5f, 0.5f),

            new Vector2(0.560606f, 0.5f),
            new Vector2(0.621212f, 0.666667f),
            new Vector2(0.6818f, 1.0f),

            new Vector2(0.742424f, 0.833333f),
            new Vector2(0.803030f, 0.833333f),
            new Vector2(0.8636f, 1.0f),

            new Vector2(0.893939f, 0.958333f),
            new Vector2(0.924242f, 0.958333f),
            new Vector2(0.9550f, 1.0f),

            new Vector2(0.969697f, 0.989583f),
            new Vector2(0.984848f, 0.989583f),
            new Vector2(1.0f, 1.0f),
        };

        public static readonly ReadOnlyCollection<Vector2> EaseInBouncePoints = Array.AsReadOnly(easeInBouncePoints);
        public static readonly ReadOnlyCollection<Vector2> EaseOutBouncePoints = Array.AsReadOnly(easeOutBouncePoints);
        public static readonly ReadOnlyCollection<Vector2> EaseInOutBouncePoints = Array.AsReadOnly(easeInOutBouncePoints);

        public static AnimationCurve EaseInBounce() => CubicBezier(easeInBouncePoints);
        public static AnimationCurve EaseOutBounce() => CubicBezier(easeOutBouncePoints);
        public static AnimationCurve EaseInOutBounce() => CubicBezier(easeInOutBouncePoints);
        #endregion

        #region Bezier Constructors

        public static AnimationCurve Bezier(params Vector2[] points)
        {
            int len = points.Length;
            if (len >= 4 && (len - 4) % 3 == 0) return CubicBezier(points);
            if (len >= 3 && (len - 3) % 2 == 0) return QuadraticBezier(points);
            return LinearBezier(points);
        }

        public static AnimationCurve Bezier(IEnumerable<Vector2> points)
        {
            Vector2[] array = points.ToArray();
            return Bezier(array);
        }

        // TODO Maybe implement some way to ensure no two keyframe (or its tangents) have the same x value
        // Or: explicitly document this as a limitation of animation curves
        public static AnimationCurve LinearBezier(Vector2 start, Vector2 end)
        {
            Keyframe kf0 = new Keyframe(start.x, start.y, 0f, 0f, 0f, 0f);
            Keyframe kf1 = new Keyframe(end.x, end.y, 0f, 0f, 0f, 0f);

            return new AnimationCurve(kf0, kf1);
        }

        public static AnimationCurve LinearBezier(params Vector2[] points)
        {
            if (points.Length < 2) throw new System.ArgumentException();

            Keyframe[] keyframes = new Keyframe[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 point = points[i];
                keyframes[i] = new Keyframe(point.x, point.y, 0f, 0f, 0f, 0f);
            }

            return new AnimationCurve(keyframes);
        }

        public static AnimationCurve LinearBezier(IEnumerable<Vector2> points)
        {
            Vector2[] array = points.ToArray();
            return LinearBezier(array);
        }

        public static AnimationCurve QuadraticBezier(Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint)
        {
            Vector2 controlpoint0 = startPoint + (2f / 3f) * (controlPoint - startPoint);
            Vector2 controlpoint1 = endPoint + (2f / 3f) * (controlPoint - endPoint);
            return CubicBezier(startPoint, controlpoint0, controlpoint1, endPoint);
        }

        public static AnimationCurve QuadraticBezier(params Vector2[] points)
        {
            int len = points.Length;
            if (points == null || len < 3 || (len > 3 && (len - 3) % 2 != 0))
            {
                throw new System.ArgumentException();
            }

            int segmentCount = 1 + (len - 3) / 2;

            Vector2[] cubicPoints = new Vector2[len + segmentCount];

            int offset = 0;
            for (int i = 0; i < len; i++)
            {
                if (i % 2 == 1)
                {
                    Vector2 controlpoint0 = points[i - 1] + (2f / 3f) * (points[i] - points[i - 1]);
                    Vector2 controlpoint1 = points[i + 1] + (2f / 3f) * (points[i] - points[i + 1]);

                    cubicPoints[i + offset++] = controlpoint0;
                    cubicPoints[i + offset] = controlpoint1;
                }
                else
                {
                    cubicPoints[i + offset] = points[i];
                }
            }
            return CubicBezier(cubicPoints);
        }

        public static AnimationCurve QuadraticBezier(IEnumerable<Vector2> points)
        {
            Vector2[] array = points.ToArray();
            return QuadraticBezier(array);
        }

        public static AnimationCurve CubicBezier(Vector2 startPoint, Vector2 controlPoint0, Vector2 controlPoint1, Vector2 endPoint)
        {
            var curve = new AnimationCurve();
            BezierToAnimationCurve(curve, new Vector2[] { startPoint, controlPoint0, controlPoint1, endPoint });
            return curve;
        }

        public static AnimationCurve CubicBezier(params Vector2[] points)
        {
            if (points == null || points.Length < 4 || (points.Length > 4 && (points.Length - 4) % 3 != 0))
            {
                throw new System.ArgumentException();
            }
            var curve = new AnimationCurve();
            BezierToAnimationCurve(curve, points);
            return curve;
        }

        public static AnimationCurve CubicBezier(IEnumerable<Vector2> points)
        {
            Vector2[] array = points.ToArray();
            return CubicBezier(array);
        }
        #endregion

        #region Bezier AnimationCurve transformation
        private static void BezierToAnimationCurve(AnimationCurve outCurve, Vector2[] controlPointStrips)
        {
            if (controlPointStrips.Length < 4)
            {
                throw new System.ArgumentException("The number of control point strips should more than 4!");
            }
            if ((controlPointStrips.Length - 4) % 3 != 0)
            {
                throw new System.ArgumentException("The number of control point strips N should be (N-4)%3==0");
            }

            int bezierCount = 1 + (controlPointStrips.Length - 4) / 3;
            Keyframe[] keyframes = new Keyframe[bezierCount + 1];
            // Init the first keyframe with the first cp.
            keyframes[0] = new Keyframe(controlPointStrips[0].x, controlPointStrips[0].y)
            {
                // Weight affects the position of the control point.
                // By default, WeightedMode is None which makes every points has 1/3 weight.(because 4 cp makes 2 point with 2 inner points)
                // https://docs.unity3d.com/ScriptReference/Keyframe-inWeight.html
                weightedMode = WeightedMode.Both
            };
            for (int i = 0; i < bezierCount; i++)
            {
                int cp = i * 3;
                // Set the outTangent of cp1 which means cp2.
                keyframes[i].outTangent = Tangent(controlPointStrips[cp], controlPointStrips[cp + 1]);
                // Makes the weight as the x offset of (cp2 - cp1).
                float bezierLength = controlPointStrips[cp + 3].x - controlPointStrips[cp].x;
                keyframes[i].outWeight = Weight(controlPointStrips[cp], controlPointStrips[cp + 1], bezierLength);
                // Create cp4 as keyframe and set its inTangent which means cp3.
                keyframes[i + 1] = new Keyframe(controlPointStrips[cp + 3].x, controlPointStrips[cp + 3].y)
                {
                    inTangent = Tangent(controlPointStrips[cp + 2], controlPointStrips[cp + 3]),
                    inWeight = Weight(controlPointStrips[cp + 2], controlPointStrips[cp + 3], bezierLength),
                    weightedMode = WeightedMode.Both
                };
            }

            if (outCurve == null)
            {
                outCurve = new AnimationCurve();
            }
            outCurve.keys = keyframes;
        }

        private static float Tangent(in Vector2 from, in Vector2 to)
        {
            Vector2 vec = to - from;
            return vec.y / vec.x;
        }

        // Weight has to be normalized by the distance of a single bezier.
        private static float Weight(in Vector2 from, in Vector2 to, float length)
        {
            return (to.x - from.x) / length;
        }
        #endregion

        #region Mapping
        internal static readonly ReadOnlyDictionary<string, ReadOnlyCollection<Vector2>> NamePointsMapping = new ReadOnlyDictionary<string, ReadOnlyCollection<Vector2>>(new Dictionary<string, ReadOnlyCollection<Vector2>>()
        {
            { "easeinsine", EaseInSinePoints },
            { "easeoutsine", EaseOutSinePoints },
            { "easeinoutsine", EaseInOutSinePoints },
            { "easein-sine", EaseInSinePoints },
            { "easeout-sine", EaseOutSinePoints },
            { "easeinout-sine", EaseInOutSinePoints },

            { "easeinquad", EaseInQuadPoints },
            { "easeoutquad", EaseOutQuadPoints },
            { "easeinoutquad", EaseInOutQuadPoints },
            { "easein-quad", EaseInQuadPoints },
            { "easeout-quad", EaseOutQuadPoints },
            { "easeinout-quad", EaseInOutQuadPoints },

            { "easeincubic", EaseInCubicPoints },
            { "easeoutcubic", EaseOutCubicPoints },
            { "easeinoutcubic", EaseInOutCubicPoints },
            { "easein-cubic", EaseInCubicPoints },
            { "easeout-cubic", EaseOutCubicPoints },
            { "easeinout-cubic", EaseInOutCubicPoints },

            { "easeinquart", EaseInQuartPoints },
            { "easeoutquart", EaseOutQuartPoints },
            { "easeinoutquart", EaseInOutQuartPoints },
            { "easein-quart", EaseInQuartPoints },
            { "easeout-quart", EaseOutQuartPoints },
            { "easeinout-quart", EaseInOutQuartPoints },

            { "easeinquint", EaseInQuintPoints },
            { "easeoutquint", EaseOutQuintPoints },
            { "easeinoutquint", EaseInOutQuintPoints },
            { "easein-quint", EaseInQuintPoints },
            { "easeout-quint", EaseOutQuintPoints },
            { "easeinout-quint", EaseInOutQuintPoints },

            { "easeinexpo", EaseInExpoPoints },
            { "easeoutexpo", EaseOutExpoPoints },
            { "easeinoutexpo", EaseInOutExpoPoints },
            { "easein-expo", EaseInExpoPoints },
            { "easeout-expo", EaseOutExpoPoints },
            { "easeinout-expo", EaseInOutExpoPoints },

            { "easeincirc", EaseInCircPoints },
            { "easeoutcirc", EaseOutCircPoints },
            { "easeinoutcirc", EaseInOutCircPoints },
            { "easein-circ", EaseInCircPoints },
            { "easeout-circ", EaseOutCircPoints },
            { "easeinout-circ", EaseInOutCircPoints },

            { "easeinback", EaseInBackPoints },
            { "easeoutback", EaseOutBackPoints },
            { "easeinoutback", EaseInOutBackPoints },
            { "easein-back", EaseInBackPoints },
            { "easeout-back", EaseOutBackPoints },
            { "easeinout-back", EaseInOutBackPoints },

            { "easeinelastic", EaseInElasticPoints },
            { "easeoutelastic", EaseOutElasticPoints },
            { "easeinoutelastic", EaseInOutElasticPoints },
            { "easein-elastic", EaseInElasticPoints },
            { "easeout-elastic", EaseOutElasticPoints },
            { "easeinout-elastic", EaseInOutElasticPoints },

            { "easeinbounce", EaseInBouncePoints },
            { "easeoutbounce", EaseOutBouncePoints },
            { "easeinoutbounce", EaseInOutBouncePoints },
            { "easein-bounce", EaseInBouncePoints },
            { "easeout-bounce", EaseOutBouncePoints },
            { "easeinout-bounce", EaseInOutBouncePoints },
        });

        internal static readonly ReadOnlyDictionary<string, Func<AnimationCurve>> NameConstructorMapping = new ReadOnlyDictionary<string, Func<AnimationCurve>>(new Dictionary<string, Func<AnimationCurve>>()
        {
            { "easeinsine", EaseInSine },
            { "easeoutsine", EaseOutSine },
            { "easeinoutsine", EaseInOutSine },
            { "easein-sine", EaseInSine },
            { "easeout-sine", EaseOutSine },
            { "easeinout-sine", EaseInOutSine },

            { "easeinquad", EaseInQuad },
            { "easeoutquad", EaseOutQuad },
            { "easeinoutquad", EaseInOutQuad },
            { "easein-quad", EaseInQuad },
            { "easeout-quad", EaseOutQuad },
            { "easeinout-quad", EaseInOutQuad },

            { "easeincubic", EaseInCubic },
            { "easeoutcubic", EaseOutCubic },
            { "easeinoutcubic", EaseInOutCubic },
            { "easein-cubic", EaseInCubic },
            { "easeout-cubic", EaseOutCubic },
            { "easeinout-cubic", EaseInOutCubic },

            { "easeinquart", EaseInQuart },
            { "easeoutquart", EaseOutQuart },
            { "easeinoutquart", EaseInOutQuart },
            { "easein-quart", EaseInQuart },
            { "easeout-quart", EaseOutQuart },
            { "easeinout-quart", EaseInOutQuart },

            { "easeinquint", EaseInQuint },
            { "easeoutquint", EaseOutQuint },
            { "easeinoutquint", EaseInOutQuint },
            { "easein-quint", EaseInQuint },
            { "easeout-quint", EaseOutQuint },
            { "easeinout-quint", EaseInOutQuint },

            { "easeinexpo", EaseInExpo },
            { "easeoutexpo", EaseOutExpo },
            { "easeinoutexpo", EaseInOutExpo },
            { "easein-expo", EaseInExpo },
            { "easeout-expo", EaseOutExpo },
            { "easeinout-expo", EaseInOutExpo },

            { "easeincirc", EaseInCirc },
            { "easeoutcirc", EaseOutCirc },
            { "easeinoutcirc", EaseInOutCirc },
            { "easein-circ", EaseInCirc },
            { "easeout-circ", EaseOutCirc },
            { "easeinout-circ", EaseInOutCirc },

            { "easeinback", EaseInBack },
            { "easeoutback", EaseOutBack },
            { "easeinoutback", EaseInOutBack },
            { "easein-back", EaseInBack },
            { "easeout-back", EaseOutBack },
            { "easeinout-back", EaseInOutBack },

            { "easeinelastic", EaseInElastic },
            { "easeoutelastic", EaseOutElastic },
            { "easeinoutelastic", EaseInOutElastic },
            { "easein-elastic", EaseInElastic },
            { "easeout-elastic", EaseOutElastic },
            { "easeinout-elastic", EaseInOutElastic },

            { "easeinbounce", EaseInBounce },
            { "easeoutbounce", EaseOutBounce },
            { "easeinoutbounce", EaseInOutBounce },
            { "easein-bounce", EaseInBounce },
            { "easeout-bounce", EaseOutBounce },
            { "easeinout-bounce", EaseInOutBounce },
        });

        internal static readonly ReadOnlyDictionary<string, Func<IEnumerable<Vector2>, AnimationCurve>> NameBezierConstructorMapping = new ReadOnlyDictionary<string, Func<IEnumerable<Vector2>, AnimationCurve>>(new Dictionary<string, Func<IEnumerable<Vector2>, AnimationCurve>>()
        {
            { "linear", LinearBezier },
            { "quadratic", QuadraticBezier },
            { "cubic", CubicBezier },

            { "linear-bezier", LinearBezier },
            { "quadratic-bezier", QuadraticBezier },
            { "cubic-bezier", CubicBezier },

            { "linearbezier", LinearBezier },
            { "quadraticbezier", QuadraticBezier },
            { "cubicbezier", CubicBezier },
        });
        #endregion
    }
}
