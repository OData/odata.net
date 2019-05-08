//---------------------------------------------------------------------
// <copyright file="EdmJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Edm JSON writer.
    /// </summary>
    internal class EdmJsonWriter : IEdmJsonWriter
    {
        /// <summary>
        /// The text writer.
        /// </summary>
        private readonly TextWriter writer;

        /// <summary>
        /// The text writer.
        /// </summary>
        private readonly CsdlJsonWriterSettings settings;

        /// <summary>
        /// Number which specifies the level of indentation.
        /// </summary>
        private int indentLevel;

        /// <summary>
        /// Scope of the JSON element - object, array, property.
        /// </summary>
        private Stack<Scope> scopes;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmJsonWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="settings">The Csdl settings.</param>
        public EdmJsonWriter(TextWriter textWriter, CsdlJsonWriterSettings settings)
        {
            EdmUtil.CheckArgumentNull(textWriter, "textWriter");
            EdmUtil.CheckArgumentNull(settings, "settings");

            this.writer = textWriter;
            this.settings = settings;
            this.scopes = new Stack<Scope>();
            this.indentLevel = 0;
        }

        /// <summary>
        /// Write the start object.
        /// </summary>
        public virtual void StartObjectScope()
        {
            var currentScope = CurrentScope();
            if (currentScope != null && currentScope.ScopeType == ScopeType.Array)
            {
                if (currentScope.ObjectCount != 0)
                {
                    this.writer.Write(JsonConstants.ArrayElementSeparator);
                }

                WriteNewLine();
                WriteIndentation();
            }

            this.writer.Write(JsonConstants.StartObjectScope);

            StartScope(ScopeType.Object);
        }

        /// <summary>
        /// Write the end object.
        /// </summary>
        public virtual void EndObjectScope()
        {
            var currentScope = EndScope(ScopeType.Object);
            if (currentScope.ObjectCount != 0)
            {
                WriteNewLine();
                DecreaseIndentation();
                WriteIndentation();
            }
            else
            {
                this.writer.Write(JsonConstants.WhiteSpaceForEmptyObject);
                DecreaseIndentation();
            }

            this.writer.Write(JsonConstants.EndObjectScope);
        }

        /// <summary>
        /// Write the start array.
        /// </summary>
        public virtual void StartArrayScope()
        {
            var currentScope = CurrentScope();
            if (currentScope != null && currentScope.ScopeType == ScopeType.Array)
            {
                if (currentScope.ObjectCount != 0)
                {
                    this.writer.Write(JsonConstants.ArrayElementSeparator);
                }

                WriteNewLine();
                WriteIndentation();
            }

            this.writer.Write(JsonConstants.StartArrayScope);
            StartScope(ScopeType.Array);
        }

        /// <summary>
        /// Write the end array.
        /// </summary>
        public virtual void EndArrayScope()
        {
            var current = EndScope(ScopeType.Array);

            if (current.ObjectCount != 0)
            {
                WriteNewLine();
                DecreaseIndentation();
                WriteIndentation();
            }
            else
            {
                this.writer.Write(JsonConstants.WhiteSpaceForEmptyArray);
                DecreaseIndentation();
            }

            this.writer.Write(JsonConstants.EndArrayScope);
        }

        /// <summary>
        /// Write the property name.
        /// </summary>
        public virtual void WritePropertyName(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "The name must be specified.");
            Debug.Assert(this.scopes.Count > 0, "There must be an active scope for name to be written.");
            Debug.Assert(this.scopes.Peek().ScopeType == ScopeType.Object, "The active scope must be an object scope for name to be written.");

            var currentScope = CurrentScope();
            if (currentScope.ObjectCount != 0)
            {
                this.writer.Write(JsonConstants.ObjectMemberSeparator);
            }

            WriteNewLine();
            currentScope.ObjectCount++;
            WriteIndentation();

            this.writer.Write(JsonConstants.QuoteCharacter);
            this.writer.Write(name);
            this.writer.Write(JsonConstants.QuoteCharacter);

            if (settings.Indent)
            {
                this.writer.Write(JsonConstants.NameValueSeparator);
            }
            else
            {
                this.writer.Write(JsonConstants.ColonCharacter);
            }
        }

        /// <summary>
        /// Write null value.
        /// </summary>
        public virtual void WriteNull()
        {
            WriteValueSeparator();
            this.writer.Write(JsonConstants.JsonNullLiteral);
        }

        /// <summary>
        /// Write the string value.
        /// </summary>
        public virtual void WriteValue(string value)
        {
            WriteValueSeparator();
            this.writer.Write(JsonConstants.QuoteCharacter);
            this.writer.Write(value);
            this.writer.Write(JsonConstants.QuoteCharacter);
        }

        /// <summary>
        /// Add a long value to the current json scope.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public virtual  void WriteValue(long value)
        {
            if (this.settings.IsIeee754Compatible)
            {
                WriteValue(value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                WriteValueSeparator();
                this.writer.Write(value);
            }
        }

        /// <summary>
        /// Write the decimal value.
        /// </summary>
        public virtual void WriteValue(decimal value)
        {
            if (this.settings.IsIeee754Compatible)
            {
                WriteValue(value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                WriteValueSeparator();
                this.writer.Write(value);
            }
        }

        /// <summary>
        /// Write the double value.
        /// </summary>
        public virtual void WriteValue(double value)
        {
            WriteValueSeparator();
            this.writer.Write(value);
        }

        /// <summary>
        /// Write the int value.
        /// </summary>
        public virtual void WriteValue(int value)
        {
            WriteValueSeparator();
            this.writer.Write(value);
        }

        /// <summary>
        /// Write the bool value.
        /// </summary>
        public virtual void WriteValue(bool value)
        {
            WriteValueSeparator();
            this.writer.Write(value.ToString().ToLower());
        }

        /// <summary>
        /// Flush the writer.
        /// </summary>
        public virtual void Flush()
        {
            writer.Flush();
        }

        /// <summary>
        /// Writes a separator of a value if it's needed for the next value to be written.
        /// </summary>
        protected virtual void WriteValueSeparator()
        {
            if (this.scopes.Count == 0)
            {
                return;
            }

            var currentScope = this.scopes.Peek();

            if (currentScope.ScopeType == ScopeType.Array)
            {
                if (currentScope.ObjectCount != 0)
                {
                    writer.Write(JsonConstants.ArrayElementSeparator);
                }

                WriteNewLine();
                WriteIndentation();
                currentScope.ObjectCount++;
            }
        }

        /// <summary>
        /// Increases the level of indentation applied to the output.
        /// </summary>
        public virtual void WriteNewLine()
        {
            if (settings.Indent)
            {
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Write the indentation.
        /// </summary>
        public virtual void WriteIndentation()
        {
            if (settings.Indent)
            {
                for (var i = 0; i < this.indentLevel; i++)
                {
                    writer.Write(JsonConstants.IndentationString);
                }
            }
        }

        /// <summary>
        /// Increases the level of indentation applied to the output.
        /// </summary>
        public virtual void IncreaseIndentation()
        {
            if (settings.Indent)
            {
                this.indentLevel++;
            }
        }

        /// <summary>
        /// Decreases the level of indentation applied to the output.
        /// </summary>
        public virtual void DecreaseIndentation()
        {
            if (settings.Indent)
            {
                Debug.Assert(this.indentLevel > 0);
                this.indentLevel--;
            }
        }

        /// <summary>
        /// Get current scope.
        /// </summary>
        /// <returns></returns>
        protected Scope CurrentScope()
        {
            return this.scopes.Count == 0 ? null : this.scopes.Peek();
        }

        /// <summary>
        /// Start the scope given the scope type.
        /// </summary>
        /// <param name="type">The scope type to start.</param>
        protected Scope StartScope(ScopeType type)
        {
            if (this.scopes.Count != 0)
            {
                var currentScope = scopes.Peek();
                currentScope.ObjectCount++;
            }

            IncreaseIndentation();

            var scope = new Scope(type);
            scopes.Push(scope);
            return scope;
        }

        /// <summary>
        /// End the scope of the given scope type.
        /// </summary>
        /// <param name="type">The scope type to end.</param>
        /// <returns>The end scope.</returns>
        protected Scope EndScope(ScopeType type)
        {
            Debug.Assert(this.scopes.Count > 0, "No scope to end.");
            Debug.Assert(scopes.Peek().ScopeType == type, "Ending scope does not match.");

            return scopes.Pop();
        }
    }
}