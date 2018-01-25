//---------------------------------------------------------------------
// <copyright file="ODataT4CamelCaseTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using AstoriaUnitTests.Tests;
    using FluentAssertions;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Materialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OData = Microsoft.OData;

    public partial class Container : DataServiceContext
    {
        public Container(Uri serviceRoot) :
            base(serviceRoot, ODataProtocolVersion.V4)
        {
            this.ResolveName = this.ResolveNameFromType;
            this.ResolveType = this.ResolveTypeFromName;
        }

        protected new Type ResolveTypeFromName(string typeName)
        {
            Type resolvedType = this.DefaultResolveType(typeName, "namespace.test", "AstoriaUnitTests.TDD.Tests.Client");
            if ((resolvedType != null))
            {
                return resolvedType;
            }

            return null;
        }

        protected string ResolveNameFromType(Type clientType)
        {
            OriginalNameAttribute originalNameAttribute = (OriginalNameAttribute)clientType.GetCustomAttributes(typeof(OriginalNameAttribute), true).SingleOrDefault();
            if (clientType.Namespace.Equals("AstoriaUnitTests.TDD.Tests.Client", StringComparison.Ordinal))
            {
                if (originalNameAttribute != null)
                {
                    return string.Concat("namespace.test.", originalNameAttribute.OriginalName);
                }

                return string.Concat("namespace.test.", clientType.Name);
            }

            if (originalNameAttribute != null)
            {
                return clientType.Namespace + "." + originalNameAttribute.OriginalName;
            }

            return clientType.FullName;
        }

        public DataServiceQuery<BaseType> BaseSet
        {
            get
            {
                if ((this._BaseSet == null))
                {
                    this._BaseSet = this.CreateQuery<BaseType>("baseSet");
                }
                return this._BaseSet;
            }
        }
        private DataServiceQuery<BaseType> _BaseSet;

        public DataServiceQuery<EntityType> EntitySet
        {
            get
            {
                if ((this._EntitySet == null))
                {
                    this._EntitySet = this.CreateQuery<EntityType>("entitySet");
                }
                return this._EntitySet;
            }
        }
        private DataServiceQuery<EntityType> _EntitySet;

        public void AddToEntitySet(EntityType entityType)
        {
            this.AddObject("entitySet", entityType);
        }

        public DataServiceQuery<SingleType> Singleton
        {
            get
            {
                if ((this._Singleton == null))
                {
                    this._Singleton = this.CreateSingletonQuery<SingleType>("singleton");
                }
                return this._Singleton;
            }
        }
        private DataServiceQuery<SingleType> _Singleton;
    }

    [Key("keyProp")]
    [OriginalNameAttribute("baseType")]
    public class BaseType : BaseEntityType
    {
        [OriginalNameAttribute("keyProp")]
        public int KeyProp { get; set; }

        [OriginalNameAttribute("colorProp")]
        public Color? ColorProp { get; set; }

        [OriginalNameAttribute("primitiveCollection")]
        public ObservableCollection<int> PrimitiveCollection { get; set; }

        [OriginalNameAttribute("enumCollection")]
        public ObservableCollection<Color> EnumCollection { get; set; }

        [OriginalNameAttribute("complexCollection")]
        public ObservableCollection<ComplexType> ComplexCollection { get; set; }
    }

    [OriginalNameAttribute("entityType")]
    public class EntityType : BaseType
    {
        [OriginalNameAttribute("access")]
        public AccessLevel Access { get; set; }

        [OriginalNameAttribute("complexProp")]
        public ComplexType ComplexProp { get; set; }

        [OriginalNameAttribute("navigationProperty")]
        public Navigation NavigationProperty { get; set; }
    }

    [Key("keyProp")]
    [OriginalNameAttribute("singleType")]
    public class SingleType : BaseEntityType
    {
        [OriginalNameAttribute("keyProp")]
        public int KeyProp { get; set; }

        [OriginalNameAttribute("name")]
        public string Name { get; set; }
    }

    [OriginalNameAttribute("complexType")]
    public class ComplexType
    {
        [OriginalNameAttribute("age")]
        public int Age { get; set; }

        [OriginalNameAttribute("name")]
        public string Name { get; set; }

        [OriginalNameAttribute("color")]
        public Color Color { get; set; }

        [OriginalNameAttribute("primitives")]
        public ObservableCollection<int> Primitives { get; set; }

        [OriginalNameAttribute("enums")]
        public ObservableCollection<Color> Enums { get; set; }
    }

    [OriginalNameAttribute("navigation")]
    public class Navigation
    {
        [OriginalNameAttribute("id")]
        public int Id { get; set; }

        [OriginalNameAttribute("name")]
        public string Name { get; set; }
    }

    [OriginalNameAttribute("color")]
    public enum Color
    {
        [OriginalNameAttribute("red")]
        Red = 1,
        [OriginalNameAttribute("white")]
        White = 2,
        [OriginalNameAttribute("blue")]
        Blue = 3
    }

    [Flags]
    [OriginalNameAttribute("accessLevel")]
    public enum AccessLevel
    {
        [OriginalNameAttribute("read")]
        Read,
        Write,
        [OriginalNameAttribute("execute")]
        Execute
    }

    [TestClass]
    public class ODataT4CamelCaseTests
    {
        public const string ServerNamespace = "namespace.test";
        public Container Context = new Container(new Uri("http://www.odata.org/service.svc"));

        [TestMethod]
        public void EntitySetUriShouldUseOriginalName()
        {
            var entitySet = Context.EntitySet;
            entitySet.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet");
        }

        [TestMethod]
        public void SingletonUriShouldUseOriginalName()
        {
            var singleton = Context.Singleton;
            singleton.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/singleton");

            var singletonProp = Context.Singleton.Select(e => e.Name) as DataServiceQuery;
            singletonProp.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/singleton/name");
        }

        [TestMethod]
        public void EntityUriShouldUseOriginalName()
        {
            var entity = Context.EntitySet.Where(e => e.KeyProp == 1) as DataServiceQuery;
            entity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet(1)");
        }

        [TestMethod]
        public void NavigationUriShouldUseOriginalName()
        {
            var navigation = Context.EntitySet.Expand(e => e.NavigationProperty).Where(e => e.KeyProp == 1) as DataServiceQuery;
            navigation.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/entitySet(1)?$expand=navigationProperty");
        }

        [TestMethod]
        public void TypeNameUriShouldUseOriginalName()
        {
            var baseEntity = Context.BaseSet.Where(b => b.KeyProp == 1).Select(b => (b as EntityType).ComplexProp) as DataServiceQuery;
            baseEntity.RequestUri.OriginalString.Should().Be("http://www.odata.org/service.svc/baseSet(1)/" + ServerNamespace + ".entityType/complexProp");
        }

        [TestMethod]
        public void MaterializeEntityShouldWork()
        {
            var odataEntry = new OData.ODataResource() { Id = new Uri("http://www.odata.org/service.svc/entitySet(1)") };
            odataEntry.Properties = new OData.ODataProperty[]
            {
                new OData.ODataProperty { Name = "keyProp", Value = 1 },
                new OData.ODataProperty { Name = "colorProp", Value = new OData.ODataEnumValue("blue") },
                new OData.ODataProperty {
                    Name = "primitiveCollection",
                    Value = new OData.ODataCollectionValue
                    {
                        TypeName = "Edm.Int32",
                        Items = new List<object> {1, 2, 3}
                    }
                },
                new OData.ODataProperty {
                    Name = "enumCollection",
                    Value = new OData.ODataCollectionValue
                    {
                        TypeName = "color",
                        Items = new List<OData.ODataEnumValue> { new OData.ODataEnumValue("white"), new OData.ODataEnumValue("blue") }
                    }
                }
            };

            var complexP = new OData.ODataNestedResourceInfo()
            {
                Name = "complexProp",
                IsCollection = false
            };

            var complexResource = new OData.ODataResource
            {
                Properties = new OData.ODataProperty[]
                {
                    new OData.ODataProperty { Name = "age", Value = 11 },
                    new OData.ODataProperty { Name = "name", Value = "June" }
                }
            };

            var complexColP = new OData.ODataNestedResourceInfo
            {
                Name = "complexCollection",
                IsCollection = true
            };

            var complexColResourceSet = new OData.ODataResourceSet();

            var items = new List<OData.ODataResource>
            {
                new OData.ODataResource
                {
                    Properties = new OData.ODataProperty[]
                    {
                        new OData.ODataProperty { Name = "name", Value = "Aug" },
                        new OData.ODataProperty { Name = "age", Value = 8 },
                        new OData.ODataProperty { Name = "color", Value = new OData.ODataEnumValue("white")}
                    }
                },
                new OData.ODataResource
                {
                    Properties = new OData.ODataProperty[]
                    {
                        new OData.ODataProperty { Name = "name", Value = "Sep" },
                        new OData.ODataProperty { Name = "age", Value = 9 },
                        new OData.ODataProperty { Name = "color", Value = new OData.ODataEnumValue("blue") }
                    }
                }
            };

            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var context = new DataServiceContext();
            var materializerEntry = MaterializerEntry.CreateEntry(odataEntry, OData.ODataFormat.Json, true, clientEdmModel);

            MaterializerNavigationLink.CreateLink(complexP, MaterializerEntry.CreateEntry(complexResource, OData.ODataFormat.Json, true, clientEdmModel));
            MaterializerFeed.CreateFeed(complexColResourceSet, items);
            MaterializerNavigationLink.CreateLink(complexColP, complexColResourceSet);

            var materializerContext = new TestMaterializerContext() { Model = clientEdmModel, Context = context };
            var adapter = new EntityTrackingAdapter(new TestEntityTracker(), MergeOption.OverwriteChanges, clientEdmModel, context);
            QueryComponents components = new QueryComponents(new Uri("http://foo.com/Service"), new Version(4, 0), typeof(EntityType), null, new Dictionary<Expression, Expression>());

            var entriesMaterializer = new ODataEntriesEntityMaterializer(new OData.ODataResource[] { odataEntry }, materializerContext, adapter, components, typeof(EntityType), null, OData.ODataFormat.Json);

            var customersRead = new List<EntityType>();
            while (entriesMaterializer.Read())
            {
                customersRead.Add(entriesMaterializer.CurrentValue as EntityType);
            }

            customersRead.Should().HaveCount(1);
            customersRead[0].KeyProp.Should().Be(1);
            customersRead[0].ComplexProp.Should().Equals(new ComplexType { Age = 11, Name = "June" });
            customersRead[0].ColorProp.Should().Equals(Color.Blue);
            customersRead[0].PrimitiveCollection.Should().Equals(new List<int> { 1, 2, 3 });
            ComplexType complex1 = new ComplexType { Name = "Aug", Age = 8, Color = Color.White };
            ComplexType complex2 = new ComplexType { Name = "Sep", Age = 9, Color = Color.Blue };
            customersRead[0].ComplexCollection.Should().Equals(new List<ComplexType> { complex1, complex2 });
            customersRead[0].EnumCollection.Should().Equals(new List<Color> { Color.White, Color.Blue });
        }

        [TestMethod]
        public void MaterializeComplexTypeShouldWork()
        {
            OData.ODataResource complexValue = new OData.ODataResource
            {
                Properties = new OData.ODataProperty[]
                {
                    new OData.ODataProperty { Name = "age", Value = 11 },
                    new OData.ODataProperty { Name = "name", Value = "June" },
                    new OData.ODataProperty { Name = "color", Value = new OData.ODataEnumValue("blue") },
                    new OData.ODataProperty
                    {
                        Name = "primitives",
                        Value = new OData.ODataCollectionValue
                        {
                            TypeName = "Edm.Int32",
                            Items = new List<object> {1, 2, 3}
                        }
                    },
                    new OData.ODataProperty
                    {
                        Name = "enums",
                        Value = new OData.ODataCollectionValue
                        {
                            TypeName = "color",
                            Items = new List<OData.ODataEnumValue> { new OData.ODataEnumValue("white"), new OData.ODataEnumValue("blue") }
                        }
                    }
                }
            };
            var materializerEntry = MaterializerEntry.CreateEntry(complexValue, OData.ODataFormat.Json, false, new ClientEdmModel(ODataProtocolVersion.V4));
            this.CreateEntryMaterializationPolicy().Materialize(materializerEntry, typeof(ComplexType), false);
            var complex = materializerEntry.ResolvedObject as ComplexType;
            complex.Name.Should().Be("June");
            complex.Age.Should().Be(11);
            complex.Color.Should().Be(Color.Blue);
            complex.Primitives.Should().ContainInOrder(1, 2, 3);
            complex.Enums.Should().ContainInOrder(Color.White, Color.Blue);
        }

        [TestMethod]
        public void MaterializeEnumTypeShouldWork()
        {
            OData.ODataEnumValue enumValue = new OData.ODataEnumValue("blue");
            OData.ODataProperty property = new OData.ODataProperty { Name = "enumProperty", Value = enumValue };
            var enumPolicy = new EnumValueMaterializationPolicy(new TestMaterializerContext());
            var result = enumPolicy.MaterializeEnumTypeProperty(typeof(Color), property);
            property.GetMaterializedValue().Should().Be(Color.Blue);
            result.Should().Be(Color.Blue);
        }

        [TestMethod]
        public void WriteUriOperationParametersShouldUseOriginalName()
        {
            var requestInfo = new RequestInfo(Context);
            var serializer = new Serializer(requestInfo);
            ComplexType complex = new ComplexType()
            {
                Age = 11,
                Name = "June",
                Color = Color.White,
                Primitives = new ObservableCollection<int> { 1, 2, 3 },
                Enums = new ObservableCollection<Color> { Color.White, Color.Blue }
            };
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Function");
            List<UriOperationParameter> parameters = new List<UriOperationParameter>
            {
                new UriOperationParameter("complexArg", complex),
                new UriOperationParameter("color", Color.Blue),
                new UriOperationParameter("colors", new List<Color> { Color.White, Color.Blue })
            };
            string complexParameterString = Uri.EscapeDataString("{\"@odata.type\":\"#" + ServerNamespace + ".complexType\",\"age\":11,\"color@odata.type\":\"#namespace.test.color\",\"color\":\"white\",\"enums@odata.type\":\"#Collection(namespace.test.color)\",\"enums\":[\"white\",\"blue\"],\"name\":\"June\",\"primitives@odata.type\":\"#Collection(Int32)\",\"primitives\":[1,2,3]}");
            // Should use ServerNamespace and type name in schema for enum type in url
            string enumParameterString = Uri.EscapeDataString("namespace.test" + ".color'blue'");
            string enumsParameterString = Uri.EscapeDataString("[\"white\",\"blue\"]");
            Uri expectedUri = new Uri(string.Format("http://www.odata.org/service.svc/Function(complexArg=%40complexArg,color={0},colors=%40colors)?%40complexArg={1}&%40colors={2}", enumParameterString, complexParameterString, enumsParameterString));
            requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);
            Assert.AreEqual(expectedUri, requestUri);
        }

        [TestMethod]
        public void WriteBodyOperationParametersShouldUseOriginalName()
        {
            var requestInfo = new RequestInfo(Context);
            var serializer = new Serializer(requestInfo);
            ComplexType complex = new ComplexType()
            {
                Age = 11,
                Name = "June",
                Color = Color.White,
                Primitives = new ObservableCollection<int> { 1, 2, 3 },
                Enums = new ObservableCollection<Color> { Color.White, Color.Blue }
            };
            Uri requestUri = new Uri("http://www.odata.org/service.svc/Action");
            List<BodyOperationParameter> parameters = new List<BodyOperationParameter>
            {
                new BodyOperationParameter("p1", 1),
                new BodyOperationParameter("complexArg", complex),
                new BodyOperationParameter("color", Color.Blue),
                new BodyOperationParameter("colors", new List<Color> { Color.White, Color.Blue })
            };
            ODataRequestMessageWrapper requestMessage = new RequestInfo(Context).WriteHelper.CreateRequestMessage(Context.CreateRequestArgsAndFireBuildingRequest("POST", requestUri, new HeaderCollection(), Context.HttpStack, null /*descriptor*/));
            const string complexParameterString = "\"complexArg\":{\"@odata.type\":\"#" + ServerNamespace + ".complexType\",\"age\":11,\"color@odata.type\":\"#namespace.test.color\",\"color\":\"white\",\"enums@odata.type\":\"#Collection(" + ServerNamespace + ".color)\",\"enums\":[\"white\",\"blue\"],\"name\":\"June\",\"primitives@odata.type\":\"#Collection(Int32)\",\"primitives\":[1,2,3]}";
            const string enumParameterString = "\"color\":\"blue\"";
            const string enumsParameterString = "\"colors\":[\"white\",\"blue\"]";
            serializer.WriteBodyOperationParameters(parameters, requestMessage);
            MemoryStream stream = (MemoryStream)requestMessage.CachedRequestStream.Stream;
            StreamReader streamReader = new StreamReader(stream);
            String body = streamReader.ReadToEnd();
            body.Should().Contain("\"p1\":1");
            body.Should().Contain(complexParameterString);
            body.Should().Contain(enumParameterString);
            body.Should().Contain(enumsParameterString);
        }

        internal EntryValueMaterializationPolicy CreateEntryMaterializationPolicy(TestMaterializerContext materializerContext = null)
        {
            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var context = new DataServiceContext();
            materializerContext = materializerContext ?? new TestMaterializerContext() { Model = clientEdmModel, Context = context };
            var adapter = new EntityTrackingAdapter(new TestEntityTracker(), MergeOption.OverwriteChanges, clientEdmModel, context);
            var lazyPrimitivePropertyConverter = new Microsoft.OData.Client.SimpleLazy<PrimitivePropertyConverter>(() => new PrimitivePropertyConverter());
            var primitiveValueMaterializerPolicy = new PrimitiveValueMaterializationPolicy(materializerContext, lazyPrimitivePropertyConverter);
            var entryPolicy = new EntryValueMaterializationPolicy(materializerContext, adapter, lazyPrimitivePropertyConverter, null);
            var collectionPolicy = new CollectionValueMaterializationPolicy(materializerContext, primitiveValueMaterializerPolicy);
            var intanceAnnotationPolicy = new InstanceAnnotationMaterializationPolicy(materializerContext);

            entryPolicy.CollectionValueMaterializationPolicy = collectionPolicy;
            entryPolicy.InstanceAnnotationMaterializationPolicy = intanceAnnotationPolicy;

            return entryPolicy;
        }
    }
}
