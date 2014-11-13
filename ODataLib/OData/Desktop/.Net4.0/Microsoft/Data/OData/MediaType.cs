//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData
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
            DebugUtils.CheckNoExternalCallers();
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
            this.parameters = parameters;
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
#if SILVERLIGHT || WINDOWS_PHONE || PORTABLELIB  // ISO-8859-1 not available
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
                DebugUtils.CheckNoExternalCallers();
                return this.type + "/" + this.subType;
            }
        }

        /// <summary>Returns the subtype part of the media type.</summary>
        internal string SubTypeName
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.subType;
            }
        }

        /// <summary>Returns the type part of the media type.</summary>
        internal string TypeName
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.type;
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
            DebugUtils.CheckNoExternalCallers();

            return ToText(null);
        }

        /// <summary>
        /// Converts the current <see cref="MediaType"/> to a string representation suitable for use in a content-type header.
        /// </summary>
        /// <param name="encoding">The encoding to use when converting the media type into text.</param>
        /// <returns>The string representation of the current media type.</returns>
        internal string ToText(Encoding encoding)
        {
            DebugUtils.CheckNoExternalCallers();

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
