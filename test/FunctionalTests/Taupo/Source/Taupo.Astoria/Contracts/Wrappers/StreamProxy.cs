//---------------------------------------------------------------------
// <copyright file="StreamProxy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// this code has been generated using WrapperGenerator
// Do not modify the file, but instead modify and re-run the generator.

// disable warnings about 'new' keyword being required/not needed and for obsolete classes
#pragma warning disable 108, 109, 618

namespace Microsoft.Test.Taupo.Astoria.Contracts.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Wrappers;
    
    /// <summary>
    /// Wraps the 'System.IO.Stream' type.
    /// </summary>
    public partial class StreamProxy : System.IO.Stream, IProxyObject, IDisposable
    {
        private static readonly IDictionary<string, Type> TypeCache = new Dictionary<string, Type>();
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        private System.IO.Stream underlyingImplementation;
        
        /// <summary>
        /// Initializes a new instance of the StreamProxy class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="underlyingImplementation">The underlying implementation of the proxy.</param>
        public StreamProxy(IWrapperScope wrapperScope, System.IO.Stream underlyingImplementation)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(wrapperScope, "wrapperScope");
            ExceptionUtilities.CheckArgumentNotNull(underlyingImplementation, "underlyingImplementation");
            
            this.Scope = wrapperScope;
            this.underlyingImplementation = underlyingImplementation;
        }
        
        /// <summary>
        /// Gets the wrapper scope.
        /// </summary>
        public IWrapperScope Scope { get; private set; }
        
        /// <summary>
        /// Gets the product instance wrapped by this wrapper.
        /// </summary>
        public object Product
        {
            get { return this.underlyingImplementation; }
        }
        
        /// <summary>
        /// Gets a value indicating whether CanRead on 'System.IO.Stream' is set to true
        /// </summary>
        public override bool CanRead
        {
            get
            {
                var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Boolean get_CanRead()");
                return WrapperUtilities.InvokeMethodAndCast<bool>(this, methodInfo, new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether CanSeek on 'System.IO.Stream' is set to true
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Boolean get_CanSeek()");
                return WrapperUtilities.InvokeMethodAndCast<bool>(this, methodInfo, new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether CanTimeout on 'System.IO.Stream' is set to true
        /// </summary>
        public override bool CanTimeout
        {
            get
            {
                var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Boolean get_CanTimeout()");
                return WrapperUtilities.InvokeMethodAndCast<bool>(this, methodInfo, new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether CanWrite on 'System.IO.Stream' is set to true
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Boolean get_CanWrite()");
                return WrapperUtilities.InvokeMethodAndCast<bool>(this, methodInfo, new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Length' property on 'System.IO.Stream'
        /// </summary>
        public override long Length
        {
            get
            {
                var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Int64 get_Length()");
                return WrapperUtilities.InvokeMethodAndCast<long>(this, methodInfo, new object[] { });
            }
        }
        
        /// <summary>
        /// Gets or sets a value of the 'Position' property on 'System.IO.Stream'
        /// </summary>
        public override long Position
        {
            get
            {
                var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Int64 get_Position()");
                return WrapperUtilities.InvokeMethodAndCast<long>(this, methodInfo, new object[] { });
            }
            
            set
            {
                var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Void set_Position(Int64)");
                WrapperUtilities.InvokeMethodWithoutResult(this, methodInfo, new object[] { value });
            }
        }
        
        /// <summary>
        /// Gets or sets a value of the 'ReadTimeout' property on 'System.IO.Stream'
        /// </summary>
        public override int ReadTimeout
        {
            get
            {
                var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Int32 get_ReadTimeout()");
                return WrapperUtilities.InvokeMethodAndCast<int>(this, methodInfo, new object[] { });
            }
            
            set
            {
                var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Void set_ReadTimeout(Int32)");
                WrapperUtilities.InvokeMethodWithoutResult(this, methodInfo, new object[] { value });
            }
        }
        
        /// <summary>
        /// Gets or sets a value of the 'WriteTimeout' property on 'System.IO.Stream'
        /// </summary>
        public override int WriteTimeout
        {
            get
            {
                var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Int32 get_WriteTimeout()");
                return WrapperUtilities.InvokeMethodAndCast<int>(this, methodInfo, new object[] { });
            }
            
            set
            {
                var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Void set_WriteTimeout(Int32)");
                WrapperUtilities.InvokeMethodWithoutResult(this, methodInfo, new object[] { value });
            }
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginRead(Byte[], Int32, Int32, System.AsyncCallback, System.Object)' on the 'System.IO.Stream' type.
        /// </summary>
        /// <param name="buffer">The value of the 'buffer' parameter.</param>
        /// <param name="offset">The value of the 'offset' parameter.</param>
        /// <param name="count">The value of the 'count' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public override System.IAsyncResult BeginRead(byte[] buffer, int offset, int count, System.AsyncCallback callback, object state)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "System.IAsyncResult BeginRead(Byte[], Int32, Int32, System.AsyncCallback, System.Object)");
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, methodInfo, new object[] { buffer, offset, count, callback, state });
        }
        
        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginWrite(Byte[], Int32, Int32, System.AsyncCallback, System.Object)' on the 'System.IO.Stream' type.
        /// </summary>
        /// <param name="buffer">The value of the 'buffer' parameter.</param>
        /// <param name="offset">The value of the 'offset' parameter.</param>
        /// <param name="count">The value of the 'count' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public override System.IAsyncResult BeginWrite(byte[] buffer, int offset, int count, System.AsyncCallback callback, object state)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "System.IAsyncResult BeginWrite(Byte[], Int32, Int32, System.AsyncCallback, System.Object)");
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, methodInfo, new object[] { buffer, offset, count, callback, state });
        }

        /// <summary>
        /// Wraps the 'Void Close()' on the 'System.IO.Stream' type.
        /// </summary>
        public override void Close()
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Void Close()");
            WrapperUtilities.InvokeMethodWithoutResult(this, methodInfo, new object[] { });
            this.Dispose(true);
        }
        
        /// <summary>
        /// Wraps the 'Int32 EndRead(System.IAsyncResult)' on the 'System.IO.Stream' type.
        /// </summary>
        /// <param name="asyncResult">The value of the 'asyncResult' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public override int EndRead(System.IAsyncResult asyncResult)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Int32 EndRead(System.IAsyncResult)");
            return WrapperUtilities.InvokeMethodAndCast<int>(this, methodInfo, new object[] { asyncResult });
        }
        
        /// <summary>
        /// Wraps the 'Void EndWrite(System.IAsyncResult)' on the 'System.IO.Stream' type.
        /// </summary>
        /// <param name="asyncResult">The value of the 'asyncResult' parameter.</param>
        public override void EndWrite(System.IAsyncResult asyncResult)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Void EndWrite(System.IAsyncResult)");
            WrapperUtilities.InvokeMethodWithoutResult(this, methodInfo, new object[] { asyncResult });
        }

        /// <summary>
        /// Wraps the 'Boolean Equals(System.Object)' on the 'System.IO.Stream' type.
        /// </summary>
        /// <param name="obj">The value of the 'obj' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public override bool Equals(object obj)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.Object", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Boolean Equals(System.Object)");
            return WrapperUtilities.InvokeMethodAndCast<bool>(this, methodInfo, new object[] { obj });
        }
        
        /// <summary>
        /// Wraps the 'Void Flush()' on the 'System.IO.Stream' type.
        /// </summary>
        public override void Flush()
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Void Flush()");
            WrapperUtilities.InvokeMethodWithoutResult(this, methodInfo, new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Int32 GetHashCode()' on the 'System.IO.Stream' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public override int GetHashCode()
        {
            var type = WrapperUtilities.GetTypeFromCache("System.Object", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Int32 GetHashCode()");
            return WrapperUtilities.InvokeMethodAndCast<int>(this, methodInfo, new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Int32 Read(Byte[], Int32, Int32)' on the 'System.IO.Stream' type.
        /// </summary>
        /// <param name="buffer">The value of the 'buffer' parameter.</param>
        /// <param name="offset">The value of the 'offset' parameter.</param>
        /// <param name="count">The value of the 'count' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Int32 Read(Byte[], Int32, Int32)");
            return WrapperUtilities.InvokeMethodAndCast<int>(this, methodInfo, new object[] { buffer, offset, count });
        }
        
        /// <summary>
        /// Wraps the 'Int32 ReadByte()' on the 'System.IO.Stream' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public override int ReadByte()
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Int32 ReadByte()");
            return WrapperUtilities.InvokeMethodAndCast<int>(this, methodInfo, new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Int64 Seek(Int64, System.IO.SeekOrigin)' on the 'System.IO.Stream' type.
        /// </summary>
        /// <param name="offset">The value of the 'offset' parameter.</param>
        /// <param name="origin">The value of the 'origin' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Int64 Seek(Int64, System.IO.SeekOrigin)");
            return WrapperUtilities.InvokeMethodAndCast<long>(this, methodInfo, new object[] { offset, origin });
        }
        
        /// <summary>
        /// Wraps the 'Void SetLength(Int64)' on the 'System.IO.Stream' type.
        /// </summary>
        /// <param name="value">The value of the 'value' parameter.</param>
        public override void SetLength(long value)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Void SetLength(Int64)");
            WrapperUtilities.InvokeMethodWithoutResult(this, methodInfo, new object[] { value });
        }
        
        /// <summary>
        /// Wraps the 'System.String ToString()' on the 'System.IO.Stream' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public override string ToString()
        {
            var type = WrapperUtilities.GetTypeFromCache("System.Object", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "System.String ToString()");
            return WrapperUtilities.InvokeMethodAndCast<string>(this, methodInfo, new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Void Write(Byte[], Int32, Int32)' on the 'System.IO.Stream' type.
        /// </summary>
        /// <param name="buffer">The value of the 'buffer' parameter.</param>
        /// <param name="offset">The value of the 'offset' parameter.</param>
        /// <param name="count">The value of the 'count' parameter.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Void Write(Byte[], Int32, Int32)");
            WrapperUtilities.InvokeMethodWithoutResult(this, methodInfo, new object[] { buffer, offset, count });
        }
        
        /// <summary>
        /// Wraps the 'Void WriteByte(Byte)' on the 'System.IO.Stream' type.
        /// </summary>
        /// <param name="value">The value of the 'value' parameter.</param>
        public override void WriteByte(byte value)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.IO.Stream", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Void WriteByte(Byte)");
            WrapperUtilities.InvokeMethodWithoutResult(this, methodInfo, new object[] { value });
        }
        
        /// <summary>
        /// Disposes the wrapped instance if it implements IDisposable.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Disposes the wrapped instance if it implements IDisposable.
        /// </summary>
        /// <param name="disposing">Whether or not to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            CallOrderUtilities.TryWrapArbitraryMethodCall(
                () => this.Dispose(),
                () =>
                    {
                        var d = this.underlyingImplementation as IDisposable;
                        if (d != null)
                        {
                            d.Dispose();
                            this.underlyingImplementation = null;
                        }
                    });
            base.Dispose(true);
        }
    }
}
