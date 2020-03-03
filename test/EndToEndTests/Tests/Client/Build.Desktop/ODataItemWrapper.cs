//---------------------------------------------------------------------
// <copyright file="ODataItemWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData;
using my = Microsoft.Test.OData.Tests.Client;

namespace Microsoft.Test.OData.Tests.Client
{
    public abstract class ODataItemWrapper

    {
        public abstract ODataItem Item { get; }
    }
}