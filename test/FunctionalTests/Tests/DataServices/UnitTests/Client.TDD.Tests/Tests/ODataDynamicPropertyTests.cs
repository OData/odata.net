//---------------------------------------------------------------------
// <copyright file="ODataDynamicPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using Microsoft.OData.Client;
    using Microsoft.OData.Edm;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using Xunit;

    public class ODataDynamicPropertyTests
    {
        private EdmModel serviceEdmModel;
        private EdmEnumType genreEnumType;
        private EdmComplexType addressComplexType;
        private EdmComplexType nextOfKinComplexType;
        private EdmEntityType directorEntityType;
        private EdmEntityType editorEntityType;
        private EdmEntityType producerEntityType;
        private EdmEntityType actorEntityType;
        private EdmEntityType awardEntityType;
        private string genreEnumTypeName;
        private string addressComplexTypeName;
        private string nextOfKinComplexTypeName;
        private string directorEntityTypeName;
        private string directorEntitySetName;
        private string editorEntityTypeName;
        private string editorEntitySetName;
        private string producerEntityTypeName;
        private string producerEntitySetName;
        private string actorEntityTypeName;
        private string actorEntitySetName;
        private string awardEntityTypeName;
        private string awardEntitySetName;

        private ClientEdmModel clientEdmModel;
        private DataServiceContext context;
        private IDictionary<string, string> typeNameToEntitySetMapping;

        public ODataDynamicPropertyTests()
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

            // Actor entity type
            this.actorEntitySetName = "Actors";
            this.actorEntityTypeName = "ServiceNS.Actor";
            this.actorEntityType = new EdmEntityType("ServiceNS", "Actor", null /* baseType */, false /* isAbstract */, true /* isOpen */); // Open type
            this.actorEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.actorEntityType.AddKeys(this.actorEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
            this.serviceEdmModel.AddElement(actorEntityType);

            // Award entity type
            this.awardEntitySetName = "Awards";
            this.awardEntityTypeName = "ServiceNS.Award";
            this.awardEntityType = new EdmEntityType("ServiceNS", "Award"); // Not an open type
            this.awardEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.awardEntityType.AddKeys(this.awardEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
            this.serviceEdmModel.AddElement(awardEntityType);

            // Entity container
            EdmEntityContainer container = new EdmEntityContainer("ServiceNS", "Container");
            container.AddEntitySet(this.directorEntitySetName, this.directorEntityType, true);
            container.AddEntitySet(this.editorEntitySetName, this.editorEntityType, true);
            container.AddEntitySet(this.producerEntitySetName, this.producerEntityType, true);
            container.AddEntitySet(this.actorEntitySetName, this.actorEntityType, true);
            container.AddEntitySet(this.awardEntitySetName, this.awardEntityType, true);
            this.serviceEdmModel.AddElement(container);

            this.clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);

            this.context = new DataServiceContext(new Uri("http://tempuri.org/"), ODataProtocolVersion.V4, this.clientEdmModel);
            this.context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            this.context.Format.UseJson(this.serviceEdmModel);

            var serverTypeNames = new[]
            {
                this.genreEnumTypeName,
                this.addressComplexTypeName,
                this.nextOfKinComplexTypeName,
                this.directorEntityTypeName,
                this.editorEntityTypeName,
                this.producerEntityTypeName,
                this.actorEntityTypeName,
                this.awardEntityTypeName
            };

            this.context.ResolveName = type => {
                // Lazy approach to resolving server type names - alternative would be multiple if/else blocks
                return serverTypeNames.FirstOrDefault(d => d.EndsWith(type.Name, StringComparison.Ordinal));
            };

            var serverTypeNameToClientTypeMapping = new Dictionary<string, Type>
            {
                { this.genreEnumTypeName, typeof(Genre) },
                { this.addressComplexTypeName, typeof(Address) },
                { this.nextOfKinComplexTypeName, typeof(NextOfKin) },
                { this.directorEntityTypeName, typeof(Director) },
                { this.producerEntityTypeName, typeof(Producer) },
                { this.editorEntityTypeName, typeof(Editor) },
                { this.actorEntityTypeName, typeof(Actor) },
                { this.awardEntityTypeName, typeof(Award) }
            };

            this.context.ResolveType = name => {
                if (!serverTypeNameToClientTypeMapping.ContainsKey(name))
                {
                    return null;
                }
                // Lazy approach to resolving client types - alternative would be multiple if/else blocks
                return serverTypeNameToClientTypeMapping[name];
            };

            this.typeNameToEntitySetMapping = new Dictionary<string, string>
            {
                { typeof(Director).Name, this.directorEntitySetName },
                { typeof(Editor).Name, this.editorEntitySetName },
                { typeof(Producer).Name, this.producerEntitySetName },
                { typeof(Actor).Name, this.actorEntitySetName }
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

        private T MaterializeEntity<T>(string rawJsonResponse)
        {
            if (!this.typeNameToEntitySetMapping.ContainsKey(typeof(T).Name))
                return default(T);

            var entitySetName = this.typeNameToEntitySetMapping[typeof(T).Name];

            // Ride on OnMessageCreating to substitute the transport layer by supplying the equivalent response from an odata service
            context.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                var contentTypeHeader = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8";
                var odataVersionHeader = "4.0";

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
                return new Microsoft.OData.Client.TDDUnitTests.Tests.CustomizedHttpWebRequestMessage(args,
                    rawJsonResponse,
                    new Dictionary<string, string>
                    {
                        {"Content-Type", contentTypeHeader},
                        {"OData-Version", odataVersionHeader},
                    });
#else
                var requestMessage = new Microsoft.OData.Tests.InMemoryMessage { Url = args.RequestUri, Method = args.Method, Stream = new MemoryStream() };

                foreach (var header in args.Headers)
                {
                    requestMessage.SetHeader(header.Key, header.Value);
                }

                return new ClientExtensions.TestDataServiceClientRequestMessage(requestMessage, () => {
                    var responseMessage = new Microsoft.OData.Tests.InMemoryMessage
                    {
                        StatusCode = 200,
                        Stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(rawJsonResponse))
                    };
                    
                    responseMessage.SetHeader("Content-Type", contentTypeHeader);
                    responseMessage.SetHeader("OData-Version", odataVersionHeader);

                    return responseMessage;
                });
#endif
            };

            var query = context.CreateQuery<T>(entitySetName);

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
            T materializedObject = query.Execute().FirstOrDefault();
#else
            var asyncResult = query.BeginExecute(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            var queryResult = query.EndExecute(asyncResult);

            T materializedObject = queryResult.FirstOrDefault();
#endif
            return materializedObject;
        }

        internal void VerifyMessageBody(string expected, string actual)
        {
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SerializationWithNoDynamicProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void SerializationWithDynamicPropertyWithDynamicProperty()
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

        [Fact]
        public void SerializationWithDiverseDynamicProperties()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("Title", "Prof");
            director.DynamicProperties.Add("YearOfBirth", 1970); // Integer
            director.DynamicProperties.Add("Salary", 700000m); // Decimal
            director.DynamicProperties.Add("Pi", 3.14159265359d); // Double
            director.DynamicProperties.Add("BigInt", 6078747774547L); // Long Integer
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
                "\"BigInt@odata.type\":\"#Int64\",\"BigInt\":6078747774547," +
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

        [Fact]
        public void SerializationIgnoresDynamicPropertyIfNameConflictsWithDeclaredProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("Name", "Director X");

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            // "Name" dynamic property ignored
            var expectedResult = "{\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [Fact]
        public void SerializationIgnoresEntityTypeAsDynamicProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("Producer", new Producer { Id = 2, Name = "Producer 2" });

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            // "Producer" entity type dynamic property ignored
            var expectedResult = "{\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [Fact]
        public void SerializationIgnoresDictionaryWithNoContainerPropertyAttribute()
        {
            var editor = new Editor { Id = 1, Name = "Editor 1" };
            editor.DynamicProperties.Add("Title", "Dr");

            var messageBody = SerializeEntity(this.editorEntitySetName, editor);
            // Nothing in the dictionary property is serialized
            var expectedResult = "{\"@odata.type\":\"#ServiceNS.Editor\",\"Id\":1,\"Name\":\"Editor 1\"}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [Fact]
        public void SerializationIgnoresContainerPropertyIfEdmTypeIsNotOpen()
        {
            // Producer is not an open type but client type has a dictionary of string and object
            var producer = new Producer { Id = 1, Name = "Producer 1" };
            producer.DynamicProperties.Add("Title", "Ms");

            var messageBody = SerializeEntity(this.producerEntitySetName, producer);
            // Nothing in the dictionary property is serialized
            var expectedResult = "{\"@odata.type\":\"#ServiceNS.Producer\",\"Id\":1,\"Name\":\"Producer 1\"}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [Fact]
        public void SerializationWithNullValueAsDynamicProperty()
        {
            var director = new Director { Id = 1, Name = "Director 1" };
            director.DynamicProperties.Add("Title", null);

            var messageBody = SerializeEntity(this.directorEntitySetName, director);
            var expectedResult = "{\"@odata.type\":\"#ServiceNS.Director\",\"Id\":1,\"Name\":\"Director 1\"}";

            VerifyMessageBody(expectedResult, messageBody);
        }

        [Fact]
        public void MaterializationWithNoDynamicProperty()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\"," +
                "\"Id\":1,\"Name\":\"Director 1\"" +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);

            Assert.NotNull(materializedObject);
            Assert.Equal(1, materializedObject.Id);
            Assert.Equal("Director 1", materializedObject.Name);
            Assert.Equal(0, materializedObject.DynamicProperties.Count);
        }

        private void AssertCommon(Director materializedObject, Type dynamicPropertyType, string dynamicPropertyName)
        {
            Assert.NotNull(materializedObject);
            Assert.Equal(1, materializedObject.Id);
            Assert.Equal("Director 1", materializedObject.Name);
            Assert.Equal(1, materializedObject.DynamicProperties.Count);
            Assert.True(materializedObject.DynamicProperties.ContainsKey(dynamicPropertyName));
            Assert.True(materializedObject.DynamicProperties[dynamicPropertyName].GetType() == dynamicPropertyType);
        }

        [Fact]
        public void MaterializationWithPrimitiveDynamicProperty()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\"," +
                "\"Id\":1,\"Name\":\"Director 1\"," +
                "\"Title\":\"Prof\"" +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);

            AssertCommon(materializedObject, typeof(string), "Title");
            Assert.Equal("Prof", materializedObject.DynamicProperties["Title"]);
        }

        [Fact]
        public void MaterializationWithPrimitiveCollectionDynamicProperty()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\"," +
                "\"Id\":1,\"Name\":\"Director 1\"," +
                "\"NickNames@odata.type\":\"#Collection(String)\",\"NickNames\":[\"N1\",\"N2\"]" +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);

            AssertCommon(materializedObject, typeof(Collection<string>), "NickNames");
            var primitiveValueCollection = materializedObject.DynamicProperties["NickNames"] as Collection<string>;
            Assert.NotNull(primitiveValueCollection);
            Assert.Equal(2, primitiveValueCollection.Count);
            Assert.Contains("N1", primitiveValueCollection, StringComparer.Ordinal);
            Assert.Contains("N2", primitiveValueCollection, StringComparer.Ordinal);
        }

        [Fact]
        public void MaterializationWithEnumDynamicProperty()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\"," +
                "\"Id\":1,\"Name\":\"Director 1\"," +
                "\"FavoriteGenre@odata.type\":\"#ServiceNS.Genre\",\"FavoriteGenre\":\"SciFi\"" +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);

            AssertCommon(materializedObject, typeof(Genre), "FavoriteGenre");
            Assert.Equal(Genre.SciFi, materializedObject.DynamicProperties["FavoriteGenre"]);
        }

        [Fact]
        public void MaterializationWithEnumCollectionDynamicProperty()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\"," +
                "\"Id\":1,\"Name\":\"Director 1\"," +
                "\"Genres@odata.type\":\"#Collection(ServiceNS.Genre)\",\"Genres\":[\"Thriller\",\"Epic\"]" +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);

            AssertCommon(materializedObject, typeof(Collection<Genre>), "Genres");
            var enumValueCollection = materializedObject.DynamicProperties["Genres"] as Collection<Genre>;
            Assert.NotNull(enumValueCollection);
            Assert.Equal(2, enumValueCollection.Count);
            Assert.Contains(Genre.Epic, enumValueCollection);
            Assert.Contains(Genre.Thriller, enumValueCollection);
        }

        [Fact]
        public void MaterializationWithComplexDynamicProperty()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\"," +
                "\"Id\":1,\"Name\":\"Director 1\"," +
                "\"WorkAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL1\",\"City\":\"C1\"}" +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);

            AssertCommon(materializedObject, typeof(Address), "WorkAddress");
            var workAddress = materializedObject.DynamicProperties["WorkAddress"] as Address;
            Assert.NotNull(workAddress);
            Assert.Equal("AL1", workAddress.AddressLine);
            Assert.Equal("C1", workAddress.City);
            Assert.Equal(0, workAddress.DynamicProperties.Count);
        }

        [Fact]
        public void MaterializationWithComplexCollectionDynamicProperty()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\"," +
                "\"Id\":1,\"Name\":\"Director 1\"," +
                "\"Addresses@odata.type\":\"#Collection(ServiceNS.Address)\",\"Addresses\":[" +
                "{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL2\",\"City\":\"C2\"}," +
                "{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL3\",\"City\":\"C3\"}]" +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);

            AssertCommon(materializedObject, typeof(Collection<Address>), "Addresses");
            var addresses = materializedObject.DynamicProperties["Addresses"] as Collection<Address>;
            Assert.NotNull(addresses);
            Assert.Equal(2, addresses.Count);
            var address1 = addresses.SingleOrDefault(d => d.AddressLine.Equals("AL2", StringComparison.Ordinal));
            var address2 = addresses.SingleOrDefault(d => d.AddressLine.Equals("AL3", StringComparison.Ordinal));
            Assert.NotNull(address1);
            Assert.Equal("AL2", address1.AddressLine);
            Assert.Equal("C2", address1.City);
            Assert.Equal(0, address1.DynamicProperties.Count);
            Assert.NotNull(address2);
            Assert.Equal("AL3", address2.AddressLine);
            Assert.Equal("C3", address2.City);
            Assert.Equal(0, address2.DynamicProperties.Count);
        }

        [Fact]
        public void MaterializationWithComplexDynamicPropertyWithNestedComplexProperty()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\"," +
                "\"Id\":1,\"Name\":\"Director 1\"," +
                "\"NextOfKin\":{\"@odata.type\":\"#ServiceNS.NextOfKin\",\"Name\":\"Nok 1\",\"HomeAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL4\",\"City\":\"C4\"}}" +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);

            AssertCommon(materializedObject, typeof(NextOfKin), "NextOfKin");
            var nextOfKin = materializedObject.DynamicProperties["NextOfKin"] as NextOfKin;
            Assert.NotNull(nextOfKin);
            Assert.Equal("Nok 1", nextOfKin.Name);
            Assert.NotNull(nextOfKin.HomeAddress);
            Assert.Equal("AL4", nextOfKin.HomeAddress.AddressLine);
            Assert.Equal("C4", nextOfKin.HomeAddress.City);
            Assert.Equal(0, nextOfKin.HomeAddress.DynamicProperties.Count);
        }

        [Fact]
        public void MaterializationWithDynamicPropertyWithDynamicProperty()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\"," +
                "\"Id\":1,\"Name\":\"Director 1\"," +
                "\"WorkAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL5\",\"City\":\"C5\",\"State\":\"S5\"}" +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);

            AssertCommon(materializedObject, typeof(Address), "WorkAddress");
            var workAddress = materializedObject.DynamicProperties["WorkAddress"] as Address;
            Assert.NotNull(workAddress);
            Assert.Equal("AL5", workAddress.AddressLine);
            Assert.Equal("C5", workAddress.City);
            Assert.Equal(1, workAddress.DynamicProperties.Count);
            Assert.True(workAddress.DynamicProperties.ContainsKey("State"));
            Assert.True(workAddress.DynamicProperties["State"].GetType() == typeof(string));
            Assert.Equal("S5", workAddress.DynamicProperties["State"]);
        }

        [Fact]
        public void MaterializationWithDiverseDynamicProperties()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\",\"Id\":1,\"Name\":\"Director 1\"," +
                "\"Title\":\"Prof\"," +
                "\"YearOfBirth\":1970," + // Integer
                "\"Salary@odata.type\":\"#Decimal\",\"Salary\":700000," + // Decimal
                "\"Pi\":3.14159265359," + // Double
                "\"BigInt@odata.type\":\"#Int64\",\"BigInt\":6078747774547," + // Long Integer
                "\"NickNames@odata.type\":\"#Collection(String)\",\"NickNames\":[\"N1\",\"N2\"]," +
                "\"FavoriteGenre@odata.type\":\"#ServiceNS.Genre\",\"FavoriteGenre\":\"SciFi\"," +
                "\"Genres@odata.type\":\"#Collection(ServiceNS.Genre)\",\"Genres\":[\"Thriller\",\"Epic\"]," +
                "\"WorkAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL1\",\"City\":\"C1\"}," +
                "\"Addresses@odata.type\":\"#Collection(ServiceNS.Address)\",\"Addresses\":[" +
                "{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL2\",\"City\":\"C2\"}," +
                "{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL3\",\"City\":\"C3\"}]," +
                "\"NextOfKin\":{\"@odata.type\":\"#ServiceNS.NextOfKin\",\"Name\":\"Nok 1\",\"HomeAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL4\",\"City\":\"C4\"}}" +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);

            Assert.NotNull(materializedObject);
            Assert.Equal(11, materializedObject.DynamicProperties.Count);

            var dynamicProperties = materializedObject.DynamicProperties;
            Assert.True(dynamicProperties.ContainsKey("Title") && dynamicProperties["Title"].GetType() == typeof(string));
            Assert.True(dynamicProperties.ContainsKey("YearOfBirth") && dynamicProperties["YearOfBirth"].GetType() == typeof(int));
            Assert.True(dynamicProperties.ContainsKey("Salary") && dynamicProperties["Salary"].GetType() == typeof(decimal));
            Assert.True(dynamicProperties.ContainsKey("Pi") && dynamicProperties["Pi"].GetType() == typeof(double));
            Assert.True(dynamicProperties.ContainsKey("BigInt") && dynamicProperties["BigInt"].GetType() == typeof(long));
            Assert.True(dynamicProperties.ContainsKey("NickNames") && dynamicProperties["NickNames"].GetType() == typeof(Collection<string>));
            Assert.True(dynamicProperties.ContainsKey("FavoriteGenre") && dynamicProperties["FavoriteGenre"].GetType() == typeof(Genre));
            Assert.True(dynamicProperties.ContainsKey("Genres") && dynamicProperties["Genres"].GetType() == typeof(Collection<Genre>));
            Assert.True(dynamicProperties.ContainsKey("WorkAddress") && dynamicProperties["WorkAddress"].GetType() == typeof(Address));
            Assert.True(dynamicProperties.ContainsKey("Addresses") && dynamicProperties["Addresses"].GetType() == typeof(Collection<Address>));
            Assert.True(dynamicProperties.ContainsKey("NextOfKin") && dynamicProperties["NextOfKin"].GetType() == typeof(NextOfKin));
        }

        [Fact]
        public void MaterializationIgnoresEntityTypeAsDynamicProperty()
        {
            // NOTE: Whether entity type as a dynamic property is currently supported is a different matter altogether
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\"," +
                "\"Id\":1,\"Name\":\"Director 1\"," +
                "\"NotableAward\":{\"@odata.type\":\"#ServiceNS.Award\",\"Id\":\"1\",\"Name\":\"Golden Globe\"}" +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);
            Assert.Equal(0, materializedObject.DynamicProperties.Count);
        }

        [Fact]
        public void MaterializationIgnoresDictionaryWithNoContainerPropertyAttribute()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Editors/$entity\"," +
                "\"Id\":1,\"Name\":\"Editor 1\"," +
                "\"Title\":\"Dr\"" +
                "}";

            var materializedObject = MaterializeEntity<Editor>(rawJsonResponse);
            Assert.Equal(0, materializedObject.DynamicProperties.Count);
        }

        [Fact]
        public void MaterializationWithContainerPropertyNotPreInitialized()
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Actors/$entity\"," +
                "\"Id\":1,\"Name\":\"Actor 1\"," +
                "\"Title\":\"Mr\"," +
                "\"FavoriteGenre@odata.type\":\"#ServiceNS.Genre\",\"FavoriteGenre\":\"SciFi\"," +
                "\"Genres@odata.type\":\"#Collection(ServiceNS.Genre)\",\"Genres\":[\"Thriller\",\"Epic\"]," +
                "\"WorkAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL1\",\"City\":\"C1\"}" +
                "}";

            var materializedObject = MaterializeEntity<Actor>(rawJsonResponse);

            Assert.NotNull(materializedObject);
            // Dictionary should have been dynamically initialized
            Assert.NotNull(materializedObject.DynamicProperties);
            // Total of 5 declared properties on the client type. We should only have 1 dynamic property in the dictionary
            Assert.Equal(1, materializedObject.DynamicProperties.Count);
            Assert.True(materializedObject.DynamicProperties.ContainsKey("FavoriteGenre"));
            Assert.True(materializedObject.DynamicProperties["FavoriteGenre"].GetType() == typeof(Genre));
            Assert.Equal(Genre.SciFi, materializedObject.DynamicProperties["FavoriteGenre"]);
        }

        [Fact]
        public void MaterializationWithPartDynamicPropertiesMappedToDeclaredPropertiesOnClientType()
        {
            var rawJsonResponse = "{" +
               "\"@odata.context\":\"http://tempuri.org/$metadata#Actors/$entity\",\"Id\":1,\"Name\":\"Actor 1\"," +
               "\"Title\":\"Mr\"," +
               "\"NickNames@odata.type\":\"#Collection(String)\",\"NickNames\":[\"N1\",\"N2\"]," +
               "\"FavoriteGenre@odata.type\":\"#ServiceNS.Genre\",\"FavoriteGenre\":\"SciFi\"," +
               "\"Genres@odata.type\":\"#Collection(ServiceNS.Genre)\",\"Genres\":[\"Thriller\",\"Epic\"]," +
               "\"WorkAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL1\",\"City\":\"C1\"}," +
               "\"NextOfKin\":{\"@odata.type\":\"#ServiceNS.NextOfKin\",\"Name\":\"Nok 1\",\"HomeAddress\":{\"@odata.type\":\"#ServiceNS.Address\",\"AddressLine\":\"AL4\",\"City\":\"C4\"}}" +
               "}";

            var materializedObject = MaterializeEntity<Actor>(rawJsonResponse);

            Assert.NotNull(materializedObject);
            // Declared properties mapped to declared properties on client type
            Assert.Equal("Mr", materializedObject.Title);
            Assert.Contains(Genre.Epic, materializedObject.Genres);
            Assert.Contains(Genre.Thriller, materializedObject.Genres);
            Assert.Equal("AL1", materializedObject.WorkAddress.AddressLine);
            Assert.Equal("C1", materializedObject.WorkAddress.City);
            // Dictionary should only have 3 dynamic properties
            Assert.Equal(3, materializedObject.DynamicProperties.Count);

            var dynamicProperties = materializedObject.DynamicProperties;
            Assert.True(dynamicProperties.ContainsKey("NickNames") && dynamicProperties["NickNames"].GetType() == typeof(Collection<string>));
            Assert.True(dynamicProperties.ContainsKey("FavoriteGenre") && dynamicProperties["FavoriteGenre"].GetType() == typeof(Genre));
            Assert.True(dynamicProperties.ContainsKey("NextOfKin") && dynamicProperties["NextOfKin"].GetType() == typeof(NextOfKin));
        }

        [Theory]
        [InlineData("\"Title\":null")]
        [InlineData("\"Title@odata.type\":\"Edm.String\",\"Title\":null")]
        public void MaterializationIgnoresDynamicPropertyWithNullValue(string dynamicProperty)
        {
            var rawJsonResponse = "{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Directors/$entity\"," +
                "\"Id\":1,\"Name\":\"Director 1\"," +
                dynamicProperty +
                "}";

            var materializedObject = MaterializeEntity<Director>(rawJsonResponse);

            Assert.Equal(0, materializedObject.DynamicProperties.Count);
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

            [ContainerProperty]
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

            [ContainerProperty]
            public IDictionary<string, object> DynamicProperties { get; set; } = new Dictionary<string, object>();
        }

        [Key("Id")]
        public class Editor
        {
            public int Id { get; set; }
            public string Name { get; set; }
            // No ContainerProperty attribute - will not be considered as dynamic properties container
            public IDictionary<string, object> DynamicProperties { get; set; } = new Dictionary<string, object>();
        }

        [Key("Id")]
        public class Producer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            [ContainerProperty]
            public IDictionary<string, object> DynamicProperties { get; set; } = new Dictionary<string, object>();
        }

        [Key("Id")]
        public class Actor
        {
            public int Id { get; set; }
            public string Name { get; set; }
            // Three declared properties on client type - corresponding to respective dynamic properties on server side
            public string Title { get; set; }
            public List<Genre> Genres { get; set; }
            public Address WorkAddress { get; set; }
            [ContainerProperty] // Dynamic property container not initialized - should be dynamically initialized 
            public IDictionary<string, object> DynamicProperties { get; set; }
        }

        [Key("Id")]
        public class Award
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
