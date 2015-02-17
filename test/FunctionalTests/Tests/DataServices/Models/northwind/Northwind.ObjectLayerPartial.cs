//---------------------------------------------------------------------
// <copyright file="Northwind.ObjectLayerPartial.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace NorthwindModel
{
    using System.Collections.Generic;
    using System.Linq;
    
    /// <summary>
    /// There are no comments for NorthwindContext in the schema.
    /// </summary>
    public partial class NorthwindContext : System.Data.Objects.ObjectContext
    {
        public static string ContextConnectionString = "name=NorthwindContext";

        #region Helper methods for ServiceOperation code gen.

        public void VoidSingleMethod()
        {
        }

        public void VoidMultipleMethod()
        {
        }

        public string StringSingleMethod()
        {
            return "String";
        }

        public IEnumerable<string> StringMultipleMethod()
        {
            return new string[] { "s0", "s1" };
        }

        public IEnumerable<string> StringEnumerableSingleMethod()
        {
            return new string[] { "String0" };
        }

        public IEnumerable<string> StringEnumerableMultipleMethod()
        {
            return new string[] { "String0", "String1" };
        }

        public IQueryable<string> StringQueryableSingleMethod()
        {
            return new List<string>(StringEnumerableSingleMethod()).AsQueryable();
        }

        public IQueryable<string> StringQueryableMultipleMethod()
        {
            return new List<string>(StringEnumerableMultipleMethod()).AsQueryable();
        }

        public Customers CustomerSingleMethod()
        {
            return this.Customers.First();
        }

        public IQueryable<Customers> CustomerMultipleMethod()
        {
            return null;
        }

        public IEnumerable<Customers> CustomerEnumerableSingleMethod()
        {
            return this.Customers.Take(1);
        }

        public IEnumerable<Customers> CustomerEnumerableMultipleMethod()
        {
            return this.Customers.Take(3);
        }

        public IQueryable<Customers> CustomerQueryableSingleMethod()
        {
            return this.Customers.AsQueryable().Take(1);
        }

        public IQueryable<Customers> CustomerQueryableMultipleMethod()
        {
            return this.Customers.AsQueryable().Take(3);
        }

        #endregion Helper methods for ServiceOperation code gen.
    }
}
