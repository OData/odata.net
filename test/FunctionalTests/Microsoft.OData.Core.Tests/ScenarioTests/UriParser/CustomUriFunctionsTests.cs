﻿using Microsoft.OData;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Tests.UriParser;

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
                CustomUriFunctions.AddCustomUriFunction("my.MyNullCustomFunction", null);

            addNullFunctionAction.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddCustomFunction_FunctionNameCannotBeNull()
        {
            FunctionSignatureWithReturnType customFunctionSignature =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), EdmCoreModel.Instance.GetInt32(false));

            Action addNullFunctionNameAction = () =>
                CustomUriFunctions.AddCustomUriFunction(null, customFunctionSignature);

            addNullFunctionNameAction.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddCustomFunction_FunctionNameCannotBeEmptyString()
        {
            FunctionSignatureWithReturnType customFunctionSignature =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), EdmCoreModel.Instance.GetInt32(false));

            Action addCustomFunctionSignature = () =>
                CustomUriFunctions.AddCustomUriFunction(string.Empty, customFunctionSignature);

            addCustomFunctionSignature.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddCustomFunction_CannotAddFunctionSignatureWithNullReturnType()
        {
            FunctionSignatureWithReturnType customFunctionSignatureWithNullReturnType =
                new FunctionSignatureWithReturnType(null, EdmCoreModel.Instance.GetInt32(false));

            Action addCustomFunctionSignature = () =>
                CustomUriFunctions.AddCustomUriFunction("my.customFunctionWithNoReturnType",
                                                        customFunctionSignatureWithNullReturnType);

            addCustomFunctionSignature.ShouldThrow<ArgumentNullException>();
        }

        #endregion

        [Fact]
        public void AddCustomFunction_CannotAddFunctionWhichAlreadyExistsAsBuiltInWithSameFullSignature_AddAsOverload()
        {
            try
            {
                // Add exisiting with 'addAsOverload' 'true'
                Action addCustomFunction = () =>
                    CustomUriFunctions.AddCustomUriFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME,
                                                            GEO_DISTANCE_BUILTIN_FUNCTION_SIGNATURE);

                // Assert
                addCustomFunction.ShouldThrow<ODataException>().
                    WithMessage(Strings.CustomUriFunctions_AddCustomUriFunction_BuiltInExistsFullSignature(BUILT_IN_GEODISTANCE_FUNCTION_NAME));
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                CustomUriFunctions.RemoveCustomUriFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME);
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
                CustomUriFunctions.AddCustomUriFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME, customFunctionSignature);

                FunctionSignatureWithReturnType[] resultFunctionSignaturesWithReturnType =
                    this.GetCustomFunctionSignaturesOrNull(BUILT_IN_GEODISTANCE_FUNCTION_NAME);

                // Assert
                resultFunctionSignaturesWithReturnType.Should().NotBeNull();
                resultFunctionSignaturesWithReturnType.Length.Should().Be(1);
                resultFunctionSignaturesWithReturnType[0].Should().Be(customFunctionSignature);
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                CustomUriFunctions.RemoveCustomUriFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME);
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
                CustomUriFunctions.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                // Test
                var newCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));

                Action addCustomFunction = () =>
                    CustomUriFunctions.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

                // Assert
                addCustomFunction.ShouldThrow<ODataException>().
                    WithMessage(Strings.CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists(customFunctionName));
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                CustomUriFunctions.RemoveCustomUriFunction(customFunctionName);
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
                CustomUriFunctions.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                // Test
                var newCustomFunctionSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));

                Action addCustomFunction = () =>
                    CustomUriFunctions.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

                // Asserts
                addCustomFunction.ShouldThrow<ODataException>().
                    WithMessage(Strings.CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists(customFunctionName));
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                CustomUriFunctions.RemoveCustomUriFunction(customFunctionName);
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
                CustomUriFunctions.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

                // Assert
                // Make sure both signatures exists
                FunctionSignatureWithReturnType[] customFunctionSignatures =
                    GetCustomFunctionSignaturesOrNull(customFunctionName);

                customFunctionSignatures.Length.Should().Be(1);
                customFunctionSignatures[0].Should().BeSameAs(newCustomFunctionSignature);
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                CustomUriFunctions.RemoveCustomUriFunction(customFunctionName);
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
                CustomUriFunctions.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

                // Assert
                // Make sure both signatures exists
                FunctionSignatureWithReturnType[] customFunctionSignatures =
                    GetCustomFunctionSignaturesOrNull(customFunctionName);

                customFunctionSignatures.Length.Should().Be(1);
                customFunctionSignatures[0].Should().BeSameAs(newCustomFunctionSignature);
            }
            finally
            {
                // Clean from CustomUriFunctions cache
                CustomUriFunctions.RemoveCustomUriFunction(customFunctionName);
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
                CustomUriFunctions.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                //Test
                // Same name, but different signature
                var newCustomFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetInt32(false), EdmCoreModel.Instance.GetBoolean(false));
                CustomUriFunctions.AddCustomUriFunction(customFunctionName, newCustomFunctionSignature);

                // Assert
                // Make sure both signatures exists
                bool areSiganturesAdded =
                    GetCustomFunctionSignaturesOrNull(customFunctionName).
                        All(x => x.Equals(existingCustomFunctionSignature) || x.Equals(newCustomFunctionSignature));

                areSiganturesAdded.Should().BeTrue();
            }
            finally
            {
                // Clean both signatures from CustomUriFunctions cache
                CustomUriFunctions.RemoveCustomUriFunction(customFunctionName);
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
                CustomUriFunctions.RemoveCustomUriFunction(null);

            // Assert
            removeFunction.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void RemoveCustomFunction_EmptyStringFunctionName()
        {
            // Test
            Action removeFunction = () =>
                CustomUriFunctions.RemoveCustomUriFunction(string.Empty);

            // Assert
            removeFunction.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void RemoveCustomFunction_NullFunctionSignature()
        {
            // Test
            Action removeFunction = () =>
                CustomUriFunctions.RemoveCustomUriFunction("FunctionName", null);

            // Assert
            removeFunction.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void RemoveCustomFunction_FunctionSignatureWithoutAReturnType()
        {
            FunctionSignatureWithReturnType existingCustomFunctionSignature =
                   new FunctionSignatureWithReturnType(null, EdmCoreModel.Instance.GetBoolean(false));

            // Test
            Action removeFunction = () =>
                CustomUriFunctions.RemoveCustomUriFunction("FunctionName", existingCustomFunctionSignature);

            // Assert
            removeFunction.ShouldThrow<ArgumentNullException>();
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
            CustomUriFunctions.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

            GetCustomFunctionSignaturesOrNull(customFunctionName)[0].
                Equals(existingCustomFunctionSignature).
                Should().BeTrue();

            // Test
            bool isRemoveSucceeded = CustomUriFunctions.RemoveCustomUriFunction(customFunctionName);

            // Assert
            isRemoveSucceeded.Should().BeTrue();
            GetCustomFunctionSignaturesOrNull(customFunctionName).Should().BeNull();
        }


        // Remove not existing
        [Fact]
        public void RemoveCustomFunction_CannotRemoveFunctionWhichDoesntExist_ByName()
        {
            string customFunctionName = "my.ExistingCustomFunction";

            // Test
            bool isRemoveSucceeded = CustomUriFunctions.RemoveCustomUriFunction(customFunctionName);

            // Assert
            isRemoveSucceeded.Should().BeFalse();
        }

        // Remove signature, function name doesn't exist
        [Fact]
        public void RemoveCustomFunction_CannotRemoveFunctionWhichDoesntExist_ByNameAndSignature()
        {
            string customFunctionName = "my.ExistingCustomFunction";
            FunctionSignatureWithReturnType customFunctionSignature =
                 new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), EdmCoreModel.Instance.GetBoolean(false));

            // Test
            bool isRemoveSucceeded = CustomUriFunctions.RemoveCustomUriFunction(customFunctionName, customFunctionSignature);

            // Assert
            isRemoveSucceeded.Should().BeFalse();
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
                CustomUriFunctions.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                GetCustomFunctionSignaturesOrNull(customFunctionName)[0].Equals(existingCustomFunctionSignature).Should().BeTrue();

                // Function with different siganture
                FunctionSignatureWithReturnType customFunctionSignatureToRemove =
                        new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetInt16(false), EdmCoreModel.Instance.GetBoolean(false));

                // Test

                // Try Remove a function with the same name but different siganture
                bool isRemoveSucceeded = CustomUriFunctions.RemoveCustomUriFunction(customFunctionName, customFunctionSignatureToRemove);

                // Assert
                isRemoveSucceeded.Should().BeFalse();
            }
            finally
            {
                // Clean up cahce
                CustomUriFunctions.RemoveCustomUriFunction(customFunctionName);
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
                CustomUriFunctions.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                GetCustomFunctionSignaturesOrNull(customFunctionName)[0].Equals(existingCustomFunctionSignature).Should().BeTrue();

                // Test
                bool isRemoveSucceeded = CustomUriFunctions.RemoveCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                // Assert
                isRemoveSucceeded.Should().BeTrue();

                GetCustomFunctionSignaturesOrNull(customFunctionName).Should().BeNull();
            }
            finally
            {
                CustomUriFunctions.RemoveCustomUriFunction(customFunctionName);
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
                CustomUriFunctions.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                FunctionSignatureWithReturnType existingCustomFunctionSignatureTwo =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), EdmCoreModel.Instance.GetDate(false));
                CustomUriFunctions.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignatureTwo);

                // Validate that the two overloads as
                GetCustomFunctionSignaturesOrNull(customFunctionName).
                    All(funcSignature => funcSignature.Equals(existingCustomFunctionSignature) ||
                                            funcSignature.Equals(existingCustomFunctionSignatureTwo)).
                        Should().BeTrue();

                // Remove the first overload, second overload should not be removed
                bool isRemoveSucceeded = CustomUriFunctions.RemoveCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                // Assert
                isRemoveSucceeded.Should().BeTrue();

                FunctionSignatureWithReturnType[] overloads = GetCustomFunctionSignaturesOrNull(customFunctionName);
                overloads.Length.Should().Be(1);
                overloads[0].Should().Be(existingCustomFunctionSignatureTwo);
            }
            finally
            {
                // Clean up cache
                CustomUriFunctions.RemoveCustomUriFunction(customFunctionName);
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
                CustomUriFunctions.AddCustomUriFunction("mystringfunction", myStringFunction);

                var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=mystringfunction(Name, 'BlaBla')");
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("mystringfunction").And.Parameters.ToList();
                startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
                startsWithArgs[1].ShouldBeConstantQueryNode("BlaBla");
            }
            finally
            {
                CustomUriFunctions.RemoveCustomUriFunction("mystringfunction").Should().BeTrue();
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
                CustomUriFunctions.AddCustomUriFunction("myFirstMixedCasestringfunction", myStringFunction);

                // Uri with mixed-case, should work for resolver with case insensitive enabled.
                var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=mYFirstMixedCasesTrInGfUnCtIoN(Name, 'BlaBla')");
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);
                parser.Resolver.EnableCaseInsensitive = true;

                var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("myFirstMixedCasestringfunction")
                    .And.Parameters.ToList();
                startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
                startsWithArgs[1].ShouldBeConstantQueryNode("BlaBla");
            }
            finally
            {
                CustomUriFunctions.RemoveCustomUriFunction("myFirstMixedCasestringfunction").Should().BeTrue();
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
            CustomUriFunctions.AddCustomUriFunction(lowerCaseName, myStringFunction);
            CustomUriFunctions.AddCustomUriFunction(upperCaseName, myStringFunction);
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
                        .And.Parameters.ToList();
                    startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
                    startsWithArgs[1].ShouldBeConstantQueryNode("BlaBla");
                }
            }
            finally
            {
                CustomUriFunctions.RemoveCustomUriFunction(lowerCaseName).Should().BeTrue();
                CustomUriFunctions.RemoveCustomUriFunction(upperCaseName).Should().BeTrue();
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
            CustomUriFunctions.AddCustomUriFunction(lowerCaseName, myStringFunction);
            CustomUriFunctions.AddCustomUriFunction(upperCaseName, myStringFunction);
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
                action.ShouldThrow<ODataException>();
            }
            finally
            {
                CustomUriFunctions.RemoveCustomUriFunction(lowerCaseName).Should().BeTrue();
                CustomUriFunctions.RemoveCustomUriFunction(upperCaseName).Should().BeTrue();
            }
        }

        [Fact]
        public void ParseWithMixedCaseCustomUriFunction_DisableCaseInsensitive_ShouldFailed()
        {
            bool exceptionThrown = false;
            try
            {
                FunctionSignatureWithReturnType myStringFunction
                    = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(true),
                        EdmCoreModel.Instance.GetString(true), EdmCoreModel.Instance.GetString(true));

                // Add a custom uri function
                CustomUriFunctions.AddCustomUriFunction("myMixedCasestringfunction", myStringFunction);

                // Uri with mixed-case, should fail for default resolver with case-insensitive disabled.
                var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=mYMixedCasesTrInGfUnCtIoN(Name, 'BlaBla')");
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel,
                    new Uri("http://www.odata.com/OData/"), fullUri);
                parser.Resolver.EnableCaseInsensitive = false;

                parser.ParseFilter();
            }
            catch (ODataException e)
            {
                Assert.True(e.Message.Equals("An unknown function with name 'mYMixedCasesTrInGfUnCtIoN' was found. " +
                    "This may also be a function import or a key lookup on a navigation property, which is not allowed."));
                exceptionThrown = true;
            }
            finally
            {
                CustomUriFunctions.RemoveCustomUriFunction("myMixedCasestringfunction").Should().BeTrue();
            }

            Assert.True(exceptionThrown, "Exception should be thrown trying to parse mixed-case uri function when case-insensitive is disabled.");
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
                CustomUriFunctions.AddCustomUriFunction("startswith", customStartWithFunctionSignature);

                var fullUri = new Uri("http://www.odata.com/OData/People" + "?$filter=startswith(Name, 66)");
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                var startsWithArgs = parser.ParseFilter().Expression.ShouldBeSingleValueFunctionCallQueryNode("startswith").And.Parameters.ToList();
                startsWithArgs[0].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
                startsWithArgs[1].ShouldBeConstantQueryNode(66);
            }
            finally
            {
                CustomUriFunctions.RemoveCustomUriFunction("startswith").Should().BeTrue();
            }
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
