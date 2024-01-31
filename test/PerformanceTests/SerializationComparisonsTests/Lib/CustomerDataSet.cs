//---------------------------------------------------------------------
// <copyright file="CustomerDataSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;

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
                    Bio = $"This is a bio {i}",
                    Content = new byte[] { 1, 2, 3, 4, (byte)(i % 256) },
                    HomeAddress = new Address { City = $"City{i} \uD800\udc05 \u00e4", Misc = $"This is a test{i}", Street = $"Street{i}\n\"escape this\"" },
                    Addresses = new List<Address>
                    {
                        new Address { City = $"CityA{i}", Misc = $"This is a test A{i}", Street = $"StreetA{i}" },
                        new Address { City = $"CityB{i}", Misc = $"This is a test B{i}", Street = $"StreetB{i}" }
                    }
                }); ;
            }

            return data;
        }

        public static IEnumerable<Customer> GetDataWithLargeFields(int count)
        {
            var data = new List<Customer>();
            for (int i = 1; i <= count; i++)
            {
                data.Add(new Customer
                {
                    Id = i,
                    Name = $"Cust{i} \uD800\udc05 \u00e4",
                    Emails = new List<string> { $"emailA@mailer.com{i}", $"emailB@mailer.com{i}" },
                    Bio = GetBioText(i, 1024 * 1024),
                    Content = GetContentBytes(i, 1024 * 1024),
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

        private static string GetBioText(int index, int minLength)
        {
            string baseString = $"Cust{index} abcxyz123-_ 𐀅 ä";
            StringBuilder builder = new StringBuilder();
            while (builder.Length < minLength)
            {
                builder.Append(baseString);
            }

            return builder.ToString();
        }

        private static byte[] GetContentBytes(int index, int minLength)
        {
            byte[] content = new byte[minLength];
            content[0] = (byte)(index % 256);
            for (int i = 1; i < content.Length; i++)
            {
                content[i] = (byte)(i % 256);
            }

            return content;
        }
    }
}
