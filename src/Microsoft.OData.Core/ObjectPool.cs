//---------------------------------------------------------------------
// <copyright file="ObjectPool.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData
{
    internal class ObjectPool<T>
    {
        private readonly Func<T> objectGenerator;
        private readonly ObjectWrapper[] items;

        public ObjectPool(Func<T> objectGenerator)
        {
            this.objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
            items = new ObjectWrapper[32];
        }
        public T Get()
        {
            for (int i = 0; i < items.Length; i++)
            {
                 T item = items[i].Element;
                if (item != null)
                {
                    items[i].Element = default(T);
                    return item;
                }
            }

            return objectGenerator();
        }

        public void Return(T obj)
        {
            for (var i = 0; i < items.Length; ++i)
            {
                if(items[i].Element == null)
                {
                    items[i].Element = obj;
                    break;
                }
            }
        }

        // PERF: the struct wrapper avoids array-covariance-checks from the runtime when assigning to elements of the array.
        private struct ObjectWrapper
        {
            public T Element;
            public ObjectWrapper(T item) => Element = item;
            public static implicit operator T(ObjectWrapper wrapper) => wrapper.Element;
        }
    }
}
