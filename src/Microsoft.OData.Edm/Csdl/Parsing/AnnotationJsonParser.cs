//---------------------------------------------------------------------
// <copyright file="AnnotationJsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// Provides functionalities for parsing CSDL-JSON annotations.
    /// </summary>
    internal static class AnnotationJsonParser
    {
        /// <summary>
        /// Try to parse the input json value <see cref="JsonElement"/> to <see cref="CsdlAnnotation"/>.
        /// </summary>
        /// <param name="annotionName">The input annotation name, annotation name should start with '@'.</param>
        /// <param name="element">The JSON value to parse.</param>
        /// <param name="context">The parser context.</param>
        /// <param name="csdlAnnotation">The built CSDL annotation.</param>
        /// <returns>true/false.</returns>
        public static bool TryParseCsdlAnnotation(string annotionName, JsonElement element, JsonParserContext context, out CsdlAnnotation csdlAnnotation)
        {
            EdmUtil.CheckArgumentNull(context, nameof(context));

            csdlAnnotation = null;

            // An annotation is represented as a member whose name consists of an at (@) character,
            // followed by the qualified name of a term, optionally followed by a hash (#) and a qualifier.
            if (!ParseAnnotationName(annotionName, out string termName, out string qualifier))
            {
                return false;
            }

            context.EnterScope(annotionName);

            CsdlExpressionBase expression = ParseExpression(element, context);

            context.LeaveScope(annotionName);

            csdlAnnotation = new CsdlAnnotation(termName, qualifier, expression, context.Location());
            return true;
        }

        /// <summary>
        /// Parse the <see cref="CsdlExpressionBase"/> from the <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="element">JSON value to parse.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>null or the parsed <see cref="CsdlExpressionBase"/>.</returns>
        public static CsdlExpressionBase ParseExpression(JsonElement element, JsonParserContext context)
        {
            EdmUtil.CheckArgumentNull(context, nameof(context));

            CsdlLocation location = context.Location();

            switch (element.ValueKind)
            {
                case JsonValueKind.True:
                    return new CsdlConstantExpression(EdmValueKind.Boolean, "true", location);

                case JsonValueKind.False:
                    return new CsdlConstantExpression(EdmValueKind.Boolean, "false", location);

                case JsonValueKind.String:
                    // we can't distiguish "Guid, DateTimeOffset, ..." from String at here.
                    // So, let's create string for all of string values.
                    return new CsdlConstantExpression(EdmValueKind.String, element.GetString(), location);

                case JsonValueKind.Null:
                    return new CsdlConstantExpression(EdmValueKind.Null, null, location);

                case JsonValueKind.Number:
                    return ParseNumberExpression(element, context);

                case JsonValueKind.Object:
                    return ParseObjectExpression(element, context);

                case JsonValueKind.Array:
                    IList<CsdlExpressionBase> elements = element.ParseAsArray(context, (e, c) => ParseExpression(e, c));
                    return new CsdlCollectionExpression(null, elements, location);

                case JsonValueKind.Undefined:
                default:
                    context.ReportError(EdmErrorCode.UnknownElementValueKind, Strings.CsdlJsonParser_UnknownJsonElementValueKind(element.ValueKind, context.Path));
                    return null;
            }
        }

        /// <summary>
        /// Parse the JSON number to <see cref="CsdlExpressionBase"/>.
        /// </summary>
        /// <param name="element">The JSON value to parse.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>null or the parsed <see cref="CsdlExpressionBase"/>.</returns>
        private static CsdlExpressionBase ParseNumberExpression(JsonElement element, JsonParserContext context)
        {
            Debug.Assert(element.ValueKind == JsonValueKind.Number);
            Debug.Assert(context != null);

            CsdlLocation location = context.Location();

            // Int64 can handle all these integer types
            if (element.TryGetInt64(out long longValue))
            {
                return new CsdlConstantExpression(EdmValueKind.Integer, longValue.ToString(CultureInfo.InvariantCulture), location);
            }

            // Decimal goes ahead double
            if (element.TryGetDecimal(out decimal decimalValue))
            {
                return new CsdlConstantExpression(EdmValueKind.Decimal, decimalValue.ToString(CultureInfo.InvariantCulture), location);
            }

            // Double
            if (element.TryGetDouble(out double doubleValue))
            {
                return new CsdlConstantExpression(EdmValueKind.Floating, doubleValue.ToString(CultureInfo.InvariantCulture), location);
            }

            // Any others?
            // Report error for unknown number
            context.ReportError(EdmErrorCode.InvalidNumberType, Strings.CsdlJsonParser_InvalidJsonNumberType(element, context.Path));
            return null;
        }

        /// <summary>
        /// Parse the JSON object to <see cref="CsdlExpressionBase"/>.
        /// We don't know the object type, so try it one by one using the keyword, for example $Cast for Cast expression.
        /// </summary>
        /// <param name="element">The JSON value to parse.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>null or the parsed <see cref="CsdlExpressionBase"/>.</returns>
        private static CsdlExpressionBase ParseObjectExpression(JsonElement element, JsonParserContext context)
        {
            Debug.Assert(element.ValueKind == JsonValueKind.Object);
            Debug.Assert(context != null);

            // $Path
            if (TryParseValuePathExpression(element, context, out CsdlPathExpression pathExp))
            {
                return pathExp;
            }

            // $Cast
            if (TryParseCastExpression(element, context, out CsdlCastExpression castExp))
            {
                return castExp;
            }

            // $Apply
            if (TryParseApplyExpression(element, context, out CsdlApplyExpression applyExp))
            {
                return applyExp;
            }

            // $If
            if (TryParseIfExpression(element, context, out CsdlIfExpression ifExp))
            {
                return ifExp;
            }

            // $IsOf
            if (BuildIsOfExpression(element, context, out CsdlIsTypeExpression isOfExp))
            {
                return isOfExp;
            }

            // $LabeledElement
            if (TryParseLabeledElementExpression(element, context, out CsdlLabeledExpression labeledExp))
            {
                return labeledExp;
            }

            // $LabeledElementReference
            if (TryParseLabeledElementReferenceExpression(element, context, out CsdlLabeledExpressionReferenceExpression labeledReferenceExp))
            {
                return labeledReferenceExp;
            }

            // For all others, let's try build it as "Record"
            return ParseRecordExpression(element, context);
        }

        /// <summary>
        /// Parse an instance path with a Value path.
        /// An instance path is on object with a "$Path" member.
        /// "@UI.DisplayName#second": {
        ///   "$Path": "@vCard.Address#work/FullName"
        /// }
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <param name="pathExp">The built path expression.</param>
        /// <returns>true/false.</returns>
        private static bool TryParseValuePathExpression(JsonElement element, JsonParserContext context, out CsdlPathExpression pathExp)
        {
            Debug.Assert(context != null);

            pathExp = null;
            // Path expressions are represented as an object with a single member $Path
            JsonElement propertyValue;
            if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty("$Path", out propertyValue))
            {
                return false;
            }

            // whose value is a string containing a path.
            string pathStr = propertyValue.ProcessProperty("$Path", context, (v, p) => v.ParseAsString(p));

            pathExp = new CsdlPathExpression(pathStr, context.Location());
            return true;
        }

        /// <summary>
        /// Parse the Cast expression using <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <param name="castExp">The built cast expression.</param>
        /// <returns>true/false.</returns>
        private static bool TryParseCastExpression(JsonElement element, JsonParserContext context, out CsdlCastExpression castExp)
        {
            Debug.Assert(context != null);

            castExp = null;
            // Cast expressions are represented as an object with a member $Cast
            JsonElement propertyValue;
            if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty("$Cast", out propertyValue))
            {
                return false;
            }

            // with a member $Cast whose value is an annotation expression
            CsdlExpressionBase expression = propertyValue.ProcessProperty("$Cast", context, ParseExpression);

            // a member $Type whose value is a string containing the qualified type name, and optionally a member $Collection with a value of true.
            CsdlTypeReference typeReference = CsdlJsonParseHelper.ParseCsdlTypeReference(element, context);

            castExp = new CsdlCastExpression(typeReference, expression, context.Location());
            return true;
        }

        /// <summary>
        /// Parse the Apply expression using <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <param name="applyExp">The built apply expression.</param>
        /// <returns>true/false.</returns>
        private static bool TryParseApplyExpression(JsonElement element, JsonParserContext context, out CsdlApplyExpression applyExp)
        {
            Debug.Assert(context != null);
            applyExp = null;

            // Apply expressions are represented as an object with a member $Apply whose value is an array
            JsonElement propertyValue;
            if (element.ValueKind != JsonValueKind.Object ||
                !element.TryGetProperty("$Apply", out propertyValue) ||
                propertyValue.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            // whose value is an array of annotation expressions,
            IList<CsdlExpressionBase> arguments = propertyValue.ProcessArrayProperty("$Apply", context, ParseExpression);

            // a member $Function whose value is a string containing the qualified name of the client-side function to be applied.
            string functionName = element.ProcessRequiredProperty("$Function", context, (e, c) => e.ParseAsString(c));

            applyExp = new CsdlApplyExpression(functionName, arguments, context.Location());
            return true;
        }

        /// <summary>
        /// Parse the If expression using <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <param name="ifExp">The built if expression.</param>
        /// <returns>true/false</returns>
        private static bool TryParseIfExpression(JsonElement element, JsonParserContext context, out CsdlIfExpression ifExp)
        {
            Debug.Assert(context != null);
            ifExp = null;

            // Conditional expressions are represented as an object with a member $If whose value is an array of two or three annotation expressions.
            JsonElement propertyValue;
            if (element.ValueKind != JsonValueKind.Object ||
                !element.TryGetProperty("$If", out propertyValue) ||
                propertyValue.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            // first child is the test
            CsdlExpressionBase testExpression = propertyValue.ProcessItem(0, context, ParseExpression);

            // the second child is ture clause
            CsdlExpressionBase trueExpression = propertyValue.ProcessItem(1, context, ParseExpression);

            // the third child is false clause
            CsdlExpressionBase falseExpression = null;
            if (propertyValue.GetArrayLength() >= 3)
            {
                // if and only if the if-then-else expression is an item of a collection expression,
                // the third child expression MAY be omitted, reducing it to an if-then expression. 
                falseExpression = propertyValue.ProcessItem(2, context, ParseExpression);
            }

            ifExp = new CsdlIfExpression(testExpression, trueExpression, falseExpression, context.Location());
            return true;
        }

        /// <summary>
        /// Parse the IsOf expression using <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <param name="isOfExp">The built IsOf expression.</param>
        /// <returns>true/false</returns>
        private static bool BuildIsOfExpression(JsonElement element, JsonParserContext context, out CsdlIsTypeExpression isOfExp)
        {
            Debug.Assert(context != null);
            isOfExp = null;

            // Is-of expressions are represented as an object with a member $IsOf whose value is an annotation expression,
            JsonElement propertyValue;
            if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty("$IsOf", out propertyValue))
            {
                return false;
            }

            // whose value is  an annotation expression,
            CsdlExpressionBase expression = propertyValue.ProcessProperty("$IsOf", context, ParseExpression);

            // a member $Type whose value is a string containing an qualified type name, and optionally a member $Collection with a value of true.

            // If the specified type is a primitive type or a collection of primitive types,
            // the facet members $MaxLength, $Unicode, $Precision, $Scale, and $SRID MAY be specified if applicable to the specified primitive type.
            // If the facet members are not specified, their values are considered unspecified.
            CsdlTypeReference typeReference = CsdlJsonParseHelper.ParseCsdlTypeReference(element, context);

            isOfExp = new CsdlIsTypeExpression(typeReference, expression, context.Location());
            return true;
        }

        /// <summary>
        /// Parse the Labeled Element expression using <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <param name="labeledExp">The built labeled element expression.</param>
        /// <returns>true/false</returns>
        private static bool TryParseLabeledElementExpression(JsonElement element, JsonParserContext context, out CsdlLabeledExpression labeledExp)
        {
            Debug.Assert(context != null);
            labeledExp = null;

            // Labeled element expressions are represented as an object with a member $LabeledElement whose value is an annotation expression
            JsonElement propertyValue;
            if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty("$LabeledElement", out propertyValue))
            {
                return false;
            }

            CsdlExpressionBase expression = propertyValue.ProcessProperty("$LabeledElement", context, ParseExpression);

            // a member $Name whose value is a string containing the labeled element’s name.
            string label = element.ProcessRequiredProperty("$Name", context, (v, p) => v.ParseAsString(p));

            labeledExp = new CsdlLabeledExpression(label, expression, context.Location());
            return true;
        }

        /// <summary>
        /// Parse the Labeled Element Reference expression using <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <param name="labeledReferenceExp">The built Labeled Element Reference expression.</param>
        /// <returns>true/false</returns>
        private static bool TryParseLabeledElementReferenceExpression(JsonElement element, JsonParserContext context, out CsdlLabeledExpressionReferenceExpression labeledReferenceExp)
        {
            Debug.Assert(context != null);
            labeledReferenceExp = null;

            // Labeled element reference expressions are represented as an object with a member $LabeledElementReference whose value is a string containing an qualified name.
            JsonElement propertyValue;
            if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty("$LabeledElementReference", out propertyValue))
            {
                return false;
            }

            string label = propertyValue.ProcessProperty("$LabeledElementReference", context, (e, c) => e.ParseAsString(c));

            labeledReferenceExp = new CsdlLabeledExpressionReferenceExpression(label, context.Location());
            return true;
        }

        /// <summary>
        /// Parse the record expression expression using <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="element">The input JSON element.</param>
        /// <param name="context">The parser context.</param>
        /// <returns>the built record expression.</returns>
        private static CsdlRecordExpression ParseRecordExpression(JsonElement element, JsonParserContext context)
        {
            Debug.Assert(context != null);

            // A record expression MAY specify the structured type of its result, which MUST be an entity type or complex type in scope.
            // If not explicitly specified, the type is derived from the expression’s context

            // The type of a record expression is represented as the @type control information
            // for example: "@type": "https://example.org/vocabs/person#org.example.person.Manager",
            // So far, ODL doesn't support the type "@type" with relative path, only supports like "#Model.VipCustomer", or without #
            // for 4.0, this name MUST be prefixed with the hash symbol (#);
            // or non-OData 4.0 payloads, built-in primitive type values SHOULD be represented without the hash symbol,
            // but consumers of 4.01 or greater payloads MUST support values with or without the hash symbol.

            CsdlTypeReference typeReference = null;
            if (element.TryGetProperty("@type", out JsonElement typeValue))
            {
                // Try to build the type. The type should be "Complex type" or "Entity Type".
                string typeName = typeValue.ProcessProperty("@type", context, (e, c) => e.ParseAsString(c));
                int index = typeName.IndexOf('#');
                if (index >= 0)
                {
                    typeName = typeName.Substring(index + 1); // remove the "#"
                }

                typeReference = new CsdlNamedTypeReference(typeName, true, context.Location());
            }

            IList<CsdlPropertyValue> propertyValues = new List<CsdlPropertyValue>();
            element.ParseAsObject(context, (propertyName, propertyValue) =>
            {
                // skips the @type, because it's processed above.
                if (propertyName == "@type")
                {
                    return;
                }

                // It MAY contain annotations for itself and its members. Annotations for record members are prefixed with the member name.
                // So far, it's not supported. So report non-fatal error for all the annotations on record.
                if (propertyName.IndexOf('@') != -1)
                {
                    context.ReportError(EdmErrorCode.UnsupportedElement, Strings.CsdlJsonParser_UnsupportedJsonMember(context.Path));
                    return;
                }

                CsdlExpressionBase propertyValueExpression = ParseExpression(propertyValue, context);
                propertyValues.Add(new CsdlPropertyValue(propertyName, propertyValueExpression, context.Location()));
            });

            return new CsdlRecordExpression(typeReference, propertyValues, context.Location());
        }

        /// <summary>
        /// Parse the input string to see whether it's a valid annotation name.
        /// If it's valid, seperate string into term name or optional qualifier name.
        /// It it's not valid, return false
        /// </summary>
        /// <param name="annotationName">The input annotation name.</param>
        /// <param name="term">The output term string.</param>
        /// <param name="qualifier">The output qualified string.</param>
        /// <returns>true/false.</returns>
        private static bool ParseAnnotationName(string annotationName, out string term, out string qualifier)
        {
            term = null;
            qualifier = null;
            if (string.IsNullOrWhiteSpace(annotationName) || annotationName[0] != '@')
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

            int index = annotationName.IndexOf('#');
            if (index != -1)
            {
                term = annotationName.Substring(1, index - 1); // remove '@'
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
#endif
