//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralParserUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    /// <summary>
    /// Test the public API of CustomUriLiteralParser class
    /// </summary>
    public class CustomUriLiteralParserUnitTests
    {
        #region Consts

        public const string CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE = "NonConvetionalBooleanTrue";

        public const string CUSTOM_PARSER_BOOLEAN_INVALID_VALUE = "InvalidValueOfNonConvertionalBoolean";

        public const string CUSTOM_PARSER_BOOLEAN_VALUE_TRUE_VALID = "ValidValueOfNonConvertionalBooleanTrueForRegisteredFalseForGeneral";

        public const string CUSTOM_PARSER_INT_VALID_VALUE = "NonConvetionalInt55";

        public const string CUSTOM_PARSER_STRING_VALID_VALUE = "BlaBla";

        public const string CUSTOM_PARSER_STRING_ADDED_VALUE = "MyCoolStringParserAddedValue";

        public const string CUSTOM_PARSER_STRING_VALUE_CAUSEBUG = "StringValueWithBug";

        public const string BOOLEAN_LITERAL_PREFIX = "myCustomBooleanTypePrefixLiteral";

        public const string STRING_LITERAL_PREFIX = "myCustomStringTypePrefixLiteral";

        #endregion

        #region AddCustomUriLiteralParser Method

        [Fact]
        public void AddCustomUriLiteralParser_CannotAddNullLiteralParser()
        {
            Action addNullCustomUriLiteralParser = () =>
                CustomUriLiteralParsers.AddCustomUriLiteralParser(null);

            Assert.Throws<ArgumentNullException>("customUriLiteralParser", addNullCustomUriLiteralParser);
        }

        [Fact]
        public void AddCustomUriLiteralParser_CannotAddNullLiteralParserAndNullEdmTypeReference()
        {
            Action addNullCustomUriLiteralParserAndNullParser = () =>
                CustomUriLiteralParsers.AddCustomUriLiteralParser(null, null);

            Assert.Throws<ArgumentNullException>("edmTypeReference", addNullCustomUriLiteralParserAndNullParser);
        }

        [Fact]
        public void AddCustomUriLiteralParser_CannotAddNullLiteralParserWithEdmType()
        {
            IEdmTypeReference typeReference = EdmCoreModel.Instance.GetBoolean(false);

            Action addNullCustomUriLiteralParser = () =>
                CustomUriLiteralParsers.AddCustomUriLiteralParser(typeReference, null);

            Assert.Throws<ArgumentNullException>("customUriLiteralParser", addNullCustomUriLiteralParser);
        }

        [Fact]
        public void AddCustomUriLiteralParser_CannotAddNullEdmTypeReferenceWithLiteralParser()
        {
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser =
                new MyCustomBooleanUriLiteralParser();

            Action addCustomUriLiteralParserWithNullEdmType = () =>
                CustomUriLiteralParsers.AddCustomUriLiteralParser(null, customBooleanUriTypePraser);

            Assert.Throws<ArgumentNullException>("edmTypeReference", addCustomUriLiteralParserWithNullEdmType);
        }

        [Fact]
        public void AddCustomUriLiteralParser_GeneralParsers_CanAdd()
        {
            RegisterTestCase("AddCustomUriLiteralParser_GeneralParsers_CanAdd");
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();

            try
            {
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
            }
            finally
            {
                // Clean up from cache
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void AddCustomUriLiteralParser_GeneralParsers_CanAddMultipleDifferentInstances()
        {
            RegisterTestCase("AddCustomUriLiteralParser_GeneralParsers_CanAddMultipleDifferentInstances");
            IUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            IUriLiteralParser customIntBooleanUriTypePraser = new MyCustomIntAndBooleanUriLiteralParser();

            try
            {
                // Add two different instances to GeneralLiteralParsers
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanUriTypePraser);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customIntBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
                this.ParseNonConvetionalIntValueSuccessfully();
            }
            finally
            {
                // Clean up from cache
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customIntBooleanUriTypePraser));
            }
        }

        [Fact]
        public void AddCustomUriLiteralParser_GeneralParsers_CannotAddToGeneraldIfSameInstanceAlreadyExists()
        {
            RegisterTestCase("AddCustomUriLiteralParser_GeneralParsers_CannotAddToGeneraldIfSameInstanceAlreadyExists");
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();

            try
            {
                // Add Once
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanUriTypePraser);

                // Add again - Should throw exception
                Action addCustomUriLiteralParser = () =>
                    CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanUriTypePraser);

                addCustomUriLiteralParser.Throws<ODataException>(Strings.UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void AddCustomUriLiteralParser_RegisterToEdmType_CanAddCustomParser()
        {
            RegisterTestCase("AddCustomUriLiteralParser_RegisterToEdmType_CanAddCustomParser");
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            try
            {
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void AddCustomUriLiteralParser_RegisterToEdmType_CanAddMultipleDifferentInstances()
        {
            RegisterTestCase("AddCustomUriLiteralParser_RegisterToEdmType_CanAddMultipleDifferentInstances");
            IUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            IUriLiteralParser customIntBooleanUriTypePraser = new MyCustomIntAndBooleanUriLiteralParser();
            IEdmTypeReference intEdmTypeReference = EdmCoreModel.Instance.GetInt32(false);

            try
            {
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(intEdmTypeReference, customIntBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
                this.ParseNonConvetionalIntValueSuccessfully();
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customIntBooleanUriTypePraser));
            }
        }

        [Fact]
        public void AddCustomUriLiteralParser_RegisterToEdmType_CannotAddIfAlreadyRegistedToTheSameEdmType()
        {
            RegisterTestCase("AddCustomUriLiteralParser_RegisterToEdmType_CannotAddIfAlreadyRegistedToTheSameEdmType");
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
            try
            {
                // Add once
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

                // Add again - Should throw exception
                Action addCustomUriLiteralParser = () =>
                    CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

                addCustomUriLiteralParser.Throws<ODataException>(Strings.UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists(booleanTypeReference.FullName()));
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void AddCustomUriLiteralParser_RegisterToEdmType_CanAddIfSameParserInstanceExistsButRegisteredToDifferentEdmType()
        {
            RegisterTestCase("AddCustomUriLiteralParser_RegisterToEdmType_CanAddIfSameParserInstanceExistsButRegisteredToDifferentEdmType");
            IUriLiteralParser customIntAndBooleanUriTypePraser = new MyCustomIntAndBooleanUriLiteralParser();

            try
            {
                // Add once
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customIntAndBooleanUriTypePraser);

                // Add same type converter but registered to a different EdmType(string instead of boolean)
                IEdmTypeReference intTypeReference = EdmCoreModel.Instance.GetInt32(false);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(intTypeReference, customIntAndBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
                this.ParseNonConvetionalIntValueSuccessfully();
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customIntAndBooleanUriTypePraser));
            }
        }

        [Fact]
        public void AddCustomUriLiteralParser_RegisterToEdmType_CanAddIfSameParserInstanceExistsAsGeneralLiteralParser()
        {
            RegisterTestCase("AddCustomUriLiteralParser_RegisterToEdmType_CanAddIfSameParserInstanceExistsAsGeneralLiteralParser");
            IUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();

            try
            {
                // Add once as general LiteralParser (with no specific EdmType)
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanUriTypePraser);

                // Add again with registered EdmType
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void AddCustomUriLiteralParser_GeneralParsers_CanAddIfSameParserInstanceAlreadyRegisteredToEdmType()
        {
            RegisterTestCase("AddCustomUriLiteralParser_GeneralParsers_CanAddIfSameParserInstanceAlreadyRegisteredToEdmType");
            IUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();

            try
            {
                // Add once with registered EdmType
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

                // Add again as general LiteralParser (with no specific EdmType)
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void AddCustomUriLiteralParser_CanAddMultipleDifferentInstancesToRegisteredAndGeneral()
        {
            RegisterTestCase("AddCustomUriLiteralParser_CanAddMultipleDifferentInstancesToRegisteredAndGeneral");

            IUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            IUriLiteralParser customIntBooleanUriTypePraser = new MyCustomIntAndBooleanUriLiteralParser();

            try
            {
                // Add to registered edm types
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

                // Add to general parsers
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customIntBooleanUriTypePraser);

                // Test of custom parser is working
                this.ParseNonConvetionalBooleanValueSuccessfully();
                this.ParseNonConvetionalIntValueSuccessfully();
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customIntBooleanUriTypePraser));
            }
        }

        #endregion

        #region RemoveCustomUriLiteralParser Method

        // ** Test 'RemoveCustomUriLiteralParser' Method **

        // Remove Validation
        // Cannot remove parser which is not added
        // Can remove in general parser
        // Can remove in registered Edm
        // Can remove if exists in both
        // Can remove if exists in general and 2 in registred

        [Fact]
        public void RemoveCustomUriLiteralParser_CannotRemoveNull()
        {
            Action removeNullCustomUriLiteralParser = () =>
                CustomUriLiteralParsers.RemoveCustomUriLiteralParser(null);

            Assert.Throws<ArgumentNullException>("customUriLiteralParser", removeNullCustomUriLiteralParser);
        }

        [Fact]
        public void RemoveCustomUriLiteralParser_CannotRemoveNotExistingParser()
        {
            IUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            bool isRemoveSucceeded = CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser);

            // Assert
            Assert.False(isRemoveSucceeded);
        }

        [Fact]
        public void RemoveCustomUriLiteralParser_CanRemoveGeneralParser()
        {
            RegisterTestCase("RemoveCustomUriLiteralParser_CanRemoveGeneralParser");
            IUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanUriTypePraser);

            bool isRemoved = CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser);
            Assert.True(isRemoved);

            this.NoParsesForNonConvetionalBooleanValue();
        }

        [Fact]
        public void RemoveCustomUriLiteralParser_CanRemoveParserWhichIsRegisteredToEdmType()
        {
            RegisterTestCase("RemoveCustomUriLiteralParser_CanRemoveParserWhichIsRegisteredToEdmType");
            IUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

            bool isRemoved = CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser);
            Assert.True(isRemoved);

            this.NoParsesForNonConvetionalBooleanValue();
        }

        /// <summary>
        /// Should remove both from registered and general parsers.
        /// </summary>
        [Fact]
        public void RemoveCustomUriLiteralParser_CanRemoveSameInstanceOfParserAddedAsGeneralAndRegistedWithEdmType()
        {
            RegisterTestCase("RemoveCustomUriLiteralParser_CanRemoveSameInstanceOfParserAddedAsGeneralAndRegistedWithEdmType");
            IUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanUriTypePraser);
            CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

            bool isRemoved = CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser);
            Assert.True(isRemoved);

            this.NoParsesForNonConvetionalBooleanValue();
        }

        /// <summary>
        /// Should remove both from registered and general parsers.
        /// </summary>
        [Fact]
        public void RemoveCustomUriLiteralParser_CanRemoveSameInstanceOfParserAddedAsGeneralAndMultipleRegistedWithEdmType()
        {
            RegisterTestCase("RemoveCustomUriLiteralParser_CanRemoveSameInstanceOfParserAddedAsGeneralAndMultipleRegistedWithEdmType");
            IUriLiteralParser customIntBooleanUriTypePraser = new MyCustomIntAndBooleanUriLiteralParser();
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
            IEdmTypeReference intTypeReference = EdmCoreModel.Instance.GetInt32(false);

            CustomUriLiteralParsers.AddCustomUriLiteralParser(customIntBooleanUriTypePraser);
            CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customIntBooleanUriTypePraser);
            CustomUriLiteralParsers.AddCustomUriLiteralParser(intTypeReference, customIntBooleanUriTypePraser);

            bool isRemoved = CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customIntBooleanUriTypePraser);
            Assert.True(isRemoved);

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
            RegisterTestCase("ParseUriStringToType_GeneralParsers_ValidValue_ParsingSucceeded");
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            try
            {
                // Add to general parsers
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanUriTypePraser);
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

                UriLiteralParsingException exception;
                object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE, EdmCoreModel.Instance.GetBoolean(false), out exception);

                // Assert
                Assert.Null(exception);
                Assert.Equal(true, output);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void ParseUriStringToType_GeneralParsers_InvalidValue_ParsingHasFailed()
        {
            RegisterTestCase("ParseUriStringToType_GeneralParsers_InvalidValue_ParsingHasFailed");
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            try
            {
                // Add to general parsers
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanUriTypePraser);
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

                UriLiteralParsingException exception;
                object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_INVALID_VALUE, EdmCoreModel.Instance.GetBoolean(false), out exception);

                // Assert
                Assert.Null(output);

                Action action = () =>
                {
                    throw exception;
                };

                Assert.Throws<UriLiteralParsingException>(action);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void ParseUriStringToType_GeneralParsers_ParserCannotParse_ParsingHasFailed()
        {
            RegisterTestCase("ParseUriStringToType_GeneralParsers_ParserCannotParse_ParsingHasFailed");
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            try
            {
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanUriTypePraser);

                UriLiteralParsingException exception;
                object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_INVALID_VALUE, EdmCoreModel.Instance.GetBoolean(false), out exception);

                // Assert
                Assert.Null(output);

                Action action = () =>
                {
                    throw exception;
                };

                Assert.Throws<UriLiteralParsingException>(action);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void ParseUriStringToType_RegisterToEdmType_ValidValue_ParsingSucceeded()
        {
            RegisterTestCase("ParseUriStringToType_RegisterToEdmType_ValidValue_ParsingSucceeded");
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

                UriLiteralParsingException exception;
                object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE, booleanTypeReference, out exception);

                // Assert
                Assert.Null(exception);
                Assert.Equal(true, output);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void ParseUriStringToType_RegisterToEdmType_InvalidValue_ParsingHasFailed()
        {
            RegisterTestCase("ParseUriStringToType_RegisterToEdmType_InvalidValue_ParsingHasFailed");
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

                UriLiteralParsingException exception;
                object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_INVALID_VALUE, booleanTypeReference, out exception);

                // Assert
                Assert.Null(output);

                Action action = () =>
                {
                    throw exception;
                };

                Assert.Throws<UriLiteralParsingException>(action);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void ParseUriStringToType_RegisterToEdmType_ParserCannotParse_ParsingHasFailed()
        {
            RegisterTestCase("ParseUriStringToType_RegisterToEdmType_ParserCannotParse_ParsingHasFailed");
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

                UriLiteralParsingException exception;
                object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_INVALID_VALUE, booleanTypeReference, out exception);

                // Assert
                Assert.Null(output);

                Action action = () =>
                {
                    throw exception;
                };

                Assert.Throws<UriLiteralParsingException>(action);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void ParseUriStringToType_RegisterToEdmType_ParserIsRegisteredToDifferentEdmType()
        {
            MyCustomBooleanUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);

                UriLiteralParsingException exception;
                object output =
                    CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_INVALID_VALUE, EdmCoreModel.Instance.GetString(true), out exception);

                // Assert
                Assert.Null(output);
                Assert.Null(exception);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
            }
        }

        [Fact]
        public void ParseUriStringToType_TryParseWithoutParsersAdded()
        {
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            UriLiteralParsingException exception;
            object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE, booleanTypeReference, out exception);

            // Assert
            Assert.Null(output);
            Assert.Null(exception);
        }

        [Fact]
        public void ParseUriStringToType_ParseFirstWithRegisteredEdmType()
        {
            RegisterTestCase("ParseUriStringToType_ParseFirstWithRegisteredEdmType");
            IUriLiteralParser customBooleanUriTypePraser = new MyCustomBooleanUriLiteralParser();
            IUriLiteralParser customIntBooleanUriTypePraser = new MyCustomIntAndBooleanUriLiteralParser();

            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriTypePraser);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customIntBooleanUriTypePraser);

                // The boolean type parse will parse the value to 'True'
                // The int boolean type parse will throw exception.
                // If result is 'True' it means the registered parser is used first.
                // If result is exception it means the general parse is used first
                UriLiteralParsingException exception;
                object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALUE_TRUE_VALID, booleanTypeReference, out exception);

                // Assert
                Assert.Equal(true, output);
                Assert.Null(exception);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriTypePraser));
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customIntBooleanUriTypePraser));
            }
        }

        #endregion

        #region ODataUriParser

        [Fact]
        public void ParseWithCustomUriFunction_CanParseByGeneralCustomParser()
        {
            RegisterTestCase("ParseWithCustomUriFunction_CanParseByGeneralCustomParser");
            IUriLiteralParser customStringLiteralParser = new MyCustomStringUriLiteralParser();
            try
            {
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customStringLiteralParser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq '{0}'", CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                    .Right.ShouldBeConstantQueryNode(CUSTOM_PARSER_STRING_VALID_VALUE + CUSTOM_PARSER_STRING_ADDED_VALUE);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customStringLiteralParser));
            }
        }

        [Fact]
        public void ParseWithCustomUriFunction_CanParseByCustomParserRegisterdToEdmTpe()
        {
            RegisterTestCase("ParseWithCustomUriFunction_CanParseByCustomParserRegisterdToEdmTpe");
            IUriLiteralParser customStringLiteralParser = new MyCustomStringUriLiteralParser();
            try
            {
                CustomUriLiteralParsers.AddCustomUriLiteralParser(EdmCoreModel.Instance.GetString(true), customStringLiteralParser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq '{0}'", CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                    .Right.ShouldBeConstantQueryNode(CUSTOM_PARSER_STRING_VALID_VALUE + CUSTOM_PARSER_STRING_ADDED_VALUE);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customStringLiteralParser));
            }
        }

        [Fact]
        public void ParseWithCustomUriFunction_CustomGeneralParserThrowsException()
        {
            RegisterTestCase("ParseWithCustomUriFunction_CustomGeneralParserThrowsException");
            IUriLiteralParser customStringLiteralParser = new MyCustomStringUriLiteralParser();
            try
            {
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customStringLiteralParser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq '{0}'", CUSTOM_PARSER_STRING_VALUE_CAUSEBUG));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parseUriAction = () =>
                    parser.ParseFilter();

                ODataException exception = Assert.Throws<ODataException>(parseUriAction);
                Assert.IsType<UriLiteralParsingException>(exception.InnerException);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customStringLiteralParser));
            }
        }

        [Fact]
        public void ParseWithCustomUriFunction_CustomParserRegisteredToEdmTypeThrowsException()
        {
            RegisterTestCase("ParseWithCustomUriFunction_CustomParserRegisteredToEdmTypeThrowsException");
            IUriLiteralParser customStringLiteralParser = new MyCustomStringUriLiteralParser();
            try
            {
                CustomUriLiteralParsers.AddCustomUriLiteralParser(EdmCoreModel.Instance.GetString(true), customStringLiteralParser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq '{0}'", CUSTOM_PARSER_STRING_VALUE_CAUSEBUG));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parseUriAction = () =>
                    parser.ParseFilter();

                ODataException exception = Assert.Throws<ODataException>(parseUriAction);
                Assert.IsType<UriLiteralParsingException>(exception.InnerException);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customStringLiteralParser));
            }
        }

        /// <summary>
        /// Similates a stituation which the client implements a buggy UriLiteralParser.
        /// It throw an exception and returns a value which is not null.
        /// </summary>
        [Fact]
        public void ParseWithCustomUriFunction_CustomParserThrowsExceptionAndReturnNotNullValue()
        {
            RegisterTestCase("ParseWithCustomUriFunction_CustomParserThrowsExceptionAndReturnNotNullValue");
            IUriLiteralParser customStringLiteralParser = new MyCustomStringUriLiteralParser();
            try
            {
                CustomUriLiteralParsers.AddCustomUriLiteralParser(EdmCoreModel.Instance.GetString(true), customStringLiteralParser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq '{0}'", CUSTOM_PARSER_STRING_VALUE_CAUSEBUG));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                Action parseUriAction = () =>
                    parser.ParseFilter();

                ODataException exception = Assert.Throws<ODataException>(parseUriAction);
                Assert.IsType<UriLiteralParsingException>(exception.InnerException);
            }
            finally
            {
                Assert.True(CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customStringLiteralParser));
            }
        }


        /// <summary>
        /// In fact, this test shows the real power of the customization of UriLiteralParser and LiteralPrefix.
        /// Here we can see a special parsing for an EdmType 'Heartbeat' which is a complex type.
        /// The 'Heartbeat' type has it's own LiteralPrefix.
        /// The result is an instacne object of 'Heartbeat' class. 
        /// </summary>
        [Fact]
        public void CustomUriLiteralPrefix_CanSetCustomLiteralWithCustomLiteralParserCustomType()
        {
            RegisterTestCase("CustomUriLiteralPrefix_CanSetCustomLiteralWithCustomLiteralParserCustomType");
            const string HEARTBEAT_LITERAL_PREFIX = "myCustomHeartbeatTypePrefixLiteral";
            IUriLiteralParser customHeartbeatUriTypePraser = new HeatBeatCustomUriLiteralParser();
            IEdmTypeReference heartbeatTypeReference = HeatBeatCustomUriLiteralParser.HeartbeatComplexType;

            try
            {
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(HEARTBEAT_LITERAL_PREFIX, heartbeatTypeReference);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(heartbeatTypeReference, customHeartbeatUriTypePraser);

                var fullUri = new Uri("http://www.odata.com/OData/Lions" + string.Format("?$filter=LionHeartbeat eq {0}'55.9'", HEARTBEAT_LITERAL_PREFIX));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                HeatBeatCustomUriLiteralParser.HeatBeat heartbeatValue = Assert.IsType<HeatBeatCustomUriLiteralParser.HeatBeat>(
                  (parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConvertQueryNode(heartbeatTypeReference).Source as ConstantNode).Value);

                Assert.NotNull(heartbeatValue);
                Assert.Equal(55.9, heartbeatValue.Frequency);
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(HEARTBEAT_LITERAL_PREFIX);
                CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customHeartbeatUriTypePraser);
            }
        }

        [Fact]
        public void CustomUriLiteralPrefix_CanSetCustomLiteralWithCustomLiteralParser()
        {
            RegisterTestCase("CustomUriLiteralPrefix_CanSetCustomLiteralWithCustomLiteralParser");
            IUriLiteralParser customstringUriTypePraser = new MyCustomStringUriLiteralParser();
            IEdmTypeReference stringTypeReference = EdmCoreModel.Instance.GetString(true);

            try
            {
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(STRING_LITERAL_PREFIX, stringTypeReference);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(stringTypeReference, customstringUriTypePraser);

                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq {0}'{1}'", STRING_LITERAL_PREFIX, CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                    .Right.ShouldBeConstantQueryNode(CUSTOM_PARSER_STRING_VALID_VALUE + CUSTOM_PARSER_STRING_ADDED_VALUE);
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(STRING_LITERAL_PREFIX);
                CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customstringUriTypePraser);
            }
        }


        [Fact]
        public void CustomUriLiteralPrefix_ParseTypeWithCorrectLiteralPrefixAndUriParser()
        {
            RegisterTestCase("CustomUriLiteralPrefix_ParseTypeWithCorrectLiteralPrefixAndUriParser");
            var customBooleanAndIntUriLiteralParser = new MyCustomIntAndBooleanUriLiteralParser();
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(BOOLEAN_LITERAL_PREFIX, booleanTypeReference);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(customBooleanAndIntUriLiteralParser);

                var fullUri = new Uri("http://www.odata.com/OData/Chimeras" + string.Format("?$filter=Upgraded eq {0}'{1}'", BOOLEAN_LITERAL_PREFIX, CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                    .Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetBoolean(true)).Source.ShouldBeConstantQueryNode(true);
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(BOOLEAN_LITERAL_PREFIX);
                CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanAndIntUriLiteralParser);
            }
        }

        [Fact]
        public void CustomUriLiteralPrefix_ParseTypeWithCorrectLiteralPrefixAndUriParserPerEdmType()
        {
            RegisterTestCase("CustomUriLiteralPrefix_ParseTypeWithCorrectLiteralPrefixAndUriParserPerEdmType");
            var customBooleanUriLiteralParser = new CustomUriLiteralParserUnitTests.MyCustomBooleanUriLiteralParser();
            try
            {
                IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(CustomUriLiteralParserUnitTests.BOOLEAN_LITERAL_PREFIX, booleanTypeReference);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(booleanTypeReference, customBooleanUriLiteralParser);

                var fullUri = new Uri("http://www.odata.com/OData/Chimeras" + string.Format("?$filter=Upgraded eq {0}'{1}'", CustomUriLiteralParserUnitTests.BOOLEAN_LITERAL_PREFIX, CustomUriLiteralParserUnitTests.CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri) { Resolver = new ODataUriResolver() { EnableCaseInsensitive = false } };

                parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                    .Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetBoolean(true)).Source.ShouldBeConstantQueryNode(true);
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(CustomUriLiteralParserUnitTests.BOOLEAN_LITERAL_PREFIX);
                CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customBooleanUriLiteralParser);
            }
        }

        [Fact]
        public void CustomUriLiteralPrefix_CanSetCustomLiteralToQuotedValue()
        {
            RegisterTestCase("CustomUriLiteralPrefix_CanSetCustomLiteralToQuotedValue");
            const string LITERAL_PREFIX = "myCustomStringTypePrefixLiteral";
            IUriLiteralParser customstringUriTypePraser = new MyCustomStringUriLiteralParser();
            IEdmTypeReference stringTypeReference = EdmCoreModel.Instance.GetString(true);

            try
            {
                CustomUriLiteralPrefixes.AddCustomLiteralPrefix(LITERAL_PREFIX, stringTypeReference);
                CustomUriLiteralParsers.AddCustomUriLiteralParser(stringTypeReference, customstringUriTypePraser);


                var fullUri = new Uri("http://www.odata.com/OData/People" + string.Format("?$filter=Name eq {0}'{1}'", LITERAL_PREFIX, CUSTOM_PARSER_STRING_VALID_VALUE));
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData/"), fullUri);

                parser.ParseFilter().Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                    .Right.ShouldBeConstantQueryNode(CUSTOM_PARSER_STRING_VALID_VALUE + CUSTOM_PARSER_STRING_ADDED_VALUE);
            }
            finally
            {
                CustomUriLiteralPrefixes.RemoveCustomLiteralPrefix(LITERAL_PREFIX);
                CustomUriLiteralParsers.RemoveCustomUriLiteralParser(customstringUriTypePraser);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Try to convert a valid value that the Custom boolean type parser can parse.
        /// If the parser has been added successfully to the UriCustomLiteralParser, this method will passed successfully.
        /// </summary>
        private void ParseNonConvetionalBooleanValueSuccessfully()
        {
            // Consider to add  -
            // PrivateType, check if the custom type parser has been insterted to Parsers lists

            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            UriLiteralParsingException exception;
            object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE, booleanTypeReference, out exception);

            // Assert
            Assert.Null(exception);
            Assert.Equal(true, output);
        }

        /// <summary>
        /// Try to convert a valid value that the Custom int type parser can parse.
        /// If the parser has been added successfully to the UriCustomLiteralParser, this method will passed successfully.
        /// </summary>
        private void ParseNonConvetionalIntValueSuccessfully()
        {
            // Consider to add  -
            // PrivateType, check if the custom type parser has been insterted to Parsers lists


            IEdmTypeReference Int32TypeReference = EdmCoreModel.Instance.GetInt32(false);

            UriLiteralParsingException exception;
            object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_INT_VALID_VALUE, Int32TypeReference, out exception);

            // Assert
            Assert.Null(exception);
            Assert.Equal(55, output);
        }

        /// <summary>
        /// Try to convert a valid value that the Custom boolean type parser can parse.
        /// If the parser has NOT been added to the UriCustomLiteralParser, this method will passed successfully.
        /// </summary>
        private void NoParsesForNonConvetionalBooleanValue()
        {
            IEdmTypeReference booleanTypeReference = EdmCoreModel.Instance.GetBoolean(false);

            UriLiteralParsingException exception;
            object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE, booleanTypeReference, out exception);

            // Assert
            Assert.Null(exception);
            Assert.Null(output);
        }

        /// <summary>
        /// Try to convert a valid value that the Custom Int32 type parser can parse.
        /// If the parser has NOT been added to the UriCustomLiteralParser, this method will passed successfully.
        /// </summary>
        private void NoParsesForNonConvetionalIntValue()
        {
            IEdmTypeReference Int32TypeReference = EdmCoreModel.Instance.GetInt32(false);

            UriLiteralParsingException exception;
            object output = CustomUriLiteralParsers.Instance.ParseUriStringToType(CUSTOM_PARSER_INT_VALID_VALUE, Int32TypeReference, out exception);

            // Assert
            Assert.Null(exception);
            Assert.Null(output);
        }

        #endregion

        private static List<string> registeredTestCases = new List<string>();

        public static List<string> RegisteredTestCases { get { return registeredTestCases; } }

        private static void RegisterTestCase(string caseName)
        {
            if (!registeredTestCases.Contains(caseName))
            {
                registeredTestCases.Add(caseName);
            }
        }

        #region Custom Type Parsers

        internal class MyCustomBooleanUriLiteralParser : IUriLiteralParser
        {
            public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
            {
                parsingException = null;

                if (!RegisteredTestCases.Exists(testCase => Environment.StackTrace.ToString().Contains(testCase)))
                {
                    return null;
                }

                if (!targetType.IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(false)))
                {
                    return null;
                }

                // Take care of literals
                if (text.StartsWith(BOOLEAN_LITERAL_PREFIX))
                {
                    text = text.Replace(BOOLEAN_LITERAL_PREFIX, string.Empty);
                    text = UriParserHelper.RemoveQuotes(text);
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
                    parsingException = new UriLiteralParsingException("Failed to convert boolean.");
                    return null;
                }

                return null;
            }
        }

        internal class MyCustomIntAndBooleanUriLiteralParser : IUriLiteralParser
        {
            public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
            {
                if (!RegisteredTestCases.Exists(testCase => Environment.StackTrace.ToString().Contains(testCase)))
                {
                    parsingException = null;
                    return null;
                }

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

            private object ParseUriStringToInt(string text, out UriLiteralParsingException parsingException)
            {
                parsingException = null;

                if (!RegisteredTestCases.Exists(testCase => Environment.StackTrace.ToString().Contains(testCase)))
                {
                    return null;
                }

                if (text == CUSTOM_PARSER_INT_VALID_VALUE)
                {
                    return 55;
                }

                return null;
            }

            public object ParseUriStringToBoolean(string text, out UriLiteralParsingException parsingException)
            {
                parsingException = null;

                if (!RegisteredTestCases.Exists(testCase => Environment.StackTrace.ToString().Contains(testCase)))
                {
                    return null;
                }

                // Take care of literals
                if (text.StartsWith(BOOLEAN_LITERAL_PREFIX))
                {
                    text = text.Replace(BOOLEAN_LITERAL_PREFIX, string.Empty);
                    text = UriParserHelper.RemoveQuotes(text);
                }

                if (text == CUSTOM_PARSER_BOOLEAN_VALID_VALUE_TRUE)
                {
                    return true;
                }
                else if (text == CUSTOM_PARSER_BOOLEAN_VALUE_TRUE_VALID)
                {
                    parsingException = new UriLiteralParsingException("Failed to convert boolean.");
                }
                else if (text == CUSTOM_PARSER_BOOLEAN_INVALID_VALUE)
                {
                    parsingException = new UriLiteralParsingException("Failed to convert boolean.");
                    return null;
                }

                return null;
            }
        }

        internal class MyCustomStringUriLiteralParser : IUriLiteralParser
        {
            public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
            {
                parsingException = null;
                if (!RegisteredTestCases.Exists(testCase => Environment.StackTrace.ToString().Contains(testCase)))
                {
                    return null;
                }

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
                if (text.StartsWith(STRING_LITERAL_PREFIX))
                {
                    text = text.Replace(STRING_LITERAL_PREFIX, string.Empty);

                    // If Literal exists, not need of quotes
                    isLiteralPrefixExists = true;
                }

                if (!isLiteralPrefixExists && !UriParserHelper.IsUriValueQuoted(text))
                {
                    parsingException = new UriLiteralParsingException("Edm.String value must be quoted");
                    return null;
                }

                text = UriParserHelper.RemoveQuotes(text);

                // Simulates a bug in a client Parser
                if (text == CUSTOM_PARSER_STRING_VALUE_CAUSEBUG)
                {
                    parsingException = new UriLiteralParsingException("Parsing to Edm.String has failed for some reasons");
                    return "RetunsAValueButSupposeToBeNull";
                }

                if (text.StartsWith(CUSTOM_PARSER_STRING_VALID_VALUE))
                {
                    return text + CUSTOM_PARSER_STRING_ADDED_VALUE;
                }

                return null;
            }
        }

        internal class HeatBeatCustomUriLiteralParser : IUriLiteralParser
        {
            public static EdmComplexTypeReference HeartbeatComplexType;

            static HeatBeatCustomUriLiteralParser()
            {
                HeartbeatComplexType = new EdmComplexTypeReference(HardCodedTestModel.GetHeatbeatComplexType(), true);
            }

            public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
            {
                parsingException = null;
                if (!RegisteredTestCases.Exists(testCase => Environment.StackTrace.ToString().Contains(testCase)))
                {
                    return null;
                }

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
                    parsingException = new UriLiteralParsingException("Edm.Heartbeat value must be quoted");
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
