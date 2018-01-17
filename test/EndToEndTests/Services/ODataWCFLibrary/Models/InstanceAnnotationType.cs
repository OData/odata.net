//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using Microsoft.OData;

    [Serializable]
    public abstract class InstanceAnnotationType
    {
        protected InstanceAnnotationType(string ns, string subName, object value, string target)
        {
            this.subname = subName;
            this.ns = ns;
            this.Value = value;
            this.Target = target;
        }

        private string subname;

        private string ns;

        public string Name
        {
            get
            {
                return string.Format("{0}.{1}", this.ns, this.subname);
            }
        }

        public string Target { get; protected set; }

        public object Value { get; protected set; }

        public virtual ODataValue ConvertValueToODataValue()
        {
            ODataValue value = ODataObjectModelConverter.CreateODataValue(this.Value) as ODataValue ?? new ODataPrimitiveValue(this.Value);
            return value;
        }
    }
}