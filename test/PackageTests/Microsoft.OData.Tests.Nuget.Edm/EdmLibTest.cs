//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
namespace Microsoft.OData.Tests.Nuget.Edm
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using System.IO;
    using Xunit;

    public class EdmLibTest
    {
        [Fact]
        public void BasicTest()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "Microsoft.OData.Tests.Nuget.Edm.Schema.xml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    IEdmModel model = CsdlReader.Parse(XmlTextReader.Create(stream));
                    Assert.NotEmpty(model.EntityContainer.Name);
                }
            }
        }
    }
}
