//---------------------------------------------------------------------
// <copyright file="EdmParseException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Edm.Validation;
    using ErrorStrings = Microsoft.OData.Edm.Strings;

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