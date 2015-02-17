//---------------------------------------------------------------------
// <copyright file="QueryValueToClrValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Converts a query value into its equivalent CLR typed value
    /// </summary>
    public class QueryValueToClrValueConverter : IQueryValueVisitor<object>
    {
        private Dictionary<QueryValue, object> objectLookup;

        /// <summary>
        /// Initializes a new instance of the QueryValueToClrValueConverter type
        /// </summary>
        public QueryValueToClrValueConverter()
        {
            this.objectLookup = new Dictionary<QueryValue, object>(ReferenceEqualityComparer.Create<QueryValue>());
        }

        /// <summary>
        /// Visits a query collection value and returns an IList with the clr values of the elements
        /// </summary>
        /// <param name="value">The collection value which contains the elements</param>
        /// <returns>An IList with the clr values of the elements</returns>
        public object Visit(QueryCollectionValue value)
        {
            object clrInstance = null;
            if (this.objectLookup.TryGetValue(value, out clrInstance))
            {
                return clrInstance;
            }

            IQueryClrType queryEntityType = value.Type.ElementType as IQueryClrType;
            Type clrQueryType = queryEntityType.ClrType;
            var dummyListType = typeof(List<>).MakeGenericType(clrQueryType);
            var stronglyTypedListConstructor = dummyListType.GetConstructor(Type.EmptyTypes);
            clrInstance = stronglyTypedListConstructor.Invoke(null);
            IList stronglyTypedList = clrInstance as IList;
            this.objectLookup.Add(value, stronglyTypedList);

            foreach (var entity in value.Elements)
            {
                stronglyTypedList.Add(entity.Accept<object>(this));
            }

            return clrInstance;
        }

        /// <summary>
        /// Visits a QueryScalarValue and returns the clr value of the primitive value
        /// </summary>
        /// <param name="value">The QueryScalarValue which contains the clr value of the primitive value</param>
        /// <returns>the clr value of the primitive value</returns>
        public object Visit(QueryScalarValue value)
        {
            if (value.IsNull || value.Value == UninitializedData.Value)
            {
                return null;
            }

            return value.Value;
        }

        /// <summary>
        /// Visits a QueryRecordValue and returns the clr value of the record value
        /// </summary>
        /// <param name="value">The QueryRecordValue which contains the clr value of the record value</param>
        /// <returns>The clr instance of the record value</returns>
        public object Visit(QueryRecordValue value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a QueryReferenceValue and returns the clr value of the reference value
        /// </summary>
        /// <param name="value">The QueryReferenceValue which contains the clr value of the structural type : complex/entity  type</param>
        /// <returns>The clr instance of the reference value</returns>
        public object Visit(QueryReferenceValue value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a QueryStructuralValue and returns the clr value of the structural value
        /// </summary>
        /// <param name="value">The QueryStructuralValue which contains the clr value of the structural type : complex/entity  type</param>
        /// <returns>The clr instance of the structural value</returns>
        public object Visit(QueryStructuralValue value)
        {
            object clrInstance = null;
            if (this.objectLookup.TryGetValue(value, out clrInstance))
            {
                return clrInstance;
            }

            ExceptionUtilities.CheckObjectNotNull(value.Type as IQueryClrType, "Structural type does not implement IQueryClrType");

            IQueryClrType clrTypeQueryable = value.Type as IQueryClrType;
            Type clrType = clrTypeQueryable.ClrType;

            ExceptionUtilities.CheckObjectNotNull(clrType, "ClrType should not be null");

            clrInstance = clrType.GetConstructor(Type.EmptyTypes).Invoke(null);
            this.objectLookup.Add(value, clrInstance);

            foreach (var member in value.MemberNames)
            {
                QueryValue queryValue = value.GetValue(member);
                var memberValue = queryValue.Accept(this);
                PropertyInfo memberProperty = clrType.GetProperty(member);
                memberProperty.SetValue(clrInstance, memberValue, null);
            }

            return clrInstance;
        }
    }
}
