//---------------------------------------------------------------------
// <copyright file="IMethodReplacementStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Contracts
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Contract for a component that helps re-write method-call expressions in expression trees
    /// </summary>
    public interface IMethodReplacementStrategy
    {
        /// <summary>
        /// Tries to get a replacement expression for the given method with the given parameters
        /// </summary>
        /// <param name="toReplace">The method to replace</param>
        /// <param name="parameters">The parameters to the method</param>
        /// <param name="replaced">The replaced expression</param>
        /// <returns>True if a replacement was made, false otherwise</returns>
        bool TryGetReplacement(MethodInfo toReplace, IEnumerable<Expression> parameters, out Expression replaced);
    }
}
