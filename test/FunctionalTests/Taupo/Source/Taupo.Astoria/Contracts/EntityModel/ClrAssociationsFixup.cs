//---------------------------------------------------------------------
// <copyright file="ClrAssociationsFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Fixup for changing the model's associations to be consistent with the CLR semantics used in Astoria providers
    /// </summary>
    public class ClrAssociationsFixup : IEntityModelFixup
    {
        /// <summary>
        /// Changes the all association ends with multiplicity one to have multiplicity zero-one and removes all referential constraints.
        /// </summary>
        /// <param name="model">The model to fixup</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (var association in model.Associations)
            {
                if (association.ReferentialConstraint != null)
                {
                    association.ReferentialConstraint = null;
                }

                foreach (var end in association.Ends.Where(e => e.Multiplicity == EndMultiplicity.One))
                {
                    end.Multiplicity = EndMultiplicity.ZeroOne;
                    end.DeleteBehavior = OperationAction.None;
                }
            }
        }
    }
}