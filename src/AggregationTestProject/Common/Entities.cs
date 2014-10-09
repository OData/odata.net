using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AggregationTestProject.Common
{

    public class Sales
    {
        public int Id { get; set; }

        public Product Product { get; set; }

        public double Amount { get; set; }

        public int Time { get; set; }

    }

    public class Product
    {
        public int ProductIdentifier { get; set; }

        public Category Category { get; set; }

        public string ProductName { get; set; }

        public string Color { get; set; }

        public double TaxRate { get; set; }

    }

    public class Category
    {
        public int CategoryIdentifier { get; set; }

        public string Name { get; set; }

    }
}
