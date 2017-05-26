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

        [Fact]
        public void ComplexTypeRoundtripJsonLightTest()
        {
            var age = new ODataProperty() { Name = "Age", Value = (Int16)18 };
            var email = new ODataProperty() { Name = "Email", Value = "my@microsoft.com" };
            var tel = new ODataProperty() { Name = "Tel", Value = "0123456789" };
            var id = new ODataProperty() { Name = "ID", Value = Guid.Empty };

            ODataResource complex = new ODataResource() { TypeName = "NS.PersonalInfo", Properties = new[] { age, email, tel, id } };
            this.VerifyComplexRoundtrip("Info", null, complex);
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

            ODataResource complex = new ODataResource() { TypeName = "NS.DerivedDerivedPersonalInfo", Properties = new[] { age, email, tel, id, hobby, edu } };
            this.VerifyComplexRoundtrip("Info", null, complex);
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

            ODataResource complex1 = new ODataResource() { TypeName = "NS.BaseComplex", Properties = new[] { id1, id2, id3 } };
            this.VerifyComplexRoundtrip("TestInheritComplex", null, complex1);

            ODataResource complex2 = new ODataResource() { TypeName = "NS.DerivedComplex", Properties = new[] { id1, id2, id3 } };
            this.VerifyComplexRoundtrip("TestInheritComplex", null, complex2);

            ODataResourceSet complexCollectionResourceSet = new ODataResourceSet { TypeName = "Collection(NS.BaseComplex)" };
            this.VerifyComplexRoundtrip("TestCollection", complexCollectionResourceSet, complex1, complex2);
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
            ODataResource subject0 = new ODataResource() { TypeName = "NS.Subject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "English" }, new ODataProperty() { Name = "Score", Value = (Int16)98 } } };
            ODataResource subject1 = new ODataResource() { TypeName = "NS.Subject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "Math" }, new ODataProperty() { Name = "Score", Value = (Int16)90 } } };
            ODataResourceSet complexCollection = new ODataResourceSet { TypeName = "Collection(NS.Subject)" };

            this.VerifyComplexRoundtrip("Subjects", complexCollection, subject0, subject1);
        }

        [Fact]
        public void InheritComplexTypeCollectionRoundtripJsonLightTest()
        {
            ODataResource subject0 = new ODataResource() { TypeName = "NS.DerivedDerivedSubject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "English" }, new ODataProperty() { Name = "Score", Value = (Int16)98 }, new ODataProperty() { Name = "Teacher", Value = "Mr Li" }, new ODataProperty() { Name = "Classroom", Value = "Room101" } } };
            ODataResource subject1 = new ODataResource() { TypeName = "NS.DerivedSubject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "Math" }, new ODataProperty() { Name = "Score", Value = (Int16)90 }, new ODataProperty() { Name = "Teacher", Value = "Mr Liu" } } };
            ODataResourceSet complexCollection = new ODataResourceSet { TypeName = "Collection(NS.Subject)" };

            this.VerifyComplexRoundtrip("Subjects", complexCollection, subject0, subject1);
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

            var settings = new ODataMessageWriterSettings
            {
                Version = ODataVersion.V4,
                Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
            };

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

        private void VerifyComplexRoundtrip(string propertyName, ODataResourceSet resourceSet, params ODataResource[] resources)
        {
            var nestedResourceInfo = new ODataNestedResourceInfo() { Name = propertyName, IsCollection = resourceSet != null };
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
                jsonLightWriter.WriteStart(nestedResourceInfo);
                if (resourceSet != null)
                {
                    jsonLightWriter.WriteStart(resourceSet);
                }

                foreach (var value in resources)
                {
                    jsonLightWriter.WriteStart(value);
                    jsonLightWriter.WriteEnd();
                }

                if (resourceSet != null)
                {
                    jsonLightWriter.WriteEnd();
                }

                jsonLightWriter.WriteEnd();
                jsonLightWriter.WriteEnd();
            }

            stream.Position = 0;
            List<ODataResource> actualResources = new List<ODataResource>();

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
                        actualResources.Add(jsonLightReader.Item as ODataResource);
                    }
                }
            }

            var count = actualResources.Count;
            actualResources.RemoveAt(count - 1);
            TestUtils.AssertODataResourceSetAreEqual(actualResources, resources.ToList());
        }



    }
}
