//---------------------------------------------------------------------
// <copyright file="EnumBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.UriParser
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Enum binder
    /// </summary>
    internal sealed class EnumBinder
    {
        /// <summary>
        /// Try to bind a dotted identifier as enum node
        /// </summary>
        /// <param name="dottedIdentifierToken">a dotted identifier token</param>
        /// <param name="parent">the parent node</param>
        /// <param name="state">the current state of the binding algorithm</param>
        /// <param name="resolver">ODataUriResolver</param>
        /// <param name="boundEnum">the output bound enum node</param>
        /// <returns>true if we bound an enum node, false otherwise.</returns>
        internal static bool TryBindDottedIdentifierAsEnum(DottedIdentifierToken dottedIdentifierToken, SingleValueNode parent, BindingState state, ODataUriResolver resolver, out QueryNode boundEnum)
        {
            return TryBindIdentifier(dottedIdentifierToken.Identifier.AsMemory(), null, state.Model, resolver, out boundEnum);
        }

        /// <summary>
        /// Try to bind an identifier to a EnumNode
        /// </summary>
        /// <param name="identifier">the identifier to bind</param>
        /// <param name="typeReference">the enum typeReference</param>
        /// <param name="modelWhenNoTypeReference">the current model when no enum typeReference.</param>
        /// <param name="boundEnum">an enum node .</param>
        /// <returns>true if we bound an enum for this token.</returns>
        internal static bool TryBindIdentifier(string identifier, IEdmEnumTypeReference typeReference, IEdmModel modelWhenNoTypeReference, out QueryNode boundEnum)
        {
            return TryBindIdentifier(identifier.AsMemory(), typeReference, modelWhenNoTypeReference, null, out boundEnum);
        }

        /// <summary>
        /// Try to bind an identifier to a EnumNode
        /// </summary>
        /// <param name="identifier">the identifier to bind</param>
        /// <param name="typeReference">the enum typeReference</param>
        /// <param name="modelWhenNoTypeReference">the current model when no enum typeReference.</param>
        /// <param name="resolver">ODataUriResolver .</param>
        /// <param name="boundEnum">an enum node .</param>
        /// <returns>true if we bound an enum for this token.</returns>
        internal static bool TryBindIdentifier(ReadOnlyMemory<char> identifier, IEdmEnumTypeReference typeReference, IEdmModel modelWhenNoTypeReference, ODataUriResolver resolver, out QueryNode boundEnum)
        {
            boundEnum = null;
            ReadOnlySpan<char> text = identifier.Span;

            // Parse the string, e.g., NS.Color'Green'
            // get type information, and also convert Green into an ODataEnumValue

            // find the first ', before that, it is namespace.type
            int indexOfSingleQuote = text.IndexOf('\'');
            if (indexOfSingleQuote < 0)
            {
                return false;
            }

            ReadOnlySpan<char> namespaceAndType = text.Slice(0, indexOfSingleQuote);
            Debug.Assert((typeReference == null) || (modelWhenNoTypeReference == null), "((typeReference == null) || (modelWhenNoTypeReference == null)");

            // validate typeReference but allow type name not found in model for delayed throwing.
            if ((typeReference != null) && !namespaceAndType.Equals(typeReference.FullName(), StringComparison.Ordinal))
            {
                return false;
            }

            // get the type
            IEdmEnumType enumType = typeReference != null ?
               typeReference.EnumDefinition() :
               UriEdmHelpers.FindEnumTypeFromModel(modelWhenNoTypeReference, namespaceAndType.ToString(), resolver);
            if (enumType == null)
            {
                return false;
            }

            // now, find out the value
            UriParserHelper.TryRemovePrefix(namespaceAndType, ref text);
            UriParserHelper.TryRemoveSingleQuotes(ref text, out string value);

            // parse string or int value to edm enum value
            ODataEnumValue enumValue;

            if (!TryParseEnum(enumType, text, out enumValue))
            {
                return false;
            }

            // create an enum node, enclosing an odata enum value
            IEdmEnumTypeReference enumTypeReference = typeReference ?? new EdmEnumTypeReference(enumType, false);

            MemoryMarshal.TryGetString(identifier, out string identifierString, out int _, out int _);
            boundEnum = new ConstantNode(enumValue, identifierString, enumTypeReference);

            return true;
        }


        /// <summary>
        /// Parse string or integer to enum value
        /// </summary>
        /// <param name="enumType">edm enum type</param>
        /// <param name="value">input string value</param>
        /// <param name="enumValue">output edm enum value</param>
        /// <returns>true if parse succeeds, false if fails</returns>
        internal static bool TryParseEnum(IEdmEnumType enumType, ReadOnlySpan<char> value, out ODataEnumValue enumValue)
        {
            long parsedValue;
            bool success = enumType.TryParseEnum(value, true, out parsedValue);
            enumValue = null;
            if (success)
            {
                // Sam noted 04/08/20226: The following sentence doesn't make sense.But we want to keep the behavior unchanged for backward compatibility, so we keep it as is.

                // ODataEnumValue.Value will always be numeric string like '3', '10' instead of 'Cyan', 'Solid,Yellow', etc.
                // so user code can easily Enum.Parse() them into CLR value.
                enumValue = new ODataEnumValue(parsedValue.ToString(CultureInfo.InvariantCulture), enumType.FullTypeName());
            }

            return success;
        }
    }
}
