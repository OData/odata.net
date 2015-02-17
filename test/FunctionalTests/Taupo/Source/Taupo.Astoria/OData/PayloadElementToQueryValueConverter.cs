//---------------------------------------------------------------------
// <copyright file="PayloadElementToQueryValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Default implementation of the converter between payload elements and QueryValue
    /// </summary>
    [ImplementationName(typeof(IPayloadElementToQueryValueConverter), "Default")]
    public class PayloadElementToQueryValueConverter : IPayloadElementToQueryValueConverter
    {
        /// <summary>
        /// Gets or sets a PayloadElementToNamedValuesConverter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IPayloadElementToNamedValuesConverter PayloadElementToNamedValuesConverter { get; set; }

        /// <summary>
        /// Gets or sets a NamedValueToQueryValueConverter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public INamedValueToQueryValueConverter NamedValueToQueryValueConverter { get; set; }

        /// <summary>
        /// Converts the given payload into a series of named value pairs
        /// </summary>
        /// <param name="element">The payload to convert</param>
        /// <param name="queryTypeToBuild">Query Type to build</param>
        /// <returns>The queryValue that represents the ODataPayload</returns>
        public QueryValue Convert(ODataPayloadElement element, QueryType queryTypeToBuild)
        {
            var typedValue = element as ITypedValue;
            if (typedValue != null && typedValue.IsNull)
            {
                return queryTypeToBuild.NullValue;
            }
            
            var queryCollectionType = queryTypeToBuild as QueryCollectionType;

            // return empty QueryCollectionValue for all types of empty collections
            PrimitiveCollection primitiveCollection = element as PrimitiveCollection;
            ComplexInstanceCollection complexInstanceCollection = element as ComplexInstanceCollection;
            PrimitiveMultiValue primitiveMultiValue = element as PrimitiveMultiValue;
            ComplexMultiValue complexMultiValue = element as ComplexMultiValue;
            if ((primitiveCollection != null && primitiveCollection.Count == 0) ||
                (complexInstanceCollection != null && complexInstanceCollection.Count == 0) ||
                (primitiveMultiValue != null && primitiveMultiValue.Count == 0) ||
                (complexMultiValue != null && complexMultiValue.Count == 0))
            {
                return queryCollectionType.CreateCollectionWithValues(new QueryValue[] { });
            }
            
            if (element.ElementType == ODataPayloadElementType.PrimitiveValue)
            {
                return CreateQueryScalarValue(element, queryTypeToBuild);
            }
            else if (queryCollectionType != null && element.ElementType == ODataPayloadElementType.PrimitiveMultiValue)
            {
                return queryCollectionType.CreateCollectionWithValues(primitiveMultiValue.Select(p => CreateQueryScalarValue(p, queryCollectionType.ElementType)));
            }

            var namedValues = this.PayloadElementToNamedValuesConverter.ConvertToNamedValues(element);

            return this.NamedValueToQueryValueConverter.Convert(namedValues, queryTypeToBuild);
        }

        private static QueryValue CreateQueryScalarValue(ODataPayloadElement payload, QueryType queryTypeToBuild)
        {
            var primitiveValue = payload as PrimitiveValue;
            var queryScalarType = queryTypeToBuild as QueryScalarType;
            ExceptionUtilities.CheckObjectNotNull(queryScalarType, "payload is a primitive type so queryType must be as well");

            return queryScalarType.CreateValue(primitiveValue.ClrValue);
        }
    }
}