//---------------------------------------------------------------------
// <copyright file="VersioningUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.Xml.Linq;
    using AstoriaUnitTests.Tests.RequestDescriptionFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #region ServiceVersion
    public class ServiceVersion
    {
        public int Version { get; set; }

        public ServiceVersion() { }

        public ServiceVersion(int version)
        {
            this.Version = version;
        }

        public ServiceVersion(ODataProtocolVersion version)
        {
            switch (version)
            {
                case ODataProtocolVersion.V4: this.Version = 40; break;
            }
        }

        public static implicit operator ServiceVersion(int? version)
        {
            return version.HasValue ? new ServiceVersion(version.Value) : null;
        }

        public static ServiceVersion Combine(ServiceVersion a, ServiceVersion b)
        {
            if (a == null) { return b; }
            if (b == null) { return a; }
            if (a.Version > b.Version) { return a; }
            return b;
        }

        public static string FormatVersion(int version)
        {
            return string.Format("{0}.{1}", (version / 10).ToString(), (version % 10).ToString());
        }

        public override string ToString()
        {
            return FormatVersion(this.Version);
        }

        public ODataProtocolVersion ToProtocolVersion()
        {
            if (this.Version == 40) return ODataProtocolVersion.V4;
            else Assert.Fail("Can't convert version value '" + this.ToString() + "' to a ODataProtocolVersion."); return ODataProtocolVersion.V4;
        }

        public static ServiceVersion FromHeaderValue(string value)
        {
            value = value.Split(';').First(s => s.Length > 0);
            int major = Int32.Parse(value.Substring(0, value.IndexOf('.')));
            int minor = Int32.Parse(value.Substring(value.IndexOf('.') + 1));
            return new ServiceVersion(major * 10 + minor);
        }

        public static ServiceVersion[] ValidVersions = new ServiceVersion[] { null, 40 };

        public static ODataProtocolVersion[] DataServiceProtocolVersions = new ODataProtocolVersion[] {
                ODataProtocolVersion.V4,
            };
    }
    #endregion

    #region ServiceVersions
    public class ServiceVersions
    {
        private ServiceVersion minRequestDSV;
        public ServiceVersion MinRequestDSV
        {
            get { return this.minRequestDSV; }
            set
            {
                Assert.IsNull(this.minRequestDSV, "ServiceVersions should be 'immutable'.");
                this.minRequestDSV = value;
            }
        }
        private ServiceVersion minRequestMDSV;
        public ServiceVersion MinRequestMDSV
        {
            get { return this.minRequestMDSV; }
            set
            {
                Assert.IsNull(this.minRequestMDSV, "ServiceVersions should be 'immutable'.");
                this.minRequestMDSV = value;
            }
        }
        private ServiceVersion responseDSV;
        public ServiceVersion ResponseDSV
        {
            get { return this.responseDSV; }
            set
            {
                Assert.IsNull(this.responseDSV, "ServiceVersions should be 'immutable'.");
                this.responseDSV = value;
            }
        }

        public ServiceVersions()
        {
            this.MinRequestDSV = 40;
            this.MinRequestMDSV = 40;
            this.ResponseDSV = 40;
        }

        public ServiceVersions(ServiceVersions source)
        {
            if (source != null)
            {
                this.MinRequestDSV = source.MinRequestDSV;
                this.MinRequestMDSV = source.MinRequestMDSV;
                this.ResponseDSV = source.ResponseDSV;
            }
        }

        public static ServiceVersions Combine(ServiceVersions a, ServiceVersions b)
        {
            if (a == null) return b;
            if (b == null) return a;

            ServiceVersions result = new ServiceVersions();
            result.MinRequestDSV = ServiceVersion.Combine(a.MinRequestDSV, b.MinRequestDSV);
            result.MinRequestMDSV = ServiceVersion.Combine(a.MinRequestMDSV, b.MinRequestMDSV);
            result.ResponseDSV = ServiceVersion.Combine(a.ResponseDSV, b.ResponseDSV);
            return result;
        }

        public static ServiceVersions Override(ServiceVersions a, ServiceVersions b)
        {
            if (a == null) return b;
            if (b == null) return a;

            ServiceVersions result = new ServiceVersions();
            result.MinRequestDSV = b.MinRequestDSV ?? a.MinRequestDSV;
            result.MinRequestMDSV = b.MinRequestMDSV ?? a.MinRequestMDSV;
            result.ResponseDSV = b.ResponseDSV ?? a.ResponseDSV;
            return result;

        }

        public override string ToString()
        {
            return string.Format("MinDSV: {0}, MinMDSV: {1}, ResponseDSV: {2}",
                this.MinRequestDSV.Version,
                this.MinRequestMDSV.Version,
                this.ResponseDSV.Version);
        }

        public bool AreRequestVersionsValid(ServiceVersion DSV, ServiceVersion MDSV)
        {
            if (DSV != null && DSV.Version < this.MinRequestDSV.Version)
            {
                return false;
            }

            // If DSV is null and MDSV is specified, then DSV takes the MDSV value for 2.0 or above.
            // Otherwise the DSV values is set to 2.0
            if (DSV == null && MDSV != null)
            {
                int version = (MDSV.Version >= 20) ? MDSV.Version : 20;
                if (version < this.MinRequestDSV.Version)
                {
                    return false;
                }
            }

            if (MDSV != null && MDSV.Version < this.MinRequestMDSV.Version)
            {
                return false;
            }

            return true;
        }
    }
    #endregion

    #region VersionsAnnotationsAndFacets
    public enum UpdatePathType
    {
        Unknown,
        ResourceSet,
        Property,
        PropertyValue,
        SingleLink,
        LinkCollection,
        Stream,
    }

    /// <summary>Extension methods class which provides first class support for version annotations and facets</summary>
    public static class VersionsAnnotationsAndFacets
    {
        private const string GETVersionsAnnotationName = "GETVersions";
        private const string PUTMERGEVersionsAnnotationName = "PUTMERGEVersions";
        private const string POSTVersionsAnnotationName = "POSTVersions";
        private const string ExpandPropertyAnnotationName = "ExpandProperty";
        private const string SpecialExpandOnLinksPropertyAnnotationName = "SpecialExpandOnLinksProperty";
        private const string InherentOrderByKeysAnnotationName = "InherentOrderByKeys";
        private const string InvalidAnnotationName = "Invalid";
        private const string InvalidClientRequestAnnotationName = "InvalidClientRequest";
        private const string PropertyForPropertyOnlyOperationName = "PropertyForPropertyOnlyOperation";
        private const string MaxProtocolVersionName = "MaxProtocolVersion";
        private const string BaseServiceProtocolVersionName = "BaseServiceProtocolVersion";   // The minimum protocol version with which the service will even start
        private const string MetadataVersionName = "MetadataVersion";
        private const string MetadataEdmNamespaceName = "MetadataEdmNamespace";
        private const string IncludeRelationshipLinksInResponseName = "IncludeAssociationLinksInResponse";
        private const string UpdatePathTypeName = "UpdatePathType";
        private const string ExpandedResourceTypesName = "ExpandedResourceTypes";
        private const string PostServiceOperationName = "PostServiceOperation";

        public static void SetIncludeRelationshipLinksInResponse(this RequestDescription requestDescription, bool value)
        { requestDescription.SetAnnotation(IncludeRelationshipLinksInResponseName, value); }
        public static bool GetIncludeRelationshipLinksInResponse(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotationOrDefault(IncludeRelationshipLinksInResponseName, false); }

        public static void SetUpdatePathType(this RequestDescription requestDescription, UpdatePathType value)
        { requestDescription.SetAnnotation(UpdatePathTypeName, value); }
        public static UpdatePathType GetUpdatePathType(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotationOrDefault(UpdatePathTypeName, UpdatePathType.Unknown); }

        public static void SetGETVersions(this RequestDescription requestDescription, ServiceVersions versions)
        { requestDescription.SetAnnotation(GETVersionsAnnotationName, versions); }
        public static ServiceVersions GetGETVersions(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotation<ServiceVersions>(GETVersionsAnnotationName); }

        public static void SetPUTPATCHVersions(this RequestDescription requestDescription, ServiceVersions versions)
        { requestDescription.SetAnnotation(PUTMERGEVersionsAnnotationName, versions); }
        public static ServiceVersions GetPUTPATCHVersions(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotation<ServiceVersions>(PUTMERGEVersionsAnnotationName); }

        public static void SetPOSTVersions(this RequestDescription requestDescription, ServiceVersions versions)
        { requestDescription.SetAnnotation(POSTVersionsAnnotationName, versions); }
        public static ServiceVersions GetPOSTVersions(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotation<ServiceVersions>(POSTVersionsAnnotationName); }

        public static void SetExpandProperty(this RequestDescription requestDescription, ResourceProperty expandProperty)
        { requestDescription.SetAnnotation(ExpandPropertyAnnotationName, expandProperty); }
        public static ResourceProperty GetExpandProperty(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotation<ResourceProperty>(ExpandPropertyAnnotationName); }

        public static void SetSpecialExpandOnLinksProperty(this RequestDescription requestDescription, ResourceProperty expandProperty)
        { requestDescription.SetAnnotation(SpecialExpandOnLinksPropertyAnnotationName, expandProperty); }
        public static ResourceProperty GetSpecialExpandOnLinksProperty(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotation<ResourceProperty>(SpecialExpandOnLinksPropertyAnnotationName); }

        public static void SetInherentOrderByKeys(this RequestDescription requestDescription)
        { requestDescription.SetAnnotation(InherentOrderByKeysAnnotationName, true); }
        public static bool GetInherentOrderByKeys(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotation<bool?>(InherentOrderByKeysAnnotationName) == true; }

        public static void SetInvalid(this RequestDescription requestDescription)
        { requestDescription.SetAnnotation(InvalidAnnotationName, true); }
        public static bool GetInvalid(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotation<bool?>(InvalidAnnotationName) == true; }

        public static void SetInvalidClientRequest(this RequestDescription requestDescription)
        { requestDescription.SetAnnotation(InvalidClientRequestAnnotationName, true); }
        public static bool GetInvalidClientRequest(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotation<bool?>(InvalidClientRequestAnnotationName) == true; }

        public static void SetPropertyForPropertyOnlyOperation(this RequestDescription requestDescription, ResourceProperty property)
        { requestDescription.SetAnnotation(PropertyForPropertyOnlyOperationName, property); }
        public static ResourceProperty GetPropertyForPropertyOnlyOperation(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotation<ResourceProperty>(PropertyForPropertyOnlyOperationName); }

        public static void AddExpandedResourceType(this RequestDescription requestDescription, ResourceType resourceType)
        {
            List<ResourceType> resourceTypes = requestDescription.GetExpandedResourceTypes();
            if (resourceTypes == null)
            {
                resourceTypes = new List<ResourceType>();
                requestDescription.SetAnnotation(ExpandedResourceTypesName, resourceTypes);
            }
            resourceTypes.Add(resourceType);
        }
        public static List<ResourceType> GetExpandedResourceTypes(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotation<List<ResourceType>>(ExpandedResourceTypesName); }

        public static void SetPostServiceOperation(this RequestDescription requestDescription)
        { requestDescription.SetAnnotation(PostServiceOperationName, true); }
        public static bool GetPostServiceOperation(this RequestDescription requestDescription)
        { return requestDescription.GetAnnotation<bool?>(PostServiceOperationName) == true; }

        public static void SetMaxProtocolVersion(this ServiceContextDescription serviceDescription, ODataProtocolVersion? protocolVersion)
        { serviceDescription.SetAnnotation(MaxProtocolVersionName, protocolVersion); }
        public static ODataProtocolVersion? GetMaxProtocolVersion(this ServiceContextDescription serviceDescription)
        { return serviceDescription.GetAnnotation<ODataProtocolVersion?>(MaxProtocolVersionName); }
        public static ODataProtocolVersion GetMaxProtocolVersion(this ServiceContextDescription serviceDescription, ODataProtocolVersion defaultVersion)
        { return GetMaxProtocolVersion(serviceDescription) ?? defaultVersion; }

        public static void SetBaseServiceProtocolVersion(this ServiceContextDescription serviceDescription, ODataProtocolVersion protocolVersion)
        { serviceDescription.SetAnnotation(BaseServiceProtocolVersionName, protocolVersion); }
        public static ODataProtocolVersion GetBaseServiceProtocolVersion(this ServiceContextDescription serviceDescription)
        { return serviceDescription.GetAnnotation<ODataProtocolVersion>(BaseServiceProtocolVersionName); }

        public static void SetMetadataVersion(this ServiceContextDescription serviceDescription, ServiceVersion metadataVersion)
        { serviceDescription.SetAnnotation(MetadataVersionName, metadataVersion); }
        public static ServiceVersion GetMetadataVersion(this ServiceContextDescription serviceDescription)
        { return serviceDescription.GetAnnotation<ServiceVersion>(MetadataVersionName); }

        public static void SetMetadataEdmNamespace(this ServiceContextDescription serviceDescription, XNamespace edmNamespace)
        { serviceDescription.SetAnnotation(MetadataEdmNamespaceName, edmNamespace); }
        public static XNamespace GetMetadataEdmNamespace(this ServiceContextDescription serviceDescription)
        { return serviceDescription.GetAnnotation<XNamespace>(MetadataEdmNamespaceName); }
    }
    #endregion

    #region HelperExtensionMethods
    /// <summary>Helper extension methods for versioning tests.</summary>
    public static class VersioningHelpers
    {
        public static bool HasNamedStreams(this ComplexType resourceType)
        {
            return resourceType.Facets.NamedStreams.Any();
        }

        public static bool IsNamedStream(this ResourceProperty resourceProperty)
        {
            return resourceProperty.ResourceType.Facets.NamedStreams.Contains(resourceProperty.Name);
        }

        public static bool HasNavigationProperty(this ComplexType resourceType)
        {
            return resourceType.Properties.OfType<ResourceProperty>().Any(p => p.IsNavigation);
        }

        public static bool HasNavigationPropertyOnInheritanceTree(this ResourceType resourceType)
        {
            IEnumerable<ResourceType> inheritanceTreeTypesAndSelf = (new ResourceType[] { resourceType }).Concat(resourceType.DerivedTypes);
            foreach (var type in inheritanceTreeTypesAndSelf)
            {
                if (type.HasNavigationProperty()) return true;
            }

            return false;
        }

        public static bool HasNamedStreamsOnInheritanceTree(this ResourceType resourceType)
        {
            IEnumerable<ResourceType> inheritanceTreeTypesAndSelf = (new ResourceType[] { resourceType }).Concat(resourceType.DerivedTypes);
            foreach (var type in inheritanceTreeTypesAndSelf)
            {
                if (type.HasNamedStreams()) return true;
            }

            return false;
        }

        public static bool HasCollectionProperty(this ComplexType resourceType)
        {
            return TypeHasCollectionProperty(resourceType, new HashSet<ComplexType>());
        }

        public static bool HasCollectionPropertyOnInheritanceTree(this ResourceType resourceType)
        {
            IEnumerable<ResourceType> inheritanceTreeTypesAndSelf = (new ResourceType[] { resourceType }).Concat(resourceType.DerivedTypes);
            foreach (var type in inheritanceTreeTypesAndSelf)
            {
                if (TypeHasCollectionProperty(type, new HashSet<ComplexType>())) return true;
            }

            return false;
        }

        private static bool TypeHasCollectionProperty(ComplexType resourceType, HashSet<ComplexType> visitedTypes)
        {
            visitedTypes.Add(resourceType);

            foreach (NodeProperty property in resourceType.Properties)
            {
                if (property.Type is PrimitiveOrComplexCollectionType) return true;
                if (property.Type is ComplexType && !((property is ResourceProperty) && ((ResourceProperty)property).IsNavigation))
                {
                    if (!visitedTypes.Contains(property.Type))
                    {
                        if (TypeHasCollectionProperty((ComplexType)property.Type, visitedTypes))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
    #endregion
}
