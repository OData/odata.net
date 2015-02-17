//---------------------------------------------------------------------
// <copyright file="EmptyComplexTypesContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;

namespace AstoriaUnitTests.Stubs.EmptyComplexTypesNotSupported
{
    public class EmptyComplexTypesContext
    {
        public IQueryable<Customer> Customers
        {
            get
            {
                return Enumerable.Empty<Customer>().AsQueryable();
            }
        }
    }

    public class Customer
    {
        public int ID { get; set; }

        public EmptyComplexType EmptyComplexProperty { get; set; }
    }

    public class EmptyComplexType
    {
    }
}
