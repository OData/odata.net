//---------------------------------------------------------------------
// <copyright file="PayloadValueConverterServiceDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PluggableFormat
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.PluggableFormat;

    public class PayloadValueConverterServiceDataSource : PluggableFormatServiceDataSource
    {
        public PayloadValueConverterServiceDataSource()
        {
            this.OperationProvider = new PluggableFormatOperationProvider<PayloadValueConverterServiceDataSource>();
        }

        protected override IEdmModel CreateModel()
        {
            return TestHelper.GetModel("Microsoft.Test.OData.Services.PluggableFormat.Csdl.PluggableFormat.xml");
        }

        protected override void ConfigureContainer(IServiceCollection services)
        {
            base.ConfigureContainer(services);
            services.AddSingleton<ODataPayloadValueConverter, BinaryPayloadConverter>();
        }
    }
}
