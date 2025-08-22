//---------------------------------------------------------------------
// <copyright file="IUriLiteralParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Defines a contract for parsing URI literal values into .NET objects based on their corresponding EDM types.
    /// Implementations of this interface are used by the OData URI parser to convert segments of an OData URI
    /// into strongly-typed CLR objects according to the provided <see cref="IEdmTypeReference"/>.
    /// </summary>
    public interface IUriLiteralParser
    {
        /// <summary>
        /// Parse the given text of Edm type <paramref name="targetType"/> to it's object instance.
        /// </summary>
        /// <param name="text">Part of the URI which has to be parsed to a value of Edm type <paramref name="targetType"/>.</param>
        /// <param name="targetType">The Edm type which the URI text has to be parsed to.</param>
        /// <param name="parsingException">
        /// When the parser recognizes the <paramref name="targetType"/> and attempts to parse <paramref name="text"/>, 
        /// but an error occurs during the parsing process (for example, invalid format or value out of range), 
        /// assign a <see cref="UriLiteralParsingException"/> describing the failure to this parameter.
        /// Set to <c>null</c> if parsing is successful or if the parser does not support the specified <paramref name="targetType"/>.
        /// </param>
        /// <returns>The parsed object if parsing process succeeds; otherwise <c>null</c>.</returns>
        /// <remarks>
        /// The <paramref name="parsingException"/> parameter should be assigned a non-null value only
        /// if the parser supports the specified <paramref name="targetType"/> and an error occurs
        /// during the parsing process (for example, due to invalid format or value out of range).
        /// If the parser does not support the <paramref name="targetType"/>, or if parsing is successful,
        /// <paramref name="parsingException"/> must be set to <c>null</c>.
        /// </remarks>
        object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException);

        // Consider add this API:
        // bool TryParseUriStringToType(string text, IEdmTypeReference targetType,out object targetValue, out UriTypeParsingException parsingException);
        //      This can be problematic because of the the 'bool' return type + the out exception parameter. The 'Try' function could return 'false' with null exception because it doesn't support the given type,
        //      bnd this not a 'standard' function so it can confuse the developers. Standard 'Try' function assign the out Exception parameter when return value is 'false'.
    }
}
