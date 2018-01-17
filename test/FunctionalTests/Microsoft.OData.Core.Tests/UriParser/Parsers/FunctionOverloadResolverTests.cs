//---------------------------------------------------------------------
// <copyright file="FunctionOverloadResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    /// <summary>
    /// Test code for testing the function overload resolver.
    /// TODO: Analyze and add coverage
    /// </summary>
    public class FunctionOverloadResolverTests
    {
        private static readonly ODataUriResolver DefaultUriResolver = ODataUriResolver.GetUriResolver(null);

        [Fact]
        public void ModelWithMultipleOverloadedActionsShouldThrow()
        {
            var model = ModelBuildingHelpers.GetModelWithMixedActionsAndFunctionsWithSameName();
            var parameters = new string[0];

            IEdmOperationImport function;
            Action test = () => FunctionOverloadResolver.ResolveOperationImportFromList("Foo", parameters, model, out function, DefaultUriResolver);
            test.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionOverloadResolver_MultipleOperationImportOverloads("Foo"));
        }

        [Fact]
        public void ActionWithParametersInURLShouldThrowError()
        {
            var model = HardCodedTestModel.TestModel;
            var parameters = new string[] { "123 bob st." };

            IEdmOperation function;
            Action test = () => FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Move", parameters, HardCodedTestModel.GetPersonType(), model, out function, DefaultUriResolver);
            test.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates("Fully.Qualified.Namespace.Move"));
        }

        [Fact]
        public void ActionWithMultipleOverloadsForTheSameBindingParameter()
        {
            var model = ModelBuildingHelpers.GetModelWithIllegalActionOverloads();
            var parameters = new string[] { };

            IEdmOperationImport function;
            Action resolve = () => FunctionOverloadResolver.ResolveOperationImportFromList("Action", parameters, model, out function, DefaultUriResolver);
            resolve.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionOverloadResolver_MultipleActionImportOverloads("Action"));
        }

        [Fact]
        public void SingleFunctionSuccessfullyResolved()
        {
            var parameters = new[] { "inOffice" };

            IEdmOperation function;
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.HasDog", parameters, HardCodedTestModel.GetPersonType(), HardCodedTestModel.TestModel, out function, DefaultUriResolver).Should().BeTrue();
            function.Should().BeSameAs(HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters());
        }

        [Fact]
        public void SingleActionSuccessfullyResolved()
        {
            var parameters = new string[] { };

            IEdmOperation function;
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Move", parameters, HardCodedTestModel.GetEmployeeType(), HardCodedTestModel.TestModel, out function, DefaultUriResolver).Should().BeTrue();
            function.Should().BeSameAs(HardCodedTestModel.GetMoveOverloadForEmployee());
        }

        [Fact]
        public void ActionSuccessfullyResolvedWithDerivedType()
        {
            var parameters = new string[] { };

            IEdmOperation function;
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Move", parameters, HardCodedTestModel.GetManagerType(), HardCodedTestModel.TestModel, out function, DefaultUriResolver).Should().BeTrue();
            function.Should().BeSameAs(HardCodedTestModel.GetMoveOverloadForEmployee());
        }

        [Fact]
        public void SingleServiceOperationSuccessfullyResolved()
        {
            var parameters = new string[] { };

            IEdmOperationImport function;
            FunctionOverloadResolver.ResolveOperationImportFromList("GetCoolPeople", parameters, HardCodedTestModel.TestModel, out function, DefaultUriResolver).Should().BeTrue();
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

            // Add function with two required and two optional parameters.
            var functionWithTwoRequiredOneOptionalParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference);
            functionWithTwoRequiredOneOptionalParameter.AddParameter("Parameter", int32TypeReference);
            functionWithTwoRequiredOneOptionalParameter.AddParameter("Parameter2", int32TypeReference);
            functionWithTwoRequiredOneOptionalParameter.AddOptionalParameter("Parameter3", int32TypeReference);
            functionWithTwoRequiredOneOptionalParameter.AddOptionalParameter("Parameter4", int32TypeReference);
            model.AddElement(functionWithTwoRequiredOneOptionalParameter);

            // Add function with one required and one optional parameters.
            var functionWithOneRequiredOneOptionalParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference);
            functionWithOneRequiredOneOptionalParameter.AddParameter("Parameter1", int32TypeReference);
            functionWithOneRequiredOneOptionalParameter.AddOptionalParameter("Parameter3", int32TypeReference);
            model.AddElement(functionWithOneRequiredOneOptionalParameter);

            // Add function with one required and two optional parameters.
            var functionWithOneRequiredTwoOptionalParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference);
            functionWithOneRequiredTwoOptionalParameter.AddParameter("Parameter1", int32TypeReference);
            functionWithOneRequiredTwoOptionalParameter.AddOptionalParameter("Parameter3", int32TypeReference);
            functionWithOneRequiredTwoOptionalParameter.AddOptionalParameter("Parameter4", int32TypeReference);
            model.AddElement(functionWithOneRequiredTwoOptionalParameter);

            // Add function with one required and two optional parameters.
            var functionWithOneRequiredThreeOptionalParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference, true, null, true);
            functionWithOneRequiredThreeOptionalParameter.AddParameter("BindingParameter", int32TypeReference);
            functionWithOneRequiredThreeOptionalParameter.AddParameter("Parameter1", int32TypeReference);
            functionWithOneRequiredThreeOptionalParameter.AddOptionalParameter("Parameter3", int32TypeReference);
            functionWithOneRequiredThreeOptionalParameter.AddOptionalParameter("Parameter4", int32TypeReference);
            functionWithOneRequiredThreeOptionalParameter.AddOptionalParameter("Parameter5", int32TypeReference);
            model.AddElement(functionWithOneRequiredThreeOptionalParameter);

            // Add function import.
            var container = new EdmEntityContainer("Fully.Qualified.Namespace", "Container");
            model.AddElement(container);
            var functionImportWithoutParameter = container.AddFunctionImport("FunctionImport", functionWithoutParameter);
            var functionImportWithParameter = container.AddFunctionImport("FunctionImport", functionWithParameter);
            var functionImportWithTwoParameter = container.AddFunctionImport("FunctionImport", functionWithTwoParameter);
            var functionImportWithTwoRequiredOneOptionalParameter = container.AddFunctionImport("FunctionImport", functionWithTwoRequiredOneOptionalParameter);
            var functionImportWithOneRequiredOneOptionalParameter = container.AddFunctionImport("FunctionImport", functionWithOneRequiredOneOptionalParameter);
            var functionImportWithOneRequiredTwoOptionalParameter = container.AddFunctionImport("FunctionImport", functionWithOneRequiredTwoOptionalParameter);
            var functionImportWithOneRequiredThreeOptionalParameter = container.AddFunctionImport("FunctionImport", functionWithOneRequiredThreeOptionalParameter);

            IEdmOperationImport functionImport;

            // Resolve overload function import without parameter.
            var parameters = new string[] { };
            FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver).Should().BeTrue();
            functionImport.Should().BeSameAs(functionImportWithoutParameter);

            // Resolve overload function import with parameter.
            parameters = new string[] { "Parameter" };
            FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver).Should().BeTrue();
            functionImport.Should().BeSameAs(functionImportWithParameter);

            // Resolve overload function import with parameter.
            parameters = new string[] { "Parameter", "Parameter2" };
            FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver).Should().BeTrue();
            functionImport.Should().BeSameAs(functionImportWithTwoParameter);

            // Resolve overload function with two required and one optional parameter.
            parameters = new string[] { "Parameter", "Parameter2", "Parameter3" };
            FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver).Should().BeTrue();
            functionImport.Should().BeSameAs(functionImportWithTwoRequiredOneOptionalParameter);

            // Resolve overload function with one required and two optional parameters.
            parameters = new string[] { "Parameter1", "Parameter3" };
            FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver).Should().BeTrue();
            functionImport.Should().BeSameAs(functionImportWithOneRequiredOneOptionalParameter);

            // Resolve overload function with one required and three optional parameters (one omitted).
            parameters = new string[] { "Parameter1", "Parameter3", "Parameter5" };
            FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver).Should().BeTrue();
            functionImport.Should().BeSameAs(functionImportWithOneRequiredThreeOptionalParameter);

            // Raise exception if more than one match.
            parameters = new string[] { "Parameter1", "Parameter4" };
            Action resolve = () => FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver);
            resolve.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionOverloadResolver_MultipleOperationImportOverloads("FunctionImport"));

        }

        [Fact]
        public void OverloadBoundFunctionsSuccessfullyResolved()
        {
            var model = new EdmModel();

            // Add function without parameter.
            var int32TypeReference = EdmCoreModel.Instance.GetInt32(false);
            var functionWithoutParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference, true, null, true);
            functionWithoutParameter.AddParameter("BindingParameter", int32TypeReference);
            model.AddElement(functionWithoutParameter);

            // Add function with parameter.
            var functionWithParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference, true, null, true);
            functionWithParameter.AddParameter("BindingParameter", int32TypeReference);
            functionWithParameter.AddParameter("Parameter", int32TypeReference);
            model.AddElement(functionWithParameter);

            // Add function with two parameters.
            var functionWithTwoParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference, true, null, true);
            functionWithTwoParameter.AddParameter("BindingParameter", int32TypeReference);
            functionWithTwoParameter.AddParameter("Parameter", int32TypeReference);
            functionWithTwoParameter.AddParameter("Parameter2", int32TypeReference);
            model.AddElement(functionWithTwoParameter);

            // Add function with two required and two optional parameters.
            var functionWithTwoRequiredOneOptionalParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference, true, null, true);
            functionWithTwoRequiredOneOptionalParameter.AddParameter("BindingParameter", int32TypeReference);
            functionWithTwoRequiredOneOptionalParameter.AddParameter("Parameter", int32TypeReference);
            functionWithTwoRequiredOneOptionalParameter.AddParameter("Parameter2", int32TypeReference);
            functionWithTwoRequiredOneOptionalParameter.AddOptionalParameter("Parameter3", int32TypeReference);
            functionWithTwoRequiredOneOptionalParameter.AddOptionalParameter("Parameter4", int32TypeReference);
            model.AddElement(functionWithTwoRequiredOneOptionalParameter);

            // Add function with one required and one optional parameters.
            var functionWithOneRequiredOneOptionalParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference, true, null, true);
            functionWithOneRequiredOneOptionalParameter.AddParameter("BindingParameter", int32TypeReference);
            functionWithOneRequiredOneOptionalParameter.AddParameter("Parameter1", int32TypeReference);
            functionWithOneRequiredOneOptionalParameter.AddOptionalParameter("Parameter3", int32TypeReference);
            model.AddElement(functionWithOneRequiredOneOptionalParameter);

            // Add function with one required and two optional parameters.
            var functionWithOneRequiredTwoOptionalParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference, true, null, true);
            functionWithOneRequiredTwoOptionalParameter.AddParameter("BindingParameter", int32TypeReference);
            functionWithOneRequiredTwoOptionalParameter.AddParameter("Parameter1", int32TypeReference);
            functionWithOneRequiredTwoOptionalParameter.AddOptionalParameter("Parameter3", int32TypeReference);
            functionWithOneRequiredTwoOptionalParameter.AddOptionalParameter("Parameter4", int32TypeReference);
            model.AddElement(functionWithOneRequiredTwoOptionalParameter);

            // Add function with one required and two optional parameters.
            var functionWithOneRequiredThreeOptionalParameter = new EdmFunction("Fully.Qualified.Namespace", "Function", int32TypeReference, true, null, true);
            functionWithOneRequiredThreeOptionalParameter.AddParameter("BindingParameter", int32TypeReference);
            functionWithOneRequiredThreeOptionalParameter.AddParameter("Parameter1", int32TypeReference);
            functionWithOneRequiredThreeOptionalParameter.AddOptionalParameter("Parameter3", int32TypeReference);
            functionWithOneRequiredThreeOptionalParameter.AddOptionalParameter("Parameter4", int32TypeReference);
            functionWithOneRequiredThreeOptionalParameter.AddOptionalParameter("Parameter5", int32TypeReference);
            model.AddElement(functionWithOneRequiredThreeOptionalParameter);

            IEdmOperation function;

            // Resolve overload function without parameter.
            var parameters = new string[] { };
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver).Should().BeTrue();
            function.Should().BeSameAs(functionWithoutParameter);

            // Resolve overload function with parameter.
            parameters = new string[] { "Parameter" };
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver).Should().BeTrue();
            function.Should().BeSameAs(functionWithParameter);

            // Resolve overload function with parameter.
            parameters = new string[] { "Parameter", "Parameter2" };
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver).Should().BeTrue();
            function.Should().BeSameAs(functionWithTwoParameter);

            // Resolve overload function with two required and one optional parameter.
            parameters = new string[] { "Parameter", "Parameter2", "Parameter3" };
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver).Should().BeTrue();
            function.Should().BeSameAs(functionWithTwoRequiredOneOptionalParameter);

            // Resolve overload function with one required and two optional parameters (one omitted).
            parameters = new string[] { "Parameter1", "Parameter3" };
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver).Should().BeTrue();
            function.Should().BeSameAs(functionWithOneRequiredOneOptionalParameter);

            // Resolve overload function with one required and three optional parameters (one omitted).
            parameters = new string[] { "Parameter1", "Parameter3", "Parameter5" };
            FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver).Should().BeTrue();
            function.Should().BeSameAs(functionWithOneRequiredThreeOptionalParameter);

            // Raise exception if more than one match.
            parameters = new string[] { "Parameter1", "Parameter4" };
            Action resolve = () => FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver).Should().BeTrue();
            resolve.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionOverloadResolver_NoSingleMatchFound("Fully.Qualified.Namespace.Function", string.Join(",", parameters)));

        }
    }
}

