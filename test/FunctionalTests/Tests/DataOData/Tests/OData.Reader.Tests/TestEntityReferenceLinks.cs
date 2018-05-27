//---------------------------------------------------------------------
// <copyright file="TestEntityReferenceLinks.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper class to create all interesting entity reference link instances used in payloads.
    /// </summary>
    internal static class TestEntityReferenceLinks
    {
        /// <summary>
        /// Creates a set of interesting entity reference link instances.
        /// </summary>
        /// <returns>List of interesting entity reference link instances.</returns>
        internal static IEnumerable<DeferredLink> CreateEntityReferenceLinkValues()
        {
            yield return PayloadBuilder.DeferredLink("http://odata.org/deferred");

            // TODO: enable support for relative links; decide whether to support them in JSON
            // yield return PayloadBuilder.DeferredLink("/deferred");
        }

        /// <summary>
        /// Creates a set of interesting entity reference link instances.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <returns>List of test descriptors with interesting entity reference link instances as payload.</returns>
        internal static IEnumerable<LinkCollection> CreateEntityReferenceLinksValues()
        {
            DeferredLink link1 = PayloadBuilder.DeferredLink("http://odata.org/deferred1");
            DeferredLink link2 = PayloadBuilder.DeferredLink("http://odata.org/deferred2");
            DeferredLink link3 = PayloadBuilder.DeferredLink("http://odata.org/deferred3");

            yield return PayloadBuilder.LinkCollection();

            yield return PayloadBuilder.LinkCollection().Item(link1);

            yield return PayloadBuilder.LinkCollection().Item(link1).Item(link2).Item(link3);

            yield return PayloadBuilder.LinkCollection().Item(link1).InlineCount(1);

            yield return PayloadBuilder.LinkCollection().Item(link1).InlineCount(-1);

            yield return PayloadBuilder.LinkCollection().Item(link1).NextLink("http://odata.org/nextlink");

            yield return PayloadBuilder.LinkCollection().Item(link1).InlineCount(1).NextLink("http://odata.org/nextlink");

            yield return PayloadBuilder.LinkCollection().Item(link1).Item(link2).Item(link3).InlineCount(1);

            yield return PayloadBuilder.LinkCollection().Item(link1).Item(link2).Item(link3).InlineCount(-1);

            yield return PayloadBuilder.LinkCollection().Item(link1).Item(link2).Item(link3).NextLink("http://odata.org/nextlink");

            yield return PayloadBuilder.LinkCollection().Item(link1).Item(link2).Item(link3).InlineCount(1).NextLink("http://odata.org/nextlink");
        }

        /// <summary>
        /// Creates a set of interesting entity reference link instances.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <returns>List of test descriptors with interesting entity reference link instances as payload.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateEntityReferenceLinkDescriptors(PayloadReaderTestDescriptor.Settings settings)
        {
            return CreateEntityReferenceLinkValues().Select(erl =>
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = erl,
                });
        }

        /// <summary>
        /// Creates a set of interesting entity reference links (collection) instances.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <returns>List of test descriptors with interesting entity reference links instances as payload.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateEntityReferenceLinksDescriptors(PayloadReaderTestDescriptor.Settings settings)
        {
            return CreateEntityReferenceLinksValues().Select(erl =>
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = erl,
                    SkipTestConfiguration = c => (c.IsRequest) && (erl.NextLink != null || erl.InlineCount.HasValue),
                    ExpectedResultCallback = tc =>
                            new PayloadReaderTestExpectedResult(settings.ExpectedResultSettings)
                });
        }
    }
}
