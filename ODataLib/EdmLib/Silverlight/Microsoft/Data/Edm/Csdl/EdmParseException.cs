//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
