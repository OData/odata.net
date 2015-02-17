//---------------------------------------------------------------------
// <copyright file="ReplaceBinaryKeysFixup.cs" company="Microsoft">
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
    /// Entity model fixup for replacing binary key properties with another primitive type while maintaining nullability
    /// </summary>
    public class ReplaceBinaryKeysFixup : IEntityModelFixup
    {
        private PrimitiveDataType replacementType;

        /// <summary>
        /// Initializes a new instance of the ReplaceBinaryKeysFixup class that replaces binary keys with 32-bit integers
        /// </summary>
        public ReplaceBinaryKeysFixup()
            : this(DataTypes.Integer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ReplaceBinaryKeysFixup class that uses the given replacement type
        /// </summary>
        /// <param name="replacementType">The type to replace binary key properties with</param>
        public ReplaceBinaryKeysFixup(PrimitiveDataType replacementType)
        {
            ExceptionUtilities.CheckArgumentNotNull(replacementType, "replacementType");
            this.replacementType = replacementType;
        }

        /// <summary>
        /// Replace binary key properties with properties of the replacement type while maintaining nullability
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            List<MemberProperty> modifiedProperties = new List<MemberProperty>();
            foreach (var et in model.EntityTypes)
            {
                foreach (var prop in et.AllKeyProperties)
                {
                    if (prop.PropertyType is BinaryDataType)
                    {
                        modifiedProperties.Add(prop);
                        this.Fixup(prop);
                    }
                }
            }

            if (modifiedProperties.Count > 0)
            {
                var constraints = model.Associations
                    .Select(a => a.ReferentialConstraint)
                    .Where(c => c != null)
                    .Where(c => c.PrincipalProperties.Any(p => modifiedProperties.Contains(p)));

                foreach (var constraint in constraints)
                {
                    foreach (var prop in constraint.DependentProperties)
                    {
                        if (prop.PropertyType is BinaryDataType)
                        {
                            modifiedProperties.Add(prop);
                            this.Fixup(prop);
                        }
                    }
                }
            }
        }

        private void Fixup(MemberProperty property)
        {
            property.PropertyType = this.replacementType.Nullable(property.PropertyType.IsNullable);
        }
    }
}