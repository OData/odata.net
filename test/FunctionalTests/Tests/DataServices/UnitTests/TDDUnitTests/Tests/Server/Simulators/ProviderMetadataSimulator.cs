//---------------------------------------------------------------------
// <copyright file="ProviderMetadataSimulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using Microsoft.OData.Service.Providers;
using System.Linq;

namespace AstoriaUnitTests.Tests.Server.Simulators
{
    internal class ProviderMetadataSimulator : IProviderMetadata
    {
        List<Type> types;

        public ProviderMetadataSimulator(List<Type> types)
        {
            this.types = types;
        }

        public IProviderType GetProviderType(string providerTypeName)
        {
            return new ProviderTypeSimulator(types.Where(t => String.Format("{0}.{1}", t.Namespace, t.Name) == providerTypeName).Single());
        }

        public Type GetClrType(StructuralType edmType)
        {
            throw new NotImplementedException();
        }
    }
}
