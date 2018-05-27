//---------------------------------------------------------------------
// <copyright file="PayloadObject.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Collections.Generic;
    using System.Linq;

    public class PayloadObject
    {
        public PayloadObject(CommonPayload parent)
        {
            Payload = parent;
            PayloadObjects = new List<PayloadObject>();
            PayloadProperties = new List<PayloadProperty>();
            NamedStreams = new List<PayloadNamedStream>();
            CustomEpmMappedProperties = new Dictionary<string, string>();
        }

        public CommonPayload Payload
        {
            get;
            private set;
        }

        public List<PayloadObject> PayloadObjects
        {
            get;
            private set;
        }

        public List<PayloadProperty> PayloadProperties
        {
            get;
            private set;
        }

        public List<PayloadNamedStream> NamedStreams
        {
            get;
            private set;
        }

        public PayloadProperty this[string propertyName]
        {
            get
            {
                return PayloadProperties.Single(p => p.Name == propertyName);
            }
        }

        private string _uri = null;
        public string Uri
        {
            get
            {
                return _uri;
            }
            set
            {
                _uri = value;
                if (_uri == null)
                    AbsoluteUri = null;
                else if (_uri.StartsWith(Payload.Workspace.ServiceUri))
                    AbsoluteUri = _uri;
                else
                    AbsoluteUri = Payload.Workspace.ServiceUri + "/" + _uri;
            }
        }

        public string AbsoluteUri
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public bool Deferred
        {
            get;
            set;
        }

        public bool Reference
        {
            get;
            set;
        }

        public string ETag
        {
            get;
            set;
        }

        public SerializationFormatKind Format
        {
            get
            {
                return Payload.Format;
            }
        }

        public Dictionary<string, string> CustomEpmMappedProperties
        {
            get;
            private set;
        }
    }
}
