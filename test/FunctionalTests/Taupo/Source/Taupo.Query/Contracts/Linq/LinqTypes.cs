//---------------------------------------------------------------------
// <copyright file="LinqTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Factory methods used to construct QueryTypes.
    /// </summary>
    public static class LinqTypes
    {
        /// <summary>
        /// Creates a LinqLambdaType given the body type and parameter types.
        /// </summary>
        /// <param name="bodyType">Type of the lambda body.</param>
        /// <param name="parameterTypes">Collection of lambda parameter types.</param>
        /// <returns>LinqLambdaType with given body and parameter types.</returns>
        public static LinqLambdaType Lambda(QueryType bodyType, params QueryType[] parameterTypes)
        {
            return Lambda(bodyType, parameterTypes.AsEnumerable());
        }

        /// <summary>
        /// Creates a LinqLambdaType given the body type and parameter types.
        /// </summary>
        /// <param name="bodyType">Type of the lambda body.</param>
        /// <param name="parameterTypes">Collection of lambda parameter types.</param>
        /// <returns>LinqLambdaType with given body and parameter types.</returns>
        public static LinqLambdaType Lambda(QueryType bodyType, IEnumerable<QueryType> parameterTypes)
        {
            ExceptionUtilities.CheckArgumentNotNull(bodyType, "bodyType");
            ExceptionUtilities.CheckArgumentNotNull(parameterTypes, "parameterTypes");

            return new LinqLambdaType(bodyType, parameterTypes, bodyType.EvaluationStrategy);
        }
    }
}
