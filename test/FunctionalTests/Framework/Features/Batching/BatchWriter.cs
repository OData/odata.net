//---------------------------------------------------------------------
// <copyright file="BatchWriter.cs" company="Microsoft">
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
    public static class BatchWriter
    {
        public static void WriteBatchRequest(BatchRequest batchRequest, StringWriter writer)
        {
            //--batchresponse_aa2a62c3-d5f4-447e-9302-d31f062b02f7
            //Content-Type: multipart/mixed; boundary=changesetresponse_1837899f-f0ce-447e-a980-d2596a561051

            string batchBoundary = "batch_" + batchRequest.Identifier;

            foreach (BatchChangeset changeset in batchRequest.Changesets)
            {
                string setBoundary = "changeset_" + changeset.Identifier;
                writer.Write("--");
                writer.WriteLine(batchBoundary);
                writer.WriteLine("Content-Type: " + RequestUtil.RandomizeContentTypeCapitalization("multipart/mixed; boundary=" + setBoundary));
                writer.WriteLine();

                foreach (AstoriaRequest request in changeset)
                {
                    writer.Write("--");
                    writer.WriteLine(setBoundary);

                    WriteBatchRequestFragment(request, writer);
                }

                writer.Write("--");
                writer.Write(setBoundary);
                writer.WriteLine("--");
            }

            foreach (AstoriaRequest request in batchRequest.Requests)
            {
                // TODO: newline?
                writer.Write("--");
                writer.WriteLine(batchBoundary);
                // TODO: newline?

                WriteBatchRequestFragment(request, writer);
            }

            writer.Write("--");
            writer.Write(batchBoundary);
            writer.WriteLine("--");
        }

        public static void WriteBatchRequestFragment(AstoriaRequest request, StringWriter writer)
        {
            //  assume boundary has been written
            //Content-Type: application/http
            //Content-Transfer-Encoding:binary
            writer.WriteLine("Content-Type: " + RequestUtil.RandomizeContentTypeCapitalization("application/http"));
            writer.WriteLine("Content-Transfer-Encoding:binary");
            writer.WriteLine();

            //GET {uri} HTTP/1.1
            //Content-Type: application/atom+xml;type=entry
            //Content-Length: ###
            
            string uri;
            if(request.UseRelativeUri)
                uri = request.RelativeUri;
            else
                uri = request.URI;
            
            writer.WriteLine("{0} {1} HTTP/1.1", request.Verb.ToString().ToUpper(), Uri.EscapeUriString(uri));

            foreach (KeyValuePair<string, string> header in request.Headers)
                writer.WriteLine("{0}: {1}", header.Key, header.Value);

            writer.WriteLine();
            if(request.Payload != null)
                writer.WriteLine(request.Payload);
        }
    }
}
