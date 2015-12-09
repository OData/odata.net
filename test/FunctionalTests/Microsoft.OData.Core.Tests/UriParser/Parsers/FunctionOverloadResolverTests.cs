//---------------------------------------------------------------------
// <copyright file="FunctionOverloadResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Metadata;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    /// <summary>
    /// Test code for testing the function overload resolver.
    /// TODO: Analyze and add coverage
    /// </summary>
    public class FunctionOverloadResolverTests
    {
        // TryResolveFunction

        [Fact]
        public void ModelWithMultipleOverloadedActionsShouldThrow()
        {
            var model = ModelBuildingHelpers.GetModelWithMixedActionsAndFunctionsWithSameName();
            var parameters = new string[0];

            IEdmOperationImport function;
            Action test = () => FunctionOverloadResolver.ResolveOperationImportFromList("Foo", parameters, model, out function, ODataUriResolver.Default);
            test.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionOverloadResolver_MultipleOperationImportOverloads("Foo"));
        }

        [Fact]
        public void ActionWithParametersInURLShouldThrowError()
        {
            var model = HardCodedTestModel.TestModel;
            var parameters = new string[] { "123 bob st." };

            IEdmOperation function;
            Action test = () => FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Move", parameters, HardCodedTestModel.GetPersonType(), model, out function, ODataUriResolver.Default);
            test.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates("Fully.Qualified.Namespace.Move"));
        }

        [Fact]
        public void ActionWithMultipleOverloadsForTheSameBindingParameter()
        {
            var model = ModelBuildingHelpers.GetModelWithIllegalActionOverloads();
            var parameters = new string[] { };

            IEdmOperationImport function;
            Action resolve = () => FunctionOverloadResolver.ResolveOperationImportFromList("Action", parameters, model, out function, ODataUriResolver.Default);
            resolve.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionOverloadResolver_MultipleActionImportOverloads("Action"));
        }

        [Fact]
        public void SingleFunctionSuccessfullyResolved()
        {
            var parameters = new[] { "inOffice" };

            IEdmOperation function;
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.HasDog", parameters, HardCodedTestModel.GetPersonType(), HardCodedTestModel.TestModel, out function, ODataUriResolver.Default).Should().BeTrue();
            function.Should().BeSameAs(HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters());
        }

        [Fact]
        public void SingleActionSuccessfullyResolved()
        {
            var parameters = new string[] { };

            IEdmOperation function;
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Move", parameters, HardCodedTestModel.GetEmployeeType(), HardCodedTestModel.TestModel, out function, ODataUriResolver.Default).Should().BeTrue();
            function.Should().BeSameAs(HardCodedTestModel.GetMoveOverloadForEmployee());
        }

        [Fact]
        public void ActionSuccessfullyResolvedWithDerivedType()
        {
            var parameters = new string[] { };

            IEdmOperation function;
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Move", parameters, HardCodedTestModel.GetManagerType(), HardCodedTestModel.TestModel, out function, ODataUriResolver.Default).Should().BeTrue();
            function.Should().BeSameAs(HardCodedTestModel.GetMoveOverloadForEmployee());
        }

        [Fact]
        public void SingleServiceOperationSuccessfullyResolved()
        {
            var parameters = new string[] { };

            IEdmOperationImport function;
            FunctionOverloadResolver.ResolveOperationImportFromList("GetCoolPeople", parameters, HardCodedTestModel.TestModel, out function, ODataUriResolver.Default).Should().BeTrue();
            function.Should().BeSameAs(HardCodedTestModel.GetFunctionImportForGetCoolPeople());
        }

        [Fact]
        public void OverloadServiceOperationSuccessfullyResolved()
        {
            var model = new EdmModel();

            // Add function without parameter.
            var int32TypeReference = EdmCoreModel.Instance.GetInt32(false);
            var functionWithoutParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference);
            model.AddElement(functionWithoutParameter);

            // Add function with parameter.
            var functionWithParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference);
            functionWithParameter.AddParameter("Parameter", int32TypeReference);
            model.AddElement(functionWithParameter);

            // Add function with two parameters.
            var functionWithTwoParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference);
            functionWithTwoParameter.AddParameter("Parameter", int32TypeReference);
            functionWithTwoParameter.AddParameter("Parameter2", int32TypeReference);
            model.AddElement(functionWithTwoParameter);

            // Add function import.
            var container = new EdmEntityContainer("Fully.Qualified.Namespace", "Container");
            model.AddElement(container);
            var functionImportWithoutParameter = container.AddFunctionImport("FunctionImport", functionWithoutParameter);
            var functionImportWithParameter = container.AddFunctionImport("FunctionImport", functionWithParameter);
            var functionImportWithTwoParameter = container.AddFunctionImport("FunctionImport", functionWithTwoParameter);

            IEdmOperationImport functionImport;

            // Resolve overload function import without parameter.
            var parameters = new string[] { };
            FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, ODataUriResolver.Default).Should().BeTrue();
            functionImport.Should().BeSameAs(functionImportWithoutParameter);

            // Resolve overload function import with parameter.
            parameters = new string[] { "Parameter" };
            FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, ODataUriResolver.Default).Should().BeTrue();
            functionImport.Should().BeSameAs(functionImportWithParameter);

            // Resolve overload function import with parameter.
            parameters = new string[] { "Parameter", "Parameter2" };
            FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, ODataUriResolver.Default).Should().BeTrue();
            functionImport.Should().BeSameAs(functionImportWithTwoParameter);
        }
    }
}

