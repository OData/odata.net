//-----------------------------------------------------------------------------
// <copyright file="OpenTypesServiceClientTestDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Server
{
    public class OpenTypesServiceClientTestsEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Row>("Row");
            builder.EntitySet<RowIndex>("RowIndex");
            builder.ComplexType<ContactDetails>();

            return builder.GetEdmModel();
        }
    }
}
