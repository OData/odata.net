//---------------------------------------------------------------------
// <copyright file="BatchRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;
    using System.Text;

    public class BatchChangeset : IEnumerable<AstoriaRequest>
    {
        private List<AstoriaRequest> _requests = new List<AstoriaRequest>();
        private BatchRequest _parent;

        public string Identifier
        {
            get;
            set;
        }

        internal BatchChangeset(BatchRequest parent, string id)
            : base()
        {
            _parent = parent;
            Identifier = id;
        }

        internal BatchChangeset(BatchRequest parent)
            : this(parent, Guid.NewGuid().ToString())
        { }

        public void Add(AstoriaRequest request)
        {
            Add(request, true);
        }

        public void Add(AstoriaRequest request, bool generateContentID)
        {
            Add(request, (generateContentID ? _parent.TotalRequests.ToString() : null), false);
        }

        public void Add(AstoriaRequest request, string contentID)
        {
            Add(request, contentID, false);
        }

        public void Add(AstoriaRequest request, string contentID, bool includeIfNull)
        {
            if (includeIfNull || contentID != null)
                request.Headers["Content-ID"] = contentID;
            request.Batched = true;
            _requests.Add(request);
        }

        public int Count
        {
            get
            {
                return _requests.Count;
            }
        }

        #region IEnumerable<AstoriaRequest> Members

        IEnumerator<AstoriaRequest> IEnumerable<AstoriaRequest>.GetEnumerator()
        {
            return _requests.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _requests.GetEnumerator();
        }

        #endregion
    }

    public class BatchRequest : AstoriaRequest
    {
        private List<BatchChangeset> _changesets;
        private List<AstoriaRequest> _requests;

        public int TotalRequests
        {
            get
            {
                return _changesets.Sum(set => set.Count) + _requests.Count;
            }
        }

        public IEnumerable<BatchChangeset> Changesets
        {
            get
            {
                return _changesets.AsEnumerable();
            }
        }

        public IEnumerable<AstoriaRequest> Requests
        {
            get
            {
                return _requests.AsEnumerable();
            }
        }

        public BatchChangeset GetChangeset()
        {
            BatchChangeset set = new BatchChangeset(this);
            _changesets.Add(set);
            return set;
        }

        public BatchChangeset GetChangeset(string id)
        {
            BatchChangeset set = new BatchChangeset(this, id);
            _changesets.Add(set);
            return set;
        }

        public void Add(BatchChangeset changeset)
        {
            _changesets.Add(changeset);
        }

        public void Add(AstoriaRequest request)
        {
            Add(request, true);
        }

        public void Add(AstoriaRequest request, bool generateContentID)
        {
            Add(request, (generateContentID ? TotalRequests.ToString() : null), false);
        }

        public void Add(AstoriaRequest request, string contentID)
        {
            Add(request, contentID, false);
        }

        public void Add(AstoriaRequest request, string contentID, bool includeIfNull)
        {
            if(includeIfNull || contentID != null) 
                request.Headers["Content-ID"] = contentID;
            request.Batched = true;
            _requests.Add(request);
        }

        public string Identifier
        {
            get;
            set;
        }

        public BatchRequest(Workspace w, string id)
            : base(w)
        {
            Identifier = id;
            _changesets = new List<BatchChangeset>();
            _requests = new List<AstoriaRequest>();
            Verb_Internal = RequestVerb.Post;
            Headers["Mime-Version"] = "1.0";
        }

        public BatchRequest(Workspace w)
            : this(w, Guid.NewGuid().ToString())
        { }

        public override AstoriaResponse GetResponse()
        {
            LogRequest();

            foreach (AstoriaRequest subRequest in Changesets.SelectMany(c => c).Union(this.Requests))
                subRequest.OnSend(this);

#if !ClientSKUFramework

            // NOTHING should come in between this and actually sending the request
            SetupAPICallLog();
#endif
            AstoriaResponse response = RequestSender.SendRequest(this);

#if !ClientSKUFramework
            // NOTHING should come in between this and actually recieving the response
            RetrieveAPICallLog();
#endif
            BatchResponse batchResponse = new BatchResponse(this, response);

            foreach (AstoriaResponse subResponse in batchResponse.Responses)
                subResponse.Request.OnReceive(this, subResponse);

            return batchResponse;
        }

        protected override void RefreshPayload()
        {
            StringBuilder sb = new StringBuilder();
            BatchWriter.WriteBatchRequest(this, new StringWriter(sb));
            Payload_Internal = sb.ToString();
        }

        public override void LogRequest(StringBuilder builder, bool logPayload)
        {
            base.LogRequest(builder, false);
            builder.AppendLine();

            builder.Append('-', 10);
            builder.AppendFormat(" Batch({0}) ", this.Identifier);
            builder.Append('-', 10);
            builder.AppendLine();

            foreach (BatchChangeset changeset in this.Changesets)
            {
                builder.Append('-', 20);
                builder.AppendFormat(" Changeset({0}) ", changeset.Identifier);
                builder.Append('-', 20);
                builder.AppendLine();

                foreach (AstoriaRequest request in changeset)
                {
                    request.LogRequest(builder, logPayload);
                }
                builder.Append('-', 20);
                builder.Append(" End Changeset ");
                builder.Append('-', 20);
                builder.AppendLine();
            }

            foreach (AstoriaRequest request in this.Requests )
            {
                request.LogRequest(builder, logPayload);
            }
            builder.Append('-', 10);
            builder.Append(" End Batch ");
            builder.Append('-', 10);
            builder.AppendLine();
        }

        protected override void RefreshAccept()
        {
            Accept_Internal = "*/*";
        }

        protected override void RefreshContentType()
        {
            ContentType_Internal = "multipart/mixed; boundary=batch_" + this.Identifier;
        }

        protected override void RefreshURI()
        {
            URI_Internal = Workspace.ServiceUri + "/$batch";
        }

        protected override void RefreshExpectedStatusCode()
        {
            ExpectedStatusCode_Internal = HttpStatusCode.Accepted;
        }
    }
}