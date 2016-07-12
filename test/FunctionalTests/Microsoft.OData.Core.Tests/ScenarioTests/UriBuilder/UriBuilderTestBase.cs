//---------------------------------------------------------------------
// <copyright file="UriBuilderTestBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Tests.ScenarioTests.UriBuilder
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

        public static Uri UriBuilder(Uri queryUri, ODataUrlKeyDelimiter urlKeyDelimiter, ODataUriParserSettings settings)
        {
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, queryUri);
            SetODataUriParserSettingsTo(settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = urlKeyDelimiter;
            ODataUri odataUri = odataUriParser.ParseUri();

            return odataUri.BuildUri(urlKeyDelimiter);
        }
        #endregion  
    }
}
