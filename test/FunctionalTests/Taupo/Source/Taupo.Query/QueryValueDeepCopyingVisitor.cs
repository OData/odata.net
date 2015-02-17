//---------------------------------------------------------------------
// <copyright file="QueryValueDeepCopyingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Default implementation of contract for creating deep copies of query values
    /// </summary>
    [ImplementationName(typeof(IQueryValueDeepCopyingVisitor), "Default")]
    public class QueryValueDeepCopyingVisitor : IQueryValueDeepCopyingVisitor
    {
        private IDictionary<QueryValue, QueryValue> copyCache = new Dictionary<QueryValue, QueryValue>(ReferenceEqualityComparer.Create<QueryValue>());

        /// <summary>
        /// Performs a deep copy on the given value and returns the copy
        /// </summary>
        /// <typeparam name="TValue">The type of the value and copy</typeparam>
        /// <param name="value">The value to copy</param>
        /// <returns>The deep-copy of the value</returns>
        public TValue PerformDeepCopy<TValue>(TValue value) where TValue : QueryValue
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");
            this.copyCache.Clear();
            return (TValue)value.Accept(this);
        }

        /// <summary>
        /// Creates a copy of the given value recursively
        /// </summary>
        /// <param name="value">The value to copy</param>
        /// <returns>The copied value</returns>
        public QueryValue Visit(QueryCollectionValue value)
        {
            return this.HandleCommonCasesAndCache(
                value,
                () => value.Type.CreateCollectionWithValues(Enumerable.Empty<QueryValue>()),
                copy =>
                {
                    var collectionCopy = (QueryCollectionValue)copy;
                    foreach (var element in value.Elements)
                    {
                        collectionCopy.Elements.Add(element.Accept(this));
                    }
                });
        }

        /// <summary>
        /// Creates a copy of the given value recursively
        /// </summary>
        /// <param name="value">The value to copy</param>
        /// <returns>The copied value</returns>
        public QueryValue Visit(QueryRecordValue value)
        {
            return this.HandleCommonCasesAndCache(
                value,
                () => value.Type.CreateNewInstance(),
                copy =>
                {
                    for (int i = 0; i < value.Type.Properties.Count; i++)
                    {
                        var memberValue = value.GetMemberValue(i);
                        var memberValueCopy = memberValue.Accept(this);
                        ((QueryRecordValue)copy).SetMemberValue(i, memberValueCopy);
                    }
                });
        }

        /// <summary>
        /// Creates a copy of the given value recursively
        /// </summary>
        /// <param name="value">The value to copy</param>
        /// <returns>The copied value</returns>
        public QueryValue Visit(QueryReferenceValue value)
        {
            return this.HandleCommonCasesAndCache(
                value,
                () =>
                {
                    if (value.EntityValue.IsNull)
                    {
                        var keyCopy = (QueryRecordValue)value.KeyValue.Accept(this);
                        return value.Type.CreateReferenceValue(value.EntitySetFullName, keyCopy);
                    }
                    else
                    {
                        // no way to create the reference value first. Possible infinite recursion here.
                        var entityValueCopy = (QueryStructuralValue)value.EntityValue.Accept(this);
                        return value.Type.CreateReferenceValue(entityValueCopy);
                    }
                },
                null);
        }

        /// <summary>
        /// Creates a copy of the given value recursively
        /// </summary>
        /// <param name="value">The value to copy</param>
        /// <returns>The copied value</returns>
        public virtual QueryValue Visit(QueryScalarValue value)
        {
            return this.HandleCommonCasesAndCache(
                value,
                () => value.Type.CreateValue(value.Value),
                null);
        }

        /// <summary>
        /// Creates a copy of the given value recursively
        /// </summary>
        /// <param name="value">The value to copy</param>
        /// <returns>The copied value</returns>
        public QueryValue Visit(QueryStructuralValue value)
        {
            return this.HandleCommonCasesAndCache(
                value,
                () => value.Type.CreateNewInstance(),
                copy =>
                {
                    foreach (var memberName in value.MemberNames)
                    {
                        var memberValue = value.GetValue(memberName);
                        var memberValueCopy = memberValue.Accept(this);
                        ((QueryStructuralValue)copy).SetValue(memberName, memberValueCopy);
                    }
                });
        }

        /// <summary>
        /// Handles cases for errors and null values, then falls back to the given delegates while caching the result.
        /// Note that the creation is seperated from updating because the update is typically recursive and the new item needs to be cached first.
        /// </summary>
        /// <param name="value">The value being copied</param>
        /// <param name="createCopy">A delegate to create a copy which will be cached</param>
        /// <param name="updateCopy">A delegate to update the copy with cloned property values, etc</param>
        /// <returns>The copied value</returns>
        private QueryValue HandleCommonCasesAndCache(QueryValue value, Func<QueryValue> createCopy, Action<QueryValue> updateCopy)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");

            if (value.EvaluationError != null)
            {
                return value.Type.CreateErrorValue(value.EvaluationError);
            }

            if (value.IsNull)
            {
                return value.Type.NullValue;
            }

            QueryValue cachedCopy;
            if (!this.copyCache.TryGetValue(value, out cachedCopy))
            {
                cachedCopy = createCopy();
                this.copyCache[value] = cachedCopy;

                if (updateCopy != null)
                {
                    updateCopy(cachedCopy);
                }

                foreach (var annotation in value.Annotations)
                {
                    cachedCopy.Annotations.Add(annotation.Clone());
                }
            }

            return cachedCopy;
        }
    }
}