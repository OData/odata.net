//---------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

//namespace DDBasics.Data.Test
namespace System.Data.Test.Astoria
{
    class NativeMethods
    {
        #region Windows Native methods and constants

        const int ERROR_INSUFFICIENT_BUFFER = 0x0000007a;
        const int STATUS_PENDING = 0x00000103;
        internal const uint GENERIC_WRITE = 0x40000000;
        internal const uint OPEN_ALWAYS = 4;
        internal const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        internal const int ERROR_IO_PENDING = 0x3e5;

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        internal static extern bool QueryActCtxW(
            int dwFlags,
            IntPtr hActCtx,
            IntPtr pvSubInstance,
            int ulInfoClass,
            byte[] pvBuffer,
            IntPtr cbBuffer,
            ref IntPtr pcbWrittenOrRequired);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetModuleHandleW(string moduleName);

        [DllImport("psapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        internal static extern bool EnumProcessModules(IntPtr processHandle, IntPtr[] modules, int size, ref int needed);

        [DllImport("psapi.dll", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int GetModuleFileNameExW(
            IntPtr hProcess,
            IntPtr hModule,
            StringBuilder lpFilename,
            int nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetOverlappedResult(
            SafeFileHandle hFile,
            ref NativeOverlapped lpOverlapped,
            out int lpNumberOfBytesTransferred, 
            bool bWait);

        internal static bool HasOverlappedIoCompleted(NativeOverlapped lpOverlapped)
        {
            // this method is defined as a MACRO in winbase.h:
            // #define HasOverlappedIoCompleted(lpOverlapped) (((DWORD)(lpOverlapped)->Internal) != STATUS_PENDING)
            // OVERLAPPED::Internal === NativeOverlapped.InternalLow
            return lpOverlapped.InternalLow.ToInt32() != STATUS_PENDING;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
          uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
          uint dwFlagsAndAttributes, IntPtr hTemplateFile);


        [DllImport("kernel32", SetLastError=true)]
        internal static extern bool WriteFile(
            SafeFileHandle hFile,
            byte[] lpBuffer,
            int nNumberOfBytesToWrite,
            ref int lpNumberOfBytesWritten,
            ref NativeOverlapped lpOverlapped);

        #endregion

        #region helper methods

        internal static readonly Version Win7_Win2K8R2_Version = new Version(6, 1);

        internal static string GetModuleFileName(IntPtr module)
        {
            StringBuilder name = new StringBuilder(1024);
            int size = GetModuleFileNameExW(Process.GetCurrentProcess().Handle, module, name, name.Capacity);
            if (size == 0)
                throw new Win32Exception(); // uses GetLastError automatically
            return name.ToString(0, size);
        }

        internal static IntPtr[] GetModules()
        {
            int required = 0;
            bool ret = EnumProcessModules(Process.GetCurrentProcess().Handle, null, 0, ref required);
            if (!ret && Marshal.GetLastWin32Error() != ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception();
            }

            IntPtr[] modules = new IntPtr[required / IntPtr.Size];
            ret = EnumProcessModules(Process.GetCurrentProcess().Handle, modules, modules.Length * IntPtr.Size, ref required);
            if (!ret)
            {
                throw new Win32Exception();
            }

            int modulesCount = required / IntPtr.Size;

            if (modules.Length == modulesCount)
            {
                return modules;
            }
            else
            {
                // we get less than asked, module might be just unloaded
                IntPtr[] newModules = new IntPtr[modulesCount];
                Array.Copy(modules, 0, newModules, 0, modulesCount);
                return newModules;
            }
        }

        internal static readonly Guid Win7Compat = new Guid("35138b9a-5d96-4fbd-8e2d-a2440225f93a");
        internal static readonly Guid VistaCompat = new Guid("e2011457-1546-43c5-a5fe-008deee3d3f0");

        // http://msdn.microsoft.com/en-us/library/dd765159(VS.85).aspx
        enum  ActcxCompatibilityElementType {
            Unknown = 0,
            OS ,
        }

        internal static OsCompatibilityFlags QueryOsCompatibilityInformation(IntPtr dllHandle)
        {
            if (Environment.OSVersion.Version < Win7_Win2K8R2_Version)
                throw new NotSupportedException(
                    "CompatibilityInformationInActivationContext flag was added in Win7 and Windows 2008 R2 only. It is not supported on current OS.");

            const int QUERY_ACTCTX_FLAG_ACTCTX_IS_HMODULE = 0x00000008;
            const int CompatibilityInformationInActivationContext = 6;

            IntPtr cbBuffer = IntPtr.Zero;
            bool ret = QueryActCtxW(
                QUERY_ACTCTX_FLAG_ACTCTX_IS_HMODULE,
                dllHandle,
                IntPtr.Zero,
                CompatibilityInformationInActivationContext,
                null,
                IntPtr.Zero,
                ref cbBuffer);
            if (ret)
            {
                Debug.Assert(cbBuffer == IntPtr.Zero);
                return OsCompatibilityFlags.None;
            }

            int status = Marshal.GetLastWin32Error();
            if (status != ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception(status);
            }

            byte[] CtxCompatInfo = new byte[cbBuffer.ToInt32()];
            ret = QueryActCtxW(
                QUERY_ACTCTX_FLAG_ACTCTX_IS_HMODULE,
                dllHandle,
                IntPtr.Zero,
                CompatibilityInformationInActivationContext,
                CtxCompatInfo,
                cbBuffer,
                ref cbBuffer);
            if (!ret)
            {
                throw new Win32Exception(); // error message is taken from GetLastError
            }

            // read the compatibility information, native structures are:
            // typedef struct _ACTIVATION_CONTEXT_COMPATIBILITY_INFORMATION {
            //     DWORD                         ElementCount;
            //     COMPATIBILITY_CONTEXT_ELEMENT Elements[];
            // } ACTIVATION_CONTEXT_COMPATIBILITY_INFORMATION, *PACTIVATION_CONTEXT_COMPATIBILITY_INFORMATION;
            // typedef struct _COMPATIBILITY_CONTEXT_ELEMENT {
            //   GUID                              Id;
            //   ACTCTX_COMPATIBILITY_ELEMENT_TYPE Type;
            // } COMPATIBILITY_CONTEXT_ELEMENT, *PCOMPATIBILITY_CONTEXT_ELEMENT;

            BinaryReader reader = new BinaryReader(new MemoryStream(CtxCompatInfo));
            int elmCount = 0;
            elmCount = reader.ReadInt32();

            OsCompatibilityFlags osCompatFlags = OsCompatibilityFlags.None;
            for (int i = 0; i < elmCount; i++)
            {
                Guid g = new Guid(reader.ReadBytes(16));
                ActcxCompatibilityElementType type = (ActcxCompatibilityElementType)reader.ReadInt32();
                if (type == ActcxCompatibilityElementType.OS)
                {
                    if (g == Win7Compat)
                    {
                        osCompatFlags |= OsCompatibilityFlags.Windows7;
                    }
                    else if (g == VistaCompat)
                    {
                        osCompatFlags |= OsCompatibilityFlags.Vista;
                    }
                    else
                    {
                        osCompatFlags |= OsCompatibilityFlags.Unknown;
                    }
                }
            }

            return osCompatFlags;
        }

        #endregion
    }
}