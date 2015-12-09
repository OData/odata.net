//---------------------------------------------------------------------
// <copyright file="NonPrimitiveTypeRoundtripJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Core.Tests.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.JsonLight
{
    public class NonPrimitiveTypeRoundtripJsonLightTests
    {
        private EdmModel model;
        private const string MyNameSpace = "NS";
        private EdmEntityType studentInfo;
        private EdmEntitySet studentSet;

        public NonPrimitiveTypeRoundtripJsonLightTests()
        {
            this.model = new EdmModel();

            EdmComplexType personalInfo = new EdmComplexType(MyNameSpace, "PersonalInfo");
            personalInfo.AddStructuralProperty("Age", EdmPrimitiveTypeKind.Int16);
            personalInfo.AddStructuralProperty("Email", EdmPrimitiveTypeKind.String);
            personalInfo.AddStructuralProperty("Tel", EdmPrimitiveTypeKind.String);
            personalInfo.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Guid);

            EdmComplexType derivedPersonalInfo = new EdmComplexType(MyNameSpace, "DerivedPersonalInfo", personalInfo);
            derivedPersonalInfo.AddStructuralProperty("Hobby", EdmPrimitiveTypeKind.String);

            EdmComplexType derivedDerivedPersonalInfo = new EdmComplexType(MyNameSpace, "DerivedDerivedPersonalInfo", derivedPersonalInfo);
            derivedDerivedPersonalInfo.AddStructuralProperty("Education", EdmPrimitiveTypeKind.String);

            EdmComplexType subjectInfo = new EdmComplexType(MyNameSpace, "Subject");
            subjectInfo.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            subjectInfo.AddStructuralProperty("Score", EdmPrimitiveTypeKind.Int16);

            EdmComplexType derivedSubjectInfo = new EdmComplexType(MyNameSpace, "DerivedSubject", subjectInfo);
            derivedSubjectInfo.AddStructuralProperty("Teacher", EdmPrimitiveTypeKind.String);

            EdmComplexType derivedDerivedSubjectInfo = new EdmComplexType(MyNameSpace, "DerivedDerivedSubject", derivedSubjectInfo);
            derivedDerivedSubjectInfo.AddStructuralProperty("Classroom", EdmPrimitiveTypeKind.String);

            EdmCollectionTypeReference subjectsCollection = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(subjectInfo, isNullable: true)));

            studentInfo = new EdmEntityType(MyNameSpace, "Student");
            studentInfo.AddStructuralProperty("Info", new EdmComplexTypeReference(personalInfo, isNullable: false));
            studentInfo.AddProperty(new EdmStructuralProperty(studentInfo, "Subjects", subjectsCollection));

            // enum with flags
            var enumFlagsType = new EdmEnumType(MyNameSpace, "ColorFlags", isFlags: true);
            enumFlagsType.AddMember("Red", new EdmIntegerConstant(1));
            enumFlagsType.AddMember("Green", new EdmIntegerConstant(2));
            enumFlagsType.AddMember("Blue", new EdmIntegerConstant(4));
            studentInfo.AddStructuralProperty("ClothesColors", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEnumTypeReference(enumFlagsType, true))));

            EdmCollectionTypeReference hobbiesCollection = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: false)));
            studentInfo.AddProperty(new EdmStructuralProperty(studentInfo, "Hobbies", hobbiesCollection));

            model.AddElement(enumFlagsType);
            model.AddElement(studentInfo);
            model.AddElement(personalInfo);
            model.AddElement(derivedPersonalInfo);
            model.AddElement(derivedDerivedPersonalInfo);
            model.AddElement(subjectInfo);
            model.AddElement(derivedSubjectInfo);
            model.AddElement(derivedDerivedSubjectInfo);

            IEdmEntityContainer defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            model.AddElement(defaultContainer);

            this.studentSet = new EdmEntitySet(defaultContainer, "MySet", this.studentInfo);
        }

        [Fact]
        public void ComplexTypeRoundtripJsonLightTest()
        {
            var age = new ODataProperty() { Name = "Age", Value = (Int16)18 };
            var email = new ODataProperty() { Name = "Email", Value = "my@microsoft.com" };
            var tel = new ODataProperty() { Name = "Tel", Value = "0123456789" };
            var id = new ODataProperty() { Name = "ID", Value = Guid.Empty };

            ODataComplexValue complexValue = new ODataComplexValue() { TypeName = "NS.PersonalInfo", Properties = new[] { age, email, tel, id } };
            this.VerifyNonPrimitiveTypeRoundtrip(complexValue, "Info");
        }

        [Fact]
        public void InheritComplexTypeRoundtripJsonLightTest()
        {
            var age = new ODataProperty() { Name = "Age", Value = (Int16)18 };
            var email = new ODataProperty() { Name = "Email", Value = "my@microsoft.com" };
            var tel = new ODataProperty() { Name = "Tel", Value = "0123456789" };
            var id = new ODataProperty() { Name = "ID", Value = Guid.Empty };
            var hobby = new ODataProperty() { Name = "Hobby", Value = "none" };
            var edu = new ODataProperty() { Name = "Education", Value = "MIT" };

            ODataComplexValue complexValue = new ODataComplexValue() { TypeName = "NS.DerivedDerivedPersonalInfo", Properties = new[] { age, email, tel, id, hobby, edu } };
            this.VerifyNonPrimitiveTypeRoundtrip(complexValue, "Info");
        }

        [Fact]
        public void OpenInheritComplexTypeAndComplexTypeCollectionRoundtripJsonLightTest()
        {
            EdmComplexType baseComplexType = new EdmComplexType(MyNameSpace, "BaseComplex", null, false, true);
            baseComplexType.AddStructuralProperty("ID1", EdmPrimitiveTypeKind.Int16);

            EdmComplexType derivedComplexType = new EdmComplexType(MyNameSpace, "DerivedComplex", baseComplexType, false, true);
            derivedComplexType.AddStructuralProperty("ID2", EdmPrimitiveTypeKind.Int16);

            EdmCollectionTypeReference complexCollection = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(baseComplexType, isNullable: true)));

            this.model.AddElement(baseComplexType);
            this.model.AddElement(derivedComplexType);
            this.studentInfo.AddStructuralProperty("TestInheritComplex", new EdmComplexTypeReference(baseComplexType, isNullable: false));
            this.studentInfo.AddProperty(new EdmStructuralProperty(studentInfo, "TestCollection", complexCollection));
            var id1 = new ODataProperty() { Name = "ID1", Value = (Int16)18 };
            var id2 = new ODataProperty() { Name = "ID2", Value = (Int16)18 };
            var id3 = new ODataProperty() { Name = "ID3", Value = (Int16)18 };

            ODataComplexValue complexValue1 = new ODataComplexValue() { TypeName = "NS.BaseComplex", Properties = new[] { id1, id2, id3 } };
            this.VerifyNonPrimitiveTypeRoundtrip(complexValue1, "TestInheritComplex");

            ODataComplexValue complexValue2 = new ODataComplexValue() { TypeName = "NS.DerivedComplex", Properties = new[] { id1, id2, id3 } };
            this.VerifyNonPrimitiveTypeRoundtrip(complexValue2, "TestInheritComplex");

            ODataCollectionValue complexCollectionValue = new ODataCollectionValue { TypeName = "Collection(NS.BaseComplex)", Items = new[] { complexValue1, complexValue2 } };
            this.VerifyNonPrimitiveTypeRoundtrip(complexCollectionValue, "TestCollection");
        }

        #region enum
        [Fact]
        public void EnumTypeCollectionRoundtripJsonLightTest()
        {
            ODataEnumValue subject0 = new ODataEnumValue("Red", "NS.ColorFlags");
            ODataEnumValue subject1 = new ODataEnumValue("123", "NS.ColorFlags");
            ODataCollectionValue complexCollectionValue = new ODataCollectionValue { TypeName = "Collection(NS.ColorFlags)", Items = new[] { subject0, subject1 } };

            this.VerifyNonPrimitiveTypeRoundtrip(complexCollectionValue, "ClothesColors");
        }
        #endregion

        [Fact]
        public void ComplexTypeCollectionRoundtripJsonLightTest()
        {
            ODataComplexValue subject0 = new ODataComplexValue() { TypeName = "NS.Subject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "English" }, new ODataProperty() { Name = "Score", Value = (Int16)98 } } };
            ODataComplexValue subject1 = new ODataComplexValue() { TypeName = "NS.Subject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "Math" }, new ODataProperty() { Name = "Score", Value = (Int16)90 } } };
            ODataCollectionValue complexCollectionValue = new ODataCollectionValue { TypeName = "Collection(NS.Subject)", Items = new[] { subject0, subject1 } };

            this.VerifyNonPrimitiveTypeRoundtrip(complexCollectionValue, "Subjects");
        }

        [Fact]
        public void InheritComplexTypeCollectionRoundtripJsonLightTest()
        {
            ODataComplexValue subject0 = new ODataComplexValue() { TypeName = "NS.DerivedDerivedSubject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "English" }, new ODataProperty() { Name = "Score", Value = (Int16)98 }, new ODataProperty() { Name = "Teacher", Value = "Mr Li" }, new ODataProperty() { Name = "Classroom", Value = "Room101" } } };
            ODataComplexValue subject1 = new ODataComplexValue() { TypeName = "NS.DerivedSubject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "Math" }, new ODataProperty() { Name = "Score", Value = (Int16)90 }, new ODataProperty() { Name = "Teacher", Value = "Mr Liu" } } };
            ODataCollectionValue complexCollectionValue = new ODataCollectionValue { TypeName = "Collection(NS.Subject)", Items = new[] { subject0, subject1 } };

            this.VerifyNonPrimitiveTypeRoundtrip(complexCollectionValue, "Subjects");
        }

        [Fact]
        public void PrimitiveTypeCollectionRoundtripJsonLightTest()
        {
            ODataCollectionValue primitiveCollectionValue = new ODataCollectionValue { TypeName = "Collection(Edm.String)", Items = new[] { "Basketball", "Swimming" } };
            this.VerifyNonPrimitiveTypeRoundtrip(primitiveCollectionValue, "Hobbies");
        }

        private void VerifyNonPrimitiveTypeRoundtrip(object value, string propertyName)
        {
            var properties = new[] { new ODataProperty { Name = propertyName, Value = value } };
            var entry = new ODataEntry() { TypeName = "NS.Student", Properties = properties };

            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            MemoryStream stream = new MemoryStream();

            using (ODataJsonLightOutputContext outputContext = new ODataJsonLightOutputContext(
                ODataFormat.Json,
                new NonDisposingStream(stream),
                new ODataMediaType("application", "json"),
                Encoding.UTF8,
                settings,
                /*writingResponse*/ false,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null))
            {
                var jsonLightWriter = new ODataJsonLightWriter(outputContext, this.studentSet, this.studentInfo, /*writingFeed*/ false);
                jsonLightWriter.WriteStart(entry);
                jsonLightWriter.WriteEnd();
            }

            stream.Position = 0;
            object actualValue = null;

            using (ODataJsonLightInputContext inputContext = new ODataJsonLightInputContext(
                ODataFormat.Json,
                stream,
                JsonLightUtils.JsonLightStreamingMediaType,
                Encoding.UTF8,
                new ODataMessageReaderSettings(),
                /*readingResponse*/ false,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null))
            {
                var jsonLightReader = new ODataJsonLightReader(inputContext, this.studentSet, this.studentInfo, /*readingFeed*/ false);
                while (jsonLightReader.Read())
                {
                    if (jsonLightReader.State == ODataReaderState.EntryEnd)
                    {
                        ODataEntry entryOut = jsonLightReader.Item as ODataEntry;
                        actualValue = entryOut.Properties.Single(p => p.Name == propertyName).ODataValue;
                    }
                }
            }

            TestUtils.AssertODataValueAreEqual(actualValue as ODataValue, value as ODataValue);
        }
    }
}
