//---------------------------------------------------------------------
// <copyright file="DataComparisonException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Text;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Exception raised wherever there's data miscompare in the test.
    /// </summary>
    [Serializable]
    public class DataComparisonException : TestFailedException
    {
        /// <summary>
        /// Initializes a new instance of the DataComparisonException class without a message.
        /// </summary>
        public DataComparisonException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataComparisonException class with a given message.
        /// </summary>
        /// <param name="message">Exception message</param>
        public DataComparisonException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataComparisonException class with a given message and inner exception.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception.</param>
        public DataComparisonException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataComparisonException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected DataComparisonException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context)
        {
            //
            // we serialize expected and actual values as strings
            // they are mostly used for presentation
            //
            // change that to use typed objects when needed
            //
            this.ExpectedValue = info.GetString("ExpectedValue");
            this.ActualValue = info.GetString("ActualValue");
        }

        /// <summary>
        /// Gets or sets actual value.
        /// </summary>
        public object ActualValue { get; set; }

        /// <summary>
        /// Gets or sets expected value.
        /// </summary>
        public object ExpectedValue { get; set; }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <value></value>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        public override string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(base.Message);
                if (this.ExpectedValue != null || this.ActualValue != null)
                {
                    sb.AppendLine();
                    sb.Append("Expected: ").Append(this.ExpectedValue).AppendLine();
                    sb.Append("Actual:   ").Append(this.ActualValue);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Adds exception data to <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ExpectedValue", ToStringConverter.ConvertObjectToString(this.ExpectedValue));
            info.AddValue("ActualValue", ToStringConverter.ConvertObjectToString(this.ActualValue));
        }
    }
}