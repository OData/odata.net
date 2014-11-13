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

namespace System.Data.Services
{
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.Data.OData;

    /// <summary>
    /// Extends <see cref="ODataFormat"/> to also carry a set of media type parameters.
    /// </summary>
    internal class ODataFormatWithParameters
    {
        /// <summary>The raw media type represented by this instance.</summary>
        private readonly string rawMediaType;

        /// <summary>The parameters of the raw media type once it has been parsed.</summary>
        private ContentTypeUtil.MediaParameter[] mediaTypeParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataFormatWithParameters"/> class.
        /// </summary>
        /// <param name="format">The format to extend.</param>
        /// <param name="rawMediaType">The raw media type represented by this instance.</param>
        internal ODataFormatWithParameters(ODataFormat format, string rawMediaType)
            : this(format)
        {
            this.rawMediaType = rawMediaType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataFormatWithParameters"/> class.
        /// </summary>
        /// <param name="format">The format to extend.</param>
        internal ODataFormatWithParameters(ODataFormat format)
        {
            Debug.Assert(format != null, "format != null");
            this.Format = format;
            this.IsAtom = this.Format == ODataFormat.Atom;
            this.IsJsonLight = this.Format == ODataFormat.Json;
        }

        /// <summary>
        /// Gets the format this instance is extending.
        /// </summary>
        internal ODataFormat Format { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is the Atom format.
        /// </summary>
        internal bool IsAtom { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is the JsonLight format.
        /// </summary>
        internal bool IsJsonLight { get; private set; }

        /// <summary>
        /// Gets the value for the parameter of the specified name, or null if it is not found.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The value of the parameter or null.</returns>
        internal string GetParameterValue(string parameterName)
        {
            this.PopulateMediaTypeParameters();

            if (this.mediaTypeParameters.Length == 0)
            {
                return null;
            }

            return this.mediaTypeParameters
                .Where(p => ParameterNameMatches(p, parameterName))
                .Select(p => p.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns whether the parameter name matches using case-insensitive comparison.
        /// </summary>
        /// <param name="mediaParameter">The media parameter.</param>
        /// <param name="parameterName">Name to compare to.</param>
        /// <returns>Whether the name matches.</returns>
        private static bool ParameterNameMatches(ContentTypeUtil.MediaParameter mediaParameter, string parameterName)
        {
            return string.Compare(parameterName, mediaParameter.Name, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Ensures that the media type has been parsed and that the parameters list has been populated.
        /// This is done lazily to avoid parsing them if GetParameterValue is never called.
        /// </summary>
        private void PopulateMediaTypeParameters()
        {
            if (this.mediaTypeParameters != null)
            {
                return;
            }

            if (this.rawMediaType != null)
            {
                string mime;
                Encoding encoding;
                this.mediaTypeParameters = ContentTypeUtil.ReadContentType(this.rawMediaType, out mime, out encoding);
            }

            // if we had no media type or it had no parameters, create an empty list
            if (this.mediaTypeParameters == null)
            {
                this.mediaTypeParameters = new ContentTypeUtil.MediaParameter[0];
            }
        }
    }
}
