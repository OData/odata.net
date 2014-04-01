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
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// The result of parsing an OData context URI in JSON Lite.
    /// </summary>
    internal sealed class ODataJsonLightContextUriParseResult
    {
        /// <summary>The context URI read from the payload in its unparsed form.</summary>
        private readonly Uri contextUriFromPayload;

        /// <summary>The metadata document URI as read from the payload.</summary>
        private Uri metadataDocumentUri;

        /// <summary>The fragment portion of the metadata URI.</summary>
        private string fragment;

        /// <summary>The $select query option.</summary>
        private string selectQueryOption;

        /// <summary>The resolved entity set as specified in the metadata URI.</summary>
        private IEdmEntitySet entitySet;

        /// <summary>The resolved structured type as specified in the metadata URI.</summary>
        private IEdmType edmType;

        /// <summary>The navigation property as specified in the metadata URI.</summary>
        private IEdmNavigationProperty navigationProperty;

        /// <summary>The detected payload kinds from parsing the metadata URI.</summary>
        private IEnumerable<ODataPayloadKind> detectedPayloadKinds;

        /// <summary>true if we just parsed the metadata Uri for null properties, i.e. ~/$metadata#Edm.Null; false otherwise.</summary>
        private bool isNullProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataJsonLightContextUriParseResult"/> class.
        /// </summary>
        /// <param name="contextUriFromPayload">The metadata URI read from the payload in its unparsed form.</param>
        internal ODataJsonLightContextUriParseResult(Uri contextUriFromPayload)
        {
            DebugUtils.CheckNoExternalCallers();
            this.contextUriFromPayload = contextUriFromPayload;
        }

        /// <summary>
        /// The metadata URI read from the payload in its unparsed form.
        /// </summary>
        internal Uri ContextUri
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.contextUriFromPayload;
            }
        }

        /// <summary>
        /// The metadata document URI as read from the payload.
        /// </summary>
        /// <remarks>This is the metadata URI as read from the payload without the fragment.</remarks>
        internal Uri MetadataDocumentUri
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.metadataDocumentUri;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.metadataDocumentUri = value;
            }
        }

        /// <summary>
        /// The fragment portion of the metadata URI.
        /// </summary>
        internal string Fragment
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.fragment;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.fragment = value;
            }
        }

        /// <summary>
        /// The $select query option.
        /// </summary>
        internal string SelectQueryOption
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.selectQueryOption;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.selectQueryOption = value;
            }
        }

        /// <summary>
        /// The resolved entity set as specified in the metadata URI.
        /// </summary>
        internal IEdmEntitySet EntitySet
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.entitySet;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.entitySet = value;
            }
        }

        /// <summary>
        /// The resolved structured type as specified in the metadata URI.
        /// </summary>
        internal IEdmType EdmType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.edmType;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.edmType = value;
            }
        }

        /// <summary>
        /// The navigation property as specified in the metadata URI.
        /// </summary>
        internal IEdmNavigationProperty NavigationProperty
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.navigationProperty;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.navigationProperty = value;
            }
        }

        /// <summary>
        /// The detected payload kinds from parsing the metadata URI.
        /// </summary>
        internal IEnumerable<ODataPayloadKind> DetectedPayloadKinds
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.detectedPayloadKinds;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.detectedPayloadKinds = value;
            }
        }

        /// <summary>
        /// true if we just parsed the metadata Uri for null properties, i.e. ~/$metadata#Edm.Null; false otherwise.
        /// </summary>
        internal bool IsNullProperty
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.isNullProperty;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.isNullProperty = value;
            }
        }
    }
}
