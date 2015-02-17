//---------------------------------------------------------------------
// <copyright file="DataServiceContextExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
#if SILVERLIGHT

    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    public static class DataServiceContextExtensions
    {
        public static EntityDescriptor GetEntityDescriptor(this DataServiceContext context, object entity)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            return context.Entities.Where(e => e.Entity == entity).SingleOrDefault();
        }

        public static LinkDescriptor GetLinkDescriptor(this DataServiceContext context, object source, string sourceProperty, object target)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            return context.Links.Where(l => l.Source == source && l.Target == target && l.SourceProperty == sourceProperty).SingleOrDefault(); 
        }
    }

#endif
}
