﻿//---------------------------------------------------------------------
// <copyright file="TestUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Tests
{
    public static class TestUtils
    {
        /// <summary>
        /// Creates a new ODataResource from the specified entity set, instance, and type.
        /// </summary>
        /// <param name="entitySet">Entity set for the new entry.</param>
        /// <param name="value">Entity instance for the new entry.</param>
        /// <param name="entityType">Entity type for the new entry.</param>
        /// <returns>New ODataResource with the specified entity set and type, property values from the specified instance.</returns>
        internal static ODataResource CreateODataEntry(IEdmEntitySet entitySet, IEdmStructuredValue value, IEdmEntityType entityType)
        {
            var entry = new ODataResource();
            entry.TypeAnnotation = new ODataTypeAnnotation(entityType.FullTypeName());
            entry.Properties = value.PropertyValues.Select(p =>
            {
                object propertyValue;
                if (p.Value.ValueKind == EdmValueKind.Null)
                {
                    propertyValue = null;
                }
                else if (p.Value is IEdmPrimitiveValue)
                {
                    propertyValue = ((IEdmPrimitiveValue)p.Value).ToClrValue();
                }
                else
                {
                    Assert.True(false, "Test only currently supports creating ODataResource from IEdmPrimitiveValue instances.");
                    return null;
                }

                return new ODataProperty() { Name = p.Name, Value = propertyValue };
            });

            return entry;
        }

        /// <summary>
        /// Easily wrap existing models as referenced models, and return a main model.
        /// </summary>
        /// <param name="referencedModels">NONE of them should have the container to extend.</param>
        /// <returns>The main model.</returns>
        public static IEdmModel WrapReferencedModelsToMainModel(params IEdmModel[] referencedModels)
        {
            Assert.True(referencedModels[0] != null, "referencedModels[0] != null");
            string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Test.Chh"" Alias=""NO_Alias"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""MainModel_NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            IEdmModel ret;
            IEnumerable<EdmError> errors;
            if (CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new List<IEdmModel>(referencedModels), out ret, out errors))
            {
                return ret;
            }

            Assert.False(errors.Any(), "should be zero error.");
            return null;
        }

        /// <summary>
        /// Easily wrap existing models as referenced models, and return a main model.
        /// </summary>
        /// <param name="namespaceOfContainerToExtend"></param>
        /// <param name="nameOfContainerToExtend"></param>
        /// <param name="referencedModels">Ths first one should have the container (of namespaceOfContainerToExtend) to extend.</param>
        /// <returns>The main model.</returns>
        public static IEdmModel WrapReferencedModelsToMainModel(string namespaceOfContainerToExtend, string nameOfContainerToExtend, params IEdmModel[] referencedModels)
        {
            Assert.True(nameOfContainerToExtend + "_sub" == referencedModels[0].EntityContainer.Name, "the container name of '" + nameOfContainerToExtend + "' will be used by main model, so the container name in referenced model must have been appended with '_sub'.");
            string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Test.Chh"" Alias=""NO_Alias"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""{0}"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityContainer Name=""{1}"" Extends=""{0}.{1}_sub"">
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            mainModelxml = string.Format(mainModelxml, namespaceOfContainerToExtend, nameOfContainerToExtend);
            IEdmModel ret;
            IEnumerable<EdmError> errors;
            if (CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new List<IEdmModel>(referencedModels), out ret, out errors))
            {
                return ret;
            }

            Assert.False(errors.Any(), "should be zero error.");
            return null;
        }
        #region Util methods to AssertAreEqual ODataValues

        public static void AssertODataResourceAreEqual(ODataResource value1, ODataResource value2)
        {
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.Equal(value1.TypeName, value2.TypeName);
            AssertODataPropertiesAreEqual(value1.Properties, value2.Properties);
        }

        public static void AssertODataResourceSetAreEqual(List<ODataResource> collectionValue1, List<ODataResource> collectionValue2)
        {
            Assert.NotNull(collectionValue1);
            Assert.NotNull(collectionValue2);

            Assert.Equal(collectionValue1.Count, collectionValue2.Count);
            for (int i = 0; i < collectionValue1.Count; i++)
            {
                AssertODataResourceAreEqual(collectionValue1[i], collectionValue2[i]);
            }
        }

        public static void AssertODataValueAreEqual(ODataValue value1, ODataValue value2)
        {
            if (value1.IsNullValue && value2.IsNullValue)
            {
                return;
            }

            ODataPrimitiveValue primitiveValue1 = value1 as ODataPrimitiveValue;
            ODataPrimitiveValue primitiveValue2 = value2 as ODataPrimitiveValue;
            if (primitiveValue1 != null && primitiveValue2 != null)
            {
                AssertODataPrimitiveValueAreEqual(primitiveValue1, primitiveValue2);
            }
            else
            {
                ODataResourceValue resourceValue1 = value1 as ODataResourceValue;
                ODataResourceValue resourceValue2 = value2 as ODataResourceValue;
                if (resourceValue1 != null && resourceValue2 != null)
                {
                    AssertODataResourceValueAreEqual(resourceValue1, resourceValue2);
                    return;
                }

                ODataEnumValue enumValue1 = value1 as ODataEnumValue;
                ODataEnumValue enumValue2 = value2 as ODataEnumValue;
                if (enumValue1 != null && enumValue2 != null)
                {
                    AssertODataEnumValueAreEqual(enumValue1, enumValue2);
                }
                else
                {
                    ODataCollectionValue collectionValue1 = value1 as ODataCollectionValue;
                    ODataCollectionValue collectionValue2 = value2 as ODataCollectionValue;
                    if (collectionValue1 != null && collectionValue2 != null)
                    {
                        AssertODataCollectionValueAreEqual(collectionValue1, collectionValue2);
                    }
                    else
                    {
                        ODataUntypedValue untyped1 = value1 as ODataUntypedValue;
                        ODataUntypedValue untyped2 = value2 as ODataUntypedValue;
                        Assert.Equal(untyped1.RawValue, untyped2.RawValue);
                    }
                }
            }
        }

        private static void AssertODataCollectionValueAreEqual(ODataCollectionValue collectionValue1, ODataCollectionValue collectionValue2)
        {
            Assert.NotNull(collectionValue1);
            Assert.NotNull(collectionValue2);
            Assert.Equal(collectionValue1.TypeName, collectionValue2.TypeName);
            var itemsArray1 = collectionValue1.Items.OfType<object>().ToArray();
            var itemsArray2 = collectionValue2.Items.OfType<object>().ToArray();

            Assert.Equal(itemsArray1.Length, itemsArray2.Length);
            for (int i = 0; i < itemsArray1.Length; i++)
            {
                var odataValue1 = itemsArray1[i] as ODataValue;
                var odataValue2 = itemsArray2[i] as ODataValue;
                if (odataValue1 != null && odataValue2 != null)
                {
                    AssertODataValueAreEqual(odataValue1, odataValue2);
                }
                else
                {
                    Assert.Equal(itemsArray1[i], itemsArray2[i]);
                }
            }
        }

        private static void AssertODataResourceValueAreEqual(ODataResourceValue resourceValue1, ODataResourceValue resourceValue2)
        {
            Assert.NotNull(resourceValue1);
            Assert.NotNull(resourceValue2);
            Assert.Equal(resourceValue1.TypeName, resourceValue2.TypeName);
            AssertODataPropertiesAreEqual(resourceValue1.Properties, resourceValue2.Properties);
        }

        public static void AssertODataPropertiesAreEqual(IEnumerable<ODataProperty> properties1, IEnumerable<ODataProperty> properties2)
        {
            if (properties1 == null && properties2 == null)
            {
                return;
            }

            Assert.NotNull(properties1);
            Assert.NotNull(properties2);
            var propertyArray1 = properties1.ToArray();
            var propertyArray2 = properties2.ToArray();
            Assert.Equal(propertyArray1.Length, propertyArray2.Length);
            for (int i = 0; i < propertyArray1.Length; i++)
            {
                AssertODataPropertyAreEqual(propertyArray1[i], propertyArray2[i]);
            }
        }

        public static void AssertODataPropertyAreEqual(ODataProperty odataProperty1, ODataProperty odataProperty2)
        {
            Assert.NotNull(odataProperty1);
            Assert.NotNull(odataProperty2);
            Assert.Equal(odataProperty1.Name, odataProperty2.Name);
            AssertODataValueAreEqual(ToODataValue(odataProperty1.Value), ToODataValue(odataProperty2.Value));
        }

        private static ODataValue ToODataValue(object value)
        {
            if (value == null)
            {
                return new ODataNullValue();
            }

            var odataValue = value as ODataValue;
            if (odataValue != null)
            {
                return odataValue;
            }

            return new ODataPrimitiveValue(value);
        }

        private static void AssertODataPrimitiveValueAreEqual(ODataPrimitiveValue primitiveValue1, ODataPrimitiveValue primitiveValue2)
        {
            Assert.NotNull(primitiveValue1);
            Assert.NotNull(primitiveValue2);
            Assert.Equal(primitiveValue1.Value, primitiveValue2.Value);
        }

        private static void AssertODataEnumValueAreEqual(ODataEnumValue enumValue1, ODataEnumValue enumValue2)
        {
            Assert.NotNull(enumValue1);
            Assert.NotNull(enumValue2);
            Assert.Equal(enumValue1.Value, enumValue2.Value);
            Assert.Equal(enumValue1.TypeName, enumValue2.TypeName);
        }
        #endregion Util methods to AssertAreEqual ODataValues
    }
}
