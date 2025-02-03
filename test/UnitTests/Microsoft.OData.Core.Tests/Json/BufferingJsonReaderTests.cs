//---------------------------------------------------------------------
// <copyright file="BufferingJsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class BufferingJsonReaderTests
    {
        private const string ErrorCode = "NRE";
        private const string ErrorMessage = "Object reference not set to an instance of an object";
        private const string ErrorTarget = "ConApp";
        private const string InnerErrorMessage = "Exception thrown due to attempt to access a member on a variable that currently holds a null reference";
        private const string InnerErrorTypeName = "System.NullReferenceException";
        private const string InnerErrorStackTrace = "   at ConApp.Program.Main(String[] args) in C:\\\\Projects\\\\ConApp\\\\ConApp\\\\Program.cs:line 10";
        private const string ErrorDetailErrorCode = "FCE";
        private const string ErrorDetailErrorMessage = "A first chance exception of type 'System.NullReferenceException' occurred in ConApp.exe";
        private const int MaxInnerErrorDepth = 4;

        [Fact]
        public void StartBufferingAndTryToReadInStreamErrorPropertyValue_Works()
        {
            // Arrange
            const string payload =
                @"{""code"":"""",""message"":"""",""target"":""any target"","
                + @"""details"":[{""code"":""500"",""target"":""another target"",""message"":""any msg""}]}";
            var stringReader = new StringReader(payload);
            var innerReader = new JsonReader(stringReader, false);
            var jsonReader = new BufferingJsonReader(innerReader, "any", 0);
            ODataError error;

            // Act
            jsonReader.Read();
            var result = jsonReader.StartBufferingAndTryToReadInStreamErrorPropertyValue(out error);

            // Assert
            Assert.True(result);
            Assert.Equal("any target", error.Target);
            Assert.Equal(1, error.Details.Count);
            var detail = error.Details.Single();
            Assert.Equal("500", detail.Code);
            Assert.Equal("another target", detail.Target);
            Assert.Equal("any msg", detail.Message);
        }

        [Theory]
        [MemberData(nameof(GetUnintelligibleErrorData))]
        public void ReadInStreamErrorProperty_DoesNotThrowExceptionForUnintelligibleError(string payload)
        {
            // Arrange
            var stringReader = new StringReader(payload);
            using (var jsonReader = new JsonReader(stringReader, false))
            {
                var bufferingReader = new BufferingJsonReader(jsonReader, "error", MaxInnerErrorDepth);

                // Act & Assert
                Assert.True(bufferingReader.Read());
            }
        }

        [Fact]
        public void StartBufferingAndTryToReadInStreamErrorPropertyValueWithMultipleErrorDetails_Works()
        {
            // Arrange
            var payload = "{" +
                $"\"code\":\"{ErrorCode}\"," +
                $"\"message\":\"{ErrorMessage}\"," +
                $"\"target\":\"{ErrorTarget}\"," +
                "\"innererror\":{" +
                $"\"type\":\"{InnerErrorTypeName}\"," +
                $"\"message\":\"{InnerErrorMessage}\"," +
                $"\"stacktrace\":\"{InnerErrorStackTrace}\"," +
                $"\"internalexception\":{{}}}}," +
                $"\"details\":[{{\"code\":\"dxd1\",\"message\":\"dmg1\"}},{{\"code\":\"dxd2\",\"message\":\"dmg2\"}}]}}";
            ODataError error;

            var stringReader = new StringReader(payload);
            using (var jsonReader = new JsonReader(stringReader, false))
            {
                var bufferingReader = new BufferingJsonReader(jsonReader, "other", MaxInnerErrorDepth);

                // Act
                var result1 = bufferingReader.Read();
                var result2 = bufferingReader.StartBufferingAndTryToReadInStreamErrorPropertyValue(out error);

                // Assert
                Assert.True(result1);
                Assert.True(result2);

                Assert.NotNull(error);
                Assert.Equal(ErrorCode, error.Code);
                Assert.Equal(ErrorMessage, error.Message);
                Assert.Equal(ErrorTarget, error.Target);
                var innerError = error.InnerError;
                Assert.NotNull(innerError);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorMessageName, out ODataValue innerErrorMessage));
                Assert.Equal(InnerErrorMessage, Assert.IsType<ODataPrimitiveValue>(innerErrorMessage).Value);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorTypeNameName, out ODataValue innerErrorTypeName));
                Assert.Equal(InnerErrorTypeName, Assert.IsType<ODataPrimitiveValue>(innerErrorTypeName).Value);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorStackTraceName, out ODataValue innerErrorStackTrace));
                Assert.Equal(InnerErrorStackTrace.Replace("\\\\", "\\"), Assert.IsType<ODataPrimitiveValue>(innerErrorStackTrace).Value);
                Assert.NotNull(innerError.InnerError);
                Assert.NotNull(error.Details);
                Assert.Equal(2, error.Details.Count);
                Assert.Equal("dxd1", error.Details.ElementAt(0).Code);
                Assert.Equal("dmg1", error.Details.ElementAt(0).Message);
                Assert.Equal("dxd2", error.Details.ElementAt(1).Code);
                Assert.Equal("dmg2", error.Details.ElementAt(1).Message);
            }
        }

        [Fact]
        public async Task ReadInStreamErrorPropertyAsync_ThrowsExceptionForODataErrorRead()
        {
            // Arrange
            var payload = "{\"error\":{" +
                $"\"code\":\"{ErrorCode}\"," +
                $"\"message\":\"{ErrorMessage}\"," +
                $"\"target\":\"{ErrorTarget}\"," +
                "\"innererror\":{" +
                $"\"type\":\"{InnerErrorTypeName}\"," +
                $"\"message\":\"{InnerErrorMessage}\"," +
                $"\"stacktrace\":\"{InnerErrorStackTrace}\"," +
                $"\"internalexception\":{{}}}}," +
                $"\"details\":[{{\"code\":\"{ErrorDetailErrorCode}\",\"message\":\"{ErrorDetailErrorMessage}\",\"target\":null}}]}}}}";

            var stringReader = new StringReader(payload);
            using (var jsonReader = new JsonReader(stringReader, false))
            {
                var bufferingReader = new BufferingJsonReader(jsonReader, "error", MaxInnerErrorDepth);

                // Act
                var exception = await Assert.ThrowsAsync<ODataErrorException>(
                    () => bufferingReader.ReadAsync());

                // Assert
                var error = exception.Error;
                Assert.NotNull(error);
                Assert.Equal(ErrorCode, error.Code);
                Assert.Equal(ErrorMessage, error.Message);
                Assert.Equal(ErrorTarget, error.Target);
                var innerError = error.InnerError;
                Assert.NotNull(innerError);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorMessageName, out ODataValue innerErrorMessage));
                Assert.Equal(InnerErrorMessage, Assert.IsType<ODataPrimitiveValue>(innerErrorMessage).Value);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorTypeNameName, out ODataValue innerErrorTypeName));
                Assert.Equal(InnerErrorTypeName, Assert.IsType<ODataPrimitiveValue>(innerErrorTypeName).Value);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorStackTraceName, out ODataValue innerErrorStackTrace));
                Assert.Equal(InnerErrorStackTrace.Replace("\\\\", "\\"), Assert.IsType<ODataPrimitiveValue>(innerErrorStackTrace).Value);
                Assert.NotNull(innerError.InnerError);
                Assert.NotNull(error.Details);
                var errorDetail = Assert.Single(error.Details);
                Assert.Equal(ErrorDetailErrorCode, errorDetail.Code);
                Assert.Equal(ErrorDetailErrorMessage, errorDetail.Message);
                Assert.Null(errorDetail.Target);
            }
        }

        [Fact]
        public async Task StartBufferingAndTryToReadInStreamErrorPropertyValueAsync_Works()
        {
            // Arrange
            var payload = "{" +
                $"\"code\":\"{ErrorCode}\"," +
                $"\"message\":\"{ErrorMessage}\"," +
                $"\"target\":\"{ErrorTarget}\"," +
                "\"innererror\":{" +
                $"\"type\":\"{InnerErrorTypeName}\"," +
                $"\"message\":\"{InnerErrorMessage}\"," +
                $"\"stacktrace\":\"{InnerErrorStackTrace}\"," +
                $"\"internalexception\":{{}}}}," +
                $"\"details\":[{{\"code\":\"{ErrorDetailErrorCode}\",\"message\":\"{ErrorDetailErrorMessage}\",\"target\":null}}]}}";

            var stringReader = new StringReader(payload);
            using (var jsonReader = new JsonReader(stringReader, false))
            {
                var bufferingReader = new BufferingJsonReader(jsonReader, "other", MaxInnerErrorDepth);

                // Act
                var result1 = await bufferingReader.ReadAsync();
                var result2 = await bufferingReader.StartBufferingAndTryToReadInStreamErrorPropertyValueAsync();

                // Assert
                Assert.True(result1);
                Assert.True(result2.IsReadSuccessfully);

                var error = result2.Error;
                Assert.NotNull(error);
                Assert.Equal(ErrorCode, error.Code);
                Assert.Equal(ErrorMessage, error.Message);
                Assert.Equal(ErrorTarget, error.Target);
                var innerError = error.InnerError;
                Assert.NotNull(innerError);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorMessageName, out ODataValue innerErrorMessage));
                Assert.Equal(InnerErrorMessage, Assert.IsType<ODataPrimitiveValue>(innerErrorMessage).Value);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorTypeNameName, out ODataValue innerErrorTypeName));
                Assert.Equal(InnerErrorTypeName, Assert.IsType<ODataPrimitiveValue>(innerErrorTypeName).Value);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorStackTraceName, out ODataValue innerErrorStackTrace));
                Assert.Equal(InnerErrorStackTrace.Replace("\\\\", "\\"), Assert.IsType<ODataPrimitiveValue>(innerErrorStackTrace).Value);
                Assert.NotNull(innerError.InnerError);
                Assert.NotNull(error.Details);
                var errorDetail = Assert.Single(error.Details);
                Assert.Equal(ErrorDetailErrorCode, errorDetail.Code);
                Assert.Equal(ErrorDetailErrorMessage, errorDetail.Message);
                Assert.Null(errorDetail.Target);
            }
        }

        public static IEnumerable<object[]> GetUnintelligibleErrorData()
        {
            // Error code is expected to be a string
            yield return new object[] { "{\"error\":{\"code\":13}}" };

            // Duplicate error code
            yield return new object[]
            {
                $"{{\"error\":{{\"code\":\"{ErrorCode}\",\"code\":\"{ErrorCode}\"}}}}"
            };

            // Error message is expected to be a string
            yield return new object[] { "{\"error\":{\"message\":13}}" };

            // Duplicate error message
            yield return new object[]
            {
                $"{{\"error\":{{\"message\":\"{ErrorMessage}\",\"message\":\"{ErrorMessage}\"}}}}"
            };

            // Error target is expected to be a string
            yield return new object[] { "{\"error\":{\"target\":13}}" };

            // Duplicate error target
            yield return new object[]
            {
                $"{{\"error\":{{\"target\":\"{ErrorTarget}\",\"target\":\"{ErrorTarget}\"}}}}"
            };

            // Error inner error message is expected to be a string
            yield return new object[] { "{\"error\":{\"innererror\":{\"message\":13}}}" };

            // Error inner error type is expected to be a string
            yield return new object[] { "{\"error\":{\"innererror\":{\"type\":13}}}" };

            // Error inner error stacktrace is expected to be a string
            yield return new object[] { "{\"error\":{\"innererror\":{\"stacktrace\":13}}}" };

            // Error inner error internalexception is expected to be a string
            yield return new object[] { "{\"error\":{\"innererror\":{\"internalexception\":13}}}" };

            // Duplicate inner error message
            yield return new object[]
            {
                $"{{\"error\":{{\"innererror\":{{\"message\":\"{InnerErrorMessage}\",\"message\":\"{InnerErrorMessage}\"}}}}}}"
            };

            // Duplicate inner error type
            yield return new object[]
            {
                $"{{\"error\":{{\"innererror\":{{\"type\":\"{InnerErrorTypeName}\",\"type\":\"{InnerErrorTypeName}\"}}}}}}"
            };

            // Duplicate inner error stacktrace
            yield return new object[]
            {
                $"{{\"error\":{{\"innererror\":{{\"stacktrace\":\"{InnerErrorStackTrace}\",\"stacktrace\":\"{InnerErrorStackTrace}\"}}}}}}"
            };

            // Duplicate inner error internalexception
            yield return new object[]
            {
                $"{{\"error\":{{\"innererror\":{{\"internalexception\":{{}},\"internalexception\":{{}}}}}}}}"
            };

            // Duplicate inner error
            yield return new object[]
            {
                $"{{\"error\":{{\"innererror\":{{\"message\":\"{InnerErrorMessage}\"}},\"innererror\":{{\"message\":\"{InnerErrorMessage}\"}}}}}}"
            };

            // Error detail error code is expected to be a string
            yield return new object[] { "{\"error\":{\"details\":[{\"code\":13}]}}" };

            // Error detail error message is expected to be a string
            yield return new object[] { "{\"error\":{\"details\":[{\"message\":13}]}}" };

            // Error detail error target is expected to be a string
            yield return new object[] { "{\"error\":{\"details\":[{\"target\":13}]}}" };

            // Error detail error code is duplicated
            yield return new object[]
            {
                $"{{\"error\":{{\"details\":[{{\"code\":\"{ErrorDetailErrorCode}\",\"code\":\"{ErrorDetailErrorCode}\"}}]}}}}"
            };

            // Error detail error message is duplicated
            yield return new object[]
            {
                $"{{\"error\":{{\"details\":[{{\"message\":\"{ErrorDetailErrorMessage}\",\"message\":\"{ErrorDetailErrorMessage}\"}}]}}}}"
            };

            // Error detail error target is duplicated
            yield return new object[]
            {
                $"{{\"error\":{{\"details\":[{{\"target\":null,\"target\":null}}]}}}}"
            };

            // Duplicate details
            yield return new object[]
            {
                $"{{\"error\":{{\"details\":[{{\"code\":\"{ErrorDetailErrorCode}\"}}],\"details\":[{{\"code\":\"{ErrorDetailErrorCode}\"}}]}}}}"
            };

            // error object null
            yield return new object[] { "{\"error\":null}" };

            // inner error property null
            yield return new object[] { "{\"error\":{\"innererror\":null}}" };

            // details property null
            yield return new object[] { "{\"error\":{\"details\":null}}" };

            // Non-supported property
            yield return new object[] { "{\"error\":{\"foo\":\"bar\"}}" };

            // Any other property than the expected in-stream error property
            yield return new object[] { "{\"any\":{\"code\":\"foobar\"}}" };
        }

        [Theory]
        [MemberData(nameof(GetUnintelligibleErrorData))]
        public async Task ReadInStreamErrorPropertyAsync_DoesNotThrowExceptionForUnintelligibleError(string payload)
        {
            // Arrange
            var stringReader = new StringReader(payload);
            using (var jsonReader = new JsonReader(stringReader, false))
            {
                var bufferingReader = new BufferingJsonReader(jsonReader, "error", MaxInnerErrorDepth);

                // Act & Assert
                Assert.True(await bufferingReader.ReadAsync());
            }
        }

        [Fact]
        public async Task ReadInStreamErrorPropertyAsync_SkipsNonSupportedPropertiesInInnerError()
        {
            // Arrange
            var payload = "{\"error\":{" +
                $"\"code\":\"{ErrorCode}\"," +
                $"\"message\":\"{ErrorMessage}\"," +
                $"\"target\":\"{ErrorTarget}\"," +
                "\"innererror\":{" +
                $"\"type\":\"{InnerErrorTypeName}\"," +
                $"\"message\":\"{InnerErrorMessage}\"," +
                $"\"stacktrace\":\"{InnerErrorStackTrace}\"," +
                "\"foo\":\"bar\"}}}";

            var stringReader = new StringReader(payload);
            using (var jsonReader = new JsonReader(stringReader, false))
            {
                var bufferingReader = new BufferingJsonReader(jsonReader, "error", MaxInnerErrorDepth);

                // Act
                var exception = await Assert.ThrowsAsync<ODataErrorException>(
                    () => bufferingReader.ReadAsync());

                // Assert
                var error = exception.Error;
                Assert.NotNull(error);
                var innerError = error.InnerError;
                Assert.NotNull(innerError);
                Assert.Equal(3, innerError.Properties.Count);
                Assert.Single(innerError.Properties.Where(d => d.Key.Equals("message")));
                Assert.Single(innerError.Properties.Where(d => d.Key.Equals("type")));
                Assert.Single(innerError.Properties.Where(d => d.Key.Equals("stacktrace")));
                Assert.Empty(innerError.Properties.Where(d => d.Key.Equals("foo")));
            }
        }

        [Fact]
        public async Task ReadInStreamErrorPropertyAsync_SkipsNonSupportedPropertiesInErrorDetails()
        {
            // Arrange
            var payload = "{\"error\":{" +
                $"\"code\":\"{ErrorCode}\"," +
                $"\"message\":\"{ErrorMessage}\"," +
                $"\"target\":\"{ErrorTarget}\"," +
                $"\"details\":[{{\"code\":\"{ErrorDetailErrorCode}\",\"message\":\"{ErrorDetailErrorMessage}\",\"target\":null,\"foo\":\"bar\"}}]}}}}";

            var stringReader = new StringReader(payload);
            using (var jsonReader = new JsonReader(stringReader, false))
            {
                var bufferingReader = new BufferingJsonReader(jsonReader, "error", MaxInnerErrorDepth);

                // Act
                var exception = await Assert.ThrowsAsync<ODataErrorException>(
                    () => bufferingReader.ReadAsync());

                // Assert
                var error = exception.Error;
                Assert.NotNull(error);
                Assert.Null(error.InnerError);
                Assert.NotNull(error.Details);
                var errorDetail = Assert.Single(error.Details);
                Assert.Equal(ErrorDetailErrorCode, errorDetail.Code);
                Assert.Equal(ErrorDetailErrorMessage, errorDetail.Message);
                Assert.Null(errorDetail.Target);
            }
        }

        [Fact]
        public async Task StartBufferingAndTryToReadInStreamErrorPropertyValueWithMultipleErrorDetailsAsync_Works()
        {
            // Arrange
            var payload = "{" +
                $"\"code\":\"{ErrorCode}\"," +
                $"\"message\":\"{ErrorMessage}\"," +
                $"\"target\":\"{ErrorTarget}\"," +
                "\"innererror\":{" +
                $"\"type\":\"{InnerErrorTypeName}\"," +
                $"\"message\":\"{InnerErrorMessage}\"," +
                $"\"stacktrace\":\"{InnerErrorStackTrace}\"," +
                $"\"internalexception\":{{}}}}," +
                $"\"details\":[{{\"code\":\"dxd1\",\"message\":\"dmg1\"}},{{\"code\":\"dxd2\",\"message\":\"dmg2\"}}]}}";

            var stringReader = new StringReader(payload);
            using (var jsonReader = new JsonReader(stringReader, false))
            {
                var bufferingReader = new BufferingJsonReader(jsonReader, "other", MaxInnerErrorDepth);

                // Act
                var result1 = await bufferingReader.ReadAsync();
                var result2 = await bufferingReader.StartBufferingAndTryToReadInStreamErrorPropertyValueAsync();

                // Assert
                Assert.True(result1);
                Assert.True(result2.IsReadSuccessfully);

                var error = result2.Error;
                Assert.NotNull(error);
                Assert.Equal(ErrorCode, error.Code);
                Assert.Equal(ErrorMessage, error.Message);
                Assert.Equal(ErrorTarget, error.Target);
                var innerError = error.InnerError;
                Assert.NotNull(innerError);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorMessageName, out ODataValue innerErrorMessage));
                Assert.Equal(InnerErrorMessage, Assert.IsType<ODataPrimitiveValue>(innerErrorMessage).Value);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorTypeNameName, out ODataValue innerErrorTypeName));
                Assert.Equal(InnerErrorTypeName, Assert.IsType<ODataPrimitiveValue>(innerErrorTypeName).Value);
                Assert.True(innerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorStackTraceName, out ODataValue innerErrorStackTrace));
                Assert.Equal(InnerErrorStackTrace.Replace("\\\\", "\\"), Assert.IsType<ODataPrimitiveValue>(innerErrorStackTrace).Value);
                Assert.NotNull(innerError.InnerError);
                Assert.NotNull(error.Details);
                Assert.Equal(2, error.Details.Count);
                Assert.Equal("dxd1", error.Details.ElementAt(0).Code);
                Assert.Equal("dmg1", error.Details.ElementAt(0).Message);
                Assert.Equal("dxd2", error.Details.ElementAt(1).Code);
                Assert.Equal("dmg2", error.Details.ElementAt(1).Message);
            }
        }

        [Fact]
        public async Task ReadODataResourceAsync()
        {
            // Arrange
            var payload = "{\"Customer\":{\"Id\":1,\"Name\":\"Sue\"}}";

            var stringReader = new StringReader(payload);
            using (var jsonReader = new JsonReader(stringReader, false))
            {
                var bufferingReader = new BufferingJsonReader(jsonReader, "any", MaxInnerErrorDepth);

                // Act
                await bufferingReader.ReadAsync();
                await bufferingReader.ReadStartObjectAsync();
                await bufferingReader.ReadPropertyNameAsync();
                var odataValue = await bufferingReader.ReadODataValueAsync();

                // Assert
                var resourceValue = Assert.IsType<ODataResourceValue>(odataValue);
                Assert.Equal(2, resourceValue.Properties.Count());
                var prop1 = resourceValue.Properties.First();
                var prop2 = resourceValue.Properties.Last();
                Assert.Equal("Id", prop1.Name);
                Assert.Equal(1, prop1.Value);
                Assert.Equal("Name", prop2.Name);
                Assert.Equal("Sue", prop2.Value);
            }
        }

        [Theory]
        [InlineData("13", false)]
        [InlineData("null", true)]
        [InlineData("\"The quick brown fox jumps over the lazy dog.\"", true)]
        public async Task CanStreamAsync_ForReaderNotInBufferingState(string payload, bool expected)
        {
            var stringReader = new StringReader(string.Format("{{\"Data\":{0}}}", payload));

            using (var jsonReader = new JsonReader(stringReader, false))
            {
                var bufferingReader = new BufferingJsonReader(jsonReader, "any", MaxInnerErrorDepth);

                await bufferingReader.ReadAsync();
                await bufferingReader.ReadStartObjectAsync();
                await bufferingReader.ReadPropertyNameAsync();

                Assert.Equal(expected, await bufferingReader.CanStreamAsync());
            }
        }

        [Theory]
        [InlineData("13", false)]
        [InlineData("null", true)]
        [InlineData("\"The quick brown fox jumps over the lazy dog.\"", true)]
        [InlineData("{\"Id\":1,\"Name\":\"Sue\"}", true)]
        [InlineData("[{\"Id\":1,\"Name\":\"Sue\"}]", true)]
        public async Task CanStreamAsync_ForReaderInBufferingState(string payload, bool expected)
        {
            var stringReader = new StringReader(string.Format("{{\"Data\":{0}}}", payload));

            using (var jsonReader = new JsonReader(stringReader, false))
            {
                var bufferingReader = new BufferingJsonReader(jsonReader, "any", MaxInnerErrorDepth);

                await bufferingReader.ReadAsync();
                await bufferingReader.ReadStartObjectAsync();
                await bufferingReader.ReadPropertyNameAsync();
                await bufferingReader.StartBufferingAsync();

                Assert.Equal(expected, await bufferingReader.CanStreamAsync());
            }
        }
    }
}
