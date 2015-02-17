//---------------------------------------------------------------------
// <copyright file="ResourceUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Resources;                 //ResourceManager
    using System.Text.RegularExpressions;   //Regex

#if ClientFramework
    using Microsoft.Test.ModuleCore;        //TestFailedException
#endif
    using System.Reflection;                //TargetInvocationException
    using System.Threading;                 //Thread

    //Exception Verification Flags
    public enum ComparisonFlag
    {
        Full,
        RegularExpression,
        StartsWith,
        EndsWith,
        Contains
    }
    public class ResourceUtil
    {

        public static string MissingLocalizeResourceString { get { return "Localized resource string could not be found"; } }
        public static string GetLocalizedResourceString(ResourceIdentifier identifier, params Object[] args)
        {
            ResourceManager resourceManager = null;
            resourceManager = ResourceManagerUtil.GetResourceManagerFromAssembly(identifier.AssemblyResourceContainer);

            string result = resourceManager.GetString(identifier.Id, Thread.CurrentThread.CurrentUICulture);
            if (result == null)
                throw new ArgumentException(MissingLocalizeResourceString);

            return FormatResourceString(result, args);
        }

        public static string FormatResourceString(string pattern, params object[] args)
        {
            // logic copied from TextRes.GetString in Microsoft.OData.Client.cs on Jan 14, 2009
            // 
            if (args != null && args.Length > 0)
            { 
                for (int i = 0; i < args.Length; i++)
                {
                    String value = args[i] as String;
                    if (value != null && value.Length > 1024)
                    {
                        args[i] = value.Substring(0, 1024 - 3) + "...";
                    }
                }
                return String.Format(System.Globalization.CultureInfo.CurrentCulture, pattern, args);
            }
            return pattern;
        }


#if ClientFramework
        public static void VerifyException(Exception e, ResourceIdentifier identifier, params Object[] args)
        {
            if (identifier == null)
                throw new TestFailedException("VerifyException: an exception was expected, but ResourceIdentifier is null");

            //Exception not thrown, and should have
            if (e == null)
                throw new TestFailedException("VerifyException: an exception was expected, but not thrown Id='" + identifier.CompleteIdInfo + "'");

            if (identifier.ExpectedExceptionType == null)
                throw new TestFailedException("VerifyException: ResourceIdentifier doesn't contain a ExpectedException");

            //Reflection
            while (e is TargetInvocationException && e.InnerException != null && identifier.ExpectedExceptionType != typeof(TargetInvocationException))
                e = e.InnerException;

            //Test Exceptions (never expected)
            if (e is TestException)
                throw e;

            if (String.IsNullOrEmpty(identifier.Id))
            {
                if (e != null)
                    throw new TestFailedException("VerifyException: an exception was thrown, incorrect Identifier");
                return;
            }


            //Verify the type
            if (identifier.ExpectedExceptionType != null && e.GetType() != identifier.ExpectedExceptionType)
                throw new TestFailedException("Verify exception type", e.GetType(), identifier.ExpectedExceptionType, e);

            if (e.Source == null || e.Source == String.Empty)
                throw new TestFailedException("Exception is thrown, but the Source property of the exception is not set.");


            //Exception not thrown, and should have
            if (e == null && identifier.Id != null)
                throw new TestFailedException("VerifyException: an exception was expected, but not thrown Id='" + identifier.Id + "'");

            AstoriaTestLog.TraceInfo("VerifyException: " + identifier.CompleteIdInfo + " - " + e.GetType().Name);

            string expectedMessage = GetLocalizedResourceString(identifier);

            VerifyMessage(e.Message, expectedMessage, identifier.ComparisonFlag);
        }



        public static void VerifyMessage(string actualMessage, ResourceIdentifier identifier, params Object[] args)
        {
            //Lookup Id
            string expectedLocalizedMessage = GetLocalizedResourceString(identifier, args);
            VerifyMessage(actualMessage, expectedLocalizedMessage, identifier.ComparisonFlag);
        }

        public static void VerifyMessage(string actualMessage, string expectedMessage, ComparisonFlag compareFlag)
        {
            bool matched = false;
            switch (compareFlag)
            {
                case ComparisonFlag.RegularExpression:
                    //Regular expression match
                    matched = IsMatchInPattern(actualMessage, expectedMessage, compareFlag);
                    break;
                case ComparisonFlag.EndsWith:
                    matched = EndsWith(actualMessage, expectedMessage);
                    break;
                case ComparisonFlag.StartsWith:
                    matched = StartsWith(actualMessage, expectedMessage);
                    break;
                case ComparisonFlag.Contains:
                    matched = Contains(actualMessage, expectedMessage);
                    break;
                case ComparisonFlag.Full:
                default:
                    //exact
                    matched = actualMessage == expectedMessage;
                    break;
            }
            if (matched == false)
            {
                throw new TestFailedException(String.Format("Message does not match using comparison:{0} \n\t  Actual:{1} \n\texpected:{2}", compareFlag.ToString(), actualMessage, expectedMessage));
            }
        }

        private static bool IsMatchInPattern(string actualMessage, String expectedMessage, ComparisonFlag compareFlag)
        {
            string pattern = expectedMessage;
            if (compareFlag != ComparisonFlag.RegularExpression)
            {
                pattern = pattern.Replace(@"(", @"\(");      //Escape the brackets since they are used by regular expression
                pattern = pattern.Replace(@")", @"\)");
                pattern = pattern.Replace(@"[", @"\[");
                pattern = pattern.Replace(@"]", @"\]");
                pattern = pattern.Replace(@"?", @"\?"); //Escape ? since it's used by regular expression
                pattern = pattern.Replace(@"*", @"\*"); //Escape * since it's used by regular expression
                pattern = Regex.Replace(pattern, @"{\d}", @"(.*)");  //replace format item space holder with pattern expression
            }

            if (compareFlag == ComparisonFlag.StartsWith)
                pattern = pattern + "(.*)";
            else if (compareFlag == ComparisonFlag.EndsWith)
                pattern = "(.*)" + pattern;
            return Regex.IsMatch(actualMessage, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        }
        private static bool EndsWith(string actualMessage, String expectedMessage)
        {
            string[] substrs = Regex.Split(expectedMessage, @"{\d}");
            if (substrs != null && substrs.Length > 0)
                expectedMessage = substrs[substrs.Length - 1];
            return actualMessage.EndsWith(expectedMessage);
        }
        private static bool StartsWith(string actualMessage, String expectedMessage)
        {
            string[] substrs = Regex.Split(expectedMessage, @"{\d}");
            if (substrs != null && substrs.Length > 0)
                expectedMessage = substrs[0];
            return actualMessage.StartsWith(expectedMessage);
        }
        private static bool Contains(string actualMessage, String expectedMessage)
        {
            return actualMessage.Contains(expectedMessage);
        }
#endif
    }
    public class ResourceIdentifier
    {
        private string _localizedIdName;
        private Assembly _assemblyResouceIn;
        private Type _expectedExceptionType;
        private ComparisonFlag _comparisonFlag = ComparisonFlag.Full;

        public ResourceIdentifier(Assembly resContainer, string id, ComparisonFlag comparisonFlag, Type expectedExceptionType)
        {
            _localizedIdName = id;
            _assemblyResouceIn = resContainer;
            _expectedExceptionType = expectedExceptionType;
            _comparisonFlag = comparisonFlag;
        }
        public ResourceIdentifier(Assembly resContainer, string id, ComparisonFlag comparisonFlag)
            : this(resContainer, id, comparisonFlag, null)
        {
        }
        public ResourceIdentifier(Assembly resContainer, string id)
            : this(resContainer, id, ComparisonFlag.Full, null)
        {
        }
        public string Id { get { return _localizedIdName; } }
        public Assembly AssemblyResourceContainer { get { return _assemblyResouceIn; } }
        public Type ExpectedExceptionType { get { return _expectedExceptionType; } }
        public ComparisonFlag ComparisonFlag { get { return _comparisonFlag; } }
        internal string CompleteIdInfo
        {
            get { return AssemblyResourceContainer.GetName().Name + "_" + this.Id; }
        }
    }
}
