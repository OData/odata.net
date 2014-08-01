//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Query
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.Evaluation;
    #endregion Namespaces

    /// <summary>
    /// Parse uri to ODataUri 
    /// </summary>
    public sealed class ODataQueryUriParser
    {
        /// <summary>
        /// Model to use for metadata binding.
        /// </summary>
        private readonly IEdmModel model;

        /// <summary>
        /// Absolute URI of the service root.
        /// </summary>
        private readonly Uri serviceRoot;

        /// <summary>
        /// Uri parser settings 
        /// </summary>
        private readonly ODataUriParserSettings settings;

        /// <summary>
        /// ODataQueryUriParser constructor
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="serviceRoot">Absolute URI of the service root.</param>
        /// <param name="settings">Uri parser settings, may be null for default settings to be used </param>
        public ODataQueryUriParser(IEdmModel model, Uri serviceRoot, ODataUriParserSettings settings)
        {
            this.model = model;
            this.serviceRoot = serviceRoot;
            this.settings = settings;
        }

        /// <summary>
        /// Parses a given full Uri 
        /// </summary>
        /// <param name="fullUri">full Uri to be parsed</param>
        /// <param name="urlConventions">mark whether is KeyAsSegment</param>
        /// <returns>A <see cref="ODataUri"/> representing the semactic tree.</returns>
        public ODataUri ParseUri(Uri fullUri, ODataUrlConventions urlConventions)
        {
            ODataUri odataUri = new ODataUri();
            var parser = new ODataUriParser(this.model, this.serviceRoot, fullUri);

            SetODataUriParserSettingsTo(this.settings, parser.Settings);
            parser.UrlConventions = urlConventions;

            odataUri.ServiceRoot = serviceRoot;
            odataUri.Path = parser.ParsePath();
            odataUri.SelectAndExpand = parser.ParseSelectAndExpand();
            odataUri.Filter = parser.ParseFilter();
            odataUri.ParameterAliasValueAccessor = parser.ParameterAliasValueAccessor;       
            odataUri.OrderBy = parser.ParseOrderBy();
            odataUri.Search = parser.ParseSearch();
            odataUri.Top = parser.ParseTop();
            odataUri.Skip = parser.ParseSkip();
            odataUri.QueryCount = parser.ParseCount(); 
            return odataUri;
        }

        /// <summary>
        /// set new value on ODataUriParserSettings
        /// </summary>
        /// <param name="sourceSettings">new value will be assigned to the settings</param>
        /// <param name="destSettings">settings will be modified</param>
        private static void SetODataUriParserSettingsTo(ODataUriParserSettings sourceSettings, ODataUriParserSettings destSettings)
        {
            if (sourceSettings != null)
            {
                destSettings.FilterLimit = sourceSettings.FilterLimit;
                destSettings.PathLimit = sourceSettings.PathLimit;
                destSettings.OrderByLimit = sourceSettings.OrderByLimit;
                destSettings.SelectExpandLimit = sourceSettings.SelectExpandLimit;
                destSettings.SearchLimit = sourceSettings.SearchLimit;
                destSettings.MaximumExpansionCount = sourceSettings.MaximumExpansionCount;
                destSettings.MaximumExpansionDepth = sourceSettings.MaximumExpansionDepth;
            }
        }
    }
}
