//---------------------------------------------------------------------
// <copyright file="IDataTypeVisitor`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Visitor for data types.
    /// </summary>
    /// <typeparam name="TValue">The type of the value returned from Visit methods.</typeparam>
    public interface IDataTypeVisitor<TValue>
    {
        /// <summary>
        /// Visits the specified collection type.
        /// </summary>
        /// <param name="dataType">Data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(CollectionDataType dataType);

        /// <summary>
        /// Visits the specified complex type.
        /// </summary>
        /// <param name="dataType">Data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(ComplexDataType dataType);

        /// <summary>
        /// Visits the specified entity type.
        /// </summary>
        /// <param name="dataType">Data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(EntityDataType dataType);

        /// <summary>
        /// Visits the specified primitive type.
        /// </summary>
        /// <param name="dataType">Data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(PrimitiveDataType dataType);

        /// <summary>
        /// Visits the specified reference type.
        /// </summary>
        /// <param name="dataType">Data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(ReferenceDataType dataType);
    }
}
