//---------------------------------------------------------------------
// <copyright file="IDataInserter.cs" company="Microsoft">
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
    /// Interface used by implementations of IDataGenerator to insert the generated data into the service
    /// </summary>
    public interface IDataInserter
    {
        #region events
        /// <summary>
        /// Event fired when AddAssociation is called
        /// </summary>
        event Action<KeyedResourceInstance, ResourceProperty, KeyedResourceInstance> OnAddingAssociation;

        /// <summary>
        /// Event fired when AddEntity is called
        /// </summary>
        event Action<KeyExpression, KeyedResourceInstance> OnAddingEntity;
        #endregion

        #region properties
        /// <summary>
        /// whether the insertion needs to happen before/after the service is created
        /// </summary>
        bool BeforeServiceCreation
        {
            get;
        }
        #endregion

        #region methods
        /// <summary>
        /// Adds an association between the given entities, along the given navigation property
        /// </summary>
        /// <param name="parent">Update tree of parent, must have already been added using AddEntity</param>
        /// <param name="property">Navigation property for the association</param>
        /// <param name="child">Update tree of child, must have already been added using AddEntity</param>
        void AddAssociation(KeyedResourceInstance parent, ResourceProperty property, KeyedResourceInstance child);

        /// <summary>
        /// Inserts a new entity based on the given update tree
        /// </summary>
        /// <param name="key">key expression of new entity</param>
        /// <param name="entity">update tree of new entity</param>
        void AddEntity(KeyExpression key, KeyedResourceInstance entity);

        /// <summary>
        /// Close the inserter and perform any final operations
        /// </summary>
        void Close();

        /// <summary>
        /// Flush pending changes to the service
        /// </summary>
        void Flush();
        #endregion
    }
}
