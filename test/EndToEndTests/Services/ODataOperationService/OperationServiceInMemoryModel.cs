//---------------------------------------------------------------------
// <copyright file="OperationServiceInMemoryModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.Test.OData.Services.ODataOperationService
{
    public static class OperationServiceInMemoryModel
    {
        public static IEdmModel CreateODataServiceModel(string ns)
        {
            EdmModel model = new EdmModel();
            var defaultContainer = new EdmEntityContainer(ns, "OperationService");
            model.AddElement(defaultContainer);

            #region ComplexType

            var addressType = new EdmComplexType(ns, "Address");
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Street", EdmCoreModel.Instance.GetString(false)));
            addressType.AddProperty(new EdmStructuralProperty(addressType, "City", EdmCoreModel.Instance.GetString(false)));
            addressType.AddProperty(new EdmStructuralProperty(addressType, "PostalCode", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(addressType);

            var homeAddressType = new EdmComplexType(ns, "HomeAddress", addressType, false);
            homeAddressType.AddProperty(new EdmStructuralProperty(homeAddressType, "FamilyName", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(homeAddressType);

            var companyAddressType = new EdmComplexType(ns, "CompanyAddress", addressType, false);
            companyAddressType.AddProperty(new EdmStructuralProperty(companyAddressType, "CompanyName", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(companyAddressType);

            var orderDetailType = new EdmComplexType(ns, "OrderDetail");
            orderDetailType.AddProperty(new EdmStructuralProperty(orderDetailType, "Quantity", EdmCoreModel.Instance.GetInt32(false)));
            orderDetailType.AddProperty(new EdmStructuralProperty(orderDetailType, "UnitPrice", EdmCoreModel.Instance.GetSingle(false)));
            model.AddElement(orderDetailType);

            var infoFromCustomerType = new EdmComplexType(ns, "InfoFromCustomer");
            infoFromCustomerType.AddProperty(new EdmStructuralProperty(infoFromCustomerType, "CustomerMessage", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(infoFromCustomerType);

            #endregion

            #region EnumType

            var customerLevelType = new EdmEnumType(ns, "CustomerLevel");
            customerLevelType.AddMember("Common", new EdmEnumMemberValue(0));
            customerLevelType.AddMember("Silver", new EdmEnumMemberValue(1));
            customerLevelType.AddMember("Gold", new EdmEnumMemberValue(2));
            model.AddElement(customerLevelType);

            #endregion

            #region EntityType

            var customerType = new EdmEntityType(ns, "Customer");
            var customerIdProperty = new EdmStructuralProperty(customerType, "ID", EdmCoreModel.Instance.GetInt32(false));
            customerType.AddProperty(customerIdProperty);
            customerType.AddKeys(customerIdProperty);
            customerType.AddStructuralProperty("FirstName", EdmCoreModel.Instance.GetString(false));
            customerType.AddStructuralProperty("LastName", EdmCoreModel.Instance.GetString(false));
            customerType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));
            customerType.AddStructuralProperty("Emails", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            customerType.AddStructuralProperty("Level", new EdmEnumTypeReference(customerLevelType, false));
            model.AddElement(customerType);

            var orderType = new EdmEntityType(ns, "Order");
            var orderIdProperty = new EdmStructuralProperty(orderType, "ID", EdmCoreModel.Instance.GetInt32(false));
            orderType.AddProperty(orderIdProperty);
            orderType.AddKeys(orderIdProperty);
            orderType.AddStructuralProperty("OrderDate", EdmCoreModel.Instance.GetDateTimeOffset(false));
            orderType.AddStructuralProperty("Notes", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            orderType.AddStructuralProperty("OrderDetails", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(orderDetailType, true))));
            orderType.AddStructuralProperty("InfoFromCustomer", new EdmComplexTypeReference(infoFromCustomerType, true));
            model.AddElement(orderType);

            var ordersNavigation = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Orders",
                Target = orderType,
                TargetMultiplicity = EdmMultiplicity.Many
            });

            var customerForOrderNavigation = orderType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Customer",
                Target = customerType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            #endregion

            #region EntitySet

            var customerSet = new EdmEntitySet(defaultContainer, "Customers", customerType);
            defaultContainer.AddElement(customerSet);

            var orderSet = new EdmEntitySet(defaultContainer, "Orders", orderType);
            defaultContainer.AddElement(orderSet);

            customerSet.AddNavigationTarget(ordersNavigation, orderSet);
            orderSet.AddNavigationTarget(customerForOrderNavigation, customerSet);

            #endregion

            #region Operations

            //UnBound Action: ResetDataSource
            var resetDataSourceAction = new EdmAction(ns, "ResetDataSource", null, false, null);
            model.AddElement(resetDataSourceAction);
            defaultContainer.AddActionImport(resetDataSourceAction);

            var customerTypeReference = new EdmEntityTypeReference(customerType, true);
            var customerCollectionTypeReference =
                new EdmCollectionTypeReference(new EdmCollectionType(customerTypeReference));
            var addressTypeReference = new EdmComplexTypeReference(addressType, true);
            var addressCollectionTypeRefernce =
                new EdmCollectionTypeReference(new EdmCollectionType(addressTypeReference));
            var stringTypeReference = EdmCoreModel.Instance.GetString(true);
            var stringCollectionTypeReference =
                new EdmCollectionTypeReference(new EdmCollectionType(stringTypeReference));
            var orderTypeReference = new EdmEntityTypeReference(orderType, true);
            var orderCollectionTypeReference =
               new EdmCollectionTypeReference(new EdmCollectionType(orderTypeReference));

            //Bound Function : Bound to Collection Of Entity, Parameter is Complex Value, Return Entity.
            var getCustomerForAddressFunction = new EdmFunction(ns, "GetCustomerForAddress", customerTypeReference, true, new EdmPathExpression("customers"), true);
            getCustomerForAddressFunction.AddParameter("customers", customerCollectionTypeReference);
            getCustomerForAddressFunction.AddParameter("address", addressTypeReference);
            model.AddElement(getCustomerForAddressFunction);

            //Bound Function : Bound to Collection Of Entity, Parameter is Collection of Complex Value, Return Collection Of Entity.
            var getCustomersForAddressesFunction = new EdmFunction(ns, "GetCustomersForAddresses", customerCollectionTypeReference, true, new EdmPathExpression("customers"), true);
            getCustomersForAddressesFunction.AddParameter("customers", customerCollectionTypeReference);
            getCustomersForAddressesFunction.AddParameter("addresses", addressCollectionTypeRefernce);
            model.AddElement(getCustomersForAddressesFunction);

            //Bound Function : Bound to Collection Of Entity, Parameter is Collection of Complex Value, Return Entity
            var getCustomerForAddressesFunction = new EdmFunction(ns, "GetCustomerForAddresses", customerTypeReference, true, new EdmPathExpression("customers"), true);
            getCustomerForAddressesFunction.AddParameter("customers", customerCollectionTypeReference);
            getCustomerForAddressesFunction.AddParameter("addresses", addressCollectionTypeRefernce);
            model.AddElement(getCustomerForAddressesFunction);

            //Bound Function : Bound to Entity, Return Complex Value
            var getCustomerAddressFunction = new EdmFunction(ns, "GetCustomerAddress", addressTypeReference, true, null, true);
            getCustomerAddressFunction.AddParameter("customer", customerTypeReference);
            model.AddElement(getCustomerAddressFunction);

            //Bound Function : Bound to Entity, Parameter is Complex Value, Return Entity
            var verifyCustomerAddressFunction = new EdmFunction(ns, "VerifyCustomerAddress", customerTypeReference, true, new EdmPathExpression("customer"), true);
            verifyCustomerAddressFunction.AddParameter("customer", customerTypeReference);
            verifyCustomerAddressFunction.AddParameter("addresses", addressTypeReference);
            model.AddElement(verifyCustomerAddressFunction);

            //Bound Function : Bound to Entity, Parameter is Entity, Return Entity
            var verifyCustomerByOrderFunction = new EdmFunction(ns, "VerifyCustomerByOrder", customerTypeReference, true, new EdmPathExpression("customer"), true);
            verifyCustomerByOrderFunction.AddParameter("customer", customerTypeReference);
            verifyCustomerByOrderFunction.AddParameter("order", orderTypeReference);
            model.AddElement(verifyCustomerByOrderFunction);

            //Bound Function : Bound to Entity, Parameter is Collection of String, Return Collection of Entity
            var getOrdersFromCustomerByNotesFunction = new EdmFunction(ns, "GetOrdersFromCustomerByNotes", orderCollectionTypeReference, true, new EdmPathExpression("customer/Orders"), true);
            getOrdersFromCustomerByNotesFunction.AddParameter("customer", customerTypeReference);
            getOrdersFromCustomerByNotesFunction.AddParameter("notes", stringCollectionTypeReference);
            model.AddElement(getOrdersFromCustomerByNotesFunction);

            //Bound Function : Bound to Collection Of Entity, Parameter is String, Return Collection of Entity
            var getOrdersByNoteFunction = new EdmFunction(ns, "GetOrdersByNote", orderCollectionTypeReference, true, new EdmPathExpression("orders"), true);
            getOrdersByNoteFunction.AddParameter("orders", orderCollectionTypeReference);
            getOrdersByNoteFunction.AddParameter("note", stringTypeReference);
            model.AddElement(getOrdersByNoteFunction);

            //Bound Function : Bound to Collection Of Entity, Parameter is Collection of String, Return Entity
            var getOrderByNoteFunction = new EdmFunction(ns, "GetOrderByNote", orderTypeReference, true, new EdmPathExpression("orders"), true);
            getOrderByNoteFunction.AddParameter("orders", orderCollectionTypeReference);
            getOrderByNoteFunction.AddParameter("notes", stringCollectionTypeReference);
            model.AddElement(getOrderByNoteFunction);

            //Bound Function : Bound to Collection Of Entity, Parameter is Collection of Entity, Return Collection Of Entity
            var getCustomersByOrdersFunction = new EdmFunction(ns, "GetCustomersByOrders", customerCollectionTypeReference, true, new EdmPathExpression("customers"), true);
            getCustomersByOrdersFunction.AddParameter("customers", customerCollectionTypeReference);
            getCustomersByOrdersFunction.AddParameter("orders", orderCollectionTypeReference);
            model.AddElement(getCustomersByOrdersFunction);

            //Bound Function : Bound to Collection Of Entity, Parameter is Entity, Return Entity
            var getCustomerByOrderFunction = new EdmFunction(ns, "GetCustomerByOrder", customerTypeReference, true, new EdmPathExpression("customers"), true);
            getCustomerByOrderFunction.AddParameter("customers", customerCollectionTypeReference);
            getCustomerByOrderFunction.AddParameter("order", orderTypeReference);
            model.AddElement(getCustomerByOrderFunction);

            // Function Import: Parameter is Collection of Entity, Return Collection Of Entity
            var getCustomersByOrdersUnboundFunction = new EdmFunction(ns, "GetCustomersByOrders", customerCollectionTypeReference, false, null, true);
            getCustomersByOrdersUnboundFunction.AddParameter("orders", orderCollectionTypeReference);
            model.AddElement(getCustomersByOrdersUnboundFunction);
            defaultContainer.AddFunctionImport(getCustomersByOrdersUnboundFunction.Name, getCustomersByOrdersUnboundFunction, new EdmPathExpression("Customers"));

            // Function Import: Parameter is Collection of Entity, Return Entity
            var getCustomerByOrderUnboundFunction = new EdmFunction(ns, "GetCustomerByOrder", customerTypeReference, false, null, true);
            getCustomerByOrderUnboundFunction.AddParameter("order", orderTypeReference);
            model.AddElement(getCustomerByOrderUnboundFunction);
            defaultContainer.AddFunctionImport(getCustomerByOrderUnboundFunction.Name, getCustomerByOrderUnboundFunction, new EdmPathExpression("Customers"));

            // Function Import: Bound to Entity, Return Complex Value
            var getCustomerAddressUnboundFunction = new EdmFunction(ns, "GetCustomerAddress", addressTypeReference, false, null, true);
            getCustomerAddressUnboundFunction.AddParameter("customer", customerTypeReference);
            model.AddElement(getCustomerAddressUnboundFunction);
            defaultContainer.AddFunctionImport(getCustomerAddressUnboundFunction.Name, getCustomerAddressUnboundFunction);

            #endregion

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            if (errors.Any())
            {
                throw new SystemException("The model is not valid, please correct it");
            }

            return model;
        }
    }
}
