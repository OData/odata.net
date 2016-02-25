using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers.TypeParsers;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    /// <summary>
    /// Tests the CustomTypePreixLiteral class public API
    /// </summary>
    public class UriCustomTypePrefixLiteralUnitsTests
    {
        #region AddCustomUriTypePrefixLiteral Method

        // Validation
        [Fact]
        public void UriCustomTypePrefixLiteral_CannotAddWithNullLiteralName()
        {
            // Add null literal prefix name
            Action addUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(null, EdmCoreModel.Instance.GetBoolean(false));

            addUriCustomTypeLitral.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CannotAddWithEmptyLiteralName()
        {
            // Add Empty literal prefix name
            Action addUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(string.Empty, EdmCoreModel.Instance.GetBoolean(false));

            addUriCustomTypeLitral.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CannotAddWithNullEdmType()
        {
            // Add literal prefix name as null value
            Action addUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral("MyCustomLiteralPrefix", null);

            addUriCustomTypeLitral.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_Add_LiteralNameValidation()
        {
            const string INVALID_TYPE_PREFIX_LITERAL_NUMBER = "myCustomTypePrefixLiteral56";
            const string INVALID_TYPE_PREFIX_LITERAL_PUNCTUATION = "myCustomTypePrefixLiteral?";
            const string INVALID_TYPE_PREFIX_LITERAL_WHITESPACE = "myCustomTypePrefixLiteral ";
            const string INVALID_TYPE_PREFIX_LITERAL_SIGN = "myCustomTypePrefixLiteral*";

            Action addUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(INVALID_TYPE_PREFIX_LITERAL_NUMBER, EdmCoreModel.Instance.GetBoolean(false));
            addUriCustomTypeLitral.ShouldThrow<ArgumentException>();

            addUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(INVALID_TYPE_PREFIX_LITERAL_PUNCTUATION, EdmCoreModel.Instance.GetBoolean(false));
            addUriCustomTypeLitral.ShouldThrow<ArgumentException>();

            addUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(INVALID_TYPE_PREFIX_LITERAL_WHITESPACE, EdmCoreModel.Instance.GetBoolean(false));
            addUriCustomTypeLitral.ShouldThrow<ArgumentException>();

            addUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(INVALID_TYPE_PREFIX_LITERAL_SIGN, EdmCoreModel.Instance.GetBoolean(false));
            addUriCustomTypeLitral.ShouldThrow<ArgumentException>();

            const string VALID_TYPE_PREFIX_LITERAL_LETTERS = "myCustomTypePrefixLiteral";
            const string VALID_TYPE_PREFIX_LITERAL_POINT = "myCustom.TypePrefixLiteral";

            try
            {
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(VALID_TYPE_PREFIX_LITERAL_LETTERS, EdmCoreModel.Instance.GetBoolean(false));
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(VALID_TYPE_PREFIX_LITERAL_POINT, EdmCoreModel.Instance.GetBoolean(false));
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(VALID_TYPE_PREFIX_LITERAL_LETTERS);
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(VALID_TYPE_PREFIX_LITERAL_POINT);
            }
        }

        // AddCustom type prefix literal
        [Fact]
        public void UriCustomTypePrefixLiteral_CannotAddExistingPrefixNameWithExistingEdmType()
        {
            const string TYPE_PREFIX_LITERAL = "myCustomTypePrefixLiteral";

            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL, booleanTypeReference);

                // Should throw exception
                Action addUriCustomTypeLitral = () =>
                    CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL, booleanTypeReference);

                addUriCustomTypeLitral.ShouldThrow<ODataException>().
                    WithMessage(Strings.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists(TYPE_PREFIX_LITERAL));
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL).Should().BeTrue();
            }
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CannotAddExistingPrefixNameWithDifferentEdmType()
        {
            const string TYPE_PREFIX_LITERAL = "myCustomTypePrefixLiteral";

            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL, booleanTypeReference);

                IEdmTypeReference intTypeReference = EdmCoreModel.Instance.GetInt32(false);

                // Should throw exception
                Action addUriCustomTypeLitral = () =>
                    CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL, intTypeReference);

                addUriCustomTypeLitral.ShouldThrow<ODataException>().
                    WithMessage(Strings.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists(TYPE_PREFIX_LITERAL));
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL).Should().BeTrue();
            }
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CanAddDifferentPrefixNameWithExistingEdmTypeWith()
        {
            const string FIRST_TYPE_PREFIX_LITERAL = "myFirstCustomTypePrefixLiteral";
            const string SECOND_TYPE_PREFIX_LITERAL = "mySecondCustomTypePrefixLiteral";

            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(FIRST_TYPE_PREFIX_LITERAL, booleanTypeReference);

                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(SECOND_TYPE_PREFIX_LITERAL, booleanTypeReference);

                CustomUriTypePrefixLiterals.GetCustomEdmTypeByLiteralPrefix(FIRST_TYPE_PREFIX_LITERAL).IsEquivalentTo(booleanTypeReference).Should().BeTrue();
                CustomUriTypePrefixLiterals.GetCustomEdmTypeByLiteralPrefix(SECOND_TYPE_PREFIX_LITERAL).IsEquivalentTo(booleanTypeReference).Should().BeTrue();
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(FIRST_TYPE_PREFIX_LITERAL).Should().BeTrue();
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(SECOND_TYPE_PREFIX_LITERAL).Should().BeTrue();
            }
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CanAddDifferentPrefixNameWithDifferentEdmTypeWith()
        {
            const string FIRST_TYPE_PREFIX_LITERAL = "myFirstCustomTypePrefixLiteral";
            const string SECOND_TYPE_PREFIX_LITERAL = "mySecondCustomTypePrefixLiteral";

            try
            {
                IEdmTypeReference first_booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(FIRST_TYPE_PREFIX_LITERAL, first_booleanTypeReference);

                IEdmTypeReference second_stringTypeReference = EdmCoreModel.Instance.GetString(false);
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(SECOND_TYPE_PREFIX_LITERAL, second_stringTypeReference);

                CustomUriTypePrefixLiterals.GetCustomEdmTypeByLiteralPrefix(FIRST_TYPE_PREFIX_LITERAL).IsEquivalentTo(first_booleanTypeReference).Should().BeTrue();
                CustomUriTypePrefixLiterals.GetCustomEdmTypeByLiteralPrefix(SECOND_TYPE_PREFIX_LITERAL).IsEquivalentTo(second_stringTypeReference).Should().BeTrue();
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(FIRST_TYPE_PREFIX_LITERAL).Should().BeTrue();
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(SECOND_TYPE_PREFIX_LITERAL).Should().BeTrue();
            }
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CanAddLiteralPrefixNameOfBuiltInLiteral()
        {
            const string EXISITING_BUILTIN_LITERAL_NAME = "geometry";

            try
            {
                IEdmTypeReference first_booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(EXISITING_BUILTIN_LITERAL_NAME, first_booleanTypeReference);

                CustomUriTypePrefixLiterals.GetCustomEdmTypeByLiteralPrefix(EXISITING_BUILTIN_LITERAL_NAME).IsEquivalentTo(first_booleanTypeReference).Should().BeTrue();
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(EXISITING_BUILTIN_LITERAL_NAME).Should().BeTrue();
            }
        }

        #endregion

        #region RemoveCustomUriTypePrefixLiteral Method

        [Fact]
        public void UriCustomTypePrefixLiteral_CannotRemoveWithNullLiteralName()
        {
            // Remove 'null' literal prefix name
            Action removeUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(null);

            removeUriCustomTypeLitral.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CannotRemoveWithEmptyLiteralName()
        {
            // Rempve empty literal prefix name
            Action removeUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(string.Empty);

            removeUriCustomTypeLitral.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_Remove_LiteralNameValidation()
        {
            const string INVALID_TYPE_PREFIX_LITERAL_NUMBER = "myCustomTypePrefixLiteral56";
            const string INVALID_TYPE_PREFIX_LITERAL_PUNCTUATION = "myCustomTypePrefixLiteral?";
            const string INVALID_TYPE_PREFIX_LITERAL_WHITESPACE = "myCustomTypePrefixLiteral ";
            const string INVALID_TYPE_PREFIX_LITERAL_SIGN = "myCustomTypePrefixLiteral*";

            Action addUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(INVALID_TYPE_PREFIX_LITERAL_NUMBER);
            addUriCustomTypeLitral.ShouldThrow<ArgumentException>();

            addUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(INVALID_TYPE_PREFIX_LITERAL_PUNCTUATION);
            addUriCustomTypeLitral.ShouldThrow<ArgumentException>();

            addUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(INVALID_TYPE_PREFIX_LITERAL_WHITESPACE);
            addUriCustomTypeLitral.ShouldThrow<ArgumentException>();

            addUriCustomTypeLitral = () =>
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(INVALID_TYPE_PREFIX_LITERAL_SIGN);
            addUriCustomTypeLitral.ShouldThrow<ArgumentException>();

            const string VALID_TYPE_PREFIX_LITERAL_LETTERS = "myCustomTypePrefixLiteral";
            const string VALID_TYPE_PREFIX_LITERAL_POINT = "myCustom.TypePrefixLiteral";

            try
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(VALID_TYPE_PREFIX_LITERAL_LETTERS);
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(VALID_TYPE_PREFIX_LITERAL_POINT);
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(VALID_TYPE_PREFIX_LITERAL_LETTERS);
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(VALID_TYPE_PREFIX_LITERAL_POINT);
            }
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CanRemoveExistingLiteral()
        {
            const string TYPE_PREFIX_LITERAL = "myCustomTypePrefixLiteral";

            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
            CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL, booleanTypeReference);

            CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL).Should().BeTrue();
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CannotRemoveNotExistingLiteral()
        {
            const string TYPE_PREFIX_LITERAL = "myCustomTypePrefixLiteral";

            CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL).Should().BeFalse();
        }

        #endregion

        #region GetCustomEdmTypeByLiteralPrefix Method

        [Fact]
        public void UriCustomTypePrefixLiteral_GetNotExistingPrefix()
        {
            const string NOT_EXISTING_PREFIX_NAME = "NOT_EXISTING_PREFIX_NAME";

            IEdmTypeReference typePrefixEdmType =
                CustomUriTypePrefixLiterals.GetCustomEdmTypeByLiteralPrefix(NOT_EXISTING_PREFIX_NAME);

            typePrefixEdmType.Should().BeNull();
        }

        #endregion

        #region ODataUriParser

        /// <summary>
        /// In this case, the built-in parser for Edm.String cannot parse  myCustomStringTypePrefixLiteral'MyString' so it fails.
        /// Edm.String is just an example.
        /// </summary>
        [Fact]
        public void UriCustomTypePrefixLiteral_CannotParseWithCustomLiteralPrefix_IfBuiltInParserDontRecognizeCustomLiteral()
        {
            const string STRING_TYPE_PREFIX_LITERAL = "myCustomStringTypePrefixLiteral";

            try
            {
                IEdmTypeReference stringTypeReference = EdmCoreModel.Instance.GetString(true);
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(STRING_TYPE_PREFIX_LITERAL, stringTypeReference);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq {0}'{1}'", STRING_TYPE_PREFIX_LITERAL, UriCustomTypeParserUnitTests.CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parsingFilter = () => parser.ParseFilter();
                parsingFilter.ShouldThrow<ODataException>();
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(STRING_TYPE_PREFIX_LITERAL);
            }
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CannotParseTypeWithWrongLiteralPrefix()
        {
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(UriCustomTypeParserUnitTests.BOOLEAN_TYPE_PREFIX_LITERAL, booleanTypeReference);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq {0}'{1}'", UriCustomTypeParserUnitTests.BOOLEAN_TYPE_PREFIX_LITERAL, UriCustomTypeParserUnitTests.CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parsingFilterAction = () =>
                    parser.ParseFilter();

                parsingFilterAction.ShouldThrow<ODataException>();
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(UriCustomTypeParserUnitTests.BOOLEAN_TYPE_PREFIX_LITERAL);
            }
        }
        #endregion
    }
}
