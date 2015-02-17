//---------------------------------------------------------------------
// <copyright file="AstoriaQueryDataSetBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Query;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Builds <see cref="QueryDataSet"/> based on <see cref="AstoriaWorkspace"/>.
    /// </summary>
    public class AstoriaQueryDataSetBuilder : AstoriaQueryDataSetBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the AstoriaQueryDataSetBuilder class.
        /// </summary>
        public AstoriaQueryDataSetBuilder()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the convention-based link generator to use
        /// </summary>
        [InjectDependency]
        public IODataConventionBasedLinkGenerator LinkGenerator { get; set; }

        /// <summary>
        /// Extends value population to include stream data
        /// </summary>
        /// <param name="row">The row containing the data</param>
        /// <param name="instance">The structural instacne</param>
        protected override void PopulateInstanceFromRow(EntitySetDataRow row, QueryStructuralValue instance)
        {
            base.PopulateInstanceFromRow(row, instance);
            var rowWithStreams = row as EntitySetDataRowWithStreams;
            if (rowWithStreams != null)
            {
                foreach (var stream in rowWithStreams.Streams)
                {
                    if (stream.IsEditLinkBasedOnConvention)
                    {
                        ExceptionUtilities.CheckObjectNotNull(this.LinkGenerator, "Cannot compute convention-based edit link without injected generator");
                        stream.EditLink = this.LinkGenerator.GenerateStreamEditLink(instance, stream.Name);

                        // for the default stream, there must always be a self-link
                        if (stream.Name == null && stream.SelfLink == null)
                        {
                            stream.SelfLink = stream.EditLink;
                        }
                    }

                    instance.SetStreamValue(stream.Name, stream.ContentType, stream.ETag, stream.EditLink, stream.SelfLink, stream.Content);
                }
            }

            instance.MarkDynamicPropertyValues();
        }
    }
}
