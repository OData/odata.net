//---------------------------------------------------------------------
// <copyright file="BinderErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.Test.Taupo.Common;
using Microsoft.Test.Taupo.Execution;
using Microsoft.Test.Taupo.OData.Common;
using Microsoft.Test.Taupo.OData.Contracts;
using Microsoft.Test.Taupo.OData.Query.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.Test.Taupo.OData.Query.Tests.MetadataBinder
{
    /// <summary>
    /// Various error tests for the MetadataBinder.
    /// </summary>
    [TestClass, TestCase]
    public class BinderErrorTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency(IsRequired = true)]
        public UntypedDataServiceProviderFactory UntypedDataServiceProviderFactory { get; set; }

        [TestMethod]
        public void UnsupportedTokenBinderErrorTest()
        {
            IEdmModel model = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.UntypedDataServiceProviderFactory);
            var binder = new ErrorMetadataBinder(model);
            var token = new LiteralToken("d");
            Action action = () => binder.Bind(token);
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_BoundNodeCannotBeNull(token.Kind));
        }

        /* github issue: https://github.com/OData/odata.net/issues/864
        [TestMethod]
        public void KeyLookupBinderErrorTest()
        {
            IEdmModel model = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.UntypedDataServiceProviderFactory);

            var testCases = new[]{
                new {
                        CaseUri = new Uri("Customers(ID=1,ID=2)", UriKind.Relative),
                        ExpectedMessage = "A key property 'ID' was found twice in a key lookup. Each key property can be specified just once in a key lookup."
                    },
                new {
                        CaseUri = new Uri("Customers(Name='Bob')", UriKind.Relative),
                        ExpectedMessage = "Property 'Name' is not declared on type 'TestNS.Customer' or is not a key property. Only key properties can be used in key lookups."
                    },
                new {
                        CaseUri = new Uri("Customers(UndeclaredProperty='Bob')", UriKind.Relative),
                        ExpectedMessage = "Property 'UndeclaredProperty' is not declared on type 'TestNS.Customer' or is not a key property. Only key properties can be used in key lookups.",
                    },
                new {
                        CaseUri = new Uri("MultiKeys(KeyA='Bob')", UriKind.Relative),
                        ExpectedMessage = "A key lookup on type 'TestNS.MultiKey' didn't specify values for all key properties. All key properties must be specified in a key lookup.",
                    },
                new {
                        CaseUri = new Uri("MultiKeys('Bob')", UriKind.Relative),
                        ExpectedMessage = "An unnamed key value was used in a key lookup on a type 'TestNS.MultiKey' which has more than one key property. Unnamed key value can only be used on a type with one key property.",
                    },
                new {
                        CaseUri = new Uri("Customers(ID='Bob')", UriKind.Relative),
                        ExpectedMessage = "Expression of type 'Edm.String' cannot be converted to type 'Edm.Int32'.",
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases, (testCase) =>
                {
                    var parser = new ODataUriParser(model, testCase.CaseUri);
                    Action action = () => parser.ParsePath();
                    action.ShouldThrow<ODataException>().WithMessage(testCase.ExpectedMessage);
                });
        }
        */

        internal sealed class ErrorMetadataBinder : Microsoft.OData.UriParser.MetadataBinder
        {
            public ErrorMetadataBinder(IEdmModel model)
                : base(new BindingState(new ODataUriParserConfiguration(model)))
            {
            }

            protected override QueryNode BindLiteral(LiteralToken literalToken)
            {
                return null;
            }
        }
    }
}
