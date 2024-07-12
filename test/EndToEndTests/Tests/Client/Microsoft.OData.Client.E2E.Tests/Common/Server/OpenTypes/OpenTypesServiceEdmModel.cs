//-----------------------------------------------------------------------------
// <copyright file="OpenTypesServiceEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.OpenTypes
{
    public class OpenTypesServiceEdmModel
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
