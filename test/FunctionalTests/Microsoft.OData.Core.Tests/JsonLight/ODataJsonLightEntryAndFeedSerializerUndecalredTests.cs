namespace Microsoft.Test.OData.TDD.Tests.Writer.JsonLight
{
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.OData.Core.Tests;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using System;
    using System.IO;
    using System.Text;
    using Xunit;

    public class ODataJsonLightEntryAndFeedSerializerUndeclaredTests
    {
        private Uri metadataDocumentUri = new Uri("http://odata.org/test/$metadata/");

        private EdmModel serverModel;
        private EdmEntityType serverEntityType;
        private EdmEntityType serverOpenEntityType;
        private EdmEntitySet serverEntitySet;
        private EdmEntitySet serverOpenEntitySet;

        public ODataJsonLightEntryAndFeedSerializerUndeclaredTests()
        {
            this.serverModel = new EdmModel();
            var addressType = new EdmComplexType("Server.NS", "Address");
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            this.serverModel.AddElement(addressType);

            // non-open entity type
            this.serverEntityType = new EdmEntityType("Server.NS", "ServerEntityType");
            this.serverModel.AddElement(this.serverEntityType);
            this.serverEntityType.AddKeys(this.serverEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.serverEntityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));

            // open entity type
            this.serverOpenEntityType = new EdmEntityType("Server.NS", "ServerOpenEntityType",
                baseType: null, isAbstract: false, isOpen: true);
            this.serverModel.AddElement(this.serverOpenEntityType);
            this.serverOpenEntityType.AddKeys(this.serverOpenEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.serverOpenEntityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));

            EdmEntityContainer container = new EdmEntityContainer("Server.NS", "container1");
            this.serverEntitySet = container.AddEntitySet("serverEntitySet", this.serverEntityType);
            this.serverOpenEntitySet = container.AddEntitySet("serverOpenEntitySet", this.serverOpenEntityType);
            this.serverModel.AddElement(container);
        }

        private ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings
        {
            UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty
        };

        #region non-open entity's property unknown name + known value type

        [Fact]
        public void WriteEntryUndeclaredPropertiesTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties= new []
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteNonOpenEntryUndeclaredPropertiesWithNullValueTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredType1", Value = null},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredType1"":null,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteNonOpenEntryUndeclaredPropertiesTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteNonOpenEntryUndeclaredComplexPropertiesTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredComplexType1", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.1000,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.1001,Zixing Rd Minhang")},
                            },
                        }},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredComplexType1"":{""Street"":""No.1000,Zixing Rd Minhang"",""UndeclaredStreet"":""No.1001,Zixing Rd Minhang""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteNonOpenEntryUndeclaredEmptyComplexPropertiesTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredComplexType1", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new ODataProperty[]{},
                        }},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredComplexType1"":{},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteNonOpenEntryUndeclaredCollectionPropertiesTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredCollection1", Value = new ODataCollectionValue(){
                            TypeName = "Collection(Edm.String)",
                            Items = new[]
                            {
                                "mystr1",
                                "mystr2",
                                "mystr3"
                            }}},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredCollection1"":[""mystr1"",""mystr2"",""mystr3""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteNonOpenEntryUndeclaredEmptyCollectionPropertiesTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredCollection1", Value = new ODataCollectionValue(){
                            TypeName = "Collection(Edm.String)",
                            Items = new string[]{},
                        }},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredCollection1"":[],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        #endregion

        #region non-open entity's property unknown name + unknown value type
        [Fact]
        public void WriteEntryUntypedFloatDoubleTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataUndeclaredPropertyValue(){RawValue="12.3"}},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties= new []
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreetNo", Value = new ODataUndeclaredPropertyValue(){RawValue="12.0"}},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreetNo"":12.0}}");
        }

        [Fact]
        public void WriteEntryUntypedStringTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties= new []
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataUndeclaredPropertyValue(){RawValue=@"""No.10000000999,Zixing Rd Minhang"""}},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteEntryUntypedComplexTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "UndeclaredAddress1", Value = 
                            new ODataUndeclaredPropertyValue(){RawValue=@"{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}"}
                        },
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""UndeclaredAddress1"":{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}");
        }

        [Fact]
        public void WriteEntryUntypedCollectionTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "UndeclaredCollection1", Value = 
                            new ODataUndeclaredPropertyValue(){RawValue=@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]"}
                        },
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""UndeclaredCollection1"":[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]}");
        }
        #endregion

        #region open entity's property unknown name + known value type

        [Fact]
        public void WriteOpenEntryUndeclaredPropertiesWithNullValueTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredType1", Value = null},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredType1"":null,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteOpenEntryUndeclaredPropertiesTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }
        [Fact]
        public void WriteOpenEntryUndeclaredComplexPropertiesTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredComplexType1", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.1000,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.1001,Zixing Rd Minhang")},
                            },
                        }},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredComplexType1"":{""@odata.type"":""#Server.NS.Address"",""Street"":""No.1000,Zixing Rd Minhang"",""UndeclaredStreet"":""No.1001,Zixing Rd Minhang""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteOpenEntryUndeclaredEmptyComplexPropertiesTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredComplexType1", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new ODataProperty[]{},
                        }},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredComplexType1"":{""@odata.type"":""#Server.NS.Address""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteOpenEntryUndeclaredCollectionPropertiesTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredCollection1", Value = new ODataCollectionValue(){
                            TypeName = "Collection(Edm.String)",
                            Items = new[]
                            {
                                "mystr1",
                                "mystr2",
                                "mystr3"
                            }}},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredCollection1@odata.type"":""#Collection(String)"",""UndeclaredCollection1"":[""mystr1"",""mystr2"",""mystr3""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteOpenEntryUndeclaredEmptyCollectionPropertiesTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredCollection1", Value = new ODataCollectionValue(){
                            TypeName = "Collection(Edm.String)",
                            Items = new string[]{},
                        }},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties = new[]
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredCollection1@odata.type"":""#Collection(String)"",""UndeclaredCollection1"":[],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        #endregion

        #region open entity's property unknown name + unknown value type
        [Fact]
        public void WriteOpenEntryUntypedFloatDoubleTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataUndeclaredPropertyValue(){RawValue="12.3"}},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties= new []
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreetNo", Value = new ODataUndeclaredPropertyValue(){RawValue="12.0"}},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreetNo"":12.0}}");
        }

        [Fact]
        public void WriteOpenEntryUntypedStringTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "Address", Value = new ODataComplexValue()
                        {
                            TypeName = "Server.NS.Address",
                            Properties= new []
                            {
                                new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                                new ODataProperty{Name = "UndeclaredStreet", Value = new ODataUndeclaredPropertyValue(){RawValue=@"""No.10000000999,Zixing Rd Minhang"""}},
                            },
                        }},
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}");
        }

        [Fact]
        public void WriteOpenEntryUntypedComplexTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "UndeclaredAddress1", Value = 
                            new ODataUndeclaredPropertyValue(){RawValue=@"{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}"}
                        },
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""UndeclaredAddress1"":{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}");
        }

        [Fact]
        public void WriteOpenEntryUntypedCollectionTest()
        {
            var entry = new ODataEntry
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "UndeclaredCollection1", Value = 
                            new ODataUndeclaredPropertyValue(){RawValue=@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]"}
                        },
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""UndeclaredCollection1"":[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]}");
        }

        #endregion

        private string WriteEntryPayload(EdmEntitySet entitySet, EdmEntityType entityType, Action<ODataWriter> action)
        {
            MemoryStream stream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage() { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.sampletest.com"));
            using (var msgReader = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, this.serverModel))
            {
                var writer = msgReader.CreateODataEntryWriter(entitySet, entityType);
                action(writer);

                stream.Seek(0, SeekOrigin.Begin);
                string payload = (new StreamReader(stream)).ReadToEnd();
                return payload;
            }
        }

        private ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, bool writingResponse = true, bool setMetadataDocumentUri = true)
        {
            IEdmModel model = new EdmModel();

            ODataMessageWriterSettings settings = new ODataMessageWriterSettings
            {
                Version = ODataVersion.V4
            };
            if (setMetadataDocumentUri)
            {
                settings.SetServiceDocumentUri(this.metadataDocumentUri);
            }

            return new ODataJsonLightOutputContext(
                ODataFormat.Json,
                new NonDisposingStream(stream),
                new ODataMediaType("application", "json"),
                Encoding.UTF8,
                settings,
                writingResponse,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null);
        }
    }
}
