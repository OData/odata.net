//---------------------------------------------------------------------
// <copyright file="AstoriaQueryValueDeepCopyingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Extended version of the query value deep-copying visitor for values specific to Astoria
    /// </summary>
    public class AstoriaQueryValueDeepCopyingVisitor : QueryValueDeepCopyingVisitor, IAstoriaQueryValueVisitor<QueryValue>
    {
        /// <summary>
        /// Creates a copy of the given value recursively
        /// </summary>
        /// <param name="value">The value to copy</param>
        /// <returns>The copied value</returns>
        public QueryValue Visit(AstoriaQueryStreamValue value)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");

            if (value.EvaluationError != null)
            {
                return value.Type.CreateErrorValue(value.EvaluationError);
            }

            if (value.IsNull)
            {
                return value.Type.NullValue;
            }

            return value.Type.CreateValue(value.Value, value.ContentType, value.ETag, value.EditLink, value.SelfLink);
        }
    }
}