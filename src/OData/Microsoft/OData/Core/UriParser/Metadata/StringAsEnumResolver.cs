//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Core.UriParser.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Implementation for resolving a literal value without qualified namespace to enum type.
    /// </summary>
    public sealed class StringAsEnumResolver : ODataUriResolver
    {
        /// <summary>
        /// Promote the left and right operand types, supports enum property and string constant scenario.
        /// </summary>
        /// <param name="binaryOperatorKind">the operator kind</param>
        /// <param name="leftNode">the left operand</param>
        /// <param name="rightNode">the right operand</param>
        /// <param name="typeReference">type reference for the result BinaryOperatorNode.</param>
        public override void PromoteBinaryOperandTypes(
               BinaryOperatorKind binaryOperatorKind,
               ref SingleValueNode leftNode,
               ref SingleValueNode rightNode,
               out IEdmTypeReference typeReference)
        {
            typeReference = null;

            if (leftNode.TypeReference != null && rightNode.TypeReference != null)
            {
                if ((leftNode.TypeReference.IsEnum()) && (rightNode.TypeReference.IsString()) && rightNode is ConstantNode)
                {
                    string text = ((ConstantNode)rightNode).Value as string;
                    ODataEnumValue val;
                    IEdmTypeReference typeRef = leftNode.TypeReference;

                    if (TryParseEnum(typeRef.Definition as IEdmEnumType, text, out val))
                    {
                        rightNode = new ConstantNode(val, text, typeRef);
                        return;
                    }
                }
                else if ((rightNode.TypeReference.IsEnum()) && (leftNode.TypeReference.IsString()) && leftNode is ConstantNode)
                {
                    string text = ((ConstantNode)leftNode).Value as string;
                    ODataEnumValue val;
                    IEdmTypeReference typeRef = rightNode.TypeReference;
                    if (TryParseEnum(typeRef.Definition as IEdmEnumType, text, out val))
                    {
                        leftNode = new ConstantNode(val, text, typeRef);
                        return;
                    }
                }
            }

            // fallback
            base.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
        }

        /// <summary>
        /// Resolve operation's parameters. Using this extension, enum value could be written as string value.
        /// </summary>
        /// <param name="operation">Current operation for parameters.</param>
        /// <param name="input">A dictionary the paramenter list.</param>
        /// <returns>A dictionary containing resolved parameters.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Parameter type is needed here to check whether can be convert from source")]
        public override IDictionary<IEdmOperationParameter, SingleValueNode> ResolveOperationParameters(IEdmOperation operation, IDictionary<string, SingleValueNode> input)
        {
            Dictionary<IEdmOperationParameter, SingleValueNode> result = new Dictionary<IEdmOperationParameter, SingleValueNode>(EqualityComparer<IEdmOperationParameter>.Default);
            foreach (var item in input)
            {
                IEdmOperationParameter functionParameter = null;
                if (EnableCaseInsensitive)
                {
                    functionParameter = ODataUriResolver.ResolveOpearationParameterNameCaseInsensitive(operation, item.Key);
                }
                else
                {
                    functionParameter = operation.FindParameter(item.Key);
                }

                // ensure parameter name existis
                if (functionParameter == null)
                {
                    throw new ODataException(Strings.ODataParameterWriterCore_ParameterNameNotFoundInOperation(item.Key, operation.Name));
                }

                SingleValueNode newVal = item.Value;

                if (functionParameter.Type.IsEnum() && (newVal.TypeReference.IsString()) && newVal is ConstantNode)
                {
                    string text = ((ConstantNode)item.Value).Value as string;
                    ODataEnumValue val;
                    IEdmTypeReference typeRef = functionParameter.Type;

                    if (TryParseEnum(typeRef.Definition as IEdmEnumType, text, out val))
                    {
                        newVal = new ConstantNode(val, text, typeRef);
                    }
                }

                result.Add(functionParameter, newVal);
            }

            return result;
        }

        /// <summary>
        /// Resolve keys for certain entity set, this function would be called when key is specified as positional values. E.g. EntitySet('key')
        /// Enum value could omit type name prefix using this resolver.
        /// </summary>
        /// <param name="type">Type for current entityset.</param>
        /// <param name="positionalValues">The list of positional values.</param>
        /// <param name="convertFunc">The convert function to be used for value converting.</param>
        /// <returns>The resolved key list.</returns>
        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type, IList<string> positionalValues, Func<IEdmTypeReference, string, object> convertFunc)
        {
            return base.ResolveKeys(
                type,
                positionalValues,
                (typeRef, valueText) =>
                {
                    if (typeRef.IsEnum() && valueText.StartsWith("'", StringComparison.Ordinal) && valueText.EndsWith("'", StringComparison.Ordinal))
                    {
                        valueText = typeRef.FullName() + valueText;
                    }

                    return convertFunc(typeRef, valueText);
                });
        }

        /// <summary>
        /// Resolve keys for certain entity set, this function would be called when key is specified as name value pairs. E.g. EntitySet(ID='key')
        /// Enum value could omit type name prefix using this resolver.
        /// </summary>
        /// <param name="type">Type for current entityset.</param>
        /// <param name="namedValues">The dictionary of name value pairs.</param>
        /// <param name="convertFunc">The convert function to be used for value converting.</param>
        /// <returns>The resolved key list.</returns>
        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type, IDictionary<string, string> namedValues, Func<IEdmTypeReference, string, object> convertFunc)
        {
            return base.ResolveKeys(
                type,
                namedValues,
                (typeRef, valueText) =>
                {
                    if (typeRef.IsEnum() && valueText.StartsWith("'", StringComparison.Ordinal) && valueText.EndsWith("'", StringComparison.Ordinal))
                    {
                        valueText = typeRef.FullName() + valueText;
                    }

                    return convertFunc(typeRef, valueText);
                });
        }

        /// <summary>
        /// Parse string or integer to enum value
        /// </summary>
        /// <param name="enumType">edm enum type</param>
        /// <param name="value">input string value</param>
        /// <param name="enumValue">output edm enum value</param>
        /// <returns>true if parse succeeds, false if fails</returns>
        private static bool TryParseEnum(IEdmEnumType enumType, string value, out ODataEnumValue enumValue)
        {
            long parsedValue;
            bool success = enumType.TryParseEnum(value, true, out parsedValue);
            enumValue = null;
            if (success)
            {
                enumValue = new ODataEnumValue(parsedValue.ToString(CultureInfo.InvariantCulture), enumType.FullTypeName());
            }

            return success;
        }
    }
}
