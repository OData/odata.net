//---------------------------------------------------------------------
// <copyright file="ModelRefInMemoryModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.SampleService.Models.ModelRefDemo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Services.ODataWCFService;

    public static class ModelRefInMemoryModel
    {
        public static IEdmModel CreateModelRefServiceModel()
        {
            IEnumerable<EdmError> errors;

            Func<Uri, XmlReader> getReferencedSchemaFunc = uri =>
            {
                if (uri.AbsoluteUri.StartsWith(ServiceConstants.ServiceBaseUri.AbsoluteUri))
                {
                    string filename = uri.AbsoluteUri.Substring(ServiceConstants.ServiceBaseUri.AbsoluteUri.Length, uri.AbsoluteUri.Length - ServiceConstants.ServiceBaseUri.AbsoluteUri.Length);
                    return XmlReader.Create(ReadResourceFromAssembly(filename));
                }
                else
                {
                    return null;
                }
            };

            IEdmModel model = ReadModel("TruckDemo.csdl", getReferencedSchemaFunc);
            model.Validate(out errors);
            return model;
        }

        private static IEdmModel ReadModel(string fileName, Func<Uri, XmlReader> getReferencedSchemaFunc = null)
        {
            IEdmModel model;
            using (Stream csdlStream = ReadResourceFromAssembly(fileName))
            {
                bool parseResult;
                IEnumerable<EdmError> errors;
                if (getReferencedSchemaFunc == null)
                {
                    parseResult = CsdlReader.TryParse(XmlReader.Create(csdlStream), out model, out errors);
                }
                else
                {
                    parseResult = CsdlReader.TryParse(XmlReader.Create(csdlStream), getReferencedSchemaFunc, out model, out errors);
                }

                if (!parseResult)
                {
                    throw new InvalidOperationException("Failed to load model : " + string.Join(Environment.NewLine, errors.Select(e => e.ErrorMessage)));
                }
            }
            return model;
        }

        private static Stream ReadResourceFromAssembly(string resourceName)
        {
            var testAssembly = Assembly.GetAssembly(typeof(ModelRefInMemoryModel));
            string fullResourceName = testAssembly.GetManifestResourceNames().Single(n => n.EndsWith(resourceName));
            Stream resourceStream = testAssembly.GetManifestResourceStream(fullResourceName);

            var reader = new StreamReader(resourceStream);
            string str = reader.ReadToEnd();
            str = str.Replace("[ServiceBaseUrl]", ServiceConstants.ServiceBaseUri.AbsoluteUri);
            byte[] byteArray = Encoding.UTF8.GetBytes(str);

            resourceStream = new MemoryStream(byteArray);
            return resourceStream;
        }
    }
}
