//---------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Common
{
    #region Namespaces
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper extension methods for reader tests
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Adds expected exception to the test descriptor.
        /// </summary>
        /// <param name="testDescriptor">The payload reader test descriptor to add expected exception to.</param>
        /// <param name="expectedException">The expected exception to add.</param>
        /// <returns>The <paramref name="testDescriptor"/> with the expected exception added.</returns>
        public static PayloadReaderTestDescriptor WithExpectedException(this PayloadReaderTestDescriptor testDescriptor, ExpectedException expectedException)
        {
            testDescriptor.ExpectedException = expectedException;
            return testDescriptor;
        }

        /// <summary>
        /// Determines if the <paramref name="payloadElement"/> is an association link without the navigation link portion.
        /// </summary>
        /// <param name="payloadElement">The payload element to inspect.</param>
        /// <returns>true if the <paramref name="payloadElement"/> is an association link only, or false otherwise.</returns>
        public static bool IsAssociationLinkOnly(this ODataPayloadElement payloadElement)
        {
            NavigationPropertyInstance navigationProperty = payloadElement as NavigationPropertyInstance;
            return navigationProperty != null && navigationProperty.Value == null && navigationProperty.AssociationLink != null;
        }

        /// <summary>
        /// Adds payload order items to the specified payload element.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to add the items to.</typeparam>
        /// <param name="payloadElement">The payload element to add the items to.</param>
        /// <param name="items">The items to add.</param>
        /// <returns>The <paramref name="payloadElement"/> with the items added.</returns>
        public static T PayloadOrderItems<T>(this T payloadElement, params string[] items) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            PayloadOrderODataPayloadElementAnnotation payloadOrderAnnotation =
                payloadElement.Annotations.OfType<PayloadOrderODataPayloadElementAnnotation>().SingleOrDefault();
            if (payloadOrderAnnotation == null)
            {
                payloadOrderAnnotation = new PayloadOrderODataPayloadElementAnnotation();
                payloadElement.Add(payloadOrderAnnotation);
            }

            payloadOrderAnnotation.PayloadItems.AddRange(items);
            return payloadElement;
        }
    }
}