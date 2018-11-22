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
        /// Delegate for a function that normalizes a string item representing a certain type.
        /// Each type should define a different implementation of the delegate.
        /// </summary>
        /// <param name="item">The item to be normalized.</param>
        /// <returns>Normalized string of the item.</returns>
        private delegate string NormalizeFunction(string item);

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

                    StringBuilder replacedText = new StringBuilder(bracketLiteralText);
                    replacedText[0] = '[';
                    replacedText[replacedText.Length - 1] = ']';
                    bracketLiteralText = replacedText.ToString();

                    Debug.Assert(expectedType.IsCollection());
                    string expectedTypeFullName = expectedType.Definition.AsElementType().FullTypeName();
                    if (expectedTypeFullName.Equals("Edm.String"))
                    {
                        // For collection of strings, need to convert single-quoted string to double-quoted string,
                        // and also, per ABNF, two consecutive single quotes  to one single quote.
                        // Sample: ['a''bc','''def','xyz'''] ==> ["a'bc","'def","xyz'"], which is legitimate Json format.
                        bracketLiteralText = NormalizeCollectionItems(bracketLiteralText, NormalizeStringItem);
                    }
                    else if (expectedTypeFullName.Equals("Edm.Guid"))
                    {
                        // For collection of Guids, need to convert the Guid literals to single-quoted form, so that it is compatible
                        // with the Json reader used for deserialization.
                        // Sample: (D01663CF-EB21-4A0E-88E0-361C10ACE7FD, 492CF54A-84C9-490C-A7A4-B5010FAD8104)
                        //    ==>  ('D01663CF-EB21-4A0E-88E0-361C10ACE7FD', '492CF54A-84C9-490C-A7A4-B5010FAD8104')
                        bracketLiteralText = NormalizeCollectionItems(bracketLiteralText, NormalizeGuidItem);
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

        private static string NormalizeCollectionItems(string bracketLiteralText, NormalizeFunction normalizeFunc)
        {
            string[] items = bracketLiteralText.Substring(1, bracketLiteralText.Length - 2).Split(',')
                .Select(s => s.Trim()).ToArray();

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < items.Length; i++)
            {
                string convertedItem = normalizeFunc(items[i]);
                if (i != items.Length - 1)
                {
                    builder.AppendFormat(CultureInfo.InvariantCulture, "{0},", convertedItem);
                }
                else
                {
                    // No trailing comma separator for last str of the collection.
                    builder.Append(convertedItem);
                }
            }

            return String.Format(CultureInfo.InvariantCulture, "[{0}]", builder.ToString());
        }

        /// <summary>
        /// Function to normalize quoted string, ensuring single quotes are escaped properly.
        /// If the string is double-quoted, no op since single quote doesn't need to be escaped.
        /// </summary>
        /// <param name="str">The quoted string item to be normalized.</param>
        /// <returns>The double-quoted string with single quotes properly escaped.</returns>
        private static string NormalizeStringItem(string str)
        {
            // Validate the string item is quoted properly.
            if ( !(    str[0] == '\'' && str[str.Length-1] == '\''
                    || str[0] == '"'  && str[str.Length-1] == '"')
                )
            {
                throw new ODataException(ODataErrorStrings.StringItemShouldBeQuoted(str));
            }

            // Skip conversion if the items are already in double-quote format (for backward compatibility).
            // Note that per ABNF, query option strings should use single quotes.
            string convertedString = str;
            if (str[0] == '\'')
            {
                convertedString = String.Format(CultureInfo.InvariantCulture, "\"{0}\"", UriParserHelper.RemoveQuotes(str));
            }

            return convertedString;
        }

        /// <summary>
        /// Function to normalize string representing GUID so that it is compatible with Json reader for de-serialization.
        /// No op if the input string is ready in quoted form.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns>A Guid string in quoted form.</returns>
        private static string NormalizeGuidItem(string guid)
        {
            // Skip conversion if the items are already in quoted format (for backward compatibility).
            // Otherwise, make it single-quoted.
            if (guid[0] == '\'' || guid[0] == '"')
            {
                return guid;
            }
            else
            {
                return String.Format(CultureInfo.InvariantCulture, "'{0}'", guid);
            }
        }
    }
}
