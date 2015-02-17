//---------------------------------------------------------------------
// <copyright file="StoredProcedureReturnTypeFunctionAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Annotation storing information used to generate stored procedure
    /// </summary>
    public class StoredProcedureReturnTypeFunctionAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the StoredProcedureReturnTypeFunctionAnnotation class.
        /// </summary>
        /// <param name="returnType">The return type for the stored procedure.</param>
        public StoredProcedureReturnTypeFunctionAnnotation(DataType returnType)
        {
            this.ReturnType = returnType;
        }

        /// <summary>
        /// Gets or sets the return type the stored procedure should return
        /// </summary>
        public DataType ReturnType { get; set; }

        /// <summary>
        /// Gets or sets the body generation annotation associated with this return type
        /// </summary>
        public StoreFunctionBodyGenerationInfoAnnotation BodyGenerationAnnotation { get; set; }
    }
}
