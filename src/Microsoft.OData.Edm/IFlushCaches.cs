//---------------------------------------------------------------------
// <copyright file="IFlushCaches.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Interface describing anything that can have cached data that might need flushing.
    /// </summary>
    internal interface IFlushCaches
    {
        void FlushCaches();
    }
}
