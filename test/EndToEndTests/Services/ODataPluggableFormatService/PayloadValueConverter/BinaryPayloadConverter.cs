//---------------------------------------------------------------------
// <copyright file="BinaryPayloadConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PluggableFormat
{
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;

    public class BinaryPayloadConverter : ODataPayloadValueConverter
    {
        public override object ConvertToPayloadValue(object value, IEdmTypeReference edmTypeReference)
        {
            // Return the variation when requesting the property directly only
            if (edmTypeReference == null)
            {
                var bin = value as byte[];
                if (bin != null)
                {
                    return Binary2String(bin);
                }
            }

            return base.ConvertToPayloadValue(value, edmTypeReference);
        }

        private string Binary2String(byte[] data)
        {
            return string.Join("-", data.Select(t => t.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
