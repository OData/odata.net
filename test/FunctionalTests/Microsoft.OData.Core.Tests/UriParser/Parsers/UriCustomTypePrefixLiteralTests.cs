using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Core.UriParser.Parsers.TypeParsers.Common;
using Microsoft.OData.Core.UriParser.Parsers.TypeParsers;
using Microsoft.OData.Core.UriParser.Semantic;

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
        /// In fact, this test shows the real power of the customization of UriTypeParser and LiteralPrefix.
        /// Here we can see a special parsing for an EdmType 'Heartbeat' which is a complex type.
        /// The 'Heartbeat' type has it's own LiteralPrefix.
        /// The result is an instacne object of 'Heartbeat' class. 
        /// </summary>
        [Fact]
        public void UriCustomTypePrefixLiteral_CanSetCustomLiteralWithCustomTypeParserCustomType()
        {
            const string HEARTBEAT_TYPE_PREFIX_LITERAL = "myCustomHeartbeatTypePrefixLiteral";
            IUriTypeParser customHeartbeatUriTypePraser = new UriCustomTypeParserUnitTests.HeatBeatCustomUriTypeParser();
            IEdmTypeReference heartbeatTypeReference = UriCustomTypeParserUnitTests.HeatBeatCustomUriTypeParser.HeartbeatComplexType;

            try
            {
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(HEARTBEAT_TYPE_PREFIX_LITERAL, heartbeatTypeReference);
                UriCustomTypeParsers.AddCustomUriTypeParser(heartbeatTypeReference, customHeartbeatUriTypePraser);

                var fullUri = new Uri("http://www.odata.com/OData/Lions" + string.Format("?$filter=LionHeartbeat eq {0}'55.9'", HEARTBEAT_TYPE_PREFIX_LITERAL));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                UriCustomTypeParserUnitTests.HeatBeatCustomUriTypeParser.HeatBeat heartbeatValue =
                  (parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeConvertQueryNode(heartbeatTypeReference).And.Source as ConstantNode).
                  Value.As<UriCustomTypeParserUnitTests.HeatBeatCustomUriTypeParser.HeatBeat>();

                heartbeatValue.Should().NotBeNull();
                heartbeatValue.Frequency.Should().Be(55.9);
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(HEARTBEAT_TYPE_PREFIX_LITERAL);
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customHeartbeatUriTypePraser);
            }
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CanSetCustomLiteralWithCustomTypeParser()
        {
            const string TYPE_PREFIX_LITERAL = "myCustomStringTypePrefixLiteral";
            IUriTypeParser customstringUriTypePraser = new UriCustomTypeParserUnitTests.MyCustomStringUriTypeParser();
            IEdmTypeReference stringTypeReference = EdmCoreModel.Instance.GetString(true);

            try
            {
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL, stringTypeReference);
                UriCustomTypeParsers.AddCustomUriTypeParser(stringTypeReference, customstringUriTypePraser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq {0}'{1}'", TYPE_PREFIX_LITERAL, UriCustomTypeParserUnitTests.CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                    .And.Right.ShouldBeConstantQueryNode(UriCustomTypeParserUnitTests.CUSTOM_PARSER_STRING_VALID_VALUE + UriCustomTypeParserUnitTests.CUSTOM_PARSER_STRING_ADDED_VALUE);
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL);
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customstringUriTypePraser);
            }
        }

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
            const string BOOLEAN_TYPE_PREFIX_LITERAL = "myCustomBooleanTypePrefixLiteral";

            try
            {
                IEdmTypeReference stringTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(BOOLEAN_TYPE_PREFIX_LITERAL, stringTypeReference);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq {0}'{0}'", BOOLEAN_TYPE_PREFIX_LITERAL, UriCustomTypeParserUnitTests.CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parsingFilterAction = ()=> 
                    parser.ParseFilter();

                parsingFilterAction.ShouldThrow<ODataException>();
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(BOOLEAN_TYPE_PREFIX_LITERAL);
            }
        }

        [Fact]
        public void UriCustomTypePrefixLiteral_CanSetCustomLiteralToQuotedValue()
        {
            const string TYPE_PREFIX_LITERAL = "myCustomStringTypePrefixLiteral";
            IUriTypeParser customstringUriTypePraser = new UriCustomTypeParserUnitTests.MyCustomStringUriTypeParser();
            IEdmTypeReference stringTypeReference = EdmCoreModel.Instance.GetString(true);

            try
            {
                CustomUriTypePrefixLiterals.AddCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL, stringTypeReference);
                UriCustomTypeParsers.AddCustomUriTypeParser(stringTypeReference, customstringUriTypePraser);


                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq {0}'{1}'", TYPE_PREFIX_LITERAL, UriCustomTypeParserUnitTests.CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                    .And.Right.ShouldBeConstantQueryNode(UriCustomTypeParserUnitTests.CUSTOM_PARSER_STRING_VALID_VALUE + UriCustomTypeParserUnitTests.CUSTOM_PARSER_STRING_ADDED_VALUE);
            }
            finally
            {
                CustomUriTypePrefixLiterals.RemoveCustomUriTypePrefixLiteral(TYPE_PREFIX_LITERAL);
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customstringUriTypePraser);
            }
        }

        #endregion
    }
}
