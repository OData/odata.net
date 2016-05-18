//---------------------------------------------------------------------
// <copyright file="UriTemplateExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// The class representing the URI Template parsing result.
    /// </summary>
    /// <remarks>
    /// A URI Template is a string wrapped in brackets, it is capable for describing a range of Uniform Resource Identifiers.
    /// The URI Template can be used for both building and parsing URI.
    /// Set the EnableUriTemplateParsing property of <see cref="ODataUriParser"/> to true, to enable parsing the Uri template.
    ///
    /// For example, in the following URI
    /// http://example.org/service/Customers({CID})
    /// {CID} is a validate Uri template for Customers' ID segment, in the parsing result of <see cref="UriTemplateExpression"/>,
    /// <see cref="LiteralText"/> would be the original literal "{CID}", and <see cref="ExpectedType"/> would be the actual type for Customers' ID.
    ///
    /// See RFC6570 for detail.
    /// </remarks>
    public sealed class UriTemplateExpression
    {
        /// <summary>
        /// The original text for the Uri template.
        /// </summary>
        public string LiteralText { get; internal set; }

        /// <summary>
        /// The expected type of the object which the Uri template stands for.
        /// </summary>
        public IEdmTypeReference ExpectedType { get; internal set; }
    }
}
