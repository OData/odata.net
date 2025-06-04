﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

/// <summary>
/// See: https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_ControllingtheAmountofControlInforma
/// </summary>
internal enum ODataMetadataLevel
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
