//---------------------------------------------------------------------
// <copyright file="FilterAndOrderByBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core.UriParser;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriBuilder
{
    public class FilterAndOrderByBuilderTests : UriBuilderTestBase
    {
        #region test $filter
        [Fact]
        public void BuildFilterLongValuesWithOptionalSuffix()
        {
            // filter is a binaryOperatorNode and its right is a int value
            Uri queryUri = new Uri("People?$filter=ID eq 123", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID eq 123"), actualUri);

            // filter is a binaryOperatorNode and its right is a long value
            queryUri = new Uri("People?$filter=ID eq 123L", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID eq 123L"), actualUri);

            // // filter is a binaryOperatorNode and its right is NaN
            queryUri = new Uri("People?$filter=ID eq NaN", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID eq NaN"), actualUri);

            // filter is a binaryOperatorNode and its right is also a binaryOperatorNode
            queryUri = new Uri("People?$filter=ID add 123 eq 123", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID add 123 eq 123"), actualUri);

            // filter is a binaryOperatorNode and its right is also a binaryOperatorNode whose left value
            queryUri = new Uri("People?$filter=ID add 123L eq 123", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID add 123L eq 123"), actualUri);
        }

        [Fact]
        public void BuildFilterLongValuesNeedPromotion()
        {
            // $filter is a binaryOperatorNode and its right is Float value
            Uri queryUri = new Uri("People?$filter=ID eq 1F", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID eq 1F"), actualUri);

            // $filter is a binaryOperatorNode and its right is Double value
            queryUri = new Uri("People?$filter=ID eq 1D", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID eq 1D"), actualUri);

            // $filter is a binaryOperatorNode and its right is Decimal value
            queryUri = new Uri("People?$filter=ID eq 1M", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID eq 1M"), actualUri);
        }

        [Fact]
        public void BuildFilterINFNaNWithoutSuffix()
        {
            Uri queryUri = new Uri("People?$filter=ID gt INFF", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID gt INFF"), actualUri);

            queryUri = new Uri("People?$filter=ID add INFF eq INFF", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID add INFF eq INFF"), actualUri);

            queryUri = new Uri("People?$filter=ID gt -INFF", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID gt -INFF"), actualUri);

            queryUri = new Uri("People?$filter=ID eq NaNF", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=ID eq NaNF"), actualUri);

            queryUri = new Uri("Pet1Set?$filter=SingleID add INFF eq INFF", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet1Set?$filter=SingleID add INFF eq INFF"), actualUri);

            queryUri = new Uri("Pet1Set?$filter=DoubleID add INFF eq INFF", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet1Set?$filter=DoubleID add INFF eq INFF"), actualUri);
        }

        [Fact]
        public void ParseFilterWithBoolean()
        {
            Uri queryUri = new Uri("Pet5Set?$filter=ID eq true", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet5Set?$filter=ID eq true"), actualUri);

            queryUri = new Uri("Pet5Set?$filter=ID eq false", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet5Set?$filter=ID eq false"), actualUri);

            queryUri = new Uri("Pet5Set?$filter=ID eq not true", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet5Set?$filter=ID eq not true"), actualUri);

            queryUri = new Uri("Pet5Set?$filter=ID eq not false", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet5Set?$filter=ID eq not false"), actualUri);

            queryUri = new Uri("Pet5Set?$filter=ID and true eq false", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet5Set?$filter=ID and true eq false"), actualUri);
        }

        [Fact]
        public void BuildFilterNodeInComplexExpression()
        {
            Uri queryUri = new Uri("Pet1Set?$filter=(ID mul 1 add 1.01 sub 1.000000001) mul 2 ge 1", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet1Set?$filter=(ID mul 1 add 1.01 sub 1.000000001) mul 2 ge 1"), actualUri);

            queryUri = new Uri("Pet1Set?$filter=(ID add 1 mul 1.000000000000001 sub 1.000000001) mul 2 ge 1", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet1Set?$filter=(ID add 1 mul 1.000000000000001 sub 1.000000001) mul 2 ge 1"), actualUri);
        }

        [Fact]
        public void BuildFilterDoublePrecision()
        {
            Uri queryUri = new Uri("Pet1Set?$filter=DoubleID eq 1.0099999904632568", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet1Set?$filter=DoubleID eq 1.0099999904632568"), actualUri);
        }

        [Fact]
        public void BuildFilterSinglePrecision()
        {
            Uri queryUri = new Uri("Pet1Set?$filter=SingleID eq 1.0099999904632568", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet1Set?$filter=SingleID eq 1.0099999904632568"), actualUri);
        }

        [Fact]
        public void ParseFilterWithEntitySetShouldBeAbleToDetermineSets()
        {
            Uri queryUri = new Uri("People?$filter=MyDog%2FColor%20eq%20%27Brown%27", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=MyDog%2FColor%20eq%20%27Brown%27"), actualUri);
        }

        [Fact]
        public void BuildFilterWithReplaceShouldWork()
        {
            Uri queryUri = new Uri("People?$filter=replace(Name%2C%20%27a%27%2C%20%27e%27)%20eq%20%27endrew%27", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=replace(Name%2C%27a%27%2C%27e%27)%20eq%20%27endrew%27"), actualUri);
        }

        [Fact]
        public void BuildFilterWithHasShouldWork()
        {
            Uri queryUri = new Uri("Pet2Set?$filter=PetColorPattern has Fully.Qualified.Namespace.ColorPattern'SolidYellow' eq true", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet2Set?$filter=PetColorPattern has Fully.Qualified.Namespace.ColorPattern'SolidYellow' eq true"), actualUri);
        }

        [Fact]
        public void MultipleDeepLevelExpansionsAndSelectionsShouldWork()
        {
            Uri queryUri = new Uri("People?$filter=replace(Name%2C%20%27a%27%2C%20%27e%27)%20eq%20%27endrew%27", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=replace(Name%2C%27a%27%2C%27e%27)%20eq%20%27endrew%27"), actualUri);
        }

        [Fact]
        public void BuildFilterWithAnyOnStringCollectionProperty()
        {
            Uri queryUri = new Uri("People?$filter=MyAddress%2fMyNeighbors%2fany()", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=MyAddress%2FMyNeighbors%2Fany()"), actualUri);
        }

        [Fact]
        public void BuildFilterWithDuration()
        {
            Uri queryUri = new Uri("People?$filter=duration'PT0H0M15S' eq duration'P1DT0H0M30S'", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=duration'PT0H0M15S' eq duration'P1DT0H0M30S'"), actualUri);
        }

        [Fact]
        public void NullValueInCanonicalFunction()
        {
            Uri queryUri = new Uri("People?$filter=day(null)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=day(null)"), actualUri);
        }

        [Fact]
        public void BuildFilterWithIsOfFunctionWorksWithSingleQuotesOnType()
        {
            Uri queryUri = new Uri("People?$filter=isof(Shoe%2C%20%27Edm.String%27)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=isof(Shoe%2C%27Edm.String%27)"), actualUri);
        }

        [Fact]
        public void BuildFilterWithCastFunctionWithNoSingleQuotesOnType()
        {
            Uri queryUri = new Uri("People?$filter=cast(Shoe%2C%20Edm.String)%20eq%20%27blue%27", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=cast(Shoe%2CEdm.String)%20eq%20%27blue%27"), actualUri);
        }

        [Fact]
        public void BuildFilterWithCastFunctionWorksForEnum()
        {
            Uri queryUri = new Uri("People?$filter=cast(Shoe%2C%20Fully.Qualified.Namespace.ColorPattern)%20eq%20Fully.Qualified.Namespace.ColorPattern%27blue%27", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=cast(Shoe%2CFully.Qualified.Namespace.ColorPattern)%20eq%20Fully.Qualified.Namespace.ColorPattern%27blue%27"), actualUri);
        }

        [Fact]
        public void CastFunctionWorksForCastFromNullToEnum()
        {
            Uri queryUri = new Uri("People?$filter=cast(null%2C%20Fully.Qualified.Namespace.ColorPattern)%20eq%20Fully.Qualified.Namespace.ColorPattern%27blue%27", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=cast(null%2CFully.Qualified.Namespace.ColorPattern)%20eq%20Fully.Qualified.Namespace.ColorPattern%27blue%27"), actualUri);
        }

        [Fact]
        public void CastFunctionProducesAnEntityType()
        {
            Uri queryUri = new Uri("People?$filter=cast(MyDog%2C%20%27Fully.Qualified.Namespace.Dog%27)%2FColor%20eq%20%27blue%27", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=cast(MyDog%2C%27Fully.Qualified.Namespace.Dog%27)%2FColor%20eq%20%27blue%27"), actualUri);
        }

        [Fact]
        public void BuildFilterWithOpenPropertyInsideAny()
        {
            Uri queryUri = new Uri("People?$filter=MyPaintings%2Fany(p%3Ap%2FOpenProperty)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=MyPaintings%2Fany(p%3Ap%2FOpenProperty)"), actualUri);
        }

        [Fact]
        public void BuildFilterWithControlCharactersShouldBeIgnored()
        {
            Uri queryUri = new Uri("People?$filter=length(Name) \neq 30", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=length(Name) eq 30"), actualUri);

            queryUri = new Uri("People?$filter=length(Name) \req 30", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=length(Name) eq 30"), actualUri);
        }

        [Fact]
        public void ActionIsTreatedAsOpenProperty()
        {
            Uri queryUri = new Uri("Paintings?$filter=Restore", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Paintings?$filter=Restore"), actualUri);
        }

        [Fact]
        public void BuildFilterWithLongFunctionChain()
        {
            Uri queryUri = new Uri("People?$filter=Fully.Qualified.Namespace.AllMyFriendsDogs()%2FFully.Qualified.Namespace.OwnerOfFastestDog()%2FMyDog%2FMyPeople%2FFully.Qualified.Namespace.AllHaveDog(inOffice%3Dtrue)%20eq%20true", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.AllMyFriendsDogs()%2FFully.Qualified.Namespace.OwnerOfFastestDog()%2FMyDog%2FMyPeople%2FFully.Qualified.Namespace.AllHaveDog(inOffice%3Dtrue)%20eq%20true"), actualUri);
        }

        [Fact]
        public void FunctionWithoutBindingParameterShouldWorkInFilter()
        {
            Uri queryUri = new Uri("People?$filter=Fully.Qualified.Namespace.FindMyOwner(dogsName%3D%27fido%27)%2FName%20eq%20%27Bob%27", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.FindMyOwner(dogsName%3D%27fido%27)%2FName%20eq%20%27Bob%27"), actualUri);
        }

        [Fact]
        public void BuildFilterWithBinaryOperatorBetweenUInt16AndPrimitive()
        {
            Uri queryUri = new Uri("People?$filter=FavoriteNumber eq " + UInt16.MaxValue, UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=FavoriteNumber eq " + UInt16.MaxValue), actualUri);
        }

        [Fact]
        public void BuildFilterWithBinaryOperatorBetweenUInt32AndPrimitive()
        {
            Uri queryUri = new Uri("People?$filter=FavoriteNumber eq " + UInt32.MaxValue, UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=FavoriteNumber eq " + UInt32.MaxValue), actualUri);
        }

        [Fact]
        public void BuildFilterWithBinaryOperatorBetweenUInt64AndPrimitive()
        {
            Uri queryUri = new Uri("People?$filter=LifeTime ne " + UInt64.MaxValue, UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=LifeTime ne " + UInt64.MaxValue), actualUri);
        }

        [Fact]
        public void BuildFilterWithBinaryOperatorBetweenPrimitiveAndUInt()
        {
            Uri queryUri = new Uri("People?$filter=123 ne StockQuantity", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=123 ne StockQuantity"), actualUri);
        }

        [Fact]
        public void BuildFilterWithBinaryOperatorBetweenUInts()
        {
            Uri queryUri = new Uri("People?$filter=FavoriteNumber eq StockQuantity", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=FavoriteNumber eq StockQuantity"), actualUri);
        }

        [Fact]
        public void BuildFilterWithBinaryOperatorAddBetweenPrimitiveAndUInt()
        {
            Uri queryUri = new Uri("People?$filter=123 add StockQuantity eq 1", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=123 add StockQuantity eq 1"), actualUri);
        }

        [Fact]
        public void BuildFilterWithBinaryOperatorAddBetweenUInts()
        {
            Uri queryUri = new Uri("People?$filter=FavoriteNumber add StockQuantity eq 1", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=FavoriteNumber add StockQuantity eq 1"), actualUri);
        }

        [Fact]
        public void BuildFilterWithDate()
        {
            Uri queryUri = new Uri("People?$filter=MyDate%20gt%201997-02-04", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Uri expectedUri = new Uri("http://gobbledygook/People?$filter=MyDate%20gt%201997-02-04");
            Assert.Equal(expectedUri, actualUri);
        }

        [Fact]
        public void BuildFilterWithDatetimeOffset()
        {
            Uri queryUri = new Uri("People?$filter=Birthdate%20gt%201997-02-04%2B11%3A00", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Uri expectedUri = new Uri("http://gobbledygook/People?$filter=Birthdate%20gt%201997-02-04%2B11%3A00");
            Assert.Equal(expectedUri, actualUri);
        }

        [Fact]
        public void BuildFilterWithTimeOfDay()
        {
            Uri queryUri = new Uri("People?$filter=MyTimeOfDay%20gt%2019%3A4%3A50.123", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Uri expectedUri = new Uri("http://gobbledygook/People?$filter=MyTimeOfDay%20gt%2019%3A4%3A50.123");
            Assert.Equal(expectedUri, actualUri);
        }

        [Fact]
        public void BuildFilterOnTypeDefinitionProperty()
        {
            Uri queryUri = new Uri("People?$filter=FirstName ne 'Bob'", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=FirstName ne 'Bob'"), actualUri);
        }

        [Fact]
        public void BuildFilterWithAnyHasNoParameters()
        {
            Uri queryUri = new Uri("People?$filter=MyAddress/MyNeighbors/any()", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=MyAddress%2FMyNeighbors%2Fany()"), actualUri);
        }

        [Fact]
        public void BuildFilterWithAnyHasConstBoolValueParameters()
        {
            Uri queryUri = new Uri("People?$filter=MyPaintings%2Fany(a%3A%20true)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=MyPaintings%2Fany(a%3Atrue)"), actualUri);
        }

        [Fact]
        public void BuildFilterWithAnyHasParameters()
        {
            Uri queryUri = new Uri("People?$filter=MyPaintings/any(a:a/OpenProperty)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=MyPaintings%2Fany(a%3Aa%2FOpenProperty)"), actualUri);
        }

        [Fact]
        public void BuildFilterWithAnyHasExpressionParameters()
        {
            Uri queryUri = new Uri("People?$filter=MyPaintings/any(a:a/OpenProperty eq 1)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$filter=MyPaintings%2Fany(a%3Aa%2FOpenProperty%20eq%201)"), actualUri);
        }
        #endregion

        #region test $orderby
        [Fact]
        public void BuildOrderByWithEntitySetShouldBeAbleToDetermineSets()
        {
            Uri queryUri = new Uri("People?$orderby=MyDog%2FColor", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=MyDog%2FColor"), actualUri);
        }

        [Fact]
        public void BuildOrderByWithContainedEntitySetShouldBeAbleToDetermineSets()
        {
            Uri queryUri = new Uri("People?$orderby=MyContainedDog%2FColor", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=MyContainedDog%2FColor"), actualUri);
        }

        [Fact]
        public void BuildMultipleOrderBys()
        {
            Uri queryUri = new Uri("People?$orderby=Name%20asc%2C%20Shoe%20desc", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=Name%2CShoe%20desc"), actualUri);
        }

        [Fact]
        public void BuildEnumMultipleOrderBys()
        {
            Uri queryUri = new Uri("Pet2Set?$orderby=PetColorPattern%20asc%2C%20cast(PetColorPattern%2C%27Edm.String%27)%20desc%2C%20PetColorPattern%20has%20Fully.Qualified.Namespace.ColorPattern%27SolidYellow%27%20asc", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet2Set?$orderby=PetColorPattern%2Ccast(PetColorPattern%2C%27Edm.String%27)%20desc%2CPetColorPattern%20has%20Fully.Qualified.Namespace.ColorPattern%27SolidYellow%27"), actualUri);
        }

        [Fact]
        public void BuildEnumPropertyOrderBy()
        {
            Uri queryUri = new Uri("Pet2Set?$orderby=PetColorPattern asc", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet2Set?$orderby=PetColorPattern"), actualUri);
        }

        [Fact]
        public void ParseEnumConstantOrderBy()
        {
            Uri queryUri = new Uri("Pet2Set?$orderby=Fully.Qualified.Namespace.ColorPattern'SolidYellow' asc", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Pet2Set?$orderby=Fully.Qualified.Namespace.ColorPattern'SolidYellow'"), actualUri);
        }

        [Fact]
        public void BuildOrderbyWithDate()
        {
            Uri queryUri = new Uri("People?$orderby=MyDate", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=MyDate"), actualUri);
        }

        [Fact]
        public void BuildOrderbyWithTimeOfDay()
        {
            Uri queryUri = new Uri("People?$orderby=MyTimeOfDay", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=MyTimeOfDay"), actualUri);
        }

        [Fact]
        public void BuildOrderbyWithDatetimeOffset()
        {
            Uri queryUri = new Uri("People?$orderby=Birthdate", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=Birthdate"), actualUri);
        }

        [Fact]
        public void BuildOrderByWithExpression()
        {
            Uri queryUri = new Uri("People?$orderby=Shoe eq 'blue'", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=Shoe eq 'blue'"), actualUri);
        }

        [Fact]
        public void ActionsSucceedOnOpenTypeInOrderby()
        {
            Uri queryUri = new Uri("Paintings?$orderby=Restore asc", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Paintings?$orderby=Restore"), actualUri);
        }

        [Fact]
        public void FunctionCallWithGeometryAndNullParameterValuesShouldWorkInOrderBy()
        {
            Uri queryUri = new Uri("Paintings?$orderby=Fully.Qualified.Namespace.GetColorAtPosition(position%3Dgeometry%27SRID%3D0%3BPOINT(1%202)%27%2CincludeAlpha%3Dnull)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/Paintings?$orderby=Fully.Qualified.Namespace.GetColorAtPosition(position%3Dgeometry%27SRID%3D0%3BPOINT(1%202)%27%2CincludeAlpha%3Dnull)"), actualUri);
        }
        [Fact]
        public void OrderByTypeDefinitionProperty()
        {
            Uri queryUri = new Uri("People?$orderby=FirstName", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=FirstName"), actualUri);
        }

        #endregion
    }
}
