//---------------------------------------------------------------------
// <copyright file="AsyncTask.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.ODataWCFService.Handlers;

    public class AsyncTask
    {
        // This is an assumption that every async task takes the same amount of time to finish
        public const int DefaultDuration = 5;

        private static Dictionary<string, AsyncTask> asyncTaskMap = new Dictionary<string, AsyncTask>();

        private RequestHandler requestHandler;
        private IODataRequestMessage requestMessage;
        private DateTime dueAt;

        public bool Ready
        {
            get { return dueAt < DateTime.Now; }
        }

        public AsyncTask(RequestHandler requestHandler, IODataRequestMessage requestMessage, DateTime dueAt)
        {
            this.requestHandler = requestHandler;
            this.requestMessage = requestMessage;
            this.dueAt = dueAt;
        }

        public void Execute(ODataAsynchronousResponseMessage responseMessage)
        {
            this.requestHandler.Process(this.requestMessage, responseMessage);
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
    }
}