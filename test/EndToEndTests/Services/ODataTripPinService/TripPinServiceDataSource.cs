//---------------------------------------------------------------------
// <copyright file="TripPinServiceDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Web.Hosting;
    using Microsoft.OData.Edm;
    using Microsoft.OData.SampleService.Models.TripPin;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    [Serializable]
    public class TripPinServiceDataSource : ODataReflectionDataSource
    {
        public EntityCollection<Person> People { get; private set; }
        public EntityCollection<Airport> Airports { get; private set; }
        public EntityCollection<Airline> Airlines { get; private set; }
        public EntityCollection<Photo> Photos { get; private set; }
        public Person Me { get; private set; }

        public TripPinServiceDataSource()
        {
            this.OperationProvider = new TripPinServiceOperationProvider();
        }

        public override void Reset()
        {
            this.People = new EntityCollection<Person>();
            this.Airports = new EntityCollection<Airport>();
            this.Airlines = new EntityCollection<Airline>();
            this.Photos = new EntityCollection<Photo>();
        }

        public override void Initialize()
        {
            #region Airports

            this.Airports.AddRange(new List<Airport>()
            {
                new Airport()
                {
                    Name = "San Francisco International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "South McDonnell Road, San Francisco, CA 94128",
                        City = new City()
                        {
                            Name = "San Francisco",
                            CountryRegion = "United States",
                            Region = "California"    
                        },
                        Loc = GeographyPoint.Create(37.6188888888889, -122.374722222222)
                    },
                    IataCode = "SFO",
                    IcaoCode = "KSFO"
                },
                new Airport()
                {
                    Name = "Los Angeles International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "1 World Way, Los Angeles, CA, 90045",
                        City = new City()
                        {
                            Name = "Los Angeles",
                            CountryRegion = "United States", 
                            Region = "California"
                        },
                        Loc = GeographyPoint.Create(33.9425, -118.408055555556)
                    },
                    IataCode = "LAX",
                    IcaoCode = "KLAX"
                },
                new Airport()
                {
                    Name = "Shanghai Hongqiao International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Hongqiao Road 2550, Changning District",
                        City = new City()
                        {
                            Name = "Shanghai",
                            CountryRegion = "China",
                            Region = "Shanghai"
                        },
                        Loc = GeographyPoint.Create(31.1977777777778, 121.336111111111)
                    },
                    IataCode = "SHA",
                    IcaoCode = "ZSSS"
                },
                new Airport()
                {
                    Name = "Beijing Capital International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Airport Road, Chaoyang District, Beijing, 100621",
                        City = new City()
                        {
                            Name = "Beijing",
                            CountryRegion = "China",
                            Region = "Beijing"
                        },
                        Loc = GeographyPoint.Create(40.08, 116.584444444444)
                    },
                    IataCode = "PEK",
                    IcaoCode = "ZBAA"
                },
                new Airport()
                {
                    Name = "John F. Kennedy International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Jamaica, New York, NY 11430",
                        City = new City()
                        {
                            Name = "New York City",
                            CountryRegion = "United States",
                            Region = "New York"
                        },
                        Loc = GeographyPoint.Create(40.6397222222222, -73.7788888888889)
                    },
                    IataCode = "JFK",
                    IcaoCode = "KJFK"
                },
                new Airport()
                {
                    Name = "Rome Ciampino Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Via Appia Nuova, 1651",
                        City = new City()
                        {
                            Name = "Rome",
                            CountryRegion = "Italy",
                            Region = ""
                        },
                        Loc = GeographyPoint.Create(41.7991666666667, 12.5947222222222)
                    },
                    IataCode = "CIA",
                    IcaoCode = "LIRA"
                },
                new Airport()
                {
                    Name = "Toronto Pearson International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "6301 Silver Dart Dr Mississauga",
                        City = new City()
                        {
                            Name = "Mississauga",
                            CountryRegion = "Canada",
                            Region = "Ontario"
                        },
                        Loc = GeographyPoint.Create(43.6772222222222, -79.6305555555555)
                    },
                    IataCode = "YYZ",
                    IcaoCode = "CYYZ"
                },
                new Airport()
                {
                    Name = "Sydney Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Airport Dr Sydney NSW 2020",
                        City = new City()
                        {
                            Name = "Sydney",
                            CountryRegion = "Australia",
                            Region = ""
                        },
                        Loc = GeographyPoint.Create(-33.9461111111111, 151.177222222222)
                    },
                    IataCode = "SYD",
                    IcaoCode = "YSSY"
                },
                new Airport()
                {
                    Name = "Istanbul Ataturk Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Yeşilköy Mh.34149 İstanbul",
                        City = new City()
                        {
                            Name = "Istanbul",
                            CountryRegion = "Turkey",
                            Region = ""
                        },
                        Loc = GeographyPoint.Create(40.9766666666667, 28.8211111111111)
                    },
                    IataCode = "IST",
                    IcaoCode = "LTBA"
                },
                new Airport()
                {
                    Name = "Singapore Changi Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Airport Blvd, Singapore",
                        City = new City()
                        {
                            Name = "Changi",
                            CountryRegion = "Singapore",
                            Region = ""
                        },
                        Loc = GeographyPoint.Create(1.35555555555556, 103.987222222222),
                    },
                    IataCode = "SIN",
                    IcaoCode = "WSSS"
                },
                new Airport()
                {
                    Name = "Abu Dhabi International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Sheik Maktoum Bin Rashid Rd Abu Dhabi",
                        City = new City()
                        {
                            Name = "Abu Dhabi",
                            CountryRegion = "United Arab Emirates",
                            Region = ""
                        },
                        Loc = GeographyPoint.Create(24.4327777777778, 54.6511111111111)
                    },
                    IataCode = "AUH",
                    IcaoCode = "OMAA"
                },
                new Airport()
                {
                    Name = "Guangzhou Baiyun International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Jichang Road, Renhezhen, Huadu",
                        City = new City()
                        {
                            Name = "Guangzhou",
                            CountryRegion = "China",
                            Region = "Guangdong"
                        },
                        Loc = GeographyPoint.Create(23.1841666666667, 113.265833333333)
                    },
                    IataCode = "CAN",
                    IcaoCode = "ZGGG"
                },
                new Airport()
                {
                    Name = "O'Hare International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "10000 W O'Hare Ave",
                        City = new City()
                        {
                            Name = "Chicago",
                            CountryRegion = "United States",
                            Region = "Illinois"
                        },
                        Loc = GeographyPoint.Create(41.9794444444444, -87.9044444444445)
                    },
                    IataCode = "ORD",
                    IcaoCode = "KORD"
                },
                new Airport()
                {
                   Name = "Hartsfield-Jackson Atlanta International Airport",
                   Location = new AirportLocation()
                   {
                       Address = "6000 N Terminal Pkwy",
                       City = new City()
                       {
                           Name = "Atlanta",
                           CountryRegion = "United States",
                           Region = "Georgia"
                       },
                       Loc = GeographyPoint.Create(33.6402777777778, -84.4269444444444)
                   },
                   IataCode = "ATL",
                   IcaoCode = "KATL"
                },
                new Airport()
                {
                    Name = "Seattle-Tacoma International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "17801 International Blvd",
                        City = new City()
                        {
                            Name = "SeaTac",
                            CountryRegion = "United States",
                            Region = "Washington"
                        },
                        Loc = GeographyPoint.Create(47.4488888888889, -122.309166666667)
                    },
                    IataCode = "SEA",
                    IcaoCode = "KSEA"
                }
            });

            #endregion

            #region Airlines

            this.Airlines.AddRange(new List<Airline>()
            {
                new Airline()
                {
                    Name = "American Airlines",
                    AirlineCode = "AA" 
                },

                new Airline()
                {
                    Name = "Shanghai Airline",
                    AirlineCode = "FM"
                },

                new Airline()
                {
                    Name = "China Eastern Airlines",
                    AirlineCode = "MU"
                },

                new Airline()
                {
                    Name = "Air France",
                    AirlineCode = "AF"
                },

                new Airline()
                {
                    Name = "Alitalia",
                    AirlineCode = "AZ"
                },

                new Airline()
                {
                    Name = "Air Canada",
                    AirlineCode = "AC"
                },

                new Airline()
                {
                    Name = "Austrian Airlines",
                    AirlineCode = "OS"
                },

                new Airline()
                {
                    Name = "Turkish Airlines",
                    AirlineCode = "TK"
                },

                new Airline()
                {
                    Name = "Japan Airlines",
                    AirlineCode = "JL"
                },

                new Airline()
                {
                    Name = "Singapore Airlines",
                    AirlineCode = "SQ"

                },

                new Airline()
                {
                    Name = "Korean Air",
                    AirlineCode = "KE"
                },

                new Airline()
                {
                    Name = "China Southern",
                    AirlineCode = "CZ"
                },

                new Airline()
                {
                    Name = "AirAsia",
                    AirlineCode = "AK"
                },

                new Airline()
                {
                    Name = "Cathay Pacific Airways",
                    AirlineCode = "CX"
                },

                new Airline()
                {
                    Name = "Emirates",
                    AirlineCode = "EK"
                }
            });

            #endregion

            #region Photos

            this.Photos.AddRange(new[]
            {
                new Photo()
                {
                    Id = 1,
                    Name = "My Photo 1",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 11,
                    Name = "Trip Photo 11",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 12,
                    Name = "Trip Photo 12",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 13,
                    Name = "Trip Photo 13",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 14,
                    Name = "Trip Photo 14",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 2,
                    Name = "My Photo 2",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 21,
                    Name = "Trip Photo 21",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 22,
                    Name = "Trip Photo 22",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 23,
                    Name = "Trip Photo 23",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 24,
                    Name = "Trip Photo 24",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 3,
                    Name = "My Photo 3",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 31,
                    Name = "Trip Photo 31",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 32,
                    Name = "Trip Photo 32",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 33,
                    Name = "Trip Photo 33",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
                new Photo()
                {
                    Id = 34,
                    Name = "Trip Photo 34",
                    ContentType = "image/jpeg",
                    Stream = CreateDefaultStream(),
                },
            });

            #endregion

            #region Sample Trip

            Event event1 = new Event()
            {
                PlanItemId = 12,
                Description = "Client Meeting",
                ConfirmationCode = "4372899DD",
                StartsAt = new DateTimeOffset(new DateTime(2014, 1, 2, 13, 0, 0)),
                EndsAt = new DateTimeOffset(new DateTime(2014, 1, 2, 16, 0, 0)),
                Duration = new TimeSpan(3, 0, 0),
                OccursAt = new EventLocation()
                {
                    BuildingInfo = "Regus Business Center",
                    City = new City()
                    {
                        Name = "New York City",
                        CountryRegion = "United States",
                        Region = "New York"
                    },
                    Address = "100 Church Street, 8th Floor, Manhattan, 10007"
                }
            };

            Event event2 = new Event()
            {
                PlanItemId = 14,
                Description = "Visit the Brooklyn Bridge Park",
                ConfirmationCode = "",
                StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1, 15, 0, 0)),
                EndsAt = new DateTimeOffset(new DateTime(2014, 1, 1, 16, 0, 0)),
                Duration = new TimeSpan(1, 0, 0),
                OccursAt = new EventLocation()
                {
                    BuildingInfo = "Brooklyn Bridge Park, at Fulton Ferry Landing",
                    City = new City()
                    {
                        Name = "New York City",
                        CountryRegion = "United States",
                        Region = "New York"
                    },
                    Address = "Main St Dumbo Brooklyn 11201"
                }
            };
            event2.OpenProperties.Add("Transport Information", "Subway: A, C to High St; F to York St");
            event2.OpenProperties.Add("Phone Number", "718-802-0603");
            event2.OpenProperties.Add("WebSite", "brooklynbridgepark.org");

            Event event3 = new Event()
            {
                PlanItemId = 15,
                Description = "Empire State Building",
                ConfirmationCode = "",
                StartsAt = new DateTimeOffset(new DateTime(2014, 1, 3, 10, 0, 0)),
                EndsAt = new DateTimeOffset(new DateTime(2014, 1, 3, 12, 0, 0)),
                Duration = new TimeSpan(2, 0, 0),
                OccursAt = new EventLocation()
                {
                    BuildingInfo = "Empire State Building",
                    City = new City()
                    {
                        Name = "New York City",
                        CountryRegion = "United States",
                        Region = "New York"
                    },
                    Address = "Empire State Building, 350 5th Ave"
                }
            };
            event3.OpenProperties.Add("Ticket Information", "There are several kinds of tickets, the cheapest one for adult is 29 dollars.");

            Event event4 = new Event()
            {
                PlanItemId = 16,
                Description = "Coney Island",
                ConfirmationCode = "",
                StartsAt = new DateTimeOffset(new DateTime(2014, 1, 3, 14, 0, 0)),
                EndsAt = new DateTimeOffset(new DateTime(2014, 1, 3, 20, 0, 0)),
                Duration = new TimeSpan(6, 0, 0),
                OccursAt = new EventLocation()
                {
                    BuildingInfo = "",
                    City = new City()
                    {
                        Name = "New York City",
                        CountryRegion = "United States",
                        Region = "New York"
                    },
                    Address = "1208 Surf Ave, Brooklyn"
                }
            };
            event4.OpenProperties.Add("Phone Number", "+1 718-372-5159");
            event4.OpenProperties.Add("Transport Information", "Train D, F, N and Q");

            Event event5 = new Event()
            {
                PlanItemId = 17,
                Description = "Shopping at Times Square",
                ConfirmationCode = "",
                StartsAt = new DateTimeOffset(new DateTime(2014, 1, 4, 10, 0, 0)),
                EndsAt = new DateTimeOffset(new DateTime(2014, 1, 4, 15, 0, 0)),
                Duration = new TimeSpan(5, 0, 0),
                OccursAt = new EventLocation()
                {
                    BuildingInfo = "",
                    City = new City()
                    {
                        Name = "New York City",
                        CountryRegion = "United States",
                        Region = "New York"
                    },
                    Address = "Broadway, 7th Avenue, 42nd and 47th Streets"
                }
            };

            Trip sampleTrip = new Trip()
            {
                TripId = 1001,
                ShareId = new Guid("9d9b2fa0-efbf-490e-a5e3-bac8f7d47354"),
                Name = "Trip in US",
                Budget = 3000.0f,
                Description = "Trip from San Francisco to New York City. Nice trip with two friends. It is a 4 days' trip. We actually had a client meeting, but we also took one to go sightseeings in New York.",
                Tags = new Collection<string>
                {
                    "Trip in New York",
                    "business",
                    "sightseeing"
                },
                StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1)),
                EndsAt = new DateTimeOffset(new DateTime(2014, 1, 4)),
                Photos = new EntityCollection<Photo>() { Photos[6], Photos[7], },
                PlanItems = new EntityCollection<PlanItem>()
                {
                    new Flight()
                    {
                        PlanItemId = 11,
                        ConfirmationCode = "JH58493",
                        FlightNumber = "AA26",
                        StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1, 6, 15, 0)),
                        EndsAt = new DateTimeOffset(new DateTime(2014, 1, 1, 11, 35, 0)),
                        Airline = Airlines[0],
                        From = Airports[12],
                        To = Airports[4]
                    },
                    new Flight()
                    {
                        PlanItemId = 13,
                        ConfirmationCode = "JH38143",
                        FlightNumber = "AA4035",
                        StartsAt = new DateTimeOffset(new DateTime(2014, 1, 4, 17, 55, 0)),
                        EndsAt = new DateTimeOffset(new DateTime(2014, 1, 4, 20, 45, 0)),
                        Airline = Airlines[0],
                        From = Airports[4],
                        To = Airports[12]
                    },
                }
            };
            sampleTrip.PlanItems.Add(event1);
            sampleTrip.PlanItems.Add(event2);
            sampleTrip.PlanItems.Add(event3);
            sampleTrip.PlanItems.Add(event4);
            sampleTrip.PlanItems.Add(event5);

            Trip sampleTrip1 = sampleTrip.Clone() as Trip;
            Trip sampleTrip2 = sampleTrip.Clone() as Trip;

            #endregion

            #region Me

            this.Me = new Person()
            {
                FirstName = "April",
                LastName = "Cline",
                UserName = "aprilcline",
                Gender = PersonGender.Female,
                Emails = new Collection<string> { "April@example.com", "April@contoso.com" },
                Photo = Photos[0],
                AddressInfo = new Collection<Location>
                {
                    new Location()
                    {
                        Address = "P.O. Box 555",
                        City = new City()
                        {
                            CountryRegion = "United States",
                            Name = "Lander",
                            Region = "WY"
                        }
                    }
                },
                Trips = 
                {
                    sampleTrip,

                    new Trip()
                    {
                        TripId = 2,
                        Name = "Trip in Beijing",
                        Budget = 3000.0f,
                        ShareId = new Guid("f94e9116-8bdd-4dac-ab61-08438d0d9a71"),
                        Description = "Trip from Shanghai to Beijing",
                        Tags = new Collection<string>{"Travel", "Beijing"},
                        StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1)),
                        EndsAt = new DateTimeOffset(new DateTime(2014, 2, 4)),
                        Photos = new EntityCollection<Photo>() { Photos[3], Photos[4] },
                        PlanItems = new EntityCollection<PlanItem>()
                        {
                            new Flight()
                            {
                                PlanItemId = 21,
                                ConfirmationCode = "JH58494",
                                FlightNumber = "FM1930",
                                StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 8, 0, 0)),
                                EndsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 9, 20, 0)),
                                Airline = Airlines[1],
                                SeatNumber = "B11",
                                From = Airports[2],
                                To = Airports[3]
                            },
                            new Flight()
                            {
                                PlanItemId = 32,
                                ConfirmationCode = "JH58495",
                                FlightNumber = "MU1930",
                                StartsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 15, 00, 0)),
                                EndsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 16, 30, 0)),
                                Airline = Airlines[2],
                                SeatNumber = "A32",
                                From = Airports[3],
                                To = Airports[2]
                            },
                            new Event()
                            {
                                PlanItemId = 5,
                                Description = "Dinner",
                                StartsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 18, 0, 0)),
                                EndsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 21, 0, 0)),
                                Duration = new TimeSpan(3, 0, 0),
                                OccursAt = new EventLocation()
                                {
                                    Address = "10 Beijing Street, 100000",
                                    City = new City(){
                                        Name = "Beijing",
                                        CountryRegion = "China",
                                        Region = "Beijing"
                                    },
                                    BuildingInfo = "Beijing Restaurant"
                                }
                            }
                        }
                    },
                    new Trip()
                    {
                        TripId = 3,
                        ShareId = new Guid("9ce142c3-5fd6-4a71-848e-5220ebf1e9f3"),
                        Name = "Honeymoon",
                        Budget = 800.0f,
                        Description = "Happy honeymoon trip",
                        Tags = new Collection<string>{"Travel", "honeymoon"},
                        StartsAt = new DateTime(2014, 2, 1),
                        EndsAt = new DateTime(2014, 2, 4)
                    },
                    new Trip()
                    {
                        TripId = 4,
                        ShareId = new Guid("4CCFB043-C79C-44EF-8CFE-CD493CED6654"),
                        Name = "Business trip to OData",
                        Budget = 324.6f,
                        Description = "Business trip to OData",
                        Tags = new Collection<string>{"business", "odata"},
                        StartsAt = new DateTime(2013, 1, 1),
                        EndsAt = new DateTime(2013, 1, 4)
                    },
                    new Trip()
                    {
                        TripId = 5,
                        ShareId = new Guid("4546F419-0070-45F7-BA2C-19E4BC3647E1"),
                        Name = "Travel trip in US",
                        Budget = 1250.0f,
                        Description = "Travel trip in US",
                        Tags = new Collection<string>{"travel", "overseas"},
                        StartsAt = new DateTime(2013, 1, 19),
                        EndsAt = new DateTime(2013, 1, 28)
                    },
                    new Trip()
                    {
                        TripId = 6,
                        ShareId = new Guid("26F0E8F6-657A-4561-BF3B-719366EF04FA"),
                        Name = "Study music in Europe",
                        Budget = 3200.0f,
                        Description = "Study music in Europe",
                        Tags = new Collection<string>{"study", "overseas"},
                        StartsAt = new DateTime(2013, 3, 1),
                        EndsAt = new DateTime(2013, 5, 4)
                    },
                    new Trip()
                    {
                        TripId = 7,
                        ShareId = new Guid("2E77BF06-A354-454B-8BCA-5F004C1AFB59"),
                        Name = "Conference talk about OData",
                        Budget = 2120.55f,
                        Description = "Conference talk about ODatan",
                        Tags = new Collection<string>{"odata", "overseas"},
                        StartsAt = new DateTime(2013, 7, 2),
                        EndsAt = new DateTime(2013, 7, 5)
                    },
                    new Trip()
                    {
                        TripId = 8,
                        ShareId = new Guid("E6E23FB2-C428-439E-BDAB-9283482F49F0"),
                        Name = "Vocation at hometown",
                        Budget = 1500.0f,
                        Description = "Vocation at hometown",
                        Tags = new Collection<string>{"voaction"},
                        StartsAt = new DateTime(2013, 10, 1),
                        EndsAt = new DateTime(2013, 10, 5)
                    },
                    new Trip()
                    {
                        TripId = 9,
                        ShareId = new Guid("FAE31279-35CE-4119-9BDC-53F6E19DD1C5"),
                        Name = "Business trip for tech training",
                        Budget = 100.0f,
                        Description = "Business trip for tech training",
                        Tags = new Collection<string>{"business"},
                        StartsAt = new DateTime(2013, 9, 1),
                        EndsAt = new DateTime(2013, 9, 4)
                    }
                }
            };

            #endregion


            #region People

            this.People.AddRange(new List<Person>()
            {
                #region russellwhyte

                new Person()
                {
                    FirstName = "Russell",
                    LastName = "Whyte",
                    UserName = "russellwhyte",
                    Gender = PersonGender.Male,
                    Emails = new Collection<string> { "Russell@example.com", "Russell@contoso.com" },
                    Photo = Photos[5],
                    AddressInfo = new Collection<Location>
                    {
                      new Location()
                      {
                          Address = "187 Suffolk Ln.",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "Boise",
                              Region = "ID"
                          }
                      },
                    },
                    Trips = 
                    {                        
                        sampleTrip1,

                        new Trip()
                        {
                            TripId = 001003,
                            Name = "Trip in Beijing",
                            Budget = 2000.0f,
                            ShareId = new Guid("f94e9116-8bdd-4dac-ab61-08438d0d9a71"),
                            Description = "Trip from Shanghai to Beijing",
                            Tags = new Collection<string>{"Travel", "Beijing"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 2, 4)),
                            Photos = new EntityCollection<Photo>() { Photos[8], Photos[9], },
                            PlanItems = new EntityCollection<PlanItem>()
                            {
                                new Flight()
                                {
                                    PlanItemId = 21,
                                    ConfirmationCode = "JH58494",
                                    FlightNumber = "FM1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 8, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 9, 20, 0)),
                                    Airline = Airlines[1],
                                    SeatNumber = "B11",
                                    From = Airports[2],
                                    To = Airports[3]
                                },
                                new Flight()
                                {
                                    PlanItemId = 32,
                                    ConfirmationCode = "JH58495",
                                    FlightNumber = "MU1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 15, 30, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 16, 30, 0)),
                                    Airline = Airlines[2],
                                    SeatNumber = "A32",
                                    From = Airports[3],
                                    To = Airports[2]
                                },
                                new Event()
                                {
                                    PlanItemId = 5,
                                    Description = "Dinner",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 18, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 21, 0, 0)),
                                    Duration = new TimeSpan(3, 0, 0),
                                    OccursAt = new EventLocation()
                                    {
                                        BuildingInfo = "Beijing Restaurant",
                                        City = new City()
                                        {
                                            Name = "Beijing",
                                            CountryRegion = "China",
                                            Region = "Beijing"
                                        },
                                        Address = "10 Beijing Street, 100000"
                                    }
                                }
                            }
                        },
                        new Trip()
                        {
                            TripId = 001007,
                            ShareId = new Guid("9ce142c3-5fd6-4a71-848e-5220ebf1e9f3"),
                            Name = "Honeymoon",
                            Budget = 2650.0f,
                            Description = "Happy honeymoon trip",
                            Tags = new Collection<string>{"Travel", "honeymoon"},
                            StartsAt = new DateTime(2014, 2, 1),
                            EndsAt = new DateTime(2014, 2, 4)
                        }
                    }
                },            

                #endregion

                #region scottketchum

                new Person()
                {
                    FirstName = "Scott",
                    LastName = "Ketchum",
                    UserName = "scottketchum",
                    Gender = PersonGender.Male,
                    Emails = new Collection<string> { "Scott@example.com" },
                    Photo = Photos[10],
                    AddressInfo = new Collection<Location>
                    {
                      new Location()
                      {
                          Address = "2817 Milton Dr.",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "Albuquerque",
                              Region = "NM"
                          }
                      }
                    },
                    Trips = 
                    {
                        sampleTrip2,

                        new Trip()
                        {
                            TripId = 002004,
                            ShareId = new Guid("f94e9116-8bdd-4dac-ab61-08438d0d9a71"),
                            Name = "Trip in Beijing",
                            Budget = 11000.0f,
                            Description = "Trip from Shanghai to Beijing",
                            Tags = new Collection<string>{"Travel", "Beijing"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 2, 4)),
                            Photos = new EntityCollection<Photo>() { Photos[13], Photos[14], },
                            PlanItems = new EntityCollection<PlanItem>()
                            {
                                new Flight()
                                {
                                    PlanItemId = 21,
                                    ConfirmationCode = "JH58494",
                                    FlightNumber = "FM1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 8, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 9, 20, 0)),
                                    Airline = Airlines[1],
                                    SeatNumber = "B12",
                                    From = Airports[2],
                                    To = Airports[3]
                                },
                                new Flight()
                                {
                                    PlanItemId = 32,
                                    ConfirmationCode = "JH58495",
                                    FlightNumber = "MU1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 16, 30, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 16, 30, 0)),
                                    Airline = Airlines[2],
                                    SeatNumber = "A33",
                                    From = Airports[3],
                                    To = Airports[2]
                                },
                                new Event()
                                {
                                    PlanItemId = 5,
                                    Description = "Dinner",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 18, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 21, 0, 0)),
                                    Duration = new TimeSpan(3, 0, 0),
                                    OccursAt = new EventLocation()
                                    {
                                        BuildingInfo = "Beijing Restaurant",
                                        City = new City()
                                        {
                                            Name = "Beijing",
                                            CountryRegion = "China",
                                            Region = "Beijing"
                                        },
                                        Address = "10 Beijing Street, 100000"
                                    }
                                }
                            }
                        }
                    }
                },            

                #endregion

                new Person()
                {
                    FirstName = "Ronald",
                    LastName = "Mundy",
                    UserName = "ronaldmundy",
                    Gender = PersonGender.Male,
                    Emails = new Collection<string> { "Ronald@example.com", "Ronald@contoso.com" },
                    Trips = 
                    {
                        new Trip()
                        {
                            TripId = 003009,
                            ShareId = new Guid("dd6a09c0-e59b-4745-8612-f4499b676c47"),
                            Name = "Gradutaion trip",
                            Budget = 6000.0f,
                            Description = "Gradution trip with friends",
                            Tags = new Collection<string>{"Travel"},
                            StartsAt = new DateTimeOffset(new DateTime(2013, 5, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2013, 5, 8))
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Javier",
                    LastName = "Alfred",
                    UserName = "javieralfred",
                    Gender = PersonGender.Male,
                    Emails = new Collection<string> { "Javier@example.com", "Javier@contoso.com" },
                    AddressInfo = new Collection<Location>
                    {
                      new Location()
                      {
                          Address = "89 Jefferson Way Suite 2",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "Portland",
                              Region = "WA"
                          }
                      }
                    },
                    Trips = 
                    {
                        new Trip()
                        {
                            TripId = 004005,
                            ShareId = new Guid("f94e9116-8bdd-4dac-ab61-08438d0d9a71"),
                            Name = "Trip in Beijing",
                            Budget = 800.0f,
                            Description = "Trip from Shanghai to Beijing",
                            Tags = new Collection<string>{"Travel", "Beijing"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 2, 4))
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Willie",
                    LastName = "Ashmore",
                    UserName = "willieashmore",
                    Gender = PersonGender.Male, 
                    Emails = new Collection<string> { "Willie@example.com", "Willie@contoso.com" },
                    Trips = 
                    {
                        new Trip()
                        {
                            TripId = 005007,
                            ShareId = new Guid("5ae142c3-5ad6-4a71-768e-5220ebf1e9f3"),
                            Name = "Business Trip",
                            Budget = 3800.5f,
                            Description = "This is my first business trip",
                            Tags = new Collection<string>{"business", "first"},
                            StartsAt = new DateTime(2014, 2, 1),
                            EndsAt = new DateTime(2014, 2, 4)
                        },
                        new Trip()
                        {
                            TripId = 005008,
                            ShareId = new Guid("9ce32ac3-5fd6-4a72-848e-2250ebf1e9f3"),
                            Name = "Trip in Europe",
                            Budget = 2000.0f,
                            Description = "The trip is currently in plan.",
                            Tags = new Collection<string>{"Travel", "plan"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 2, 4))
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Vincent",
                    LastName = "Calabrese",
                    UserName = "vincentcalabrese", 
                    Gender = PersonGender.Male, 
                    Emails = new Collection<string> { "Vincent@example.com", "Vincent@contoso.com" },
                    AddressInfo = new Collection<Location>
                    {
                      new Location()
                      {
                          Address = "55 Grizzly Peak Rd.",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "Butte",
                              Region = "MT"
                          }
                      }
                    },
                    Trips = 
                    {
                        new Trip()
                        {
                            TripId = 007010,
                            ShareId = new Guid("dd6a09c0-e59b-4745-8612-f4499b676c47"),
                            Name = "Gradutaion trip",
                            Budget = 1000.0f,
                            Description = "Gradution trip with friends",
                            Tags = new Collection<string>{"Travel"},
                            StartsAt = new DateTimeOffset(new DateTime(2013, 5, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2013, 5, 8))
                        }
                    }
                },                
                new Person()
                {
                    FirstName = "Clyde",
                    LastName = "Guess", 
                    UserName = "clydeguess",
                    Gender = PersonGender.Male,
                    Emails = new Collection<string> { "Clyde@example.com" },
                    Trips = 
                    {
                        new Trip()
                        {
                            TripId = 008011,
                            ShareId = new Guid("a88f675d-9199-4392-9656-b08e3b46df8a"),
                            Name = "Study trip",
                            Budget = 1550.3f,
                            Description = "This is a 2 weeks study trip",
                            Tags = new Collection<string>{"study"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 1, 14))
                        }
                    }
                },                
                new Person()
                {
                    FirstName = "Keith",
                    LastName = "Pinckney", 
                    UserName = "keithpinckney",
                    Gender = PersonGender.Male, 
                    Emails = new Collection<string> { "Keith@example.com", "Keith@contoso.com" },
                },
                new Person()
                {
                    FirstName = "Marshall", 
                    LastName = "Garay",
                    UserName = "marshallgaray", 
                    Gender = PersonGender.Male, 
                    Emails = new Collection<string> { "Marshall@example.com", "Marshall@contoso.com" },
                },
                new Person()
                {
                    FirstName = "Ryan", 
                    LastName = "Theriault", 
                    UserName = "ryantheriault", 
                    Gender = PersonGender.Male,
                    Emails = new Collection<string> { "Ryan@example.com", "Ryan@contoso.com" },
                },
                new Person()
                {
                    FirstName = "Elaine",
                    LastName = "Stewart", 
                    UserName = "elainestewart",
                    Gender = PersonGender.Female,
                    Emails = new Collection<string> { "Elaine@example.com", "Elaine@contoso.com" }
                },                
                new Person()
                {
                    FirstName = "Sallie", 
                    LastName = "Sampson",
                    UserName = "salliesampson",
                    Gender = PersonGender.Female,
                    Emails = new Collection<string> { "Sallie@example.com", "Sallie@contoso.com" },
                    AddressInfo = new Collection<Location>
                    {
                      new Location()
                      {
                          Address = "87 Polk St. Suite 5",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "San Francisco",
                              Region = "CA"
                          }
                      },
                      new Location()
                      {
                          Address = "89 Chiaroscuro Rd.",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "Portland",
                              Region = "OR"
                          }
                      }
                    },
                    Trips = 
                    {
                        new Trip()
                        {
                            TripId = 013012,
                            ShareId = new Guid("a88f675d-9199-4392-9656-b08e3b46df8a"),
                            Name = "Study trip",
                            Budget = 600.0f,
                            Description = "This is a 2 weeks study trip",
                            Tags = new Collection<string>{"study"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 1, 14))
                        }
                    }
                },                
                new Person()
                {
                    FirstName = "Joni",
                    LastName = "Rosales",
                    UserName = "jonirosales", 
                    Gender = PersonGender.Female, 
                    Emails = new Collection<string> { "Joni@example.com", "Joni@contoso.com" },
                    Trips = 
                    {
                        new Trip()
                        {
                            TripId = 014013,
                            ShareId = new Guid("a88f675d-9199-4392-9656-b08e3b46df8a"),
                            Name = "Study trip",
                            Budget = 2000.0f,
                            Description = "This is a 2 weeks study trip",
                            Tags = new Collection<string>{"study"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 1, 14))
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Georgina",
                    LastName = "Barlow",
                    UserName = "georginabarlow",
                    Gender = PersonGender.Female,
                    Emails = new Collection<string> { "Georgina@example.com", "Georgina@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Angel", 
                    LastName = "Huffman", 
                    UserName = "angelhuffman", Gender = PersonGender.Female,
                    Emails = new Collection<string> { "Angel@example.com" },
                    Trips = 
                    {
                        new Trip()
                        {
                            TripId = 016014,
                            ShareId = new Guid("cb0b8acb-79cb-4127-8316-772bc4302824"),
                            Name = "DIY Trip",
                            Budget = 1500.3f,
                            Description = "This is a DIY trip",
                            Tags = new Collection<string>{"Travel", "DIY"},
                            StartsAt = new DateTimeOffset(new DateTime(2011, 2, 11)),
                            EndsAt = new DateTimeOffset(new DateTime(2011, 2, 14))
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Laurel", 
                    LastName = "Osborn",
                    UserName = "laurelosborn", 
                    Gender = PersonGender.Female, 
                    Emails = new Collection<string> { "Laurel@example.com", "Laurel@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Sandy",
                    LastName = "Osborn",
                    UserName = "sandyosborn",
                    Gender = PersonGender.Female, 
                    Emails = new Collection<string> { "Sandy@example.com", "Sandy@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Ursula",
                    LastName = "Bright",
                    UserName = "ursulabright",
                    Gender = PersonGender.Female,
                    Emails = new Collection<string> { "Ursula@example.com", "Ursula@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Genevieve",
                    LastName = "Reeves",
                    UserName = "genevievereeves", 
                    Gender = PersonGender.Female,
                    Emails = new Collection<string> { "Genevieve@example.com", "Genevieve@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Krista", 
                    LastName = "Kemp",
                    UserName = "kristakemp",
                    Gender = PersonGender.Female,
                    Emails = new Collection<string> { "Krista@example.com" }
                }
            });

            #endregion

            People[0].Friends.AddRange(new[] { People[1], People[2], People[3], People[14] });
            People[1].Friends.AddRange(new[] { People[0], People[2] });
            People[2].Friends.AddRange(new[] { People[0], People[1] });
            People[3].Friends.AddRange(new[] { People[4], People[5], People[13] });
            People[4].Friends.AddRange(new[] { People[3], People[5] });
            People[5].Friends.AddRange(new[] { People[3], People[4] });
            People[6].Friends.AddRange(new[] { People[7], People[17] });
            People[7].Friends.AddRange(new[] { People[6], People[8] });
            People[8].Friends.AddRange(new[] { People[7], People[10], People[12] });
            People[10].Friends.AddRange(new[] { People[8] });
            People[12].Friends.AddRange(new[] { People[8] });
            People[13].Friends.AddRange(new[] { People[3] });
            People[14].Friends.AddRange(new[] { People[0] });
            People[15].Friends.AddRange(new[] { People[16] });
            People[16].Friends.AddRange(new[] { People[15] });
            People[17].Friends.AddRange(new[] { People[7] });
            People[18].Friends.AddRange(new[] { People[19] });
            People[19].Friends.AddRange(new[] { People[18] });

            Me.Friends.AddRange(People);
        }

        protected override IEdmModel CreateModel()
        {
            return TripPinInMemoryModel.CreateTripPinServiceModel("Microsoft.OData.SampleService.Models.TripPin");
        }

        private static Stream CreateDefaultStream()
        {
            var result = new NonClosingStream();

            try
            {
                if (HostingEnvironment.IsHosted)
                {
                    try
                    {
                        // On IIS based deployment, e.g. Azure, we read image from virtual directory
                        CreateDefaultStreamFromVirtualDirectory(result);
                    }
                    catch (Exception)
                    {
                        // to prevent the failure of web service, we read image from assembly resource
                        CreateDefaultStreamFromAssemblyResource(result);
                    }
                }
                else
                {
                    // the code here is for unit test
                    // when unit test, we are unable to get file from a directory
                    CreateDefaultStreamFromAssemblyResource(result);
                }

                result.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                throw new ODataServiceException(HttpStatusCode.InternalServerError, "The data source initialization of TripPin service failed.", ex);
            }

            return result;
        }

        private static void CreateDefaultStreamFromVirtualDirectory(NonClosingStream nonClosingStream)
        {
            var imagePath = HostingEnvironment.MapPath("~/bin/ODataLogo.jpg");
            using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                stream.CopyTo(nonClosingStream);
            }
        }

        private static void CreateDefaultStreamFromAssemblyResource(NonClosingStream nonClosingStream)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ODataSamples.Services.TripPin.ODataLogo.jpg"))
            {
                stream.CopyTo(nonClosingStream);
            }
        }
    }
}