//---------------------------------------------------------------------
// <copyright file="CustomUriFunctionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Tests the CustomUriFunctions class.
    /// </summary>
    public class CustomUriFunctionsTests
    {
        #region Constants

        // Existing built-in uri functions
        private const string BUILT_IN_GEODISTANCE_FUNCTION_NAME = "geo.distance";

        private readonly FunctionSignatureWithReturnType GEO_DISTANCE_BUILTIN_FUNCTION_SIGNATURE = new FunctionSignatureWithReturnType(
                            EdmCoreModel.Instance.GetDouble(true),
                            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true),
                            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true));

        #endregion

        #region Add Custom Function

        #region Validation

        [Fact]
        public void AddCustomFunction_FunctionCannotBeNull()
        {
            Action addNullFunctionAction = () =>
                HardCodedTestModel.TestModel.AddCustomUriFunction("my.MyNullCustomFunction", null);

            Assert.Throws<ArgumentNullException>("functionSignature", addNullFunctionAction);
        }

        [Fact]
        public void AddCustomFunction_FunctionNameCannotBeNull()
        {
            FunctionSignatureWithReturnType customFunctionSignature =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), EdmCoreModel.Instance.GetInt32(false));

            Action addNullFunctionNameAction = () =>
                HardCodedTestModel.TestModel.AddCustomUriFunction(null, customFunctionSignature);

            Assert.Throws<ArgumentNullException>("functionName", addNullFunctionNameAction);
        }

        [Fact]
        public void AddCustomFunction_FunctionNameCannotBeEmptyString()
        {
            FunctionSignatureWithReturnType customFunctionSignature =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), EdmCoreModel.Instance.GetInt32(false));

            Action addCustomFunctionSignature = () =>
                HardCodedTestModel.TestModel.AddCustomUriFunction(string.Empty, customFunctionSignature);

            Assert.Throws<ArgumentNullException>("functionName", addCustomFunctionSignature);
        }

        [Fact]
        public void AddCustomFunction_CannotAddFunctionSignatureWithNullReturnType()
        {
            FunctionSignatureWithReturnType customFunctionSignatureWithNullReturnType =
                new FunctionSignatureWithReturnType(null, EdmCoreModel.Instance.GetInt32(false));

            Action addCustomFunctionSignature = () =>
                HardCodedTestModel.TestModel.AddCustomUriFunction("my.customFunctionWithNoReturnType",
                                                        customFunctionSignatureWithNullReturnType);

            Assert.Throws<ArgumentNullException>("functionSignatureWithReturnType must contain a return type", addCustomFunctionSignature);
        }

        #endregion

        [Fact]
        public void AddCustomFunction_CannotAddFunctionWhichAlreadyExistsAsBuiltInWithSameFullSignature_AddAsOverload()
        {
            try
            {
                // Add exisiting with 'addAsOverload' 'true'
                Action addCustomFunction = () =>
                    HardCodedTestModel.TestModel.AddCustomUriFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME,
                                                            GEO_DISTANCE_BUILTIN_FUNCTION_SIGNATURE);

                // Assert
                addCustomFunction.Throws<ODataException>(Error.Format(SRResources.CustomUriFunctions_AddCustomUriFunction_BuiltInExistsFullSignature, BUILT_IN_GEODISTANCE_FUNCTION_NAME));
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                HardCodedTestModel.TestModel.RemoveCustomUriFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME);
            }
        }

        [Fact]
        public void AddCustomFunction_ShouldAddFunctionWhichAlreadyExistsAsBuiltInWithSameName_AddAsOverload()
        {
            try
            {
                FunctionSignatureWithReturnType customFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false),
                                                        EdmCoreModel.Instance.GetBoolean(false));

                // Add with 'addAsOverload' 'true'
                HardCodedTestModel.TestModel.AddCustomUriFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME, customFunctionSignature);

                FunctionSignatureWithReturnType[] resultFunctionSignaturesWithReturnType =
                    this.GetCustomFunctionSignaturesOrNull(BUILT_IN_GEODISTANCE_FUNCTION_NAME);

                // Assert
                Assert.NotNull(resultFunctionSignaturesWithReturnType);
                Assert.Single(resultFunctionSignaturesWithReturnType);
                Assert.Same(customFunctionSignature, resultFunctionSignaturesWithReturnType[0]);
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                HardCodedTestModel.TestModel.RemoveCustomUriFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME);
            }
        }

        // Existing Custom Function
        [Fact]
        public void AddCustomFunction_CannotAddFunctionWithFullSignatureExistsAsCustomFunction()
        {
            string customFunctionName = "my.ExistingCustomFunction";
            try
            {
                // Prepare
                var existingCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
                HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                // Test
                var newCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));

                Action addCustomFunction = () =>
                    HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

                // Assert
                addCustomFunction.Throws<ODataException>(Error.Format(SRResources.CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists, customFunctionName));
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName);
            }
        }

        [Fact]
        public void AddCustomFunction_CannotAddFunctionWithFullSignatureExistsAsCustomFunction_AddAsOverload()
        {
            string customFunctionName = "my.ExistingCustomFunction";
            try
            {
                // Prepare
                var existingCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
                HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                // Test
                var newCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));

                Action addCustomFunction = () =>
                    HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

                // Asserts
                addCustomFunction.Throws<ODataException>(Error.Format(SRResources.CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists, customFunctionName));
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName);
            }
        }

        [Fact]
        public void AddCustomFunction_CustomFunctionDoesntExist_ShouldAdd()
        {
            string customFunctionName = "my.NewCustomFunction";
            try
            {
                // New not existing custom function
                var newCustomFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetInt32(false), EdmCoreModel.Instance.GetBoolean(false));
                HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

                // Assert
                // Make sure both signatures exists
                FunctionSignatureWithReturnType[] customFunctionSignatures =
                    GetCustomFunctionSignaturesOrNull(customFunctionName);

                Assert.Single(customFunctionSignatures);
                Assert.Same(newCustomFunctionSignature, customFunctionSignatures[0]);
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName);
            }
        }

        [Fact]
        public void AddCustomFunction_CustomFunctionDoesntExist_ShouldAdd_NoArgumnetsToFunctionSignature()
        {
            string customFunctionName = "my.NewCustomFunction";
            try
            {
                // New not existing custom function - function without any argumnets
                var newCustomFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false));
                HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

                // Assert
                // Make sure both signatures exists
                FunctionSignatureWithReturnType[] customFunctionSignatures =
                    GetCustomFunctionSignaturesOrNull(customFunctionName);

                Assert.Single(customFunctionSignatures);
                Assert.Same(newCustomFunctionSignature, customFunctionSignatures[0]);
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName);
            }
        }

        [Fact]
        public void AddCustomFunction_CustomFunctionNameExistsButNotFullSignature_ShouldAddAsAnOverload()
        {
            string customFunctionName = "my.ExistingCustomFunction";
            try
            {
                // Prepare
                FunctionSignatureWithReturnType existingCustomFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
                HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                //Test
                // Same name, but different signature
                var newCustomFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetInt32(false), EdmCoreModel.Instance.GetBoolean(false));
                HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

                // Assert
                // Make sure both signatures exists
                bool areSiganturesAdded =
                    GetCustomFunctionSignaturesOrNull(customFunctionName).
                        All(x => x.Equals(existingCustomFunctionSignature) || x.Equals(newCustomFunctionSignature));

                Assert.True(areSiganturesAdded);
            }
            finally
            {
                // Clean both signatures from CustomUriFunctions cache
                Assert.True(HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName));
            }
        }

        #endregion

        #region Remove Custom Function

        // Validation

        #region Validation

        [Fact]
        public void RemoveCustomFunction_NullFunctionName()
        {
            // Test
            Action removeFunction = () =>
                HardCodedTestModel.TestModel.RemoveCustomUriFunction(null);

            // Assert
            Assert.Throws<ArgumentNullException>("functionName", removeFunction);
        }

        [Fact]
        public void RemoveCustomFunction_EmptyStringFunctionName()
        {
            // Test
            Action removeFunction = () =>
                HardCodedTestModel.TestModel.RemoveCustomUriFunction(string.Empty);

            // Assert
            Assert.Throws<ArgumentNullException>("functionName", removeFunction);
        }

        [Fact]
        public void RemoveCustomFunction_NullFunctionSignature()
        {
            // Test
            Action removeFunction = () =>
                HardCodedTestModel.TestModel.RemoveCustomUriFunction("FunctionName", null);

            // Assert
            Assert.Throws<ArgumentNullException>("functionSignature", removeFunction);
        }

        [Fact]
        public void RemoveCustomFunction_FunctionSignatureWithoutAReturnType()
        {
            FunctionSignatureWithReturnType existingCustomFunctionSignature =
                   new FunctionSignatureWithReturnType(null, EdmCoreModel.Instance.GetBoolean(false));

            // Test
            Action removeFunction = () =>
                HardCodedTestModel.TestModel.RemoveCustomUriFunction("FunctionName", existingCustomFunctionSignature);

            // Assert
            Assert.Throws<ArgumentNullException>("functionSignatureWithReturnType must contain a return type", removeFunction);
        }

        #endregion

        // Remove existing
        [Fact]
        public void RemoveCustomFunction_ShouldRemoveAnExistingFunction_ByName()
        {
            string customFunctionName = "my.ExistingCustomFunction";

            // Prepare
            FunctionSignatureWithReturnType existingCustomFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
            HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

            Assert.True(GetCustomFunctionSignaturesOrNull(customFunctionName)[0].Equals(existingCustomFunctionSignature));

            // Test
            bool isRemoveSucceeded = HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName);

            // Assert
            Assert.True(isRemoveSucceeded);
            Assert.Null(GetCustomFunctionSignaturesOrNull(customFunctionName));
        }


        // Remove not existing
        [Fact]
        public void RemoveCustomFunction_CannotRemoveFunctionWhichDoesntExist_ByName()
        {
            string customFunctionName = "my.ExistingCustomFunction";

            // Test
            bool isRemoveSucceeded = HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName);

            // Assert
            Assert.False(isRemoveSucceeded);
        }

        // Remove signature, function name doesn't exist
        [Fact]
        public void RemoveCustomFunction_CannotRemoveFunctionWhichDoesntExist_ByNameAndSignature()
        {
            string customFunctionName = "my.ExistingCustomFunction";
            FunctionSignatureWithReturnType customFunctionSignature =
                 new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));

            // Test
            bool isRemoveSucceeded = HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName, customFunctionSignature);

            // Assert
            Assert.False(isRemoveSucceeded);
        }

        // Remove signature, function name exists, signature doesn't
        [Fact]
        public void RemoveCustomFunction_CannotRemoveFunctionWithSameNameAndDifferentSignature()
        {
            string customFunctionName = "my.ExistingCustomFunction";

            try
            {
                // Prepare
                FunctionSignatureWithReturnType existingCustomFunctionSignature =
                        new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
                HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                Assert.True(GetCustomFunctionSignaturesOrNull(customFunctionName)[0].Equals(existingCustomFunctionSignature));

                // Function with different siganture
                FunctionSignatureWithReturnType customFunctionSignatureToRemove =
                        new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetInt16(false), EdmCoreModel.Instance.GetBoolean(false));

                // Test

                // Try Remove a function with the same name but different siganture
                bool isRemoveSucceeded = HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName, customFunctionSignatureToRemove);

                // Assert
                Assert.False(isRemoveSucceeded);
            }
            finally
            {
                // Clean up cahce
                HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName);
            }

        }

        // Remove signature, function and signature exists
        [Fact]
        public void RemoveCustomFunction_RemoveFunctionWithSameNameAndSignature()
        {
            string customFunctionName = "my.ExistingCustomFunction";

            try
            {
                // Prepare
                FunctionSignatureWithReturnType existingCustomFunctionSignature =
                        new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
                HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                Assert.True(GetCustomFunctionSignaturesOrNull(customFunctionName)[0].Equals(existingCustomFunctionSignature));

                // Test
                bool isRemoveSucceeded = HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                // Assert
                Assert.True(isRemoveSucceeded);

                Assert.Null(GetCustomFunctionSignaturesOrNull(customFunctionName));
            }
            finally
            {
                HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName);
            }
        }

        // Remove one overload
        [Fact]
        public void RemoveCustomFunction_RemoveFunctionWithSameNameAndSignature_OtherOverloadsExists()
        {
            string customFunctionName = "my.ExistingCustomFunction";

            try
            {
                // Prepare
                FunctionSignatureWithReturnType existingCustomFunctionSignature =
                        new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));
                HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                FunctionSignatureWithReturnType existingCustomFunctionSignatureTwo =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), EdmCoreModel.Instance.GetDate(false));
                HardCodedTestModel.TestModel.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignatureTwo);

                // Validate that the two overloads as
                Assert.True(GetCustomFunctionSignaturesOrNull(customFunctionName).
                    All(funcSignature => funcSignature.Equals(existingCustomFunctionSignature) ||
                                            funcSignature.Equals(existingCustomFunctionSignatureTwo)));

                // Remove the first overload, second overload should not be removed
                bool isRemoveSucceeded = HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                // Assert
                Assert.True(isRemoveSucceeded);

                FunctionSignatureWithReturnType[] overloads = GetCustomFunctionSignaturesOrNull(customFunctionName);
                Assert.Single(overloads);
                Assert.Same(existingCustomFunctionSignatureTwo, overloads[0]);
            }
            finally
            {
                // Clean up cache
                HardCodedTestModel.TestModel.RemoveCustomUriFunction(customFunctionName);
            }
        }

        #endregion

        #region ODataUriParser

        [Fact]
        public void ParseWithCustomUriFunction()
        {
            try
            {
                FunctionSignatureWithReturnType myStringFunction
                    = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true), EdmCoreModel.Instance.GetString(true), EdmCoreModel.Instance.GetString(true));

                // Add a custom uri function
                HardCodedTestModel.TestModel.AddCustomUriFunction("mystringfunction", myStringFunction);

                var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=mystringfunction(Name, 'BlaBla')");
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("mystringfunction").Parameters.ToList();
                startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
                startsWithArgs[1].ShouldBeConstantQueryNode("BlaBla");
            }
            finally
            {
                Assert.True(HardCodedTestModel.TestModel.RemoveCustomUriFunction("mystringfunction"));
            }
        }

        [Fact]
        public void ParseWithMixedCaseCustomUriFunction_EnableCaseInsensitive_ShouldWork()
        {
            try
            {
                FunctionSignatureWithReturnType myStringFunction
                    = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true), EdmCoreModel.Instance.GetString(true), EdmCoreModel.Instance.GetString(true));

                // Add a custom uri function
                HardCodedTestModel.TestModel.AddCustomUriFunction("myFirstMixedCasestringfunction", myStringFunction);

                // Uri with mixed-case, should work for resolver with case insensitive enabled.
                var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=mYFirstMixedCasesTrInGfUnCtIoN(Name, 'BlaBla')");
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);
                parser.Resolver.EnableCaseInsensitive = true;

                var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("myFirstMixedCasestringfunction")
                    .Parameters.ToList();
                startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
                startsWithArgs[1].ShouldBeConstantQueryNode("BlaBla");
            }
            finally
            {
                Assert.True(HardCodedTestModel.TestModel.RemoveCustomUriFunction("myFirstMixedCasestringfunction"));
            }
        }

        [Fact]
        public void ParseWithExactMatchCustomUriFunction_EnableCaseInsensitive_ShouldWorkForMultipleEquivalentArgumentsMatches()
        {
            string lowerCaseName = "myfunction";
            string upperCaseName = lowerCaseName.ToUpper();

            FunctionSignatureWithReturnType myStringFunction
                = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true), EdmCoreModel.Instance.GetString(true), EdmCoreModel.Instance.GetString(true));

            // Add two customer uri functions with same argument types, with names different in cases.
            HardCodedTestModel.TestModel.AddCustomUriFunction(lowerCaseName, myStringFunction);
            HardCodedTestModel.TestModel.AddCustomUriFunction(upperCaseName, myStringFunction);
            string rootUri = "http://www.odata.com/OData/";
            string uriTemplate = rootUri + "People?$filter={0}(Name,'BlaBla')";

            try
            {
                foreach (string functionName in new string[] { lowerCaseName, upperCaseName })
                {
                    // Uri with case-sensitive function names referring to equivalent-argument-typed functions,
                    // should work for resolver with case insensitive enabled.
                    var fullUri = new Uri(string.Format(uriTemplate, functionName));
                    ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(rootUri), fullUri);
                    parser.Resolver.EnableCaseInsensitive = true;

                    var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode(functionName)
                        .Parameters.ToList();
                    startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
                    startsWithArgs[1].ShouldBeConstantQueryNode("BlaBla");
                }
            }
            finally
            {
                Assert.True(HardCodedTestModel.TestModel.RemoveCustomUriFunction(lowerCaseName));
                Assert.True(HardCodedTestModel.TestModel.RemoveCustomUriFunction(upperCaseName));
            }
        }

        [Fact]
        public void ParseWithCustomUriFunction_EnableCaseInsensitive_ShouldThrowDueToAmbiguity()
        {
            string lowerCaseName = "myfunction";
            string upperCaseName = lowerCaseName.ToUpper();

            FunctionSignatureWithReturnType myStringFunction
                = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true), EdmCoreModel.Instance.GetString(true), EdmCoreModel.Instance.GetString(true));

            // Add two customer uri functions with same argument types, with names different in cases.
            HardCodedTestModel.TestModel.AddCustomUriFunction(lowerCaseName, myStringFunction);
            HardCodedTestModel.TestModel.AddCustomUriFunction(upperCaseName, myStringFunction);
            string rootUri = "http://www.odata.com/OData/";
            string uriTemplate = rootUri + "People?$filter={0}(Name,'BlaBla')";

            try
            {
                int strLen = lowerCaseName.Length;
                string mixedCaseFunctionName = lowerCaseName.Substring(0, strLen/2).ToUpper() + lowerCaseName.Substring(strLen/2);
                // Uri with mix-case function names referring to equivalent-argument-typed functions,
                // should result in exception for resolver with case insensitive enabled due to ambiguity (multiple equivalent matches).
                var fullUri = new Uri(string.Format(uriTemplate, mixedCaseFunctionName));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(rootUri), fullUri);
                parser.Resolver.EnableCaseInsensitive = true;

                Action action = () => parser.ParseFilter();
                Assert.Throws<ODataException>(action);
            }
            finally
            {
                Assert.True(HardCodedTestModel.TestModel.RemoveCustomUriFunction(lowerCaseName));
                Assert.True(HardCodedTestModel.TestModel.RemoveCustomUriFunction(upperCaseName));
            }
        }

        [Fact]
        public void ParseWithMixedCaseCustomUriFunction_DisableCaseInsensitive_ShouldFailed()
        {
            try
            {
                FunctionSignatureWithReturnType myStringFunction
                    = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true),
                        EdmCoreModel.Instance.GetString(true), EdmCoreModel.Instance.GetString(true));

                // Add a custom uri function
                HardCodedTestModel.TestModel.AddCustomUriFunction("myMixedCasestringfunction", myStringFunction);

                // Uri with mixed-case, should fail for default resolver with case-insensitive disabled.
                var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=mYMixedCasesTrInGfUnCtIoN(Name, 'BlaBla')");
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel,
                    new Uri("http://www.odata.com/OData/"), fullUri);
                parser.Resolver.EnableCaseInsensitive = false;

                var exception = Assert.Throws<ODataException>(() => parser.ParseFilter());
                Assert.Equal("An unknown function with name 'mYMixedCasesTrInGfUnCtIoN' was found. " +
                    "This may also be a function import or a key lookup on a navigation property, which is not allowed.", exception.Message);
            }
            finally
            {
                Assert.True(HardCodedTestModel.TestModel.RemoveCustomUriFunction("myMixedCasestringfunction"));
            }
        }

        [Fact]
        public void ParseWithCustomUriFunction_AddAsOverloadToBuiltIn()
        {
            FunctionSignatureWithReturnType customStartWithFunctionSignature =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true),
                                                    EdmCoreModel.Instance.GetString(true),
                                                    EdmCoreModel.Instance.GetInt32(true));
            try
            {
                // Add with override 'true'
                HardCodedTestModel.TestModel.AddCustomUriFunction("startswith", customStartWithFunctionSignature);

                var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=startswith(Name, 66)");
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("startswith").Parameters.ToList();
                startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
                startsWithArgs[1].ShouldBeConstantQueryNode(66);
            }
            finally
            {
                Assert.True(HardCodedTestModel.TestModel.RemoveCustomUriFunction("startswith"));
            }
        }

        [Fact]
        public void ParseWithCustomFunction_EnumParameter()
        {
            try
            {
                var enumType = new EdmEnumType("Fully.Qualified.Namespace", "NonFlagShape", EdmPrimitiveTypeKind.SByte, false);
                enumType.AddMember("Rectangle", new EdmEnumMemberValue(1));
                enumType.AddMember("Triangle", new EdmEnumMemberValue(2));
                enumType.AddMember("foursquare", new EdmEnumMemberValue(3));
                var enumTypeRef = new EdmEnumTypeReference(enumType, false);

                FunctionSignatureWithReturnType signature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), enumTypeRef);

                HardCodedTestModel.TestModel.AddCustomUriFunction("enumFunc", signature);

                var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=enumFunc('Rectangle')");
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                var enumFuncWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("enumFunc").Parameters.ToList();
                enumFuncWithArgs[0].ShouldBeEnumNode(enumType, "Rectangle");
            }
            finally
            {
                Assert.True(HardCodedTestModel.TestModel.RemoveCustomUriFunction("enumFunc"));
            }
        }

        [Fact]
        public void ParseWithCustomUriFunction_OverloadsWithDifferentSignatures()
        {
            var functionName = "equals";
            var functionSignature1 =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), EdmCoreModel.Instance.GetString(false), EdmCoreModel.Instance.GetString(false));
            var functionSignature2 =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), EdmCoreModel.Instance.GetString(false), EdmCoreModel.Instance.GetString(false), EdmCoreModel.Instance.GetBoolean(false));

            try
            {
                HardCodedTestModel.TestModel.AddCustomUriFunction(functionName, functionSignature1);
                HardCodedTestModel.TestModel.AddCustomUriFunction(functionName, functionSignature2);

                var odataUri1 = new Uri("http://tempuri.org/People" + "?$filter=equals(Name,'Sue')");
                var uriParser1 = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://tempuri.org/"), odataUri1);

                var equals1Args = uriParser1.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("equals").Parameters.ToList();
                Assert.Equal(2, equals1Args.Count);

                var convertNamePropNode1 = equals1Args[0].ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetString(false));
                var convertLiteralNode1 = equals1Args[1].ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetString(false));
                convertNamePropNode1.Source.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
                convertLiteralNode1.Source.ShouldBeConstantQueryNode("Sue");

                var odataUri2 = new Uri("http://tempuri.org/People" + "?$filter=equals(Name,'bob',false)");
                var uriParser2 = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://tempuri.org/"), odataUri2);

                var equals2Args = uriParser2.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("equals").Parameters.ToList();
                Assert.Equal(3, equals2Args.Count);

                var convertNamePropNode2 = equals2Args[0].ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetString(false));
                var convertLiteralNode2 = equals2Args[1].ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetString(false));
                convertNamePropNode2.Source.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
                convertLiteralNode2.Source.ShouldBeConstantQueryNode("bob");
                equals2Args[2].ShouldBeConstantQueryNode(false);
            }
            finally
            {
                Assert.True(HardCodedTestModel.TestModel.RemoveCustomUriFunction(functionName));
            }
        }

        [Fact]
        public void ParseWithCustomUriFunction_TryGetNonExistentCustomUriFunction()
        {
            Assert.False(HardCodedTestModel.TestModel.TryGetCustomUriFunction(
                Guid.NewGuid().ToString(),
                out IReadOnlyList<KeyValuePair<string, FunctionSignatureWithReturnType>> _));
        }

        #endregion

        #region Private Methods

        private FunctionSignatureWithReturnType[] GetCustomFunctionSignaturesOrNull(string customFunctionName)
        {
            IReadOnlyList<KeyValuePair<string, FunctionSignatureWithReturnType>> resultFunctionSignaturesWithReturnType = null;
            HardCodedTestModel.TestModel.TryGetCustomUriFunction(customFunctionName, out resultFunctionSignaturesWithReturnType);

            return resultFunctionSignaturesWithReturnType?.Select( _ => _.Value).ToArray();
        }
        #endregion
    }
}
