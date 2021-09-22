using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SerializationBaselineTests
{
    interface IExperimentWriter
    {
        Task WriteCustomers(IEnumerable<Customer> payload, Stream stream);
    }
}
