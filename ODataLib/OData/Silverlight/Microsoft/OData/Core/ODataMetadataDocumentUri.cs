//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
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
