//---------------------------------------------------------------------
// <copyright file="QueryBuiltInFunction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Represents query built-in function (i.e. canonical or provider specific).
    /// </summary>
    public class QueryBuiltInFunction
    {
        /// <summary>
        /// Initializes a new instance of the QueryBuiltInFunction class with a resolver for the return type.
        /// </summary>
        /// <param name="namespaceName">The function namespace.</param>
        /// <param name="name">The funciton name.</param>
        /// <param name="returnTypeResolver">The resolver for the function return type.</param>
        public QueryBuiltInFunction(string namespaceName, string name, Func<QueryType[], QueryType> returnTypeResolver)
            : this(namespaceName, name)
        {
            ExceptionUtilities.CheckArgumentNotNull(returnTypeResolver, "returnTypeResolver");
            this.ReturnTypeResolver = returnTypeResolver;
        }

        /// <summary>
        /// Initializes a new instance of the QueryBuiltInFunction class with a return data type.
        /// </summary>
        /// <param name="namespaceName">The function namespace.</param>
        /// <param name="name">The funciton name.</param>
        /// <param name="returnType">The return data type.</param>
        public QueryBuiltInFunction(string namespaceName, string name, DataType returnType)
            : this(namespaceName, name)
        {
            ExceptionUtilities.CheckArgumentNotNull(returnType, "returnType");
            this.ReturnType = returnType;
        }

        private QueryBuiltInFunction(string namespaceName, string name)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(namespaceName, "namespaceName");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            this.Name = name;
            this.NamespaceName = namespaceName;
        }

        /// <summary>
        /// Gets the function name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the funciton namespace.
        /// </summary>
        public string NamespaceName { get; private set; }

        /// <summary>
        /// Gets the function return data type (if any).
        /// </summary>
        public DataType ReturnType { get; private set; }

        /// <summary>
        /// Gets the resolver for the function query return type (if any).
        /// </summary>
        public Func<QueryType[], QueryType> ReturnTypeResolver { get; private set; }

        /// <summary>
        /// Gets the function's full name which includes NamespaceName and Name separated by dot. 
        /// </summary>
        public string FullName
        {
            get
            {
                string fullName = string.Empty;

                if (!string.IsNullOrEmpty(this.NamespaceName))
                {
                    fullName = this.NamespaceName + ".";
                }

                fullName += this.Name;

                return fullName;
            }
        }
    }
}