//---------------------------------------------------------------------
// <copyright file="SchemaObject.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System.Collections.Generic;
    using System.Text;
    
    /// <summary>
    /// Represents a named schema object (with catalog, schema and name).
    /// </summary>
    public abstract class SchemaObject
    {
        /// <summary>
        /// Initializes a new instance of the SchemaObject class with given qualified name.
        /// </summary>
        /// <param name="catalog">SchemaObject catalog</param>
        /// <param name="schema">SchemaObject schema</param>
        /// <param name="name">SchemaObject name</param>
        protected SchemaObject(string catalog, string schema, string name)
        {
            this.Name = name;
            this.Schema = schema;
            this.Catalog = catalog;
            this.Annotations = new List<Annotation>();
        }

        /// <summary>
        /// Gets or sets object name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets schema name (such as 'dbo')
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Gets or sets catalog name.
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// Gets the list of StoredFunction annotations.
        /// </summary>
        /// <value>The annotations.</value>
        public IList<Annotation> Annotations { get; private set; }

        /// <summary>
        /// Adds the specified annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        public void Add(Annotation annotation)
        {
            this.Annotations.Add(annotation);
        }

        /// <summary>
        /// Returns a fully qualified schema object name.
        /// </summary>
        /// <returns>string in the form of: catalog.schema.name</returns>
        /// <remarks>If name is null, "(null)" string is used. Note that names are not quoted in any way, 
        /// so results of this function should only be used for debugging</remarks>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.Catalog != null)
            {
                sb.Append(this.Catalog);
                sb.Append(".");
            }

            if (this.Schema != null || sb.Length > 0)
            {
                sb.Append(this.Schema);
                sb.Append(".");
            }

            sb.Append(this.Name ?? "(null)");
            return sb.ToString();
        }
    }
}
