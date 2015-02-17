//---------------------------------------------------------------------
// <copyright file="NumberOfComplexTypesGoal.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Goals
{
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;    

    /// <summary>
    /// NumberOfComplexTypesGoal Class
    /// </summary>
    public class NumberOfComplexTypesGoal : EntityModelGoal
    {
        /// <summary>
        /// Initializes a new instance of the NumberOfComplexTypesGoal class.
        /// </summary>
        /// <param name="min">Minimum Number of ComplexTypes In this Model</param>
        public NumberOfComplexTypesGoal(int min)
        {
            this.MinNumberOfComplexTypes = min;
        }

        /// <summary>
        /// Gets MinNumberOfComplexTypes per model
        /// </summary>
        public int MinNumberOfComplexTypes { get; private set; }

        /// <summary>
        /// Checks whether goals have Been Met
        /// </summary>
        /// <param name="model">Model being improved</param>
        /// <returns>Returns whether goal has been met</returns>
        public override bool HasBeenMet(EntityModelSchema model)
        {
            return model.ComplexTypes.Count() >= this.MinNumberOfComplexTypes;
        }

        /// <summary>
        /// Improve the model if goals not yet met
        /// </summary>
        /// <param name="model">Model to improve</param>
        public override void Improve(EntityModelSchema model)
        {
            while (model.ComplexTypes.Count() < this.MinNumberOfComplexTypes)
            {
                model.Add(new ComplexType());
            }
        }
    }
}
