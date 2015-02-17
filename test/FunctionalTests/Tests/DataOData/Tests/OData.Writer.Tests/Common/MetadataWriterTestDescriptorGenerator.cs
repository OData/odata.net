//---------------------------------------------------------------------
// <copyright file="MetadataWriterTestDescriptorGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.OData.Utils.Metadata;
    #endregion Namespaces

    /// <summary>
    /// A helper class to create <see cref="MetadataWriterTestDescriptor"/> instances.
    /// </summary>
    internal static class MetadataWriterTestDescriptorGenerator
    {
        /// <summary>
        /// Creates a set of metadata document test descriptors.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <returns>List of test descriptors with metadata documents as payload.</returns>
        public static IEnumerable<MetadataWriterTestDescriptor> CreateMetadataDocumentWriterDescriptors(MetadataWriterTestDescriptor.Settings settings)
        {
            return TestModels.CreateModels().Select(m =>
                new MetadataWriterTestDescriptor(settings)
                {
                    EdmVersion = EdmVersion.Latest,
                    Model = m,
                });
        }
    }
}
