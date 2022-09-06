//---------------------------------------------------------------------
// <copyright file="ODataWorkspaceBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData
{
    using System;
    using System.CodeDom;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.EntityModel.Edm;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// ODataServiceDocument Builder
    /// </summary>
    public class ODataWorkspaceBuilder : IWorkspaceBuilder
    {
        private static string[] referenceAssemblies = 
        {
            "mscorlib.dll",
            "system.dll",
            "System.Core.dll",
            DataFxAssemblyRef.File.DataServicesClient,
            DataFxAssemblyRef.File.ODataLib,
            "System.Net.dll",
            "System.Xml.dll",
        };

        /// <summary>
        /// Gets or sets the language used to build the code.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProgrammingLanguageStrategy Language { get; set; }

        /// <summary>
        /// Gets or sets the primitive type resolver
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IEntityModelPrimitiveTypeResolver PrimitiveTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets the code generator for generating CLR types from an EntityModelSchema.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IObjectLayerCodeGenerator ObjectLayerCodeGenerator { get; set; }

        /// <summary>
        /// Gets or sets the test case's query evaluator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryEvaluationStrategy QueryEvaluationStrategy { get; set; }

        /// <summary>
        /// Returns a Workspace
        /// </summary>
        /// <param name="modelSchema">model schema</param>
        /// <returns> An OData Workspace </returns>
        public Workspace BuildWorkspace(EntityModelSchema modelSchema)
        {
            // Note this must be done after the binary fixup, as we replace binary keys with integer ones, but before the provider-specific fixup
            this.PrimitiveTypeResolver.ResolveProviderTypes(modelSchema, new EdmDataTypeResolver());
            new SetDefaultCollectionTypesFixup().Fixup(modelSchema);
            new SetDefaultDataServiceConfigurationBehaviors().Fixup(modelSchema);
            new RemoveHigherVersionFeaturesFixup(DataServiceProtocolVersion.V4).Fixup(modelSchema);
            ODataTestWorkspace workspace = new ODataTestWorkspace();

            workspace.ConceptualModel = modelSchema;
            workspace.ObjectLayerAssembly = this.GenerateObjectLayer(workspace, modelSchema);
            return workspace;
        }

        /// <summary>
        /// Generates an assembly that contains the object layer types for the given EntityModelSchema
        /// </summary>
        /// <param name="workspace">test workspace under construction</param> 
        /// <param name="schema">The schema under test</param>
        /// <returns>An assembly that contains the object layer types for the given EntityModelSchema</returns>
        private Assembly GenerateObjectLayer(ODataTestWorkspace workspace, EntityModelSchema schema)
        {
            CodeCompileUnit objectLayerCompileUnit = new CodeCompileUnit();
            this.ObjectLayerCodeGenerator.GenerateObjectLayer(objectLayerCompileUnit, schema);
            string objectLayerCode;
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.Language.CreateCodeGenerator().GenerateCodeFromCompileUnit(objectLayerCompileUnit, writer, null);
                objectLayerCode = writer.ToString();
            }

            string outputFilePath = Guid.NewGuid().ToString();
            outputFilePath = outputFilePath + ".dll";
            this.Language.CompileAssemblyFromSource(outputFilePath, new[] { objectLayerCode }, referenceAssemblies);
            var assembly = AssemblyHelpers.LoadAssembly(outputFilePath, referenceAssemblies);
            return assembly;
        }
    }
}
