//---------------------------------------------------------------------
// <copyright file="StoredProcedure.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Database stored procedure.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class StoredProcedure : SchemaObject, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the StoredProcedure class.
        /// </summary>
        public StoredProcedure() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StoredProcedure class with given name.
        /// </summary>
        /// <param name="name">Procedure name</param>
        public StoredProcedure(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StoredProcedure class with given schema and name.
        /// </summary>
        /// <param name="schema">Procedure schema</param>
        /// <param name="name">Procedure name</param>
        public StoredProcedure(string schema, string name)
            : this(null, schema, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StoredProcedure class with given catalog, schema and name.
        /// </summary>
        /// <param name="catalog">Procedure catalog</param>
        /// <param name="schema">Procedure schema</param>
        /// <param name="name">Procedure name</param>
        public StoredProcedure(string catalog, string schema, string name)
            : base(catalog, schema, name)
        {
            this.Parameters = new List<DatabaseFunctionParameter>();
        }

        /// <summary>
        /// Gets or sets procedure body (in store-specific SQL dialect)
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets procedure parameters.
        /// </summary>
        public IList<DatabaseFunctionParameter> Parameters { get; private set; }

        /// <summary>
        /// Adds parameter to the procedure.
        /// </summary>
        /// <param name="parameter">Procedure parameter.</param>
        public void Add(DatabaseFunctionParameter parameter)
        {
            this.Parameters.Add(parameter);
        }

        /// <summary>
        /// Initializes <see cref="Body"/> property with given string.
        /// </summary>
        /// <param name="body">Function body</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public StoredProcedure WithBody(string body)
        {
            this.Body = body;
            return this;
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }
    }
}
