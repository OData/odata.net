//---------------------------------------------------------------------
// <copyright file="UriBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Core.E2E.Tests.UriBuilderTests;

public class UriBuilderTests
{
    private readonly Uri _baseUri = new Uri("http://localhost/");
    private readonly ODataUriParserSettings _settings = new ODataUriParserSettings();

    [Fact]
    public void ODataUriBuilderWithEntitySet_VerifyPath()
    {
        // Arrange
        var fullUri = new Uri("http://localhost/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);

        odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        EntitySetSegment entitySet = (EntitySetSegment)odataUri.Path.FirstSegment;
        Assert.Equal("People", entitySet.EntitySet.Name);
        Assert.Single(odataUri.Path);
        Assert.Equal("NS.Person", odataUri.Path[0].TargetEdmType.FullTypeName());
    }

    [Fact]
    public void ODataUriBuilderWithEntitySet_VerifyFilter()
    {
        // Arrange
        var fullUri = new Uri("http://localhost/People?$filter=MyDog%2FColor%20eq%20%27Brown%27");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);

        odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        var binaryOperator = (BinaryOperatorNode)odataUri.Filter.Expression;
        var singleValueProperty = (SingleValuePropertyAccessNode)binaryOperator.Left;
        var singleNavigation = (SingleNavigationNode)singleValueProperty.Source;
        var constantNode = (ConstantNode)binaryOperator.Right;

        // Assert
        Assert.Equal(BinaryOperatorKind.Equal, binaryOperator.OperatorKind);
        Assert.Equal("Color", singleValueProperty.Property.Name);
        Assert.Equal("MyDog", singleNavigation.NavigationProperty.Name);
        Assert.Equal("'Brown'", constantNode.LiteralText);
    }

    [Fact]
    public void ODataUriBuilderWithEntitySet_VerifySelectAndExpand()
    {
        // Arrange
        var fullUri = new Uri("http://localhost/People?$select=ID&$expand=MyDog%2CMyCat/$ref");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);

        odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        var selectItems = odataUri.SelectAndExpand.SelectedItems;
        var expandedNavigationSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)).OfType<ExpandedNavigationSelectItem>();
        var expandedReferenceSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)).OfType<ExpandedReferenceSelectItem>();
        var pathSelectItem = selectItems.Where(I => I.GetType() == typeof(PathSelectItem)).OfType<PathSelectItem>();

        // Assert
        Assert.Single(expandedNavigationSelectItem);
        Assert.Single(expandedReferenceSelectItem);
        Assert.Equal(2, pathSelectItem.Count());

        var navigationProperty = (NavigationPropertySegment)expandedNavigationSelectItem.First().PathToNavigationProperty.FirstSegment;
        Assert.Equal("MyDog", navigationProperty.NavigationProperty.Name);

        navigationProperty = (NavigationPropertySegment)expandedReferenceSelectItem.First().PathToNavigationProperty.FirstSegment;
        Assert.Equal("MyCat", navigationProperty.NavigationProperty.Name);
    }

    [Fact]
    public void ODataUriBuilderWithEntitySet_VerifyOrderBy()
    {
        // Arrange
        var fullUri = new Uri("http://localhost/People?$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);

        odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        var orderby = (SingleValuePropertyAccessNode)odataUri.OrderBy.Expression;
        Assert.Equal("ID", orderby.Property.Name);
    }

    [Fact]
    public void ODataUriBuilderWithEntitySet_VerifyTop()
    {
        // Arrange
        var fullUri = new Uri("http://localhost/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        Assert.Equal(odataUri.Top, 1);
    }

    [Fact]
    public void ODataUriBuilderWithEntitySet_VerifySkip()
    {
        // Arrange
        var fullUri = new Uri("http://localhost/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        Assert.Equal(odataUri.Skip, 2);
    }

    [Fact]
    public void ODataUriBuilderWithEntitySet_VerifyCount()
    {
        // Arrange
        var fullUri = new Uri("http://localhost/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        Assert.Equal(odataUri.QueryCount, true);
    }

    [Fact]
    public void ODataUriBuilderWithEntitySet_VerifySearch()
    {
        // Arrange
        var fullUri = new Uri("http://localhost/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        var searchTermNode = (SearchTermNode)odataUri.Search.Expression;
        Assert.Equal("FA", searchTermNode.Text);
    }

    [Fact]
    public void ODataUriBuilderWithEntitySet_VerifyBuiltUri()
    {
        // Arrange
        var fullUri = new Uri("http://localhost/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        var actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
        Assert.Equal(new Uri("http://localhost/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID%2CMyCat&$expand=MyDog%2CMyCat%2F%24ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA"), actualUri);

        actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
        Assert.Equal(new Uri("http://localhost/People?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID%2CMyCat&$expand=MyDog%2CMyCat%2F%24ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA"), actualUri);
    }

    [Theory]
    [InlineData("http://localhost/People(1)", "Parentheses")]
    [InlineData("http://localhost/People/1", "Slash")]
    public void ODataUriBuilderWithKeySegment_VerifyPath(string baseUrlPart, string urlKeyDelimiter)
    {
        // Arrange
        var fullUri = new Uri($"{baseUrlPart}?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = urlKeyDelimiter == "Slash" ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses;

        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        var entitySet = (EntitySetSegment)odataUri.Path.FirstSegment;
        var keySegment = (KeySegment)odataUri.Path.LastSegment;
        IEnumerable<KeyValuePair<string, object>> keyValuePairs = keySegment.Keys;

        // Assert
        Assert.Equal(2, odataUri.Path.Count);
        Assert.Equal("People", entitySet.EntitySet.Name);
        Assert.All(keySegment.Keys, kvp =>
        {
            Assert.Equal("ID", kvp.Key);
            Assert.Equal(1, kvp.Value);
        });
    }

    [Theory]
    [InlineData("http://localhost/People(1)", "Parentheses")]
    [InlineData("http://localhost/People/1", "Slash")]
    public void ODataUriBuilderForEntityWithKeySegment_VerifyFilter(string baseUrlPart, string urlKeyDelimiter)
    {
        // Arrange
        var fullUri = new Uri($"{baseUrlPart}?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = urlKeyDelimiter == "Slash" ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses;

        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        var binaryOperator = (BinaryOperatorNode)odataUri.Filter.Expression;
        var singleValueProperty = (SingleValuePropertyAccessNode)binaryOperator.Left;
        var singleNavigation = (SingleNavigationNode)singleValueProperty.Source;
        var constantNode = (ConstantNode)binaryOperator.Right;

        // Assert
        Assert.Equal(BinaryOperatorKind.Equal, binaryOperator.OperatorKind);
        Assert.Equal("Color", singleValueProperty.Property.Name);
        Assert.Equal("MyDog", singleNavigation.NavigationProperty.Name);
        Assert.Equal("'Brown'", constantNode.LiteralText);
    }

    [Theory]
    [InlineData("http://localhost/People(1)", "Parentheses")]
    [InlineData("http://localhost/People/1", "Slash")]
    public void ODataUriBuilderForEntityWithKeySegment_VerifySelectAndExpand(string baseUrlPart, string urlKeyDelimiter)
    {
        // Arrange
        var fullUri = new Uri($"{baseUrlPart}?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = urlKeyDelimiter == "Slash" ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses;

        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        IEnumerable<SelectItem> selectItems = odataUri.SelectAndExpand.SelectedItems;
        IEnumerable<ExpandedNavigationSelectItem> expandedNavigationSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)).OfType<ExpandedNavigationSelectItem>();
        IEnumerable<ExpandedReferenceSelectItem> expandedReferenceSelectItem = selectItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)).OfType<ExpandedReferenceSelectItem>();
        IEnumerable<PathSelectItem> pathSelectItem = selectItems.Where(I => I.GetType() == typeof(PathSelectItem)).OfType<PathSelectItem>();

        // Assert
        Assert.Single(expandedNavigationSelectItem);
        Assert.Single(expandedReferenceSelectItem);
        Assert.Equal(2, pathSelectItem.Count());

        var navigationProperty = (NavigationPropertySegment)expandedNavigationSelectItem.First().PathToNavigationProperty.FirstSegment;
        Assert.Equal("MyDog", navigationProperty.NavigationProperty.Name);

        navigationProperty = (NavigationPropertySegment)expandedReferenceSelectItem.First().PathToNavigationProperty.FirstSegment;
        Assert.Equal("MyCat", navigationProperty.NavigationProperty.Name);
    }

    [Theory]
    [InlineData("http://localhost/People(1)", "Parentheses")]
    [InlineData("http://localhost/People/1", "Slash")]
    public void ODataUriBuilderForEntityWithKeySegment_VerifyOrderBy(string baseUrlPart, string urlKeyDelimiter)
    {
        // Arrange
        var fullUri = new Uri($"{baseUrlPart}?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = urlKeyDelimiter == "Slash" ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses;

        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        var orderby = (SingleValuePropertyAccessNode)odataUri.OrderBy.Expression;

        // Assert
        Assert.Equal("ID", orderby.Property.Name);
    }

    [Theory]
    [InlineData("http://localhost/People(1)", "Parentheses")]
    [InlineData("http://localhost/People/1", "Slash")]
    public void ODataUriBuilderForEntityWithKeySegment_VerifyTop(string baseUrlPart, string urlKeyDelimiter)
    {
        // Arrange
        var fullUri = new Uri($"{baseUrlPart}?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = urlKeyDelimiter == "Slash" ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses;

        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        Assert.Equal(odataUri.Top, 1);
    }

    [Theory]
    [InlineData("http://localhost/People(1)", "Parentheses")]
    [InlineData("http://localhost/People/1", "Slash")]
    public void ODataUriBuilderForEntityWithKeySegment_VerifySkip(string baseUrlPart, string urlKeyDelimiter)
    {
        // Arrange
        var fullUri = new Uri($"{baseUrlPart}?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = urlKeyDelimiter == "Slash" ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses;

        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        Assert.Equal(odataUri.Skip, 2);
    }

    [Theory]
    [InlineData("http://localhost/People(1)", "Parentheses")]
    [InlineData("http://localhost/People/1", "Slash")]
    public void ODataUriBuilderForEntityWithKeySegment_VerifyCount(string baseUrlPart, string urlKeyDelimiter)
    {
        // Arrange
        var fullUri = new Uri($"{baseUrlPart}?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = urlKeyDelimiter == "Slash" ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses;

        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        Assert.Equal(odataUri.QueryCount, true);
    }

    [Theory]
    [InlineData("http://localhost/People(1)", "Parentheses")]
    [InlineData("http://localhost/People/1", "Slash")]
    public void ODataUriBuilderForEntityWithKeySegment_VerifySearch(string baseUrlPart, string urlKeyDelimiter)
    {
        // Arrange
        var fullUri = new Uri($"{baseUrlPart}?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = urlKeyDelimiter == "Slash" ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses;

        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        var searchTermNode = (SearchTermNode)odataUri.Search.Expression;
        Assert.Equal("FA", searchTermNode.Text);
    }

    [Theory]
    [InlineData("http://localhost/People(1)", "Parentheses")]
    [InlineData("http://localhost/People/1", "Slash")]
    public void ODataUriBuilderForEntityWithKeySegment_VerifyBuildUri(string baseUrlPart, string urlKeyDelimiter)
    {
        // Arrange
        var fullUri = new Uri($"{baseUrlPart}?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID&$expand=MyDog%2CMyCat/$ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, fullUri);
        odataUriParser.UrlKeyDelimiter = urlKeyDelimiter == "Slash" ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses;

        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        // Assert
        var actualUri = odataUri.BuildUri(odataUriParser.UrlKeyDelimiter);
        Assert.Equal(new Uri($"{baseUrlPart}?$filter=MyDog%2FColor%20eq%20%27Brown%27&$select=ID%2CMyCat&$expand=MyDog%2CMyCat%2F%24ref&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA"), actualUri);
    }

    [Fact]
    public void RelativeUriBuildWithEmptyQueryOptions()
    {
        // Arrange
        var queryUri = new Uri("People", UriKind.Relative);
        var odataUriParser = new ODataUriParser(this.GetModel(), queryUri);
        odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        var actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);

        // Assert
        Assert.Equal(queryUri, actualUri);
    }

    [Fact]
    public void AbsoluteUriBuildWithEmptyQueryOptions()
    {
        var queryUri = new Uri("http://localhost/People");
        var odataUriParser = new ODataUriParser(this.GetModel(), _baseUri, queryUri);
        odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
        odataUriParser.Settings.MaximumExpansionCount = this._settings.MaximumExpansionCount;
        odataUriParser.Settings.MaximumExpansionDepth = this._settings.MaximumExpansionDepth;

        // Act
        ODataUri odataUri = odataUriParser.ParseUri();

        var actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);

        // Assert
        Assert.Equal(queryUri, actualUri);
    }

    #region Private

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

    #endregion
}
