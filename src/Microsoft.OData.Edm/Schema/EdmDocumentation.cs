//---------------------------------------------------------------------
// <copyright file="EdmDocumentation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM documentation.
    /// </summary>
    internal class EdmDocumentation : IEdmDocumentation
    {
        private readonly string summary;
        private readonly string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDocumentation"/> class.
        /// </summary>
        /// <param name="summary">Summary of the documentation.</param>
        /// <param name="description">The documentation contents.</param>
        public EdmDocumentation(string summary, string description)
        {
            this.summary = summary;
            this.description = description;
        }

        /// <summary>
        /// Gets summary of this documentation.
        /// </summary>
        public string Summary
        {
            get { return this.summary; }
        }

        /// <summary>
        /// Gets documentation.
        /// </summary>
        public string Description
        {
            get { return this.description; }
        }
    }
}
