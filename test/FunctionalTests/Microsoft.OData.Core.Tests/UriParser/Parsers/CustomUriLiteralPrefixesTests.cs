//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralPrefixUnitsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    /// <summary>
    /// Tests the CustomUriLiteralPreix class public API
    /// </summary>
    [Collection("CustomUriLiteralTests")] // these tests modify the shared CustomUriLiteralPrefixes
    public class CustomUriLiteralPrefixUnitsTests
    {
        #region AddCustomLiteralPrefix Method

        // Validation
        [Fact]
        public void CustomUriLiteralPrefix_CannotAddWithNullLiteralName()
        {
            // Add null literal prefix name
            Action addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(null, EdmCoreModel.Instance.GetBoolean(false));

            Assert.Throws<ArgumentNullException>("literalPrefix", addCustomUriLiteralPrefix);
        }

        [Fact]
        public void CustomUriLiteralPrefix_CannotAddWithEmptyLiteralName()
        {
            // Add Empty literal prefix name
            Action addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(string.Empty, EdmCoreModel.Instance.GetBoolean(false));

            Assert.Throws<ArgumentNullException>("literalPrefix", addCustomUriLiteralPrefix);
        }

        [Fact]
        public void CustomUriLiteralPrefix_CannotAddWithNullEdmType()
        {
            // Add literal prefix name as null value
            Action addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix("MyCustomLiteralPrefix", null);

            Assert.Throws<ArgumentNullException>("literalEdmTypeReference", addCustomUriLiteralPrefix);
        }

        [Theory]
        [InlineData("myCustomUriLiteralPrefix56")]
        [InlineData("myCustomUriLiteralPrefix?")]
        [InlineData("myCustomUriLiteralPrefix ")]
        [InlineData("myCustomUriLiteralPrefix*")]
        public void CustomUriLiteralPrefix_Add_InvalidLiteralNameThrows(string literalPrefix)
        {
            Action addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(literalPrefix, EdmCoreModel.Instance.GetBoolean(false));

            addCustomUriLiteralPrefix.Throws<ArgumentException>(Strings.UriParserHelper_InvalidPrefixLiteral(literalPrefix));
        }

        [Theory]
        [InlineData("myCustomUriLiteralPrefix")]
        [InlineData("myCustom.LiteralPrefix")]
        public void CustomUriLiteralPrefix_Add_ValidLiteralNamePassValidation(string literalPrefix)
        {
            try
            {
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(literalPrefix, EdmCoreModel.Instance.GetBoolean(false));
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(literalPrefix);
            }
        }

        // AddCustom type prefix literal
        [Fact]
        public void CustomUriLiteralPrefix_CannotAddExistingPrefixNameWithExistingEdmType()
        {
            const string LITERAL_PREFIX = "myCustomUriLiteralPrefix";

            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(LITERAL_PREFIX, booleanTypeReference);

                // Should throw exception
                Action addCustomUriLiteralPrefix = () =>
                    CustomUriLiteralPrefixes.AddCustomLiteralPrefix(LITERAL_PREFIX, booleanTypeReference);

                addCustomUriLiteralPrefix.Throws<ODataException>(Strings.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists(LITERAL_PREFIX));
            }
            finally
            {
                Assert.True(CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(LITERAL_PREFIX));
            }
        }

        [Fact]
        public void CustomUriLiteralPrefix_CannotAddExistingPrefixNameWithDifferentEdmType()
        {
            const string LITERAL_PREFIX = "myCustomUriLiteralPrefix";

            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(LITERAL_PREFIX, booleanTypeReference);

                IEdmTypeReference intTypeReference = EdmCoreModel.Instance.GetInt32(false);

                // Should throw exception
                Action addCustomUriLiteralPrefix = () =>
                    CustomUriLiteralPrefixes.AddCustomLiteralPrefix(LITERAL_PREFIX, intTypeReference);

                addCustomUriLiteralPrefix.Throws<ODataException>(Strings.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists(LITERAL_PREFIX));
            }
            finally
            {
                Assert.True(CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(LITERAL_PREFIX));
            }
        }

        [Fact]
        public void CustomUriLiteralPrefix_CanAddDifferentPrefixNameWithExistingEdmTypeWith()
        {
            const string FIRST_LITERAL_PREFIX = "myFirstCustomLiteralPrefix";
            const string SECOND_LITERAL_PREFIX = "mySecondCustomLiteralPrefix";

            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(FIRST_LITERAL_PREFIX, booleanTypeReference);

                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(SECOND_LITERAL_PREFIX, booleanTypeReference);

                Assert.True(CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(FIRST_LITERAL_PREFIX).IsEquivalentTo(booleanTypeReference));
                Assert.True(CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(SECOND_LITERAL_PREFIX).IsEquivalentTo(booleanTypeReference));
            }
            finally
            {
                Assert.True(CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(FIRST_LITERAL_PREFIX));
                Assert.True(CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(SECOND_LITERAL_PREFIX));
            }
        }

        [Fact]
        public void CustomUriLiteralPrefix_CanAddDifferentPrefixNameWithDifferentEdmTypeWith()
        {
            const string FIRST_LITERAL_PREFIX = "myFirstCustomLiteralPrefix";
            const string SECOND_LITERAL_PREFIX = "mySecondCustomLiteralPrefix";

            try
            {
                IEdmTypeReference first_booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(FIRST_LITERAL_PREFIX, first_booleanTypeReference);

                IEdmTypeReference second_stringTypeReference = EdmCoreModel.Instance.GetString(false);
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(SECOND_LITERAL_PREFIX, second_stringTypeReference);

                Assert.True(CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(FIRST_LITERAL_PREFIX).IsEquivalentTo(first_booleanTypeReference));
                Assert.True(CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(SECOND_LITERAL_PREFIX).IsEquivalentTo(second_stringTypeReference));
            }
            finally
            {
                Assert.True(CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(FIRST_LITERAL_PREFIX));
                Assert.True(CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(SECOND_LITERAL_PREFIX));
            }
        }

        [Fact]
        public void CustomUriLiteralPrefix_CanAddLiteralPrefixNameOfBuiltInLiteral()
        {
            const string EXISITING_BUILTIN_LITERAL_NAME = "geometry";

            try
            {
                IEdmTypeReference first_booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(EXISITING_BUILTIN_LITERAL_NAME, first_booleanTypeReference);

                Assert.True(CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(EXISITING_BUILTIN_LITERAL_NAME).IsEquivalentTo(first_booleanTypeReference));
            }
            finally
            {
                Assert.True(CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(EXISITING_BUILTIN_LITERAL_NAME));
            }
        }

        #endregion

        #region RemoveCustomLiteralPrefix Method

        [Fact]
        public void CustomUriLiteralPrefix_CannotRemoveWithNullLiteralName()
        {
            // Remove 'null' literal prefix name
            Action removeCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(null);

            Assert.Throws<ArgumentNullException>("literalPrefix", removeCustomUriLiteralPrefix);
        }

        [Fact]
        public void CustomUriLiteralPrefix_CannotRemoveWithEmptyLiteralName()
        {
            // Rempve empty literal prefix name
            Action removeCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(string.Empty);

            Assert.Throws<ArgumentNullException>("literalPrefix", removeCustomUriLiteralPrefix);
        }

        [Theory]
        [InlineData("myCustomUriLiteralPrefix56")]
        [InlineData("myCustomUriLiteralPrefix?")]
        [InlineData("myCustomUriLiteralPrefix ")]
        [InlineData("myCustomUriLiteralPrefix*")]
        public void CustomUriLiteralPrefix_Remove_InvalidLiteralNameThrows(string literalPrefix)
        {
            Action addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(literalPrefix);

            addCustomUriLiteralPrefix.Throws<ArgumentException>(Strings.UriParserHelper_InvalidPrefixLiteral(literalPrefix));
        }

        [Theory]
        [InlineData("myCustomUriLiteralPrefix")]
        [InlineData("myCustom.LiteralPrefix")]
        public void CustomUriLiteralPrefix_Remove_ValidLiteralNameValidation(string literalPrefix)
        {
            try
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(literalPrefix);
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(literalPrefix);
            }
        }

        [Fact]
        public void CustomUriLiteralPrefix_CanRemoveExistingLiteral()
        {
            const string LITERAL_PREFIX = "myCustomUriLiteralPrefix";

            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
            CustomUriLiteralPrefixes.AddCustomLiteralPrefix(LITERAL_PREFIX, booleanTypeReference);

            Assert.True(CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(LITERAL_PREFIX));
        }

        [Fact]
        public void CustomUriLiteralPrefix_CannotRemoveNotExistingLiteral()
        {
            const string LITERAL_PREFIX = "myCustomUriLiteralPrefix";

            Assert.False(CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(LITERAL_PREFIX));
        }

        #endregion

        #region GetEdmTypeByCustomLiteralPrefix Method

        [Fact]
        public void CustomUriLiteralPrefix_GetNotExistingPrefix()
        {
            const string NOT_EXISTING_PREFIX_NAME = "NOT_EXISTING_PREFIX_NAME";

            IEdmTypeReference edmType =
                CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(NOT_EXISTING_PREFIX_NAME);

            Assert.Null(edmType);
        }

        #endregion

        #region ODataUriParser

        /// <summary>
        /// In this case, the built-in parser for Edm.String cannot parse  myCustomStringLiteralPrefix'MyString' so it fails.
        /// Edm.String is just an example.
        /// </summary>
        [Fact]
        public void CustomUriLiteralPrefix_CannotParseWithCustomLiteralPrefix_IfBuiltInParserDontRecognizeCustomLiteral()
        {
            const string STRING_LITERAL_PREFIX = "myCustomStringLiteralPrefix";

            try
            {
                IEdmTypeReference stringTypeReference = EdmCoreModel.Instance.GetString(true);
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(STRING_LITERAL_PREFIX, stringTypeReference);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq {0}'{1}'", STRING_LITERAL_PREFIX, CustomUriLiteralParserUnitTests.CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parsingFilter = () => parser.ParseFilter();
                Assert.Throws<ODataException>(parsingFilter);
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(STRING_LITERAL_PREFIX);
            }
        }

        [Fact]
        public void CustomUriLiteralPrefix_CannotParseTypeWithWrongLiteralPrefix()
        {
            // Ensure the prefix under test is registered. Handle the exception gracefully if
            // the prefix is already registered.
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
            //const string BOOLEAN_LITERAL_PREFIX = "booleanLiteralPrefix";

            try
            {
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(
                    CustomUriLiteralParserUnitTests.BOOLEAN_LITERAL_PREFIX, booleanTypeReference);
            }
            catch (ODataException e)
            {
                if (!String.Equals(e.Message, Strings.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists(
                    CustomUriLiteralParserUnitTests.BOOLEAN_LITERAL_PREFIX), StringComparison.Ordinal))
                {
                    // unexpected exception, re-throw.
                    throw;
                }

                // Swallow the exception since it is due to trying to register a prefix that is already added.
            }

            try
            {


                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq {0}'{1}'", CustomUriLiteralParserUnitTests.BOOLEAN_LITERAL_PREFIX, CustomUriLiteralParserUnitTests.CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parsingFilterAction = () =>
                    parser.ParseFilter();

                Assert.Throws<ODataException>(parsingFilterAction);
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(CustomUriLiteralParserUnitTests.BOOLEAN_LITERAL_PREFIX);
            }
        }
        #endregion
    }
}
