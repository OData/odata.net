//---------------------------------------------------------------------
// <copyright file="ServicePrototype.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;

namespace Microsoft.OData
{
    internal class ServicePrototype<TService>
    {
        public ServicePrototype(TService instance)
        {
            Debug.Assert(instance != null, "instance != null");

            this.Instance = instance;
        }

        public TService Instance { get; private set; }
    }
}
