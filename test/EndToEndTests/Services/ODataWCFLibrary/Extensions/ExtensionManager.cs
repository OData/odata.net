//---------------------------------------------------------------------
// <copyright file="ExtensionManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.Hosting;

    public static class ExtensionManager
    {
        private static readonly CompositionContainer container = Create();

        public static CompositionContainer Container
        {
            get { return container; }
        }

        private static CompositionContainer Create()
        {
            var directory = default(string);
            if (HostingEnvironment.IsHosted)
            {
                directory = HostingEnvironment.MapPath("~/bin");
            }
            else
            {
                directory = Path.GetDirectoryName(typeof(ExtensionManager).Assembly.Location);
            }

            var directoryCatalog = new DirectoryCatalog(directory);

            return new CompositionContainer(directoryCatalog);
        }
    }
}
