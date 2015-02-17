//---------------------------------------------------------------------
// <copyright file="TestMatrixMethodBasedConstraint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A constraint that delegates IsValid resolution to the underlying method.
    /// </summary>
    internal class TestMatrixMethodBasedConstraint : IConstraint
    {
        private List<QualifiedDimension> requiredDimensions;
        private MethodInfo methodInfo;
        private object instance;

        /// <summary>
        /// Initializes a new instance of the TestMatrixMethodBasedConstraint class.
        /// </summary>
        /// <param name="methodInfo">Method info of the method that does IsValid resolution.</param>
        /// <param name="instance">The instance on which method should be called when method is not static.</param>
        /// <param name="dimensions">Dimensions of the test matrix.</param>
        public TestMatrixMethodBasedConstraint(MethodInfo methodInfo, object instance, IEnumerable<Dimension> dimensions)
        {
            ExceptionUtilities.CheckArgumentNotNull(methodInfo, "methodInfo");
            ExceptionUtilities.CheckArgumentNotNull(dimensions, "dimensions");

            ExceptionUtilities.Assert(methodInfo.IsStatic || instance != null, "Method '{0}' must be static or instance must be non-null. Declaring Type: '{1}'.", methodInfo.Name, methodInfo.DeclaringType.FullName);
            ExceptionUtilities.Assert(methodInfo.ReturnType == typeof(bool), "Method '{0}' must return boolean. Declaring Type: '{1}'.", methodInfo.Name, methodInfo.DeclaringType.FullName);
            this.methodInfo = methodInfo;
            this.instance = instance;

            this.requiredDimensions = new List<QualifiedDimension>();

            // allow dimension name to have non-alphanumeric characters
            var normalizedDimensions = dimensions.Select(d => Tuple.Create(d, Regex.Replace(d.Name, "[^\\w]", string.Empty))).ToArray();

            // add dimensions in the same order as method parameters
            foreach (var parameter in methodInfo.GetParameters())
            {
                var matchingDimensions = normalizedDimensions.Where(d => string.Equals(d.Item2, parameter.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                ExceptionUtilities.Assert(matchingDimensions.Count == 1, "Should find exactly 1 dimension matching parameter '{0}'. Found: {1}.", parameter.Name, matchingDimensions.Count);
                this.requiredDimensions.Add(matchingDimensions[0].Item1);
            }
        }

        /// <summary>
        /// Gets the set of dimensions checked by this constraint (that should be present in the Vector given to IsValid).
        /// </summary>
        public ReadOnlyCollection<QualifiedDimension> RequiredDimensions
        {
            get
            {
                return this.requiredDimensions.AsReadOnly();
            }
        }

        /// <summary>
        /// Checks the input vector and returns true if it doesn't violate this constraint.
        /// </summary>
        /// <param name="target">The target vector to validate.</param>
        /// <returns>
        ///     <c>true</c> if the specified <paramref name="target"/>is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(Vector target)
        {
            object[] parameterValues = this.requiredDimensions.Select(d => target.GetValue(d)).ToArray();

            bool result = (bool)this.methodInfo.Invoke(this.instance, parameterValues);

            return result;
        }
    }
}
