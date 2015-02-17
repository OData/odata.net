//---------------------------------------------------------------------
// <copyright file="fxEnumerable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;       //Hashtable
using System.Reflection;

namespace System.Data.Test.Astoria
{
    public class fxEnumerator : IEnumerator
    {
        //Data
        private Object _product;

        internal fxEnumerator(Object product)
        {
            _product = product;
        }

        public IEnumerator Product
        {
            get { return (IEnumerator)_product; }
        }

        public object Current
        {
            get 
            {
                PropertyInfo propertyInfo = _product.GetType().GetProperty("Current");
                return propertyInfo.GetValue(_product, new object[] { });
            }
        }

        public bool MoveNext()
        {
            MethodInfo methodInfo = _product.GetType().GetMethod("MoveNext");
            return (bool)methodInfo.Invoke(_product, new object[] {});
        }

        public void Reset()
        {
            MethodInfo methodInfo = _product.GetType().GetMethod("Reset");
            methodInfo.Invoke(_product, new object[] { });
        }

    }

    public class fxEnumerable : IEnumerable
    {
        private object _product;

        public fxEnumerable(object product)
        {
            _product = product;
        }

        public IEnumerable Product
        {
            get
            {
                return (IEnumerable)_product;
            }
        }

        public IEnumerator GetEnumerator()
        {
            MethodInfo methodInfo = _product.GetType().GetMethod("GetEnumerator");
            return (IEnumerator)methodInfo.Invoke(_product, new object[] { });
        }
    }

}

