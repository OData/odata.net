//---------------------------------------------------------------------
// <copyright file="PayloadNamedStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    public class PayloadNamedStream
    {
        public string Name
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }

        public string EditLink
        {
            get;
            set;
        }

        public string SelfLink
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string ETag
        {
            get;
            set;
        }
    }
}
