//---------------------------------------------------------------------
// <copyright file="AnnotationJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.OData.Edm.Csdl.Json.Value;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Json.Parser
{
    /// <summary>
    /// Provides functionalities for parsing CSDL-JSON annotations.
    /// </summary>
    internal class AnnotationJsonParser
    {
        /// <summary>
        /// Try to parse the input json value <see cref="IJsonValue"/> to <see cref="CsdlAnnotation"/>.
        /// </summary>
        /// <param name="annotionName">The input annotation name, annotation name should start with '@'.</param>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to read from.</param>
        /// <param name="jsonPath">The json node path.</param>
        /// <returns>null or the parsed <see cref="CsdlAnnotation"/>.</returns>
        public static CsdlAnnotation ParseCsdlAnnotation(string annotionName, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                throw new ArgumentNullException("jsonPath");
            }

            // An annotation is represented as a member whose name consists of an at (@) character,
            // followed by the qualified name of a term, optionally followed by a hash (#) and a qualifier.
            string qualifier;
            string termName;
            if (!ParseAnnotationName(annotionName, out termName, out qualifier))
            {
                return null;
            }

            CsdlLocation location = new CsdlLocation(jsonPath.ToString());

            jsonPath.Push(annotionName);
            CsdlExpressionBase expression = BuildExpression(jsonValue, jsonPath);
            jsonPath.Pop();

            return new CsdlAnnotation(termName, qualifier, expression, location);
        }

        /// <summary>
        /// Build the <see cref="CsdlExpressionBase"/> from the <see cref="IJsonValue"/>.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to read from.</param>
        /// <param name="jsonPath">The json node path.</param>
        /// <returns>null or the parsed <see cref="CsdlExpressionBase"/>.</returns>
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

                // We can't distiguish the string from other types now.
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
            string pathStr = jsonValue.ParseAsString(jsonPath);
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
                functionName = jsonValue.ParseAsString(jsonPath);
            }

            return new CsdlApplyExpression(functionName, arguments, new CsdlLocation(jsonPath.Path));
        }

        private static CsdlExpressionBase BuildCastExpression(JsonObjectValue objectValue, IJsonPath jsonPath)
        {
            // Cast expressions are represented as an object with a member $Cast whose value is an annotation expression,
            // a member $Type whose value is a string containing the qualified type name, and optionally a member $Collection with a value of true.
            IJsonValue jsonValue = objectValue["$Cast"];

            CsdlExpressionBase expression = BuildExpression(jsonValue, jsonPath);

            CsdlTypeReference typeReference = CsdlJsonParseHelper.ParseCsdlTypeReference(objectValue, jsonPath);

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

            // If the specified type is a primitive type or a collection of primitive types,
            // the facet members $MaxLength, $Unicode, $Precision, $Scale, and $SRID MAY be specified if applicable to the specified primitive type.
            // If the facet members are not specified, their values are considered unspecified.
            CsdlTypeReference typeReference = CsdlJsonParseHelper.ParseCsdlTypeReference(objectValue, jsonPath);

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
                label = jsonValue.ParseAsString(jsonPath);
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

            string label = value.ParseAsString(jsonPath);

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
                string typeName = objectValue.ParseOptionalProperty("@type", jsonPath, (v, p) => v.ParseAsString(p));
                int index = typeName.IndexOf('#');
                if (index >= 0)
                {
                    typeName = typeName.Substring(index + 1); // remove the "#"
                }

                typeReference = new CsdlNamedTypeReference(typeName, isNullable: true, new CsdlLocation(jsonPath.Path));
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

        /// <summary>
        /// Parse the input string to see whether it's a valid annotation name.
        /// If it's valid, seperate string into term name or optional qualifier name.
        /// It it's not valid, return false
        /// </summary>
        /// <param name="annotationName">The input annotation name.</param>
        /// <param name="term"></param>
        /// <param name="qualifier"></param>
        /// <returns></returns>
        internal static bool ParseAnnotationName(string annotationName, out string term, out string qualifier)
        {
            term = null;
            qualifier = null;
            if (string.IsNullOrWhiteSpace(annotationName))
            {
                return false;
            }

            if (annotationName[0] != '@')
            {
                return false;
            }

            // BE caution:
            // An annotation can itself be annotated. Annotations on annotations are represented as a member
            // whose name consists of the annotation name (including the optional qualifier),
            // followed by an at (@) character, followed by the qualified name of a term, optionally followed by a hash (#) and a qualifier.
            // for example: 
            // "@Measures.ISOCurrency": "USD",
            // "@Measures.ISOCurrency@Core.Description": "The parent company’s currency"
            //
            // So, Core.Description is annotation for "Measures.ISOCurrency annotation.

            // followed by an at (@) character, followed by the qualified name of a term, optionally followed by a hash (#) and a qualifier.
            int index = annotationName.IndexOf('#');
            if (index != -1)
            {
                term = annotationName.Substring(1, index); // remove '@'
                qualifier = annotationName.Substring(index + 1);
            }
            else
            {
                term = annotationName.Substring(1); // remove '@'
            }

            // So, in annotation on annotation, the return "term" will contains the two term names.
            // for example:  "@Measures.ISOCurrency@Core.Description#Qualifier"
            // term includes "Measures.ISOCurrency@Core.Description"
            // Once ODL supports annotation on annotation, we can process this term to support it.
            return true;
        }
    }
}
