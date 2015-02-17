//---------------------------------------------------------------------
// <copyright file="StaticCallbackManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using System;

    /// <summary>Use this class to register and unregister static callbacks.</summary>
    /// <typeparam name="TEventArgs">Type of event arguments.</typeparam>
    public class StaticCallbackManager<TEventArgs> where TEventArgs : EventArgs
    {
        #region Private fields.

        /// <summary>Static handler for events.</summary>
        private static EventHandler<TEventArgs> staticHandler;

        #endregion Private fields.

        #region Events.

        /// <summary>Event fired through <see cref="FireEvent"/>.</summary>
        public static event EventHandler<TEventArgs> EventInvoked
        {
            add
            {
                staticHandler += value;
            }
            remove
            {
                staticHandler -= value;
            }
        }

        #endregion Events.

        #region Methods.

        /// <summary>Clears all events registered.</summary>
        public static void ClearInvoked()
        {
            staticHandler = null;
        }

        /// <summary>Fires an event to all registered handlers.</summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        public static void FireEvent(object sender, TEventArgs args)
        {
            EventHandler<TEventArgs> handler = staticHandler;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        /// <summary>
        /// Registers a static handler and returns an object to be disposed to 
        /// unregister.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <returns>An object that should be disposed to unregister the handler.</returns>
        public static IDisposable RegisterStatic(EventHandler<TEventArgs> handler)
        {
            return new DisposableRegistration(handler);
        }

        #endregion Methods.

        #region Inner types.

        /// <summary>Helper to register/unregister handlers.</summary>
        internal class DisposableRegistration: IDisposable
        {
            /// <summary>Handler to be unregistered.</summary>
            private readonly EventHandler<TEventArgs> handler;

            /// <summary>
            /// Initializes a new <see cref="DisposableRegistration"/> instance
            /// and registers a handler for the event.
            /// </summary>
            /// <param name="handler">Handler for the event.</param>
            internal DisposableRegistration(EventHandler<TEventArgs> handler)
            {
                this.handler = handler;
                StaticCallbackManager<TEventArgs>.EventInvoked += this.handler;
            }

            /// <summary>Unregisters the handler.</summary>
            public void Dispose()
            {
                StaticCallbackManager<TEventArgs>.EventInvoked -= this.handler;
            }
        }

        #endregion Inner types.
    }
}
