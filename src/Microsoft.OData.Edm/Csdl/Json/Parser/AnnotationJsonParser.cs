//---------------------------------------------------------------------
// <copyright file="AnnotationJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Csdl.Json.Value;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Json.Parser
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
        public static IList<CsdlAnnotations> ParseOutOfLineAnnotations(IJsonValue jsonValue, IJsonPath jsonPath)
        {
            JsonObjectValue annotationsObj = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);

            IList<CsdlAnnotations> annotationsCollection = new List<CsdlAnnotations>();
            annotationsObj.ProcessProperty(jsonPath, (n, v) =>
            {
                // The member name is a path identifying the annotation target
                string target = n;

                string qualifier = null; // It's form the "target" name, or it's not set again in JSON?

                // the member value is an object containing annotations for that target.
                if (v.ValueKind == JsonValueKind.JObject)
                {
                    IList<CsdlAnnotation> subAnnotations = new List<CsdlAnnotation>();
                    JsonObjectValue subOject = (JsonObjectValue)v;
                    subOject.ProcessProperty(jsonPath, (subName, subValue) =>
                    {
                        string outPropertyName;
                        CsdlAnnotation subAnnotation = TryParseCsdlAnnotation(subName, subValue, jsonPath, null, out outPropertyName);
                        subAnnotations.Add(subAnnotation);
                    });

                    CsdlAnnotations annotations = new CsdlAnnotations(subAnnotations, target, qualifier);
                    annotationsCollection.Add(annotations);
                }
                else
                {
                    // Reporting for unknown?
                }
            });

            return annotationsCollection;
        }

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
        public static CsdlAnnotation TryParseCsdlAnnotation(string annotionName, IJsonValue jsonValue, IJsonPath jsonPath, CsdlSerializerOptions options, out string elementName)
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

        public static CsdlExpressionBase BuildExpression(IJsonValue jsonValue, IJsonPath jsonPath)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                throw new ArgumentNullException("jsonPath");
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

        public static CsdlExpressionBase BuildConstantExpression(JsonPrimitiveValue primitiveValue, IJsonPath jsonPath)
        {
            CsdlLocation location = new CsdlLocation(jsonPath.Path);
            // From JsonReader, the primitiveValue (not nullable) could be:
            // null
            // String,
            // boolean
            // int
            // decimal
            // double
            if (primitiveValue.Value == null)
            {
                return new CsdlConstantExpression(EdmValueKind.Null, null, location);
            }

            object value = primitiveValue.Value;
            Type valueType = value.GetType();

            // Boolean
            if (valueType == typeof(bool))
            {
                bool boolValue = (bool)value;
                return new CsdlConstantExpression(EdmValueKind.Boolean, boolValue ? "true" : "false", location);
            }

            // Integer
            if (valueType == typeof(int) || valueType == typeof(long))
            {
                long intValue = (long)value;
                return new CsdlConstantExpression(EdmValueKind.Integer, intValue.ToString(CultureInfo.InvariantCulture), location);
            }

            // decimal
            if (valueType == typeof(decimal))
            {
                decimal decimalValue = (decimal)value;
                return new CsdlConstantExpression(EdmValueKind.Decimal, decimalValue.ToString(CultureInfo.InvariantCulture), location);
            }

            // double
            if (valueType == typeof(float) || valueType == typeof(double))
            {
                double doubleValue = (double)value;
                return new CsdlConstantExpression(EdmValueKind.Floating, doubleValue.ToString(CultureInfo.InvariantCulture), location);
            }

            // string
            if (valueType == typeof(string))
            {
                string stringValue = (string)value;

                // We can't distiguish the string from other types now..
                // So for others, let's return as string constant.
                return new CsdlConstantExpression(EdmValueKind.String, stringValue, location);
            }

            // We should never been here?
            throw new Exception();
        }

        public static CsdlExpressionBase BuildObjectExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
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

        public static CsdlExpressionBase BuildCollectionExpression(JsonArrayValue arrayValue, IJsonPath jsonPath)
        {
            IList<CsdlExpressionBase> elements = new List<CsdlExpressionBase>();
            arrayValue.ProcessItem(jsonPath, (v) =>
            {
                CsdlExpressionBase element = BuildExpression(v, jsonPath);

                elements.Add(element);
            });

            return new CsdlCollectionExpression(null, elements, new CsdlLocation(jsonPath.ToString()));
        }

        /// <summary>
        /// Build an instance path with a Value path.
        /// An instance path is on object with a "$Path" member.
        /// </summary>
        /// <param name="objectValue"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        private static CsdlPathExpression BuildValuePathExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            IJsonValue jsonValue = objectValue["$Path"];
            string pathStr = jsonValue.ParseAsStringPrimitive(jsonPath);
            return new CsdlPathExpression(pathStr, new CsdlLocation(jsonPath.Path));
        }

        private static CsdlApplyExpression BuildApplyExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Apply expressions are represented as an object with a member $Apply whose value is an array of annotation expressions,
            // and a member $Function whose value is a string containing the qualified name of the client-side function to be applied.
            IJsonValue jsonValue = objectValue["$Apply"];

            IList<CsdlExpressionBase> arguments = new List<CsdlExpressionBase>();

            // TODO: build the arguments using the jsonvalue.
            JsonArrayValue arrayValue = jsonValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);
            arrayValue.ProcessItem(jsonPath, v =>
            {
                arguments.Add(BuildExpression(v, jsonPath));
            });

            string functionName = null;
            if (objectValue.TryGetValue("$Function", out jsonValue))
            {
                functionName = jsonValue.ParseAsStringPrimitive(jsonPath);
            }

            return new CsdlApplyExpression(functionName, arguments, new CsdlLocation(jsonPath.Path));
        }

        private static CsdlExpressionBase BuildCastExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Cast expressions are represented as an object with a member $Cast whose value is an annotation expression,
            // a member $Type whose value is a string containing the qualified type name, and optionally a member $Collection with a value of true.
            IJsonValue jsonValue = objectValue["$Cast"];

            CsdlExpressionBase expression = BuildExpression(jsonValue, jsonPath);

            CsdlTypeReference typeReference = null; // build the type??????????

            return new CsdlCastExpression(typeReference, expression, new CsdlLocation(jsonPath.Path));
        }

        private static CsdlIfExpression BuildIfExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Is-of expressions are represented as an object with a member $IsOf whose value is an annotation expression,
            // a member $Type whose value is a string containing an qualified type name, and optionally a member $Collection with a value of true.
            IJsonValue jsonValue = objectValue["$If"];

            JsonArrayValue arrayValue = jsonValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);

            // first child is the test
            CsdlExpressionBase testExpression = BuildExpression(arrayValue[0], jsonPath);

            // the second child is ture clause
            CsdlExpressionBase trueExpression = BuildExpression(arrayValue[1], jsonPath);

            // the third child is false clause
            CsdlExpressionBase falseExpression = null;
            if (arrayValue.Count >= 3)
            {
                // if and only if the if-then-else expression is an item of a collection expression,
                // the third child expression MAY be omitted, reducing it to an if-then expression. 
                falseExpression = BuildExpression(arrayValue[2], jsonPath);
            }

            return new CsdlIfExpression(testExpression, trueExpression, falseExpression, new CsdlLocation(jsonPath.Path));
        }

        private static CsdlIsTypeExpression BuildIsOfExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Is-of expressions are represented as an object with a member $IsOf whose value is an annotation expression,
            // a member $Type whose value is a string containing an qualified type name, and optionally a member $Collection with a value of true.
            IJsonValue jsonValue;
            CsdlExpressionBase expression = null;
            if (objectValue.TryGetValue("$IsOf", out jsonValue))
            {
                expression = BuildExpression(jsonValue, jsonPath);
            }

            CsdlTypeReference typeReference = null; // build the type??????????
            // If the specified type is a primitive type or a collection of primitive types,
            // the facet members $MaxLength, $Unicode, $Precision, $Scale, and $SRID MAY be specified if applicable to the specified primitive type.
            // If the facet members are not specified, their values are considered unspecified.
            // edmTypeReference = BuildTypeReference(); // TODO: 

            return new CsdlIsTypeExpression(typeReference, expression, new CsdlLocation(jsonPath.Path));
        }

        private static CsdlLabeledExpression BuildLabeledElementExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Labeled element expressions are represented as an object with a member $LabeledElement whose value is an annotation expression
            IJsonValue jsonValue;
            CsdlExpressionBase expression = null;
            if (objectValue.TryGetValue("$LabeledElement", out jsonValue))
            {
                expression = BuildExpression(jsonValue, jsonPath);
            }

            // a member $Name whose value is a string containing the labeled element’s name.
            string label = null;
            if (objectValue.TryGetValue("$Name", out jsonValue))
            {
                label = jsonValue.ParseAsStringPrimitive(jsonPath);
            }

            // Verify Name if not null?
            // Verify expression is not null?
            return new CsdlLabeledExpression(label, expression, new CsdlLocation(jsonPath.Path));
        }

        private static CsdlLabeledExpressionReferenceExpression BuildLabeledElementReferenceExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Labeled element reference expressions are represented as an object with a member $LabeledElementReference whose value is a string containing an qualified name.

            // Verify there's only one member???

            IJsonValue value = objectValue["$LabeledElementReference"];

            string label = value.ParseAsStringPrimitive(jsonPath);

            return new CsdlLabeledExpressionReferenceExpression(label, new CsdlLocation(jsonPath.Path));
        }

        private static CsdlRecordExpression BuildRecordExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // A record expression MAY specify the structured type of its result, which MUST be an entity type or complex type in scope.
            // If not explicitly specified, the type is derived from the expression’s context

            // The type of a record expression is represented as the @type control information
            // for example: "@type": "https://example.org/vocabs/person#org.example.person.Manager",
            // So far, ODL doesn't support the type "@type" with relative path, only supports like "#Model.VipCustomer", or without #
            // for 4.0, this name MUST be prefixed with the hash symbol (#);
            // or non-OData 4.0 payloads, built-in primitive type values SHOULD be represented without the hash symbol,
            // but consumers of 4.01 or greater payloads MUST support values with or without the hash symbol.

            CsdlTypeReference typeReference = null;
            if (objectValue.ContainsKey("@type"))
            {
                // Try to build the type. The type should be "Complex type" or "Entity Type".
                // structuredTypeReference = GetOrFindTheType();
            }

            IList<CsdlPropertyValue> propertyValues = new List<CsdlPropertyValue>();

            objectValue.ProcessProperty(jsonPath, (propertyName, propertyValue) =>
            {
                // It MAY contain annotations for itself and its members. Annotations for record members are prefixed with the member name.
                // So far, it's not supported. So skip it.
                // It also skips the @type, because it's processed above.
                if (propertyName.IndexOf('@') != -1)
                {
                    return;
                }

                CsdlExpressionBase propertyValueExpression = BuildExpression(propertyValue, jsonPath);
                propertyValues.Add(new CsdlPropertyValue(propertyName, propertyValueExpression, new CsdlLocation(jsonPath.Path)));
            });

            return new CsdlRecordExpression(typeReference, propertyValues, new CsdlLocation(jsonPath.Path));
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

            return CsdlJsonSchemaParser.ParseTypeReference(elementType.FullName(), isCollection,
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
