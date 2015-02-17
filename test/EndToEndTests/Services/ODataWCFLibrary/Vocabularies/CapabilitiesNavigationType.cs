//---------------------------------------------------------------------
// <copyright file="CapabilitiesNavigationType.cs" company="Microsoft">
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
    public enum CapabilitiesNavigationType
    {
        Recursive,
        Single,
        None
    }
}
