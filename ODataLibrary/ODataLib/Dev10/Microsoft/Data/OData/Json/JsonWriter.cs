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

#if SPATIAL
namespace System.Data.Spatial.Json
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
    internal sealed class JsonWriter
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
        /// Creates a new instance of Json writer.
        /// </summary>
        /// <param name="writer">Writer to which text needs to be written.</param>
        /// <param name="indent">If the output should be indented or not.</param>
        internal JsonWriter(TextWriter writer, bool indent)
        {
            DebugUtils.CheckNoExternalCallers();
            this.writer = new IndentedTextWriter(writer, indent);
            this.scopes = new Stack<Scope>();
        }

        /// <summary>
        /// Various scope types for Json writer.
        /// </summary>
        private enum ScopeType
        {
            /// <summary>
            /// Array scope.
            /// </summary>
            Array = 0,

            /// <summary>
            /// Object scope.
            /// </summary>
            Object = 1
        }

        /// <summary>
        /// Start the object scope.
        /// </summary>
        internal void StartObjectScope()
        {
            DebugUtils.CheckNoExternalCallers();
            this.StartScope(ScopeType.Object);
        }

        /// <summary>
        /// End the current object scope.
        /// </summary>
        internal void EndObjectScope()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.scopes.Count > 0, "No scope to end.");

            this.writer.WriteLine();
            this.writer.DecreaseIndentation();

#if DEBUG
            Scope scope = this.scopes.Pop();
            Debug.Assert(scope.Type == ScopeType.Object, "Calling EndArrayScope when the current active scope is an object.");
#else
            this.scopes.Pop();
#endif
            this.writer.Write(JsonConstants.EndObjectScope);
        }

        /// <summary>
        /// Start the array scope.
        /// </summary>
        internal void StartArrayScope()
        {
            DebugUtils.CheckNoExternalCallers();
            this.StartScope(ScopeType.Array);
        }

        /// <summary>
        /// End the current array scope.
        /// </summary>
        internal void EndArrayScope()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.scopes.Count > 0, "No scope to end.");

            this.writer.WriteLine();
            this.writer.DecreaseIndentation();

#if DEBUG
            Scope scope = this.scopes.Pop();
            Debug.Assert(scope.Type == ScopeType.Array, "Calling EndArrayScope when the current active scope is an object.");
#else
            this.scopes.Pop();
#endif
            this.writer.Write(JsonConstants.EndArrayScope);
        }

        /// <summary>
        /// Write the "d" wrapper text.
        /// </summary>
        internal void WriteDataWrapper()
        {
            DebugUtils.CheckNoExternalCallers();
            this.writer.Write(JsonConstants.ODataDataWrapper);
        }

        /// <summary>
        /// Write the "results" header for the data array.
        /// </summary>
        internal void WriteDataArrayName()
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteName(JsonConstants.ODataResultsName);
        }

        /// <summary>
        /// Write the name for the object property.
        /// </summary>
        /// <param name="name">Name of the object property.</param>
        internal void WriteName(string name)
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
        /// Write a boolean value.
        /// </summary>
        /// <param name="value">Boolean value to be written.</param>
        internal void WriteValue(bool value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write an integer value.
        /// </summary>
        /// <param name="value">Integer value to be written.</param>
        internal void WriteValue(int value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a float value.
        /// </summary>
        /// <param name="value">Float value to be written.</param>
        internal void WriteValue(float value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a short value.
        /// </summary>
        /// <param name="value">Short value to be written.</param>
        internal void WriteValue(short value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a long value.
        /// </summary>
        /// <param name="value">Long value to be written.</param>
        internal void WriteValue(long value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a double value.
        /// </summary>
        /// <param name="value">Double value to be written.</param>
        internal void WriteValue(double value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a Guid value.
        /// </summary>
        /// <param name="value">Guid value to be written.</param>
        internal void WriteValue(Guid value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a decimal value
        /// </summary>
        /// <param name="value">Decimal value to be written.</param>
        internal void WriteValue(decimal value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a DateTime value
        /// </summary>
        /// <param name="value">DateTime value to be written.</param>
        internal void WriteValue(DateTime value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Writes a DateTimeOffset value
        /// </summary>
        /// <param name="value">DateTimeOffset value to be written.</param>
        internal void WriteValue(DateTimeOffset value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Writes a TimeSpan value
        /// </summary>
        /// <param name="value">TimeSpan value to be written.</param>
        internal void WriteValue(TimeSpan value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a byte value.
        /// </summary>
        /// <param name="value">Byte value to be written.</param>
        internal void WriteValue(byte value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write an sbyte value.
        /// </summary>
        /// <param name="value">SByte value to be written.</param>
        internal void WriteValue(sbyte value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a string value.
        /// </summary>
        /// <param name="value">String value to be written.</param>
        internal void WriteValue(string value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

#if !SPATIAL
        /// <summary>
        /// Try to write the value using a registered primitive type converter, if one is registered for this type.
        /// </summary>
        /// <param name="value">Value to be written.</param>
        /// <returns>True if the value was written with a registered converter, otherwise false.</returns>
        internal bool TryWriteValueWithConverter(object value)
        {
            DebugUtils.CheckNoExternalCallers();
            return WriterUtils.PrimitiveConverter.TryWriteJson(value, this.writer);
        }
#endif

        /// <summary>
        /// Clears all buffers for the current writer.
        /// </summary>
        internal void Flush()
        {
            DebugUtils.CheckNoExternalCallers();
            this.writer.Flush();
        }

        /// <summary>
        /// Writes a separator of a value if it's needed for the next value to be written.
        /// </summary>
        private void WriteValueSeparator()
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
        private void StartScope(ScopeType type)
        {
            if (this.scopes.Count != 0)
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

            this.writer.Write(type == ScopeType.Array ? JsonConstants.StartArrayScope : JsonConstants.StartObjectScope);
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
