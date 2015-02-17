//---------------------------------------------------------------------
// <copyright file="NumberOfInheritanceLevelsGoal.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Goals
{
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Goal for the minimum number inheritance levels in a model
    /// </summary>
    public class NumberOfInheritanceLevelsGoal : EntityModelGoal
    {
        /// <summary>
        /// Initializes a new instance of the NumberOfInheritanceLevelsGoal class.
        /// </summary>
        /// <param name="minNumberOfInheritanceLevels">Minimum Number of inheritance levels.</param>
        public NumberOfInheritanceLevelsGoal(int minNumberOfInheritanceLevels)
        {
            this.MinNumberOfInheritanceLevels = minNumberOfInheritanceLevels;
        }

        /// <summary>
        /// Gets the minimum number of inheritance levels
        /// </summary>
        public int MinNumberOfInheritanceLevels { get; private set; }

        /// <summary>
        /// Checks whether goals have Been Met
        /// </summary>
        /// <param name="model">model being improved</param>
        /// <returns>Returns whether goal has been met</returns>
        public override bool HasBeenMet(EntityModelSchema model)
        {
            return model.EntityTypes.Any(e => this.GetNumberOfInheritanceLevel(model, e) >= this.MinNumberOfInheritanceLevels);
        }

        /// <summary>
        /// Improve the model if goals not yet met
        /// </summary>
        /// <param name="model">model to improve</param>
        public override void Improve(EntityModelSchema model)
        {
            var baseType = new EntityType();
            model.Add(baseType);

            for (int i = 0; i < this.MinNumberOfInheritanceLevels; i++)
            {
                var derivedType = new EntityType() { BaseType = baseType };
                model.Add(derivedType);

                baseType = derivedType;
            }        
        }

        private int GetNumberOfInheritanceLevel(EntityModelSchema model, EntityType entityType)
        {
            var children = model.EntityTypes.Where(e => e.BaseType == entityType);

            if (children.Any())
            {
                return 1 + children.Max(e => this.GetNumberOfInheritanceLevel(model, e));
            }

            return 0;
        }
    }
}
