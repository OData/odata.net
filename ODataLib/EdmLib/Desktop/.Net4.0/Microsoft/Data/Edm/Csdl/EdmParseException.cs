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

namespace Microsoft.Data.Edm.Csdl
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm.Validation;
    using ErrorStrings = Microsoft.Data.Edm.Strings;

    /// <summary>
    /// Exception type representing a failure to parse an EDM document. Carries the set of errors along with it.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1032", Justification = "We do not intend to support serialization of this exception yet, nor does it need the full suite of constructors.")]
    [SuppressMessage("Microsoft.Usage", "CA2237", Justification = "We do not intend to support serialization of this exception yet.")]
    [DebuggerDisplay("{Message}")]
    public class EdmParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmParseException"/> class.
        /// </summary>
        /// <param name="parseErrors">The errors encountered while parsing.</param>
        public EdmParseException(IEnumerable<EdmError> parseErrors)
            : this(parseErrors.ToList())
        {
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="EdmParseException"/> class from being created.
        /// </summary>
        /// <param name="parseErrors">The parse errors.</param>
        private EdmParseException(List<EdmError> parseErrors)
            : base(ConstructMessage(parseErrors))
        {
            this.Errors = new ReadOnlyCollection<EdmError>(parseErrors);
        }

        /// <summary>
        /// Gets the set of errors that were encountered while parsing.
        /// </summary>
        public ReadOnlyCollection<EdmError> Errors { get; private set; }

        /// <summary>
        /// Constructs an appropriate exception message from the set of parsing errors.
        /// </summary>
        /// <param name="parseErrors">The parse errors.</param>
        /// <returns>The exception message.</returns>
        private static string ConstructMessage(IEnumerable<EdmError> parseErrors)
        {
            return ErrorStrings.EdmParseException_ErrorsEncounteredInEdmx(string.Join(Environment.NewLine, parseErrors.Select(p => p.ToString()).ToArray()));
        }
    }
}
