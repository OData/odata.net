//---------------------------------------------------------------------
// <copyright file="AsyncTask.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.E2E.TestCommon.Common;

namespace Microsoft.OData.Core.E2E.Tests.AsyncRequestTests;

/// <summary>
/// Represents an asynchronous task that is managed using a unique token.
/// This class is used to simulate and manage asynchronous operations in tests.
/// </summary>
internal class AsyncTask
{
    public const int DefaultDuration = 5;

    // A static dictionary to store and manage async tasks using a unique token as the key
    private static Dictionary<string, AsyncTask> asyncTaskMap = new Dictionary<string, AsyncTask>();

    // The time at which the task is due to be completed
    private DateTime dueAt;

    // The original URL associated with the async task
    private Uri originalUrl;

    // The HTTP request message associated with the async task, if any
    private TestHttpClientRequestMessage? requestMessage;

    // Indicates whether the task is ready to be processed (i.e., its due time has passed)
    public bool Ready
    {
        get { return dueAt < DateTime.Now; }
    }

    public AsyncTask(Uri originalUrl, DateTime dueAt, TestHttpClientRequestMessage? requestMessage = null)
    {
        this.originalUrl = originalUrl;
        this.dueAt = dueAt;
        this.requestMessage = requestMessage;
    }

    // Retrieves an async task from the map using the provided async token
    public static AsyncTask GetTask(string asyncToken)
    {
        AsyncTask task;
        asyncTaskMap.TryGetValue(asyncToken, out task);
        return task;
    }

    // Adds a new async task to the map with the specified async token
    public static bool AddTask(string asyncToken, AsyncTask newTask)
    {
        asyncTaskMap.Add(asyncToken, newTask);
        return true;
    }

    // Returns the original URL associated with the async task
    public Uri GetOriginalUrl()
    {
        return this.originalUrl;
    }

    // Returns the HTTP request message associated with the async task, if any
    public TestHttpClientRequestMessage? GetRequestMessage()
    {
        return this.requestMessage;
    }
}
