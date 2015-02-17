//---------------------------------------------------------------------
// <copyright file="DatabaseFunction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Represents a database function.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class DatabaseFunction : SchemaObject, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the DatabaseFunction class without a name.
        /// </summary>
        public DatabaseFunction() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseFunction class with a given name.
        /// </summary>
        /// <param name="name">Function name</param>
        public DatabaseFunction(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseFunction class with given schema and name.
        /// </summary>
        /// <param name="schema">Function schema</param>
        /// <param name="name">Function name</param>
        public DatabaseFunction(string schema, string name)
            : this(null, schema, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseFunction class with given catalog, schema and name.
        /// </summary>
        /// <param name="catalog">Function catalog</param>
        /// <param name="schema">Function schema</param>
        /// <param name="name">Function name</param>
        public DatabaseFunction(string catalog, string schema, string name)
            : base(catalog, schema, name)
        {
            this.Parameters = new List<DatabaseFunctionParameter>();
        }

        /// <summary>
        /// Gets function parameters.
        /// </summary>
        public IList<DatabaseFunctionParameter> Parameters { get; private set; }

        /// <summary>
        /// Gets or sets ReturnType.
        /// </summary>
        public DataType ReturnType { get; set; }

        /// <summary>
        /// Gets or sets function body (in store-specific SQL dialect)
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Sets a return type for a <see cref="DatabaseFunction"/>
        /// </summary>
        /// <param name="dataType">Return type</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public DatabaseFunction WithReturnType(DataType dataType)
        {
            this.ReturnType = dataType;
            return this;
        }

        /// <summary>
        /// Adds a <see cref="DatabaseFunctionParameter"/> to this function.
        /// </summary>
        /// <param name="parameter">Parameter to add</param>
        public void Add(DatabaseFunctionParameter parameter)
        {
            this.Parameters.Add(parameter);
        }

        /// <summary>
        /// Initializes <see cref="Body"/> property with given string.
        /// </summary>
        /// <param name="body">Function body</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public DatabaseFunction WithBody(string body)
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
