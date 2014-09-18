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

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Values;

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
        internal bool TryBindDottedIdentifierAsEnum(DottedIdentifierToken dottedIdentifierToken, SingleValueNode parent, BindingState state, out QueryNode boundEnum)
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
            if ((typeReference != null) && !string.Equals(namespaceAndType, typeReference.ODataFullName()))
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
            UriPrimitiveTypeParser.TryRemovePrefix(namespaceAndType, ref text);
            UriPrimitiveTypeParser.TryRemoveQuotes(ref text);

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
                // ODataEnumValue.Value will always be numeric string like '3', '10' instead of 'Red', 'Solid,Yellow', etc.
                // so user code can easily Enum.Parse() them into CLR value.
                enumValue = new ODataEnumValue(parsedValue.ToString(CultureInfo.InvariantCulture), enumType.ODataFullName());
            }

            return success;
        }
    }
}
