//---------------------------------------------------------------------
// <copyright file="EntityModelDataServicesBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Base class for the entity model data services.
    /// </summary>
    internal abstract class EntityModelDataServicesBase
    {
        private Dictionary<string, List<string>> entityTypeToEntitySetMap = new Dictionary<string, List<string>>();

        /// <summary>
        /// Determines the key to use for looking-up a data service for the given entity type and set.
        /// </summary>
        /// <param name="entityTypeFullName">Full name of the entity type.</param>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <returns>The key to use for looking-up a data service for the specified entity type and set .</returns>
        protected string DetermineKey(string entityTypeFullName, string entitySetName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(entityTypeFullName, "entityTypeFullName");

            if (!string.IsNullOrEmpty(entitySetName))
            {
                return this.CreateKey(entityTypeFullName, entitySetName);
            }

            // If entity set name is not specified try to look-up in the entityTypeToEntitySetMap.
            List<string> entitySetDataGenKeys;
            if (this.entityTypeToEntitySetMap.TryGetValue(entityTypeFullName, out entitySetDataGenKeys))
            {
                if (entitySetDataGenKeys.Count > 1)
                {
                    throw new TaupoInvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Entity type '{0}' belongs to multiple entity sets. Entity set name cannot be null or empty.",
                            entityTypeFullName));
                }

                ExceptionUtilities.Assert(entitySetDataGenKeys.Count == 1, "entitySetDataGenKeys cannot be empty");
                return entitySetDataGenKeys.First();
            }

            throw new TaupoInvalidOperationException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Cannot find entity set data generator for the entity type '{0}'.",
                    entityTypeFullName));
        }

        /// <summary>
        /// Creates the key to use for storing and looking-up a data service for the given entity type and set.
        /// </summary>
        /// <param name="entityTypeFullName">Full name of the entity type.</param>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <returns>Key for the specified entity type and set.</returns>
        protected string CreateKey(string entityTypeFullName, string entitySetName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", entitySetName, entityTypeFullName);
        }

        /// <summary>
        /// Registers the specified data service.
        /// </summary>
        /// <typeparam name="TDataService">The type of the data service.</typeparam>
        /// <param name="entityType">The entity type to register the data service for.</param>
        /// <param name="entitySetName">The entity set name to register the data service for.</param> 
        /// <param name="dataService">The data service.</param>
        /// <param name="dictionary">The dictionary to register the data service in.</param>
        protected void Register<TDataService>(EntityType entityType, string entitySetName, TDataService dataService, Dictionary<string, TDataService> dictionary)
        {
            var key = this.CreateKey(entityType.FullName, entitySetName);

            dictionary.Add(key, dataService);

            this.RegisterEntitySetKeyForEntityType(entityType, key);
        }

        /// <summary>
        /// Registers a data service (like data generator or data adapter) for the specified structural type.
        /// </summary>
        /// <typeparam name="TDataService">The type of the data service.</typeparam>
        /// <param name="structuralType">The structural type.</param>
        /// <param name="dataService">The data service.</param>
        /// <param name="dictionary">The dictionary to register the data service in.</param>
        protected void Register<TDataService>(NamedStructuralType structuralType, TDataService dataService, Dictionary<string, TDataService> dictionary)
        {
            dictionary.Add(structuralType.FullName, dataService);
        }

        /// <summary>
        /// Registers a data service (like a data generator) for the specified enum type.
        /// </summary>
        /// <typeparam name="TDataService">The type of the data service.</typeparam>
        /// <param name="enumType">The enum type.</param>
        /// <param name="dataService">The data service.</param>
        /// <param name="dictionary">The dictionary to register the data service in.</param>
        protected void Register<TDataService>(EnumType enumType, TDataService dataService, Dictionary<string, TDataService> dictionary)
        {
            dictionary.Add(enumType.FullName, dataService);
        }

        /// <summary>
        /// Gets a data service from the dictionary with the specified key.
        /// </summary>
        /// <typeparam name="TDataService">The type of the data service.</typeparam>
        /// <param name="key">The key to look-up in the dictionary.</param>
        /// <param name="dictionary">The dictionary of data services.</param>
        /// <returns>The data service from the dictionary with the specified key.</returns>
        /// <exception cref="TaupoInvalidOperationException">If a data service with the specified key cannot be found in the dictionary</exception>
        protected TDataService GetDataService<TDataService>(string key, Dictionary<string, TDataService> dictionary)
        {
            TDataService service;

            if (!dictionary.TryGetValue(key, out service))
            {
                throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot find {0} for '{1}'.", typeof(TDataService).Name, key));
            }

            return service;
        }

        /// <summary>
        /// Registers a key of an entity set data service for the entity type.
        /// Need this to be able to retrieve a data service for an entity set based only on entity type
        /// when there is only one entity set per entity type.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="key">A key to store/look-up an entityset data service for the entity type.</param>
        private void RegisterEntitySetKeyForEntityType(EntityType entityType, string key)
        {
            List<string> entitySetKeys;

            if (this.entityTypeToEntitySetMap.TryGetValue(entityType.FullName, out entitySetKeys))
            {
                if (!entitySetKeys.Contains(key))
                {
                    entitySetKeys.Add(key);
                }
            }
            else
            {
                entitySetKeys = new List<string>();
                entitySetKeys.Add(key);
                this.entityTypeToEntitySetMap.Add(entityType.FullName, entitySetKeys);
            }
        }
    }
}
