//---------------------------------------------------------------------
// <copyright file="AssertJustificationAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;

    /// <summary>
    /// Used to justify a security assert.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class AssertJustificationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertJustificationAttribute"/> class.
        /// </summary>
        /// <param name="justification">The justification for the permission assertion.</param>
        public AssertJustificationAttribute(string justification)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(justification, "justification");
            this.Justification = justification;
        }

        /// <summary>
        /// Gets the justification for the assertion applied to this member.
        /// </summary>
        public string Justification { get; private set; }
    }
}
