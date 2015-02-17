//---------------------------------------------------------------------
// <copyright file="JsonValueComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json
{
    #region Namespaces
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion

    /// <summary>
    /// Comparer for JsonValue objects.
    /// </summary>
    [ImplementationName(typeof(IJsonValueComparer), "JsonValueComparer")]
    public class JsonValueComparer : IJsonValueComparer
    {
        /// <summary>
        /// The assertion handler to use.
        /// </summary>
        private StackBasedAssertionHandler assert;

        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonValueComparer()
        {
            this.assert = new StackBasedAssertionHandler();
        }

        /// <summary>
        /// Compares two JsonValue objects.
        /// </summary>
        /// <param name="expected">The expected JsonValue.</param>
        /// <param name="actual">The actual JsonValue.</param>
        public void Compare(JsonValue expected, JsonValue actual)
        {
            assert.AreEqual(expected.JsonType, actual.JsonType, "The types of nodes '{0}' and '{1}' are not equal.", expected.ToString(), actual.ToString());

            switch (expected.JsonType)
            {
                case JsonValueType.JsonProperty:
                    CompareProperty((JsonProperty)expected, (JsonProperty)actual);
                    break;
                case JsonValueType.JsonObject:
                    CompareObject((JsonObject)expected, (JsonObject)actual);
                    break;
                case JsonValueType.JsonArray:
                    CompareArray((JsonArray)expected, (JsonArray)actual);
                    break;
                case JsonValueType.JsonPrimitiveValue:
                    ComparePrimitiveValue((JsonPrimitiveValue)expected, (JsonPrimitiveValue)actual);
                    break;
                default:
                    assert.Fail("Unexpected JsonType value '{0}'.", expected.JsonType);
                    break;
            }
        }

        /// <summary>
        /// Compares JSON properties.
        /// </summary>
        /// <param name="expected">Expected property./param>
        /// <param name="actual">Actual property.</param>
        private void CompareProperty(JsonProperty expected, JsonProperty actual)
        {
            assert.AreEqual(expected.Name, actual.Name, "The names of properties don't match.");
            using (this.assert.WithMessage("JsonProperty {0}", expected.Name))
            {
                this.Compare(expected.Value, actual.Value);
            }
        }

        /// <summary>
        /// Compares JSON objects.
        /// </summary>
        /// <param name="expected">Expected object.</param>
        /// <param name="actual">Actual object.</param>
        private void CompareObject(JsonObject expected, JsonObject actual)
        {
            using (this.assert.WithMessage("JsonObject"))
            {
                VerificationUtils.VerifyEnumerationsAreEqual(
                    expected.Properties,
                    actual.Properties,
                    (expectedProperty, actualProperty, a) => this.CompareProperty(expectedProperty, actualProperty),
                    (prop) => prop.ToString(),
                    this.assert);
            }
        }

        /// <summary>
        /// Compares JSON arrays.
        /// </summary>
        /// <param name="expected">Expected array.</param>
        /// <param name="actual">Actual array.</param>
        private void CompareArray(JsonArray expected, JsonArray actual)
        {
            using (this.assert.WithMessage("JsonArray"))
            {
                // We don't use the VerificationUtils here so that we can wrap the errors in a context noting the index of the element in the array.
                int expectedCount = expected.Elements.Count();
                this.assert.AreEqual(expectedCount, actual.Elements.Count(), "The number of elements in the array differs.");
                for (int i = 0; i < expectedCount; i++)
                {
                    using (this.assert.WithMessage("Element index {0}", i))
                    {
                        this.Compare(expected.Elements.ElementAt(i), actual.Elements.ElementAt(i));
                    }
                }
            }
        }

        /// <summary>
        /// Compares JSON primitive values.
        /// </summary>
        /// <param name="expected">Expected primitive value.</param>
        /// <param name="actual">Actual primitive value.</param>
        private void ComparePrimitiveValue(JsonPrimitiveValue expected, JsonPrimitiveValue actual)
        {
            this.assert.AreEqual(expected.Value, actual.Value, "The primitive values differ.");
        }
    }
}
