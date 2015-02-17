//---------------------------------------------------------------------
// <copyright file="FunctionImportReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// A return type for a FunctionInfo.  Multiple return types are for stored procedures only.  
    /// It is expected that any composable function will have exactly 1 FunctionImportReturnType.
    /// </summary>
    public class FunctionImportReturnType
    {
        /// <summary>
        /// Initializes a new instance of the FunctionImportReturnType class.
        /// </summary>
        public FunctionImportReturnType()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FunctionImportReturnType class with the given return type.
        /// </summary>
        /// <param name="returnType">The return type for the FunctionImport</param>
        public FunctionImportReturnType(DataType returnType)
            : this(returnType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FunctionImportReturnType class with the given return type and entity set.
        /// </summary>
        /// <param name="returnType">The return type for the FunctionImport</param>
        /// <param name="entitySet">The entity set associated with the return type</param>
        public FunctionImportReturnType(DataType returnType, EntitySet entitySet)
        {
            this.DataType = returnType;
            this.EntitySet = entitySet;
        }

        /// <summary>
        /// Gets or sets the return type
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// Gets or sets the EntitySet this FunctionImport binds to (optional)
        /// </summary>
        public EntitySet EntitySet { get; set; }
    }
}
