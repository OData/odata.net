//---------------------------------------------------------------------
// <copyright file="DataServiceExecuteVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Base class for building and verifying queries using Astoria client;
    /// </summary>
    [ImplementationName(typeof(IQueryVerifier), "ClientExecute")]
    public class DataServiceExecuteVerifier : IQueryVerifier
    {
        private AstoriaWorkspace workspace;
        private List<QueryExpression> pendingQueries = new List<QueryExpression>();

        /// <summary>
        /// Initializes a new instance of the DataServiceExecuteVerifier class.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public DataServiceExecuteVerifier(AstoriaWorkspace workspace)
        {
            this.workspace = workspace;
            this.Logger = Logger.Null;
            this.IsUri = false;
            this.UseSendingRequestEventVerifier = true;
        }

        /// <summary>
        /// Gets or sets the logger used to print diagnostic messages.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets the code builder used to generate code.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public CodeBuilder CodeBuilder { get; set; }

        /// <summary>
        /// Gets or sets the code builder used to generate code.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryResolver LinqQueryResolver { get; set; }

        /// <summary>
        /// Gets or sets the result comparer.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClientQueryResultComparer ResultComparer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether too verify the sendingRequest events or not
        /// </summary>
        /// <value><c>true</c> if this instance is specified then the sending request verification is used; otherwise, <c>false</c>.</value>
        [InjectTestParameter("UseSendingRequestEventVerifier", DefaultValueDescription = "false")]
        public bool UseSendingRequestEventVerifier { get; set; }

        /// <summary>
        /// Gets or sets the DataServiceContextCreator 
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextCreator DataServiceContextCreator { get; set; }

        /// <summary>
        /// Gets or sets the DataServiceContextScope 
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextTrackingScope DataServiceContextScope { get; set; }

        /// <summary>
        /// Gets or sets the sending request event verifier to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ISendingRequestEventVerifier SendingRequestEventVerifier { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [InjectDependency(IsRequired = true)]
        public IProgrammingLanguageStrategy Language { get; set; }

        /// <summary>
        /// Gets or sets the Client Query Single Variation Code Builder Factory.
        /// </summary>
        /// <value>The language.</value>
        [InjectDependency(IsRequired = true)]
        public IClientQuerySingleVariationCodeBuilderResolver ClientQuerySingleVariationCodeBuilderResolver { get; set; }

        /// <summary>
        /// Gets or sets the Versioning error oracle
        /// </summary>
        /// <value>The language.</value>
        [InjectDependency(IsRequired = true)]
        public IClientQueryVersionErrorCalculator ClientQueryVersionErrorCalculator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is URI.
        /// </summary>
        /// <value><c>true</c> if this instance is URI; otherwise, <c>false</c>.</value>
        [InjectTestParameter("ExecuteURI", DefaultValueDescription = "false")]
        public bool IsUri { get; set; }

        /// <summary>
        /// Verify the passed expression tree.
        /// </summary>
        /// <param name="expression">The expression tree which will be verified</param>
        public virtual void Verify(QueryExpression expression)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.ThrowSkippedExceptionIfQueryExpressionNotSupported(expression);

            if (this.pendingQueries.Count == 0)
            {
                AsyncExecutionContext.EnqueueAsynchronousAction(this.VerifyAllQueries);
            }

            this.pendingQueries.Add(expression);
        }

        internal static IEnumerable<string> GetNamespacesToImport(EntityModelSchema model)
        {
            yield return "System";
            yield return "System.Collections.Generic";
            yield return "System.Collections.ObjectModel";
            yield return "Microsoft.OData.Client";
            yield return "System.Linq";
            yield return typeof(AsyncHelpers).Namespace;
            yield return typeof(ClientQueryExtensionMethods).Namespace;

            if (model.HasSpatialFeatures())
            {
                // needed for extension methods for distance, etc.
                yield return "Microsoft.Spatial";
            }
        }

        internal static IEnumerable<string> GetAssembliesToReference(EntityModelSchema model, IProgrammingLanguageStrategy language)
        {
            if (language.LanguageName != "VB")
            {
                yield return "mscorlib.dll";
            }

            yield return "System.dll";
            yield return "System.Core.dll";
            yield return DataFxAssemblyRef.File.DataServicesClient;
            yield return DataFxAssemblyRef.File.ODataLib;
            yield return DataFxAssemblyRef.File.EntityDataModel;

            if (model.HasSpatialFeatures())
            {
                yield return DataFxAssemblyRef.File.SpatialCore;
            }

            yield return "Microsoft.Test.Taupo.dll";
            yield return "Microsoft.Test.Taupo.Astoria.dll";
            yield return "Microsoft.Test.Taupo.Query.dll";
        }

        /// <summary>
        /// Verify all queries.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        protected virtual void VerifyAllQueries(IAsyncContinuation continuation)
        {
            var pending = this.pendingQueries;
            this.pendingQueries = new List<QueryExpression>();

            var code = this.PrepareCodeBuilder();

            var contextType = this.GetClientContextType();
            var dataServiceContext = this.DataServiceContextCreator.CreateContext(this.DataServiceContextScope, contextType, this.workspace.ServiceUri);
            dataServiceContext.MergeOption = this.DataServiceContextScope.Wrap<WrappedEnum>(MergeOption.OverwriteChanges);
            var context = dataServiceContext.Product as DataServiceContext;
            var contextVariable = code.AddExternalProperty("Context", new CodeTypeReference(context.GetType()), context);
            var resultComparerVariable = code.AddExternalProperty("ResultComparer", this.ResultComparer);

            if (this.UseSendingRequestEventVerifier)
            {
                this.SendingRequestEventVerifier.RegisterEventHandler(context);
                continuation = continuation.OnContinueOrFail(isError => this.SendingRequestEventVerifier.UnregisterEventHandler(context, isError));
            }

            for (int i = 0; i < pending.Count; ++i)
            {
                var query = this.LinqQueryResolver.Resolve(pending[i]);

                this.ResultComparer.EnqueueNextQuery(query);
                this.BuildSingleVariation(contextVariable, resultComparerVariable, query, dataServiceContext);
            }

            code.RunVariations(continuation);
        }

        /// <summary>
        /// Builds the single variation.
        /// </summary>
        /// <param name="contextVariable">The context variable.</param>
        /// <param name="resultComparerVariable">The result comparer variable.</param>
        /// <param name="query">The query.</param>
        /// <param name="dataServiceContext">Wrapped Data Service Context</param>
        protected virtual void BuildSingleVariation(CodeExpression contextVariable, CodeExpression resultComparerVariable, QueryExpression query, WrappedDataServiceContext dataServiceContext)
        {
            DataServiceProtocolVersion maxProtocolVersion = this.workspace.ConceptualModel.GetMaxProtocolVersion();
            DataServiceProtocolVersion maxClientProtocolVersion = DataServiceProtocolVersion.Unspecified;
            maxClientProtocolVersion = ((ODataProtocolVersion)dataServiceContext.MaxProtocolVersion.Product).ToTestEnum();
            var singleVariationCodeBuilder = this.ClientQuerySingleVariationCodeBuilderResolver.Resolve(query);

            // TODO: Fix service operation test coverage in Taupo.Clientko
            if (singleVariationCodeBuilder != null)
            {
                var expectedError = this.ClientQueryVersionErrorCalculator.CalculateExpectedClientVersionError(query, !this.IsUri, maxClientProtocolVersion, maxProtocolVersion);
                var expectedClientErrorValue = ClientQueryCodeGenHelperMethods.BuildClientExpectedErrorExpression(expectedError);

                singleVariationCodeBuilder.Build(this.CodeBuilder, query, expectedClientErrorValue, contextVariable, resultComparerVariable);
            }
        }

        /// <summary>
        /// Creates a new CodeBuilder object.
        /// </summary>
        /// <returns>A new CodeBuilder object</returns>
        protected CodeBuilder PrepareCodeBuilder()
        {
            var code = this.CodeBuilder;

            code.Reset();

            foreach (var namespaceToImport in GetNamespacesToImport(this.workspace.ConceptualModel))
            {
                code.AddNamespaceImport(namespaceToImport);
            }

            foreach (var asm in GetAssembliesToReference(this.workspace.ConceptualModel, this.Language))
            {
                code.AddReferencedAssembly(asm);
            }

            foreach (var asmName in this.GetClientAssemblies())
            {
                code.AddReferencedAssembly(asmName);
            }

            return code;
        }

        /// <summary>
        /// Gets the assemblies from the workspace
        /// </summary>
        /// <returns>list of assemblies</returns>
        protected virtual IEnumerable<string> GetClientAssemblies()
        {
            List<string> assemblies = new List<string>();
            foreach (var asm in this.workspace.Assemblies)
            {
                assemblies.Add(asm.FileName);
            }

            return assemblies;
        }

        /// <summary>
        /// Get the Client ContextType
        /// </summary>
        /// <returns>Returns the Type of the Context</returns>
        protected virtual Type GetClientContextType()
        {
            return this.workspace.ContextType;
        }

        private void ThrowSkippedExceptionIfQueryExpressionNotSupported(QueryExpression expression)
        {
            // Uri will always work, Client linq has cases where it will not
            if (!this.IsUri)
            {
                ODataUriToClientLinqReplacingVisitor visitor = new ODataUriToClientLinqReplacingVisitor();
                visitor.ReplaceExpression(expression);

                if (visitor.InvalidClientQuery == true)
                {
                    throw new TestSkippedException(string.Format(CultureInfo.InvariantCulture, "Invalid Query '{0}', query not supported using Linq to Astoria client", expression.ToString()));
                }
            }
        }

        /// <summary>
        /// Class visits QueryExpression and indicates whether this expression is valid for a client
        /// query test
        /// </summary>
        private class ODataUriToClientLinqReplacingVisitor : LinqToAstoriaExpressionReplacingVisitor
        {
            internal ODataUriToClientLinqReplacingVisitor()
            {
                this.InvalidClientQuery = false;
            }

            internal bool InvalidClientQuery { get; private set; }

            /// <summary>
            /// Overrides the Visit expression and finds and analyzes property expressions to see if the KeyExpression is before
            /// If it is then this is an invalid query for the client
            /// </summary>
            /// <param name="expression">Expression to be analyzed</param>
            /// <returns>The same expression</returns>
            public override QueryExpression Visit(QueryPropertyExpression expression)
            {
                var keyExpression = expression.Instance as LinqToAstoriaKeyExpression;
                if (keyExpression == null)
                {
                    // Also check if a KeyExpression followed by an As operator is before the current expression.
                    var asExpression = expression.Instance as QueryAsExpression;
                    if (asExpression != null)
                    {
                        keyExpression = asExpression.Source as LinqToAstoriaKeyExpression;
                    }
                }

                if (keyExpression != null)
                {
                    this.InvalidClientQuery = true;
                }

                return base.Visit(expression);
            }
        }
    }
}