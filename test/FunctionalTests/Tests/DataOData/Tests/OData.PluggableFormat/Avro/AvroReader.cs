//---------------------------------------------------------------------
// <copyright file="AvroReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Hadoop.Avro.Container;

    internal class AvroReader : IDisposable
    {
        private IAvroReader<object> reader;
        private IEnumerator<object> blockEnumerator;

        public object Current { get; private set; }

        public AvroReader(IAvroReader<object> innerReader)
        {
            this.reader = innerReader;
        }

        public bool MoveNext()
        {
            this.Current = null;
            while (this.Current == null)
            {
                while (this.blockEnumerator == null)
                {
                    if (!this.reader.MoveNext()) return false;
                    this.blockEnumerator = this.reader.Current.Objects.GetEnumerator();
                }

                if (this.blockEnumerator.MoveNext())
                {
                    this.Current = this.blockEnumerator.Current;
                }
                else
                {
                    this.blockEnumerator = null;
                }
            }

            return true;
        }

        public void Dispose()
        {
            if (this.reader != null)
            {
                this.reader.Dispose();
                this.reader = null;
            }
        }
    }
}
#endif