//---------------------------------------------------------------------
// <copyright file="RemoveSpatialFeatureFixup.cs" company="Microsoft">
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
    /// Entity model fixup for spatial related features
    /// </summary>
    public class RemoveSpatialFeatureFixup : IEntityModelFixup
    {
        /// <summary>
        /// Remove spatial properties and functions
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (var structuralType in model.EntityTypes.OfType<NamedStructuralType>().Concat(model.ComplexTypes.OfType<NamedStructuralType>()))
            {
                foreach (var spatialProperty in structuralType.Properties.ToArray().Where(p => p.PropertyType.IsSpatial()))
                {
                    structuralType.Properties.Remove(spatialProperty);
                }
            }

            foreach (var function in model.Functions.ToList().Where(f => (f.ReturnType != null && f.ReturnType.IsSpatial()) || f.Parameters.Any(p => p.DataType.IsSpatial())))
            {
                model.Remove(function);
            }
        }
    }
}