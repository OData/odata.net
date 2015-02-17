//---------------------------------------------------------------------
// <copyright file="Log.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test Logger interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the unique id of the logger
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="type">Message type</param>
        /// <param name="callid">Message callid</param>
        /// <param name="msg">Message txt</param>
        void Log(TraceEventType type, string callid, string msg);

        /// <summary>
        /// The unique id of the logger
        /// </summary>
        /// <param name="callid">Message callid</param>
        /// <param name="filePath">file Path</param>
        void AddFile(string callid, string filePath);
    }

    /// <summary>
    /// The log class
    /// </summary>
    public class Log
    {
        /// <summary>
        /// The logger list
        /// </summary>
        private static readonly Dictionary<Guid, ILogger> Loggersdic = new Dictionary<Guid, ILogger>();

        /// <summary>
        /// Initializes static members of the Log class
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Set property")]
        static Log()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DefaultErrorLevel = TraceEventType.Critical;
#if DEBUG
            LogLevel = TraceEventType.Verbose;
#else
            LogLevel = TraceEventType.Information ;
#endif
        }

        /// <summary>
        /// Gets or sets the loglevel 
        /// </summary>
        public static TraceEventType LogLevel { get; set; }

        /// <summary>
        /// Gets or sets the default error level
        /// </summary>
        public static TraceEventType DefaultErrorLevel { get; set; }

        /// <summary>
        /// Gets the logger list
        /// </summary>
        private static IEnumerable<ILogger> Loggers
        {
            get { return Loggersdic.Values; }
        }

        /// <summary>
        /// Gets the caller ID
        /// This property function has to be called before any other code in Log entry function
        /// in case the StackFrame messup
        /// In debug build inhibits CLR optimization including inclining, , static property get_CallerID is then count as a frame.
        /// So the skip frame is 3, GetCallerIdentity, get_CallerID and Log entry function
        /// In realease build with CLR optimization, get_CakkerID will be compiled as inline, so the skip frame is 2.
        /// </summary>
        private static string CallerID
        {
            get
            {
#if DEBUG
                return GetCallerIdentity(3);
#else
                return GetCallerIdentity(2);
#endif
            }
        }

        /// <summary>
        /// In the stop, we dispose and remove ID
        /// </summary>
        public static void Stop()
        {
            Loggersdic.Clear();
        }

        /// <summary>
        /// Add logger into the loggers list
        /// </summary>
        /// <param name="logger">The test logger</param>
        public static void Add(ILogger logger)
        {
            Loggersdic[logger.ID] = logger;
        }

        /// <summary>
        /// Add logger into the loggers list
        /// </summary>
        /// <param name="logger">The test logger</param>
        public static void Remove(ILogger logger)
        {
            if (Loggersdic.ContainsKey(logger.ID))
            {
                Loggersdic.Remove(logger.ID);
            }
        }

        /// <summary>
        /// Log info message
        /// </summary>
        /// <param name="obj">The log object</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Info(object obj)
        {
            string callid = CallerID;
            LogMessage(TraceEventType.Information, callid, ReplaceNulls(obj));
        }

        /// <summary>
        /// Log info message
        /// </summary>
        /// <param name="msg">The log message</param>
        /// <param name="paras">The message parameters</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Info(string msg, params object[] paras)
        {
            string callid = CallerID;
            LogMessage(TraceEventType.Information, callid, msg, paras);
        }

        /// <summary>
        /// Log info message
        /// </summary>
        /// <param name="msg">The log message</param>
        /// <param name="paras">The message parameters</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Error(string msg, params object[] paras)
        {
            string callid = CallerID;
            LogMessage(TraceEventType.Error, callid, msg, paras);
        }

        /// <summary>
        /// Check if two string are equal
        /// </summary>
        /// <param name="expected">Expected string</param>
        /// <param name="actual">Acutal string</param>
        /// <param name="ignoreCase">Ignore case</param>
        /// <param name="msg">The log message</param>
        /// <param name="parameters">The parameters</param>
        /// <returns>True if equals</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool AreEqual(
            string expected, string actual, bool ignoreCase = false, string msg = null, params object[] parameters)
        {
            string callid = CallerID;
            if (!string.IsNullOrEmpty(msg))
            {
                msg = string.Format(msg, parameters);
                msg += " ";
            }

            msg += "Expected : {0}, Actual : {1}";
            if (string.Compare(expected, actual, ignoreCase) != 0)
            {
                LogMessage(DefaultErrorLevel, callid, msg, ReplaceNulls(expected), ReplaceNulls(actual));
                return false;
            }

            LogMessage(TraceEventType.Information, callid, msg, ReplaceNulls(expected), ReplaceNulls(actual));
            return true;
        }

        /// <summary>
        /// Compare a datetime
        /// </summary>
        /// <param name="expected">Expected datetime</param>
        /// <param name="actual">Actual datetime</param>
        /// <param name="precisionTicks">Precision ticks</param>
        /// <param name="msg">The log message</param>
        /// <param name="parameters">The log parameters</param>
        /// <returns>True if equals</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool AreEqual(
            DateTime expected, DateTime actual, long precisionTicks = 0, string msg = null, params object[] parameters)
        {
            string callid = CallerID;
            if (!string.IsNullOrEmpty(msg))
            {
                msg = string.Format(msg, parameters);
                msg += " ";
            }

            msg += "Expected : {0} Ticks {1}, Actual : {2}m Ticks {3}";
            if (Math.Abs(expected.Ticks - actual.Ticks) > precisionTicks)
            {
                LogMessage(
                    DefaultErrorLevel, callid, msg, ReplaceNulls(expected), expected.Ticks, ReplaceNulls(actual), actual.Ticks);
                return false;
            }

            LogMessage(
                TraceEventType.Information, callid, msg, ReplaceNulls(expected), expected.Ticks, ReplaceNulls(actual), actual.Ticks);
            return true;
        }

        /// <summary>
        /// Check if two object are equal
        /// </summary>
        /// <param name="expected">Expect object</param>
        /// <param name="actual">Actual object</param>
        /// <param name="msg">The log message</param>
        /// <param name="parameters">The parameters</param>
        /// <returns>True if equals</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool AreEqual(object expected, object actual, string msg = null, params object[] parameters)
        {
            string callid = CallerID;
            if (!string.IsNullOrEmpty(msg))
            {
                msg = string.Format(msg, parameters);
                msg += " ";
            }

            msg += "Expected : {0}, Actual : {1}";
            bool ret = true;
            if (Equals(expected, actual))
            {
                // same object
                LogMessage(TraceEventType.Information, callid, msg, ReplaceNulls(expected), ReplaceNulls(actual));
            }
            else if (expected == null || actual == null)
            {
                // not equal - one is null another is not
                LogMessage(DefaultErrorLevel, callid, msg, ReplaceNulls(expected), ReplaceNulls(actual));
                ret = false;
            }
            else
            {
                if (!Equals(expected, actual))
                {
                    LogMessage(DefaultErrorLevel, callid, msg, ReplaceNulls(expected), ReplaceNulls(actual));
                    ret = false;
                }
                else
                {
                    LogMessage(TraceEventType.Information, callid, msg, ReplaceNulls(expected), ReplaceNulls(actual));
                }
            }

            return ret;
        }

        /// <summary>
        /// Compare two IEnumerables
        /// </summary>
        /// <typeparam name="T">The type of the items</typeparam>
        /// <param name="expected">Expected IEnumerable</param>
        /// <param name="actual">Actual IEumerable</param>
        /// <param name="msg">The log message</param>
        /// <param name="parameters">The log parameters</param>
        /// <returns>True if equals</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool AreEqual<T>(
            IEnumerable<T> expected, IEnumerable<T> actual, string msg = null, params object[] parameters)
        {
            return AreEqual(expected, actual, true, msg, parameters);
        }

        /// <summary>
        /// Check if two Enumerable object are equal
        /// </summary>
        /// <typeparam name="T">The type of the items</typeparam>
        /// <param name="expected">Expected IEnumerable</param>
        /// <param name="actual">Actual IEumerable</param>
        /// <param name="detailsInfo">Output info</param>
        /// <param name="msg">The log message</param>
        /// <param name="parameters">The log parameters</param>
        /// <returns>True if equals</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool AreEqual<T>(
            IEnumerable<T> expected, IEnumerable<T> actual, bool detailsInfo, string msg = null, params object[] parameters)
        {
            string callid = CallerID;
            if (!string.IsNullOrEmpty(msg))
            {
                msg = string.Format(msg, parameters);
                msg += " ";
            }

            msg += "Expected : {0}, Actual : {1}";
            if (Equals(expected, actual))
            {
                // same object
                LogMessage(TraceEventType.Information, callid, msg, ReplaceNulls(expected), ReplaceNulls(actual));
                return true;
            }

            if (expected == null || actual == null)
            {
                // not equal - one is null another is not
                LogMessage(DefaultErrorLevel, callid, msg, ReplaceNulls(expected), ReplaceNulls(actual));
                return false;
            }

            expected = expected.ToArray();
            actual = actual.ToArray();
            if (expected.Count() != actual.Count())
            {
                // not equal - different count
                return false;
            }

            // got same count, then checking each element
            LogMessage(TraceEventType.Information, callid, msg + " (count of elements)", expected.Count(), actual.Count());
            bool ret = true;
            TraceEventType ori = LogLevel;
            try
            {
                if (!detailsInfo)
                {
                    // hide the detail Log Info for each single element
                    LogLevel = ori < TraceEventType.Warning ? TraceEventType.Warning : ori;
                }

                for (int i = 0; i < expected.Count(); i++)
                {
                    dynamic ee = expected.ElementAt(i);
                    dynamic ae = actual.ElementAt(i);
                    if (!AreEqual(ee, ae))
                    {
                        ret = false;
                        break;
                    }
                }
            }
            finally
            {
                LogLevel = ori;
            }

            return ret;
        }

        /// <summary>
        /// Specialized overload of AreEqual to compare two ILogCompare derived objects
        /// </summary>
        /// <param name="expected">Expected ILogComparable</param>
        /// <param name="actual">Actual ILogComparable</param>
        /// <param name="msg">The log message</param>
        /// <param name="parameters">The log parameters</param>
        /// <returns>True if equals</returns>
        public static bool AreEqual(
            ILogComparable expected, ILogComparable actual, string msg = null, params object[] parameters)
        {
            bool result = false;

            Info(msg, parameters);

            if (expected.GetType() != actual.GetType())
            {
                Error("expecting type of {0}, but actual is {1}", expected.GetType(), actual.GetType());
            }
            else
            {
                var propertiesToCompare =
                    expected.GetType()
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                            .Where(p => p.GetCustomAttributes(false).OfType<ToCompareAttribute>().Any());

                foreach (var p in propertiesToCompare)
                {
                    dynamic pe = p.GetValue(expected, null);
                    dynamic pa = p.GetValue(actual, null);

                    if (pe == null && pa == null)
                    {
                        result = true;
                    }
                    else if (pe == null || pa == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = AreEqual(pe, pa, "comparing {0}", p.Name);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Log info message
        /// </summary>
        /// <param name="msg">The log message</param>
        /// <param name="paras">The log parameters</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Verbose(string msg, params object[] paras)
        {
            string callid = CallerID;
            LogMessage(TraceEventType.Verbose, callid, msg, paras);
        }

        /// <summary>
        /// Log Warning message
        /// </summary>
        /// <param name="msg">The log message</param>
        /// <param name="paras">The log parameters</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Warning(string msg, params object[] paras)
        {
            string callid = CallerID;
            LogMessage(TraceEventType.Warning, callid, msg, paras);
        }

        /// <summary>
        /// Log Exception message
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="rethrow">whether re-throw the exception out</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Exception(Exception ex, bool rethrow = true)
        {
            string callid = CallerID;
            LogMessage(TraceEventType.Critical, callid, ex.ToString());

            // re-throw
            if (rethrow)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Log CSV message
        /// </summary>
        /// <param name="paras">The log objects</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void CSV(params object[] paras)
        {
            string callid = CallerID;
            var sb = new StringBuilder();
            if (paras != null && paras.Length > 0)
            {
                for (int i = 0; i < paras.Length; i++)
                {
                    string buf = string.Empty;
                    if (paras[i] != null)
                    {
                        buf = paras[i].ToString();
                    }

                    buf = buf.Replace("\r", "\\r").Replace("\n", "\\n").Replace(",", "$(comma)");
                    sb.Append(buf);
                    if (i != paras.Length - 1)
                    {
                        sb.Append(",");
                    }
                }
            }

            LogMessage(TraceEventType.Transfer, callid, sb.ToString());
        }

        /// <summary>
        /// Add an additional file
        /// </summary>
        /// <param name="filename">The file name</param>
        public static void AddFile(string filename)
        {
            string id = CallerID;
            foreach (ILogger logger in Loggers)
            {
                logger.AddFile(id, filename);
            }
        }

        /// <summary>
        /// Checks for a condition and writes a message and throws an exception if the condition is false.
        /// </summary>
        /// <param name="condition">true to prevent a message being written; otherwise false.</param>
        /// <param name="assertDescription">A formatted message to write to Descript the assert</param>
        /// <param name="args">list of arguments for the formatted message</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Assert(bool condition, string assertDescription = null, params object[] args)
        {
            string callid = CallerID;
            if (assertDescription != null && args.Length != 0)
            {
                assertDescription = string.Format(assertDescription, args);
            }

            Assert(callid, condition, assertDescription);
        }

        /// <summary>
        /// Start test case
        /// </summary>
        /// <param name="testCase">Test case name</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void StartTestCase(string testCase)
        {
            LogMessage(TraceEventType.Start, CallerID, testCase);
        }

        /// <summary>
        /// End test case
        /// </summary>
        /// <param name="testCase">Test case name</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void StopTestCase(string testCase)
        {
            LogMessage(TraceEventType.Stop, CallerID, testCase);
        }

        /// <summary>
        /// Checks for a condition and writes a message and throws an exception if the condition is false.
        /// </summary>
        /// <param name="callid">the caller id</param>
        /// <param name="condition">true to prevent a message being written; otherwise false.</param>
        /// <param name="assertDescription">A formatted message to write to Descript the assert</param>
        private static void Assert(string callid, bool condition, string assertDescription)
        {
            if (!condition)
            {
                string errorMessage = string.Format("ASSERT \'{0}\' ", assertDescription);
                LogMessage(TraceEventType.Critical, callid, errorMessage);
                throw new AssertFailedException(errorMessage);
            }
        }

        /// <summary>
        /// This function will return the identity of immediate-1 caller
        /// </summary>
        /// <param name="skipFrames">The number of frames up the stack to skip
        /// Usually shoud 2, the current frame and "Log::Info" frame</param>
        /// <returns>Identity of immediate-1 caller in the form of [ClassName:MethodName]</returns>
        private static string GetCallerIdentity(int skipFrames)
        {
            // Get the strack frame for caller
            var frame = new StackFrame(skipFrames, true);
            if (null != frame.GetMethod() && null != frame.GetMethod().DeclaringType)
            {
                // Get the identity in form of [ClassName:MethodName]
                return string.Format(
                    "{0}:{1}", frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name);
            }

            return string.Empty;
        }

        private static string ReplaceNulls(object input)
        {
            if (input == null)
            {
                return "<Null>";
            }

            return input + " <" + input.GetType().Name + ">";
        }

        /// <summary>
        /// unhandled exception handler
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception((Exception)e.ExceptionObject);
        }

        /// <summary>
        /// Log the message
        /// </summary>
        /// <param name="type">Message type</param>
        /// <param name="callerid">caller id</param>
        /// <param name="msg">Message text</param>
        /// <param name="paras">Message paras</param>
        private static void LogMessage(TraceEventType type, string callerid, string msg, params object[] paras)
        {
            if (type < LogLevel || type >= TraceEventType.Start)
            {
                if (paras.Length != 0)
                {
                    msg = string.Format(msg, paras);
                }

                Exception firstex = null;
                foreach (ILogger logger in Loggers)
                {
                    try
                    {
                        logger.Log(type, callerid, msg);
                    }
                    catch (Exception ex)
                    {
                        if (firstex == null)
                        {
                            firstex = ex;
                        }
                    }
                }

                if (firstex != null)
                {
                    throw firstex;
                }
            }
        }
    }
}
