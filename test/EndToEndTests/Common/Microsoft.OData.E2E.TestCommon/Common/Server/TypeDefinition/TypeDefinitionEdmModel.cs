//---------------------------------------------------------------------
// <copyright file="TypeDefinitionEdmModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition;

public static class TypeDefinitionEdmModel
{
    public static IEdmModel GetEdmModel()
    {
        // The namespace of the model
        var ns = "Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition";

        var model = new EdmModel();

        var defaultContainer = new EdmEntityContainer(ns, "DefaultContainer");
        model.AddElement(defaultContainer);

        // Define a new type for Name
        var nameType = new EdmTypeDefinition(ns, "Name", EdmPrimitiveTypeKind.String);
        model.AddElement(nameType);

        var nameTypeDefinitionRef = new EdmTypeDefinitionReference(nameType, false);

        // Define a new type for height

        var heightType = new EdmTypeDefinition(ns, "Height", EdmPrimitiveTypeKind.String);
        model.AddElement(heightType);

        // Define a new type for Temperature
        var temperatureType = new EdmTypeDefinition(ns, "Temperature", EdmPrimitiveTypeKind.String);
        model.AddElement(temperatureType);

        var addressType = new EdmComplexType(ns, "Address");
        addressType.AddProperty(new EdmStructuralProperty(addressType, "Road", nameTypeDefinitionRef));
        addressType.AddProperty(new EdmStructuralProperty(addressType, "City", EdmCoreModel.Instance.GetString(false)));
        model.AddElement(addressType);

        var personType = new EdmEntityType(ns, "Person");
        var personIdProperty = new EdmStructuralProperty(personType, "PersonId", EdmCoreModel.Instance.GetInt32(false));
        personType.AddProperty(personIdProperty);
        personType.AddKeys(new IEdmStructuralProperty[] { personIdProperty });
        personType.AddProperty(new EdmStructuralProperty(personType, "FirstName", nameTypeDefinitionRef));
        personType.AddProperty(new EdmStructuralProperty(personType, "LastName", nameTypeDefinitionRef));
        personType.AddProperty(new EdmStructuralProperty(personType, "Address", new EdmComplexTypeReference(addressType, true)));
        personType.AddProperty(new EdmStructuralProperty(personType, "Descriptions", new EdmCollectionTypeReference(new EdmCollectionType(nameTypeDefinitionRef))));
        var heightProp = personType.AddStructuralProperty("Height", new EdmTypeDefinitionReference(heightType, true));
        model.AddElement(personType);

        var heightPropInfo = typeof(Person).GetProperty("Height");
        model.SetAnnotationValue(heightProp, new ClrPropertyInfoAnnotation(heightPropInfo));

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
        var tempProp = productType.AddStructuralProperty("Temperature", new EdmTypeDefinitionReference(temperatureType, true));
        model.AddElement(productType);

        var tempPropInfo = typeof(Product).GetProperty("Temperature");
        model.SetAnnotationValue(tempProp, new ClrPropertyInfoAnnotation(tempPropInfo));

        var productsSet = new EdmEntitySet(defaultContainer, "Products", productType);
        defaultContainer.AddElement(productsSet);

        //Bound Function: bound to entity, return defined type
        var getFullNameFunction = new EdmFunction(ns, "GetFullName", nameTypeDefinitionRef, true, null, false);
        getFullNameFunction.AddParameter("person", new EdmEntityTypeReference(personType, false));
        getFullNameFunction.AddParameter("nickname", nameTypeDefinitionRef);
        model.AddElement(getFullNameFunction);

        //Bound Action: bound to entity, return UInt64
        var extendLifeTimeAction = new EdmAction(ns, "ExtendLifeTime", model.GetUInt64(ns, false), true, null);
        extendLifeTimeAction.AddParameter("product", new EdmEntityTypeReference(productType, false));
        extendLifeTimeAction.AddParameter("seconds", model.GetUInt64(ns, false));
        model.AddElement(extendLifeTimeAction);

        //UnBound Action: ResetDataSource
        var resetDataSourceAction = new EdmAction(ns, "Default.ResetDefaultDataSource", null, false, null);
        model.AddElement(resetDataSourceAction);
        defaultContainer.AddActionImport(resetDataSourceAction);

        model.Validate(out var errors);

        return model;
    }
}
