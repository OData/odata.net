//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationDictWeakKeyComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Annotation
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Used to compare key in DataServiceContext.InstanceAnnotations.
    /// </summary>
    internal class InstanceAnnotationDictWeakKeyComparer : WeakKeyComparer<object>
    {
        private static InstanceAnnotationDictWeakKeyComparer defaultInstance;

        /// <summary>
        /// The constructor to create an InstanceAnnotationDictWeakKeyComparer
        /// </summary>
        private InstanceAnnotationDictWeakKeyComparer()
            : base(null)
        {
        }

        /// <summary>
        /// Get the default comparer.
        /// </summary>
        public static new InstanceAnnotationDictWeakKeyComparer Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new InstanceAnnotationDictWeakKeyComparer();
                }

                return defaultInstance;
            }
        }

        /// <summary>
        /// Get the hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object of which a hash code is to be returned.</param>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        public override int GetHashCode(object obj)
        {
            WeakKeyReference<object> weakKey = obj as WeakKeyReference<object>;
            if (weakKey != null)
            {
                return weakKey.HashCode;
            }

            Tuple<object, MemberInfo> tuple = obj as Tuple<object, MemberInfo>;
            if (tuple != null)
            {
                return tuple.Item1.GetHashCode() ^ tuple.Item2.GetHashCode();
            }

            Tuple<WeakKeyReference<object>, MemberInfo> tuple2 = obj as Tuple<WeakKeyReference<object>, MemberInfo>;
            if (tuple2 != null)
            {
                return tuple2.Item1.HashCode ^ tuple2.Item2.GetHashCode();
            }

            return this.comparer.GetHashCode(obj);
        }

        /// <summary>
        /// Create a dictionary key for the specified obj.
        /// </summary>
        /// <param name="obj">The specified object used to create a key</param>
        /// <returns>Returns a Tuple&lt;WeakKeyReference&lt;object&gt;, MemberInfo&gt; if the input object is a tuple,
        /// or returns a WeakKeyReference&lt;object&gt;</returns>
        public object CreateKey(object obj)
        {
            Tuple<object, MemberInfo> tm = obj as Tuple<object, MemberInfo>;
            if (tm != null)
            {
                return new Tuple<WeakKeyReference<object>, MemberInfo>(new WeakKeyReference<object>(tm.Item1, this), tm.Item2);
            }
            else
            {
                return new WeakKeyReference<object>(obj, this);
            }
        }

        /// <summary>
        /// A rule to determine whether an entry with the key could be removed.
        /// </summary>
        /// <param name="key">The key of an entry to be checked.</param>
        /// <returns>Returns true if the target of a WeakKeyReference&lt;object&gt; in a tuple is dead, else false.</returns>
        public bool RemoveRule(object key)
        {
            var tupleObj = key as Tuple<WeakKeyReference<object>, MemberInfo>;
            if (tupleObj != null)
            {
                return !tupleObj.Item1.IsAlive;
            }

            return false;
        }

        /// <summary>
        /// Gets the target of the input object if it is a <see cref="WeakKeyReference&lt;T&gt;"/>,
        /// else a new Tuple&lt;object, MemberInfo&gt; if it is a Tuple&lt;WeakKeyReferece&lt;object&gt;, MemberInfo&gt;.
        /// </summary>
        /// <param name="obj">The input object from which to get the target.</param>
        /// <param name="isDead">Indicate whether the object is dead if it is a <see cref="WeakKeyReference&lt;T&gt;"/>.
        /// Or wehther the first item of a tuple is dead.
        /// </param>
        /// <returns>The target of the input object.</returns>
        protected override object GetTarget(object obj, out bool isDead)
        {
            WeakKeyReference<object> wref = obj as WeakKeyReference<object>;
            if (wref != null)
            {
                isDead = !wref.IsAlive;
                return wref.Target;
            }

            Tuple<WeakKeyReference<object>, MemberInfo> tuple = obj as Tuple<WeakKeyReference<object>, MemberInfo>;
            if (tuple != null)
            {
                return new Tuple<object, MemberInfo>(GetTarget(tuple.Item1, out isDead), tuple.Item2);
            }

            isDead = false;
            return obj;
        }
    }
}