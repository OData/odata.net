//---------------------------------------------------------------------
// <copyright file="SocketExceptionHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Data.Test.Astoria.FullTrust;

namespace System.Data.Test.Astoria
{
    public static class SocketExceptionHandler
    {
        private const int SocketWaitMinutesMax = 4;
        private const int SocketExceptionWaitMinutes = SocketWaitMinutesMax;
        private const int SocketExceptionRetries = 5;
        private const int TimeoutWaitSeconds = 30;

        public static bool CatchTimeouts = true;

        private static void HandleSocketException(bool inWebRequest, SocketException exc)
        {
            //Wait for a couple of minutes to allow sockets to be freed up
            //http://blogs.msdn.com/dgorti/archive/2005/09/18/470766.aspx
            if (inWebRequest)
                AstoriaTestLog.WriteLineIgnore("A socket exception has occurred in WebRequest.GetResponse(), waiting for some to finish");
            else
                AstoriaTestLog.WriteLineIgnore("A socket exception has occurred, waiting for some to finish");
            AstoriaTestLog.WriteLineIgnore("\tException message was: " + exc.Message);
            AstoriaTestLog.WriteLineIgnore("Time before sleep: " + System.DateTime.Now);
            System.Threading.Thread.Sleep(new TimeSpan(0, SocketExceptionWaitMinutes, 0));
            AstoriaTestLog.WriteLineIgnore("Time after sleep: " + System.DateTime.Now);
            GC.Collect();
        }

        private static void HandleWebRequestTimeout(bool inWebRequest, WebException exc)
        {
            if (inWebRequest)
                AstoriaTestLog.WriteLineIgnore("A web request timed out in AstoriaRequest.GetResponse(), waiting before re-trying");
            else
                AstoriaTestLog.WriteLineIgnore("A web request timed out, waiting before re-trying");

            AstoriaTestLog.WriteLineIgnore("\tException message was: " + exc.Message);
            AstoriaTestLog.WriteLineIgnore("Time before sleep: " + System.DateTime.Now);
            System.Threading.Thread.Sleep(new TimeSpan(0, 0, TimeoutWaitSeconds));
            AstoriaTestLog.WriteLineIgnore("Time after sleep: " + System.DateTime.Now);
        }

        public static T Execute<T>(Func<T> f)
        {
            System.Net.WebException exc;
            return Execute<T>(f, false, out exc);
        }

        public static T Execute<T>(Func<T> f, bool inWebRequest, out System.Net.WebException exc)
        {
            exc = null;
            for (int i = 0; i < SocketExceptionRetries; i++)
            {
                try
                {
                    exc = null;
                    return TrustedMethods.ReturnFuncResult<T>(f);
                }
                catch (WebException ex)
                {
                    exc = ex;
                    if (ex.InnerException is SocketException)
                        HandleSocketException(inWebRequest, (SocketException)exc.InnerException);
                    else
                    {
                        if (inWebRequest)
                            return default(T); // it will handle this

                        if (ex.Status == WebExceptionStatus.Timeout && CatchTimeouts)
                            HandleWebRequestTimeout(inWebRequest, ex);
                        else
                            throw ex;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null || !(ex.InnerException is System.Net.WebException))
                        throw ex;

                    exc = (WebException)ex.InnerException;
                    if (exc.InnerException != null && (exc.InnerException is SocketException))
                        HandleSocketException(inWebRequest, (SocketException)exc.InnerException);
                    else if (exc.Status == WebExceptionStatus.Timeout && CatchTimeouts)
                        HandleWebRequestTimeout(inWebRequest, exc);
                    else
                        throw ex;
                }
            }
            AstoriaTestLog.FailAndThrow("Repeated socket exceptions encountered, consequences unpredictable.");
            return default(T);
        }
    }
}
