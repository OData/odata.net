//-----------------------------------------------------------------------------
// <copyright file="ActionOverloadingEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server
{
    public class ActionOverloadingEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Product>("Products");
            builder.EntitySet<OrderLine>("OrderLines");
            builder.EntitySet<Person>("People");
            builder.EntitySet<PersonMetadata>("PersonsMetadata");

            builder.Action("RetrieveProduct")
                .Returns<int>();

            builder.EntityType<Product>()
                .Action("RetrieveProduct")
                .Returns<int>();

            builder.EntityType<OrderLine>()
                .Action("RetrieveProduct")
                .Returns<int>();

            builder.Action("UpdatePersonInfo");

            builder.EntityType<Person>()
                .Action("UpdatePersonInfo");

            builder.EntityType<Employee>()
                .Action("UpdatePersonInfo");

            builder.EntityType<SpecialEmployee>()
                .Action("UpdatePersonInfo");

            builder.EntityType<Contractor>()
                .Action("UpdatePersonInfo");

            builder.EntityType<Employee>()
                .Action("IncreaseEmployeeSalary")
                .Returns<bool>()
                .Parameter<int>("n");

            builder.EntityType<SpecialEmployee>()
                .Action("IncreaseEmployeeSalary")
                .Returns<int>();

            builder.EntityType<Employee>().Collection
                .Action("IncreaseSalaries")
                .Parameter<int>("n");

            builder.EntityType<SpecialEmployee>().Collection
                .Action("IncreaseSalaries")
                .Parameter<int>("n");

            return builder.GetEdmModel();
        }
    }
}
