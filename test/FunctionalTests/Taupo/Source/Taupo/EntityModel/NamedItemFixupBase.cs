//---------------------------------------------------------------------
// <copyright file="NamedItemFixupBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Base class for the fixup for all items that have Name and Namespace
    /// </summary>
    public abstract class NamedItemFixupBase : IEntityModelFixupWithValidate
    {
        /// <summary>
        /// Performs the fixup for all items that have Name and Namespace.
        /// </summary>
        /// <param name="model">Model to perform fixup on.</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (INamedItem item in this.GetAllNamedItems(model))
            {
                this.FixupNamedItem(item);
            }
        }

        /// <summary>
        /// Checks whether the Validation Rule has been met
        /// </summary>
        /// <param name="model">Model being checked</param>
        /// <returns>
        /// Returns whether validation rule has been met
        /// </returns>
        public bool IsModelValid(EntityModelSchema model)
        {
            return this.GetAllNamedItems(model).All(c => this.IsNamedItemValid(c));
        }

        /// <summary>
        /// Perform the fix up on the item that has Name and Namespace
        /// </summary>
        /// <param name="item">The item to be fixed up</param>
        protected abstract void FixupNamedItem(INamedItem item);

        /// <summary>
        /// Check if the item that has Name and Namespace is valid
        /// </summary>
        /// <param name="item">The item to be checked</param>
        /// <returns>true if the item is valid, false otherwise.</returns>
        protected abstract bool IsNamedItemValid(INamedItem item);

        private IEnumerable<INamedItem> GetAllNamedItems(EntityModelSchema model)
        {
            return model.EntityTypes.Cast<INamedItem>()
                .Concat(model.ComplexTypes.Cast<INamedItem>())
                .Concat(model.Associations.Cast<INamedItem>())
                .Concat(model.Functions.Cast<INamedItem>())
                .Concat(model.EnumTypes.Cast<INamedItem>())
                .Concat(model.EntityContainers.Cast<INamedItem>());
        }
    }
}
