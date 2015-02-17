//---------------------------------------------------------------------
// <copyright file="RemoveConcurrencyAnnotationFromDerivedTypesFixup.cs" company="Microsoft">
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
    /// Entity model fixup for removing concurrency tokens defined on derivedTypes
    /// </summary>
    public class RemoveConcurrencyAnnotationFromDerivedTypesFixup : IEntityModelFixup
    {   
        /// <summary>
        /// Remove concurrency tokens defined on complex types and properties 
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (var et in model.EntityTypes)
            {
                if (et.BaseType != null) 
                { 
                    foreach (var property in et.Properties)
                    {
                        // ToList is to avoid modifying the collection during enumeration
                        foreach (var annotation in property.Annotations.OfType<ConcurrencyTokenAnnotation>().ToList())
                        {
                            property.Annotations.Remove(annotation);
                        }
                    }
                }
            }
        }
    }
}