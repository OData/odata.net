//---------------------------------------------------------------------
// <copyright file="AnnotationsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Caching;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Service.Serializers;
    using System.IO;
    using System.Linq;
    using Microsoft.OData.Service;
    using System.Xml;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class AnnotationsTests
    {
        private Version v4 = new Version(4, 0);

        const string vocabulary =
@"<Schema Namespace=""MyVocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Rating"" Type=""Int32"" />
    <Term Name=""CanEdit"" Type=""Boolean"" />

    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""String"" />
        <Property Name=""FirstName"" Type=""String"" Nullable=""false"" />
        <Property Name=""LastName"" Type=""String"" Nullable=""false"" />
    </EntityType>

    <EntityContainer Name=""MyContainer"">
        <EntitySet Name=""Persons"" EntityType=""MyVocabulary.Person"" />
    </EntityContainer>
</Schema>";

        const string annotations1 =
@"<Schema Namespace=""MyAnnotations1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">

    <EntityType Name=""DummyType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" />
    </EntityType>

    <EntityContainer Name=""DummyContainer"" Extends=""MyVocabulary.MyContainer"">
        <EntitySet Name=""Dummies"" EntityType=""MyAnnotations1.Dummy"" />
    </EntityContainer>

    <Annotations Target=""MyModel.Customer"">
        <Annotation Term=""MyVocabulary.Rating"" Qualifier=""Primary"" Int=""1"" />
    </Annotations>

    <Annotations Target=""Splat.Order"">
        <Annotation Term=""Vocab.Rating"" Int=""5"" />
    </Annotations>
</Schema>";

        // TODO: Target entity-sets and properties once they are supported in EdmLib
        const string annotations2 =
@"<Schema Namespace=""MyAnnotations2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">

    <Annotations Target=""MyModel.CustomersContainer"">
        <Annotation Term=""MyVocabulary.CanEdit"" Bool=""true"" />
    </Annotations>

    <Annotations Target=""MyModel.CustomersContainer/Customers"">
        <Annotation Term=""MyVocabulary.CanEdit"" Bool=""true"" />
    </Annotations>

    <Annotations Target=""MyModel.Customer"">
        <!-- Duplicate -->
        <Annotation Term=""MyVocabulary.Rating"" Qualifier=""Primary"" Int=""10"" />

        <Annotation Term=""MyVocabulary.Rating"" Qualifier=""Secondary"" Int=""2"" />
    </Annotations>

    <Annotations Target=""MyModel.Customer/FirstName"">
        <Annotation Term=""MyVocabulary.CanEdit"" Bool=""true"" />
    </Annotations>
</Schema>";

        const int validAnnotationsCount = 5;

        [TestMethod]
        public void CsdlAnnotations()
        {
            DataServiceConfiguration config;
            DataServiceOperationContext operationContext;
            DataServiceProviderWrapper provider = CreateProvider(out config, out operationContext);

            IEnumerable<EdmError> errors;

            config.AnnotationsBuilder = (model) =>
            {
                XmlReader[] xmlReaders;
                bool parsed;

                IEdmModel vocabularyModel;
                xmlReaders = new XmlReader[] { XmlReader.Create(new StringReader(vocabulary)) };
                parsed = SchemaReader.TryParse(xmlReaders, out vocabularyModel, out errors);
                Assert.IsTrue(parsed);

                IEdmModel annotationsModel1;
                xmlReaders = new XmlReader[] { XmlReader.Create(new StringReader(annotations1)) };
                parsed = SchemaReader.TryParse(xmlReaders, new IEdmModel[] { model, vocabularyModel }, out annotationsModel1, out errors);
                Assert.IsTrue(parsed);

                IEdmModel annotationsModel2;
                xmlReaders = new XmlReader[] { XmlReader.Create(new StringReader(annotations2)) };
                parsed = SchemaReader.TryParse(xmlReaders, new IEdmModel[] { model, vocabularyModel }, out annotationsModel2, out errors);
                Assert.IsTrue(parsed);

                // Annotations are intentionally duplicated to test duplicate filtering
                return new IEdmModel[] { annotationsModel1, annotationsModel2, annotationsModel2, annotationsModel1 };
            };

            IEdmModel annotatedModel = MetadataSerializer.PrepareModelForSerialization(provider, config);

            Assert.AreEqual(v4, provider.ResponseMetadataVersion);

            // TODO: Stop accounting for invalid names
            EdmValidator.Validate(annotatedModel, out errors);
            Assert.AreEqual(validAnnotationsCount, errors.Count());
            Assert.IsTrue(errors.All(e => e.ErrorCode == EdmErrorCode.BadUnresolvedTerm));
            
            Version edmxVersion = v4;
            MetadataSerializer.ValidateModel(annotatedModel, edmxVersion);

            IEdmEntityContainer customersContainer;
            IEdmEntitySet customersSet;
            IEdmEntityType customerType;
            VerifyEntitySetsAndTypes(annotatedModel, out customersContainer, out customersSet, out customerType);

            Assert.AreEqual(validAnnotationsCount, annotatedModel.VocabularyAnnotations.Count());

            IEdmVocabularyAnnotation[] customerAnnotations = 
                annotatedModel.FindDeclaredVocabularyAnnotations(customerType).ToArray();
            Assert.AreEqual(2, customerAnnotations.Count());
            
            var ratingPrimary = customerAnnotations[0];
            Assert.AreEqual(customerType, ratingPrimary.Target);
            Assert.AreEqual("Primary", ratingPrimary.Qualifier);
            Assert.AreEqual("Rating", ratingPrimary.Term.Name);        
            Assert.AreEqual(1, ((IEdmIntegerConstantExpression)ratingPrimary.Value).Value);

            var ratingSecondary = customerAnnotations[1];
            Assert.AreEqual(customerType, ratingSecondary.Target);
            Assert.AreEqual("Secondary", ratingSecondary.Qualifier);
            Assert.AreEqual("Rating", ratingSecondary.Term.Name);
            Assert.AreEqual(2, ((IEdmIntegerConstantExpression)ratingSecondary.Value).Value);

            IEdmVocabularyAnnotation canEdit;

            canEdit = annotatedModel.FindDeclaredVocabularyAnnotations(customersContainer).Single();
            Assert.AreEqual(customersContainer, canEdit.Target);
            Assert.AreEqual("CanEdit", canEdit.Term.Name);
            Assert.AreEqual(true, ((IEdmBooleanConstantExpression)canEdit.Value).Value);

            canEdit = annotatedModel.FindDeclaredVocabularyAnnotations(customersSet).Single();
            Assert.AreEqual(customersSet, canEdit.Target);
            Assert.AreEqual("CanEdit", canEdit.Term.Name);
            Assert.AreEqual(true, ((IEdmBooleanConstantExpression)canEdit.Value).Value);

            IEdmProperty firstNameProperty = customerType.FindProperty("FirstName");
            canEdit = annotatedModel.FindDeclaredVocabularyAnnotations(firstNameProperty).Single();
            Assert.AreEqual(firstNameProperty, canEdit.Target);
            Assert.AreEqual("CanEdit", canEdit.Term.Name);
            Assert.AreEqual(true, ((IEdmBooleanConstantExpression)canEdit.Value).Value);
        }

        [TestMethod]
        public void EmptyAnnotationsBuilderResult()
        {
            DataServiceConfiguration config;
            DataServiceOperationContext operationContext;
            CreateProvider(out config, out operationContext);

            config.AnnotationsBuilder = model => new IEdmModel[0];

            var testSubject = new VocabularyAnnotationCache(new EdmModel());
            testSubject.PopulateFromConfiguration(config);

            testSubject.VocabularyAnnotations.Should().BeEmpty();
        }

        [TestMethod]
        public void NullAnnotationsBuilderResult()
        {
            DataServiceConfiguration config;
            DataServiceOperationContext operationContext;
            CreateProvider(out config, out operationContext);

            config.AnnotationsBuilder = (model) => null;

            var testSubject = new VocabularyAnnotationCache(new EdmModel());
            testSubject.PopulateFromConfiguration(config);

            testSubject.VocabularyAnnotations.Should().BeEmpty();
        }

        [TestMethod]
        public void NullAnnotationsModel()
        {
            DataServiceConfiguration config;
            DataServiceOperationContext operationContext;
            CreateProvider(out config, out operationContext);

            config.AnnotationsBuilder = model => new IEdmModel[] { null };

            var testSubject = new VocabularyAnnotationCache(new EdmModel());
            Action populateFromConfig = () => testSubject.PopulateFromConfiguration(config);
            populateFromConfig.ShouldThrow<InvalidOperationException>().WithMessage("The collection returned by DataServiceConfiguration.AnnotationsBuilder must not contain null elements.");
        }

        [TestMethod]
        public void AnnotatedModelShouldSupportAddingStringAnnotations()
        {
            var primaryModel = new EdmModel();
            var entityContainer = new EdmEntityContainer("Fake", "Container");
            primaryModel.AddElement(entityContainer);

            var testSubject = new VocabularyAnnotationCache(primaryModel);

            var annotation = new EdmVocabularyAnnotation(entityContainer, new EdmTerm("fake", "foo", EdmPrimitiveTypeKind.String), new EdmStringConstant("bar"));
            testSubject.Add(annotation);
            testSubject.FindDeclaredVocabularyAnnotations(entityContainer).Should().Contain(annotation).And.HaveCount(1);
        }

        private static DataServiceProviderWrapper CreateProvider(out DataServiceConfiguration config, out DataServiceOperationContext operationContext)
        {
            var baseUri = new Uri("http://localhost");
            var host = new DataServiceHostSimulator()
            {
                AbsoluteServiceUri = baseUri,
                AbsoluteRequestUri = new Uri(baseUri.AbsoluteUri + "/$metadata", UriKind.Absolute),
                RequestHttpMethod = "GET",
                RequestAccept = "application/xml+atom",
                RequestVersion = "4.0",
                RequestMaxVersion = "4.0",
            };

            operationContext = new DataServiceOperationContext(host);
            var dataService = new DataServiceSimulator() { OperationContext = operationContext };
            operationContext.InitializeAndCacheHeaders(dataService);

            DataServiceProviderSimulator providerSimulator = new DataServiceProviderSimulator();
            providerSimulator.ContainerNamespace = "MyModel";
            providerSimulator.ContainerName = "CustomersContainer";

            ResourceType customerEntityType = new ResourceType(
                typeof(object), ResourceTypeKind.EntityType, null, "MyModel", "Customer", false)
            { 
                CanReflectOnInstanceType = false 
            };

            ResourcePropertyKind idPropertyKind = ResourcePropertyKind.Primitive | ResourcePropertyKind.Key;
            ResourceProperty idProperty = new ResourceProperty(
                "Id", idPropertyKind, ResourceType.GetPrimitiveResourceType(typeof(int)))
            { 
                CanReflectOnInstanceTypeProperty = false 
            };
            customerEntityType.AddProperty(idProperty);

            ResourcePropertyKind firstNamePropertyKind = ResourcePropertyKind.Primitive | ResourcePropertyKind.Key;
            ResourceProperty firstNameProperty = new ResourceProperty(
                "FirstName", firstNamePropertyKind, ResourceType.GetPrimitiveResourceType(typeof(string)))
            {
                CanReflectOnInstanceTypeProperty = false
            };
            customerEntityType.AddProperty(firstNameProperty);            
            
            customerEntityType.SetReadOnly();
            providerSimulator.AddResourceType(customerEntityType);

            ResourceSet customerSet = new ResourceSet("Customers", customerEntityType);
            customerSet.SetReadOnly();
            providerSimulator.AddResourceSet(customerSet);

            config = new DataServiceConfiguration(providerSimulator);
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

            IDataServiceProviderBehavior providerBehavior = DataServiceProviderBehavior.CustomDataServiceProviderBehavior;
            DataServiceStaticConfiguration staticConfig = new DataServiceStaticConfiguration(dataService.Instance.GetType(), providerSimulator);

            DataServiceProviderWrapper provider = new DataServiceProviderWrapper(
                new DataServiceCacheItem(config, staticConfig), providerSimulator, providerSimulator, dataService, false);

            dataService.ProcessingPipeline = new DataServiceProcessingPipeline();
            dataService.Provider = provider;
            provider.ProviderBehavior = providerBehavior;
            dataService.ActionProvider = DataServiceActionProviderWrapper.Create(dataService);
#if DEBUG
            dataService.ProcessingPipeline.SkipDebugAssert = true;
#endif
            operationContext.RequestMessage.InitializeRequestVersionHeaders(VersionUtil.ToVersion(config.DataServiceBehavior.MaxProtocolVersion));
            return provider;
        }

        private static void VerifyEntitySetsAndTypes(
            IEdmModel annotatedModel, 
            out IEdmEntityContainer customersContainer, 
            out IEdmEntitySet customersSet, 
            out IEdmEntityType customerType)
        {
            customersContainer = annotatedModel.EntityContainer;
            Assert.AreEqual("CustomersContainer", customersContainer.Name);

            Assert.AreEqual(1, customersContainer.EntitySets().Count());
            customersSet = customersContainer.FindEntitySet("Customers");
            Assert.IsNotNull(customersSet);

            IEnumerable<IEdmEntityType> entityTypes = annotatedModel.SchemaElements.OfType<IEdmEntityType>();
            Assert.AreEqual(1, entityTypes.Count());
            customerType = (IEdmEntityType) annotatedModel.FindDeclaredType("MyModel.Customer");
        }
    }
}
