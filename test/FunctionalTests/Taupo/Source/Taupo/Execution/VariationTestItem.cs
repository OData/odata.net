//---------------------------------------------------------------------
// <copyright file="VariationTestItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents test variation (test method + parameters)
    /// </summary>
    public class VariationTestItem : TestItem
    {
        private MethodInfo method;
        private VariationAttribute variationAttribute;

        /// <summary>
        /// Initializes a new instance of the VariationTestItem class for given <see cref="MethodInfo"/> and <see cref="VariationAttribute"/>.
        /// </summary>
        /// <param name="parentTestCase">The parent test case.</param>
        /// <param name="methodInfo">Method info</param>
        public VariationTestItem(TestItem parentTestCase, MethodInfo methodInfo)
        {
            ExceptionUtilities.CheckArgumentNotNull(parentTestCase, "parentTestCase");
            ExceptionUtilities.CheckArgumentNotNull(methodInfo, "methodInfo");

            this.method = methodInfo;
            this.Parent = parentTestCase;

            var metadata = this.Metadata;
            metadata.Name = metadata.Description = methodInfo.Name;
        }

        /// <summary>
        /// Initializes a new instance of the VariationTestItem class for given <see cref="MethodInfo"/> and <see cref="VariationAttribute"/>.
        /// </summary>
        /// <param name="parentTestCase">The parent test case.</param>
        /// <param name="methodInfo">Method info</param>
        /// <param name="variationAttribute"><see cref="VariationAttribute"/> that describes variation metadata.</param>
        public VariationTestItem(TestItem parentTestCase, MethodInfo methodInfo, VariationAttribute variationAttribute)
            : this(parentTestCase, methodInfo)
        {
            ExceptionUtilities.CheckArgumentNotNull(variationAttribute, "variationAttribute");

            this.variationAttribute = variationAttribute;

            ReadMetadataFromAttribute(variationAttribute);

            this.Timeout = variationAttribute.Timeout;
        }

        /// <summary>
        /// Gets or sets the asynchronous execution timeout (in milliseconds, negative values mean no timeout).
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Executes a variation.
        /// </summary>
        public override void Execute()
        {
            this.method.Invoke(this.Parent, this.variationAttribute.Parameters);
        }

        /// <summary>
        /// Executes the variation asynchonously.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        public void ExecuteAsync(IAsyncContinuation continuation)
        {
            try
            {
                IEnumerable<Action<IAsyncContinuation>> actions = null;
                using (var context = AsyncExecutionContext.Begin())
                {
                    this.Execute();
                    actions = context.GetQueuedActions();
                }

                AsyncHelpers.RunActionSequence(continuation, actions);
            }
            catch (TargetInvocationException ex)
            {
                continuation.Fail(ex.InnerException);
            }
        }

        /// <summary>
        /// Cleans up after test item execution.
        /// </summary>
        public override void Terminate()
        {
            this.method = null;
            this.variationAttribute = null;

            base.Terminate();
        }

        /// <summary>
        /// Returns default name of the variation based on name of the method.
        /// </summary>
        /// <returns>Variation name.</returns>
        protected override string GetDefaultMetadataName()
        {
            return this.method.Name;
        }
    }
}
