//---------------------------------------------------------------------
// <copyright file="ObjectPool.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData
{
    internal class ObjectPool<T>
    {
        private readonly List<T> _objectsAvailable;
        private readonly Func<T> _objectGenerator;

        public ObjectPool(Func<T> objectGenerator)
        {
            _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
            _objectsAvailable = new List<T>();
        }
        public T GetObject()
        {
            int listSize = _objectsAvailable.Count;
            if (listSize != 0)
            {
                var obj = _objectsAvailable[listSize - 1];
                _objectsAvailable.RemoveAt(listSize - 1);

                return obj;
            }
            else
            {
                T obj = _objectGenerator();

                return obj;
            }
        }

        public void ReleaseObject(T obj)
        {
            _objectsAvailable.Add(obj);
        }
    }
}
