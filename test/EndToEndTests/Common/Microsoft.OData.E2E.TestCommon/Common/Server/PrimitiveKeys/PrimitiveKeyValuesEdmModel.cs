//-----------------------------------------------------------------------------
// <copyright file="PrimitiveKeyValuesEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.PrimitiveKeys
{
    public class PrimitiveKeyValuesEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<EdmBinary>("EdmBinarySet");
            builder.EntitySet<EdmBoolean>("EdmBooleanSet");
            builder.EntitySet<EdmByte>("EdmByteSet");
            builder.EntitySet<EdmDecimal>("EdmDecimalSet");
            builder.EntitySet<EdmDouble>("EdmDoubleSet");
            builder.EntitySet<EdmSingle>("EdmSingleSet");
            builder.EntitySet<EdmGuid>("EdmGuidSet");
            builder.EntitySet<EdmInt16>("EdmInt16Set");
            builder.EntitySet<EdmInt32>("EdmInt32Set");
            builder.EntitySet<EdmInt64>("EdmInt64Set");
            builder.EntitySet<EdmString>("EdmStringSet");
            builder.EntitySet<EdmTime>("EdmTimeSet");
            builder.EntitySet<EdmDateTimeOffset>("EdmDateTimeOffsetSet");

            return builder.GetEdmModel();
        }
    }
}
