//---------------------------------------------------------------------
// <copyright file="RemoveConcurrencyTokensFromComplexTypesFixup.cs" company="Microsoft">
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
    /// Entity model fixup for removing concurrency tokens defined on complex types and properties
    /// </summary>
    public class RemoveConcurrencyTokensFromComplexTypesFixup : IEntityModelFixup
    {
        /// <summary>
        /// Initializes a new instance of the RemoveConcurrencyTokensFromComplexTypesFixup class
        /// </summary>
        public RemoveConcurrencyTokensFromComplexTypesFixup()
        {
        }

        /// <summary>
        /// Remove concurrency tokens defined on complex types and properties
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (var ct in model.ComplexTypes)
            {
                foreach (var property in ct.Properties)
                {
                    // ToList is to avoid modifying the collection during enumeration
                    foreach (var annotation in property.Annotations.OfType<ConcurrencyTokenAnnotation>().ToList())
                    {
                        property.Annotations.Remove(annotation);
                    }
                }
            }

            foreach (var et in model.EntityTypes)
            {
                foreach (var property in et.AllProperties.Where(p => p.PropertyType is ComplexDataType))
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