//---------------------------------------------------------------------
// <copyright file="NumberOfComplexPropertiesPerEntityGoal.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Goals
{
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// NumberOfComplexPropertiesPerEntityGoal class
    /// </summary>
    public class NumberOfComplexPropertiesPerEntityGoal : EntityModelGoal
    {
        /// <summary>
        /// Initializes a new instance of the NumberOfComplexPropertiesPerEntityGoal class.
        /// </summary>
        /// <param name="min">The min num of Complex Prop Per Entity.</param>
        public NumberOfComplexPropertiesPerEntityGoal(int min)
        {
            this.MinNumberOfComplexPropertiesPerEntity = min;
        }

        /// <summary>
        /// Initializes a new instance of the NumberOfComplexPropertiesPerEntityGoal class
        /// </summary>
        /// <param name="min">min number of complex properties per entity</param>
        /// <param name="max">max number of complex properties per entity</param>
        /// <param name="random">Random Number</param>
        public NumberOfComplexPropertiesPerEntityGoal(int min, int max, IRandomNumberGenerator random)
        {
            this.RandomNumberGenerator = random;
            this.MinNumberOfComplexPropertiesPerEntity = min;
            this.MaxNumberOfComplexPropertiesPerEntity = max;
        }

        /// <summary>
        /// Initializes a new instance of the NumberOfComplexPropertiesPerEntityGoal class
        /// </summary>
        /// <param name="min">Number of complex properties per entity</param>
        /// <param name="random">Random Number Generator</param>
        public NumberOfComplexPropertiesPerEntityGoal(int min, IRandomNumberGenerator random)
        {
            this.RandomNumberGenerator = random;
            this.MinNumberOfComplexPropertiesPerEntity = min;
            this.MaxNumberOfComplexPropertiesPerEntity = min;
        }

        /// <summary>
        /// Gets RandomNumberGenerator
        /// </summary>
        public IRandomNumberGenerator RandomNumberGenerator { get; private set; }

        /// <summary>
        /// Gets Minimum Number of Complex Properties Per Entity
        /// </summary>
        public int MinNumberOfComplexPropertiesPerEntity { get; private set; }

        /// <summary>
        /// Gets Maximum Number of Complex Properties Per Entity
        /// </summary>
        public int MaxNumberOfComplexPropertiesPerEntity { get; private set; }

        /// <summary>
        /// Checks whether goals have Been Met
        /// </summary>
        /// <param name="model">Schema being improved</param>
        /// <returns>Returns whether goals have Been Met</returns>
        public override bool HasBeenMet(EntityModelSchema model)
        {
            return model.EntityTypes
                .All(c => c.Properties.Where(p => p.PropertyType is ComplexDataType).Count() >= this.MinNumberOfComplexPropertiesPerEntity);
        }

        /// <summary>
        /// Improve the model if goal not yet met
        /// </summary>
        /// <param name="model">Model to improve</param>
        public override void Improve(EntityModelSchema model)
        {
            foreach (var et in model.EntityTypes)
            {
                var existingComplexProperties = et.Properties.Where(p => p.PropertyType is ComplexDataType).ToList();

                if (existingComplexProperties.Count >= this.MinNumberOfComplexPropertiesPerEntity)
                {
                    continue;
                }

                int numOfComplexPropertiesPerEntity = this.MinNumberOfComplexPropertiesPerEntity;
                if (this.RandomNumberGenerator != null)
                {
                    numOfComplexPropertiesPerEntity = this.RandomNumberGenerator.NextFromRange(this.MinNumberOfComplexPropertiesPerEntity, this.MaxNumberOfComplexPropertiesPerEntity);
                }

                int remaining = numOfComplexPropertiesPerEntity - existingComplexProperties.Count;

                for (int i = 0; i < remaining; ++i)
                {
                    et.Properties.Add(new MemberProperty()
                    {
                        PropertyType = DataTypes.ComplexType,
                    });
                }
            }
        }
    }
}
