//---------------------------------------------------------------------
// <copyright file="UriComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Tests
{
    public class UriComparer<T> : IEqualityComparer<Uri>
    {
        public bool Equals(Uri x, Uri y)
        {
            return Uri.Compare(x, y, UriComponents.Host | UriComponents.PathAndQuery,
            UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0 ||
            x.AbsoluteUri.Trim().Replace("[", "%5B").Replace("]", "%5D").Replace("'", "%27").Replace("*", "%2A").Replace("(", "%28").Replace(")", "%29").Equals(y.AbsoluteUri.Trim().Replace("'", "%27").Replace("*", "%2A").Replace("(", "%28").Replace(")", "%29").Replace("[", "%5B").Replace("]", "%5D"), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(Uri obj)
        {
            return 0;
        }
    }
}
