//---------------------------------------------------------------------
// <copyright file="AsyncTask.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.E2E.TestCommon.Common;

namespace Microsoft.OData.Core.E2E.Tests.AsyncRequestTests;

public class AsyncTask
{
    // This is an assumption that every async task takes the same amount of time to finish
    public const int DefaultDuration = 5;

    private static Dictionary<string, AsyncTask> asyncTaskMap = new Dictionary<string, AsyncTask>();

    private DateTime dueAt;
    private Uri originalUrl;
    private TestHttpClientRequestMessage? requestMessage;

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

    public static AsyncTask GetTask(string asyncToken)
    {
        AsyncTask task;
        asyncTaskMap.TryGetValue(asyncToken, out task);
        return task;
    }

    public static bool AddTask(string asyncToken, AsyncTask newTask)
    {
        asyncTaskMap.Add(asyncToken, newTask);
        return true;
    }

    public Uri GetOriginalUrl()
    {
        return this.originalUrl;
    }

    public TestHttpClientRequestMessage? GetRequestMessage()
    {
        return this.requestMessage;
    }
}
