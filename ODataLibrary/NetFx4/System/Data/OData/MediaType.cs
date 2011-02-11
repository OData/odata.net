//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Class representing a media type definition.
    /// </summary>
    [DebuggerDisplay("MediaType [{type}/{subType}]")]
    internal sealed class MediaType
    {
        /// <summary>The default quality value (in the normalized range from 0 .. 1000).</summary>
        private const int DefaultQualityValue = 1000;

        /// <summary>Parameters specified on the media type.</summary>
        private readonly IList<KeyValuePair<string, string>> parameters;

        /// <summary>Sub-type specification (for example, 'plain').</summary>
        private readonly string subType;

        /// <summary>Type specification (for example, 'text').</summary>
        private readonly string type;

        /// <summary>
        /// Initializes a new <see cref="MediaType"/> read-only instance.
        /// </summary>
        /// <param name="type">Type specification (for example, 'text').</param>
        /// <param name="subType">Sub-type specification (for example, 'plain').</param>
        internal MediaType(string type, string subType)
            : this(type, subType, (IList<KeyValuePair<string, string>>)null)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Initializes a new <see cref="MediaType"/> read-only instance.
        /// </summary>
        /// <param name="type">Type specification (for example, 'text').</param>
        /// <param name="subType">Sub-type specification (for example, 'plain').</param>
        /// <param name="parameter">A single parameter specified on the media type.</param>
        internal MediaType(string type, string subType, KeyValuePair<string, string> parameter)
            : this(type, subType, new KeyValuePair<string, string>[] { parameter })
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Initializes a new <see cref="MediaType"/> read-only instance.
        /// </summary>
        /// <param name="type">Type specification (for example, 'text').</param>
        /// <param name="subType">Sub-type specification (for example, 'plain').</param>
        /// <param name="parameters">Parameters specified on the media type.</param>
        internal MediaType(string type, string subType, IList<KeyValuePair<string, string>> parameters)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(type != null, "type != null");
            Debug.Assert(subType != null, "subType != null");

            this.type = type;
            this.subType = subType;
            this.parameters = parameters == null ? null : parameters;
        }

        /// <summary>Encoding to fall back to an appropriate encoding is not available.</summary>
        internal static Encoding FallbackEncoding
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return MediaTypeUtils.EncodingUtf8NoPreamble;
            }
        }

        /// <summary>Encoding implied by an unspecified encoding value.</summary>
        /// <remarks>See http://tools.ietf.org/html/rfc2616#section-3.4.1 for details.</remarks>
        internal static Encoding MissingEncoding
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
#if SILVERLIGHT || WINDOWS_PHONE   // ISO-8859-1 not available
                return Encoding.UTF8;
#else
                return Encoding.GetEncoding("ISO-8859-1", new EncoderExceptionFallback(), new DecoderExceptionFallback());
#endif
            }
        }

        /// <summary>Returns the media type in standard type/subtype form, without parameters.</summary>
        internal string TypeName
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.type + "/" + this.subType;
            }
        }

        /// <summary>media type parameters</summary>
        internal IList<KeyValuePair<string, string>> Parameters
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.parameters;
            }
        }

        /// <summary>
        /// Selects the encoding appropriate for this media type specification
        /// (possibly null).
        /// </summary>
        /// <returns>
        /// The encoding explicitly defined on the media type specification, or
        /// the default encoding for well-known media types.
        /// </returns>
        /// <remarks>
        /// As per http://tools.ietf.org/html/rfc2616#section-3.7, the type, 
        /// subtype and parameter name attributes are case-insensitive.
        /// </remarks>
        internal Encoding SelectEncoding()
        {
            DebugUtils.CheckNoExternalCallers();
            if (this.parameters != null)
            {
                foreach (KeyValuePair<string, string> parameter in this.parameters)
                {
                    if (String.Equals(parameter.Key, HttpConstants.Charset, StringComparison.OrdinalIgnoreCase))
                    {
                        string encodingName = parameter.Value.Trim();
                        if (encodingName.Length > 0)
                        {
                            return MediaType.EncodingFromName(encodingName);
                        }
                    }
                }
            }

            // Select the default encoding for this media type.
            if (String.Equals(this.type, MimeConstants.MimeTextType, StringComparison.OrdinalIgnoreCase))
            {
                // HTTP 3.7.1 Canonicalization and Text Defaults
                // "text" subtypes default to ISO-8859-1
                //
                // Unless the subtype is XML, in which case we should default
                // to us-ascii. Instead we return null, to let the encoding
                // in the <?xml ...?> PI win (http://tools.ietf.org/html/rfc3023#section-3.1)
                if (String.Equals(this.subType, MimeConstants.MimeXmlSubType, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                else
                {
                    return MissingEncoding;
                }
            }
            else if (String.Equals(this.type, MimeConstants.MimeApplicationType, StringComparison.OrdinalIgnoreCase) &&
                     String.Equals(this.subType, MimeConstants.MimeJsonSubType, StringComparison.OrdinalIgnoreCase))
            {
                // http://tools.ietf.org/html/rfc4627#section-3
                // The default encoding is UTF-8.
                return FallbackEncoding;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a number of non-* matching type name parts and parameters; returns the number of matching type name parts or -1 if not matching at all.
        /// The <paramref name="matchingParameters"/> represent the number of matched parameters or -1 if there are no parameters to match and the candidate 
        /// type also has no parameters.
        /// </summary>
        /// <param name="candidate">Candidate media type to match.</param>
        /// <param name="matchingParameters">The number of matched parameters or -1 if there are no parameters to match and the <paramref name="candidate"/> does not have parameters either.</param>
        /// <param name="qualityValue">The quality value of the candidate (or -1 if none is specified).</param>
        /// <param name="parameterCount">The number of parameters of this type that are not accept-parameters (and thus ignored).</param>
        /// <returns>Returns the number of matching type name parts or -1 if not matching at all. The <paramref name="matchingParameters"/> 
        /// represent the number of matched parameters or -1 if there are no parameters to match and the candidate 
        /// type also has no parameters.
        /// </returns>
        internal int GetMatchingParts(MediaType candidate, out int matchingParameters, out int qualityValue, out int parameterCount)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(candidate != null, "candidate must not be null.");

            int matchedNameParts = -1;
            if (this.type == "*")
            {
                matchedNameParts = 0;
            }
            else
            {
                if (HttpUtils.CompareMediaTypeNames(this.type, candidate.type))
                {
                    if (this.subType == "*")
                    {
                        // only type matches
                        matchedNameParts = 1;
                    }
                    else if (HttpUtils.CompareMediaTypeNames(this.subType, candidate.subType))
                    {
                        // both type and subtype match
                        matchedNameParts = 2;
                    }
                }
            }

            qualityValue = DefaultQualityValue;
            parameterCount = 0;
            matchingParameters = 0;

            // NOTE: we know that the candidates don't have accept parameters so we can rely
            //       on the total parameter count.
            IList<KeyValuePair<string, string>> candidateParameters = candidate.Parameters;
            bool candidateHasParams = candidateParameters != null && candidateParameters.Count > 0;
            bool thisHasParams = this.parameters != null && this.parameters.Count > 0;
            if (thisHasParams)
            {
                for (int i = 0; i < this.parameters.Count; ++i)
                {
                    string parameterName = this.parameters[i].Key;
                    if (IsQualityValueParameter(parameterName))
                    {
                        // once we hit the q-value in the parameters we know that only accept-params will follow
                        // that don't contribute to the matching. Parse the quality value but don't continue processing
                        // parameters.
                        qualityValue = ParseQualityValue(this.parameters[i].Value.Trim());
                        break;
                    }

                    parameterCount = i + 1;

                    if (candidateHasParams)
                    {
                        // find the current parameter name in the set of parameters of the candidate and compare the value;
                        // if they match increase the result count
                        string parameterValue;
                        if (TryFindMediaTypeParameter(candidateParameters, parameterName, out parameterValue) &&
                            string.Compare(this.parameters[i].Value.Trim(), parameterValue.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            matchingParameters++;
                        }
                    }
                }
            }

            // if we have no non-accept parameters special rules apply
            if (!thisHasParams || parameterCount == 0)
            {
                if (candidateHasParams)
                {
                    if (matchedNameParts == 0 || matchedNameParts == 1)
                    {
                        // this is a media range specification using the '*' wildcard.
                        // we assume that all parameters are matched for such ranges so the server
                        // will pick the most specific one
                        matchingParameters = candidateParameters.Count;
                    }
                    else
                    {
                        // the candidate type has parameters while this type has not;
                        // return 0 to indicate that none of the candidate's parameters match.
                        matchingParameters = 0;
                    }
                }
                else
                {
                    // neither this type nor the candidate have parameters; in this case we return -1 to indicate
                    // that the candidate parameters are a perfect.
                    matchingParameters = -1;
                }
            }

            return matchedNameParts;
        }

        /// <summary>Selects a quality value for the specified type.</summary>
        /// <param name="qualityValueText">The text representation of the quality value.</param>
        /// <returns>The quality value, in range from 0 through 1000.</returns>
        /// <remarks>See http://tools.ietf.org/html/rfc2616#section-14.1 for further details.</remarks>
        private static int ParseQualityValue(string qualityValueText)
        {
            int qualityValue = DefaultQualityValue;
            if (qualityValueText.Length > 0)
            {
                int textIndex = 0;
                HttpUtils.ReadQualityValue(qualityValueText, ref textIndex, out qualityValue);
            }

            return qualityValue;
        }

        /// <summary>
        /// Tries to find a parameter with the specified <paramref name="parameterName"/> in the given list <paramref name="candidateParameters"/> of parameters.
        /// Does not include accept parameters (i.e., parameters after the q quality value parameter)
        /// </summary>
        /// <param name="candidateParameters">The list of parameters to search.</param>
        /// <param name="parameterName">The name of the parameter to find.</param>
        /// <param name="parameterValue">The parameter value of the parameter with the specified <paramref name="parameterName"/>.</param>
        /// <returns>True if a parameter with the specified <paramref name="parameterName"/> was found; otherwise false.</returns>
        private static bool TryFindMediaTypeParameter(IList<KeyValuePair<string, string>> candidateParameters, string parameterName, out string parameterValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "!string.IsNullOrEmpty(parameterName)");

            parameterValue = null;

            if (candidateParameters != null)
            {
                for (int i = 0; i < candidateParameters.Count; ++i)
                {
                    string candidateParameterName = candidateParameters[i].Key;

                    // we currently use this method to match acceptable parameters against the parameters of the types we support; none of our
                    // supported types have a quality value
                    Debug.Assert(!IsQualityValueParameter(candidateParameterName), "!IsQualityValueParamter(candidateParameters[i]");

                    if (string.Compare(parameterName, candidateParameterName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        parameterValue = candidateParameters[i].Value;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a flag indicating whether a given media type parameter name is the Http quality value parameter.
        /// </summary>
        /// <param name="parameterName">The parameter name to check.</param>
        /// <returns>True if the parameter name is for the quality value; otherwise false.</returns>
        private static bool IsQualityValueParameter(string parameterName)
        {
            return string.Compare(parameterName, HttpConstants.HttpQValueParameter, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>Gets the named encoding if specified.</summary>
        /// <param name="name">Name (possibly null or empty).</param>
        /// <returns>
        /// The named encoding if specified; the encoding for HTTP missing 
        /// charset specification otherwise.
        /// </returns>
        /// <remarks>
        /// See http://tools.ietf.org/html/rfc2616#section-3.4.1 for details.
        /// </remarks>
        private static Encoding EncodingFromName(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "!string.IsNullOrEmpty(name)");
            Debug.Assert(name.Trim() == name, "Should already be trimmed.");

            Encoding result = HttpUtils.GetEncodingFromCharsetName(name);
            if (result == null)
            {
                throw new ODataException(Strings.MediaType_EncodingNotSupported(name));
            }

            return result;
        }
    }
}
