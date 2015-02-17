//---------------------------------------------------------------------
// <copyright file="Tuple.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace System {

    /// <summary>
    /// Helper so we can call some tuple methods recursively without knowing the underlying types.
    /// </summary>
    internal interface ITuple {
        string ToString(StringBuilder sb);
        int GetHashCode(IEqualityComparer comparer);
        int Size { get; }

    }

    public static class Tuple {
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) {
            return new Tuple<T1, T2>(item1, item2);
        }

        // From System.Web.Util.HashCodeCombiner
        internal static int CombineHashCodes(int h1, int h2) {
            return (((h1 << 5) + h1) ^ h2);
        }
    }

    public class Tuple<T1, T2> : IComparable, ITuple {

        private readonly T1 m_Item1;
        private readonly T2 m_Item2;

        public T1 Item1 { get { return m_Item1; } }
        public T2 Item2 { get { return m_Item2; } }

        public Tuple(T1 item1, T2 item2) {
            m_Item1 = item1;
            m_Item2 = item2;
        }

        public override Boolean Equals(Object obj)
        {
            return this.Equals(obj, EqualityComparer<Object>.Default); ;
        }

        private Boolean Equals(Object other, IEqualityComparer comparer)
        {
            if (other == null) return false;

            Tuple<T1, T2> objTuple = other as Tuple<T1, T2>;

            if (objTuple == null)
            {
                return false;
            }

            return comparer.Equals(m_Item1, objTuple.m_Item1) && comparer.Equals(m_Item2, objTuple.m_Item2);
        }

        Int32 IComparable.CompareTo(Object obj) {
            return this.CompareTo(obj, Comparer<Object>.Default);
        }

        private Int32 CompareTo(Object other, IComparer comparer) {
            if (other == null) return 1;

            Tuple<T1, T2> objTuple = other as Tuple<T1, T2>;

            if (objTuple == null) {
                throw new ArgumentException("Tuple incorrect type: " + this.GetType().ToString(), "other");
            }

            int c = 0;

            c = comparer.Compare(m_Item1, objTuple.m_Item1);

            if (c != 0) return c;

            return comparer.Compare(m_Item2, objTuple.m_Item2);
        }

        public override int GetHashCode() {
            return this.GetHashCode(EqualityComparer<Object>.Default);
        }

        private Int32 GetHashCode(IEqualityComparer comparer) {
            return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item1), comparer.GetHashCode(m_Item2));
        }

        Int32 ITuple.GetHashCode(IEqualityComparer comparer) {
            return this.GetHashCode(comparer);
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            return ((ITuple)this).ToString(sb);
        }

        string ITuple.ToString(StringBuilder sb) {
            sb.Append(m_Item1);
            sb.Append(", ");
            sb.Append(m_Item2);
            sb.Append(")");
            return sb.ToString();
        }

        int ITuple.Size {
            get {
                return 2;
            }
        }
    }
}
