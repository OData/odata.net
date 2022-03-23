//---------------------------------------------------------------------
// <copyright file="ObjectPool.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData
{
    /// <summary>
    /// Manages a pool of reusable objects. An object retrieved from the pool should be returned when it's no longer being used so that it can be later recycled.
    /// </summary>
    /// <typeparam name="T">The type of objects to pool.</typeparam>
    /// <remarks>This object pool is not thread-safe and not intended to be shared across concurrent requests.
    /// Consider using the DefaultObjectPool in 8.x once we drop support for netstandard 1.1
    /// </remarks>
    internal class ObjectPool<T> where T : class
    {
        private readonly Func<T> objectGenerator;
        private ObjectWrapper[] items;
        private T firstItem;

        // The pool size is equal to the level of nesting in the response.
        // 32 is an arbitrary figure. Could be adjusted appropriately.
        private const int POOL_SIZE = 8;

        /// <summary>
        /// To initialize the object pool.
        /// </summary>
        /// <param name="objectGenerator">Used to create an instance of a <typeparamref name="T"/>.</param>
        public ObjectPool(Func<T> objectGenerator)
        {
            this.objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
        }

        /// <summary>
        /// Gets an object from the pool if one is available, otherwise creates one. The object will need to be returned to the pool when no longer in use.
        /// </summary>
        /// <returns>An object of type <typeparamref name="T"/> from the pool.</returns>
        /// <remarks>It's the responsibility of the caller to clean up the object before using it.</remarks>
        public T Get()
        {
            T item = firstItem;

            if (item != null)
            {
                firstItem = default(T);
                return item;
            }

            if (items == null)
            {
                items = new ObjectWrapper[POOL_SIZE];
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
        /// <param name="obj">The object to return to the pool.</param> 
        public void Return(T obj)
        {
            if (firstItem == null)
            {
                firstItem = obj;
                return;
            }

            for (int i = 0; i < items.Length; ++i)
            {
                if (items[i].Element == null)
                {
                    items[i].Element = obj;
                    break;
                }
            }
        }

        // PERF: the struct wrapper avoids array-covariance-checks from the runtime when assigning to elements of the array.
        // See comment in this PR https://github.com/dotnet/extensions/pull/314#discussion_r169390645
        private struct ObjectWrapper
        {
            public T Element;
        }
    }
}
