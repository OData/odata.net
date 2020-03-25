//---------------------------------------------------------------------
// <copyright file="AnnotationJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Csdl.Json.Value;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Json.Builder
{
    /// <summary>
    /// Provides functionalities for parsing CSDL-JSON annotations.
    /// </summary>
    internal class AnnotationJsonBuilder
    {
        private CsdlSerializerOptions _options;

        public AnnotationJsonBuilder(CsdlSerializerOptions options)
        {
            _options = options;
        }

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
        /// Build the <see cref="IJsonValue"/> to dynamic <see cref="IEdmExpression"/>.
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath">Track on path of this JSON value.</param>
        /// <returns></returns>
        public IEdmExpression BuildExpression(IJsonValue jsonValue, IJsonPath jsonPath)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                throw new ArgumentNullException("jsonPath");
            }

            if (_options.Indented)
            {
                return null;
            }

            switch (jsonValue.ValueKind)
            {
                case JsonValueKind.JPrimitive:
                    return BuildConstantExpression((JsonPrimitiveValue)jsonValue, jsonPath);

                case JsonValueKind.JObject:
                    return BuildObjectExpression((JsonObjectValue)jsonValue, jsonPath);

                case JsonValueKind.JArray:
                default:
                    return BuildCollectionExpression((JsonArrayValue)jsonValue, jsonPath);
            }
        }

        /// <summary>
        /// Build the <see cref="IJsonValue"/> to <see cref="IEdmExpression"/>
        /// using a term type.
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath">Track on path of this JSON value.</param>
        /// <param name="termType"></param>
        /// <returns></returns>
        public IEdmExpression BuildExpression(IJsonValue jsonValue, IJsonPath jsonPath, IEdmTypeReference termType)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                throw new ArgumentNullException("jsonPath");
            }

            if (termType == null)
            {
                throw new ArgumentNullException("termType");
            }

            EdmTypeKind termTypeKind = termType.TypeKind();
            switch (termTypeKind)
            {
                case EdmTypeKind.Primitive:
                    IEdmPrimitiveTypeReference primitiveType = (IEdmPrimitiveTypeReference)termType;
                    return BuildPrimitiveExpression(jsonValue, jsonPath, primitiveType);

                case EdmTypeKind.Complex:
                case EdmTypeKind.Entity:
                    JsonObjectValue objectValue = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);
                    IEdmStructuredTypeReference structuredType = (IEdmStructuredTypeReference)termType;
                    return BuildRecordExpression(objectValue, jsonPath, structuredType);

                case EdmTypeKind.Enum:
                    return BuildEnumMemberExpression(jsonValue, jsonPath, termType.AsEnum());

                case EdmTypeKind.TypeDefinition:
                    break;

                case EdmTypeKind.Path:
                    return BuildModelPathExpression(jsonValue, jsonPath, termType.AsPath());

                case EdmTypeKind.Collection:
                    JsonArrayValue arrayValue = jsonValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);
                    return BuildCollectionExpression(arrayValue, jsonPath, termType.AsCollection());

                case EdmTypeKind.Untyped:
                // So far, we don't support an Untyped term.
                default:
                    // A valid term should not be here.
                    return BuildExpression(jsonValue, jsonPath);
            }

            return null;
        }

        /// <summary>
        /// Build a model path such as "Annotation Path, Model Element Path, Navigation Property Path and Property Path".
        /// Annotation Path: represented as a string containing a path.
        /// Model element path: represented as a string containing a path.
        /// Navigation property path: represented as a string containing a path.
        /// Property path : represented as a string containing a path.
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath"></param>
        /// <param name="pathType"></param>
        /// <returns></returns>
        private static IEdmPathExpression BuildModelPathExpression(IJsonValue jsonValue, IJsonPath jsonPath, IEdmPathTypeReference pathType)
        {
            // A model path is represented as a string containing a path.

            string path = jsonValue.ParseAsString(jsonPath);

            EdmPathTypeKind kind = pathType.PathKind();
            switch (kind)
            {
                case EdmPathTypeKind.AnnotationPath:
                    return new EdmAnnotationPathExpression(path);

                case EdmPathTypeKind.PropertyPath:
                    return new EdmPropertyPathExpression(path);

                case EdmPathTypeKind.NavigationPropertyPath:
                    return new EdmNavigationPropertyPathExpression(path);

                // so far, ODL doesn't support "Model Element Path" and others
                case EdmPathTypeKind.None:
                default:
                    break;
            }

            return null;
        }

        private IEdmExpression BuildObjectExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // $Path
            if (objectValue.ContainsKey("$Path"))
            {
                return BuildValuePathExpression(objectValue, jsonPath);
            }

            // $Apply
            if (objectValue.ContainsKey("$Apply"))
            {
                return BuildApplyExpression(objectValue, jsonPath);
            }

            // $Cast
            if (objectValue.ContainsKey("$Cast"))
            {
                return BuildCastExpression(objectValue, jsonPath);
            }

            // $If
            if (objectValue.ContainsKey("$If"))
            {
                return BuildIfExpression(objectValue, jsonPath);
            }

            // $IsOf
            if (objectValue.ContainsKey("$IsOf"))
            {
                return BuildIsOfExpression(objectValue, jsonPath);
            }

            // $LabeledElement
            if (objectValue.ContainsKey("$LabeledElement"))
            {
                return BuildLabeledElementExpression(objectValue, jsonPath);
            }

            // $LabeledElementReference
            if (objectValue.ContainsKey("$LabeledElementReference"))
            {
                return BuildLabeledElementReferenceExpression(objectValue, jsonPath);
            }

            // For all others, let's try build it as "Record"
            return BuildRecordExpression(objectValue, jsonPath);
        }

        private IEdmExpression BuildCollectionExpression(JsonArrayValue arrayValue, IJsonPath jsonPath, IEdmCollectionTypeReference collectionTypeReference = null)
        {
            IEdmTypeReference elementType = null;
            if (collectionTypeReference != null)
            {
                elementType = collectionTypeReference.ElementType();
            }
            IList<IEdmExpression> elements = new List<IEdmExpression>();
            arrayValue.ProcessItem(jsonPath, (v) =>
            {
                IEdmExpression element;
                if (elementType == null)
                {
                    element = BuildExpression(v, jsonPath);
                }
                else
                {
                    element = BuildExpression(v, jsonPath, elementType);
                }

                elements.Add(element);
            });

            return new EdmCollectionExpression(collectionTypeReference, elements);
        }

        private static IEdmExpression BuildConstantExpression(JsonPrimitiveValue primitiveValue, IJsonPath jsonPath)
        {
            // From JsonReader, the primitiveValue (not nullable) could be:
            // null
            // String,
            // boolean
            // int
            // decimal
            // double
            if (primitiveValue.Value == null)
            {
                return EdmNullExpression.Instance;
            }

            object value = primitiveValue.Value;
            Type valueType = value.GetType();

            // Boolean
            if (valueType == typeof(bool))
            {
                return new EdmBooleanConstant((bool)value);
            }

            // Integer
            if (valueType == typeof(int) || valueType == typeof(long))
            {
                return new EdmIntegerConstant((long)value);
            }

            // decimal
            if (valueType == typeof(decimal))
            {
                return new EdmDecimalConstant((decimal)value);
            }

            // double
            if (valueType == typeof(float) || valueType == typeof(double))
            {
                return new EdmFloatingConstant((double)value);
            }

            // string
            if (valueType == typeof(string))
            {
                string stringValue = (string)value;

                // NaN, INF, -INF are special values both for decimal and floating.
                // System.Decimal doesn't have special value defined
                // So try to use "double"
                if (stringValue == "NaN")
                {
                    return new EdmFloatingConstant(double.NaN);
                }

                if (stringValue == "INF")
                {
                    return new EdmFloatingConstant(double.PositiveInfinity);
                }

                if (stringValue == "-INF")
                {
                    return new EdmFloatingConstant(double.NegativeInfinity);
                }

                // Date
                Date? dateValue;
                if (EdmValueParser.TryParseDate(stringValue, out dateValue))
                {
                    return new EdmDateConstant(dateValue.Value);
                }

                // TimeOfDay
                TimeOfDay? timeOfDateValue;
                if (EdmValueParser.TryParseTimeOfDay(stringValue, out timeOfDateValue))
                {
                    return new EdmTimeOfDayConstant(timeOfDateValue.Value);
                }

                // DateTimeOffset
                DateTimeOffset? dateTimeOffsetResult;
                if (EdmValueParser.TryParseDateTimeOffset(stringValue, out dateTimeOffsetResult))
                {
                    return new EdmDateTimeOffsetConstant(dateTimeOffsetResult.Value);
                }

                // Guid
                Guid? guidValue;
                if (EdmValueParser.TryParseGuid(stringValue, out guidValue))
                {
                    return new EdmGuidConstant(guidValue.Value);
                }

                // Duration
                TimeSpan? timeSpan;
                if(EdmValueParser.TryParseDuration(stringValue, out timeSpan))
                {
                    return new EdmDurationConstant(timeSpan.Value);
                }

                // Binary
                byte[] binaryValue;
                if (EdmValueParser.TryParseBinary(stringValue, out binaryValue))
                {
                    return new EdmBinaryConstant(binaryValue);
                }

                // We can't distiguish the string from enum member string.
                // So for others, let's return as string constant.
                return new EdmStringConstant(stringValue);
            }

            throw new Exception();
        }

        private static IEdmExpression BuildPrimitiveExpression(IJsonValue jsonValue, IJsonPath jsonPath, IEdmPrimitiveTypeReference primitiveTypeReference)
        {
            // make sure the input JSON value is a primitive value also.
            JsonPrimitiveValue constantValue = jsonValue.ValidateRequiredJsonValue<JsonPrimitiveValue>(jsonPath);
            if (constantValue.Value == null)
            {
                // TODO: should verify the nullability of the primitiveType reference?
                return EdmNullExpression.Instance;
            }

            string value = null;
            switch (primitiveTypeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    value = jsonValue.ParseAsString(jsonPath);
                    byte[] binaryValue;
                    if (EdmValueParser.TryParseBinary(value, out binaryValue))
                    {
                        return new EdmBinaryConstant(binaryValue);
                    }
                    break;

                case EdmPrimitiveTypeKind.Boolean:
                    bool? boolValue = jsonValue.ParseAsBoolean(jsonPath);
                    return new EdmBooleanConstant((bool)boolValue);

                case EdmPrimitiveTypeKind.Date:
                    value = jsonValue.ParseAsString(jsonPath);
                    Date? dateValue;
                    if (EdmValueParser.TryParseDate(value, out dateValue))
                    {
                        return new EdmDateConstant(dateValue.Value);
                    }
                    break;

                case EdmPrimitiveTypeKind.DateTimeOffset:
                    value = jsonValue.ParseAsString(jsonPath);
                    DateTimeOffset? dateTimeOffsetResult;
                    if (EdmValueParser.TryParseDateTimeOffset(value, out dateTimeOffsetResult))
                    {
                        return new EdmDateTimeOffsetConstant(dateTimeOffsetResult.Value);
                    }
                    break;

                case EdmPrimitiveTypeKind.Decimal:
                    return new EdmDecimalConstant((decimal)constantValue.Value);

                case EdmPrimitiveTypeKind.Duration:
                    value = jsonValue.ParseAsString(jsonPath);
                    TimeSpan? timeSpan;
                    if (EdmValueParser.TryParseDuration(value, out timeSpan))
                    {
                        return new EdmDurationConstant(timeSpan.Value);
                    }
                    break;

                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Double:
                    return new EdmFloatingConstant((double)constantValue.Value);

                case EdmPrimitiveTypeKind.Guid:
                    value = jsonValue.ParseAsString(jsonPath);
                    Guid? guidValue;
                    if (EdmValueParser.TryParseGuid(value, out guidValue))
                    {
                        return new EdmGuidConstant(guidValue.Value);
                    }
                    break;

                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                    return new EdmIntegerConstant((long)constantValue.Value);

                case EdmPrimitiveTypeKind.TimeOfDay:
                    value = jsonValue.ParseAsString(jsonPath);
                    TimeOfDay? timeOfDateValue;
                    if (EdmValueParser.TryParseTimeOfDay(value, out timeOfDateValue))
                    {
                        return new EdmTimeOfDayConstant(timeOfDateValue.Value);
                    }
                    break;

                case EdmPrimitiveTypeKind.String:
                    return new EdmStringConstant(value);

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

            throw new Exception();
        }


        private static IEdmEnumMemberExpression BuildEnumMemberExpression(IJsonValue jsonValue, IJsonPath jsonPath, IEdmEnumTypeReference enumType)
        {
          //  JsonPrimitiveValue enumValue = jsonValue.ValidateRequiredJsonValue<JsonPrimitiveValue>(jsonPath);

            string enumMemberPath = jsonValue.ParseAsString(jsonPath);

            return new EdmEnumMemberExpression(TryParserEnumMembers(enumMemberPath).ToArray());
           // return new CsdlEnumMemberExpression(enumMemberPath, new CsdlLocation(jsonPath.ToString()));
        }

        private static IList<IEdmEnumMember> TryParserEnumMembers(string enumMemberPath)
        {
            return null;
        }

        //private static CsdlCollectionExpression BuildCollectionExpression(IJsonValue jsonValue, JsonPath jsonPath, IEdmCollectionTypeReference collectionType)
        //{
        //    JsonArrayValue jsonArrayValue = jsonValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);

        //    CsdlTypeReference csdlTypeReference = ParseTypeReference(collectionType, jsonPath);

        //    IEdmTypeReference elementType = collectionType.AsCollection().ElementType();

        //    IList<CsdlExpressionBase> itemExpressions = new List<CsdlExpressionBase>();
        //    jsonArrayValue.ProcessItem(jsonPath, (v) =>
        //    {
        //        itemExpressions.Add(BuildExpression(v, jsonPath, elementType));
        //    });

        //    return new CsdlCollectionExpression(csdlTypeReference, itemExpressions, new CsdlLocation(jsonPath.ToString()));
        //}

        //public static CsdlTypeReference ParseTypeReference(IEdmTypeReference typeReference,
        //    JsonPath jsonPath)
        //{
        //    bool isCollection = false;
        //    IEdmTypeReference elementType = typeReference;
        //    if (typeReference.IsCollection())
        //    {
        //        isCollection = true;
        //        elementType = typeReference.AsCollection().ElementType();
        //    }

        //    bool isNullable = typeReference.IsNullable;

        //    bool isBounded = false;
        //    int? maxLength;
        //    bool? unicode;
        //    int? precision;
        //    int? scale;
        //    int? srid;
        //    GetFacts(elementType, out isBounded, out maxLength, out unicode, out precision, out scale, out srid);

        //    return null;
        //}

        //private static void GetFacts(IEdmTypeReference edmTypeReference, out bool isBounded,
        //    out int? maxLength, out bool? unicode, out int? precision, out int? scale, out int? srid)
        //{
        //    isBounded = false;
        //    maxLength = null;
        //    unicode = null;
        //    precision = null;
        //    scale = null;
        //    srid = null;

        //    IEdmType typeDefinition = edmTypeReference.Definition;
        //    if (typeDefinition.TypeKind == EdmTypeKind.Primitive)
        //    {
        //        var primitiveDefinition = typeDefinition as IEdmPrimitiveType;
        //        if (primitiveDefinition != null)
        //        {
        //            switch (primitiveDefinition.PrimitiveKind)
        //            {
        //                case EdmPrimitiveTypeKind.Boolean:
        //                case EdmPrimitiveTypeKind.Byte:
        //                case EdmPrimitiveTypeKind.Date:
        //                case EdmPrimitiveTypeKind.Double:
        //                case EdmPrimitiveTypeKind.Guid:
        //                case EdmPrimitiveTypeKind.Int16:
        //                case EdmPrimitiveTypeKind.Int32:
        //                case EdmPrimitiveTypeKind.Int64:
        //                case EdmPrimitiveTypeKind.SByte:
        //                case EdmPrimitiveTypeKind.Single:
        //                case EdmPrimitiveTypeKind.Stream:
        //                case EdmPrimitiveTypeKind.PrimitiveType:
        //                    return;

        //                case EdmPrimitiveTypeKind.Binary:
        //                    IEdmBinaryTypeReference binaryType = edmTypeReference.AsBinary();
        //                    isBounded = binaryType.IsUnbounded;
        //                    maxLength = binaryType.MaxLength;
        //                    return;
        //                case EdmPrimitiveTypeKind.Decimal:
        //                    IEdmDecimalTypeReference decimalTypeReference = edmTypeReference.AsDecimal();
        //                    precision = decimalTypeReference.Precision;
        //                    scale = decimalTypeReference.Scale;
        //                    return;
        //                case EdmPrimitiveTypeKind.String:
        //                    IEdmStringTypeReference stringType = edmTypeReference.AsString();
        //                    isBounded = stringType.IsUnbounded;
        //                    maxLength = stringType.MaxLength;
        //                    unicode = stringType.IsUnicode;
        //                    return;
        //                case EdmPrimitiveTypeKind.Duration:
        //                case EdmPrimitiveTypeKind.DateTimeOffset:
        //                case EdmPrimitiveTypeKind.TimeOfDay:
        //                    IEdmTemporalTypeReference temporal = edmTypeReference.AsTemporal();
        //                    precision = temporal.Precision;
        //                    return;
        //                case EdmPrimitiveTypeKind.Geography:
        //                case EdmPrimitiveTypeKind.GeographyPoint:
        //                case EdmPrimitiveTypeKind.GeographyLineString:
        //                case EdmPrimitiveTypeKind.GeographyPolygon:
        //                case EdmPrimitiveTypeKind.GeographyCollection:
        //                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
        //                case EdmPrimitiveTypeKind.GeographyMultiLineString:
        //                case EdmPrimitiveTypeKind.GeographyMultiPoint:
        //                case EdmPrimitiveTypeKind.Geometry:
        //                case EdmPrimitiveTypeKind.GeometryPoint:
        //                case EdmPrimitiveTypeKind.GeometryLineString:
        //                case EdmPrimitiveTypeKind.GeometryPolygon:
        //                case EdmPrimitiveTypeKind.GeometryCollection:
        //                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
        //                case EdmPrimitiveTypeKind.GeometryMultiLineString:
        //                case EdmPrimitiveTypeKind.GeometryMultiPoint:
        //                    IEdmSpatialTypeReference spatialType = edmTypeReference.AsSpatial();
        //                    srid = spatialType.SpatialReferenceIdentifier;
        //                    return;
        //                case EdmPrimitiveTypeKind.None:
        //                    break;
        //            }
        //        }
        //    }
        //    else if (typeDefinition.TypeKind == EdmTypeKind.TypeDefinition)
        //    {
        //        IEdmTypeDefinitionReference reference = edmTypeReference as IEdmTypeDefinitionReference;
        //        if (reference == null)
        //        {
        //            // No facet available if not IEdmTypeDefinitionReference.
        //            return;
        //        }

        //        isBounded = reference.IsUnbounded;
        //        maxLength = reference.MaxLength;
        //        unicode = reference.IsUnicode;
        //        precision = reference.Precision;
        //        scale = reference.Scale;
        //        srid = reference.SpatialReferenceIdentifier;
        //    }

        //    return;
        //}

        /// <summary>
        /// Build an instance path with a Value path.
        /// An instance path is on object with a "$Path" member.
        /// </summary>
        /// <param name="objectValue"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        private static IEdmPathExpression BuildValuePathExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            IJsonValue jsonValue = objectValue["$Path"];
            string pathStr = jsonValue.ParseAsString(jsonPath);
            return new EdmPathExpression(pathStr);
        }

        private IEdmApplyExpression BuildApplyExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Apply expressions are represented as an object with a member $Apply whose value is an array of annotation expressions,
            // and a member $Function whose value is a string containing the qualified name of the client-side function to be applied.
            IJsonValue jsonValue = objectValue["$Apply"];

            IList<IEdmExpression> arguments = new List<IEdmExpression>();

            // TODO: build the arguments using the jsonvalue.
            JsonArrayValue arrayValue = jsonValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);
            arrayValue.ProcessItem(jsonPath, v =>
            {
                arguments.Add(BuildExpression(v, jsonPath));
            });

            string functionName = null;
            if (objectValue.TryGetValue("$Function", out jsonValue))
            {
                functionName = jsonValue.ParseAsString(jsonPath);
            }

            IEdmFunction edmFunction = FindAppliedFunction(functionName);

            return new EdmApplyExpression(edmFunction, arguments);
        }

        private static IEdmFunction FindAppliedFunction(string functionQualifiedName)
        {
            return null;
        }

        private IEdmCastExpression BuildCastExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Cast expressions are represented as an object with a member $Cast whose value is an annotation expression,
            // a member $Type whose value is a string containing the qualified type name, and optionally a member $Collection with a value of true.
            IJsonValue jsonValue = objectValue["$Cast"];

            IEdmExpression expression = BuildExpression(jsonValue, jsonPath);

            IEdmTypeReference edmTypeReference = null; // build the type
            return new EdmCastExpression(expression, edmTypeReference);
        }

        //private static IEdmTypeReference BuildTypeReference(JsonObjectValue keyValuePairs, IJsonPath jsonPath)
        //{
        //    string typeName = null;
        //    bool? collection = null;
        //    bool? nullable = null;
        //    int? maxLength = null;
        //    bool? unicode = null;
        //    int? precision = null;
        //    int? scale = null;
        //    int? srid = null;
        //    keyValuePairs.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
        //    {
        //        switch (propertyName)
        //        {
        //            case "$Type":
        //                typeName = propertyValue.ParseAsString(jsonPath);
        //                break;
        //            case "$Collection":
        //                collection = propertyValue.ParseAsBoolean(jsonPath);
        //                break;
        //            case "$Nullable":
        //                nullable = propertyValue.ParseAsBoolean(jsonPath);
        //                break;
        //            case "$MaxLength":
        //                maxLength = propertyValue.ParseAsIntegerPrimitive(jsonPath);
        //                break;
        //            case "$Unicode":
        //                unicode = propertyValue.ParseAsBoolean(jsonPath);
        //                break;
        //            case "$Precision":
        //                precision = propertyValue.ParseAsIntegerPrimitive(jsonPath);
        //                break;
        //            case "$Scale":
        //                scale = propertyValue.ParseAsIntegerPrimitive(jsonPath);
        //                break;
        //            case "SRID":
        //                srid = propertyValue.ParseAsIntegerPrimitive(jsonPath);
        //                break;
        //            default:
        //                break;
        //        }
        //    });

        //    // TODO: call EdmTypeJsonBuilder.ParseTypeReference(...)
        //    return null;
        //}

       

        private IEdmIfExpression BuildIfExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Is-of expressions are represented as an object with a member $IsOf whose value is an annotation expression,
            // a member $Type whose value is a string containing an qualified type name, and optionally a member $Collection with a value of true.
            IJsonValue jsonValue = objectValue["$If"];

            JsonArrayValue arrayValue = jsonValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);

            // first child is the test
            IEdmExpression testExpression = BuildExpression(arrayValue[0], jsonPath);

            // the second child is ture clause
            IEdmExpression trueExpression = BuildExpression(arrayValue[1], jsonPath);

            // the third child is false clause
            IEdmExpression falseExpression = null;
            if (arrayValue.Count >= 3)
            {
                // if and only if the if-then-else expression is an item of a collection expression,
                // the third child expression MAY be omitted, reducing it to an if-then expression. 
                falseExpression = BuildExpression(arrayValue[2], jsonPath);
            }

            return new EdmIfExpression(testExpression, trueExpression, falseExpression);
        }

        private IEdmIsTypeExpression BuildIsOfExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Is-of expressions are represented as an object with a member $IsOf whose value is an annotation expression,
            // a member $Type whose value is a string containing an qualified type name, and optionally a member $Collection with a value of true.
            IJsonValue jsonValue;
            IEdmExpression expression = null;
            if (objectValue.TryGetValue("$IsOf", out jsonValue))
            {
                expression = BuildExpression(jsonValue, jsonPath);
            }

            IEdmTypeReference edmTypeReference = null;
            // If the specified type is a primitive type or a collection of primitive types,
            // the facet members $MaxLength, $Unicode, $Precision, $Scale, and $SRID MAY be specified if applicable to the specified primitive type.
            // If the facet members are not specified, their values are considered unspecified.
            // edmTypeReference = BuildTypeReference(); // TODO: 

            return new EdmIsTypeExpression(expression, edmTypeReference);
        }

        private IEdmLabeledExpression BuildLabeledElementExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Labeled element expressions are represented as an object with a member $LabeledElement whose value is an annotation expression
            IJsonValue jsonValue;
            IEdmExpression expression = null;
            if (objectValue.TryGetValue("$LabeledElement", out jsonValue))
            {
                expression = BuildExpression(jsonValue, jsonPath);
            }

            // a member $Name whose value is a string containing the labeled element’s name.
            string name = null;
            if (objectValue.TryGetValue("$Name", out jsonValue))
            {
                name = jsonValue.ParseAsString(jsonPath);
            }

            // Verify Name if not null?
            // Verify expression is not null?
            return new EdmLabeledExpression(name, expression);
        }

        private static IEdmLabeledExpressionReferenceExpression BuildLabeledElementReferenceExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Labeled element reference expressions are represented as an object with a member $LabeledElementReference whose value is a string containing an qualified name.

            // Verify there's only one member???

            IJsonValue value = objectValue["$LabeledElementReference"];

            string strValue = value.ParseAsString(jsonPath);

            return new EdmLabeledExpressionReferenceExpression
            {
                Name = strValue
            };
        }

        private IEdmRecordExpression BuildRecordExpression(JsonObjectValue objectValue, IJsonPath jsonPath, IEdmStructuredTypeReference structuredType = null)
        {
            // A record expression MAY specify the structured type of its result, which MUST be an entity type or complex type in scope.
            // If not explicitly specified, the type is derived from the expression’s context

            // The type of a record expression is represented as the @type control information
            // for example: "@type": "https://example.org/vocabs/person#org.example.person.Manager",
            // So far, ODL doesn't support the type "@type" with relative path, only supports like "#Model.VipCustomer", or without #
            // for 4.0, this name MUST be prefixed with the hash symbol (#);
            // or non-OData 4.0 payloads, built-in primitive type values SHOULD be represented without the hash symbol,
            // but consumers of 4.01 or greater payloads MUST support values with or without the hash symbol.

            if (objectValue.ContainsKey("@type"))
            {
                // Try to build the type. The type should be "Complex type" or "Entity Type".
                // structuredTypeReference = GetOrFindTheType();
            }

            IList<IEdmPropertyConstructor> propertyValues = new List<IEdmPropertyConstructor>();

            objectValue.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                // It MAY contain annotations for itself and its members. Annotations for record members are prefixed with the member name.
                // So far, it's not supported. So skip it.
                // It also skips the @type, because it's processed above.
                if (propertyName.IndexOf('@') != -1)
                {
                    return;
                }

                IEdmExpression propertyValueExpression = BuildExpression(propertyValue, jsonPath);
                propertyValues.Add(new EdmPropertyConstructor(propertyName, propertyValueExpression));
            });

            return new EdmRecordExpression(propertyValues);
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
