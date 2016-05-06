//---------------------------------------------------------------------
// <copyright file="ContainerBuilderHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData;

namespace Microsoft.Test.OData.DependencyInjection
{
    public static class ContainerBuilderHelper
    {
        public static IServiceProvider BuildContainer(Action<IContainerBuilder> action)
        {
            IContainerBuilder builder = new TestContainerBuilder();

            builder.AddDefaultODataServices();

            if (action != null)
            {
                action(builder);
            }

            return builder.BuildContainer();
        }
    }
}
