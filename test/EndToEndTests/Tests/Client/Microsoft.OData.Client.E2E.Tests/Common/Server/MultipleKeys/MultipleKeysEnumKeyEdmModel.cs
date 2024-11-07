//-----------------------------------------------------------------------------
// <copyright file="MultipleKeysEnumKeyEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.MultipleKeys
{
    public class MultipleKeysEnumKeyEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<EmployeeWithEnumKey>("Employees");
            builder.EntitySet<Organization>("Organizations");

            return builder.GetEdmModel();
        }
    }
}
