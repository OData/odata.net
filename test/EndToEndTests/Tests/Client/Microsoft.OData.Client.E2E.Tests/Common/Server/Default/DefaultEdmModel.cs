//-----------------------------------------------------------------------------
// <copyright file="DefaultEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.Default
{
    public class DefaultEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Person>("People");
            builder.Singleton<Person>("Boss");
            builder.EntitySet<Customer>("Customers");
            builder.Singleton<Customer>("VipCustomer");
            builder.EntitySet<Employee>("Employees");
            builder.EntitySet<Product>("Products");
            builder.EntitySet<ProductDetail>("ProductDetails");
            builder.EntitySet<ProductReview>("ProductReviews");
            builder.EntitySet<Calendar>("Calendars");
            builder.EntitySet<OrderDetail>("OrderDetails");
            builder.EntitySet<Department>("Departments");
            builder.Singleton<Company>("Company");
            builder.Singleton<Company>("PublicCompany");
            builder.Singleton<LabourUnion>("LabourUnion");
            builder.EntitySet<Account>("Accounts");
            builder.EntitySet<Order>("Orders");
            builder.EntitySet<PaymentInstrument>("PaymentInstruments");
            builder.EntityType<AbstractEntity>().Abstract();
            builder.EntitySet<StoredPI>("StoredPIs");
            builder.EntitySet<Subscription>("SubscriptionTemplates");
            builder.Singleton<StoredPI>("DefaultStoredPI");
            builder.EntitySet<Bank>("Banks");
            builder.EntitySet<BankAccount>("BankAccounts");

            builder.EntityType<Product>()
                .Action("AddAccessRight")
                .Returns<AccessLevel>()
                .Parameter<AccessLevel>("accessRight");

            builder.EntityType<Company>()
                .Action("IncreaseRevenue")
                .Returns<int>()
                .Parameter<int>("IncreaseValue");

            var resetAddressAction = builder.EntityType<Person>()
                .Action("ResetAddress")
                .ReturnsEntityViaEntitySetPath<Person>("bindingParameter");
            resetAddressAction.CollectionParameter<Address>("addresses");
            resetAddressAction.Parameter<int>("index");

            var placeOrder = builder.EntityType<Customer>()
                .Action("PlaceOrder")
                .ReturnsEntityViaEntitySetPath<Order>("bindingParameter/Orders");
            placeOrder.Parameter<Order>("order");

            var placeOrders = builder.EntityType<Customer>()
                .Action("PlaceOrders")
                .ReturnsCollectionViaEntitySetPath<Order>("bindingParameter/Orders");
            placeOrders.CollectionParameter<Order>("orders");

            var discount = builder.EntityType<Product>().Collection
                .Action("Discount")
                .ReturnsCollectionViaEntitySetPath<Product>("bindingParameter");
            discount.Parameter<int>("percentage");

            var changeLabourUnionName = builder.EntityType<LabourUnion>()
                .Action("ChangeLabourUnionName");
            changeLabourUnionName.Parameter<string>("name");

            var changeShipTimeAndDate = builder.EntityType<Order>()
                .Action("ChangeShipTimeAndDate")
                .ReturnsEntityViaEntitySetPath<Order>("bindingParameter");
            changeShipTimeAndDate.Parameter<Date>("date");
            changeShipTimeAndDate.Parameter<TimeOfDay>("time");

            builder.Action("Discount")
                .Parameter<int>("percentage");

            builder.Action("ResetBossEmail")
                .ReturnsCollection<string>()
                .CollectionParameter<string>("emails");

            builder.Action("ResetBossAddress")
                .Returns<Address>()
                .Parameter<Address>("address");

            builder.Action("ResetDefaultDataSource");

            builder.EntityType<Product>()
                .Function("GetProductDetails")
                .ReturnsCollectionViaEntitySetPath<ProductDetail>("bindingParameter/Details")
                .Parameter<int>("count");

            builder.EntityType<ProductDetail>()
                .Function("GetRelatedProduct")
                .ReturnsEntityViaEntitySetPath<ProductDetail>("bindingParameter/RelatedProduct");

            builder.EntityType<Customer>()
                .Function("getOrderAndOrderDetails")
                .ReturnsCollectionViaEntitySetPath<AbstractEntity>("bindingParameter/Orders");

            builder.EntityType<Employee>().Collection
                .Function("GetSeniorEmployees")
                .ReturnsEntityViaEntitySetPath<Employee>("bindingParameter");

            builder.EntityType<Order>()
                .Function("GetShipDate")
                .Returns<Date>();

            builder.EntityType<Order>()
                .Function("GetShipTime")
                .Returns<TimeOfDay>();

            builder.EntityType<Order>()
                .Function("CheckShipTime")
                .Returns<bool>()
                .Parameter<TimeOfDay>("time");

            builder.EntityType<Order>()
                .Function("CheckShipDate")
                .Returns<bool>()
                .Parameter<Date>("date");

            builder.Function("GetDefaultColor")
                .Returns<Color>();

            builder.Function("GetPerson")
                .Returns<Person>()
                .Parameter<Address>("address");

            builder.Function("GetPerson2")
                .Returns<string>()
                .Parameter<Address>("city");

            builder.Function("GetAllProducts")
                .ReturnsCollection<Product>();

            var getBossEmails = builder.Function("GetBossEmails")
                .ReturnsCollection<string>();
            getBossEmails.Parameter<int>("start");
            getBossEmails.Parameter<int>("count");

            builder.Function("GetProductsByAccessLevel")
                .Returns<double>()
                .Parameter<AccessLevel>("accessLevel");

            builder.EntityType<GiftCard>()
                .Function("GetActualAmount")
                .Returns<double>()
                .Parameter<double>("bonusRate");

            builder.EntityType<Account>()
                .Function("GetDefaultPI")
                .ReturnsEntityViaEntitySetPath<PaymentInstrument>("bindingParameter/MyPaymentInstruments");

            builder.EntityType<Account>()
                .Action("RefreshDefaultPI")
                .ReturnsEntityViaEntitySetPath<PaymentInstrument>("bindingParameter/MyPaymentInstruments")
                .Parameter<DateTimeOffset>("newDate");

            builder.EntityType<Person>()
                .Function("GetHomeAddress")
                .Returns<HomeAddress>();

            builder.EntityType<Account>()
                .Function("GetAccountInfo")
                .Returns<AccountInfo>();

            return builder.GetEdmModel();
        }
    }
}
