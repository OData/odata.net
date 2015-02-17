//---------------------------------------------------------------------
// <copyright file="UpdatableToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Common
{
    using System.Collections.Generic;

    /// <summary>
    /// Class for representing a resource in the process of being updated
    /// </summary>
    public class UpdatableToken
    {
        /// <summary>
        /// Initializes a new instance of the UpdatableToken class
        /// </summary>
        /// <param name="resource">The actual resource</param>
        public UpdatableToken(object resource)
        {
            ExceptionUtilities.CheckArgumentNotNull(resource, "resource");
            this.Resource = resource;
            this.PendingPropertyUpdates = new Dictionary<string, object>();
        }
     
        /// <summary>
        /// Gets the actual resource being updated
        /// </summary>
        public object Resource { get; private set; }

        /// <summary>
        /// Gets the pending property values that have not been committed yet
        /// </summary>
        public IDictionary<string, object> PendingPropertyUpdates { get; private set; }

        /// <summary>
        /// Asserts that the given resource is a token and returns it
        /// </summary>
        /// <param name="resource">The resource that might be a token</param>
        /// <param name="name">The name to use in the error message if it is not a token</param>
        /// <returns>The resource, cast to a token</returns>
        public static UpdatableToken AssertIsToken(object resource, string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(resource, "resource");
            var token = resource as UpdatableToken;
            ExceptionUtilities.CheckObjectNotNull(token, "{0} was not a token. Type was: '{1}'", name, resource.GetType());
            return token;
        }

        /// <summary>
        /// Asserts that the given reosurce is a token, and resolves it if it is
        /// </summary>
        /// <param name="resource">The resource to resolve</param>
        /// <param name="name">The name to use in the error message if it is not a token</param>
        /// <returns>The resolved resource</returns>
        public static object AssertIsTokenAndResolve(object resource, string name)
        {
            return AssertIsToken(resource, name).Resource;
        }

        /// <summary>
        /// Resolves the given resource if it is a token
        /// </summary>
        /// <param name="resource">The resource to resolve</param>
        /// <returns>The resolved resource or the original if it was not a token</returns>
        public static object ResolveIfToken(object resource)
        {
            var token = resource as UpdatableToken;
            if (token != null)
            {
                resource = token.Resource;
            }

            return resource;
        }
    }
}
