//---------------------------------------------------------------------
// <copyright file="AddMissingTypesFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Provides a usable property type for <see cref="MemberProperty"/> objects that don't specify it.
    /// </summary>
    public class AddMissingTypesFixup : IEntityModelFixupWithValidate
    {
        private IRandomNumberGenerator random;
        private DataType[] types;

        /// <summary>
        /// Initializes a new instance of the AddMissingTypesFixup class.
        /// </summary>
        /// <param name="randomNumberGenerator">The random number generator.</param>
        /// <param name="types">The types to use.</param>
        public AddMissingTypesFixup(IRandomNumberGenerator randomNumberGenerator, params DataType[] types)
        {
            this.random = randomNumberGenerator;
            this.types = types;
        }

        /// <summary>
        /// Provides default property types for <see cref="MemberProperty"/> objects that don't specify it.
        /// </summary>
        /// <param name="model">Model to fixup.</param>
        /// <remarks>For now the class always uses Int32 type, but this may change in the future.</remarks>
        public void Fixup(EntityModelSchema model)
        {
            this.FixupEntityTypes(model.EntityTypes);
            this.FixupComplexTypes(model.ComplexTypes);
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
            return model.EntityTypes.All(t => t.Properties.All(prop => prop.PropertyType != null))
                && model.ComplexTypes.All(t => t.Properties.All(prop => prop.PropertyType != null));
        }

        private void FixupComplexTypes(IEnumerable<ComplexType> complexTypes)
        {
            foreach (ComplexType type in complexTypes)
            {
                this.AddMissingPropertyTypes(type.Properties);
            }
        }

        private void FixupEntityTypes(IEnumerable<EntityType> entityTypes)
        {
            foreach (EntityType type in entityTypes)
            {
                this.AddMissingPropertyTypes(type.Properties);
            }
        }

        private void AddMissingPropertyTypes(IEnumerable<MemberProperty> properties)
        {
            foreach (MemberProperty prop in properties)
            {
                if (prop.PropertyType == null)
                {
                    prop.PropertyType = this.random.ChooseFrom(this.types);
                }
            }
        }
    }
}
