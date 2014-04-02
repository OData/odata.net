//---------------------------------------------------------------------
// <copyright file="ScriptObjectUtility.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      Utility class for browser scriptable objects.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Http
{
    #region Namespaces.

    using System;
    using System.Diagnostics;
    using System.Windows.Browser;

    #endregion Namespaces.

    /// <summary>Utility methods for interacting with script objects.</summary>
    internal static class ScriptObjectUtility
    {
        /// <summary>Helper script text.</summary>
        private const string HelperScript =
@"({
    cd: function(f) { return function() { f(); }; },
    callOpen: function(requestObj, method, uri) {
        requestObj.open(method,uri,true);
    },
    setReadyStateChange: function(requestObj, o1) {
        requestObj.onreadystatechange = o1;
    },
    setReadyStateChangeToNull: function(requestObj) {
        try { requestObj.onreadystatechange = null; }
        catch (e) { requestObj.onreadystatechange = new Function(); }
    }
})";

        /// <summary>Object with helper functions as methods..</summary>
        private static readonly ScriptObject HelperScriptObject = (ScriptObject)HtmlPage.Window.Eval(HelperScript);

        /// <summary>Creates an invokable object.</summary>
        /// <param name="d">Delegate for the callback.</param>
        /// <returns>An invokable object.</returns>
        internal static ScriptObject ToScriptFunction(Delegate d)
        {
            Debug.Assert(d != null, "d != null");
            return (ScriptObject)HelperScriptObject.Invoke("cd", d);
        }

        /// <summary>Invokes the 'open' method on a request object.</summary>
        /// <param name="request">Request object.</param>
        /// <param name="method">Method name.</param>
        /// <param name="uri">Target URI.</param>
        internal static void CallOpen(ScriptObject request, string method, string uri)
        {
            Debug.Assert(request != null, "request != null");
            HelperScriptObject.Invoke("callOpen", request, method, uri);
        }

        /// <summary>Sets the readyStateChanged callback handler.</summary>
        /// <param name="request">Request to set callback on.</param>
        /// <param name="callback">Callback.</param>
        internal static void SetReadyStateChange(ScriptObject request, ScriptObject callback)
        {
            Debug.Assert(request != null, "request != null");
            if (callback == null)
            {
                HelperScriptObject.Invoke("setReadyStateChangeToNull", request);
            }
            else
            {
                HelperScriptObject.Invoke("setReadyStateChange", request, callback);
            }
        }
    }
}
