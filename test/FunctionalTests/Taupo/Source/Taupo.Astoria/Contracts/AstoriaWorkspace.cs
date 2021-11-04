//---------------------------------------------------------------------
// <copyright file="AstoriaWorkspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.ServiceReferences;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataServiceBuilderService.DotNet;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    
    /// <summary>
    /// Workspace for Astoria client tests.
    /// </summary>
    public class AstoriaWorkspace : AstoriaWorkspaceBase, IAstoriaServiceDescriptor, IAstoriaStringResourceVerifiers
    {
        /// <summary>
        /// Initializes a new instance of the AstoriaWorkspace class.
        /// </summary>
        /// <param name="workspaceBuilder">workspace builder</param>
        public AstoriaWorkspace(IWorkspaceBuilder workspaceBuilder)
        {
            this.WorkspaceBuilder = workspaceBuilder;
            this.Assemblies = new List<FileContents<Assembly>>();
        }

        /// <summary>
        /// Gets the assemblies containing client code.
        /// </summary>
        public IList<FileContents<Assembly>> Assemblies { get; private set; }

        /// <summary>
        /// Gets or sets the CLR type of the data context.
        /// </summary>
        public Type ContextType { get; set; }

        /// <summary>
        /// Gets or sets the service URI of the data service.
        /// </summary>
        /// <value>The service URI.</value>
        public Uri ServiceUri { get; set; }

        /// <summary>
        /// Gets the service URI of the Oracle service.
        /// </summary>
        /// <value>The Oracle service URI.</value>
        public Uri OracleServiceUri { get; internal set; }

        /// <summary>
        /// Gets or sets the workspace information.
        /// </summary>
        public AstoriaWorkspaceInfo WorkspaceInfo { get; set; }

        /// <summary>
        /// Gets the Entity Set Resolver to resolve an entity set name to the Collection Uri
        /// </summary>
        public IEntitySetResolver EntitySetResolver { get; internal set; }
                
        /// <summary>
        /// Gets the workspace builder
        /// </summary>
        public IWorkspaceBuilder WorkspaceBuilder { get; private set; }

        /// <summary>
        /// Gets the verifier for resources in Microsoft.OData.Service
        /// </summary>
        public virtual IStringResourceVerifier SystemDataServicesStringVerifier { get; internal set; }

        /// <summary>
        /// Gets the verifier for resources in Microsoft.OData.Client
        /// </summary>
        public IStringResourceVerifier SystemDataServicesClientStringVerifier { get; internal set; }

        /// <summary>
        /// Gets the verifier for resources in Microsoft.OData.Core
        /// </summary>
        public IStringResourceVerifier MicrosoftDataODataStringVerifier { get; internal set; }

        /// <summary>
        /// Gets the entity container data which was downloaded from the oracle service at workspace creation time.
        /// </summary>
        public EntityContainerData DownloadedEntityContainerData { get; internal set; }

        /// <summary>
        /// Gets the edm model.
        /// </summary>
        public IEdmModel EdmModel { get; internal set; }

        /// <summary>
        /// Gets or sets ResourceLookup for resources in Microsoft.OData.Service
        /// Used by unit tests only.
        /// </summary>
        internal IResourceLookup SystemDataServicesResourceLookup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the stream data is setup or not
        /// </summary>
        internal bool StreamsDataAlreadyAdd { get; set; }
    }
}
