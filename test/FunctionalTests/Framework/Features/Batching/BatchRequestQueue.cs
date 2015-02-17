//---------------------------------------------------------------------
// <copyright file="BatchRequestQueue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    public class BatchRequestQueue
    {
        // Note: using this across workspaces is possible, but not supported

        public BatchRequestQueue(uint size) : this(size, true)
        {
        }

        public BatchRequestQueue(uint size, bool autoSubmit)
        {
            Size = size;
            AutoSubmit = autoSubmit;
        }

        private BatchRequest currentBatch = null;
        private BatchChangeset currentChangeset = null;

        public uint Size
        {
            get;
            private set;
        }

        public bool AutoSubmit
        {
            get;
            private set;
        }

        public void Add(AstoriaRequest request)
        {
            if (currentBatch == null)
                currentBatch = new BatchRequest(request.Workspace);

            if (request.Verb == RequestVerb.Get)
            {
                currentBatch.Add(request);
            }
            else
            {
                if (currentChangeset == null)
                    currentChangeset = currentBatch.GetChangeset();
                currentChangeset.Add(request);
            }

            if (currentBatch.TotalRequests >= Size && AutoSubmit)
                Finish();
        }

        public void Finish()
        {
            if(currentBatch != null)
                currentBatch.GetResponse().Verify();

            currentBatch = null;
            currentChangeset = null;
        }
    }
}
