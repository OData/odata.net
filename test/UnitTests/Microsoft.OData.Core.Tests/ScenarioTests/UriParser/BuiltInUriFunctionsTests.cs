//---------------------------------------------------------------------
// <copyright file="BuiltInUriFunctionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
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

            Assert.Equal(6, builtInFunctionsSignature.Length);
        }

        [Fact]
        public void GetBuiltInFunctionSignaturesDateTimeOffsetYear()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("year", out builtInFunctionsSignature);

            Assert.Equal(4, builtInFunctionsSignature.Length); // DateTimeOffset and Date, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignaturesDateTimeOffsetMonth()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("month", out builtInFunctionsSignature);

            Assert.Equal(4, builtInFunctionsSignature.Length); // DateTimeOffset and Date, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignaturesDateTimeOffsetDay()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("day", out builtInFunctionsSignature);

            Assert.Equal(4, builtInFunctionsSignature.Length); // DateTimeOffset and Date, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignatureHour()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("hour", out builtInFunctionsSignature);

            Assert.Equal(6, builtInFunctionsSignature.Length); // DateTimeOffset, Duration and TimeOfDay, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignatureMinute()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("minute", out builtInFunctionsSignature);

            Assert.Equal(6, builtInFunctionsSignature.Length); // DateTimeOffset, Duration and TimeOfDay, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignatureSecond()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("second", out builtInFunctionsSignature);

            Assert.Equal(6, builtInFunctionsSignature.Length); // DateTimeOffset, Duration and TimeOfDay, Nullable and not Nullable
        }

        [Fact]
        public void GetBuiltInFunctionSignatureFractionalseconds()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction("fractionalseconds", out builtInFunctionsSignature);

            Assert.Equal(4, builtInFunctionsSignature.Length); // DateTimeOffset, TimeOfDay, Nullable and not Nullable
        }

        [Fact]
        public void CreateBuiltInSpatialFunctionsSignaturesCorrectly()
        {
            var functions = new Dictionary<string, FunctionSignatureWithReturnType[]>(StringComparer.Ordinal);
            BuiltInUriFunctions.CreateSpatialFunctions(functions);
            FunctionSignatureWithReturnType[] signatures;
            functions.TryGetValue("geo.distance", out signatures);
            Assert.Equal(2, signatures.Length);
            Assert.Equal("Edm.Double", signatures[0].ReturnType.Definition.FullTypeName());

            Assert.Equal("Edm.GeographyPoint", signatures[0].ArgumentTypes[0].Definition.FullTypeName());
            Assert.Equal("Edm.GeographyPoint", signatures[0].ArgumentTypes[1].Definition.FullTypeName());

            Assert.Equal("Edm.Double", signatures[1].ReturnType.Definition.FullTypeName());
            Assert.Equal("Edm.GeometryPoint", signatures[1].ArgumentTypes[0].Definition.FullTypeName());
            Assert.Equal("Edm.GeometryPoint", signatures[1].ArgumentTypes[1].Definition.FullTypeName());
        }

        [Fact]
        public void GetBuiltInFunctionSignaturesShouldThrowOnIncorrectName()
        {
            FunctionSignatureWithReturnType[] builtInFunctionsSignature;
            var result = BuiltInUriFunctions.TryGetBuiltInFunction("rarg", out builtInFunctionsSignature);

            Assert.False(result);
            Assert.Null(builtInFunctionsSignature);
        }
    }
}
