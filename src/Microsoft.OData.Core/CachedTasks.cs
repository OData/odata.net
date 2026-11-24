//---------------------------------------------------------------------
// <copyright file="CachedTasks.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Threading.Tasks;
    using Microsoft.OData.Json;

    internal static class CachedTasks
    {
        internal static readonly Task Completed = Task.FromResult(true);
        internal static readonly Task<bool> True = Task.FromResult(true);
        internal static readonly Task<bool> False = Task.FromResult(false);
        internal static readonly Task<JsonNodeType> StartObject = Task.FromResult(JsonNodeType.StartObject);
        internal static readonly Task<JsonNodeType> StartArray = Task.FromResult(JsonNodeType.StartArray);
        internal static readonly Task<JsonNodeType> PrimitiveValue = Task.FromResult(JsonNodeType.PrimitiveValue);
    }
}
