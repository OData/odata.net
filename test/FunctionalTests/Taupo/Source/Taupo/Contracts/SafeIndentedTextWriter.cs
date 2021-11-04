//---------------------------------------------------------------------
// <copyright file="SafeIndentedTextWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.CodeDom.Compiler;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;

    /// <summary>
    /// Provides a way to access the functionality of <see cref="IndentedTextWriter"/>
    /// in partial trust scenarios.
    /// </summary>
    public class SafeIndentedTextWriter : TextWriter
    {
        /// <summary>
        /// Specifies the default tab string.
        /// </summary>
        public const string DefaultTabString = IndentedTextWriter.DefaultTabString;

        private IndentedTextWriterDelegator writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeIndentedTextWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer to wrap.</param>
        public SafeIndentedTextWriter(TextWriter writer) :
            this(writer, DefaultTabString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeIndentedTextWriter"/> class
        /// with a custom string to represent a tab.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="tabValue">The string that represents a tab.</param>
        [SecuritySafeCritical]
        public SafeIndentedTextWriter(TextWriter writer, string tabValue) :
            base(CultureInfo.InvariantCulture)
        {
            this.writer = new IndentedTextWriterDelegator(writer, tabValue, this);
        }

        /// <summary>
        /// Gets the <see cref="T:System.Text.Encoding"/> in which the output is written.
        /// </summary>
        public override Encoding Encoding
        {
            [SecuritySafeCritical]
            get { return this.writer.Encoding; }
        }

        /// <summary>
        /// Gets or sets the number of tabs to indent.
        /// </summary>
        public int Indent
        {
            [SecuritySafeCritical]
            get { return this.writer.Indent; }

            [SecuritySafeCritical]
            set { this.writer.Indent = value; }
        }

        /// <summary>
        /// Gets the <see cref="TextWriter"/> to use. You will not be able to use this property in partial trust.
        /// </summary>
        public TextWriter InnerWriter
        {
            [SecuritySafeCritical]
            get { return this.writer.InnerWriter; }
        }

        /// <summary>
        /// Gets or sets the line terminator string used by the current <see cref="TextWriter"/>.
        /// </summary>
        public override string NewLine
        {
            [SecuritySafeCritical]
            get { return this.writer.NewLine; }

            [SecuritySafeCritical]
            set { this.writer.NewLine = value; }
        }

        /// <summary>
        /// Closes the current writer and releases any system resources associated with the writer.
        /// </summary>
        [SecuritySafeCritical]
        public override void Close()
        {
            this.writer.Close();
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        [SecuritySafeCritical]
        public override void Flush()
        {
            this.writer.Flush();
        }

        /// <summary>
        /// Writes the text representation of a Boolean value to the text stream.
        /// </summary>
        /// <param name="value">The Boolean to write.</param>
        [SecuritySafeCritical]
        public override void Write(bool value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes a character to the text stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        [SecuritySafeCritical]
        public override void Write(char value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes a character array to the text stream.
        /// </summary>
        /// <param name="buffer">The character array to write to the text stream.</param>
        [SecuritySafeCritical]
        public override void Write(char[] buffer)
        {
            this.writer.Write(buffer);
        }

        /// <summary>
        /// Writes a subarray of characters to the text stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">Starting index in the buffer.</param>
        /// <param name="count">The number of characters to write.</param>
        [SecuritySafeCritical]
        public override void Write(char[] buffer, int index, int count)
        {
            this.writer.Write(buffer, index, count);
        }

        /// <summary>
        /// Writes the text representation of a decimal value to the text stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        [SecuritySafeCritical]
        public override void Write(decimal value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte floating-point value to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        [SecuritySafeCritical]
        public override void Write(double value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        [SecuritySafeCritical]
        public override void Write(float value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        [SecuritySafeCritical]
        public override void Write(int value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte signed integer to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        [SecuritySafeCritical]
        public override void Write(long value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes the text representation of an object to the text stream by calling ToString on that object.
        /// </summary>
        /// <param name="value">The object to write.</param>
        [SecuritySafeCritical]
        public override void Write(object value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as <see cref="M:System.string.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg0">An object to write into the formatted string.</param>
        [SecuritySafeCritical]
        public override void Write(string format, object arg0)
        {
            this.writer.Write(format, arg0);
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as <see cref="M:System.string.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg0">An object to write into the formatted string.</param>
        /// <param name="arg1">A second object to write into the formatted string.</param>
        [SecuritySafeCritical]
        public override void Write(string format, object arg0, object arg1)
        {
            this.writer.Write(format, arg0, arg1);
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as <see cref="M:System.string.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg0">An object to write into the formatted string.</param>
        /// <param name="arg1">A second object to write into the formatted string.</param>
        /// <param name="arg2">A third object to write into the formatted string.</param>
        [SecuritySafeCritical]
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            this.writer.Write(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as <see cref="M:System.string.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg">The object array to write into the formatted string.</param>
        [SecuritySafeCritical]
        public override void Write(string format, params object[] arg)
        {
            this.writer.Write(format, arg);
        }

        /// <summary>
        /// Writes a string to the text stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        [SecuritySafeCritical]
        public override void Write(string value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte unsigned integer to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        [SecuritySafeCritical]
        public override void Write(uint value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        [SecuritySafeCritical]
        public override void Write(ulong value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes a line terminator to the text stream.
        /// </summary>
        [SecuritySafeCritical]
        public override void WriteLine()
        {
            this.writer.WriteLine();
        }

        /// <summary>
        /// Writes the text representation of a Boolean followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The Boolean to write.</param>
        [SecuritySafeCritical]
        public override void WriteLine(bool value)
        {
            this.writer.WriteLine(value);
        }

        /// <summary>
        /// Writes a character followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        [SecuritySafeCritical]
        public override void WriteLine(char value)
        {
            this.writer.WriteLine(value);
        }

        /// <summary>
        /// Writes an array of characters followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        [SecuritySafeCritical]
        public override void WriteLine(char[] buffer)
        {
            this.writer.WriteLine(buffer);
        }

        /// <summary>
        /// Writes a subarray of characters followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        /// <param name="index">The index into <paramref name="buffer"/> at which to begin reading.</param>
        /// <param name="count">The maximum number of characters to write.</param>
        [SecuritySafeCritical]
        public override void WriteLine(char[] buffer, int index, int count)
        {
            this.writer.WriteLine(buffer, index, count);
        }

        /// <summary>
        /// Writes the text representation of a decimal value followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        [SecuritySafeCritical]
        public override void WriteLine(decimal value)
        {
            this.writer.WriteLine(value);
        }

        /// <summary>
        /// Writes the text representation of a 8-byte floating-point value followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        [SecuritySafeCritical]
        public override void WriteLine(double value)
        {
            this.writer.WriteLine(value);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        [SecuritySafeCritical]
        public override void WriteLine(float value)
        {
            this.writer.WriteLine(value);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        [SecuritySafeCritical]
        public override void WriteLine(int value)
        {
            this.writer.WriteLine(value);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte signed integer followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        [SecuritySafeCritical]
        public override void WriteLine(long value)
        {
            this.writer.WriteLine(value);
        }

        /// <summary>
        /// Writes the text representation of an object by calling ToString on this object, followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The object to write. If <paramref name="value"/> is null, only the line termination characters are written.</param>
        [SecuritySafeCritical]
        public override void WriteLine(object value)
        {
            this.writer.WriteLine(value);
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.string.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatted string.</param>
        /// <param name="arg0">The object to write into the formatted string.</param>
        [SecuritySafeCritical]
        public override void WriteLine(string format, object arg0)
        {
            this.writer.WriteLine(format, arg0);
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.string.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg0">An object to write into the format string.</param>
        /// <param name="arg1">A second object to write into the format string.</param>
        [SecuritySafeCritical]
        public override void WriteLine(string format, object arg0, object arg1)
        {
            this.writer.WriteLine(format, arg0, arg1);
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.string.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg0">An object to write into the format string.</param>
        /// <param name="arg1">A second object to write into the format string.</param>
        /// <param name="arg2">A third object to write into the format string.</param>
        [SecuritySafeCritical]
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            this.writer.WriteLine(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.string.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg">The object array to write into format string.</param>
        [SecuritySafeCritical]
        public override void WriteLine(string format, params object[] arg)
        {
            this.writer.WriteLine(format, arg);
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value"/> is null, only the line termination characters are written.</param>
        [SecuritySafeCritical]
        public override void WriteLine(string value)
        {
            this.writer.WriteLine(value);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        [SecuritySafeCritical]
        public override void WriteLine(uint value)
        {
            this.writer.WriteLine(value);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        [SecuritySafeCritical]
        public override void WriteLine(ulong value)
        {
            this.writer.WriteLine(value);
        }

        /// <summary>
        /// Writes the specified string to a line without tabs.
        /// </summary>
        /// <param name="value">The string to write.</param>
        [SecuritySafeCritical]
        public void WriteLineNoTabs(string value)
        {
            this.writer.WriteLineNoTabs(value);
        }

        /// <summary>
        /// Outputs the tab string once for each level of indentation according to the
        /// <see cref="Indent"/> property.
        /// </summary>
        [SecuritySafeCritical]
        protected virtual void OutputTabs()
        {
            this.writer.BaseOutputTabs();
        }

        /// <summary>
        /// Provides a way to expose the same protected virtual interface from
        /// <see cref="SafeIndentedTextWriter"/> that <see cref="IndentedTextWriter"/>.
        /// </summary>
        private sealed class IndentedTextWriterDelegator : IndentedTextWriter
        {
            private SafeIndentedTextWriter wrapper;

            /// <summary>
            /// Initializes a new instance of the <see cref="IndentedTextWriterDelegator"/> class.
            /// </summary>
            /// <param name="writer">The writer to wrap.</param>
            /// <param name="tabString">The tab string.</param>
            /// <param name="wrapper">The <see cref="SafeIndentedTextWriter"/> to which the call
            /// to <see cref="OutputTabs"/> is delegated.</param>
            /// <remarks>
            /// This method is safe-critical in order to satisfy the LinkDemand for full trust
            /// when calling the <see cref="IndentedTextWriter"/>'s constructor.
            /// </remarks>
            [SecuritySafeCritical]
            public IndentedTextWriterDelegator(TextWriter writer, string tabString, SafeIndentedTextWriter wrapper) :
                base(writer, tabString)
            {
                this.wrapper = wrapper;
            }

            /// <summary>
            /// Calls the base implementation of <see cref="IndentedTextWriter.OutputTabs"/>.
            /// </summary>
            [SecuritySafeCritical]
            public void BaseOutputTabs()
            {
                base.OutputTabs();
            }

            /// <summary>
            /// Outputs the tab string once for each level of indentation according to the
            /// <see cref="P:System.CodeDom.Compiler.IndentedTextWriter.Indent"/> property.
            /// </summary>
            protected override void OutputTabs()
            {
                this.wrapper.OutputTabs();
            }
        }
    }
}
