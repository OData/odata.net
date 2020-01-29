//---------------------------------------------------------------------
// <copyright file="TupleInternal.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    internal static class TupleInternal
    {
        public static TupleInternal<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new TupleInternal<T1, T2>(item1, item2);
        }
    }

    internal class TupleInternal<T1, T2>
    {
        public TupleInternal(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        public T1 Item1
        {
            get; private set;
        }

        public T2 Item2
        {
            get; private set;
        }
    }
}
