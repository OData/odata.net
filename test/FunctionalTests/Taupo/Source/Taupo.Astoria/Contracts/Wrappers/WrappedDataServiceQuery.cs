//---------------------------------------------------------------------
// <copyright file="WrappedDataServiceQuery.cs" company="Microsoft">
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
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Wrappers;
    
    /// <summary>
    /// Wraps the 'Microsoft.OData.Client.DataServiceQuery' type.
    /// </summary>
    public partial class WrappedDataServiceQuery : WrappedDataServiceRequest
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        private static readonly IDictionary<string, ConstructorInfo> BeforeEventArgs = new Dictionary<string, ConstructorInfo>();
        private static readonly IDictionary<string, ConstructorInfo> AfterEventArgs = new Dictionary<string, ConstructorInfo>();
        private static readonly IDictionary<string, MethodInfo> BeforeEvents = new Dictionary<string, MethodInfo>();
        private static readonly IDictionary<string, MethodInfo> AfterEvents = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedDataServiceQuery class.
        /// </summary>
        static WrappedDataServiceQuery()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.DataServiceQuery", "Microsoft.OData.Client");
            BeforeEventArgs["System.Collections.IEnumerable Execute()"] = typeof(EventsTracker.BeforeExecuteEventArgs).GetInstanceConstructors(true).First();
            AfterEventArgs["System.Collections.IEnumerable Execute()"] = typeof(EventsTracker.AfterExecuteEventArgs).GetInstanceConstructors(true).First();
            BeforeEvents["System.Collections.IEnumerable Execute()"] = typeof(EventsTracker).GetMethod("RaiseBeforeExecute", null, false);
            AfterEvents["System.Collections.IEnumerable Execute()"] = typeof(EventsTracker).GetMethod("RaiseAfterExecute", null, false);
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedDataServiceQuery class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedDataServiceQuery(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
            this.TrackEvents = new EventsTracker(this);
        }
        
        /// <summary>
        /// Gets a value of the 'Expression' property on 'Microsoft.OData.Client.DataServiceQuery'
        /// </summary>
        public virtual WrappedObject Expression
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Linq.Expressions.Expression get_Expression()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Provider' property on 'Microsoft.OData.Client.DataServiceQuery'
        /// </summary>
        public virtual WrappedIQueryProvider Provider
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedIQueryProvider>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Linq.IQueryProvider get_Provider()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets the value of the EventsTracker class
        /// </summary>
        public EventsTracker TrackEvents { get; private set; }
        
        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginExecute(System.AsyncCallback, System.Object)' on the 'Microsoft.OData.Client.DataServiceQuery' type.
        /// </summary>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginExecute(System.AsyncCallback callback, WrappedObject state)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginExecute(System.AsyncCallback, System.Object)"), new object[] { callback, state });
        }
        
        /// <summary>
        /// Wraps the 'System.Collections.IEnumerable EndExecute(System.IAsyncResult)' on the 'Microsoft.OData.Client.DataServiceQuery' type.
        /// </summary>
        /// <param name="asyncResult">The value of the 'asyncResult' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIEnumerable EndExecute(System.IAsyncResult asyncResult)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerable>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.IEnumerable EndExecute(System.IAsyncResult)"), new object[] { asyncResult });
        }
        
        /// <summary>
        /// Wraps the 'System.Collections.IEnumerable Execute()' on the 'Microsoft.OData.Client.DataServiceQuery' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIEnumerable Execute()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerable>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.IEnumerable Execute()"), new object[] { }, BeforeEvents["System.Collections.IEnumerable Execute()"], AfterEvents["System.Collections.IEnumerable Execute()"], BeforeEventArgs["System.Collections.IEnumerable Execute()"], AfterEventArgs["System.Collections.IEnumerable Execute()"]);
        }
        
        /// <summary>
        /// Initializes a new instance of the EventsTracker class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Type must be visible")]
        public class EventsTracker
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Code is generated")]
            private WrappedDataServiceQuery wrapperType;
            
            /// <summary>
            /// Initializes a new instance of the EventsTracker class.
            /// </summary>
            /// <param name="wrapType">The wrapper type.</param>
            public EventsTracker(WrappedDataServiceQuery wrapType)
            {
                this.wrapperType = wrapType;
            }
            
            /// <summary>
            /// Event for ExecuteBefore.
            /// </summary>
            public event EventHandler<BeforeExecuteEventArgs> ExecuteBeforeEvent;
            
            /// <summary>
            /// Event for ExecuteAfter.
            /// </summary>
            public event EventHandler<AfterExecuteEventArgs> ExecuteAfterEvent;
            
            /// <summary>
            /// Raises the BeforeExecute event.
            /// </summary>
            /// <param name="args">The arguments.</param>
            public void RaiseBeforeExecute(BeforeExecuteEventArgs args)
            {
                if (this.ExecuteBeforeEvent != null)
                {
                    this.ExecuteBeforeEvent(this, args);
                }
            }
            
            /// <summary>
            /// Raises the AfterExecute event.
            /// </summary>
            /// <param name="args">The arguments.</param>
            public void RaiseAfterExecute(AfterExecuteEventArgs args)
            {
                if (this.ExecuteAfterEvent != null)
                {
                    this.ExecuteAfterEvent(this, args);
                }
            }
            
            /// <summary>
            /// Represents the event arguments.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Type must be visible")]
            public class BeforeExecuteEventArgs : EventArgs
            {
                /// <summary>
                /// Initializes a new instance of the BeforeExecuteEventArgs class.
                /// </summary>
                public BeforeExecuteEventArgs()
                {
                }
            }
            
            /// <summary>
            /// Represents the event arguments.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Type must be visible")]
            public class AfterExecuteEventArgs : EventArgs
            {
                /// <summary>
                /// Initializes a new instance of the AfterExecuteEventArgs class.
                /// </summary>
                /// <param name="result">The value of the Result parameter.</param>
                public AfterExecuteEventArgs(WrappedIEnumerable result)
                {
                    this.Result = result;
                }
                
                /// <summary>
                /// Gets the value of Result property.
                /// </summary>
                public WrappedIEnumerable Result { get; private set; }
            }
        }
    }
}
