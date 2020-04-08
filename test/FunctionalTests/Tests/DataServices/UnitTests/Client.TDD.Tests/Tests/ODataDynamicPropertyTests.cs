//---------------------------------------------------------------------
// <copyright file="ODataDynamicPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using Microsoft.OData.Client;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    [TestClass]
    public class ODataDynamicPropertyTests
    {
        private EdmModel serviceEdmModel;
        private EdmEnumType genreEnumType;
        private EdmComplexType addressComplexType;
        private EdmComplexType nextOfKinComplexType;
        private EdmEntityType directorEntityType;
        private EdmEntityType editorEntityType;
        private EdmEntityType producerEntityType;
        private string genreEnumTypeName;
        private string addressComplexTypeName;
        private string nextOfKinComplexTypeName;
        private string directorEntityTypeName;
        private string directorEntitySetName;
        private string editorEntityTypeName;
        private string editorEntitySetName;
        private string producerEntityTypeName;
        private string producerEntitySetName;

        private ClientEdmModel clientEdmModel;
        private DataServiceContext context;

        [TestInitialize]
        public void Init()
        {
            this.serviceEdmModel = new EdmModel();

            // Enum type
            this.genreEnumTypeName = "ServiceNS.Genre";
            this.genreEnumType = new EdmEnumType("ServiceNS", "Genre");
            this.genreEnumType.AddMember(new EdmEnumMember(this.genreEnumType, "Thriller", new EdmEnumMemberValue(1)));
            this.genreEnumType.AddMember(new EdmEnumMember(this.genreEnumType, "SciFi", new EdmEnumMemberValue(2)));
            this.genreEnumType.AddMember(new EdmEnumMember(this.genreEnumType, "Epic", new EdmEnumMemberValue(3)));
            this.serviceEdmModel.AddElement(this.genreEnumType);

            // Complex type
            this.addressComplexTypeName = "ServiceNS.Address";
            this.addressComplexType = new EdmComplexType("ServiceNS", "Address", null /* baseType */, false /* isAbstract */, true /* isOpen */);
            this.addressComplexType.AddStructuralProperty("AddressLine", EdmPrimitiveTypeKind.String);
            this.addressComplexType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            this.serviceEdmModel.AddElement(this.addressComplexType);

            // Nested complex type
            this.nextOfKinComplexTypeName = "ServiceNS.NextOfKin";
            this.nextOfKinComplexType = new EdmComplexType("ServiceNS", "NextOfKin");
            this.nextOfKinComplexType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.nextOfKinComplexType.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(this.addressComplexType, true));
            this.serviceEdmModel.AddElement(this.nextOfKinComplexType);

            // Director entity type
            this.directorEntitySetName = "Directors";
            this.directorEntityTypeName = "ServiceNS.Director";
            this.directorEntityType = new EdmEntityType("ServiceNS", "Director", null /* baseType */, false /* isAbstract */, true /* isOpen */); // Open type
            this.directorEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.directorEntityType.AddKeys(this.directorEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
            this.serviceEdmModel.AddElement(directorEntityType);

            // Editor entity type
            this.editorEntitySetName = "Editors";
            this.editorEntityTypeName = "ServiceNS.Editor";
            this.editorEntityType = new EdmEntityType("ServiceNS", "Editor", null /* baseType */, false /* isAbstract */, true /* isOpen */); // Open type
            this.editorEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.editorEntityType.AddKeys(this.editorEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
            this.serviceEdmModel.AddElement(editorEntityType);

            // Producer entity type
            this.producerEntitySetName = "Producers";
            this.producerEntityTypeName = "ServiceNS.Producer";
            this.producerEntityType = new EdmEntityType("ServiceNS", "Producer"); // Not an open type
            this.producerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.producerEntityType.AddKeys(this.producerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
            this.serviceEdmModel.AddElement(producerEntityType);

            // Entity container
            EdmEntityContainer container = new EdmEntityContainer("ServiceNS", "Container");
            container.AddEntitySet(this.directorEntitySetName, this.directorEntityType, true);
            container.AddEntitySet(this.editorEntitySetName, this.editorEntityType, true);
            container.AddEntitySet(this.producerEntitySetName, this.producerEntityType, true);
            this.serviceEdmModel.AddElement(container);

            this.clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);

            this.context = new DataServiceContext(new Uri("http://tempuri/"), ODataProtocolVersion.V4, this.clientEdmModel);
            this.context.Format.UseJson(this.serviceEdmModel);
            
            this.context.ResolveName = type => {
                if (type == typeof(Genre))
                {
                    return this.genreEnumTypeName;
                }
                else if (type == typeof(Address))
                {
                    return this.addressComplexTypeName;
                }
                else if (type == typeof(NextOfKin))
                {
                    return this.nextOfKinComplexTypeName;
                }
                else if (type == typeof(Director))
                {
                    return this.directorEntityTypeName;
                }
                else if (type == typeof(Editor))
                {
                    return this.editorEntityTypeName;
                }
                else if (type == typeof(Producer))
                {
                    return this.producerEntityTypeName;
                }
                
                return null;
            };
        }

        private string SerializeEntity<T>(string entitySetName, T entityObject)
        {
            this.context.AddObject(entitySetName, entityObject);
            var requestInfo = new RequestInfo(this.context);
            var serializer = new Serializer(requestInfo);
            var headers = new HeaderCollection();
            var entityDescriptor = new EntityDescriptor(this.clientEdmModel);
            entityDescriptor.State = EntityStates.Added;
            entityDescriptor.Entity = entityObject;
            var requestMessageArgs = new BuildingRequestEventArgs("POST", new Uri("http://tempuri.org"), headers, entityDescriptor, HttpStack.Auto);
            var odataRequestMessageWrapper = ODataRequestMessageWrapper.CreateRequestMessageWrapper(requestMessageArgs, requestInfo);
            var linkDescriptors = new LinkDescriptor[0];

            serializer.WriteEntry(entityDescriptor, linkDescriptors, odataRequestMessageWrapper);

            var stream = (MemoryStream)odataRequestMessageWrapper.CachedRequestStream.Stream;
            var streamReader = new StreamReader(stream);
            var body = streamReader.ReadToEnd();

            return body;
        }

        public void VerifyMessageBody(string expected, string actual)
        {
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SerializationWithNoDynamicProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationWithPrimitiveDynamicProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("Title", "Prof");

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{" +
                "\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"," +
                "\"Title\":\"Prof\"" +
                "}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationWithPrimitiveCollectionDynamicProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("NickNames", new Collection<string> { "N1", "N2" });

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{" +
                "\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"," +
                "\"NickNames@odata.type\":\"#Collection(String)\",\"NickNames\":[\"N1\",\"N2\"]" +
                "}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationWithEnumDynamicProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("FavoriteGenre", Genre.SciFi);

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{" +
                "\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"," +
                "\"FavoriteGenre@odata.type\":\"#ServiceNS.Genre\",\"FavoriteGenre\":\"SciFi\"" +
                "}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationWithEnumCollectionDynamicProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("Genres", new Collection<Genre> { Genre.Thriller, Genre.Epic });

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{" +
                "\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"," +
                "\"Genres@odata.type\":\"#Collection(ServiceNS.Genre)\",\"Genres\":[\"Thriller\",\"Epic\"]" +
                "}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationWithComplexDynamicProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("WorkAddress", new Address { AddressLine = "AL1", City = "C1" });

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{" +
                "\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"," +
                "\"WorkAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL1\",\"City\":\"C1\"}" +
                "}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationWithComplexCollectionDynamicProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("Addresses", 
                new Collection<Address> { new Address { AddressLine = "AL2", City = "C2" }, new Address { AddressLine = "AL3", City = "C3" } });

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{" +
                "\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"," +
                "\"Addresses@odata.type\":\"#Collection(ServiceNS.Address)\"," +
                "\"Addresses\":[" +
                "{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL2\",\"City\":\"C2\"}," +
                "{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL3\",\"City\":\"C3\"}]" +
                "}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationWithComplexDynamicPropertyWithNestedComplexProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("NextOfKin", new NextOfKin { Name = "Nok 1", HomeAddress = new Address { AddressLine = "AL4", City = "C4" } });

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{" +
                "\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"," +
                "\"NextOfKin\":{\"@odata.type\":\"#ServiceNS.NextOfKin\",\"Name\":\"Nok 1\",\"HomeAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL4\",\"City\":\"C4\"}}" +
                "}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerialiDynamicPropertyWithDynamicProperty()
        {
            // Address object with dynamic property
            var workAddress = new Address { AddressLine = "AL5", City = "C5" };
            workAddress.DynamicProperties.Add("State", "S5");

            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("WorkAddress", workAddress);

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{" +
                "\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"," +
                "\"WorkAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL5\",\"City\":\"C5\",\"State\":\"S5\"}" +
                "}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationWithDiverseDynamicProperties()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("Title", "Prof");
            director.DynamicProperties.Add("YearOfBirth", 1970); // Integer
            director.DynamicProperties.Add("Salary", 700000m); // Decimal
            director.DynamicProperties.Add("Pi", 3.14159265359d); // Double
            director.DynamicProperties.Add("NickNames", new Collection<string> { "N1", "N2" });
            director.DynamicProperties.Add("FavoriteGenre", Genre.SciFi);
            director.DynamicProperties.Add("Genres", new Collection<Genre> { Genre.Thriller, Genre.Epic });
            director.DynamicProperties.Add("WorkAddress", new Address { AddressLine = "AL1", City = "C1" });
            director.DynamicProperties.Add("Addresses",
                new Collection<Address> { new Address { AddressLine = "AL2", City = "C2" }, new Address { AddressLine = "AL3", City = "C3" } });
            director.DynamicProperties.Add("NextOfKin", new NextOfKin { Name = "Nok 1", HomeAddress = new Address { AddressLine = "AL4", City = "C4" } });

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{" +
                "\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"," +
                "\"Title\":\"Prof\"," +
                "\"YearOfBirth\":1970," +
                "\"Salary@odata.type\":\"#Decimal\",\"Salary\":700000," +
                "\"Pi\":3.14159265359," +
                "\"NickNames@odata.type\":\"#Collection(String)\",\"NickNames\":[\"N1\",\"N2\"]," +
                "\"FavoriteGenre@odata.type\":\"#ServiceNS.Genre\",\"FavoriteGenre\":\"SciFi\"," +
                "\"Genres@odata.type\":\"#Collection(ServiceNS.Genre)\",\"Genres\":[\"Thriller\",\"Epic\"]," +
                "\"WorkAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL1\",\"City\":\"C1\"}," +
                "\"Addresses@odata.type\":\"#Collection(ServiceNS.Address)\",\"Addresses\":[" +
                "{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL2\",\"City\":\"C2\"}," +
                "{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL3\",\"City\":\"C3\"}]," +
                "\"NextOfKin\":{\"@odata.type\":\"#ServiceNS.NextOfKin\",\"Name\":\"Nok 1\",\"HomeAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL4\",\"City\":\"C4\"}}" +
                "}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationIgnoresDynamicPropertyIfNameConflictsWithDeclaredProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("Name", "Director X");

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            // "Name" dynamic property ignored
            var expectedResult = "{\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationIgnoresEntityTypeAsDynamicProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("Producer", new Producer { Id = 2, Name = "Producer 2" });

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            // "Producer" entity type dynamic property ignored
            var expectedResult = "{\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationIgnoresDynamicPropertiesDictionaryWithIgnoreClientPropertyAttribute()
        {
            var editor = new Editor { Id = 1, Name = "Editor 1" };
            editor.DynamicProperties.Add("Title", "Dr");

            var messageBody = SerializeEntity(this.editorEntitySetName, editor);
            // Nothing in the dictionary property is serialized
            var expectedResult = "{\"@odata.type\":\"#ServiceNS.Editor\",\"Id\":1,\"Name\":\"Editor 1\"}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [TestMethod]
        public void SerializationIgnoresDynamicPropertiesDictionaryIfEdmTypeIsNotOpen()
        {
            // Producer is not an open type but client type has a dictionary of string and object
            var producer = new Producer { Id = 1, Name = "Producer 1" };
            producer.DynamicProperties.Add("Title", "Ms");

            var messageBody = SerializeEntity(this.producerEntitySetName, producer);
            // Nothing in the dictionary property is serialized
            var expectedResult = "{\"@odata.type\":\"#ServiceNS.Producer\",\"Id\":1,\"Name\":\"Producer 1\"}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        public enum Genre
        {
            Thriller = 1,
            SciFi = 2,
            Epic = 3
        }

        public class Address
        {
            public string AddressLine { get; set; }
            public string City { get; set; }
            public IDictionary<string, object> DynamicProperties { get; set; } = new Dictionary<string, object>();
        }

        public class NextOfKin
        {
            public string Name { get; set; }
            public Address HomeAddress { get; set; }
        }

        [Key("Id")]
        public class Director
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public IDictionary<string, object> DynamicProperties { get; set; } = new Dictionary<string, object>();
        }

        [Key("Id")]
        public class Editor
        {
            public int Id { get; set; }
            public string Name { get; set; }
            [IgnoreClientProperty]
            public IDictionary<string, object> DynamicProperties { get; set; } = new Dictionary<string, object>();
        }

        [Key("Id")]
        public class Producer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public IDictionary<string, object> DynamicProperties { get; set; } = new Dictionary<string, object>();
        }
    }
}