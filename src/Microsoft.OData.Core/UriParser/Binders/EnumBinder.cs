//---------------------------------------------------------------------
// <copyright file="EnumBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Parsers.UriParsers;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;

    /// <summary>
    /// Enum binder
    /// </summary>
    internal sealed class EnumBinder
    {
        /// <summary>
        /// Method to use for binding the parent node, if needed.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor bindMethod;

        /// <summary>
        /// Constructs a EnumBinder with the given method to be used binding the parent token if needed.
        /// </summary>
        /// <param name="bindMethod">Method to use for binding the parent token, if needed.</param>
        internal EnumBinder(MetadataBinder.QueryTokenVisitor bindMethod)
        {
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Try to bind a dotted identifier as enum node
        /// </summary>
        /// <param name="dottedIdentifierToken">a dotted identifier token</param>
        /// <param name="parent">the parent node</param>
        /// <param name="state">the current state of the binding algorithm</param>
        /// <param name="boundEnum">the output bound enum node</param>
        /// <returns>true if we bound an enum node, false otherwise.</returns>
        internal static bool TryBindDottedIdentifierAsEnum(DottedIdentifierToken dottedIdentifierToken, SingleValueNode parent, BindingState state, out QueryNode boundEnum)
        {
            return TryBindIdentifier(dottedIdentifierToken.Identifier, null, state.Model, out boundEnum);
        }

        /// <summary>
        /// Try to bind an identifier to a EnumNode
        /// </summary>
        /// <param name="identifier">the identifier to bind</param> 
        /// <param name="typeReference">the enum typeReference</param>
        /// <param name="modelWhenNoTypeReference">the current model when no enum typeReference.</param>
        /// <param name="boundEnum">an enum node .</param>
        /// <returns>true if we bound an enum for this token.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        internal static bool TryBindIdentifier(string identifier, IEdmEnumTypeReference typeReference, IEdmModel modelWhenNoTypeReference, out QueryNode boundEnum)
        {
            boundEnum = null;
            string text = identifier;

            // parse the string, e.g., NS.Color'Green'
            // get type information, and also convert Green into an ODataEnumValue

            // find the first ', before that, it is namespace.type
            int indexOfSingleQuote = text.IndexOf('\'');
            if (indexOfSingleQuote < 0)
            {
                return false;
            }

            string namespaceAndType = text.Substring(0, indexOfSingleQuote);
            Debug.Assert((typeReference == null) || (modelWhenNoTypeReference == null), "((typeReference == null) || (modelWhenNoTypeReference == null)");

            // validate typeReference but allow type name not found in model for delayed throwing.
            if ((typeReference != null) && !string.Equals(namespaceAndType, typeReference.FullName()))
            {
                return false;
            }

            // get the type
            IEdmEnumType enumType = typeReference != null
                ?
               (IEdmEnumType)typeReference.Definition
                :
                UriEdmHelpers.FindEnumTypeFromModel(modelWhenNoTypeReference, namespaceAndType);
            if (enumType == null)
            {
                return false;
            }

            // now, find out the value
            UriParserHelper.TryRemovePrefix(namespaceAndType, ref text);
            UriParserHelper.TryRemoveQuotes(ref text);

            // parse string or int value to edm enum value
            string enumValueString = text;
            ODataEnumValue enumValue;

            if (!TryParseEnum(enumType, enumValueString, out enumValue))
            {
                return false;
            }

            // create an enum node, enclosing an odata enum value
            IEdmEnumTypeReference enumTypeReference = typeReference ?? new EdmEnumTypeReference(enumType, false);
            boundEnum = new ConstantNode(enumValue, identifier, enumTypeReference);

            return true;
        }


        /// <summary>
        /// Parse string or integer to enum value
        /// </summary>
        /// <param name="enumType">edm enum type</param>
        /// <param name="value">input string value</param>
        /// <param name="enumValue">output edm enum value</param>
        /// <returns>true if parse succeeds, false if fails</returns>
        internal static bool TryParseEnum(IEdmEnumType enumType, string value, out ODataEnumValue enumValue)
        {
            long parsedValue;
            bool success = enumType.TryParseEnum(value, true, out parsedValue);
            enumValue = null;
            if (success)
            {
                // ODataEnumValue.Value will always be numeric string like '3', '10' instead of 'Cyan', 'Solid,Yellow', etc.
                // so user code can easily Enum.Parse() them into CLR value.
                enumValue = new ODataEnumValue(parsedValue.ToString(CultureInfo.InvariantCulture), enumType.FullTypeName());
            }

            return success;
        }
    }
}
