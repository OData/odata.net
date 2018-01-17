//---------------------------------------------------------------------
// <copyright file="InMemoryModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Query.Metadata;
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
            if (model == null)
            {
                ConstructableMetadata metadata = new ConstructableMetadata("InMemoryEntities", "Microsoft.Test.Taupo.OData.WCFService");
                IEdmComplexType addressType = metadata.AddComplexType("Address", typeof(Address), null, false);
                metadata.AddPrimitiveProperty(addressType, "Street", typeof(string));
                metadata.AddPrimitiveProperty(addressType, "City", typeof(string));
                metadata.AddPrimitiveProperty(addressType, "PostalCode", typeof(string));

                IEdmEntityType personType = metadata.AddEntityType("Person", typeof(Person), null, false, "Microsoft.Test.Taupo.OData.WCFService");
                metadata.AddKeyProperty(personType, "PersonID", typeof(Int32));
                metadata.AddPrimitiveProperty(personType, "FirstName", typeof(string));
                metadata.AddPrimitiveProperty(personType, "LastName", typeof(string));
                metadata.AddComplexProperty(personType, "HomeAddress", addressType);
                metadata.AddPrimitiveProperty(personType, "Home", typeof(GeographyPoint));
                metadata.AddMultiValueProperty(personType, "Numbers", typeof(string));
                var peopleset = metadata.AddEntitySet("People", personType);

                IEdmEntityType customerType = metadata.AddEntityType("Customer", typeof(Customer), personType, false, "Microsoft.Test.Taupo.OData.WCFService");
                metadata.AddPrimitiveProperty(customerType, "City", typeof(string));
                metadata.AddPrimitiveProperty(customerType, "Birthday", typeof(DateTime));
                metadata.AddPrimitiveProperty(customerType, "TimeBetweenLastTwoOrders", typeof(TimeSpan));
                var customerset = metadata.AddEntitySet("Customers", customerType);

                IEdmEntityType employeeType = metadata.AddEntityType("Employee", typeof(Employee), personType, false, "Microsoft.Test.Taupo.OData.WCFService", true);
                metadata.AddPrimitiveProperty(employeeType, "DateHired", typeof(DateTime));
                metadata.AddPrimitiveProperty(employeeType, "Office", typeof(GeographyPoint));
                var employeeset = metadata.AddEntitySet("Employees", employeeType);

                IEdmEntityType productType = metadata.AddEntityType("Product", typeof(Product), null, false, "Microsoft.Test.Taupo.OData.WCFService");
                metadata.AddKeyProperty(productType, "ProductID", typeof(Int32));
                metadata.AddPrimitiveProperty(productType, "Name", typeof(string));
                metadata.AddPrimitiveProperty(productType, "QuantityPerUnit", typeof(string));
                metadata.AddPrimitiveProperty(productType, "UnitPrice", typeof(float));
                metadata.AddPrimitiveProperty(productType, "QuantityInStock", typeof(Int32));
                metadata.AddPrimitiveProperty(productType, "Discontinued", typeof(bool));
                var productset = metadata.AddEntitySet("Products", productType);

                IEdmEntityType orderType = metadata.AddEntityType("Order", typeof(Order), null, false, "Microsoft.Test.Taupo.OData.WCFService");
                metadata.AddKeyProperty(orderType, "OrderID", typeof(Int32));
                metadata.AddPrimitiveProperty(orderType, "CustomerID", typeof(Int32));
                metadata.AddPrimitiveProperty(orderType, "EmployeeID", typeof(Int32?));
                metadata.AddPrimitiveProperty(orderType, "OrderDate", typeof(DateTime));
                metadata.AddResourceReferenceProperty(orderType, "LoggedInEmployee", employeeset, null);
                metadata.AddResourceReferenceProperty(orderType, "CustomerForOrder", customerset, null);
                var orderset = metadata.AddEntitySet("Orders", orderType);

                IEdmEntityType orderDetailType = metadata.AddEntityType("OrderDetail", typeof(OrderDetail), null, false, "Microsoft.Test.Taupo.OData.WCFService");
                metadata.AddKeyProperty(orderDetailType, "OrderID", typeof(Int32));
                metadata.AddKeyProperty(orderDetailType, "ProductID", typeof(Int32));
                metadata.AddPrimitiveProperty(orderDetailType, "OrderPlaced", typeof(DateTimeOffset));
                metadata.AddPrimitiveProperty(orderDetailType, "Quantity", typeof(Int32));
                metadata.AddPrimitiveProperty(orderDetailType, "UnitPrice", typeof(float));
                var productOrderedNavigation = metadata.AddResourceReferenceProperty(orderDetailType, "ProductOrdered", productset, null);
                var associatedOrderNavigation = metadata.AddResourceReferenceProperty(orderDetailType, "AssociatedOrder", orderset, null);
                var orderdetailsSet = metadata.AddEntitySet("OrderDetails", orderDetailType);

                // FUNCTIONS
                // Function that binds to single order
                metadata.AddFunction("GetOrderRate", orderType.ToTypeReference(),
                                     MetadataUtils.GetPrimitiveTypeReference(typeof (Int32)), null, null, true);

                //Function that binds to a single order and returns a single order
                metadata.AddFunction("GetNextOrder", orderType.ToTypeReference(),
                                     orderType.ToTypeReference(), null, null, true);

                // Function that returns a set of orders
                metadata.AddFunction("OrdersWithMoreThanTwoItems",
                    new EdmCollectionType(orderType.ToTypeReference()).ToTypeReference(),
                    new EdmCollectionType(orderType.ToTypeReference()).ToTypeReference(), 
                    orderset, 
                    null, 
                    true /*isBindable*/);

                var overload1 = metadata.AddFunction("OrdersWithMoreThanTwoItems",
                                    new EdmCollectionType(orderType.ToTypeReference()).ToTypeReference(),
                                    new EdmCollectionType(orderType.ToTypeReference()).ToTypeReference(),
                                    orderset,
                                    null, 
                                    true);
                overload1.AddParameter(new EdmFunctionParameter(overload1, "IntParameter",
                                                                MetadataUtils.GetPrimitiveTypeReference(typeof (Int32))));

                var overload2 = metadata.AddFunction("OrdersWithMoreThanTwoItems",
                                    new EdmCollectionType(orderType.ToTypeReference()).ToTypeReference(),
                                    new EdmCollectionType(orderType.ToTypeReference()).ToTypeReference(),
                                    orderset,
                                    null,
                                    true);
                overload2.AddParameter(new EdmFunctionParameter(overload2, "IntParameter", MetadataUtils.GetPrimitiveTypeReference(typeof(Int32))));
                overload2.AddParameter(new EdmFunctionParameter(overload2, "EntityParameter", productType.ToTypeReference()));

                var customersInCity = metadata.AddFunction("InCity",
                                   new EdmCollectionType(customerType.ToTypeReference()).ToTypeReference(),
                                   new EdmCollectionType(customerType.ToTypeReference()).ToTypeReference(),
                                   customerset,
                                   null,
                                   true);

                customersInCity.AddParameter(new EdmFunctionParameter(customersInCity, "City",
                                                                MetadataUtils.GetPrimitiveTypeReference(typeof(String))));

                var customersWithin = metadata.AddFunction("Within",
                                   new EdmCollectionType(customerType.ToTypeReference()).ToTypeReference(),
                                   new EdmCollectionType(customerType.ToTypeReference()).ToTypeReference(),
                                   customerset,
                                   null,
                                   true);

                customersWithin.AddParameter(new EdmFunctionParameter(customersWithin, "Location",
                                                                MetadataUtils.GetPrimitiveTypeReference(typeof(GeographyPoint))));
                customersWithin.AddParameter(new EdmFunctionParameter(customersWithin, "Address", addressType.ToTypeReference(true /*nullable*/)));
                customersWithin.AddParameter(new EdmFunctionParameter(customersWithin, "Distance",
                                                                MetadataUtils.GetPrimitiveTypeReference(typeof(Double))));
                customersWithin.AddParameter(new EdmFunctionParameter(customersWithin, "ArbitraryInt",
                                                                MetadataUtils.GetPrimitiveTypeReference(typeof(Int32))));
                customersWithin.AddParameter(new EdmFunctionParameter(customersWithin, "DateTimeOffset",
                                                                                MetadataUtils.GetPrimitiveTypeReference(typeof(DateTimeOffset?))));
                customersWithin.AddParameter(new EdmFunctionParameter(customersWithin, "Byte",
                                                                                MetadataUtils.GetPrimitiveTypeReference(typeof(Byte))));
                customersWithin.AddParameter(new EdmFunctionParameter(customersWithin, "LineString",
                                                                                MetadataUtils.GetPrimitiveTypeReference(typeof(GeometryLineString))));

                var within = metadata.AddFunction("Within", 
                                    customerType.ToTypeReference(),
                                    MetadataUtils.GetPrimitiveTypeReference(typeof(bool)),
                                    null,
                                    null,
                                    true);
                within.AddParameter(new EdmFunctionParameter(within, "Location",
                                                                addressType.ToTypeReference()));
                within.AddParameter(new EdmFunctionParameter(within, "Distance", 
                                                                MetadataUtils.GetPrimitiveTypeReference(typeof(Int32))));
                //Unbound Functions
                var lotsofOrders= metadata.AddFunction("HasLotsOfOrders",
                   null,
                   MetadataUtils.GetPrimitiveTypeReference(typeof(bool)),
                   null,
                   null,
                   false /*isBindable*/);

                lotsofOrders.AddParameter(new EdmFunctionParameter(lotsofOrders, "Person", personType.ToTypeReference()));

                metadata.AddFunction("HowManyPotatoesEaten", null, MetadataUtils.GetPrimitiveTypeReference(typeof(Int32)), null, null, false);
                metadata.AddFunction("QuoteOfTheDay", null, MetadataUtils.GetPrimitiveTypeReference(typeof(string)), null, null, false);

                // ACTIONS

                var action1 = metadata.AddAction("ChangeAddress", personType.ToTypeReference(), null, null, null, true);
                action1.AddParameter(new EdmFunctionParameter(action1, "Street", MetadataUtils.GetPrimitiveTypeReference(typeof (string))));
                action1.AddParameter(new EdmFunctionParameter(action1, "City", MetadataUtils.GetPrimitiveTypeReference(typeof(string))));
                action1.AddParameter(new EdmFunctionParameter(action1, "PostalCode", MetadataUtils.GetPrimitiveTypeReference(typeof(string))));
                
                // Unbound action with no parameters
                metadata.AddAction("GetRecentCustomers",
                                    null,
                                    new EdmCollectionTypeReference(new EdmCollectionType(orderType.ToTypeReference()), false),
                                    orderset,
                                    null,
                                    false);

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
                ((EdmEntitySet)orderdetailsSet).AddNavigationTarget(productOrderedNavigation, orderset);
                
                
                model = metadata;
            }

            return model;
        }
    }
}
