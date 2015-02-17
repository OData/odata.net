//---------------------------------------------------------------------
// <copyright file="DatabaseSchema.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a database model with tables, views, procedures, functions, etc.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class DatabaseSchema : IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the DatabaseSchema class.
        /// </summary>
        public DatabaseSchema()
        {
            this.Tables = new List<Table>();
            this.Views = new List<View>();
            this.Procedures = new List<StoredProcedure>();
            this.Functions = new List<DatabaseFunction>();
        }

        /// <summary>
        /// Gets tables in the database.
        /// </summary>
        public IList<Table> Tables { get; private set; }

        /// <summary>
        /// Gets views in the database.
        /// </summary>
        public IList<View> Views { get; private set; }

        /// <summary>
        /// Gets stored procedures in the database.
        /// </summary>
        public IList<StoredProcedure> Procedures { get; private set; }

        /// <summary>
        /// Gets functions in the database.
        /// </summary>
        public IList<DatabaseFunction> Functions { get; private set; }

        /// <summary>
        /// Adds given <see cref="Table"/> the <see cref="Tables"/> collection.
        /// </summary>
        /// <param name="table">Table to add.</param>
        public void Add(Table table)
        {
            this.Tables.Add(table);
        }

        /// <summary>
        /// Adds given <see cref="View"/> to the <see cref="Views"/> collection.
        /// </summary>
        /// <param name="view">View to add.</param>
        public void Add(View view)
        {
            this.Views.Add(view);
        }

        /// <summary>
        /// Adds given <see cref="StoredProcedure"/> to <see cref="Procedures"/> collection.
        /// </summary>
        /// <param name="procedure">Procedure to add.</param>
        public void Add(StoredProcedure procedure)
        {
            this.Procedures.Add(procedure);
        }

        /// <summary>
        /// Adds given <see cref="DatabaseFunction"/> to <see cref="Functions"/> collection.
        /// </summary>
        /// <param name="function">Function to add.</param>
        public void Add(DatabaseFunction function)
        {
            this.Functions.Add(function);
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
