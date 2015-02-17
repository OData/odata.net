//---------------------------------------------------------------------
// <copyright file="MetadataReaderTestDescriptorGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;

    #endregion Namespaces

    /// <summary>
    /// A helper class to create <see cref="MetadataReaderTestDescriptor"/> instances.
    /// </summary>
    internal static class MetadataReaderTestDescriptorGenerator
    {
        /// <summary>
        /// Creates a set of metadata reader test descriptors.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <returns>List of test descriptors with metadata documents as payload.</returns>
        public static IEnumerable<MetadataReaderTestDescriptor> CreateMetadataDocumentReaderDescriptors(MetadataReaderTestDescriptor.Settings settings)
        {
            return Microsoft.Test.OData.Utils.Metadata.TestModels.CreateModels().Select(m =>
                new MetadataReaderTestDescriptor(settings)
                {
                    PayloadEdmModel = m
                });
        }
    }
}
