//---------------------------------------------------------------------
// <copyright file="SRResourcesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Microsoft.Spatial.Tests.Resources;

public class SRResourcesTests
{
    [Theory]
    [InlineData("GeoJsonReader_ExpectedArray", new object[] { })]
    [InlineData("GeoJsonReader_ExpectedNumeric", new object[] { })]
    [InlineData("GeoJsonReader_InvalidCrsName", new object[] { "Value" })]
    [InlineData("GeoJsonReader_InvalidCrsType", new object[] { "Value" })]
    [InlineData("GeoJsonReader_InvalidNullElement", new object[] { })]
    [InlineData("GeoJsonReader_InvalidPosition", new object[] { })]
    [InlineData("GeoJsonReader_InvalidTypeName", new object[] { "Value" })]
    [InlineData("GeoJsonReader_MissingRequiredMember", new object[] { "Value" })]
    [InlineData("GmlReader_EmptyRingsNotAllowed", new object[] { })]
    [InlineData("GmlReader_ExpectReaderAtElement", new object[] { })]
    [InlineData("GmlReader_InvalidAttribute", new object[] { "Value1", "Value2" })]
    [InlineData("GmlReader_InvalidSpatialType", new object[] { "Value" })]
    [InlineData("GmlReader_InvalidSrsDimension", new object[] { })]
    [InlineData("GmlReader_InvalidSrsName", new object[] { "Value" })]
    [InlineData("GmlReader_PosListNeedsEvenCount", new object[] { })]
    [InlineData("GmlReader_PosNeedTwoNumbers", new object[] { })]
    [InlineData("GmlReader_UnexpectedElement", new object[] { "Value" })]
    [InlineData("InvalidPointCoordinate", new object[] { "Value1", "Value2" })]
    [InlineData("JsonReaderExtensions_CannotReadPropertyValueAsString", new object[] { "Value1", "Value2" })]
    [InlineData("JsonReaderExtensions_CannotReadValueAsJsonObject", new object[] { "Value" })]
    [InlineData("PlatformHelper_DateTimeOffsetMustContainTimeZone", new object[] { "Value" })]
    [InlineData("Point_AccessCoordinateWhenEmpty", new object[] { })]
    [InlineData("SpatialBuilder_CannotCreateBeforeDrawn", new object[] { })]
    [InlineData("SpatialImplementation_NoRegisteredOperations", new object[] { })]
    [InlineData("Validator_FullGlobeCannotHaveElements", new object[] { })]
    [InlineData("Validator_FullGlobeInCollection", new object[] { })]
    [InlineData("Validator_InvalidLatitudeCoordinate", new object[] { "Value" })]
    [InlineData("Validator_InvalidLongitudeCoordinate", new object[] { "Value" })]
    [InlineData("Validator_InvalidPointCoordinate", new object[] { "Value1", "Value2", "Value3", "Value4" })]
    [InlineData("Validator_InvalidPolygonPoints", new object[] { })]
    [InlineData("Validator_InvalidType", new object[] { "Value" })]
    [InlineData("Validator_LineStringNeedsTwoPoints", new object[] { })]
    [InlineData("Validator_NestingOverflow", new object[] { "Value" })]
    [InlineData("Validator_SridMismatch", new object[] { })]
    [InlineData("Validator_UnexpectedCall", new object[] { "Value1", "Value2" })]
    [InlineData("Validator_UnexpectedCall2", new object[] { "Value1", "Value2", "Value3" })]
    [InlineData("Validator_UnexpectedGeography", new object[] { })]
    [InlineData("Validator_UnexpectedGeometry", new object[] { })]
    [InlineData("WellKnownText_TooManyDimensions", new object[] { })]
    [InlineData("WellKnownText_UnexpectedCharacter", new object[] { "Value" })]
    [InlineData("WellKnownText_UnexpectedToken", new object[] { "Value1", "Value2", "Value3" })]
    [InlineData("WellKnownText_UnknownTaggedText", new object[] { "Value" })]

    public void ResourceKey_ShouldFormatWithoutException(string resourceKey, object[] formatArgs)
    {
        // Arrange
        var resourceValue = SRResources.ResourceManager.GetString(resourceKey, SRResources.Culture);
        Assert.NotNull(resourceValue); // Ensure the resource exists

        // Act & Assert
        var exception = Record.Exception(() => Error.Format(resourceValue, formatArgs));
        Assert.Null(exception); // Ensure no Exception is thrown

        // Match numbers of arguments required in the resource string and validate against formatArgs
        var matches = Regex.Matches(resourceValue, @"\{\d+\}").Select(m => m.Value).Distinct();
        Assert.Equal(matches.Count(), formatArgs.Length);

        var formattedValue = Error.Format(resourceValue, formatArgs);
        Assert.NotNull(formattedValue);
    }
}
