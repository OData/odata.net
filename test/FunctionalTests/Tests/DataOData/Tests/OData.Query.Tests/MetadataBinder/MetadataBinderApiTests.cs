//---------------------------------------------------------------------
// <copyright file="MetadataBinderApiTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.MetadataBinder
{
    #region Namespaces

    using System;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Query;
    using Microsoft.OData.Core.Query.SemanticAst;
    using Microsoft.OData.Core.Query.SyntacticAst;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Query.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the MetadataBinder on a filter expression.
    /// </summary>
    [TestClass, TestCase]
    public class MetadataBinderApiTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency(IsRequired = true)]
        public UntypedDataServiceProviderFactory UntypedDataServiceProviderFactory { get; set; }

        [TestMethod, Variation(Description = "Verifies correct behavior of the default BindExtension implementation")]
        public void BindExtensionTest()
        {
            var metadata = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.UntypedDataServiceProviderFactory);

            MetadataBinder binder = new MetadataBinder(metadata);
            SyntacticTree syntax = new SyntacticTree(new TestExtensionQueryToken(), null, null, null, null, null, null, null, null, null);

            this.Assert.ExpectedException<ODataException>(
                () => binder.BindQuery(syntax),
                "An unsupported extension query token was found.",
                "BindExtensionTest");
        }

        internal class TestExtensionQueryToken : QueryToken
        {
            public override QueryTokenKind Kind
            {
                get { return QueryTokenKind.Extension; }
            }

            public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
            {
                throw new NotImplementedException();
            }
        }
    }
}
