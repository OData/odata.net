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
            CollectionNode right = this.GetCollectionOperandFromToken(
                inToken.Right, new EdmCollectionTypeReference(new EdmCollectionType(left.TypeReference)), state.Model);

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
                    if (bracketLiteralText.Replace(" ", String.Empty) == "()")
                    {
                        throw new ODataException(ODataErrorStrings.MetadataBinder_RightOperandNotCollectionValue);
                    }

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

        private static string NormalizeStringCollectionItems(string literalText)
        {
            // a comma-separated list of primitive values, enclosed in parentheses, or a single expression that resolves to a collection
            // However, for String collection, we should process:
            // 1) comma could be part of the string value
            // 2) single quote could not be part of string value
            // 3) double quote could be part of string value, double quote also could be the starting and ending character.

            // remove the '[' and ']'
            string normalizedText = literalText.Substring(1, literalText.Length - 2).Trim();
            int length = normalizedText.Length;
            StringBuilder sb = new StringBuilder(length + 2);
            sb.Append('[');
            for (int i = 0; i < length; i++)
            {
                char ch = normalizedText[i];
                switch (ch)
                {
                    case '"':
                        i = ProcessDoubleQuotedStringItem(i, normalizedText, sb);
                        break;

                    case '\'':
                        i = ProcessSingleQuotedStringItem(i, normalizedText, sb);
                        break;

                    case ' ':
                        // ignore all whitespaces between items
                        break;

                    case ',':
                        // for multiple comma(s) between items, for example ('abc',,,'xyz'),
                        // We let it go and let the next layer to identify the problem by design.
                        sb.Append(',');
                        break;

                    case 'n':
                        // it maybe null
                        int index = normalizedText.IndexOf(',', i + 1);
                        string subStr;
                        if (index < 0)
                        {
                            subStr = normalizedText.Substring(i).TrimEnd(' ');
                            i = length - 1;
                        }
                        else
                        {
                            subStr = normalizedText.Substring(i, index - i).TrimEnd(' ');
                            i = index - 1;
                        }

                        if (subStr == "null")
                        {
                            sb.Append("null");
                        }
                        else
                        {
                            throw new ODataException(ODataErrorStrings.StringItemShouldBeQuoted(subStr));
                        }

                        break;

                    default:
                        // any other character between items is not valid.
                        throw new ODataException(ODataErrorStrings.StringItemShouldBeQuoted(ch));
                }
            }

            sb.Append(']');
            return sb.ToString();
        }

        private static int ProcessDoubleQuotedStringItem(int start, string input, StringBuilder sb)
        {
            Debug.Assert(input[start] == '"');

            int length = input.Length;
            int k = start + 1;

            // no matter it's single quote or not, just starting it as double quote (JSON).
            sb.Append('"');

            for (; k < length; k++)
            {
                char next = input[k];
                if (next == '"')
                {
                    break;
                }
                else if (next == '\\')
                {
                    sb.Append('\\');
                    if (k + 1 >= length)
                    {
                        // if end of string, stop it.
                        break;
                    }
                    else
                    {
                        // otherwise, append "\x" into
                        sb.Append(input[k + 1]);
                        k++;
                    }
                }
                else
                {
                    sb.Append(next);
                }
            }

            // no matter it's single quote or not, just ending it as double quote.
            sb.Append('"');
            return k;
        }

        private static int ProcessSingleQuotedStringItem(int start, string input, StringBuilder sb)
        {
            Debug.Assert(input[start] == '\'');

            int length = input.Length;
            int k = start + 1;

            // no matter it's single quote or not, just starting it as double quote (JSON).
            sb.Append('"');

            for (; k < length; k++)
            {
                char next = input[k];
                if (next == '\'')
                {
                    if (k + 1 >= length || input[k + 1] != '\'')
                    {
                        // match with single qutoe ('), stop it.
                        break;
                    }
                    else
                    {
                        // Unescape the double single quotes as one single quote, and continue
                        sb.Append('\'');
                        k++;
                    }
                }
                else if (next == '"')
                {
                    sb.Append('\\');
                    sb.Append('"');
                }
                else if (next == '\\')
                {
                    sb.Append("\\\\");
                }
                else
                {
                    sb.Append(next);
                }
            }

            // no matter it's single quote or not, just ending it as double quote.
            sb.Append('"');
            return k;
        }

        private static string NormalizeGuidCollectionItems(string bracketLiteralText)
        {
            string[] items = bracketLiteralText.Substring(1, bracketLiteralText.Length - 2).Split(',')
                .Select(s => s.Trim()).ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i][0] != '\'' && items[i][0] != '"')
                {
                    items[i] = String.Format(CultureInfo.InvariantCulture, "'{0}'", items[i]);
                }
            }

            return "[" + String.Join(",", items) + "]";
        }
    }
}
