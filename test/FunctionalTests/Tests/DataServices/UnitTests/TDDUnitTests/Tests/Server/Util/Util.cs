//---------------------------------------------------------------------
// <copyright file="Util.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.Tests.Server
{
    public static class ResourceUtil
    {
        private const string SystemSpatialBaseName = "Microsoft.Spatial";
        private const string SystemDataServicesBaseName = "Microsoft.OData.Service";
        private const string SystemDataServicesClientBaseName = "Microsoft.OData.Client";
        private const string SystemDataServicesDesignBaseName = "Microsoft.OData.Service.Design";
        private const string ODataLibBaseName = "Microsoft.OData.Core";

        public static System.Resources.ResourceManager SystemDataServicesResourceManager = new System.Resources.ResourceManager(SystemDataServicesBaseName, typeof(Microsoft.OData.Service.DataService<>).Assembly);

        public static string GetStringResource(System.Resources.ResourceManager manager, string name, params object[] args)
        {
            Assert.IsNotNull(manager, "ResourceManager");
            Assert.IsFalse(string.IsNullOrEmpty(name), "resource name parameter is empty");

            string res = manager.GetString(name, null/*use ResourceManager default, CultureInfo.CurrentUICulture*/);
            Assert.IsNotNull(res, "Failed to load resource \"{0}\"", name);

            if ((null != args) && (0 < args.Length))
            {
                res = String.Format(null/*use ResourceManager default, CultureInfo.CurrentUICulture*/, res, args);
            }

            return res;
        }
    }

    public static class DataServicesResourceUtil
    {
        public static string GetString(string name, params object[] args)
        {
            return ResourceUtil.GetStringResource(ResourceUtil.SystemDataServicesResourceManager, name, args);
        }
    }
}