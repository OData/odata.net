using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Evaluation
{
    /// <summary>
    /// Interface for ODataMetaDataBuilder factory. This is created for using dependency injection to allow for customization for 
    /// </summary>
    interface IODataMetadataBuilderFactory
    {
        ODataResourceMetadataBuilder CreateEntityMetadataBuilder(IODataResourceMetadataContext resourceMetadataContext, IODataMetadataContext metadataContext, ODataUriBuilder uriBuilder);

        ODataResourceMetadataBuilder CreateResourceMetadataBuilder(IODataResourceMetadataContext resourceMetadataContext, IODataMetadataContext metadataContext, ODataUriBuilder uriBuilder);

        ODataResourceMetadataBuilder CreateNoOpResourceMetadataBuilder(ODataResourceBase resource);
    }
}
