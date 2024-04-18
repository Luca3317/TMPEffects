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
        #region Linear
        public static AnimationCurve Linear()
        {
            return LinearBezier(new(0, 0), new(1, 1));
        }
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

        public static AnimationCurve GetInverse(AnimationCurve originalCurve)
        {
            AnimationCurve curve = new AnimationCurve();

            for (int i = originalCurve.keys.Length - 1; i >= 0; i--)
            {
                Keyframe keyFrame = originalCurve.keys[i];

                keyFrame.time = Mathf.Lerp(1, 0, keyFrame.time);
                float tmp = keyFrame.inWeight;
                keyFrame.inWeight = keyFrame.outWeight;
                keyFrame.outWeight = tmp;
                keyFrame.inTangent *= -1;
                keyFrame.outTangent *= -1;

                curve.AddKey(keyFrame);
            }

            return curve;
        }

        public static AnimationCurve Invert(AnimationCurve curve)
        {
            List<Keyframe> frames = new List<Keyframe>(curve.keys);

            int len = curve.keys.Length;
            for (int i = len - 1; i >= 0; i--)
            {
                curve.RemoveKey(i);
            }

            for (int i = frames.Count - 1; i >= 0; i--)
            {
                Keyframe keyFrame = frames[i];

                keyFrame.time = Mathf.Lerp(1, 0, keyFrame.time);
                float tmp = keyFrame.inWeight;
                keyFrame.inWeight = keyFrame.outWeight;
                keyFrame.outWeight = tmp;
                keyFrame.inTangent *= -1;
                keyFrame.outTangent *= -1;

                curve.AddKey(keyFrame);
            }

            return curve;
        }

        #region Mappings
        public static readonly ReadOnlyDictionary<string, ReadOnlyCollection<Vector2>> NamePointsMapping = new ReadOnlyDictionary<string, ReadOnlyCollection<Vector2>>(new Dictionary<string, ReadOnlyCollection<Vector2>>()
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

        public static readonly ReadOnlyDictionary<string, Func<AnimationCurve>> NameConstructorMapping = new ReadOnlyDictionary<string, Func<AnimationCurve>>(new Dictionary<string, Func<AnimationCurve>>()
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

            { "linear", Linear}
        });

        public static readonly ReadOnlyDictionary<string, Func<IEnumerable<Vector2>, AnimationCurve>> NameBezierConstructorMapping = new ReadOnlyDictionary<string, Func<IEnumerable<Vector2>, AnimationCurve>>(new Dictionary<string, Func<IEnumerable<Vector2>, AnimationCurve>>()
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

        #region Curve Fitting
        /// <summary>
        /// To make cubic bezier spline by data.
        /// </summary>
        internal static class CubicBezierFitter
        {
            private const int MAX_DATA_COUNT = 100000;
            private const float MIN_ERROR = 1e-06f;
            public static List<Vector2> FitCurve(Vector2[] d, float error)
            {
                Vector2 tHat1, tHat2;    /*  Unit tangent vectors at endVector2s */

                tHat1 = ComputeLeftTangent(d, 0);
                tHat2 = ComputeRightTangent(d, d.Length - 1);
                List<Vector2> result = new List<Vector2>() {
                new Vector2(0, 0) // The first cp is always (0, 0)
            };
                FitCubic(d, 0, d.Length - 1, tHat1, tHat2, error, result);
                return result;
            }


            private static void FitCubic(Vector2[] d, int first, int last, Vector2 tHat1, Vector2 tHat2, float error, List<Vector2> result)
            {
                Vector2[] bezCurve; /*Control Vector2s of fitted Bezier curve*/
                float[] u;     /*  Parameter values for Vector2  */
                float[] uPrime;    /*  Improved parameter values */
                float maxError;    /*  Maximum fitting error    */
                int splitVector2; /*  Vector2 to split Vector2 set at  */
                int nPts;       /*  Number of Vector2s in subset  */
                float iterationError; /*Error below which you try iterating  */
                int maxIterations = 4; /*  Max times to try iterating  */
                Vector2 tHatCenter;      /* Unit tangent vector at splitVector2 */
                int i;

                error = Mathf.Max(error, MIN_ERROR);
                iterationError = error * error;
                nPts = last - first + 1;

                /*  Use heuristic if region only has two Vector2s in it */
                if (nPts == 2)
                {
                    float dist = (d[first] - d[last]).magnitude / 3.0f;

                    bezCurve = new Vector2[4];
                    bezCurve[0] = d[first];
                    bezCurve[3] = d[last];
                    bezCurve[1] = (tHat1 * dist) + bezCurve[0];
                    bezCurve[2] = (tHat2 * dist) + bezCurve[3];

                    result.Add(bezCurve[1]);
                    result.Add(bezCurve[2]);
                    result.Add(bezCurve[3]);
                    return;
                }

                /*  Parameterize Vector2s, and attempt to fit curve */
                u = ChordLengthParameterize(d, first, last);
                bezCurve = GenerateBezier(d, first, last, u, tHat1, tHat2);

                /*  Find max deviation of Vector2s to fitted curve */
                maxError = ComputeMaxError(d, first, last, bezCurve, u, out splitVector2);
                if (maxError < error)
                {
                    result.Add(bezCurve[1]);
                    result.Add(bezCurve[2]);
                    result.Add(bezCurve[3]);
                    return;
                }


                /*  If error not too large, try some reparameterization  */
                /*  and iteration */
                if (maxError < iterationError)
                {
                    for (i = 0; i < maxIterations; i++)
                    {
                        uPrime = Reparameterize(d, first, last, u, bezCurve);
                        bezCurve = GenerateBezier(d, first, last, uPrime, tHat1, tHat2);
                        maxError = ComputeMaxError(d, first, last,
                                   bezCurve, uPrime, out splitVector2);
                        if (maxError < error)
                        {
                            result.Add(bezCurve[1]);
                            result.Add(bezCurve[2]);
                            result.Add(bezCurve[3]);
                            return;
                        }
                        u = uPrime;
                    }
                }

                /* Fitting failed -- split at max error Vector2 and fit recursively */
                tHatCenter = ComputeCenterTangent(d, splitVector2);
                FitCubic(d, first, splitVector2, tHat1, tHatCenter, error, result);
                tHatCenter = -tHatCenter;
                FitCubic(d, splitVector2, last, tHatCenter, tHat2, error, result);
            }

            static Vector2[] GenerateBezier(Vector2[] d, int first, int last, float[] uPrime, Vector2 tHat1, Vector2 tHat2)
            {
                int i;
                Vector2[,] A = new Vector2[MAX_DATA_COUNT, 2];/* Precomputed rhs for eqn    */

                int nPts;           /* Number of pts in sub-curve */
                float[,] C = new float[2, 2];            /* Matrix C     */
                float[] X = new float[2];          /* Matrix X         */
                float det_C0_C1,      /* Determinants of matrices */
                    det_C0_X,
                    det_X_C1;
                float alpha_l,        /* Alpha values, left and right */
                    alpha_r;
                Vector2 tmp;            /* Utility variable     */
                Vector2[] bezCurve = new Vector2[4];    /* RETURN bezier curve ctl pts  */
                nPts = last - first + 1;

                /* Compute the A's  */
                for (i = 0; i < nPts; i++)
                {
                    Vector2 v1, v2;
                    v1 = tHat1;
                    v2 = tHat2;
                    v1 *= B1(uPrime[i]);
                    v2 *= B2(uPrime[i]);
                    A[i, 0] = v1;
                    A[i, 1] = v2;
                }

                /* Create the C and X matrices  */
                C[0, 0] = 0.0f;
                C[0, 1] = 0.0f;
                C[1, 0] = 0.0f;
                C[1, 1] = 0.0f;
                X[0] = 0.0f;
                X[1] = 0.0f;

                for (i = 0; i < nPts; i++)
                {
                    C[0, 0] += V2Dot(A[i, 0], A[i, 0]);
                    C[0, 1] += V2Dot(A[i, 0], A[i, 1]);
                    /*                  C[1][0] += V2Dot(&A[i][0], &A[i][9]);*/
                    C[1, 0] = C[0, 1];
                    C[1, 1] += V2Dot(A[i, 1], A[i, 1]);

                    tmp = ((Vector2)d[first + i] -
                        (
                          ((Vector2)d[first] * B0(uPrime[i])) +
                            (
                                ((Vector2)d[first] * B1(uPrime[i])) +
                                        (
                                        ((Vector2)d[last] * B2(uPrime[i])) +
                                            ((Vector2)d[last] * B3(uPrime[i]))))));


                    X[0] += V2Dot(A[i, 0], tmp);
                    X[1] += V2Dot(A[i, 1], tmp);
                }

                /* Compute the determinants of C and X  */
                det_C0_C1 = C[0, 0] * C[1, 1] - C[1, 0] * C[0, 1];
                det_C0_X = C[0, 0] * X[1] - C[1, 0] * X[0];
                det_X_C1 = X[0] * C[1, 1] - X[1] * C[0, 1];

                /* Finally, derive alpha values */
                alpha_l = (det_C0_C1 == 0) ? 0.0f : det_X_C1 / det_C0_C1;
                alpha_r = (det_C0_C1 == 0) ? 0.0f : det_C0_X / det_C0_C1;

                /* If alpha negative, use the Wu/Barsky heuristic (see text) */
                /* (if alpha is 0, you get coincident control Vector2s that lead to
                 * divide by zero in any subsequent NewtonRaphsonRootFind() call. */
                float segLength = (d[first] - d[last]).magnitude;
                float epsilon = 1.0e-6f * segLength;
                if (alpha_l < epsilon || alpha_r < epsilon)
                {
                    /* fall back on standard (probably inaccurate) formula, and subdivide further if needed. */
                    float dist = segLength / 3.0f;
                    bezCurve[0] = d[first];
                    bezCurve[3] = d[last];
                    bezCurve[1] = (tHat1 * dist) + bezCurve[0];
                    bezCurve[2] = (tHat2 * dist) + bezCurve[3];
                    return (bezCurve);
                }

                /*  First and last control Vector2s of the Bezier curve are */
                /*  positioned exactly at the first and last data Vector2s */
                /*  Control Vector2s 1 and 2 are positioned an alpha distance out */
                /*  on the tangent vectors, left and right, respectively */
                bezCurve[0] = d[first];
                bezCurve[3] = d[last];
                bezCurve[1] = (tHat1 * alpha_l) + bezCurve[0];
                bezCurve[2] = (tHat2 * alpha_r) + bezCurve[3];
                return (bezCurve);
            }

            /*
             *  Reparameterize:
             *  Given set of Vector2s and their parameterization, try to find
             *   a better parameterization.
             *
             */
            static float[] Reparameterize(Vector2[] d, int first, int last, float[] u, Vector2[] bezCurve)
            {
                int nPts = last - first + 1;
                int i;
                float[] uPrime = new float[nPts];      /*  New parameter values    */

                for (i = first; i <= last; i++)
                {
                    uPrime[i - first] = NewtonRaphsonRootFind(bezCurve, d[i], u[i - first]);
                }
                return uPrime;
            }



            /*
             *  NewtonRaphsonRootFind :
             *  Use Newton-Raphson iteration to find better root.
             */
            static float NewtonRaphsonRootFind(Vector2[] Q, Vector2 P, float u)
            {
                float numerator, denominator;
                Vector2[] Q1 = new Vector2[3], Q2 = new Vector2[2];   /*  Q' and Q''          */
                Vector2 Q_u, Q1_u, Q2_u; /*u evaluated at Q, Q', & Q''  */
                float uPrime;     /*  Improved u          */
                int i;

                /* Compute Q(u) */
                Q_u = BezierII(3, Q, u);

                /* Generate control vertices for Q' */
                for (i = 0; i <= 2; i++)
                {
                    Q1[i].x = (Q[i + 1].x - Q[i].x) * 3.0f;
                    Q1[i].y = (Q[i + 1].y - Q[i].y) * 3.0f;
                }

                /* Generate control vertices for Q'' */
                for (i = 0; i <= 1; i++)
                {
                    Q2[i].x = (Q1[i + 1].x - Q1[i].x) * 2.0f;
                    Q2[i].y = (Q1[i + 1].y - Q1[i].y) * 2.0f;
                }

                /* Compute Q'(u) and Q''(u) */
                Q1_u = BezierII(2, Q1, u);
                Q2_u = BezierII(1, Q2, u);

                /* Compute f(u)/f'(u) */
                numerator = (Q_u.x - P.x) * (Q1_u.x) + (Q_u.y - P.y) * (Q1_u.y);
                denominator = (Q1_u.x) * (Q1_u.x) + (Q1_u.y) * (Q1_u.y) +
                              (Q_u.x - P.x) * (Q2_u.x) + (Q_u.y - P.y) * (Q2_u.y);
                if (denominator == 0.0f) return u;

                /* u = u - f(u)/f'(u) */
                uPrime = u - (numerator / denominator);
                return (uPrime);
            }



            /*
             *  Bezier :
             *      Evaluate a Bezier curve at a particular parameter value
             * 
             */
            static Vector2 BezierII(int degree, Vector2[] V, float t)
            {
                int i, j;
                Vector2 Q;          /* Vector2 on curve at parameter t    */
                Vector2[] Vtemp;      /* Local copy of control Vector2s     */

                /* Copy array   */
                Vtemp = new Vector2[degree + 1];
                for (i = 0; i <= degree; i++)
                {
                    Vtemp[i] = V[i];
                }

                /* Triangle computation */
                for (i = 1; i <= degree; i++)
                {
                    for (j = 0; j <= degree - i; j++)
                    {
                        Vtemp[j].x = (1.0f - t) * Vtemp[j].x + t * Vtemp[j + 1].x;
                        Vtemp[j].y = (1.0f - t) * Vtemp[j].y + t * Vtemp[j + 1].y;
                    }
                }

                Q = Vtemp[0];
                return Q;
            }


            /*
             *  B0, B1, B2, B3 :
             *  Bezier multipliers
             */
            static float B0(float u)
            {
                float tmp = 1.0f - u;
                return (tmp * tmp * tmp);
            }


            static float B1(float u)
            {
                float tmp = 1.0f - u;
                return (3 * u * (tmp * tmp));
            }

            static float B2(float u)
            {
                float tmp = 1.0f - u;
                return (3 * u * u * tmp);
            }

            static float B3(float u)
            {
                return (u * u * u);
            }

            /*
             * ComputeLeftTangent, ComputeRightTangent, ComputeCenterTangent :
             *Approximate unit tangents at endVector2s and "center" of digitized curve
             */
            static Vector2 ComputeLeftTangent(Vector2[] d, int end)
            {
                Vector2 tHat1;
                tHat1 = d[end + 1] - d[end];
                tHat1.Normalize();
                return tHat1;
            }

            static Vector2 ComputeRightTangent(Vector2[] d, int end)
            {
                Vector2 tHat2;
                tHat2 = d[end - 1] - d[end];
                tHat2.Normalize();
                return tHat2;
            }

            static Vector2 ComputeCenterTangent(Vector2[] d, int center)
            {
                Vector2 V1, V2, tHatCenter = new Vector2();

                V1 = d[center - 1] - d[center];
                V2 = d[center] - d[center + 1];
                tHatCenter.x = (V1.x + V2.x) / 2.0f;
                tHatCenter.y = (V1.y + V2.y) / 2.0f;
                tHatCenter.Normalize();
                return tHatCenter;
            }


            /*
             *  ChordLengthParameterize :
             *  Assign parameter values to digitized Vector2s 
             *  using relative distances between Vector2s.
             */
            static float[] ChordLengthParameterize(Vector2[] d, int first, int last)
            {
                int i;
                float[] u = new float[last - first + 1];           /*  Parameterization        */

                u[0] = 0.0f;
                for (i = first + 1; i <= last; i++)
                {
                    u[i - first] = u[i - first - 1] + (d[i - 1] - d[i]).magnitude;
                }

                for (i = first + 1; i <= last; i++)
                {
                    u[i - first] = u[i - first] / u[last - first];
                }

                return u;
            }




            /*
             *  ComputeMaxError :
             *  Find the maximum squared distance of digitized Vector2s
             *  to fitted curve.
            */
            static float ComputeMaxError(Vector2[] d, int first, int last, Vector2[] bezCurve, float[] u, out int splitVector2)
            {
                int i;
                float maxDist;        /*  Maximum error       */
                float dist;       /*  Current error       */
                Vector2 P;          /*  Vector2 on curve      */
                Vector2 v;          /*  Vector2 from Vector2 to curve  */

                splitVector2 = (last - first + 1) / 2;
                maxDist = 0.0f;
                for (i = first + 1; i < last; i++)
                {
                    P = BezierII(3, bezCurve, u[i - first]);
                    v = P - d[i];
                    dist = v.sqrMagnitude;
                    if (dist >= maxDist)
                    {
                        maxDist = dist;
                        splitVector2 = i;
                    }
                }
                return maxDist;
            }

            private static float V2Dot(Vector2 a, Vector2 b)
            {
                return ((a.x * b.x) + (a.y * b.y));
            }

        }
        #endregion
    }
}
