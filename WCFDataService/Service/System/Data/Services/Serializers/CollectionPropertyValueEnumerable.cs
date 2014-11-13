//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Serializers
{
    using System;
    using System.Collections;
    using System.Diagnostics;

    /// <summary>Implementation of IEnumerable interface to be used as the value of a collection property
    /// when passed the provider.</summary>
    /// <remarks>This implementation can only be enumerated once and doesn't provide any other interfaces
    /// then the IEnumerable.
    /// The IEnumerator returned by this does not support Reset and implement IDisposable.</remarks>
    internal class CollectionPropertyValueEnumerable : IEnumerable
    {
        /// <summary>The source enumerable this class is wrapping.</summary>
        /// <remarks>This is reset to null once an enumerator was created
        /// and is used to detect possible multiple calls to GetEnumerator.</remarks>
        private IEnumerable sourceEnumerable;

        /// <summary>Constructor.</summary>
        /// <param name="sourceEnumerable">The source enumerable which contains the values to report.</param>
        internal CollectionPropertyValueEnumerable(IEnumerable sourceEnumerable)
        {
            Debug.Assert(sourceEnumerable != null, "sourceEnumerable != null");
            this.sourceEnumerable = sourceEnumerable;
        }

        #region IEnumerable Members

        /// <summary>Returns an enumerator to use for enumerating over the results.</summary>
        /// <returns>Enumerator to use for enumerating over the results.</returns>
        public IEnumerator GetEnumerator()
        {
            if (this.sourceEnumerable == null)
            {
                throw new InvalidOperationException(Strings.CollectionCanOnlyBeEnumeratedOnce);
            }

            IEnumerator enumerator = new CollectionPropertyValueEnumerator(this.sourceEnumerable.GetEnumerator());
            this.sourceEnumerable = null;
            return enumerator;
        }

        #endregion

        /// <summary>Implementation of the IEnumerator interface.</summary>
        /// <remarks>This implementation also implements IDisposable to denote
        /// that callers should call Dispose once done with it.</remarks>
        private class CollectionPropertyValueEnumerator : IEnumerator, IDisposable
        {
            /// <summary>The enumerator to get the values from.</summary>
            private IEnumerator sourceEnumerator;

            /// <summary>Constructor.</summary>
            /// <param name="sourceEnumerator">The source enumerator which is used to get the actual values to report.</param>
            internal CollectionPropertyValueEnumerator(IEnumerator sourceEnumerator)
            {
                Debug.Assert(sourceEnumerator != null, "sourceEnumerator != null");
                this.sourceEnumerator = sourceEnumerator;
            }

            #region IEnumerator Members

            /// <summary>The current result.</summary>
            public object Current
            {
                get 
                {
                    if (this.sourceEnumerator == null)
                    {
                        throw new ObjectDisposedException("CollectionPropertyValueEnumerator");
                    }

                    return this.sourceEnumerator.Current; 
                }
            }

            /// <summary>Moves to the next result.</summary>
            /// <returns>true if next result is available, false if no more results are available.</returns>
            public bool MoveNext()
            {
                if (this.sourceEnumerator == null)
                {
                    throw new ObjectDisposedException("CollectionPropertyValueEnumerator");
                }

                return this.sourceEnumerator.MoveNext();
            }

            /// <summary>Resets the enumeration.</summary>
            /// <remarks>This method is not supported.</remarks>
            public void Reset()
            {
                // Using NotSupportedException - same as for example SQL reader, which throws just NotSupportedException here as well.
                throw new NotSupportedException();
            }

            #endregion

            #region IDisposable Members

            /// <summary>Diposes the enumerator.</summary>
            public void Dispose()
            {
                WebUtil.Dispose(this.sourceEnumerator);
                this.sourceEnumerator = null;
            }

            #endregion
        }
    }
}
