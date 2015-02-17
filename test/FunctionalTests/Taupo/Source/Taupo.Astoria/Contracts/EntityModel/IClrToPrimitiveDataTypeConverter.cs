//---------------------------------------------------------------------
// <copyright file="IClrToPrimitiveDataTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;
    
    /// <summary>
    /// Contract for converting between clr and primitive data types
    /// </summary>
    [ImplementationSelector("ClrToPrimitiveDataTypeConverter", DefaultImplementation = "Default")]
    public interface IClrToPrimitiveDataTypeConverter
    {
        /// <summary>
        /// Converts the clr type to a primitive data type
        /// </summary>
        /// <param name="clrType">The clr type</param>
        /// <returns>A primitive data type that corresponds to the clr type</returns>
        PrimitiveDataType ToDataType(Type clrType);

        /// <summary>
        /// Converts the primitive data type to a clr type
        /// </summary>
        /// <param name="dataType">The data type</param>
        /// <returns>The clr type that corresponds to the primitive data type</returns>
        Type ToClrType(PrimitiveDataType dataType);

        /// <summary>
        /// Gets the clr type that corresponds to the edm primitive data type name
        /// </summary>
        /// <param name="edmTypeName">The edm name of the data type</param>
        /// <returns>The clr type</returns>
        Type ToClrType(string edmTypeName);
    }
}