//---------------------------------------------------------------------
// <copyright file="DelegateBasedEqualityComparer`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Implements IEqualityComparer based on delegates
    /// </summary>
    /// <typeparam name="TObject">The type of objects to compare for equality</typeparam>
    public class DelegateBasedEqualityComparer<TObject> : EqualityComparer<TObject>
    {
        private Func<TObject, TObject, bool> equalsFunc;
        private Func<TObject, int> getHashCodeFunc;

        /// <summary>
        /// Initializes a new instance of the DelegateBasedEqualityComparer class
        /// </summary>
        /// <param name="equalsFunc">The delegate to use for checking equality</param>
        /// <param name="getHashCodeFunc">The delegate to use for computing hash codes</param>
        public DelegateBasedEqualityComparer(Func<TObject, TObject, bool> equalsFunc, Func<TObject, int> getHashCodeFunc)
        {
            ExceptionUtilities.CheckArgumentNotNull(equalsFunc, "equalsFunc");
            ExceptionUtilities.CheckArgumentNotNull(getHashCodeFunc, "getHashCodeFunc");

            this.equalsFunc = equalsFunc;
            this.getHashCodeFunc = getHashCodeFunc;
        }

        /// <summary>
        /// Initializes a new instance of the DelegateBasedEqualityComparer class using a default implementation for getting hash codes.
        /// </summary>
        /// <param name="equalsFunc">The delegate to use for checking equality</param>
        public DelegateBasedEqualityComparer(Func<TObject, TObject, bool> equalsFunc)
            : this(equalsFunc, DefaultGetHashCodeFunc)
        {
        }

        /// <summary>
        /// Determines equality using the delegate provided during initialization
        /// </summary>
        /// <param name="x">The first object to compare</param>
        /// <param name="y">The second object to compare</param>
        /// <returns>A value indicating whether or not the objects are equal according to the delegate</returns>
        public override bool Equals(TObject x, TObject y)
        {
            return this.equalsFunc(x, y);
        }

        /// <summary>
        /// Calculates the hash code of the given object using the delegate provided during initialization
        /// </summary>
        /// <param name="obj">The object to compute the hash code for</param>
        /// <returns>The result of calling the hash code delegate</returns>
        public override int GetHashCode(TObject obj)
        {
            return this.getHashCodeFunc(obj);
        }

        /// <summary>
        /// Default implementation to get the hash code using the runtime-helpers implementation provided by the framework
        /// </summary>
        /// <param name="obj">The object to get the hash code for</param>
        /// <returns>The hash code of the object</returns>
        internal static int DefaultGetHashCodeFunc(TObject obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}