//---------------------------------------------------------------------
// <copyright file="IDataGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    /// <summary>
    /// Used by non-database-backed services to populate data
    /// </summary>
    public interface IDataGenerator
    {
        /// <summary>
        /// Whether or not to skip linking entities together
        /// </summary>
        bool SkipAssociations 
        { 
            get;
            set; 
        }

        /// <summary>
        /// Adds the data to the service using the DataInserter
        /// </summary>
        void Run();

        /// <summary>
        /// Whether or not the data generation is done, so that workspaces can decide to call the Get__GeneratedKey(s) methods
        /// </summary>
        bool Done
        {
            get;
        }

        /// <summary>
        /// Returns a random generated KeyExpression, should not incur a request to the service or underlying store
        /// </summary>
        /// <param name="container">container to draw keys from</param>
        /// <param name="type">desired type of entity to get a key for, or null if no specific type is desired</param>
        /// <returns>a single key expression, or null if one could not be found</returns>
        KeyExpression GetRandomGeneratedKey(ResourceContainer container, ResourceType type);

        /// <summary>
        /// Returns a list of all keys generated for the given parameters, should not incur a request to the service or underlying store
        /// </summary>
        /// <param name="container">container to draw keys from</param>
        /// <param name="type">desired type of entity to get keys for, or null if no specific type is desired</param>
        /// <returns>A list of all key expressions generated for the given container/type</returns>
        KeyExpressions GetAllGeneratedKeys(ResourceContainer container, ResourceType type);

        /// <summary>
        /// The data inserter to use during data generation
        /// </summary>
        IDataInserter DataInserter
        {
            get;
            set;
        }
    }
}
