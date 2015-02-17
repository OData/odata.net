//---------------------------------------------------------------------
// <copyright file="IEntityModelObjectServices.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Provides object services for entity model data generation, modification, lookup.
    /// </summary>
    public interface IEntityModelObjectServices
    {
        /// <summary>
        /// Gets an <see cref="IEntityGraphCreator"/> that can create graphs of entities based on current metadata.
        /// </summary>
        /// <param name="containerName">The name of the entity container for which to retrieve an <see cref="IEntityGraphCreator"/>.</param>
        /// <returns>An <see cref="IEntityGraphCreator"/> based on current metadata.</returns>
        IEntityGraphCreator GetEntityGraphCreator(string containerName);

        /// <summary>
        /// Gets data adapter to store/modify/look-up structural data in form of object.
        /// </summary>
        /// <param name="structuralTypeFullName">A structural type full name.</param>
        /// <returns>Object data adapter for the specified <see cref="StructuralType"/>.</returns>
        IStructuralDataAdapter GetObjectAdapter(string structuralTypeFullName);
        
        /// <summary>
        /// Gets a data generator for the specified <see cref="EntityType"/> and entity set name.
        /// </summary>
        /// <param name="entityTypeFullName">An entity type full name.</param>
        /// <param name="entitySetName">A name of the entity set for which object needs to be generated.</param>
        /// <returns>A generator that generates entity set object.</returns>
        IDataGenerator<IEntitySetData> GetEntitySetObjectGenerator(string entityTypeFullName, string entitySetName);

        /// <summary>
        /// Gets object generator for the specified <see cref="ComplexType"/>.
        /// </summary>
        /// <param name="complexTypeFullName">A complex type full name.</param>
        /// <returns>An <see cref="IDataGenerator"/> that generates object for for the specified complex type.</returns>
        IDataGenerator GetObjectGenerator(string complexTypeFullName);

        /// <summary>
        /// Generates values for the properties with the specified paths.
        /// </summary>
        /// <param name="entityTypeFullName">An entity type full name.</param>
        /// <param name="entitySetName">A name of the entity set for which data needs to be generated.</param>
        /// <param name="propertyPaths">The property paths for which to generate values.</param>
        /// <param name="valuesToAvoid">The collection of named values where name is a property path and value is a property value which should be avoided when generating value for this property.</param>
        /// <returns>Generated properties' values.</returns>
        /// <example>Modify an entity:
        /// var currentValues = objectServices.GetPropertiesValues(entity, entityType);
        /// var newValues = objectServices.GeneratePropertyValues(entityType.FullName, entitySet.ContainerQualifiedName, updatableProperties, currentValues);
        /// this.GetObjectAdapter(entityType.FullName).SetMemberValues(entity, newValues);
        /// </example>
        IEnumerable<NamedValue> GeneratePropertyValues(string entityTypeFullName, string entitySetName, IEnumerable<string> propertyPaths, IEnumerable<NamedValue> valuesToAvoid);
    }
}
