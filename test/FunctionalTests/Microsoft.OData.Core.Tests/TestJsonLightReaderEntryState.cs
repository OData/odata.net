//---------------------------------------------------------------------
// <copyright file="TestJsonLightReaderEntryState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Evaluation;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Tests
{
    internal class TestJsonLightReaderEntryState : IODataJsonLightReaderResourceState
    {
        private ODataResourceBase entry = ReaderUtils.CreateNewResource();
        private EdmStructuredType edmStructuredType = new EdmEntityType("TestNamespace", "EntityType");
        private SelectedPropertiesNode selectedProperties;
        private PropertyAndAnnotationCollector propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);

        public ODataResourceBase Resource
        {
            get { return this.entry; }
            set { this.entry = value; }
        }

        public IEdmStructuredType ResourceType
        {
            get
            {
                if (!this.edmStructuredType.Properties().Any())
                {
                    this.edmStructuredType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32);
                }

                return this.edmStructuredType;
            }
            set
            {
                this.edmStructuredType = (EdmStructuredType)value;
            }
        }

        public IEdmStructuredType ResourceTypeFromMetadata { get; set; }

        public IEdmNavigationSource NavigationSource { get; set; }

        public ODataResourceMetadataBuilder MetadataBuilder { get; set; }

        public bool AnyPropertyFound { get; set; }

        public ODataJsonLightReaderNestedResourceInfo FirstNestedResourceInfo
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public PropertyAndAnnotationCollector PropertyAndAnnotationCollector
        {
            get { return this.propertyAndAnnotationCollector; }
        }

        public SelectedPropertiesNode SelectedProperties
        {
            get
            {
                if (this.selectedProperties == null)
                {
                    throw new NotImplementedException();
                }

                return this.selectedProperties;
            }

            set { this.selectedProperties = value; }
        }

        public List<string> NavigationPropertiesRead
        {
            get { throw new NotImplementedException(); }
        }

        public bool ProcessingMissingProjectedNestedResourceInfos
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public LinkedList<IEdmNavigationProperty> MissingProjectedNavigationLinks
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}