//---------------------------------------------------------------------
// <copyright file="JsonLightTestUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;

    internal static class JsonLightTestUtil
    {
        /// <summary>
        /// Configures a DataServiceContext for JSON Light.
        /// </summary>
        /// <param name="context">Context to be configured.</param>
        /// <param name="edmx">EDMX that contains the service model. If null, the EDMX from the service's $metadata endpoint is used.</param>
        internal static void ConfigureContextForJsonLight(DataServiceContext context, string edmx = null)
        {
            IEdmModel serviceModel;
            if (edmx == null)
            {
                serviceModel = ResolveModelFromMetadataUri(context);
            }
            else
            {
                serviceModel = ResolveModelFromString(edmx);
            }
            
            context.Format.UseJson(serviceModel);
        }

        private static IEdmModel ResolveModelFromMetadataUri(DataServiceContext context)
        {
            var metadataUri = context.GetMetadataUri().AbsoluteUri;
            return CsdlReader.Parse(XmlReader.Create(metadataUri));
        }

        private static IEdmModel ResolveModelFromString(string edmxToParse)
        {
            return CsdlReader.Parse(XmlReader.Create(new StringReader(edmxToParse)));
        }
    }
}
