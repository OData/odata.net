//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
namespace Microsoft.OData.Tests.Nuget.Client
{
    using System;
    using System.Linq;
    using Microsoft.OData.SampleService.Models.TripPin;
    using OData.Client;
    using Xunit;

    public class ClientLibTest
    {
        [Fact]
        public void BasicTest()
        {
            var dsc = new DefaultContainer(new Uri("http://services.odata.org/V4/(S(ply0t0sqxoh4wooviq211x55))/TripPinServiceRW/"));
            var people = dsc.People;
            people.BeginExecute(ReadingPeople, people);

        }

        private void ReadingPeople(IAsyncResult ar)
        {
            var peopleQuery = ar.AsyncState as DataServiceQuery<Person>;
            if (peopleQuery != null)
            {
                var people = peopleQuery.EndExecute(ar);
                Console.WriteLine(people.Count());
            }
        }
    }
}
