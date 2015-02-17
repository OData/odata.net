//---------------------------------------------------------------------
// <copyright file="EntitySetDataRowWithStreams.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel.Data
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;

    /// <summary>
    /// Extends the entity-set data row to include stream data
    /// </summary>
    internal class EntitySetDataRowWithStreams : EntitySetDataRow
    {
        /// <summary>
        /// Initializes a new instance of the EntitySetDataRowWithStreams class
        /// </summary>
        /// <param name="parent">The parent of the row</param>
        /// <param name="entityType">The entity type of the row</param>
        public EntitySetDataRowWithStreams(EntitySetData parent, EntityType entityType)
            : base(parent, entityType)
        {
            this.Streams = new List<StreamData>();
        }

        /// <summary>
        /// Gets the stream data for this row
        /// </summary>
        public IList<StreamData> Streams { get; private set; }
    }
}
