//---------------------------------------------------------------------
// <copyright file="LinqToSqlTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Security.Permissions;
using System.Security;

namespace System.Data.Test.Astoria
{
    public static class LinqToSqlTypes
    {
        public static readonly NodeType Binary = new LinqToSqlBinary();
        public static readonly NodeType XElement = new LinqToSqlXml();
    }

    // LinqToSqlBinary
    public class LinqToSqlBinary : PrimitiveType<System.Data.Linq.Binary>
    {
        //Data

        //Constructor
        public LinqToSqlBinary() : base(NodeFacet.Nullable(true))
        {
            Assembly dataLinqAssembly = TypeUtil.GetAssembly(this.Namespace);
            if (dataLinqAssembly == null)
               dataLinqAssembly = Assembly.LoadWithPartialName(this.Namespace);

            this.ClrType = dataLinqAssembly.GetType("System.Data.Linq.Binary");
        }

        public override NodeValue CreateRandomValueForFacets(NodeFacets propertyFacets)
        {
            fxList<byte> bytes = new fxList<byte>();
            int maxSize = propertyFacets.MaxSize != null ? propertyFacets.MaxSize.Value : 512;
            int actualSize = maxSize;
            if (!propertyFacets.FixedLength)
            {
                //Set max size to be random
                actualSize = AstoriaTestProperties.Random.Next(maxSize);
                if (actualSize > 1000)
                    actualSize = 1000;
            }
            for (int i = 0; i < actualSize; i++)
            {
                bytes.Add((byte)AstoriaTestProperties.Random.Next(byte.MinValue, byte.MaxValue));
            }
            return new NodeValue(new System.Data.Linq.Binary(bytes.ToArray()), this);
        }
    }

    // LinqToSqlXml
    public class LinqToSqlXml : PrimitiveType<System.Xml.Linq.XElement>
    {
        //Data

        //Constructor
        public LinqToSqlXml() : base(NodeFacet.Nullable(true))
        {
            Assembly dataLinqAssembly = TypeUtil.GetAssembly(this.Namespace);
            if (dataLinqAssembly == null)
                dataLinqAssembly = Assembly.LoadWithPartialName(this.Namespace);

            this.ClrType = dataLinqAssembly.GetType("System.Xml.Linq.XElement");
        }

        public override NodeValue CreateRandomValueForFacets(NodeFacets propertyFacets)
        {
            // TODO: Add some random goo
            return new NodeValue(new System.Xml.Linq.XElement("Root"), this);
        }
    }

    [FileIOPermission(SecurityAction.Assert, Unrestricted=true)]
    [SecuritySafeCritical]
    internal static class TypeUtil
    {
        internal static Assembly GetAssembly(string name)
        {
            Assembly foundAssembly = null;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    foundAssembly = assembly;
                    break;
                }
            }

            return foundAssembly;
        }
    }
}
