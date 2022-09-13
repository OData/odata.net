//---------------------------------------------------------------------
// <copyright file="RemoteClientCodeLayerGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.CodeDom;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataServiceBuilderService.DotNet;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Contract for generating client code layers.
    /// </summary>
    [ImplementationName(typeof(IClientCodeLayerGenerator), "Remote", HelpText = "Uses the client code generator service")]
    public class RemoteClientCodeLayerGenerator : IClientCodeLayerGenerator
    {
        /// <summary>
        /// Initializes a new instance of the RemoteClientCodeLayerGenerator class
        /// </summary>
        public RemoteClientCodeLayerGenerator()
        {
            this.DesignVersion = "current";
            this.ClientVersion = "v3";
        }

        /// <summary>
        /// Gets or sets the data service builder.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceBuilderService DataServiceBuilder { get; set; }

        /// <summary>
        /// Gets or sets the design version to send to the service
        /// </summary>
        public string DesignVersion { get; set; }

        /// <summary>
        /// Gets or sets the client version to send to the service
        /// </summary>
        public string ClientVersion { get; set; }

        /// <summary>
        /// Generates the client-side proxy classes then calls the given callback
        /// </summary>
        /// <param name="continuation">The async continuation to report completion on</param>
        /// <param name="serviceRoot">The root uri of the service</param>
        /// <param name="model">The model for the service</param>
        /// <param name="language">The language to generate code in</param>
        /// <param name="onCompletion">The action to invoke with the generated code</param>
        public void GenerateClientCode(IAsyncContinuation continuation, Uri serviceRoot, EntityModelSchema model, IProgrammingLanguageStrategy language, Action<string> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(serviceRoot, "serviceRoot");
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckArgumentNotNull(language, "language");
            ExceptionUtilities.CheckArgumentNotNull(onCompletion, "onCompletion");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            // because the product code-gen does not produce this overload of the DataServiceContext constructor, we need to add it ourselves
            // namespace <contextNamespace>
            // {
            //   partial class <contextType>
            //   {
            //     public <contextType>(Uri serviceUri, DataServiceProtocolVersion maxProtocolVersion)
            //       : base(serviceUri, maxProtocolVersion)
            //     {
            //     }
            //   }
            // }
            var compileUnit = new CodeCompileUnit();
            var contextNamespace = compileUnit.AddNamespace(model.EntityTypes.First().NamespaceName);
            var contextType = contextNamespace.DeclareType(model.EntityContainers.Single().Name);
            contextType.IsPartial = true;

            contextType.AddConstructor()
                .WithArgument(Code.TypeRef<Uri>(), "serviceUri")
                .WithArgument(Code.TypeRef("Microsoft.OData.Client.ODataProtocolVersion"), "maxProtocolVersion")
                .WithBaseConstructorArgument(Code.Variable("serviceUri"))
                .WithBaseConstructorArgument(Code.Variable("maxProtocolVersion"));

            string constructorOverload = language.CreateCodeGenerator().GenerateCodeFromNamespace(contextNamespace);

            this.DataServiceBuilder.BeginGenerateClientLayerCode(
                serviceRoot.OriginalString,
                this.DesignVersion,
                this.ClientVersion,
                language.FileExtension,
                result =>
                {
                    AsyncHelpers.CatchErrors(
                        continuation,
                        () =>
                        {
                            string errorMessage;
                            string clientCode = this.DataServiceBuilder.EndGenerateClientLayerCode(out errorMessage, result);
                            if (errorMessage != null)
                            {
                                throw new TaupoInfrastructureException(errorMessage);
                            }

                            // add the extra constructor overload we generated above
                            clientCode = string.Concat(clientCode, Environment.NewLine, constructorOverload);

                            onCompletion(clientCode);

                            continuation.Continue();
                        });
                },
                null);
        }
    }
}
