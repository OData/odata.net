//---------------------------------------------------------------------
// <copyright file="CustomObjectContext_Partial.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstoriaUnitTests.ObjectContextStubs
{
    public partial class CustomObjectContext
    {
        public static IDisposable CreateChangeScope()
        {
            PopulateData.CreateTableAndPopulateData();
            return new CustomObjectContextChangeScope();
        }
    }

    public class CustomObjectContextChangeScope : IDisposable
    {
        public void Dispose()
        {
            PopulateData.ClearConnection();
        }
    }
}
