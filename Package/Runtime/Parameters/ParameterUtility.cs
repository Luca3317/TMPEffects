using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.ParameterUtilityGenerator.Attributes;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.Parameters.ParameterTypes;

namespace TMPEffects.Parameters
{
    /// <summary>
    /// Utility class for easy parameter handling.
    /// </summary>
    [GenerateParameterUtility] 
    public static partial class ParameterUtility
    { 
        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases.<br />
        /// A parameter is well-defined if there is exactly one of the given aliases (including the name) present in the parameters.
        /// </summary>
        /// <param name="value">Set to the name of the defined parameter if successful.</param>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if the parameter is well-defined, false otherwise.</returns>
        public static bool TryGetDefinedParameter(out string value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            value = null;
            if (parameters.ContainsKey(name)) value = name;
            if (aliases == null)
            {
                return value != null;
            }

            for (int i = 0; i < aliases.Length; i++)
            {
                if (parameters.ContainsKey(aliases[i])) 
                {
                    if (value != null) return false;
                    else value = aliases[i];
                }
            }

            return value != null;
        }

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases.<br />
        /// A parameter is well-defined if there is exactly one of the given aliases (including the name) present in the parameters.
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if the parameter is well-defined, false otherwise.</returns>
        public static bool ParameterDefined(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetDefinedParameter(out _, parameters, name, aliases);
        }
        
        public static readonly ReadOnlyDictionary<string, Vector2> AnchorKeywords = new ReadOnlyDictionary<string, Vector2>(new Dictionary<string, Vector2>()
        {
            { "a:top", Vector2.up },
            { "a:bottom", Vector2.down },
            { "a:bttm", Vector2.down },
            { "a:right", Vector2.right },
            { "a:left", Vector2.left },

            { "a:topright", Vector2.up + Vector2.right },
            { "a:tr",  Vector2.up + Vector2.right },

            { "a:bottomright", Vector2.down + Vector2.right },
            { "a:bttmright", Vector2.down + Vector2.right },
            { "a:br", Vector2.down + Vector2.right  },

            { "a:topleft", Vector2.up + Vector2.left },
            { "a:tl", Vector2.up + Vector2.left },

            { "a:bottomleft", Vector2.down + Vector2.left },
            { "a:bttmleft", Vector2.down + Vector2.left },
            { "a:bl", Vector2.down + Vector2.left },

            { "a:center", Vector2.zero }
        });

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is not of type Array&lt;T&gt; (=> can not be converted to Array&lt;T&gt;).
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is not of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool HasNonArrayParameter<T>(IDictionary<string, string> parameters, ParseDelegate<string, T, IDictionary<string, T>, bool> func, string name, params string[] aliases)
        {
            return HasNonArrayParameter(parameters, func, null, name, aliases);
        }
        
        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is not of type Array&lt;T&gt; (=> can not be converted to Array&lt;T&gt;).
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is not of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool HasNonArrayParameter<T>(IDictionary<string, string> parameters, ParseDelegate<string, T, IDictionary<string, T>, bool> func, IDictionary<string, T> keywords, string name, params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetArrayParameter(out T[] _, parameters, func, keywords, name, aliases);
        }
        
        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type Array<T> (=> can no be converted to Array<T>).
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type Array<T>, false otherwise.</returns>
        public static bool HasArrayParameter<T>(IDictionary<string, string> parameters, ParseDelegate<string, T, IDictionary<string, T>, bool> func, string name, params string[] aliases)
        {
            return HasArrayParameter(parameters, func, null, name, aliases);
        }
        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type Array<T> (=> can no be converted to Array<T>).
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type Array<T>, false otherwise.</returns>
        public static bool HasArrayParameter<T>(IDictionary<string, string> parameters, ParseDelegate<string, T, IDictionary<string, T>, bool> func, IDictionary<string, T> keywords, string name, params string[] aliases)
        {
            return TryGetArrayParameter(out T[] _, parameters, func, name, aliases);
        }
        
        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type Array<T> (=> can be converted to Array<T>).<br />
        /// </summary>
        /// <param name="value">Set to the value of the parameter if successful.</param>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type Array<T>, false otherwise.</returns>
        public static bool TryGetArrayParameter<T>(out T[] value, IDictionary<string, string> parameters, ParseDelegate<string, T, IDictionary<string, T>, bool> func, string name, params string[] aliases)
        {
            return TryGetArrayParameter(out value, parameters, func, null, name, aliases);
        }

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type Array<T> (=> can be converted to Array<T>).<br />
        /// </summary>
        /// <param name="value">Set to the value of the parameter if successful.</param>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type Array<T>, false otherwise.</returns>
        public static bool TryGetArrayParameter<T>(out T[] value, IDictionary<string, string> parameters, ParseDelegate<string, T, IDictionary<string, T>, bool> func, IDictionary<string, T> keywords, string name, params string[] aliases)
        {
            value = null;
            if (!TryGetDefinedParameter(out string parameterName, parameters, name, aliases)) return false;

            string[] contents = parameters[parameterName].Split(";");

            List<T> result = new List<T>();

            for (int i = 0; i < contents.Length; i++)
            {
                string contentValue = contents[i];

                if (!func(contentValue, out T t, keywords))
                {
                    return false;
                }

                result.Add(t);
            }

            value = result.ToArray();
            return true;
        }
        
        public delegate W ParseDelegate<T, U, V, W>(T input, out U output, V keywords);

        /// <summary>
        /// A parameter bundle that defines a <see cref="Wave"/>.
        /// </summary>
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
        public static WaveParameters GetWaveParameters(IDictionary<string, string> parameters, string prefix = "",
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

            if (waveVelocity && TryGetFloatParameter(out float f, parameters, prefix + "velocity", prefix + "vlc")) wp.wavevelocity = f;
            if (waveLength && TryGetFloatParameter(out f, parameters, prefix + "wavelength", prefix + "wavelen", prefix + "wlength", prefix + "wlen"))
            {
                if (wp.wavevelocity != null)
                    throw new System.Exception("Must define either wave velocity, wave length or uniformity; not multiple");

                wp.wavelength = f;
            }

            if (waveUniformity && TryGetFloatParameter(out f, parameters, prefix + "uniformity", prefix + "uni")) wp.waveuniformity = f;
            if (upwardCurve && TryGetAnimCurveParameter(out AnimationCurve crv, parameters, prefix + "upcurve", prefix + "upcrv", prefix + "up")) wp.upwardCurve = crv;
            if (downwardCurve && TryGetAnimCurveParameter(out crv, parameters, prefix + "downcurve", prefix + "downcrv", prefix + "down", prefix + "dn")) wp.downwardCurve = crv;
            if (upPeriod && TryGetFloatParameter(out f, parameters, prefix + "upperiod", prefix + "uppd")) wp.upPeriod = f;
            if (downPeriod && TryGetFloatParameter(out f, parameters, prefix + "downperiod", prefix + "downpd", prefix + "dnpd")) wp.downPeriod = f;
            if (crestWait && TryGetFloatParameter(out f, parameters, prefix + "crestwait", prefix + "crestw", prefix + "cwait", prefix + "cw")) wp.crestWait = f;
            if (troughWait && TryGetFloatParameter(out f, parameters, prefix + "troughwait", prefix + "troughw", prefix + "twait", prefix + "tw")) wp.troughWait = f;
            if (amplitude && TryGetFloatParameter(out f, parameters, prefix + "amplitude", prefix + "amp")) wp.amplitude = f;

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
        public static bool ValidateWaveParameters(IDictionary<string, string> parameters, string prefix = "",
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

            if (waveVelocity && TryGetDefinedParameter(out defined, parameters, prefix + "velocity", prefix + "vlc"))
            {
                if (HasNonFloatParameter(parameters, defined)) return false;
                contained = true;
            }

            if (waveLength && TryGetDefinedParameter(out defined, parameters, prefix + "wavelength", prefix + "wavelen", prefix + "wlength", prefix + "wlen"))
            {
                if (contained) return false;
                if (HasNonFloatParameter(parameters, defined)) return false;
            }

            if (waveUniformity && HasNonFloatParameter(parameters, prefix + "uniformity", prefix + "uni")) return false;
            if (upwardCurve && HasNonAnimCurveParameter(parameters, prefix + "upcurve", prefix + "upcrv", prefix + "up")) return false;
            if (downwardCurve && HasNonAnimCurveParameter(parameters, prefix + "downcurve", prefix + "downcrv", prefix + "down", prefix + "dn")) return false;
            if (upPeriod && HasNonFloatParameter(parameters, prefix + "upperiod", prefix + "uppd")) return false;
            if (downPeriod && HasNonFloatParameter(parameters, prefix + "downperiod", prefix + "downpd", prefix + "dnpd")) return false;
            if (crestWait && HasNonFloatParameter(parameters, prefix + "crestwait", prefix + "crestw", prefix + "cwait", prefix + "cw")) return false;
            if (troughWait && HasNonFloatParameter(parameters, prefix + "troughwait", prefix + "troughw", prefix + "twait", prefix + "tw")) return false;
            if (amplitude && HasNonFloatParameter(parameters, prefix + "amplitude", prefix + "amp")) return false;

            return true;
        }


        /// <summary>
        /// Create a new <see cref="Wave"/> using the passed in one as a template, and replacing any of its properties that are defined in the passed in <see cref="WaveParameters"/>.<br/>
        /// This is not in-place. The passed in <see cref="Wave"> will not be modified.
        /// </summary>
        /// <param name="wave"></param>
        /// <param name="wp"></param>
        /// <returns>A new <see cref="Wave"/>, that combines the properties of the passed in objects.</returns>
        public static Wave CreateWave(Wave wave, ParameterUtility.WaveParameters wp)
        {
            float upPeriod = wp.upPeriod == null ? wave.UpPeriod : wp.upPeriod.Value;
            float downPeriod = wp.downPeriod == null ? wave.DownPeriod : wp.downPeriod.Value;

            Wave newWave = new Wave
            (
                wp.upwardCurve == null ? wave.UpwardCurve : wp.upwardCurve,
                wp.downwardCurve == null ? wave.DownwardCurve : wp.downwardCurve,
                upPeriod,
                downPeriod,
                wp.amplitude == null ? wave.Amplitude : wp.amplitude.Value,
                wp.crestWait == null ? wave.CrestWait : wp.crestWait.Value,
                wp.troughWait == null ? wave.TroughWait : wp.troughWait.Value,
                wp.waveuniformity == null ? wave.Uniformity : wp.waveuniformity.Value
            );

            return newWave;
        }


        // TODO Either make usage of these consistent or delete totally
        // Aliases for common parameters
        public static readonly string[] WaveOffsetAliases = new string[] { "woffset", "waveoff", "woff" };
        public static readonly string[] SpeedAliases = new string[] { "sp", "s" };
        public static readonly string[] CurveAliases = new string[] { "crv" };
        public static readonly string[] FrequencyAliases = new string[] { "freq", "fq" };
        public static readonly string[] AmplitudeAliases = new string[] { "amp" };
        public static readonly string[] PivotAliases = new string[] { "pvt" };
        public static readonly string[] RadiusAliases = new string[] { "rad" };

        /// <summary>
        /// A variety of built-in keywords for float parameters.
        /// </summary>
        public static readonly ReadOnlyDictionary<string, float> BuiltInFloatKeywords = new ReadOnlyDictionary<string, float>(new Dictionary<string, float>()
        {
            { "e", (float)System.Math.E },
            { "pi", (float)Mathf.PI },
            { "epsilon", Mathf.Epsilon },
            { "phi", 1.61803f }
        });

        /// <summary>
        /// A variety of built-in keywords for Vector2 parameters.
        /// </summary>
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

        /// <summary>
        /// A variety of built-in keywords for Vector3 parameters.
        /// </summary>
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

    public class GenerateParameterUtilityAttribute : Attribute
    {
    }
}