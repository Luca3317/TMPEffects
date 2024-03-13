using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Text.RegularExpressions;
using TMPEffects.TextProcessing;
using TMPEffects.TMPAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using UnityEngine.WSA;

namespace TMPEffects
{
    public static class ParameterUtility
    {
        public static bool ParameterDefined(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                GetDefinedParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryGetDefinedParameter(out string value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetDefinedParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        public static string GetDefinedParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string ret = null;
            if (parameters.ContainsKey(name)) ret = name;
            if (aliases == null)
            {
                if (ret != null) return ret;
                throw new System.Exception("Parameter " + name + " not defined");
            }

            for (int i = 0; i < aliases.Length; i++)
            {
                if (parameters.ContainsKey(aliases[i]))
                {
                    if (ret != null) throw new System.Exception("Parameter " + name + " or its aliases defined multiple times");
                    else ret = aliases[i];
                }
            }

            if (ret == null) throw new System.Exception("Parameter " + name + " not defined, nor any of its aliases");
            return ret;
        }


        public static bool HasNonFloatParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetFloatParameter(out float _, parameters, name, aliases);
        }

        public static bool HasFloatParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetFloatParameter(out float _, parameters, name, aliases);
        }

        public static bool TryGetFloatParameter(out float value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetFloatParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static float GetFloatParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToFloat(parameters[defined], out float value))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid float");
        }


        public static bool HasNonIntParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetIntParameter(out int _, parameters, name, aliases);
        }

        public static bool HasIntParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetIntParameter(out int _, parameters, name, aliases);
        }

        public static bool TryGetIntParameter(out int value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetIntParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static int GetIntParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToInt(parameters[defined], out int value))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid int");
        }


        public static bool HasNonBoolParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetBoolParameter(out bool _, parameters, name, aliases);
        }

        public static bool HasBoolParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetBoolParameter(out bool _, parameters, name, aliases);
        }

        public static bool TryGetBoolParameter(out bool value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetBoolParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static bool GetBoolParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToBool(parameters[defined], out bool value))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid bool");
        }


        public static bool HasNonVector2Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetVector2Parameter(out Vector2 _, parameters, name, aliases);
        }

        public static bool HasVector2Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetVector2Parameter(out Vector2 _, parameters, name, aliases);
        }

        public static bool TryGetVector2Parameter(out Vector2 value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetVector2Parameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static Vector2 GetVector2Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToVector2(parameters[defined], out Vector2 value, BuiltInVector2Keywords))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid Vector2");
        }


        public static bool HasNonVector3Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetVector3Parameter(out Vector3 _, parameters, name, aliases);
        }

        public static bool HasVector3Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetVector3Parameter(out Vector3 _, parameters, name, aliases);
        }

        public static bool TryGetVector3Parameter(out Vector3 value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetVector3Parameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static Vector3 GetVector3Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToVector3(parameters[defined], out Vector3 value, BuiltInVector3Keywords))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid Vector3");
        }


        // TODO Make v2
        public static bool HasNonAnchorParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetAnchorParameter(out Vector3 _, parameters, name, aliases);
        }

        public static bool HasAnchorParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetAnchorParameter(out Vector3 _, parameters, name, aliases);
        }

        public static bool TryGetAnchorParameter(out Vector3 value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetAnchorParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static Vector3 GetAnchorParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (AnchorKeywords.ContainsKey(parameters[defined])) return AnchorKeywords[parameters[defined]];

            if (ParsingUtility.StringToAnchor(parameters[defined], out Vector3 value, BuiltInVector3Keywords))
            {
                if (Mathf.Abs(value.x) > 1 || Mathf.Abs(value.y) > 1 || Mathf.Abs(value.z) > 1)
                { }
                else
                {
                    return value;
                }
            }

            throw new System.Exception("Parameter " + defined + " is not a valid Anchor");
        }


        public static bool HasNonOffsetParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetOffsetParameter(out Vector3 _, parameters, name, aliases);
        }

        public static bool HasOffsetParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetOffsetParameter(out Vector3 _, parameters, name, aliases);
        }

        public static bool TryGetOffsetParameter(out Vector3 value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetOffsetParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static Vector3 GetOffsetParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            string str = parameters[defined];

            if (str == null || str.Length < 2 || str[0] != 'o' || str[1] != ':')
                throw new System.Exception("Parameter " + defined + " is not a valid Offset");

            if (ParsingUtility.StringToVector3(str.Substring(2, str.Length - 1), out Vector3 value, BuiltInVector3Keywords))
            {
                if (Mathf.Abs(value.x) > 1 || Mathf.Abs(value.y) > 1 || Mathf.Abs(value.z) > 1)
                { }
                else
                {
                    return value;
                }
            }

            throw new System.Exception("Parameter " + defined + " is not a valid Offset");
        }

        public static readonly ReadOnlyDictionary<string, Vector3> AnchorKeywords = new ReadOnlyDictionary<string, Vector3>(new Dictionary<string, Vector3>()
        {
            { "a:top", Vector3.up },
            { "a:bottom", Vector3.down },
            { "a:bttm", Vector3.down },
            { "a:right", Vector3.right },
            { "a:left", Vector3.left },

            { "a:topright", Vector3.up + Vector3.right },
            { "a:tr",  Vector3.up + Vector3.right },

            { "a:bottomright", Vector3.down + Vector3.right },
            { "a:bttmright", Vector3.down + Vector3.right },
            { "a:br", Vector3.down + Vector3.right  },

            { "a:topleft", Vector3.up + Vector3.left },
            { "a:tl", Vector3.up + Vector3.left },

            { "a:bottomleft", Vector3.down + Vector3.left },
            { "a:bttmleft", Vector3.down + Vector3.left },
            { "a:bl", Vector3.down + Vector3.left },

            { "a:center", Vector3.zero }
        });


        public static bool HasNonAnimCurveParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetAnimCurveParameter(out AnimationCurve _, parameters, name, aliases);
        }

        public static bool HasAnimCurveParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetAnimCurveParameter(out AnimationCurve _, parameters, name, aliases);
        }

        public static bool TryGetAnimCurveParameter(out AnimationCurve value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetAnimationCurveParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static AnimationCurve GetAnimationCurveParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToAnimationCurve(parameters[defined], out AnimationCurve value))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid AnimationCurve");
        }



        public static bool HasNonWaveOffsetParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetWaveOffsetParameter(out AnimationUtility.WaveOffsetType _, parameters, name, aliases);
        }

        public static bool HasWaveOffsetParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetWaveOffsetParameter(out AnimationUtility.WaveOffsetType _, parameters, name, aliases);
        }

        public static bool TryGetWaveOffsetParameter(out AnimationUtility.WaveOffsetType value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetWaveOffsetParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static AnimationUtility.WaveOffsetType GetWaveOffsetParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToWaveOffsetType(parameters[defined], out AnimationUtility.WaveOffsetType value))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid WaveOffsetType");
        }




        // TODO Move both of these somewhere else
        public enum VectorType
        {
            Position,
            PositionOffset,
            Anchor
        }

        public struct WaveParameters
        {
            public AnimationCurve upwardCurve;
            public AnimationCurve downwardCurve;

            public float? upPeriod;
            public float? downPeriod;

            public float? crestWait;
            public float? troughWait;

            public float? wavevelocity;
            public float? wavelength;
            public float? waveuniformity;

            public float? amplitude;
        }

        /// <summary>
        /// Get all parameter relevant to a wave.<br/>
        /// Important: this reserves the following parameter names:
        /// <list type="bullet">
        /// <item>upcurve, upcrv, upc</item>
        /// <item>downcurve, downcrv, downc, dcrv, dc</item>
        /// <item>upperiod, uppd</item>
        /// <item>downperiod, downpd, dpd</item>
        /// <item>crestwait, crestw, cwait</item>
        /// <item>troughwait, troughw, twait</item>
        /// <item>wavevelocity, wavevlc, wvelocity, wvlc</item>
        /// <item>wavelength, wavelen, wlength, wlen</item>
        /// <item>waveuniformity, waveuni, wuniformity, wuni</item>
        /// <item>waveamplitude, wamplitude, waveamp, wamp</item>
        /// </list>
        /// <param name="parameters">The dictionary containing the parameters.</param>
        /// <param name="upwardCurve">Whether to get the upwardCurve parameter.</param>
        /// <param name="downwardCurve">Whether to get the downwardCurve parameter.</param>
        /// <param name="upPeriod">Whether to get the upPeriod parameter.</param>
        /// <param name="downPeriod">Whether to get the downPeriod parameter.</param>
        /// <param name="crestWait">Whether to get the crestWait parameter.</param>
        /// <param name="troughWait">Whether to get the troughWait parameter.</param>
        /// <param name="waveVelocity">Whether to get the waveVelocity parameter.</param>
        /// <param name="waveLength">Whether to get the waveLength parameter.</param>
        /// <param name="waveUniformity">Whether to get the waveUniformity parameter.</param>
        /// <param name="amplitude">Whether to get the amplitude parameter.</param>
        /// <returns>A <see cref="WaveParameters"/> object containing the parsed fields.</returns>
        /// <exception cref="System.Exception">If conflicting parameters are specified</exception>
        public static WaveParameters GetWaveParameters(IDictionary<string, string> parameters,
                bool upwardCurve = true,
                bool downwardCurve = true,
                bool upPeriod = true,
                bool downPeriod = true,
                bool crestWait = true,
                bool troughWait = true,
                bool waveVelocity = true,
                bool waveLength = true,
                bool waveUniformity = true,
                bool amplitude = true
            )
        {
            WaveParameters wp = new WaveParameters();

            wp.upwardCurve = null;
            wp.downwardCurve = null;
            wp.upPeriod = null;
            wp.downPeriod = null;
            wp.crestWait = null;
            wp.troughWait = null;
            wp.wavevelocity = null;
            wp.amplitude = null;

            if (waveVelocity && TryGetFloatParameter(out float f, parameters, "wavevelocity", "wavevlc", "wvelocity", "wvlc")) wp.wavevelocity = f;
            if (waveLength && TryGetFloatParameter(out f, parameters, "wavelength", "wavelen", "wlength", "wlen"))
            {
                if (wp.wavevelocity != null)
                    throw new System.Exception("Must define either wave velocity, wave length or uniformity; not multiple");

                wp.wavelength = f;
            }

            if (waveUniformity && TryGetFloatParameter(out f, parameters, "waveuniformity", "waveuni", "wuniformity", "wuni")) wp.waveuniformity = f;
            if (upwardCurve && TryGetAnimCurveParameter(out AnimationCurve crv, parameters, "upcurve", "upcrv", "upc")) wp.upwardCurve = crv;
            if (downwardCurve && TryGetAnimCurveParameter(out crv, parameters, "downcurve", "downcrv", "downc", "dcrv", "dc")) wp.downwardCurve = crv;
            if (upPeriod && TryGetFloatParameter(out f, parameters, "upperiod", "uppd")) wp.upPeriod = f;
            if (downPeriod && TryGetFloatParameter(out f, parameters, "downperiod", "downpd", "dpd")) wp.downPeriod = f;
            if (crestWait && TryGetFloatParameter(out f, parameters, "crestwait", "crestw", "cwait")) wp.crestWait = f;
            if (troughWait && TryGetFloatParameter(out f, parameters, "troughwait", "troughw", "twait")) wp.crestWait = f;
            if (amplitude && TryGetFloatParameter(out f, parameters, "waveamplitude", "wamplitude", "waveamp", "wamp")) wp.amplitude = f;

            return wp;
        }

        /// <summary>
        /// Validate all parameters relevant to a wave.<br/>
        /// Important: this reserves the following parameter names:
        /// <list type="bullet">
        /// <item>upcurve, upcrv, upc</item>
        /// <item>downcurve, downcrv, downc, dcrv, dc</item>
        /// <item>upperiod, uppd</item>
        /// <item>downperiod, downpd, dpd</item>
        /// <item>crestwait, crestw, cwait</item>
        /// <item>troughwait, troughw, twait</item>
        /// <item>wavevelocity, wavevlc, wvelocity, wvlc</item>
        /// <item>wavelength, wavelen, wlength, wlen</item>
        /// <item>waveuniformity, waveuni, wuniformity, wuni</item>
        /// <item>waveamplitude, wamplitude, waveamp, wamp</item>
        /// </list>
        /// <param name="parameters">The dictionary containing the parameters.</param>
        /// <param name="upwardCurve">Whether to validate the upwardCurve parameter.</param>
        /// <param name="downwardCurve">Whether to validate the downwardCurve parameter.</param>
        /// <param name="upPeriod">Whether to validate the upPeriod parameter.</param>
        /// <param name="downPeriod">Whether to validate the downPeriod parameter.</param>
        /// <param name="crestWait">Whether to validate the crestWait parameter.</param>
        /// <param name="troughWait">Whether to validate the troughWait parameter.</param>
        /// <param name="waveVelocity">Whether to validate the waveVelocity parameter.</param>
        /// <param name="waveLength">Whether to validate the waveLength parameter.</param>
        /// <param name="waveUniformity">Whether to validate the waveUniformity parameter.</param>
        /// <param name="amplitude">Whether to validate the amplitude parameter.</param>
        /// <returns>true if all specified fields were successfully validate; otherwise, false.</returns>
        public static bool ValidateWaveParameters(IDictionary<string, string> parameters,
                bool upwardCurve = true,
                bool downwardCurve = true,
                bool upPeriod = true,
                bool downPeriod = true,
                bool crestWait = true,
                bool troughWait = true,
                bool waveVelocity = true,
                bool waveLength = true,
                bool waveUniformity = true,
                bool amplitude = true
            )
        {
            bool contained = false;
            string defined;

            if (waveVelocity && TryGetDefinedParameter(out defined, parameters, "wavevelocity", "wavevlc", "wvelocity", "wvlc"))
            {
                if (HasNonFloatParameter(parameters, defined)) return false;
                contained = true;
            }

            if (waveLength && TryGetDefinedParameter(out defined, parameters, "wavelength", "wavelen", "wlength", "wlen"))
            {
                if (contained) return false;
                if (HasNonFloatParameter(parameters, defined)) return false;
            }

            if (waveUniformity && HasNonFloatParameter(parameters, "waveuniformity", "waveuni", "wuniformity", "wuni")) return false;
            if (upwardCurve && HasNonAnimCurveParameter(parameters, "upcurve", "upcrv", "upc")) return false;
            if (downwardCurve && HasNonAnimCurveParameter(parameters, "downcurve", "downcrv", "downc", "dcrv", "dc")) return false;
            if (upPeriod && HasNonFloatParameter(parameters, "upperiod", "uppd")) return false;            
            if (downPeriod && HasNonFloatParameter(parameters, "downperiod", "downpd", "dpd")) return false;
            if (crestWait && HasNonFloatParameter(parameters, "crestwait", "crestw", "cwait")) return false;
            if (troughWait && HasNonFloatParameter(parameters, "troughwait", "troughw", "twait")) return false;
            if (amplitude && HasNonFloatParameter(parameters, "waveamplitude", "wamplitude", "waveamp", "wamp")) return false;

            return true;
        }


        // Aliases for common parameters
        public static readonly string[] SpeedAliases = new string[] { "sp" };
        public static readonly string[] CurveAliases = new string[] { "crv" };
        public static readonly string[] FrequencyAliases = new string[] { "freq", "fq" };
        public static readonly string[] AmplitudeAliases = new string[] { "amp" };
        public static readonly string[] PivotAliases = new string[] { "pvt" };
        public static readonly string[] RadiusAliases = new string[] { "rad" };

        public static readonly ReadOnlyDictionary<string, float> BuiltInFloatKeywords = new ReadOnlyDictionary<string, float>(new Dictionary<string, float>()
        {
            { "e", (float)System.Math.E },
            { "pi", (float)Mathf.PI },
            { "epsilon", Mathf.Epsilon },
            { "phi", 1.61803f }
        });

        public static readonly ReadOnlyDictionary<string, Vector2> BuiltInVector2Keywords = new ReadOnlyDictionary<string, Vector2>(new Dictionary<string, Vector2>()
        {
            { "up", Vector2.up },
            { "down", Vector2.down },
            { "right", Vector2.right },
            { "left", Vector2.left },

            { "up right", Vector2.up + Vector2.right},
            { "up left", Vector2.up + Vector2.left},
            { "down right", Vector2.down + Vector2.right},
            { "down left", Vector2.down + Vector2.left},
            { "right up", Vector2.right + Vector2.up},
            { "right down", Vector2.right + Vector2.down},
            { "left up", Vector2.left + Vector2.up},
            { "left down", Vector2.left + Vector2.down},

            { "upright", Vector2.up + Vector2.right},
            { "upleft", Vector2.up + Vector2.left},
            { "downright", Vector2.down + Vector2.right},
            { "downleft", Vector2.down + Vector2.left},
            { "rightup", Vector2.right + Vector2.up},
            { "rightdown", Vector2.right + Vector2.down},
            { "leftup", Vector2.left + Vector2.up},
            { "leftdown", Vector2.left + Vector2.down},

            { "inf", Vector2.positiveInfinity },
            { "ninf", Vector2.negativeInfinity },
            { "-inf", Vector2.negativeInfinity },
            { "+inf", Vector2.negativeInfinity },

            { "zero", Vector2.zero }
        });

        public static readonly ReadOnlyDictionary<string, Vector3> BuiltInVector3Keywords = new ReadOnlyDictionary<string, Vector3>(new Dictionary<string, Vector3>()
        {
            { "up", Vector3.up},
            { "down", Vector3.down},
            { "right", Vector3.right},
            { "left", Vector3.left},
            { "forward", Vector3.forward},
            { "fwd", Vector3.forward},
            { "back", Vector3.back},

            { "inf", Vector3.positiveInfinity },
            { "ninf", Vector3.negativeInfinity },
            { "-inf", Vector3.negativeInfinity },
            { "+inf", Vector3.negativeInfinity },
            { "zero", Vector3.zero },

            { "up right", Vector3.up + Vector3.right},
            { "up right forward", Vector3.up + Vector3.right + Vector3.forward},
            { "up right fwd", Vector3.up + Vector3.right + Vector3.forward},
            { "up right back", Vector3.up + Vector3.right + Vector3.back},
            { "up left", Vector3.up + Vector3.left},
            { "up left forward", Vector3.up + Vector3.left + Vector3.forward},
            { "up left fwd", Vector3.up + Vector3.left + Vector3.forward},
            { "up left back", Vector3.up + Vector3.left + Vector3.back},
            { "down right", Vector3.down + Vector3.right},
            { "down right forward", Vector3.down + Vector3.right + Vector3.forward},
            { "down right fwd", Vector3.down + Vector3.right + Vector3.forward},
            { "down right back", Vector3.down + Vector3.right + Vector3.back},
            { "down left", Vector3.down + Vector3.left},
            { "down left forward", Vector3.down + Vector3.left + Vector3.forward},
            { "down left fwd", Vector3.down + Vector3.left + Vector3.forward},
            { "down left back", Vector3.down + Vector3.left + Vector3.back},
            { "up forward", Vector3.up + Vector3.forward},
            { "up forward right", Vector3.up + Vector3.forward + Vector3.right},
            { "up forward left", Vector3.up + Vector3.forward + Vector3.left},
            { "up fwd", Vector3.up + Vector3.forward},
            { "up fwd right", Vector3.up + Vector3.forward + Vector3.right},
            { "up fwd left", Vector3.up + Vector3.forward + Vector3.left},
            { "up back", Vector3.up + Vector3.back},
            { "up back right", Vector3.up + Vector3.back + Vector3.right},
            { "up back left", Vector3.up + Vector3.back + Vector3.left},
            { "down forward", Vector3.down + Vector3.forward},
            { "down forward right", Vector3.down + Vector3.forward + Vector3.right},
            { "down forward left", Vector3.down + Vector3.forward + Vector3.left},
            { "down fwd", Vector3.down + Vector3.forward},
            { "down fwd right", Vector3.down + Vector3.forward + Vector3.right},
            { "down fwd left", Vector3.down + Vector3.forward + Vector3.left},
            { "down back", Vector3.down + Vector3.back},
            { "down back right", Vector3.down + Vector3.back + Vector3.right},
            { "down back left", Vector3.down + Vector3.back + Vector3.left},
            { "right up", Vector3.right + Vector3.up},
            { "right up forward", Vector3.right + Vector3.up + Vector3.forward},
            { "right up fwd", Vector3.right + Vector3.up + Vector3.forward},
            { "right up back", Vector3.right + Vector3.up + Vector3.back},
            { "right down", Vector3.right + Vector3.down},
            { "right down forward", Vector3.right + Vector3.down + Vector3.forward},
            { "right down fwd", Vector3.right + Vector3.down + Vector3.forward},
            { "right down back", Vector3.right + Vector3.down + Vector3.back},
            { "left up", Vector3.left + Vector3.up},
            { "left up forward", Vector3.left + Vector3.up + Vector3.forward},
            { "left up fwd", Vector3.left + Vector3.up + Vector3.forward},
            { "left up back", Vector3.left + Vector3.up + Vector3.back},
            { "left down", Vector3.left + Vector3.down},
            { "left down forward", Vector3.left + Vector3.down + Vector3.forward},
            { "left down fwd", Vector3.left + Vector3.down + Vector3.forward},
            { "left down back", Vector3.left + Vector3.down + Vector3.back},
            { "right forward", Vector3.right + Vector3.forward},
            { "right forward up", Vector3.right + Vector3.forward + Vector3.up},
            { "right forward down", Vector3.right + Vector3.forward + Vector3.down},
            { "right fwd", Vector3.right + Vector3.forward},
            { "right fwd up", Vector3.right + Vector3.forward + Vector3.up},
            { "right fwd down", Vector3.right + Vector3.forward + Vector3.down},
            { "right back", Vector3.right + Vector3.back},
            { "right back up", Vector3.right + Vector3.back + Vector3.up},
            { "right back down", Vector3.right + Vector3.back + Vector3.down},
            { "left forward", Vector3.left + Vector3.forward},
            { "left forward up", Vector3.left + Vector3.forward + Vector3.up},
            { "left forward down", Vector3.left + Vector3.forward + Vector3.down},
            { "left fwd", Vector3.left + Vector3.forward},
            { "left fwd up", Vector3.left + Vector3.forward + Vector3.up},
            { "left fwd down", Vector3.left + Vector3.forward + Vector3.down},
            { "left back", Vector3.left + Vector3.back},
            { "left back up", Vector3.left + Vector3.back + Vector3.up},
            { "left back down", Vector3.left + Vector3.back + Vector3.down},
            { "forward right", Vector3.forward + Vector3.right},
            { "forward right up", Vector3.forward + Vector3.right + Vector3.up},
            { "forward right down", Vector3.forward + Vector3.right + Vector3.down},
            { "forward left", Vector3.forward + Vector3.left},
            { "forward left up", Vector3.forward + Vector3.left + Vector3.up},
            { "forward left down", Vector3.forward + Vector3.left + Vector3.down},
            { "fwd right", Vector3.forward + Vector3.right},
            { "fwd right up", Vector3.forward + Vector3.right + Vector3.up},
            { "fwd right down", Vector3.forward + Vector3.right + Vector3.down},
            { "fwd left", Vector3.forward + Vector3.left},
            { "fwd left up", Vector3.forward + Vector3.left + Vector3.up},
            { "fwd left down", Vector3.forward + Vector3.left + Vector3.down},
            { "back right", Vector3.back + Vector3.right},
            { "back right up", Vector3.back + Vector3.right + Vector3.up},
            { "back right down", Vector3.back + Vector3.right + Vector3.down},
            { "back left", Vector3.back + Vector3.left},
            { "back left up", Vector3.back + Vector3.left + Vector3.up},
            { "back left down", Vector3.back + Vector3.left + Vector3.down},
            { "forward up", Vector3.forward + Vector3.up},
            { "forward up right", Vector3.forward + Vector3.up + Vector3.right},
            { "forward up left", Vector3.forward + Vector3.up + Vector3.left},
            { "forward down", Vector3.forward + Vector3.down},
            { "forward down right", Vector3.forward + Vector3.down + Vector3.right},
            { "forward down left", Vector3.forward + Vector3.down + Vector3.left},
            { "fwd up", Vector3.forward + Vector3.up},
            { "fwd up right", Vector3.forward + Vector3.up + Vector3.right},
            { "fwd up left", Vector3.forward + Vector3.up + Vector3.left},
            { "fwd down", Vector3.forward + Vector3.down},
            { "fwd down right", Vector3.forward + Vector3.down + Vector3.right},
            { "fwd down left", Vector3.forward + Vector3.down + Vector3.left},
            { "back up", Vector3.back + Vector3.up},
            { "back up right", Vector3.back + Vector3.up + Vector3.right},
            { "back up left", Vector3.back + Vector3.up + Vector3.left},
            { "back down", Vector3.back + Vector3.down},
            { "back down right", Vector3.back + Vector3.down + Vector3.right},
            { "back down left", Vector3.back + Vector3.down + Vector3.left},

            { "upright", Vector3.up + Vector3.right},
            { "uprightforward", Vector3.up + Vector3.right + Vector3.forward},
            { "uprightfwd", Vector3.up + Vector3.right + Vector3.forward},
            { "uprightback", Vector3.up + Vector3.right + Vector3.back},
            { "upleft", Vector3.up + Vector3.left},
            { "upleftforward", Vector3.up + Vector3.left + Vector3.forward},
            { "upleftfwd", Vector3.up + Vector3.left + Vector3.forward},
            { "upleftback", Vector3.up + Vector3.left + Vector3.back},
            { "downright", Vector3.down + Vector3.right},
            { "downrightforward", Vector3.down + Vector3.right + Vector3.forward},
            { "downrightfwd", Vector3.down + Vector3.right + Vector3.forward},
            { "downrightback", Vector3.down + Vector3.right + Vector3.back},
            { "downleft", Vector3.down + Vector3.left},
            { "downleftforward", Vector3.down + Vector3.left + Vector3.forward},
            { "downleftfwd", Vector3.down + Vector3.left + Vector3.forward},
            { "downleftback", Vector3.down + Vector3.left + Vector3.back},
            { "upforward", Vector3.up + Vector3.forward},
            { "upforwardright", Vector3.up + Vector3.forward + Vector3.right},
            { "upforwardleft", Vector3.up + Vector3.forward + Vector3.left},
            { "upfwd", Vector3.up + Vector3.forward},
            { "upfwdright", Vector3.up + Vector3.forward + Vector3.right},
            { "upfwdleft", Vector3.up + Vector3.forward + Vector3.left},
            { "upback", Vector3.up + Vector3.back},
            { "upbackright", Vector3.up + Vector3.back + Vector3.right},
            { "upbackleft", Vector3.up + Vector3.back + Vector3.left},
            { "downforward", Vector3.down + Vector3.forward},
            { "downforwardright", Vector3.down + Vector3.forward + Vector3.right},
            { "downforwardleft", Vector3.down + Vector3.forward + Vector3.left},
            { "downfwd", Vector3.down + Vector3.forward},
            { "downfwdright", Vector3.down + Vector3.forward + Vector3.right},
            { "downfwdleft", Vector3.down + Vector3.forward + Vector3.left},
            { "downback", Vector3.down + Vector3.back},
            { "downbackright", Vector3.down + Vector3.back + Vector3.right},
            { "downbackleft", Vector3.down + Vector3.back + Vector3.left},
            { "rightup", Vector3.right + Vector3.up},
            { "rightupforward", Vector3.right + Vector3.up + Vector3.forward},
            { "rightupfwd", Vector3.right + Vector3.up + Vector3.forward},
            { "rightupback", Vector3.right + Vector3.up + Vector3.back},
            { "rightdown", Vector3.right + Vector3.down},
            { "rightdownforward", Vector3.right + Vector3.down + Vector3.forward},
            { "rightdownfwd", Vector3.right + Vector3.down + Vector3.forward},
            { "rightdownback", Vector3.right + Vector3.down + Vector3.back},
            { "leftup", Vector3.left + Vector3.up},
            { "leftupforward", Vector3.left + Vector3.up + Vector3.forward},
            { "leftupfwd", Vector3.left + Vector3.up + Vector3.forward},
            { "leftupback", Vector3.left + Vector3.up + Vector3.back},
            { "leftdown", Vector3.left + Vector3.down},
            { "leftdownforward", Vector3.left + Vector3.down + Vector3.forward},
            { "leftdownfwd", Vector3.left + Vector3.down + Vector3.forward},
            { "leftdownback", Vector3.left + Vector3.down + Vector3.back},
            { "rightforward", Vector3.right + Vector3.forward},
            { "rightforwardup", Vector3.right + Vector3.forward + Vector3.up},
            { "rightforwarddown", Vector3.right + Vector3.forward + Vector3.down},
            { "rightfwd", Vector3.right + Vector3.forward},
            { "rightfwdup", Vector3.right + Vector3.forward + Vector3.up},
            { "rightfwddown", Vector3.right + Vector3.forward + Vector3.down},
            { "rightback", Vector3.right + Vector3.back},
            { "rightbackup", Vector3.right + Vector3.back + Vector3.up},
            { "rightbackdown", Vector3.right + Vector3.back + Vector3.down},
            { "leftforward", Vector3.left + Vector3.forward},
            { "leftforwardup", Vector3.left + Vector3.forward + Vector3.up},
            { "leftforwarddown", Vector3.left + Vector3.forward + Vector3.down},
            { "leftfwd", Vector3.left + Vector3.forward},
            { "leftfwdup", Vector3.left + Vector3.forward + Vector3.up},
            { "leftfwddown", Vector3.left + Vector3.forward + Vector3.down},
            { "leftback", Vector3.left + Vector3.back},
            { "leftbackup", Vector3.left + Vector3.back + Vector3.up},
            { "leftbackdown", Vector3.left + Vector3.back + Vector3.down},
            { "forwardright", Vector3.forward + Vector3.right},
            { "forwardrightup", Vector3.forward + Vector3.right + Vector3.up},
            { "forwardrightdown", Vector3.forward + Vector3.right + Vector3.down},
            { "forwardleft", Vector3.forward + Vector3.left},
            { "forwardleftup", Vector3.forward + Vector3.left + Vector3.up},
            { "forwardleftdown", Vector3.forward + Vector3.left + Vector3.down},
            { "fwdright", Vector3.forward + Vector3.right},
            { "fwdrightup", Vector3.forward + Vector3.right + Vector3.up},
            { "fwdrightdown", Vector3.forward + Vector3.right + Vector3.down},
            { "fwdleft", Vector3.forward + Vector3.left},
            { "fwdleftup", Vector3.forward + Vector3.left + Vector3.up},
            { "fwdleftdown", Vector3.forward + Vector3.left + Vector3.down},
            { "backright", Vector3.back + Vector3.right},
            { "backrightup", Vector3.back + Vector3.right + Vector3.up},
            { "backrightdown", Vector3.back + Vector3.right + Vector3.down},
            { "backleft", Vector3.back + Vector3.left},
            { "backleftup", Vector3.back + Vector3.left + Vector3.up},
            { "backleftdown", Vector3.back + Vector3.left + Vector3.down},
            { "forwardup", Vector3.forward + Vector3.up},
            { "forwardupright", Vector3.forward + Vector3.up + Vector3.right},
            { "forwardupleft", Vector3.forward + Vector3.up + Vector3.left},
            { "forwarddown", Vector3.forward + Vector3.down},
            { "forwarddownright", Vector3.forward + Vector3.down + Vector3.right},
            { "forwarddownleft", Vector3.forward + Vector3.down + Vector3.left},
            { "fwdup", Vector3.forward + Vector3.up},
            { "fwdupright", Vector3.forward + Vector3.up + Vector3.right},
            { "fwdupleft", Vector3.forward + Vector3.up + Vector3.left},
            { "fwddown", Vector3.forward + Vector3.down},
            { "fwddownright", Vector3.forward + Vector3.down + Vector3.right},
            { "fwddownleft", Vector3.forward + Vector3.down + Vector3.left},
            { "backup", Vector3.back + Vector3.up},
            { "backupright", Vector3.back + Vector3.up + Vector3.right},
            { "backupleft", Vector3.back + Vector3.up + Vector3.left},
            { "backdown", Vector3.back + Vector3.down},
            { "backdownright", Vector3.back + Vector3.down + Vector3.right},
            { "backdownleft", Vector3.back + Vector3.down + Vector3.left},
        });
    }
}