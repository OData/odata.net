//---------------------------------------------------------------------
// <copyright file="ProviderTypeSimulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Service.Providers;
using System.Reflection;

namespace AstoriaUnitTests.Tests.Server.Simulators
{
    internal class ProviderTypeSimulator : IProviderType
    {
        private Type type;

        public ProviderTypeSimulator(Type type)
        {
            this.type = type;
        }

        public IEnumerable<IProviderMember> Members
        {
            get
            {
                foreach (PropertyInfo property in type.GetProperties().Where(p => p.DeclaringType == type))
                {
                    yield return new ProviderMemberSimulator(property);
                }
            }
        }

        public string Name
        {
            get
            {
                return type.Name;
            }
        }
    }
}
