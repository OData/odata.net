//---------------------------------------------------------------------
// <copyright file="JsonLightDataContextFormatApplier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for generating client code layers.
    /// </summary>
    [ImplementationName(typeof(IClientDataContextFormatApplier), "JsonLight")]
    public class JsonLightDataContextFormatApplier : IClientDataContextFormatApplier
    {
        private readonly AstoriaWorkspace workspace;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonLightDataContextFormatApplier"/> class.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="clientCodeLayerGenerator">The ClientCodeLayerGenerator</param>
        public JsonLightDataContextFormatApplier(AstoriaWorkspace workspace, IClientCodeLayerGenerator clientCodeLayerGenerator)
        {
            ExceptionUtilities.CheckArgumentNotNull(workspace, "workspace");
            ExceptionUtilities.CheckArgumentNotNull(clientCodeLayerGenerator, "clientCodeLayerGenerator");

            this.workspace = workspace;
        }

        /// <summary>
        /// Determines whether [is using content type] [the specified content type].
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        ///   <c>true</c> if [is using content type] [the specified content type]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsUsingContentType(string contentType)
        {
            // May want to make this smarter at some point
            if (contentType == MimeTypes.ApplicationJsonLight)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Applies the specified data service client format.
        /// For this strategy nothing is done
        /// </summary>
        /// <param name="context">The data service client context to format.</param>
        public void Apply(DataServiceContext context)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            if (context.Format.LoadServiceModel != null)
            {
                context.Format.UseJson();
            }
            else
            {
                context.Format.UseJson(this.workspace.EdmModel);
            }
        }
    }
}