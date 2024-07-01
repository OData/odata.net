//-----------------------------------------------------------------------------
// <copyright file="PeopleController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.OData.Client.E2E.Tests.AnnotationTests.Server
{
    public class PeopleController : ODataController
    {
        public PeopleController() { }

        [EnableQuery]
        public IActionResult Get()
        {
            var people = AnnotationTestsDataSource.People;
            return Ok(people);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var person = AnnotationTestsDataSource.People?.FirstOrDefault(a => a.PersonID == key);
            return Ok(person);
        }
    }
}
