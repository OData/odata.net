//---------------------------------------------------------------------
// <copyright file="AstoriaTestObjectContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    public class AstoriaTestObjectContext : System.Data.Objects.ObjectContext
    {
        public AstoriaTestObjectContext(string connectionString) : base(connectionString) { }
        public AstoriaTestObjectContext(System.Data.EntityClient.EntityConnection connection) : base(connection) { }
        public AstoriaTestObjectContext(string connectionString, string defaultContainerName) : base(connectionString, defaultContainerName) { }
        public AstoriaTestObjectContext(System.Data.EntityClient.EntityConnection connection, string defaultContainerName) : base(connection, defaultContainerName) { }
    }

    public class AstoriaTestEntityObject : System.Data.Objects.DataClasses.EntityObject { }
}