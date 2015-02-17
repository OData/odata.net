//---------------------------------------------------------------------
// <copyright file="PayloadProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    public abstract class PayloadProperty
    {
        public PayloadProperty(PayloadObject parent)
        {
            this.ParentObject = parent;
            this.MappedOutOfContent = false; //by default, assume not
        }

        public PayloadObject ParentObject
        {
            get;
            internal set;
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

        public bool IsNull
        {
            get;
            set;
        }

        internal bool MappedOutOfContent
        {
            get;
            set;
        }
    }

    public class PayloadSimpleProperty : PayloadProperty
    {
        public PayloadSimpleProperty(PayloadObject parent)
            : base(parent)
        { }
    
        public string Value
        {
            get;
            set;
        }
    }

    public class PayloadComplexProperty : PayloadProperty
    {
        public PayloadComplexProperty(PayloadObject parent)
            : base(parent)
        {
            PayloadProperties = new Dictionary<string, PayloadProperty>();
        }

        public Dictionary<string, PayloadProperty> PayloadProperties
        {
            get;
            private set;
        }
    }
}
