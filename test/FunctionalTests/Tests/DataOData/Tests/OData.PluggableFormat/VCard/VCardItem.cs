//---------------------------------------------------------------------
// <copyright file="VCardItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat.VCard
{
    using System.Collections.Generic;

    internal class VCardItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Groups { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }
}
