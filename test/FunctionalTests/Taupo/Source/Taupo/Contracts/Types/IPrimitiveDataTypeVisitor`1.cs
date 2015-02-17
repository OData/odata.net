//---------------------------------------------------------------------
// <copyright file="IPrimitiveDataTypeVisitor`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Visitor for primitive data types.
    /// </summary>
    /// <typeparam name="TValue">The type of the value returned from the Visit method.</typeparam>
    public interface IPrimitiveDataTypeVisitor<TValue>
    {
        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(BinaryDataType dataType);

        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(BooleanDataType dataType);

        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(DateTimeDataType dataType);

        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(EnumDataType dataType);

        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(FixedPointDataType dataType);

        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(FloatingPointDataType dataType);

        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(GuidDataType dataType);

        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(IntegerDataType dataType);

        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(SpatialDataType dataType);

        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(StreamDataType dataType);

        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(StringDataType dataType);

        /// <summary>
        /// Visits the specified data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Implementation-specific value.</returns>
        TValue Visit(TimeOfDayDataType dataType);
    }
}
