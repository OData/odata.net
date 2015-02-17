//---------------------------------------------------------------------
// <copyright file="AddNavigationPropertyFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Enricher to add Navigation properties to an EntitySchemaModel
    /// </summary>
    public class AddNavigationPropertyFixup : IEntityModelFixup
    {
        private IIdentifierGenerator identifierGenerator;

        /// <summary>
        /// Initializes a new instance of the AddNavigationPropertyFixup class object.
        /// </summary>
        /// <param name="identifierGen">Identifier generator to use.</param>
        public AddNavigationPropertyFixup(IIdentifierGenerator identifierGen)
        {
            this.identifierGenerator = identifierGen;
        }

        /// <summary>
        /// Add Navigation properties to an EntitySchemaModel
        /// </summary>
        /// <param name="model">Model to perform fixup on.</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (AssociationType association in model.Associations)
            {
                foreach (AssociationEnd end in association.Ends)
                {
                    if (!end.Annotations.OfType<FixupNoNavigationAnnotation>().Any())
                    {
                        if (!end.EntityType.NavigationProperties.Any(np => np.Association == association && np.FromAssociationEnd == end))
                        {
                            AssociationEnd otherEnd = association.Ends.Single(e => e != end);
                            end.EntityType.NavigationProperties.Add(new NavigationProperty()
                            {
                                Name = this.identifierGenerator.GenerateIdentifier(otherEnd.RoleName),
                                Association = association,
                                FromAssociationEnd = end,
                                ToAssociationEnd = otherEnd,
                            });
                        }
                    }
                }
            }
        }
    }
}
