//---------------------------------------------------------------------
// <copyright file="DataServiceSimulatorWithGetService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.Collections.Generic;
    using AstoriaUnitTests.Tests.Server.Simulators;

    internal class DataServiceSimulatorWithGetService : DataServiceSimulator, IServiceProvider
    {
        private readonly Dictionary<Type, object> providers = new Dictionary<Type, object>();

        internal IDictionary<Type, object> Providers
        {
            get { return this.providers; }
        }

        public object GetService(Type serviceType)
        {
            object provider;
            if (!this.providers.TryGetValue(serviceType, out provider))
            {
                provider = null;
            }

            return provider;
        }
    }
}