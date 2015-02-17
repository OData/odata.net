//---------------------------------------------------------------------
// <copyright file="Folder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PrimitiveKeysService
{
    using System.Collections.Generic;
    using Microsoft.OData.Client;

    [Key("Id")]
    public class Folder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Folder Parent { get; set; }

        public static IEnumerable<Folder> GetData()
        {
            var folder1 = new Folder { Id = 1, Name = "Program Files" };
            yield return folder1;

            var folder2 = new Folder { Id = 2, Name = "Microsoft WCF Data Services", Parent = folder1 };
            yield return folder2;

            var folder3 = new Folder { Id = 3, Name = "Current", Parent = folder2 };
            yield return folder3;
        }
    }
}
