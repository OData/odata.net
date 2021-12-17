using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    class FieldNameAttribute : Attribute
    {
        public string Name { get; set; }

        public FieldNameAttribute(string name)
        {
            Name = name;
        }
    }
}
