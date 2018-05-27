//---------------------------------------------------------------------
// <copyright file="ODataFormatWithParameters.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.OData;

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
            this.IsJsonLight = this.Format == ODataFormat.Json;
        }

        /// <summary>
        /// Gets the format this instance is extending.
        /// </summary>
        internal ODataFormat Format { get; private set; }

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