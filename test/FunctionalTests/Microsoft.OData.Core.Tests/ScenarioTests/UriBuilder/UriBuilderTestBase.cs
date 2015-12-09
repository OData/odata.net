//---------------------------------------------------------------------
// <copyright file="UriBuilderTestBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriBuilder;
using Microsoft.OData.Core.UriParser;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriBuilder
{
    public class UriBuilderTestBase
    {
        protected static Uri ServiceRoot = new Uri("http://gobbledygook/");
        protected readonly ODataUriParserSettings settings = new ODataUriParserSettings();

        #region private methods
        public static void SetODataUriParserSettingsTo(ODataUriParserSettings sourceSettings, ODataUriParserSettings destSettings)
        {
            if (sourceSettings != null)
            {
                destSettings.MaximumExpansionCount = sourceSettings.MaximumExpansionCount;
                destSettings.MaximumExpansionDepth = sourceSettings.MaximumExpansionDepth;
            }
        }

        public static Uri UriBuilder(Uri queryUri, ODataUrlConventions urlConventions, ODataUriParserSettings settings)
        {
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, queryUri);
            SetODataUriParserSettingsTo(settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = urlConventions;
            ODataUri odataUri = odataUriParser.ParseUri();

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(urlConventions, odataUri);
            return odataUriBuilder.BuildUri();
        }
        #endregion  
    }
}
