//---------------------------------------------------------------------
// <copyright file="NumberOfPropertiesPerComplexTypeGoal.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Goals
{
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Number Of Entities EntityModelGoal Class
    /// </summary>
    public class NumberOfPropertiesPerComplexTypeGoal : EntityModelGoal
    {
        /// <summary>
        /// Initializes a new instance of the NumberOfPropertiesPerComplexTypeGoal class.
        /// </summary>
        /// <param name="min">Minimum Number of Properties per Complex Type In this Model</param>
        public NumberOfPropertiesPerComplexTypeGoal(int min)
        {
            this.MinNumberOfPropertiesPerComplexType = min;
        }

        /// <summary>
        /// Gets MinNumberOfPropertiesPerComplexType per model
        /// </summary>
        public int MinNumberOfPropertiesPerComplexType { get; private set; }

        /// <summary>
        /// Checks whether goals have Been Met
        /// </summary>
        /// <param name="model">model being improved</param>
        /// <returns>Returns whether goal has been met</returns>
        public override bool HasBeenMet(EntityModelSchema model)
        {
            return model.ComplexTypes.All(e => e.Properties.Count() >= this.MinNumberOfPropertiesPerComplexType);
        }

        /// <summary>
        /// Improve the model if goals not yet met
        /// </summary>
        /// <param name="model">Model to improve</param>
        public override void Improve(EntityModelSchema model)
        {
            foreach (var et in model.ComplexTypes.Where(ct => ct.Properties.Count() < this.MinNumberOfPropertiesPerComplexType))
            {
                while (et.Properties.Count() < this.MinNumberOfPropertiesPerComplexType)
                {
                    et.Properties.Add(new MemberProperty());
                }
            }
        }
    }
}
