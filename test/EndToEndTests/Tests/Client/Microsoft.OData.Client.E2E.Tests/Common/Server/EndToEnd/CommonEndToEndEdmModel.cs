//-----------------------------------------------------------------------------
// <copyright file="CommonEndToEndEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd
{
    public class CommonEndToEndEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Product>("Products");
            builder.EntitySet<OrderLine>("OrderLines");
            builder.EntitySet<Person>("People");
            builder.EntitySet<PersonMetadata>("PersonsMetadata");
            builder.EntitySet<AllSpatialTypes>("AllGeoTypesSet");
            builder.EntitySet<AllSpatialCollectionTypes>("AllGeoCollectionTypesSet");
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Login>("Login");
            builder.EntitySet<RSAToken>("RSATokens");
            builder.EntitySet<PageView>("PageViews");
            builder.EntitySet<LastLogin>("LastLogins");
            builder.EntitySet<Message>("Messages");
            builder.EntitySet<MessageAttachment>("MessageAttachments");
            builder.EntitySet<Order>("Orders");
            builder.EntitySet<ProductDetail>("ProductDetails");
            builder.EntitySet<ProductReview>("ProductReviews");
            builder.EntitySet<ProductPhoto>("ProductPhotos");
            builder.EntitySet<CustomerInfo>("CustomerInfos");
            builder.EntitySet<Computer>("Computers");
            builder.EntitySet<ComputerDetail>("ComputerDetails");
            builder.EntitySet<Driver>("Drivers");
            builder.EntitySet<License>("Licenses");
            builder.EntitySet<MappedEntityType>("MappedEntityTypes");
            builder.EntitySet<Car>("Cars");
            builder.EntitySet<Bank>("Banks");
            builder.EntitySet<BankAccount>("BankAccounts");

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

            builder.EntityType<SpecialEmployee>()
                .Action("IncreaseEmployeeSalary")
                .Returns<int>();

            builder.EntityType<Employee>()
                .Action("IncreaseEmployeeSalary")
                .Returns<bool>()
                .Parameter<int>("n");

            builder.EntityType<Employee>().Collection
                .Action("IncreaseSalaries")
                .Parameter<int>("n");

            builder.EntityType<SpecialEmployee>().Collection
                .Action("IncreaseSalaries")
                .Parameter<int>("n");

            builder.EntityType<Employee>()
                .Action("Sack");

            builder.Action("ResetDataSource");

            builder.EntityType<Customer>()
                .Action("ChangeCustomerAuditInfo")
                .Parameter<AuditInfo>("auditInfo");

            var action = builder.EntityType<ComputerDetail>()
                .Action("ResetComputerDetailsSpecifications");
            action.CollectionParameter<string>("specifications");
            action.Parameter<DateTimeOffset>("purchaseTime");

            builder.Function("GetPrimitiveString")
                .Returns<string>();

            builder.Function("GetSpecificCustomer")
                .ReturnsCollectionFromEntitySet<Customer>("Customers")
                .Parameter<string>("Name");

            builder.Function("GetCustomerCount")
                .Returns<int>();

            builder.Function("GetArgumentPlusOne")
                .Returns<int>()
                .Parameter<int>("arg1");

            builder.Function("EntityProjectionReturnsCollectionOfComplexTypes")
                .ReturnsCollection<ContactDetails>();

            builder.Function("InStreamErrorGetCustomer")
                .ReturnsCollectionFromEntitySet<Customer>("Customers");

            return builder.GetEdmModel();
        }
    }
}
