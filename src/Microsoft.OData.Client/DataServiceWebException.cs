//---------------------------------------------------------------------
// <copyright file="DataServiceWebException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using Microsoft.OData;

    /// <summary>
    /// Class to describe errors thrown by transport layer.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "No longer relevant after .NET 4 introduction of SerializeObjectState event and ISafeSerializationData interface.")]
    public class DataServiceTransportException : InvalidOperationException
    {
        private readonly IODataResponseMessage ResponseMessage;
        /// <summary>
        /// Constructs a new instance of DataServiceTransportException.
        /// </summary>
        /// <param name="response">ResponseMessage from the exception so that the error payload can be read.</param>
        /// <param name="innerException">Actual exception that this exception is wrapping.</param>
        public DataServiceTransportException(IODataResponseMessage response, Exception innerException)
            : base(innerException.Message, innerException)
        {
            Util.CheckArgumentNull(innerException, "innerException");

            this.ResponseMessage = response;

        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected DataServiceTransportException(SerializationInfo info, StreamingContext context)
   : base(info, context)
        {
            this.ResponseMessage = (IODataResponseMessage)info.GetValue("ResponseMessage", typeof(IODataResponseMessage));
        }

        /// <summary>
        /// Gets the response message for this exception.
        /// </summary>
        public IODataResponseMessage Response
        {
            get { return this.ResponseMessage; }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            info.AddValue("ResponseMessage", this.ResponseMessage, typeof(IODataResponseMessage));
            // MUST call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }


    }
}