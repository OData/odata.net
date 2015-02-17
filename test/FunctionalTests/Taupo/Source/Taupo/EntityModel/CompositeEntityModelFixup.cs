//---------------------------------------------------------------------
// <copyright file="CompositeEntityModelFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// An entity model fixup that contains other fixups and applies each of them to a given model
    /// </summary>
    public class CompositeEntityModelFixup : IEntityModelFixup
    {
        /// <summary>
        /// Initializes a new instance of the CompositeEntityModelFixup class
        /// </summary>
        /// <param name="fixups">The initial set of fixups</param>
        public CompositeEntityModelFixup(params IEntityModelFixup[] fixups)
            : this(fixups.AsEnumerable())
        {
        }

        /// <summary>
        /// Initializes a new instance of the CompositeEntityModelFixup class
        /// </summary>
        /// <param name="fixups">The initial set of fixups</param>
        public CompositeEntityModelFixup(IEnumerable<IEntityModelFixup> fixups)
        {
            ExceptionUtilities.CheckCollectionDoesNotContainNulls(fixups, "fixups");
            this.Fixups = new List<IEntityModelFixup>(fixups);
        }

        /// <summary>
        /// Gets the collection of fixups that should be applied
        /// </summary>
        public IList<IEntityModelFixup> Fixups { get; private set; }

        /// <summary>
        /// Invokes each of the fixup's in the collection
        /// </summary>
        /// <param name="model">the model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            foreach (var fixup in this.Fixups)
            {
                fixup.Fixup(model);
            }
        }
    }
}