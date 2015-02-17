//---------------------------------------------------------------------
// <copyright file="SupportedOperationsAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// This annotation specifies the different operations allowed by an entity set 
    /// </summary>
    public class SupportedOperationsAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the SupportedOperationsAnnotation class
        /// </summary>
        public SupportedOperationsAnnotation()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SupportedOperationsAnnotation class
        /// </summary>
        /// <param name="supportsAllOperations">A value indicating whether this entity set can be queried,inserted,updated,deleted</param>
        public SupportedOperationsAnnotation(bool supportsAllOperations)
            : this(supportsAllOperations, supportsAllOperations, supportsAllOperations, supportsAllOperations)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SupportedOperationsAnnotation class
        /// </summary>
        /// <param name="supportsQuery">A value indicating whether this entity set can be queried</param>
        /// <param name="supportsInsert">A value indicating whether instances can be added to this entity set </param>
        /// <param name="supportsUpdate">A value indicating whether instances from this entity set can be updated </param>
        /// <param name="supportsDelete">A value indicating whether instances from this entity set can be deleted</param>
        public SupportedOperationsAnnotation(bool supportsQuery, bool supportsInsert, bool supportsUpdate, bool supportsDelete)
            : base()
        {
            this.SupportsQuery = supportsQuery;
            this.SupportsInsert = supportsInsert;
            this.SupportsUpdate = supportsUpdate;
            this.SupportsDelete = supportsDelete;
        }

        /// <summary>
        /// Gets a value indicating whether this entity set can be queried
        /// </summary>
        public bool SupportsQuery { get; private set; }

        /// <summary>
        /// Gets a value indicating whether instances can be added to this entity set
        /// </summary>
        public bool SupportsInsert { get; private set; }

        /// <summary>
        /// Gets a value indicating whether instances from this entity set can be updated
        /// </summary>
        public bool SupportsUpdate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether instances from this entity set can be deleted
        /// </summary>
        public bool SupportsDelete { get; private set; }
    }
}
