//---------------------------------------------------------------------
// <copyright file="CustomerDataSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace ExperimentsLib
{
    public class CustomerDataSet
    {
        /// <summary>
        /// Generates a collection of <see cref="Customer"/> instances.
        /// </summary>
        /// <param name="count">The number of objects to generate.</param>
        /// <returns>The generated collection.</returns>
        public static IEnumerable<Customer> GetCustomers(int count)
        {
            var data = new List<Customer>();
            for (int i = 1; i <= count; i++)
            {
                data.Add(new Customer
                {
                    Id = i,
                    Name = $"Cust{i} \uD800\udc05 \u00e4",
                    Emails = new List<string> { $"emailA@mailer.com{i}", $"emailB@mailer.com{i}" },
                    HomeAddress = new Address { City = $"City{i} \uD800\udc05 \u00e4", Misc = $"This is a test{i}", Street = $"Street{i}\n\"escape this\"" },
                    Addresses = new List<Address>
                    {
                        new Address { City = $"CityA{i}", Misc = $"This is a test A{i}", Street = $"StreetA{i}" },
                        new Address { City = $"CityB{i}", Misc = $"This is a test B{i}", Street = $"StreetB{i}" }
                    }
                });
            }

            return data;
        }
    }
}
