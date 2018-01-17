//---------------------------------------------------------------------
// <copyright file="TestUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
#if !ClientSKUFramework
    using System.Data.Entity.Design;
    using System.Data.Metadata.Edm;
#endif
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Xml;
    using System.Security.Principal;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using Microsoft.Test.ModuleCore;
    using System.Data.Test.Astoria.Util;
    using Microsoft.Win32;
    using System.Security.Permissions;
    using System.Security;
    using System.Data.Test.Astoria.FullTrust;

    #endregion Namespaces

    /// <summary>This class provides a variety of utility methods for tests.</summary>
    public static partial class TestUtil
    {
        #region Private fields.

        /// <summary>Data.Services assembly - access through DataWebAssembly to fault in.</summary>
        private static Assembly dataWebAssembly;

        /// <summary>Cached 'cache' field in metadata cache.</summary>
        private static FieldInfo providerMetadataCacheField;

        /// <summary>Microsoft.OData.Service.Caching.MetadataCache type - access through MetadataCacheType to fault in.</summary>
        private static Type providerMetadataCacheType;

        /// <summary>Cached 'cache' field in metadata cache.</summary>
        private static FieldInfo configurationCacheField;

        /// <summary>Microsoft.OData.Service.Caching</summary>
        private static Type configurationCacheType;

        /// <summary>Reusable namespace manager for tests.</summary>
        private static XmlNamespaceManager testNamespaceManager;

        /// <summary>Reusable name table for tests.</summary>
        private static XmlNameTable testNameTable;
        #endregion Private fields.

        public static string ConnectionString
        {
            get
            {
                return "@\"provider=System.Data.SqlClient;metadata={0};provider connection string='server=tcp:markash420,1432;database=Northwind;uid=DataWorks;pwd=fakepwd;persist security info=true;connect timeout=60;MultipleActiveResultSets=true;'\"";
            }
        }

        public static string CommonPayloadNamespaces
        {
            get
            {
                return "xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" " +
                       "xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\" " +
                       "xmlns=\"http://www.w3.org/2005/Atom\"";
            }
        }

        /// <summary>Microsoft.OData.Service.dll assembly.</summary>
        public static Assembly DataWebAssembly
        {
            get
            {
#if ClientSKUFramework
    dataWebAssembly = typeof(Int32).Assembly;
        throw new NotSupportedException("This method should not be called in a Client SKu Run");
        

#else
                if (dataWebAssembly == null)
                {


                    dataWebAssembly = typeof(Microsoft.OData.Service.IDataServiceHost).Assembly;

                }
                return dataWebAssembly;
#endif


            }
        }

        /// <summary>Directory to which dynamically generated assemblies are saved.</summary>
        public static string DynamicAssemblyDirectory
        {
            get { return GeneratedFilesLocation; }
        }

        /// <summary>A zero-length object array.</summary>
        public static readonly object[] EmptyObjectArray = new object[0];

        /// <summary>built binaries directory via %DD_BuiltTarget% or current location of the assembly.</summary>
        public static readonly string BinariesDirectory = GetBinariesDirectory();
        private static string GetBinariesDirectory()
        {
            string dir = Environment.GetEnvironmentVariable("DD_BuiltTarget");

            if (string.IsNullOrWhiteSpace(dir))
            {
                dir = Path.GetDirectoryName(typeof(System.Data.Test.Astoria.TestUtil).Assembly.Location);
            }

            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
            {
                throw new ApplicationException(@"Missing binaries directory - %DD_BuiltTarget% environment variables need to be defined and the directory exist: " + dir);
            }

            return dir;
        }

        /// <summary>suite binaries directory via %DD_SuitesTarget% or $(BinariesDirectory)</summary>
        public static readonly string SuiteBinDirectory = GetSuiteBinDirectory();
        private static string GetSuiteBinDirectory()
        {
            string dir = Environment.GetEnvironmentVariable("DD_SuitesTarget");

            if (string.IsNullOrWhiteSpace(dir))
            {
                dir = BinariesDirectory;
            }

            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
            {
                throw new ApplicationException(@"Missing suitebin directory - either %DD_SuitesTarget% or $(BinariesDirectory)\suitebin environment variables need to be defined and the directory exist: " + dir);
            }

            return dir;
        }

        /// <summary>generated files directory via $(SuiteBinDirectory)\GeneratedFiles\{guid}</summary>
        public static readonly string GeneratedFilesLocation = GetGeneratedFilesLocation();
        private static string GetGeneratedFilesLocation()
        {
            string dir = Path.Combine(BinariesDirectory, @"GeneratedFiles\\" + Guid.NewGuid());
            Directory.CreateDirectory(dir);
            return dir;
        }

        /// <summary>server unit test metadata files directory via $(DDSuitesDirectory)\src\fx\DataWeb\UnitTests\ServerUnitTests</summary>
        public static readonly string ServerUnitTestSamples = GetServerUnitTestSamples();
        private static string GetServerUnitTestSamples()
        {
            string dir = Path.Combine(EnlistmentRoot, @"test\FunctionalTests\Tests\Data\ServerUnitTests");
            if (!Directory.Exists(dir))
            {
                throw new ApplicationException(@"Missing ServerUnitTests directory: " + dir);
            }

            return dir;
        }

        /// <summary>
        /// Directory where Northwind metadata files are -
        /// '%sdxroot%\ddsuites\src\fx\DataWeb\Models\Northwind'.
        /// </summary>
        public static string NorthwindMetadataDirectory
        {
            get
            {
                string dir = Path.Combine(EnlistmentRoot, @"test\FunctionalTests\Tests\Data\Northwind");
                if (!Directory.Exists(dir))
                {
                    throw new ApplicationException(@"Missing NorthwindMetadataDirectory: " + dir);
                }

                return dir;
            }
        }

        /// <summary>Random number generator.</summary>
        public static Random Random
        {
            get
            {
                return AstoriaTestProperties.Random;
            }
        }

        /// <summary>Whether the test environment is in a minilab.</summary>
        public static bool RunningInMinilab
        {
            get
            {
                string minilabFile = Environment.ExpandEnvironmentVariables(@"%SystemDrive%\%COMPUTERNAME%.RollingBuild.xml");
                return File.Exists(minilabFile);
            }
        }

        /// <summary>Reusable name table for tests.</summary>
        public static XmlNameTable TestNameTable
        {
            get
            {
                if (testNameTable == null)
                {
                    testNameTable = new NameTable();
                }

                return testNameTable;
            }
        }

        /// <summary>Reusable namespace manager for tests.</summary>
        public static XmlNamespaceManager TestNamespaceManager
        {
            get
            {
                if (testNamespaceManager == null)
                {
                    testNamespaceManager = new XmlNamespaceManager(TestNameTable);

                    // Some common namespaces used by legacy tests.
                    testNamespaceManager.AddNamespace("dw", "http://docs.oasis-open.org/odata/ns/data");
                    testNamespaceManager.AddNamespace("csdl1", "http://schemas.microsoft.com/ado/2006/04/edm");
                    testNamespaceManager.AddNamespace("ads", "http://docs.oasis-open.org/odata/ns/data");
                    testNamespaceManager.AddNamespace("adsm", "http://docs.oasis-open.org/odata/ns/metadata");
                    testNamespaceManager.AddNamespace("csdl", "http://docs.oasis-open.org/odata/ns/edm");
                    testNamespaceManager.AddNamespace("ssdl", "http://schemas.microsoft.com/ado/2006/04/edm/ssdl");
                    testNamespaceManager.AddNamespace("msl", "urn:schemas-microsoft-com:windows:storage:mapping:CS");
                    testNamespaceManager.AddNamespace("app", "http://www.w3.org/2007/app");
                    testNamespaceManager.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                    testNamespaceManager.AddNamespace("edmx", "http://docs.oasis-open.org/odata/ns/edmx");
                    testNamespaceManager.AddNamespace("cy", "http://www.currency.org");
                    testNamespaceManager.AddNamespace("geo", "http://www.georss.org/georss");
                    testNamespaceManager.AddNamespace("gml", "http://www.opengis.net/gml");
                    testNamespaceManager.AddNamespace("ad", "http://www.address.org");
                    testNamespaceManager.AddNamespace("tmpNs", "http://tempuri.org");
                    testNamespaceManager.AddNamespace("c", "http://www.customer.org");
                }

                return testNamespaceManager;
            }
        }

        public static void AddToArray<T>(ref T[] existing, T element)
        {
            if (existing == null || existing.Length == 0)
            {
                existing = new T[] { element };
            }
            else
            {
                Array.Resize(ref existing, existing.Length + 1);
                existing[existing.Length - 1] = element;
            }
        }

#if !ClientSKUFramework


        /// <summary>Applies an enumeration of rights to containers.</summary>
        /// <param name="rights">Rights to apply to containers.</param>
        /// <param name="configuration">Configuration to apply rights to.</param>
        public static void ApplyResourceRights(IEnumerable<KeyValuePair<string, Microsoft.OData.Service.EntitySetRights>> rights, Microsoft.OData.Service.IDataServiceConfiguration configuration)
        {
            foreach (var rule in rights)
            {
                configuration.SetEntitySetAccessRule(rule.Key, rule.Value);
            }
        }

#endif


        /// <summary>Asserts that both enumerable arguments are equal; fails and throws otherwise.</summary>
        /// <param name="left">Left enumeration.</param>
        /// <param name="right">Right enumeration.</param>
        public static void AssertAreIEnumerablesEqual(IEnumerable left, IEnumerable right)
        {
            AssertAreIEnumerablesEqual(left, right, null);
        }


        public static void GenerateSilverlightXAPUsingMsBuild(string projectFilePath, string destinationPath, string assemblyName)
        {

            string greenbits = Environment.GetFolderPath(Environment.SpecialFolder.System);
            if (8 == IntPtr.Size)
            {
                greenbits = Path.Combine(greenbits, @"..\Microsoft.Net\Framework64\v3.5\MSBuild.exe");
            }
            else
            {
                greenbits = Path.Combine(greenbits, @"..\Microsoft.Net\Framework\v3.5\MSBuild.exe");
            }
            string commandOptions = String.Format("/v:m {0}", projectFilePath);
            Process chironCOmpiler = Process.Start(greenbits, commandOptions);
            chironCOmpiler.WaitForExit();
            if (!chironCOmpiler.HasExited)
                chironCOmpiler.Kill();

        }

        public static string SilverlightRuntimeVersion()
        {
            string runtimeVersion = null;
            try
            {
                // First we have to try to open the Wow64 so that we work correctly on 64bit OS
                RegistryKey key = null;
                try
                {
                    key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Silverlight");
                }
                catch (Exception)
                {
                }
                if (key == null)
                {
                    key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Silverlight");
                }

                runtimeVersion = key.GetValue("Version").ToString();
            }
            catch (Exception e)
            {
                throw new ApplicationException("No Silverlight runtime was found. You need to have Silverlight installed to be able to run xap applications.", e);
            }

            return runtimeVersion;
        }

        private static string silverlightRuntimeInstallPath;
        public static string SilverlightRuntimeInstallPath()
        {
            if (silverlightRuntimeInstallPath == null)
            {
                string programFilesPath = null;
                try
                {
                    programFilesPath = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                }
                catch (Exception)
                {
                }
                if (programFilesPath == null)
                {
                    programFilesPath = Environment.GetEnvironmentVariable("ProgramFiles");
                }

                string runtimeVersion = SilverlightRuntimeVersion();
                string runtimeInstallPath = Path.Combine(programFilesPath, "Microsoft Silverlight");
                runtimeInstallPath = Path.Combine(runtimeInstallPath, runtimeVersion);

                if (!Directory.Exists(runtimeInstallPath))
                {
                    throw new ApplicationException("The install path '" + runtimeInstallPath + "' doesn't exist.");
                }

                silverlightRuntimeInstallPath = runtimeInstallPath;
            }
            return silverlightRuntimeInstallPath;
        }

        public static void DeleteFile(string filename)
        {
            if (File.Exists(filename))
            {
                if ((File.GetAttributes(filename) & (FileAttributes.ReadOnly | FileAttributes.Hidden)) != 0)
                {
                    File.SetAttributes(filename, FileAttributes.Normal);
                }
                File.Delete(filename);
            }
        }

        /// <summary>Asserts that both enumerable arguments are equal; fails and throws otherwise.</summary>
        /// <param name="left">Left enumeration.</param>
        /// <param name="right">Right enumeration.</param>
        /// <param name="message">Message for assertion.</param>
        public static void AssertAreIEnumerablesEqual(IEnumerable left, IEnumerable right, string message)
        {
            if (left == null && right == null)
            {
                return;
            }

            if (left == null)
            {
                AstoriaTestLog.FailAndThrow("Left array is null but right one isn't - " + message);
            }

            if (right == null)
            {
                AstoriaTestLog.FailAndThrow("Right array is null but left one isn't - " + message);
            }

            IEnumerator leftEnumerator = left.GetEnumerator();
            IEnumerator rightEnumerator = right.GetEnumerator();
            try
            {
                int i = 0;
                while (true)
                {
                    if (leftEnumerator.MoveNext())
                    {
                        AstoriaTestLog.IsTrue(rightEnumerator.MoveNext());

                        i++;
                        object leftElement = leftEnumerator.Current;
                        object rightElement = rightEnumerator.Current;
                        if (leftElement is byte[] && rightElement is byte[])
                        {
                            AssertAreIEnumerablesEqual((byte[])leftElement, (byte[])rightElement);
                        }
                        else
                        {
                            AstoriaTestLog.AreEqual(
                                leftElement,
                                rightElement,
                                "Element #" + i + " matches in left and right enumerables left=[" + leftElement +
                                "], right=[" + rightElement + "] - " + message,
                                false /* continueOnFail */);
                        }
                    }
                    else
                    {
                        AstoriaTestLog.IsFalse(
                            rightEnumerator.MoveNext(),
                            "Right enumerator has no more elements as expected - " + message);
                        break;
                    }
                }
            }
            finally
            {
                if (leftEnumerator is IDisposable)
                {
                    ((IDisposable)leftEnumerator).Dispose();
                }
                if (rightEnumerator is IDisposable)
                {
                    ((IDisposable)rightEnumerator).Dispose();
                }
            }
        }

        /// <summary>
        /// Verifies that the specified <paramref name="text"/> contains the
        /// given <paramref name="value"/> a given number of times.
        /// </summary>
        /// <param name="expectedCount">Expected char matches.</param>
        /// <param name="text">Text to search in.</param>
        /// <param name="value">Text to search for.</param>
        public static void AssertCharCountEquals(int expectedCount, string text, char value)
        {
            int actualCount = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == value)
                {
                    actualCount++;
                }
            }

            AstoriaTestLog.AreEqual(expectedCount, actualCount,
                "Expected to find " + expectedCount + " occurrences of " + value +
                " in [" + text + "] and found " + actualCount + ".");
        }

        /// <summary>
        /// Verifies that the specified <paramref name="text"/> contains the
        /// given <paramref name="value"/>.
        /// </summary>
        /// <param name="text">Text to search in.</param>
        /// <param name="value">Text to search for.</param>
        public static void AssertContains(string text, string value)
        {
            if (!text.Contains(value))
            {
                AstoriaTestLog.FailAndThrow("Value [" + value + "] is unexpectedly not contained in [" + text + "]");
            }
        }

        /// <summary>
        /// Verifies that the specified <paramref name="text"/> does not contain the
        /// given <paramref name="value"/>.
        /// </summary>
        /// <param name="text">Text to search in.</param>
        /// <param name="value">Text to search for.</param>
        public static void AssertContainsFalse(string text, string value)
        {
            if (text.Contains(value))
            {
                AstoriaTestLog.FailAndThrow("Value [" + value + "] is unexpectedly contained in [" + text + "]");
            }
        }

        /// <summary>
        /// Verifies that the specified enumerable throws an exception that includes
        /// the <paramref name="textInMessage"/> value.
        /// </summary>
        /// <param name="e">Enumerable to iterate over.</param>
        /// <param name="textInMessage">Text expected in message (null or empty to skip this check).</param>
        public static void AssertEnumerableExceptionMessage(IEnumerable e, string textInMessage)
        {
            CheckArgumentNotNull(e, "e");

            Exception thrownException = null;
            try
            {
                foreach (object o in e) { }
            }
            catch (Exception exception)
            {
                thrownException = exception;
            }

            TestUtil.AssertExceptionExpected(thrownException, true);
            if (!String.IsNullOrEmpty(textInMessage))
            {
                TestUtil.AssertContains(thrownException.ToString(), textInMessage);
            }
        }

        /// <summary>
        /// Verifies that the specified enum <paramref name="value"/> is in the 
        /// given <paramref name="set"/>.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="set">Set that should contain <paramref name="value"/>.</param>
        public static void AssertEnumIn(object value, object set)
        {
            Debug.Assert(value != null, "value != null");
            Debug.Assert(set != null, "set != null");
            Debug.Assert(value.GetType().IsEnum);
            Debug.Assert(set.GetType().IsEnum);

            int intValue = (int)value;
            int intSet = (int)set;
            AstoriaTestLog.AreEqual(intValue, (intSet & intValue), value.ToString() + " is found in " + set.ToString());
        }

        /// <summary>Verifies that an exception was thrown if it was expected and logs appropriate information.</summary>
        /// <param name="exception">Exception thrown, or null if none was.</param>
        /// <param name="isExpected">true if an exception was expected, false otherwise.</param>
        public static void AssertExceptionExpected(Exception exception, bool isExpected)
        {
            if (isExpected && exception == null)
            {
                AstoriaTestLog.FailAndThrow("Exception expected but none thrown.");
            }
            else if (!isExpected && exception != null)
            {
                AssertFailUnexpectedException(exception);
            }
        }

        /// <summary>Verifies that an exception was thrown if it was expected and logs appropriate information.</summary>
        /// <param name="exception">Exception thrown, or null if none was.</param>
        /// <param name="isExpected">true if an exception was expected, false otherwise.</param>
        public static void AssertExceptionExpected(Exception exception, params bool[] expectedDisjunctive)
        {
            bool isExpected = false;
            for (int i = 0; i < expectedDisjunctive.Length; i++)
            {
                bool expectedIsTrue = expectedDisjunctive[i];
                if (expectedIsTrue && exception == null)
                {
                    string callingStatement = ExtractCallingStatement();
                    if (callingStatement != null) callingStatement = "\r\n" + callingStatement;
                    AstoriaTestLog.FailAndThrow("Exception expected because condition #" + (i + 1)
                        + " of " + expectedDisjunctive.Length + " is true but none thrown." + callingStatement);
                }
                isExpected = isExpected || expectedIsTrue;
            }

            if (!isExpected && exception != null)
            {
                AssertFailUnexpectedException(exception);
            }
        }

        /// <summary>Verifies that an exception was thrown for an expected HTTP status code.</summary>
        /// <param name="exception">Exception thrown.</param>
        /// <param name="expectedStatusCode">Expected status code.</param>
        /// <param name="message">Message to log.</param>
        public static void AssertExceptionStatusCode(Exception exception, int expectedStatusCode, string message)
        {
            CheckArgumentNotNull(exception, "exception");
            int actualStatusCode = (int)GetStatusCodeFromException(exception);
            AstoriaTestLog.AreSame(
                expectedStatusCode,
                actualStatusCode,
                String.Format("Expected status code {0}, actual {1}. {2}", expectedStatusCode, actualStatusCode, message));
        }

        /// <summary>Verifies that the specified exception is null; asserts failure otherwise.</summary>
        /// <param name="exception">Exception to check for null.</param>
        public static void AssertFailUnexpectedException(Exception exception)
        {
            if (exception != null)
            {
                if (exception.Data != null)
                {
                    string originalStackTrace = exception.Data["OriginalStackTrace"] as string;
                    if (originalStackTrace != null)
                    {
                        AstoriaTestLog.WriteLine("Original Stack Trace:\r\n" + originalStackTrace);
                    }
                }

                AstoriaTestLog.FailAndThrow("No exception expected, but found: " + exception);
            }
        }

        /// <summary>
        /// Returns the index of the first occurrence of the specified <paramref name="value"/> in 
        /// <paramref name="text "/> and asserts it's been found.
        /// </summary>
        /// <param name="text">String to look in.</param>
        /// <param name="value">The String to seek.</param>
        /// <returns>
        /// The index position of value if that string is found, an exception otherwise. If value is Empty, the 
        /// return value is 0.
        /// </returns>
        public static int AssertIndexOf(string text, string value)
        {
            TestUtil.CheckArgumentNotNull(text, "text");
            int result = text.IndexOf(value);
            if (result == -1)
            {
                throw new InvalidOperationException("Unable to find [" + value + "] in [" + text + "]");
            }

            return result;
        }

        /// <summary>
        /// Returns the index of the first occurrence of the specified <paramref name="value"/> in 
        /// <paramref name="text "/> and asserts it's been found. The search starts at a specified character position.
        /// </summary>
        /// <param name="text">String to look in.</param>
        /// <param name="value">The String to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>
        /// The index position of value if that string is found, an exception otherwise. If value is Empty, the 
        /// return value is <paramref name="startIndex"/>.
        /// </returns>
        public static int AssertIndexOf(string text, string value, int startIndex)
        {
            TestUtil.CheckArgumentNotNull(text, "text");
            int result = text.IndexOf(value, startIndex);
            if (result == -1)
            {
                throw new InvalidOperationException("Unable to find [" + value + "] in [" + text + "] starting at " + startIndex);
            }

            return result;
        }

        /// <summary>Checks that the specified URI is base of a given source.</summary>
        /// <param name="source">Expected base URI.</param>
        /// <param name="uri">Expected child URI.</param>
        /// <example>AssertIsBaseOf(new Uri("http://www.microsoft.com/"), new Uri("http://www.microsoft.com/default.aspx"));
        /// </example>
        public static void AssertIsBaseOf(Uri source, Uri uri)
        {
            if (!source.IsBaseOf(uri))
            {
                AstoriaTestLog.FailAndThrow("Failed expectation that <" + source.OriginalString + "> be a base of <" +
                    uri.OriginalString + ">.");
            }
        }

        /// <summary>Checks that the specified stream is empty.</summary>
        /// <param name="stream">Stream to check.</param>
        public static void AssertIsEmpty(Stream stream)
        {
            if (stream != null)
            {
                string content = new StreamReader(stream).ReadToEnd();
                if (!String.IsNullOrEmpty(content))
                {
                    AstoriaTestLog.FailAndThrow("Stream should be null but has this content: [" + content + "]");
                }
            }
        }

        /// <summary>
        /// Asserts that the specified attribute (possibly mapped to the
        /// <paramref name="namespaceName"/> on its prefix) has value 
        /// <paramref name="expectedValue"/>.
        /// </summary>
        /// <param name="node">Element node to look in.</param>
        /// <param name="attributeName">Name of attribute to look for.</param>
        /// <param name="namespaceName">Namespace mapped for attribute prefix (possibly null).</param>
        /// <param name="expectedValue">Expected value (null to indicate that the attribute should not be present).</param>
        public static void AssertNodeAttributeEquals(XmlNode node, string attributeName, string namespaceName, string expectedValue)
        {
            foreach (XmlAttribute item in node.Attributes)
            {
                if (item.LocalName == attributeName &&
                    (item.NamespaceURI == namespaceName || namespaceName == null && String.IsNullOrEmpty(item.NamespaceURI)))
                {
                    if (item.Value == expectedValue)
                    {
                        return;
                    }

                    AstoriaTestLog.AreSame(expectedValue, item.Value, "Expecting [" + expectedValue + "] for attribute " + attributeName + ", found " + item.Value);
                }
            }

            if (expectedValue != null)
            {
                TraceXml(node);
                AstoriaTestLog.FailAndThrow("Unable to find attribute " + attributeName + " on namespace [" + namespaceName + "] on last traced node.");
            }
        }

        /// <summary>Selects nodes from the specified node asserting their existence.</summary>
        /// <param name="node">Node to look in.</param>
        /// <param name="xpath">XPath for element to be returned.</param>
        /// <returns>The list of nodes found by the xpath.</returns>
        public static XmlNodeList AssertSelectNodes(XmlNode node, string xpath)
        {
            Debug.Assert(node != null, "node != null");
            Debug.Assert(xpath != null, "xpath != null");

            XmlNodeList result = node.SelectNodes(xpath, TestNamespaceManager);
            if (result.Count == 0)
            {
                TraceXml(node);
                AstoriaTestLog.FailAndThrow("Selection of [" + xpath + "] failed to return one or more nodes in last traced XML.");
            }

            return result;
        }

        /// <summary>
        /// Selects nodes from the specified node asserting their existence, 
        /// segment by segment (useful for debugging).
        /// </summary>
        /// <param name="node">Node to look in.</param>
        /// <param name="xpath">XPath for element to be returned.</param>
        /// <returns>The list of nodes found by the xpath.</returns>
        public static XmlNodeList AssertSelectNodesBySegment(XmlNode node, string xpath)
        {
            Debug.Assert(node != null, "node != null");
            Debug.Assert(xpath != null, "xpath != null");

            XmlNodeList lastResult = null;
            string[] xpathSegments = xpath.Split('/');
            string testSegment = "";
            foreach (string segment in xpathSegments)
            {
                testSegment += "/" + segment;
                lastResult = AssertSelectNodes(node, testSegment);
            }

            return lastResult;
        }

        /// <summary>Selects a single <see cref="XmlElement"/> from the specified node asserting its existence.</summary>
        /// <param name="node">Node to look in.</param>
        /// <param name="xpath">XPath for element to be returned.</param>
        /// <returns>The element found by the xpath.</returns>
        public static XmlElement AssertSelectSingleElement(XmlNode node, string xpath)
        {
            Debug.Assert(node != null, "node != null");
            Debug.Assert(xpath != null, "xpath != null");

            XmlNode result = node.SelectSingleNode(xpath, TestNamespaceManager);
            AstoriaTestLog.IsNotNull(result, "Selecting single node for xpath [" + xpath + "] in node " + node.OuterXml);
            return (XmlElement)result;
        }

        /// <summary>Builds a hex-dump representation of the contents of the specified <paramref name="stream"/>.</summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string BuildHexDump(Stream stream)
        {
            CheckArgumentNotNull(stream, "stream");
            const int CharsPerLine = 16;
            StringBuilder builder = new StringBuilder();
            StringBuilder textPart = new StringBuilder(CharsPerLine);
            int charsInLine = 0;
            int byteValue;
            while ((byteValue = stream.ReadByte()) >= 0)
            {
                if (charsInLine == CharsPerLine)
                {
                    charsInLine = 0;
                    builder.Append(' ');
                    builder.Append(textPart.ToString());
                    builder.Append("\r\n");
                    textPart = new StringBuilder(CharsPerLine);
                }

                string byteText = byteValue.ToString("X");
                if (byteText.Length == 1) builder.Append('0');
                builder.Append(byteText);
                builder.Append(" ");
                char charValue = (char)byteValue;
                if (!Char.IsControl(charValue) && charValue < 128)
                {
                    textPart.Append(charValue);
                }
                else
                {
                    textPart.Append(' ');
                }
                charsInLine++;
            }

            if (textPart.Length > 0)
            {
                builder.Append(' ', 3 * (CharsPerLine - textPart.Length) + 1);
                builder.Append(textPart.ToString());
            }

            return builder.ToString();
        }

        /// <summary>Checks that <paramref name="argumentValue"/> is not null, throws an exception otherwise.</summary>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void CheckArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>Verifies that the specified enumeration has no null elements.</summary>
        /// <param name="enumerable">Enumerable elements.</param>
        /// <param name="argumentName">Name of argument to check.</param>
        public static void CheckArgumentElementsNotNull(IEnumerable enumerable, string argumentName)
        {
            if (enumerable != null)
            {
                int index = 0;
                foreach (object o in enumerable)
                {
                    if (o == null)
                    {
                        throw new ArgumentException("Argument contains null element at zero-based position #" + index, argumentName);
                    }
                    index++;
                }
            }
        }

        /// <summary>Clears the Astoria metadata cache in the current AppDomain.</summary>
        public static void ClearMetadataCache()
        {
            if (providerMetadataCacheField == null)
            {
                providerMetadataCacheField = MetadataCacheType.GetField("cache", BindingFlags.NonPublic | BindingFlags.Static);
            }

            IDictionary dictionary = (IDictionary)providerMetadataCacheField.GetValue(null);
            dictionary.Clear();

            // Since we cache some metadata in the data service cache, we need to clear that also
            TestUtil.ClearConfiguration();
        }

        /// <summary>Clears the Astoria metadata cache in the current AppDomain.</summary>
        public static void ClearConfiguration()
        {
            if (configurationCacheField == null)
            {
                configurationCacheField = ConfigurationCacheType.GetField("cache", BindingFlags.NonPublic | BindingFlags.Static);
            }

            IDictionary dictionary = (IDictionary)configurationCacheField.GetValue(null);
            dictionary.Clear();
        }

        /// <summary>Concatenates two object enumerables.</summary>
        /// <param name="first">First enumerable.</param>
        /// <param name="second">Second enumerable.</param>
        /// <returns>
        /// An IEnumerable&lt;object&gt; with the elements of 
        /// <paramref name="first"/> and <paramref name="second"/>.
        /// </returns>
        public static IEnumerable<object> ConcatObjectEnumerables(IEnumerable first, IEnumerable second)
        {
            if (first != null)
            {
                foreach (object o in first) yield return o;
            }

            if (second != null)
            {
                foreach (object o in second) yield return o;
            }
        }

        /// <summary>Creates an initialized dictionary.</summary>
        public static IEnumerable<KeyValuePair<T, T>> CreateDictionary<T>(params T[] values)
        {
            List<KeyValuePair<T, T>> result = new List<KeyValuePair<T, T>>();
            using (IEnumerator<T> enumerator = values.AsEnumerable().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    T key = enumerator.Current;
                    if (!enumerator.MoveNext())
                    {
                        throw new Exception("values has an odd number of elements");
                    }

                    result.Add(new KeyValuePair<T, T>(key, enumerator.Current));
                }
            }

            return result;
        }

        /// <summary>Creates an initialized dictionary.</summary>
        public static Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            using (IEnumerator<TKey> keyEnumerator = keys.GetEnumerator())
            using (IEnumerator<TValue> valueEnumerator = values.GetEnumerator())
            {
                while (keyEnumerator.MoveNext())
                {
                    if (!valueEnumerator.MoveNext()) throw new Exception();
                    result.Add(keyEnumerator.Current, valueEnumerator.Current);
                }
                if (valueEnumerator.MoveNext()) throw new Exception();
            }
            return result;
        }

        /// <summary>Creates an initialized dictionary.</summary>
        public static Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(params KeyValuePair<TKey, TValue>[] values)
        {
            return CreateDictionary<TKey, TValue>((IEnumerable<KeyValuePair<TKey, TValue>>)values);
        }

        /// <summary>Creates an initialized dictionary.</summary>
        public static Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            foreach (var value in values)
            {
                result.Add(value.Key, value.Value);
            }
            return result;
        }

        /// <summary>Creates a <see cref="ModuleBuilder"/> with the specified <paramref name="name"/>.</summary>
        /// <param name="name">Assembly and module name.</param>
        public static ModuleBuilder CreateModuleBuilder(string name)
        {
            return CreateModuleBuilder(name, false);
        }

        /// <summary>Creates a <see cref="ModuleBuilder"/> with the specified <paramref name="name"/>.</summary>
        /// <param name="name">Assembly and module name.</param>
        /// <param name="allowSaving">Whether the assembly can be saved to disk.</param>
        public static ModuleBuilder CreateModuleBuilder(string name, bool allowSaving)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            const DebuggableAttribute.DebuggingModes debuggingModes = DebuggableAttribute.DebuggingModes.DisableOptimizations;
            AssemblyName assemblyName = new AssemblyName(name);
            AssemblyBuilder assembly;
            if (allowSaving)
            {
                assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, DynamicAssemblyDirectory);
            }
            else
            {
                assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            }

            // Enable debugging information.
            var attributeConstructor = typeof(DebuggableAttribute).GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
            var attributeBuilder = new CustomAttributeBuilder(attributeConstructor, new object[] { debuggingModes });
            assembly.SetCustomAttribute(attributeBuilder);

            if (allowSaving)
            {
                return assembly.DefineDynamicModule(name, name + ".dll", true);
            }
            else
            {
                return assembly.DefineDynamicModule(name, true);
            }
        }

#if !ClientSKUFramework

        /// <summary>
        /// Enables the debug behavior with IncludeExceptionDetailInFaults set to true on the specified 
        /// <paramref name="serviceHost"/>.
        /// </summary>
        /// <param name="serviceHost">Host for service to enable debugging for.</param>


        public static void EnableServiceDebugBehavior(System.ServiceModel.ServiceHost serviceHost)
        {
            TestUtil.CheckArgumentNotNull(serviceHost, "serviceHost");
            var debugBehavior = serviceHost.Description.Behaviors.Find<System.ServiceModel.Description.ServiceDebugBehavior>();
            if (debugBehavior == null)
            {
                debugBehavior = new System.ServiceModel.Description.ServiceDebugBehavior();
                serviceHost.Description.Behaviors.Add(debugBehavior);
            }

            debugBehavior.IncludeExceptionDetailInFaults = true;
        }

#endif

        public static void EnsureDirectoryExists(string directoryName)
        {
            if (directoryName == null)
            {
                throw new ArgumentNullException("directoryName");
            }

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        /// <summary>Removes all trailing slashes from the specified (possibly null) string.</summary>
        /// <param name="text">Text to remove trailing slashes from (possibly null).</param>
        /// <returns><paramref name="text"/> without any trailing slashes.</returns>
        public static string ExcludeAllTrailingSlashes(string text)
        {
            if (text != null && text.Length > 0)
            {
                return text.TrimEnd('/');
            }
            else
            {
                return text;
            }
        }

        public static string ExtractCallingStatement()
        {
            try
            {
                StackFrame frame = new StackFrame(2, true);
                string fileName = frame.GetFileName();
                int line = frame.GetFileLineNumber();
                if (line <= 0 || String.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                {
                    return null;
                }

                using (StreamReader reader = new StreamReader(fileName))
                {
                    string text = null;
                    while (line > 0)
                    {
                        line--;
                        text = reader.ReadLine();
                    }

                    if (text == null)
                    {
                        return null;
                    }

                    if (fileName.ToLowerInvariant().EndsWith(".cs"))
                    {
                        while (!text.Contains(';'))
                        {
                            text += "\r\n" + reader.ReadLine();
                        }
                    }
                    else if (fileName.ToLowerInvariant().EndsWith(".vb"))
                    {
                        while (text.EndsWith("_"))
                        {
                            text += "\r\n" + reader.ReadLine();
                        }
                    }

                    return text;
                }
            }
            catch
            {
                return null;
            }
        }

        public static string GenerateAssembly(string schemaFileName, bool isReflectionBasedProvider)
        {
            TestUtil.CheckArgumentNotNull(schemaFileName, "schemaFileName");
            return GenerateAssembly(schemaFileName, isReflectionBasedProvider, null);
        }

        public static string GenerateAssembly(string schemaFileName, string assemblyName, bool isReflectionBasedProvider)
        {
            TestUtil.CheckArgumentNotNull(schemaFileName, "schemaFileName");
            return GenerateAssembly(schemaFileName, isReflectionBasedProvider, null, new string[0], new string[0], new string[0], assemblyName);
        }

        public static string GenerateAssembly(string schemaFileName, bool isReflectionBasedProvider, string connectionString)
        {
            TestUtil.CheckArgumentNotNull(schemaFileName, "schemaFileName");
            return GenerateAssembly(schemaFileName, isReflectionBasedProvider, connectionString, new string[0], new string[0], new string[0], null);
        }

        public static string GenerateAssembly(
            string schemaFileName,
            bool isReflectionBasedProvider,
            string connectionString,
            IEnumerable<string> dependentSchemaFiles,
            IEnumerable<string> dependentAssembliesLocation,
            IEnumerable<string> additionalSourceFiles)
        {
            return GenerateAssembly(
                    schemaFileName,
                    isReflectionBasedProvider,
                    connectionString,
                    dependentSchemaFiles,
                    dependentAssembliesLocation,
                    additionalSourceFiles,
                    null);
        }

        public static string GenerateAssembly(
            string schemaFileName,
            bool isReflectionBasedProvider,
            string connectionString,
            IEnumerable<string> dependentSchemaFiles,
            IEnumerable<string> dependentAssembliesLocation,
            IEnumerable<string> additionalSourceFiles,
            string assemblyName)
        {
            TestUtil.CheckArgumentNotNull(schemaFileName, "schemaFileName");
            string codeFileName, newCodeFileName;
            string appendName = isReflectionBasedProvider ? "_ReflectionBased.cs" : "_ObjectContext.cs";

            if (String.IsNullOrEmpty(assemblyName))
            {
                codeFileName = Path.Combine(GeneratedFilesLocation, Path.ChangeExtension(Path.GetFileName(schemaFileName), "cs"));
            }
            else
            {
                codeFileName = Path.Combine(GeneratedFilesLocation, Path.ChangeExtension(Path.GetFileName(assemblyName), "cs"));
            }

            newCodeFileName = codeFileName.Replace(".cs", appendName);
            assemblyName = Path.Combine(GeneratedFilesLocation, Path.ChangeExtension(Path.GetFileName(newCodeFileName), "dll"));

            bool bypassGeneration =
                (AstoriaTestProperties.RuntimeEnvironment == TestRuntimeEnvironment.VisualStudioIde) &&
                File.Exists(codeFileName) &&
                File.Exists(newCodeFileName) &&
                File.Exists(assemblyName) &&
                File.GetCreationTime(codeFileName) > File.GetLastWriteTime(schemaFileName) &&
                File.GetCreationTime(newCodeFileName) > File.GetLastWriteTime(schemaFileName) &&
                File.GetCreationTime(assemblyName) > File.GetLastWriteTime(schemaFileName);

            if (!bypassGeneration)
            {
                // check if the schema is a v2 schema
                string fileContents = File.ReadAllText(schemaFileName);
                bool isV2Schema = fileContents.Contains("http://schemas.microsoft.com/ado/2008/09/edm");
                GenerateCodeUsingEdmToolsApi(schemaFileName, codeFileName, dependentSchemaFiles, isV2Schema);
                ChangeGeneratedCode(schemaFileName, codeFileName, newCodeFileName, isReflectionBasedProvider, connectionString, isV2Schema);
                List<string> sourceFiles = new List<string>();
                sourceFiles.Add(newCodeFileName);
                if (additionalSourceFiles != null)
                {
                    sourceFiles.AddRange(additionalSourceFiles);
                }
                GenerateAssembly(sourceFiles.ToArray(), assemblyName, dependentAssembliesLocation);
            }

            return assemblyName;
        }

        /// <summary>Gets all combinations for the parameterized enumeration type.</summary>
        /// <typeparam name="T">Enumeration type with flags to get enumeration for.</typeparam>
        /// <returns>An array of combination enumerations.</returns>
        public static T[] GetEnumCombinations<T>()
        {
            Debug.Assert(typeof(T).IsEnum, "typeof(" + typeof(T).Name + ").IsEnum");
            HashSet<T> combinations = new HashSet<T>();
            bool[] booleanValues = new bool[] { true, false };
            T[] values = (T[])Enum.GetValues(typeof(T));
            Dimension[] dimensions = values.Select(v => new Dimension(v.ToString(), booleanValues)).ToArray();
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(dimensions);
            Hashtable cs = new Hashtable();
            while (engine.Next(cs))
            {
                int combination = 0;
                for (int i = 0; i < dimensions.Length; i++)
                {
                    if ((bool)cs[dimensions[i].Name])
                    {
                        object o = values[i];
                        combination = combination | (int)o;
                    }
                }

                object temp = combination;
                combinations.Add((T)temp);
            }

            return combinations.ToArray();
        }

        /// <summary>
        /// Gets the media type value included in the specified Content-Type header value.
        /// </summary>
        /// <param name="contentType">Value of Content-Type header.</param>
        /// <returns>The media type value included in the specified Content-Type header value.</returns>
        /// <remarks>
        /// In the following example of a Content-Type header, the media type is "application/x-myType".
        /// 
        /// content-type: application/x-myType; name=data.xyz 
        /// </remarks>
        public static string GetMediaType(string contentType)
        {
            System.Net.Mime.ContentType c = new System.Net.Mime.ContentType(contentType);
            string mimeType = c.MediaType;

            // Since we need to make sure that if the accept header contains a value for the odata parameter
            // the response will always contain odata type parameter with the same value.
            int index = contentType.IndexOf(";odata.metadata=", StringComparison.OrdinalIgnoreCase);
            if (index != -1)
            {
                int endIndex = contentType.IndexOf(';', index + 1);
                if (endIndex == -1)
                {
                    endIndex = contentType.Length;
                }

                string odataTypeParameter = contentType.Substring(index, endIndex - index);
                mimeType += odataTypeParameter;
            }

            return mimeType;
        }

        public static Type GetIQueryableElement(Type type)
        {
            return GetTypeParameter(type, typeof(IQueryable<>), 0);
        }

        public static Type GetIEnumerableElement(Type type)
        {
            return GetTypeParameter(type, typeof(IEnumerable<>), 0);
        }

        /// <summary>Gets the web status code in the specified <paramref name="exception"/>.</summary>
        /// <param name="exception">Exception to extract status code from.</param>
        /// <returns>The HttpStatusCode for the exception.</returns>
        public static System.Net.HttpStatusCode GetStatusCodeFromException(Exception exception)
        {
            TestUtil.CheckArgumentNotNull(exception, "exception");
            Exception originalException = exception;
            while (exception != null)
            {
                if (exception is System.Net.WebException)
                {
                    System.Net.HttpWebResponse response = ((System.Net.WebException)exception).Response as System.Net.HttpWebResponse;
                    if (response != null)
                    {
                        return response.StatusCode;
                    }
                    else
                    {
                        exception = exception.InnerException;
                    }
                }
#if !ClientSKUFramework

                else if (exception is Microsoft.OData.Service.DataServiceException)
                {
                    return (System.Net.HttpStatusCode)((Microsoft.OData.Service.DataServiceException)exception).StatusCode;
                }
#endif

                else
                {
                    exception = exception.InnerException;
                }
            }

            // Other known operations that eventually get translated into DataServiceExceptions with
            // a 500 status code; however some initialization errors may not go through this state.
            if (originalException is InvalidOperationException ||
                originalException is ArgumentOutOfRangeException ||
                originalException is ArgumentNullException)
            {
                return System.Net.HttpStatusCode.InternalServerError;
            }

            throw new InvalidOperationException("Unable to find status code under:\r\n" + originalException.ToString());
        }

        public static Type GetTypeParameter(Type type, Type parameterizedType, int parameterIndex)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(parameterizedType != null, "parameterizedType != null");
            Debug.Assert(parameterizedType.IsGenericTypeDefinition);
            Debug.Assert(parameterIndex >= 0, "parameterIndex >= 0");

            if (parameterizedType.IsInterface)
            {
                if (InterfaceTypeFilter(type, parameterizedType))
                {
                    return type.GetGenericArguments()[parameterIndex];
                }

                Type[] interfaces = type.FindInterfaces(InterfaceTypeFilter, parameterizedType);
                if (interfaces.Length == 0 || interfaces.Length > 1)
                {
                    return null;
                }
                return interfaces[0].GetGenericArguments()[parameterIndex];
            }
            else
            {
                while (type != null && (!type.IsGenericType || type.GetGenericTypeDefinition() != parameterizedType))
                {
                    type = type.BaseType;
                }

                if (type == null)
                {
                    return null;
                }

                return type.GetGenericArguments()[parameterIndex];
            }
        }

        public static bool InterfaceTypeFilter(Type type, object parameterizedTypeCriteria)
        {
            Type parameterizedType = (Type)parameterizedTypeCriteria;
            return type.IsGenericType && type.GetGenericTypeDefinition() == parameterizedType;
        }


        /// <summary>Generic method to invoke a Where method on an IQueryable source.</summary>
        /// <typeparam name="TSource">Element type of the source.</typeparam>
        /// <param name="query">Source query.</param>
        /// <param name="predicate">Lambda expression that filters the result of the query.</param>
        /// <returns>A new query that filters the query.</returns>
        public static IQueryable InvokeWhere<TSource>(IQueryable query, System.Linq.Expressions.LambdaExpression predicate)
        {
            System.Diagnostics.Debug.Assert(query != null, "query != null");
            System.Diagnostics.Debug.Assert(predicate != null, "predicate != null");

            IQueryable<TSource> typedQueryable = (IQueryable<TSource>)query;
            System.Linq.Expressions.Expression<Func<TSource, bool>> typedPredicate = (System.Linq.Expressions.Expression<Func<TSource, bool>>)predicate;
            return Queryable.Where<TSource>(typedQueryable, typedPredicate);
        }

        /// <summary>Non-generic method to invoke a Where method on an IQueryable source.</summary>
        /// <param name="query">Source query.</param>
        /// <param name="predicate">Lambda expression that filters the result of the query.</param>
        /// <returns>A new query that filters the query.</returns>
        public static IQueryable InvokeWhereCasting(IQueryable query, System.Linq.Expressions.LambdaExpression predicate)
        {
            System.Diagnostics.Debug.Assert(query != null, "query != null");
            System.Diagnostics.Debug.Assert(predicate != null, "predicate != null");
            MethodInfo whereMethod = typeof(TestUtil).GetMethod("InvokeWhere", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(query.ElementType);
            return (IQueryable)whereMethod.Invoke(null, new object[] { query, predicate });
        }

        public static Type LoadDerivedTypeFromAssembly(Assembly assembly, Type baseType)
        {
            CheckArgumentNotNull(assembly, "assembly");
            CheckArgumentNotNull(baseType, "baseType");
            Type result = null;
            foreach (Type type in assembly.GetTypes())
            {
                if (baseType.IsAssignableFrom(type))
                {
                    if (result != null)
                    {
                        throw new Exception("Ambiguous derived types; assembly '" + assembly +
                            "' has types '" + type + "' and '" + result + "' both assignable from '" +
                            baseType + "'.");
                    }
                    result = type;
                }
            }

            if (result == null)
            {
                throw new Exception("Unable to find a type in assembly '" + assembly +
                    "' that derives from '" + baseType + "'.");
            }

            return result;
        }

        public static Type LoadDerivedTypeFromAssembly(string path, Type baseType)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (baseType == null)
            {
                throw new ArgumentNullException("baseType");
            }

            Assembly assembly = Assembly.LoadFile(path);
            return LoadDerivedTypeFromAssembly(assembly, baseType);
        }

#if !ClientSKUFramework

        /// <summary>Parses a string into an enumerable object of container/right pairs.</summary>
        /// <param name="text">Text to parse.</param>
        /// <returns>An enumerable object of container/right pairs.</returns>
        /// <remarks>
        /// Right declarations, formatted as follows:
        /// Right-Decls ::= Right-Decl | Right-Decl ";" Right-Decls
        /// Right-Decl  ::= ContainerName ":" Rights
        /// Rights      ::= Right | Right "," Rights
        /// Right       ::= "N" | "RS" | "RM" | "WA" | "WR" | "WM" | "WD" | "All"
        /// </remarks>
        public static IEnumerable<KeyValuePair<string, Microsoft.OData.Service.EntitySetRights>> ParseResourceRightsText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            var result = new List<KeyValuePair<string, Microsoft.OData.Service.EntitySetRights>>();
            string[] declarations = text.Split(';');
            foreach (string declaration in declarations)
            {
                string[] declarationParts = declaration.Split(':');
                string containerName = declarationParts[0].Trim();
                string[] rights = declarationParts[1].Split(',');
                Microsoft.OData.Service.EntitySetRights containerRights = default(Microsoft.OData.Service.EntitySetRights);
                foreach (string right in rights)
                {
                    switch (right.ToUpperInvariant().Trim())
                    {
                        case "N":
                            containerRights |= Microsoft.OData.Service.EntitySetRights.None;
                            break;
                        case "RS":
                            containerRights |= Microsoft.OData.Service.EntitySetRights.ReadSingle;
                            break;
                        case "RM":
                            containerRights |= Microsoft.OData.Service.EntitySetRights.ReadMultiple;
                            break;
                        case "AR":
                            containerRights |= Microsoft.OData.Service.EntitySetRights.AllRead;
                            break;
                        case "WA":
                            containerRights |= Microsoft.OData.Service.EntitySetRights.WriteAppend;
                            break;
                        case "WM":
                            containerRights |= Microsoft.OData.Service.EntitySetRights.WriteMerge;
                            break;
                        case "WR":
                            containerRights |= Microsoft.OData.Service.EntitySetRights.WriteReplace;
                            break;
                        case "WD":
                            containerRights |= Microsoft.OData.Service.EntitySetRights.WriteDelete;
                            break;
                        case "AW":
                            containerRights |= Microsoft.OData.Service.EntitySetRights.AllWrite;
                            break;
                        case "ALL":
                            containerRights |= Microsoft.OData.Service.EntitySetRights.All;
                            break;
                        default:
                            throw new InvalidOperationException("Unknown right: " + right);
                    }
                }
                result.Add(new KeyValuePair<string, Microsoft.OData.Service.EntitySetRights>(containerName, containerRights));
            }

            return result;
        }
#endif

        /// <summary>Creates an object that will restore a static value on disposal.</summary>
        /// <param name="type">Type to read static value from.</param>
        /// <param name="propertyName">Name of property to read value from.</param>
        /// <returns>An object that will restore a static value on disposal.</returns>
        /// <remarks>
        /// The usage pattern is:
        /// using (var r = TestUtil.RestoreStaticValueOnDispose(typeof(Foo), "Prop")) { ... }
        /// </remarks>
        public static IDisposable RestoreStaticValueOnDispose(Type type, string propertyName)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
            MemberTypes memberTypes = MemberTypes.Property | MemberTypes.Field;
            MemberInfo propertyInfo = type.GetMember(propertyName, memberTypes, flags).FirstOrDefault();
            if (propertyInfo == null)
            {
                throw new Exception("Unable to find property " + propertyName + " on type " + type + ".");
            }
            return new StaticValueRestorer(propertyInfo);
        }

        /// <summary>Creates an object that will restore all static members of the given type on disposal</summary>
        /// <param name="type">Type to restore</param>
        /// <returns>An object that will restore all static members of the given type on disposal</returns>
        public static IDisposable RestoreStaticMembersOnDispose(Type type)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.SetProperty;
            List<MemberInfo> memberInfos = new List<MemberInfo>();
            memberInfos.AddRange(type.GetFields(flags));
            memberInfos.AddRange(type.GetProperties(flags));
            return new StaticValueRestorer(memberInfos.ToArray());
        }

        /// <summary>
        /// Runs the specified <paramref name="testCallback"/> on all combinations
        /// for the <paramref name="engine"/>.
        /// </summary>
        /// <param name="engine">Engine that produces combinations to run.</param>
        /// <param name="testCallback">Callback for test.</param>
        public static void RunCombinatorialEngineNoFail(CombinatorialEngine engine, Action<Hashtable> testCallback)
        {
            int passCount, failCount;
            RunCombinatorialEngine(engine, testCallback, out passCount, out failCount);
        }

        /// <summary>
        /// Runs the specified <paramref name="testCallback"/> on all combinations
        /// for the <paramref name="engine"/>.
        /// </summary>
        /// <param name="engine">Engine that produces combinations to run.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="passCount">Number of combinations that passed.</param>
        /// <param name="failCount">Number of combinations that failed.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinatorialEngine(CombinatorialEngine engine, Action<Hashtable> testCallback,
            out int passCount, out int failCount, int? skipTo = null)
        {
            Debug.Assert(engine != null, "engine != null");
            Debug.Assert(testCallback != null, "testCallback != null");

            if (skipTo.HasValue && !Debugger.IsAttached)
            {
                throw new Exception("The skipTo parameter was used in TestUtil.RunCombinatorialEngine or TestUtil.RunCombinations and the code is not running under debugger. " +
                    "The skipTo is only intended for usage during debugging and MUST NOT be present in checked-in code.");
            }

            bool shouldContinue = String.IsNullOrEmpty(TrustedMethods.GetEnvironmentVariable("_NTTREE"));

            failCount = 0;
            passCount = 0;
            int callbackCount = 0;
            Hashtable table = new Hashtable();
            StringBuilder combinationLogger = new StringBuilder();
            bool savedFailureFound = AstoriaTestLog.FailureFound;
            while (engine.Next(table))
            {
                combinationLogger.Remove(0, combinationLogger.Length);

                callbackCount++;
                if (skipTo.HasValue && callbackCount < skipTo.Value)
                {
                    continue;
                }

                try
                {
                    using (ScopedTraceListener listener = new ScopedTraceListener(combinationLogger))
                    {
                        listener.Install();
                        AstoriaTestLog.FailureFound = false;
                        testCallback(table);
                    }

                    if (AstoriaTestLog.FailureFound)
                    {
                        throw new Exception("A failure was marked in AstoriaTestLog.FailureFound.");
                    }

                    passCount++;
                }
                catch (Exception exceptionThrown)
                {
                    bool failureFoundSet = AstoriaTestLog.FailureFound;
                    AstoriaTestLog.FailureFound = savedFailureFound;

                    failCount++;
                    string breakMessage = "(break with conditional breakpoint " + engine.DescribeStateCondition() + ")";
                    AstoriaTestLog.WriteLine("Test callback failed for the following state on combination #" + callbackCount + ": " + breakMessage);
                    AstoriaTestLog.WriteLine(engine.DescribeState());

                    if (combinationLogger.Length > 0)
                    {
                        AstoriaTestLog.WriteLine("Additional logging:");
                        AstoriaTestLog.Write(combinationLogger.ToString());
                    }

                    AstoriaTestLog.WriteLine("Exception:");
                    AstoriaTestLog.WriteLine(exceptionThrown);
                    if (exceptionThrown.Data.Contains("OriginalStackTrace"))
                    {
                        AstoriaTestLog.WriteLine("Original stack trace: " + exceptionThrown.Data["OriginalStackTrace"]);
                    }

                    if (shouldContinue)
                    {
                        AstoriaTestLog.WriteLine("Continuing after a failure in " + testCallback.Method.Name + "...");
                    }
                    else
                    {
                        AstoriaTestLog.WriteLine("Not continuing after a failure in " + testCallback.Method.Name + "...");
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Runs the specified <paramref name="testCallback"/> on all combinations
        /// for the <paramref name="engine"/>, and throws an exception if any
        /// combinations fail.
        /// </summary>
        /// <param name="engine">Engine that produces combinations to run.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinatorialEngineFail(CombinatorialEngine engine, Action<Hashtable> testCallback, int? skipTo = null)
        {
            int passCount, failCount;
            RunCombinatorialEngine(engine, testCallback, out passCount, out failCount, skipTo);
            if (failCount > 0)
            {
                throw new Exception(failCount + " of " + (passCount + failCount) + " test cases failed - view log for details.");
            }
        }

        public static Exception UnwrapReflectionException(Exception e)
        {
            if (e is TargetInvocationException)
            {
                return UnwrapReflectionException(e.InnerException);
            }

            return e;
        }

        /// <summary>Runs the specified action and catches any thrown exception.</summary>
        /// <param name="action">Action to run.</param>
        /// <typeparam name="T">The exception type</typeparam>
        /// <returns>Caught exception; null if none was thrown, or the thrown exception is of the wrong type</returns>
        public static T RunCatching<T>(Action action)
            where T : class
        {
            Exception ex = RunCatching(action);
            if (ex == null)
            {
                return null;
            }
            else if (ex is T)
            {
                return ex as T;
            }
            else
            {
                AstoriaTestLog.FailAndThrow("Exception of the wrong type thrown - expected " + typeof(T).Name + " but got " + ex.GetType().Name);
                return null;
            }
        }

        /// <summary>Runs the specified action and catches any thrown exception.</summary>
        /// <param name="action">Action to run.</param>
        /// <returns>Caught exception; null if none was thrown.</returns>
        public static Exception RunCatching(Action action)
        {
            Debug.Assert(action != null, "action != null");

            Exception result = null;
            try
            {
                action();
            }
            catch (Exception exception)
            {
                result = exception;
            }

            return result;
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0>(IEnumerable<TDimension0> dimension0, Action<TDimension0> testCallback, int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <typeparam name="TDimension1">Type of objects in <paramref name="dimension1"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension1">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0, TDimension1>(IEnumerable<TDimension0> dimension0, IEnumerable<TDimension1> dimension1,
            Action<TDimension0, TDimension1> testCallback, int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0, dimension1);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <typeparam name="TDimension1">Type of objects in <paramref name="dimension1"/></typeparam>
        /// <typeparam name="TDimension2">Type of objects in <paramref name="dimension2"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension1">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension2">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0, TDimension1, TDimension2>(IEnumerable<TDimension0> dimension0,
            IEnumerable<TDimension1> dimension1, IEnumerable<TDimension2> dimension2, Action<TDimension0, TDimension1, TDimension2> testCallback,
            int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0, dimension1, dimension2);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <typeparam name="TDimension1">Type of objects in <paramref name="dimension1"/></typeparam>
        /// <typeparam name="TDimension2">Type of objects in <paramref name="dimension2"/></typeparam>
        /// <typeparam name="TDimension3">Type of objects in <paramref name="dimension3"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension1">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension2">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension3">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0, TDimension1, TDimension2, TDimension3>(IEnumerable<TDimension0> dimension0,
            IEnumerable<TDimension1> dimension1, IEnumerable<TDimension2> dimension2, IEnumerable<TDimension3> dimension3,
            Action<TDimension0, TDimension1, TDimension2, TDimension3> testCallback,
            int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0, dimension1, dimension2, dimension3);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <typeparam name="TDimension1">Type of objects in <paramref name="dimension1"/></typeparam>
        /// <typeparam name="TDimension2">Type of objects in <paramref name="dimension2"/></typeparam>
        /// <typeparam name="TDimension3">Type of objects in <paramref name="dimension3"/></typeparam>
        /// <typeparam name="TDimension4">Type of objects in <paramref name="dimension4"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension1">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension2">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension3">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension4">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4>(IEnumerable<TDimension0> dimension0,
            IEnumerable<TDimension1> dimension1, IEnumerable<TDimension2> dimension2, IEnumerable<TDimension3> dimension3, IEnumerable<TDimension4> dimension4,
            Action<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4> testCallback,
            int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0, dimension1, dimension2, dimension3, dimension4);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <typeparam name="TDimension1">Type of objects in <paramref name="dimension1"/></typeparam>
        /// <typeparam name="TDimension2">Type of objects in <paramref name="dimension2"/></typeparam>
        /// <typeparam name="TDimension3">Type of objects in <paramref name="dimension3"/></typeparam>
        /// <typeparam name="TDimension4">Type of objects in <paramref name="dimension4"/></typeparam>
        /// <typeparam name="TDimension5">Type of objects in <paramref name="dimension5"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension1">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension2">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension3">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension4">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension5">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5>(IEnumerable<TDimension0> dimension0,
            IEnumerable<TDimension1> dimension1, IEnumerable<TDimension2> dimension2, IEnumerable<TDimension3> dimension3, IEnumerable<TDimension4> dimension4, IEnumerable<TDimension5> dimension5,
            Action<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5> testCallback,
            int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0, dimension1, dimension2, dimension3, dimension4, dimension5);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <typeparam name="TDimension1">Type of objects in <paramref name="dimension1"/></typeparam>
        /// <typeparam name="TDimension2">Type of objects in <paramref name="dimension2"/></typeparam>
        /// <typeparam name="TDimension3">Type of objects in <paramref name="dimension3"/></typeparam>
        /// <typeparam name="TDimension4">Type of objects in <paramref name="dimension4"/></typeparam>
        /// <typeparam name="TDimension5">Type of objects in <paramref name="dimension5"/></typeparam>
        /// <typeparam name="TDimension6">Type of objects in <paramref name="dimension6"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension1">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension2">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension3">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension4">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension5">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension6">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6>(IEnumerable<TDimension0> dimension0, IEnumerable<TDimension1> dimension1,
            IEnumerable<TDimension2> dimension2, IEnumerable<TDimension3> dimension3, IEnumerable<TDimension4> dimension4, IEnumerable<TDimension5> dimension5, IEnumerable<TDimension6> dimension6,
            Action<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6> testCallback,
            int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0, dimension1, dimension2, dimension3, dimension4, dimension5, dimension6);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <typeparam name="TDimension1">Type of objects in <paramref name="dimension1"/></typeparam>
        /// <typeparam name="TDimension2">Type of objects in <paramref name="dimension2"/></typeparam>
        /// <typeparam name="TDimension3">Type of objects in <paramref name="dimension3"/></typeparam>
        /// <typeparam name="TDimension4">Type of objects in <paramref name="dimension4"/></typeparam>
        /// <typeparam name="TDimension5">Type of objects in <paramref name="dimension5"/></typeparam>
        /// <typeparam name="TDimension6">Type of objects in <paramref name="dimension6"/></typeparam>
        /// <typeparam name="TDimension7">Type of objects in <paramref name="dimension7"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension1">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension2">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension3">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension4">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension5">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension6">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension7">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6, TDimension7>(IEnumerable<TDimension0> dimension0, IEnumerable<TDimension1> dimension1,
            IEnumerable<TDimension2> dimension2, IEnumerable<TDimension3> dimension3, IEnumerable<TDimension4> dimension4, IEnumerable<TDimension5> dimension5, IEnumerable<TDimension6> dimension6, IEnumerable<TDimension7> dimension7,
            Action<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6, TDimension7> testCallback,
            int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0, dimension1, dimension2, dimension3, dimension4, dimension5, dimension6, dimension7);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <typeparam name="TDimension1">Type of objects in <paramref name="dimension1"/></typeparam>
        /// <typeparam name="TDimension2">Type of objects in <paramref name="dimension2"/></typeparam>
        /// <typeparam name="TDimension3">Type of objects in <paramref name="dimension3"/></typeparam>
        /// <typeparam name="TDimension4">Type of objects in <paramref name="dimension4"/></typeparam>
        /// <typeparam name="TDimension5">Type of objects in <paramref name="dimension5"/></typeparam>
        /// <typeparam name="TDimension6">Type of objects in <paramref name="dimension6"/></typeparam>
        /// <typeparam name="TDimension7">Type of objects in <paramref name="dimension7"/></typeparam>
        /// <typeparam name="TDimension8">Type of objects in <paramref name="dimension7"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension1">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension2">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension3">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension4">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension5">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension6">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension7">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension8">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6, TDimension7, TDimension8>(IEnumerable<TDimension0> dimension0, IEnumerable<TDimension1> dimension1,
            IEnumerable<TDimension2> dimension2, IEnumerable<TDimension3> dimension3, IEnumerable<TDimension4> dimension4, IEnumerable<TDimension5> dimension5, IEnumerable<TDimension6> dimension6, IEnumerable<TDimension7> dimension7, IEnumerable<TDimension8> dimension8,
            Action<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6, TDimension7, TDimension8> testCallback,
            int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0, dimension1, dimension2, dimension3, dimension4, dimension5, dimension6, dimension7, dimension8);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <typeparam name="TDimension1">Type of objects in <paramref name="dimension1"/></typeparam>
        /// <typeparam name="TDimension2">Type of objects in <paramref name="dimension2"/></typeparam>
        /// <typeparam name="TDimension3">Type of objects in <paramref name="dimension3"/></typeparam>
        /// <typeparam name="TDimension4">Type of objects in <paramref name="dimension4"/></typeparam>
        /// <typeparam name="TDimension5">Type of objects in <paramref name="dimension5"/></typeparam>
        /// <typeparam name="TDimension6">Type of objects in <paramref name="dimension6"/></typeparam>
        /// <typeparam name="TDimension7">Type of objects in <paramref name="dimension7"/></typeparam>
        /// <typeparam name="TDimension8">Type of objects in <paramref name="dimension8"/></typeparam>
        /// <typeparam name="TDimension9">Type of objects in <paramref name="dimension9"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension1">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension2">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension3">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension4">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension5">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension6">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension7">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension8">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension9">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6, TDimension7, TDimension8, TDimension9>(IEnumerable<TDimension0> dimension0, IEnumerable<TDimension1> dimension1,
            IEnumerable<TDimension2> dimension2, IEnumerable<TDimension3> dimension3, IEnumerable<TDimension4> dimension4, IEnumerable<TDimension5> dimension5, IEnumerable<TDimension6> dimension6, IEnumerable<TDimension7> dimension7, IEnumerable<TDimension8> dimension8, IEnumerable<TDimension9> dimension9,
            Action<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6, TDimension7, TDimension8, TDimension9> testCallback,
            int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0, dimension1, dimension2, dimension3, dimension4, dimension5, dimension6, dimension7, dimension8, dimension9);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <typeparam name="TDimension1">Type of objects in <paramref name="dimension1"/></typeparam>
        /// <typeparam name="TDimension2">Type of objects in <paramref name="dimension2"/></typeparam>
        /// <typeparam name="TDimension3">Type of objects in <paramref name="dimension3"/></typeparam>
        /// <typeparam name="TDimension4">Type of objects in <paramref name="dimension4"/></typeparam>
        /// <typeparam name="TDimension5">Type of objects in <paramref name="dimension5"/></typeparam>
        /// <typeparam name="TDimension6">Type of objects in <paramref name="dimension6"/></typeparam>
        /// <typeparam name="TDimension7">Type of objects in <paramref name="dimension7"/></typeparam>
        /// <typeparam name="TDimension8">Type of objects in <paramref name="dimension8"/></typeparam>
        /// <typeparam name="TDimension9">Type of objects in <paramref name="dimension9"/></typeparam>
        /// <typeparam name="TDimension10">Type of objects in <paramref name="dimension10"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension1">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension2">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension3">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension4">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension5">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension6">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension7">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension8">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension9">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension10">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6, TDimension7, TDimension8, TDimension9, TDimension10>(IEnumerable<TDimension0> dimension0, IEnumerable<TDimension1> dimension1,
            IEnumerable<TDimension2> dimension2, IEnumerable<TDimension3> dimension3, IEnumerable<TDimension4> dimension4, IEnumerable<TDimension5> dimension5, IEnumerable<TDimension6> dimension6, IEnumerable<TDimension7> dimension7, IEnumerable<TDimension8> dimension8, IEnumerable<TDimension9> dimension9, IEnumerable<TDimension10> dimension10,
            Action<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6, TDimension7, TDimension8, TDimension9, TDimension10> testCallback,
            int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0, dimension1, dimension2, dimension3, dimension4, dimension5, dimension6, dimension7, dimension8, dimension9, dimension10);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <typeparam name="TDimension0">Type of objects in <paramref name="dimension0"/></typeparam>
        /// <typeparam name="TDimension1">Type of objects in <paramref name="dimension1"/></typeparam>
        /// <typeparam name="TDimension2">Type of objects in <paramref name="dimension2"/></typeparam>
        /// <typeparam name="TDimension3">Type of objects in <paramref name="dimension3"/></typeparam>
        /// <typeparam name="TDimension4">Type of objects in <paramref name="dimension4"/></typeparam>
        /// <typeparam name="TDimension5">Type of objects in <paramref name="dimension5"/></typeparam>
        /// <typeparam name="TDimension6">Type of objects in <paramref name="dimension6"/></typeparam>
        /// <typeparam name="TDimension7">Type of objects in <paramref name="dimension7"/></typeparam>
        /// <typeparam name="TDimension8">Type of objects in <paramref name="dimension8"/></typeparam>
        /// <typeparam name="TDimension9">Type of objects in <paramref name="dimension9"/></typeparam>
        /// <typeparam name="TDimension10">Type of objects in <paramref name="dimension10"/></typeparam>
        /// <typeparam name="TDimension11">Type of objects in <paramref name="dimension11"/></typeparam>
        /// <param name="dimension0">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension1">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension2">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension3">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension4">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension5">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension6">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension7">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension8">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension9">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension10">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="dimension11">Set of test values - only one is selected at a time for specific combination.</param>
        /// <param name="testCallback">Callback for test.</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        public static void RunCombinations<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6, TDimension7, TDimension8, TDimension9, TDimension10, TDimension11>(IEnumerable<TDimension0> dimension0, IEnumerable<TDimension1> dimension1,
            IEnumerable<TDimension2> dimension2, IEnumerable<TDimension3> dimension3, IEnumerable<TDimension4> dimension4, IEnumerable<TDimension5> dimension5, IEnumerable<TDimension6> dimension6, IEnumerable<TDimension7> dimension7, IEnumerable<TDimension8> dimension8, IEnumerable<TDimension9> dimension9, IEnumerable<TDimension10> dimension10, IEnumerable<TDimension11> dimension11,
            Action<TDimension0, TDimension1, TDimension2, TDimension3, TDimension4, TDimension5, TDimension6, TDimension7, TDimension8, TDimension9, TDimension10, TDimension11> testCallback,
            int? skipTo = null)
        {
            RunCombinations(testCallback, skipTo, dimension0, dimension1, dimension2, dimension3, dimension4, dimension5, dimension6, dimension7, dimension8, dimension9, dimension10, dimension11);
        }

        /// <summary>
        /// Wrapper for RunCombinatorialEngineFail that allows to execute testcases defined as anonymous types
        /// </summary>
        /// <param name="testCallback">testCallback</param>
        /// <param name="skipTo">Skips to the variation with the specified number - used for debugging failures.
        /// This will cause a test failure if it's not null and the code is not running under a debugger.</param>
        /// <param name="dimensions">Set of test dimensions that are used to create a single combination.</param>
        public static void RunCombinations(Delegate testCallback, int? skipTo, params IEnumerable[] dimensions)
        {
            ParameterInfo[] parameters = testCallback.Method.GetParameters();
            Dimension[] dimensionInstances = dimensions.Select((dimensionEnumerable, dimensionIndex) =>
                new Dimension(parameters[dimensionIndex].Name, dimensionEnumerable)).ToArray();

            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(dimensionInstances);
            TestUtil.RunCombinatorialEngineFail(engine, (values) =>
            {
                testCallback.DynamicInvoke(parameters.Select(parameter => values[parameter.Name]).ToArray());
            }, skipTo);
        }

        /// <summary>Copies content from one stream into another.</summary>
        /// <param name="source">Stream to read from.</param>
        /// <param name="destination">Stream to write to.</param>
        /// <returns>The number of bytes copied from the source.</returns>
        public static long CopyStream(Stream source, Stream destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            long bytesCopied = 0;
            int bytesRead;
            byte[] buffer = new byte[64 * 1024];
            do
            {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                destination.Write(buffer, 0, bytesRead);
                bytesCopied += bytesRead;
            }
            while (bytesRead > 0);

            return bytesCopied;
        }

        /// <summary>
        /// Compare the content of two streams.
        /// </summary>
        /// <param name="left">stream 1</param>
        /// <param name="right">stream 2</param>
        /// <returns>true if both streams are identical.</returns>
        public static bool CompareStream(Stream left, Stream right)
        {
            if (left == null || !left.CanRead)
            {
                throw new ArgumentException("Must be a valid readable stream.", "left");
            }

            if (right == null || !right.CanRead)
            {
                throw new ArgumentException("Must be a valid readable stream.", "right");
            }

            if (left == right)
            {
                return true;
            }

            if (left.CanSeek)
            {
                left.Position = 0;
            }

            if (right.CanSeek)
            {
                right.Position = 0;
            }

            int bufferSize = 64 * 1024;
            byte[] leftBuffer = new byte[bufferSize];
            byte[] rightBuffer = new byte[bufferSize];
            int leftReadCount = 0;
            int rightReadCount = 0;

            do
            {
                leftReadCount = FillBuffer(leftBuffer, left);
                rightReadCount = FillBuffer(rightBuffer, right);

                if (leftReadCount != rightReadCount)
                {
                    return false;
                }

                for (int idx = 0; idx < leftReadCount; idx++)
                {
                    if (leftBuffer[idx] != rightBuffer[idx])
                    {
                        return false;
                    }
                }
            }
            while (leftReadCount > 0);

            return true;
        }

        private static int FillBuffer(byte[] buffer, Stream stream)
        {
            int bufferSize = buffer.Length;
            int total = 0;
            int count = 0;

            do
            {
                count = stream.Read(buffer, total, bufferSize - total);
                total += count;
            } while (count > 0 && total < bufferSize);

            return total;
        }

        /// <summary>Copies content from one stream into another.</summary>
        /// <param name="source">Stream to read from.</param>
        /// <param name="destination">Stream to write to.</param>
        /// <returns>The number of bytes copied from the source.</returns>
        public static Byte[] ReadStream(Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            int numberOfBytesRead = 0;
            int bytesRead;
            List<byte[]> bytesCopied = new List<byte[]>();
            byte[] buffer;
            do
            {
                buffer = new byte[4000];
                bytesRead = source.Read(buffer, 0, buffer.Length);
                numberOfBytesRead += bytesRead;
                bytesCopied.Add(buffer);
            }
            while (bytesRead == buffer.Length);

            buffer = new byte[numberOfBytesRead];
            for (int i = 0; i < bytesCopied.Count - 1; i++)
            {
                Buffer.BlockCopy(bytesCopied[i], 0, buffer, i * 4000, 4000);
            }

            Buffer.BlockCopy(bytesCopied[bytesCopied.Count - 1], 0, buffer, (bytesCopied.Count - 1) * 4000, bytesRead);
            return buffer;
        }

        /// <summary>Ensures that the specified stream can seek, possibly creating a new one.</summary>
        /// <param name="stream"><see cref="Stream"/> to ensure supports seeking.</param>
        /// <returns><paramref name="stream"/> if CanSeek is true, otherwise an in-memory copy.</returns>
        public static Stream EnsureStreamWithSeek(Stream stream)
        {
            CheckArgumentNotNull(stream, "stream");
            if (stream.CanSeek)
            {
                return stream;
            }
            else
            {
                MemoryStream result = new MemoryStream();
                CopyStream(stream, result);
                result.Position = 0;
                return result;
            }
        }

        /// <summary>Creates variations of documents by modifying a node at a time.</summary>
        /// <param name="document">Document to modify.</param>
        /// <param name="shouldApplyVariation">
        /// Callback to determine whether a variation should be created for a node.
        /// </param>
        /// <param name="applyVariation">
        /// Callback to modify a node which should have a variation applied to it.
        /// </param>
        /// <returns>An enumerable object with pairs of documents and modified nodes.</returns>
        public static IEnumerable<KeyValuePair<XmlDocument, XmlNode>> EnumerateDocumentVariations(
            XmlDocument document,
            Predicate<XmlNode> shouldApplyVariation,
            Action<XmlNode> applyVariation)
        {
            Stack<XmlNode> pendingNodes = new Stack<XmlNode>();
            pendingNodes.Push(document);
            while (pendingNodes.Count > 0)
            {
                XmlNode node = pendingNodes.Pop();
                if (shouldApplyVariation(node))
                {
                    XmlNode savedNode = node.Clone();
                    applyVariation(node);
                    yield return new KeyValuePair<XmlDocument, XmlNode>(document, node);
                    node.ParentNode.ReplaceChild(savedNode, node);
                }

                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    pendingNodes.Push(node.ChildNodes[i]);
                }
            }
        }

        /// <summary>Saves the specified <paramref name="builder"/>.</summary>
        /// <param name="builder">Module to save.</param>
        /// <returns>The path to the saved module.</returns>
        /// <remarks>
        /// <paramref name="builder"/> should have been created with a call to
        /// <see cref="CreateModuleBuilder(string,bool)"/>, passing true for the allowSaving
        /// argument.
        /// </remarks>
        public static string SaveModule(ModuleBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            AssemblyBuilder assembly = builder.Assembly as AssemblyBuilder;
            if (assembly == null)
            {
                throw new InvalidOperationException("Cannot save builder when its Assembly is not an AssemblyBuilder.");
            }

            foreach (Type type in builder.GetTypes())
            {
                TypeBuilder typeBuilder = type as TypeBuilder;
                if (typeBuilder != null && !typeBuilder.IsCreated())
                {
                    typeBuilder.CreateType();
                }
            }

            assembly.Save(builder.ScopeName + ".dll");
            return Path.Combine(DynamicAssemblyDirectory, builder.ScopeName + ".dll");
        }

        /// <summary>Performs a Select on the values of the enumeration.</summary>
        /// <typeparam name="TEnum">Type of enumeration.</typeparam>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectEnumValues<TEnum, TResult>(Func<TEnum, TResult> f)
        {
            Debug.Assert(typeof(TEnum).IsEnum, "typeof(TEnum).IsEnum");
            TEnum[] values = (TEnum[])Enum.GetValues(typeof(TEnum));
            foreach (TEnum value in values)
            {
                yield return f(value);
            }
        }

        public static string StringRepeat(string text, int count)
        {
            if (text == null)
            {
                return null;
            }

            StringBuilder builder = new StringBuilder(text.Length * count);
            for (int i = 0; i < count; i++)
            {
                builder.Append(text);
            }
            return builder.ToString();
        }

        public static void TraceResourceText(string url)
        {
            Trace.WriteLine("Tracing request to \"" + url + "\"...");
            System.Net.WebClient client = new System.Net.WebClient();
            string text = client.DownloadString(url);
            Trace.WriteLine(text);
        }

        /// <summary>Invokes the specified action, discarding traces unless an exception is thrown.</summary>
        /// <param name="description">Action description.</param>
        /// <param name="action">Action to invoke.</param>
        [DebuggerStepThrough]
        public static void TraceScopeForException(string description, Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            StringBuilder builder = new StringBuilder();
            ScopedTraceListener listener = new ScopedTraceListener(builder);
            bool succeeded = false;
            try
            {
                action();
                succeeded = true;
            }
            finally
            {
                listener.Dispose();
                if (!succeeded)
                {
                    Trace.WriteLine("Failed on " + description);
                    Trace.WriteLine(builder.ToString());
                }
            }
        }

        /// <summary>Writes the contents of the stream as text to the Trace object.</summary>
        /// <param name="stream">Stream to write (possibly null).</param>
        /// <returns>The stream that was written, at the starting position.</returns>
        public static Stream TraceStream(Stream stream)
        {
            if (stream == null)
            {
                Trace.WriteLine("<null stream>");
                return null;
            }

            Stream result = EnsureStreamWithSeek(stream);
            StreamReader reader = new StreamReader(result);
            Trace.WriteLine(reader.ReadToEnd());

            result.Position = 0;
            return result;
        }

        /// <summary>Writes the specified node to the Trace debugging object.</summary>
        /// <param name="node">Node to write.</param>
        public static void TraceXml(XmlNode node)
        {
            if (node == null)
            {
                Trace.WriteLine("<null node>");
                return;
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = false;
            settings.CloseOutput = false;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineHandling = NewLineHandling.None;
            StringBuilder output = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(output, settings);
            writer.WriteNode(node.CreateNavigator(), false);
            writer.Flush();
            Trace.WriteLine(output.ToString());
        }

        private static void GenerateCodeUsingEdmToolsApi(string schemaFileName, string codeFileName, IEnumerable<string> dependentSchemaFiles, bool isV2Schema)
        {
#if !ClientSKUFramework

            IList<EdmSchemaError> errors;

            if (isV2Schema)
            {
                EntityCodeGenerator classGenerator = new EntityCodeGenerator(LanguageOption.GenerateCSharpCode);
                errors = classGenerator.GenerateCode(schemaFileName, codeFileName, dependentSchemaFiles);
            }
            else
            {
                EntityClassGenerator classGenerator = new EntityClassGenerator(LanguageOption.GenerateCSharpCode);
                errors = classGenerator.GenerateCode(schemaFileName, codeFileName, dependentSchemaFiles);
            }

            // Only fail if there are errors. We need to ignore the warnings.
            if (errors != null && errors.Count(e => e.Severity == EdmSchemaErrorSeverity.Error) != 0)
            {
                string errorMessage = string.Empty;
                for (int i = 0; i < errors.Count; i++)
                {
                    errorMessage = errorMessage + errors[i].ToString();
                }
                errorMessage += "\r\nSchema file available at " + schemaFileName;

                AstoriaTestLog.FailAndThrow(errorMessage);
            }
#endif
        }

        private static void ChangeGeneratedCode(string csdlSchemaFileName, string fileName, string newFileName, bool isReflectionBasedProvider, string defaultConnectionString, bool isV2Schema)
        {
            string assemblyAttributeLineStart;
            string objectContextBaseType;
            string objectQueryType;

            const string navigationSetterTrimStart = "((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.";
            string dataWebObjectContextBaseType = " : global::AstoriaUnitTests.DataWebObjectContext";
            string publicClassDefinition = "public partial class ";
            string navPropertyDefinition = "global::System.Data.Objects.DataClasses.EntityReference<";
            string iqueryableType = "global::System.Linq.IQueryable";
            string keyAttribute = "EntityKeyProperty=true";
            string objectContextTypeName = string.Empty;

            if (isV2Schema)
            {
                assemblyAttributeLineStart = "[assembly: ";
                objectContextBaseType = " : ObjectContext";
                objectQueryType = "ObjectSet";
            }
            else
            {
                assemblyAttributeLineStart = "[assembly: global::System.Data.Objects.";
                objectContextBaseType = " : global::System.Data.Objects.ObjectContext";
                objectQueryType = "global::System.Data.Objects.ObjectQuery";
            }

            if (defaultConnectionString == null)
            {
                string path = Path.GetDirectoryName(csdlSchemaFileName);
                string ssdlSchemaFileName = Path.ChangeExtension(csdlSchemaFileName, "ssdl");
                string mslSchemaFileName = Path.ChangeExtension(csdlSchemaFileName, "msl");
                string metadataFiles = csdlSchemaFileName;
                if (File.Exists(ssdlSchemaFileName) && File.Exists(mslSchemaFileName))
                {
                    metadataFiles = String.Format("{0}|{1}|{2}", csdlSchemaFileName, mslSchemaFileName, ssdlSchemaFileName);
                }
                defaultConnectionString = String.Format(ConnectionString, metadataFiles);
            }
            else
            {
                defaultConnectionString = defaultConnectionString.Replace("\\", "\\\\");
                defaultConnectionString = "\"" + defaultConnectionString.Replace("\"", "\\\"") + "\"";
            }

            using (StreamReader reader = new StreamReader(fileName))
            using (StreamWriter writer = new StreamWriter(newFileName, false))
            {
                bool isConfigConnectionStringPartiallyDefined = false;
                bool isEntityReferencePropertyEncountered = false;
                bool skipUntilClosingBrace = false;

                if (isReflectionBasedProvider)
                {
                    writer.WriteLine("// Adapted to work as a plain CLR-based model in TestUtil.ChangeGeneratedCode");
                }

                string input;
                while ((input = reader.ReadLine()) != null)
                {
                    string trimmedInput = input.Trim();
                    if (skipUntilClosingBrace)
                    {
                        if (trimmedInput == "}")
                        {
                            skipUntilClosingBrace = false;
                        }
                        else
                        {
                            input = null;
                        }
                    }
                    else if (isReflectionBasedProvider && input.StartsWith(assemblyAttributeLineStart))
                    {
                        input = null;
                    }
                    else if (isReflectionBasedProvider && trimmedInput.StartsWith(navigationSetterTrimStart))
                    {
                        input = "// The navigation property would have been set here.";
                        skipUntilClosingBrace = true;
                    }
                    else if (isReflectionBasedProvider && isEntityReferencePropertyEncountered && input.EndsWith(";"))
                    {
                        input = input.Replace(";", ".Value;");
                        isEntityReferencePropertyEncountered = false;
                    }
                    else if (isConfigConnectionStringPartiallyDefined)
                    {
                        int startingTruncatedIndex = input.IndexOf("\"");
                        int endingTruncationIndex = input.IndexOf(",");
                        input = input.Remove(startingTruncatedIndex, endingTruncationIndex - startingTruncatedIndex);
                        isConfigConnectionStringPartiallyDefined = false;
                    }
                    else if (input.Contains(objectContextBaseType))
                    {
                        int objectContextTypeNameEndIndex = input.IndexOf(objectContextBaseType);
                        if (isReflectionBasedProvider)
                        {
                            input = input.Replace(objectContextBaseType, dataWebObjectContextBaseType);
                        }
                        int objectContextTypeNameStartingIndex = input.IndexOf(publicClassDefinition) + publicClassDefinition.Length;
                        objectContextTypeName = input.Substring(objectContextTypeNameStartingIndex, objectContextTypeNameEndIndex - objectContextTypeNameStartingIndex);
                    }
                    else if (input.Contains("\"name="))
                    {
                        string connectionString = defaultConnectionString;
                        int parameterStartIndex = input.IndexOf("\"name=");
                        isConfigConnectionStringPartiallyDefined = input.EndsWith("+");
                        if (isConfigConnectionStringPartiallyDefined)
                        {
                            input = input.Replace(input.Substring(parameterStartIndex), connectionString);
                        }
                        else
                        {
                            string parameterName = "\"name=" + objectContextTypeName + "\"";
                            input = input.Replace(parameterName, connectionString);
                        }
                    }
                    else if (isReflectionBasedProvider && input.Contains(navPropertyDefinition))
                    {
                        input = input.Replace(navPropertyDefinition, String.Empty);
                        input = input.Replace(">", String.Empty);
                        isEntityReferencePropertyEncountered = true;
                    }
                    else if (isReflectionBasedProvider && input.Contains(keyAttribute))
                    {
                        // Write the data web key attribute
                        int whiteSpacesIndex = input.IndexOf("[");
                        writer.Write(input.Substring(0, whiteSpacesIndex) + "[global::Microsoft.OData.Client.KeyAttribute()]");
                        writer.Write(Environment.NewLine);
                    }
                    else if (isReflectionBasedProvider)
                    {
                        input = input.Replace(objectQueryType, iqueryableType);
                    }

                    if (input != null)
                    {
                        writer.WriteLine(input);
                    }
                }
            }
        }

        public static void GenerateAssembly(string[] sourceFileNames, string assemblyName, IEnumerable<string> dependentAssembliesLocation)
        {
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.CompilerResults results = provider.CompileAssemblyFromFile(GetCompilerParameters(assemblyName, dependentAssembliesLocation), sourceFileNames);
            if (results.Errors != null && results.Errors.Count != 0)
            {
                string errorMessage = string.Empty;
                for (int i = 0; i < results.Errors.Count; i++)
                {
                    errorMessage = errorMessage + results.Errors[i].ToString();
                }
                AstoriaTestLog.FailAndThrow(errorMessage);
            }
        }

        public static void GenerateAssembly(string source, string assemblyName, IEnumerable<string> dependentAssembliesLocation)
        {
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.CompilerResults results = provider.CompileAssemblyFromSource(GetCompilerParameters(assemblyName, dependentAssembliesLocation), source);
            if (results.Errors != null && results.Errors.Count != 0)
            {
                string errorMessage = string.Empty;
                for (int i = 0; i < results.Errors.Count; i++)
                {
                    errorMessage = errorMessage + results.Errors[i].ToString();
                }
                AstoriaTestLog.FailAndThrow(errorMessage);
            }
        }

        /// <summary>Gets the full type name for the specified type, without assembly information.</summary>
        /// <param name="type">Type to get information for.</param>
        /// <param name="firstNeedsNamespace">Whether the first type includes the namespace.</param>
        /// <returns>The full type name for the specified type, without assembly information.</returns>
        public static string GetTypeName(Type type, bool firstNeedsNamespace)
        {
            StringBuilder result = new StringBuilder();
            GetTypeName(type, firstNeedsNamespace, result);
            return result.ToString();
        }

        private static System.CodeDom.Compiler.CompilerParameters GetCompilerParameters(string assemblyName, IEnumerable<string> dependentAssembliesLocation)
        {
            System.CodeDom.Compiler.CompilerParameters options = new System.CodeDom.Compiler.CompilerParameters();
            options.OutputAssembly = assemblyName;
            options.GenerateExecutable = false;
            options.GenerateInMemory = false;
            options.IncludeDebugInformation = false;
            string path = Path.GetDirectoryName(typeof(System.Data.Test.Astoria.TestUtil).Assembly.Location);
            options.ReferencedAssemblies.Add(Path.Combine(GreenBitsReferenceAssembliesDirectory, "System.dll"));
            options.ReferencedAssemblies.Add(Path.Combine(GreenBitsReferenceAssembliesDirectory, "System.Xml.dll"));
            options.ReferencedAssemblies.Add(Path.Combine(GreenBitsReferenceAssembliesDirectory, "System.Data.dll"));
            options.ReferencedAssemblies.Add(Path.Combine(GreenBitsReferenceAssembliesDirectory, "System.Runtime.Serialization.dll"));
            options.ReferencedAssemblies.Add(Environment.ExpandEnvironmentVariables(System.IO.Path.Combine(DataFxAssemblyRef.File.DE_ReferenceAssemblyPath, DataFxAssemblyRef.File.DataEntity)));
            options.ReferencedAssemblies.Add(Path.Combine(path, DataFxAssemblyRef.File.SpatialCore));
            options.ReferencedAssemblies.Add(Path.Combine(GreenBitsReferenceAssembliesDirectory, "System.Core.dll"));
            options.ReferencedAssemblies.Add(Path.Combine(path, DataFxAssemblyRef.File.ODataLib));
            options.ReferencedAssemblies.Add(Path.Combine(path, DataFxAssemblyRef.File.DataServicesClient));
            options.ReferencedAssemblies.Add(Path.Combine(path, DataFxAssemblyRef.File.DataServices));
            options.ReferencedAssemblies.Add(Path.Combine(path, "AstoriaUnitTests.dll"));
            options.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            if (dependentAssembliesLocation != null)
            {
                foreach (string assemblyLocation in dependentAssembliesLocation)
                {
                    options.ReferencedAssemblies.Add(assemblyLocation);
                }
            }
            options.TreatWarningsAsErrors = true;
            return options;
        }

        /// <summary>Gets the full type name for the specified type, without assembly information.</summary>
        /// <param name="type">Type to get information for.</param>
        /// <param name="needsNamespace">Whether the namespace is required.</param>
        /// <param name="resultBuilder">StringBuilder to which the type name should be written.</param>
        private static void GetTypeName(Type type, bool needsNamespace, StringBuilder resultBuilder)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(resultBuilder != null, "resultBuilder != null");

            // Every type but the first will require a namespace qualification.
            // The first type is qualified by the Web3SBase namespace.
            if (needsNamespace)
            {
                resultBuilder.Append(type.Namespace);
                resultBuilder.Append('.');
            }

            resultBuilder.Append(type.Name);

            // Add generic argument information as necessary.
            if (type.IsGenericType)
            {
                resultBuilder.Append('[');

                Type[] genericTypes = type.GetGenericArguments();
                for (int i = 0; i < genericTypes.Length; i++)
                {
                    if (i > 0)
                    {
                        resultBuilder.Append(' ');
                    }

                    GetTypeName(genericTypes[i], true /* needsNamespace */, resultBuilder);
                }

                resultBuilder.Append(']');
            }
        }

        /// <summary>Joins the specified values, separating them with the given text.</summary>
        /// <param name="separator">Text to use as separator.</param>
        /// <param name="values">Values to join.</param>
        /// <returns>The given <paramref name="values"/> joined by the specified <paramref name="separator"/>.</returns>
        public static string Join(string separator, IEnumerable<string> values)
        {
            CheckArgumentNotNull(separator, "separator");
            CheckArgumentNotNull(values, "values");
            StringBuilder builder = new StringBuilder();
            foreach (string s in values)
            {
                if (builder.Length > 0)
                {
                    builder.Append(separator);
                }
                builder.Append(s);
            }
            return builder.ToString();
        }

        private static Type MetadataCacheType
        {
            get
            {
                if (providerMetadataCacheType == null)
                {
                    providerMetadataCacheType = DataWebAssembly.GetType("Microsoft.OData.Service.Caching.MetadataCache`1", true);
                    providerMetadataCacheType = providerMetadataCacheType.MakeGenericType(DataWebAssembly.GetType("Microsoft.OData.Service.Caching.ProviderMetadataCacheItem", true));
                }
                return providerMetadataCacheType;
            }
        }

        private static Type ConfigurationCacheType
        {
            get
            {
                if (configurationCacheType == null)
                {
                    configurationCacheType = DataWebAssembly.GetType("Microsoft.OData.Service.Caching.MetadataCache`1", true);
                    Type dataServiceConfigurationType = DataWebAssembly.GetType("Microsoft.OData.Service.Caching.DataServiceCacheItem", true);
                    configurationCacheType = configurationCacheType.MakeGenericType(dataServiceConfigurationType);
                }
                return configurationCacheType;
            }
        }

        /// <summary>Return true if the current process does not have administrator authority.</summary>
        public static bool shouldWorkaroundDueToVistaUAC()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return false;
            }
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principle = new WindowsPrincipal(currentIdentity);
            SecurityIdentifier sidAdmin = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
            if (!principle.IsInRole(sidAdmin))
            {
                return true;
            }
            return false;
        }

        /// <summary>Execute a program through Commander.</summary>
        /// <param name="programPath">Path and name of the program to run.</param>
        /// <param name="arguments">Arguments to the program.</param>
        /// <returns>ExecutableResults object containing exitcode and output.</returns>
        public static ExecutableResults runInCommander(string programPath, string arguments)
        {
            Process[] processes = Process.GetProcessesByName("Commander.Server");
            if (processes.Length > 0)
            {
                TcpChannel channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, true);
                Commander.RemoteServer remoteServer = Commander.RemoteServer.CreateLoggedInClientRemoteServer("localhost");

                Commander.RemoteExecutableResults results = remoteServer.ExecuteFile(programPath, arguments);
                ExecutableResults executableResults = new ExecutableResults(results.ExitCode, results.Output);

                AstoriaTestLog.TraceLine(results.Output);
                ChannelServices.UnregisterChannel(channel);

                return executableResults;
            }
            else
                throw new TestFailedException("Expected a RemoteServer program called Commander.Server.exe to be running");
        }

        public static bool CompareMimeType(string mimeType1, string mimeType2)
        {
            return mimeType1.StartsWith(mimeType2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Provides an IDisposable type that can be used to capture all tracing.</summary>
        internal sealed class ScopedTraceListener : TraceListener, IDisposable
        {
            /// <summary>Builder to which tracing should be redirected.</summary>
            private readonly StringBuilder builder;

            /// <summary>Whether the listener is installed.</summary>
            private bool installed;

            /// <summary>Original listeners to be restored.</summary>
            private List<TraceListener> listeners;

            /// <summary>Initializes a new listener with the specified (possibly null) StringBuilder.</summary>
            /// <param name="builder">Builder to which tracing should be redirected.</param>
            internal ScopedTraceListener(StringBuilder builder)
            {
                this.builder = builder;
            }

            /// <summary>Sets this up as the only listener.</summary>
            [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
            [SecuritySafeCritical]
            public void Install()
            {
                if (!this.installed)
                {
                    this.listeners = new List<TraceListener>();
                    foreach (TraceListener listener in Trace.Listeners)
                    {
                        this.listeners.Add(listener);
                    }

                    Trace.Listeners.Clear();
                    Trace.Listeners.Add(this);
                    this.installed = true;
                }
            }

            /// <summary>Stops listening and reinstalls original listeners.</summary>
            [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
            [SecuritySafeCritical]
            public void Uninstall()
            {
                if (installed)
                {
                    Trace.Listeners.Remove(this);
                    Trace.Listeners.AddRange(this.listeners.ToArray());
                    this.listeners = null;
                    this.installed = false;
                }
            }

            /// <summary>Called when failure happens. For example Debug.Fail calls this, or Debug.Assert.</summary>
            /// <param name="message">The message of the failure.</param>
            /// <param name="detailMessage">Detailed message information.</param>
            public override void Fail(string message, string detailMessage)
            {
                base.Fail(message, detailMessage);
                AstoriaTestLog.FailAndThrow(message + (detailMessage == null ? "" : " " + detailMessage));
            }

            /// <summary>Writes the specified message to the trace target.</summary>
            /// <param name="message">Message to write.</param>
            public override void Write(string message)
            {
                if (builder != null)
                {
                    builder.Append(message);
                }
            }

            /// <summary>Writes the specified message to the trace target.</summary>
            /// <param name="message">Message to write.</param>
            public override void WriteLine(string message)
            {
                if (builder != null)
                {
                    builder.AppendLine(message);
                }
            }

            /// <summary>Restores original listeners.</summary>
            void IDisposable.Dispose()
            {
                Uninstall();
            }
        }

        public static IDisposable MetadataCacheCleaner()
        {
            return new MetadataCacheResetter();
        }

        private class MetadataCacheResetter : IDisposable
        {
            internal MetadataCacheResetter()
            {
                TestUtil.ClearMetadataCache();
                TestUtil.ClearConfiguration();
            }

            public void Dispose()
            {
                TestUtil.ClearMetadataCache();
                TestUtil.ClearConfiguration();
            }
        }

        internal class StaticValueRestorer : IDisposable
        {
            private Dictionary<MemberInfo, object> membersToRestore = new Dictionary<MemberInfo, object>();

            internal StaticValueRestorer(params MemberInfo[] memberInfos)
            {
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    object value = null;
                    if (memberInfo is PropertyInfo)
                    {
                        value = ((PropertyInfo)memberInfo).GetValue(null, EmptyObjectArray);
                        if (IsRestorable(((PropertyInfo)memberInfo).PropertyType))
                        {
                            value = ((IRestorable)value).Restore();
                        }
                    }
                    else
                    {
                        FieldInfo fieldInfo = memberInfo as FieldInfo;
                        Debug.Assert(fieldInfo != null, "fieldInfo != null");
                        if (fieldInfo.IsLiteral || (IsRestorable(fieldInfo.FieldType) && fieldInfo.IsPrivate))
                        {
                            // We do not need to restore const fields
                            // We do not need to restore Restorable private fields as those are very likely exposed as properties.
                            continue;
                        }

                        value = fieldInfo.GetValue(null);
                    }

                    membersToRestore.Add(memberInfo, value);
                }
            }

            public void Dispose()
            {
                foreach (KeyValuePair<MemberInfo, object> member in this.membersToRestore)
                {
                    if (member.Key is PropertyInfo)
                    {
                        object value = member.Value;
                        if (IsRestorable(((PropertyInfo)member.Key).PropertyType))
                        {
                            Debug.Assert(value is IDisposable, "Property '" + member.Key.ToString() + "' is IRestorable but we didn't store IDisposable for it.");
                            ((IDisposable)value).Dispose();
                        }
                        else
                        {
                            ((PropertyInfo)member.Key).SetValue(null, member.Value, EmptyObjectArray);
                        }
                    }
                    else
                    {
                        FieldInfo field = member.Key as FieldInfo;
                        Debug.Assert(
                            field != null && !(field.IsLiteral || (IsRestorable(field.FieldType) && field.IsPrivate)),
                            "field != null && !(field.IsLiteral || (IsRestorable(field.FieldType) && field.IsPrivate)");
                        field.SetValue(null, member.Value);
                    }
                }
            }

            private static bool IsRestorable(Type type)
            {
                return typeof(IRestorable).IsAssignableFrom(type);
            }
        }
    }

    /// <summary>Interface adding the ability for a given value to be restored.</summary>
    /// <remarks>This is used by tests to restore the state of things to the original values, so that other tests are not affected
    /// by the changes this test made.</remarks>
    public interface IRestorable
    {
        IDisposable Restore();
    }

    /// <summary>Implementation of IRestorable over a stored value.</summary>
    /// <typeparam name="T">The type of the value stored.</typeparam>
    /// <remarks>Use this class to store a value of type <typeparamref name="T"/> instead of storing it directly if
    /// such value might need to be restored after a test.
    /// The suggested pattern is to have a static private field storing the Restorable instance and a public property with only a getter
    /// returning such instance.
    /// For example:
    /// <![CDATA[
    /// private static Restorable<int> magicalSetting = new Restorable<int>(42);
    /// public static Restorable<int> MagicalSetting { get { return magicalSetting; } }
    /// ]]>
    /// </remarks>
    public class Restorable<T> : IRestorable
    {
        /// <summary>The storage for the actual value.</summary>
        private T value;

        /// <summary>The number of restorable states created over this instance of restorable.</summary>
        /// <remarks>If this value is zero (no states are stored anywhere), the value can't be set (as it would not be able to get restored).</remarks>
        private int restorableStatesCount;

        /// <summary>Constructor which sets the value to its type's deafult value.</summary>
        public Restorable()
        {
            this.value = default(T);
        }

        /// <summary>Constructor wich sets the stored value to the specified <paramref name="value"/>.</summary>
        /// <param name="value">The value to set as stored.</param>
        public Restorable(T value)
        {
            this.value = value;
        }

        /// <summary>Implicit conversion to the stored value type.</summary>
        /// <param name="restorable">The instacne of the restorable to convert to its value.</param>
        /// <returns>The stored value of the restorable.</returns>
        /// <remarks>This allows to read from a field/property of the restorable type as if it's of the stored value type.</remarks>
        public static implicit operator T(Restorable<T> restorable)
        {
            return restorable.value;
        }

        /// <summary>Property which exposes the underlying stored value. The setter on this property
        /// allows modifications of the stored value only if there an existing restorable state.</summary>
        public T Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.restorableStatesCount <= 0)
                {
                    Debug.Fail("Trying to set a restorable property without the using(Restore) around it.");
                    TestLog.Compare(false, "Trying to set a restorable property without the using(Restore) around it.");
                }
                this.value = value;
            }
        }

        /// <summary>Creates a new restorable state which is returned.</summary>
        /// <returns>New restorable state for this restorable value.</returns>
        /// <remarks>Typically this is used inside a using statement to get the Dispose called automatically on the returned restorable state.</remarks>
        public IDisposable Restore()
        {
            return new RestorableState(this);
        }

        /// <summary>Class which stores a state for a given restorable.</summary>
        /// <remarks>This class implements <see cref="IDisposable"/> which when disposed restored the attached restorable
        /// to the value stored upon construction of this object.</remarks>
        private class RestorableState : IDisposable
        {
            /// <summary>The stored value to be restored upon dispose.</summary>
            private T storedValue;

            /// <summary>The restorable instance this state is attached to.</summary>
            private Restorable<T> restorable;

            /// <summary>Constructor which stores current value of the restorable.</summary>
            /// <param name="restorable">The restorable to create the restorable state from.</param>
            public RestorableState(Restorable<T> restorable)
            {
                this.restorable = restorable;
                System.Threading.Interlocked.Increment(ref this.restorable.restorableStatesCount);
                this.storedValue = this.restorable.value;
            }

            /// <summary>Dispose method which restores the value of the attached restorable to the one remembered upon creation of this object.</summary>
            public void Dispose()
            {
                this.restorable.value = this.storedValue;
                if (System.Threading.Interlocked.Decrement(ref this.restorable.restorableStatesCount) < 0)
                {
                    Debug.Fail("Probably calling dispose twice on the same object.");
                    TestLog.Compare(false, "Probably calling dispose twice on the same object.");
                }
                GC.SuppressFinalize(this);
            }

            /// <summary>Finalizer - this should get never called as each instance should be properly disposed.</summary>
            ~RestorableState()
            {
                Debug.Fail("Restorable state which wasn't Disposed of properly. You should use using or equivalent functionality to dispose of the restorable states.");
                TestLog.Compare(false, "Restorable state which wasn't Disposed of properly. You should use using or equivalent functionality to dispose of the restorable states.");
            }
        }
    }
}
