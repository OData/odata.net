//---------------------------------------------------------------------
// <copyright file="FunctionBodyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Annotation representing the body of a Function as a QueryExpression
    /// </summary>
    public class FunctionBodyAnnotation : Annotation
    {
        /// <summary>
        /// Gets or sets the QueryExpression that represents the Function Body
        /// </summary>
        public QueryExpression FunctionBody { get; set; }

        /// <summary>
        /// Gets or sets a function for dynamically generating the Function Body expression.
        /// </summary>
        /// <remarks>
        /// Useful for cases where the expression cannot be declared within the model (e.g.
        /// when using Entity Sets)
        /// </remarks>
        public Func<EntityModelSchema, QueryExpression> FunctionBodyGenerator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Function Body represents a root level query
        /// </summary>
        public bool IsRoot { get; set; }
    }
}
