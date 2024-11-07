using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.Databases;
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
        public static bool TryGetDefinedParameter(out string value, IDictionary<string, string> parameters, string name,
            params string[] aliases)
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
        public static bool ParameterDefined(IDictionary<string, string> parameters, string name,
            params string[] aliases)
        {
            return TryGetDefinedParameter(out _, parameters, name, aliases);
        }

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is not of type Array&lt;T&gt; (=> can not be converted to Array&lt;T&gt;).
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is not of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool HasNonArrayParameter<T>(IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, string name, params string[] aliases)
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
        public static bool HasNonArrayParameter<T>(IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, ITMPKeywordDatabase keywords, string name,
            params string[] aliases)
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
        public static bool HasArrayParameter<T>(IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, string name, params string[] aliases)
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
        public static bool HasArrayParameter<T>(IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, ITMPKeywordDatabase keywords, string name,
            params string[] aliases)
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
        public static bool TryGetArrayParameter<T>(out T[] value, IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, string name, params string[] aliases)
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
        public static bool TryGetArrayParameter<T>(out T[] value, IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, ITMPKeywordDatabase keywords, string name,
            params string[] aliases)
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
        public static WaveParameters GetWaveParameters(IDictionary<string, string> parameters,
            ITMPKeywordDatabase keywords = null,
            string prefix = "",
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

            if (waveVelocity &&
                TryGetFloatParameter(out float f, parameters, keywords, prefix + "velocity", prefix + "vlc"))
                wp.wavevelocity = f;
            if (waveLength && TryGetFloatParameter(out f, parameters, keywords, prefix + "wavelength",
                    prefix + "wavelen",
                    prefix + "wlength", prefix + "wlen"))
            {
                if (wp.wavevelocity != null)
                    throw new System.Exception(
                        "Must define either wave velocity, wave length or uniformity; not multiple");

                wp.wavelength = f;
            }

            if (waveUniformity &&
                TryGetFloatParameter(out f, parameters, keywords, prefix + "uniformity", prefix + "uni"))
                wp.waveuniformity = f;
            if (upwardCurve && TryGetAnimCurveParameter(out AnimationCurve crv, parameters, keywords,
                    prefix + "upcurve",
                    prefix + "upcrv", prefix + "up")) wp.upwardCurve = crv;
            if (downwardCurve && TryGetAnimCurveParameter(out crv, parameters, keywords, prefix + "downcurve",
                    prefix + "downcrv",
                    prefix + "down", prefix + "dn")) wp.downwardCurve = crv;
            if (upPeriod && TryGetFloatParameter(out f, parameters, keywords, prefix + "upperiod", prefix + "uppd"))
                wp.upPeriod = f;
            if (downPeriod && TryGetFloatParameter(out f, parameters, keywords, prefix + "downperiod",
                    prefix + "downpd",
                    prefix + "dnpd")) wp.downPeriod = f;
            if (crestWait && TryGetFloatParameter(out f, parameters, keywords, prefix + "crestwait", prefix + "crestw",
                    prefix + "cwait", prefix + "cw")) wp.crestWait = f;
            if (troughWait && TryGetFloatParameter(out f, parameters, keywords, prefix + "troughwait",
                    prefix + "troughw",
                    prefix + "twait", prefix + "tw")) wp.troughWait = f;
            if (amplitude && TryGetFloatParameter(out f, parameters, keywords, prefix + "amplitude", prefix + "amp"))
                wp.amplitude = f;

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
            ITMPKeywordDatabase keywords = null,
            string prefix = "",
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
                if (HasNonFloatParameter(parameters, keywords, defined)) return false;
                contained = true;
            }

            if (waveLength && TryGetDefinedParameter(out defined, parameters, prefix + "wavelength", prefix + "wavelen",
                    prefix + "wlength", prefix + "wlen"))
            {
                if (contained) return false;
                if (HasNonFloatParameter(parameters, keywords, defined)) return false;
            }

            if (waveUniformity && HasNonFloatParameter(parameters, keywords, prefix + "uniformity", prefix + "uni"))
                return false;
            if (upwardCurve &&
                HasNonAnimCurveParameter(parameters, keywords, prefix + "upcurve", prefix + "upcrv", prefix + "up"))
                return false;
            if (downwardCurve && HasNonAnimCurveParameter(parameters, keywords, prefix + "downcurve",
                    prefix + "downcrv",
                    prefix + "down", prefix + "dn")) return false;
            if (upPeriod && HasNonFloatParameter(parameters, keywords, prefix + "upperiod", prefix + "uppd"))
                return false;
            if (downPeriod &&
                HasNonFloatParameter(parameters, keywords, prefix + "downperiod", prefix + "downpd", prefix + "dnpd"))
                return false;
            if (crestWait && HasNonFloatParameter(parameters, keywords, prefix + "crestwait", prefix + "crestw",
                    prefix + "cwait",
                    prefix + "cw")) return false;
            if (troughWait && HasNonFloatParameter(parameters, keywords, prefix + "troughwait", prefix + "troughw",
                    prefix + "twait", prefix + "tw")) return false;
            if (amplitude && HasNonFloatParameter(parameters, keywords, prefix + "amplitude", prefix + "amp"))
                return false;

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
    }

    public class GenerateParameterUtilityAttribute : Attribute
    {
    }
}