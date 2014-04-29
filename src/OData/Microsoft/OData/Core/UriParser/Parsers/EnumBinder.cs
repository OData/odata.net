//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Diagnostics.CodeAnalysis;

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
            return TryBindIdentifier(dottedIdentifierToken.Identifier, state.Model, out boundEnum);
        }

        /// <summary>
        /// Try to bind an identifier to a EnumNode
        /// </summary>
        /// <param name="identifier">the identifier to bind</param> 
        /// <param name="model">the current model.</param>
        /// <param name="boundEnum">an enum node .</param>
        /// <returns>true if we bound an enum for this token.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        internal static bool TryBindIdentifier(string identifier, IEdmModel model, out QueryNode boundEnum)
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

            // find the type from edm model
            IEdmEnumType enumType = UriEdmHelpers.FindEnumTypeFromModel(model, namespaceAndType);
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
            enumType.TryParseEnum(enumValueString, out enumValue);

            if (enumValue == null)
            {
                return false;
            }

            // create an enum node, enclosing an odata enum value
            EdmEnumTypeReference enumTypeReference = new EdmEnumTypeReference(enumType, false);
            boundEnum = new ConstantNode(enumValue, identifier, enumTypeReference);

            return true;
        }
    }
}
