//---------------------------------------------------------------------
// <copyright file="DataServiceContextCreator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Wrappers;

    /// <summary>
    /// Creates a wrapped DataServiceContext
    /// </summary>
    [ImplementationName(typeof(IDataServiceContextCreator), "Default")]
    public class DataServiceContextCreator : IDataServiceContextCreator
    {
        /// <summary>
        /// a dictionary of any additional authentication headers to add to the outgoing requests
        /// </summary>
        private IDictionary<string, string> authenticationHeaders;

        /// <summary>
        /// Initializes a new instance of the DataServiceContextCreator class
        /// </summary>
        public DataServiceContextCreator()
        {
            //this.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            this.MaxProtocolVersion = DataServiceProtocolVersion.V4;
            this.ClientRequestAcceptHeader = string.Empty;
        }

        /// <summary>
        /// Gets or sets the format applier.
        /// </summary>
        /// <value>
        /// The format applier.
        /// </value>
        [InjectDependency(IsRequired = true)]
        public IClientDataContextFormatApplier FormatApplier { get; set; }

        /// <summary>
        /// Gets or sets the authenication provider used to authenticate against the Data Service
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAuthenticationProvider AuthenticationProvider { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the value of the "Accept" header of the client requests. If left empty, the value set by the product will not be overriden.
        /// </summary>
        [InjectTestParameter("ClientRequestAcceptHeader", DefaultValueDescription = "Empty string", HelpText = "Value to set for 'Accept' header of requests")]
        public string ClientRequestAcceptHeader { get; set; }

        ///// <summary>
        ///// Gets or sets a value indicating whether the client ignores properties present on the server but not in the client type.
        ///// </summary>
        //[InjectTestParameter("UndeclaredPropertyBehavior", DefaultValueDescription = "Support", HelpText = "Allows the client to ignore properties present on the server but not in the client type")]
        //internal UndeclaredPropertyBehavior UndeclaredPropertyBehavior { get; set; }

        /// <summary>
        /// Gets or sets the max protocol version to allow the context to use
        /// </summary>
        [InjectTestParameter("ClientMaxProtocolVersion", DefaultValueDescription = "V3", HelpText = "The max protocol version to allow the context to use")]
        public DataServiceProtocolVersion MaxProtocolVersion { get; set; }

        /// <summary>
        /// Gets or sets the EntitySetResolver
        /// </summary>
        [InjectDependency]
        public IEntitySetResolver EntitySetResolver { get; set; }

        /// <summary>
        /// Creates a default wrapped data service context without registering SendingRequest(2) event.
        /// </summary>
        /// <param name="scope">DataServiceContext TrackingScope</param>
        /// <param name="dataServiceContextType">The type of the DataServiceContext instance to be created</param>
        /// <param name="serviceBaseUri">service BaseUri</param>
        /// <returns>Wrapped DataServiceContext</returns>
        public WrappedDataServiceContext CreateContextWithoutSendingRequest(IWrapperScope scope, Type dataServiceContextType, Uri serviceBaseUri)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);
            ExceptionUtilities.CheckArgumentNotNull(scope, "scope");
            ExceptionUtilities.CheckArgumentNotNull(dataServiceContextType, "dataServiceContextType");
            ExceptionUtilities.Assert(typeof(DataServiceContext).IsAssignableFrom(dataServiceContextType), "Given type did not derive from DataServiceContext");

            WrappedDataServiceContext ctx = scope.CreateDataServiceContext(dataServiceContextType, serviceBaseUri, this.MaxProtocolVersion);
            DataServiceContext context = ctx.Product as DataServiceContext;

            this.SetCredentials(context);
            this.authenticationHeaders = this.AuthenticationProvider.GetAuthenticationHeaders();

            this.SetAcceptAndContentTypeHeaders(context);
            //ctx.UndeclaredPropertyBehavior = this.UndeclaredPropertyBehavior;

            this.FormatApplier.Apply(context);

            if (this.FormatApplier.IsUsingContentType(MimeTypes.ApplicationJsonLight))
            {
                // Setup the resolver.
                context.ResolveType = (name) => dataServiceContextType.Assembly.GetType(name);
                context.ResolveName = (type) => type.Namespace + "." + type.Name;
            }

            if (this.EntitySetResolver != null)
            {
                ctx.ResolveEntitySet = this.EntitySetResolver.ResolveEntitySetUri;
            }

            return ctx;
        }

        /// <summary>
        /// Register SendingRequest(2) event to inject authentication cookies.
        /// </summary>
        /// <param name="context">the WrappedDataServiceContext</param>
        public void RegisterSendingRequestEvent(WrappedObject context)
        {
            DataServiceContext ctx = context.Product as DataServiceContext;
            ExceptionUtilities.CheckObjectNotNull(ctx, "context has to be WrappedDataServiceContext.");
            // use SendingRequest2 since the client does not allow SendingRequest and BuildingRequest registered at the same time
            ctx.SendingRequest2 += this.InjectAuthenticationCookies;
        }

        /// <summary>
        /// Creates a default wrapped data service context.
        /// </summary>
        /// <param name="scope">DataServiceContext TrackingScope</param>
        /// <param name="dataServiceContextType">The type of the DataServiceContext instance to be created</param>
        /// <param name="serviceBaseUri">service BaseUri</param>
        /// <returns>Wrapped DataServiceContext</returns>
        public virtual WrappedDataServiceContext CreateContext(IWrapperScope scope, Type dataServiceContextType, Uri serviceBaseUri)
        {
            WrappedDataServiceContext ctx = this.CreateContextWithoutSendingRequest(scope, dataServiceContextType, serviceBaseUri);
            this.RegisterSendingRequestEvent(ctx);
            return ctx;
        }

        internal void SetAcceptAndContentTypeHeaders(DataServiceContext context)
        {
            if (!string.IsNullOrWhiteSpace(this.ClientRequestAcceptHeader))
            {
                context.SendingRequest2 += (sender, e) =>
                {
                    e.RequestMessage.SetHeader(HttpHeaders.Accept, this.ClientRequestAcceptHeader);
                    e.RequestMessage.SetHeader(HttpHeaders.ContentType, this.ClientRequestAcceptHeader);
                };
            }
        }

        // use SendingRequest2/SendingRequest2EventArgs since the client does not allow SendingRequest and BuildingRequest registered at the same time
        private void InjectAuthenticationCookies(object sender, SendingRequest2EventArgs e)
        {
            if (this.authenticationHeaders != null)
            {
                foreach (var header in this.authenticationHeaders)
                {
                    e.RequestMessage.SetHeader(header.Key, header.Value);
                }
            }
        }

        private void SetCredentials(DataServiceContext context)
        {
            if (this.AuthenticationProvider.UseDefaultCredentials)
            {
                context.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
            else if (this.AuthenticationProvider.GetAuthenticationCredentials() != null)
            {
                context.Credentials = this.AuthenticationProvider.GetAuthenticationCredentials();
            }
        }
    }
}
