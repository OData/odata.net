//---------------------------------------------------------------------
// <copyright file="ODataMediaType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Class representing a media type definition.
    /// </summary>
    [DebuggerDisplay("MediaType [{ToText()}]")]
    public sealed class ODataMediaType
    {
        /// <summary>Parameters specified on the media type.</summary>
        private readonly IEnumerable<KeyValuePair<string, string>> parameters;

        /// <summary>Sub-type specification (for example, 'plain').</summary>
        private readonly string subType;

        /// <summary>Type specification (for example, 'text').</summary>
        private readonly string type;

        /// <summary>
        /// Initializes a new <see cref="ODataMediaType"/> read-only instance.
        /// </summary>
        /// <param name="type">Type specification (for example, 'text').</param>
        /// <param name="subType">Sub-type specification (for example, 'plain').</param>
        public ODataMediaType(string type, string subType)
            : this(type, subType, null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ODataMediaType"/> read-only instance.
        /// </summary>
        /// <param name="type">Type specification (for example, 'text').</param>
        /// <param name="subType">Sub-type specification (for example, 'plain').</param>
        /// <param name="parameters">Parameters specified on the media type.</param>
        public ODataMediaType(string type, string subType, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(subType != null, "subType != null");

            this.type = type;
            this.subType = subType;
            this.parameters = parameters;
        }

        /// <summary>
        /// Initializes a new <see cref="ODataMediaType"/> read-only instance.
        /// </summary>
        /// <param name="type">Type specification (for example, 'text').</param>
        /// <param name="subType">Sub-type specification (for example, 'plain').</param>
        /// <param name="parameter">The parameter specified on the media type.</param>
        internal ODataMediaType(string type, string subType, KeyValuePair<string, string> parameter)
            : this(type, subType, new[] { parameter })
        {
        }

        /// <summary>Returns the subtype part of the media type.</summary>
        public string SubType
        {
            get
            {
                return this.subType;
            }
        }

        /// <summary>Returns the type part of the media type.</summary>
        public string Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>media type parameters</summary>
        public IEnumerable<KeyValuePair<string, string>> Parameters
        {
            get
            {
                return this.parameters;
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
                return HttpUtils.CompareMediaTypeNames(MimeConstants.MimeXmlSubType, this.subType) ? null : MediaTypeUtils.MissingEncoding;
            }

            if (HttpUtils.CompareMediaTypeNames(MimeConstants.MimeApplicationType, this.type) &&
                HttpUtils.CompareMediaTypeNames(MimeConstants.MimeJsonSubType, this.subType))
            {
                // http://tools.ietf.org/html/rfc4627#section-3
                // The default encoding is UTF-8.
                return MediaTypeUtils.FallbackEncoding;
            }

            return null;
        }

        /// <summary>
        /// Converts the current <see cref="ODataMediaType"/> to a string representation suitable for use in a content-type header.
        /// </summary>
        /// <returns>The string representation of media type.</returns>
        internal string ToText()
        {
            return ToText(null);
        }

        /// <summary>
        /// Converts the current <see cref="ODataMediaType"/> to a string representation suitable for use in a content-type header.
        /// </summary>
        /// <param name="encoding">The encoding to use when converting the media type into text.</param>
        /// <returns>The string representation of the current media type.</returns>
        internal string ToText(Encoding encoding)
        {
            // TODO: for now we include all the parameters since we know that we will not have accept parameters (after the quality value)
            //       that needed to be ignored.
            if (this.parameters == null || !this.parameters.Any())
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
