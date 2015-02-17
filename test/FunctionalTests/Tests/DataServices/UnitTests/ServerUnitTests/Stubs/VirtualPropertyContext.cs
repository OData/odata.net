//---------------------------------------------------------------------
// <copyright file="VirtualPropertyContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;

namespace AstoriaUnitTests.Stubs.VirtualPropertiesAreSupported
{
    using System.Collections.Generic;

    public class VirtualPropertyContext
    {
        public VirtualPropertyContext()
        {
            this.VirtualProperty = new List<Var1>().AsQueryable();
        }
        public virtual IQueryable<Var1> VirtualProperty { get; set; }
    }

    public class Var1
    {
        public int ID { get; set; }
    }
}
