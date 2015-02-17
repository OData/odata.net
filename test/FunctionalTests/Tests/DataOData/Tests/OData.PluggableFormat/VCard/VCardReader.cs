//---------------------------------------------------------------------
// <copyright file="VCardReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat.VCard
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal enum VCardReaderState
    {
        None,
        Begin,
        End,
        Item,
        Error
    }

    internal class VCardReader
    {
        private readonly TextReader reader;

        public VCardReaderState State { get; private set; }

        public string Groups { get; private set; }
        public string Name { get; private set; }
        public string Params { get; private set; }
        public string Value { get; private set; }

        public VCardReader(TextReader reader)
        {
            this.State = VCardReaderState.None;
            this.reader = reader;
        }

        public bool Read()
        {
            this.Groups = null;
            this.Name = null;
            this.Params = null;
            this.Value = null;
            this.State = VCardReaderState.None;

            string line = this.reader.ReadLine();
            if (line == null)
            {
                this.State = VCardReaderState.End;
                return false;
            }

            if (CompareIgnoreCase(VCardConstant.Begin, line))
            {
                this.State = VCardReaderState.Begin;
            }
            else if (CompareIgnoreCase(VCardConstant.End, line))
            {
                this.State = VCardReaderState.End;
            }
            else
            {
                // [groups.]name[;param;parama;]:value
                // Not support folding now
                // need to care about when those delimers appear at the end.
                string p1, p2;

                if (!this.TrySplitByChar(line, ':', out p1, out p2))
                {
                    this.State = VCardReaderState.Error;
                }
                else
                {
                    this.Value = p2;
                    line = p1;

                    if (this.TrySplitByChar(line, '.', out p1, out p2, true))
                    {
                        this.Groups = p1;
                        line = p2;
                    }

                    if (this.TrySplitByChar(line, ';', out p1, out p2))
                    {
                        this.Params = p2;
                        line = p1;
                    }

                    this.Name = line;
                }

                this.State = VCardReaderState.Item;
            }

            Debug.Assert(this.State != VCardReaderState.None);

            return true;
        }

        private static bool CompareIgnoreCase(string strA, string strB)
        {
            return string.Equals(strA, strB, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool TrySplitByChar(string input, char seperator, out string p1, out string p2, bool reverse = false)
        {
            int index = reverse ? input.LastIndexOf(seperator) : input.IndexOf(seperator);
            if (index < 0)
            {
                p1 = null;
                p2 = null;
                return false;
            }

            p1 = input.Substring(0, index);
            p2 = input.Substring(index + 1);
            return true;
        }
    }
}
