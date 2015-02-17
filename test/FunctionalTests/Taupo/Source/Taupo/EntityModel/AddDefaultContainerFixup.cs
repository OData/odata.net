//---------------------------------------------------------------------
// <copyright file="AddDefaultContainerFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Enricher to add default EntityContainer to an EntitySchemaModel
    /// </summary>
    public class AddDefaultContainerFixup : IEntityModelFixup
    {
        /// <summary>
        /// Initializes a new instance of the AddDefaultContainerFixup class
        /// </summary>
        public AddDefaultContainerFixup()
            : this("DefaultContainer")
        {
        }

        /// <summary>
        /// Initializes a new instance of the AddDefaultContainerFixup class with given default container name
        /// </summary>
        /// <param name="defaultContainerName">the given default container name</param>
        public AddDefaultContainerFixup(string defaultContainerName)
        {
            this.DefaultContainerName = defaultContainerName;
        }

        /// <summary>
        /// Gets or sets the default container name
        /// </summary>
        public string DefaultContainerName { get; set; }

        /// <summary>
        /// Add default EntityContainer to the given EntitySchemaModel
        ///   For each base entity type, add an EntitySet
        ///   For each association, add an AssociationSet, between two possible EntitySets
        /// </summary>
        /// <param name="model">the given EntitySchemaModel</param>
        public void Fixup(EntityModelSchema model)
        {
            EntityContainer container = model.EntityContainers.FirstOrDefault();
            if (container == null)
            {
                container = new EntityContainer(this.DefaultContainerName);
                model.Add(container);
            }

            foreach (EntityType entity in model.EntityTypes.Where(e => e.BaseType == null && !container.EntitySets.Any(set => set.EntityType == e)))
            {
                if (!entity.Annotations.OfType<FixupNoSetAnnotation>().Any())
                {
                    container.Add(new EntitySet(entity.Name, entity));
                }
            }

            foreach (AssociationType association in model.Associations.Where(assoc => !container.AssociationSets.Any(assocSet => assocSet.AssociationType == assoc)))
            {
                if (!association.Annotations.OfType<FixupNoSetAnnotation>().Any())
                {
                    container.Add(new AssociationSet(association.Name, association)
                    {
                        Ends =
                            {
                                new AssociationSetEnd(association.Ends[0], container.EntitySets.First(es => association.Ends[0].EntityType.IsKindOf(es.EntityType))),
                                new AssociationSetEnd(association.Ends[1], container.EntitySets.First(es => association.Ends[1].EntityType.IsKindOf(es.EntityType))),
                            }
                    });
                }
            }
        }
    }
}
