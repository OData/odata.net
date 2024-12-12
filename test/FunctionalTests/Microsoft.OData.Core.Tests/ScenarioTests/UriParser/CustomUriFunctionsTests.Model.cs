//---------------------------------------------------------------------
// <copyright file="CustomUriFunctionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests.UriParser;
using Xunit;
using Microsoft.OData.Core;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Tests the CustomUriFunctions class.
    /// </summary>
    public partial class CustomUriFunctionsTests
    {
        private FunctionSignatureWithReturnType lengthSignature = new FunctionSignatureWithReturnType(
            EdmCoreModel.Instance.GetInt32(false),
            EdmCoreModel.Instance.GetString(true));

        [Fact]
        public void AddCustomFunction_ForModel_ParmetersCannotBeNull()
        {
            // Model is null
            IEdmModel model = null;
            Action test = () => model.AddCustomUriFunction("my.MyNullCustomFunction", null);
            Assert.Throws<ArgumentNullException>("model", test);

            model = EdmCoreModel.Instance;
            // function name is null or empty
            test = () => model.AddCustomUriFunction(null, null);
            Assert.Throws<ArgumentNullException>("functionName", test);

            test = () => model.AddCustomUriFunction(string.Empty, null);
            Assert.Throws<ArgumentNullException>("functionName", test);

            // function signature is null
            test = () => model.AddCustomUriFunction("my.MyNullCustomFunction", null);
            Assert.Throws<ArgumentNullException>("functionSignature", test);
        }

        [Fact]
        public void AddCustomFunction_ForModel_CannotAddFunctionSignatureWithNullReturnType()
        {
            FunctionSignatureWithReturnType customFunctionSignatureWithNullReturnType = new FunctionSignatureWithReturnType(null, EdmCoreModel.Instance.GetInt32(false));
            Action test = () => EdmCoreModel.Instance.AddCustomUriFunction("my.customFunctionWithNoReturnType", customFunctionSignatureWithNullReturnType);
            Assert.Throws<ArgumentNullException>("functionSignatureWithReturnType must contain a return type", test);
        }

        [Fact]
        public void AddCustomFunction_ForModel_CannotAddFunctionWhichAlreadyExistsAsBuiltInWithSameFullSignature_AddAsOverload()
        {
            EdmModel model = new EdmModel();
            Action test = () => model.AddCustomUriFunction("length", lengthSignature);
            test.Throws<ODataException>(Error.Format(SRResources.CustomUriFunctions_AddCustomUriFunction_BuiltInExistsFullSignature, "length"));
        }

        [Fact]
        public void AddCustomFunction_ForModel_ShouldAddFunctionWhichAlreadyExistsAsBuiltInWithSameName_AddAsOverload()
        {
            EdmModel model = new EdmModel();
            string functionName = "length";

            FunctionSignatureWithReturnType customFunctionSignature =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false),
                EdmCoreModel.Instance.GetBoolean(false));

            // Add with 'addAsOverload' 'true'
            model.AddCustomUriFunction(functionName, customFunctionSignature);

            FunctionSignatureWithReturnType[] resultFunctionSignaturesWithReturnType = GetCustomFunctionSignaturesOrNull(model, functionName);

            // Assert
            Assert.NotNull(resultFunctionSignaturesWithReturnType);
            Assert.Single(resultFunctionSignaturesWithReturnType);
            Assert.Same(customFunctionSignature, resultFunctionSignaturesWithReturnType[0]);
        }

        // Existing Custom Function
        [Fact]
        public void AddCustomFunction_ForModel_CannotAddFunctionWithFullSignatureExistsAsCustomFunction()
        {
            EdmModel model = new EdmModel();
            string customFunctionName = "my.ExistingCustomFunction";

            // Prepare
            var existingCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
            model.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

            // Test
            var newCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
            Action addCustomFunction = () =>  model.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

            // Assert
            addCustomFunction.Throws<ODataException>(Error.Format(SRResources.CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists, customFunctionName));
        }

        [Fact]
        public void AddCustomFunction_ForModel_CannotAddFunctionWithFullSignatureExistsAsCustomFunction_AddAsOverload()
        {
            EdmModel model = new EdmModel();
            string customFunctionName = "my.ExistingCustomFunction";

            // Prepare
            var existingCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
            model.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

            // Test
            var newCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));

            Action addCustomFunction = () => model.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

            // Asserts
            addCustomFunction.Throws<ODataException>(Error.Format(SRResources.CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists, customFunctionName));
        }

        [Fact]
        public void AddCustomFunction_ForModel_CustomFunctionDoesntExist_ShouldAdd()
        {
            EdmModel model = new EdmModel();
            string customFunctionName = "my.NewCustomFunction";

            // New not existing custom function
            var newCustomFunctionSignature =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetInt32(false), EdmCoreModel.Instance.GetBoolean(false));
            model.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

            // Assert
            // Make sure both signatures exists
            FunctionSignatureWithReturnType[] customFunctionSignatures = GetCustomFunctionSignaturesOrNull(model, customFunctionName);

            Assert.Single(customFunctionSignatures);
            Assert.Same(newCustomFunctionSignature, customFunctionSignatures[0]);
        }

        [Fact]
        public void AddCustomFunction_ForModel_CustomFunctionDoesntExist_ShouldAdd_NoArgumnetsToFunctionSignature()
        {
            EdmModel model = new EdmModel();
            string customFunctionName = "my.NewCustomFunction";

            // New not existing custom function - function without any argumnets
            var newCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false));
            model.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

            // Assert
            // Make sure both signatures exists
            FunctionSignatureWithReturnType[] customFunctionSignatures = GetCustomFunctionSignaturesOrNull(model, customFunctionName);

            Assert.Single(customFunctionSignatures);
            Assert.Same(newCustomFunctionSignature, customFunctionSignatures[0]);
        }

        [Fact]
        public void AddCustomFunction_ForModel_CustomFunctionNameExistsButNotFullSignature_ShouldAddAsAnOverload()
        {
            EdmModel model = new EdmModel();
            string customFunctionName = "my.ExistingCustomFunction";

            // Prepare
            FunctionSignatureWithReturnType existingCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
            model.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

            //Test
            // Same name, but different signature
            var newCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetInt32(false), EdmCoreModel.Instance.GetBoolean(false));
            model.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

            // Assert
            // Make sure both signatures exists
            bool areSiganturesAdded = GetCustomFunctionSignaturesOrNull(model,customFunctionName).All(x => x.Equals(existingCustomFunctionSignature) || x.Equals(newCustomFunctionSignature));

            Assert.True(areSiganturesAdded);
        }

        #region Remove Custom Function

        // Validation

        #region Validation

        [Fact]
        public void RemoveCustomFunction_ForModel_ParmetersCannotBeNull()
        {
            // Model is null
            IEdmModel model = null;
            Assert.Throws<ArgumentNullException>("model", () => model.RemoveCustomUriFunction(null, null));

            // function name is empty or null
            model = EdmCoreModel.Instance;
            Assert.Throws<ArgumentNullException>("functionName", () => model.RemoveCustomUriFunction(null, null));
            Assert.Throws<ArgumentNullException>("functionName", () => model.RemoveCustomUriFunction(string.Empty, null));

            // function signature is null
            Assert.Throws<ArgumentNullException>("functionSignature", () => model.RemoveCustomUriFunction("FunctionName", null));
        }


        [Fact]
        public void RemoveCustomFunction_ForModel_FunctionSignatureWithoutAReturnType()
        {
            EdmModel model = new EdmModel();
            FunctionSignatureWithReturnType existingCustomFunctionSignature =
                   new FunctionSignatureWithReturnType(null, EdmCoreModel.Instance.GetBoolean(false));

            // Test
            Action removeFunction = () => model.RemoveCustomUriFunction("FunctionName", existingCustomFunctionSignature);

            // Assert
            Assert.Throws<ArgumentNullException>("functionSignatureWithReturnType must contain a return type", removeFunction);
        }

        #endregion

        // Remove existing
        [Fact]
        public void RemoveCustomFunction_ForModel_ShouldRemoveAnExistingFunction_ByName()
        {
            EdmModel model = new EdmModel();
            string customFunctionName = "my.ExistingCustomFunction";

            // Prepare
            FunctionSignatureWithReturnType existingCustomFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
            model.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

            Assert.True(GetCustomFunctionSignaturesOrNull(model, customFunctionName)[0].Equals(existingCustomFunctionSignature));

            // Test
            bool isRemoveSucceeded = model.RemoveCustomUriFunction(customFunctionName);

            // Assert
            Assert.True(isRemoveSucceeded);
            Assert.Null(GetCustomFunctionSignaturesOrNull(model, customFunctionName));
        }

        // Remove not existing
        [Fact]
        public void RemoveCustomFunction_ForModel_CannotRemoveFunctionWhichDoesntExist_ByName()
        {
            EdmModel model = new EdmModel();
            string customFunctionName = "my.ExistingCustomFunction";

            // Test
            bool isRemoveSucceeded = model.RemoveCustomUriFunction(customFunctionName);

            // Assert
            Assert.False(isRemoveSucceeded);
        }

        // Remove signature, function name doesn't exist
        [Fact]
        public void RemoveCustomFunction_ForModel_CannotRemoveFunctionWhichDoesntExist_ByNameAndSignature()
        {
            EdmModel model = new EdmModel();
            string customFunctionName = "my.ExistingCustomFunction";
            FunctionSignatureWithReturnType customFunctionSignature =
                 new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));

            // Test
            bool isRemoveSucceeded = model.RemoveCustomUriFunction(customFunctionName, customFunctionSignature);

            // Assert
            Assert.False(isRemoveSucceeded);
        }

        // Remove signature, function name exists, signature doesn't
        [Fact]
        public void RemoveCustomFunction_ForModel_CannotRemoveFunctionWithSameNameAndDifferentSignature()
        {
            EdmModel model = new EdmModel();
            string customFunctionName = "my.ExistingCustomFunction";

            // Prepare
            FunctionSignatureWithReturnType existingCustomFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
            model.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

            Assert.True(GetCustomFunctionSignaturesOrNull(model, customFunctionName)[0].Equals(existingCustomFunctionSignature));

            // Function with different siganture
            FunctionSignatureWithReturnType customFunctionSignatureToRemove =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetInt16(false), EdmCoreModel.Instance.GetBoolean(false));

            // Try Remove a function with the same name but different siganture
            bool isRemoveSucceeded = model.RemoveCustomUriFunction(customFunctionName, customFunctionSignatureToRemove);

            // Assert
            Assert.False(isRemoveSucceeded);
        }

        // Remove signature, function and signature exists
        [Fact]
        public void RemoveCustomFunction_ForModel_RemoveFunctionWithSameNameAndSignature()
        {
            EdmModel model = new EdmModel();
            string customFunctionName = "my.ExistingCustomFunction";

            // Prepare
            FunctionSignatureWithReturnType existingCustomFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
            model.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

            Assert.True(GetCustomFunctionSignaturesOrNull(model, customFunctionName)[0].Equals(existingCustomFunctionSignature));

            // Test
            bool isRemoveSucceeded = model.RemoveCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

            // Assert
            Assert.True(isRemoveSucceeded);

            Assert.Null(GetCustomFunctionSignaturesOrNull(model, customFunctionName));

        }

        // Remove one overload
        [Fact]
        public void RemoveCustomFunction_ForModel_RemoveFunctionWithSameNameAndSignature_OtherOverloadsExists()
        {
            EdmModel model = new EdmModel();
            string customFunctionName = "my.ExistingCustomFunction";

            // Prepare
            FunctionSignatureWithReturnType existingCustomFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
            model.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

            FunctionSignatureWithReturnType existingCustomFunctionSignatureTwo =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), EdmCoreModel.Instance.GetDate(false));
            model.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignatureTwo);

            // Validate that the two overloads as
            Assert.True(GetCustomFunctionSignaturesOrNull(model,customFunctionName).
                All(funcSignature => funcSignature.Equals(existingCustomFunctionSignature) ||
                                        funcSignature.Equals(existingCustomFunctionSignatureTwo)));

            // Remove the first overload, second overload should not be removed
            bool isRemoveSucceeded = model.RemoveCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

            // Assert
            Assert.True(isRemoveSucceeded);

            FunctionSignatureWithReturnType[] overloads = GetCustomFunctionSignaturesOrNull(model, customFunctionName);
            Assert.Single(overloads);
            Assert.Same(existingCustomFunctionSignatureTwo, overloads[0]);
        }

        #endregion

        #region ODataUriParser
        private static EdmModel BuildNewModel(out IEdmEntityType outPerson, out IEdmStructuralProperty outName, out IEdmEntitySet outPeople)
        {
            EdmModel model = new EdmModel();
            EdmEntityType person = new EdmEntityType("NS", "Person");
            person.AddKeys(person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            outName = person.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(person);

            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            outPeople = container.AddEntitySet("People", person);
            model.AddElement(container);
            outPerson = person;
            return model;
        }

        [Fact]
        public void ParseWithCustomUriFunction_ForModel()
        {
            IEdmModel model = BuildNewModel(out IEdmEntityType person, out IEdmStructuralProperty nameProp, out IEdmEntitySet entitySet);

            FunctionSignatureWithReturnType myStringFunction
                = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true), EdmCoreModel.Instance.GetString(true), EdmCoreModel.Instance.GetString(true));

            // Add a custom uri function
            model.AddCustomUriFunction("mystringfunction", myStringFunction);

            var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=mystringfunction(Name, 'BlaBla')");
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.odata.com/OData/"), fullUri);

            var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("mystringfunction").Parameters.ToList();
            startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(nameProp);
            startsWithArgs[1].ShouldBeConstantQueryNode("BlaBla");
        }

        [Fact]
        public void ParseWithMixedCaseCustomUriFunction_ForModel_EnableCaseInsensitive_ShouldWork()
        {
            IEdmModel model = BuildNewModel(out IEdmEntityType person, out IEdmStructuralProperty nameProp, out IEdmEntitySet entitySet);

            FunctionSignatureWithReturnType myStringFunction
                    = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true), EdmCoreModel.Instance.GetString(true), EdmCoreModel.Instance.GetString(true));

            // Add a custom uri function
            model.AddCustomUriFunction("myFirstMixedCasestringfunction", myStringFunction);

            // Uri with mixed-case, should work for resolver with case insensitive enabled.
            var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=mYFirstMixedCasesTrInGfUnCtIoN(Name, 'BlaBla')");
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.odata.com/OData/"), fullUri);
            parser.Resolver.EnableCaseInsensitive = true;

            var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("myFirstMixedCasestringfunction")
                .Parameters.ToList();
            startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(nameProp);
            startsWithArgs[1].ShouldBeConstantQueryNode("BlaBla");
        }

        [Fact]
        public void ParseWithExactMatchCustomUriFunction_ForModel_EnableCaseInsensitive_ShouldWorkForMultipleEquivalentArgumentsMatches()
        {
            IEdmModel model = BuildNewModel(out IEdmEntityType person, out IEdmStructuralProperty nameProp, out IEdmEntitySet entitySet);
            string lowerCaseName = "myfunction";
            string upperCaseName = lowerCaseName.ToUpper();

            FunctionSignatureWithReturnType myStringFunction
                = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true), EdmCoreModel.Instance.GetString(true), EdmCoreModel.Instance.GetString(true));

            // Add two customer uri functions with same argument types, with names different in cases.
            model.AddCustomUriFunction(lowerCaseName, myStringFunction);
            model.AddCustomUriFunction(upperCaseName, myStringFunction);
            string rootUri = "http://www.odata.com/OData/";
            string uriTemplate = rootUri + "People?$filter={0}(Name,'BlaBla')";

            foreach (string functionName in new string[] { lowerCaseName, upperCaseName })
            {
                // Uri with case-sensitive function names referring to equivalent-argument-typed functions,
                // should work for resolver with case insensitive enabled.
                var fullUri = new Uri(string.Format(uriTemplate, functionName));
                ODataUriParser parser = new ODataUriParser(model, new Uri(rootUri), fullUri);
                parser.Resolver.EnableCaseInsensitive = true;

                var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode(functionName).Parameters.ToList();
                startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(nameProp);
                startsWithArgs[1].ShouldBeConstantQueryNode("BlaBla");
            }
        }

        [Fact]
        public void ParseWithCustomUriFunction_ForModel_EnableCaseInsensitive_ShouldThrowDueToAmbiguity()
        {
            IEdmModel model = BuildNewModel(out IEdmEntityType person, out IEdmStructuralProperty nameProp, out IEdmEntitySet entitySet);
            string lowerCaseName = "myfunction";
            string upperCaseName = lowerCaseName.ToUpper();

            FunctionSignatureWithReturnType myStringFunction
                = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true), EdmCoreModel.Instance.GetString(true), EdmCoreModel.Instance.GetString(true));

            // Add two customer uri functions with same argument types, with names different in cases.
            model.AddCustomUriFunction(lowerCaseName, myStringFunction);
            model.AddCustomUriFunction(upperCaseName, myStringFunction);
            string rootUri = "http://www.odata.com/OData/";
            string uriTemplate = rootUri + "People?$filter={0}(Name,'BlaBla')";

            int strLen = lowerCaseName.Length;
            string mixedCaseFunctionName = lowerCaseName.Substring(0, strLen / 2).ToUpper() + lowerCaseName.Substring(strLen / 2);
            // Uri with mix-case function names referring to equivalent-argument-typed functions,
            // should result in exception for resolver with case insensitive enabled due to ambiguity (multiple equivalent matches).
            var fullUri = new Uri(string.Format(uriTemplate, mixedCaseFunctionName));
            ODataUriParser parser = new ODataUriParser(model, new Uri(rootUri), fullUri);
            parser.Resolver.EnableCaseInsensitive = true;

            Action action = () => parser.ParseFilter();
            Assert.Throws<ODataException>(action);
        }

        [Fact]
        public void ParseWithMixedCaseCustomUriFunction_ForModel_DisableCaseInsensitive_ShouldFailed()
        {
            IEdmModel model = BuildNewModel(out IEdmEntityType person, out IEdmStructuralProperty nameProp, out IEdmEntitySet entitySet);
            bool exceptionThrown = false;
            try
            {
                FunctionSignatureWithReturnType myStringFunction
                    = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true),
                        EdmCoreModel.Instance.GetString(true), EdmCoreModel.Instance.GetString(true));

                // Add a custom uri function
                model.AddCustomUriFunction("myMixedCasestringfunction", myStringFunction);

                // Uri with mixed-case, should fail for default resolver with case-insensitive disabled.
                var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=mYMixedCasesTrInGfUnCtIoN(Name, 'BlaBla')");
                ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.odata.com/OData/"), fullUri);
                parser.Resolver.EnableCaseInsensitive = false;

                parser.ParseFilter();
            }
            catch (ODataException e)
            {
                Assert.Equal("An unknown function with name 'mYMixedCasesTrInGfUnCtIoN' was found. " +
                    "This may also be a function import or a key lookup on a navigation property, which is not allowed.", e.Message);
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown, "Exception should be thrown trying to parse mixed-case uri function when case-insensitive is disabled.");
        }

        [Fact]
        public void ParseWithCustomUriFunction_ForModel_AddAsOverloadToBuiltIn()
        {
            IEdmModel model = BuildNewModel(out IEdmEntityType person, out IEdmStructuralProperty nameProp, out IEdmEntitySet entitySet);
            FunctionSignatureWithReturnType customStartWithFunctionSignature =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true),
                                                    EdmCoreModel.Instance.GetString(true),
                                                    EdmCoreModel.Instance.GetInt32(true));

            // Add with override 'true'
            model.AddCustomUriFunction("startswith", customStartWithFunctionSignature);

            var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=startswith(Name, 66)");
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.odata.com/OData/"), fullUri);

            var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("startswith").Parameters.ToList();
            startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(nameProp);
            startsWithArgs[1].ShouldBeConstantQueryNode(66);
        }

        [Fact]
        public void ParseWithCustomFunction_ForModel_EnumParameter()
        {
            EdmModel model = BuildNewModel(out IEdmEntityType person, out IEdmStructuralProperty nameProp, out IEdmEntitySet entitySet);

            var enumType = new EdmEnumType("NS", "NonFlagShape", EdmPrimitiveTypeKind.SByte, false);
            enumType.AddMember("Rectangle", new EdmEnumMemberValue(1));
            enumType.AddMember("Triangle", new EdmEnumMemberValue(2));
            enumType.AddMember("foursquare", new EdmEnumMemberValue(3));
            var enumTypeRef = new EdmEnumTypeReference(enumType, false);

            FunctionSignatureWithReturnType signature =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), enumTypeRef);

            model.AddCustomUriFunction("enumFunc", signature);

            var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=enumFunc('Rectangle')");
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.odata.com/OData/"), fullUri);

            var enumFuncWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("enumFunc").Parameters.ToList();
            enumFuncWithArgs[0].ShouldBeEnumNode(enumType, "Rectangle");
        }

        #endregion

        #region Private Methods

        private FunctionSignatureWithReturnType[] GetCustomFunctionSignaturesOrNull(string customFunctionName)
        {
            IList<KeyValuePair<string, FunctionSignatureWithReturnType>> resultFunctionSignaturesWithReturnType = null;
            CustomUriFunctions.TryGetCustomFunction(customFunctionName, out resultFunctionSignaturesWithReturnType);

            return resultFunctionSignaturesWithReturnType?.Select( _ => _.Value).ToArray();
        }
        #endregion
    }
}
