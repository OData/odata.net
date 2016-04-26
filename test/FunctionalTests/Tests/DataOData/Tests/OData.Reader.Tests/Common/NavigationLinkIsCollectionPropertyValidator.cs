//---------------------------------------------------------------------
// <copyright file="NavigationLinkIsCollectionPropertyValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Contracts;
    using Microsoft.Test.Taupo.Contracts;
    #endregion Namespaces

    /// <summary>
    /// OData object model validator for the IsCollection property on ODataNestedResourceInfo objects.
    /// </summary>
    [ImplementationName(typeof(IODataObjectModelValidator), "NavigationLinkIsCollectionPropertyValidator")]
    public class NavigationLinkIsCollectionPropertyValidator : IODataObjectModelValidator
    {
        public AssertionHandler Assert { get; set; }
        public IDictionary<string, bool?> ExpectedIsCollectionValues { get; set; }

        /// <summary>
        /// Validates the OData object model.
        /// </summary>
        /// <param name="objectModelRoot">The root of the object model.</param>
        public void ValidateODataObjectModel(object objectModelRoot)
        {
            new NavigationLinkIsCollectionValidatingVisitor(this.ExpectedIsCollectionValues, this.Assert).Visit(objectModelRoot);
        }

        private class NavigationLinkIsCollectionValidatingVisitor : ODataObjectModelVisitor
        {
            private IDictionary<string, bool?> expectedIsCollectionValues;
            private AssertionHandler assertionHandler;

            public NavigationLinkIsCollectionValidatingVisitor(IDictionary<string, bool?> expectedIsCollectionValues, AssertionHandler assertionHandler)
            {
                this.expectedIsCollectionValues = expectedIsCollectionValues;
                this.assertionHandler = assertionHandler;
            }

            protected override void  VisitNavigationLink(ODataNestedResourceInfo navigationLink)
            {
                bool? expectedIsCollectionValue;
                if (this.expectedIsCollectionValues != null && this.expectedIsCollectionValues.TryGetValue(navigationLink.Name, out expectedIsCollectionValue))
                {
                    this.assertionHandler.AreEqual(
                        expectedIsCollectionValue,
                        navigationLink.IsCollection,
                        "Value for IsCollection on NavigationLink '{0}' is unexpected",
                        navigationLink.Name);
                }

                base.VisitNavigationLink(navigationLink);
            }
        }
    }
}
