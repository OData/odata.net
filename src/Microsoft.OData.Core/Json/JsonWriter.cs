//---------------------------------------------------------------------
// <copyright file="JsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if SPATIAL
namespace Microsoft.Data.Spatial
#else
namespace Microsoft.OData.Core.Json
#endif
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using Microsoft.OData.Edm.Library;
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
        private readonly TextWriterWrapper writer;

        /// <summary>
        /// Scope of the json text - object, array.
        /// </summary>
        private readonly Stack<Scope> scopes;

        /// <summary>
        /// If it is IEEE754Compatible, write quoted string for INT64 and decimal to prevent dota loss;
        /// otherwise keep number without quots.
        /// </summary>
        private readonly bool isIeee754Compatible;

        /// <summary>
        /// Creates a new instance of Json writer.
        /// </summary>
        /// <param name="writer">Writer to which text needs to be written.</param>
        /// <param name="indent">If the output should be indented or not.</param>
        /// <param name="jsonFormat">The json-based format to use when writing.</param>
        /// <param name="isIeee754Compatible">if it is IEEE754Compatible</param>
        internal JsonWriter(TextWriter writer, bool indent, ODataFormat jsonFormat, bool isIeee754Compatible)
        {
            Debug.Assert(jsonFormat == ODataFormat.Json, "Expected a json-based format.");

            if (indent == true)
            {
                this.writer = new IndentedTextWriter(writer);
            }
            else
            {
                this.writer = new NonIndentedTextWriter(writer);
            }

            this.scopes = new Stack<Scope>();
            this.isIeee754Compatible = isIeee754Compatible;
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
            Debug.Assert(this.scopes.Count == 0, "Padding scope can only be the outer most scope.");
            this.StartScope(ScopeType.Padding);
        }

        /// <summary>
        /// End the padding function scope.
        /// </summary>
        public void EndPaddingFunctionScope()
        {
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
            this.StartScope(ScopeType.Object);
        }

        /// <summary>
        /// End the current object scope.
        /// </summary>
        public void EndObjectScope()
        {
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
            this.StartScope(ScopeType.Array);
        }

        /// <summary>
        /// End the current array scope.
        /// </summary>
        public void EndArrayScope()
        {
            Debug.Assert(this.scopes.Count > 0, "No scope to end.");

            this.writer.WriteLine();
            this.writer.DecreaseIndentation();
            Scope scope = this.scopes.Pop();

            Debug.Assert(scope.Type == ScopeType.Array, "Ending scope does not match.");

            this.writer.Write(scope.EndString);
        }

        /// <summary>
        /// Write the name for the object property.
        /// </summary>
        /// <param name="name">Name of the object property.</param>
        public void WriteName(string name)
        {
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
            this.writer.Write(functionName);
        }

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="value">Boolean value to be written.</param>
        public void WriteValue(bool value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write an integer value.
        /// </summary>
        /// <param name="value">Integer value to be written.</param>
        public void WriteValue(int value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a float value.
        /// </summary>
        /// <param name="value">Float value to be written.</param>
        public void WriteValue(float value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a short value.
        /// </summary>
        /// <param name="value">Short value to be written.</param>
        public void WriteValue(short value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a long value.
        /// </summary>
        /// <param name="value">Long value to be written.</param>
        public void WriteValue(long value)
        {
            this.WriteValueSeparator();

            // if it is IEEE754Compatible, write numbers with quots; otherwise, write numbers directly.
            if (isIeee754Compatible)
            {
                JsonValueUtils.WriteValue(this.writer, value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                JsonValueUtils.WriteValue(this.writer, value);
            }
        }

        /// <summary>
        /// Write a double value.
        /// </summary>
        /// <param name="value">Double value to be written.</param>
        public void WriteValue(double value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a Guid value.
        /// </summary>
        /// <param name="value">Guid value to be written.</param>
        public void WriteValue(Guid value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a decimal value
        /// </summary>
        /// <param name="value">Decimal value to be written.</param>
        public void WriteValue(decimal value)
        {
            this.WriteValueSeparator();

            // if it is not IEEE754Compatible, write numbers directly without quots;
            if (isIeee754Compatible)
            {
                JsonValueUtils.WriteValue(this.writer, value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                JsonValueUtils.WriteValue(this.writer, value);
            }
        }

        /// <summary>
        /// Writes a DateTimeOffset value
        /// </summary>
        /// <param name="value">DateTimeOffset value to be written.</param>
        public void WriteValue(DateTimeOffset value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value, ODataJsonDateTimeFormat.ISO8601DateTime);
        }

        /// <summary>
        /// Writes a TimeSpan value
        /// </summary>
        /// <param name="value">TimeSpan value to be written.</param>
        public void WriteValue(TimeSpan value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a Date value
        /// </summary>
        /// <param name="value">Date value to be written.</param>
        public void WriteValue(TimeOfDay value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a Date value
        /// </summary>
        /// <param name="value">Date value to be written.</param>
        public void WriteValue(Date value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a byte value.
        /// </summary>
        /// <param name="value">Byte value to be written.</param>
        public void WriteValue(byte value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write an sbyte value.
        /// </summary>
        /// <param name="value">SByte value to be written.</param>
        public void WriteValue(sbyte value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a string value.
        /// </summary>
        /// <param name="value">String value to be written.</param>
        public void WriteValue(string value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a byte array.
        /// </summary>
        /// <param name="value">Byte array to be written.</param>
        public void WriteValue(byte[] value)
        {
            this.WriteValueSeparator();
            JsonValueUtils.WriteValue(this.writer, value);
        }

        /// <summary>
        /// Write a raw value.
        /// </summary>
        /// <param name="rawValue">Raw value to be written.</param>
        public void WriteRawValue(string rawValue)
        {
            this.WriteValueSeparator();
            this.writer.Write(rawValue);
        }

        /// <summary>
        /// Clears all buffers for the current writer.
        /// </summary>
        public void Flush()
        {
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
            /// What to write at the end of this scope.
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
