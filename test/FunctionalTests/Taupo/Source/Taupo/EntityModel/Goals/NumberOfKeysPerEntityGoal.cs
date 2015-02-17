//---------------------------------------------------------------------
// <copyright file="NumberOfKeysPerEntityGoal.cs" company="Microsoft">
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
    /// NumberOfKeysPerEntityGoal class
    /// </summary>
    public class NumberOfKeysPerEntityGoal : EntityModelGoal
    {
        /// <summary>
        /// Initializes a new instance of the NumberOfKeysPerEntityGoal class
        /// </summary>
        /// <param name="min">Min Number Of Entities</param>
        public NumberOfKeysPerEntityGoal(int min)
        {            
            this.MinNumberOfKeysPerEntities = min;
            this.MaxNumberOfKeysPerEntities = min;
        }

        /// <summary>
        /// Initializes a new instance of the NumberOfKeysPerEntityGoal class.
        /// </summary>
        /// <param name="min">Minimum Number Of Keys Per Entities</param>
        /// <param name="max">Max Number Of Keys Per Entities</param>
        /// <param name="random">Random Number Generator</param>
        public NumberOfKeysPerEntityGoal(int min, int max, IRandomNumberGenerator random)
        {
            this.Random = random;
            this.MinNumberOfKeysPerEntities = min;
            this.MaxNumberOfKeysPerEntities = max;
        }

        /// <summary>
        /// Gets Random Number Generator
        /// </summary>
        public IRandomNumberGenerator Random { get; private set; }

        /// <summary>
        /// Gets Max Number Of Keys Per Entities
        /// </summary>
        public int MaxNumberOfKeysPerEntities { get; private set; }

        /// <summary>
        /// Gets Min Number Of Keys Per Entities
        /// </summary>
        public int MinNumberOfKeysPerEntities { get; private set; }

        /// <summary>
        /// Checks whether goal has been met
        /// </summary>
        /// <param name="model">Model to enrich</param>
        /// <returns>Returns whether goal has been met</returns>
        public override bool HasBeenMet(EntityModelSchema model)
        {
            return model.EntityTypes
                .Where(c => c.BaseType == null)
                .All(p => p.AllKeyProperties.Count() >= this.MinNumberOfKeysPerEntities);
        }

        /// <summary>
        /// Improve the model if goal not yet met
        /// </summary>
        /// <param name="model">Model to improve</param>
        public override void Improve(EntityModelSchema model)
        {
            foreach (var et in model.EntityTypes
                .Where(c => c.BaseType == null)
                .Where(c => c.AllKeyProperties.Count() < this.MinNumberOfKeysPerEntities))
            {
                int numOfKeyPropertiesNeeded = this.MinNumberOfKeysPerEntities;
                if (this.MinNumberOfKeysPerEntities != this.MaxNumberOfKeysPerEntities)
                {
                    numOfKeyPropertiesNeeded = this.Random.NextFromRange(this.MinNumberOfKeysPerEntities, this.MaxNumberOfKeysPerEntities);
                }

                // Consume the existing properties...
                foreach (MemberProperty mp in et.Properties.Where(p => p.IsPrimaryKey == false).SkipWhile(q => q.PropertyType is ComplexDataType))
                {
                    if (et.AllKeyProperties.Count() < numOfKeyPropertiesNeeded)
                    {
                        mp.IsPrimaryKey = true;
                    }
                    else
                    {
                        break;
                    }
                }

                // Create new properties and make them key...
                while (et.AllKeyProperties.Count() < numOfKeyPropertiesNeeded)
                {
                    et.Properties.Add(new MemberProperty() 
                    { 
                        IsPrimaryKey = true 
                    });
                }
            }
        }
    }
}
