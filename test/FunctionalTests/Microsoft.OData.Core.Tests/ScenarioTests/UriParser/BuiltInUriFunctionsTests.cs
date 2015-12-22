using Microsoft.OData.Core.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Tests the BuiltInUriFunctions class.
    /// </summary>
    public class BuiltInUriFunctionsTests
    {
        [Fact]
        public void GetBuiltInFunctionSignaturesSubstring()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("substring", out builtInFunctionsSignature);

            builtInFunctionsSignature.Length.Should().Be(6);
        }

        [Fact]
        public void GetBuiltInFunctionSignaturesDateTimeOffsetYear()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("year", out builtInFunctionsSignature);

            builtInFunctionsSignature.Length.Should().Be(4); // DateTimeOffset and Date, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignaturesDateTimeOffsetMonth()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("month", out builtInFunctionsSignature);

            builtInFunctionsSignature.Length.Should().Be(4); // DateTimeOffset and Date, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignaturesDateTimeOffsetDay()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("day", out builtInFunctionsSignature);

            builtInFunctionsSignature.Length.Should().Be(4); // DateTimeOffset and Date, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignatureHour()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("hour", out builtInFunctionsSignature);

            builtInFunctionsSignature.Length.Should().Be(6); // DateTimeOffset, Duration and TimeOfDay, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignatureMinute()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("minute", out builtInFunctionsSignature);

            builtInFunctionsSignature.Length.Should().Be(6); // DateTimeOffset, Duration and TimeOfDay, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignatureSecond()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("second", out builtInFunctionsSignature);

            builtInFunctionsSignature.Length.Should().Be(6); // DateTimeOffset, Duration and TimeOfDay, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignatureFractionalseconds()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("fractionalseconds", out builtInFunctionsSignature);

            builtInFunctionsSignature.Length.Should().Be(4); // DateTimeOffset, TimeOfDay, Nullable and not Nullable
        }

        [Fact]
        public void CreateBuiltInSpatialFunctionsSignaturesCorrectly()
        {
            var functions = new Dictionary<string, FunctionSignatureWithReturnType[]>(StringComparer.Ordinal);
            BuiltInUriFunctions.CreateSpatialFunctions(functions);
            FunctionSignatureWithReturnType[] signatures;
            functions.TryGetValue("geo.distance", out signatures);
            signatures.Length.Should().Be(2);
            signatures[0].ReturnType.Definition.FullTypeName().Should().Be(EdmCoreModel.Instance.GetDouble(true).FullName());
            signatures[0].ArgumentTypes[0].Definition.FullTypeName().Should()
                                          .Be(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true).FullName());
            signatures[0].ArgumentTypes[1].Definition.FullTypeName().Should()
                                          .Be(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true).FullName());
            signatures[1].ReturnType.Definition.FullTypeName().Should().Be(EdmCoreModel.Instance.GetDouble(true).FullName());
            signatures[1].ArgumentTypes[0].Definition.FullTypeName().Should()
                                          .Be(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true).FullName());
            signatures[1].ArgumentTypes[1].Definition.FullTypeName().Should()
                                          .Be(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true).FullName());
        }

        [Fact]
        public void GetBuiltInFunctionSignaturesShouldThrowOnIncorrectName()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            var result = BuiltInUriFunctions.TryGetBuiltInFunction("rarg", out builtInFunctionsSignature);

            result.Should().BeFalse();
            builtInFunctionsSignature.Should().BeNull();
        }
    }
}
