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

#if SPATIAL
namespace Microsoft.Data.Spatial
#else
namespace Microsoft.Data.OData.Json
#endif
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    #endregion Namespaces

    /// <summary>
    /// Writer for the JSON format. http://www.json.org
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This class does not own the underlying stream/writer and thus should never dispose it.")]
    internal sealed class JsonWriter : IJsonWriter
    {
        /// <summary>
        /// Writer to write text into.
        /// </summary>
        private readonly IndentedTextWriter writer;

        /// <summary>
        /// Scope of the json text - object, array.
        /// </summary>
        private readonly Stack<Scope> scopes;

        /// <summary>
        /// If true, all double values will be written so that they either have an 'E' for scientific notation or contain a decimal point.
        /// </summary>
        private readonly bool mustWriteDecimalPointInDoubleValues;

        /// <summary>
        /// Creates a new instance of Json writer.
        /// </summary>
        /// <param name="writer">Writer to which text needs to be written.</param>
        /// <param name="indent">If the output should be indented or not.</param>
        /// <param name="jsonFormat">The json-based format to use when writing.</param>
        internal JsonWriter(TextWriter writer, bool indent, ODataFormat jsonFormat)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonFormat == ODataFormat.Json || jsonFormat == ODataFormat.VerboseJson, "Expected a json-based format.");
            this.writer = new IndentedTextWriter(writer, indent);
            this.scopes = new Stack<Scope>();
            this.mustWriteDecimalPointInDoubleValues = jsonFormat == ODataFormat.Json;
        }

        /// <summary>
        /// Various scope types for Json writer.
        /// </summary>
        internal enum ScopeType
        {
            /// <summary>
            /// Array scope.
            /// </summary>
            Array = 0,

            /// <summary>
            /// Object scope.
            /// </summary>
            Object = 1,

            /// <summary>
            /// JSON padding function scope.
            /// </summary>
            Padding = 2,
        }

        /// <summary>
        /// Start the padding function scope.
        /// </summary>
        public void StartPaddingFunctionScope()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.scopes.Count == 0, "Padding scope can only be the outer most scope.");
            this.StartScope(ScopeType.Padding);
        }

        /// <summary>
        /// End the padding function scope.
        /// </summary>
        public void EndPaddingFunctionScope()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.scopes.Count > 0, "No scope to end.");

            this.writer.WriteLine();
            this.writer.DecreaseIndentation();
            Scope scope = this.scopes.Pop();

            Debug.Assert(scope.Type == ScopeType.Padding, "Ending scope does not match.");

            this.writer.Write(scope.EndString);
        }

        /// <summary>
        /// Start the object scope.
        /// </summary>
        public void StartObjectScope()
        {
            DebugUtils.CheckNoExternalCallers();
            this.StartScope(ScopeType.Object);
        }

        /// <summary>
        /// End the current object scope.
        /// </summary>
        public void EndObjectScope()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.scopes.Count > 0, "No scope to end.");

            this.writer.WriteLine();
            this.writer.DecreaseIndentation();
            Scope scope = this.scopes.Pop();

            Debug.Assert(scope.Type == ScopeType.Object, "Ending scope does not match.");

            this.writer.Write(scope.EndString);
        }

        /// <summary>
        /// Start the array scope.
        /// </summary>
        public void StartArrayScope()
        {
            DebugUtils.CheckNoExternalCallers();
            this.StartScope(ScopeType.Array);
        }

        /// <summary>
        /// End the current array scope.
        /// </summary>
        public void EndArrayScope()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.scopes.Count > 0, "No scope to end.");

            this.writer.WriteLine();
            this.writer.DecreaseIndentation();
            Scope scope = this.scopes.Pop();

            Debug.Assert(scope.Type == ScopeType.Array, "Ending scope does not match.");

            this.writer.Write(scope.EndString);
        }

        /// <summary>
        /// Write the "d" wrapper text.
        /// </summary>
        public void WriteDataWrapper()
        {
            DebugUtils.CheckNoExternalCallers();
            this.writer.Write(JsonConstants.ODataDataWrapper);
        }

        /// <summary>
        /// Write the "results" header for the data array.
        /// </summary>
        public void WriteDataArrayName()
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteName(JsonConstants.ODataResultsName);
        }

        /// <summary>
        /// Write the name for the object property.
        /// </summary>
        /// <param name="name">Name of the object property.</param>
        public void WriteName(string name)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(name), "The name must be specified.");
            Debug.Assert(this.scopes.Count > 0, "There must be an active scope for name to be written.");
            Debug.Assert(this.scopes.Peek().Type == ScopeType.Object, "The active scope must be an object scope for name to be written.");

            Scope currentScope = this.scopes.Peek();
            if (currentScope.ObjectCount != 0)
            {
                this.writer.Write(JsonConstants.ObjectMemberSeparator);
            }

            currentScope.ObjectCount++;

            JsonValueUtils.WriteEscapedJsonString(this.writer, name);
            this.writer.Write(JsonConstants.NameValueSeparator);
        }

        /// <summary>
        /// Writes a function name for JSON padding.
        /// </summary>
        /// <param name="functionName">Name of the padding function to write.</param>
        public void WritePaddingFunctionName(string functionName)
        {
            DebugUtils.CheckNoExternalCallers();
            this.writer.Write(functionName);
        }

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="value">Boolean value to be written.</param>
        public void WriteValue(bool value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write an integer value.
        /// </summary>
        /// <param name="value">Integer value to be written.</param>
        public void WriteValue(int value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a float value.
        /// </summary>
        /// <param name="value">Float value to be written.</param>
        public void WriteValue(float value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a short value.
        /// </summary>
        /// <param name="value">Short value to be written.</param>
        public void WriteValue(short value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a long value.
        /// </summary>
        /// <param name="value">Long value to be written.</param>
        public void WriteValue(long value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a double value.
        /// </summary>
        /// <param name="value">Double value to be written.</param>
        public void WriteValue(double value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value, this.mustWriteDecimalPointInDoubleValues);
        }

        /// <summary>
        /// Write a Guid value.
        /// </summary>
        /// <param name="value">Guid value to be written.</param>
        public void WriteValue(Guid value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a decimal value
        /// </summary>
        /// <param name="value">Decimal value to be written.</param>
        public void WriteValue(decimal value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a DateTime value
        /// </summary>
        /// <param name="value">DateTime value to be written.</param>
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        public void WriteValue(DateTime value, ODataVersion odataVersion)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            if (odataVersion < ODataVersion.V3)
            {
                JsonValueUtils.WriteValue(this.writer, value, ODataJsonDateTimeFormat.ODataDateTime);
            }
            else
            {
                JsonValueUtils.WriteValue(this.writer, value, ODataJsonDateTimeFormat.ISO8601DateTime);
            }
        }

        /// <summary>
        /// Writes a DateTimeOffset value
        /// </summary>
        /// <param name="value">DateTimeOffset value to be written.</param>
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        public void WriteValue(DateTimeOffset value, ODataVersion odataVersion)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            if (odataVersion < ODataVersion.V3)
            {
                JsonValueUtils.WriteValue(this.writer, value, ODataJsonDateTimeFormat.ODataDateTime);
            }
            else
            {
                JsonValueUtils.WriteValue(this.writer, value, ODataJsonDateTimeFormat.ISO8601DateTime);
            }
        }

        /// <summary>
        /// Writes a TimeSpan value
        /// </summary>
        /// <param name="value">TimeSpan value to be written.</param>
        public void WriteValue(TimeSpan value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a byte value.
        /// </summary>
        /// <param name="value">Byte value to be written.</param>
        public void WriteValue(byte value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write an sbyte value.
        /// </summary>
        /// <param name="value">SByte value to be written.</param>
        public void WriteValue(sbyte value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a string value.
        /// </summary>
        /// <param name="value">String value to be written.</param>
        public void WriteValue(string value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Clears all buffers for the current writer.
        /// </summary>
        public void Flush()
        {
            DebugUtils.CheckNoExternalCallers();
            this.writer.Flush();
        }

        /// <summary>
        /// Writes a separator of a value if it's needed for the next value to be written.
        /// </summary>
        public void WriteValueSeparator()
        {
            if (this.scopes.Count == 0)
            {
                return;
            }

            Scope currentScope = this.scopes.Peek();
            if (currentScope.Type == ScopeType.Array)
            {
                if (currentScope.ObjectCount != 0)
                {
                    this.writer.Write(JsonConstants.ArrayElementSeparator);
                }

                currentScope.ObjectCount++;
            }
        }

        /// <summary>
        /// Start the scope given the scope type.
        /// </summary>
        /// <param name="type">The scope type to start.</param>
        public void StartScope(ScopeType type)
        {
            if (this.scopes.Count != 0 && this.scopes.Peek().Type != ScopeType.Padding)
            {
                Scope currentScope = this.scopes.Peek();
                if ((currentScope.Type == ScopeType.Array) &&
                    (currentScope.ObjectCount != 0))
                {
                    this.writer.Write(JsonConstants.ArrayElementSeparator);
                }

                currentScope.ObjectCount++;
            }

            Scope scope = new Scope(type);
            this.scopes.Push(scope);

            this.writer.Write(scope.StartString);
            this.writer.IncreaseIndentation();
            this.writer.WriteLine();
        }

        /// <summary>
        /// Class representing scope information.
        /// </summary>
        private sealed class Scope
        {
            /// <summary>
            /// The type of the scope.
            /// </summary>
            private readonly ScopeType type;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="type">The type of the scope.</param>
            public Scope(ScopeType type)
            {
                this.type = type;
                switch (type)
                {
                    case ScopeType.Array:
                        this.StartString = JsonConstants.StartArrayScope;
                        this.EndString = JsonConstants.EndArrayScope;
                        break;
                    case ScopeType.Object:
                        this.StartString = JsonConstants.StartObjectScope;
                        this.EndString = JsonConstants.EndObjectScope;
                        break;
                    case ScopeType.Padding:
                        this.StartString = JsonConstants.StartPaddingFunctionScope;
                        this.EndString = JsonConstants.EndPaddingFunctionScope;
                        break;
                }
            }

            /// <summary>
            /// What to write at the beginning of this scope.
            /// </summary>
            public string StartString
            {
                get;
                private set;
            }

            /// <summary>
            /// What to write at teh end of this scope.
            /// </summary>
            public string EndString
            {
                get;
                private set;
            }

            /// <summary>
            /// Get/Set the object count for this scope.
            /// </summary>
            public int ObjectCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets the scope type for this scope.
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
