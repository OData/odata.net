//---------------------------------------------------------------------
// <copyright file="WrappedDataServiceStreamResponse.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// this code has been generated using WrapperGenerator
// Do not modify the file, but instead modify and re-run the generator.

// disable warnings about 'new' keyword being required/not needed
#pragma warning disable 108, 109

namespace Microsoft.Test.Taupo.Astoria.Contracts.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Wrappers;
    
    /// <summary>
    /// Wraps the 'Microsoft.OData.Client.DataServiceStreamResponse' type.
    /// </summary>
    public partial class WrappedDataServiceStreamResponse : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedDataServiceStreamResponse class.
        /// </summary>
        static WrappedDataServiceStreamResponse()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.DataServiceStreamResponse", "Microsoft.OData.Client");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedDataServiceStreamResponse class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedDataServiceStreamResponse(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'ContentDisposition' property on 'Microsoft.OData.Client.DataServiceStreamResponse'
        /// </summary>
        public virtual string ContentDisposition
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_ContentDisposition()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'ContentType' property on 'Microsoft.OData.Client.DataServiceStreamResponse'
        /// </summary>
        public virtual string ContentType
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_ContentType()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Headers' property on 'Microsoft.OData.Client.DataServiceStreamResponse'
        /// </summary>
        public virtual System.Collections.Generic.Dictionary<string, string> Headers
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Collections.Generic.Dictionary<string, string>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.Generic.Dictionary`2[System.String,System.String] get_Headers()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Stream' property on 'Microsoft.OData.Client.DataServiceStreamResponse'
        /// </summary>
        public virtual Stream Stream
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<Stream>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IO.Stream get_Stream()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Wraps the 'Void Dispose()' on the 'Microsoft.OData.Client.DataServiceStreamResponse' type.
        /// </summary>
        public virtual void Dispose()
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void Dispose()"), new object[] { });
        }
    }
}
