//---------------------------------------------------------------------
// <copyright file="WrappedEntityInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Wrappers
{
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Wraps the instance of an entity type.
    /// </summary>
    public partial class WrappedEntityInstance : WrappedObject
    {
        /// <summary>
        /// Initializes a new instance of the WrappedEntityInstance class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedEntityInstance(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }

        /// <summary>
        /// Gets the value of a property which is a collection.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Collection wrapper for the value.</returns>
        public WrappedIList<WrappedEntityInstance> GetCollection(string propertyName)
        {
            return this.Scope.Wrap<WrappedIList<WrappedEntityInstance>>(WrapperUtilities.GetPropertyValue(this, propertyName));
        }

        /// <summary>
        /// Gets the value of a property which is another entity instance.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Entity instance wrapper for the returned value.</returns>
        public WrappedEntityInstance GetReference(string propertyName)
        {
            return this.Scope.Wrap<WrappedEntityInstance>(WrapperUtilities.GetPropertyValue(this, propertyName));
        }

        /// <summary>
        /// Gets the value of a scalar property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the scalar property (unwrapped).</returns>
        public object GetScalar(string propertyName)
        {
            return WrapperUtilities.GetPropertyValue(this, propertyName);
        }

        /// <summary>
        /// Sets the value of a collection property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">New property value.</param>
        public void SetCollection(string propertyName, WrappedIList<WrappedEntityInstance> value)
        {
            WrapperUtilities.SetPropertyValue(this, propertyName, value);
        }

        /// <summary>
        /// Sets the value of a reference property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The new value of the property.</param>
        public void SetReference(string propertyName, WrappedEntityInstance value)
        {
            WrapperUtilities.SetPropertyValue(this, propertyName, value);
        }

        /// <summary>
        /// Sets the value of a scalar property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The new property value.</param>
        public void SetScalar(string propertyName, object value)
        {
            WrapperUtilities.SetPropertyValue(this, propertyName, value);
        }
    }
}