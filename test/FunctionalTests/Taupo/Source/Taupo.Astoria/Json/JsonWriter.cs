//---------------------------------------------------------------------
// <copyright file="JsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Json
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Json writer class
    /// </summary>
    public class JsonWriter : IDisposable
    {
        /// <summary>scope of the json text - object, array, etc</summary>
        private readonly Stack<Scope> scopes = new Stack<Scope>();

        /// <summary>
        /// Initializes a new instance of the JsonWriter class
        /// </summary>
        /// <param name="writer">writer to which text needs to be written</param>
        public JsonWriter(TextWriter writer)
        {
            ExceptionUtilities.CheckArgumentNotNull(writer, "writer");
            this.TextWriter = new IndentedTextWriter(writer);
        }

        /// <summary>
        /// Various scope types for Json writer
        /// </summary>
        private enum ScopeType
        {
            /// <summary> array scope </summary>
            Array = 0,

            /// <summary> object scope</summary>
            Object = 1,

            /// <summary> ValueName scope</summary>
            ValueName = 2,
        }

        /// <summary>
        /// Gets the indented writer to write text into 
        /// </summary>
        internal IndentedTextWriter TextWriter { get; private set; }

        /// <summary>
        /// End the current scope
        /// </summary>
        public void EndScope()
        {
            if (this.scopes.Count == 0)
            {
                throw new TaupoInvalidOperationException("No active scope to end.");
            }

            this.TextWriter.WriteLine();
            this.TextWriter.Indent--;

            Scope scope = this.scopes.Pop();
            if (scope.Type == ScopeType.Array)
            {
                this.TextWriter.Write("]");
            }
            else if (scope.Type == ScopeType.Object)
            {
                this.TextWriter.Write("}");
            }
        }

        /// <summary>
        /// Start the array scope
        /// </summary>
        public void StartArrayScope()
        {
            this.StartScope(ScopeType.Array);
        }

        /// <summary>
        /// Start the object scope
        /// </summary>
        public void StartObjectScope()
        {
            this.StartScope(ScopeType.Object);
        }

        /// <summary>
        /// Write the name for the object property
        /// </summary>
        /// <param name="name">name of the object property </param>
        public void WriteName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new TaupoArgumentNullException("name");
            }

            if (this.scopes.Count > 0 /*Has Active scope to write into.*/)
            {
                Scope currentScope = this.scopes.Peek();

                if (currentScope.Type == ScopeType.Object)
                {
                    if (currentScope.ObjectCount != 0)
                    {
                        this.TextWriter.WriteTrimmed(", ");
                        this.TextWriter.WriteLine();
                    }

                    currentScope.ObjectCount++;
                    this.WriteString(name);
                    this.TextWriter.WriteTrimmed(": ");
                }
            }
        }
        
         /// <summary>
        /// Writes a null value.
        /// </summary>
        public void WriteNull()
        {
            this.WriteRaw("null");
        }

        /// <summary>
        /// Writes a string value.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteString(string value)
        {
            if (value == null)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteRaw('"' + JsonUtilities.BuildEscapedJavaScriptString(value) + '"');
            }
        }

        /// <summary>
        /// Writes a raw json string
        /// </summary>
        /// <param name="jsonText">the json string to write</param>
        public void WriteRaw(string jsonText)
        {
            if (this.scopes.Count != 0)
            {
                Scope currentScope = this.scopes.Peek();
                if (currentScope.Type == ScopeType.Array)
                {
                    if (currentScope.ObjectCount != 0)
                    {
                        this.TextWriter.WriteTrimmed(", ");
                        this.TextWriter.WriteLine();
                    }

                    currentScope.ObjectCount++;
                }
            }

            this.TextWriter.Write(jsonText);
        }
        
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Start the scope given the scope type
        /// </summary>
        /// <param name="type">scope type</param>
        private void StartScope(ScopeType type)
        {
            this.StartScope(type, false);
        }

        private void StartScope(ScopeType type, bool succeedsValueName)
        {
            if (this.scopes.Count != 0)
            {
                Scope currentScope = this.scopes.Peek();
                if (currentScope.Type != ScopeType.ValueName)
                {
                    if ((currentScope.Type == ScopeType.Array) &&
                        (currentScope.ObjectCount != 0) && !succeedsValueName)
                    {
                        this.TextWriter.WriteTrimmed(", ");
                        this.TextWriter.WriteLine();
                    }

                    if (type != ScopeType.ValueName)
                    {
                        currentScope.ObjectCount++;
                    }
                }
            }

            Scope scope = new Scope(type);
            this.scopes.Push(scope);

            if (type == ScopeType.Array)
            {
                this.TextWriter.Write("[");
            }
            else if (type == ScopeType.Object)
            {
                this.TextWriter.Write("{");
            }

            this.TextWriter.Indent++;
            if (type != ScopeType.ValueName)
            {
                this.TextWriter.WriteLine();
            }
        }

        /// <summary>
        /// Releases unmanaged and unmanaged resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.scopes.Count == 0)
                {
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Flushes and Disposes the writer 
        /// </summary>
        private void Close()
        {
            this.TextWriter.Flush();
            this.TextWriter.Close();
            this.TextWriter.Dispose();
        }

        /// <summary>Writes the Json text in indented format.</summary>
        /// <remarks>
        /// There are many more methods implemented in previous versions
        /// of this file to handle more type and newline cases.
        /// </remarks>
        internal sealed class IndentedTextWriter : TextWriter
        {
            /// <summary> writer to which Json text needs to be written</summary>
            private TextWriter writer;

            /// <summary> keeps track of the indentLevel</summary>
            private int indentLevel;

            /// <summary> keeps track of pending tabs</summary>
            private bool tabsPending;

            /// <summary> string representation of tab</summary>
            private string tabString;

            /// <summary>
            /// Initializes a new instance of the IndentedTextWriter class over the given text writer
            /// </summary>
            /// <param name="writer">writer which IndentedTextWriter wraps</param>
            public IndentedTextWriter(TextWriter writer)
                : base(CultureInfo.InvariantCulture)
            {
                this.writer = writer;
                this.tabString = "    ";
            }

            /// <summary> Gets the Encoding for the given writer </summary>
            public override Encoding Encoding
            {
                get
                {
                    return this.writer.Encoding;
                }
            }

            /// <summary> Gets the new line character </summary>
            public override string NewLine
            {
                get
                {
                    return this.writer.NewLine;
                }
            }

            /// <summary> Gets or sets the current indent level </summary>
            public int Indent
            {
                get
                {
                    return this.indentLevel;
                }

                set
                {
                    this.indentLevel = value;
                }
            }
            
            /// <summary> Closes the underlying writer</summary>
            public override void Close()
            {
                // This is done to make sure we don't accidently close the underlying stream.
                // Since we don't own the stream, we should never close it.
                this.writer.Close();
            }

            /// <summary> Clears all the buffer of the current writer </summary>
            public override void Flush()
            {
                this.writer.Flush();
            }

            /// <summary>
            /// Writes the given string value to the underlying writer
            /// </summary>
            /// <param name="s">string value to be written</param>
            public override void Write(string s)
            {
                this.OutputTabs();
                this.writer.Write(s);
            }

            /// <summary>
            /// Writes NewLine and sets tab pending to true.
            /// </summary>
            public override void WriteLine()
            {
                this.Write(this.NewLine);
                this.tabsPending = true;
            }

            /// <summary>
            /// Writes the trimmed text if minimizeWhiteSpeace is set to true
            /// </summary>
            /// <param name="text">string value to be written</param>
            public void WriteTrimmed(string text)
            {
                this.Write(text);
            }

            /// <summary> Writes the tabs depending on the indent level </summary>
            private void OutputTabs()
            {
                if (this.tabsPending)
                {
                    for (int i = 0; i < this.indentLevel; i++)
                    {
                        this.writer.Write(this.tabString);
                    }

                    this.tabsPending = false;
                }
            }
        }

        /// <summary>
        /// class representing scope information
        /// </summary>
        private sealed class Scope
        {
            /// <summary> keeps the count of the nested scopes </summary>
            private int objectCount;

            /// <summary> keeps the type of the scope </summary>
            private ScopeType type;

            /// <summary>
            /// Initializes a new instance of the Scope class
            /// </summary>
            /// <param name="type">type of the scope</param>
            public Scope(ScopeType type)
            {
                this.type = type;
            }

            /// <summary>
            /// Gets or sets the object count for this scope
            /// </summary>
            public int ObjectCount
            {
                get
                {
                    return this.objectCount;
                }

                set
                {
                    this.objectCount = value;
                }
            }

            /// <summary>
            /// Gets the scope type for this scope
            /// </summary>
            public ScopeType Type
            {
                get
                {
                    return this.type;
                }
            }
        }
    }
}