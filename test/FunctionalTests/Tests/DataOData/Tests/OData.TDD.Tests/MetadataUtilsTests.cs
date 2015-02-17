//---------------------------------------------------------------------
// <copyright file="MetadataUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests
{
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MetadataUtilsTests
    {
        private readonly IEdmModel model = BuildModel();

        [TestMethod]
        public void CalculateBindableOperationsForEntityTypeWithoutTypeResolver()
        {
            var bindingType = this.model.FindType("TestModel.Movie");
            var bindableOperations = MetadataUtils.CalculateBindableOperationsForType(bindingType, this.model, new EdmTypeReaderResolver(this.model, ODataReaderBehavior.DefaultBehavior));
            Assert.AreEqual(1, bindableOperations.Length);
            foreach (var operation in bindableOperations)
            {
                Assert.AreEqual("Rate", operation.Name);
            }
        }

        [TestMethod]
        public void CalculateBindableOperationsForDerivedEntityTypeWithoutTypeResolver()
        {
            var bindingType = this.model.FindType("TestModel.TVMovie");
            var bindableOperations = MetadataUtils.CalculateBindableOperationsForType(bindingType, this.model, new EdmTypeReaderResolver(this.model, ODataReaderBehavior.DefaultBehavior));
            
            Assert.AreEqual(2, bindableOperations.Length);
            Assert.IsTrue(bindableOperations.Count(o => o.Name == "Rate") == 1);
            Assert.IsTrue(bindableOperations.Count(o => o.Name == "ChangeChannel") == 1);
        }

        [TestMethod]
        public void CalculateBindableOperationsForEntityCollectionTypeWithoutTypeResolver()
        {
            var bindingType = new EdmCollectionType(this.model.FindType("TestModel.Movie").ToTypeReference(nullable:false));
            var bindableOperations = MetadataUtils.CalculateBindableOperationsForType(bindingType, this.model, new EdmTypeReaderResolver(this.model, ODataReaderBehavior.DefaultBehavior));
            Assert.AreEqual(2, bindableOperations.Length);
            foreach (var operation in bindableOperations)
            {
                Assert.AreEqual("RateMultiple", operation.Name);
            }
        }

        private IEdmType NameToTypeResolver(IEdmType expectedType, string typeName)
        {
            IEdmType movieType = this.model.FindType("TestModel.Movie");
            if (typeName == "TestModel.Movie")
            {
                return movieType;
            }

            IEdmType tvMovieType = this.model.FindType("TestModel.TVMovie");
            if (typeName == "TestModel.TVMovie")
            {
                return tvMovieType;
            }
            
            return expectedType;
        }

        [TestMethod]
        public void CalculateBindableOperationsForEntityTypeWithTypeResolver()
        {
            var bindingType = this.model.FindType("TestModel.Movie");
            var bindableOperations = MetadataUtils.CalculateBindableOperationsForType(bindingType, this.model, new EdmTypeReaderResolver(this.model, ODataReaderBehavior.CreateWcfDataServicesClientBehavior(this.NameToTypeResolver)));
            Assert.AreEqual(1, bindableOperations.Length);
            foreach (var operation in bindableOperations)
            {
                Assert.AreEqual("Rate", operation.Name);
            }
        }

        [TestMethod]
        public void CalculateBindableOperationsForDerivedEntityTypeWithTypeResolver()
        {
            var bindingType = this.model.FindType("TestModel.TVMovie");
            var bindableOperations = MetadataUtils.CalculateBindableOperationsForType(bindingType, this.model, new EdmTypeReaderResolver(this.model, ODataReaderBehavior.CreateWcfDataServicesClientBehavior(this.NameToTypeResolver)));

            Assert.AreEqual(2, bindableOperations.Length);
            Assert.IsTrue(bindableOperations.Count(o => o.Name == "Rate") == 1);
            Assert.IsTrue(bindableOperations.Count(o => o.Name == "ChangeChannel") == 1);
        }

        [TestMethod]
        public void CalculateBindableOperationsForEntityCollectionTypeWithTypeResolver()
        {
            var bindingType = new EdmCollectionType(this.model.FindType("TestModel.Movie").ToTypeReference(nullable: false));
            var bindableOperations = MetadataUtils.CalculateBindableOperationsForType(bindingType, this.model, new EdmTypeReaderResolver(this.model, ODataReaderBehavior.CreateWcfDataServicesClientBehavior(this.NameToTypeResolver)));
            Assert.AreEqual(2, bindableOperations.Length);
            foreach (var operation in bindableOperations)
            {
                Assert.AreEqual("RateMultiple", operation.Name);
            }
        }

        [TestMethod]
        public void HasDeclaredKeyProperty()
        {
            var movieType = (IEdmEntityType)this.model.FindType("TestModel.Movie");
            var idProperty = movieType.Properties().Single(p => p.Name == "Id");
            var nameProperty = movieType.Properties().Single(p => p.Name == "Name");

            Assert.IsTrue(movieType.HasDeclaredKeyProperty(idProperty));
            Assert.IsFalse(movieType.HasDeclaredKeyProperty(nameProperty));

            var tvMovieType = (IEdmEntityType)this.model.FindType("TestModel.TVMovie");
            var channelProperty = tvMovieType.Properties().Single(p => p.Name == "Channel");
            Assert.IsTrue(movieType.HasDeclaredKeyProperty(idProperty));
            Assert.IsFalse(movieType.HasDeclaredKeyProperty(channelProperty));
        }

        private static IEdmModel BuildModel()
        {
            EdmModel model = new EdmModel();
            
            var movieType = new EdmEntityType("TestModel", "Movie");
            EdmStructuralProperty idProperty = new EdmStructuralProperty(movieType, "Id", EdmCoreModel.Instance.GetInt32(false));
            movieType.AddProperty(idProperty);
            movieType.AddKeys(idProperty);
            movieType.AddProperty(new EdmStructuralProperty(movieType, "Name", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(movieType);

            var tvMovieType = new EdmEntityType("TestModel", "TVMovie", movieType);
            tvMovieType.AddProperty(new EdmStructuralProperty(tvMovieType, "Channel", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(tvMovieType);
            
            EdmEntityContainer defaultContainer = new EdmEntityContainer("TestModel", "Default");
            defaultContainer.AddEntitySet("Movies", movieType);
            model.AddElement(defaultContainer);

            EdmAction simpleAction = new EdmAction("TestModel", "SimpleAction", null /*returnType*/, false /*isBound*/, null /*entitySetPath*/);
            model.AddElement(simpleAction);
            defaultContainer.AddActionImport(simpleAction);

            EdmAction checkoutAction1 = new EdmAction("TestModel", "Checkout", EdmCoreModel.Instance.GetInt32(false), false /*isBound*/, null /*entitySetPath*/);
            checkoutAction1.AddParameter("movie", movieType.ToTypeReference());
            checkoutAction1.AddParameter("duration", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(checkoutAction1);

            EdmAction rateAction1 = new EdmAction("TestModel", "Rate", null /*returnType*/, true /*isBound*/, null /*entitySetPath*/);
            rateAction1.AddParameter("movie", movieType.ToTypeReference());
            rateAction1.AddParameter("rating", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(rateAction1);

            EdmAction changeChannelAction1 = new EdmAction("TestModel", "ChangeChannel", null /*returnType*/, true /*isBound*/, null /*entitySetPath*/);
            changeChannelAction1.AddParameter("movie", tvMovieType.ToTypeReference());
            changeChannelAction1.AddParameter("channel", EdmCoreModel.Instance.GetString(false));
            model.AddElement(changeChannelAction1);
            
            EdmAction checkoutAction = new EdmAction("TestModel", "Checkout", EdmCoreModel.Instance.GetInt32(false) /*returnType*/, false /*isBound*/, null /*entitySetPath*/);
            checkoutAction.AddParameter("movie", movieType.ToTypeReference());
            checkoutAction.AddParameter("duration", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(checkoutAction);
            
            var movieCollectionTypeReference = (new EdmCollectionType(movieType.ToTypeReference(nullable: false))).ToTypeReference(nullable:false);

            EdmAction checkoutMultiple1Action = new EdmAction("TestModel", "CheckoutMultiple", EdmCoreModel.Instance.GetInt32(false), false /*isBound*/, null /*entitySetPath*/);
            checkoutMultiple1Action.AddParameter("movies", movieCollectionTypeReference);
            checkoutMultiple1Action.AddParameter("duration", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(checkoutMultiple1Action);

            EdmAction rateMultiple1Action = new EdmAction("TestModel", "RateMultiple", null /*returnType*/, true /*isBound*/, null /*entitySetPath*/);
            rateMultiple1Action.AddParameter("movies", movieCollectionTypeReference);
            rateMultiple1Action.AddParameter("rating", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(rateMultiple1Action);

            EdmAction checkoutMultiple2Action = new EdmAction("TestModel", "CheckoutMultiple", EdmCoreModel.Instance.GetInt32(false) /*returnType*/, false /*isBound*/, null /*entitySetPath*/);
            checkoutMultiple2Action.AddParameter("movies", movieCollectionTypeReference);
            checkoutMultiple2Action.AddParameter("duration", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(checkoutMultiple2Action);

            EdmAction rateMultiple2Action = new EdmAction("TestModel", "RateMultiple", null /*returnType*/, true /*isBound*/, null /*entitySetPath*/);
            rateMultiple2Action.AddParameter("movies", movieCollectionTypeReference);
            rateMultiple2Action.AddParameter("rating", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(rateMultiple2Action);
            
            return model;
        }
    }
}
