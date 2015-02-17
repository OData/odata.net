//---------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.EntityModel.Goals;    

    /// <summary>
    /// Extensions method for MOdel Generation
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// NumberOfEntities Extension method
        /// </summary>
        /// <param name="gen">Model Generator</param>        
        /// <param name="minEntities">Minimum number of entities per model</param>
        /// <returns>Returns Model Generator</returns>
        public static ModelGenerator NumberOfEntities(this ModelGenerator gen, int minEntities)
        {
            gen.SetGoal(new NumberOfEntitiesGoal(minEntities));
            return gen;
        }

        /// <summary>
        /// NumberOfKeyPropertiesPerEntity Extension Method
        /// </summary>
        /// <param name="gen">Model Generator</param>
        /// <param name="minKeyPropertiesPerEntity">Number of key properties per entity</param>
        /// <returns>Returns Model Generator</returns>
        public static ModelGenerator NumberOfKeyPropertiesPerEntity(this ModelGenerator gen, int minKeyPropertiesPerEntity)
        {
            gen.SetGoal(new NumberOfKeysPerEntityGoal(minKeyPropertiesPerEntity));
            return gen;
        }

        /// <summary>
        /// Numbers the of properties per complex types.
        /// </summary>
        /// <param name="gen">The Model Generator.</param>
        /// <param name="minPropertiesPerComplexType">Type of the min properties per complex.</param>
        /// <returns>Model Generator</returns>
        public static ModelGenerator NumberOfPropertiesPerComplexTypes(this ModelGenerator gen, int minPropertiesPerComplexType)
        {
            gen.SetGoal(new NumberOfPropertiesPerComplexTypeGoal(minPropertiesPerComplexType));
            return gen;
        }

        /// <summary>
        /// NumberOfKeyPropertiesPerEntity Extension Method
        /// </summary>
        /// <param name="gen">Model Generator</param>
        /// <param name="maxKeysPerEntity">Max Number of key properties per entity</param>
        /// <param name="minKeysPerEntity">Min Number of key properties per entity</param>
        /// <param name="random">Random Number</param>
        /// <returns>Returns Model Generator</returns>
        public static ModelGenerator NumberOfKeyPropertiesPerEntity(this ModelGenerator gen, int maxKeysPerEntity, int minKeysPerEntity, IRandomNumberGenerator random)
        {
            gen.SetGoal(new NumberOfKeysPerEntityGoal(maxKeysPerEntity, minKeysPerEntity, random));
            return gen;
        }

        /// <summary>
        /// Numbers the of complex types.
        /// </summary>
        /// <param name="gen">Model Generator</param>
        /// <param name="minComplexTypesInModel">The min complex types in model.</param>
        /// <returns>Returns Model Generator</returns>
        public static ModelGenerator NumberOfComplexTypes(this ModelGenerator gen, int minComplexTypesInModel)
        {
            gen.SetGoal(new NumberOfComplexTypesGoal(minComplexTypesInModel));
            return gen;
        }

        /// <summary>
        /// NumberOfComplexPropertiesPerEntityGoal Extension Method
        /// </summary>
        /// <param name="gen">Model Generator</param>
        /// <param name="min">Minimum number of complex properties per entity</param>
        /// <param name="max">Maximum number of complex properties per entity</param>
        /// <param name="random">Random number generator</param>
        /// <returns>Returns Model Generator</returns>
        public static ModelGenerator NumberOfComplexPropertiesPerEntity(this ModelGenerator gen, int min, int max, IRandomNumberGenerator random)
        {
            gen.SetGoal(new NumberOfComplexPropertiesPerEntityGoal(min, max, random));
            return gen;
        }

        /// <summary>
        /// Number of the inheritance roots
        /// </summary>
        /// <param name="gen">Model generator</param>
        /// <param name="minNumberOfRoots">Minimum number of inheritance roots.</param>
        /// <returns>Returns Model Generator</returns>
        public static ModelGenerator NumberOfInheritanceRoots(this ModelGenerator gen, int minNumberOfRoots)
        {
            gen.SetGoal(new NumberOfInheritanceRootsGoal(minNumberOfRoots));
            return gen;
        }

        /// <summary>
        /// Number of the inheritance levels
        /// </summary>
        /// <param name="gen">Model generator</param>
        /// <param name="minNumberOfLevels">Minimum number of inheritance levels.</param>
        /// <returns>Returns Model Generator</returns>
        public static ModelGenerator NumberOfInheritanceLevels(this ModelGenerator gen, int minNumberOfLevels)
        {
            gen.SetGoal(new NumberOfInheritanceLevelsGoal(minNumberOfLevels));
            return gen;
        }

        /// <summary>
        /// Number of the inheritance siblings
        /// </summary>
        /// <param name="gen">Model generator</param>
        /// <param name="minNumberOfSiblings">Minimum number of inheritance siblings.</param>
        /// <returns>Returns Model Generator</returns>
        public static ModelGenerator NumberOfInheritanceSiblings(this ModelGenerator gen, int minNumberOfSiblings)
        {
            gen.SetGoal(new NumberOfInheritanceSiblingsGoal(minNumberOfSiblings));
            return gen;
        }
    }
}
