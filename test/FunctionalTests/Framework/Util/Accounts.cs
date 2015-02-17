//---------------------------------------------------------------------
// <copyright file="Accounts.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Data.Test.Astoria.FullTrust;

namespace System.Data.Test.Astoria.Util
{
    /// <summary>Use this class to perform actions while impersonating an account.</summary>
    public class ImpersonationWrapper
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        internal unsafe static extern int FormatMessage(int dwFlags, ref IntPtr lpSource,
            int dwMessageId, int dwLanguageId, ref String lpBuffer, int nSize, IntPtr* Arguments);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal extern static bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal extern static bool DuplicateToken(IntPtr ExistingTokenHandle,
            int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);
        // GetErrorMessage formats and returns an error message
        // corresponding to the input errorCode.
        internal unsafe static string GetErrorMessage(int errorCode)
        {
            int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
            int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
            int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

            //int errorCode = 0x5; //ERROR_ACCESS_DENIED
            //throw new System.ComponentModel.Win32Exception(errorCode);

            int messageSize = 255;
            String lpMsgBuf = "";
            int dwFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS;

            IntPtr ptrlpSource = IntPtr.Zero;
            IntPtr prtArguments = IntPtr.Zero;

            int retVal = FormatMessage(dwFlags, ref ptrlpSource, errorCode, 0, ref lpMsgBuf, messageSize, &prtArguments);
            if (0 == retVal)
            {
                throw new Exception("Failed to format message for error code " + errorCode + ". ");
            }

            return lpMsgBuf;
        }

        /// <summary>
        /// Performs the specified <paramref name="action"/> impersonating the given <paramref name="account"/>
        /// when <paramref name="impersonate"/> is true.
        /// </summary>
        public static void DoAction(Account account, bool impersonate, Action action)
        {
            DoAction(account.UserName, account.Domain, account.Password, impersonate, action);
        }

        /// <summary>
        /// Performs the specified <paramref name="action"/> impersonating the given account
        /// when <paramref name="impersonate"/> is true.
        /// </summary>
        public static void DoAction(String userName, String domainName, String password, bool impersonate, Action action)
        {
            if (impersonate)
            {
                TestUtil.CheckArgumentNotNull(userName, "userName");
                TestUtil.CheckArgumentNotNull(domainName, "domainName");
                TestUtil.CheckArgumentNotNull(password, "password");
                IntPtr tokenHandle = IntPtr.Zero;
                IntPtr dupeTokenHandle = IntPtr.Zero;
                try
                {
                    const int LOGON32_PROVIDER_DEFAULT = 0;
                    //This parameter causes LogonUser to create a primary token.
                    const int LOGON32_LOGON_INTERACTIVE = 2;
                    const int SecurityImpersonation = 2;

                    // Call LogonUser to obtain a handle to an access token.
                    bool returnValue = LogonUser(userName, domainName, password,
                        LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                        ref tokenHandle);

                    AstoriaTestLog.TraceInfo("LogonUser called for user " + domainName + "\\" + userName);
                    if (false == returnValue)
                    {
                        int ret = Marshal.GetLastWin32Error();
                        AstoriaTestLog.TraceInfo(String.Format("LogonUser failed with error code : {0}", ret));
                        AstoriaTestLog.TraceInfo(String.Format("Error: [{0}] {1}\n", ret, GetErrorMessage(ret)));
                        int errorCode = 0x5; //ERROR_ACCESS_DENIED
                        throw new System.ComponentModel.Win32Exception(errorCode);
                    }

                    AstoriaTestLog.TraceInfo("Value of Windows NT token: " + tokenHandle);
                    bool retVal = DuplicateToken(tokenHandle, SecurityImpersonation, ref dupeTokenHandle);
                    if (false == retVal)
                    {
                        throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                    }

                    // The token that is passed to the following constructor must 
                    // be a primary token in order to use it for impersonation.
                    using (WindowsIdentity newId = new WindowsIdentity(dupeTokenHandle))
                    using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                    {
                        action.Invoke();

                        // Stop impersonating the user.
                        impersonatedUser.Undo();
                    }
                }
                catch (Exception ex)
                {
                    AstoriaTestLog.TraceInfo("Exception occurred. " + ex.Message);
                }
                finally
                {
                    // Free the tokens.
                    if (tokenHandle != IntPtr.Zero)
                        CloseHandle(tokenHandle);
                    if (dupeTokenHandle != IntPtr.Zero)
                        CloseHandle(dupeTokenHandle);
                }
            }
            else
                action.Invoke();

        }
    }

    public class Account
    {
        private string _userName;
        private string _domain;
        private string _password;

        internal Account(string userName, string domain, string password)
        {
            _userName = userName;
            _domain = domain;
            _password = password;
        }
        public string UserName
        {
            get { return _userName; }
        }
        public string Password
        {
            get { return _password; }
        }
        public string QualifiedUserName
        {
            get { return this.Domain + "\\" + this.UserName; }
        }
        public string Domain
        {
            get { return _domain; }
        }
        public bool IsCurrentAccount
        {
            get
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                if(identity.Name.Equals(this.QualifiedUserName))
                    return true;
                return false;
            }
        }
    }
    public class Accounts : fxList<Account>
    {
        public Account GetCurrentAccount()
        {
            foreach (Account a in this)
            {
                if (a.IsCurrentAccount)
                    return a;
            }
            //Unknown account is running
            return null;
        }
    }
}
