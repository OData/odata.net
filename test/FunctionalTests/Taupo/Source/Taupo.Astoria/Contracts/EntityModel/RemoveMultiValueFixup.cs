//---------------------------------------------------------------------
// <copyright file="RemoveMultiValueFixup.cs" company="Microsoft">
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
    /// Entity model fixup for replacing type of bag property with non-bag element type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
    public class RemoveMultiValueFixup : IEntityModelFixup
    {
        /// <summary>
        /// Initializes a new instance of the RemoveMultiValueFixup class that replace bags with non-bag property in the model
        /// </summary>
        public RemoveMultiValueFixup()
        {
        }

        /// <summary>
        /// Remove bags and corresponding features in model for EF provider
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (var namedStructuralType in model.ComplexTypes.OfType<NamedStructuralType>().Concat(model.EntityTypes.OfType<NamedStructuralType>()))
            {
                foreach (var bag in namedStructuralType.Properties.Where(p => p.PropertyType.HasCollection())) 
                {
                    var collectionType = bag.PropertyType as CollectionDataType;
                    bag.PropertyType = collectionType.ElementDataType;
                }
            }

            foreach (var function in model.Functions.ToArray().Where(f => f.ReturnType != null && f.ReturnType.HasCollection()))
            {
                model.Remove(function);
            }

            foreach (var function in model.Functions.ToArray().Where(f => f.Parameters.Any(p => p.DataType.HasCollection())))
            {
                model.Remove(function);
            }
        }
    }
}