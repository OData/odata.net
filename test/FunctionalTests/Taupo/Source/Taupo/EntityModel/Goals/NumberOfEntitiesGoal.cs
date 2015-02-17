//---------------------------------------------------------------------
// <copyright file="NumberOfEntitiesGoal.cs" company="Microsoft">
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
    public class NumberOfEntitiesGoal : EntityModelGoal
    {
        /// <summary>
        /// Initializes a new instance of the NumberOfEntitiesGoal class.
        /// </summary>
        /// <param name="minNumberOfEntities">Minimum Number of Entities In this Model</param>
        public NumberOfEntitiesGoal(int minNumberOfEntities)
        {
            this.MinNumberOfEntities = minNumberOfEntities;
        }

        /// <summary>
        /// Gets MinNumberOfEntities per model
        /// </summary>
        public int MinNumberOfEntities { get; private set; }

        /// <summary>
        /// Checks whether goals have Been Met
        /// </summary>
        /// <param name="model">model being improved</param>
        /// <returns>Returns whether goal has been met</returns>
        public override bool HasBeenMet(EntityModelSchema model)
        {
            return model.EntityTypes.Count() >= this.MinNumberOfEntities;
        }

        /// <summary>
        /// Improve the model if goals not yet met
        /// </summary>
        /// <param name="model">model to improve</param>
        public override void Improve(EntityModelSchema model)
        {
            while (model.EntityTypes.Count() < this.MinNumberOfEntities)
            {
                model.Add(new EntityType());
            }
        }
    }
}
