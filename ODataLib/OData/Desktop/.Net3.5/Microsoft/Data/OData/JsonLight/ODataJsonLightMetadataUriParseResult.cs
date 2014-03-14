//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// The result of parsing an OData metadata URI in JSON Lite.
    /// </summary>
    internal sealed class ODataJsonLightMetadataUriParseResult
    {
        /// <summary>The metadata URI read from the payload in its unparsed form.</summary>
        private readonly Uri metadataUriFromPayload;

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
        /// Initializes a new instance of the <see cref="ODataJsonLightMetadataUriParseResult"/> class.
        /// </summary>
        /// <param name="metadataUriFromPayload">The metadata URI read from the payload in its unparsed form.</param>
        internal ODataJsonLightMetadataUriParseResult(Uri metadataUriFromPayload)
        {
            DebugUtils.CheckNoExternalCallers();
            this.metadataUriFromPayload = metadataUriFromPayload;
        }

        /// <summary>
        /// The metadata URI read from the payload in its unparsed form.
        /// </summary>
        internal Uri MetadataUri
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.metadataUriFromPayload;
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
