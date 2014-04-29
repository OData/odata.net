//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// The result of parsing an OData context URI in JSON Lite.
    /// </summary>
    internal sealed class ODataJsonLightContextUriParseResult
    {
        /// <summary>The context URI read from the payload in its unparsed form.</summary>
        private readonly Uri contextUriFromPayload;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataJsonLightContextUriParseResult"/> class.
        /// </summary>
        /// <param name="contextUriFromPayload">The context URI read from the payload in its unparsed form.</param>
        internal ODataJsonLightContextUriParseResult(Uri contextUriFromPayload)
        {
            this.contextUriFromPayload = contextUriFromPayload;
        }

        /// <summary>
        /// The context URI read from the payload in its unparsed form.
        /// </summary>
        internal Uri ContextUri
        {
            get
            {
                return this.contextUriFromPayload;
            }
        }

        /// <summary>
        /// The metadata document URI as read from the payload.
        /// </summary>
        /// <remarks>This is the metadata document URI as read from the payload without the fragment.</remarks>
        internal Uri MetadataDocumentUri { get; set; }

        /// <summary>
        /// The fragment portion of the context URI.
        /// </summary>
        internal string Fragment { get; set; }

        /// <summary>
        /// The $select query option.
        /// </summary>
        internal string SelectQueryOption { get; set; }

        /// <summary>
        /// The resolved navigation source as specified in the context URI.
        /// </summary>
        internal IEdmNavigationSource NavigationSource { get; set; }

        /// <summary>
        /// The resolved structured type as specified in the context URI.
        /// </summary>
        internal IEdmType EdmType { get; set; }

        /// <summary>
        /// The detected payload kinds from parsing the context URI.
        /// </summary>
        internal IEnumerable<ODataPayloadKind> DetectedPayloadKinds { get; set; }

        /// <summary>
        /// true if we just parsed the context Uri for null properties, i.e. ~/$metadata#Edm.Null; false otherwise.
        /// </summary>
        internal bool IsNullProperty { get; set; }

        /// <summary>
        /// ODataPath parsed from context Url
        /// </summary>
        internal ODataPath Path { get; set; }

        /// <summary>
        /// DeltaKind from context Url, only applicable when payload kind is Delta
        /// </summary>
        internal ODataDeltaKind DeltaKind { get; set; }
    }
}
