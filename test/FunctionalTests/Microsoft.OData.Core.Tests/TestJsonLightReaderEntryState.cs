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
using Microsoft.OData.Edm;

namespace Microsoft.OData.Tests
{
    internal class TestJsonLightReaderEntryState : IODataJsonLightReaderResourceState
    {
        private ODataResource entry = ReaderUtils.CreateNewResource();
        private readonly EdmEntityType edmEntityType = new EdmEntityType("TestNamespace", "EntityType");
        private SelectedPropertiesNode selectedProperties;
        private DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(false, true);

        public ODataResource Resource
        {
            get { return this.entry; }
            set { this.entry = value; }
        }

        public IEdmEntityType EntityType
        {
            get
            {
                if (!this.edmEntityType.Properties().Any())
                {
                    this.edmEntityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32);
                }

                return this.edmEntityType;
            }
        }

        public ODataResourceMetadataBuilder MetadataBuilder { get; set; }

        public bool AnyPropertyFound { get; set; }

        public ODataJsonLightReaderNestedResourceInfo FirstNestedResourceInfo
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public DuplicatePropertyNamesChecker DuplicatePropertyNamesChecker
        {
            get { return this.duplicatePropertyNamesChecker; }
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