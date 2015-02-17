//---------------------------------------------------------------------
// <copyright file="Test_PartialClass.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;

    /// <summary>
    /// There are no comments for BugEntityContainer in the schema.
    /// </summary>
    public partial class BugEntityContainer : global::System.Data.Objects.ObjectContext
    {
        private static List<Type> registeredTypes = new List<Type>();

        public static void InitializeService(IDataServiceConfiguration configuration)
        {
            foreach (Type type in registeredTypes)
            {
                configuration.RegisterKnownType(type);
            }
        }

        public static void RegisterType(Type type)
        {
            registeredTypes.Add(type);
        }
    }
}