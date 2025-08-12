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
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            ExecuteIntFunc = (uri, method, single, parameters) => new[] { 42 }
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
    public void GetValue_ReturnsDefaultIfNoResult()
    {
        // Arrange
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            ExecuteIntFunc = (uri, method, single, parameters) => Enumerable.Empty<int>()
        };

        var query = new DataServiceActionQuerySingle<int>(
            context,
            "http://service/Action");

        // Act
        var result = query.GetValue();

        // Assert
        Assert.Equal(default, result);
    }

    [Fact]
    public void GetValue_Throws_WhenMultipleResults()
    {
        // Arrange
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            ExecuteIntFunc = (uri, method, single, parameters) => new[] { 1, 2 }
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
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            BeginExecuteIntFunc = (uri, callback, state, method, single, parameters) => asyncResult
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
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            EndExecuteIntFunc = (ar) => new[] { 99 }
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
    public void EndGetValue_ReturnsDefault_WhenNoResults()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            EndExecuteIntFunc = (ar) => Enumerable.Empty<int>()
        };
        var query = new DataServiceActionQuerySingle<int>(context, "http://service/Action");

        // Act
        var result = query.EndGetValue(asyncResult);

        // Assert
        Assert.Equal(default, result);
    }

    [Fact]
    public void EndGetValue_Throws_WhenMultipleResults()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            EndExecuteIntFunc = (ar) => new[] { 1, 2 }
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
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            BeginExecuteIntFunc = (uri, callback, state, method, single, parameters) =>
            {
                callback?.Invoke(asyncResult);
                return asyncResult;
            },
            EndExecuteIntFunc = (ar) => new[] { 567 }
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
    public async Task GetValueAsync_UsesBeginEndPattern_ReturnsDefault_WhenNoResults()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            BeginExecuteIntFunc = (uri, callback, state, method, single, parameters) =>
            {
                callback?.Invoke(asyncResult);
                return asyncResult;
            },
            EndExecuteIntFunc = (ar) => Enumerable.Empty<int>()
        };

        var query = new DataServiceActionQuerySingle<int>(
            context,
            "http://service/Action");

        // Act
        var result = await query.GetValueAsync();

        // Assert
        Assert.Equal(default, result);
    }

    [Fact]
    public async Task GetValueAsync_UsesBeginEndPattern_Throws_WhenMultipleResults()
    {
        // Arrange
        var asyncResult = new TestAsyncResult();
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            BeginExecuteIntFunc = (uri, callback, state, method, single, parameters) =>
            {
                callback?.Invoke(asyncResult);
                return asyncResult;
            },
            EndExecuteIntFunc = (ar) => new[] { 1, 2 }
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
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            BeginExecuteIntFunc = (uri, callback, state, method, single, parameters) =>
            {
                callback?.Invoke(asyncResult);
                return asyncResult;
            },
            EndExecuteIntFunc = (ar) => new[] { 888 }
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
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            BeginExecuteIntFunc = (uri, callback, state, method, single, parameters) =>
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
        var context = new TestDataServiceContext(new Uri("http://service/"));

        // Act
        var query = new DataServiceActionQuerySingle<int>(context, "http://service/Action");

        // Assert
        Assert.Equal(new Uri("http://service/Action"), query.RequestUri);
    }

    [Fact]
    public void GetValue_ThrowsIfContextThrows()
    {
        // Arrange
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            ExecuteIntFunc = (uri, method, single, parameters) => throw new InvalidOperationException("fail")
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
        var context = new TestDataServiceContext(new Uri("http://service/"))
        {
            EndExecuteIntFunc = (ar) => new[] { 1 }
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
    private class TestDataServiceContext : DataServiceContext
    {
        public Func<Uri, string, bool, BodyOperationParameter[], IEnumerable<int>> ExecuteIntFunc { get; set; }
        public Func<Uri, AsyncCallback, object, string, bool, BodyOperationParameter[], IAsyncResult> BeginExecuteIntFunc { get; set; }
        public Func<IAsyncResult, IEnumerable<int>> EndExecuteIntFunc { get; set; }

        public TestDataServiceContext(Uri serviceRoot) : base(serviceRoot) { }

        public override IEnumerable<TElement> Execute<TElement>(Uri requestUri, string httpMethod, bool singleResult, params OperationParameter[] operationParameters)
        {
            if (typeof(TElement) == typeof(int) && ExecuteIntFunc != null)
                return (IEnumerable<TElement>)ExecuteIntFunc(requestUri, httpMethod, singleResult, operationParameters.Cast<BodyOperationParameter>().ToArray());

            throw new NotImplementedException();
        }

        public override IAsyncResult BeginExecute<TElement>(Uri requestUri, AsyncCallback callback, object state, string httpMethod, bool singleResult, params OperationParameter[] operationParameters)
        {
            if (typeof(TElement) == typeof(int) && BeginExecuteIntFunc != null)
                return BeginExecuteIntFunc(requestUri, callback, state, httpMethod, singleResult, operationParameters.Cast<BodyOperationParameter>().ToArray());

            throw new NotImplementedException();
        }

        public override IEnumerable<TElement> EndExecute<TElement>(IAsyncResult asyncResult)
        {
            if (typeof(TElement) == typeof(int) && EndExecuteIntFunc != null)
                return (IEnumerable<TElement>)EndExecuteIntFunc(asyncResult);

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
