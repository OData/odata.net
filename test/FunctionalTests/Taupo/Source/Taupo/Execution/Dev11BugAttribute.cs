//---------------------------------------------------------------------
// <copyright file="Dev11BugAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;

    /// <summary>
    /// Indicates that a test class or variation is associated with a specific Dev11 bug.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class Dev11BugAttribute : BaseBugAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dev11BugAttribute"/> class.
        /// </summary>
        /// <param name="bugId">The ID of the bug in the Dev11 database.</param>
        public Dev11BugAttribute(int bugId) :
            base("http://localhost1:8080/", "Dev11", bugId)
        {
        }
    }
}
