//---------------------------------------------------------------------
// <copyright file="DataWebObjectContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Collections.Generic;

namespace AstoriaUnitTests
{
    /// <summary>
    /// ObjectContext is the top-level object that encapsulates a connection between the CLR and the database,
    /// serving as a gateway for Create, Read, Update, and Delete operations.
    /// </summary>
    public abstract class DataWebObjectContext
    {
        #region Fields
        private readonly EntityConnection _connection;
        private readonly string _defaultEntityContainerName;
        private readonly string _connectionString;
        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates an ObjectContext with the given connection string and
        /// default entity container name.  This protected constructor creates and initializes an EntityConnection so that the context 
        /// is ready to use; no other initialization is necessary.  The given connection string must be valid for an EntityConnection; 
        /// connection strings for other connection types are not supported.
        /// </summary>
        /// <param name="connectionString">the connection string to use in the underlying EntityConnection to the store</param>
        /// <param name="defaultContainerName">the name of the default entity container</param>
        /// <exception cref="ArgumentNullException">connectionString is null</exception>
        /// <exception cref="ArgumentException">either connectionString or defaultContainerName is invalid</exception>
        protected DataWebObjectContext(string connectionString, string defaultContainerName)
        {
            _connectionString = connectionString;
            _defaultEntityContainerName = defaultContainerName;
        }

        /// <summary>
        /// Creates an ObjectContext with the given connection and metadata workspace.
        /// </summary>
        /// <param name="connection">connection to the store</param>
        /// <param name="defaultContainerName">the name of the default entity container</param>
        protected DataWebObjectContext(EntityConnection connection, string defaultContainerName)
        {
            _connection = connection;
            _defaultEntityContainerName = defaultContainerName;
        }
        #endregion //Constructors

        #region Methods

        /// <summary>
        /// Adds an object to the cache.  If it doesn't already have an entity key, the
        /// entity set is determined based on the type and the O-C map.
        /// If the object supports relationships (i.e. it implements IEntityWithRelationships),
        /// this also sets the context onto its RelationshipManager object.
        /// </summary>
        /// <param name="entitySetName">entitySetName the Object to be added. It might be qualifed with container name </param>
        /// <param name="entity">Object to be added.</param>
        public void AddObject(string entitySetName, object entity)
        {
        }

        public IQueryable<T> CreateQuery<T>(string queryString)
        {
            return null;
        }

        /// <summary>
        /// Executes the given function on the default container. 
        /// </summary>
        /// <typeparam name="T_Element">Element type for function results.</typeparam>
        /// <param name="function">Name of function. May include container (e.g. ContainerName.FunctionName)
        /// or just function name when DefaultContainerName is known.</param>
        /// <param name="parameters"></param>
        /// <exception cref="ArgumentException">If function is null or empty</exception>
        /// <exception cref="InvalidOperationException">If function is invalid (syntax,
        /// does not exist, refers to a function with return type incompatible with T)</exception>
        protected ObjectResult<T_Element> ExecuteFunction<T_Element>(string function, params ObjectParameter[] parameters)
        {
            return null;
        }

        #endregion //Methods
    }
}
