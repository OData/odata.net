using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers.TypeParsers;
using Microsoft.OData.Core.UriParser.Parsers.TypeParsers.Common;
using Microsoft.OData.Core.UriParser.Parsers.UriParsers;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    /// <summary>
    /// Test the public API of UriCustomTypeParser class
    /// </summary>
    public class UriCustomTypeParserUnitTests
    {
        #region Consts

        public const string CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE = "NonConvetionalBooleanTrue";

        public const string CUSTOM_PARSER_BOOLEAN_INVALID_VALUE = "InvalidValueOfNonConvertionalBoolean";

        public const string CUSTOM_PARSER_BOOLEAN_VALUE_TRUE_VALID = "ValidValueOfNonConvertionalBooleanTrueForRegisteredFalseForGeneral";

        public const string CUSTOM_PARSER_INT_VALID_VALUE = "NonConvetionalInt55";

        public const string CUSTOM_PARSER_STRING_VALID_VALUE = "BlaBla";

        public const string CUSTOM_PARSER_STRING_ADDED_VALUE = "MyCoolStringParserAddedValue";

        public const string CUSTOM_PARSER_STRING_VALUE_CAUSEBUG = "StringValueWithBug";

        #endregion

        #region AddCustomUriTypeParser Method

		// ** Test'AddCustomUriTypeParser' Methods

        // Validation

        // Can add 
        // Cannot add if same instace already exists
        // Can add multiple different
        
        // Can add
        // Cannot add if EdmType already exists
        // Can add if same UriParser exists but different Edm
        // Can add multiple different

        // Can add if same UriParser for specific Edm if exist as general parser
        // Can add to general parser if already registred with EdmType
        // Can add different instance to each registered and general

        [Fact]
        public void AddCustomUriTypeParser_CannotAddNullTypeParser()
        {
            Action addNullCustomUriTypeParser = () =>
                UriCustomTypeParsers.AddCustomUriTypeParser(null);

            addNullCustomUriTypeParser.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddCustomUriTypeParser_CannotAddNullTypeParserAndNullEdmTypeReference()
        {
            Action addNullCustomUriTypeParserAndNullParser = () =>
                UriCustomTypeParsers.AddCustomUriTypeParser(null, null);

            addNullCustomUriTypeParserAndNullParser.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddCustomUriTypeParser_CannotAddNullTypeParserWithEdmType()
        {
            IEdmTypeReference typeReference = EdmCoreModel.Instance.GetBoolean(false);

            Action addNullCustomUriTypeParser = () =>
                UriCustomTypeParsers.AddCustomUriTypeParser(typeReference, null);

            addNullCustomUriTypeParser.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddCustomUriTypeParser_CannotAddNullEdmTypeReferenceWithTypeParser()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser =
                new MyCustomBooleanUriTypeParser();

            Action addCustomUriTypeParserWithNullEdmType = () =>
                UriCustomTypeParsers.AddCustomUriTypeParser(null, customBooleanUriTypePraser);

            addCustomUriTypeParserWithNullEdmType.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddCustomUriTypeParser_GeneralParsers_CanAdd()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();

            try
            {
                UriCustomTypeParsers.AddCustomUriTypeParser(customBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
            }
            finally
            {
                // Clean up from cache
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void AddCustomUriTypeParser_GeneralParsers_CanAddMultipleDifferentInstances()
        {
            IUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            IUriTypeParser customIntBooleanUriTypePraser = new MyCustomIntAndBooleanUriTypeParser();

            try
            {
                // Add two different instances to GeneralTypeParsers
                UriCustomTypeParsers.AddCustomUriTypeParser(customBooleanUriTypePraser);
                UriCustomTypeParsers.AddCustomUriTypeParser(customIntBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
                this.ParseNonConvetionalIntValueSuccessfully();
            }
            finally
            {
                // Clean up from cache
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customIntBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void AddCustomUriTypeParser_GeneralParsers_CannotAddToGeneraldIfSameInstanceAlreadyExists()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();

            try
            {
                // Add Once
                UriCustomTypeParsers.AddCustomUriTypeParser(customBooleanUriTypePraser);

                // Add again - Should throw exception
                Action addCustomUriTypeParser = () =>
                    UriCustomTypeParsers.AddCustomUriTypeParser(customBooleanUriTypePraser);

                addCustomUriTypeParser.ShouldThrow<ODataException>().
                    WithMessage(Strings.UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists);
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void AddCustomUriTypeParser_RegisterToEdmType_CanAddCustomParser()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            try
            {
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void AddCustomUriTypeParser_RegisterToEdmType_CanAddMultipleDifferentInstances()
        {
            IUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            IUriTypeParser customIntBooleanUriTypePraser = new MyCustomIntAndBooleanUriTypeParser();
            IEdmTypeReference intEdmTypeReference = EdmCoreModel.Instance.GetInt32(false);

            try
            {
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);
                UriCustomTypeParsers.AddCustomUriTypeParser(intEdmTypeReference, customIntBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
                this.ParseNonConvetionalIntValueSuccessfully();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customIntBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void AddCustomUriTypeParser_RegisterToEdmType_CannotAddIfAlreadyRegistedToTheSameEdmType()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
            try
            {
                // Add once
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

                // Add again - Should throw exception
                Action addCustomUriTypeParser = () =>
                    UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

                addCustomUriTypeParser.ShouldThrow<ODataException>().
                    WithMessage(Strings.UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists(booleanTypeReference.FullName()));
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void AddCustomUriTypeParser_RegisterToEdmType_CanAddIfSameParserInstanceExistsButRegisteredToDifferentEdmType()
        {
            IUriTypeParser customIntAndBooleanUriTypePraser = new MyCustomIntAndBooleanUriTypeParser();

            try
            {
                // Add once
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customIntAndBooleanUriTypePraser);

                // Add same type converter but registered to a different EdmType(string instead of boolean)
                IEdmTypeReference intTypeReference = EdmCoreModel.Instance.GetInt32(false);
                UriCustomTypeParsers.AddCustomUriTypeParser(intTypeReference, customIntAndBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
                this.ParseNonConvetionalIntValueSuccessfully();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customIntAndBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void AddCustomUriTypeParser_RegisterToEdmType_CanAddIfSameParserInstanceExistsAsGeneralTypeParser()
        {
            IUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();

            try
            {
                // Add once as general TypeParser (with no specific EdmType)
                UriCustomTypeParsers.AddCustomUriTypeParser(customBooleanUriTypePraser);

                // Add again with registered EdmType
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void AddCustomUriTypeParser_GeneralParsers_CanAddIfSameParserInstanceAlreadyRegisteredToEdmType()
        {
            IUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();

            try
            {
                // Add once with registered EdmType
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

                // Add again as general TypeParser (with no specific EdmType)
                UriCustomTypeParsers.AddCustomUriTypeParser(customBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void AddCustomUriTypeParser_CanAddMultipleDifferentInstancesToRegisteredAndGeneral()
        {
            IUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            IUriTypeParser customIntBooleanUriTypePraser = new MyCustomIntAndBooleanUriTypeParser();

            try
            {
                // Add to registered edm types
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

                // Add to general parsers
                UriCustomTypeParsers.AddCustomUriTypeParser(customIntBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
                this.ParseNonConvetionalIntValueSuccessfully();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customIntBooleanUriTypePraser).Should().BeTrue();
            }
        }

	    #endregion        
        
        #region RemoveCustomUriTypeParser Method
		
        // ** Test 'RemoveCustomUriTypeParser' Method **

        // Remove Validation
        // Cannot remove parser which is not added
        // Can remove in general parser
        // Can remove in registered Edm
        // Can remove if exists in both
        // Can remove if exists in general and 2 in registred

        [Fact]
        public void RemoveCustomUriTypeParser_CannotRemoveNull()
        {
            Action removeNullCustomUriTypeParser = () =>
                UriCustomTypeParsers.RemoveCustomUriTypeParser(null);

            removeNullCustomUriTypeParser.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void RemoveCustomUriTypeParser_CannotRemoveNotExistingParser()
        {
            IUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            bool isRemoveSucceeded = UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser);

            // Assert
            isRemoveSucceeded.Should().BeFalse();
        }

        [Fact]
        public void RemoveCustomUriTypeParser_CanRemoveGeneralParser()
        {
            IUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            UriCustomTypeParsers.AddCustomUriTypeParser(customBooleanUriTypePraser);

            bool isRemoved = UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser);
            isRemoved.Should().BeTrue();

            this.NoParsesForNonConvetionalBooleanValue();
        }

        [Fact]
        public void RemoveCustomUriTypeParser_CanRemoveParserWhichIsRegisteredToEdmType()
        {
            IUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

            bool isRemoved = UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser);
            isRemoved.Should().BeTrue();

            this.NoParsesForNonConvetionalBooleanValue();
        }

        /// <summary>
        /// Should remove both from registered and general parsers.
        /// </summary>
        [Fact]
        public void RemoveCustomUriTypeParser_CanRemoveSameInstanceOfParserAddedAsGeneralAndRegistedWithEdmType()
        {
            IUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            UriCustomTypeParsers.AddCustomUriTypeParser(customBooleanUriTypePraser);
            UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

            bool isRemoved = UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser);
            isRemoved.Should().BeTrue();

            this.NoParsesForNonConvetionalBooleanValue();
        }

        /// <summary>
        /// Should remove both from registered and general parsers.
        /// </summary>
        [Fact]
        public void RemoveCustomUriTypeParser_CanRemoveSameInstanceOfParserAddedAsGeneralAndMultipleRegistedWithEdmType()
        {
            IUriTypeParser customIntBooleanUriTypePraser = new MyCustomIntAndBooleanUriTypeParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
            IEdmTypeReference intTypeReference = EdmCoreModel.Instance.GetInt32(false);

            UriCustomTypeParsers.AddCustomUriTypeParser(customIntBooleanUriTypePraser);
            UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customIntBooleanUriTypePraser);
            UriCustomTypeParsers.AddCustomUriTypeParser(intTypeReference, customIntBooleanUriTypePraser);

            bool isRemoved = UriCustomTypeParsers.RemoveCustomUriTypeParser(customIntBooleanUriTypePraser);
            isRemoved.Should().BeTrue();

            this.NoParsesForNonConvetionalBooleanValue();
            this.NoParsesForNonConvetionalIntValue();
        }
 
	    #endregion       
        
        #region ParseUriStringToType - Method

        // ** Test 'ParseUriStringToType' Method **
        // General Parsers - Can Parse - Valid Value
        // General Parsers - Can Parse - Invalid Value
        // General Parsers - Cannot Parse
        // Registed EdmType Parsers - Can Parse - Valid Value
        // Registed EdmType Parsers - Can Parse - Invalid Value
        // Registed EdmType Parsers - Cannot Parse
        // No Parsers added
        // Two parsers - one general and one for Edm. Both can parse. Who is first?

        [Fact]
        public void ParseUriStringToType_GeneralParsers_ValidValue_ParsingSucceeded()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            try
            {
                // Add to general parsers
                UriCustomTypeParsers.AddCustomUriTypeParser(customBooleanUriTypePraser);
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

                UriTypeParsingException exception;
                object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE, EdmCoreModel.Instance.GetBoolean(false), out exception);

                // Assert
                exception.Should().BeNull();
                output.ShouldBeEquivalentTo(true);
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void ParseUriStringToType_GeneralParsers_InvalidValue_ParsingHasFailed()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            try
            {
                // Add to general parsers
                UriCustomTypeParsers.AddCustomUriTypeParser(customBooleanUriTypePraser);
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

                UriTypeParsingException exception;
                object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_INVALID_VALUE, EdmCoreModel.Instance.GetBoolean(false), out exception);

                // Assert
                output.Should().BeNull();

                Action action = () =>
                {
                    throw exception;
                };

                action.ShouldThrow<UriTypeParsingException>();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void ParseUriStringToType_GeneralParsers_ParserCannotParse_ParsingHasFailed()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            try
            {
                UriCustomTypeParsers.AddCustomUriTypeParser(customBooleanUriTypePraser);

                UriTypeParsingException exception;
                object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_INVALID_VALUE, EdmCoreModel.Instance.GetBoolean(false), out exception);

                // Assert
                output.Should().BeNull();

                Action action = () =>
                {
                    throw exception;
                };

                action.ShouldThrow<UriTypeParsingException>();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void ParseUriStringToType_RegisterToEdmType_ValidValue_ParsingSucceeded()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

                UriTypeParsingException exception;
                object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE, booleanTypeReference, out exception);

                // Assert
                exception.Should().BeNull();
                output.ShouldBeEquivalentTo(true);
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void ParseUriStringToType_RegisterToEdmType_InvalidValue_ParsingHasFailed()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

                UriTypeParsingException exception;
                object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_INVALID_VALUE, booleanTypeReference, out exception);

                // Assert
                output.Should().BeNull();

                Action action = () =>
                {
                    throw exception;
                };

                action.ShouldThrow<UriTypeParsingException>();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void ParseUriStringToType_RegisterToEdmType_ParserCannotParse_ParsingHasFailed()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

                UriTypeParsingException exception;
                object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_INVALID_VALUE, booleanTypeReference, out exception);

                // Assert
                output.Should().BeNull();

                Action action = () =>
                {
                    throw exception;
                };

                action.ShouldThrow<UriTypeParsingException>();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void ParseUriStringToType_RegisterToEdmType_ParserIsRegisteredToDifferentEdmType()
        {
            MyCustomBooleanUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);

                UriTypeParsingException exception;
                object output =
                    UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_INVALID_VALUE, EdmCoreModel.Instance.GetString(true), out exception);

                // Assert
                output.Should().BeNull();
                exception.Should().BeNull();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
            }
        }

        [Fact]
        public void ParseUriStringToType_TryParseWithoutParsersAdded()
        {
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            UriTypeParsingException exception;
            object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE, booleanTypeReference, out exception);

            // Assert
            output.Should().BeNull();
            exception.Should().BeNull();
        }

        [Fact]
        public void ParseUriStringToType_ParseFirstWithRegisteredEdmType()
        {
            IUriTypeParser customBooleanUriTypePraser = new MyCustomBooleanUriTypeParser();
            IUriTypeParser customIntBooleanUriTypePraser = new MyCustomIntAndBooleanUriTypeParser();

            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                UriCustomTypeParsers.AddCustomUriTypeParser(booleanTypeReference, customBooleanUriTypePraser);
                UriCustomTypeParsers.AddCustomUriTypeParser(customIntBooleanUriTypePraser);

                // The boolean type parse will parse the value to 'True'
                // The int boolean type parse will throw exception.
                // If result is 'True' it means the registered parser is used first.
                // If result is exception it means the general parse is used first
                UriTypeParsingException exception;
                object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALUE_TRUE_VALID, booleanTypeReference, out exception);

                // Assert
                output.Should().Be(true);
                exception.Should().BeNull();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customBooleanUriTypePraser).Should().BeTrue();
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customIntBooleanUriTypePraser).Should().BeTrue();
            }
        }
        
        #endregion

        #region ODataUriParser

        [Fact]
        public void ParseWithCustomUriFunction_CanParseByGeneralCustomParser()
        {
            IUriTypeParser customStringTypeParser = new MyCustomStringUriTypeParser();
            try
            {
                UriCustomTypeParsers.AddCustomUriTypeParser(customStringTypeParser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq '{0}'",CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                    .And.Right.ShouldBeConstantQueryNode(CUSTOM_PARSER_STRING_VALID_VALUE + CUSTOM_PARSER_STRING_ADDED_VALUE);
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customStringTypeParser).Should().BeTrue();
            }
        }

        [Fact]
        public void ParseWithCustomUriFunction_CanParseByCustomParserRegisterdToEdmTpe()
        {
            IUriTypeParser customStringTypeParser = new MyCustomStringUriTypeParser();
            try
            {
                UriCustomTypeParsers.AddCustomUriTypeParser(EdmCoreModel.Instance.GetString(true), customStringTypeParser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq '{0}'",CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                    .And.Right.ShouldBeConstantQueryNode(CUSTOM_PARSER_STRING_VALID_VALUE + CUSTOM_PARSER_STRING_ADDED_VALUE);
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customStringTypeParser).Should().BeTrue();
            }
        }

        [Fact]
        public void ParseWithCustomUriFunction_CustomGeneralParserThrowsException()
        {
            IUriTypeParser customStringTypeParser = new MyCustomStringUriTypeParser();
            try
            {
                UriCustomTypeParsers.AddCustomUriTypeParser(customStringTypeParser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq '{0}'", CUSTOM_PARSER_STRING_VALUE_CAUSEBUG));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parseUriAction =()=>
                    parser.ParseFilter();

                parseUriAction.ShouldThrow<ODataException>().
                    WithInnerException<UriTypeParsingException>();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customStringTypeParser).Should().BeTrue();
            }
        }

        [Fact]
        public void ParseWithCustomUriFunction_CustomParserRegisteredToEdmTypeThrowsException()
        {
            IUriTypeParser customStringTypeParser = new MyCustomStringUriTypeParser();
            try
            {
                UriCustomTypeParsers.AddCustomUriTypeParser(EdmCoreModel.Instance.GetString(true), customStringTypeParser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq '{0}'", CUSTOM_PARSER_STRING_VALUE_CAUSEBUG));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parseUriAction = () =>
                    parser.ParseFilter();

                parseUriAction.ShouldThrow<ODataException>().
                    WithInnerException<UriTypeParsingException>();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customStringTypeParser).Should().BeTrue();
            }
        }

        /// <summary>
        /// Similates a stituation which the client implements a buggy UriTypeParser.
        /// It throw an exception and returns a value which is not null.
        /// </summary>
        [Fact]
        public void ParseWithCustomUriFunction_CustomParserThrowsExceptionAndReturnNotNullValue()
        {
            IUriTypeParser customStringTypeParser = new MyCustomStringUriTypeParser();
            try
            {
                UriCustomTypeParsers.AddCustomUriTypeParser(EdmCoreModel.Instance.GetString(true), customStringTypeParser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq '{0}'", CUSTOM_PARSER_STRING_VALUE_CAUSEBUG));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parseUriAction = () =>
                    parser.ParseFilter();

                parseUriAction.ShouldThrow<ODataException>().
                    WithInnerException<UriTypeParsingException>();
            }
            finally
            {
                UriCustomTypeParsers.RemoveCustomUriTypeParser(customStringTypeParser).Should().BeTrue();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Try to convert a valid value that the Custom boolean type parser can parse.
        /// If the parser has been added successfully to the UriCustmTypeParser, this method will passed successfully.
        /// </summary>
        private void ParseNonConvetionalBooleanValueSuccessfully()
        {
            // Consider to add  -
            // PrivateType, check if the custom type parser has been insterted to Parsers lists

            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            UriTypeParsingException exception;
            object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE, booleanTypeReference, out exception);

            // Assert
            exception.Should().BeNull();
            output.ShouldBeEquivalentTo(true);
        }

        /// <summary>
        /// Try to convert a valid value that the Custom int type parser can parse.
        /// If the parser has been added successfully to the UriCustmTypeParser, this method will passed successfully.
        /// </summary>
        private void ParseNonConvetionalIntValueSuccessfully()
        {
            // Consider to add  -
            // PrivateType, check if the custom type parser has been insterted to Parsers lists


            IEdmTypeReference Int32TypeReference = EdmCoreModel.Instance.GetInt32(false);

            UriTypeParsingException exception;
            object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_INT_VALID_VALUE, Int32TypeReference, out exception);

            // Assert
            exception.Should().BeNull();
            output.ShouldBeEquivalentTo(55);
        }

        /// <summary>
        /// Try to convert a valid value that the Custom boolean type parser can parse.
        /// If the parser has NOT been added to the UriCustmTypeParser, this method will passed successfully.
        /// </summary>
        private void NoParsesForNonConvetionalBooleanValue()
        {
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            UriTypeParsingException exception;
            object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE, booleanTypeReference, out exception);

            // Assert
            exception.Should().BeNull();
            output.Should().BeNull();
        }

        /// <summary>
        /// Try to convert a valid value that the Custom Int32 type parser can parse.
        /// If the parser has NOT been added to the UriCustmTypeParser, this method will passed successfully.
        /// </summary>
        private void NoParsesForNonConvetionalIntValue()
        {
            IEdmTypeReference Int32TypeReference = EdmCoreModel.Instance.GetInt32(false);

            UriTypeParsingException exception;
            object output = UriCustomTypeParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_INT_VALID_VALUE, Int32TypeReference, out exception);

            // Assert
            exception.Should().BeNull();
            output.Should().BeNull();
        }

        #endregion

        #region Custom Type Parsers

        internal class MyCustomBooleanUriTypeParser : IUriTypeParser
        {
            public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriTypeParsingException parsingException)
            {
                parsingException = null;

                if (!targetType.IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(false)))
                {
                    return null;
                }

                if (text == CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE)
                {
                    return true;
                }
                else if (text == CUSTOM_PARSER_BOOLEAN_VALUE_TRUE_VALID)
                {
                    return true;
                }
                else if (text == CUSTOM_PARSER_BOOLEAN_INVALID_VALUE)
                {
                    parsingException = new UriTypeParsingException("Failed to convert boolean.", "Value must be bla bla bla");
                    return null;
                }

                return null;
            }
        }

        internal class MyCustomIntAndBooleanUriTypeParser : IUriTypeParser
        {
            public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriTypeParsingException parsingException)
            {
                if (targetType.IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(false)))
                {
                    return this.ParseUriStringToBoolean(text, out parsingException);
                }

                if (targetType.IsEquivalentTo(EdmCoreModel.Instance.GetInt32(false)))
                {
                    return this.ParseUriStringToInt(text, out parsingException);
                }

                parsingException = null;
                return null;
            }

            private object ParseUriStringToInt(string text, out UriTypeParsingException parsingException)
            {
                parsingException = null;

                if (text == CUSTOM_PARSER_INT_VALID_VALUE)
                {
                    return 55;
                }

                return null;
            }

            public object ParseUriStringToBoolean(string text, out UriTypeParsingException parsingException)
            {
                parsingException = null;

                if (text == CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE)
                {
                    return true;
                }
                else if (text == CUSTOM_PARSER_BOOLEAN_VALUE_TRUE_VALID)
                {
                    parsingException = new UriTypeParsingException("Failed to convert boolean.", "Value must be bla bla bla");
                }
                else if (text == CUSTOM_PARSER_BOOLEAN_INVALID_VALUE)
                {
                    parsingException = new UriTypeParsingException("Failed to convert boolean.", "Value must be bla bla bla");
                    return null;
                }

                return null;
            }
        }

        internal class MyCustomStringUriTypeParser : IUriTypeParser
        {
            public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriTypeParsingException parsingException)
            {
                parsingException = null;

                if (!targetType.IsEquivalentTo(EdmCoreModel.Instance.GetString(true)))
                {
                    return null;
                }
                if (text == null)
                {
                    return null;
                }

                bool isLiteralPrefixExists = false;

                // Take care of literals
                if (text.StartsWith("myCustomStringTypePrefixLiteral"))
                {
                    text = text.Replace("myCustomStringTypePrefixLiteral", string.Empty);
                    
                    // If Literal exists, not need of quotes
                    isLiteralPrefixExists = true;
                }

                if (!isLiteralPrefixExists && !UriParserHelper.IsUriValueQuoted(text))
                {
                    parsingException = new UriTypeParsingException("Edm.String value must be quoted");
                    return null;
                }

                text = UriParserHelper.RemoveQuotes(text);

                // Simulates a bug in a client Parser
                if (text == CUSTOM_PARSER_STRING_VALUE_CAUSEBUG)
                {
                    parsingException = new UriTypeParsingException("Parsing to Edm.String has failed for some reasons");
                    return "RetunsAValueButSupposeToBeNull";
                }

                if (text.StartsWith(CUSTOM_PARSER_STRING_VALID_VALUE))
                {
                    return text + CUSTOM_PARSER_STRING_ADDED_VALUE;
                }

                return null;
            }
        }

        internal class HeatBeatCustomUriTypeParser : IUriTypeParser
        {
            public static EdmComplexTypeReference HeartbeatComplexType;

            static HeatBeatCustomUriTypeParser()
            {
                HeartbeatComplexType = new EdmComplexTypeReference(HardCodedTestModel.GetHeatbeatComplexType(),false);
            }

            public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriTypeParsingException parsingException)
            {
                parsingException = null;

                if (!targetType.IsEquivalentTo(HeartbeatComplexType))
                {
                    return null;
                }
                if (text == null)
                {
                    return null;
                }

                // Take care of literals
                if (!text.StartsWith("myCustomHeartbeatTypePrefixLiteral"))
                {
                    return null;
                }

                text = text.Replace("myCustomHeartbeatTypePrefixLiteral", string.Empty);

                if (!UriParserHelper.IsUriValueQuoted(text))
                {
                    parsingException = new UriTypeParsingException("Edm.Heartbeat value must be quoted");
                    return null;
                }

                text = UriParserHelper.RemoveQuotes(text);

                double textIntValue;
                // Simulates a bug in a client Parser
                if (double.TryParse(text, out textIntValue))
                {
                    return new HeatBeat() { Frequency = textIntValue };
                }

                return null;
            }

            internal class HeatBeat
            {
                public double Frequency { get; set; }
            }
        }

        #endregion
    }
}
