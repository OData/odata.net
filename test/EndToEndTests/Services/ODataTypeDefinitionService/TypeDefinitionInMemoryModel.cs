//---------------------------------------------------------------------
// <copyright file="TypeDefinitionInMemoryModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;
    using System;
    using System.Collections.Generic;

    public static class TypeDefinitionInMemoryModel
    {
        public static IEdmModel CreateModel(string ns)
        {
            EdmModel model = new EdmModel();
            var defaultContainer = new EdmEntityContainer(ns, "DefaultContainer");
            model.AddElement(defaultContainer);

            var nameType = new EdmTypeDefinition(ns, "Name", EdmPrimitiveTypeKind.String);
            model.AddElement(nameType);

            var addressType = new EdmComplexType(ns, "Address");
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Road", new EdmTypeDefinitionReference(nameType, false)));
            addressType.AddProperty(new EdmStructuralProperty(addressType, "City", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(addressType);

            var personType = new EdmEntityType(ns, "Person");
            var personIdProperty = new EdmStructuralProperty(personType, "PersonId", EdmCoreModel.Instance.GetInt32(false));
            personType.AddProperty(personIdProperty);
            personType.AddKeys(new IEdmStructuralProperty[] { personIdProperty });
            personType.AddProperty(new EdmStructuralProperty(personType, "FirstName", new EdmTypeDefinitionReference(nameType, false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "LastName", new EdmTypeDefinitionReference(nameType, false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "Address", new EdmComplexTypeReference(addressType, true)));
            personType.AddProperty(new EdmStructuralProperty(personType, "Descriptions", new EdmCollectionTypeReference(new EdmCollectionType(new EdmTypeDefinitionReference(nameType, false)))));

            model.AddElement(personType);
            var peopleSet = new EdmEntitySet(defaultContainer, "People", personType);
            defaultContainer.AddElement(peopleSet);

            var numberComboType = new EdmComplexType(ns, "NumberCombo");
            numberComboType.AddProperty(new EdmStructuralProperty(numberComboType, "Small", model.GetUInt16(ns, false)));
            numberComboType.AddProperty(new EdmStructuralProperty(numberComboType, "Middle", model.GetUInt32(ns, false)));
            numberComboType.AddProperty(new EdmStructuralProperty(numberComboType, "Large", model.GetUInt64(ns, false)));
            model.AddElement(numberComboType);

            var productType = new EdmEntityType(ns, "Product");
            var productIdProperty = new EdmStructuralProperty(productType, "ProductId", model.GetUInt16(ns, false));
            productType.AddProperty(productIdProperty);
            productType.AddKeys(new IEdmStructuralProperty[] { productIdProperty });
            productType.AddProperty(new EdmStructuralProperty(productType, "Quantity", model.GetUInt32(ns, false)));
            productType.AddProperty(new EdmStructuralProperty(productType, "NullableUInt32", model.GetUInt32(ns, true)));
            productType.AddProperty(new EdmStructuralProperty(productType, "LifeTimeInSeconds", model.GetUInt64(ns, false)));
            productType.AddProperty(new EdmStructuralProperty(productType, "TheCombo", new EdmComplexTypeReference(numberComboType, true)));
            productType.AddProperty(new EdmStructuralProperty(productType, "LargeNumbers", new EdmCollectionTypeReference(new EdmCollectionType(model.GetUInt64(ns, false)))));

            model.AddElement(productType);
            var productsSet = new EdmEntitySet(defaultContainer, "Products", productType);
            defaultContainer.AddElement(productsSet);

            //Bound Function: bound to entity, return defined type
            var getFullNameFunction = new EdmFunction(ns, "GetFullName", new EdmTypeDefinitionReference(nameType, false), true, null, false);
            getFullNameFunction.AddParameter("person", new EdmEntityTypeReference(personType, false));
            getFullNameFunction.AddParameter("nickname", new EdmTypeDefinitionReference(nameType, false));
            model.AddElement(getFullNameFunction);

            //Bound Action: bound to entity, return UInt64
            var extendLifeTimeAction = new EdmAction(ns, "ExtendLifeTime", model.GetUInt64(ns, false), true, null);
            extendLifeTimeAction.AddParameter("product", new EdmEntityTypeReference(productType, false));
            extendLifeTimeAction.AddParameter("seconds", model.GetUInt32(ns, false));
            model.AddElement(extendLifeTimeAction);

            //UnBound Action: ResetDataSource
            var resetDataSourceAction = new EdmAction(ns, "ResetDataSource", null, false, null);
            model.AddElement(resetDataSourceAction);
            defaultContainer.AddActionImport(resetDataSourceAction);

            IEnumerable<EdmError> errors = null;
            model.Validate(out errors);

            return model;
        }

        internal class UInt32ValueConverter : IPrimitiveValueConverter
        {
            private static readonly IPrimitiveValueConverter instance = new UInt32ValueConverter();

            internal static IPrimitiveValueConverter Instance
            {
                get { return instance; }
            }

            public object ConvertToUnderlyingType(object value)
            {
                return Convert.ToString(value);
            }

            public object ConvertFromUnderlyingType(object value)
            {
                return Convert.ToUInt32(value);
            }
        }
    }
}
