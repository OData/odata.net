using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.Adapters;

public abstract class ODataTypeInfo
{
    public virtual Type Type { get; init; }

    /// <summary>
    /// Gets or sets the EDM type associated with the CLR type
    /// represented by this <see cref="ODataTypeInfo"/>.". This
    /// should be set if the same EDM type is always used for this CLR type.
    /// </summary>
    public virtual IEdmType? EdmType { get; init; }
}
