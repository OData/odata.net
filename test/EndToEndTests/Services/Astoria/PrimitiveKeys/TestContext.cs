//---------------------------------------------------------------------
// <copyright file="TestContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PrimitiveKeysService
{
    using System.Linq;

    public class TestContext
    {
        #region Entity Sets

        public IQueryable<EdmBinary> EdmBinarySet { get { return EdmBinary.GetData().AsQueryable(); } }
        public IQueryable<EdmBoolean> EdmBooleanSet { get { return EdmBoolean.GetData().AsQueryable(); } }
        public IQueryable<EdmByte> EdmByteSet { get { return EdmByte.GetData().AsQueryable(); } }
        public IQueryable<EdmDecimal> EdmDecimalSet { get { return EdmDecimal.GetData().AsQueryable(); } }
        public IQueryable<EdmDouble> EdmDoubleSet { get { return EdmDouble.GetData().AsQueryable(); } }
        public IQueryable<EdmSingle> EdmSingleSet { get { return EdmSingle.GetData().AsQueryable(); } }
        public IQueryable<EdmGuid> EdmGuidSet { get { return EdmGuid.GetData().AsQueryable(); } }
        public IQueryable<EdmInt16> EdmInt16Set { get { return EdmInt16.GetData().AsQueryable(); } }
        public IQueryable<EdmInt32> EdmInt32Set { get { return EdmInt32.GetData().AsQueryable(); } }
        public IQueryable<EdmInt64> EdmInt64Set { get { return EdmInt64.GetData().AsQueryable(); } }
        public IQueryable<EdmString> EdmStringSet { get { return EdmString.GetData().AsQueryable(); } }
        public IQueryable<EdmTime> EdmTimeSet { get { return EdmTime.GetData().AsQueryable(); } }
        public IQueryable<EdmDateTimeOffset> EdmDateTimeOffsetSet { get { return EdmDateTimeOffset.GetData().AsQueryable(); } }

        public IQueryable<Folder> Folders { get { return Folder.GetData().AsQueryable(); } }

        #endregion
    }
}
