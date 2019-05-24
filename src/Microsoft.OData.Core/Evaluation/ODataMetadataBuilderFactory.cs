using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Evaluation
{
    internal class ODataMetadataBuilderFactory : IODataMetadataBuilderFactory
    {
        public ODataResourceMetadataBuilder CreateEntityMetadataBuilder(IODataResourceMetadataContext resourceMetadataContext, IODataMetadataContext metadataContext, ODataUriBuilder uriBuilder)
        {
            return new ODataConventionalEntityMetadataBuilder(resourceMetadataContext, metadataContext, uriBuilder);
        }

        public ODataResourceMetadataBuilder CreateNoOpResourceMetadataBuilder(ODataResourceBase resource)
        {
            return new NoOpResourceMetadataBuilder(resource);
        }

        public ODataResourceMetadataBuilder CreateResourceMetadataBuilder(IODataResourceMetadataContext resourceMetadataContext, IODataMetadataContext metadataContext, ODataUriBuilder uriBuilder)
        {
            return new ODataConventionalResourceMetadataBuilder(resourceMetadataContext, metadataContext, uriBuilder);
        }
    }
}
