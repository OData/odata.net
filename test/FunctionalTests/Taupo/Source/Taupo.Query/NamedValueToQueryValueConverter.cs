//---------------------------------------------------------------------
// <copyright file="NamedValueToQueryValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Component that can convert query structural values based on a list of NamedValues
    /// </summary>
    [ImplementationName(typeof(INamedValueToQueryValueConverter), "Default")]
    public class NamedValueToQueryValueConverter : INamedValueToQueryValueConverter
    {
        /// <summary>
        /// Gets or sets a NamedValue to QueryValueStructural Updater
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public INamedValueToQueryValueUpdater NamedValueToQueryStructuralValueUpdater { get; set; }

        /// <summary>
        /// Converts namedvalues to query values 
        /// </summary>
        /// <param name="namedValues">values to update to</param>
        /// <param name="queryTypeToBuild">Query Type to build</param>
        /// <returns>Returns a new queryValue with the named values tranferred into it</returns>
        public QueryValue Convert(IEnumerable<NamedValue> namedValues, QueryType queryTypeToBuild)
        {
            var queryStructuralType = queryTypeToBuild as QueryStructuralType;

            if (queryStructuralType != null)
            {
                var queryComplexValue = queryStructuralType.CreateNewInstance();
                this.NamedValueToQueryStructuralValueUpdater.UpdateValues(queryComplexValue, namedValues);

                return queryComplexValue;
            }
            else
            {
                var collectionType = queryTypeToBuild as QueryCollectionType;
                ExceptionUtilities.CheckObjectNotNull(collectionType, "Unexpected query type {0}", queryTypeToBuild);

                var queryCollectionValue = collectionType.CreateCollectionWithValues(new QueryValue[] { });
                this.NamedValueToQueryStructuralValueUpdater.UpdateValues(queryCollectionValue, namedValues);

                return queryCollectionValue;
            }
        }
    }
}
