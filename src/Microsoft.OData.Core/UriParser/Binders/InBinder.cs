//---------------------------------------------------------------------
// <copyright file="InBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Class that knows how to bind the In operator.
    /// </summary>
    internal sealed class InBinder
    {
        private const string NullLiteral = "null";

        /// <summary>
        /// Method to use for binding the parent node, if needed.
        /// </summary>
        private readonly Func<QueryToken, QueryNode> bindMethod;

        /// <summary>
        /// Constructs a InBinder with the given method to be used binding the parent token if needed.
        /// </summary>
        /// <param name="bindMethod">Method to use for binding the parent token, if needed.</param>
        internal InBinder(Func<QueryToken, QueryNode> bindMethod)
        {
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Binds an In operator token.
        /// </summary>
        /// <param name="inToken">The In operator token to bind.</param>
        /// <param name="state">State of the metadata binding.</param>
        /// <returns>The bound In operator token.</returns>
        internal QueryNode BindInOperator(InToken inToken, BindingState state)
        {
            ExceptionUtils.CheckArgumentNotNull(inToken, "inToken");

            SingleValueNode left = this.GetSingleValueOperandFromToken(inToken.Left);
            CollectionNode right = null;
            if (left.TypeReference != null)
            {
                right = this.GetCollectionOperandFromToken(
                    inToken.Right, new EdmCollectionTypeReference(new EdmCollectionType(left.TypeReference)), state.Model);
            }
            else 
            {
                right = this.GetCollectionOperandFromToken(
                    inToken.Right, new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetUntyped())), state.Model);
            }

            return new InNode(left, right);
        }

        /// <summary>
        /// Retrieve SingleValueNode bound with given query token.
        /// </summary>
        /// <param name="queryToken">The query token</param>
        /// <returns>The corresponding SingleValueNode</returns>
        private SingleValueNode GetSingleValueOperandFromToken(QueryToken queryToken)
        {
            SingleValueNode operand = this.bindMethod(queryToken) as SingleValueNode;
            if (operand == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_LeftOperandNotSingleValue);
            }

            return operand;
        }

        /// <summary>
        /// Retrieve CollectionNode bound with given query token.
        /// </summary>
        /// <param name="queryToken">The query token</param>
        /// <param name="expectedType">The expected type that this collection holds</param>
        /// <param name="model">The Edm model</param>
        /// <returns>The corresponding CollectionNode</returns>
        private CollectionNode GetCollectionOperandFromToken(QueryToken queryToken, IEdmTypeReference expectedType, IEdmModel model)
        {
            CollectionNode operand = null;
            LiteralToken literalToken = queryToken as LiteralToken;
            if (literalToken != null)
            {
                string originalLiteralText = literalToken.OriginalText;

                // Parentheses-based collections are not standard JSON but bracket-based ones are.
                // Temporarily switch our collection to bracket-based so that the JSON reader will
                // correctly parse the collection. Then pass the original literal text to the token.
                string bracketLiteralText = originalLiteralText;
                if (bracketLiteralText[0] == '(')
                {
                    Debug.Assert(bracketLiteralText[bracketLiteralText.Length - 1] == ')',
                        "Collection with opening '(' should have corresponding ')'");

                    StringBuilder replacedText = new StringBuilder(bracketLiteralText);
                    replacedText[0] = '[';
                    replacedText[replacedText.Length - 1] = ']';
                    bracketLiteralText = replacedText.ToString();

                    Debug.Assert(expectedType.IsCollection());
                    string expectedTypeFullName = expectedType.Definition.AsElementType().FullTypeName();
                    if (expectedTypeFullName.Equals("Edm.String", StringComparison.Ordinal))
                    {
                        // For collection of strings, need to convert single-quoted string to double-quoted string,
                        // and also, per ABNF, a single quote within a string literal is "encoded" as two consecutive single quotes in either
                        // literal or percent - encoded representation.
                        // Sample: ['a''bc','''def','xyz'''] ==> ["a'bc","'def","xyz'"], which is legitimate Json format.
                        bracketLiteralText = NormalizeStringCollectionItems(bracketLiteralText);
                    }
                    else if (expectedTypeFullName.Equals("Edm.Guid", StringComparison.Ordinal))
                    {
                        // For collection of Guids, need to convert the Guid literals to single-quoted form, so that it is compatible
                        // with the Json reader used for deserialization.
                        // Sample: [D01663CF-EB21-4A0E-88E0-361C10ACE7FD, 492CF54A-84C9-490C-A7A4-B5010FAD8104]
                        //    ==>  ['D01663CF-EB21-4A0E-88E0-361C10ACE7FD', '492CF54A-84C9-490C-A7A4-B5010FAD8104']
                        bracketLiteralText = NormalizeGuidCollectionItems(bracketLiteralText);
                    }
                    else if (expectedTypeFullName.Equals("Edm.DateTimeOffset", StringComparison.Ordinal) ||
                             expectedTypeFullName.Equals("Edm.Date", StringComparison.Ordinal) ||
                             expectedTypeFullName.Equals("Edm.TimeOfDay", StringComparison.Ordinal) ||
                             expectedTypeFullName.Equals("Edm.Duration", StringComparison.Ordinal))
                    {
                        // For collection of Date/Time/Duration items, need to convert the Date/Time/Duration literals to single-quoted form, so that it is compatible
                        // with the Json reader used for deserialization.
                        // Sample: [1970-01-01T00:00:00Z, 1980-01-01T01:01:01+01:00]
                        //    ==>  ['1970-01-01T00:00:00Z', '1980-01-01T01:01:01+01:00']
                        bracketLiteralText = NormalizeDateTimeCollectionItems(bracketLiteralText);
                    }
                }

                object collection = ODataUriConversionUtils.ConvertFromCollectionValue(bracketLiteralText, model, expectedType);
                LiteralToken collectionLiteralToken = new LiteralToken(collection, originalLiteralText, expectedType);
                operand = this.bindMethod(collectionLiteralToken) as CollectionConstantNode;
            }
            else
            {
                operand = this.bindMethod(queryToken) as CollectionNode;
            }

            if (operand == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_RightOperandNotCollectionValue);
            }

            return operand;
        }

        internal static string NormalizeStringCollectionItems(string literalText)
        {
            // a comma-separated list of primitive values, enclosed in parentheses,
            // or a single expression that resolves to a collection
            StringBuilder sb = new StringBuilder();
            char? openStringChar = null;
            char previousChar = '\0';
            int closeIndex = 0;

            for (int index = 0; index < literalText.Length; index++)
            {
                char c = literalText[index];
                if (openStringChar == null)
                {
                    // We are outside a string
                    switch (c)
                    {
                        case '"':
                            // We open the string
                            openStringChar = c;
                            sb.Append('"');
                            break;
                        case '\'':
                            if (previousChar == '\'')
                            {
                                // We re-open the string
                                openStringChar = c;
                                // We replace the characters append for closing by a simple quote
                                sb.Length = closeIndex;
                                sb.Append(c);
                                previousChar = c;
                                continue;
                            }
                            else
                            {
                                // We open the string
                                openStringChar = c;
                                sb.Append('"');
                            }
                            break;
                        case ',':
                        case '[':
                        case ']':
                            sb.Append(c);
                            break;
                        case '(':
                            sb.Append('[');
                            break;
                        case ')':
                            sb.Append(']');
                            break;
                        case 'n':
                            if (index + NullLiteral.Length <= literalText.Length
                                && literalText.Substring(index, NullLiteral.Length) == NullLiteral)
                            {
                                sb.Append(NullLiteral);
                            }
                            break;
                    }
                    // We reset the previous char
                    previousChar = '\0';
                }
                else
                {
                    // We are inside a string
                    if (c == openStringChar)
                    {
                        // This is the character to close the string
                        if (c == '"')
                        {
                            // String was openned with double quote
                            if (previousChar == '\\')
                            {
                                sb.Append('"');
                                // Keep the previous char inside the string
                                previousChar = c;
                                continue;
                            }
                        }
                        // We leave the current string
                        openStringChar = null;
                        closeIndex = sb.Length;
                        if (previousChar == '\0')
                        {
                            // We append \"\" so as to return "\"\"" instead of "".
                            // This is to avoid passing an empty string to the ConstantNode.
                            sb.Append("\\\"\\\"");
                        }
                        sb.Append('"');
                    }
                    else
                    {
                        // Add the character in string
                        if (c == '"' && openStringChar == '\'')
                        {
                            sb.Append('\\');
                        }
                        sb.Append(c);
                    }
                    // Keep the previous char inside the string
                    previousChar = c;
                }
            }
            // Auto close the last string (for compatibility with previous method version)
            if (openStringChar.HasValue)
            {
                if (sb[sb.Length - 1] == ']')
                {
                    sb.Insert(sb.Length - 1, '"');
                }
                else
                {
                    sb.Append("\"]");
                }
            }
            return sb.ToString();
        }

        private static string NormalizeGuidCollectionItems(string bracketLiteralText)
        {
            string normalizedText = bracketLiteralText.Substring(1, bracketLiteralText.Length - 2).Trim();

            // If we have empty brackets ()
            if (normalizedText.Length == 0)
            {
                return "[]";
            }

            string[] items = normalizedText.Split(',')
                .Select(s => s.Trim()).ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != NullLiteral && items[i][0] != '\'' && items[i][0] != '"')
                {
                    items[i] = String.Format(CultureInfo.InvariantCulture, "'{0}'", items[i]);
                }
            }

            return "[" + String.Join(",", items) + "]";
        }

        private static string NormalizeDateTimeCollectionItems(string bracketLiteralText)
        {
            string normalizedText = bracketLiteralText.Substring(1, bracketLiteralText.Length - 2).Trim();

            // If we have empty brackets ()
            if (normalizedText.Length == 0)
            {
                return "[]";
            }

            string[] items = normalizedText.Split(',')
                .Select(s => s.Trim()).ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                const string durationPrefix = "duration";
                if (items[i] == NullLiteral)
                {
                    continue;
                }
                if (items[i].StartsWith(durationPrefix, StringComparison.Ordinal))
                {
                    items[i] = items[i].Remove(0, durationPrefix.Length);
                }
                if (items[i][0] != '\'' && items[i][0] != '"')
                {
                    items[i] = String.Format(CultureInfo.InvariantCulture, "'{0}'", items[i]);
                }
            }

            return "[" + String.Join(",", items) + "]";
        }
    }
}
