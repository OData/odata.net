//---------------------------------------------------------------------
// <copyright file="IObjectQueryWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider
{
    using System.Data.Objects;

    interface IObjectQueryWrapper
    {
        ObjectQuery ObjectQuery { get; }
    }
}
