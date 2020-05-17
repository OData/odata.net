//---------------------------------------------------------------------
// <copyright file="ActionOnDispose.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;

    /// <summary>
    /// This class is responsible for executing an action the first time dispose is called on it.
    /// </summary>
    internal class ActionOnDispose : IDisposable
    {
        /// <summary>The action to be executed on dispose</summary>
        private Action action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionOnDispose" /> class.
        /// </summary>
        /// <param name="action">the action to be execute on dispose</param>
        public ActionOnDispose(Action action)
        {
            Util.CheckArgumentNull(action, "action");
            this.action = action;
        }

        /// <summary>
        /// The dispose method of the IDisposable interface
        /// </summary>
        public void Dispose()
        {
            if (this.action != null)
            {
                this.action();
                this.action = null;
            }
        }
    }
}
