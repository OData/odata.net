//---------------------------------------------------------------------
// <copyright file="Utils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.JsonLight
{
    public class Utils
    {
        /// <summary>
        /// Creates a model, used model information at http://schema.org/Person as inspiration
        /// </summary>
        /// <returns></returns>
        public static IEdmModel BuildModel(string modelNamespace)
        {
            var edmModel = new EdmModel();
            var container = new EdmEntityContainer(modelNamespace, "Container");
            edmModel.AddElement(container);

            // Create types
            var addressType = new EdmComplexType(modelNamespace, "PostalAddress");
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            addressType.AddStructuralProperty("State", EdmPrimitiveTypeKind.String);
            addressType.AddStructuralProperty("ZipCode", EdmPrimitiveTypeKind.String);
            var addressRefType = new EdmComplexTypeReference(addressType, true);

            var placeType = new EdmEntityType(modelNamespace, "Place");
            var placeTypeKeyProp = placeType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            placeType.AddKeys(placeTypeKeyProp);
            placeType.AddStructuralProperty("MapUri", EdmPrimitiveTypeKind.String);
            placeType.AddStructuralProperty("TelephoneNumber", EdmPrimitiveTypeKind.String);
            placeType.AddStructuralProperty("Address", addressRefType);

            var personType = new EdmEntityType(modelNamespace, "Person");
            var personTypeKeyProp = personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            personType.AddKeys(personTypeKeyProp);
            personType.AddStructuralProperty("FirstName", EdmPrimitiveTypeKind.String);
            personType.AddStructuralProperty("LastName", EdmPrimitiveTypeKind.String);
            personType.AddStructuralProperty("Address", addressRefType);

            var organizationType = new EdmEntityType(modelNamespace, "Organization");
            var organizationTypeKeyProp = organizationType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            organizationType.AddKeys(organizationTypeKeyProp);
            organizationType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            organizationType.AddStructuralProperty("Address", addressRefType);

            var corporationType = new EdmEntityType(modelNamespace, "Corporation", organizationType);
            corporationType.AddStructuralProperty("TickerSymbol", EdmPrimitiveTypeKind.String);

            var localBusinessType = new EdmEntityType(modelNamespace, "LocalBusiness", organizationType);

            // Create associations
            personType.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() {Name = "Employers", Target = organizationType, TargetMultiplicity = EdmMultiplicity.Many}, 
                new EdmNavigationPropertyInfo() {Name = "Employees", Target = personType, TargetMultiplicity = EdmMultiplicity.Many});

            personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "CurrentPosition", Target = placeType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            personType.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Children", Target = personType, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Parent", Target = personType, TargetMultiplicity = EdmMultiplicity.One });

            localBusinessType.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "LocalBusinessBranches", Target = organizationType, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "MainOrganization", Target = localBusinessType, TargetMultiplicity = EdmMultiplicity.One });

            edmModel.AddElement(addressType);
            edmModel.AddElement(personType);
            edmModel.AddElement(organizationType);
            edmModel.AddElement(corporationType);
            edmModel.AddElement(localBusinessType);
            edmModel.AddElement(placeType);

            container.AddEntitySet("People", personType);
            container.AddEntitySet("Organizations", organizationType); 
            container.AddEntitySet("Places", organizationType);
            
            return edmModel;
        }
    }
}
