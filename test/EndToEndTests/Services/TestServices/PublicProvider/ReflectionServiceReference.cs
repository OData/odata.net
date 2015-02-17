//---------------------------------------------------------------------
// <copyright file="ReflectionServiceReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.TestServices.PublicProviderReflectionServiceReference
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Linq;

    partial class DefaultContainer
    {
        public int GetPersonCount()
        {
            var uri = new Uri("/GetPersonCount", UriKind.Relative);

            return Execute<int>(uri).FirstOrDefault();
        }

        public Person GetPersonByExactName(string name)
        {
            if (name == null)
            {
                throw new DataServiceClientException("GetPersonByExactName requires name");
            }
            var uri = new Uri("/GetPersonByExactName?name='" + name + "'", UriKind.Relative);

            return Execute<Person>(uri).FirstOrDefault();

        }

        public IEnumerable<Person> GetPersonsByName(string name)
        {
            if (name == null)
            {
                throw new DataServiceClientException("GetPersonsByName requires name");
            }
            var uri = new Uri("/GetPersonsByName?name='" + name + "'", UriKind.Relative);

            return Execute<Person>(uri);
        }
    }
}
