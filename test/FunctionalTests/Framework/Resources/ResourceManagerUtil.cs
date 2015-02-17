//---------------------------------------------------------------------
// <copyright file="ResourceManagerUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Collections;
    using System.Resources;
    using System.Reflection;

    public class ResourceManagerUtil
    {
        private static string extName =".Resources";
        private static Dictionary<string, ResourceManager> _resourceManagerLookup;
        public static ResourceManager GetResourceManagerFromAssembly(Assembly assembly)
        {
            if (_resourceManagerLookup == null)
            {
                _resourceManagerLookup = new Dictionary<string, ResourceManager>();
            }
            if (_resourceManagerLookup.ContainsKey(assembly.FullName))
                return _resourceManagerLookup[assembly.FullName];
            else
            {
                string filename = GetResourceFileName(assembly, extName);
                string basename = filename.Remove(filename.IndexOf(extName, StringComparison.OrdinalIgnoreCase), extName.Length);
                
                if( assembly.FullName.Contains("DataSvcUtil") )
                    basename = basename.Replace(".", "");

                ResourceManager resourceMgr = new ResourceManager(basename, assembly);
                _resourceManagerLookup.Add(assembly.FullName, resourceMgr);
                return resourceMgr;
            }
   
        }
        private static string GetResourceFileName(Assembly assembly, string extensionName)
        {
            string[] resourceNames = assembly.GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.EndsWith(extensionName, StringComparison.OrdinalIgnoreCase))
                {
                    return resourceName;
                }
            }

            return null;
        }
    }

}
