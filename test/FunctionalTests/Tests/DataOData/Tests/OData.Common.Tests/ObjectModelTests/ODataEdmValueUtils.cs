//---------------------------------------------------------------------
// <copyright file="ODataEdmValueUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Values;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Helper class for comparing the ODataLib IEdmValue implementations
    /// </summary>
    internal static class ODataEdmValueUtils
    {
        private static readonly Type odataEdmStructuredValueType = 
            typeof(ODataFormat).Assembly.GetType("Microsoft.OData.Core.Evaluation.ODataEdmStructuredValue");

        private static readonly Type odataTypeAnnotationType =
            typeof(ODataFormat).Assembly.GetType("Microsoft.OData.Core.ODataTypeAnnotation");

        internal static void CompareValue(IEdmValue edmValue, object odataValue, AssertionHandler assert)
        {
            if (odataValue == null)
            {
                ValidateNullValue(edmValue, assert);
                return;
            }

            ODataResource entry = odataValue as ODataResource;
            if (entry != null)
            {
                CompareStructuralValue(edmValue, entry, assert);
                return;
            }

            ODataComplexValue complexValue = odataValue as ODataComplexValue;
            if (complexValue != null)
            {
                CompareStructuralValue(edmValue, complexValue, assert);
                return;
            }

            ODataCollectionValue collectionValue = odataValue as ODataCollectionValue;
            if (collectionValue != null)
            {
                CompareCollectionValue(edmValue, collectionValue, assert);
                return;
            }

            ComparePrimitiveValue(edmValue, odataValue, assert);
        }

        internal static IEdmValue CreateStructuredEdmValue(ODataResource entry, IEdmEntitySet entitySet, IEdmEntityTypeReference entityType)
        {
            if (entitySet != null)
            {
                object typeAnnotation = ReflectionUtils.CreateInstance(
                    odataTypeAnnotationType,
                    new Type[] { typeof(IEdmEntitySet), typeof(IEdmEntityTypeReference) },
                    entitySet, entityType);
                entry.SetAnnotation(typeAnnotation);
            }

            return (IEdmValue)ReflectionUtils.CreateInstance(
                odataEdmStructuredValueType, 
                new Type[] { typeof(ODataResource) },
                entry);
        }

        internal static IEdmValue CreateStructuredEdmValue(ODataComplexValue complexValue, IEdmComplexTypeReference complexType)
        {
            if (complexType != null)
            {
                object typeAnnotation = ReflectionUtils.CreateInstance(
                    odataTypeAnnotationType,
                    new Type[] { typeof(IEdmComplexTypeReference) },
                    complexType);
                complexValue.SetAnnotation(typeAnnotation);
            }

            return (IEdmValue)ReflectionUtils.CreateInstance(
                odataEdmStructuredValueType,
                new Type[] { typeof(ODataComplexValue) },
                complexValue);
        }

        private static void ComparePrimitiveValue(IEdmValue edmValue, object odataValue, AssertionHandler assert)
        {
            TypeCode typeCode = Type.GetTypeCode(odataValue.GetType());
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    assert.AreEqual(EdmValueKind.Boolean, edmValue.ValueKind, "Value kinds differ.");
                    assert.AreEqual(odataValue, ((IEdmBooleanValue)edmValue).Value, "Values differ.");
                    if (edmValue.Type != null)
                    {
                        assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                        assert.AreEqual(EdmPrimitiveTypeKind.Boolean, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                    }

                    return;

                case TypeCode.Byte:
                    assert.AreEqual(EdmValueKind.Integer, edmValue.ValueKind, "Value kinds differ.");
                    assert.AreEqual(odataValue, (byte)((IEdmIntegerValue)edmValue).Value, "Values differ.");
                    if (edmValue.Type != null)
                    {
                        assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                        assert.AreEqual(EdmPrimitiveTypeKind.Byte, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                    }

                    return;

                case TypeCode.SByte:
                    assert.AreEqual(EdmValueKind.Integer, edmValue.ValueKind, "Value kinds differ.");
                    assert.AreEqual(odataValue, (sbyte)((IEdmIntegerValue)edmValue).Value, "Values differ.");
                    if (edmValue.Type != null)
                    {
                        assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                        assert.AreEqual(EdmPrimitiveTypeKind.SByte, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                    }

                    return;

                case TypeCode.Int16:
                    assert.AreEqual(EdmValueKind.Integer, edmValue.ValueKind, "Value kinds differ.");
                    assert.AreEqual(odataValue, (Int16)((IEdmIntegerValue)edmValue).Value, "Values differ.");
                    if (edmValue.Type != null)
                    {
                        assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                        assert.AreEqual(EdmPrimitiveTypeKind.Int16, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                    }

                    return;

                case TypeCode.Int32:
                    assert.AreEqual(EdmValueKind.Integer, edmValue.ValueKind, "Value kinds differ.");
                    assert.AreEqual(odataValue, (Int32)((IEdmIntegerValue)edmValue).Value, "Values differ.");
                    if (edmValue.Type != null)
                    {
                        assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                        assert.AreEqual(EdmPrimitiveTypeKind.Int32, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                    }

                    return;

                case TypeCode.Int64:
                    assert.AreEqual(EdmValueKind.Integer, edmValue.ValueKind, "Value kinds differ.");
                    assert.AreEqual(odataValue, (Int64)((IEdmIntegerValue)edmValue).Value, "Values differ.");
                    if (edmValue.Type != null)
                    {
                        assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                        assert.AreEqual(EdmPrimitiveTypeKind.Int64, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                    }

                    return;

                case TypeCode.Decimal:
                    assert.AreEqual(EdmValueKind.Decimal, edmValue.ValueKind, "Value kinds differ.");
                    assert.AreEqual(odataValue, ((IEdmDecimalValue)edmValue).Value, "Values differ.");
                    if (edmValue.Type != null)
                    {
                        assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                        assert.AreEqual(EdmPrimitiveTypeKind.Decimal, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                    }

                    return;

                case TypeCode.Single:
                    assert.AreEqual(EdmValueKind.Floating, edmValue.ValueKind, "Value kinds differ.");
                    assert.AreEqual(odataValue, (Single)((IEdmFloatingValue)edmValue).Value, "Values differ.");
                    if (edmValue.Type != null)
                    {
                        assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                        assert.AreEqual(EdmPrimitiveTypeKind.Single, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                    }

                    return;

                case TypeCode.Double:
                    assert.AreEqual(EdmValueKind.Floating, edmValue.ValueKind, "Value kinds differ.");
                    assert.AreEqual(odataValue, ((IEdmFloatingValue)edmValue).Value, "Values differ.");
                    if (edmValue.Type != null)
                    {
                        assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                        assert.AreEqual(EdmPrimitiveTypeKind.Double, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                    }

                    return;

                case TypeCode.String:
                    assert.AreEqual(EdmValueKind.String, edmValue.ValueKind, "Value kinds differ.");
                    assert.AreEqual(odataValue, ((IEdmStringValue)edmValue).Value, "Values differ.");
                    if (edmValue.Type != null)
                    {
                        assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                        assert.AreEqual(EdmPrimitiveTypeKind.String, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                    }

                    return;

                default:
                    byte[] bytes = odataValue as byte[];
                    if (bytes != null)
                    {
                        assert.AreEqual(EdmValueKind.Binary, edmValue.ValueKind, "Value kinds differ.");
                        assert.AreEqual(odataValue, ((IEdmBinaryValue)edmValue).Value, "Values differ.");
                        if (edmValue.Type != null)
                        {
                            assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                            assert.AreEqual(EdmPrimitiveTypeKind.Binary, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                        }

                        return;
                    }

                    if (odataValue is DateTimeOffset)
                    {
                        assert.AreEqual(EdmValueKind.DateTimeOffset, edmValue.ValueKind, "Value kinds differ.");
                        assert.AreEqual(odataValue, ((IEdmDateTimeOffsetValue)edmValue).Value, "Values differ.");
                        if (edmValue.Type != null)
                        {
                            assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                            assert.AreEqual(EdmPrimitiveTypeKind.DateTimeOffset, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                        }

                        return;
                    }

                    if (odataValue is Guid)
                    {
                        assert.AreEqual(EdmValueKind.Guid, edmValue.ValueKind, "Value kinds differ.");
                        assert.AreEqual(odataValue, ((IEdmGuidValue)edmValue).Value, "Values differ.");
                        if (edmValue.Type != null)
                        {
                            assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                            assert.AreEqual(EdmPrimitiveTypeKind.Guid, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                        }

                        return;
                    }

                    if (odataValue is TimeSpan)
                    {
                        assert.AreEqual(EdmValueKind.Duration, edmValue.ValueKind, "Value kinds differ.");
                        assert.AreEqual(odataValue, ((IEdmDurationValue)edmValue).Value, "Values differ.");
                        if (edmValue.Type != null)
                        {
                            assert.AreEqual(EdmTypeKind.Primitive, edmValue.Type.Definition.TypeKind, "EDM type kinds differ.");
                            assert.AreEqual(EdmPrimitiveTypeKind.Duration, edmValue.Type.PrimitiveKind(), "EDM primitive kinds differ.");
                        }

                        return;
                    }

                    if (odataValue is ISpatial)
                    {
                        // TODO: [JsonLight] Add support for spatial values in ODataEdmStructuredValue
                        throw new System.NotSupportedException();
                    }

                    assert.Fail("Unsupported primitive type: " + odataValue.GetType().FullName);
                    return;
            }
        }

        private static void CompareStructuralValue(IEdmValue edmValue, ODataResource entry, AssertionHandler assert)
        {
            assert.IsNotNull(edmValue, "EDM value instance must not be null.");

            if (entry == null)
            {
                ValidateNullValue(edmValue, assert);
                return;
            }

            assert.AreEqual(EdmValueKind.Structured, edmValue.ValueKind, "Value kinds differ.");
            if (edmValue.Type != null)
            {
                assert.AreEqual(EdmTypeKind.Entity, edmValue.Type.TypeKind(), "Type kinds differ.");
            }

            CompareStructuralValue(edmValue, entry.Properties, assert);
        }

        private static void CompareStructuralValue(IEdmValue edmValue, ODataComplexValue complexValue, AssertionHandler assert)
        {
            assert.IsNotNull(edmValue, "EDM value instance must not be null.");

            if (complexValue == null)
            {
                ValidateNullValue(edmValue, assert);
                return;
            }

            assert.AreEqual(EdmValueKind.Structured, edmValue.ValueKind, "Value kinds differ.");
            if (edmValue.Type != null)
            {
                assert.AreEqual(EdmTypeKind.Complex, edmValue.Type.TypeKind(), "Type kinds differ.");
            }

            CompareStructuralValue(edmValue, complexValue.Properties, assert);
        }

        private static void CompareStructuralValue(IEdmValue edmValue, IEnumerable<ODataProperty> properties, AssertionHandler assert)
        {
            IEdmStructuredValue structuredEdmValue = (IEdmStructuredValue)edmValue;
            if (properties != null)
            {
                // Use FindPropertyValue
                foreach (ODataProperty property in properties)
                {
                    IEdmPropertyValue edmPropertyValue = structuredEdmValue.FindPropertyValue(property.Name);
                    CompareProperty(edmPropertyValue, property, assert);
                }

                // Enumerate the properties
                CompareProperties(structuredEdmValue.PropertyValues, properties, assert);
            }
            else
            {
                assert.IsTrue(
                    structuredEdmValue.PropertyValues == null || structuredEdmValue.PropertyValues.Count() == 0,
                    "Expected empty structured value.");
            }
        }

        private static void CompareProperties(IEnumerable<IEdmPropertyValue> edmProperties, IEnumerable<ODataProperty> odataProperties, AssertionHandler assert)
        {
            using (IEnumerator<IEdmPropertyValue> edmEnumerator = edmProperties.GetEnumerator())
            using (IEnumerator<ODataProperty> odataEnumerator = odataProperties.GetEnumerator())
            {
                while (odataEnumerator.MoveNext())
                {
                    assert.IsTrue(edmEnumerator.MoveNext(), "Expected more EDM properties.");
                    CompareProperty(edmEnumerator.Current, odataEnumerator.Current, assert);
                }

                assert.IsFalse(edmEnumerator.MoveNext(), "Expected no more EDM properties.");
            }
        }

        private static void CompareProperty(IEdmPropertyValue edmProperty, ODataProperty odataProperty, AssertionHandler assert)
        {
            assert.AreEqual(edmProperty.Name, odataProperty.Name, "Property names don't match.");
            CompareValue(edmProperty.Value, odataProperty.Value, assert);
        }

        private static void CompareCollectionValue(IEdmValue edmValue, ODataCollectionValue collectionValue, AssertionHandler assert)
        {
            assert.IsNotNull(edmValue, "EDM value instance must not be null.");

            if (collectionValue == null)
            {
                ValidateNullValue(edmValue, assert);
                return;
            }

            assert.AreEqual(EdmValueKind.Collection, edmValue.ValueKind, "Value kinds differ.");
            if (edmValue.Type != null)
            {
                assert.AreEqual(EdmTypeKind.Collection, edmValue.Type.TypeKind(), "Type kinds differ.");
            }

            IEdmCollectionValue edmCollectionValue = (IEdmCollectionValue)edmValue;
            IEnumerable items = collectionValue.Items;
            if (items != null)
            {
                CompareCollectionItems(edmCollectionValue.Elements, items, assert);
            }
            else
            {
                assert.IsTrue(
                    edmCollectionValue.Elements == null || edmCollectionValue.Elements.Count() == 0,
                    "Expected empty collection value.");
            }
        }

        private static void CompareCollectionItems(IEnumerable<IEdmDelayedValue> edmItems, IEnumerable odataItems, AssertionHandler assert)
        {
            using (IEnumerator<IEdmDelayedValue> edmEnumerator = edmItems.GetEnumerator())
            using (IEnumerator<object> odataEnumerator = odataItems.Cast<object>().GetEnumerator())
            {
                while (odataEnumerator.MoveNext())
                {
                    assert.IsTrue(edmEnumerator.MoveNext(), "Expected more EDM items.");
                    CompareValue(edmEnumerator.Current.Value, odataEnumerator.Current, assert);
                }

                assert.IsFalse(edmEnumerator.MoveNext(), "Expected no more EDM items.");
            }
        }

        private static void ValidateNullValue(IEdmValue edmValue, AssertionHandler assert)
        {
            assert.AreEqual(EdmValueKind.Null, edmValue.ValueKind, "Expected null value kind.");
            assert.IsTrue(edmValue is IEdmNullValue, "Expected IEdmNullValue instance.");
        }
    }
}
