//---------------------------------------------------------------------
// <copyright file="BinaryPayloadConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using System.Globalization;

namespace Microsoft.OData.E2E.TestCommon.Converters
{
    public class BinaryPayloadConverter : ODataPayloadValueConverter
    {
        public override object ConvertToPayloadValue(object value, IEdmTypeReference edmTypeReference)
        {
            // Return the variation when requesting the property directly only.
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
