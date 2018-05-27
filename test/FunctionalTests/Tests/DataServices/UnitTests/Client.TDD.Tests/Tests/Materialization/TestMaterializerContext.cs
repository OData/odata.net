//---------------------------------------------------------------------
// <copyright file="TestMaterializerContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Materialization;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Implementation of a Test Materializer Context that is used to test non entity materialization
    /// </summary>
    internal class TestMaterializerContext : IODataMaterializerContext
    {
        public TestMaterializerContext()
        {
            this.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException;
            this.ResponsePipeline = new DataServiceClientResponsePipelineConfiguration(this);
            this.Model = new ClientEdmModel(ODataProtocolVersion.V4);
            this.Context = new DataServiceContext();
        }

        public Func<Type, string, ClientTypeAnnotation> ResolveTypeForMaterializationOverrideFunc { get; set; }

        public UndeclaredPropertyBehavior UndeclaredPropertyBehavior { get; set; }

        public ClientEdmModel Model { get; set; }

        public DataServiceClientResponsePipelineConfiguration ResponsePipeline { get; set; }

        public ClientTypeAnnotation ResolveTypeForMaterialization(Type expectedType, string wireTypeName)
        {
            if (this.ResolveTypeForMaterializationOverrideFunc != null)
            {
                return ResolveTypeForMaterializationOverrideFunc(expectedType, wireTypeName);
            }

            var edmType = this.Model.GetOrCreateEdmType(expectedType);
            return new ClientTypeAnnotation(edmType, expectedType, expectedType.FullName, this.Model);
        }

        public IEdmType ResolveExpectedTypeForReading(Type expectedType)
        {
            return this.Model.GetOrCreateEdmType(expectedType);
        }

        public DataServiceContext Context { get; set; }
    }
}
