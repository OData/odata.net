//---------------------------------------------------------------------
// <copyright file="MaterializationTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Client;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Suites.Data.Test;

    public partial class ClientModule
    {
        /// <summary>
        /// Summary description for MaterializationTest
        /// </summary>
        [TestClass]
        public class MaterializationTest
        {
            public class TestCompletedEventArgs : EventArgs
            {
                public string Status { get; set; }
                public TestCompletedEventArgs(string status)
                {
                    this.Status = status;
                }
            }

            [System.Runtime.InteropServices.ComVisible(true)]
            public class SilverlightExternal
            {
                internal event EventHandler<TestCompletedEventArgs> TestCompletedEvent;

                public void TestCompleted(string text)
                {
                    if (TestCompletedEvent != null)
                    {
                        TestCompletedEvent(this, new TestCompletedEventArgs(text));
                    }
                }
            }

            //[TestMethod]
            public void SilverlightBasicTest()
            {
                // setup northwind database
                ServiceModelData modelData = ServiceModelData.Values[0];
                using (TestWebRequest request = TestWebRequest.CreateForLocal())
                {
                    request.DataServiceType = modelData.ServiceModelType;
                    request.StartService();

                    #region copy compiled binaries for Silverlight request

                    string baseUri = request.BaseUri;
                    Assert.IsTrue(baseUri.EndsWith(".svc", StringComparison.Ordinal));

                    string FileTargetPath = LocalWebServerHelper.FileTargetPath;
                    Assert.IsTrue(!FileTargetPath.EndsWith("\\"));

                    string SilverlightSrcPath = LocalWebServerHelper.BinarySourcePath + "\\Silverlight";
                    string ClientBinPath = FileTargetPath + "\\ClientBin";

                    string startPage = "Astoria.Silverlight.html";
                    string requestUri = baseUri.Substring(0, baseUri.LastIndexOf('/') + 1) + "ClientBin/" + startPage;

                    string[] binariesToCopy = new string[] { // to ClientBinPath
                        DataFxAssemblyRef.File.DataServicesClient,
                        DataFxAssemblyRef.File.DataServicesClient.Replace(".dll", ".pdb"),
                        "Astoria.Silverlight.UnitTests.dll",
                        "Astoria.Silverlight.UnitTests.pdb",
                    };

                    string[] pagesToCopy = new string[] { // to FileTargetPath
                        "Astoria.Silverlight.xaml",
                        startPage,
                        startPage + ".js",
                        "Silverlight.js",
                    };

                    foreach (string file in binariesToCopy)
                    {
                        Assert.IsTrue(File.Exists(SilverlightSrcPath + "\\" + file), file);
                    }

                    foreach (string file in pagesToCopy)
                    {
                        Assert.IsTrue(File.Exists(SilverlightSrcPath + "\\" + file), file);
                    }

                    Directory.CreateDirectory(ClientBinPath);

                    foreach (string file in binariesToCopy)
                    {
                        File.Copy(
                            SilverlightSrcPath + "\\" + file,
                            ClientBinPath + "\\" + file,
                            /*overwrite*/true);
                    }

                    foreach (string binary in pagesToCopy)
                    {
                        File.Copy(
                            SilverlightSrcPath + "\\" + binary,
                            ClientBinPath + "\\" + binary,
                            /*overwrite*/true);
                    }

                    #endregion

                    string status = "missing";

                    Thread uiThread = new Thread(delegate(object o)
                    {
                        Form form = new Form();
                        form.Width = 640;
                        form.Height = 480;
                        WebBrowser browser = new WebBrowser();
                        browser.Width = form.Width;
                        browser.Height = form.Height;
                        form.Controls.Add(browser);
                        browser.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                        SilverlightExternal scriptingObject = new SilverlightExternal();
                        scriptingObject.TestCompletedEvent += delegate(object sender, TestCompletedEventArgs e)
                        {
                            status = e.Status;
#if false
                            Action action = delegate { form.Text = e.Status; };
                            form.Invoke(action);
#else
                            form.Close();
#endif
                        };

                        browser.ObjectForScripting = scriptingObject;

                        form.Load += delegate(object sender, EventArgs e)
                        {
                            browser.Url = new Uri(requestUri);
                            form.BringToFront();
                        };
                        form.ShowDialog();
                        return;
                    });

                    uiThread.SetApartmentState(ApartmentState.STA);
                    uiThread.Start();
                    if (!uiThread.Join(new TimeSpan(0, 3, 0)))
                    {
                        status = "timeout";
                    }

                    if (status != "test finished")
                    {
                        throw new Exception(status);
                    }
                }
            }
        }
    }
}
