//---------------------------------------------------------------------
// <copyright file="UriBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.UriBuilderTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Xunit;

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

        [Fact]
        public void ODataUriBuilderWithEntitySet()
        {
            Uri fullUri = new Uri("http://www.example.com/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
            ODataUriParser odataUriParser = new ODataUriParser(this.GetModel(), serviceRoot, fullUri);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            ODataUri odataUri = odataUriParser.ParseUri();

            //verify path
            EntitySetSegment entitySet = (EntitySetSegment)odataUri.Path.FirstSegment;
            Assert.Equal(entitySet.EntitySet.Name, "People");
            Assert.Equal(odataUri.Path.Count, 1);

            //verify $filter
            BinaryOperatorNode binaryOperator = (BinaryOperatorNode)odataUri.Filter.Expression;
            SingleValuePropertyAccessNode singleValueProperty = (SingleValuePropertyAccessNode)binaryOperator.Left;
            SingleNavigationNode singleNavigation = (SingleNavigationNode)singleValueProperty.Source;
            ConstantNode constantNode = (ConstantNode)binaryOperator.Right;

            Assert.Equal(binaryOperator.OperatorKind, BinaryOperatorKind.Equal);
            Assert.Equal(singleValueProperty.Property.Name, "Color");
            Assert.Equal(singleNavigation.NavigationProperty.Name, "MyDog");
            Assert.Equal(constantNode.LiteralText, "'Brown'");

            //verify $select and $expand
            IEnumerable<SelectItem> selectItems = odataUri.SelectAndExpand.SelectedItems;
            IEnumerable<ExpandedNavigationSelectItem> expandedNavigationSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)).OfType<ExpandedNavigationSelectItem>();
            IEnumerable<ExpandedReferenceSelectItem> expandedReferenceSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)).OfType<ExpandedReferenceSelectItem>();
            IEnumerable<PathSelectItem> pathSelectItem = selectItems.Where(I => I.GetType() == typeof(PathSelectItem)).OfType<PathSelectItem>();
            Assert.Equal(expandedNavigationSelectItem.Count(), 1);
            Assert.Equal(expandedReferenceSelectItem.Count(), 1);
            Assert.Equal(pathSelectItem.Count(), 2);
            NavigationPropertySegment navigationProperty = (NavigationPropertySegment)expandedNavigationSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.Equal(navigationProperty.NavigationProperty.Name, "MyDog");
            navigationProperty = (NavigationPropertySegment)expandedReferenceSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.Equal(navigationProperty.NavigationProperty.Name, "MyCat");

            //verify $orderby
            SingleValuePropertyAccessNode orderby = (SingleValuePropertyAccessNode)odataUri.OrderBy.Expression;
            Assert.Equal(orderby.Property.Name, "ID");

            //verify $top
            Assert.Equal(odataUri.Top, 1);

            //verify $skip
            Assert.Equal(odataUri.Skip, 2);

            //verify $count
            Assert.Equal(odataUri.QueryCount, true);

            //verify $search
            SearchTermNode searchTermNode = (SearchTermNode)odataUri.Search.Expression;
            Assert.Equal(searchTermNode.Text, "FA");

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(new Uri("http://www.example.com/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID%2CMyCat&$expand=MyDog%2CMyCat%2F%24ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA"), actualUri);

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(new Uri("http://www.example.com/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID%2CMyCat&$expand=MyDog%2CMyCat%2F%24ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA"), actualUri);
        }

        [Fact]
        public void TestODataUriBuilderWithKeySegment()
        {
            Uri fullUri = new Uri("http://www.example.com/People(1)?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
            ODataUriParser odataUriParser = new ODataUriParser(this.GetModel(), serviceRoot, fullUri);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            ODataUri odataUri = odataUriParser.ParseUri();

            //verify path
            EntitySetSegment entitySet = (EntitySetSegment)odataUri.Path.FirstSegment;
            KeySegment keySegment = (KeySegment)odataUri.Path.LastSegment;
            IEnumerable<KeyValuePair<string, object>> keyValuePairs = keySegment.Keys;
            Assert.Equal(odataUri.Path.Count, 2);
            Assert.Equal(entitySet.EntitySet.Name, "People");
            foreach (var keyValuePair in keyValuePairs)
            {
                Assert.Equal(keyValuePair.Key, "ID");
                Assert.Equal(keyValuePair.Value, 1);
            }

            //verify $filter
            BinaryOperatorNode binaryOperator = (BinaryOperatorNode)odataUri.Filter.Expression;
            SingleValuePropertyAccessNode singleValueProperty = (SingleValuePropertyAccessNode)binaryOperator.Left;
            SingleNavigationNode singleNavigation = (SingleNavigationNode)singleValueProperty.Source;
            ConstantNode constantNode = (ConstantNode)binaryOperator.Right;

            Assert.Equal(binaryOperator.OperatorKind, BinaryOperatorKind.Equal);
            Assert.Equal(singleValueProperty.Property.Name, "Color");
            Assert.Equal(singleNavigation.NavigationProperty.Name, "MyDog");
            Assert.Equal(constantNode.LiteralText, "'Brown'");

            //verify $select and $expand
            IEnumerable<SelectItem> selectItems = odataUri.SelectAndExpand.SelectedItems;
            IEnumerable<ExpandedNavigationSelectItem> expandedNavigationSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)).OfType<ExpandedNavigationSelectItem>();
            IEnumerable<ExpandedReferenceSelectItem> expandedReferenceSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)).OfType<ExpandedReferenceSelectItem>();
            IEnumerable<PathSelectItem> pathSelectItem = selectItems.Where(I => I.GetType() == typeof(PathSelectItem)).OfType<PathSelectItem>();
            Assert.Equal(expandedNavigationSelectItem.Count(), 1);
            Assert.Equal(expandedReferenceSelectItem.Count(), 1);
            Assert.Equal(pathSelectItem.Count(), 2);
            NavigationPropertySegment navigationProperty = (NavigationPropertySegment)expandedNavigationSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.Equal(navigationProperty.NavigationProperty.Name, "MyDog");
            navigationProperty = (NavigationPropertySegment)expandedReferenceSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.Equal(navigationProperty.NavigationProperty.Name, "MyCat");

            //verify $orderby
            SingleValuePropertyAccessNode orderby = (SingleValuePropertyAccessNode)odataUri.OrderBy.Expression;
            Assert.Equal(orderby.Property.Name, "ID");

            //verify $top
            Assert.Equal(odataUri.Top, 1);

            //verify $skip
            Assert.Equal(odataUri.Skip, 2);

            //verify $count
            Assert.Equal(odataUri.QueryCount, true);

            //verify $search
            SearchTermNode searchTermNode = (SearchTermNode)odataUri.Search.Expression;
            Assert.Equal(searchTermNode.Text, "FA");
            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(new Uri("http://www.example.com/People(1)?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID%2CMyCat&$expand=MyDog%2CMyCat%2F%24ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA"), actualUri);
        }

        [Fact]
        public void ODataUriBuilderWithKeyAsSegment()
        {
            Uri fullUri = new Uri("http://www.example.com/People/1?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$top=1&$skip=2&$count=false");
            ODataUriParser oDataUriParser = new ODataUriParser(this.GetModel(), serviceRoot, fullUri);
            oDataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash;
            SetODataUriParserSettingsTo(this.settings, oDataUriParser.Settings);
            ODataUri odataUri = oDataUriParser.ParseUri();

            //verify path
            EntitySetSegment entitySet = (EntitySetSegment)odataUri.Path.FirstSegment;
            KeySegment keySegment = (KeySegment)odataUri.Path.LastSegment;
            IEnumerable<KeyValuePair<string, object>> keyValuePairs = keySegment.Keys;
            Assert.Equal(odataUri.Path.Count, 2);
            Assert.Equal(entitySet.EntitySet.Name, "People");
            foreach (var keyValuePair in keyValuePairs)
            {
                Assert.Equal(keyValuePair.Key, "ID");
                Assert.Equal(keyValuePair.Value, 1);
            }

            //verify $filter
            BinaryOperatorNode binaryOperator = (BinaryOperatorNode)odataUri.Filter.Expression;
            SingleValuePropertyAccessNode singleValueProperty = (SingleValuePropertyAccessNode)binaryOperator.Left;
            SingleNavigationNode singleNavigation = (SingleNavigationNode)singleValueProperty.Source;
            ConstantNode constantNode = (ConstantNode)binaryOperator.Right;

            Assert.Equal(binaryOperator.OperatorKind, BinaryOperatorKind.Equal);
            Assert.Equal(singleValueProperty.Property.Name, "Color");
            Assert.Equal(singleNavigation.NavigationProperty.Name, "MyDog");
            Assert.Equal(constantNode.LiteralText, "'Brown'");

            //verify $select and $expand
            IEnumerable<SelectItem> selectItems = odataUri.SelectAndExpand.SelectedItems;
            IEnumerable<ExpandedNavigationSelectItem> expandedNavigationSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)).OfType<ExpandedNavigationSelectItem>();
            IEnumerable<ExpandedReferenceSelectItem> expandedReferenceSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)).OfType<ExpandedReferenceSelectItem>();
            IEnumerable<PathSelectItem> pathSelectItem = selectItems.Where(I => I.GetType() == typeof(PathSelectItem)).OfType<PathSelectItem>();
            Assert.Equal(expandedNavigationSelectItem.Count(), 1);
            Assert.Equal(expandedReferenceSelectItem.Count(), 1);
            Assert.Equal(pathSelectItem.Count(), 2);
            NavigationPropertySegment navigationProperty = (NavigationPropertySegment)expandedNavigationSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.Equal(navigationProperty.NavigationProperty.Name, "MyDog");
            navigationProperty = (NavigationPropertySegment)expandedReferenceSelectItem.First().PathToNavigationProperty.FirstSegment;
            Assert.Equal(navigationProperty.NavigationProperty.Name, "MyCat");

            //verify $top
            Assert.Equal(odataUri.Top, 1);

            //verify $skip
            Assert.Equal(odataUri.Skip, 2);

            //verify $count
            Assert.Equal(odataUri.QueryCount, false);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(new Uri("http://www.example.com/People/1?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID%2CMyCat&$expand=MyDog%2CMyCat%2F%24ref&$top=1&$skip=2&$count=false"), actualUri);
        }

        [Fact]
        public void RelativeUriBuildWithEmptyQueryOptions()
        {
            Uri queryUri = new Uri("People", UriKind.Relative);
            ODataUriParser odataUriParser = new ODataUriParser(this.GetModel(), queryUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(queryUri, actualUri);
        }

        [Fact]
        public void AbsoluteUriBuildWithEmptyQueryOptions()
        {
            Uri queryUri = new Uri("http://www.example.com/People");
            ODataUriParser odataUriParser = new ODataUriParser(this.GetModel(), serviceRoot, queryUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(queryUri, actualUri);
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
