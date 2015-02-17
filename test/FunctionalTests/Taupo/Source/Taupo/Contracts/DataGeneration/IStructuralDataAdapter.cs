//---------------------------------------------------------------------
// <copyright file="IStructuralDataAdapter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using System.Collections.Generic;

    /// <summary>
    /// Adapter that defines storage format and provides manipulation methods over structural 
    /// data in form of key-value pairs where key is a member path and value is a member value.
    /// Member path is a member name or sequence of member names delimited by '.' for nested members.
    /// </summary>
    public interface IStructuralDataAdapter
    {
        /// <summary>
        /// Creates data with the specified members' values.
        /// </summary>
        /// <param name="memberValues">Key-value pairs where Key is a member path and Value is a member value.</param>
        /// <returns>New data with the specified members' values.</returns>
        object CreateData(IEnumerable<NamedValue> memberValues);

        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="data">The data that contains member with the specified path.</param>
        /// <param name="memberPath">The member path.</param>
        /// <returns>Value of the member.</returns>
        TValue GetMemberValue<TValue>(object data, string memberPath);

        /// <summary>
        /// Sets the member value.
        /// </summary>
        /// <param name="data">The data that contains member with the specified path.</param>
        /// <param name="memberPath">The member path.</param>
        /// <param name="value">The value.</param>
        /// <returns>member value set on the data</returns>
        object SetMemberValue(object data, string memberPath, object value);

        /// <summary>
        /// Sets the members values.
        /// </summary>
        /// <param name="data">The data that contains specified members.</param>
        /// <param name="memberValues">Key-value pairs where key is a member path and value is a member value.</param>
        /// <returns>collection of set member values</returns>
        IEnumerable<NamedValue> SetMemberValues(object data, IEnumerable<NamedValue> memberValues);

        /// <summary>
        /// Gets initialized collection member: before returning collection member initializes it if it's null.
        /// </summary>
        /// <param name="data">The data that contains specified collection member.</param>
        /// <param name="memberPath">The collection member path.</param>
        /// <returns>The collection member.</returns>
        object GetInitializedCollection(object data, string memberPath);

        /// <summary>
        /// Adds an element to a collection with the specified member path.
        /// </summary>
        /// <param name="data">The data that contains specified collection member.</param>
        /// <param name="memberPath">The collection member path.</param>
        /// <param name="element">The element to add to collection.</param>
        void AddToCollection(object data, string memberPath, object element);

        /// <summary>
        /// Removes an element from a collection with the specified member path.
        /// </summary>
        /// <param name="data">The data that contains specified collection member.</param>
        /// <param name="memberPath">The collection member path.</param>
        /// <param name="element">The element to remove from collection.</param>
        /// <returns>true if element was successfully removed from the collection; otherwise, false.
        /// This method also returns false if item is not found in the original collection.
        /// </returns>
        bool RemoveFromCollection(object data, string memberPath, object element);
    }
}
