//---------------------------------------------------------------------
// <copyright file="IEntityGraphCreator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Provides utilities for creating entity graphs.
    /// </summary>
    public interface IEntityGraphCreator
    {
        /// <summary>
        /// Creates a set of objects based on the specified <paramref name="root"/> entity instance
        /// and only its required relationships.
        /// </summary>
        /// <param name="entitySetName">The entity set name for the root entity.</param>
        /// <param name="root">The root entity around which the graph is based.</param>
        /// <param name="entityCreated">A callback function invoked every time a new entity instance is created and its properties are initialized.</param>
        /// <param name="connectEntities">A callback used to connect two objects together. Examples of actions include setting navigation properties,
        /// synchronizing FKs to PK values, and/or using the IRelatedEnd or SetLink APIs. The first two parameters are the objects that need to
        /// be connected, and the third is the <see cref="RelationshipSide"/> describing the side of the relationship which the first object participates
        /// in.</param>
        /// <returns>A list of <see cref="IEntitySetData"/> representing all objects in the graph used to satisfy the required relationships.
        /// The first object in the list is always the <paramref name="root"/>.</returns>
        IList<IEntitySetData> CreateGraphWithRequiredRelationships(
            string entitySetName,
            object root,
            Action<IEntitySetData> entityCreated,
            ConnectEntitiesCallback connectEntities);
    }
}
