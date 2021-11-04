//---------------------------------------------------------------------
// <copyright file="EdmLibTestModelExtractorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities.UnitTests
{
    using System.Linq;
    using Microsoft.OData.Edm;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EdmLibTestModelExtractorTests
    {
        [TestMethod]
        public void GetModelsTest()
        {
            EdmLibTestModelExtractor extractor = new EdmLibTestModelExtractor();
            var models = extractor.GetModels<IEdmModel>(typeof(ModelBuilder), EdmVersion.V40);
            Assert.AreEqual(models.Count(), 3);
        }

        [TestMethod]
        public void GetModelsTestWithAttribute()
        {
            EdmLibTestModelExtractor extractor = new EdmLibTestModelExtractor();
            var models = extractor.GetModels<IEdmModel>(typeof(ModelBuilder), EdmVersion.V40, new ValidationTestInvalidModelAttribute(), true);
            Assert.AreEqual(models.Count(), 2);
        }

        [TestMethod]
        public void GetModelsTestWithoutAttribute()
        {
            EdmLibTestModelExtractor extractor = new EdmLibTestModelExtractor();
            var models = extractor.GetModels<IEdmModel>(typeof(ModelBuilder), EdmVersion.V40, new ValidationTestInvalidModelAttribute(), false);
            Assert.AreEqual(models.Count(), 1);
        }

        internal class ModelBuilder
        {
            public static IEdmModel EdmModel1()
            {
                return new EdmModel();
            }

            [ValidationTestInvalidModelAttribute]
            public static IEdmModel EdmModel2()
            {
                return new EdmModel();
            }

            [ValidationTestInvalidModel()]
            public static IEdmModel EdmModel3()
            {
                return new EdmModel();
            }
        }

    }
}
