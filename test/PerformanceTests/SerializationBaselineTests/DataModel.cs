using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace SerializationBaselineTests
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
        public string Street { get; set; }
    }

    public static class DataModel
    {
        public static IEdmModel GetEdmModel()
        {
            var model = new EdmModel();

            var addressType = new EdmComplexType("NS", "Address");
            addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            model.AddElement(addressType);

            var customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            customer.AddStructuralProperty("Emails",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: true))));
            customer.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(addressType, isNullable: false));
            customer.AddStructuralProperty("Addresses",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(new EdmComplexTypeReference(addressType, isNullable: false))));
            model.AddElement(customer);

            var container = new EdmEntityContainer("NS", "DefaultContainer");
            container.AddEntitySet("Customers", customer);
            model.AddElement(container);

            return model;
        }
    }
}
