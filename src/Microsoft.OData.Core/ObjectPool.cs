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
        private readonly List<T> _objectsInUse;
        private readonly List<T> _objectsAvailable;
        private readonly Func<T> _objectGenerator;

        public ObjectPool(Func<T> objectGenerator)
        {
            _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
            _objectsInUse = new List<T>();
            _objectsAvailable = new List<T>();
        }
        public T GetObject()
        {
            if (_objectsAvailable.Count != 0)
            {
                var obj = _objectsAvailable[0];
                _objectsInUse.Add(obj);
                _objectsAvailable.RemoveAt(0);

                return obj;
            }
            else
            {
                T obj = _objectGenerator();
                _objectsInUse.Add(obj);

                return obj;
            }
        }

        public void ReleaseObject(T obj)
        {
            _objectsInUse.Remove(obj);
            _objectsAvailable.Add(obj);
        }
    }
}
