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

                    StringBuilder replacedText = new StringBuilder(bracketLiteralText);
                    replacedText[0] = '[';
                    replacedText[replacedText.Length - 1] = ']';
                    bracketLiteralText = replacedText.ToString();

                    Debug.Assert(expectedType.IsCollection());
                    if (expectedType.Definition.AsElementType().FullTypeName().Equals("Edm.String"))
                    {
                        // For collection of strings, need to convert single-quoted string to double-quoted string,
                        // and also, per ABNF, two consecutive single quotes  to one single quote.
                        // Sample: ['a''bc','''def','xyz'''] ==> ["a'bc","'def","xyz'"], which is legitimate Json format.
                        string[] items = bracketLiteralText.Substring(1, bracketLiteralText.Length - 2).Split(',');

                        // Skip conversion if the items are already in double-quote format (for backward compatibility).
                        // Note that per ABNF, query option strings should use single quotes.
                        if (items.Length > 0 && items[0][0] == '\'')
                        {
                            StringBuilder builder = new StringBuilder();
                            for (int i = 0; i < items.Length; i++)
                            {
                                string convertedItem = UriParserHelper.RemoveQuotes(items[i]);
                                if (i != items.Length - 1)
                                {
                                    builder.AppendFormat(CultureInfo.InvariantCulture, "\"{0}\",", convertedItem);
                                }
                                else
                                {
                                    // No trailing comma separator for last item of the collection.
                                    builder.AppendFormat(CultureInfo.InvariantCulture, "\"{0}\"", convertedItem);
                                }
                            }

                            bracketLiteralText =
                                String.Format(CultureInfo.InvariantCulture, "[{0}]", builder.ToString());
                        }
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
    }
}
