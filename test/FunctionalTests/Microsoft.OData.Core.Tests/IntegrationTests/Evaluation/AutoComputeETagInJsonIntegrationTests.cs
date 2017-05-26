//---------------------------------------------------------------------
// <copyright file="AutoComputeETagInJsonIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.IntegrationTests.Evaluation
{
    public class AutoComputeETagInJsonIntegrationTests
    {
        [Fact]
        public void AutoComputeETagWithOptimisticConcurrencyAnnotation()
        {
            const string expected = "{" +
                "\"@odata.context\":\"http://example.com/$metadata#People/$entity\"," +
                "\"@odata.id\":\"People(123)\"," +
                "\"@odata.etag\":\"W/\\\"'lucy',12306\\\"\"," +
                "\"@odata.editLink\":\"People(123)\"," +
                "\"@odata.mediaEditLink\":\"People(123)/$value\"," +
                "\"ID\":123," +
                "\"Name\":\"lucy\"," +
                "\"Class\":12306," +
                "\"Alias\":\"lily\"}";
            EdmModel model = new EdmModel();

            EdmEntityType personType = new EdmEntityType("MyNs", "Person", null, false, false, true);
            personType.AddKeys(personType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            var nameProperty = personType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: true));
            var classProperty = personType.AddStructuralProperty("Class", EdmPrimitiveTypeKind.Int32);
            personType.AddStructuralProperty("Alias", EdmCoreModel.Instance.GetString(isNullable: true), null);

            var container = new EdmEntityContainer("MyNs", "Container");
            model.AddElement(personType);
            container.AddEntitySet("People", personType);
            model.AddElement(container);

            IEdmEntitySet peopleSet = model.FindDeclaredEntitySet("People");
            model.SetOptimisticConcurrencyAnnotation(peopleSet, new[] { nameProperty, classProperty });
            ODataResource entry = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty {Name = "ID", Value = 123},
                    new ODataProperty {Name = "Name", Value = "lucy"},
                    new ODataProperty {Name = "Class", Value = 12306},
                    new ODataProperty {Name = "Alias", Value = "lily"},
                }
            };

            string actual = GetWriterOutputForContentTypeAndKnobValue(entry, model, peopleSet, personType);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AutoComputeETagWithOptimisticConcurrencyAnnotationForDerivedType()
        {
            const string expected = "{" +
                "\"@odata.context\":\"http://example.com/$metadata#Managers/$entity\"," +
                "\"@odata.id\":\"Managers(123)\"," +
                "\"@odata.etag\":\"W/\\\"'lucy',10\\\"\"," +
                "\"@odata.editLink\":\"Managers(123)\"," +
                "\"@odata.mediaEditLink\":\"Managers(123)/$value\"," +
                "\"ID\":123," +
                "\"Name\":\"lucy\"," +
                "\"Class\":12306," +
                "\"Alias\":\"lily\"," +
                "\"TeamSize\":10}";
            EdmModel model = new EdmModel();

            EdmEntityType personType = new EdmEntityType("MyNs", "Person", null, false, false, true);
            personType.AddKeys(personType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            var nameProperty = personType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: true));
            personType.AddStructuralProperty("Class", EdmPrimitiveTypeKind.Int32);
            personType.AddStructuralProperty("Alias", EdmCoreModel.Instance.GetString(isNullable: true), null);
            EdmEntityType managerType = new EdmEntityType("MyNs", "Manager", personType, false, false, true);
            var tsProperty = managerType.AddStructuralProperty("TeamSize", EdmPrimitiveTypeKind.Int32);

            var container = new EdmEntityContainer("MyNs", "Container");
            model.AddElement(personType);
            model.AddElement(managerType);
            container.AddEntitySet("Managers", managerType);
            model.AddElement(container);

            IEdmEntitySet managerSet = model.FindDeclaredEntitySet("Managers");

            model.SetOptimisticConcurrencyAnnotation(managerSet, new[] { nameProperty, tsProperty });

            ODataResource entry = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty {Name = "ID", Value = 123},
                    new ODataProperty {Name = "Name", Value = "lucy"},
                    new ODataProperty {Name = "Class", Value = 12306},
                    new ODataProperty {Name = "Alias", Value = "lily"},
                    new ODataProperty {Name = "TeamSize", Value = 10},
                }
            };

            string actual = GetWriterOutputForContentTypeAndKnobValue(entry, model, managerSet, managerType);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NoAutoComputeETagIfNoConcurrencyAnnotationAndAttribute()
        {
            const string expected = "{" +
                "\"@odata.context\":\"http://example.com/$metadata#People/$entity\"," +
                "\"@odata.id\":\"People(123)\"," +
                "\"@odata.editLink\":\"People(123)\"," +
                "\"@odata.mediaEditLink\":\"People(123)/$value\"," +
                "\"ID\":123," +
                "\"Name\":\"lucy\"," +
                "\"Class\":12306," +
                "\"Alias\":\"lily\"}";
            EdmModel model = new EdmModel();

            EdmEntityType personType = new EdmEntityType("MyNs", "Person", null, false, false, true);
            personType.AddKeys(personType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            personType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: true));
            personType.AddStructuralProperty("Class", EdmPrimitiveTypeKind.Int32);
            personType.AddStructuralProperty("Alias", EdmCoreModel.Instance.GetString(isNullable: true), null);

            var container = new EdmEntityContainer("MyNs", "Container");
            model.AddElement(personType);
            container.AddEntitySet("People", personType);
            model.AddElement(container);
            IEdmEntitySet peopleSet = model.FindDeclaredEntitySet("People");

            ODataResource entry = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty {Name = "ID", Value = 123},
                    new ODataProperty {Name = "Name", Value = "lucy"},
                    new ODataProperty {Name = "Class", Value = 12306},
                    new ODataProperty {Name = "Alias", Value = "lily"},
                }
            };

            string actual = GetWriterOutputForContentTypeAndKnobValue(entry, model, peopleSet, personType);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ExceptionThrowForInvalidPropertyPath()
        {
            EdmModel model = new EdmModel();

            EdmEntityType personType = new EdmEntityType("MyNs", "Person", null, false, false, true);
            personType.AddKeys(personType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            personType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: true));

            var container = new EdmEntityContainer("MyNs", "Container");
            model.AddElement(personType);
            container.AddEntitySet("People", personType);
            model.AddElement(container);
            IEdmEntitySet peopleSet = model.FindDeclaredEntitySet("People");

            IEdmPathExpression nameExpression = new EdmPropertyPathExpression("NameName");

            IEdmCollectionExpression collection = new EdmCollectionExpression(new[] { nameExpression });
            IEdmTerm term = null;
            foreach (var referencedModel in model.ReferencedModels)
            {
                term = referencedModel.FindDeclaredTerm("Org.OData.Core.V1.OptimisticConcurrency");

                if (term != null)
                {
                    break;
                }
            }

            Assert.NotNull(term);

            EdmVocabularyAnnotation valueAnnotationOnEntitySet = new EdmVocabularyAnnotation(peopleSet, term, collection);
            valueAnnotationOnEntitySet.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(valueAnnotationOnEntitySet);

            ODataResource entry = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty {Name = "ID", Value = 123},
                    new ODataProperty {Name = "Name", Value = "lucy"},
                }
            };

            Action action = () => GetWriterOutputForContentTypeAndKnobValue(entry, model, peopleSet, personType);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.EdmValueUtils_PropertyDoesntExist("MyNs.Person", "NameName"));
        }

        private string GetWriterOutputForContentTypeAndKnobValue(ODataResource entry, EdmModel model, IEdmEntitySetBase entitySet, EdmEntityType entityType)
        {
            MemoryStream outputStream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage() { Stream = outputStream };
            message.SetHeader("Content-Type", "application/json;odata.metadata=full");
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.SetServiceDocumentUri(new Uri("http://example.com"));

            string output;
            using (var messageWriter = new ODataMessageWriter(message, settings, model))
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(entitySet, entityType);
                writer.WriteStart(entry);
                writer.WriteEnd();
                outputStream.Seek(0, SeekOrigin.Begin);
                output = new StreamReader(outputStream).ReadToEnd();
            }

            return output;
        }

    }
}
