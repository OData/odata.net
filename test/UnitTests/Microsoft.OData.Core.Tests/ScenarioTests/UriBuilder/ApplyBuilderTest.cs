//---------------------------------------------------------------------
// <copyright file="ApplyBuilderTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests.ScenarioTests.UriBuilder;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriBuilder
{
    public class ApplyBuilderTest : UriBuilderTestBase
    {
        [Theory]
        [InlineData("http://gobbledygook/People?$apply=filter(Shoe eq 'blue')/groupby((FirstName,MyDog/LionWhoAteMe/AngerLevel),aggregate(LifeTime with average as avgLifeTime,FavoriteNumber with sum as sumFavoriteNumber,MyDog/LionWhoAteMe/LionHeartbeat with max as maxLionHeartbeat))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((FirstName),aggregate(LifeTime with average as avgLifeTime))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((FirstName),aggregate(LifeTime with countdistinct as cntdistinctLifeTime))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((FirstName),aggregate(LifeTime with max as maxLifeTime))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((FirstName),aggregate(LifeTime with min as minLifeTime))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((FirstName),aggregate(LifeTime with sum as sumLifeTime))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((FirstName),aggregate($count as cnt))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((FirstName),aggregate(LifeTime with Custom.Aggregate as custLifeTime))")]
        [InlineData("http://gobbledygook/People?$apply=compute((cast(LifeTime,'Edm.Double') add MyDog/LionWhoAteMe/AngerLevel) mul 2 as lifeAngerLevel,StockQuantity div FavoriteNumber as StockNumber)")]
        [InlineData("http://gobbledygook/People?$apply=groupby((MyDog/Color,MyDog/Breed))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((MyDog/FastestOwner/FirstName,MyDog/LionWhoAteMe/LionHeartbeat/Frequency,MyFavoritePainting/ArtistAddress/City))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((MyAddress/City,MyAddress/Street))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((MyAddress/NextHome/City,MyAddress/NextHome/PostBoxPainting/Artist,MyAddress/PostBoxPainting/Artist))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((MyAddress/PostBoxPainting/Owner/MyAddress/City,MyFavoritePainting/ArtistAddress/NextHome/PostBoxPainting/Artist))")]
        [InlineData("http://gobbledygook/People?$apply=groupby((FirstName),aggregate(MyPaintings($count as cnt)))")]
        [InlineData("http://gobbledygook/People?$apply=expand(MyPaintings, filter(FrameColor eq 'Red'))/groupby((LifeTime),aggregate(MyPaintings($count as Count)))")]
        [InlineData("http://gobbledygook/People?$apply=expand(MyPaintings, filter(FrameColor eq 'Red'), expand(Owner, filter(FirstName eq 'Me')))/groupby((LifeTime),aggregate(MyPaintings($count as Count)))")]
        public void BuildUrlWithApply(string query)
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(query));
            ODataUri odataUri = parser.ParseUri();

            Uri result = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(query, Uri.UnescapeDataString(result.OriginalString));
        }

        [Fact]
        public void BuildUrlWithApplyAggregateOnCollectionProperty()
        {
            string customFunctionName = "NS.UnionDate";
            string query = $"http://gobbledygook/People?$apply=aggregate(MyDates with {customFunctionName} as UnionDate)";

            try
            {
                var argument = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDate(/*isNullable*/false));
                var existingCustomFunctionSignature = new FunctionSignatureWithReturnType(argument, argument);
                CustomUriFunctions.AddCustomUriFunction(customFunctionName, existingCustomFunctionSignature);

                var parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(query));
                ODataUri odataUri = parser.ParseUri();

                Uri result = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
                Assert.Equal(query, Uri.UnescapeDataString(result.OriginalString));
            }
            finally
            {
                CustomUriFunctions.RemoveCustomUriFunction(customFunctionName);
            }
        }
    }
}
