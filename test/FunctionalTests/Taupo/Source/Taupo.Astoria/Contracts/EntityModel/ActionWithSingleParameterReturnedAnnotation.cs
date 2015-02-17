//---------------------------------------------------------------------
// <copyright file="ActionWithSingleParameterReturnedAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Annotation that indicates the that a service member will just return the value passed to it
    /// </summary>
    public class ActionWithSingleParameterReturnedAnnotation : DataServiceMemberGeneratorAnnotation, IVerifyServiceActionQueryResult
    {
        /// <summary>
        /// Gets the expected query value for the action request
        /// </summary>
        /// <param name="initialExpectedResults">Initial expected values for an action</param>
        /// <param name="parameterValues">Parameter values for the action</param>
        /// <returns>A query Value that is the expected value</returns>
        public QueryValue GetExpectedQueryValue(QueryValue initialExpectedResults, params QueryValue[] parameterValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(initialExpectedResults, "initialExpectedResults");
            ExceptionUtilities.Assert(parameterValues.Length == 1, "There must be only one parameter that is passed in");

            return parameterValues[0];
        }
    }
}
