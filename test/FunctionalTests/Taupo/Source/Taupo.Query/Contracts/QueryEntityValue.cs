//---------------------------------------------------------------------
// <copyright file="QueryEntityValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Result of a query evaluation, which is an entity 
    /// </summary>
    public class QueryEntityValue : QueryStructuralValue
    {
        /// <summary>
        /// Stores the information of all the entity instances that 'Navigate' can reach, starting from this entity instance 
        /// </summary>
        /// <remarks>Given AssociationType and ToEnd, finds all Navigate results.</remarks>
        private Dictionary<AssociationType, Dictionary<AssociationEnd, QueryValue>> navigateResultLookup;

        /// <summary>
        /// Initializes a new instance of the QueryEntityValue class.
        /// </summary>
        /// <param name="type">The type of the value.</param>
        /// <param name="isNull">If set to <c>true</c> the structural value is null.</param>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        internal QueryEntityValue(QueryEntityType type, bool isNull, QueryError evaluationError, IQueryEvaluationStrategy evaluationStrategy)
            : base(type, isNull, evaluationError, evaluationStrategy)
        {
            this.navigateResultLookup = new Dictionary<AssociationType, Dictionary<AssociationEnd, QueryValue>>();
        }

        /// <summary>
        /// Get navigate result for specified associaion type and end
        /// </summary>
        /// <param name="associationType">The association type.</param>
        /// <param name="toEnd">The end to navigate to.</param>
        /// <returns>The result query entity value(s) for navigate.</returns>
        public QueryValue GetNavigateResult(AssociationType associationType, AssociationEnd toEnd)
        {
            ExceptionUtilities.Assert(this.navigateResultLookup.ContainsKey(associationType), "Invalid AssociationType for Navigate: {0}", associationType.FullName);
            var lookupBasedOnEnd = this.navigateResultLookup[associationType];
            
            ExceptionUtilities.Assert(lookupBasedOnEnd.ContainsKey(toEnd), "Invalid ToEnd for Navigate: {0}.{1}, from {2}.", associationType.FullName, toEnd.RoleName, (this.Type as QueryEntityType).EntityType.FullName);
            return lookupBasedOnEnd[toEnd];
        }

        internal void SetNavigateResult(AssociationType associationType, AssociationEnd toEnd, QueryValue result)
        {
            if (!this.navigateResultLookup.ContainsKey(associationType))
            {
                this.navigateResultLookup.Add(associationType, new Dictionary<AssociationEnd, QueryValue>());
            }

            Dictionary<AssociationEnd, QueryValue> lookupBasedOnEnd = this.navigateResultLookup[associationType];
            lookupBasedOnEnd[toEnd] = result;
        }
    }
}
