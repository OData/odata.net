//---------------------------------------------------------------------
// <copyright file="PayloadValueConverterServiceDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PluggableFormat
{
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
            var model = TestHelper.GetModel("Microsoft.Test.OData.Services.PluggableFormat.Csdl.PluggableFormat.xml");
            model.SetPayloadValueConverter(new BinaryPayloadConverter());

            return model;
        }
    }
}
