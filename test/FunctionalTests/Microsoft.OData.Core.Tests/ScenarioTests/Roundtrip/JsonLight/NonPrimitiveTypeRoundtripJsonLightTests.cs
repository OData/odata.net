//---------------------------------------------------------------------
// <copyright file="NonPrimitiveTypeRoundtripJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Tests.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight
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
            enumFlagsType.AddMember("Red", new EdmEnumMemberValue(1));
            enumFlagsType.AddMember("Green", new EdmEnumMemberValue(2));
            enumFlagsType.AddMember("Blue", new EdmEnumMemberValue(4));
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

        [Fact(Skip = "Enable after writer is implementated")]
        public void ComplexTypeRoundtripJsonLightTest()
        {
            var age = new ODataProperty() { Name = "Age", Value = (Int16)18 };
            var email = new ODataProperty() { Name = "Email", Value = "my@microsoft.com" };
            var tel = new ODataProperty() { Name = "Tel", Value = "0123456789" };
            var id = new ODataProperty() { Name = "ID", Value = Guid.Empty };

            ODataResource complexValue = new ODataResource() { TypeName = "NS.PersonalInfo", Properties = new[] { age, email, tel, id } };
            this.VerifyComplexRoundtrip("Info", complexValue);
        }

        [Fact(Skip = "Enable after writer is implementated")]
        public void InheritComplexTypeRoundtripJsonLightTest()
        {
            var age = new ODataProperty() { Name = "Age", Value = (Int16)18 };
            var email = new ODataProperty() { Name = "Email", Value = "my@microsoft.com" };
            var tel = new ODataProperty() { Name = "Tel", Value = "0123456789" };
            var id = new ODataProperty() { Name = "ID", Value = Guid.Empty };
            var hobby = new ODataProperty() { Name = "Hobby", Value = "none" };
            var edu = new ODataProperty() { Name = "Education", Value = "MIT" };

            ODataResource complexValue = new ODataResource() { TypeName = "NS.DerivedDerivedPersonalInfo", Properties = new[] { age, email, tel, id, hobby, edu } };
            this.VerifyComplexRoundtrip("Info", complexValue);
        }

        [Fact(Skip = "Enable after writer is implementated")]
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

            ODataResource complexValue1 = new ODataResource() { TypeName = "NS.BaseComplex", Properties = new[] { id1, id2, id3 } };
            this.VerifyComplexRoundtrip("TestInheritComplex", complexValue1);

            ODataResource complexValue2 = new ODataResource() { TypeName = "NS.DerivedComplex", Properties = new[] { id1, id2, id3 } };
            this.VerifyComplexRoundtrip("TestInheritComplex", complexValue2);

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

        [Fact(Skip = "Enable after writer is implementated")]
        public void ComplexTypeCollectionRoundtripJsonLightTest()
        {
            ODataComplexValue subject0 = new ODataComplexValue() { TypeName = "NS.Subject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "English" }, new ODataProperty() { Name = "Score", Value = (Int16)98 } } };
            ODataComplexValue subject1 = new ODataComplexValue() { TypeName = "NS.Subject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "Math" }, new ODataProperty() { Name = "Score", Value = (Int16)90 } } };
            ODataCollectionValue complexCollectionValue = new ODataCollectionValue { TypeName = "Collection(NS.Subject)", Items = new[] { subject0, subject1 } };

            this.VerifyNonPrimitiveTypeRoundtrip(complexCollectionValue, "Subjects");
        }

        [Fact(Skip = "Enable after writer is implementated")]
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
            var entry = new ODataResource() { TypeName = "NS.Student", Properties = properties };

            var stream = new MemoryStream();

            var messageInfoForWriter = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = false,
                IsAsync = false,
                Model = model
            };

            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty };

            using (var outputContext = new ODataJsonLightOutputContext(messageInfoForWriter, settings))
            {
                var jsonLightWriter = new ODataJsonLightWriter(outputContext, this.studentSet, this.studentInfo, /*writingFeed*/ false);
                jsonLightWriter.WriteStart(entry);
                jsonLightWriter.WriteEnd();
            }

            stream.Position = 0;
            object actualValue = null;

            var messageInfoForReader = new ODataMessageInfo
            {
                Encoding = Encoding.UTF8,
                IsResponse = false,
                MediaType = JsonLightUtils.JsonLightStreamingMediaType,
                IsAsync = false,
                Model = model,
                MessageStream = stream
            };

            using (var inputContext = new ODataJsonLightInputContext(messageInfoForReader, new ODataMessageReaderSettings()))
            {
                var jsonLightReader = new ODataJsonLightReader(inputContext, this.studentSet, this.studentInfo, /*readingFeed*/ false);
                while (jsonLightReader.Read())
                {
                    if (jsonLightReader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entryOut = jsonLightReader.Item as ODataResource;
                        actualValue = entryOut.Properties.Single(p => p.Name == propertyName).ODataValue;
                    }
                }
            }

            TestUtils.AssertODataValueAreEqual(actualValue as ODataValue, value as ODataValue);
        }

        private void VerifyComplexRoundtrip(string propertyName, params ODataResource[] values)
        {
            var nestedResourceInfo = new ODataNestedResourceInfo() { Name = propertyName, IsCollection = false};
            var entry = new ODataResource() { TypeName = "NS.Student" };

            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            MemoryStream stream = new MemoryStream();

            var messageInfoForWriter = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = false,
                IsAsync = false,
                Model = model
            };

            using (var outputContext = new ODataJsonLightOutputContext(messageInfoForWriter, settings))
            {
                var jsonLightWriter = new ODataJsonLightWriter(outputContext, this.studentSet, this.studentInfo, /*writingFeed*/ false);
                jsonLightWriter.WriteStart(entry);
                jsonLightWriter.WriteEnd();
            }

            using (var outputContext = new ODataJsonLightOutputContext(messageInfoForWriter, settings))
            {
                var jsonLightWriter = new ODataJsonLightWriter(outputContext, this.studentSet, this.studentInfo, /*writingFeed*/ false);
                jsonLightWriter.WriteStart(entry);
                jsonLightWriter.WriteStart(nestedResourceInfo);
                foreach (var value in values)
                {
                    jsonLightWriter.WriteStart(value);
                }
                jsonLightWriter.WriteEnd();
                jsonLightWriter.WriteEnd();
                jsonLightWriter.WriteEnd();
            }

            stream.Position = 0;
            List<ODataResource> actualValues = new List<ODataResource>();

            var messageInfoForReader = new ODataMessageInfo
            {
                Encoding = Encoding.UTF8,
                IsResponse = false,
                MediaType = JsonLightUtils.JsonLightStreamingMediaType,
                IsAsync = false,
                Model = model,
                MessageStream = stream
            };

            using (var inputContext = new ODataJsonLightInputContext(messageInfoForReader, new ODataMessageReaderSettings()))
            {
                var jsonLightReader = new ODataJsonLightReader(inputContext, this.studentSet, this.studentInfo, /*readingFeed*/ false);
                while (jsonLightReader.Read())
                {
                    if (jsonLightReader.State == ODataReaderState.ResourceEnd)
                    {
                        actualValues.Add(jsonLightReader.Item as ODataResource);

                    }
                }
            }

            TestUtils.AssertODataResourceSetAreEqual(actualValues.Skip(1).ToList(), values.ToList());
        }
    }
}
