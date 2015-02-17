//---------------------------------------------------------------------
// <copyright file="IODataUpdateProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IODataUpdateProvider
    {
        // TODO: workaround here. for details see the comment in CreateHandler
        object Create(string fullTypeName, object source);

        void CreateLink(object parent, string propertyName, object target);

        void Delete(object target);

        void DeleteLink(object parent, string propertyName, object target);

        void Update(object target, string propertyName, object propertyValue);

        void UpdateLink(object parent, string propertyName, object target);

        void UpdateETagValue(object target);

        void SaveChanges();

        void ClearChanges();
    }
}
