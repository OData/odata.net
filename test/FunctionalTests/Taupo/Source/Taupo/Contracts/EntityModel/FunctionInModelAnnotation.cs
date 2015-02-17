//---------------------------------------------------------------------
// <copyright file="FunctionInModelAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Annnotation for function in conceptual model
    /// </summary>
    public class FunctionInModelAnnotation : Annotation
    {
        /// <summary>
        /// Gets or sets the defining expression.
        /// </summary>
        public string DefiningExpression { get; set; }
    }
}
