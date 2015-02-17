//---------------------------------------------------------------------
// <copyright file="NonClrEntitySetDictionary.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System.Data.Test.Astoria.NonClr
{
    //TODO: replace this and InMemoryEntitySetDictionary with some common, generic, in-memory store thing

    public class NonClrEntitySetDictionary : Dictionary<string, List<RowEntityType>>
    {
        public NonClrEntitySetDictionary()
        {
        }

        public new List<RowEntityType> this[string containerName]
        {
            get
            {
                List<RowEntityType> list;
                if (!this.TryGetValue(containerName, out list))
                {
                    base[containerName] = list = new List<RowEntityType>();
                }
                return list;
            }
        }
    }
}
