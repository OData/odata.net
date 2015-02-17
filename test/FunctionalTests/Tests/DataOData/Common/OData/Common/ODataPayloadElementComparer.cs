//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Utility class to help compare two ODataPayloadElement instances.
    /// </summary>
    [ImplementationName(typeof(IODataLibPayloadElementComparer), "ODataLibPayloadElementComparer")]
    public sealed class ODataLibPayloadElementComparer : IODataLibPayloadElementComparer
    {
        // TODO, ckerer: the code for this class has been copied from the XmlDeserializerTests; I added dependency injection and support for self links.
        //               create a reusable class in the test framework.

        /// <summary>
        /// Gets or sets assertion class to be used.
        /// </summary>
        [InjectDependency]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Asserts that two payload elements are equal.
        /// </summary>
        /// <param name="expected">The expected element</param>
        /// <param name="observed">The actual, observed element</param>
        public void Compare(ODataPayloadElement expected, ODataPayloadElement observed)
        {
            if (expected == null)
            {
                this.Assert.IsNull(observed, "Observed element must be null");
                return;
            }

            this.Assert.AreEqual(expected.ElementType, observed.ElementType, "Element types are not equal.");
            this.Assert.AreEqual(expected.Annotations.Count, observed.Annotations.Count, "Annotation counts are not equal");

            Compare(expected.Annotations, observed.Annotations);

            switch (expected.ElementType)
            {
                case ODataPayloadElementType.EntitySetInstance:
                    EntitySetInstance expectedSet = expected as EntitySetInstance;
                    EntitySetInstance observedSet = observed as EntitySetInstance;
                    this.Assert.AreEqual(expectedSet.NextLink, observedSet.NextLink, "Next links are not equal");
                    this.Assert.AreEqual(expectedSet.InlineCount, observedSet.InlineCount, "Inline counts are not equal");

                    this.Assert.AreEqual(expectedSet.Count, observedSet.Count, "Entity counts are not equal");

                    for (int i = 0; i < expectedSet.Count; i++)
                    {
                        Compare(expectedSet[i], observedSet[i]);
                    }

                    break;

                case ODataPayloadElementType.EntityInstance:
                    EntityInstance expectedEntity = expected as EntityInstance;
                    EntityInstance observedEntity = observed as EntityInstance;
                    this.Assert.AreEqual(expectedEntity.Id, observedEntity.Id, "Entity IDs are not equal");
                    this.Assert.AreEqual(expectedEntity.ETag, observedEntity.ETag, "ETags are not equal");
                    this.Assert.AreEqual(expectedEntity.IsNull, observedEntity.IsNull, "IsNull flags are not equal");
                    this.Assert.AreEqual(expectedEntity.FullTypeName, observedEntity.FullTypeName, "FullTypeNames are not equal");
                    this.Assert.AreEqual(expectedEntity.StreamContentType, observedEntity.StreamContentType, "Stream content types are not equal");
                    this.Assert.AreEqual(expectedEntity.StreamEditLink, observedEntity.StreamEditLink, "Stream edit links are not equal");
                    this.Assert.AreEqual(expectedEntity.StreamETag, observedEntity.StreamETag, "Stream ETags are not equal");
                    this.Assert.AreEqual(expectedEntity.StreamSourceLink, observedEntity.StreamSourceLink, "Stream source links are not equal");
                    this.Assert.AreEqual(expectedEntity.Properties.Count(), observedEntity.Properties.Count(), "Property counts are not equal");
                    for (int i = 0; i < expectedEntity.Properties.Count(); i++)
                    {
                        Compare(expectedEntity.Properties.ElementAt(i), observedEntity.Properties.ElementAt(i));
                    }

                    break;

                case ODataPayloadElementType.ComplexInstance:
                    ComplexInstance expectedCI = expected as ComplexInstance;
                    ComplexInstance observedCI = observed as ComplexInstance;
                    this.Assert.AreEqual(expectedCI.IsNull, observedCI.IsNull, "IsNull flags are not equal");
                    this.Assert.AreEqual(expectedCI.FullTypeName, observedCI.FullTypeName, "Full type names are not equal");
                    this.Assert.AreEqual(expectedCI.Properties.Count(), observedCI.Properties.Count(), "Property counts are not equal");
                    for (int i = 0; i < expectedCI.Properties.Count(); i++)
                    {
                        Compare(expectedCI.Properties.ElementAt(i), observedCI.Properties.ElementAt(i));
                    }

                    break;

                case ODataPayloadElementType.NamedStreamInstance:
                    NamedStreamInstance expectedNsi = expected as NamedStreamInstance;
                    NamedStreamInstance observedNsi = observed as NamedStreamInstance;
                    this.Assert.AreEqual(expectedNsi.Name, observedNsi.Name, "Stream names are not equal");
                    this.Assert.AreEqual(expectedNsi.ETag, observedNsi.ETag, "Stream ETags are not equal");
                    this.Assert.AreEqual(expectedNsi.EditLink, observedNsi.EditLink, "Edit links are not equal");
                    this.Assert.AreEqual(expectedNsi.EditLinkContentType, observedNsi.EditLinkContentType, "Edit link content types are not equal");
                    this.Assert.AreEqual(expectedNsi.SourceLink, observedNsi.SourceLink, "Source links are not equal");
                    this.Assert.AreEqual(expectedNsi.SourceLinkContentType, observedNsi.SourceLinkContentType, "Source links content types are not equal");
                    break;

                case ODataPayloadElementType.NavigationPropertyInstance:
                    NavigationPropertyInstance expectedNav = expected as NavigationPropertyInstance;
                    NavigationPropertyInstance observedNav = observed as NavigationPropertyInstance;
                    Assert.AreEqual(expectedNav.Name, observedNav.Name, "Navigation property names are not equal");
                    Compare(expectedNav.AssociationLink, observedNav.AssociationLink);
                    Compare(expectedNav.Value, observedNav.Value);
                    break;

                case ODataPayloadElementType.ComplexProperty:
                    ComplexProperty expectedCP = expected as ComplexProperty;
                    ComplexProperty observedCP = observed as ComplexProperty;
                    this.Assert.AreEqual(expectedCP.Name, observedCP.Name, "Complex property names are not equal");
                    Compare(expectedCP.Value, observedCP.Value);
                    break;

                case ODataPayloadElementType.PrimitiveProperty:
                    PrimitiveProperty expectedProperty = expected as PrimitiveProperty;
                    PrimitiveProperty observedProperty = observed as PrimitiveProperty;
                    this.Assert.AreEqual(expectedProperty.Name, observedProperty.Name, "Primitive property names are not equal");
                    Compare(expectedProperty.Value, observedProperty.Value);
                    break;

                case ODataPayloadElementType.PrimitiveValue:
                    PrimitiveValue expectedValue = expected as PrimitiveValue;
                    PrimitiveValue observedValue = observed as PrimitiveValue;
                    this.Assert.AreEqual(expectedValue.IsNull, observedValue.IsNull, "IsNull flags are not equal");
                    if (expectedValue.FullTypeName != null)
                    {
                        if (expectedValue.FullTypeName.Equals("Edm.String"))
                        {
                            this.Assert.IsTrue(string.IsNullOrEmpty(observedValue.FullTypeName) || observedValue.FullTypeName.Equals("Edm.String"), "FullTypeName should be null or Edm.String");
                        }
                        else
                        {
                            this.Assert.AreEqual(expectedValue.FullTypeName, observedValue.FullTypeName, "Full type names are not equal");
                        }
                    }
                    else
                    {
                        this.Assert.IsNull(observedValue.FullTypeName, "observed full type name should be null");
                    }

                    this.Assert.AreEqual(expectedValue.ClrValue, observedValue.ClrValue, "Clr values are not equal");
                    break;

                case ODataPayloadElementType.DeferredLink:
                    DeferredLink expectedLink = expected as DeferredLink;
                    DeferredLink observedLink = observed as DeferredLink;
                    this.Assert.AreEqual(expectedLink.UriString, observedLink.UriString, "Uris are not equal");
                    break;

                case ODataPayloadElementType.ExpandedLink:
                    ExpandedLink expectedExpand = expected as ExpandedLink;
                    ExpandedLink observedExpand = observed as ExpandedLink;
                    Compare(expectedExpand.ExpandedElement, observedExpand.ExpandedElement);
                    break;

                case ODataPayloadElementType.LinkCollection:
                    LinkCollection expectedLinks = expected as LinkCollection;
                    LinkCollection observedLinks = observed as LinkCollection;
                    this.Assert.AreEqual(expectedLinks.Count, observedLinks.Count, "Link counts are not equal");
                    this.Assert.AreEqual(expectedLinks.InlineCount, observedLinks.InlineCount, "Link inline counts are not equal");
                    for (int i = 0; i < expectedLinks.Count; i++)
                    {
                        Compare(expectedLinks[i], observedLinks[i]);
                    }

                    break;

                case ODataPayloadElementType.PrimitiveMultiValueProperty:
                    var expectedPrimitiveCollectionProperty = expected as PrimitiveMultiValueProperty;
                    var observedPrimitiveCollectionProperty = observed as PrimitiveMultiValueProperty;
                    this.Assert.AreEqual(expectedPrimitiveCollectionProperty.Name, observedPrimitiveCollectionProperty.Name, "Property names are not equal");
                    this.Assert.AreEqual(expectedPrimitiveCollectionProperty.Value.FullTypeName, observedPrimitiveCollectionProperty.Value.FullTypeName, "Full type names are not equal");
                    this.Assert.AreEqual(expectedPrimitiveCollectionProperty.Value.Count, observedPrimitiveCollectionProperty.Value.Count, "Collection counts are not equal");
                    for (int i = 0; i < expectedPrimitiveCollectionProperty.Value.Count; i++)
                    {
                        Compare(expectedPrimitiveCollectionProperty.Value[i], observedPrimitiveCollectionProperty.Value[i]);
                    }

                    break;

                case ODataPayloadElementType.ComplexMultiValueProperty:
                    var expectedComplexCollectionProperty = expected as ComplexMultiValueProperty;
                    var observedComplexCollectionProperty = observed as ComplexMultiValueProperty;
                    this.Assert.AreEqual(expectedComplexCollectionProperty.Name, observedComplexCollectionProperty.Name, "Property names are not equal");
                    this.Assert.AreEqual(expectedComplexCollectionProperty.Value.FullTypeName, observedComplexCollectionProperty.Value.FullTypeName, "Full type names are not equal");
                    this.Assert.AreEqual(expectedComplexCollectionProperty.Value.Count, observedComplexCollectionProperty.Value.Count, "Collection counts are not equal");
                    for (int i = 0; i < expectedComplexCollectionProperty.Value.Count; i++)
                    {
                        Compare(expectedComplexCollectionProperty.Value[i], observedComplexCollectionProperty.Value[i]);
                    }

                    break;

                case ODataPayloadElementType.PrimitiveCollection:
                    var expectedPrimitiveCollection = expected as PrimitiveCollection;
                    var observedPrimitiveCollection = observed as PrimitiveCollection;
                    this.Assert.AreEqual(expectedPrimitiveCollection.Count, observedPrimitiveCollection.Count, "Collection counts are not equal");
                    for (int i = 0; i < expectedPrimitiveCollection.Count; i++)
                    {
                        Compare(expectedPrimitiveCollection[i], observedPrimitiveCollection[i]);
                    }

                    break;

                case ODataPayloadElementType.ComplexInstanceCollection:
                    var expectedComplexCollection = expected as ComplexInstanceCollection;
                    var observedComplexCollection = observed as ComplexInstanceCollection;
                    this.Assert.AreEqual(expectedComplexCollection.Count, observedComplexCollection.Count, "Collection counts are not equal");
                    for (int i = 0; i < expectedComplexCollection.Count; i++)
                    {
                        Compare(expectedComplexCollection[i], observedComplexCollection[i]);
                    }

                    break;

                case ODataPayloadElementType.NullPropertyInstance:
                case ODataPayloadElementType.EmptyUntypedCollection:
                    break;

                default:
                    this.Assert.Fail("Case " + expected.ElementType + " unhandled");
                    break;
            }
        }

        /// <summary>
        /// Asserts that two sets of annotaitons are the same (incl. the same order)
        /// </summary>
        /// <param name="expected">The expected set of annotations</param>
        /// <param name="observed">The actual set of annotations</param>
        private void Compare(IList<ODataPayloadElementAnnotation> expectedAnnotations, IList<ODataPayloadElementAnnotation> observedAnnotations)
        {
            for (int i = 0; i < expectedAnnotations.Count; i++)
            {
                ODataPayloadElementAnnotation expectedAnnotation = expectedAnnotations[i];
                ODataPayloadElementAnnotation matchingAnnotation = null;
                string errorMessage = null;
                matchingAnnotation = observedAnnotations.Where(ann => CompareAnnotation(expectedAnnotation, ann, out errorMessage)).SingleOrDefault();
                if (matchingAnnotation == null)
                {
                    this.Assert.Fail("For annotation " + expectedAnnotation.StringRepresentation + " no matching observed annotation was found." +
                    "Compare with expected annotations failed in: " + errorMessage);
                }
                else
                {
                    observedAnnotations.Remove(matchingAnnotation);
                }
            }
        }

        private bool CompareAnnotation(ODataPayloadElementAnnotation expectedAnnotation, ODataPayloadElementAnnotation observedAnnotation, out string errorMessage)
        {
            try
            {
                errorMessage = null;
                this.Assert.AreEqual(expectedAnnotation.GetType(), observedAnnotation.GetType(), "Annotation types should be same");
                if (expectedAnnotation is XmlTreeAnnotation)
                {
                    XmlTreeAnnotation e = expectedAnnotation as XmlTreeAnnotation;
                    XmlTreeAnnotation o = observedAnnotation as XmlTreeAnnotation;
                    this.Assert.IsNotNull(o, "Observed annotation cannot be null");
                    this.Assert.AreEqual(e.LocalName, o.LocalName, "Local names should match");
                    this.Assert.AreEqual(e.NamespaceName, o.NamespaceName, "Namespace names should be equal");
                    this.Assert.AreEqual(e.PropertyValue, o.PropertyValue, "Property values must be equal");
                    this.Assert.AreEqual(e.IsAttribute, o.IsAttribute, "IsAttribute values should be equal");
                    this.Assert.AreEqual(e.Children.Count, o.Children.Count, "Children count must be equal");
                    Compare(e.Children.Cast<ODataPayloadElementAnnotation>().ToList(), o.Children.Cast<ODataPayloadElementAnnotation>().ToList());
                }
                else if (expectedAnnotation is SelfLinkAnnotation)
                {
                    SelfLinkAnnotation e = expectedAnnotation as SelfLinkAnnotation;
                    SelfLinkAnnotation o = observedAnnotation as SelfLinkAnnotation;
                    this.Assert.IsNotNull(o, "Observed annotation cannot be null");
                    this.Assert.AreEqual(e.Value, o.Value, "Values should match");
                }
                else if (expectedAnnotation is ContentTypeAnnotation)
                {
                    ContentTypeAnnotation e = expectedAnnotation as ContentTypeAnnotation;
                    ContentTypeAnnotation o = observedAnnotation as ContentTypeAnnotation;
                    this.Assert.IsNotNull(o, "Observed annotation cannot be null");
                    this.Assert.AreEqual(e.Value, o.Value, "Values should match");
                }

                return true;
            }
            catch (DataComparisonException e)
            {
                errorMessage = "Message: " + e.Message + " Exception type:" + e.GetType() + " Stack:" + e.StackTrace;
                return false;
            }
        }
    }
}