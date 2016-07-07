//---------------------------------------------------------------------
// <copyright file="ODataUnresolvedFunctionParameterAlias.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Represents an aliased parameter in a function call that has not yet been resolved to a specific value.
    /// </summary>
    public class ODataUnresolvedFunctionParameterAlias
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ODataUnresolvedFunctionParameterAlias"/>.
        /// </summary>
        /// <param name="alias">The alias provided as the parameter value.</param>
        /// <param name="type">The EDM type of the parameter represented by this alias.</param>
        public ODataUnresolvedFunctionParameterAlias(string alias, IEdmTypeReference type)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(alias, "alias");
            this.Alias = alias;
            this.Type = type;
        }

        /// <summary>
        /// The EDM type of the parameter represented by this alias.
        /// </summary>
        public IEdmTypeReference Type { get; private set; }

        /// <summary>
        /// The alias provided as the parameter value.
        /// </summary>
        public string Alias { get; private set; }
    }
}