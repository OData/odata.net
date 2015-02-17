//---------------------------------------------------------------------
// <copyright file="AssertUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    public static class AssertUtil
    {
        public static void Contains(string expected, string actual)
        {
            Assert.IsTrue((actual ?? "").Contains(expected), "Expected to contain: {0}\r\nActual{1}", expected, actual);
        }

        public static void FileExists(string filename)
        {
            Assert.IsNotNull(filename, "null filename");
            Assert.IsTrue(File.Exists(filename) && !Directory.Exists(filename), "missing file: {0}", filename);
        }

        public static TException RunCatch<TException>(Action action) where TException : Exception
        {
            return RunCatch<TException>(action, null);
        }

        public static TException RunCatch<TException>(System.Action action, System.Action<TException> validate) where TException : Exception
        {
            TException result = null;
            Assert.IsNotNull(action, "null action");
            try
            {
                action();
                Assert.Fail("Expected {0}", typeof(TException).Name);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(TException) || (!ex.GetType().IsPublic && typeof(TException).IsInstanceOfType(ex)))
                {
                    // must be exact TException type unless the exception is a non-public type
                    result = (TException)ex;
                    if (null != validate)
                    {
                        validate((TException)ex);
                    }
                }
                else if ((ex.GetType() == typeof(System.OutOfMemoryException)) ||
                         (ex.GetType() == typeof(System.StackOverflowException)) ||
                         (ex.GetType() == typeof(System.Threading.ThreadAbortException)) ||
                         (ex.GetType() == typeof(AssertFailedException)))
                {
                    // keep throwing if something like OutOfMemory, StackOverFlow, ThreadAbort unless explictly attempting to catch one
                    throw;
                }
                else
                {
                    Assert.Fail("Expected {0}, Actual {1}", typeof(TException).Name, ex.ToString());
                }
            }
            return result;
        }

        public static void RunCatch<TException>(System.Action action, Func<string, object[], string> getResourceString, string resourceName, params object[] args) where TException : Exception
        {
            RunCatch<TException>(action, delegate(TException ex)
            {
                Assert.AreEqual<string>(getResourceString(resourceName, args), ex.Message, "{0}", ex);
            });
        }

        public static void RunCatch<TException>(System.Action action, Assembly resourceAssembly, string resourceName, params object[] resourceArgs) where TException : Exception
        {
            Func<string, object[], string> getResourceString = (string name, object[] args) =>
                {
                    string baseName = resourceAssembly.GetName().Name.Replace("Microsoft.OData.Service", "Microsoft.OData.Service");
                    ResourceManager manager = new ResourceManager(baseName, resourceAssembly);
                    string resourceString = manager.GetString(name, CultureInfo.CurrentCulture);
                    Assert.IsNotNull(resourceString, "the resourceName '{0}' was not found in the assembly {1}", resourceName, resourceAssembly.FullName);
                    if (args != null && args.Length > 0)
                    {
                        return String.Format(resourceString, args);
                    }

                    return resourceString;
                };

            RunCatch<TException>(action, getResourceString, resourceName, resourceArgs);
        }
    }

    public static class ProcessUtil
    {
        private static string _CmdExeFileName;
        public static string CmdExeFileName
        {
            get
            {
                if (null == _CmdExeFileName)
                {
                    string systemDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                    _CmdExeFileName = Path.Combine(systemDir, @"cmd.exe"); ;
                }

                return _CmdExeFileName;
            }
        }

        public static bool UseShellToExecute { get; set; }

        public static string Execute(string filename, string arguments, int? expectedExitCode)
        {
            return UseShellToExecute
                ? ShellExecute(filename, arguments, expectedExitCode)
                : ProcExecute(filename, arguments, expectedExitCode);
        }

        public static string ProcExecute(string filename, string arguments, int? expectedExitCode)
        {
            AssertUtil.FileExists(filename);

            Process process = new Process();
            process.StartInfo.FileName = filename;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (expectedExitCode.HasValue)
            {
                Assert.AreEqual(expectedExitCode.Value, process.ExitCode, "ExitCode for {0}", filename);
            }

            return output + error;
        }

        public static string ShellExecute(string filename, string arguments, int? expectedExitCode)
        {
            AssertUtil.FileExists(filename);

            string output = null;
            string tmp = null;
            Process process = null;
            try
            {
                tmp = Path.GetTempFileName();

                process = new Process();
                process.StartInfo.FileName = CmdExeFileName;
                process.StartInfo.Arguments = "/D /C " + filename + " " + arguments + " 1> \"" + tmp + "\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.WaitForExit();

                output = File.ReadAllText(tmp);
            }
            finally
            {
                if (null != tmp)
                {
                    File.Delete(tmp);
                }
            }

            if (expectedExitCode.HasValue)
            {
                Assert.AreEqual(expectedExitCode.Value, process.ExitCode, "ExitCode for {0}", filename);
            }

            return output;
        }
    }

    public sealed class CommandLineWidth : IDisposable
    {
        private const string ScreenBufferSizeValueName = @"ScreenBufferSize";
        private const string WindowSizeValueName = @"WindowSize";
        private const string ConsoleKeyName = @"Console";
        private const int WidthMask = 0x7FFF;
        private const int HeightMask = ~0xFFFF;

        private Microsoft.Win32.RegistryKey consoleKey;

        private bool originalQueried;
        private object originalScreenBufferValue;
        private Microsoft.Win32.RegistryValueKind originalScreenBufferKind;

        private object originalWindowSizeValue;
        private Microsoft.Win32.RegistryValueKind originalWindowSizeKind;

        public CommandLineWidth(int width)
        {
            // http://technet.microsoft.com/en-us/library/cc781831.aspx
            // The value of this entry is an 4-byte hexadecimal value.
            // The first 2 bytes (high word) represent the number of lines that appear on the screen.
            // The last 2 bytes (low word) represent the number of lines of columns on the screen.
            Assert.IsTrue(width <= WidthMask, "width must be less than {0}", WidthMask);

            this.consoleKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(ConsoleKeyName, Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree);
            Assert.IsNotNull(consoleKey, "need ReadWriteSubTree for {0}", ConsoleKeyName);

            // ScreenBuffer
            this.originalScreenBufferValue = this.consoleKey.GetValue(ScreenBufferSizeValueName);
            if (null != this.originalScreenBufferValue)
            {
                this.originalScreenBufferKind = this.consoleKey.GetValueKind(ScreenBufferSizeValueName);
            }

            // WindowSize
            this.originalWindowSizeValue = this.consoleKey.GetValue(WindowSizeValueName);
            if (null != this.originalWindowSizeValue)
            {
                this.originalWindowSizeKind = this.consoleKey.GetValueKind(WindowSizeValueName);
            }
            this.originalQueried = true;

            int newWidth;

            // set ScreenBuffer
            newWidth = (((int)this.originalScreenBufferValue) & HeightMask) | (width & WidthMask);
            consoleKey.SetValue(ScreenBufferSizeValueName, newWidth);

            // set WindowSize
            newWidth = (((int)this.originalWindowSizeValue) & HeightMask) | (width & WidthMask);
            consoleKey.SetValue(WindowSizeValueName, newWidth);
        }

        ~CommandLineWidth()
        {
            this.Dispose(true);
        }

        public void Dispose()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (null != this.consoleKey)
            {
                if (this.originalQueried)
                {
                    this.consoleKey.SetValue(ScreenBufferSizeValueName, this.originalScreenBufferValue, this.originalScreenBufferKind);
                    this.consoleKey.SetValue(WindowSizeValueName, this.originalWindowSizeValue, this.originalWindowSizeKind);
                }

                this.consoleKey.Close();
                this.consoleKey = null;
            }
        }
    }
}
