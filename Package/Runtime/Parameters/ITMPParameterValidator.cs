using System.Collections.Generic;
using TMPEffects.Databases;

namespace TMPEffects.Parameters
{
    /// <summary>
    /// Validates tag parameters.
    /// </summary>
    public interface ITMPParameterValidator
    {
        /// <summary>
        /// Validate the given parameters.
        /// </summary>
        /// <param name="parameters">The parameters as key-value pairs</param>
        /// <param name="keywordDatabase">The keyword database used for parsing the parameter values</param>
        /// <returns>true if the parameters were successfully validated; false otherwise.</returns>
        public bool ValidateParameters(IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase);
    }
}