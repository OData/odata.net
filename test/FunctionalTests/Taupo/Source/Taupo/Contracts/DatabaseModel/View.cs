//---------------------------------------------------------------------
// <copyright file="View.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    /// <summary>
    /// Represents a database view.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class View : Table
    {
        /// <summary>
        /// Initializes a new instance of the View class.
        /// </summary>
        public View() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the View class with a given name
        /// </summary>
        /// <param name="name">Database view name</param>
        public View(string name) : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the View class with a given name and schema.
        /// </summary>
        /// <param name="schema">Database view schema name</param>
        /// <param name="name">Database view name</param>
        public View(string schema, string name) : this(null, schema, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the View class with given catalog, schema and name.
        /// </summary>
        /// <param name="catalog">Database view catalog name.</param>
        /// <param name="schema">Database view schema name.</param>
        /// <param name="name">Database view name.</param>
        public View(string catalog, string schema, string name) : base(catalog, schema, name)
        {
        }

        /// <summary>
        /// Gets or sets view definition (in store-specific SQL dialect)
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// Initializes <see cref="Definition"/> property with given string.
        /// </summary>
        /// <param name="definition">View definition</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public View WithDefinition(string definition)
        {
            this.Definition = definition;
            return this;
        }
    }
}
