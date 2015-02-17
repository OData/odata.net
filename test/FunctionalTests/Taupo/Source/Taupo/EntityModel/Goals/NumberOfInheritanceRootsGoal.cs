//---------------------------------------------------------------------
// <copyright file="NumberOfInheritanceRootsGoal.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Goals
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Goal for the minimum number inheritance roots in a model
    /// </summary>
    public class NumberOfInheritanceRootsGoal : EntityModelGoal
    {
        /// <summary>
        /// Initializes a new instance of the NumberOfInheritanceRootsGoal class.
        /// </summary>
        /// <param name="minNumberOfInheritanceRoots">Minimum Number of Entities which are the root of inheritance.</param>        
        public NumberOfInheritanceRootsGoal(int minNumberOfInheritanceRoots)
        {
            this.MinNumberOfInheritanceRoots = minNumberOfInheritanceRoots;
        }

        /// <summary>
        /// Gets the minimum number of entities that are root of inheritance
        /// </summary>
        public int MinNumberOfInheritanceRoots { get; private set; }

        /// <summary>
        /// Checks whether goals have Been Met
        /// </summary>
        /// <param name="model">model being improved</param>
        /// <returns>Returns whether goal has been met</returns>
        public override bool HasBeenMet(EntityModelSchema model)
        {
            var roots = this.GetInheritanceRoots(model);
            return roots.Count() >= this.MinNumberOfInheritanceRoots;
        }

        /// <summary>
        /// Improve the model if goals not yet met
        /// </summary>
        /// <param name="model">model to improve</param>
        public override void Improve(EntityModelSchema model)
        {
            var roots = this.GetInheritanceRoots(model);

            int numberOfRootsToAdd = this.MinNumberOfInheritanceRoots - roots.Count();
            for (int i = 0; i < numberOfRootsToAdd; i++)
            {
                var root = new EntityType();
                model.Add(root);
                model.Add(new EntityType() { BaseType = root });
            }
        }

        private IEnumerable<EntityType> GetInheritanceRoots(EntityModelSchema model)
        {
            return model.EntityTypes.Where(e => e.BaseType == null && model.EntityTypes.Count(d => d.IsKindOf(e)) > 1);
        }
    }
}
