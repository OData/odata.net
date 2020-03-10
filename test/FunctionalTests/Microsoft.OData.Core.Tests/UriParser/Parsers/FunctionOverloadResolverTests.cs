//---------------------------------------------------------------------
// <copyright file="FunctionOverloadResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
            test.Throws<ODataException>(ODataErrorStrings.FunctionOverloadResolver_MultipleOperationImportOverloads("Foo"));
        }

        [Fact]
        public void ActionWithParametersInURLShouldThrowError()
        {
            var model = HardCodedTestModel.TestModel;
            var parameters = new string[] { "123 bob st." };

            IEdmOperation function;
            Action test = () => FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Move", parameters, HardCodedTestModel.GetPersonType(), model, out function, DefaultUriResolver);
            test.Throws<ODataException>(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates("Fully.Qualified.Namespace.Move"));
        }

        [Fact]
        public void ActionWithMultipleOverloadsForTheSameBindingParameter()
        {
            var model = ModelBuildingHelpers.GetModelWithIllegalActionOverloads();
            var parameters = new string[] { };

            IEdmOperationImport function;
            Action resolve = () => FunctionOverloadResolver.ResolveOperationImportFromList("Action", parameters, model, out function, DefaultUriResolver);
            resolve.Throws<ODataException>(ODataErrorStrings.FunctionOverloadResolver_MultipleActionImportOverloads("Action"));
        }

        [Fact]
        public void OperationOverloadsWithSameNameWithoutBindingType()
        {
            var model = ModelBuildingHelpers.GetModelWithOperationOverloadsWithSameName();
            var parameters = new string[] {"p1", "p2"};

            IEdmOperation operation;
            Action resolve = () => FunctionOverloadResolver.ResolveOperationFromList("Test.Function", parameters, null, model, out operation, DefaultUriResolver);
            resolve.Throws<ODataException>(ODataErrorStrings.FunctionOverloadResolver_NoSingleMatchFound("Test.Function", "p1,p2"));
        }

       [Fact]
        public void SingleFunctionSuccessfullyResolved()
        {
            var parameters = new[] { "inOffice" };

            IEdmOperation function;
            var result = FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.HasDog", parameters, HardCodedTestModel.GetPersonType(), HardCodedTestModel.TestModel, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters(), function);
        }

        [Fact]
        public void SingleActionSuccessfullyResolved()
        {
            var parameters = new string[] { };

            IEdmOperation function;
            var result = FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Move", parameters, HardCodedTestModel.GetEmployeeType(), HardCodedTestModel.TestModel, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(HardCodedTestModel.GetMoveOverloadForEmployee(), function);
        }

        [Fact]
        public void ActionSuccessfullyResolvedWithDerivedType()
        {
            var parameters = new string[] { };

            IEdmOperation function;
            var result = FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Move", parameters, HardCodedTestModel.GetManagerType(), HardCodedTestModel.TestModel, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(HardCodedTestModel.GetMoveOverloadForEmployee(), function);
        }

        [Fact]
        public void SingleServiceOperationSuccessfullyResolved()
        {
            var parameters = new string[] { };

            IEdmOperationImport function;
            var result = FunctionOverloadResolver.ResolveOperationImportFromList("GetCoolPeople", parameters, HardCodedTestModel.TestModel, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(HardCodedTestModel.GetFunctionImportForGetCoolPeople(), function);
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
            var result = FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionImportWithoutParameter, functionImport);

            // Resolve overload function import with parameter.
            parameters = new string[] { "Parameter" };
            result = FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionImportWithParameter, functionImport);

            // Resolve overload function import with parameter.
            parameters = new string[] { "Parameter", "Parameter2" };
            result = FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver);
            Assert.Same(functionImportWithTwoParameter, functionImport);

            // Resolve overload function with two required and one optional parameter.
            parameters = new string[] { "Parameter", "Parameter2", "Parameter3" };
            result = FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionImportWithTwoRequiredOneOptionalParameter, functionImport);

            // Resolve overload function with one required and two optional parameters.
            parameters = new string[] { "Parameter1", "Parameter3" };
            result = FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionImportWithOneRequiredOneOptionalParameter, functionImport);

            // Resolve overload function with one required and three optional parameters (one omitted).
            parameters = new string[] { "Parameter1", "Parameter3", "Parameter5" };
            result = FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionImportWithOneRequiredThreeOptionalParameter, functionImport);

            // Raise exception if more than one match.
            parameters = new string[] { "Parameter1", "Parameter4" };
            Action resolve = () => FunctionOverloadResolver.ResolveOperationImportFromList("FunctionImport", parameters, model, out functionImport, DefaultUriResolver);
            resolve.Throws<ODataException>(ODataErrorStrings.FunctionOverloadResolver_MultipleOperationImportOverloads("FunctionImport"));
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
            var result = FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionWithoutParameter, function);

            // Resolve overload function with parameter.
            parameters = new string[] { "Parameter" };
            result = FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionWithParameter, function);

            // Resolve overload function with parameter.
            parameters = new string[] { "Parameter", "Parameter2" };
            result = FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionWithTwoParameter, function);

            // Resolve overload function with two required and one optional parameter.
            parameters = new string[] { "Parameter", "Parameter2", "Parameter3" };
            result = FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionWithTwoRequiredOneOptionalParameter, function);

            // Resolve overload function with one required and two optional parameters (one omitted).
            parameters = new string[] { "Parameter1", "Parameter3" };
            result = FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionWithOneRequiredOneOptionalParameter, function);

            // Resolve overload function with one required and three optional parameters (one omitted).
            parameters = new string[] { "Parameter1", "Parameter3", "Parameter5" };
            result = FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionWithOneRequiredThreeOptionalParameter, function);

            // Raise exception if more than one match.
            parameters = new string[] { "Parameter1", "Parameter4" };
            Action resolve = () => Assert.True(FunctionOverloadResolver.ResolveOperationFromList("Fully.Qualified.Namespace.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver));
            resolve.Throws<ODataException>(ODataErrorStrings.FunctionOverloadResolver_NoSingleMatchFound("Fully.Qualified.Namespace.Function", string.Join(",", parameters)));
        }

        [Fact]
        public void OverloadBoundFunctionsWithAllOptionalParametersSuccessfullyResolved()
        {
            var model = new EdmModel();

            var int32TypeReference = EdmCoreModel.Instance.GetInt32(false);

            // Add function with all optional parameters.
            var functionWithAllOptionalParameters = new EdmFunction("NS", "Function", int32TypeReference, true, null, true);
            functionWithAllOptionalParameters.AddParameter("BindingParameter", int32TypeReference);
            functionWithAllOptionalParameters.AddOptionalParameter("Parameter1", int32TypeReference);
            functionWithAllOptionalParameters.AddOptionalParameter("Parameter2", int32TypeReference);
            model.AddElement(functionWithAllOptionalParameters);

            // Add an overload function without parameter.
            var functionWithoutParameter = new EdmFunction("NS", "Function", int32TypeReference, true, null, true);
            functionWithoutParameter.AddParameter("BindingParameter", int32TypeReference);
            model.AddElement(functionWithoutParameter);

            IEdmOperation function;

            // Resolve overload function without parameter. Be noted, it picks the one with no optional parameters.
            var parameters = new string[] { };
            var result = FunctionOverloadResolver.ResolveOperationFromList("NS.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionWithoutParameter, function);

            // Resolve overload function with one bound and two optional parameters (one omitted).
            parameters = new string[] { "Parameter1" };
            result = FunctionOverloadResolver.ResolveOperationFromList("NS.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionWithAllOptionalParameters, function);

            // Resolve overload function with one bound and two optional parameters (both specified).
            parameters = new string[] { "Parameter1", "Parameter2" };
            result = FunctionOverloadResolver.ResolveOperationFromList("NS.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver);
            Assert.True(result);
            Assert.Same(functionWithAllOptionalParameters, function);

            // Return false if no match.
            parameters = new string[] { "Parameter1", "Parameter4" };
            result = FunctionOverloadResolver.ResolveOperationFromList("NS.Function", parameters, int32TypeReference.Definition, model, out function, DefaultUriResolver);
            Assert.False(result);
        }
    }
}

