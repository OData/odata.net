//---------------------------------------------------------------------
// <copyright file="QueryKeyStructuralValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Structure that returns the key information for a EntityQueryValue
    /// </summary>
    public class QueryKeyStructuralValue : QueryStructuralValue
    {
        private IList<QueryProperty> keyProperties;
        private QueryEntityType queryEntityType;

        /// <summary>
        /// Initializes a new instance of the QueryKeyStructuralValue class.
        /// </summary>
        /// <param name="entityInstance">Entity Instance</param>
        internal QueryKeyStructuralValue(QueryEntityValue entityInstance)
            : base(entityInstance.Type, false, null, entityInstance.Type.EvaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityInstance, "entityInstance");

            this.queryEntityType = entityInstance.Type as QueryEntityType;
            this.keyProperties = this.queryEntityType.Properties.Where(p => p.IsPrimaryKey).ToList();

            ExceptionUtilities.CheckObjectNotNull(this.queryEntityType, "queryEntityType");
            ExceptionUtilities.Assert(this.keyProperties.Count > 0, "There are no key properties on QueryEntityType '{0}'", this.queryEntityType);

            foreach (QueryProperty keyProperty in this.keyProperties)
            {
                this.SetValue(keyProperty.Name, entityInstance.GetScalarValue(keyProperty.Name));
            }
        }
        
        /// <summary>
        /// Determines if two QueryKeyStructural values are equal or not
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>true if keys are equal false if not</returns>
        public override bool Equals(object obj)
        {
            var otherKeyValue = obj as QueryKeyStructuralValue;
            if (otherKeyValue != null)
            {
                if (this.Type.IsAssignableFrom(otherKeyValue.Type))
                {
                    bool match = true;
                    foreach (QueryProperty keyProperty in this.keyProperties)
                    {
                        QueryScalarValue currentKeySubPartValue = this.GetScalarValue(keyProperty.Name);
                        QueryScalarValue otherKeySubPartValue = otherKeyValue.GetScalarValue(keyProperty.Name);

                        if (!currentKeySubPartValue.Type.EvaluationStrategy.AreEqual(currentKeySubPartValue, otherKeySubPartValue))
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets hashcode of QueryKeyStructuralValue
        /// </summary>
        /// <returns>returns the Hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
