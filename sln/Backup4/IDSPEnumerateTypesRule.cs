//---------------------------------------------------------------------
// <copyright file="IDSPEnumerateTypesRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using System;
    using Microsoft.FxCop.Sdk;
    using System.Collections.Generic;

    /// <summary>
    /// This rule checks that only DataServiceWrapper.Types can call IsComplexTypeVisible() and IsEntityTypeVisible().
    /// </summary>
    public class IDSPEnumerateTypesRule : BaseDataWebRule
    {
        private Method methodUnderCheck;

        /// <summary>Initializes a new <see cref="IDSPEnumerateTypesRule"/> instance.</summary>
        public IDSPEnumerateTypesRule()
            : base("IDSPEnumerateTypesRule")
        {
        }

        /// <summary>Visibility of targets to which this rule commonly applies.</summary>
        public override TargetVisibilities TargetVisibility
        {
            get
            {
                return TargetVisibilities.All;
            }
        }

        /// <summary>Checks type members.</summary>
        /// <param name="member">Member being checked.</param>
        /// <returns>A collection of problems found in <paramref name="member"/> or null.</returns>
        public override ProblemCollection Check(Member member)
        {
            Method method = member as Method;
            if (method == null)
            {
                return null;
            }

            methodUnderCheck = method;
            MethodCallFinder finder = new MethodCallFinder(MethodsNotAllowedOutsideMetadataPath.ToArray());
            finder.Visit(method);
            if (!IsMethodSafeToEnumerateTypes(method.FullName))
            {
                foreach (string metadataOnlyMethod in MethodsNotAllowedOutsideMetadataPath)
                {
                    if (finder.Found(metadataOnlyMethod))
                    {
                        this.Problems.Add(new Problem(GetResolution(method.FullName, metadataOnlyMethod)));
                    }
                }
            }

            return Problems.Count > 0 ? Problems : null;
        }

        /// <summary>
        /// 
        /// </summary>
        private static List<string> MethodsNotAllowedOutsideMetadataPath = new List<string>()
        {
            "Microsoft.OData.Service.Providers.DataServiceProviderWrapper.get_VisibleTypes",
            "Microsoft.OData.Service.Providers.DataServiceProviderWrapper.get_Types",
            "Microsoft.OData.Service.Providers.DataServiceProviderWrapper.get_ResourceSets",
            "Microsoft.OData.Service.Providers.DataServiceProviderWrapper.get_ServiceOperations",
            "Microsoft.OData.Service.Providers.IDataServiceProvider.get_Types",
            "Microsoft.OData.Service.Providers.IDataServiceProvider.get_ResourceSets",
            "Microsoft.OData.Service.Providers.IDataServiceProvider.get_ServiceOperations",
        };

        /// <summary>
        /// 
        /// </summary>
        private static List<string> MethodsSafeToEnumerateTypes = new List<string>()
        {
            "Microsoft.OData.Service.RequestDescription.UpdateMetadataVersion(Microsoft.OData.Service.Caching.DataServiceCacheItem,Microsoft.OData.Service.Providers.DataServiceProviderWrapper,Microsoft.OData.Service.Providers.BaseServiceProvider)",
            "Microsoft.OData.Service.Providers.ObjectContextServiceProvider.CheckConfigurationConsistency(Microsoft.OData.Service.DataServiceConfiguration)",
            "Microsoft.OData.Service.Providers.ObjectContextServiceProvider.GetMetadata(System.Xml.XmlWriter,Microsoft.OData.Service.Providers.DataServiceProviderWrapper,Microsoft.OData.Service.IDataService)",
            "Microsoft.OData.Service.Providers.ObjectContextServiceProvider+MetadataManager.#ctor(System.Data.Metadata.Edm.MetadataWorkspace,System.Data.Metadata.Edm.EntityContainer,Microsoft.OData.Service.Providers.DataServiceProviderWrapper,Microsoft.OData.Service.IDataService)",
            "Microsoft.OData.Service.Serializers.ServiceDocumentSerializer.WriteServiceDocument(Microsoft.OData.Service.Providers.DataServiceProviderWrapper)",
            "Microsoft.OData.Service.Providers.MetadataProviderEdmModel.GroupResourceTypesByNamespace(System.Boolean@,System.Boolean@)",
            "Microsoft.OData.Service.Providers.MetadataProviderEdmModel.EnsureStructuredTypes",
            "Microsoft.OData.Service.Providers.MetadataProviderEdmModel.PairUpNavigationProperties",
            "Microsoft.OData.Service.Providers.MetadataProviderEdmModel.EnsureEntityContainers",
            "Microsoft.OData.Service.Providers.DataServiceProviderWrapper.PopulateMetadataCacheItemForBuiltInProvider",
            "Microsoft.OData.Service.RequestDescription.UpdateMetadataVersion(Microsoft.OData.Service.Caching.DataServiceCacheItem,Microsoft.OData.Service.Providers.DataServiceProviderWrapper)"
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private bool IsMethodSafeToEnumerateTypes(string methodName)
        {
            return MethodsSafeToEnumerateTypes.Exists(s => s == methodName);
        }
    }
}
