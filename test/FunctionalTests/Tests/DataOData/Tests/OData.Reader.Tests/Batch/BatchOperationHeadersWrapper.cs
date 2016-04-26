//---------------------------------------------------------------------
// <copyright file="BatchOperationHeadersWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Wrapper of BatchOperationHeadersWrapper to expose the internal API.
    /// </summary>
    public sealed class BatchOperationHeadersWrapper : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// The type of the batch operations headers class in the product code.
        /// </summary>
        private static readonly Type batchOperationsHeadersType = typeof(ODataBatchReader).Assembly.GetType("Microsoft.OData.ODataBatchOperationHeaders");

        /// <summary>
        /// The batch operations headers instance from the product code.
        /// </summary>
        private readonly object batchOperationHeaders;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BatchOperationHeadersWrapper()
        {
            this.batchOperationHeaders = ReflectionUtils.CreateInstance(batchOperationsHeadersType);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchReader">The batch headers.</param>
        public BatchOperationHeadersWrapper(object headers)
        {
            this.batchOperationHeaders = headers;
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        public string this[string key]
        {
            get
            {
                try
                {
                    MethodInfo method = this.batchOperationHeaders.GetType().GetMethod("get_Item");
                    return (string)method.Invoke(this.batchOperationHeaders, new object[] { key });
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            }

            set
            {
                try
                { 
                    MethodInfo method = this.batchOperationHeaders.GetType().GetMethod("set_Item");
                    method.Invoke(this.batchOperationHeaders, new object[] { key, value });
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the dictionary.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(string key, string value)
        {
            ReflectionUtils.InvokeMethod(this.batchOperationHeaders, "Add", key, value);
        }

        /// <summary>
        /// Determines whether the dictionary contains an element with the specified key using case-sensitive comparison.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>true if the dictionary contains an element with the <paramref name="key"/>; otherwise, false.</returns>
        /// <remarks>This method will only try to match the key using case-sensitive comparison.</remarks>
        public bool ContainsKeyOrdinal(string key)
        {
            return (bool)ReflectionUtils.InvokeMethod(this.batchOperationHeaders, "ContainsKeyOrdinal", key);
        }

        /// <summary>
        /// Removes the entry with the specified <paramref name="key"/> from the headers.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        /// <returns>true if the item with the specified <paramref name="key"/> was removed; otherwise false.</returns>
        public bool Remove(string key)
        {
            return (bool)ReflectionUtils.InvokeMethod(this.batchOperationHeaders, "Remove", key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; 
        /// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the dictionary contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(string key, out string value)
        {
            object[] args = new object[2];
            args[0] = key;
            bool result = (bool)ReflectionUtils.InvokeMethod(this.batchOperationHeaders, "TryGetValue", args);
            value = (string)args[1];
            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, string>>)this.batchOperationHeaders).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.batchOperationHeaders).GetEnumerator();
        }
    }
}
