//---------------------------------------------------------------------
// <copyright file="AstoriaTestLog.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using Microsoft.Test.ModuleCore;

    ////////////////////////////////////////////////////////
    // eAstoriaTraceLevel
    //
    ////////////////////////////////////////////////////////   
    public enum AstoriaTraceLevel
    {
        None,
        Info,
        All,
    }

    ////////////////////////////////////////////////////////
    // esLog
    //
    ////////////////////////////////////////////////////////   
    public class AstoriaTestLog
    {
        //Data
        private static bool failureFound;

        //Constructor

        //Accessors
        /// <summary>Whether any failures have been found.</summary>
        /// <remarks>
        /// This may be set/reset by callers to determine whether a failure 
        /// is found in a specific point in the code.
        /// </remarks>
        public static bool FailureFound
        {
            get { return failureFound; }
            set { failureFound = value; }
        }

        public static AstoriaTraceLevel Level
        {
            get { return (AstoriaTraceLevel)TestLog.Level; }
            set { TestLog.Level = (TraceLevel)value; }
        }

        //Methods
        public static void TraceInfo(String value)
        {
            AstoriaTestLog.TraceLine(AstoriaTraceLevel.Info, value);
        }

        public static void Trace(AstoriaTraceLevel level, String value)
        {
            TestLog.Trace((TraceLevel)level, value);
        }

        public static void WriteVariationStart(String Desc, int VariationID)
        {
            string strVariationTemplate = "<Variation Id=\"{1}\" Desc=\"{0}\"  Pri=\"2\"  Inheritance=\"True\"  Implemented=\"True\"  Skipped=\"False\"  Error=\"False\"  Manual=\"False\"  Security=\"0\"  Stress=\"False\"  Timeout=\"0\"  Threads=\"1\"  Repeat=\"0\" >";
            TestLog.WriteRaw(String.Format(strVariationTemplate, Desc, VariationID));
        }
        public static void WriteVariationEnd()
        {
            TestLog.WriteRaw("</Variation>");
        }
        public static void WriteVariationResult(bool Passed)
        {
            TestLog.WriteRaw(Passed ? "<Result>PASSED</Result> " : "<Result>FAILED</Result>");
        }
        public static void TraceLine(AstoriaTraceLevel level, String value)
        {
            TestLog.TraceLine((TraceLevel)level, value);
        }

        public static void TraceLine(AstoriaTraceLevel level)
        {
            TestLog.TraceLine((TraceLevel)level);
        }

        public static void TraceLine(Object value)
        {
            if (AstoriaTestProperties.RuntimeEnvironment == TestRuntimeEnvironment.Lab)
            {
                TestLog.TraceLine(value != null ? value.ToString() : null);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(value);
            }
        }

        static public void WriteLineIgnore(string text)
        {
            TestLog.WriteLineIgnore(text);
        }

        static public void WriteLineIgnore(string text, params object[] args)
        {
            TestLog.WriteLineIgnore(String.Format(text, args));
        }

        static public void WriteIgnore(string text)
        {
            TestLog.WriteIgnore(text);
        }

        static public void WriteIgnore(string text, params object[] args)
        {
            TestLog.WriteIgnore(String.Format(text, args));
        }

        public static TestResult HandleException(Exception e)
        {
            //Delegate
            FailureFound = true;
            return TestLog.HandleException(e);
        }

        // Mirror of MSTest methods
        public static void IsNotNull(object value)
        {
            AstoriaTestLog.IsNotNull(value, "");
        }

        public static void IsNotNull(object value, string message)
        {
            AstoriaTestLog.Compare(value != null, message);
        }

        public static void IsNull(object value)
        {
            AstoriaTestLog.IsNull(value, "");
        }

        public static void IsNull(object value, string message)
        {
            AstoriaTestLog.Compare(value == null, message);
        }

        public static void AreEqual(object expected, object actual)
        {
            AstoriaTestLog.AreEqual(expected, actual, "", true);
        }

        public static bool AreEqual(object expected, object actual, string message)
        {
            return AstoriaTestLog.AreEqual(expected, actual, message, true);
        }

        public static bool AreEqual(object expected, object actual, string message, bool continueOnFail)
        {
            //Equals is identical to Compare, except that Equals doesn't throw.
            if ((continueOnFail && !TestLog.Equals(actual, expected, message)) ||
                (!continueOnFail && !TestLog.Compare(actual, expected, message)))
            {
                FailureFound = true;
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void AreSame(object expected, object actual)
        {
            AstoriaTestLog.AreSame(expected, actual, "");
        }

        public static void AreSame(object expected, object actual, string message)
        {
            AstoriaTestLog.Compare(expected.Equals(actual), message);
        }

        public static void IsFalse(bool condition)
        {
            AstoriaTestLog.IsFalse(condition, "");
        }

        public static void IsFalse(bool condition, string message)
        {
            AstoriaTestLog.Compare(condition == false, message);
        }

        public static void IsTrue(bool condition)
        {
            AstoriaTestLog.IsTrue(condition, "");
        }

        public static void IsTrue(bool condition, string message)
        {
            AstoriaTestLog.Compare(condition == true, message);
        }

        public static void FailAndContinue(Exception e)
        {
            FailureFound = true;
            TestLog.HandleException(e);
        }

        public static void FailAndThrow(string message)
        {
            AstoriaTestLog.Compare(false, message);
        }

        public static void Compare(bool test, string message)
        {
            if (!test)
            {
                FailureFound = true;
            }

            TestLog.Compare(test, message);
        }

        public static void Inconclusive(string message)
        {
            AstoriaTestLog.Compare(false, message);
        }

        public static void Skip(string message)
        {
            TestLog.Skip(message);
        }

        public static void Warning(bool equal, string message)
        {
            TestLog.Warning(equal, message);
        }

        public static void Write(string text)
        {
            if (AstoriaTestProperties.RuntimeEnvironment == TestRuntimeEnvironment.Lab)
            {
                TestLog.Write(text);
            }
            else
            {
                System.Diagnostics.Trace.Write(text);
            }
        }

        public static void WriteLine(string text)
        {
            if (AstoriaTestProperties.RuntimeEnvironment == TestRuntimeEnvironment.Lab)
            {
                TestLog.WriteLine(text);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(text);
            }
        }

        public static void WriteLine(object o)
        {
            if (AstoriaTestProperties.RuntimeEnvironment == TestRuntimeEnvironment.Lab)
            {
                TestLog.WriteLine(o);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(o);
            }
        }

        public static void WriteLine(string text, params object[] args)
        {
            WriteLine(String.Format(text, args));
        }
    }
}

