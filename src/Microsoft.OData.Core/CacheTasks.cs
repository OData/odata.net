//---------------------------------------------------------------------
// <copyright file="CachedTasks.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Microsoft.OData.Json;
    #endregion Namespaces

    internal static class CachedTasks
    {
        internal static readonly Task<object> ObjectNull = Task.FromResult<object>(null);

        internal static readonly Task<object> ObjectTrue = Task.FromResult<object>(true);

        internal static readonly Task<object> ObjectFalse = Task.FromResult<object>(false);

        internal static readonly Task<JsonNodeType> StartObject = Task.FromResult(JsonNodeType.StartObject);

        internal static readonly Task<JsonNodeType> StartArray = Task.FromResult(JsonNodeType.StartArray);

        internal static readonly Task<JsonNodeType> PrimitiveValue = Task.FromResult(JsonNodeType.PrimitiveValue);

        internal static readonly Task<JsonNodeType> Property = Task.FromResult(JsonNodeType.Property);
    }
}
