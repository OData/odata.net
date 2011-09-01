//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Data.OData.Samples.Services.Data
{
    public class Employee
    {
        public Employee()
        {
            Customers = new List<Customer>();
        }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public Address Address { get; set; }

        public IEnumerable<Customer> Customers;
    }
}
