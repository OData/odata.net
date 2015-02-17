//---------------------------------------------------------------------
// <copyright file="LinqParameterNameResolutionVisitorHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Linq
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Helper used in parameter name resolution visitors.
    /// </summary>
    public class LinqParameterNameResolutionVisitorHelper
    {
        private Dictionary<LinqParameterExpression, LinqParameterExpression> parameterMappings;
        private IIdentifierGenerator parameterNameGenerator;

        /// <summary>
        /// Initializes a new instance of the LinqParameterNameResolutionVisitorHelper class.
        /// </summary>
        /// <param name="parameterNameGenerator">The parameter name generator.</param>
        public LinqParameterNameResolutionVisitorHelper(IIdentifierGenerator parameterNameGenerator)
        {
            this.parameterMappings = new Dictionary<LinqParameterExpression, LinqParameterExpression>();
            this.parameterNameGenerator = parameterNameGenerator;
        }

        /// <summary>
        /// Ensures that the given parameter has a name and creates a new instance if not.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Parameter with a name.</returns>
        /// <remarks>Subsequent calls to this method with the same expression
        /// will return the same instance of parameter.</remarks>
        public LinqParameterExpression EnsureParameterHasName(LinqParameterExpression expression)
        {
            LinqParameterExpression parameterWithName;
            if (!this.parameterMappings.TryGetValue(expression, out parameterWithName))
            {
                if (expression.Name == null)
                {
                    parameterWithName = LinqBuilder.Parameter(this.parameterNameGenerator.GenerateIdentifier("p"), expression.ExpressionType);
                }
                else
                {
                    parameterWithName = expression;
                }

                this.parameterMappings.Add(expression, parameterWithName);
            }

            return parameterWithName;
        }
    }
}
