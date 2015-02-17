//---------------------------------------------------------------------
// <copyright file="OsCompatibility.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;

//namespace DDBasics.Data.Test
namespace System.Data.Test.Astoria
{
    [Flags]
    public enum OsCompatibilityFlags
    {
        None = 0,
        Vista = 0x1,
        Windows7 = 0x2,

        Unknown = unchecked((int)0x80000000)
    }

    public class OsCompatibility
    {
        static object s_staticContextCacheLock = new object();
        static OsCompatibilityFlags s_staticContextCache;
        static bool s_staticContextCachePresent;

        /// <summary>
        /// returns true if current OS is Windows7, Windows 2008 R2 or later. Use it before asserting win7 compat mode!
        /// </summary>
        public static bool IsWindows7CompatibilitySupported
        {
            get
            {
                return Environment.OSVersion.Version >= NativeMethods.Win7_Win2K8R2_Version;
            }
        }

        /// <summary>
        /// helper method that queries Windows7 compat section in applciation EXE and returns OS compat flags from there.
        /// </summary>
        /// <returns></returns>
        private static OsCompatibilityFlags QueryStaticContext()
        {
            if (!s_staticContextCachePresent)
            {
                lock (s_staticContextCacheLock)
                {
                    if (!s_staticContextCachePresent)
                    {
                        if (!IsWindows7CompatibilitySupported)
                        {
                            // we cannot use the API with CompatibilityInformationInActivationContext on current OS, set as unknown
                            s_staticContextCache = OsCompatibilityFlags.Unknown;
                        }
                        else
                        {
                            s_staticContextCache = NativeMethods.QueryOsCompatibilityInformation(NativeMethods.GetModuleHandleW(null));
                        }
                        s_staticContextCachePresent = true;
                    }
                }
            }
            return s_staticContextCache;
        }

        /// <summary>
        /// True if application exe has Vista compatibility GUID in RT_MANIFEST. Use this to check static context.
        /// Important Note: this method will always return false if running on OS prior to Windows7 or Windows 2008 R2.
        /// </summary>
        public static bool IsVistaCompatibilityContextEnabled
        {
            get
            {
                return (QueryStaticContext() & OsCompatibilityFlags.Vista) != 0;
            }
        }

        /// <summary>
        /// True of application exe has Windows7 compatibility GUID in RT_MANIFEST or if application exe was compiled with WINVER >= 6.1. 
        /// Use this to check Windows7 compat static context.
        /// Important Note: this method will always return false if running on OS prior to Windows7 or Windows 2008 R2.
        /// </summary>
        public static bool IsWindows7CompatibilityContextEnabled
        {
            get
            {
                return (QueryStaticContext() & OsCompatibilityFlags.Windows7) != 0;
            }
        }

        /// <summary>
        /// This method uses GetOverlappedResult to detect that Windows7 dynamic compat mode is currently active.
        /// In this mode, GetOverlappedResult now always waits on the overlapped event, even if results are ready!
        /// Before the Windows7 fix: GetOverlappedResult did not wait, so event remained active
        /// The trick here is to simulate the same behavior and test the overlapped event status!
        /// </summary>
        private static bool TestWindows7DynamicContext()
        {
            AutoResetEvent overlappedEvent = new AutoResetEvent(false);
            NativeOverlapped overlapped = new NativeOverlapped();
            overlapped.EventHandle = overlappedEvent.SafeWaitHandle.DangerousGetHandle();

            bool isActive = false;
            string tmpFile = Path.GetTempFileName();
            SafeFileHandle readStreamHandle = null;
            try
            {
                readStreamHandle = NativeMethods.CreateFile(
                    tmpFile,
                    NativeMethods.GENERIC_WRITE,
                    0, // no sharing
                    IntPtr.Zero, // default sec attributes
                    NativeMethods.OPEN_ALWAYS,  // open or create
                    NativeMethods.FILE_FLAG_OVERLAPPED,  // enable overlapped
                    IntPtr.Zero // no template
                    );

                if (readStreamHandle == null || readStreamHandle.IsInvalid)
                {
                    throw new System.ComponentModel.Win32Exception();
                }

                // read from file using async API
                byte[] tempBuf = Encoding.ASCII.GetBytes("Windows7CompatTest");
                int numberOfBytesWritten = 0;
                bool res = NativeMethods.WriteFile(readStreamHandle, tempBuf, tempBuf.Length, ref numberOfBytesWritten, ref overlapped);
                if (!res)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    if (lastError != 0 && lastError != NativeMethods.ERROR_IO_PENDING)
                    {
                        throw new System.ComponentModel.Win32Exception(lastError);
                    }
                }

                // ensure overlapped result has completed
                while (!NativeMethods.HasOverlappedIoCompleted(overlapped))
                    Thread.Sleep(1);

                // the event must be set
                bool isSet = overlappedEvent.WaitOne(0);
                Trace.Assert(isSet, "IO has completed, but event is not signalled yet!");
                overlappedEvent.Set(); // reset it back

                // call into GetOverlappedResult with TRUE flag
                // if Win7 compat mode is active: GetOverlappedResult will wait and reset the event, ignoring the fact that IO is completed
                // in not (or if not running on Win7): GetOverlappedResult does not wait if IO is completed, leaving the event signalled
                res = NativeMethods.GetOverlappedResult(readStreamHandle, ref overlapped, out numberOfBytesWritten, true);
                if (!res)
                {
                    throw new System.ComponentModel.Win32Exception();
                }

                // if GetOverlappedResult actually waits after IO completion ends (and WaitOne returns false), 
                // the app dynamic compat context is Win7 or Windows 2008 R2
                isActive = !overlappedEvent.WaitOne(0); // returns immediately
            }
            finally
            {
                if (readStreamHandle != null)
                    readStreamHandle.Close();

                if (File.Exists(tmpFile))
                    File.Delete(tmpFile);
            }

            return isActive;
        }

        /// <summary>
        /// use this method to query the status of Windows7 dynamic context, the indication for GetOverlappedResult method behavior.
        /// </summary>
        public static bool IsWindows7DynamicContextActive
        {
            get
            {
                return TestWindows7DynamicContext();
            }
        }

        /// <summary>
        /// use this method in your tests to assert that Windows7 dynamic compat context is active. The method first checks the context, and
        /// if it is not currently active, the method raises exception.
        /// 
        /// Exceptions raised:
        /// * NotSupportedException: current OS is not Windows7, nor Win2K8R2, nor later. Use IsWindows7CompatibilitySupported before!
        /// * InvalidOperationException: dynamic context for Windows7 compat mode has been deactivated. The exception includes list of modules 
        ///   that might be the cause for the downgrade to happen!
        ///  * other exceptions are unwelcome
        /// </summary>
        public static void EnsureWindows7DynamicContextIsActive()
        {
            if (!IsWindows7CompatibilitySupported)
            {
                throw new NotSupportedException("Current OS does not support Win7 compat mode testing. Please use IsWindows7CompatibilitySupported before asserting");
            }

            if (IsWindows7DynamicContextActive)
            {
                // the true test passed: GetOverlappedResult behaves as expected in Win7 compat mode
                // OK to continue
                return;
            }

            // The process is not running in Windows7 compat mode, check for possible reasons.
            // The code below is for collecting troubleshooting information to be raised as InvalidOperationException to the user.

            if (!IsWindows7CompatibilityContextEnabled)
            {
                // current process has neither Windows7 compat manifest nor proper exe subversion (not compiled with WINVER >= 6.1)
                // there is no point to query other modules since QueryActCtxW (CompatibilityInformationInActivationContext) 
                // returns no GUIDs in this case, even if modules do have compat manifest.
                throw new InvalidOperationException(string.Format(
                    "Process executable ({0}) has neither Windows7 GUID in its RT_MANIFEST nor compiled with WINVER>=6.1",
                    System.Diagnostics.Process.GetCurrentProcess().ProcessName));
            }

            // Process exe seems to be Win7 compatible, search for modules that are not compatible.
            // To ease on troubleshooting, collect incompatible modules list and add them to 
            // exception message. This way the caller can find out which of the modules 
            // downgrade the process.

            StringWriter exceptionMessage = new StringWriter();
            exceptionMessage.WriteLine(
                "The dynamic Win7 compat context for this process has been downgraded due " +
                "to one or more incompatible modules.");

            IntPtr[] modules = NativeMethods.GetModules();
            bool incompatibleModuleDetected = false;

            for (int i = 0; i < modules.Length; i++)
            {
                IntPtr module = modules[i];
                OsCompatibilityFlags osCompatFlags = NativeMethods.QueryOsCompatibilityInformation(module);
                bool isWin7CompatModule = (osCompatFlags & OsCompatibilityFlags.Windows7) != 0;

                if (!isWin7CompatModule)
                {
                    if (!incompatibleModuleDetected)
                    {
                        // first incompatible module
                        exceptionMessage.WriteLine("Incompatible modules list:");
                    }
                    incompatibleModuleDetected = true;

                    string moduleName = NativeMethods.GetModuleFileName(module);
                    exceptionMessage.WriteLine("Module: {0}; flags: {1}", moduleName, osCompatFlags);
                }
            }

            if (!incompatibleModuleDetected)
            {
                exceptionMessage.WriteLine("Cannot detect incompatible modules.");
            }

            // troubleshooting info is ready, raise the message to the user
            throw new InvalidOperationException(exceptionMessage.ToString());
        }
    }
}