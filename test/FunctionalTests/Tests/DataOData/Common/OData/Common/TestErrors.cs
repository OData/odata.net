//---------------------------------------------------------------------
// <copyright file="TestErrors.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper class to create all interesting error payloads.
    /// </summary>
    public static class TestErrors
    {
        /// <summary>
        /// Creates a set of interesting error values.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <returns>List of test descriptors with interesting error values as payload.</returns>
        public static IEnumerable<PayloadTestDescriptor> CreateErrorTestDescriptors()
        {
            // error payloads
            IEnumerable<ODataErrorPayload> errors = new ODataErrorPayload[]
            {
                PayloadBuilder.Error().Code("ErrorCode"),
                PayloadBuilder.Error().Code("ErrorCode").Message("Error message"),
                PayloadBuilder.Error().Code("ErrorCode").Message(string.Empty),
                PayloadBuilder.Error().Message("Error message"),
                PayloadBuilder.Error().Message(string.Empty),
                PayloadBuilder.Error().Message("Error message"),
            };

            foreach (ODataErrorPayload errorPayload in errors)
            {
                yield return new PayloadTestDescriptor(){ PayloadElement = errorPayload.DeepCopy() };
            }

            // inner error payloads
            IEnumerable<ODataInternalExceptionPayload> innerErrors = new ODataInternalExceptionPayload[]
            {
                PayloadBuilder.InnerError(),
                PayloadBuilder.InnerError().Message("Inner error message"),
                PayloadBuilder.InnerError().StackTrace("Stack trace"),
                PayloadBuilder.InnerError().TypeName("Type name"),
                PayloadBuilder.InnerError().Message("Inner error message").StackTrace("Stack trace").TypeName("Type name"),
            };

            // create nested inner error payloads
            innerErrors = innerErrors.Concat(innerErrors.Select(inner =>
                PayloadBuilder.InnerError().Message("Outer inner error message").StackTrace("Stack trace").TypeName("Type name").InnerError(inner.DeepCopy())));

            // create a deeply nested inner error payload
            var deeplyNestedInnerError = PayloadBuilder.InnerError();
            foreach (var innerError in innerErrors)
            {
                var copyOfInnerError = innerError.DeepCopy();
                if (copyOfInnerError.InternalException == null)
                {
                    copyOfInnerError.InnerError(deeplyNestedInnerError);
                }
                else
                {
                    ExceptionUtilities.Assert(copyOfInnerError.InternalException.InternalException == null, "Did not expect inner error nesting beyond 2");
                    copyOfInnerError.InternalException.InnerError(deeplyNestedInnerError);
                }

                deeplyNestedInnerError = copyOfInnerError;
            }

            innerErrors = innerErrors.ConcatSingle(deeplyNestedInnerError);

            // put the inner errors into an empty error and all the other error payloads.
            foreach (ODataInternalExceptionPayload innerError in innerErrors)
            {
                yield return new PayloadTestDescriptor() { PayloadElement = PayloadBuilder.Error().InnerError(innerError.DeepCopy()) };

                foreach (ODataErrorPayload outerError in errors)
                {
                    yield return new PayloadTestDescriptor() { PayloadElement = outerError.DeepCopy().InnerError(innerError.DeepCopy()) };
                }
            }
        }
    }
}
