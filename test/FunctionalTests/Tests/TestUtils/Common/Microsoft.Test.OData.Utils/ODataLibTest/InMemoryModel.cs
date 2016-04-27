//---------------------------------------------------------------------
// <copyright file="InMemoryModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.ODataLibTest
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;
    using System;
    using Microsoft.Spatial;

    /// <summary>
    /// The class constructs the IEdmModel
    /// </summary>
    public class InMemoryModel 
    {
        private IEdmModel model;

        /// <summary>
        /// Gets the edm model. Constructs it if it doesn't exist
        /// </summary>
        /// <returns>Edm model</param>
        public IEdmModel GetModel()
        {
            if (this.model == null)
            {
                ConstructableMetadata metadata = new ConstructableMetadata("InMemoryEntities", "Microsoft.Test.Taupo.OData.WCFService");
                IEdmComplexType addressType = metadata.AddComplexType("Address", typeof(Address), null, false);
                metadata.AddPrimitiveProperty(addressType, "Street", typeof(string));
                metadata.AddPrimitiveProperty(addressType, "City", typeof(string));
                metadata.AddPrimitiveProperty(addressType, "PostalCode", typeof(string));

                IEdmComplexType homeAddressType = metadata.AddComplexType("HomeAddress", typeof(HomeAddress), addressType, false);
                metadata.AddPrimitiveProperty(homeAddressType, "HomeNO", typeof(string));

                IEdmEntityType personType = metadata.AddEntityType("Person", typeof(Person), null, false, "Microsoft.Test.Taupo.OData.WCFService");
                metadata.AddKeyProperty(personType, "PersonID", typeof(Int32));
                metadata.AddPrimitiveProperty(personType, "FirstName", typeof(string));
                metadata.AddPrimitiveProperty(personType, "LastName", typeof(string));
                metadata.AddComplexProperty(personType, "HomeAddress", addressType);
                metadata.AddPrimitiveProperty(personType, "Home", typeof(GeographyPoint));
                metadata.AddMultiValueProperty(personType, "Numbers", typeof(string));
                metadata.AddContainedResourceSetReferenceProperty(personType, "Brother", personType);
                metadata.AddContainedResourceReferenceProperty(personType, "Child", personType);
                var peopleset = metadata.AddEntitySet("People", personType);
                var specialPerson = metadata.AddSingleton("SpecialPerson", personType);

                IEdmEntityType schoolType = metadata.AddEntityType("School", typeof(School), null, false);
                metadata.AddKeyProperty(schoolType, "SchoolID", typeof(Int32));
                var schoolSet = metadata.AddEntitySet("Schools", schoolType);
                var studentNavigation = metadata.AddResourceSetReferenceProperty(schoolType, "Student", peopleset, null);
                ((EdmEntitySet)schoolSet).AddNavigationTarget(studentNavigation, peopleset);

                IEdmEntityType customerType = metadata.AddEntityType("Customer", typeof(Customer), personType, false, "Microsoft.Test.Taupo.OData.WCFService");
                metadata.AddPrimitiveProperty(customerType, "City", typeof(string));
                metadata.AddPrimitiveProperty(customerType, "Birthday", typeof(DateTimeOffset));
                metadata.AddPrimitiveProperty(customerType, "TimeBetweenLastTwoOrders", typeof(TimeSpan));
                var customerset = metadata.AddEntitySet("Customers", customerType);
                var vipCustomer = metadata.AddSingleton("VipCustomer", customerType);

                IEdmEntityType employeeType = metadata.AddEntityType("Employee", typeof(Employee), personType, false, "Microsoft.Test.Taupo.OData.WCFService", true);
                metadata.AddPrimitiveProperty(employeeType, "DateHired", typeof(DateTimeOffset));
                metadata.AddPrimitiveProperty(employeeType, "Office", typeof(GeographyPoint));
                var employeeset = metadata.AddEntitySet("Employees", employeeType);
                var boss = metadata.AddSingleton("Boss", employeeType);

                IEdmEntityType productType = metadata.AddEntityType("Product", typeof(Product), null, false, "Microsoft.Test.Taupo.OData.WCFService");
                metadata.AddKeyProperty(productType, "ProductID", typeof(Int32));
                metadata.AddPrimitiveProperty(productType, "Name", typeof(string));
                metadata.AddPrimitiveProperty(productType, "QuantityPerUnit", typeof(string));
                metadata.AddPrimitiveProperty(productType, "UnitPrice", typeof(float));
                metadata.AddPrimitiveProperty(productType, "QuantityInStock", typeof(Int32));
                metadata.AddPrimitiveProperty(productType, "Discontinued", typeof(bool));
                metadata.AddComplexProperty(productType, "ManufactureAddresss", addressType, true);
                var productset = metadata.AddEntitySet("Products", productType);
                var specialProduct = metadata.AddSingleton("SpecialProduct", productType);

                IEdmEntityType orderType = metadata.AddEntityType("Order", typeof(Order), null, false, "Microsoft.Test.Taupo.OData.WCFService");
                var orderset = metadata.AddEntitySet("Orders", orderType);
                metadata.AddKeyProperty(orderType, "OrderID", typeof(Int32));
                metadata.AddPrimitiveProperty(orderType, "CustomerID", typeof(Int32));
                metadata.AddPrimitiveProperty(orderType, "EmployeeID", typeof(Int32?));
                metadata.AddPrimitiveProperty(orderType, "OrderDate", typeof(DateTimeOffset));
                metadata.AddResourceReferenceProperty(orderType, "LoggedInEmployee", employeeset, null);
                metadata.AddResourceReferenceProperty(orderType, "CustomerForOrder", customerset, null);
                var specialOrder = metadata.AddSingleton("SpecialOrder", orderType);

                metadata.AddContainedResourceReferenceProperty(personType, "FirstOrder", orderType);

                IEdmEntityType orderDetailType = metadata.AddEntityType("OrderDetail", typeof(OrderDetail), null, false, "Microsoft.Test.Taupo.OData.WCFService");
                metadata.AddKeyProperty(orderDetailType, "OrderID", typeof(Int32));
                metadata.AddKeyProperty(orderDetailType, "ProductID", typeof(Int32));
                metadata.AddPrimitiveProperty(orderDetailType, "OrderPlaced", typeof(DateTimeOffset));
                metadata.AddPrimitiveProperty(orderDetailType, "Quantity", typeof(Int32));
                metadata.AddPrimitiveProperty(orderDetailType, "UnitPrice", typeof(float));
                var productOrderedNavigation = metadata.AddResourceReferenceProperty(orderDetailType, "ProductOrdered", productset, null);
                var associatedOrderNavigation = metadata.AddResourceReferenceProperty(orderDetailType, "AssociatedOrder", orderset, null);
                var orderdetailsSet = metadata.AddEntitySet("OrderDetails", orderDetailType);

                // Edm.Duration
                IEdmEntityType durationInKeyType = metadata.AddEntityType("DurationInKey", typeof(DurationInKey), null, false, "Microsoft.Test.Taupo.OData.WCFService");
                metadata.AddKeyProperty(durationInKeyType, "Id", typeof(TimeSpan));
                metadata.AddEntitySet("DurationInKeys", durationInKeyType);

                // FUNCTIONS
                // Function that binds to single order
                metadata.AddFunctionAndFunctionImport("GetOrderRate", orderType.ToTypeReference(), MetadataUtils.GetPrimitiveTypeReference(typeof (Int32)), null, true);

                //Function that binds to a single order and returns a single order
                metadata.AddFunction("GetNextOrder", orderType.ToTypeReference(), orderType.ToTypeReference(), true, new EdmPathExpression("bindingparameter"), true);

                // Function that returns a set of orders

                var collectionOrders = new EdmCollectionType(orderType.ToTypeReference()).ToTypeReference();
                metadata.AddFunction("OrdersWithMoreThanTwoItems", collectionOrders, collectionOrders, true, new EdmPathExpression("bindingparameter"), true /*iscomposable*/);
                
                var overload1Function = metadata.AddFunction("OrdersWithMoreThanTwoItems", collectionOrders, collectionOrders, true, new EdmPathExpression("bindingparameter"), true /*iscomposable*/);
                overload1Function.AddParameter("IntParameter", MetadataUtils.GetPrimitiveTypeReference(typeof (Int32)));

                var overload2Function = metadata.AddFunction("OrdersWithMoreThanTwoItems", collectionOrders, collectionOrders, true, new EdmPathExpression("bindingparameter"), true /*iscomposable*/);
                overload2Function.AddParameter("IntParameter", MetadataUtils.GetPrimitiveTypeReference(typeof(Int32)));
                overload2Function.AddParameter("EntityParameter", productType.ToTypeReference());

                var collectionCustomers = new EdmCollectionType(customerType.ToTypeReference()).ToTypeReference();
                var customersInCityFunction = metadata.AddFunction("InCity", collectionCustomers, collectionCustomers, true, new EdmPathExpression("bindingparameter"), true /*iscomposable*/);
                customersInCityFunction.AddParameter("City", MetadataUtils.GetPrimitiveTypeReference(typeof(String)));

                var customersWithinFunction = metadata.AddFunction("Within", collectionCustomers, collectionCustomers, true, new EdmPathExpression("bindingparameter"), true /*iscomposable*/);
                customersWithinFunction.AddParameter("Location", MetadataUtils.GetPrimitiveTypeReference(typeof(GeographyPoint)));
                customersWithinFunction.AddParameter("Address", addressType.ToTypeReference(true /*nullable*/));
                customersWithinFunction.AddParameter("Distance", MetadataUtils.GetPrimitiveTypeReference(typeof(Double)));
                customersWithinFunction.AddParameter("ArbitraryInt", MetadataUtils.GetPrimitiveTypeReference(typeof(Int32)));
                customersWithinFunction.AddParameter("DateTimeOffset", MetadataUtils.GetPrimitiveTypeReference(typeof(DateTimeOffset?)));
                customersWithinFunction.AddParameter("Byte", MetadataUtils.GetPrimitiveTypeReference(typeof(Byte)));
                customersWithinFunction.AddParameter("LineString", MetadataUtils.GetPrimitiveTypeReference(typeof(GeometryLineString)));

                var withinFunction = metadata.AddFunction("Within", customerType.ToTypeReference(), MetadataUtils.GetPrimitiveTypeReference(typeof(bool)), true, null, true /*iscomposable*/);
                withinFunction.AddParameter("Location", addressType.ToTypeReference());
                withinFunction.AddParameter("Distance", MetadataUtils.GetPrimitiveTypeReference(typeof(Int32)));

                var withinFunction2 = metadata.AddFunction("Within", customerType.ToTypeReference(), MetadataUtils.GetPrimitiveTypeReference(typeof(bool)), true, null, true /*iscomposable*/);
                withinFunction2.AddParameter("Distance", MetadataUtils.GetPrimitiveTypeReference(typeof(Int32)));

                metadata.AddFunction("GetChild", personType.ToTypeReference(), personType.ToTypeReference(), true, new EdmPathExpression("bindingparameter/Child"), true /*iscomposable*/);
                metadata.AddAction("GetBrothers", personType.ToTypeReference(), new EdmCollectionTypeReference(new EdmCollectionType(personType.ToTypeReference())), true, new EdmPathExpression("bindingparameter/Child"));

                //Unbound Functions
                var lotsofOrders = metadata.AddFunctionAndFunctionImport("HasLotsOfOrders",
                   null,
                   MetadataUtils.GetPrimitiveTypeReference(typeof(bool)),
                   null,
                   false /*isBindable*/);

                lotsofOrders.Function.AsEdmFunction().AddParameter("Person", personType.ToTypeReference());

                metadata.AddFunctionAndFunctionImport("HowManyPotatoesEaten", null, MetadataUtils.GetPrimitiveTypeReference(typeof(Int32)), null, false);
                metadata.AddFunctionAndFunctionImport("QuoteOfTheDay", null, MetadataUtils.GetPrimitiveTypeReference(typeof(string)), null, false);

                // ACTIONS
                var action1 = metadata.AddAction("ChangeAddress", personType.ToTypeReference(), null /*returnType*/, true /*isbound*/, null /*entitySetPathExpression*/);
                action1.AddParameter(new EdmOperationParameter(action1, "Street", MetadataUtils.GetPrimitiveTypeReference(typeof(string))));
                action1.AddParameter(new EdmOperationParameter(action1, "City", MetadataUtils.GetPrimitiveTypeReference(typeof(string))));
                action1.AddParameter(new EdmOperationParameter(action1, "PostalCode", MetadataUtils.GetPrimitiveTypeReference(typeof(string))));
                
                metadata.AddActionImport("ChangeAddress", action1, null /*entitySet*/);
                
                // Unbound action with no parameters
                var getRecentCustomersAction = metadata.AddAction("GetRecentCustomers", null /*boundType*/, new EdmCollectionTypeReference(new EdmCollectionType(orderType.ToTypeReference())), false /*isbound*/, null /*entitySetPathExpression*/);
                metadata.AddActionImport("GetRecentCustomers", getRecentCustomersAction, orderset);

                //Adding order details navigation property to order.
                var orderDetailNavigation = metadata.AddResourceSetReferenceProperty(orderType, "OrderDetails", orderdetailsSet, null);

                //Adding orders navigation to Customer.
                var ordersNavigation = metadata.AddResourceSetReferenceProperty(customerType, "Orders", orderset, null);
                ((EdmEntitySet)customerset).AddNavigationTarget(ordersNavigation, orderset);

                //Adding parent navigation to person
                metadata.AddResourceSetReferenceProperty(personType, "Parent", null, personType);

                //Since the people set can contain a customer we need to include the target for that navigation in the people set.
                ((EdmEntitySet)peopleset).AddNavigationTarget(ordersNavigation, orderset);

                //Since the OrderSet can contain a OrderDetail we need to include the target for that navigation in the order set.
                ((EdmEntitySet)orderset).AddNavigationTarget(orderDetailNavigation, orderdetailsSet);

                //Since the OrderDetailSet can contain a AssociatedOrder we need to include the target for that navigation in the orderdetail set.
                ((EdmEntitySet)orderdetailsSet).AddNavigationTarget(associatedOrderNavigation, orderset);

                //Since the OrderDetailSet can contain a ProductOrdered we need to include the target for that navigation in the orderdetail set.
                ((EdmEntitySet)orderdetailsSet).AddNavigationTarget(productOrderedNavigation, productset);

                ((EdmSingleton)specialOrder).AddNavigationTarget(orderDetailNavigation, orderdetailsSet);
                ((EdmSingleton)specialPerson).AddNavigationTarget(ordersNavigation, orderset);

                this.model = metadata;
            }

            return this.model;
        }
    }
}
