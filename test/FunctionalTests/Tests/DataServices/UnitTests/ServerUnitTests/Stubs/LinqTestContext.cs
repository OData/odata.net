//---------------------------------------------------------------------
// <copyright file="LinqTestContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.OData.Client;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    #endregion Namespaces

    public class BaseContext
    {
        protected static List<PropertyInfo> includes = new List<PropertyInfo>();

        public static void AddBaselineIncludes(Type t, string propertyName)
        {
            PropertyInfo pi = t.GetProperty(propertyName);
            Debug.Assert(pi != null);
            includes.Add(pi);
        }

        public static void ClearBaselineIncludes()
        {
            includes.Clear();
        }

        public static S CheckToSeeIfIncluded<T, S>(T instance, string propertyName, S v) where S : class
        {
            PropertyInfo pi = typeof(T).GetProperty(propertyName);
            if (pi == null || !includes.Contains(pi))
            {
                return null;
            }
            else
            {
                return v;
            }
        }
    }

    public class ReadOnlyTestContext : BaseContext
    {
        private Dictionary<string, League> leagues;
        private Dictionary<string, Team> teams;
        private Dictionary<string, Stadium> stadiums;
        private Dictionary<string, StadiumName> stadiumNames;
        private Dictionary<string, Employee> employees;
        private List<YetAnotherAllPrimitiveTypesType> var1;
        private List<MultipleKeyType> moreVar1;
        private List<Parent> parents;
        private List<Table> tables;

        private List<BigState> bigStates;
        private List<BigCity> bigCities;
        private List<BigCityVar1> bigCityVar1;
        private List<Street> streets;

        // note - this constructor creates the server side context
        // it should not be called 
        public ReadOnlyTestContext()
        {
            // create test data
            leagues = new Dictionary<string, League>();
            teams = new Dictionary<string, Team>();
            stadiums = new Dictionary<string, Stadium>();
            stadiumNames = new Dictionary<string, StadiumName>();
            employees = new Dictionary<string, Employee>();

            var1 = new List<YetAnotherAllPrimitiveTypesType>();
            moreVar1 = new List<MultipleKeyType>();
            tables = new List<Table>();

            bigStates = new List<BigState>();

            ReadOnlyTestContext.ClearBaselineIncludes();

            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(IQueryable<>))
                {
                    foreach (PropertyInfo typePI in pi.PropertyType.GetGenericArguments()[0].GetProperties())
                    {
                        Type pt = typePI.PropertyType;
                        if (pt.IsGenericType)
                        {
                            pt = pt.GetGenericArguments()[0];
                        }

                        if (pt.GetCustomAttributes(typeof(KeyAttribute), true).Count() > 0 ||
                            pt.GetProperty("ID") != null ||
                            pt.GetProperty(pt.Name + "ID") != null)
                        {
                            // is entity
                            includes.Add(typePI);
                        }
                    }
                }
            }

            leagues.Add("League1", new League
            {
                ID = 1,
                Name = "League1",
                ConceptionDate = new DateTime(1901, 3, 1),
                Teams = new List<Team>()
            });

            leagues.Add("League2", new League
            {
                ID = 2,
                Name = "League2",
                ConceptionDate = new DateTime(1891, 8, 28),
                Teams = new List<Team>()
            });

            int teamID = 0;

            teams.Add("Expos", new Team { TeamID = ++teamID, City = "Montreal", Players = new List<Player>(), HomeStadium = null, TeamName = "Expos" });
            leagues["League1"].Teams.Add(teams["Expos"]);

            teams.Add("Brewers", new Team { TeamID = ++teamID, City = "Milwaukee", Players = new List<Player>(), HomeStadium = null, TeamName = "Brewers" });
            leagues["League1"].Teams.Add(teams["Brewers"]);

            teams.Add("Mets", new Team { TeamID = ++teamID, City = "New York", Players = new List<Player>(), HomeStadium = null, TeamName = "Mets" });
            leagues["League1"].Teams.Add(teams["Mets"]);

            teams.Add("Astros", new Team { TeamID = ++teamID, City = "Houston", Players = new List<Player>(), HomeStadium = null, TeamName = "Astros" });
            leagues["League1"].Teams.Add(teams["Astros"]);

            teams.Add("Mariners", new Team { TeamID = ++teamID, City = "Seattle", Players = new List<Player>(), HomeStadium = null, TeamName = "Mariners" });
            leagues["League2"].Teams.Add(teams["Mariners"]);

            teams.Add("Yankees", new Team { TeamID = ++teamID, City = "New York", Players = new List<Player>(), HomeStadium = null, TeamName = "Yankees" });
            leagues["League2"].Teams.Add(teams["Yankees"]);

            teams.Add("White Sox", new Team { TeamID = ++teamID, City = "Chicago", Players = new List<Player>(), HomeStadium = null, TeamName = "White Sox" });
            leagues["League2"].Teams.Add(teams["White Sox"]);

            teams.Add("Cubs", new Team { TeamID = ++teamID, City = "Chicago", Players = new List<Player>(), HomeStadium = null, TeamName = "Cubs" });
            leagues["League1"].Teams.Add(teams["Cubs"]);

            teams.Add("Giants", new Team { TeamID = ++teamID, City = "New York", Players = new List<Player>(), HomeStadium = null, TeamName = "Giants" });
            leagues["League1"].Teams.Add(teams["Giants"]);

            int stadiumID = 0;

            stadiums.Add("Minute Maid Park", new Stadium { ID = ++stadiumID, Name = "Minute Maid Park", Capacity = 65000, City = "Houston", Sponsor = new StadiumSponsor { StadiumSponsorID = 99999, Name = "Minute Maid" } });
            teams["Astros"].HomeStadium = stadiums["Minute Maid Park"];
            teams["Astros"].Mascot = new Mascot { ID = 1, Name = "Big Orange" };

            stadiums.Add("Center Stadium", new Stadium { ID = ++stadiumID, Name = "Center Stadium", Capacity = 77000, City = "Montreal", Sponsor = new StadiumSponsor { StadiumSponsorID = 100000, Name = "Some one" } });
            teams["Expos"].HomeStadium = stadiums["Center Stadium"];
            teams["Expos"].Mascot = new Mascot { ID = 2, Name = "Kid Expo" };

            stadiums.Add("Brewer Stadium", new Stadium { ID = ++stadiumID, Name = "Brewer Stadium", Capacity = 45500, City = "Milwaukee", Sponsor = new StadiumSponsor { StadiumSponsorID = 100001, Name = "Miller Beer" } });
            teams["Brewers"].HomeStadium = stadiums["Brewer Stadium"];
            teams["Brewers"].Mascot = new Mascot { ID = 3, Name = "Beer Guy" };

            stadiums.Add("Polo Grounds", new Stadium { ID = ++stadiumID, Name = "Polo Grounds", Capacity = 29900, City = "New York", Sponsor = new StadiumSponsor { StadiumSponsorID = 100002, Name = "King Kong" } });
            teams["Giants"].HomeStadium = stadiums["Polo Grounds"];
            teams["Giants"].Mascot = new Mascot { ID = 4, Name = "Crab" };

            stadiums.Add("Safeco Stadium", new Stadium { ID = ++stadiumID, Name = "Safeco Stadium", Capacity = 67000, City = "Seattle", Sponsor = new StadiumSponsor { StadiumSponsorID = 100003, Name = "Safeco" } });
            teams["Mariners"].HomeStadium = stadiums["Safeco Stadium"];
            teams["Mariners"].Mascot = new Mascot { ID = 5, Name = "Moose" };

            int nameID = 2000;
            List<StadiumName> names = new List<StadiumName>();
            StadiumName sn = new StadiumName { StadiumNameID = ++nameID, Name = "Enron Field", StartDate = new DateTime(2000, 1, 1), EndDate = new DateTime(2002, 1, 1) };
            this.stadiumNames.Add(sn.Name, sn);
            names.Add(sn);
            sn = new StadiumName { StadiumNameID = ++nameID, Name = "Astros Field", StartDate = new DateTime(2002, 2, 1), EndDate = new DateTime(2002, 6, 1) };
            this.stadiumNames.Add(sn.Name, sn);
            names.Add(sn);
            stadiums["Minute Maid Park"].FormerNames = names;

            names = new List<StadiumName>();
            sn = new StadiumName { StadiumNameID = ++nameID, Name = "Brush Stadium", StartDate = new DateTime(1911, 1, 1), EndDate = new DateTime(1919, 1, 1) };
            this.stadiumNames.Add(sn.Name, sn);
            names.Add(sn);
            stadiums["Polo Grounds"].FormerNames = names;

            int playerID = 1000;
            teams["Expos"].Players.Add(new Player { ID = ++playerID, FirstName = "Jeff", Lastname = "Jones", Team = null });
            teams["Expos"].Players.Add(new Player { ID = ++playerID, FirstName = "Tom", Lastname = "Jones", Team = null });

            int employeeID = 7500;

            employees.Add("VP", new Employee { EmployeeID = ++employeeID, Name = "VP" });
            employees.Add("Admin", new Employee { EmployeeID = ++employeeID, Name = "Admin" });
            employees.Add("Big Boss", new Employee { EmployeeID = ++employeeID, Name = "Big Boss" });
            employees.Add("Middle Manager", new Employee { EmployeeID = ++employeeID, Name = "Middle Manager" });
            employees.Add("Intern", new Employee { EmployeeID = ++employeeID, Name = "Intern" });
            employees.Add("Dev", new Employee { EmployeeID = ++employeeID, Name = "Dev" });
            employees.Add("Tester", new Employee { EmployeeID = ++employeeID, Name = "Tester" });

            employees["VP"].AddReport(employees["Big Boss"]);
            employees["Big Boss"].AddReport(employees["Middle Manager"]);
            employees["Big Boss"].AddReport(employees["Admin"]);
            employees["Middle Manager"].AddReport(employees["Intern"]);
            employees["Middle Manager"].AddReport(employees["Dev"]);
            employees["Middle Manager"].AddReport(employees["Tester"]);
            employees["Tester"].AddReport(employees["VP"]);

            int valuesID = 0;
            var1.Add(new YetAnotherAllPrimitiveTypesType
            {
                ID = ++valuesID,
                Boolean = true,
                Byte = 0xA,
                Short = 5,
                Int = 89898,
                Long = -67677676,
                String = "foo",
                Float = 5.8f,
                Decimal = 9.95M,
                Double = 6.98989d,
                DateTimeOffset = new DateTime(1970, 9, 1),
                Guid = new Guid(5, 4, 3, new byte[] { 5, 6, 2, 1, 7, 3, 3, 3 }),
                NullableBoolean = null,
                NullableByte = null,
                NullableDateTimeOffset = null,
                NullableDecimal = null,
                NullableDouble = null,
                NullableFloat = null,
                NullableGuid = null,
                NullableInt = null,
                NullableLong = null,
                NullableShort = null
            });

            var1.Add(new YetAnotherAllPrimitiveTypesType
            {
                ID = ++valuesID,
                Boolean = false,
                Byte = 81,
                Short = 15,
                Int = 89118,
                Long = -6111176,
                String = "bar",
                Float = 5.8121f,
                Decimal = 19.951M,
                Double = 16.9189189d,
                DateTimeOffset = new DateTime(1971, 9, 11),
                Guid = new Guid(1, 1, 3, new byte[] { 4, 6, 4, 1, 7, 3, 3, 3 }),
                NullableBoolean = false,
                NullableByte = 5,
                NullableDateTimeOffset = new DateTime(1971, 9, 11),
                NullableDecimal = 18.78M,
                NullableDouble = 9.99d,
                NullableFloat = 9.9888f,
                NullableGuid = new Guid(1, 1, 3, new byte[] { 4, 6, 4, 1, 7, 3, 3, 3 }),
                NullableInt = 63,
                NullableLong = 67,
                NullableShort = 122
            });

            var1.Add(new YetAnotherAllPrimitiveTypesType
            {
                ID = ++valuesID,
                Boolean = true,
                Byte = 14,
                Short = 15,
                Int = -1,
                Long = 0,
                String = "",
                Float = 5.8f,
                Decimal = 9.95M,
                Double = 6.98989d,
                DateTimeOffset = new DateTime(1970, 9, 1, 12, 16, 18),
                Guid = new Guid(5, 4, 3, new byte[] { 5, 6, 2, 1, 7, 3, 3, 3 }),
                NullableBoolean = null,
                NullableByte = 5,
                NullableDateTimeOffset = null,
                NullableDecimal = 18.78M,
                NullableDouble = null,
                NullableFloat = 9.9888f,
                NullableGuid = null,
                NullableInt = 63,
                NullableLong = null,
                NullableShort = 122
            });

            moreVar1.Add(new MultipleKeyType { Key1 = 1, Key2 = "23", Key3 = 37, Data1 = "foo", Data2 = "bar" });
            moreVar1.Add(new MultipleKeyType { Key1 = 2, Key2 = "23", Key3 = 37, Data1 = "foo2", Data2 = "bar2" });
            moreVar1.Add(new MultipleKeyType { Key1 = 3, Key2 = "23", Key3 = 37, Data1 = "foo3", Data2 = "bar3" });

            MultipleKeyType2 m = new MultipleKeyType2 { Key = 1, FKKey = 1, Data2 = "Eagle" };
            moreVar1.Where(n => n.Key1 == 1).First().Related.Add(m);

            m = new MultipleKeyType2 { Key = 2, FKKey = 1, Data2 = "Birdie" };
            moreVar1.Where(n => n.Key1 == 1).First().Related.Add(m);

            m = new MultipleKeyType2 { Key = 2, FKKey = 2, Data2 = "Double Eagle" };
            moreVar1.Where(n => n.Key1 == 2).First().Related.Add(m);

            m = new MultipleKeyType2 { Key = 3, FKKey = 2, Data2 = "Bogey" };
            moreVar1.Where(n => n.Key1 == 2).First().Related.Add(m);

            m = new MultipleKeyType2 { Key = 1, FKKey = 3, Data2 = "Triple Bogey" };
            moreVar1.Where(n => n.Key1 == 3).First().Related.Add(m);

            parents = new List<Parent>();
            parents.Add(new Parent { Id = 1, Children = new List<Child>() });
            parents.Add(new Parent { Id = 2, Children = new List<Child>() });

            Child c = new Child { Id = 99, Parents = new List<Parent>() };
            parents[0].Children.Add(c);
            parents[1].Children.Add(c);
            c.Parents.Add(parents[0]);
            c.Parents.Add(parents[1]);
            c.Mom = parents[0];
            c.Dad = parents[1];

            parents.Add(new Parent { Id = 3, Children = new List<Child>() });
            c = new Child { Id = 100, Parents = new List<Parent>() };
            c.Parents.Add(parents[0]);
            c.Parents.Add(parents[2]);
            c.Mom = parents[0];
            c.Dad = parents[2];

            Table customerTable = new Table { Rows = new List<Row>(),
                                              TableName = "Customers",
                                              TableInfo = new TableInfo { TableName = "Customers", Info = "foo" }
                                            };

            customerTable.Rows.AddRange(new Row[] { new Row { Id = 1, Name = "John", Zip = 98074, Details = new List<RowDetail>()},
                                                    new Row { Id = 2, Name = "Ted", Zip = 98052, Details = new List<RowDetail>()},
                                                    new Row { Id = 3, Name = "Fred", Zip = 91118, Details = new List<RowDetail>()}});

            customerTable.Rows[0].Details.AddRange(new RowDetail[] { new RowDetail { Id = 1, Word = "Cameo"},
                                                                       new RowDetail { Id = 2, Word = "Def Lepard" },
                                                                       new RowDetail { Id = 3, Word = "Van Halen"}});
            this.tables.Add(customerTable);

            Table emptyTable = new Table
                               {
                                    Rows = new List<Row>(),
                                    TableName = "EmptyTable",
                                    TableInfo = new TableInfo { TableName = "EmptyTable", Info = "foo" }
                                };


            this.tables.Add(emptyTable);

            Table customer2Table = new Table
            {
                Rows = new List<Row>(),
                TableName = "Customers2",
                TableInfo = new TableInfo { TableName = "Customers2", Info = "foo" }
            };

            customer2Table.Rows.AddRange(new Row[] { new Row { Id = 1, Name = "John2", Zip = 98074, Details = new List<RowDetail>()},
                                                    new Row { Id = 2, Name = "Ted2", Zip = 98052, Details = new List<RowDetail>()},
                                                    new Row { Id = 3, Name = "Fred2", Zip = 91118, Details = new List<RowDetail>()}});

            this.tables.Add(customer2Table);

            this.bigStates.Add(new BigState { StateName = "Oregon", TopCity = null, Cities = new List<BigCity>(), Population = 500000, ID = 1, CoolestCity = null });
            this.bigStates.Add(new BigState { StateName = "Washington", TopCity = null, Cities = new List<BigCity>(), Population = 2500000, ID = 2, CoolestCity = null });
        
            BigState Oregon = this.bigStates.Where(s => s.ID == 1).First();
            Oregon.Cities.Add(new BigCity
            {
                BigCityID = 1,
                Name = "Salem",
                Var1 = new BigCityVar1() { String = "foo", NullableBoolean = false, NullableInt = 78, NullableLong = 101 },
                Mayor = "JoeBlow",
                MiscInfo = new BigCityComplexType { A = "5", B = "7" },
                Population = 12,
                State = Oregon,
                Streets = new List<Street>()
            });

            Oregon.Cities[0].Streets.Add(new Street { StreetName = "Fir", NumberOfBlocks = 100 });
            Oregon.Cities[0].Streets.Add(new Street { StreetName = "Pine", NumberOfBlocks = 200 });
            Oregon.Cities[0].Streets.Add(new Street { StreetName = "Oak", NumberOfBlocks = 100 });

            Oregon.TopCity = Oregon.Cities[0];
            Oregon.Cities.Add(new BigCity { 
                BigCityID = 2, 
                Name = "Portland", 
                Var1 = new BigCityVar1() { String = "bar", NullableBoolean = true, NullableInt = 10078, NullableLong = -101 }, 
                Mayor = "MaryMary",
                MiscInfo = new BigCityComplexType { A = "bar", B = "foo" }, 
                Population = 120000, 
                State = Oregon,
                Streets = new List<Street>()});

            Oregon.CoolestCity = Oregon.Cities[1];

            BigState Washington = this.bigStates.Where(s => s.ID == 2).First();
            Washington.Cities.Add(new BigCity { 
                BigCityID = 3, 
                Name = "Oly", 
                Var1 = new BigCityVar1() { String = "foofoo", NullableBoolean = false, NullableInt = 78, NullableLong = 101 },
                Mayor = "JimBob", 
                MiscInfo = new BigCityComplexType { A = "bar", B = "foo" }, 
                Population = 99, 
                State = Washington,
                Streets = new List<Street>()});

            Washington.TopCity = Washington.Cities[0];
            bigCities = bigStates.SelectMany(state => state.Cities).ToList();
            bigCityVar1 = bigCities.Select(bc => bc.Var1).ToList();
            streets = bigCities.SelectMany(bc => bc.Streets).ToList();
        }

        public static ReadOnlyTestContext CreateBaseLineContext()
        {
            return new ReadOnlyTestContext();
        }

        public IQueryable<League> Leagues
        {
            get { return leagues.Values.AsQueryable(); }
        }

        public IQueryable<Team> Teams
        {
            get { return teams.Values.AsQueryable(); }
        }

        public IQueryable<Player> Players
        {
            get
            {
                return teams.Values.SelectMany(t => t.Players).AsQueryable();
            }
        }

        public IQueryable<Mascot> Mascots
        {
            get
            {
                return teams.Values.Where(t => t.Mascot != null).Select(t => t.Mascot).AsQueryable();
            }
        }

        public IQueryable<StadiumSponsor> StadiumSponsors
        {
            get
            {
                return teams.Values.Where(t => t.HomeStadium != null && t.HomeStadium.Sponsor != null)
                    .Select(t => t.HomeStadium.Sponsor).AsQueryable();
            }
        }


        public IQueryable<StadiumName> StadiumNames
        {
            get { return stadiumNames.Values.AsQueryable(); }
        }

        public IQueryable<Stadium> Stadiums
        {
            get { return stadiums.Values.AsQueryable(); }
        }

        public IQueryable<Employee> Employees
        {
            get { return employees.Values.AsQueryable(); }
        }

        public IQueryable<YetAnotherAllPrimitiveTypesType> Var1
        {
            get { return this.var1.AsQueryable(); }
        }

        public IQueryable<MultipleKeyType> MoreVar1
        {
            get { return this.moreVar1.AsQueryable(); }
        }

        public IQueryable<MultipleKeyType2> RelatedVar1
        {
            get { return this.moreVar1.SelectMany(x => x.Related).AsQueryable(); }
        }

        public IQueryable<LameTypeWithPublicFields> LameTypeWithPublicFields
        {
            get
            {
                return new List<LameTypeWithPublicFields>()
                        { new LameTypeWithPublicFields { Id =5, Name = "Foo" },
                        new LameTypeWithPublicFields { Id =7, Name = "Bar" } }.
                    AsQueryable();
            }
        }

        public IQueryable<Parent> Parents
        {
            get { return this.parents.AsQueryable(); }
        }

        public IQueryable<Child> Children
        {
            get
            {
                return this.parents.SelectMany(p => p.Children).AsQueryable();
            }

        }

        public IQueryable<Table> Tables
        {
            get { return this.tables.AsQueryable(); }
        }

        public IQueryable<Row> Rows
        {
            get 
            { 
                return this.tables.SelectMany(t => t.Rows).AsQueryable(); 
            }
        }

        public IQueryable<TableInfo> TableInfos
        {
            get { return this.tables.Select(t => t.TableInfo).AsQueryable(); }
        }

        public IQueryable<RowDetail> RowDetails
        {
            get { return this.Rows.SelectMany(r => r.Details).AsQueryable(); }
        }

        public IQueryable<BigCity> BigCities
        {
            get { return this.bigCities.AsQueryable(); }
        }

        public IQueryable<BigState> BigStates
        {
            get { return this.bigStates.AsQueryable(); }
        }

        public IQueryable<BigCityVar1> BigCityVar1
        {
            get { return this.bigCityVar1.AsQueryable(); }
        }

        public IQueryable<Street> Streets
        {
            get { return this.streets.AsQueryable(); }
        }
    }

    public class League
    {
        private List<Team> teams = new List<Team>();

        public int ID { get; set; }
        public string Name { get; set; }

        public List<Team> Teams
        {
            get
            {
                return (List<Team>)ReadOnlyTestContext.CheckToSeeIfIncluded(this, "Teams", this.teams);
            }

            set
            {
                teams = value;
            }
        }

        public DateTimeOffset ConceptionDate { get; set; }

        public override bool Equals(object obj)
        {
            League left = this;
            League right = obj as League;

            if ((right == null) ||
               (right.ID != left.ID) ||
               (right.Name != left.Name) ||
               (right.ConceptionDate != left.ConceptionDate))
            {
                return false;
            }

            if (left.Teams != null && right.Teams != null)
            {

                IEnumerator<Team> leftTeams = left.Teams.GetEnumerator();
                IEnumerator<Team> rightTeams = right.Teams.GetEnumerator();

                if (left.Teams.Count != right.Teams.Count)
                    return false;

                while (leftTeams.MoveNext() && rightTeams.MoveNext())
                {
                    if (!leftTeams.Current.Equals(rightTeams.Current))
                    {
                        return false;
                    }
                }

            }

            return true;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }

    [DebuggerDisplay("Team TeamID={TeamID}")]
    public class Team
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public string City { get; set; }

        private Stadium stadium;
        private Mascot mascot;
        private List<Player> players;

        public byte[] Photo
        {
            get { return new byte[] { 9, 9, 9, 9, 9, 9, 9, 9 }; }
            set { value = null; }
        }

        public Stadium HomeStadium
        {
            get
            {
                return ReadOnlyTestContext.CheckToSeeIfIncluded(this, "HomeStadium", stadium);
            }
            set
            {
                stadium = value;
            }
        }

        public Mascot Mascot
        {
            get
            {
                return ReadOnlyTestContext.CheckToSeeIfIncluded(this, "Mascot", mascot);
            }
            set
            {
                mascot = value;
            }
        }

        public List<Player> Players
        {
            get
            {
                return ReadOnlyTestContext.CheckToSeeIfIncluded(this, "Players", players);
            }
            set
            {
                players = value;
            }
        }

        public Team()
        {
            this.Players = new List<Player>();
        }

        public override bool Equals(object obj)
        {
            Team left = this;
            Team right = obj as Team;

            if (left == null && right == null)
            {
                return true;
            }

            if ((right == null) ||
               (right.TeamID != left.TeamID) ||
               (right.TeamName != left.TeamName) ||
               (right.City != left.City))
            {
                return false;
            }

            if (left.Players != null && right.Players != null)
            {
                IEnumerator<Player> leftPlayers = left.Players.GetEnumerator();
                IEnumerator<Player> rightPlayers = right.Players.GetEnumerator();

                if (left.Players.Count != right.Players.Count)
                    return false;

                while (leftPlayers.MoveNext() && rightPlayers.MoveNext())
                {
                    if (!leftPlayers.Current.Equals(rightPlayers.Current))
                    {
                        return false;
                    }
                }
            }

            if ((left.HomeStadium == null && right.HomeStadium != null)
                || (left.HomeStadium != null && right.HomeStadium == null))
            {
                return false;
            }

            if (left.HomeStadium != null && right.HomeStadium != null)
            {
                if (!left.HomeStadium.Equals(right.HomeStadium))
                {
                    return false;
                }
            }

            if ((left.Mascot == null && right.Mascot != null)
                || (left.Mascot != null && right.Mascot == null))
            {
                return false;
            }

            if (left.Mascot != null && right.Mascot != null)
            {
                if (!left.Mascot.Equals(right.Mascot))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this.TeamID;
        }

        public override string ToString()
        {
            return "Team=" + this.TeamID;
        }
    }

    public class AllStarTeam : Team
    {
    }

    [Key("ID")]
    [DebuggerDisplay("Stadiumd ID={ID}")]
    public class Stadium
    {
        public int ID { get; set; }
        public long Capacity { get; set; }
        public string City { get; set; }
        private StadiumSponsor sponsor;
        private string name;
        private List<StadiumName> formerNames;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public static explicit operator NarrowStadium(Stadium s)
        {
            return new NarrowStadium { ID = s.ID, City = s.City };
        }

        public StadiumSponsor Sponsor
        {
            get
            {
                return ReadOnlyTestContext.CheckToSeeIfIncluded(this, "Sponsor", sponsor);
            }
            set
            {
                sponsor = value;
            }
        }

        public List<StadiumName> FormerNames
        {
            get
            {
                return ReadOnlyTestContext.CheckToSeeIfIncluded(this, "FormerNames", formerNames);
            }
            set
            {
                formerNames = value;
            }
        }

        public override bool Equals(object obj)
        {
            Stadium left = this;
            Stadium right = obj as Stadium;

            if (right == null)
            {
                return false;
            }

            if ((left.ID != right.ID) ||
                (left.Capacity != right.Capacity) ||
                (left.City != right.City) ||
                (left.Name != right.Name))
            {
                return false;
            }

            if ((left.Sponsor == null && right.Sponsor != null)
                || (left.Sponsor != null && right.Sponsor == null))
            {
                return false;
            }

            if (left.Sponsor != null && right.Sponsor != null)
            {
                if (!left.Sponsor.Equals(right.Sponsor))
                {
                    return false;
                }
            }

            if (left.FormerNames != null && right.FormerNames != null)
            {
                if (left.FormerNames.Count != right.FormerNames.Count)
                    return false;

                IEnumerator<StadiumName> leftNames = left.FormerNames.GetEnumerator();
                IEnumerator<StadiumName> rightNames = right.FormerNames.GetEnumerator();


                while (leftNames.MoveNext() && rightNames.MoveNext())
                {
                    if (!leftNames.Current.Equals(rightNames.Current))
                    {
                        return false;
                    }
                }
            }
            else if (left.FormerNames != null || right.FormerNames != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public override string ToString()
        {
            return "Stadium(ID=" + this.ID + ",Name=" + this.Name + ")";
        }
    }


    [Key("StadiumSponsorID")]
    public class NarrowStadiumSponsor
    {
        public int StadiumSponsorID { get; set; }

        public override bool Equals(object obj)
        {
            NarrowStadiumSponsor l = this;
            NarrowStadiumSponsor r = obj as NarrowStadiumSponsor;

            if (l.StadiumSponsorID != r.StadiumSponsorID)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.StadiumSponsorID;
        }
    }

    [Key("ID")]
    public class NarrowStadium
    {
        public int ID { get; set; }
        public string City { get; set; }
        private NarrowStadiumSponsor sponsor;
        private List<StadiumName> formerNames;

        public NarrowStadiumSponsor Sponsor
        {
            get
            {
                return ReadOnlyTestContext.CheckToSeeIfIncluded(this, "Sponsor", sponsor);
            }
            set
            {
                sponsor = value;
            }
        }

        public List<StadiumName> FormerNames
        {
            get
            {
                return ReadOnlyTestContext.CheckToSeeIfIncluded(this, "FormerNames", formerNames);
            }
            set
            {
                formerNames = value;
            }
        }

        public override bool Equals(object obj)
        {
            NarrowStadium left = this;
            NarrowStadium right = obj as NarrowStadium;

            if ((left.ID != right.ID) ||
                (left.City != right.City))
            {
                return false;
            }

            if ((left.Sponsor == null && right.Sponsor != null)
                || (left.Sponsor != null && right.Sponsor == null))
            {
                return false;
            }

            if (left.Sponsor != null && right.Sponsor != null)
            {
                if (!left.Sponsor.Equals(right.Sponsor))
                {
                    return false;
                }
            }

            if (left.FormerNames != null && right.FormerNames != null)
            {
                if (left.FormerNames.Count != right.FormerNames.Count)
                    return false;

                IEnumerator<StadiumName> leftNames = left.FormerNames.GetEnumerator();
                IEnumerator<StadiumName> rightNames = right.FormerNames.GetEnumerator();


                while (leftNames.MoveNext() && rightNames.MoveNext())
                {
                    if (!leftNames.Current.Equals(rightNames.Current))
                    {
                        return false;
                    }
                }
            }
            else if (left.FormerNames != null || right.FormerNames != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public override string ToString()
        {
            return "NwStadium(ID= " + this.ID;
        }
    }

    [Key("StadiumSponsorID")]
    public class StadiumSponsor
    {
        public int StadiumSponsorID { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            StadiumSponsor l = this;
            StadiumSponsor r = obj as StadiumSponsor;

            if ((l.StadiumSponsorID != r.StadiumSponsorID) ||
                (l.Name != r.Name))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.StadiumSponsorID;
        }
    }

    [Key("StadiumNameID")]
    public class StadiumName
    {
        public int StadiumNameID { get; set; }
        public string Name { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        public override bool Equals(object obj)
        {
            StadiumName l = this;
            StadiumName r = obj as StadiumName;

            if ((l.StadiumNameID != r.StadiumNameID) ||
                (l.Name != r.Name) ||
                (!l.StartDate.Equals(r.StartDate)) ||
                (!l.EndDate.Equals(r.EndDate)))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.StadiumNameID;
        }
    }

    public class Mascot
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            Mascot l = this;
            Mascot r = obj as Mascot;

            if ((l.ID != r.ID) ||
                (l.Name != r.Name))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }

    [DebuggerDisplay("Player ID={ID}")]
    public class Player : INotifyPropertyChanged 
    {
        private string firstName;
        public static bool EqualsUsesReferences { get; set; }

        public int ID { get; set; }
        public string FirstName 
        {
            get { return this.firstName; }
            set
            {
                this.firstName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("FirstName"));
                }
            }
        }

        public string Lastname { get; set; }
        public Team Team { get; set; }

        public override bool Equals(object obj)
        {
            if (EqualsUsesReferences)
            {
                return object.ReferenceEquals(this, obj);
            }

            Player left = this;
            Player right = obj as Player;

            if (left == null && right == null)
            {
                return true;
            }

            if ((right == null) ||
               (right.ID != left.ID) ||
               (right.FirstName != left.FirstName) ||
               (right.Lastname != left.Lastname))
            {
                return false;
            }

            if ((right.Team == null && left.Team != null)
                || (right.Team != null && left.Team == null))
            {
                return false;
            }

            // team is a back pointer, so just check team name to avoid cicular reference
            if (right.Team != null && left.Team != null && right.Team.TeamName != left.Team.TeamName)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            if (EqualsUsesReferences)
            {
                return base.GetHashCode();
            }

            return this.ID;
        }

        public override string ToString()
        {
            return "Player ID=" + this.ID;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class YetAnotherAllPrimitiveTypesType
    {
        public int ID { get; set; }

        // primitives

        public Boolean Boolean { get; set; }
        public byte Byte { get; set; }
        public short Short { get; set; }
        public int Int { get; set; }
        public long Long { get; set; }
        public string String { get; set; }
        public float Float { get; set; }
        public decimal Decimal { get; set; }
        public double Double { get; set; }
        public Guid Guid { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public TimeSpan TimeSpan { get; set; }

        // nullables

        public Boolean? NullableBoolean { get; set; }
        public byte? NullableByte { get; set; }
        public short? NullableShort { get; set; }
        public int? NullableInt { get; set; }
        public long? NullableLong { get; set; }
        public float? NullableFloat { get; set; }
        public decimal? NullableDecimal { get; set; }
        public double? NullableDouble { get; set; }
        public Guid? NullableGuid { get; set; }
        public DateTimeOffset? NullableDateTimeOffset { get; set; }
        public TimeSpan? NullableTimeSpan { get; set; }

        public override bool Equals(object obj)
        {
            YetAnotherAllPrimitiveTypesType left = this;
            YetAnotherAllPrimitiveTypesType right = obj as YetAnotherAllPrimitiveTypesType;

            return (left.ID == right.ID &&
                left.Boolean == right.Boolean &&
                left.Byte == right.Byte &&
                left.Short == right.Short &&
                left.Int == right.Int &&
                left.Long == right.Long &&
                left.String == right.String &&
                left.Float == right.Float &&
                left.Decimal == right.Decimal &&
                left.Double == right.Double &&
                left.Guid == right.Guid &&
                left.DateTimeOffset == right.DateTimeOffset &&
                left.TimeSpan == right.TimeSpan &&
                left.ID == right.ID &&
                left.NullableBoolean == right.NullableBoolean &&
                left.NullableByte == right.NullableByte &&
                left.NullableShort == right.NullableShort &&
                left.NullableInt == right.NullableInt &&
                left.NullableLong == right.NullableLong &&
                left.NullableFloat == right.NullableFloat &&
                left.NullableDecimal == right.NullableDecimal &&
                left.NullableDouble == right.NullableDouble &&
                left.NullableGuid == right.NullableGuid &&
                left.NullableDateTimeOffset == right.NullableDateTimeOffset &&
                left.NullableTimeSpan == right.NullableTimeSpan
                );
        }

        public override int GetHashCode()
        {
            return (int)this.ID;
        }
    }

    public class YetAnotherAllPrimitiveTypesNotEntityType
    {
        // primitives

        public Boolean Boolean { get; set; }
        public byte Byte { get; set; }
        public short Short { get; set; }
        public int Int { get; set; }
        public long Long { get; set; }
        public string String { get; set; }
        public float Float { get; set; }
        public decimal Decimal { get; set; }
        public double Double { get; set; }
        public Guid Guid { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public TimeSpan TimeSpan { get; set; }

        // nullables

        public Boolean? NullableBoolean { get; set; }
        public byte? NullableByte { get; set; }
        public short? NullableShort { get; set; }
        public int? NullableInt { get; set; }
        public long? NullableLong { get; set; }
        public float? NullableFloat { get; set; }
        public decimal? NullableDecimal { get; set; }
        public double? NullableDouble { get; set; }
        public Guid? NullableGuid { get; set; }
        public DateTimeOffset? NullableDateTimeOffset { get; set; }
        public TimeSpan? NullableTimeSpan { get; set; }

        public override bool Equals(object obj)
        {
            YetAnotherAllPrimitiveTypesNotEntityType left = this;
            YetAnotherAllPrimitiveTypesNotEntityType right = obj as YetAnotherAllPrimitiveTypesNotEntityType;

            return (
                left.Boolean == right.Boolean &&
                left.Byte == right.Byte &&
                left.Short == right.Short &&
                left.Int == right.Int &&
                left.Long == right.Long &&
                left.String == right.String &&
                left.Float == right.Float &&
                left.Decimal == right.Decimal &&
                left.Double == right.Double &&
                left.Guid == right.Guid &&
                left.DateTimeOffset == right.DateTimeOffset &&
                left.TimeSpan == right.TimeSpan &&
                left.NullableBoolean == right.NullableBoolean &&
                left.NullableByte == right.NullableByte &&
                left.NullableShort == right.NullableShort &&
                left.NullableInt == right.NullableInt &&
                left.NullableLong == right.NullableLong &&
                left.NullableFloat == right.NullableFloat &&
                left.NullableDecimal == right.NullableDecimal &&
                left.NullableDouble == right.NullableDouble &&
                left.NullableGuid == right.NullableGuid &&
                left.NullableDateTimeOffset == right.NullableDateTimeOffset &&
                left.NullableTimeSpan == right.NullableTimeSpan
                );
        }

        public override int GetHashCode()
        {
            return (int)this.Int;
        }
    }

    [Key("Key1", "Key2", "Key3")]
    public class MultipleKeyType
    {
        public int Key1 { get; set; }
        public string Key2 { get; set; }
        public long Key3 { get; set; }

        public string Data1 { get; set; }

        public string Data2 { get; set; }

        private List<MultipleKeyType2> related = new List<MultipleKeyType2>();

        public List<MultipleKeyType2> Related
        {
            get
            {
                return ReadOnlyTestContext.CheckToSeeIfIncluded(this, "Related", related);
            }
            set
            {
                related = value;
            }
        }

        public override bool Equals(object obj)
        {
            MultipleKeyType right = obj as MultipleKeyType;
            MultipleKeyType left = this;

            return left.Key1 == right.Key1 &&
                left.Key2 == right.Key2 &&
                left.Key3 == right.Key3 &&
                left.Data1 == right.Data1 &&
                left.Data2 == right.Data2;
        }

        public override int GetHashCode()
        {
            return this.Key1;
        }
    }

    [Key("FKKey", "Key")]
    public class MultipleKeyType2
    {
        public int FKKey { get; set; }
        public int Key { get; set; }

        public string Data2 { get; set; }

        public override bool Equals(object obj)
        {
            MultipleKeyType2 right = obj as MultipleKeyType2;
            MultipleKeyType2 left = this;

            return left.Key == right.Key &&
                left.FKKey == right.FKKey &&
                left.Data2 == right.Data2;
        }

        public override int GetHashCode()
        {
            return this.Key;
        }
    }

    public class Employee
    {
        public int EmployeeID { get; set; }
        public Employee Manager { get; set; }
        public List<Employee> Reports { get; set; }
        public string Name { get; set; }

        public Employee()
        {
            Reports = new List<Employee>();
        }

        public void AddReport(Employee e)
        {
            Reports.Add(e);
            e.Manager = this;
        }

        public override bool Equals(object obj)
        {
            Employee left = this;
            Employee right = obj as Employee;

            if (left == null && right == null)
            {
                return true;
            }

            if ((right == null) ||
               (right.EmployeeID != left.EmployeeID) ||
               (right.Name != left.Name))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this.EmployeeID;
        }
    }

    [Key("Id")]
    public class LameTypeWithPublicFields
    {
        public int Id { get; set; }
        public string Name;
    }

    [Key("Id")]
    public class Parent
    {
        public int Id { get; set; }
        public List<Child> Children { get; set; }
    }

    [Key("Id")]
    public class Child
    {
        public int Id { get; set; }
        public List<Parent> Parents { get; set; }
        public Parent Mom { get; set; }
        public Parent Dad { get; set; }
    }

    [Key("TableName")]
    public class Table
    {
        public string TableName { get; set; }

        private TableInfo tableInfo;
        public TableInfo TableInfo
        {
            get
            {
                return (TableInfo)ReadOnlyTestContext.CheckToSeeIfIncluded(this, "TableInfo", this.tableInfo);
            }

            set
            {
                tableInfo = value;
            }
        }

        private List<Row> rows;
        public List<Row> Rows
        {
            get
            {
                return (List<Row>)ReadOnlyTestContext.CheckToSeeIfIncluded(this, "Rows", this.rows);
            }

            set
            {
                rows = value;
            }
        }

        public static explicit operator MyTable(Table r)
        {
            return new MyTable { Rows = r.Rows, TableInfo = r.TableInfo, TableName = r.TableName };
        }
    }

    [Key("Id")]
    public class Row
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Zip { get; set; }

        private List<RowDetail> details;
        public List<RowDetail> Details
        {
            get
            {
                return (List<RowDetail>)ReadOnlyTestContext.CheckToSeeIfIncluded(this, "Details", this.details);
            }

            set
            {
                details = value;
            }
        }

        public static explicit operator CustomerRow(Row r)
        {
            return new CustomerRow { Id = r.Id, Name = r.Name, Zip = r.Zip };
        }
    }

    [Key("Id")]
    public class RowDetail
    {
        public int Id { get; set; }
        public string Word { get; set; }

        public static explicit operator CustomerRowDetail(RowDetail d)
        {
            return new CustomerRowDetail { Id = d.Id, Word = d.Word };
        }
    }

    [Key("TableName")]
    public class TableInfo
    {
        public string TableName { get; set; }
        public string Info { get; set; }

        public static explicit operator CustomTableInfo(TableInfo ti)
        {
            return new CustomTableInfo { TableName = ti.TableName, Info = ti.Info };
        }
    }

    [Key("TableName")]
    public class MyTable
    {
        public string TableName { get; set; }
        public List<Row> Rows { get; set; }
        public TableInfo TableInfo { get; set; }

        public override bool Equals(object obj)
        {
            MyTable left = this;
            MyTable right = obj as MyTable;

            if (left == null && right == null)
            {
                return true;
            }

            if ((right == null) ||
               (right.TableName!= left.TableName))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }

    [Key("Id")]
    public class CustomerRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Zip { get; set; }
        public List<RowDetail> Details { get; set; }

        public override bool Equals(object obj)
        {
            CustomerRow left = this;
            CustomerRow right = obj as CustomerRow;

            if (left == null && right == null)
            {
                return true;
            }

            if ((right == null) ||
               (right.Id != left.Id) ||
               (right.Name != left.Name) ||
               (right.Zip != left.Zip))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }
    }

    [Key("Id")]
    public class CustomerRowDetail
    {
        public int Id { get; set; }
        public string Word { get; set; }

        public override bool Equals(object obj)
        {
            CustomerRowDetail left = this;
            CustomerRowDetail right = obj as CustomerRowDetail;

            if (left == null && right == null)
            {
                return true;
            }

            if ((right == null) ||
               (right.Id != left.Id) ||
               (right.Word != left.Word))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }
    }

    [Key("TableName")]
    public class CustomTableInfo
    {
        public string TableName { get; set; }
        public string Info { get; set; }

        public override bool Equals(object obj)
        {
            CustomTableInfo left = this;
            CustomTableInfo right = obj as CustomTableInfo;

            if (left == null && right == null)
            {
                return true;
            }

            if ((right == null) ||
               (right.TableName != left.TableName) ||
               (right.Info != left.Info))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }

    [Key("StateName")]
    public class BigState
    {
        private BigCity topcity;
        private BigCity coolestCity;
        private List<BigCity> cities;

        public short ID { get; set; }
        public string StateName { get; set; }
        public BigCity TopCity 
        { 
            get { return (BigCity) ReadOnlyTestContext.CheckToSeeIfIncluded(this, "TopCity", this.topcity); }
            set { this.topcity = value; }
        } 

        public BigCity CoolestCity 
        {
            get { return (BigCity)ReadOnlyTestContext.CheckToSeeIfIncluded(this, "CoolestCity", this.coolestCity); }
            set { this.coolestCity = value; }
        } 

        public int Population { get; set; }

        public List<BigCity> Cities
        {
            get { return (List<BigCity>)ReadOnlyTestContext.CheckToSeeIfIncluded(this, "Cities", this.cities); }
            set { this.cities = value; }
        } 

        public string DontEverInclude
        { 
            get { throw new Exception("Do not ever project");} 
            set { throw new Exception("Bad News"); }
        } 

    }

    [Key("StateName")]
    public class LittleState
    {
        public LittleState() { }
        public LittleState(short x) { }

        public int ID { get; set; }  // TODO: when have no key constraint, remove this and add new attribute.
        public string StateName { get; set; }
        public LittleCity TopCity { get; set; }
        public List<LittleCity> Cities { get; set; }
    }

    public class LittleState2
    {
        public int ID { get; set; }  
        public string StateName { get; set; }
        public DataServiceCollection<LittleCity> Cities { get; set; }
    }

    [Key("ID")]
    public class LittleState3
    {
        public long ID { get; set; }
        public string StateName { get; set; }
        public LittleCity3 TopCity { get; set; }
        public List<LittleCity> Cities { get; set; }
    }

    [Key("ID")]
    public class LittleState4
    {
        public long ID { get; set; }
        public string StateName { get; set; }
        public List<BigCity> Cities { get; set; }
        public List<string> CityNames { get; set; }
    }

    public class DummyEntity
    {
        public int ID { get; set; }
        public object a { get; set; }
    }

    [DebuggerDisplay("BigCity BigCityID={BigCityID}")]
    [Key("BigCityID")]
    public class BigCity
    {
        private BigCityVar1 _bigCityVar1;
        private BigState bigState;
        private List<Street> streets;

        public BigState State
        {
            get { return (BigState)ReadOnlyTestContext.CheckToSeeIfIncluded(this, "State", this.bigState); }
            set { this.bigState = value; }
        }

        public List<Street> Streets
        {
            get { return (List<Street>)ReadOnlyTestContext.CheckToSeeIfIncluded(this, "Streets", this.streets); }
            set { this.streets = value; }

        }

        public long BigCityID { get; set; } 
        public float Population { get; set; }
        public string Mayor { get; set; }
        public BigCityComplexType MiscInfo { get; set; }

        public BigCityVar1 Var1
        {
            get { return (BigCityVar1)ReadOnlyTestContext.CheckToSeeIfIncluded(this, "Var1", this._bigCityVar1); }
            set { this._bigCityVar1 = value; }
        } 

        public string Name { get; set; }
        public bool? HasBaseballTeam { get; set; }

        public static explicit operator LittleCity3(BigCity bc){throw new Exception("Use only for cast tests");}
        public static explicit operator BigCityComplexType(BigCity bc){throw new Exception("Use only for cast tests");}
        public static BigCity operator +(BigCity left, BigCity right) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator -(BigCity left, BigCity right) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator *(BigCity left, BigCity right) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator /(BigCity left, BigCity right) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator %(BigCity left, BigCity right) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator &(BigCity left, BigCity right) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator |(BigCity left, BigCity right) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator <<(BigCity left, int right) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator >>(BigCity left, int right) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator +(BigCity left) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator -(BigCity left) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator !(BigCity left) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator ~(BigCity left) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator ++(BigCity left) { throw new Exception("Use only for operator tests"); }
        public static BigCity operator --(BigCity left) { throw new Exception("Use only for operator tests"); }
        public static bool operator true(BigCity left) { throw new Exception("Use only for operator tests"); }
        public static bool operator false(BigCity left) { throw new Exception("Use only for operator tests"); }
        public static bool operator <(BigCity left, BigCity right) { throw new Exception("Use only for operator tests"); }
        public static bool operator >(BigCity left, BigCity right) { throw new Exception("Use only for operator tests"); }
        public static bool operator <=(BigCity left, BigCity right) { throw new Exception("Use only for operator tests"); }
        public static bool operator >=(BigCity left, BigCity right) { throw new Exception("Use only for operator tests"); }
    }

    public class DerivedBigCity : BigCity { }

    [Key("StreetName")]
    public class Street
    {
        private string streetName;
        private int numberOfBlocks;

        public string StreetName { get { return streetName; } set { streetName = value;  } }
        public int NumberOfBlocks { get { return numberOfBlocks; } set { numberOfBlocks = value; } }
    }

    [Key("StreetName")]
    public class NarrowStreet : INotifyPropertyChanged
    {
        private string streetName;
        private int numberOfBlocks;

        public string StreetName { get { return streetName; } set { streetName = value; NotifyPropertyChanged("StreetName"); } }
        public int NumberOfBlocks { get { return numberOfBlocks; } set { numberOfBlocks = value; NotifyPropertyChanged("NumberOfBlocks"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    [DebuggerDisplay("LittleCity BigCityID={BigCityID}")]
    [Key("BigCityID")]
    public class LittleCity : INotifyPropertyChanged
    {
        private string name;

        public long BigCityID { get; set; } 
        public LittleState State { get; set; } // TODO: need to check for include
        public float Population { get; set; }
        public string Mayor { get; set; }
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [Key("BigCityID")]
    public class LittleCity3
    {
        public long BigCityID { get; set; }
        public int Population { get; set; }
        public string Mayor { get; set; }
    }

    [Key("BigCityID")]
    [EntitySet("BigCities")]
    public class BindableLittleCity : INotifyPropertyChanged
    {
        public long bigCityID;
        public float population;
        public string mayor;
        public DataServiceCollection<NarrowStreet> streets;

        public long BigCityID { get { return bigCityID; } set { bigCityID = value; NotifyPropertyChanged("BigCityID"); } }
        public float Population { get { return population; } set { population = value; NotifyPropertyChanged("Population"); } }
        public string Mayor { get { return mayor; } set { mayor = value; NotifyPropertyChanged("Mayor"); } }

        public DataServiceCollection<NarrowStreet> Streets { get { return streets; } set { streets = value; NotifyPropertyChanged("Streets"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public class LittleCityNotAnEntity
    {
        public string BigCityID { get; set; }
        public int Population { get; set; }
        public string Mayor { get; set; }
    }

    public class BigCityComplexType
    {
        public string A { get; set; }
        public string B { get; set; }
        public BigCityComplexType MoreMiscInfo { get; set; }

        public static explicit operator int(BigCityComplexType bc) { return 5; }
        public static explicit operator int?(BigCityComplexType bc) { return 5; }
        public static BigCityComplexType operator +(BigCityComplexType left, BigCityComplexType right) { return left; }
        public static BigCityComplexType operator -(BigCityComplexType left, BigCityComplexType right) { return left; }
        public static BigCityComplexType operator *(BigCityComplexType left, BigCityComplexType right) { return left; }
        public static BigCityComplexType operator /(BigCityComplexType left, BigCityComplexType right) { return left; }
        public static BigCityComplexType operator %(BigCityComplexType left, BigCityComplexType right) { return left; }
        public static BigCityComplexType operator &(BigCityComplexType left, BigCityComplexType right) { return left; }
        public static BigCityComplexType operator |(BigCityComplexType left, BigCityComplexType right) { return left; }
        public static BigCityComplexType operator <<(BigCityComplexType left, int right) { return left; }
        public static BigCityComplexType operator >>(BigCityComplexType left, int right) { return left; }
        public static BigCityComplexType operator +(BigCityComplexType left) { return left; }
        public static BigCityComplexType operator -(BigCityComplexType left) { return left; }
        public static BigCityComplexType operator !(BigCityComplexType left) { return left; }
        public static BigCityComplexType operator ~(BigCityComplexType left) { return left; }
        public static BigCityComplexType operator ++(BigCityComplexType left) { return left; }
        public static BigCityComplexType operator --(BigCityComplexType left) { return left; }
        public static bool operator true(BigCityComplexType left) { return true; }
        public static bool operator false(BigCityComplexType left) { return false; }
        public static bool operator <(BigCityComplexType left, BigCityComplexType right) { return false; }
        public static bool operator >(BigCityComplexType left, BigCityComplexType right) { return false; }
        public static bool operator <=(BigCityComplexType left, BigCityComplexType right) { return false; }
        public static bool operator >=(BigCityComplexType left, BigCityComplexType right) { return false; }
    }

    public class BigCityComplexType2 : BigCityComplexType
    {
        public string C { get; set; }
    }

    [Key("ID")]
    public class BigCityVar1
    {
        public int ID { get; set; }

        // primitives

        public Boolean Boolean { get; set; }
        public byte Byte { get; set; }
        public int Short { get; set; }
        public int Int { get; set; }
        public int Long { get; set; }
        public string String { get; set; }
        public float Float { get; set; }
        public decimal Decimal { get; set; }
        public double Double { get; set; }
        public Guid Guid { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public TimeSpan TimeSpan { get; set; }

        // nullables

        public Boolean? NullableBoolean { get; set; }
        public byte? NullableByte { get; set; }
        public long? NullableShort { get; set; }
        public long? NullableInt { get; set; }
        public long? NullableLong { get; set; }
        public float? NullableFloat { get; set; }
        public decimal? NullableDecimal { get; set; }
        public double? NullableDouble { get; set; }
        public Guid? NullableGuid { get; set; }
        public DateTimeOffset? NullableDateTimeOffset { get; set; }
        public TimeSpan? NullableTimeSpan { get; set; }
    }
}

