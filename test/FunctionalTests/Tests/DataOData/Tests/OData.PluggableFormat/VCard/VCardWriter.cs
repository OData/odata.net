//---------------------------------------------------------------------
// <copyright file="VCardWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat.VCard
{
    using System;
    using System.IO;
    using System.Text;

    internal class VCardWriter
    {
        private readonly TextWriter writer;

        public VCardWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void WriteStart()
        {
            this.writer.WriteLine(VCardConstant.Begin);
        }

        public void WriteEnd()
        {
            this.writer.WriteLine(VCardConstant.End);
            this.writer.Flush();
        }

        public void WriteItem(string groups, string name, string @params, string value)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
            {
                throw new ApplicationException("Name and value are required");
            }

            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrEmpty(groups))
            {
                builder.Append(groups);
                builder.Append('.');
            }

            builder.Append(name);

            if (!string.IsNullOrEmpty(@params))
            {
                builder.Append(';');
                builder.Append(@params);
            }

            builder.Append(':');
            builder.Append(value);

            this.writer.WriteLine(builder.ToString());
        }
    }
}
