//---------------------------------------------------------------------
// <copyright file="CorePermission.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Vocabularies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    [Flags]
    public enum CorePermission
    {
        None = 0,
        Read = 1,
        ReadWrite = 3
    }
}
