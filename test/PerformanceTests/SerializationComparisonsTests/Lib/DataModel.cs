//---------------------------------------------------------------------
// <copyright file="DataModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace ExperimentsLib
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Emails { get; set; }
        public Address HomeAddress { get; set; }
        public List<Address> Addresses { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
        public object Misc { get; set; }
        public string Street { get; set; }
    }

    public static class DataModel
    {
        /// <summary>
        /// Gets an OData model for testing, defining
        /// and entity set of Customer instances.
        /// </summary>
        /// <returns>The OData model.</returns>
        public static IEdmModel GetEdmModel()
        {
            EdmModel model = new EdmModel();

            EdmComplexType addressType = new EdmComplexType("NS", "Address");
            addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            addressType.AddStructuralProperty("Misc", EdmUntypedStructuredTypeReference.NullableTypeReference);
            model.AddElement(addressType);

            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            customer.AddStructuralProperty("Emails",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: true))));
            customer.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(addressType, isNullable: false));
            customer.AddStructuralProperty("Addresses",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(new EdmComplexTypeReference(addressType, isNullable: false))));
            model.AddElement(customer);

            EdmEntityContainer container = new EdmEntityContainer("NS", "DefaultContainer");
            container.AddEntitySet("Customers", customer);
            model.AddElement(container);

            return model;
        }
    }
}
