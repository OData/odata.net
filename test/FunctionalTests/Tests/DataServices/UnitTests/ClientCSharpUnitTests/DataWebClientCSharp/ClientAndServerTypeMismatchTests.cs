//---------------------------------------------------------------------
// <copyright file="ClientAndServerTypeMismatchTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria.Util;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Spatial;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Tests;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using AstoriaUnitTests.ClientExtensions;

    #endregion

    [TestClass]
    public class ClientAndServerTypeMismatchTests
    {
        private readonly Uri serviceUri = new Uri("http://temp.org/");

        private readonly Dictionary<object, object> conversionsFromString = new Dictionary<object, object>
        {
            // from string to all types
            {"true", true},
            {"1", 1},
            {"2", (byte)2},
            {"-1", (sbyte)-1},
            {"3", (short)3},
            {XmlConvert.ToString(long.MaxValue), long.MaxValue},
            {XmlConvert.ToString(float.MaxValue), float.MaxValue},
            {XmlConvert.ToString(double.MinValue), double.MinValue},
            {XmlConvert.ToString(decimal.MaxValue), decimal.MaxValue},
            {XmlConvert.ToString(DateTimeOffset.MinValue), DateTimeOffset.MinValue},
            {XmlConvert.ToString(TimeSpan.MaxValue), TimeSpan.MaxValue},
            {XmlConvert.ToString(Guid.Empty), Guid.Empty},
            {Convert.ToBase64String(new byte[] {0, 1, 2}), new byte[] {0, 1, 2}},
        };

        private readonly Dictionary<object, object> conversionsToString = new Dictionary<object, object>
        {
            // from all types to string
            {true, "true"},
            {1, "1"},
            {(byte)2, "2"},
            {(sbyte)-1, "-1"},
            {(short)3, "3"},
            {long.MaxValue, XmlConvert.ToString(long.MaxValue)},
            {float.MaxValue, XmlConvert.ToString(float.MaxValue)},
            {double.MinValue, XmlConvert.ToString(double.MinValue)},
            {decimal.MaxValue, XmlConvert.ToString(decimal.MaxValue)},
            {DateTime.MinValue, XmlConvert.ToString(TypeData.ConvertDateTimeToDateTimeOffset(DateTime.MinValue))},
            {DateTimeOffset.MinValue, XmlConvert.ToString(DateTimeOffset.MinValue)},
            {TimeSpan.MaxValue, XmlConvert.ToString(TimeSpan.MaxValue)},
            {Guid.Empty, XmlConvert.ToString(Guid.Empty)},
            {new byte[] {0, 1, 2, }, Convert.ToBase64String(new byte[] {0, 1, 2})},
        };

        private readonly Dictionary<object, object> numericConversions = new Dictionary<object, object>
        {
            // from byte to other numeric types
            {(byte)5, (sbyte)5},
            {(byte)6, (short)6},
            {(byte)7, 7},
            {(byte)8, 8L},
            {(byte)9, 9.0f},
            {(byte)10, 10.0},
            {(byte)11, 11m},

            // from sbyte to other numeric types
            {(sbyte)12, (byte)12},
            {(sbyte)-13, (short)-13},
            {(sbyte)-14, -14},
            {(sbyte)-15, -15L},
            {(sbyte)-16, -16.0f},
            {(sbyte)-17, -17.0},
            {(sbyte)-18, -18m},

            // from short to other numeric types
            {(short)19, (byte)19},
            {(short)-20, (sbyte)-20},
            {(short)21, 21},
            {(short)22, 22L},
            {(short)23, 23.0f},
            {(short)24, 24.0},
            {(short)25, 25m},

            // from int to other numeric types
            {26, (byte)26},
            {-27, (sbyte)-27},
            {28, (short)28},
            {29, 29L},
            {30, 30.0f},
            {31, 31.0},
            {32, 32m},

            // from long to other numeric types
            {33L, (byte)33},
            {-34L, (sbyte)-34},
            {35L, (short)35},
            {36L, 36},
            {37L, 37.0f},
            {38L, 38.0},
            {39L, 39m},

            // from float to other numeric types
            {40.0f, (byte)40},
            {-41.0f, (sbyte)-41},
            {42.0f, (short)42},
            {43.0f, 43},
            {44.0f, 44L}, // Converting to long does not work in JSON because the value should have been quoted
            {45.1f, 45.1},
            {46.0f, 46m},

            // from double to other numeric types
            {47.0d, (byte)47},
            {-48.0d, (sbyte)-48},
            {49.0d, (short)49},
            {50.0d, 50},
            {51.0d, 51L}, // Converting to long does not work in JSON because the value should have been quoted
            {52.1d, 52.1f},
            {53.0d, 53m},

            // from decimal to other numeric types
            {54.0m, (byte)54},
            {-55.0m, (sbyte)-55},
            {56.0m, (short)56},
            {57.0m, 57},
            {58.0m, 58L}, // Converting to long does not work in JSON because the value should have been quoted
            {59.1m, 59.1f},
            {60.1m, 60.1d}
        };

        private readonly Dictionary<object, object> spatialConversions = new Dictionary<object, object>
        {
            // from Geometry to Geography (note that coordinate system is unchanged)
            {GeometryPoint.Create(CoordinateSystem.DefaultGeometry, 1, 1, null, null), GeographyPoint.Create(CoordinateSystem.DefaultGeometry, 1, 1, null, null)},

            // from Geography to Geometry (note that coordinate system is unchanged)
            {GeographyPoint.Create(CoordinateSystem.DefaultGeography, 2, 2, null, null), GeometryPoint.Create(CoordinateSystem.DefaultGeography, 2, 2, null, null)},
        };

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
        [Ignore] // Remove Atom
        // [TestMethod]
        public void NullServerPropertyShouldMaterializeIntoAnyType()
        {
            foreach (var format in new[] { UnitTestsUtil.AtomFormat, UnitTestsUtil.JsonLightMimeType })
            {
                // the server type doesn't actually matter here.
                this.ValidatePrimitiveConversion<string, bool?>(null, null, format);
                this.ValidatePrimitiveConversion<string, int?>(null, null, format);
                this.ValidatePrimitiveConversion<string, byte?>(null, null, format);
                this.ValidatePrimitiveConversion<string, sbyte?>(null, null, format);
                this.ValidatePrimitiveConversion<string, short?>(null, null, format);
                this.ValidatePrimitiveConversion<string, long?>(null, null, format);
                this.ValidatePrimitiveConversion<string, float?>(null, null, format);
                this.ValidatePrimitiveConversion<string, double?>(null, null, format);
                this.ValidatePrimitiveConversion<string, decimal?>(null, null, format);
                this.ValidatePrimitiveConversion<string, DateTimeOffset?>(null, null, format);
                this.ValidatePrimitiveConversion<string, TimeSpan?>(null, null, format);
                this.ValidatePrimitiveConversion<string, Guid?>(null, null, format);
                this.ValidatePrimitiveConversion<string, byte[]>(null, null, format);

                this.ValidateComplexConversion<string, string>(null, null, format);
            }
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
        [Ignore] // Remove Atom
        // [TestMethod]
        public void StringValueShouldMaterializeIntoAnyTypeIfValueCanBeConverted()
        {
            this.InvokeForAll(this.conversionsFromString, UnitTestsUtil.JsonLightMimeType);
        }

        [TestMethod]
        public void AllNumericConversionsShouldWorkForJsonIfValuesWouldNotBeQuoted()
        {
            this.InvokeForAll(this.numericConversions.Where(kvp => !(kvp.Value is long || kvp.Value is decimal)), UnitTestsUtil.JsonLightMimeTypeIeee754Compatible);
        }

        [TestMethod]
        public void SpatialConversionsShouldWorkForBothFormats()
        {
            this.InvokeForAll(this.spatialConversions, UnitTestsUtil.JsonLightMimeType);
        }

        private void ValidateAllConversions<TServer, TClient>(TServer serverPropertyValue, TClient expectedClientValue, string contentType)
        {
            this.ValidatePrimitiveConversion(serverPropertyValue, expectedClientValue, contentType);
            this.ValidatePrimitiveCollectionConversion(serverPropertyValue, expectedClientValue, contentType);
            this.ValidateComplexConversion(serverPropertyValue, expectedClientValue, contentType);
            this.ValidateComplexCollectionConversion(serverPropertyValue, expectedClientValue, contentType);
        }

        private void ValidatePrimitiveConversion<TServer, TClient>(TServer serverPropertyValue, TClient expectedClientValue, string contentType)
        {
            var ctx = this.CreateContextWithHardcodedResponse<TServer, TClient>(serverPropertyValue, contentType, false, false);
            TClient property = ctx.CreateQuery<ClientEntityType<TClient>>("Entities").Where(e => e.ID == 0).First().Property;

            CompareValue(expectedClientValue, property);
        }

        private void ValidatePrimitiveCollectionConversion<TServer, TClient>(TServer serverPropertyValue, TClient expectedClientValue, string contentType)
        {
            var ctx = this.CreateContextWithHardcodedResponse<TServer, List<TClient>>(serverPropertyValue, contentType, false, true);
            List<TClient> property = ctx.CreateQuery<ClientEntityType<List<TClient>>>("Entities").Where(e => e.ID == 0).First().Property;

            property.Should().HaveCount(1);
            CompareValue(expectedClientValue, property.Single());
        }

        private void ValidateComplexConversion<TServer, TClient>(TServer serverPropertyValue, TClient expectedClientValue, string contentType)
        {
            var ctx = this.CreateContextWithHardcodedResponse<TServer, ClientComplexType<TClient>>(serverPropertyValue, contentType, true, false);
            TClient property = ctx.CreateQuery<ClientEntityType<ClientComplexType<TClient>>>("Entities").Where(e => e.ID == 0).First().Property.Property;

            CompareValue(expectedClientValue, property);
        }

        private void ValidateComplexCollectionConversion<TServer, TClient>(TServer serverPropertyValue, TClient expectedClientValue, string contentType)
        {
            var ctx = this.CreateContextWithHardcodedResponse<TServer, List<ClientComplexType<TClient>>>(serverPropertyValue, contentType, true, true);
            List<ClientComplexType<TClient>> property = ctx.CreateQuery<ClientEntityType<List<ClientComplexType<TClient>>>>("Entities").Where(e => e.ID == 0).First().Property;

            property.Should().HaveCount(1);
            CompareValue(expectedClientValue, property.Single().Property);
        }

        private static string GeneratePayload<TServer, TClient>(TServer serverPropertyValue, string contentType, bool complex, bool collection)
        {
            if (contentType == UnitTestsUtil.AtomFormat)
            {
                const string atomTemplate = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<entry xml:base=""/"" xmlns:ads=""http://docs.oasis-open.org/odata/ns/data"" xmlns:adsm=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <id>http://temp.org/Entities(0)</id>
  <content type=""application/xml"">
    <adsm:properties>
      <ads:ID>0</ads:ID>
      <ads:Property{0}>{1}</ads:Property>
    </adsm:properties>
  </content>
</entry>";

                string attributes = " adsm:type=\"Edm." + GetPrimitiveTypeKind<TServer>() + "\"";
                string propertyValue = null;
                if (serverPropertyValue == null)
                {
                    attributes += " adsm:null=\"true\"";
                }
                else if (typeof(ISpatial).IsAssignableFrom(typeof(TServer)))
                {
                    var xelement = new XElement("fake");
                    using (var writer = xelement.CreateWriter())
                    {
                        GmlFormatter.Create().Write((ISpatial)serverPropertyValue, writer);
                    }

                    propertyValue = xelement.Elements().Single().ToString();
                }
                else
                {
                    propertyValue = TypeData.XmlValueFromObject(serverPropertyValue).Replace(".0", null);
                }

                const string complexTemplate = "<ads:Property{0}>{1}</ads:Property>";
                if (complex)
                {
                    propertyValue = string.Format(complexTemplate, attributes, propertyValue);
                    attributes = null;
                }

                const string collectionTemplate = "<adsm:element{0}>{1}</adsm:element>";
                if (collection)
                {
                    propertyValue = string.Format(collectionTemplate, attributes, propertyValue);
                    attributes = null;
                }

                return string.Format(atomTemplate, attributes, propertyValue);
            }
            else
            {
                const string jsonTemplate = @"{{ @odata.context:""http://temp.org/$metadata#Fake.Container.Entities/$entity"", ID:0,{0} Property:{1} }}";

                string attributes = "\"Property@odata.type\":\"Edm." + GetPrimitiveTypeKind<TServer>() + "\",";
                string propertyValue;

                if (typeof(TServer) == typeof(DateTime))
                {
                    propertyValue = '"' + XmlConvert.ToString((DateTime)(object)serverPropertyValue, XmlDateTimeSerializationMode.RoundtripKind) + '"';
                }
                else if (typeof(TServer) == typeof(DateTimeOffset))
                {
                    propertyValue = '"' + XmlConvert.ToString((DateTimeOffset)(object)serverPropertyValue) + '"';
                }
                else if (serverPropertyValue == null)
                {
                    propertyValue = "null";
                }
                else if (typeof(ISpatial).IsAssignableFrom(typeof(TServer)))
                {
                    var properties = GeoJsonObjectFormatter.Create().Write((ISpatial)serverPropertyValue);
                    StringBuilder builder = new StringBuilder();
                    AppendGeoJsonProperties(builder, properties);

                    propertyValue = builder.ToString();
                }
                else
                {
                    propertyValue = JsonPrimitiveTypesUtil.PrimitiveToString(serverPropertyValue, null).Replace(".0", null);
                }

                const string complexTemplate = "{{{0}\"Property\":{1}}}";
                if (complex)
                {
                    propertyValue = string.Format(complexTemplate, attributes, propertyValue);
                    attributes = null;
                }

                const string collectionTemplate = "[ {0} ]";
                if (collection)
                {
                    propertyValue = string.Format(collectionTemplate, propertyValue);
                    attributes = null;
                }

                return string.Format(jsonTemplate, attributes, propertyValue);
            }
        }

        private static void AppendGeoJsonProperties(StringBuilder builder, IEnumerable<KeyValuePair<string, object>> properties)
        {
            builder.Append('{');
            bool first = true;
            foreach (var property in properties)
            {
                if (!first)
                {
                    builder.Append(',');
                }

                first = false;

                builder.Append(property.Key + ": ");
                AppendGeoJsonPropertyValue(builder, property.Value);
            }
            builder.Append('}');
        }

        private static void AppendGeoJsonPropertyValue(StringBuilder builder, object value)
        {
            if (value is string || value is double)
            {
                builder.Append(JsonPrimitiveTypesUtil.PrimitiveToString(value, null));
            }
            else if (value is IEnumerable<KeyValuePair<string, object>>)
            {
                AppendGeoJsonProperties(builder, (IEnumerable<KeyValuePair<string, object>>)value);
            }
            else if (value is IEnumerable)
            {
                builder.Append('[');
                bool first = true;
                foreach (var element in ((IEnumerable)value).Cast<object>())
                {
                    if (!first)
                    {
                        builder.Append(',');
                    }

                    first = false;

                    AppendGeoJsonPropertyValue(builder, element);
                }

                builder.Append(']');
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private DataServiceContext CreateContextWithHardcodedResponse<TServer, TClient>(TServer serverPropertyValue, string contentType, bool complex, bool collection)
        {
            var payload = GeneratePayload<TServer, TClient>(serverPropertyValue, contentType, complex, collection);

            var requestMessage = new ODataTestMessage();
            var responseMessage = new ODataTestMessage { StatusCode = 200, MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            responseMessage.SetHeader("Content-Type", contentType);
            responseMessage.SetHeader("Content-Length", responseMessage.MemoryStream.Length.ToString());
            responseMessage.SetHeader("OData-Version", "4.0");

            var ctx = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, requestMessage, responseMessage);
            //ctx.EnableAtom = true;

            if (contentType == UnitTestsUtil.JsonLightMimeType || contentType == UnitTestsUtil.JsonLightMimeTypeIeee754Compatible)
            {
                var model = new EdmModel();
                var edmEntityType = new EdmEntityType("Fake", "ServerType");
                edmEntityType.AddKeys(edmEntityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
                model.AddElement(edmEntityType);

                EdmPrimitiveTypeKind kind = GetPrimitiveTypeKind<TServer>();

                if (!complex)
                {
                    if (!collection)
                    {
                        edmEntityType.AddStructuralProperty("Property", kind, true);
                    }
                    else
                    {
                        edmEntityType.AddStructuralProperty("Property", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetPrimitive(kind, false))));
                    }
                }
                else
                {
                    var complexType = new EdmComplexType("Fake", "ComplexType");
                    complexType.AddStructuralProperty("Property", kind, true);
                    model.AddElement(complexType);

                    if (!collection)
                    {
                        edmEntityType.AddStructuralProperty("Property", new EdmComplexTypeReference(complexType, true));
                    }
                    else
                    {
                        edmEntityType.AddStructuralProperty("Property", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complexType, false))));
                    }
                }

                var entityContainer = new EdmEntityContainer("Fake", "Container");
                entityContainer.AddEntitySet("Entities", edmEntityType);
                model.AddElement(entityContainer);

                ctx.Format.UseJson(model);
                ctx.ResolveType = n => n == "Fake.ServerType" ? typeof(ClientEntityType<TClient>) : null;
                ctx.ResolveName = t => t == typeof(ClientEntityType<TClient>) ? "Fake.ServerType" : null;
            }

            return ctx;
        }

        private static EdmPrimitiveTypeKind GetPrimitiveTypeKind<T>()
        {
            Type type = typeof(T);
            if (type == typeof(byte[]))
            {
                return EdmPrimitiveTypeKind.Binary;
            }

            if (type == typeof(TimeSpan))
            {
                return EdmPrimitiveTypeKind.Duration;
            }

            if (typeof(ISpatial).IsAssignableFrom(type))
            {
                type = type.BaseType;
            }

            if (type == typeof(DateTime))
            {
                return EdmPrimitiveTypeKind.DateTimeOffset;
            }

            return (EdmPrimitiveTypeKind)Enum.Parse(typeof(EdmPrimitiveTypeKind), type.Name);
        }

        private static void CompareValue<TClient>(TClient expected, TClient actual)
        {
            if (expected == null)
            {
                actual.Should().BeNull();
            }
            else if (typeof(ISpatial).IsAssignableFrom(typeof(TClient)))
            {
                actual.Should().BeAssignableTo<TClient>();
                var formatter = WellKnownTextSqlFormatter.Create();
                formatter.Write((ISpatial)actual).Should().Be(formatter.Write((ISpatial)expected));
            }
            else if (typeof(TClient) != typeof(byte[]))
            {
                actual.Should().Be(expected);
            }
            else
            {
                actual.Should().BeAssignableTo<byte[]>();
                actual.As<byte[]>().Should().HaveCount(expected.As<byte[]>().Length);
                actual.As<byte[]>().Should().ContainInOrder(expected.As<byte[]>());
            }
        }

        private void InvokeForAll(IEnumerable<KeyValuePair<object, object>> values, string contentType)
        {
            var method = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(m => m.Name == "ValidateAllConversions" && m.GetGenericArguments().Count() == 2);
            foreach (var keyValuePair in values)
            {
                var toInvoke = method.MakeGenericMethod(keyValuePair.Key.GetType(), keyValuePair.Value.GetType());
                toInvoke.Invoke(this, new[] { keyValuePair.Key, keyValuePair.Value, contentType });
            }
        }
    }

    public class ClientEntityType<T>
    {
        public int ID { get; set; }
        public T Property { get; set; }
    }

    public class ClientComplexType<T>
    {
        public T Property { get; set; }
    }
}