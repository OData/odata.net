//---------------------------------------------------------------------
// <copyright file="PayloadElementODataWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    ///     Implementation of IPayloadElementODataWriter that converts payload elements into OData OM instances 
    ///     and uses an OData writer to write them.
    /// </summary>
    [ImplementationName(typeof(IPayloadElementODataWriter), "PayloadElementODataWriter")]
    public sealed class PayloadElementODataWriter : ODataPayloadElementVisitorBase, IPayloadElementODataWriter, IODataPayloadElementVisitor
    {
        private ODataWriter writer;
        private List<ODataProperty> currentProperties = new List<ODataProperty>();
        private List<NavigationPropertyInstance> currentNavigationProperties = new List<NavigationPropertyInstance>();
        private ODataNestedResourceInfo currentLink;

        /// <summary>
        /// Write the <paramref name="element"/> payload element to the <paramref name="writer"/> OData writer.
        /// </summary>
        /// <param name="writer">The OData writer to write to.</param>
        /// <param name="element">The element to write.</param>
        public void WritePayload(ODataWriter writer, ODataPayloadElement element)
        {
            this.writer = writer;
            try
            {
                base.Recurse(element);
                this.writer.Flush();
            }
            finally
            {
                this.writer = null;
            }
        }

        /// <summary>
        /// Visits an entity set instance: creates a new ODataResourceSet instance, calls ODataWriter.WriteStart()
        /// before visiting the entries and then calls ODataWriter.WriteEnd()
        /// </summary>
        /// <param name="payloadElement">The entity set instance to write.</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            // create an ODataResourceSet and write it
            ODataResourceSet feed = new ODataResourceSet()
            {
                // NOTE: the required Id is set when processing the annotations in AddFeedMetadata()
                Count = payloadElement.InlineCount,
                SerializationInfo = new ODataResourceSerializationInfo()
                {
                    NavigationSourceEntityTypeName = "Null",
                    NavigationSourceName = "MySet",
                    ExpectedTypeName = "Null"
                }
            };

            if (payloadElement.NextLink != null)
            {
                feed.NextPageLink = new Uri(payloadElement.NextLink);
            }

            this.writer.WriteStart(feed);
            base.Visit(payloadElement);
            this.writer.WriteEnd();
        }

        /// <summary>
        /// Visits an entity instance: creates a new ODataResource instance, collects and sets all the properties,
        /// calls ODataWriter.WriteStart(), then visits the navigation properties and calls ODataWriter.WriteEnd()
        /// </summary>
        /// <param name="payloadElement">The entity instance to write.</param>
        public override void Visit(EntityInstance payloadElement)
        {
            // create an ODataResource and write it
            string editLinkString = payloadElement.GetEditLink();
            string selfLinkString = payloadElement.GetSelfLink();
            string entryId = payloadElement.Id;

            var entry = new ODataResource()
            {
                Id = string.IsNullOrEmpty(entryId) ? null : new Uri(entryId),
                ETag = payloadElement.ETag,
                EditLink = string.IsNullOrEmpty(editLinkString) ? null : new Uri(editLinkString),
                ReadLink = string.IsNullOrEmpty(selfLinkString) ? null : new Uri(selfLinkString),
                TypeName = payloadElement.FullTypeName,
                SerializationInfo = new ODataResourceSerializationInfo()
                {
                    NavigationSourceEntityTypeName = payloadElement.FullTypeName,
                    NavigationSourceName = "MySet",
                    ExpectedTypeName = payloadElement.FullTypeName
                }
            };

            if (payloadElement.IsMediaLinkEntry())
            {
                ODataStreamReferenceValue mediaResource = new ODataStreamReferenceValue();
                mediaResource.ContentType = payloadElement.StreamContentType;
                mediaResource.ETag = payloadElement.StreamETag;

                mediaResource.ReadLink = new Uri(payloadElement.StreamSourceLink);
                mediaResource.EditLink = new Uri(payloadElement.StreamEditLink);

                entry.MediaResource = mediaResource;
            }

            Debug.Assert(this.currentNavigationProperties.Count == 0);
            Debug.Assert(this.currentProperties.Count == 0);

            // visit the properties (incl. navigation properties and named stream properties)
            base.Visit(payloadElement);

            // set the primitive and complex properties
            if (this.currentProperties.Count > 0)
            {
                entry.Properties = this.currentProperties;
                this.currentProperties = new List<ODataProperty>();
            }

            this.writer.WriteStart(entry);

            var navigationProperties = new ReadOnlyCollection<NavigationPropertyInstance>(this.currentNavigationProperties);
            this.currentNavigationProperties.Clear();

            // process the navigation properties/links
            for (int i = 0; i < navigationProperties.Count; ++i)
            {
                NavigationPropertyInstance navigationProperty = navigationProperties[i];
                this.currentLink = new ODataNestedResourceInfo()
                {
                    Name = navigationProperty.Name
                };
                base.Visit(navigationProperty);
                Debug.Assert(this.currentLink == null);
            }

            this.writer.WriteEnd();
        }

        /// <summary>
        /// Not supported; we collected primitive values already.
        /// <param name="payloadElement">A primitive value payload.</param>
        public override void Visit(PrimitiveValue payloadElement)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a primitive ODataProperty and sets the value.
        /// </summary>
        /// <param name="payloadElement">The primitive property to process.</param>
        public override void Visit(PrimitiveProperty payloadElement)
        {
            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = payloadElement.Value.ClrValue,
            };

            // not calling the base since we don't want to visit the value

            this.currentProperties.Add(odataProperty);
        }

        /// <summary>
        /// Creates a new MultiValue property and sets the value to the collection of primitive values.
        /// </summary>
        /// <param name="payloadElement">The primitive collection property to process.</param>
        public override void Visit(PrimitiveMultiValueProperty payloadElement)
        {
            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = new ODataCollectionValue()
                {
                    Items = payloadElement.Value.Select(pv => pv.ClrValue),
                    TypeName = payloadElement.Value.FullTypeName
                }
            };

            // not calling the base since we don't want to visit the primitive values in the collection

            this.currentProperties.Add(odataProperty);
        }

        /// <summary>
        /// Creates a new empty MultiValue property.
        /// </summary>
        /// <param name="payloadElement">The empty collection property to process.</param>
        public override void Visit(EmptyCollectionProperty payloadElement)
        {
            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = new ODataCollectionValue()
                {
                    TypeName = payloadElement.Value.FullTypeName
                }
            };

            base.Visit(payloadElement);

            this.currentProperties.Add(odataProperty);
        }

        /// <summary>
        /// Creates a new complex property; does not set a value that will happen in Visit(ComplexInstance)
        /// </summary>
        /// <param name="payloadElement">The primitive collection property to process.</param>
        public override void Visit(ComplexProperty payloadElement)
        {
            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = new ODataComplexValue()
                {
                    TypeName = payloadElement.Value.FullTypeName
                }
            };
            this.currentProperties.Add(odataProperty);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Creates a new complex value. If the complex instance is part of a complex collection value 
        /// adds the complex value to MultiValue property's items collection; otherwise sets the complex value as 
        /// value of a complex property.
        /// </summary>
        /// <param name="payloadElement"></param>
        public override void Visit(ComplexInstance payloadElement)
        {
            // figure out whether the complex value is part of a collection property (of complex values) or
            // the value for a complex property
            ODataProperty currentProperty = this.currentProperties.Last();
            ODataCollectionValue currentMultiValue = currentProperty.Value as ODataCollectionValue;

            ODataComplexValue complexValue;
            if (currentMultiValue != null)
            {
                // create a new complex value and add it to the MultiValue
                complexValue = new ODataComplexValue();
                complexValue.TypeName = payloadElement.FullTypeName.Substring(11, payloadElement.FullTypeName.Length - 12); ;
                // we construct the MultiValue so we know that the items are a list
                IList itemsAsList = currentMultiValue.Items as IList;
                itemsAsList.Add(complexValue);
            }
            else
            {
                complexValue = (ODataComplexValue)currentProperty.Value;
                complexValue.TypeName = payloadElement.FullTypeName;
            }

            var previousProperties = this.currentProperties;
            this.currentProperties = new List<ODataProperty>();

            base.Visit(payloadElement);

            complexValue.Properties = this.currentProperties;

            this.currentProperties = previousProperties;
        }


        /// <summary>
        /// Creates a new MultiValue property and sets the value to an empty collection that will be filled when visiting the complex instances.
        /// </summary>
        /// <param name="payloadElement">The complex MultiValue property to process.</param>
        public override void Visit(ComplexMultiValueProperty payloadElement)
        {
            var odataProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = new ODataCollectionValue()
                {
                    Items = new List<object>(),
                    TypeName = payloadElement.Value.FullTypeName
                }
            };
            this.currentProperties.Add(odataProperty);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Calls the base class method to process the instance collection.
        /// </summary>
        /// <param name="payloadElement">The complex instance collection to process.</param>
        public override void Visit(ComplexInstanceCollection payloadElement)
        {
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Calls the base class method to process the empty (untyped) collection.
        /// </summary>
        /// <param name="payloadElement">The empty collection to process.</param>
        public override void Visit(EmptyUntypedCollection payloadElement)
        {
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Calls the base class method to process the primitive collection.
        /// </summary>
        /// <param name="payloadElement">The primitive collection to process.</param>
        public override void Visit(PrimitiveCollection payloadElement)
        {
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Throws an InvalidOperationExcption; we expect null properties to be removed by the normalizer.
        /// </summary>
        /// <param name="payloadElement">The null property to process.</param>
        public override void Visit(NullPropertyInstance payloadElement)
        {
            ODataProperty property = new ODataProperty
            {
                Name = payloadElement.Name,
                Value = null
            };

            this.currentProperties.Add(property);
        }

        /// <summary>
        /// Calls the base class method to process an empty payload.
        /// </summary>
        /// <param name="payloadElement">The empty payload to process.</param>
        public override void Visit(EmptyPayload payloadElement)
        {
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Processes a navigation property.
        /// </summary>
        /// <param name="payloadElement">The navigation property to process.</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            this.currentNavigationProperties.Add(payloadElement);
        }

        /// <summary>
        /// Initializes an ODataNestedResourceInfo instance for the deferred link payload.
        /// </summary>
        /// <param name="payloadElement">The deferred link to process.</param>
        public override void Visit(DeferredLink payloadElement)
        {
            Debug.Assert(this.currentLink != null);
            ODataNestedResourceInfo navigationLink = this.currentLink;
            this.currentLink = null;

            // TODO, ckerer: where do I get the info whether this links is a singleton or collection?
            navigationLink.Url = new Uri(payloadElement.UriString);

            this.writer.WriteStart(navigationLink);
            base.Visit(payloadElement);
            this.writer.WriteEnd();
        }

        /// <summary>
        /// Initializes a new expanded ODataNestedResourceInfo instance and visits the payload.
        /// </summary>
        /// <param name="payloadElement">The expanded link to visit.</param>
        public override void Visit(ExpandedLink payloadElement)
        {
            Debug.Assert(this.currentLink != null);
            ODataNestedResourceInfo navigationLink = this.currentLink;
            this.currentLink = null;

            // TODO, ckerer: where do I get the info whether this links is a singleton or collection?
            navigationLink.Url = new Uri(payloadElement.UriString);

            this.writer.WriteStart(navigationLink);
            base.Visit(payloadElement);
            this.writer.WriteEnd();
        }

        /// <summary>
        /// Throws a NotSupportedException as the ODataWriter does not support link collections.
        /// </summary>
        /// <param name="payloadElement">The link collection to process.</param>
        public override void Visit(LinkCollection payloadElement)
        {
            throw new NotSupportedException("A link collection is not supported.");
        }

        /// <summary>
        /// Creates a new ODataStreamReferenceValue for the named stream and initializes it.
        /// </summary>
        /// <param name="payloadElement">The named stream to process.</param>
        public override void Visit(NamedStreamInstance payloadElement)
        {
            var odataNamedStream = new ODataStreamReferenceValue()
            {
                ETag = payloadElement.ETag,
                ContentType = payloadElement.EditLink == null ? payloadElement.SourceLinkContentType : payloadElement.EditLinkContentType,
                ReadLink = payloadElement.SourceLink == null ? null : new Uri(payloadElement.SourceLink),
                EditLink = payloadElement.EditLink == null ? null : new Uri(payloadElement.EditLink),
            };

            var odataNamedStreamProperty = new ODataProperty()
            {
                Name = payloadElement.Name,
                Value = odataNamedStream
            };

            this.currentProperties.Add(odataNamedStreamProperty);
            base.Visit(payloadElement);
        }
    }
}
