//---------------------------------------------------------------------
// <copyright file="ODataSimplifiedInMemoryModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;

    public static class ODataSimplifiedInMemoryModel
    {
        public static IEdmModel CreateModel(string ns)
        {
            EdmModel model = new EdmModel();
            var defaultContainer = new EdmEntityContainer(ns, "DefaultContainer");
            model.AddElement(defaultContainer);

            var addressType = new EdmComplexType(ns, "Address");
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Road", EdmCoreModel.Instance.GetString(false)));
            addressType.AddProperty(new EdmStructuralProperty(addressType, "City", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(addressType);

            var personType = new EdmEntityType(ns, "Person");
            var personIdProperty = new EdmStructuralProperty(personType, "PersonId", EdmCoreModel.Instance.GetInt32(false));
            personType.AddProperty(personIdProperty);
            personType.AddKeys(new IEdmStructuralProperty[] { personIdProperty });
            personType.AddProperty(new EdmStructuralProperty(personType, "FirstName", EdmCoreModel.Instance.GetString(false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "LastName", EdmCoreModel.Instance.GetString(false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "Address", new EdmComplexTypeReference(addressType, true)));
            personType.AddProperty(new EdmStructuralProperty(personType, "Descriptions", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false)))));

            model.AddElement(personType);
            var peopleSet = new EdmEntitySet(defaultContainer, "People", personType);
            defaultContainer.AddElement(peopleSet);

            var numberComboType = new EdmComplexType(ns, "NumberCombo");
            numberComboType.AddProperty(new EdmStructuralProperty(numberComboType, "Small", EdmCoreModel.Instance.GetInt32(false)));
            numberComboType.AddProperty(new EdmStructuralProperty(numberComboType, "Middle", EdmCoreModel.Instance.GetInt64(false)));
            numberComboType.AddProperty(new EdmStructuralProperty(numberComboType, "Large", EdmCoreModel.Instance.GetDecimal(false)));
            model.AddElement(numberComboType);

            var productType = new EdmEntityType(ns, "Product");
            var productIdProperty = new EdmStructuralProperty(productType, "ProductId", EdmCoreModel.Instance.GetInt32(false));
            productType.AddProperty(productIdProperty);
            productType.AddKeys(new IEdmStructuralProperty[] { productIdProperty });
            productType.AddProperty(new EdmStructuralProperty(productType, "Quantity", EdmCoreModel.Instance.GetInt64(false)));
            productType.AddProperty(new EdmStructuralProperty(productType, "LifeTimeInSeconds", EdmCoreModel.Instance.GetDecimal(false)));
            productType.AddProperty(new EdmStructuralProperty(productType, "TheCombo", new EdmComplexTypeReference(numberComboType, true)));
            productType.AddProperty(new EdmStructuralProperty(productType, "LargeNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDecimal(false)))));

            model.AddElement(productType);
            var productsSet = new EdmEntitySet(defaultContainer, "Products", productType);
            defaultContainer.AddElement(productsSet);

            var productsProperty = personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Products",
                Target = productType,
                TargetMultiplicity = EdmMultiplicity.Many
            });
            peopleSet.AddNavigationTarget(productsProperty, productsSet);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);

            return model;
        }
    }
}
