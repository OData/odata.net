//---------------------------------------------------------------------
// <copyright file="AstoriaRequestResponseBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    public abstract class AstoriaRequestResponseBase
    {
        protected AstoriaRequestResponseBase(Workspace w)
        {
            Headers = new Dictionary<string, string>();
            Workspace = w;
        }

        public Dictionary<string, string> Headers
        {
            get;
            protected set;
        }

        public virtual string Payload
        {
            get;
            set;
        }

        public Workspace Workspace
        {
            get;
            protected set;
        }

        public string DataServiceVersion
        {
            get
            {
                string value = null;
                Headers.TryGetValue("OData-Version", out value);
                return value;
            }
            set
            {
                Headers["OData-Version"] = value;
            }
        }

        public string MaxDataServiceVersion
        {
            get
            {
                string value = null;
                Headers.TryGetValue("OData-MaxVersion", out value);
                return value;
            }
            set
            {
                Headers["OData-MaxVersion"] = value;
            }
        }

        public virtual string ETagHeader
        {
            get
            {
                return Headers["ETag"];
            }
            set
            {
                Headers["ETag"] = value;
            }
        }

        public virtual string ContentType
        {
            get
            {
                return Headers["Content-Type"];
            }
            set
            {
                Headers["Content-Type"] = value;
            }
        }

        private CommonPayload _CommonPayload = null;

        public CommonPayload CommonPayload
        {
            get
            {
                if (_CommonPayload == null)
                {
                    _CommonPayload = CommonPayload.CreateCommonPayload(this);
                }
                return _CommonPayload;
            }
        }
    }
}
