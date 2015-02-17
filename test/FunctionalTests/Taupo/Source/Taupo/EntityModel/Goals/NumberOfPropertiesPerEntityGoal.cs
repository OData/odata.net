//---------------------------------------------------------------------
// <copyright file="NumberOfPropertiesPerEntityGoal.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Goals
{
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Number Of Properties Per Entity EntityModelGoal Class
    /// </summary>
    public class NumberOfPropertiesPerEntityGoal : EntityModelGoal
    {
        /// <summary>
        /// Initializes a new instance of the NumberOfPropertiesPerEntityGoal class.
        /// </summary>
        /// <param name="min">Minimum Number of Properties per Entity In this Model</param>
        public NumberOfPropertiesPerEntityGoal(int min)
        {
            this.MinNumberOfPropertiesPerEntity = min;
        }

        /// <summary>
        /// Gets MinNumberOfPropertiesPerEntity per model
        /// </summary>
        public int MinNumberOfPropertiesPerEntity { get; private set; }

        /// <summary>
        /// Checks whether goals have Been Met
        /// </summary>
        /// <param name="model">model being improved</param>
        /// <returns>Returns whether goal has been met</returns>
        public override bool HasBeenMet(EntityModelSchema model)
        {
            return model.EntityTypes.All(e => e.Properties.Count() >= this.MinNumberOfPropertiesPerEntity);
        }

        /// <summary>
        /// Improve the model if goals not yet met
        /// </summary>
        /// <param name="model">model to improve</param>
        public override void Improve(EntityModelSchema model)
        {
            foreach (var et in model.EntityTypes.Where(e => e.Properties.Count() < this.MinNumberOfPropertiesPerEntity))
            {
                while (et.Properties.Count() < this.MinNumberOfPropertiesPerEntity)
                {
                    et.Properties.Add(new MemberProperty());
                }
            }
        }
    }
}
