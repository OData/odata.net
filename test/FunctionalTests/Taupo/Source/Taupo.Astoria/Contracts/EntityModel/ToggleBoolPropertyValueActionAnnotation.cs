//---------------------------------------------------------------------
// <copyright file="ToggleBoolPropertyValueActionAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Annotation that indicates the ServiceBuilding to add a function that Toggles the given bool property value
    /// </summary>
    public class ToggleBoolPropertyValueActionAnnotation : DataServiceMemberGeneratorAnnotation, IVerifyServiceActionQueryResult
    {
        /// <summary>
        /// Gets or sets property name to toggle value
        /// </summary>
        public string ToggleProperty { get; set; }

        /// <summary>
        /// Gets or sets property name to return value
        /// </summary>
        public string ReturnProperty { get; set; }

        /// <summary>
        /// Gets or sets the entityset name that is the source that is used when the
        /// action doesn't have a binding parameter
        /// </summary>
        public string SourceEntitySet { get; set; }

        /// <summary>
        /// Gets the expected query value for the action request
        /// </summary>
        /// <param name="initialExpectedResults">Initial expected values for an action</param>
        /// <param name="parameterValues">Parameter values for the action</param>
        /// <returns>A query Value that is the expected value</returns>
        public QueryValue GetExpectedQueryValue(QueryValue initialExpectedResults, params QueryValue[] parameterValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(initialExpectedResults, "initialExpectedResults");
            if (initialExpectedResults.IsNull || this.ReturnProperty == null)
            {
                return initialExpectedResults.Type.NullValue;
            }

            QueryStructuralValue initialStructuralValue = initialExpectedResults as QueryStructuralValue;
            QueryCollectionValue initialCollectionValue = initialExpectedResults as QueryCollectionValue;
            if (initialStructuralValue != null)
            {
                return initialStructuralValue.GetValue(this.ReturnProperty);
            }
            else
            {
                ExceptionUtilities.CheckArgumentNotNull(initialCollectionValue, "Unsupported initialExpectedResults type.");
                if (initialCollectionValue.Elements.Count > 0)
                {
                    // Sort the results by the keys
                    var queryEntityType = (QueryEntityType)initialCollectionValue.Type.ElementType;
                    var sortedList = initialCollectionValue.Elements.Select(r => (QueryStructuralValue)r).ToList();
                    foreach (var key in queryEntityType.EntityType.AllKeyProperties)
                    {
                        sortedList = sortedList.OrderBy(r => r.GetScalarValue(key.Name).Value).ToList();
                    }

                    var firstItemValue = (QueryStructuralValue)sortedList.First();
                    ExceptionUtilities.CheckArgumentNotNull(firstItemValue, "firstItemValue");
                    return firstItemValue.GetValue(this.ReturnProperty);
                }
                else
                {
                    return initialCollectionValue.Type.ElementType.NullValue;
                }
            }
        }
    }
}
