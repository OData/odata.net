//   OData .NET Libraries ver. 6.8.1
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
