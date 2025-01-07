//---------------------------------------------------------------------
// <copyright file="EdmErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Validation;
using Xunit;
using Microsoft.OData.Edm.Csdl;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Tests.Validation
{
    public class EdmErrorTests
    {
        public static IEnumerable<object[]> NoLocationNoSeverity =>
        new List<object[]>
        {
           new object[] { null, EdmErrorCode.EnumMemberMustHaveValue, "Enum Member Must Have Value"},
           new object[] { null, EdmErrorCode.AllNavigationPropertiesMustBeMapped, "All Navigation Properties Must Be Mapped"},
           new object[] { null, EdmErrorCode.BadCyclicComplex, "Bad Cyclic Complex"},
        };

        public static IEnumerable<object[]> SeverityNoLocation =>
        new List<object[]>
        {
            new object[] { null, EdmErrorCode.AlreadyDefined, "Already Defined", Severity.Error},
            new object[] { null, EdmErrorCode.BadAmbiguousElementBinding, "Bad Ambiguous Element Binding", Severity.Info},
            new object[] { null, EdmErrorCode.BadUnresolvedEntitySet, "Bad Unresolved Entity Set", Severity.Warning},
        };

        public static IEnumerable<object[]> UndefinedSeverityNoLocation =>
        new List<object[]>
        {
            new object[] { null, EdmErrorCode.IntegerConstantValueOutOfRange, "Integer Constant Value Out Of Range", Severity.Undefined},
            new object[] { null, EdmErrorCode.InvalidInteger, "Invalid Integer", Severity.Undefined},
            new object[] { null, EdmErrorCode.SameRoleReferredInReferentialConstraint, "Same Role Referred In Referential Constraint", Severity.Undefined},
        };

        public static IEnumerable<object[]> LocationNoSeverity =>
        new List<object[]>
        {
            new object[] { new CsdlLocation(3, 2), EdmErrorCode.BadUnresolvedTerm, "Bad Unresolved Term"},
            new object[] { new CsdlLocation(11, 2), EdmErrorCode.DeclaringTypeMustBeCorrect, "Declaring Type Must Be Correct"},
        };

        public static IEnumerable<object[]> LocationAndUndefinedSeverity =>
        new List<object[]>
        {
            new object[] { new CsdlLocation(1, 2), EdmErrorCode.ReferentialConstraintPrincipalEndMustBelongToAssociation, "Referential Constraint Principal End Must Belong To Association", Severity.Undefined},
            new object[] { new CsdlLocation(3, 2), EdmErrorCode.UnexpectedXmlElement, "Unexpected Xml Element", Severity.Undefined },
            new object[] { new CsdlLocation(7, 2), EdmErrorCode.TypeDefinitionUnderlyingTypeCannotBeEdmPrimitiveType, "Type Definition Underlying Type Cannot Be EdmPrimitive Type", Severity.Undefined},
        };

        public static IEnumerable<object[]> LocationAndSeverity =>
        new List<object[]>
        {
            new object[] { new CsdlLocation(1, 2), EdmErrorCode.EnumMemberMustHaveValue, "Enum Member Must Have Value", Severity.Error},
            new object[] { new CsdlLocation(3, 2), EdmErrorCode.AllNavigationPropertiesMustBeMapped, "All Navigation Properties Must Be Mapped", Severity.Warning },
            new object[] { new CsdlLocation(7, 2), EdmErrorCode.BadCyclicComplex, "Bad Cyclic Complex", Severity.Info},
        };

        [Theory]
        [MemberData(nameof(NoLocationNoSeverity))]
        public void EdmErrorConstructorWithNoSeverityAndNoLocationShouldOutputCorrectToString(EdmLocation errorLocation, EdmErrorCode errorCode, string errorMessage)
        {
            string expectedString = errorCode.ToString() + " : " + errorMessage;
            EdmError edmError = new EdmError(errorLocation, errorCode, errorMessage);
            Assert.Equal(errorCode, edmError.ErrorCode);
            Assert.Equal(errorMessage, edmError.ErrorMessage);
            Assert.Equal(Severity.Undefined, edmError.Severity);
            Assert.Equal(expectedString, edmError.ToString());
        }

        [Theory]
        [MemberData(nameof(SeverityNoLocation))]
        public void EdmErrorConstructorWithSeverityAndNoLocationShouldOutputCorrectToString(EdmLocation errorLocation, EdmErrorCode errorCode, string errorMessage, Severity severity)
        {
            string expectedString = errorCode.ToString() + " : " + errorMessage + " : " + severity.ToString();
            EdmError edmError = new EdmError(errorLocation, errorCode, errorMessage, severity);
            Assert.Equal(errorCode, edmError.ErrorCode);
            Assert.Equal(errorMessage, edmError.ErrorMessage);
            Assert.Equal(severity, edmError.Severity);
            Assert.Equal(expectedString, edmError.ToString());
        }

        [Theory]
        [MemberData(nameof(UndefinedSeverityNoLocation))]
        public void EdmErrorConstructorWithUndefinedSeverityAndNoLocationShouldOutputCorrectToString(EdmLocation errorLocation, EdmErrorCode errorCode, string errorMessage, Severity severity)
        {
            string expectedString = errorCode.ToString() + " : " + errorMessage;
            EdmError edmError = new EdmError(errorLocation, errorCode, errorMessage, severity);
            Assert.Equal(errorCode, edmError.ErrorCode);
            Assert.Equal(errorMessage, edmError.ErrorMessage);
            Assert.Equal(severity, edmError.Severity);
            Assert.Equal(expectedString, edmError.ToString());
        }

        [Theory]
        [MemberData(nameof(LocationNoSeverity))]
        public void EdmErrorConstructorWithNoSeverityAndWithLocationShouldOutputCorrectToString(EdmLocation errorLocation, EdmErrorCode errorCode, string errorMessage)
        {
            string expectedString = errorCode.ToString() + " : " + errorMessage + " : " + errorLocation.ToString();
            EdmError edmError = new EdmError(errorLocation, errorCode, errorMessage);
            Assert.Equal(errorCode, edmError.ErrorCode);
            Assert.Equal(errorMessage, edmError.ErrorMessage);
            Assert.Equal(Severity.Undefined, edmError.Severity);
            Assert.Equal(expectedString, edmError.ToString());
        }

        [Theory]
        [MemberData(nameof(LocationAndUndefinedSeverity))]
        public void EdmErrorConstructorWithUndefinedSeverityAndLocationShouldOutputCorrectToString(EdmLocation errorLocation, EdmErrorCode errorCode, string errorMessage, Severity severity)
        {
            string expectedString = errorCode.ToString() + " : " + errorMessage + " : " + errorLocation.ToString();
            EdmError edmError = new EdmError(errorLocation, errorCode, errorMessage, severity);
            Assert.Equal(errorCode, edmError.ErrorCode);
            Assert.Equal(errorMessage, edmError.ErrorMessage);
            Assert.Equal(severity, edmError.Severity);
            Assert.Equal(expectedString, edmError.ToString());
        }

        [Theory]
        [MemberData(nameof(LocationAndSeverity))]
        public void EdmErrorConstructorWithSeverityAndLocationShouldOutputCorrectToString(EdmLocation errorLocation, EdmErrorCode errorCode, string errorMessage, Severity severity)
        {
            string expectedString = errorCode.ToString() + " : " + errorMessage + " : " + errorLocation.ToString() + " : " + severity.ToString();
            EdmError edmError = new EdmError(errorLocation, errorCode, errorMessage, severity);
            Assert.Equal(errorCode, edmError.ErrorCode);
            Assert.Equal(errorMessage, edmError.ErrorMessage);
            Assert.Equal(severity, edmError.Severity);
            Assert.Equal(expectedString, edmError.ToString());
        }
    }
}

