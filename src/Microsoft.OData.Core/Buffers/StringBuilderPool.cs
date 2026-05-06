//---------------------------------------------------------------------
// <copyright file="StringBuilderPool.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Text;
    using Microsoft.Extensions.ObjectPool;

    /// <summary>Shared pool of <see cref="StringBuilder"/> for hot serialization paths.</summary>
    internal static class StringBuilderPool
    {
        public static readonly ObjectPool<StringBuilder> Shared =
            new DefaultObjectPoolProvider { MaximumRetained = 16 }
                .CreateStringBuilderPool(initialCapacity: 64, maximumRetainedCapacity: 4 * 1024);

        public static string Build(System.Action<StringBuilder> build)
        {
            StringBuilder sb = Shared.Get();
            try
            {
                build(sb);
                return sb.ToString();
            }
            finally
            {
                Shared.Return(sb);
            }
        }
    }
}
