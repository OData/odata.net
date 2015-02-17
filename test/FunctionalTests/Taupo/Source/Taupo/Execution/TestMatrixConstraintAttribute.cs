//---------------------------------------------------------------------
// <copyright file="TestMatrixConstraintAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Attribute for representing a constraint for a test matrix.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class TestMatrixConstraintAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the TestMatrixConstraintAttribute class.
        /// </summary>
        /// <param name="constraintMethodName">The name of the method which represents constraint.
        /// The method must return boolean and have parameters for each dimension participating in the constraint
        /// (i.e. it does not have to have parameters for all dimensions, just for the ones that are part of the constraint)
        /// Parameter name should match corresponding dimension name from which all spaces (if any) are removed.
        /// The match is case-insensetive.
        /// </param>
        public TestMatrixConstraintAttribute(string constraintMethodName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(constraintMethodName, "constraintMethodName");
            this.ConstraintMethodName = constraintMethodName;
        }

        /// <summary>
        /// Gets the name of the method that represents constraint.
        /// </summary>
        public string ConstraintMethodName { get; private set; }
    }
}
