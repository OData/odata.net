//---------------------------------------------------------------------
// <copyright file="BatchWebRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Text;
    using AstoriaUnitTests.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    public class BatchWebRequest
    {
        public class Changeset
        {
            private List<InMemoryWebRequest> parts = new List<InMemoryWebRequest>();

            public List<InMemoryWebRequest> Parts
            {
                get { return this.parts; }
            }
        }

        private List<InMemoryWebRequest> parts;
        private List<Changeset> changesets;

        public BatchWebRequest()
        {
            this.parts = new List<InMemoryWebRequest>();
            this.changesets = new List<Changeset>();
        }

        public List<InMemoryWebRequest> Parts
        {
            get { return this.parts; }
        }

        public List<Changeset> Changesets
        {
            get { return this.changesets; }
        }

        /// <summary>Creates a batch request from an in-memory request representation.</summary>
        /// <param name="request">The request to parse into a batch.</param>
        /// <returns>The newly created batch request.</returns>
        public static BatchWebRequest FromRequest(InMemoryWebRequest request)
        {
            BatchWebRequest batch = new BatchWebRequest();
            Assert.AreEqual("/$batch", request.RequestUriString, "The request is not a batch request, it does not target the /$batch uri/");
            batch.ParseBatchContent(request.GetRequestStream(), request.RequestContentType, false, false);
            return batch;
        }

        /// <summary>Creates a batch response from an in-memory response representation.</summary>
        /// <param name="response">The in-memory request from which to read the response.</param>
        /// <returns>The newly created batch response.</returns>
        public static BatchWebRequest FromResponse(InMemoryWebRequest response)
        {
            BatchWebRequest batch = new BatchWebRequest();
            if (response.ResponseStatusCode != 200 && response.ResponseStatusCode != 202)
            {
                Assert.Fail("The batch response must be 200 or 202, otherwise its content will not be a batch response.");
            }
            batch.ParseBatchContent(response.GetResponseStream(), response.ResponseContentType, true, false);
            return batch;
        }

        /// <summary>Writes the batch as an HTTP request into a stream (including the verb, headers and everything).</summary>
        /// <param name="stream">The stream to write the request into.</param>
        /// <param name="serviceRoot">The uri of the service root to send the request to.</param>
        public void WriteRequest(Stream stream, Uri serviceRoot)
        {
            InMemoryWebRequest request = new InMemoryWebRequest();
            this.WriteRequest(request);
            request.WriteRequest(stream, serviceRoot);
        }

        /// <summary>Writes the batch as request to the specifies TestWebRequest (sets up properties and such).</summary>
        /// <param name="request">The request to apply the batch as a request to.</param>
        public void WriteRequest(TestWebRequest request)
        {
            request.RequestUriString = "/$batch";
            request.HttpMethod = "POST";
            request.Accept = "*/*";

            this.SetContentTypeAndRequestStream(request);
        }

        public void SetContentTypeAndRequestStream(TestWebRequest request)
        {
            string boundary = "boundary_" + Guid.NewGuid().ToString();
            request.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);
            MemoryStream payload = this.CreateBatchContent(false, boundary);
            request.RequestStream = payload;
        }

        /// <summary>Writes the batch as a response into a stream (including the status line, headers and everything)</summary>
        /// <param name="stream">The stream to write the response to.</param>
        public void WriteResponse(Stream stream)
        {
            InMemoryWebRequest response = new InMemoryWebRequest();
            this.WriteResponse(response);
            response.WriteResponse(stream);
        }

        /// <summary>Writes the batch as a response into an in-memory request (sets up properties and such).</summary>
        /// <param name="request">The request to apply the batch as a response to.</param>
        public void WriteResponse(InMemoryWebRequest request)
        {
            request.SetResponseStatusCode(200);
            string boundary = "boundary_" + Guid.NewGuid().ToString();
            request.ResponseHeaders["Content-Type"] = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);

            request.SetResponseStream(this.CreateBatchContent(true, boundary));
        }

        /// <summary>Using the specified TestWebRequest sends the batch as a request and parses the response back into the batch.</summary>
        /// <param name="request">The request to use for sending the batch.</param>
        public void SendRequest(TestWebRequest request)
        {
            this.WriteRequest(request);
            request.SendRequest();
            this.ParseResponseFromRequest(request, true);
        }

        private MemoryStream CreateBatchContent(bool isResponse, string boundary)
        {
            MemoryStream payload = new MemoryStream();

            foreach (var r in this.parts)
            {
                StringBuilder header = new StringBuilder();
                header.AppendLine("--" + boundary);
                header.AppendLine("Content-Type: application/http");
                header.AppendLine("Content-Transfer-Encoding: binary");
                header.AppendLine();
                InMemoryWebRequest.WriteStringToStream(payload, header.ToString());
                if (isResponse)
                {
                    r.WriteResponse(payload);
                }
                else
                {
                    r.WriteRequest(payload, null);
                }
            }

            int contentId = 0;

            foreach (var c in this.changesets)
            {
                string changesetBoundary = "changeset_" + Guid.NewGuid().ToString();
                StringBuilder header = new StringBuilder();
                header.AppendLine("--" + boundary);
                header.AppendLine("Content-Type: multipart/mixed; boundary=" + changesetBoundary);

                MemoryStream changesetPayload = new MemoryStream();
                foreach (var r in c.Parts)
                {
                    StringBuilder cheader = new StringBuilder();
                    cheader.AppendLine("--" + changesetBoundary);
                    cheader.AppendLine("Content-Type: application/http");
                    cheader.AppendLine("Content-Transfer-Encoding: binary");
                    string contentIdStr;
                    r.ResponseHeaders.TryGetValue("Content-ID", out contentIdStr);
                    cheader.AppendLine("Content-Id: " + contentIdStr ?? (++contentId).ToString());
                    cheader.AppendLine();
                    InMemoryWebRequest.WriteStringToStream(changesetPayload, cheader.ToString());
                    if (isResponse)
                    {
                        r.WriteResponse(changesetPayload);
                    }
                    else
                    {
                        r.WriteRequest(changesetPayload, null);
                    }
                }
                InMemoryWebRequest.WriteStringToStream(changesetPayload, Environment.NewLine + "--" + changesetBoundary + "--" + Environment.NewLine);
                changesetPayload.Position = 0;

                header.AppendLine("Content-Length: " + changesetPayload.Length.ToString());
                header.AppendLine();
                InMemoryWebRequest.WriteStringToStream(payload, header.ToString());
                TestUtil.CopyStream(changesetPayload, payload);
            }

            InMemoryWebRequest.WriteStringToStream(payload, Environment.NewLine + "--" + boundary + "--" + Environment.NewLine);

            payload.Position = 0;
            return payload;
        }

        private bool ParseBatchPart(bool isResponse, TextReader reader, InMemoryWebRequest part, string boundary, string endboundary)
        {
            bool result = false;
            string line;

            if (isResponse)
            {
                part.ParseResponseStatus(reader);
            }
            else
            {
                part.ParseRequestVerb(reader);
            }

            if (isResponse)
            {
                part.ResponseHeaders.Clear();
                InMemoryWebRequest.ParseHeaders(reader, part.ResponseHeaders);
            }
            else
            {
                InMemoryWebRequest.ParseHeaders(reader, part.RequestHeaders);
                InMemoryWebRequest.ApplyHeadersToProperties(part);
            }

            StringBuilder sb = new StringBuilder();
            string lastLine = null;
            while ((line = reader.ReadLine()) != null)
            {
                if (line == boundary)
                {
                    break;
                }
                if (line == endboundary)
                {
                    result = true;
                    break;
                }
                if (lastLine != null)
                {
                    sb.AppendLine(lastLine);
                }
                lastLine = line;
            }
            // The last line must not end with a newline - the batch adds it there, but it's not actually part of the content
            sb.Append(lastLine);
            if (isResponse)
            {
                part.SetResponseStream(new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())));
            }
            else
            {
                part.SetRequestStreamAsText(sb.ToString());
            }
            return result;
        }

        public void ParseResponseFromRequest(TestWebRequest request, bool matchToExisting)
        {
            using (var contentStream = request.GetResponseStream())
            {
                this.ParseBatchContent(contentStream, request.ResponseContentType, true, matchToExisting);
            }
        }

        private void ParseBatchContent(Stream contentStream, string contentType, bool isResponse, bool matchToExisting)
        {
            int partIndex = 0;
            int changesetIndex = 0;

            if (!matchToExisting)
            {
                this.changesets = new List<Changeset>();
                this.parts = new List<InMemoryWebRequest>();
            }

            MemoryStream memoryStream = new MemoryStream(); // Create a copy since StreamReader will Close the stream, which might not be what we want
            TestUtil.CopyStream(contentStream, memoryStream);
            memoryStream.Position = 0;
            using (TextReader reader = new StreamReader(memoryStream))
            {
                Assert.IsTrue(contentType.StartsWith("multipart/mixed; "), "Response is not a batch response. Expecting 'multipart.mixed', got '" + contentType + "'.");
                string boundary = "--" + contentType.Substring(contentType.IndexOf("; boundary=") + 11).Trim();
                string endboundary = boundary + "--";

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == boundary) break;
                    if (line == endboundary)
                    {
                        Assert.IsTrue(partIndex == this.parts.Count, "The response didn't contain enough parts.");
                        return;
                    }
                }

                while (true)
                {
                    line = reader.ReadLine(); // Content-Type
                    Assert.IsTrue(line.StartsWith("Content-Type:"), "The batch part doesn't specify its content type.");
                    contentType = line.Substring("Content-Type:".Length).Trim();
                    if (contentType.StartsWith("multipart/mixed; "))
                    {
                        string changesetBoundary = "--" + contentType.Substring(contentType.IndexOf("; boundary=") + 11).Trim();
                        string changesetEndBoundary = changesetBoundary + "--";
                        int cpartIndex = 0;

                        Changeset changeset;
                        if (matchToExisting)
                        {
                            Assert.IsTrue(this.changesets.Count > changesetIndex, "The batch response contains more changesets than the number of changesets sent.");
                            changeset = this.changesets[changesetIndex];
                        }
                        else
                        {
                            changeset = new Changeset();
                            this.changesets.Add(changeset);
                        }
                        changesetIndex++;

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Trim().Length == 0) break;
                        }

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line == changesetBoundary) break;
                            if (line == changesetEndBoundary)
                            {
                                goto ChangesetEnd;
                            }
                        }

                        while (true)
                        {
                            reader.ReadLine(); // Content-Type
                            reader.ReadLine(); // Content-Transfer-Encoding

                            InMemoryWebRequest part;
                            if (matchToExisting)
                            {
                                Assert.IsTrue(changeset.Parts.Count > cpartIndex, "The batch response contains more parts than the number of request parts sent.");
                                part = changeset.Parts[cpartIndex];
                            }
                            else
                            {
                                part = new InMemoryWebRequest();
                                changeset.Parts.Add(part);
                            }

                            line = reader.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                part.RequestHeaders["Content-ID"] = line.Split(' ')[1];
                                reader.ReadLine();
                            }

                            cpartIndex++;

                            if (ParseBatchPart(isResponse, reader, part, changesetBoundary, changesetEndBoundary))
                            {
                                break;
                            }
                        }
                    ChangesetEnd:
                        if (cpartIndex < changeset.Parts.Count)
                        {
                            int lastStatusCode = changeset.Parts[cpartIndex - 1].ResponseStatusCode;
                            if (!UnitTestsUtil.IsSuccessStatusCode(lastStatusCode))
                            {
                                for (; cpartIndex < changeset.Parts.Count; cpartIndex++)
                                {
                                    changeset.Parts[cpartIndex].SetResponseStatusCode(lastStatusCode);
                                }
                            }
                        }
                        Assert.IsTrue(cpartIndex == changeset.Parts.Count, "The response didn't contain enough parts.");
                        ;

                        while (true)
                        {
                            line = reader.ReadLine();
                            if (line == boundary) break;

                            // We can hit another exception after the changeset.  For example if the changeset fails
                            // and after the server serializes out the error for the changeset, writes the endboundary
                            // for the changeset, and call IUpdatable.ClearChanges(), ClearChanges() can throw.
                            // When that happens we will see the second error but there won't be any boundary after it.
                            // We need to goto End when we either see the end boundary or reached the end of stream.
                            if (line == null || line == endboundary)
                            {
                                goto End;
                            }
                        }
                    }
                    else
                    {
                        Assert.IsTrue(contentType.StartsWith("application/http"), "Batch part is neither changeset nor HTTP.");
                        reader.ReadLine(); // Content-Transfer-Encoding
                        reader.ReadLine(); //

                        InMemoryWebRequest part;
                        if (matchToExisting)
                        {
                            Assert.IsTrue(this.parts.Count > partIndex, "The batch response contains more parts than the number of request parts sent.");
                            part = this.parts[partIndex];
                            partIndex++;
                        }
                        else
                        {
                            part = new InMemoryWebRequest();
                            this.parts.Add(part);
                            partIndex++;
                        }

                        if (ParseBatchPart(isResponse, reader, part, boundary, endboundary))
                        {
                            goto End;
                        }
                    }
                }
            End:
                Assert.IsTrue(partIndex == this.parts.Count, "The response didn't contain enough parts.");
                Assert.IsTrue(changesetIndex == this.changesets.Count, "The response didn't contain enough parts.");
            }
        }
    }
}