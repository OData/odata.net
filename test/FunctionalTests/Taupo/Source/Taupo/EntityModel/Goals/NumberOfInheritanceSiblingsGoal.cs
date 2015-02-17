//---------------------------------------------------------------------
// <copyright file="NumberOfInheritanceSiblingsGoal.cs" company="Microsoft">
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
    /// Goal for the minimum number inheritance siblings in a model
    /// </summary>
    public class NumberOfInheritanceSiblingsGoal : EntityModelGoal
    {
        /// <summary>
        /// Initializes a new instance of the NumberOfInheritanceSiblingsGoal class.
        /// </summary>
        /// <param name="minNumberOfInheritanceSiblings">Minimum Number of siblings in an inheritance level</param>
        public NumberOfInheritanceSiblingsGoal(int minNumberOfInheritanceSiblings)
        {
            this.MinNumberOfInheritanceSiblings = minNumberOfInheritanceSiblings;
        }

        /// <summary>
        /// Gets the minimum number of inheritance siblings in a level
        /// </summary>
        public int MinNumberOfInheritanceSiblings { get; private set; }

        /// <summary>
        /// Checks whether goals have Been Met
        /// </summary>
        /// <param name="model">model being improved</param>
        /// <returns>Returns whether goal has been met</returns>
        public override bool HasBeenMet(EntityModelSchema model)
        {
            var baseTypes = model.EntityTypes.Where(e => model.EntityTypes.Any(d => d.BaseType == e));

            if (baseTypes.Any())
            {
                return baseTypes.All(b => model.EntityTypes.Count(d => d.BaseType == b) >= this.MinNumberOfInheritanceSiblings);
            }

            // if no inheritance at all, also consider it done
            return true;
        }

        /// <summary>
        /// Improve the model if goals not yet met
        /// </summary>
        /// <param name="model">model to improve</param>
        public override void Improve(EntityModelSchema model)
        {
            var baseTypes = model.EntityTypes.Where(e => model.EntityTypes.Any(d => d.BaseType == e)).ToList();

            foreach (var baseType in baseTypes)
            {
                int numberOfSiblingsToAdd = this.MinNumberOfInheritanceSiblings - model.EntityTypes.Count(d => d.BaseType == baseType);

                for (int i = 0; i < numberOfSiblingsToAdd; i++)
                {
                    var derivedType = new EntityType() { BaseType = baseType };
                    model.Add(derivedType);
                }
            }
        }
    }
}
