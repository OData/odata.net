//---------------------------------------------------------------------
// <copyright file="TestUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;

    public static class TestUtils
    {
        /// <summary>
        /// Reads the common test model (based on AdventureWorks) as CSDL from assembly resources,
        /// and converts to IEdmModel.
        /// </summary>
        /// <returns>The common AdventureWorks test model as IEdmModel.</returns>
        public static IEdmModel GetAdventureWorksModel()
        {
            return ReadModelFromResources("AdventureWorksPlus.csdl");
        }

        /// <summary>
        /// Reads the model which contains entity type with different type of property
        /// </summary>
        /// <returns></returns>
        public static IEdmModel GetEntityWithDifferentPropertyTypeModel()
        {
            return ReadModelFromResources("EntityWithDifferentPropertyType.csdl");
        }

        /// <summary>
        /// Reads the common test model (based on ExchangeAttachment.csdl) as CSDL from assembly resources,
        /// and converts to IEdmModel.
        /// </summary>
        /// <returns>The ExchangeAttachment test model as IEdmModel.</returns>
        public static IEdmModel GetExchangeAttachmentModel()
        {
            return ReadModelFromResources("ExchangeAttachment.csdl");
        }

        /// <summary>
        /// Reads a resource from the test assembly.
        /// </summary>
        /// <param name="resourceName">The name of the resource to retrieve.</param>
        /// <returns>The resource as a byte array.</returns>
        public static byte[] ReadTestResource(string resourceName)
        {
            var testAssembly = Assembly.GetExecutingAssembly();
            string fullResourceName = testAssembly.GetManifestResourceNames().Single(n => n.EndsWith(resourceName));
            Stream resourceStream = testAssembly.GetManifestResourceStream(fullResourceName);
            var buffer = new byte[resourceStream.Length];
            resourceStream.Read(buffer, 0, (int)resourceStream.Length);
            return buffer;
        }

        /// <summary>
        /// Reads edm model from resource file
        /// </summary>
        /// <param name="modelName">The name of resource file containing the model</param>
        /// <returns>Edm model</returns>
        private static IEdmModel ReadModelFromResources(string modelName)
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            var csdlStream = new MemoryStream(ReadTestResource(modelName));
            bool parseResult = CsdlReader.TryParse(new[] { XmlReader.Create(csdlStream) }, out model, out errors);
            if (!parseResult)
            {
                throw new InvalidOperationException("Failed to load model : " + string.Join(Environment.NewLine, errors.Select(e => e.ErrorMessage)));
            }

            return model;
        }
    }
}
