//---------------------------------------------------------------------
// <copyright file="StoreFunctionBodyGenerationInfoAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Annotation storing information used to create store (ssdl) function.
    /// </summary>
    public class StoreFunctionBodyGenerationInfoAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the StoreFunctionBodyGenerationInfoAnnotation class.
        /// </summary>
        /// <param name="sourceEntitySet">Store EntitySet (table) that is the source of the table-valued function.</param>
        /// <param name="resultTypesToFilter">List of entity types that should be filtered from the result.</param>
        /// <param name="discriminatorColumnName">Name of the discriminator column.</param>
        public StoreFunctionBodyGenerationInfoAnnotation(EntitySet sourceEntitySet, IEnumerable<string> resultTypesToFilter, string discriminatorColumnName)
        {
            ExceptionUtilities.CheckArgumentNotNull(sourceEntitySet, "sourceEntitySet");

            this.SourceEntitySet = sourceEntitySet;
            this.ResultTypesToFilterOut = resultTypesToFilter.ToList().AsReadOnly();
            this.DiscriminatorColumnName = discriminatorColumnName;
        }

        /// <summary>
        /// Gets entity set that will be used to generate source table name for the body of the table-valued function (SELECT * FROM *Table* as t)
        /// </summary>
        public EntitySet SourceEntitySet { get; private set; }

        /// <summary>
        /// Gets the list of entity types that should be filtered out from the result of table-valued function.
        /// </summary>
        public ReadOnlyCollection<string> ResultTypesToFilterOut { get; private set; }

        /// <summary>
        /// Gets the name of discriminator column in the table valued function
        /// </summary>
        public string DiscriminatorColumnName { get; private set; }
    }
}
