//---------------------------------------------------------------------
// <copyright file="OpenTypesUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    public static class OpenTypesUtil
    {
        public static void SetupDefaultOpenTypeAttributes(Workspace w)
        {
            if (w.DataLayerProviderKind != DataLayerProviderKind.NonClr)
                AstoriaTestLog.FailAndThrow("Open types not supported for data-layers other than NonCLR");

#if !ClientSKUFramework

            // let the open types tests do their own thing
            if (w is OpenTypesWorkspace)
                return;
#endif

            if (w.Name.Equals("Northwind", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (ResourceType type in w.ServiceContainer.ResourceTypes.Distinct().Where(t => !t.Facets.IsOpenType))
                    type.Facets.Add(NodeFacet.Attribute(new OpenTypeResourceAttribute(type, (rp => rp.IsNavigation || rp.PrimaryKey != null || rp.Facets.ConcurrencyModeFixed))));
            }
            else if (w.Name.Equals("Aruba", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (ResourceType type in w.ServiceContainer.ResourceTypes.Distinct().Where(t => !t.Facets.IsOpenType))
                    type.Facets.Add(NodeFacet.Attribute(new OpenTypeResourceAttribute(type, (rp => rp.IsNavigation || rp.PrimaryKey != null || rp.Facets.ConcurrencyModeFixed))));
            }

#if !ClientSKUFramework

            w.AfterServiceCreation.Add(() => (w as NonClrWorkspace).OpenTypeMethodsImplementation = System.Data.Test.Astoria.LateBound.OpenTypeMethodsImplementations.Realistic);
#endif
            // immediately after creating the service, register the undeclared properties' types
            w.AfterServiceCreation.Insert(0, () => RegisterTypesForUnDeclaredProperties(w));
        }

        public static void RegisterTypesForUnDeclaredProperties(Workspace w)
        {
            if (w.DataService == null || w.DataService.ConfigSettings == null)
                AstoriaTestLog.FailAndThrow("Cannot register complex types as service is not set up yet");

            IEnumerable<Type> toAdd = w.ServiceContainer.ResourceTypes
                .SelectMany(rt => rt.Properties.OfType<ResourceProperty>())
                .Where(rp => !rp.Facets.IsDeclaredProperty)
                .Select(rp => rp.Type.ClrType)
                .Distinct();
            //foreach (Type type in toAdd)
            //    w.DataService.ConfigSettings.AddRegisterKnownType(type.FullName);
        }
    }
}
