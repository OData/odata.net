//---------------------------------------------------------------------
// <copyright file="EventLogger`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Simple component for logging calls to an event
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event's arguments</typeparam>
    public class EventLogger<TEventArgs> where TEventArgs : EventArgs
    {
        private List<KeyValuePair<object, TEventArgs>> events = new List<KeyValuePair<object, TEventArgs>>();

        /// <summary>
        /// Gets the list of events that have fired
        /// </summary>
        public IEnumerable<KeyValuePair<object, TEventArgs>> Events
        {
            get
            {
                return this.events.AsEnumerable();
            }
        }

        /// <summary>
        /// Callback to register on the event. Simply adds the sender and arguments to a list.
        /// </summary>
        /// <param name="sender">The object firing the event</param>
        /// <param name="args">The event arguments</param>
        public void LogEvent(object sender, TEventArgs args)
        {
            this.events.Add(new KeyValuePair<object, TEventArgs>(sender, args));
        }
    }
}