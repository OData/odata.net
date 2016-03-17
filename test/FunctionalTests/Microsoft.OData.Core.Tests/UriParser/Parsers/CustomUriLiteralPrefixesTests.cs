using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    /// <summary>
    /// Tests the CustomUriLiteralPreix class public API
    /// </summary>
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

            addCustomUriLiteralPrefix.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CustomUriLiteralPrefix_CannotAddWithEmptyLiteralName()
        {
            // Add Empty literal prefix name
            Action addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(string.Empty, EdmCoreModel.Instance.GetBoolean(false));

            addCustomUriLiteralPrefix.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CustomUriLiteralPrefix_CannotAddWithNullEdmType()
        {
            // Add literal prefix name as null value
            Action addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix("MyCustomLiteralPrefix", null);

            addCustomUriLiteralPrefix.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CustomUriLiteralPrefix_Add_LiteralNameValidation()
        {
            const string INVALID_LITERAL_PREFIX_NUMBER = "myCustomUriLiteralPrefix56";
            const string INVALID_LITERAL_PREFIX_PUNCTUATION = "myCustomUriLiteralPrefix?";
            const string INVALID_LITERAL_PREFIX_WHITESPACE = "myCustomUriLiteralPrefix ";
            const string INVALID_LITERAL_PREFIX_SIGN = "myCustomUriLiteralPrefix*";

            Action addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(INVALID_LITERAL_PREFIX_NUMBER, EdmCoreModel.Instance.GetBoolean(false));
            addCustomUriLiteralPrefix.ShouldThrow<ArgumentException>();

            addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(INVALID_LITERAL_PREFIX_PUNCTUATION, EdmCoreModel.Instance.GetBoolean(false));
            addCustomUriLiteralPrefix.ShouldThrow<ArgumentException>();

            addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(INVALID_LITERAL_PREFIX_WHITESPACE, EdmCoreModel.Instance.GetBoolean(false));
            addCustomUriLiteralPrefix.ShouldThrow<ArgumentException>();

            addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(INVALID_LITERAL_PREFIX_SIGN, EdmCoreModel.Instance.GetBoolean(false));
            addCustomUriLiteralPrefix.ShouldThrow<ArgumentException>();

            const string VALID_LITERAL_PREFIX_LETTERS = "myCustomUriLiteralPrefix";
            const string VALID_LITERAL_PREFIX_POINT = "myCustom.LiteralPrefix";

            try
            {
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(VALID_LITERAL_PREFIX_LETTERS, EdmCoreModel.Instance.GetBoolean(false));
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(VALID_LITERAL_PREFIX_POINT, EdmCoreModel.Instance.GetBoolean(false));
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(VALID_LITERAL_PREFIX_LETTERS);
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(VALID_LITERAL_PREFIX_POINT);
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

                addCustomUriLiteralPrefix.ShouldThrow<ODataException>().
                    WithMessage(Strings.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists(LITERAL_PREFIX));
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(LITERAL_PREFIX).Should().BeTrue();
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

                addCustomUriLiteralPrefix.ShouldThrow<ODataException>().
                    WithMessage(Strings.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists(LITERAL_PREFIX));
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(LITERAL_PREFIX).Should().BeTrue();
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

                CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(FIRST_LITERAL_PREFIX).IsEquivalentTo(booleanTypeReference).Should().BeTrue();
                CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(SECOND_LITERAL_PREFIX).IsEquivalentTo(booleanTypeReference).Should().BeTrue();
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(FIRST_LITERAL_PREFIX).Should().BeTrue();
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(SECOND_LITERAL_PREFIX).Should().BeTrue();
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

                CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(FIRST_LITERAL_PREFIX).IsEquivalentTo(first_booleanTypeReference).Should().BeTrue();
                CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(SECOND_LITERAL_PREFIX).IsEquivalentTo(second_stringTypeReference).Should().BeTrue();
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(FIRST_LITERAL_PREFIX).Should().BeTrue();
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(SECOND_LITERAL_PREFIX).Should().BeTrue();
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

                CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(EXISITING_BUILTIN_LITERAL_NAME).IsEquivalentTo(first_booleanTypeReference).Should().BeTrue();
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(EXISITING_BUILTIN_LITERAL_NAME).Should().BeTrue();
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

            removeCustomUriLiteralPrefix.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CustomUriLiteralPrefix_CannotRemoveWithEmptyLiteralName()
        {
            // Rempve empty literal prefix name
            Action removeCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(string.Empty);

            removeCustomUriLiteralPrefix.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CustomUriLiteralPrefix_Remove_LiteralNameValidation()
        {
            const string INVALID_LITERAL_PREFIX_NUMBER = "myCustomUriLiteralPrefix56";
            const string INVALID_LITERAL_PREFIX_PUNCTUATION = "myCustomUriLiteralPrefix?";
            const string INVALID_LITERAL_PREFIX_WHITESPACE = "myCustomUriLiteralPrefix ";
            const string INVALID_LITERAL_PREFIX_SIGN = "myCustomUriLiteralPrefix*";

            Action addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(INVALID_LITERAL_PREFIX_NUMBER);
            addCustomUriLiteralPrefix.ShouldThrow<ArgumentException>();

            addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(INVALID_LITERAL_PREFIX_PUNCTUATION);
            addCustomUriLiteralPrefix.ShouldThrow<ArgumentException>();

            addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(INVALID_LITERAL_PREFIX_WHITESPACE);
            addCustomUriLiteralPrefix.ShouldThrow<ArgumentException>();

            addCustomUriLiteralPrefix = () =>
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(INVALID_LITERAL_PREFIX_SIGN);
            addCustomUriLiteralPrefix.ShouldThrow<ArgumentException>();

            const string VALID_LITERAL_PREFIX_LETTERS = "myCustomUriLiteralPrefix";
            const string VALID_LITERAL_PREFIX_POINT = "myCustom.LiteralPrefix";

            try
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(VALID_LITERAL_PREFIX_LETTERS);
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(VALID_LITERAL_PREFIX_POINT);
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(VALID_LITERAL_PREFIX_LETTERS);
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(VALID_LITERAL_PREFIX_POINT);
            }
        }

        [Fact]
        public void CustomUriLiteralPrefix_CanRemoveExistingLiteral()
        {
            const string LITERAL_PREFIX = "myCustomUriLiteralPrefix";

            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
            CustomUriLiteralPrefixes.AddCustomLiteralPrefix(LITERAL_PREFIX, booleanTypeReference);

            CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(LITERAL_PREFIX).Should().BeTrue();
        }

        [Fact]
        public void CustomUriLiteralPrefix_CannotRemoveNotExistingLiteral()
        {
            const string LITERAL_PREFIX = "myCustomUriLiteralPrefix";

            CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(LITERAL_PREFIX).Should().BeFalse();
        }

        #endregion

        #region GetEdmTypeByCustomLiteralPrefix Method

        [Fact]
        public void CustomUriLiteralPrefix_GetNotExistingPrefix()
        {
            const string NOT_EXISTING_PREFIX_NAME = "NOT_EXISTING_PREFIX_NAME";

            IEdmTypeReference edmType =
                CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(NOT_EXISTING_PREFIX_NAME);

            edmType.Should().BeNull();
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
                parsingFilter.ShouldThrow<ODataException>();
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(STRING_LITERAL_PREFIX);
            }
        }

        [Fact]
        public void CustomUriLiteralPrefix_CannotParseTypeWithWrongLiteralPrefix()
        {
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(CustomUriLiteralParserUnitTests.BOOLEAN_LITERAL_PREFIX, booleanTypeReference);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq {0}'{1}'", CustomUriLiteralParserUnitTests.BOOLEAN_LITERAL_PREFIX, CustomUriLiteralParserUnitTests.CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parsingFilterAction = () =>
                    parser.ParseFilter();

                parsingFilterAction.ShouldThrow<ODataException>();
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(CustomUriLiteralParserUnitTests.BOOLEAN_LITERAL_PREFIX);
            }
        }
        #endregion
    }
}
