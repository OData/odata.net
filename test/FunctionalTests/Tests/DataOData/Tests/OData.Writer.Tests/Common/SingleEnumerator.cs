//---------------------------------------------------------------------
// <copyright file="SingleEnumerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    #endregion Namespaces

    /// <summary>
    /// IEnumerable which can be enumerated only once.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    public class SingleEnumerator<T> : IEnumerable<T>
    {
        /// <summary>cached values in the original IEnumerable.</summary>
        private T[] enumerableArray;
        
        /// <summary>whether this enumerator is used or not.</summary>
        private bool enumerated;

        /// <summary>Initialize the enumerator with the given IEnumerable</summary>
        /// <param name="e">IEnumerable to cache, and enumerate.</param>
        public SingleEnumerator(IEnumerable<T> e)
        {
            this.enumerableArray = e.ToArray();
            this.enumerated = false;
        }

        /// <summary>Returns the enumerator only it has not been enumerated before.</summary>
        /// <returns>IEnumerable<T> if it has not been enumerated before.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (this.enumerated)
            {
                throw new Exception("this enumerable has been already enumerated");
            }

            this.enumerated = true;
            return enumerableArray.AsEnumerable().GetEnumerator();
        }

        /// <summary>Returns the enumerator only it has not been enumerated before.</summary>
        /// <returns>IEnumerable if it has not been enumerated before.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Internal array that holds values from the original IEnumerable.</summary>
        public T[] InternalArray
        {
            get
            {
                return enumerableArray;
            }
        }
    }
}

