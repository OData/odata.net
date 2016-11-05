//---------------------------------------------------------------------
// <copyright file="DefaultInMemoryModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Annotations;
    using Microsoft.OData.Edm.Library.Expressions;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Validation;

    public static class DefaultInMemoryModel
    {
        public static IEdmModel CreateODataServiceModel(string ns)
        {
            EdmModel model = new EdmModel();
            var defaultContainer = new EdmEntityContainer(ns, "InMemoryEntities");
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

            var cityInformationType = new EdmComplexType(ns, "CityInformation");
            cityInformationType.AddProperty(new EdmStructuralProperty(cityInformationType, "CountryRegion", EdmCoreModel.Instance.GetString(false)));
            cityInformationType.AddProperty(new EdmStructuralProperty(cityInformationType, "IsCapital", EdmCoreModel.Instance.GetBoolean(false)));
            model.AddElement(cityInformationType);
            #endregion

            #region EnumType
            var accessLevelType = new EdmEnumType(ns, "AccessLevel", isFlags: true);
            accessLevelType.AddMember("None", new EdmIntegerConstant(0));
            accessLevelType.AddMember("Read", new EdmIntegerConstant(1));
            accessLevelType.AddMember("Write", new EdmIntegerConstant(2));
            accessLevelType.AddMember("Execute", new EdmIntegerConstant(4));
            accessLevelType.AddMember("ReadWrite", new EdmIntegerConstant(3));
            model.AddElement(accessLevelType);

            var colorType = new EdmEnumType(ns, "Color", isFlags: false);
            colorType.AddMember("Red", new EdmIntegerConstant(1));
            colorType.AddMember("Green", new EdmIntegerConstant(2));
            colorType.AddMember("Blue", new EdmIntegerConstant(4));
            model.AddElement(colorType);

            var companyCategory = new EdmEnumType(ns, "CompanyCategory", isFlags: false);
            companyCategory.AddMember("IT", new EdmIntegerConstant(0));
            companyCategory.AddMember("Communication", new EdmIntegerConstant(1));
            companyCategory.AddMember("Electronics", new EdmIntegerConstant(2));
            companyCategory.AddMember("Others", new EdmIntegerConstant(4));
            model.AddElement(companyCategory);
            #endregion

            #region Term
            model.AddElement(new EdmTerm(ns, "IsBoss", EdmCoreModel.Instance.GetBoolean(true), "Entity"));
            model.AddElement(new EdmTerm(ns, "AddressType", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(new EdmTerm(ns, "CityInfo", new EdmComplexTypeReference(cityInformationType, false)));
            model.AddElement(new EdmTerm(ns, "DisplayName", EdmCoreModel.Instance.GetString(true)));
            #endregion

            var personType = new EdmEntityType(ns, "Person");
            var personIdProperty = new EdmStructuralProperty(personType, "PersonID", EdmCoreModel.Instance.GetInt32(false));
            personType.AddProperty(personIdProperty);
            personType.AddKeys(new IEdmStructuralProperty[] { personIdProperty });
            personType.AddProperty(new EdmStructuralProperty(personType, "FirstName", EdmCoreModel.Instance.GetString(false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "LastName", EdmCoreModel.Instance.GetString(false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "MiddleName", EdmCoreModel.Instance.GetString(true)));
            personType.AddProperty(new EdmStructuralProperty(personType, "HomeAddress", new EdmComplexTypeReference(addressType, true)));
            personType.AddProperty(new EdmStructuralProperty(personType, "Home", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true)));
            personType.AddProperty(new EdmStructuralProperty(personType, "Numbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false)))));
            personType.AddProperty(new EdmStructuralProperty(personType, "Emails", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true)))));
            personType.AddProperty(new EdmStructuralProperty(personType, "Addresses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressType, true)))));
           
            model.AddElement(personType);
            var personSet = new EdmEntitySet(defaultContainer, "People", personType);
            defaultContainer.AddElement(personSet);
            var boss = new EdmSingleton(defaultContainer, "Boss", personType);
            defaultContainer.AddElement(boss);

            var customerType = new EdmEntityType(ns, "Customer", personType);
            customerType.AddProperty(new EdmStructuralProperty(customerType, "City", EdmCoreModel.Instance.GetString(false)));
            customerType.AddProperty(new EdmStructuralProperty(customerType, "Birthday", EdmCoreModel.Instance.GetDateTimeOffset(false)));
            customerType.AddProperty(new EdmStructuralProperty(customerType, "TimeBetweenLastTwoOrders", EdmCoreModel.Instance.GetDuration(false)));
            model.AddElement(customerType);
            var customerSet = new EdmEntitySet(defaultContainer, "Customers", customerType);
            defaultContainer.AddElement(customerSet);

            EdmSingleton vipCustomer = new EdmSingleton(defaultContainer, "VipCustomer", customerType);
            defaultContainer.AddElement(vipCustomer);

            var employeeType = new EdmEntityType(ns, "Employee", personType);
            employeeType.AddProperty(new EdmStructuralProperty(employeeType, "DateHired", EdmCoreModel.Instance.GetDateTimeOffset(false)));
            employeeType.AddProperty(new EdmStructuralProperty(employeeType, "Office", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true)));
            model.AddElement(employeeType);
            var employeeSet = new EdmEntitySet(defaultContainer, "Employees", employeeType);
            defaultContainer.AddElement(employeeSet);

            var productType = new EdmEntityType(ns, "Product");
            var productIdProperty = new EdmStructuralProperty(productType, "ProductID", EdmCoreModel.Instance.GetInt32(false));
            productType.AddProperty(productIdProperty);
            productType.AddKeys(productIdProperty);
            productType.AddProperty(new EdmStructuralProperty(productType, "Name", EdmCoreModel.Instance.GetString(false)));
            productType.AddProperty(new EdmStructuralProperty(productType, "QuantityPerUnit", EdmCoreModel.Instance.GetString(false)));
            productType.AddProperty(new EdmStructuralProperty(productType, "UnitPrice", EdmCoreModel.Instance.GetSingle(false)));
            productType.AddProperty(new EdmStructuralProperty(productType, "QuantityInStock", EdmCoreModel.Instance.GetInt32(false)));
            productType.AddProperty(new EdmStructuralProperty(productType, "Discontinued", EdmCoreModel.Instance.GetBoolean(false)));
            productType.AddProperty(new EdmStructuralProperty(productType, "UserAccess", new EdmEnumTypeReference(accessLevelType, true)));
            productType.AddProperty(new EdmStructuralProperty(productType, "SkinColor", new EdmEnumTypeReference(colorType, true)));
            productType.AddProperty(new EdmStructuralProperty(productType, "CoverColors", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEnumTypeReference(colorType, false)))));
            model.AddElement(productType);
            var productSet = new EdmEntitySet(defaultContainer, "Products", productType);
            defaultContainer.AddElement(productSet);

            var productDetailType = new EdmEntityType(ns, "ProductDetail");
            var productDetailIdProperty1 = new EdmStructuralProperty(productDetailType, "ProductID", EdmCoreModel.Instance.GetInt32(false));
            var productDetailIdProperty2 = new EdmStructuralProperty(productDetailType, "ProductDetailID", EdmCoreModel.Instance.GetInt32(false));
            productDetailType.AddProperty(productDetailIdProperty1);
            productDetailType.AddKeys(productDetailIdProperty1);
            productDetailType.AddProperty(productDetailIdProperty2);
            productDetailType.AddKeys(productDetailIdProperty2);
            productDetailType.AddProperty(new EdmStructuralProperty(productDetailType, "ProductName", EdmCoreModel.Instance.GetString(false)));
            productDetailType.AddProperty(new EdmStructuralProperty(productDetailType, "Description", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(productDetailType);
            var productDetailSet = new EdmEntitySet(defaultContainer, "ProductDetails", productDetailType);
            defaultContainer.AddElement(productDetailSet);

            var productReviewType = new EdmEntityType(ns, "ProductReview");
            var productReviewIdProperty1 = new EdmStructuralProperty(productReviewType, "ProductID", EdmCoreModel.Instance.GetInt32(false));
            var productReviewIdProperty2 = new EdmStructuralProperty(productReviewType, "ProductDetailID", EdmCoreModel.Instance.GetInt32(false));
            var productReviewIdProperty3 = new EdmStructuralProperty(productReviewType, "ReviewTitle", EdmCoreModel.Instance.GetString(false));
            var productReviewIdProperty4 = new EdmStructuralProperty(productReviewType, "RevisionID", EdmCoreModel.Instance.GetInt32(false));
            productReviewType.AddProperty(productReviewIdProperty1);
            productReviewType.AddKeys(productReviewIdProperty1);
            productReviewType.AddProperty(productReviewIdProperty2);
            productReviewType.AddKeys(productReviewIdProperty2);
            productReviewType.AddProperty(productReviewIdProperty3);
            productReviewType.AddKeys(productReviewIdProperty3);
            productReviewType.AddProperty(productReviewIdProperty4);
            productReviewType.AddKeys(productReviewIdProperty4);
            productReviewType.AddProperty(new EdmStructuralProperty(productReviewType, "Comment", EdmCoreModel.Instance.GetString(false)));
            productReviewType.AddProperty(new EdmStructuralProperty(productReviewType, "Author", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(productReviewType);
            var productReviewSet = new EdmEntitySet(defaultContainer, "ProductReviews", productReviewType);
            defaultContainer.AddElement(productReviewSet);

            var abstractType = new EdmEntityType(ns, "AbstractEntity", null, true, false);
            model.AddElement(abstractType);

            var orderType = new EdmEntityType(ns, "Order", abstractType);
            var orderIdProperty = new EdmStructuralProperty(orderType, "OrderID", EdmCoreModel.Instance.GetInt32(false));
            orderType.AddProperty(orderIdProperty);
            orderType.AddKeys(orderIdProperty);
            orderType.AddProperty(new EdmStructuralProperty(orderType, "OrderDate", EdmCoreModel.Instance.GetDateTimeOffset(false)));
            orderType.AddProperty(new EdmStructuralProperty(orderType, "ShelfLife", EdmCoreModel.Instance.GetDuration(true)));
            orderType.AddProperty(new EdmStructuralProperty(orderType, "OrderShelfLifes", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDuration(true)))));
            orderType.AddProperty(new EdmStructuralProperty(orderType, "ShipDate", EdmCoreModel.Instance.GetDate(false)));
            orderType.AddProperty(new EdmStructuralProperty(orderType, "ShipTime", EdmCoreModel.Instance.GetTimeOfDay(false)));

            model.AddElement(orderType);
            var orderSet = new EdmEntitySet(defaultContainer, "Orders", orderType);
            defaultContainer.AddElement(orderSet);

            var calendarType = new EdmEntityType(ns, "Calendar", abstractType);
            var calendarIdProperty = new EdmStructuralProperty(calendarType, "Day", EdmCoreModel.Instance.GetDate(false));
            calendarType.AddProperty(calendarIdProperty);
            calendarType.AddKeys(calendarIdProperty);
            model.AddElement(calendarType);
            var calendarSet = new EdmEntitySet(defaultContainer, "Calendars", calendarType);
            defaultContainer.AddElement(calendarSet);

            var orderDetailType = new EdmEntityType(ns, "OrderDetail", abstractType);
            var orderId = new EdmStructuralProperty(orderDetailType, "OrderID", EdmCoreModel.Instance.GetInt32(false));
            orderDetailType.AddProperty(orderId);
            orderDetailType.AddKeys(orderId);
            var productId = new EdmStructuralProperty(orderDetailType, "ProductID", EdmCoreModel.Instance.GetInt32(false));
            orderDetailType.AddProperty(productId);
            orderDetailType.AddKeys(productId);
            orderDetailType.AddProperty(new EdmStructuralProperty(orderDetailType, "OrderPlaced", EdmCoreModel.Instance.GetDateTimeOffset(false)));
            orderDetailType.AddProperty(new EdmStructuralProperty(orderDetailType, "Quantity", EdmCoreModel.Instance.GetInt32(false)));
            orderDetailType.AddProperty(new EdmStructuralProperty(orderDetailType, "UnitPrice", EdmCoreModel.Instance.GetSingle(false)));

            model.AddElement(orderDetailType);
            var orderDetailSet = new EdmEntitySet(defaultContainer, "OrderDetails", orderDetailType);
            defaultContainer.AddElement(orderDetailSet);

            var parentNavigation = personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Parent",
                Target = personType,
                TargetMultiplicity = EdmMultiplicity.One
            });
            var productOrderedNavigation = orderDetailType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "ProductOrdered",
                Target = productType,
                TargetMultiplicity = EdmMultiplicity.Many
            });

            var associatedOrderNavigation = orderDetailType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "AssociatedOrder",
                Target = orderType,
                TargetMultiplicity = EdmMultiplicity.One
            });
            var loggedInEmployeeNavigation = orderType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "LoggedInEmployee",
                Target = employeeType,
                TargetMultiplicity = EdmMultiplicity.One
            });
            var customerForOrderNavigation = orderType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "CustomerForOrder",
                Target = customerType,
                TargetMultiplicity = EdmMultiplicity.One
            });
            var orderDetailsNavigation = orderType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "OrderDetails",
                Target = orderDetailType,
                TargetMultiplicity = EdmMultiplicity.Many
            });
            var ordersNavigation = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Orders",
                Target = orderType,
                TargetMultiplicity = EdmMultiplicity.Many
            });
            var productProductDetailNavigation = productType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Details",
                TargetMultiplicity = EdmMultiplicity.Many,
                Target = productDetailType,
                DependentProperties = new List<IEdmStructuralProperty>()
                    {
                        productIdProperty
                    },
                PrincipalProperties = new List<IEdmStructuralProperty>()
                    {
                        productDetailIdProperty1
                    }
            });
            var productDetailProductNavigation = productDetailType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "RelatedProduct",
                Target = productType,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne
            });
            var productDetailProductReviewNavigation = productDetailType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Reviews",
                TargetMultiplicity = EdmMultiplicity.Many,
                Target = productReviewType,
                DependentProperties = new List<IEdmStructuralProperty>()
                    {
                        productDetailIdProperty1, 
                        productDetailIdProperty2,
                    },
                PrincipalProperties = new List<IEdmStructuralProperty>()
                    {
                        productReviewIdProperty1, 
                        productReviewIdProperty2
                    }
            });

            model.SetCoreChangeTrackingAnnotation(orderSet, new EdmStructuralProperty[] { orderIdProperty }, new EdmNavigationProperty[] { orderDetailsNavigation });

            ((EdmEntitySet)personSet).AddNavigationTarget(parentNavigation, personSet);
            ((EdmEntitySet)orderDetailSet).AddNavigationTarget(associatedOrderNavigation, orderSet);
            ((EdmEntitySet)orderDetailSet).AddNavigationTarget(productOrderedNavigation, productSet);
            ((EdmEntitySet)customerSet).AddNavigationTarget(ordersNavigation, orderSet);
            ((EdmEntitySet)customerSet).AddNavigationTarget(parentNavigation, personSet);
            ((EdmEntitySet)employeeSet).AddNavigationTarget(parentNavigation, personSet);
            ((EdmEntitySet)orderSet).AddNavigationTarget(loggedInEmployeeNavigation, employeeSet);
            ((EdmEntitySet)orderSet).AddNavigationTarget(customerForOrderNavigation, customerSet);
            ((EdmEntitySet)orderSet).AddNavigationTarget(orderDetailsNavigation, orderDetailSet);
            ((EdmEntitySet)productSet).AddNavigationTarget(productProductDetailNavigation, productDetailSet);
            ((EdmEntitySet)productDetailSet).AddNavigationTarget(productDetailProductNavigation, productSet);
            ((EdmEntitySet)productDetailSet).AddNavigationTarget(productDetailProductReviewNavigation, productReviewSet);

            #region Singleton

            var departmentType = new EdmEntityType(ns, "Department", null);
            var departmentId = new EdmStructuralProperty(departmentType, "DepartmentID", EdmCoreModel.Instance.GetInt32(false));
            departmentType.AddProperty(departmentId);
            departmentType.AddKeys(departmentId);
            departmentType.AddProperty(new EdmStructuralProperty(departmentType, "Name", EdmCoreModel.Instance.GetString(false)));
            departmentType.AddProperty(new EdmStructuralProperty(departmentType, "DepartmentNO", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(departmentType);
            EdmEntitySet departments = new EdmEntitySet(defaultContainer, "Departments", departmentType);
            defaultContainer.AddElement(departments);

            var companyType = new EdmEntityType(ns, "Company", /*baseType*/ null, /*isAbstract*/ false, /*isOpen*/ true);
            var companyId = new EdmStructuralProperty(companyType, "CompanyID", EdmCoreModel.Instance.GetInt32(false));
            companyType.AddProperty(companyId);
            companyType.AddKeys(companyId);
            companyType.AddProperty(new EdmStructuralProperty(companyType, "CompanyCategory", new EdmEnumTypeReference(companyCategory, true)));
            companyType.AddProperty(new EdmStructuralProperty(companyType, "Revenue", EdmCoreModel.Instance.GetInt64(false)));
            companyType.AddProperty(new EdmStructuralProperty(companyType, "Name", EdmCoreModel.Instance.GetString(true)));
            companyType.AddProperty(new EdmStructuralProperty(companyType, "Address", new EdmComplexTypeReference(addressType, true)));

            model.AddElement(companyType);
            EdmSingleton company = new EdmSingleton(defaultContainer, "Company", companyType);
            defaultContainer.AddElement(company);

            var publicCompanyType = new EdmEntityType(ns, "PublicCompany", /*baseType*/ companyType, /*isAbstract*/ false, /*isOpen*/ true);
            publicCompanyType.AddProperty(new EdmStructuralProperty(publicCompanyType, "StockExchange", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(publicCompanyType);
            EdmSingleton publicCompany = new EdmSingleton(defaultContainer, "PublicCompany", companyType);
            defaultContainer.AddElement(publicCompany);

            var assetType = new EdmEntityType(ns, "Asset", /*baseType*/ null, /*isAbstract*/ false, /*isOpen*/ false);
            var assetId = new EdmStructuralProperty(assetType, "AssetID", EdmCoreModel.Instance.GetInt32(false));
            assetType.AddProperty(assetId);
            assetType.AddKeys(assetId);
            assetType.AddProperty(new EdmStructuralProperty(assetType, "Name", EdmCoreModel.Instance.GetString(true)));
            assetType.AddProperty(new EdmStructuralProperty(assetType, "Number", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(assetType);

            var clubType = new EdmEntityType(ns, "Club", /*baseType*/ null, /*isAbstract*/ false, /*isOpen*/ false);
            var clubId = new EdmStructuralProperty(clubType, "ClubID", EdmCoreModel.Instance.GetInt32(false));
            clubType.AddProperty(clubId);
            clubType.AddKeys(clubId);
            clubType.AddProperty(new EdmStructuralProperty(clubType, "Name", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(clubType);

            var labourUnionType = new EdmEntityType(ns, "LabourUnion", /*baseType*/ null, /*isAbstract*/ false, /*isOpen*/ false);
            var labourUnionId = new EdmStructuralProperty(labourUnionType, "LabourUnionID", EdmCoreModel.Instance.GetInt32(false));
            labourUnionType.AddProperty(labourUnionId);
            labourUnionType.AddKeys(labourUnionId);
            labourUnionType.AddProperty(new EdmStructuralProperty(labourUnionType, "Name", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(labourUnionType);
            EdmSingleton labourUnion = new EdmSingleton(defaultContainer, "LabourUnion", labourUnionType);
            defaultContainer.AddElement(labourUnion);

            var companyEmployeeNavigation = companyType.AddBidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Employees",
                Target = employeeType,
                TargetMultiplicity = EdmMultiplicity.Many
            }, new EdmNavigationPropertyInfo()
            {
                Name = "Company",
                Target = companyType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            var companyCustomerNavigation = companyType.AddBidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "VipCustomer",
                Target = customerType,
                TargetMultiplicity = EdmMultiplicity.One
            }, new EdmNavigationPropertyInfo()
            {
                Name = "Company",
                Target = companyType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            var companyDepartmentsNavigation = companyType.AddBidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Departments",
                Target = departmentType,
                TargetMultiplicity = EdmMultiplicity.Many
            },
            new EdmNavigationPropertyInfo()
            {
                Name = "Company",
                Target = companyType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            var companyCoreDepartmentNavigation = companyType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "CoreDepartment",
                Target = departmentType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            var publicCompanyAssetNavigation = publicCompanyType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Assets",
                Target = assetType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true
            });

            var publicCompanyClubNavigation = publicCompanyType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Club",
                Target = clubType,
                TargetMultiplicity = EdmMultiplicity.One,
                ContainsTarget = true
            });

            var publicCompanyLabourUnionNavigation = publicCompanyType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "LabourUnion",
                Target = labourUnionType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            //vipCustomer->orders
            ((EdmSingleton)vipCustomer).AddNavigationTarget(ordersNavigation, orderSet);
            //vipCustomer->people
            ((EdmSingleton)vipCustomer).AddNavigationTarget(parentNavigation, personSet);
            ((EdmSingleton)boss).AddNavigationTarget(parentNavigation, personSet);
            //employeeSet<->company
            ((EdmSingleton)company).AddNavigationTarget(companyEmployeeNavigation, employeeSet);
            ((EdmEntitySet)employeeSet).AddNavigationTarget(companyEmployeeNavigation.Partner, company);
            //company<->vipcustomer
            ((EdmSingleton)company).AddNavigationTarget(companyCustomerNavigation, vipCustomer);
            ((EdmSingleton)vipCustomer).AddNavigationTarget(companyCustomerNavigation.Partner, company);
            //company<->departments
            ((EdmSingleton)company).AddNavigationTarget(companyDepartmentsNavigation, departments);
            ((EdmEntitySet)departments).AddNavigationTarget(companyDepartmentsNavigation.Partner, company);
            //company<-> Single department
            ((EdmSingleton)company).AddNavigationTarget(companyCoreDepartmentNavigation, departments);
            //publicCompany<-> Singleton
            ((EdmSingleton)publicCompany).AddNavigationTarget(publicCompanyLabourUnionNavigation, labourUnion);

            #region Action/Function
            //Bound Action : bound to Entity, return EnumType
            var productAddAccessRightAction = new EdmAction(ns, "AddAccessRight", new EdmEnumTypeReference(accessLevelType, true), true, null);
            productAddAccessRightAction.AddParameter("product", new EdmEntityTypeReference(productType, false));
            productAddAccessRightAction.AddParameter("accessRight", new EdmEnumTypeReference(accessLevelType, true));
            model.AddElement(productAddAccessRightAction);

            //Bound Action : Bound to Singleton, Primitive Parameter return Primitive Type
            EdmAction increaseRevenue = new EdmAction(ns, "IncreaseRevenue", EdmCoreModel.Instance.GetInt64(false), /*isBound*/true, /*entitySetPathExpression*/null);
            increaseRevenue.AddParameter(new EdmOperationParameter(increaseRevenue, "p", new EdmEntityTypeReference(companyType, false)));
            increaseRevenue.AddParameter(new EdmOperationParameter(increaseRevenue, "IncreaseValue", EdmCoreModel.Instance.GetInt64(true)));
            model.AddElement(increaseRevenue);

            //Bound Action : Bound to Entity, Collection Of ComplexType Parameter, Return Entity
            var resetAddressAction = new EdmAction(ns, "ResetAddress", new EdmEntityTypeReference(personType, false), true, new EdmPathExpression("person"));
            resetAddressAction.AddParameter("person", new EdmEntityTypeReference(personType, false));
            resetAddressAction.AddParameter("addresses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressType, true))));
            resetAddressAction.AddParameter("index", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(resetAddressAction);

            //Bound Action : Bound to Entity, EntityType Parameter, Return Entity
            var placeOrderAction = new EdmAction(ns, "PlaceOrder", new EdmEntityTypeReference(orderType, false), true, new EdmPathExpression("customer/Orders"));
            placeOrderAction.AddParameter("customer", new EdmEntityTypeReference(customerType, false));
            placeOrderAction.AddParameter("order", new EdmEntityTypeReference(orderType, false));
            model.AddElement(placeOrderAction);

            //Bound Action : Bound to Entity, Collection Of EntityType Parameter, Return Collection of Entity
            var placeOrdersAction = new EdmAction(ns, "PlaceOrders", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(orderType, false))), true, new EdmPathExpression("customer/Orders"));
            placeOrdersAction.AddParameter("customer", new EdmEntityTypeReference(customerType, false));
            placeOrdersAction.AddParameter("orders", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(orderType, false))));
            model.AddElement(placeOrdersAction);

            //Bound Action : Bound to collection of EntitySet, Return Collection of Entity
            var discountSeveralProductAction = new EdmAction(ns, "Discount",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(productType, false))), true, new EdmPathExpression("products"));
            discountSeveralProductAction.AddParameter("products", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(productType, false))));
            discountSeveralProductAction.AddParameter("percentage", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(discountSeveralProductAction);

            //Bound Action : Bound to Entity, return void
            var changeLabourUnionNameAction = new EdmAction(ns, "ChangeLabourUnionName", null, true, null);
            changeLabourUnionNameAction.AddParameter("labourUnion", new EdmEntityTypeReference(labourUnionType, false));
            changeLabourUnionNameAction.AddParameter("name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(changeLabourUnionNameAction);

            //Bound Action : Bound to Entity, return order take Date/TimeOfDay as Parameter
            var changeShipTimeAndDate = new EdmAction(ns, "ChangeShipTimeAndDate", new EdmEntityTypeReference(orderType, false), true, new EdmPathExpression("order"));
            changeShipTimeAndDate.AddParameter("order", new EdmEntityTypeReference(orderType, false));
            changeShipTimeAndDate.AddParameter("date", EdmCoreModel.Instance.GetDate(false));
            changeShipTimeAndDate.AddParameter("time", EdmCoreModel.Instance.GetTimeOfDay(false));
            model.AddElement(changeShipTimeAndDate);

            //UnBound Action : Primitive parameter, Return void
            var discountAction = new EdmAction(ns, "Discount", null, false, null);
            discountAction.AddParameter("percentage", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(discountAction);
            defaultContainer.AddActionImport(discountAction);

            //UnBound Action : Collection of Primitive parameter, Return Collection of Primitive
            var resetBossEmailAction = new EdmAction(ns, "ResetBossEmail", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))), false, null);
            resetBossEmailAction.AddParameter("emails", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))));
            model.AddElement(resetBossEmailAction);
            defaultContainer.AddActionImport(resetBossEmailAction);

            //UnBound Action : ComplexType parameter, Return ComplexType
            var resetBossAddressAction = new EdmAction(ns, "ResetBossAddress", new EdmComplexTypeReference(addressType, false), false, null);
            resetBossAddressAction.AddParameter("address", new EdmComplexTypeReference(addressType, false));
            model.AddElement(resetBossAddressAction);
            defaultContainer.AddActionImport(resetBossAddressAction);

            //UnBound Action: ResetDataSource
            var resetDataSourceAction = new EdmAction(ns, "ResetDataSource", null, false, null);
            model.AddElement(resetDataSourceAction);
            defaultContainer.AddActionImport(resetDataSourceAction);

            //Bound Function : Bound to Singleton, Return PrimitiveType
            EdmFunction getCompanyEmployeeCount = new EdmFunction(ns, "GetEmployeesCount", EdmCoreModel.Instance.GetInt32(false), /*isBound*/true, /*entitySetPathExpression*/null, /*isComposable*/false);
            getCompanyEmployeeCount.AddParameter(new EdmOperationParameter(getCompanyEmployeeCount, "p", new EdmEntityTypeReference(companyType, false)));
            model.AddElement(getCompanyEmployeeCount);

            //Bound Function : Bound to Entity, Return CollectionOfEntity
            var getProductDetailsFunction = new EdmFunction(ns, "GetProductDetails",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(productDetailType, false))),
                true, new EdmPathExpression("product/Details"), true);
            getProductDetailsFunction.AddParameter("product", new EdmEntityTypeReference(productType, false));
            getProductDetailsFunction.AddParameter("count", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(getProductDetailsFunction);

            //Bound Function : Bound to Entity, Return Entity
            var getRelatedProductFunction = new EdmFunction(ns, "GetRelatedProduct",
                new EdmEntityTypeReference(productType, false),
                true, new EdmPathExpression("productDetail/RelatedProduct"), true);
            getRelatedProductFunction.AddParameter("productDetail", new EdmEntityTypeReference(productDetailType, false));
            model.AddElement(getRelatedProductFunction);

            //Bound Function : Bound to Entity, Return Collection of Abstract Entity
            var getOrderAndOrderDetails = new EdmFunction(ns, "getOrderAndOrderDetails",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(abstractType, false))),
                true, new EdmPathExpression("customer/Orders"), true);
            getOrderAndOrderDetails.AddParameter("customer", new EdmEntityTypeReference(customerType, false));
            model.AddElement(getOrderAndOrderDetails);

            //Bound Function : Bound to CollectionOfEntity, Return Entity
            var getSeniorEmployees = new EdmFunction(ns, "GetSeniorEmployees",
                new EdmEntityTypeReference(employeeType, true),
                true, new EdmPathExpression("employees"), true);
            getSeniorEmployees.AddParameter("employees", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(employeeType, false))));
            model.AddElement(getSeniorEmployees);

            //Bound Function : Bound to Order, Return Edm.Date
            var getOrderShipDate = new EdmFunction(ns, "GetShipDate", EdmCoreModel.Instance.GetDate(false), true, null, false);
            getOrderShipDate.AddParameter("order", new EdmEntityTypeReference(orderType, false));
            model.AddElement(getOrderShipDate);

            //Bound Function : Bound to Order, Return Edm.TimeOfDay
            var getOrderShipTime = new EdmFunction(ns, "GetShipTime", EdmCoreModel.Instance.GetTimeOfDay(false), true, null, false);
            getOrderShipTime.AddParameter("order", new EdmEntityTypeReference(orderType, false));
            model.AddElement(getOrderShipTime);

            //Bound Function : Bound to Order, Parameter: Edm.TimeOfDay, Return Edm.Boolean
            var checkOrderShipTime = new EdmFunction(ns, "CheckShipTime", EdmCoreModel.Instance.GetBoolean(false), true, null, false);
            checkOrderShipTime.AddParameter("order", new EdmEntityTypeReference(orderType, false));
            checkOrderShipTime.AddParameter("time", EdmCoreModel.Instance.GetTimeOfDay(false));
            model.AddElement(checkOrderShipTime);

            //Bound Function : Bound to Order, Parameter: Edm.Date, Return Edm.Boolean
            var checkOrderShipDate = new EdmFunction(ns, "CheckShipDate", EdmCoreModel.Instance.GetBoolean(false), true, null, false);
            checkOrderShipDate.AddParameter("order", new EdmEntityTypeReference(orderType, false));
            checkOrderShipDate.AddParameter("date", EdmCoreModel.Instance.GetDate(false));
            model.AddElement(checkOrderShipDate);

            //UnBound Function : Return EnumType
            var defaultColorFunction = new EdmFunction(ns, "GetDefaultColor", new EdmEnumTypeReference(colorType, true), false, null, true);
            model.AddElement(defaultColorFunction);
            defaultContainer.AddFunctionImport("GetDefaultColor", defaultColorFunction, null, true);

            //UnBound Function : Complex Parameter, Return Entity
            var getPersonFunction = new EdmFunction(ns, "GetPerson",
                new EdmEntityTypeReference(personType, false),
                false, null, true);
            getPersonFunction.AddParameter("address", new EdmComplexTypeReference(addressType, false));
            model.AddElement(getPersonFunction);
            defaultContainer.AddFunctionImport("GetPerson", getPersonFunction, new EdmEntitySetReferenceExpression(personSet), true);

            //UnBound Function : Primtive Parameter, Return Entity
            var getPersonFunction2 = new EdmFunction(ns, "GetPerson2",
                new EdmEntityTypeReference(personType, false),
                false, null, true);
            getPersonFunction2.AddParameter("city", EdmCoreModel.Instance.GetString(false));
            model.AddElement(getPersonFunction2);
            defaultContainer.AddFunctionImport("GetPerson2", getPersonFunction2, new EdmEntitySetReferenceExpression(personSet), true);

            //UnBound Function : Return CollectionOfEntity
            var getAllProductsFunction = new EdmFunction(ns, "GetAllProducts",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(productType, false))),
                false, null, true);
            model.AddElement(getAllProductsFunction);
            defaultContainer.AddFunctionImport("GetAllProducts", getAllProductsFunction, new EdmEntitySetReferenceExpression(productSet), true);

            //UnBound Function : Multi ParameterS Return Collection Of ComplexType
            var getBossEmailsFunction = new EdmFunction(ns, "GetBossEmails",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))),
                false, null, false);
            getBossEmailsFunction.AddParameter("start", EdmCoreModel.Instance.GetInt32(false));
            getBossEmailsFunction.AddParameter("count", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(getBossEmailsFunction);
            defaultContainer.AddFunctionImport(getBossEmailsFunction.Name, getBossEmailsFunction, null, true);

            var getProductsByAccessLevelFunction = new EdmFunction(ns, "GetProductsByAccessLevel",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))),
                false, null, false);
            getProductsByAccessLevelFunction.AddParameter("accessLevel", new EdmEnumTypeReference(accessLevelType, false));
            model.AddElement(getProductsByAccessLevelFunction);
            defaultContainer.AddFunctionImport(getProductsByAccessLevelFunction.Name, getProductsByAccessLevelFunction, null, true);

            #endregion

            #endregion

            #region For containment

            var accountInfoType = new EdmComplexType(ns, "AccountInfo", null, false, true);
            accountInfoType.AddProperty(new EdmStructuralProperty(accountInfoType, "FirstName", EdmCoreModel.Instance.GetString(false)));
            accountInfoType.AddProperty(new EdmStructuralProperty(accountInfoType, "LastName", EdmCoreModel.Instance.GetString(false)));

            var accountType = new EdmEntityType(ns, "Account");
            var accountIdProperty = new EdmStructuralProperty(accountType, "AccountID", EdmCoreModel.Instance.GetInt32(false));
            accountType.AddProperty(accountIdProperty);
            accountType.AddKeys(accountIdProperty);
            accountType.AddProperty(new EdmStructuralProperty(accountType, "CountryRegion", EdmCoreModel.Instance.GetString(false)));
            accountType.AddProperty(new EdmStructuralProperty(accountType, "AccountInfo", new EdmComplexTypeReference(accountInfoType, true)));

            var giftCardType = new EdmEntityType(ns, "GiftCard");
            var giftCardIdProperty = new EdmStructuralProperty(giftCardType, "GiftCardID", EdmCoreModel.Instance.GetInt32(false));
            giftCardType.AddProperty(giftCardIdProperty);
            giftCardType.AddKeys(giftCardIdProperty);
            giftCardType.AddProperty(new EdmStructuralProperty(giftCardType, "GiftCardNO", EdmCoreModel.Instance.GetString(false)));
            giftCardType.AddProperty(new EdmStructuralProperty(giftCardType, "Amount", EdmCoreModel.Instance.GetDouble(false)));
            giftCardType.AddProperty(new EdmStructuralProperty(giftCardType, "ExperationDate", EdmCoreModel.Instance.GetDateTimeOffset(false)));
            giftCardType.AddProperty(new EdmStructuralProperty(giftCardType, "OwnerName", EdmCoreModel.Instance.GetString(true)));


            var paymentInstrumentType = new EdmEntityType(ns, "PaymentInstrument");
            var paymentInstrumentIdProperty = new EdmStructuralProperty(paymentInstrumentType, "PaymentInstrumentID", EdmCoreModel.Instance.GetInt32(false));
            paymentInstrumentType.AddProperty(paymentInstrumentIdProperty);
            paymentInstrumentType.AddKeys(paymentInstrumentIdProperty);
            paymentInstrumentType.AddProperty(new EdmStructuralProperty(paymentInstrumentType, "FriendlyName", EdmCoreModel.Instance.GetString(false)));
            paymentInstrumentType.AddProperty(new EdmStructuralProperty(paymentInstrumentType, "CreatedDate", EdmCoreModel.Instance.GetDateTimeOffset(false)));

            var creditCardType = new EdmEntityType(ns, "CreditCardPI", paymentInstrumentType);
            creditCardType.AddProperty(new EdmStructuralProperty(creditCardType, "CardNumber", EdmCoreModel.Instance.GetString(false)));
            creditCardType.AddProperty(new EdmStructuralProperty(creditCardType, "CVV", EdmCoreModel.Instance.GetString(false)));
            creditCardType.AddProperty(new EdmStructuralProperty(creditCardType, "HolderName", EdmCoreModel.Instance.GetString(false)));
            creditCardType.AddProperty(new EdmStructuralProperty(creditCardType, "Balance", EdmCoreModel.Instance.GetDouble(false)));
            creditCardType.AddProperty(new EdmStructuralProperty(creditCardType, "ExperationDate", EdmCoreModel.Instance.GetDateTimeOffset(false)));

            var storedPIType = new EdmEntityType(ns, "StoredPI");
            var storedPIIdProperty = new EdmStructuralProperty(storedPIType, "StoredPIID", EdmCoreModel.Instance.GetInt32(false));
            storedPIType.AddProperty(storedPIIdProperty);
            storedPIType.AddKeys(storedPIIdProperty);
            storedPIType.AddProperty(new EdmStructuralProperty(storedPIType, "PIName", EdmCoreModel.Instance.GetString(false)));
            storedPIType.AddProperty(new EdmStructuralProperty(storedPIType, "PIType", EdmCoreModel.Instance.GetString(false)));
            storedPIType.AddProperty(new EdmStructuralProperty(storedPIType, "CreatedDate", EdmCoreModel.Instance.GetDateTimeOffset(false)));

            var statementType = new EdmEntityType(ns, "Statement");
            var statementIdProperty = new EdmStructuralProperty(statementType, "StatementID", EdmCoreModel.Instance.GetInt32(false));
            statementType.AddProperty(statementIdProperty);
            statementType.AddKeys(statementIdProperty);
            statementType.AddProperty(new EdmStructuralProperty(statementType, "TransactionType", EdmCoreModel.Instance.GetString(false)));
            statementType.AddProperty(new EdmStructuralProperty(statementType, "TransactionDescription", EdmCoreModel.Instance.GetString(false)));
            statementType.AddProperty(new EdmStructuralProperty(statementType, "Amount", EdmCoreModel.Instance.GetDouble(false)));

            var creditRecordType = new EdmEntityType(ns, "CreditRecord");
            var creditRecordIdProperty = new EdmStructuralProperty(creditRecordType, "CreditRecordID", EdmCoreModel.Instance.GetInt32(false));
            creditRecordType.AddProperty(creditRecordIdProperty);
            creditRecordType.AddKeys(creditRecordIdProperty);
            creditRecordType.AddProperty(new EdmStructuralProperty(creditRecordType, "IsGood", EdmCoreModel.Instance.GetBoolean(false)));
            creditRecordType.AddProperty(new EdmStructuralProperty(creditRecordType, "Reason", EdmCoreModel.Instance.GetString(false)));
            creditRecordType.AddProperty(new EdmStructuralProperty(creditRecordType, "CreatedDate", EdmCoreModel.Instance.GetDateTimeOffset(false)));

            var subscriptionType = new EdmEntityType(ns, "Subscription");
            var subscriptionIdProperty = new EdmStructuralProperty(subscriptionType, "SubscriptionID", EdmCoreModel.Instance.GetInt32(false));
            subscriptionType.AddProperty(subscriptionIdProperty);
            subscriptionType.AddKeys(subscriptionIdProperty);
            subscriptionType.AddProperty(new EdmStructuralProperty(subscriptionType, "TemplateGuid", EdmCoreModel.Instance.GetString(false)));
            subscriptionType.AddProperty(new EdmStructuralProperty(subscriptionType, "Title", EdmCoreModel.Instance.GetString(false)));
            subscriptionType.AddProperty(new EdmStructuralProperty(subscriptionType, "Category", EdmCoreModel.Instance.GetString(false)));
            subscriptionType.AddProperty(new EdmStructuralProperty(subscriptionType, "CreatedDate", EdmCoreModel.Instance.GetDateTimeOffset(false)));

            #region Functions/Actions
            var giftCardAmountFunction = new EdmFunction(ns, "GetActualAmount", EdmCoreModel.Instance.GetDouble(false), true, null, false);
            giftCardAmountFunction.AddParameter("giftcard", new EdmEntityTypeReference(giftCardType, false));
            giftCardAmountFunction.AddParameter("bonusRate", EdmCoreModel.Instance.GetDouble(true));
            model.AddElement(giftCardAmountFunction);

            var accountDefaultPIFunction = new EdmFunction(ns, "GetDefaultPI", new EdmEntityTypeReference(paymentInstrumentType, true), true, new EdmPathExpression("account/MyPaymentInstruments"), false);
            accountDefaultPIFunction.AddParameter("account", new EdmEntityTypeReference(accountType, false));
            model.AddElement(accountDefaultPIFunction);

            var accountRefreshDefaultPIAction = new EdmAction(ns, "RefreshDefaultPI", new EdmEntityTypeReference(paymentInstrumentType, true), true, new EdmPathExpression("account/MyPaymentInstruments"));
            accountRefreshDefaultPIAction.AddParameter("account", new EdmEntityTypeReference(accountType, false));
            accountRefreshDefaultPIAction.AddParameter("newDate", EdmCoreModel.Instance.GetDateTimeOffset(true));
            model.AddElement(accountRefreshDefaultPIAction);

            //Bound Function : Bound to Entity, Return ComplexType
            var getHomeAddressFunction = new EdmFunction(ns, "GetHomeAddress",
                new EdmComplexTypeReference(homeAddressType, false),
                true, null, true);
            getHomeAddressFunction.AddParameter("person", new EdmEntityTypeReference(personType, false));
            model.AddElement(getHomeAddressFunction);

            //Bound Function : Bound to Entity, Return ComplexType
            var getAccountInfoFunction = new EdmFunction(ns, "GetAccountInfo",
                new EdmComplexTypeReference(accountInfoType, false),
                true, null, true);
            getAccountInfoFunction.AddParameter("account", new EdmEntityTypeReference(accountType, false));
            model.AddElement(getAccountInfoFunction);

            #endregion

            var accountGiftCardNavigation = accountType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "MyGiftCard",
                Target = giftCardType,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                ContainsTarget = true
            });

            var accountPIsNavigation = accountType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "MyPaymentInstruments",
                Target = paymentInstrumentType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true
            });

            var accountActiveSubsNavigation = accountType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "ActiveSubscriptions",
                Target = subscriptionType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true
            });

            var accountAvailableSubsTemplatesNavigation = accountType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "AvailableSubscriptionTemplatess",
                Target = subscriptionType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = false
            });

            var piStoredNavigation = paymentInstrumentType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "TheStoredPI",
                Target = storedPIType,
                TargetMultiplicity = EdmMultiplicity.One,
                ContainsTarget = false
            });

            var piStatementsNavigation = paymentInstrumentType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "BillingStatements",
                Target = statementType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true
            });

            var piBackupStoredPINavigation = paymentInstrumentType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "BackupStoredPI",
                Target = storedPIType,
                TargetMultiplicity = EdmMultiplicity.One,
                ContainsTarget = false
            });

            var creditCardCreditRecordNavigation = creditCardType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "CreditRecords",
                Target = creditRecordType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true
            });

            model.AddElement(accountInfoType);
            model.AddElement(accountType);
            model.AddElement(giftCardType);
            model.AddElement(paymentInstrumentType);
            model.AddElement(creditCardType);
            model.AddElement(storedPIType);
            model.AddElement(statementType);
            model.AddElement(creditRecordType);
            model.AddElement(subscriptionType);

            var accountSet = new EdmEntitySet(defaultContainer, "Accounts", accountType);
            defaultContainer.AddElement(accountSet);
            var storedPISet = new EdmEntitySet(defaultContainer, "StoredPIs", storedPIType);
            defaultContainer.AddElement(storedPISet);
            var subscriptionTemplatesSet = new EdmEntitySet(defaultContainer, "SubscriptionTemplates", subscriptionType);
            defaultContainer.AddElement(subscriptionTemplatesSet);
            var defaultStoredPI = new EdmSingleton(defaultContainer, "DefaultStoredPI", storedPIType);
            defaultContainer.AddElement(defaultStoredPI);

            ((EdmEntitySet)accountSet).AddNavigationTarget(piStoredNavigation, storedPISet);
            ((EdmEntitySet)accountSet).AddNavigationTarget(accountAvailableSubsTemplatesNavigation, subscriptionTemplatesSet);
            ((EdmEntitySet)accountSet).AddNavigationTarget(piBackupStoredPINavigation, defaultStoredPI);

            #endregion

            IEnumerable<EdmError> errors = null;
            model.Validate(out errors);
            //TODO: Fix the errors

            return model;
        }

        private static void SetCoreChangeTrackingAnnotation(this EdmModel model, EdmEntitySet entitySet, IEdmStructuralProperty[] filterableProperties, IEdmNavigationProperty[] expandableProperties)
        {
            IEdmModel termModel = ReadTermModel("CoreCapabilities.csdl");
            IEdmValueTerm changeTracking = termModel.FindDeclaredValueTerm("Core.ChangeTracking");
            var exp = new EdmRecordExpression(
                new EdmPropertyConstructor("Supported", new EdmBooleanConstant(true)),

                new EdmPropertyConstructor("FilterableProperties", new EdmCollectionExpression(filterableProperties.Select(p => new EdmPropertyPathExpression(p.Name)))),
                new EdmPropertyConstructor("ExpandableProperties", new EdmCollectionExpression(expandableProperties.Select(p => new EdmPropertyPathExpression(p.Name)))));

            EdmAnnotation annotation = new EdmAnnotation(entitySet, changeTracking, exp);
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(annotation);
        }

        private static IEdmModel ReadTermModel(string fileName)
        {
            IEdmModel instance;

            using (Stream stream = ReadResourceFromAssembly(fileName))
            {
                IEnumerable<EdmError> errors;
                CsdlReader.TryParse(new[] { XmlReader.Create(stream) }, out instance, out errors);
            }
            return instance;
        }

        private static Stream ReadResourceFromAssembly(string resourceName)
        {
            var testAssembly = Assembly.GetAssembly(typeof(DefaultInMemoryModel));
            string fullResourceName = testAssembly.GetManifestResourceNames().Single(n => n.EndsWith(resourceName));
            Stream resourceStream = testAssembly.GetManifestResourceStream(fullResourceName);

            var reader = new StreamReader(resourceStream);
            string str = reader.ReadToEnd();
            str = str.Replace("[ServiceBaseUrl]", ServiceConstants.ServiceBaseUri.AbsoluteUri);
            byte[] byteArray = Encoding.UTF8.GetBytes(str);

            resourceStream = new MemoryStream(byteArray);
            return resourceStream;
        }
    }
}
