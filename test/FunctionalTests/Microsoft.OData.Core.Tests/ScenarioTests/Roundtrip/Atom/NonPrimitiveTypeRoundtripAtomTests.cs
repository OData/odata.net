//---------------------------------------------------------------------
// <copyright file="NonPrimitiveTypeRoundtripAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Core.Atom;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.Atom
{
    public class NonPrimitiveTypeRoundtripAtomTests
    {
        private EdmModel model;
        private const string MyNameSpace = "NS";
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/service/");

        public NonPrimitiveTypeRoundtripAtomTests()
        {
            this.model = new EdmModel();

            EdmComplexType personalInfo = new EdmComplexType(MyNameSpace, "PersonalInfo");
            personalInfo.AddStructuralProperty("Age", EdmPrimitiveTypeKind.Int16);
            personalInfo.AddStructuralProperty("Email", EdmPrimitiveTypeKind.String);
            personalInfo.AddStructuralProperty("Tel", EdmPrimitiveTypeKind.String);
            personalInfo.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Guid);
            
            EdmComplexType subjectInfo = new EdmComplexType(MyNameSpace, "Subject");
            subjectInfo.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            subjectInfo.AddStructuralProperty("Score", EdmPrimitiveTypeKind.Int16);

            EdmCollectionTypeReference subjectsCollection = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(subjectInfo, isNullable:true)));

            EdmEntityType studentInfo = new EdmEntityType(MyNameSpace, "Student");
            studentInfo.AddStructuralProperty("Info", new EdmComplexTypeReference(personalInfo, isNullable: false));
            studentInfo.AddProperty(new EdmStructuralProperty(studentInfo, "Subjects", subjectsCollection));

            EdmCollectionTypeReference hobbiesCollection = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: false)));
            studentInfo.AddProperty(new EdmStructuralProperty(studentInfo, "Hobbies", hobbiesCollection));

            model.AddElement(studentInfo);
            model.AddElement(personalInfo);
            model.AddElement(subjectInfo);
        }

        [Fact]
        public void ComplexTypeRoundtripAtomTest()
        {
            var age = new ODataProperty() { Name = "Age", Value = (Int16)18 };
            var email = new ODataProperty() { Name = "Email", Value = "my@microsoft.com" };
            var tel = new ODataProperty() { Name = "Tel", Value = "0123456789" };
            var id = new ODataProperty() { Name = "ID", Value = Guid.Empty };

            ODataComplexValue complexValue = new ODataComplexValue() { TypeName = "NS.PersonalInfo", Properties = new[] { age, email, tel, id } };

            this.VerifyComplexTypeRoundtrip(complexValue, "NS.PersonalInfo");
        }

        [Fact]
        public void ComplexTypeCollectionRoundtripAtomTest()
        {
            ODataComplexValue subject0 = new ODataComplexValue() { TypeName = "NS.Subject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "English" }, new ODataProperty() { Name = "Score", Value = (Int16)98 } } };
            ODataComplexValue subject1 = new ODataComplexValue() { TypeName = "NS.Subject", Properties = new[] { new ODataProperty() { Name = "Name", Value = "Math" }, new ODataProperty() { Name = "Score", Value = (Int16)90 } } };
            ODataCollectionValue complexCollectionValue = new ODataCollectionValue { TypeName = "Collection(NS.Subject)", Items = new[] { subject0, subject1 } };
            ODataFeedAndEntrySerializationInfo info = new ODataFeedAndEntrySerializationInfo() {
                NavigationSourceEntityTypeName = subject0.TypeName,
                NavigationSourceName = "Subjects",
                ExpectedTypeName = subject0.TypeName
            };

            this.VerifyTypeCollectionRoundtrip(complexCollectionValue, "Subjects", info);
        }

        [Fact]
        public void PrimitiveTypeCollectionRoundtripAtomTest()
        {
            ODataCollectionValue primitiveCollectionValue = new ODataCollectionValue {TypeName = "Collection(Edm.String)", Items = new[]{"Basketball", "Swimming"}};
            ODataFeedAndEntrySerializationInfo info = new ODataFeedAndEntrySerializationInfo()
            {
                NavigationSourceEntityTypeName = "Edm.String",
                NavigationSourceName = "Hobbies",
                ExpectedTypeName = "Edm.String"
            };

            this.VerifyTypeCollectionRoundtrip(primitiveCollectionValue, "Hobbies", info);
        }

        private void VerifyTypeCollectionRoundtrip(ODataCollectionValue value, string propertyName, ODataFeedAndEntrySerializationInfo info)
        {
            var properties = new[] { new ODataProperty { Name = propertyName, Value = value } };
            var entry = new ODataEntry() { TypeName = "NS.Student", Properties = properties, SerializationInfo = info};
            MemoryStream stream = new MemoryStream();
            using (ODataAtomOutputContext outputContext = new ODataAtomOutputContext(
                ODataFormat.Atom,
                new NonDisposingStream(stream),
                Encoding.UTF8,
                new ODataMessageWriterSettings() { Version = ODataVersion.V4 },
                /*writingResponse*/ true,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null))
            {
                outputContext.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                var atomWriter = new ODataAtomWriter(outputContext, /*entitySet*/ null, /*entityType*/ null, /*writingFeed*/ false);
                atomWriter.WriteStart(entry);
                atomWriter.WriteEnd();

            }

            stream.Position = 0;
            object actualValue = null;

            using (ODataAtomInputContext inputContext = new ODataAtomInputContext(
                ODataFormat.Atom,
                stream,
                Encoding.UTF8,
                new ODataMessageReaderSettings(),
                /*readingResponse*/ true,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null))
            {
                var atomReader = new ODataAtomReader(inputContext, /*entitySet*/ null, /*entityType*/ null, /*writingFeed*/ false);
                while (atomReader.Read())
                {
                    if (atomReader.State == ODataReaderState.EntryEnd)
                    {
                        ODataEntry entryOut = atomReader.Item as ODataEntry;
                        actualValue = entryOut.Properties.Single(p => p.Name == propertyName).ODataValue;
                    }
                }
            }

            TestUtils.AssertODataValueAreEqual(actualValue as ODataValue, value);
        }

        private void VerifyComplexTypeRoundtrip(ODataComplexValue value, string typeName)
        {
            var typeReference = new EdmComplexTypeReference((IEdmComplexType)model.FindType(typeName), true);
            MemoryStream stream = new MemoryStream();
            using (ODataAtomOutputContext outputContext = new ODataAtomOutputContext(
                ODataFormat.Atom,
                new NonDisposingStream(stream),
                Encoding.UTF8,
                new ODataMessageWriterSettings() { Version = ODataVersion.V4 },
                /*writingResponse*/ true,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null))
            {
                ODataAtomPropertyAndValueSerializer serializer = new ODataAtomPropertyAndValueSerializer(outputContext);
                serializer.XmlWriter.WriteStartElement("ValueElement");
                serializer.WriteComplexValue(
                    value, 
                    typeReference, 
                    /*isOpenPropertyType*/ false, 
                    /*isWritingCollection*/ false, 
                    /*beforeValueAction*/ null, 
                    /*afterValueAction*/ null, 
                    new DuplicatePropertyNamesChecker(false, false), 
                    /*collectionValidator*/ null, 
                    /*projectedProperties*/ null);
                serializer.XmlWriter.WriteEndElement();
            }

            stream.Position = 0;
            object actualValue;

            using (ODataAtomInputContext inputContext = new ODataAtomInputContext(
                ODataFormat.Atom,
                stream,
                Encoding.UTF8,
                new ODataMessageReaderSettings(),
                /*readingResponse*/ true,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null))
            {
                ODataAtomPropertyAndValueDeserializer deserializer = new ODataAtomPropertyAndValueDeserializer(inputContext);
                deserializer.XmlReader.MoveToContent();
                actualValue = deserializer.ReadNonEntityValue(
                    typeReference, 
                    /*duplicatePropertyNamesChecker*/ null,
                    /*collectionValidator*/ null, 
                    /*validateNullValue*/ true);
            }

            TestUtils.AssertODataValueAreEqual(actualValue as ODataValue, value);
        }
    }
}