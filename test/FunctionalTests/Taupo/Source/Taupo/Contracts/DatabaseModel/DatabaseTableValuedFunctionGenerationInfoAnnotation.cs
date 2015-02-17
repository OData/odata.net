//---------------------------------------------------------------------
// <copyright file="DatabaseTableValuedFunctionGenerationInfoAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Annotation storing information used to create database (sql) table-valued function.
    /// </summary>
    public class DatabaseTableValuedFunctionGenerationInfoAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the DatabaseTableValuedFunctionGenerationInfoAnnotation class.
        /// </summary>
        /// <param name="sourceTableName">Name of the table that values will be selected from in the TVF.</param>
        /// <param name="discriminatorsToFilter">List of discriminators that should be filtered from the result.</param>
        /// <param name="discriminatorColumnName">Name of the discriminator column.</param>
        public DatabaseTableValuedFunctionGenerationInfoAnnotation(string sourceTableName, IEnumerable<string> discriminatorsToFilter, string discriminatorColumnName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(sourceTableName, "sourceTableName");

            this.SourceTableName = sourceTableName;
            this.DiscriminatorsToFilterOut = discriminatorsToFilter.ToList().AsReadOnly();
            this.DiscriminatorColumnName = discriminatorColumnName;
        }

        /// <summary>
        /// Gets the name of the source table.
        /// </summary>
        /// <value>The start value.</value>
        public string SourceTableName { get; private set; }

        /// <summary>
        /// Gets the list of discriminators that should be filtered out from the result of table-valued function.
        /// </summary>
        public ReadOnlyCollection<string> DiscriminatorsToFilterOut { get; private set; }

        /// <summary>
        /// Gets the name of discriminator column
        /// </summary>
        public string DiscriminatorColumnName { get; private set; }
    }
}