//---------------------------------------------------------------------
// <copyright file="RemoveUnsupportedFeaturesForCodeFirstFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Temporary entity model fixup for remove features which is not supported in astoria code-first test due to known product bugs.
    /// TODO: This fixup needs to be removed eventually after all bugs are fixed.
    /// </summary>
    public class RemoveUnsupportedFeaturesForCodeFirstFixup : IEntityModelFixup
    {
        /// <summary>
        /// Initializes a new instance of the RemoveUnsupportedFeaturesForCodeFirstFixup class.
        /// </summary>
        public RemoveUnsupportedFeaturesForCodeFirstFixup()
        {
        }

        /// <summary>
        /// Remove unsupported features in model for EF-CodeFirst provider.
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            // Creating Database using DbContext.Database.Create() results in sql exception indicating mulitple cascade paths.
            // The following two associations are removed from Astoria default model.
            // "Login_SentMessages" and "Login_ReceivedMessages"
            foreach (var type in model.EntityTypes)
            {
                var otherEndTypes = type.ToEndAssociations().Select(e => e.EntityType);
                var duplicateTypes = otherEndTypes.Where(t => otherEndTypes.Where(e => e == t).Count() > 1).Distinct();

                foreach (var association in model.Associations.Where(a => a.Ends.Any(e => e.EntityType == type))
                    .Where(a => a.Ends.Any(e => duplicateTypes.Contains(e.EntityType))).ToList())
                {
                    this.RemoveAssociation(association);
                }
            }
        }

        private void RemoveAssociation(AssociationType association)
        {
            foreach (var end in association.Ends.ToList())
            {
                var property = end.FromNavigationProperty();
                if (property != null)
                {
                    end.EntityType.NavigationProperties.Remove(property);
                }
            }

            var model = association.Model;
            model.Remove(association);
            var defaultContainer = model.EntityContainers.First();
            var set = defaultContainer.AssociationSets.SingleOrDefault(s => s.AssociationType.Name == association.Name);
            if (set != null)
            {
                defaultContainer.Remove(set);
            }
        } 
    }
}