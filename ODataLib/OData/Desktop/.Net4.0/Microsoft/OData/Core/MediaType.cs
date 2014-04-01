//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Class representing a media type definition.
    /// </summary>
    [DebuggerDisplay("MediaType [{ToText()}]")]
    internal sealed class MediaType
    {
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
            : this(type, subType, null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="MediaType"/> read-only instance.
        /// </summary>
        /// <param name="type">Type specification (for example, 'text').</param>
        /// <param name="subType">Sub-type specification (for example, 'plain').</param>
        /// <param name="parameters">The parameters specified on the media type.</param>
        internal MediaType(string type, string subType, params KeyValuePair<string, string>[] parameters)
            : this(type, subType, (IList<KeyValuePair<string, string>>)parameters)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="MediaType"/> read-only instance.
        /// </summary>
        /// <param name="type">Type specification (for example, 'text').</param>
        /// <param name="subType">Sub-type specification (for example, 'plain').</param>
        /// <param name="parameters">Parameters specified on the media type.</param>
        internal MediaType(string type, string subType, IList<KeyValuePair<string, string>> parameters)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(subType != null, "subType != null");

            this.type = type;
            this.subType = subType;
            this.parameters = parameters;
        }

        /// <summary>Encoding to fall back to an appropriate encoding is not available.</summary>
        internal static Encoding FallbackEncoding
        {
            get
            {
                return MediaTypeUtils.EncodingUtf8NoPreamble;
            }
        }

        /// <summary>Encoding implied by an unspecified encoding value.</summary>
        /// <remarks>See http://tools.ietf.org/html/rfc2616#section-3.4.1 for details.</remarks>
        internal static Encoding MissingEncoding
        {
            get
            {
#if !ORCAS
                return Encoding.UTF8;
#else
                return Encoding.GetEncoding("ISO-8859-1", new EncoderExceptionFallback(), new DecoderExceptionFallback());
#endif
            }
        }

        /// <summary>Returns the full media type in standard type/subtype form, without parameters.</summary>
        internal string FullTypeName
        {
            get
            {
                return this.type + "/" + this.subType;
            }
        }

        /// <summary>Returns the subtype part of the media type.</summary>
        internal string SubTypeName
        {
            get
            {
                return this.subType;
            }
        }

        /// <summary>Returns the type part of the media type.</summary>
        internal string TypeName
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>media type parameters</summary>
        internal IList<KeyValuePair<string, string>> Parameters
        {
            get
            {
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
            if (this.parameters != null)
            {
                foreach (string encodingName in
                    this.parameters.Where(parameter => 
                        HttpUtils.CompareMediaTypeParameterNames(ODataConstants.Charset, parameter.Key))
                        .Select(parameter => parameter.Value.Trim())
                        .Where(encodingName => encodingName.Length > 0))
                {
                    return EncodingFromName(encodingName);
                }
            }

            // Select the default encoding for this media type.
            if (HttpUtils.CompareMediaTypeNames(MimeConstants.MimeTextType, this.type))
            {
                // HTTP 3.7.1 Canonicalization and Text Defaults
                // "text" subtypes default to ISO-8859-1
                //
                // Unless the subtype is XML, in which case we should default
                // to us-ascii. Instead we return null, to let the encoding
                // in the <?xml ...?> PI win (http://tools.ietf.org/html/rfc3023#section-3.1)
                return HttpUtils.CompareMediaTypeNames(MimeConstants.MimeXmlSubType, this.subType) ? null : MissingEncoding;
            }

            if (HttpUtils.CompareMediaTypeNames(MimeConstants.MimeApplicationType, this.type) &&
                HttpUtils.CompareMediaTypeNames(MimeConstants.MimeJsonSubType, this.subType))
            {
                // http://tools.ietf.org/html/rfc4627#section-3
                // The default encoding is UTF-8.
                return FallbackEncoding;
            }

            return null;
        }

        /// <summary>
        /// Converts the current <see cref="MediaType"/> to a string representation suitable for use in a content-type header.
        /// </summary>
        /// <returns>The string representation of media type.</returns>
        internal string ToText()
        {
            return ToText(null);
        }

        /// <summary>
        /// Converts the current <see cref="MediaType"/> to a string representation suitable for use in a content-type header.
        /// </summary>
        /// <param name="encoding">The encoding to use when converting the media type into text.</param>
        /// <returns>The string representation of the current media type.</returns>
        internal string ToText(Encoding encoding)
        {
            // TODO: for now we include all the parameters since we know that we will not have accept parameters (after the quality value)
            //       that needed to be ignored.
            if (this.parameters == null || this.parameters.Count == 0)
            {
                string typeName = this.FullTypeName;
                if (encoding != null)
                {
                    typeName = string.Concat(typeName, ";", ODataConstants.Charset, "=", encoding.WebName);
                }

                return typeName;
            }

            StringBuilder builder = new StringBuilder(this.FullTypeName);
            foreach (KeyValuePair<string, string> parameter in this.parameters)
            {
                // ignore the char set if specified in the parameters; we write the one from the encoding
                if (HttpUtils.CompareMediaTypeParameterNames(ODataConstants.Charset, parameter.Key))
                {
                    continue;
                }

                builder.Append(";");
                builder.Append(parameter.Key);
                builder.Append("=");
                builder.Append(parameter.Value);
            }

            // write the encoding (if any)
            if (encoding != null)
            {
                builder.Append(";");
                builder.Append(ODataConstants.Charset);
                builder.Append("=");
                builder.Append(encoding.WebName);
            }

            return builder.ToString();
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
