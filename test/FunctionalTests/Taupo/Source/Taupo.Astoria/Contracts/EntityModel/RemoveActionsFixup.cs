//---------------------------------------------------------------------
// <copyright file="RemoveActionsFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    
    /// <summary>
    /// Entity model fixup for removing actions from the model
    /// </summary>
    public class RemoveActionsFixup : IEntityModelFixup
    {
        /// <summary>
        /// Remove navigation properties defined on derived types
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (var function in model.Functions.ToList().Where(f => f.Annotations.OfType<ServiceOperationAnnotation>().Any(so => so.IsAction)))
            {
                model.Remove(function);
            }
        }
    }
}