//---------------------------------------------------------------------
// <copyright file="ObjectPool.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData
{
    /// <summary>
    /// A pool of objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to pool.</typeparam>
    /// <remarks>This object pool is not thread-safe and not intended to be shared across concurrent requests.
    /// Consider using the DefaultObjectPool in 8.x once we drop support for netstandard 1.1
    /// </remarks>
    internal class ObjectPool<T>
    {
        private readonly Func<T> objectGenerator;
        private readonly ObjectWrapper[] items;
        private T firstItem;

        /// <summary>
        /// To initialize the object pool.
        /// </summary>
        /// <param name="objectGenerator">Used to create an instance of a <typeparamref name="T"/>.</param>
        public ObjectPool(Func<T> objectGenerator)
        {
            this.objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
            items = new ObjectWrapper[32];
        }

        /// <summary>
        /// Gets an object from the pool if one is available, otherwise creates one.
        /// </summary>
        /// <returns>A <typeparamref name="T"/>.</returns>
        public T Get()
        {
            T item = firstItem;

            if (item != null)
            {
                firstItem = default(T);
                return item;
            }

            for (int i = 0; i < items.Length; i++)
            {
                item = items[i].Element;

                if (item != null)
                {
                    items[i].Element = default(T);
                    return item;
                }
            }

            return objectGenerator();
        }

        /// <summary>
        /// Return an object to the pool.
        /// </summary>
        /// <param name="obj">The object to add to the pool.</param>
        public void Return(T obj)
        {
            if (firstItem == null)
            {
                firstItem = obj;
            }
            else
            {
                for (var i = 0; i < items.Length; ++i)
                {
                    if (items[i].Element == null)
                    {
                        items[i].Element = obj;
                        break;
                    }
                }
            }
        }

        // PERF: the struct wrapper avoids array-covariance-checks from the runtime when assigning to elements of the array.
        private struct ObjectWrapper
        {
            public T Element;
        }
    }
}
