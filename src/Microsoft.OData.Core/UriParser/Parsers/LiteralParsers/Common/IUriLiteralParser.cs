//---------------------------------------------------------------------
// <copyright file="IUriLiteralParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers.Common
{
    #region Namespaces

    using Microsoft.OData.Edm;

    #endregion

    /// <summary>
    /// Interface for Uri literal parser.
    /// To parse the uri of an OData request into objects, the ODataUriParser uses UriLiteralParses.
    /// Implementation of this interface will parse a text of an EdmType to it's instance.
    /// </summary>
    public interface IUriLiteralParser
    {
        /// <summary>
        /// Parse the given text of EdmType <paramref name="targetType"/> to it's object instance.
        /// Return 'Null' if the text could not be parsed to the requested <paramref name="targetType"/>.
        /// Assign <paramref name="parsingException"/> paramter only in case the text could be parsed to the requested <paramref name="targetType"/>, but failed during the parsing proccess.
        /// </summary>
        /// <param name="text">Part of the uri which has to be parsed to a value of EdmType <paramref name="targetType"/></param>
        /// <param name="targetType">The type which the uri text has to be parsed to</param>
        /// <param name="parsingException">Assign the exception only in case the text could be parsed to the <paramref name="targetType"/> but failed during the parsing process</param>
        /// <returns>If the parsing proceess has succeeded, returns the parsed object, otherwise returns 'Null'</returns>
        object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException);

        // Consider add this API:
        // bool TryParseUriStringToType(string text, IEdmTypeReference targetType,out object targetValue, out UriTypeParsingException parsingException);
        //      This can be problematic because of the the 'bool' return type + the out exception parameter. The 'Try' function could return 'false' with null exception because it doesn't support the given type,
        //      bnd this not a 'standart' function so it can confuse the developers. Standart 'Try' function assign the out Exception paramter when return value is 'false'.
    }
}
