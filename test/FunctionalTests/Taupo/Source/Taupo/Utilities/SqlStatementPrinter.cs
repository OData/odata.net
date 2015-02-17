//---------------------------------------------------------------------
// <copyright file="SqlStatementPrinter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.IO;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Implementation of <see cref="ISqlStatementProcessor"/> that prints statements to 
    /// <see cref="TextWriter"/>.
    /// </summary>
    public sealed class SqlStatementPrinter : ISqlStatementProcessor
    {
        private TextWriter output;
        private bool ownsOutput;

        /// <summary>
        /// Initializes a new instance of the SqlStatementPrinter class for a given <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="output">Output text writer.</param>
        /// <remarks>
        /// The text writer will not be owned by the <see cref="SqlStatementPrinter"/> and it's up to the caller
        /// to dispose of it when it's not needed.
        /// </remarks>
        public SqlStatementPrinter(TextWriter output)
            : this(output, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SqlStatementPrinter class for a given <see cref="TextWriter"/>
        /// </summary>
        /// <param name="output">Output text writer.</param>
        /// <param name="ownsOutput">If set to <c>true</c>, the output TextWriter will be owned by this <see cref="SqlStatementPrinter"/> 
        /// and will be disposed of when this instance is disposed..</param>
        public SqlStatementPrinter(TextWriter output, bool ownsOutput)
        {
            this.output = output;
            this.ownsOutput = ownsOutput;
        }

        /// <summary>
        /// Writes statement text to the <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="statementText">Statement text.</param>
        public void ProcessStatement(string statementText)
        {
            this.output.WriteLine(statementText);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.ownsOutput)
            {
                this.output.Dispose();
            }
            
            GC.SuppressFinalize(this);
        }
    }
}
