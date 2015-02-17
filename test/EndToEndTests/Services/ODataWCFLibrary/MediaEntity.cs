//---------------------------------------------------------------------
// <copyright file="MediaEntity.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public abstract class MediaEntity : ClrObject
    {
        public MediaEntity()
        {
            this.ETagValue = DateTime.UtcNow.Ticks;
        }

        public virtual long Id { get; set; }

        public virtual string ContentType { get; set; }

        public virtual Stream Stream { get; set; }

        public virtual long ETagValue { get; set; }

        public virtual string ETag
        {
            get { return Utility.FormatETagValueWeak(this.ETagValue); }
        }
    }
}
