//---------------------------------------------------------------------
// <copyright file="UriBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.UriBuilderTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriBuilder;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UriBuilderTests
    {
        private readonly Uri serviceRoot = new Uri("http://www.example.com/");
        private readonly ODataUriParserSettings settings = new ODataUriParserSettings();
     
        private EdmModel GetModel()
        {
            EdmModel model = new EdmModel();
            EdmEntityType edmEntityType = new EdmEntityType("NS", "Person");
            edmEntityType.AddKeys(edmEntityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            edmEntityType.AddStructuralProperty("Color", EdmPrimitiveTypeKind.String);
            edmEntityType.AddStructuralProperty("FA", EdmPrimitiveTypeKind.String);
            edmEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MyDog", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, Target = edmEntityType }); 
            edmEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MyCat", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, Target = edmEntityType });
            model.AddElement(edmEntityType);
            EdmEntityContainer container = new EdmEntityContainer("NS", "EntityContainer");
            model.AddElement(container);
            container.AddEntitySet("People", edmEntityType);

            return model;
        }

        [TestMethod]
        public void ODataUriBuilderWithEntitySet()
        {
            Uri fullUri = new Uri("http://www.example.com/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
            ODataUriParser odataUriParser = new ODataUriParser(this.GetModel(), serviceRoot, fullUri);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            ODataUri odataUri = odataUriParser.ParseUri();

            //verify path
            EntitySetSegment entitySet = (EntitySetSegment)odataUri.Path.FirstSegment;
            Assert.AreEqual(entitySet.EntitySet.Name, "People");
            Assert.AreEqual(odataUri.Path.Count, 1);

            //verify $filter
            BinaryOperatorNode binaryOperator = (BinaryOperatorNode)odataUri.Filter.Expression;
            SingleValuePropertyAccessNode singleValueProperty = (SingleValuePropertyAccessNode)binaryOperator.Left;
            SingleNavigationNode singleNavigation = (SingleNavigationNode)singleValueProperty.Source;
            ConstantNode constantNode = (ConstantNode)binaryOperator.Right;

            Assert.AreEqual(binaryOperator.OperatorKind, BinaryOperatorKind.Equal);
            Assert.AreEqual(singleValueProperty.Property.Name, "Color");
            Assert.AreEqual(singleNavigation.NavigationProperty.Name, "MyDog");
            Assert.AreEqual(constantNode.LiteralText, "'Brown'");

            //verify $select and $expand
            IEnumerable<SelectItem> selectItems = odataUri.SelectAndExpand.SelectedItems;
            IEnumerable<ExpandedNavigationSelectItem> expandedNavigationSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)).OfType<ExpandedNavigationSelectItem>();
            IEnumerable<ExpandedReferenceSelectItem> expandedReferenceSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)).OfType<ExpandedReferenceSelectItem>();
            IEnumerable<PathSelectItem> pathSelectItem = selectItems.Where(I => I.GetType() == typeof(PathSelectItem)).OfType<PathSelectItem>();
            Assert.AreEqual(expandedNavigationSelectItem.Count(), 1);
            Assert.AreEqual(expandedReferenceSelectItem.Count(), 1);
            Assert.AreEqual(pathSelectItem.Count(), 3);
            NavigationPropertySegment navigationProperty = (NavigationPropertySegment)expandedNavigationSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.AreEqual(navigationProperty.NavigationProperty.Name, "MyDog");
            navigationProperty = (NavigationPropertySegment)expandedReferenceSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.AreEqual(navigationProperty.NavigationProperty.Name, "MyCat");

            //verify $orderby
            SingleValuePropertyAccessNode orderby = (SingleValuePropertyAccessNode)odataUri.OrderBy.Expression;
            Assert.AreEqual(orderby.Property.Name, "ID");

            //verify $top
            Assert.AreEqual(odataUri.Top, 1);

            //verify $skip
            Assert.AreEqual(odataUri.Skip, 2);

            //verify $count
            Assert.AreEqual(odataUri.QueryCount, true);

            //verify $search
            SearchTermNode searchTermNode = (SearchTermNode)odataUri.Search.Expression;
            Assert.AreEqual(searchTermNode.Text, "FA");

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.AreEqual(new Uri("http://www.example.com/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID%2CMyDog%2CMyCat&$expand=MyDog%2CMyCat%2F%24ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA"), actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.AreEqual(new Uri("http://www.example.com/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID%2CMyDog%2CMyCat&$expand=MyDog%2CMyCat%2F%24ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA"), actualUri);
        }

        [TestMethod]
        public void TestODataUriBuilderWithKeySegment()
        {
            Uri fullUri = new Uri("http://www.example.com/People(1)?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
            ODataUriParser odataUriParser = new ODataUriParser(this.GetModel(), serviceRoot, fullUri);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            ODataUri odataUri = odataUriParser.ParseUri();

            //verify path
            EntitySetSegment entitySet = (EntitySetSegment)odataUri.Path.FirstSegment;
            KeySegment keySegment = (KeySegment)odataUri.Path.LastSegment;
            IEnumerable<KeyValuePair<string, object> > keyValuePairs = keySegment.Keys;
            Assert.AreEqual(odataUri.Path.Count, 2);
            Assert.AreEqual(entitySet.EntitySet.Name, "People");
            foreach (var keyValuePair in keyValuePairs)
            {
                Assert.AreEqual(keyValuePair.Key, "ID");
                Assert.AreEqual(keyValuePair.Value, 1);
            }

            //verify $filter
            BinaryOperatorNode binaryOperator = (BinaryOperatorNode)odataUri.Filter.Expression;
            SingleValuePropertyAccessNode singleValueProperty = (SingleValuePropertyAccessNode)binaryOperator.Left;
            SingleNavigationNode singleNavigation = (SingleNavigationNode)singleValueProperty.Source;
            ConstantNode constantNode = (ConstantNode)binaryOperator.Right;

            Assert.AreEqual(binaryOperator.OperatorKind, BinaryOperatorKind.Equal);
            Assert.AreEqual(singleValueProperty.Property.Name, "Color");
            Assert.AreEqual(singleNavigation.NavigationProperty.Name, "MyDog");
            Assert.AreEqual(constantNode.LiteralText, "'Brown'");

            //verify $select and $expand
            IEnumerable<SelectItem> selectItems = odataUri.SelectAndExpand.SelectedItems;
            IEnumerable<ExpandedNavigationSelectItem> expandedNavigationSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)).OfType<ExpandedNavigationSelectItem>();
            IEnumerable<ExpandedReferenceSelectItem> expandedReferenceSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)).OfType<ExpandedReferenceSelectItem>();
            IEnumerable<PathSelectItem> pathSelectItem = selectItems.Where(I => I.GetType() == typeof(PathSelectItem)).OfType<PathSelectItem>();
            Assert.AreEqual(expandedNavigationSelectItem.Count(), 1);
            Assert.AreEqual(expandedReferenceSelectItem.Count(), 1);
            Assert.AreEqual(pathSelectItem.Count(), 3);
            NavigationPropertySegment navigationProperty = (NavigationPropertySegment)expandedNavigationSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.AreEqual(navigationProperty.NavigationProperty.Name, "MyDog");
            navigationProperty = (NavigationPropertySegment)expandedReferenceSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.AreEqual(navigationProperty.NavigationProperty.Name, "MyCat");

            //verify $orderby
            SingleValuePropertyAccessNode orderby = (SingleValuePropertyAccessNode)odataUri.OrderBy.Expression;
            Assert.AreEqual(orderby.Property.Name, "ID");

            //verify $top
            Assert.AreEqual(odataUri.Top, 1);

            //verify $skip
            Assert.AreEqual(odataUri.Skip, 2);

            //verify $count
            Assert.AreEqual(odataUri.QueryCount, true);

            //verify $search
            SearchTermNode searchTermNode = (SearchTermNode)odataUri.Search.Expression;
            Assert.AreEqual(searchTermNode.Text, "FA");
            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.AreEqual(new Uri("http://www.example.com/People(1)?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID%2CMyDog%2CMyCat&$expand=MyDog%2CMyCat%2F%24ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA"), actualUri);
        }

        [TestMethod]
        public void ODataUriBuilderWithKeyAsSegment()
        {
            Uri fullUri = new Uri("http://www.example.com/People/1?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$top=1&$skip=2&$count=false");
            ODataUriParser oDataUriParser = new ODataUriParser(this.GetModel(), serviceRoot, fullUri);
            oDataUriParser.UrlConventions = ODataUrlConventions.KeyAsSegment;
            SetODataUriParserSettingsTo(this.settings, oDataUriParser.Settings);
            ODataUri odataUri = oDataUriParser.ParseUri();
            
            //verify path
            EntitySetSegment entitySet = (EntitySetSegment)odataUri.Path.FirstSegment;
            KeySegment keySegment = (KeySegment)odataUri.Path.LastSegment;
            IEnumerable<KeyValuePair<string, object>> keyValuePairs = keySegment.Keys;
            Assert.AreEqual(odataUri.Path.Count, 2);
            Assert.AreEqual(entitySet.EntitySet.Name, "People");
            foreach (var keyValuePair in keyValuePairs)
            {
                Assert.AreEqual(keyValuePair.Key, "ID");
                Assert.AreEqual(keyValuePair.Value, 1);
            }

            //verify $filter
            BinaryOperatorNode binaryOperator = (BinaryOperatorNode)odataUri.Filter.Expression;
            SingleValuePropertyAccessNode singleValueProperty = (SingleValuePropertyAccessNode)binaryOperator.Left;
            SingleNavigationNode singleNavigation = (SingleNavigationNode)singleValueProperty.Source;
            ConstantNode constantNode = (ConstantNode)binaryOperator.Right;

            Assert.AreEqual(binaryOperator.OperatorKind, BinaryOperatorKind.Equal);
            Assert.AreEqual(singleValueProperty.Property.Name, "Color");
            Assert.AreEqual(singleNavigation.NavigationProperty.Name, "MyDog");
            Assert.AreEqual(constantNode.LiteralText, "'Brown'");

            //verify $select and $expand
            IEnumerable<SelectItem> selectItems = odataUri.SelectAndExpand.SelectedItems;
            IEnumerable<ExpandedNavigationSelectItem> expandedNavigationSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)).OfType<ExpandedNavigationSelectItem>();
            IEnumerable<ExpandedReferenceSelectItem> expandedReferenceSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)).OfType<ExpandedReferenceSelectItem>();
            IEnumerable<PathSelectItem> pathSelectItem = selectItems.Where(I => I.GetType() == typeof(PathSelectItem)).OfType<PathSelectItem>();
            Assert.AreEqual(expandedNavigationSelectItem.Count(), 1);
            Assert.AreEqual(expandedReferenceSelectItem.Count(), 1);
            Assert.AreEqual(pathSelectItem.Count(), 3);
            NavigationPropertySegment navigationProperty = (NavigationPropertySegment)expandedNavigationSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.AreEqual(navigationProperty.NavigationProperty.Name, "MyDog");
            navigationProperty = (NavigationPropertySegment)expandedReferenceSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.AreEqual(navigationProperty.NavigationProperty.Name, "MyCat");

            //verify $top
            Assert.AreEqual(odataUri.Top, 1);

            //verify $skip
            Assert.AreEqual(odataUri.Skip, 2);

            //verify $count
            Assert.AreEqual(odataUri.QueryCount, false);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            Uri actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.AreEqual(new Uri("http://www.example.com/People/1?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID%2CMyDog%2CMyCat&$expand=MyDog%2CMyCat%2F%24ref&$top=1&$skip=2&$count=false"), actualUri);
        }

        [TestMethod]
        public void RelativeUriBuildWithEmptyQueryOptions()
        {
            Uri queryUri = new Uri("People", UriKind.Relative);
            ODataUriParser odataUriParser = new ODataUriParser(this.GetModel(), queryUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.AreEqual(queryUri, actualUri);
        }

        [TestMethod]
        public void AbsoluteUriBuildWithEmptyQueryOptions()
        {
            Uri queryUri = new Uri("http://www.example.com/People");
            ODataUriParser odataUriParser = new ODataUriParser(this.GetModel(), serviceRoot, queryUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.AreEqual(queryUri, actualUri);
        }

        #region set ODataUriParserSettings method
        private static void SetODataUriParserSettingsTo(ODataUriParserSettings sourceSettings, ODataUriParserSettings destSettings)
        {
            if (sourceSettings != null)
            {
                destSettings.MaximumExpansionCount = sourceSettings.MaximumExpansionCount;
                destSettings.MaximumExpansionDepth = sourceSettings.MaximumExpansionDepth;
            }
        }
        #endregion
    }
}
