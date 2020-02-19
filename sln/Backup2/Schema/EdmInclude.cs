//---------------------------------------------------------------------
// <copyright file="EdmInclude.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// The include information for referenced model.
    /// </summary>
    public class EdmInclude : IEdmInclude
    {
        private readonly string alias;
        private readonly string namespaceIncluded;

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="namespaceIncluded">The namespace.</param>
        public EdmInclude(string alias, string namespaceIncluded)
        {
            this.alias = alias;
            this.namespaceIncluded = namespaceIncluded;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias
        {
            get
            {
                return this.alias;
            }
        }

        /// <summary>
        /// Gets the namespace to include.
        /// </summary>
        public string Namespace
        {
            get
            {
                return this.namespaceIncluded;
            }
        }
    }
}
