//---------------------------------------------------------------------
// <copyright file="ChangeInterceptorAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Diagnostics;

    /// <summary>The <see cref="T:Microsoft.OData.Service.ChangeInterceptorAttribute" /> on a method is used to process updates on the specified entity set name.</summary>
    /// <remarks>Use this attribute on a DataService method to indicate that this method should be invoked with data changes.</remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class ChangeInterceptorAttribute : Attribute
    {
        /// <summary>Container name that the method filters.</summary>
        private readonly string entitySetName;

        /// <summary>Creates a new change interceptor for an entity set specified by the parameter <paramref name="entitySetName" />.</summary>
        /// <param name="entitySetName">The name of the entity set that contains the entity to which the interceptor applies.</param>
        public ChangeInterceptorAttribute(string entitySetName)
        {
            if (entitySetName == null)
            {
                throw Error.ArgumentNull("entitySetName");
            }

            this.entitySetName = entitySetName;
        }

        /// <summary>Gets the name of the entity set to which the interceptor applies.</summary>
        /// <returns>The string value that represents entity set name.</returns>
        public string EntitySetName
        {
            [DebuggerStepThrough]
            get { return this.entitySetName; }
        }
    }
}
