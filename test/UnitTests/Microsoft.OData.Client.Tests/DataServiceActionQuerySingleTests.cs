//---------------------------------------------------------------------
// <copyright file="DataServiceActionQuerySingleTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Client.Tests;

public class DataServiceActionQuerySingleTests
{
    [Fact]
    public void GetValue_ExecutesActionAndReturnsSingleResult()
    {
        // Arrange
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            ExecuteFunc = (uri, method, single, parameters) => new[] { 42 }
        };

        var query = new DataServiceActionQuerySingle<int>(
            context,
            "http://service/Action",
            new BodyOperationParameter("param", 1));

        // Act
        var result = query.GetValue();

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void GetValue_ReturnsNullIfNoResult_ForNullableTypeInt()
    {
        // Arrange
        var context = new TestDataServiceContext<int?>(new Uri("http://service/"))
        {
            ExecuteFunc = (uri, method, single, parameters) => []
        };

        var query = new DataServiceActionQuerySingle<int?>(
            context,
            "http://service/Action");

        // Act
        var result = query.GetValue();

        // Assert
        Assert.Equal(default, result);
        Assert.Null(result);
    }

    [Fact]
    public void GetValue_ReturnsNullIfNoResult_ForNullableTypeGuid()
    {
        // Arrange
        var context = new TestDataServiceContext<Guid?>(new Uri("http://service/"))
        {
            ExecuteFunc = (uri, method, single, parameters) => []
        };

        var query = new DataServiceActionQuerySingle<Guid?>(
            context,
            "http://service/Action");

        // Act
        var result = query.GetValue();

        // Assert
        Assert.Equal(default, result);
        Assert.Null(result);
    }

    [Fact]
    public void GetValue_ReturnsNullIfNoResult_ForNullableTypeDateTime()
    {
        // Arrange
        var context = new TestDataServiceContext<DateTime?>(new Uri("http://service/"))
        {
            ExecuteFunc = (uri, method, single, parameters) => []
        };

        var query = new DataServiceActionQuerySingle<DateTime?>(
            context,
            "http://service/Action");

        // Act
        var result = query.GetValue();

        // Assert
        Assert.Equal(default, result);
        Assert.Null(result);
    }

    [Fact]
    public void GetValue_ReturnsNullIfNoResult_ForNullableTypeByte()
    {
        // Arrange
        var context = new TestDataServiceContext<byte?>(new Uri("http://service/"))
        {
            ExecuteFunc = (uri, method, single, parameters) => []
        };

        var query = new DataServiceActionQuerySingle<byte?>(
            context,
            "http://service/Action");

        // Act
        var result = query.GetValue();

        // Assert
        Assert.Equal(default, result);
        Assert.Null(result);
    }

    [Fact]
    public void GetValue_ReturnsNullIfNoResult_ForNullableTypeChar()
    {
        // Arrange
        var context = new TestDataServiceContext<char?>(new Uri("http://service/"))
        {
            ExecuteFunc = (uri, method, single, parameters) => []
        };

        var query = new DataServiceActionQuerySingle<char?>(
            context,
            "http://service/Action");

        // Act
        var result = query.GetValue();

        // Assert
        Assert.Equal(default, result);
        Assert.Null(result);
    }

    [Fact]
    public void GetValue_ReturnsNullIfNoResult_ForNullableTypeDecimal()
    {
        // Arrange
        var context = new TestDataServiceContext<decimal?>(new Uri("http://service/"))
        {
            ExecuteFunc = (uri, method, single, parameters) => []
        };

        var query = new DataServiceActionQuerySingle<decimal?>(
            context,
            "http://service/Action");

        // Act
        var result = query.GetValue();

        // Assert
        Assert.Equal(default, result);
        Assert.Null(result);
    }

    public static TheoryData<string, IEnumerable<string>> StringData => new()
    {
        { "This is string", new List<string> { "This is string" } },
        { null, Enumerable.Empty<string>() }
    };

    [Theory]
    [MemberData(nameof(StringData))]
    public void GetValue_ReturnsNullIfNoResult_ForNullableTypeString(string expectedResult, IEnumerable<string> mockData)
    {
        // Arrange
        var context = new TestDataServiceContext<string>(new Uri("http://service/"))
        {
            ExecuteFunc = (uri, method, single, parameters) => mockData
        };

        var query = new DataServiceActionQuerySingle<string>(
            context,
            "http://service/Action");

        // Act
        var result = query.GetValue();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    public static TheoryData<object, IEnumerable<object>> ObjectData => new()
    {
        { "This is string", new List<object> { "This is string" } },
        { null, Enumerable.Empty<object>() },
        { 100934, new List<object> { 100934 } },
        { 3.14159, new List<object> { 3.14159 } },
        { true, new List<object> { true } },
        { DateTime.Parse("2024-01-01T12:00:00Z"), new List<object> { DateTime.Parse("2024-01-01T12:00:00Z") } },
        { Guid.Parse("12345678-1234-1234-1234-1234567890ab"), new List<object> { Guid.Parse("12345678-1234-1234-1234-1234567890ab") } }
    };

    [Theory]
    [MemberData(nameof(ObjectData))]
    public void GetValue_ReturnsNullIfNoResult_ForNullableTypeObject(object expectedResult, IEnumerable<object> mockData)
    {
        // Arrange
        var context = new TestDataServiceContext<object>(new Uri("http://service/"))
        {
            ExecuteFunc = (uri, method, single, parameters) => mockData
        };

        var query = new DataServiceActionQuerySingle<object>(
            context,
            "http://service/Action");

        // Act
        var result = query.GetValue();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void GetValue_Throws_WhenMultipleResults()
    {
        // Arrange
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            ExecuteFunc = (uri, method, single, parameters) => new[] { 1, 2 }
        };
        var query = new DataServiceActionQuerySingle<int>(context, "http://service/Action");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => query.GetValue());
    }

    [Fact]
    public void BeginGetValue_DelegatesToContext()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            BeginExecuteFunc = (uri, callback, state, method, single, parameters) => asyncResult
        };

        var query = new DataServiceActionQuerySingle<int>(
            context,
            "http://service/Action");

        // Act
        var result = query.BeginGetValue(null, null);

        // Assert
        Assert.Equal(asyncResult, result);
    }

    [Fact]
    public void EndGetValue_DelegatesToContextAndReturnsSingleResult()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            EndExecuteFunc = (ar) => new[] { 99 }
        };

        var query = new DataServiceActionQuerySingle<int>(
            context,
            "http://service/Action");

        // Act
        var result = query.EndGetValue(asyncResult);

        // Assert
        Assert.Equal(99, result);
    }

    [Fact]
    public void EndGetValue_ThrowsInvalidOperationException_WhenNoResultsForNonNullableType()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            EndExecuteFunc = (ar) => Enumerable.Empty<int>()
        };
        var query = new DataServiceActionQuerySingle<int>(context, "http://service/Action");

        // Assert & Act
        Assert.Throws<InvalidOperationException>(() => query.EndGetValue(asyncResult));
    }

    [Fact]
    public void EndGetValue_ThrowsInvalidOperationException_WhenMultipleResults()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            EndExecuteFunc = (ar) => new[] { 1, 2 }
        };
        var query = new DataServiceActionQuerySingle<int>(context, "http://service/Action");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => query.EndGetValue(asyncResult));
    }

    [Fact]
    public async Task GetValueAsync_UsesBeginEndPattern_ReturnsSingleValue()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            BeginExecuteFunc = (uri, callback, state, method, single, parameters) =>
            {
                callback?.Invoke(asyncResult);
                return asyncResult;
            },
            EndExecuteFunc = (ar) => new[] { 567 }
        };

        var query = new DataServiceActionQuerySingle<int>(
            context,
            "http://service/Action");

        // Act
        var result = await query.GetValueAsync();

        // Assert
        Assert.Equal(567, result);
    }

    [Fact]
    public async Task GetValueAsync_UsesBeginEndPattern_ThrowsInvalidOperationException_WhenNoResultsForNonNullableType()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            BeginExecuteFunc = (uri, callback, state, method, single, parameters) =>
            {
                callback?.Invoke(asyncResult);
                return asyncResult;
            },
            EndExecuteFunc = (ar) => Enumerable.Empty<int>()
        };

        var query = new DataServiceActionQuerySingle<int>(
            context,
            "http://service/Action");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await query.GetValueAsync());
    }

    [Fact]
    public async Task GetValueAsync_UsesBeginEndPattern_Throws_WhenMultipleResults()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            BeginExecuteFunc = (uri, callback, state, method, single, parameters) =>
            {
                callback?.Invoke(asyncResult);
                return asyncResult;
            },
            EndExecuteFunc = (ar) => new[] { 1, 2 }
        };

        var query = new DataServiceActionQuerySingle<int>(
            context,
            "http://service/Action");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => query.GetValueAsync());
    }

    [Fact]
    public async Task GetValueAsync_WithCancellationToken_ReturnsExpectedResult()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            BeginExecuteFunc = (uri, callback, state, method, single, parameters) =>
            {
                callback?.Invoke(asyncResult);
                return asyncResult;
            },
            EndExecuteFunc = (ar) => new[] { 888 }
        };
        var query = new DataServiceActionQuerySingle<int>(context, "http://service/Action");

        // Act
        var result = await query.GetValueAsync(CancellationToken.None);

        // Assert
        Assert.Equal(888, result);
    }

    [Fact]
    public void BeginGetValue_PassesParameters()
    {
        // Arrange
        var called = false;
        var param = new BodyOperationParameter("p", 5);
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            BeginExecuteFunc = (uri, callback, state, method, single, parameters) =>
            {
                called = true;
                Assert.Single(parameters);
                Assert.Equal("p", parameters[0].Name);
                Assert.Equal(5, parameters[0].Value);
                return new TestAsyncResult();
            }
        };
        var query = new DataServiceActionQuerySingle<int>(context, "http://service/Action", param);

        // Act
        query.BeginGetValue(null, null);

        // Assert
        Assert.True(called);
    }

    [Fact]
    public void Constructor_SetsRequestUri()
    {
        // Arrange
        var context = new TestDataServiceContext<int>(new Uri("http://service/"));

        // Act
        var query = new DataServiceActionQuerySingle<int>(context, "http://service/Action");

        // Assert
        Assert.Equal(new Uri("http://service/Action"), query.RequestUri);
    }

    [Fact]
    public void GetValue_ThrowsIfContextThrows()
    {
        // Arrange
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            ExecuteFunc = (uri, method, single, parameters) => throw new InvalidOperationException("fail")
        };
        var query = new DataServiceActionQuerySingle<int>(context, "http://service/Action");

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => query.GetValue());
        Assert.Equal("fail", ex.Message);
    }

    [Fact]
    public void EndGetValue_ThrowsIfAsyncResultIsNull()
    {
        // Arrange
        var context = new TestDataServiceContext<int>(new Uri("http://service/"))
        {
            EndExecuteFunc = (ar) => new[] { 1 }
        };
        var query = new DataServiceActionQuerySingle<int>(context, "http://service/Action");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => query.EndGetValue(null));
    }

    #region Private

    /// <summary>
    /// Provides a test-specific implementation of the DataServiceContext for simulating and controlling data service operations.
    /// This class allows customization of data service execution behavior by exposing delegates that can be set to control the results 
    /// of Execute, BeginExecute, and EndExecute methods.
    /// </summary>
    private class TestDataServiceContext<T> : DataServiceContext
    {
        public Func<Uri, string, bool, BodyOperationParameter[], IEnumerable<T>> ExecuteFunc { get; set; }
        public Func<Uri, AsyncCallback, object, string, bool, BodyOperationParameter[], IAsyncResult> BeginExecuteFunc { get; set; }
        public Func<IAsyncResult, IEnumerable<T>> EndExecuteFunc { get; set; }

        public TestDataServiceContext(Uri serviceRoot) : base(serviceRoot) { }

        public override IEnumerable<TElement> Execute<TElement>(Uri requestUri, string httpMethod, bool singleResult, params OperationParameter[] operationParameters)
        {
            if (typeof(TElement) == typeof(T) && ExecuteFunc != null)
                return (IEnumerable<TElement>)ExecuteFunc(requestUri, httpMethod, singleResult, operationParameters.Cast<BodyOperationParameter>().ToArray());

            throw new NotImplementedException();
        }

        public override IAsyncResult BeginExecute<TElement>(Uri requestUri, AsyncCallback callback, object state, string httpMethod, bool singleResult, params OperationParameter[] operationParameters)
        {
            if (typeof(TElement) == typeof(T) && BeginExecuteFunc != null)
                return BeginExecuteFunc(requestUri, callback, state, httpMethod, singleResult, operationParameters.Cast<BodyOperationParameter>().ToArray());

            throw new NotImplementedException();
        }

        public override IEnumerable<TElement> EndExecute<TElement>(IAsyncResult asyncResult)
        {
            if (typeof(TElement) == typeof(T) && EndExecuteFunc != null)
                return (IEnumerable<TElement>)EndExecuteFunc(asyncResult);

            throw new NotImplementedException();
        }
    }

    private class TestAsyncResult : IAsyncResult
    {
        public object AsyncState => null;
        public WaitHandle AsyncWaitHandle => null;
        public bool CompletedSynchronously => true;
        public bool IsCompleted => true;
    }

    #endregion
}
