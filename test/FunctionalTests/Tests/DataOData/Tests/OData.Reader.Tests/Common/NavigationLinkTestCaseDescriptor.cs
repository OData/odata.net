//---------------------------------------------------------------------
// <copyright file="NavigationLinkTestCaseDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Common
{
    using System.Collections.Generic;

    /// <summary>
    /// NavigationLink specific test descriptor.
    /// </summary>
    public class NavigationLinkTestCaseDescriptor : PayloadReaderTestDescriptor
    {
        /// <summary>
        /// Constructor for NavigationLinkTestCaseDescriptor.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public NavigationLinkTestCaseDescriptor(PayloadReaderTestDescriptor.Settings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Copy constructor for NavigationLinkTestCaseDescriptor.
        /// </summary>
        /// <param name="original">The original test descriptor to copy.</param>
        public NavigationLinkTestCaseDescriptor(PayloadReaderTestDescriptor original)
            : base(original)
        {
        }

        /// <summary>
        /// Collection of expected values for ODataNestedResourceInfo.IsCollection property, as (link name -> expected value) pairs.
        /// </summary>
        public IDictionary<string, bool?> ExpectedIsCollectionValues { get; set; }

        /// <summary>
        /// Called to get the expected result of the test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The expected result.</returns>
        protected override ReaderTestExpectedResult GetExpectedResult(ReaderTestConfiguration testConfiguration)
        {
            var baseResult = base.GetExpectedResult(testConfiguration) as PayloadReaderTestExpectedResult;
            var aggregateValidator = new AggregateODataObjectModelValidator(baseResult.ODataObjectModelValidator as AggregateODataObjectModelValidator);
            aggregateValidator.AddValidator(
                new NavigationLinkIsCollectionPropertyValidator
                {
                    Assert = this.settings.Assert,
                    ExpectedIsCollectionValues = this.ExpectedIsCollectionValues,
                });

            baseResult.ODataObjectModelValidator = aggregateValidator;

            return baseResult;
        }
    }
}
