//-----------------------------------------------------------------------------
// <copyright file="DefaultDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.Spatial;
using System.Collections.ObjectModel;

namespace Microsoft.OData.Client.E2E.Tests.Common
{
    public class DefaultDataSource
    {
        static DefaultDataSource()
        {
            Initialize();
        }

        public static IList<Person> People { get; private set; }

        /// <summary>
        /// Populates the data source.
        /// </summary>
        private static void Initialize()
        {
            People = new List<Person>()
            {
                new Customer()
                {
                    FirstName = "Bob",
                    LastName = "Cat",
                    Numbers = new Collection<string> { "111-111-1111", "012", "310", "bca", "ayz" },
                    Emails = new Collection<string> { "abc@abc.com" },
                    PersonID = 1,
                    Birthday = new DateTimeOffset(new DateTime(1957, 4, 3)),
                    City = "London",
                    Home = GeographyPoint.Create(32.1, 23.1),
                    TimeBetweenLastTwoOrders = new TimeSpan(1),
                    HomeAddress = new HomeAddress()
                    {
                        City = "Tokyo",
                        PostalCode = "98052",
                        Street = "1 Microsoft Way",
                        FamilyName = "Cats"
                    },
                    Addresses = new Collection<Address>
                    {
                        new HomeAddress()
                        {
                            City = "Tokyo",
                            PostalCode = "98052",
                            Street = "1 Microsoft Way",
                            FamilyName = "Cats"
                        },
                        new Address()
                        {
                            City = "Shanghai",
                            PostalCode = "200000",
                            Street = "999 Zixing Road"
                        }
                    }
                },

                new Customer()
                {
                    FirstName = "Jill",
                    LastName = "Jones",
                    Numbers = new Collection<string>(),
                    Emails = new Collection<string>(),
                    PersonID = 2,
                    Birthday = new DateTimeOffset(new DateTime(1983, 1, 15)),
                    City = "Sydney",
                    Home = GeographyPoint.Create(15.0, 161.8),
                    TimeBetweenLastTwoOrders = new TimeSpan(2)
                },

                new Employee()
                {
                    FirstName = "Jacob",
                    LastName = "Zip",
                    Numbers = new Collection<string> { "333-333-3333" },
                    Emails = new Collection<string> { null },
                    PersonID = 3,
                    DateHired = new DateTimeOffset(new DateTime(2010, 12, 13)),
                    Home = GeographyPoint.Create(15.0, 161.8),
                    Office = GeographyPoint.Create(15.0, 162),
                    HomeAddress = new HomeAddress()
                    {
                        City = "Sydney",
                        PostalCode = "98052",
                        Street = "1 Microsoft Way",
                        FamilyName = "Zips"
                    },
                    Addresses = new Collection<Address>
                    {
                        new Address()
                        {
                            City = "Shanghai2",
                            PostalCode = "200000",
                            Street = "B01, 999 Zixing Road"
                        }
                    }
                },
                new Employee()
                {
                    FirstName = "Elmo",
                    LastName = "Rogers",
                    Numbers = new Collection<string> { "444-444-4444", "555-555-5555", "666-666-6666" },
                    Emails = new Collection<string> { "def@def.org", "lmn@lmn.com","max@max.com","test@test.com" },
                    PersonID = 4,
                    DateHired = new DateTimeOffset(new DateTime(2008, 3, 27)),
                    Home = GeographyPoint.Create(-15.0, -61.8),
                    Office = GeographyPoint.Create(-15.0, -62),
                    Addresses = new Collection<Address>
                    {
                        new Address()
                        {
                            City = "Shanghai2",
                            PostalCode = "200000",
                            Street = "B01, 999 Zixing Road"
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Peter",
                    LastName = "Bee",
                    MiddleName = null,
                    Numbers = new Collection<string> { "555-555-5555" },
                    Emails = new Collection<string> { "def@test.msn" },
                    PersonID = 5,
                    Home = GeographyPoint.Create(-16.0, -261.8),
                    Addresses = new Collection<Address>
                    {
                        new HomeAddress()
                        {
                            City = "Tokyo",
                            PostalCode = "98052",
                            Street = "2 Microsoft Way",
                            FamilyName = "Cats"
                        },
                        new Address()
                        {
                            City = "Shanghai",
                            PostalCode = "200000",
                            Street = "999 Zixing Road"
                        },
                    }
                }
            };
        }
    }
}
