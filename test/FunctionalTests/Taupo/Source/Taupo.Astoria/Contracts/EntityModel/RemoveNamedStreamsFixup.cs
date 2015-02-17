//---------------------------------------------------------------------
// <copyright file="RemoveNamedStreamsFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Entity model fixup for removing named streams
    /// </summary>
    public class RemoveNamedStreamsFixup : IEntityModelFixup
    {
        /// <summary>
        /// Initializes a new instance of the RemoveNamedStreamsFixup class
        /// </summary>
        public RemoveNamedStreamsFixup()
        {
        }

        /// <summary>
        /// Remove named streams and corresponding features in model for EF provider
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (var entityType in model.EntityTypes)
            {
                entityType.Properties.RemoveAll(p => p.IsStream());
            }
        }
    }
}