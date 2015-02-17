//---------------------------------------------------------------------
// <copyright file="BatchRequestWritingUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BatchQuery : BatchItem
    {
        public Operation Operation { get; private set; }

        public BatchQuery(Operation operation)
        {
            this.Operation = operation;
        }

        public void Write(StringBuilder builder)
        {
            builder.AppendLine("Content-Type: application/http");
            builder.AppendLine("Content-Transfer-Encoding: binary");
            builder.AppendLine();

            // write the operation's specified text
            builder.Append(this.Operation.Text);
            builder.AppendLine();
        }
    }

    public class Operation
    {
        public string Text { get; private set; }

        public Operation(string text)
        {
            this.Text = text;
        }
    }

    public class Changeset : BatchItem
    {
        private int contentId;

        public Operation[] Operations { get; set; }

        public Changeset(int initContentId, params Operation[] operations)
        {
            this.Operations = operations;
            this.contentId = initContentId;
        }

        public void Write(StringBuilder builder)
        {
            Guid changesetGuid = System.Guid.NewGuid();
            string changesetDelimiter = "changeset-" + changesetGuid.ToString();

            // TODO: sum the length of all the operations with their headers and such
            int changesetLength = 0;
            foreach (var o in this.Operations)
            {
                changesetLength += o.Text.Length;
            }

            // write the start of the changeset
            builder.Append("Content-Type: multipart/mixed; ");
            builder.Append("boundary=");
            builder.AppendLine(changesetDelimiter);
            builder.Append("Content-Length: ");
            builder.AppendLine(changesetLength.ToString());
            builder.AppendLine();

            foreach (Operation operation in this.Operations)
            {
                // write the start of this operation
                builder.Append("--");
                builder.AppendLine(changesetDelimiter);
                builder.AppendLine("Content-Type: application/http");
                builder.AppendLine("Content-Transfer-Encoding: binary");
                builder.AppendLine("Content-ID: " + (++this.contentId).ToString());
                builder.AppendLine();

                // write the operation's specified text
                builder.Append(operation.Text);
                builder.AppendLine();
            }

            // close the changeset
            builder.Append("--");
            builder.Append(changesetDelimiter);
            builder.AppendLine("--");
        }
    }

    public interface BatchItem
    {
        void Write(StringBuilder builder);
    }

    public class BatchInfo
    {
        public BatchItem[] Items { get; set; }

        public BatchInfo(params BatchItem[] items)
        {
            this.Items = items;
        }
    }

    public class SimpleBatchTestCase
    {
        public BatchInfo RequestPayload { get; set; }
        public string ExpectedResponsePayloadExact { get; set; }
        public string[] ExpectedResponsePayloadContains { get; set; }
        public int ResponseStatusCode { get; set; }
        public string ResponseETag { get; set; }
        public Version ResponseVersion { get; set; }
        public Version RequestDataServiceVersion { get; set; }
        public Version RequestMaxDataServiceVersion { get; set; }
        public int ExpectedInvokeCalls { get; set; }
        public int ExpectedGetResultCalls { get; set; }
    }

    /// <summary>
    /// Utility methods for writing batch operations.
    /// TODO: merge with old BatchTestUtil, there is a lot of duplication
    /// </summary>
    public static class BatchRequestWritingUtils
    {
        public static string GetActionText(string actionName, params String[] parameters)
        {
            return GetActionText(actionName, new List<KeyValuePair<string, string>>(), parameters);
        }

        public static string GetActionText(string actionName, IEnumerable<KeyValuePair<string, string>> headers, params String[] parameters)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("POST /");
            builder.Append(actionName);
            builder.AppendLine(" HTTP/1.1");
            builder.AppendLine("Host: host");
            builder.AppendLine("Accept: " + UnitTestsUtil.JsonLightMimeType);
            builder.AppendLine("Content-Type: " + UnitTestsUtil.JsonLightMimeType);

            // Write other headers that exist
            foreach (var headerValuePair in headers)
            {
                builder.Append(headerValuePair.Key + ": ");
                builder.AppendLine(headerValuePair.Value);
            }

            builder.Append("Content-Length: ");


            if (parameters.Length > 0)
            {

                // calculate content-length
                // scope length = {\r\n or }\r\n and it's multiplied by 2 for opening and closing scope;
                const int scopesLength = 3;

                // Each parameter has a ,\r\n except the last one has only \r\n
                int parametersDelimeterLength = parameters.Length * 3 - 1;

                int extraContentLength = scopesLength * 2 + parametersDelimeterLength;

                builder.AppendLine(((int)(parameters.Sum(p => p.Length)) + extraContentLength).ToString());

                // write body if any parameters
                builder.AppendLine();
                builder.AppendLine("{");
                foreach (String p in parameters)
                {
                    builder.Append(p);
                    builder.AppendLine(",");
                }
                builder.Remove(builder.Length - 3, 1);
                builder.AppendLine("}");
                builder.AppendLine();
            }
            else
            {
                // no body
                builder.AppendLine(" 0");
            }

            return builder.ToString();
        }

        public static string GetPostJsonRequestText(string url, string jsonBody)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("POST /");
            builder.Append(url);
            builder.AppendLine(" HTTP/1.1");
            builder.AppendLine("Host: host");
            builder.AppendLine("Accept: " + UnitTestsUtil.JsonLightMimeType);
            builder.AppendLine("Content-Type: " + UnitTestsUtil.JsonLightMimeType);
            builder.Append("Content-Length: ");
            builder.AppendLine(jsonBody.Length.ToString());
            builder.AppendLine();
            builder.AppendLine(jsonBody);
            builder.AppendLine();

            return builder.ToString();
        }

        public static string GetPostJsonRequestText(string url, string jsonBody, string contentID)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("POST /");
            builder.Append(url);
            builder.AppendLine(" HTTP/1.1");
            builder.AppendLine("Host: host");
            builder.AppendLine("Accept: " + UnitTestsUtil.JsonLightMimeType);
            builder.AppendLine("Content-Type: " + UnitTestsUtil.JsonLightMimeType);
            builder.Append("Content-Length: ");
            builder.AppendLine(jsonBody.Length.ToString());
            builder.Append("Content-ID: ");
            builder.AppendLine(contentID);
            builder.AppendLine();
            builder.AppendLine(jsonBody);
            builder.AppendLine();

            return builder.ToString();
        }

        public static string GetPutJsonRequestText(string url, string jsonBody)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("PUT /");
            builder.Append(url);
            builder.AppendLine(" HTTP/1.1");
            builder.AppendLine("Host: host");
            builder.AppendLine("Accept: " + UnitTestsUtil.JsonLightMimeType);
            builder.AppendLine("Content-Type: " + UnitTestsUtil.JsonLightMimeType);
            builder.AppendLine("If-Match: *");
            builder.Append("Content-Length: ");
            builder.AppendLine(jsonBody.Length.ToString());
            builder.AppendLine();
            builder.AppendLine(jsonBody);
            builder.AppendLine();

            return builder.ToString();
        }

        public static string GetPutPlainRequestText(string url, string jsonBody)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("PUT /");
            builder.Append(url);
            builder.AppendLine(" HTTP/1.1");
            builder.AppendLine("Host: host");
            builder.AppendLine("Accept: " + UnitTestsUtil.JsonLightMimeType);
            builder.AppendLine("Content-Type: text/plain");
            builder.Append("Content-Length: ");
            builder.AppendLine(jsonBody.Length.ToString());
            builder.AppendLine();
            builder.AppendLine(jsonBody);
            builder.AppendLine();

            return builder.ToString();
        }

        public static string GetBatchText(BatchInfo info, string batchBoundary)
        {
            StringBuilder builder = new StringBuilder();

            // write each batch item
            foreach (BatchItem item in info.Items)
            {
                builder.Append("--");
                builder.AppendLine(batchBoundary);
                item.Write(builder);
            }

            // close the batch
            builder.AppendLine();
            builder.Append("--");
            builder.Append(batchBoundary);
            builder.AppendLine("--");

            return builder.ToString();
        }
    }
}