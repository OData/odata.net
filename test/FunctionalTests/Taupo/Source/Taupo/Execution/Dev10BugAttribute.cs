//---------------------------------------------------------------------
// <copyright file="Dev10BugAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Globalization;
    
    /// <summary>
    /// Indicates that a test class or variation is associated with a specific Dev10 bug.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class Dev10BugAttribute : BaseBugAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dev10BugAttribute"/> class.
        /// </summary>
        /// <param name="bugId">The ID of the bug in the Dev10 database.</param>
        public Dev10BugAttribute(int bugId) :
            base("http://localhost:8080/", "Dev10", bugId)
        {
        }
    }
}
