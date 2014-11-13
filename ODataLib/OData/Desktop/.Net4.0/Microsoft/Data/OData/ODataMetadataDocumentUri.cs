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
    using System;

    /// <summary>
    /// Simple structure for storing both a base URI and the select clause for generating metadata links in JSON-Light payloads.
    /// </summary>
    internal sealed class ODataMetadataDocumentUri
    {
        /// <summary>The base uri to the metadata document.</summary>
        private readonly Uri baseUri;

        /// <summary>The select clause to include when generating metadata links.</summary>
        private string selectClause;

        /// <summary>
        /// Initializes a new instance of <see cref="ODataMetadataDocumentUri"/>.
        /// </summary>
        /// <param name="baseUri">The base uri to the metadata document.</param>
        internal ODataMetadataDocumentUri(Uri baseUri)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(baseUri, "baseUri");
            
            if (!baseUri.IsAbsoluteUri)
            {
                throw new ODataException(Strings.WriterValidationUtils_MessageWriterSettingsMetadataDocumentUriMustBeNullOrAbsolute(UriUtilsCommon.UriToString(baseUri)));
            }

            this.baseUri = baseUri;
        }

        /// <summary>
        /// Gets the base uri to the metadata document.
        /// </summary>
        internal Uri BaseUri
        {
            get 
            { 
                DebugUtils.CheckNoExternalCallers();
                return this.baseUri; 
            }
        }

        /// <summary>
        /// Gets the select clause to include when generating metadata links.
        /// </summary>
        internal string SelectClause
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.selectClause;
            }

            set 
            {
                DebugUtils.CheckNoExternalCallers(); 
                this.selectClause = value; 
            }
        }
    }
}
