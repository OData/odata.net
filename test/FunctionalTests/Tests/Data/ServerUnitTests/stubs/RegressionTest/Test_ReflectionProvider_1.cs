//---------------------------------------------------------------------
// <copyright file="Test_ReflectionProvider_1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace TestReflectionProvider
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Linq;

    public class TestDataContext
    {
        private static List<Type> registeredTypes = new List<Type>();

        public IQueryable<TestEntityRP> Entities
        {
            get
            {
                return new List<TestEntityRP>().AsQueryable<TestEntityRP>();
            }
        }

        public static void InitializeService(IDataServiceConfiguration configuration)
        {
            foreach(Type type in registeredTypes)
            {
                configuration.RegisterKnownType(type);
            }
        }

        public static void RegisterType(Type type)
        {
            registeredTypes.Add(type);
        }
    }

    public class TestEntityRP
    {
        public int ID { get; set; }
        public string FooBaz { get; set; }
    }
}
