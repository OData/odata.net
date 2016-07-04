//---------------------------------------------------------------------
// <copyright file="DuplicatePropertyNameChecker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData
{
    /// <summary>
    /// Validates that
    ///   1) No duplicate property.
    ///   2) No duplicate "@odata.associationLink" on a property.
    ///   3) "@odata.associationLink"s are put on navigation properties.
    /// </summary>
    internal class DuplicatePropertyNameChecker : IDuplicatePropertyNameChecker
    {
        /// <summary>
        /// Caches property processing state.
        /// </summary>
        private IDictionary<string, State> propertyState = new Dictionary<string, State>();

        /// <summary>
        /// Property processing state.
        /// </summary>
        [Flags]
        private enum State
        {
            /// <summary>
            /// `ValidatePropertyUniqueness(ODataProperty)` has been called.
            /// </summary>
            NonNestedResource,

            /// <summary>
            /// `ValidatePropertyUniqueness(ODataNestedResourceInfo)` has been called.
            /// </summary>
            NestedResource,

            /// <summary>
            /// `ValidatePropertyOpenForAssociationLink` has been called.
            /// </summary>
            AssociationLink
        }

        /// <summary>
        /// Validates property uniqueness.
        /// </summary>
        /// <param name="property">The property.</param>
        public void ValidatePropertyUniqueness(ODataProperty property)
        {
            try
            {
                propertyState.Add(property.Name, State.NonNestedResource);
            }
            catch (ArgumentException)
            {
                throw new ODataException(
                    Strings.DuplicatePropertyNamesNotAllowed(
                        property.Name));
            }
        }

        /// <summary>
        /// Validates property uniqueness.
        /// </summary>
        /// <param name="property">The property.</param>
        public void ValidatePropertyUniqueness(ODataNestedResourceInfo property)
        {
            State state;
            if (!propertyState.TryGetValue(property.Name, out state))
            {
                propertyState[property.Name] = State.NestedResource;
            }
            else
            {
                if (state != State.AssociationLink)
                {
                    throw new ODataException(
                        Strings.DuplicatePropertyNamesNotAllowed(
                            property.Name));
                }
                else
                {
                    propertyState[property.Name] = State.AssociationLink | State.NestedResource;
                }
            }
        }

        /// <summary>
        /// Validates that "@odata.associationLink" is put on a navigation property,
        /// and that no duplicate exists.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void ValidatePropertyOpenForAssociationLink(string propertyName)
        {
            State state;
            if (!propertyState.TryGetValue(propertyName, out state))
            {
                propertyState[propertyName] = State.AssociationLink;
            }
            else
            {
                if (state != State.NestedResource)
                {
                    throw new ODataException(
                        Strings.DuplicatePropertyNamesNotAllowed(
                            propertyName));
                }
                else
                {
                    propertyState[propertyName] = State.NestedResource | State.AssociationLink;
                }
            }
        }

        /// <summary>
        /// Resets to initial state for reuse.
        /// </summary>
        public void Reset()
        {
            propertyState.Clear();
        }
    }

    /// <summary>
    /// Mock version of `DuplicatePropertyNameChecker`, which does nothing.
    /// </summary>
    internal class NullDuplicatePropertyNameChecker : IDuplicatePropertyNameChecker
    {
        public void ValidatePropertyUniqueness(ODataProperty property)
        {
            // nop
        }

        public void ValidatePropertyUniqueness(ODataNestedResourceInfo property)
        {
            // nop
        }

        public void ValidatePropertyOpenForAssociationLink(string propertyName)
        {
            // nop
        }

        public void Reset()
        {
            // nop
        }
    }
}
