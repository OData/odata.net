using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    public class CustomerDataSet
    {
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
                    HomeAddress = new Address { City = $"City{i} \uD800\udc05 \u00e4", Street = $"Street{i}\n\"escape this\"" },
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
