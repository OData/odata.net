//---------------------------------------------------------------------
// <copyright file="QueryError.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Query evaluation error - this is a temporary class - will be replaced with something that actually
    /// holds errors.
    /// </summary>
    public class QueryError
    {
        private string message;

        /// <summary>
        /// Initializes a new instance of the QueryError class.
        /// </summary>
        /// <param name="errorMessage">The message.</param>
        public QueryError(string errorMessage)
        {
            this.message = errorMessage;
        }

        /// <summary>
        /// Combines the specified errors into one.
        /// </summary>
        /// <param name="firstError">The first error.</param>
        /// <param name="secondError">The second error.</param>
        /// <returns>Combined error.</returns>
        public static QueryError Combine(QueryError firstError, QueryError secondError)
        {
            if (firstError != null)
            {
                return firstError;
            }
            else
            {
                return secondError;
            }
        }

        /// <summary>
        /// Combines the specified errors into one.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <returns>Combined error.</returns>
        public static QueryError Combine(params QueryError[] errors)
        {
            ExceptionUtilities.CheckArgumentNotNull(errors, "errors");

            // for now, just return the first error of the list, in the future 
            // we'll be keeping track of the error list
            foreach (var el in errors)
            {
                if (el != null)
                {
                    return el;
                }
            }

            return null;
        }

        /// <summary>
        /// Combines the specified errors into one.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <returns>Combined error.</returns>
        public static QueryError Combine(IEnumerable<QueryError> errors)
        {
            ExceptionUtilities.CheckArgumentNotNull(errors, "errors");

            return Combine(errors.ToArray());
        }

        /// <summary>
        /// Combines the errors from values.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="elements">The elements.</param>
        /// <returns>Combined errors for all values.</returns>
        public static QueryError GetErrorFromValues<TValue>(IEnumerable<TValue> elements)
            where TValue : QueryValue
        {
            if (elements == null)
            {
                return null;
            }

            return Combine(elements.Select(c => c.EvaluationError));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.message;
        }
    }
}
