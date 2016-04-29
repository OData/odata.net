//---------------------------------------------------------------------
// <copyright file="CamelCaseInMemoryModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    public static class CamelCaseInMemoryModel
    {
        public static IEdmModel CreateModel(string ns)
        {
            EdmModel model = new EdmModel();
            var defaultContainer = new EdmEntityContainer(ns, "DefaultContainer");
            model.AddElement(defaultContainer);

            var orderDetail = new EdmComplexType(ns, "orderDetail");
            orderDetail.AddProperty(new EdmStructuralProperty(orderDetail, "amount", EdmCoreModel.Instance.GetDecimal(false)));
            orderDetail.AddProperty(new EdmStructuralProperty(orderDetail, "quantity", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(orderDetail);

            var billingType = new EdmEnumType(ns, "billingType", EdmPrimitiveTypeKind.Int64, false);
            billingType.AddMember("fund", new EdmEnumMemberValue(0));
            billingType.AddMember("refund", new EdmEnumMemberValue(1));
            billingType.AddMember("chargeback", new EdmEnumMemberValue(2));
            model.AddElement(billingType);

            var category = new EdmEnumType(ns, "category", true);
            category.AddMember("toy", new EdmEnumMemberValue(1));
            category.AddMember("food", new EdmEnumMemberValue(2));
            category.AddMember("cloth", new EdmEnumMemberValue(4));
            category.AddMember("drink", new EdmEnumMemberValue(8));
            category.AddMember("computer", new EdmEnumMemberValue(16));
            model.AddElement(category);

            var namedObject = new EdmEntityType(ns, "namedObject");
            var idProperty = new EdmStructuralProperty(namedObject, "id", EdmCoreModel.Instance.GetInt32(false));
            namedObject.AddProperty(idProperty);
            namedObject.AddKeys(idProperty);
            namedObject.AddProperty(new EdmStructuralProperty(namedObject, "name", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(namedObject);

            var order = new EdmEntityType(ns, "order", namedObject);
            order.AddProperty(new EdmStructuralProperty(order, "description", EdmCoreModel.Instance.GetString(false)));
            order.AddProperty(new EdmStructuralProperty(order, "detail", new EdmComplexTypeReference(orderDetail, true)));
            order.AddProperty(new EdmStructuralProperty(order, "lineItems",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false)))));
            order.AddProperty(new EdmStructuralProperty(order, "categories", new EdmEnumTypeReference(category, false)));
            model.AddElement(order);

            var billingRecord = new EdmEntityType(ns, "billingRecord", namedObject);
            billingRecord.AddProperty(new EdmStructuralProperty(billingRecord, "amount", EdmCoreModel.Instance.GetDecimal(false)));
            billingRecord.AddProperty(new EdmStructuralProperty(billingRecord, "type", new EdmEnumTypeReference(billingType, false)));
            model.AddElement(billingRecord);

            var store = new EdmEntityType(ns, "store", namedObject);
            store.AddProperty(new EdmStructuralProperty(store, "address", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(store);

            var orders = new EdmEntitySet(defaultContainer, "orders", order);
            defaultContainer.AddElement(orders);

            var billingRecords = new EdmEntitySet(defaultContainer, "billingRecords", billingRecord);
            defaultContainer.AddElement(billingRecords);

            var myStore = new EdmSingleton(defaultContainer, "myStore", store);
            defaultContainer.AddElement(myStore);

            var order2billingRecord = order.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "billingRecords",
                Target = billingRecord,
                TargetMultiplicity = EdmMultiplicity.Many
            });

            ((EdmEntitySet)orders).AddNavigationTarget(order2billingRecord, billingRecords);

            IEnumerable<EdmError> errors = null;
            model.Validate(out errors);

            return model;
        }
    }
}
