//---------------------------------------------------------------------
// <copyright file="ConstructibleModelRemoveMethodsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstructibleModelRemoveMethodsTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void ConstructibleModelRemoveElementDependencyBasicTest()
        {
            var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(this.GetStockModel(ModelBuilder.SimpleConstructiveApiTestModel()));
            var customer = (EdmEntityType)model.FindType("Westwind.Customer");

            var entityTypeCount = model.SchemaElements.OfType<EdmEntityType>().Count();
            Assert.IsTrue(entityTypeCount > 0, "The test model should have at least one entity type");

            var entitySetCount = model.EntityContainer.Elements.OfType<EdmEntitySet>().Count();

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(0, errors.Count(), "Model should pass validation.");

            model.RemoveElement(customer);
            Assert.AreEqual(model.SchemaElements.OfType<EdmEntityType>().Count(), entityTypeCount - 1, "The number of entity types should be decreased.");

            model.Validate(out errors);
            Assert.AreEqual(2, errors.Count(), "Model should now fail validation.");
        }

        private EdmModel GetStockModel(IEnumerable<XElement> csdls)
        {
            return (new EdmToStockModelConverter()).ConvertToStockModel(this.GetParserResult(csdls));
        }

        private string GetFullName(IEdmNamedElement element)
        {
            var schemaElement = element as IEdmSchemaElement;
            if (null != schemaElement)
            {
                return schemaElement.FullName();
            }
            return element.Name;
        }
    }
}

