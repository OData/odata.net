using System.Collections.Generic;

namespace SerializationBaselineTests
{
    static class DataSet
    {
        public static IEnumerable<Customer> GetCustomers(int count)
        {
            var data = new List<Customer>();
            for (int i = 1; i <= count; i++)
            {
                data.Add(new Customer
                {
                    Id = i,
                    Name = $"Cust{i}",
                    Emails = new List<string> { $"emailA${i}", $"emailB{i} " },
                    HomeAddress = new Address { City = $"City{i}", Street = $"Street{i}\n\"escape this\"" },
                    Addresses = new List<Address>
                    {
                        new Address { City = $"CityA{i}", Street = $"StreetA{i}" },
                        new Address { City = $"CityB{i}", Street = $"StreetA{i}" }
                    }
                });
            }

            return data;
        }
    }
}
