//---------------------------------------------------------------------
// <copyright file="ODataSimplifiedRequestHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;
    using Microsoft.Test.OData.Services.ODataWCFService.Handlers;

    internal class ODataSimplifiedRequestHandler : RootRequestHandler
    {
        public ODataSimplifiedRequestHandler(HttpMethod httpMethod, IODataDataSource dataSource)
            : base(httpMethod, dataSource)
        {
        }

        protected override RequestHandler DispatchHandler()
        {
            if (this.HttpMethod == HttpMethod.GET)
            {
                return new ODataSimplifiedQueryHandler(this);
            }
            else if (this.HttpMethod == HttpMethod.POST)
            {
                return new ODataSimplifiedCreateHandler(this);
            }

            return base.DispatchHandler();
        }

        internal class ODataSimplifiedQueryHandler : QueryHandler
        {
            public ODataSimplifiedQueryHandler(RequestHandler other)
                : base(other)
            { }

            protected override ODataMessageWriterSettings GetWriterSettings()
            {
                var settings = base.GetWriterSettings();
                ODataSimplifiedOptions.GetODataSimplifiedOptions(null).EnableWritingODataAnnotationWithoutPrefix = true;
                return settings;
            }
        }

        internal class ODataSimplifiedCreateHandler : CreateHandler
        {
            public ODataSimplifiedCreateHandler(RequestHandler other)
                : base(other)
            { }

            protected override ODataMessageWriterSettings GetWriterSettings()
            {
                var settings = base.GetWriterSettings();
                ODataSimplifiedOptions.GetODataSimplifiedOptions(null).EnableWritingODataAnnotationWithoutPrefix = true;
                return settings;
            }
        }
    }
}
