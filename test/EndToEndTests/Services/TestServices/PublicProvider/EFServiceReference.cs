//---------------------------------------------------------------------
// <copyright file="EFServiceReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client;
using Microsoft.Test.OData.Services.TestServices.PublicProviderEFServiceReference.AstoriaDefaultServiceDBModel;
namespace Microsoft.Test.OData.Services.TestServices.PublicProviderEFServiceReference.Microsoft.Test.OData.Services.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class AstoriaDefaultServiceDBEntities
    {
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
