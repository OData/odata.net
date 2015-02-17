//---------------------------------------------------------------------
// <copyright file="IncrementIntegerPropertyValueActionAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Annotation that indicates the ServiceBuilding to add a function that increments the given integer property value
    /// </summary>
    public class IncrementIntegerPropertyValueActionAnnotation : DataServiceMemberGeneratorAnnotation, IVerifyServiceActionQueryResult
    {
        /// <summary>
        /// Gets or sets property name to increment value
        /// </summary>
        public string IntegerProperty { get; set; }

        /// <summary>
        /// Gets the expected query value for the action request
        /// </summary>
        /// <param name="initialExpectedResults">Initial expected values for an action</param>
        /// <param name="parameterValues">Parameter values for the action</param>
        /// <returns>A query Value that is the expected value</returns>
        public QueryValue GetExpectedQueryValue(QueryValue initialExpectedResults, params QueryValue[] parameterValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(initialExpectedResults, "initialExpectedResults");

            QueryStructuralValue initialStructuralValue = initialExpectedResults as QueryStructuralValue;
            ExceptionUtilities.CheckArgumentNotNull(initialStructuralValue, "initialStructuralValue");

            QueryScalarValue initialScalarValue = initialStructuralValue.GetScalarValue(this.IntegerProperty);
            ExceptionUtilities.CheckArgumentNotNull(initialScalarValue, "initialScalarValue");

            int intPropertyValue = (int)initialScalarValue.Value;
            ExceptionUtilities.CheckArgumentNotNull(intPropertyValue, "intPropertyValue");

            if (intPropertyValue != int.MaxValue)
            {
                initialStructuralValue.SetPrimitiveValue(this.IntegerProperty, intPropertyValue + 1);
            }
            else
            {
                initialStructuralValue.SetPrimitiveValue(this.IntegerProperty, 0);
            }

            return initialStructuralValue;
        }
    }
}
