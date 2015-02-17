//---------------------------------------------------------------------
// <copyright file="ActionPayloadElementToQueryValuesConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Default implementation of the converter between a complex instance that is parameters to a function and QueryValues
    /// </summary>
    [ImplementationName(typeof(IActionPayloadElementToQueryValuesConverter), "Default")]
    public class ActionPayloadElementToQueryValuesConverter : IActionPayloadElementToQueryValuesConverter
    {
        /// <summary>
        /// Gets or sets the QueryTypeLibrary
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public QueryTypeLibrary QueryTypeLibrary { get; set; }

        /// <summary>
        /// Gets or sets the PaylElementToQueryValueTypeConverter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IPayloadElementToQueryValueConverter PayloadElementToQueryValueConverter { get; set; }

        /// <summary>
        /// Converts a Payload that contains the action parameters into QueryValues
        /// </summary>
        /// <param name="parametersPayload">Parameter Payload</param>
        /// <param name="action">Function that has the parameters that will be converted to a QueryValue</param>
        /// <returns>A Lookup of parameter names and QueryValues</returns>
        public IDictionary<string, QueryValue> Convert(ComplexInstance parametersPayload, Function action)
        {
            var parametersLookup = new Dictionary<string, QueryValue>();

            foreach (var property in parametersPayload.Properties)
            {
                var functionParameter = action.Parameters.Single(p => p.Name == property.Name);
                var functionParameterQueryType = this.QueryTypeLibrary.GetDefaultQueryType(functionParameter.DataType);
                QueryValue parameterValue = null;
                if (property.ElementType == ODataPayloadElementType.ComplexMultiValueProperty)
                {
                    var complexMultValueProperty = property as ComplexMultiValueProperty;
                    parameterValue = this.PayloadElementToQueryValueConverter.Convert(complexMultValueProperty.Value, functionParameterQueryType);
                }
                else if (property.ElementType == ODataPayloadElementType.ComplexProperty)
                {
                    var complexProperty = property as ComplexProperty;
                    parameterValue = this.PayloadElementToQueryValueConverter.Convert(complexProperty.Value, functionParameterQueryType);
                }
                else if (property.ElementType == ODataPayloadElementType.PrimitiveMultiValueProperty)
                {
                    var derivedProperty = property as PrimitiveMultiValueProperty;
                    parameterValue = this.PayloadElementToQueryValueConverter.Convert(derivedProperty.Value, functionParameterQueryType);
                }
                else if (property.ElementType == ODataPayloadElementType.PrimitiveProperty)
                {
                    var derivedProperty = property as PrimitiveProperty;
                    parameterValue = this.PayloadElementToQueryValueConverter.Convert(derivedProperty.Value, functionParameterQueryType);
                }

                ExceptionUtilities.CheckObjectNotNull(parameterValue, "Cannot convert to query value parameter for Action that is of type {0} and has a payload like {1}", property.ElementType, property);

                parametersLookup.Add(property.Name, parameterValue);
            }

            return parametersLookup;
        }
    }
}