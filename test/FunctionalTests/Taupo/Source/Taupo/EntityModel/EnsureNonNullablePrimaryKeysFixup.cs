//---------------------------------------------------------------------
// <copyright file="EnsureNonNullablePrimaryKeysFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// A fixup applied to models that ensures every primary key property
    /// in an <see cref="EntityType"/> is made non-nullable.
    /// </summary>
    public sealed class EnsureNonNullablePrimaryKeysFixup : IEntityModelFixup
    {
        /// <summary>
        /// Performs the fixup to ensure every primary key property
        /// is made non-nullable for the model.
        /// </summary>
        /// <param name="model">Model to perform fixup on.</param>
        public void Fixup(EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            foreach (var keyProperty in model.EntityTypes.SelectMany(et => et.AllKeyProperties))
            {
                // All key properties must be non-nullable even if they are nullable in O-space.
                keyProperty.PropertyType = ((PrimitiveDataType)keyProperty.PropertyType).NotNullable();
            }
        }
    }
}
