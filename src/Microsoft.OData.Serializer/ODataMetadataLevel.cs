using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer;

/// <summary>
/// See: https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_ControllingtheAmountofControlInforma
/// </summary>
public enum ODataMetadataLevel
{
    /// <summary>
    /// See: https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_metadatanoneodatametadatanone
    /// </summary>
    None,
    /// <summary>
    /// See: https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_metadataminimalodatametadataminimal
    /// </summary>
    Minimal,
    /// <summary>
    /// See: https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_metadatafullodatametadatafull
    /// </summary>
    Full
}
