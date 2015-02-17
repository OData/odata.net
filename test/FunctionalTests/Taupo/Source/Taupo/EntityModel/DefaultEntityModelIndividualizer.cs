//---------------------------------------------------------------------
// <copyright file="DefaultEntityModelIndividualizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Modifies names of all sets (entity sets and association sets) in a model by appending common
    /// suffix.
    /// </summary>
    [ImplementationName(typeof(IEntityModelIndividualizer), "Default")]
    public class DefaultEntityModelIndividualizer : IEntityModelIndividualizer
    {
        /// <summary>
        /// Initializes a new instance of the DefaultEntityModelIndividualizer class.
        /// </summary>
        public DefaultEntityModelIndividualizer()
        {
            this.Suffix = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Gets or sets the suffix to append to all names.
        /// </summary>
        /// <value>The suffix.</value>
        public string Suffix { get; set; }

        /// <summary>
        /// Individualizes the specified storage schema.
        /// </summary>
        /// <param name="storageSchema">The storage schema.</param>
        public void Individualize(EntityModelSchema storageSchema)
        {
            foreach (EntityContainer container in storageSchema.EntityContainers)
            {
                container.Name = this.Individualize(container.Name);

                foreach (EntitySet entitySet in container.EntitySets)
                {
                    entitySet.Name = this.Individualize(entitySet.Name);
                }

                foreach (AssociationSet associationSet in container.AssociationSets)
                {
                    associationSet.Name = this.Individualize(associationSet.Name);
                }
            }

            foreach (var function in storageSchema.Functions)
            {
                function.Name = this.Individualize(function.Name);
            }
        }

        private string Individualize(string text)
        {
            if (text == null)
            {
                return null;
            }

            if (!text.EndsWith(this.Suffix, StringComparison.Ordinal))
            {
                return text + this.Suffix;
            }
            else
            {
                return text;
            }
        }
    }
}
