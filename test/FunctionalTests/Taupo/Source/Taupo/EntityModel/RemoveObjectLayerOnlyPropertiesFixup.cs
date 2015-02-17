//---------------------------------------------------------------------
// <copyright file="RemoveObjectLayerOnlyPropertiesFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// A fixup applied to object models that ensures every <see cref="MemberProperty"/>
    /// or <see cref="NavigationProperty"/> with an <see cref="ObjectLayerOnlyAnnotation"/> is removed.
    /// </summary>
    public class RemoveObjectLayerOnlyPropertiesFixup : IEntityModelFixup
    {
        /// <summary>
        /// Performs the fixup to ensure that every <see cref="MemberProperty"/>
        /// or <see cref="NavigationProperty"/> with an <see cref="ObjectLayerOnlyAnnotation"/> is removed.
        /// </summary>
        /// <param name="model">Model to perform fixup on.</param>
        public void Fixup(EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            foreach (var entityType in model.EntityTypes)
            {
                this.RemoveObjectLayerOnlyNavigationProperties(entityType);
                this.RemoveObjectLayerOnlyProperties(entityType);
            }

            foreach (var entityType in model.ComplexTypes)
            {
                this.RemoveObjectLayerOnlyProperties(entityType);
            }
        }

        private void RemoveObjectLayerOnlyNavigationProperties(EntityType entityType)
        {
            for (int i = entityType.NavigationProperties.Count - 1; i >= 0; i--)
            {
                if (entityType.NavigationProperties[i].Annotations.OfType<ObjectLayerOnlyAnnotation>().Any())
                {
                    entityType.NavigationProperties.RemoveAt(i);
                }
            }
        }

        private void RemoveObjectLayerOnlyProperties(StructuralType structuralType)
        {
            for (int i = structuralType.Properties.Count - 1; i >= 0; i--)
            {
                if (structuralType.Properties[i].Annotations.OfType<ObjectLayerOnlyAnnotation>().Any())
                {
                    structuralType.Properties.RemoveAt(i);
                }
            }
        }
    }
}