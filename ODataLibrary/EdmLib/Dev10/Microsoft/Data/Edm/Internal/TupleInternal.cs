//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.Edm.Internal
{
    internal static class TupleInternal
    {
        public static TupleInternal<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new TupleInternal<T1, T2>(item1, item2);
        }
    }

    internal class TupleInternal<T1, T2>
    {
        public T1 Item1
        {
            get; private set;
        }

        public T2 Item2
        {
            get; private set;
        }

        public TupleInternal(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
    }
}
