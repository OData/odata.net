//---------------------------------------------------------------------
// <copyright file="ProviderMemberSimulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using Microsoft.OData.Service.Providers;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.Tests.Server.Simulators
{
    internal class ProviderMemberSimulator : IProviderMember
    {
        public static string SimulatedMimeType = "SimulatedMimeType/SimulatedSubType";
        public static string KeyPropertyName = "KeyProperty";

        private PropertyInfo property;

        public ProviderMemberSimulator(PropertyInfo property)
        {
            this.property = property;
        }

        public BuiltInTypeKind EdmTypeKind
        {
            get
            {
                if (this.EdmTypeName.Equals("Edm.EnumType"))
                {
                    return BuiltInTypeKind.EnumType;
                }
                return BuiltInTypeKind.PrimitiveType;
            }
        }

        public string Name
        {
            get { return this.property.Name; }
        }

        public bool IsKey
        {
            get { return this.Name == KeyPropertyName; }
        }

        public string EdmTypeName
        {
            get { return GetEdmTypeName(property.PropertyType); }
        }

        public string MimeType
        {
            get { return IsMimeType(this.Name) ? SimulatedMimeType : null; }
        }

        public EntityType CollectionItemType
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<MetadataProperty> MetadataProperties
        {
            get { return new MetadataProperty[0]; }
        }

        public IEnumerable<Facet> Facets
        {
            get { return new Facet[0]; }
        }

        public static string GetEdmTypeName(Type type)
        {
            if (type == typeof(int) || type == typeof(int?))
            {
                return "Edm.Int32";
            }
            else if (type == typeof(string))
            {
                return "Edm.String";
            }
            else if (type == typeof(PrimitiveResourceTypeMapTests.TestPrimitiveType))
            {
                return "Edm.TestPrimitiveType";
            }
            else if (type == typeof(Enum) || type == typeof(ObjectContextServiceProvider_MetadataTests.TestEnum))
            {
                return "Edm.EnumType";
            }
            Assert.Fail("Unable to map CLR type {0} to an EDM type", type);
            return null;
        }

        public static bool IsMimeType(string propertyName)
        {
            return propertyName.EndsWith("WithMimeType");
        }
    }
}
