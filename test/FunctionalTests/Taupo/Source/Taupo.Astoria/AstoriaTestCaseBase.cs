//---------------------------------------------------------------------
// <copyright file="AstoriaTestCaseBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Base class for all astoria tests.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The testcase base class has to be coupled with many classes")]
    public abstract class AstoriaTestCaseBase : ODataQueryTestCase
    {
        /// <summary>
        /// Gets or sets the current workspace
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AstoriaWorkspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets the repository helper
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAstoriaQueryRepositoryHelper AstoriaQueryRepositoryHelper { get; set; }

        /// <summary>
        /// Gets or sets the synchronizer to use for keeping the query evaluator in sync after updates
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAsyncDataSynchronizer DataSynchronizer { get; set; }

        /// <summary>
        /// Gets or sets the streams services
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IStreamsServices StreamsServices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use asynchronouse APIs
        /// </summary>
        [InjectTestParameter("Asynchronous", DefaultValueDescription = "False", HelpText = "Whether to use asynchronous API")]
        public bool Asynchronous { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to dispose of the Data Service
        /// </summary>
        [InjectTestParameter("SkipNamedStreamsDataPopulation", DefaultValueDescription = "false")]
        public bool SkipNamedStreamsDataPopulation { get; set; }

        /// <summary>
        /// Initialize the workspace with streams data
        /// </summary>
        public override void Init()
        {
            base.Init();

            if (!this.SkipNamedStreamsDataPopulation)
            {
                AsyncExecutionContext.EnqueueAsynchronousAction(c => this.StreamsServices.PopulateStreamsData(c));
            }
        }

        /// <summary>
        /// Configures the dependencies for the test case.
        /// </summary>
        /// <param name="container">The container (private to the test case).</param>
        protected override void ConfigureDependencies(DependencyInjectionContainer container)
        {
            base.ConfigureDependencies(container);
            AstoriaTestServices.ConfigureDependencies(container);
        }

        /// <summary>
        /// Returns an instance of the astoria-specific workspace builder
        /// </summary>
        /// <returns>The workspace builder</returns>
        protected override IWorkspaceBuilder GetWorkspaceBuilder()
        {
            return null;
        }
    }
}
