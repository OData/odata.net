//---------------------------------------------------------------------
// <copyright file="ReaderEnumerablesODataObjectModelValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Test.Taupo.OData.Common;

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Implementation of IODataObjectModelValidator which validates the enumerables implementation assigned in the OM.
    /// </summary>
    [ImplementationName(typeof(IODataObjectModelValidator), "ReaderEnumerablesODataObjectModelValidator")]
    public class ReaderEnumerablesODataObjectModelValidator : IODataObjectModelValidator
    {
        /// <summary>
        /// The assertion handler to use.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Validates the OData object model.
        /// </summary>
        /// <param name="objectModelRoot">The root of the object model.</param>
        public void ValidateODataObjectModel(object objectModelRoot)
        {
            ObjectModelVisitor visitor = new ObjectModelVisitor(this.Assert);
            visitor.Visit(objectModelRoot);
        }

        /// <summary>
        /// Visitor for the OData OM which performs the verification.
        /// </summary>
        private class ObjectModelVisitor : ODataObjectModelVisitor
        {
            /// <summary>
            /// The assertion handler to use.
            /// </summary>
            private AssertionHandler assert;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="assert">The assertion handler to use.</param>
            public ObjectModelVisitor(AssertionHandler assert)
            {
                this.assert = assert;
            }

            /// <summary>
            /// Visits an entry item.
            /// </summary>
            /// <param name="entry">The entry to visit.</param>
            protected override void VisitEntry(ODataResource entry)
            {
                this.ValidateEnumerable<ODataProperty>(entry.Properties, "ODataResource.Properties");
                this.ValidateEnumerable<ODataAction>(entry.Actions, "ODataResource.Actions");
                this.ValidateEnumerable<ODataFunction>(entry.Functions, "ODataResource.Functions");
                base.VisitEntry(entry);
            }

            /// <summary>
            /// Visits a collection item.
            /// </summary>
            /// <param name="collection">The collection to visit.</param>
            protected override void VisitCollectionValue(ODataCollectionValue collection)
            {
                this.ValidateEnumerable(collection.Items, "ODataCollectionValue.Items");
                base.VisitCollectionValue(collection);
            }

            /// <summary>
            /// Visits a serviceDocument item.
            /// </summary>
            /// <param name="serviceDocument">The serviceDocument to visit.</param>
            protected override void VisitServiceDocument(ODataServiceDocument serviceDocument)
            {
                this.ValidateEnumerable<ODataEntitySetInfo>(serviceDocument.EntitySets, "ODataServiceDocument.EntitySets");
                base.VisitServiceDocument(serviceDocument);
            }

            /// <summary>
            /// Visits an entity reference links instance.
            /// </summary>
            /// <param name="entityReferenceLinks">The entity reference links to visit.</param>
            protected override void VisitEntityReferenceLinks(ODataEntityReferenceLinks entityReferenceLinks)
            {
                this.ValidateEnumerable<ODataEntityReferenceLink>(entityReferenceLinks.Links, "ODataEntityReferenceLinks.Links");
                base.VisitEntityReferenceLinks(entityReferenceLinks);
            }

            /// <summary>
            /// Validates an enumerable.
            /// </summary>
            /// <typeparam name="T">The type of the item for the enumerable.</typeparam>
            /// <param name="enumerable">The enumerable to validate.</param>
            /// <param name="description">The description of the thing which is validated.</param>
            private void ValidateEnumerable<T>(IEnumerable<T> enumerable, string description)
            {
                this.assert.IsNotNull(enumerable, "The enumerable returned by {0} is null. Readers should always set enumerable to a non-null value.", description);
                this.assert.IsFalse(enumerable.GetType().IsPublic, "The enumerable returned by {0} should not be any public type.", description);

                // Validate that enumerating the untyped enumerable returns items of the right type
                // Note that this also effectively validates that the enumeration can be enumerated over multiple times
                // since the visitor base will enumerate over it as well.
                IEnumerator enumerator = ((IEnumerable)enumerable).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    this.assert.IsNotNull(enumerator.Current, "The enumerable returned by {0} contains null items, it should not.", description);
                    this.assert.IsTrue(enumerator.Current is T, "The enumerable returned by {0} when enumerated through IEnumerable doesn't return correctly typed instances.", description);
                }
            }

            /// <summary>
            /// Validates an enumerable.
            /// </summary>
            /// <param name="enumerable">The enumerable to validate.</param>
            /// <param name="description">The description of the thing which is validated.</param>
            private void ValidateEnumerable(IEnumerable enumerable, string description)
            {
                this.assert.IsNotNull(enumerable, "The enumerable returned by {0} is null. Readers should always set enumerable to a non-null value.", description);

                // Test that the instance only implements IEnumerable, nothing else
                var interfaces = enumerable.GetType().GetInterfaces();
                this.assert.AreEqual(1, interfaces.Length, "The enumerable returned by {0} should only implement one interface.", description);
                this.assert.IsTrue(
                    interfaces.Contains(typeof(IEnumerable)),
                    "The enumerable returned by {0} should only implement IEnumerable.",
                    description);

                this.assert.IsFalse(enumerable.GetType().IsPublic, "The enumerable returned by {0} should not be any public type.", description);

                // Validate that enumerating the untyped enumerable returns items
                // Note that this also effectively validates that the enumeration can be enumerated over multiple times
                // since the visitor base will enumerate over it as well.
                IEnumerator enumerator = ((IEnumerable)enumerable).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    this.assert.IsNotNull(enumerator.Current, "The enumerable returned by {0} contains null items, it should not.", description);
                }
            }
        }
    }
}
