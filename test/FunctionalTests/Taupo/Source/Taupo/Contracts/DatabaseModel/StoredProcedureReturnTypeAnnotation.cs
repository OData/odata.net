//---------------------------------------------------------------------
// <copyright file="StoredProcedureReturnTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Annotation storing information used to generate stored procedure
    /// </summary>
    public class StoredProcedureReturnTypeAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the StoredProcedureReturnTypeAnnotation class.
        /// </summary>
        /// <param name="returnType">The return type for the stored procedure.</param>
        public StoredProcedureReturnTypeAnnotation(DataType returnType)
        {
            this.ReturnType = returnType;
        }

        /// <summary>
        /// Gets the return type the stored procedure should return
        /// </summary>
        public DataType ReturnType { get; private set; }

        /// <summary>
        /// Gets or sets the generation information associated with this return type
        /// </summary>
        public DatabaseTableValuedFunctionGenerationInfoAnnotation BodyGenerationAnnotation { get; set; }
    }
}