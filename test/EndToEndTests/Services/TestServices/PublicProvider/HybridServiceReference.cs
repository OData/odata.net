//---------------------------------------------------------------------
// <copyright file="HybridServiceReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client;
using Microsoft.Test.OData.Services.TestServices.PublicProviderHybridServiceReference.AstoriaDefaultServiceDBModel;
using Microsoft.Test.OData.Services.TestServices.PublicProviderHybridServiceReference.Microsoft.Test.OData.Services.AstoriaDefaultService;
namespace Microsoft.Test.OData.Services.TestServices.PublicProviderHybridServiceReference.HybridService
    
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
      

    partial class AstoriaDefaultServiceDBEntities
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

        public int GetEFPersonCount()
        {
            var uri = new Uri("/GetEFPersonCount", UriKind.Relative);

            return Execute<int>(uri).FirstOrDefault();
        }

        public EFPerson GetEFPersonByExactName(string name)
        {
            if (name == null)
            {
                throw new DataServiceClientException("GetEFPersonByExactName requires name");
            }
            var uri = new Uri("/GetEFPersonByExactName?name='" + name + "'", UriKind.Relative);

            return Execute<EFPerson>(uri).FirstOrDefault();

        }

        public IEnumerable<EFPerson> GetEFPersonsByName(string name)
        {
            if (name == null)
            {
                throw new DataServiceClientException("GetEFPersonsByName requires name");
            }
            var uri = new Uri("/GetEFPersonsByName?name='" + name + "'", UriKind.Relative);

            return Execute<EFPerson>(uri);
        }
    }
}
