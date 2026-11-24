//---------------------------------------------------------------------
// <copyright file="ReadResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>A portable value-type pair for asynchronous reader results.</summary>
    internal struct ReadResult<TFirst, TSecond>
    {
        internal readonly TFirst Item1;
        internal readonly TSecond Item2;

        internal ReadResult(TFirst first, TSecond second)
        {
            this.Item1 = first;
            this.Item2 = second;
        }
    }

    internal static class ReadResult
    {
        internal static ReadResult<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second)
        {
            return new ReadResult<TFirst, TSecond>(first, second);
        }
    }
}
