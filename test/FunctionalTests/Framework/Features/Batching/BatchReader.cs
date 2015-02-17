//---------------------------------------------------------------------
// <copyright file="BatchReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;       //IEnumerable<T>
using System.Collections;               //IEnumerator
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using System.IO;

namespace System.Data.Test.Astoria
{
    public static class BatchReader
    {
        private class ResponseFragment
        {
            public string ContentID;
            public string Payload;
            public string ContentType;
            public HttpStatusCode StatusCode;
            public Dictionary<string, string> Headers;
        }

        public static void ParseBatchResponse(BatchResponse response)
        {
            string contentType = response.ContentType;
            string[] pieces = contentType.Split(new string[] { "; boundary=" }, StringSplitOptions.None);
            if (pieces.Length < 2)
                AstoriaTestLog.FailAndThrow(String.Format("Could not retrieve boundary from response's content-type header. Value found was '{0}'", contentType));
            string boundary = pieces[1].Trim();

            // even if content-id is specified, we may not get it back in error cases. This doesn't feel entirely correct, it seems like there should 
            // always be a 1-1 mapping that explicitly honors content-id's

            // TODO: responses / requests without content IDs should be matched up in order within changeset only
            // changesets should have the right number of responses too

            BatchRequest batchRequest = response.Request as BatchRequest;

            List<ResponseFragment> unmatchedFragments = new List<ResponseFragment>();
            List<AstoriaRequest> unmatchedRequests = batchRequest.Requests
                .Union(batchRequest.Changesets.SelectMany(changeset => changeset.AsEnumerable()))
                .ToList();

            using (StringReader reader = new StringReader(response.Payload))
            {
                foreach (ResponseFragment fragment in ParseBatchResponse(reader, boundary))
                {
                    AstoriaRequest request = null;

                    if (fragment.ContentID != null)
                        request = unmatchedRequests
                            .FirstOrDefault(r => r.Headers.ContainsKey("Content-ID")
                                && r.Headers["Content-ID"].Equals(fragment.ContentID));

                    if (request == null)
                        unmatchedFragments.Add(fragment);
                    else
                    {
                        unmatchedRequests.Remove(request);
                        response.Responses.Add(FragmentToResponse(request, fragment));
                    }
                }
            }

            if (unmatchedFragments.Any())
            {
                if (unmatchedFragments.Count < unmatchedRequests.Count)
                    AstoriaTestLog.WriteLine("Warning: recieved fewer batch response fragments than expected");
                else if (unmatchedFragments.Count > unmatchedRequests.Count)
                    AstoriaTestLog.FailAndThrow("Recieved more batch fragments than expected");

                for (int i = 0; i < unmatchedFragments.Count; i++)
                {
                    response.Responses.Add(FragmentToResponse(unmatchedRequests[i], unmatchedFragments[i]));
                }
            }
        }

        private static AstoriaResponse FragmentToResponse(AstoriaRequest request, ResponseFragment fragment)
        {
            AstoriaResponse response = new AstoriaResponse(request);
            response.Payload = fragment.Payload;
            response.ContentType = fragment.ContentType;
            response.ActualStatusCode = fragment.StatusCode;
            response.ETagHeaderFound = false;
            foreach (var header in fragment.Headers)
            {
                if (header.Key == "ETag")
                    response.ETagHeaderFound = true;
                response.Headers[header.Key] = header.Value;
            }
            return response;
        }

        public static BatchResponse ParseBatchResponse(BatchRequest batchRequest, StringReader reader, string boundary)
        {
            // even if content-id is specified, we may not get it back in error cases. This doesn't feel entirely correct, it seems like there should 
            // always be a 1-1 mapping that explicitly honors content-id's

            // TODO: responses / requests without content IDs should be matched up in order within changeset only
            // changesets should have the right number of responses too

            BatchResponse response = null;// new BatchResponse(batchRequest);
            List<ResponseFragment> unmatchedFragments = new List<ResponseFragment>();
            List<AstoriaRequest> unmatchedRequests = batchRequest.Requests
                .Union(batchRequest.Changesets.SelectMany(changeset => changeset.AsEnumerable()))
                .ToList();

            foreach (ResponseFragment fragment in ParseBatchResponse(reader, boundary))
            {
                AstoriaRequest request = null;

                if (fragment.ContentID != null)
                    request = unmatchedRequests
                        .FirstOrDefault(r => r.Headers.ContainsKey("Content-ID")
                            && r.Headers["Content-ID"].Equals(fragment.ContentID));
                    
                if (request == null)
                    unmatchedFragments.Add(fragment);
                else
                {
                    unmatchedRequests.Remove(request);

                    response.Responses.Add(FragmentToResponse(request, fragment));
                }
            }

            if (unmatchedFragments.Any())
            {
                if (unmatchedFragments.Count < unmatchedRequests.Count)
                    AstoriaTestLog.WriteLine("Warning: recieved fewer batch response fragments than expected");
                else if (unmatchedFragments.Count > unmatchedRequests.Count)
                    AstoriaTestLog.FailAndThrow("Recieved more batch fragments than expected");

                for (int i = 0; i < unmatchedFragments.Count; i++)
                {
                    response.Responses.Add(FragmentToResponse(unmatchedRequests[i], unmatchedFragments[i]));
                }
            }

            return response;
        }

        private static IEnumerable<ResponseFragment> ParseBatchResponse(StringReader reader, string boundary)
        {
            //--batchresponse_aa2a62c3-d5f4-447e-9302-d31f062b02f7
            //Content-Type: multipart/mixed; boundary=changesetresponse_1837899f-f0ce-447e-a980-d2596a561051

            string boundaryLine = reader.ReadLine();
            if (!boundaryLine.Contains(boundary))
                AstoriaTestLog.FailAndThrow(String.Format("Unexpected line in batch response: '{0}', expected boundary '{1}'", boundaryLine, boundary));

            string headerLine;
            while (null != (headerLine = reader.ReadLine()))
            {
                Match match = Regex.Match(headerLine, @"Content-Type: multipart/mixed; boundary=(\S+)");

                if (match.Success)
                {
                    // eat the extra newline
                    reader.ReadLine();
                    
                    foreach (ResponseFragment fragment in ParseChangeSet(reader, match.Groups[1].Value, boundary))
                        yield return fragment;
                }
                else
                {
                    // this might be a batch get instead

                    //--batch(36522ad7-fc75-4b56-8c71-56071383e77b)
                    //Content-Type: application/http
                    //Content-Transfer-Encoding:binary

                    if (!Regex.IsMatch(headerLine, @"Content-Type:\s+application/http"))
                        AstoriaTestLog.FailAndThrow(String.Format("Could not parse batch content type or boundary, line was '{0}'", headerLine));

                    headerLine = reader.ReadLine();
                    if (!Regex.IsMatch(headerLine, @"Content-Transfer-Encoding:\s+binary"))
                        AstoriaTestLog.FailAndThrow(String.Format("Missing content-transfer-encoding, line was '{0}'", headerLine));

                    // eat the extra newline
                    reader.ReadLine();

                    yield return ParseBatchFragment(reader, boundary);
                }
            }
        }

        private static IEnumerable<ResponseFragment> ParseChangeSet(StringReader reader, string changeSetBoundary, string batchBoundary)
        {
            // first line should be the boundary
            // second line should be content type
            // third line should be content-transfer-encoding

            //--changesetresponse_1837899f-f0ce-447e-a980-d2596a561051
            //Content-Type: application/http
            //Content-Transfer-Encoding: binary

            string boundaryLine = reader.ReadLine();
            if (!boundaryLine.Contains(changeSetBoundary))
                AstoriaTestLog.FailAndThrow(String.Format("Unexpected line in batch response: '{0}', expected boundary '{1}'", boundaryLine, changeSetBoundary));

            string line;
            while (null != (line = reader.ReadLine()) && !line.Contains(batchBoundary))
            {
                if (!Regex.IsMatch(line, @"Content-Type:\s*application/http"))
                    AstoriaTestLog.FailAndThrow(String.Format("Could not parse change set content type, line was '{0}'", line));

                line = reader.ReadLine();
                if (!Regex.IsMatch(line, @"Content-Transfer-Encoding:\s*binary"))
                    AstoriaTestLog.FailAndThrow(String.Format("Missing change set content-transfer-encoding, line was '{0}'", line));

                // eat the newline
                reader.ReadLine();

                yield return ParseBatchFragment(reader, changeSetBoundary);
            }
        }

        private static ResponseFragment ParseBatchFragment(StringReader reader, string boundary)
        {
            //HTTP/1.1 200 Ok
            //Content-Type: application/atom+xml;type=entry
            //Content-Length: ###

            string line = reader.ReadLine();
            Match match = Regex.Match(line, @"HTTP/1.1 ([0-9]{3}).*");

            if (!match.Success)
                AstoriaTestLog.FailAndThrow(String.Format("Expected HTTP protocol and status code, got '{0}'", line));

            HttpStatusCode statusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), match.Groups[1].Value);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            while (null != (line = reader.ReadLine())
                && !line.Contains(boundary)
                && (match = Regex.Match(line, @"([A-Za-z-]+):\s*(.+)")).Success)
            {
                string headerString = match.Groups[1].Value;//.Replace("-", String.Empty).ToLowerInvariant();
                string value = match.Groups[2].Value;

                headers.Add(headerString, value);
            }

            // the rest is payload
            StringBuilder payload = new StringBuilder();
            while (null != (line = reader.ReadLine()) && !line.Contains(boundary))
                payload.AppendLine(line);

            ResponseFragment fragment = new ResponseFragment()
            {
                ContentID = null,
                ContentType = "application/xml", // will be overwritten if present
                Payload = payload.ToString().Trim(),
                StatusCode = statusCode,
                Headers = headers
            };

            headers.TryGetValue("Content-Type", out fragment.ContentType);
            headers.TryGetValue("Content-ID", out fragment.ContentID);

            return fragment;
        }
    }
}
