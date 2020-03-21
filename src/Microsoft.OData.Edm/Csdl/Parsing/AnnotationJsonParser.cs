//---------------------------------------------------------------------
// <copyright file="AnnotationJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// Provides functionalities for parsing CSDL-JSON annotations.
    /// </summary>
    internal static class AnnotationJsonParser
    {
        // out of line annotations
        /*
         * "org.example": {
  "$Alias": "self",
  "$Annotations": {
    "self.Person": {
      "@Core.Description#Tablet": "Dummy",
      …
    }
  }
}
         * */
        /// <summary>
        /// Try to parse the input json value <see cref="IJsonValue"/> to <see cref="CsdlAnnotation"/>.
        /// </summary>
        /// <param name="annotionName">The input annotation name, annotation name should start with '@'.</param>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath"></param>
        /// <param name="options"></param>
        /// <param name="elementName">output the element name if this annotation for an element.
        /// for example the enum member annotation. 
        /// "TwoDay@Core.Description": "Shipped within two days",
        /// Here, elementName has the "TwoDay"
        /// </param>
        /// <returns></returns>
        public static CsdlAnnotation TryParseCsdlAnnotation(string annotionName, IJsonValue jsonValue, JsonPath jsonPath, CsdlSerializerOptions options, out string elementName)
        {
            // An annotation is represented as a member whose name consists of an at (@) character,
            // followed by the qualified name of a term, optionally followed by a hash (#) and a qualifier.
            string qualifier;
            string termName;
            if (!ParseAnnotationName(annotionName, out elementName, out termName, out qualifier))
            {
                return null;
            }

            CsdlLocation location = new CsdlLocation(jsonPath.ToString());

            jsonPath.Push(annotionName);
            CsdlExpressionBase expression = BuildExpression(jsonValue, jsonPath);
            jsonPath.Pop();

            return new CsdlAnnotation(termName, qualifier, expression, location);
        }

        public static CsdlExpressionBase BuildExpression(IJsonValue jsonValue, JsonPath jsonPath)
        {
            return null;
        }

        public static CsdlExpressionBase BuildExpression(IJsonValue jsonValue, JsonPath jsonPath, IEdmTypeReference edmType)
        {
            EdmTypeKind termTypeKind = edmType.TypeKind();
            switch (termTypeKind)
            {
                case EdmTypeKind.Primitive:
                    IEdmPrimitiveTypeReference primitiveType = (IEdmPrimitiveTypeReference)edmType;
                    return BuildPrimitiveExpression(jsonValue, jsonPath, primitiveType);

                case EdmTypeKind.Complex:
                case EdmTypeKind.Entity:
                    IEdmStructuredTypeReference structuredType = (IEdmStructuredTypeReference)edmType;
                    return BuildRecordExpression(jsonValue, jsonPath, structuredType);

                case EdmTypeKind.Enum:
                    return BuildEnumMemberExpression(jsonValue, jsonPath, edmType.AsEnum());

                case EdmTypeKind.TypeDefinition:
                    break;

                case EdmTypeKind.Path:
                    break;

                case EdmTypeKind.Collection:
                    return BuildCollectionExpression(jsonValue, jsonPath, edmType.AsCollection());

                case EdmTypeKind.Untyped:
                    // So far, we don't support a Untyped term.
                default:
                    // A valid term should not be here.
                    Debug.Assert(false, "We should be here never for a valid term.");
                    break;
            }

            return null;
        }

        private static CsdlConstantExpression BuildPrimitiveExpression(IJsonValue jsonValue, JsonPath jsonPath, IEdmPrimitiveTypeReference primitiveTypeReference)
        {
            CsdlLocation location = new CsdlLocation(jsonPath.ToString());

            JsonPrimitiveValue constantValue = jsonValue.ValidateRequiredJsonValue<JsonPrimitiveValue>(jsonPath);
            if (constantValue.Value == null)
            {
                // verify the nullable?
                return new CsdlConstantExpression(EdmValueKind.Null, "null", location);
            }

            EdmValueKind kind = EdmValueKind.None;
            string value = null;
            switch (primitiveTypeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    value = jsonValue.ParseAsStringPrimitive(jsonPath);
                    kind = EdmValueKind.Binary;
                    break;

                case EdmPrimitiveTypeKind.Boolean:
                    bool? boolValue = jsonValue.ParseAsBooleanPrimitive(jsonPath);
                    value = boolValue.Value ? "true" : "false";
                    kind = EdmValueKind.Boolean;
                    break;

                case EdmPrimitiveTypeKind.Date:
                    kind = EdmValueKind.Date;
                    value = jsonValue.ParseAsStringPrimitive(jsonPath);
                    break;

                case EdmPrimitiveTypeKind.DateTimeOffset:
                    kind = EdmValueKind.DateTimeOffset;
                    value = jsonValue.ParseAsStringPrimitive(jsonPath);
                    break;

                case EdmPrimitiveTypeKind.Decimal:
                    kind = EdmValueKind.Decimal;
                    value = jsonValue.ParseAsStringPrimitive(jsonPath);
                    break;

                case EdmPrimitiveTypeKind.Duration:
                    kind = EdmValueKind.Duration;
                    value = jsonValue.ParseAsStringPrimitive(jsonPath);
                    break;

                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Double:
                    kind = EdmValueKind.Floating;
                    double? doubleValue = jsonValue.ParseAsFloatPrimitive(jsonPath);
                    value = doubleValue.Value.ToString(CultureInfo.InvariantCulture);
                    break;

                case EdmPrimitiveTypeKind.Guid:
                    kind = EdmValueKind.Guid;
                    value = jsonValue.ParseAsStringPrimitive(jsonPath);
                    break;

                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                    int? intValue = jsonValue.ParseAsIntegerPrimitive(jsonPath);
                    value = intValue.Value.ToString(CultureInfo.InvariantCulture);
                    kind = EdmValueKind.Integer;
                    break;

                case EdmPrimitiveTypeKind.TimeOfDay:
                    value = jsonValue.ParseAsStringPrimitive(jsonPath);
                    kind = EdmValueKind.TimeOfDay;
                    break;

                case EdmPrimitiveTypeKind.String:
                    kind = EdmValueKind.String;
                    value = jsonValue.ParseAsStringPrimitive(jsonPath);
                    break;

                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.PrimitiveType:
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                case EdmPrimitiveTypeKind.None:
                    return null;
            }

            return new CsdlConstantExpression(kind, value, location);
        }

        private static CsdlEnumMemberExpression BuildEnumMemberExpression(IJsonValue jsonValue, JsonPath jsonPath, IEdmEnumTypeReference enumType)
        {
           // JsonPrimitiveValue enumValue = jsonValue.ValidateRequiredJsonValue<JsonPrimitiveValue>(jsonPath);

            string enumMemberPath = jsonValue.ParseAsStringPrimitive(jsonPath);

            return new CsdlEnumMemberExpression(enumMemberPath, new CsdlLocation(jsonPath.ToString()));
        }

        private static CsdlCollectionExpression BuildCollectionExpression(IJsonValue jsonValue, JsonPath jsonPath, IEdmCollectionTypeReference collectionType)
        {
            JsonArrayValue jsonArrayValue = jsonValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);

            CsdlTypeReference csdlTypeReference = ParseTypeReference(collectionType, jsonPath);

            IEdmTypeReference elementType = collectionType.AsCollection().ElementType();

            IList<CsdlExpressionBase> itemExpressions = new List<CsdlExpressionBase>();
            jsonArrayValue.ProcessItem(jsonPath, (v) =>
            {
                itemExpressions.Add(BuildExpression(v, jsonPath, elementType));
            });

            return new CsdlCollectionExpression(csdlTypeReference, itemExpressions, new CsdlLocation(jsonPath.ToString()));
        }

        public static CsdlTypeReference ParseTypeReference(IEdmTypeReference typeReference,
            JsonPath jsonPath)
        {
            bool isCollection = false;
            IEdmTypeReference elementType = typeReference;
            if (typeReference.IsCollection())
            {
                isCollection = true;
                elementType = typeReference.AsCollection().ElementType();
            }

            bool isNullable = typeReference.IsNullable;

            bool isBounded = false;
            int? maxLength;
            bool? unicode;
            int? precision;
            int? scale;
            int? srid;
            GetFacts(elementType, out isBounded, out maxLength, out unicode, out precision, out scale, out srid);

            return SchemaJsonParser.ParseTypeReference(elementType.FullName(), isCollection,
                isNullable, isBounded, maxLength, unicode, precision, scale, srid, new CsdlLocation(jsonPath.ToString()));
        }

        private static void GetFacts(IEdmTypeReference edmTypeReference, out bool isBounded,
            out int? maxLength, out bool? unicode, out int? precision, out int? scale, out int? srid)
        {
            isBounded = false;
            maxLength = null;
            unicode = null;
            precision = null;
            scale = null;
            srid = null;

            IEdmType typeDefinition = edmTypeReference.Definition;
            if (typeDefinition.TypeKind == EdmTypeKind.Primitive)
            {
                var primitiveDefinition = typeDefinition as IEdmPrimitiveType;
                if (primitiveDefinition != null)
                {
                    switch (primitiveDefinition.PrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Boolean:
                        case EdmPrimitiveTypeKind.Byte:
                        case EdmPrimitiveTypeKind.Date:
                        case EdmPrimitiveTypeKind.Double:
                        case EdmPrimitiveTypeKind.Guid:
                        case EdmPrimitiveTypeKind.Int16:
                        case EdmPrimitiveTypeKind.Int32:
                        case EdmPrimitiveTypeKind.Int64:
                        case EdmPrimitiveTypeKind.SByte:
                        case EdmPrimitiveTypeKind.Single:
                        case EdmPrimitiveTypeKind.Stream:
                        case EdmPrimitiveTypeKind.PrimitiveType:
                            return;

                        case EdmPrimitiveTypeKind.Binary:
                            IEdmBinaryTypeReference binaryType = edmTypeReference.AsBinary();
                            isBounded = binaryType.IsUnbounded;
                            maxLength = binaryType.MaxLength;
                            return;
                        case EdmPrimitiveTypeKind.Decimal:
                            IEdmDecimalTypeReference decimalTypeReference = edmTypeReference.AsDecimal();
                            precision = decimalTypeReference.Precision;
                            scale = decimalTypeReference.Scale;
                            return;
                        case EdmPrimitiveTypeKind.String:
                            IEdmStringTypeReference stringType = edmTypeReference.AsString();
                            isBounded = stringType.IsUnbounded;
                            maxLength = stringType.MaxLength;
                            unicode = stringType.IsUnicode;
                            return;
                        case EdmPrimitiveTypeKind.Duration:
                        case EdmPrimitiveTypeKind.DateTimeOffset:
                        case EdmPrimitiveTypeKind.TimeOfDay:
                            IEdmTemporalTypeReference temporal = edmTypeReference.AsTemporal();
                            precision = temporal.Precision;
                            return;
                        case EdmPrimitiveTypeKind.Geography:
                        case EdmPrimitiveTypeKind.GeographyPoint:
                        case EdmPrimitiveTypeKind.GeographyLineString:
                        case EdmPrimitiveTypeKind.GeographyPolygon:
                        case EdmPrimitiveTypeKind.GeographyCollection:
                        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                        case EdmPrimitiveTypeKind.GeographyMultiLineString:
                        case EdmPrimitiveTypeKind.GeographyMultiPoint:
                        case EdmPrimitiveTypeKind.Geometry:
                        case EdmPrimitiveTypeKind.GeometryPoint:
                        case EdmPrimitiveTypeKind.GeometryLineString:
                        case EdmPrimitiveTypeKind.GeometryPolygon:
                        case EdmPrimitiveTypeKind.GeometryCollection:
                        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                        case EdmPrimitiveTypeKind.GeometryMultiLineString:
                        case EdmPrimitiveTypeKind.GeometryMultiPoint:
                            IEdmSpatialTypeReference spatialType = edmTypeReference.AsSpatial();
                            srid = spatialType.SpatialReferenceIdentifier;
                            return;
                        case EdmPrimitiveTypeKind.None:
                            break;
                    }
                }
            }
            else if (typeDefinition.TypeKind == EdmTypeKind.TypeDefinition)
            {
                IEdmTypeDefinitionReference reference = edmTypeReference as IEdmTypeDefinitionReference;
                if (reference == null)
                {
                    // No facet available if not IEdmTypeDefinitionReference.
                    return;
                }

                isBounded = reference.IsUnbounded;
                maxLength = reference.MaxLength;
                unicode = reference.IsUnicode;
                precision = reference.Precision;
                scale = reference.Scale;
                srid = reference.SpatialReferenceIdentifier;
            }

            return;
        }

        private static CsdlRecordExpression BuildRecordExpression(IJsonValue jsonValue, JsonPath jsonPath, IEdmStructuredTypeReference structuredType)
        {
            JsonObjectValue jsonObjectValue = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            CsdlLocation location = new CsdlLocation(jsonPath.ToString());
            CsdlTypeReference typReference = new CsdlNamedTypeReference(false, null, null, null, null, null, structuredType.FullName(), structuredType.IsNullable, location);

            IList<CsdlPropertyValue> propertyValues = new List<CsdlPropertyValue>();

            foreach (var property in structuredType.StructuredDefinition().Properties())
            {
                jsonPath.Push(property.Name);

                IJsonValue propertyJsonValue;
                if (jsonObjectValue.TryGetValue(property.Name, out propertyJsonValue))
                {
                    CsdlExpressionBase propertyExpression = BuildExpression(propertyJsonValue, jsonPath, property.Type);
                    CsdlPropertyValue csdlPropertyValue = new CsdlPropertyValue(property.Name, propertyExpression, new CsdlLocation(jsonPath.ToString()));
                    propertyValues.Add(csdlPropertyValue);
                }
                else
                {
                    // Shall we check nullable?
                    if (property.Type.IsNullable)
                    {
                        // TODO:
                    }
                }

                jsonPath.Pop();
            }


            return new CsdlRecordExpression(typReference, propertyValues, location);
        }

        /// <summary>
        /// Parse the input string to see whether it's a valid annotation name.
        /// If it's valid, seperate string into term name or optional qualifier name.
        /// It it's not valid, return false
        /// </summary>
        /// <param name="annotationName">The input annotation name.</param>
        /// <param name="term"></param>
        /// <param name="qualifier"></param>
        /// <returns></returns>
        internal static bool ParseAnnotationName(string annotationName, out string elementName, out string term, out string qualifier)
        {
            term = null;
            qualifier = null;
            elementName = null;
            if (string.IsNullOrWhiteSpace(annotationName))
            {
                return false;
            }

            // Annotation name consists of an at (@) character,
            // BE caution:
            // An annotation can itself be annotated. Annotations on annotations are represented as a member
            // whose name consists of the annotation name (including the optional qualifier),
            // followed by an at (@) character, followed by the qualified name of a term, optionally followed by a hash (#) and a qualifier.
            // for example: 
            // "@Measures.ISOCurrency": "USD",
            // "@Measures.ISOCurrency@Core.Description": "The parent company’s currency"
            //
            // So, Core.Description is annotation for "Measures.ISOCurrency annotation.
            // Therefore, we use "LastIndexOf".
            int index = annotationName.LastIndexOf('@');
            if (index == -1)
            {
                return false;
            }

            if (index != 0)
            {
                elementName = annotationName.Substring(0, index);
                annotationName = annotationName.Substring(index + 1);
            }

            // followed by the qualified name of a term, optionally followed by a hash (#) and a qualifier
            index = annotationName.IndexOf('#');
            if (index != -1)
            {
                term = annotationName.Substring(0, index);
                qualifier = annotationName.Substring(index + 1);
            }
            else
            {
                term = annotationName;
            }

            return true;
        }
    }
}
