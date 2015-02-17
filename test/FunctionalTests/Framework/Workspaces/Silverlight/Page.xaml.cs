//---------------------------------------------------------------------
// <copyright file="Page.xaml.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace SilverlightAstoriaTest
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using TestSL;
    using Microsoft.OData.Client;
    using RemoteClient;
    using WrapperTypes;

    public partial class Page : UserControl
    {
        private static string _ClientID = "";
        private static List<string> processedMessages = new List<string>();
        public static string ClientID
        {
            get
            {
                if (_ClientID.Length == 0)
                {
                    _ClientID = System.Guid.NewGuid().ToString();
                }
                return _ClientID;
            }
        }
        public static List<KeyValuePair<String, String>> UIMessages;


        private static Uri MessagesUri = new Uri("Messages?$filter=WhoSentMe eq 'Test'", UriKind.Relative);

        public TestLanguageEnum GetTestLanguage()
        {
            TestLanguageEnum tl = TestLanguageEnum.CSHARP;
            string uriQueryString = HtmlPage.Document.DocumentUri.Query.ToString();
            if (uriQueryString.Length == 0)
                tl = TestLanguageEnum.CSHARP;
            else
                tl = uriQueryString.ToLower().Contains("csharp") ? TestLanguageEnum.CSHARP : TestLanguageEnum.VB;
            return tl;

        }
        public Page()
        {
            InitializeComponent();
            UIMessages = new List<KeyValuePair<String, String>>();

            TestMQConnection();

            #region Setup Timer to monitor for messages
            DispatcherTimer dispatchTimer = new DispatcherTimer();
            dispatchTimer.Tick += new EventHandler(dispatchTimer_Tick);
            dispatchTimer.Interval = new TimeSpan(0, 0, 0, 1);
            dispatchTimer.Start();
            #endregion Setup Timer to monitor for messages

            #region Hookup to Events for Test Run
            RemoteClient.RemoteClientExecutionMode = ExecutionModes.Async;
            RemoteClient.TestLanguage = GetTestLanguage();
            //TestLanguageEnum.CSHARP;
            RemoteClient.EndExecution += new RemoteClient.ExecutionEnd(RemoteClient_EndExecution);
            RemoteClient.StartExecution += new RemoteClient.ExecutionStart(RemoteClient_StartExecution);
            RemoteClient.ErrorInExecution += new RemoteClient.ExecutionError(RemoteClient_ErrorInExecution);

            #endregion Hookup to Events for Test Run
        }

        #region Test Events , Start , End , Error
        void RemoteClient_ErrorInExecution(string MethodName, Message request, Exception Error)
        {
            Message response = new Message();
            response.Method = MethodName;
            if (Error != null)
            {
                response.Exception = Error.ToString();
            }
            response.WhoSentMe = "SL";
            PushResponseToQueue(request, response);
        }

        void PushResponseToQueue(Message request, Message response)
        {
            DataServiceContext AstoriaTestService = null;
            AstoriaTestService = GetServiceContext();
            AstoriaTestService.UsePostTunneling = true;
            AstoriaTestService.AttachTo("Messages", request);
            AstoriaTestService.DeleteObject(request);
            //response.MessageID = 5000;
            AstoriaTestService.AddObject("Messages", response);
            AstoriaTestService.BeginSaveChanges(SaveChangesOptions.None, PostTestWorkRespose, AstoriaTestService);

        }

        void RemoteClient_StartExecution(string MethodName)
        {
            SignalUI("TestStatus", String.Format(" Test Method {0} is running now ", MethodName));
        }

        void RemoteClient_EndExecution(string MethodName, Message request, Message response)
        {
            PushResponseToQueue(request, response);

        }
        #endregion Test Events , Start , End , Error

        private DataServiceContext GetServiceContext()
        {
            string serverVDURI = HtmlPage.Document.DocumentUri.LocalPath.Substring(0, HtmlPage.Document.DocumentUri.LocalPath.LastIndexOf('/') + 1)
                    + "AstoriaTestSilverlight.svc";

            Uri serviceRoot = new Uri(serverVDURI, UriKind.RelativeOrAbsolute);
            return new DataServiceContext(serviceRoot);
        }

        private void ClaimTest(Message testMessage)
        {
            DataServiceContext AstoriaTestService = null;
            AstoriaTestService = GetServiceContext();
            AstoriaTestService.UsePostTunneling = true;
            AstoriaTestService.AttachTo("Messages", testMessage);
            AstoriaTestService.BeginSaveChanges(PostTestWorkRespose, AstoriaTestService);
        }

        void dispatchTimer_Tick(object sender, EventArgs e)
        {
            DataServiceContext AstoriaTestService = null;
            AstoriaTestService = GetServiceContext();
            AstoriaTestService.BeginExecute<Message>(MessagesUri, PollForTestWorkMessages, AstoriaTestService);

        }
        private void PostTestWorkRespose(IAsyncResult asyncResult)
        {

            try
            {
                DataServiceContext asynchConnector = asyncResult.AsyncState as DataServiceContext;
                DataServiceResponse response = asynchConnector.EndSaveChanges(asyncResult);
                foreach (ChangeOperationResponse changset in response)
                {
                    if (changset.Error != null)
                    {
                        throw changset.Error;
                    }
                }
                SignalUI("TestStatus", "Sent Response");

            }
            catch (Exception exception)
            {
                string wrongError = "The context is already tracking a different entity with the same resource Uri.";
                if (!exception.InnerException.Message.Contains(wrongError))
                {
                    SignalUI("TestStatus", "Failed : " + exception.StackTrace.ToString());
                    SignalUI("TestStatus", "Failed : " + exception.InnerException.StackTrace.ToString());
                    DataServiceContext dsc = GetServiceContext();
                    Message response = new Message();
                    response.ReturnValue = exception.ToString();
                    response.WhoSentMe = "SL";
                    dsc.AddObject("Messages", response);
                    dsc.BeginSaveChanges(PostTestWorkRespose, dsc);
                }
            }
        }

        private void PollForTestWorkMessages(IAsyncResult asyncResult)
        {
            Message response = null;
            Message message = null;
            try
            {
                DataServiceContext asynchConnector = asyncResult.AsyncState as DataServiceContext;
                IEnumerable<Message> messages = asynchConnector.EndExecute<Message>(asyncResult);
                message = messages.FirstOrDefault<Message>();

                SignalUI("PollingStatus", "Started");

                if (null != message && !processedMessages.Any<string>(pm => pm == message.MessageID.ToString()))
                {
                    processedMessages.Add(message.MessageID.ToString());

                    #region ClaimTest
                    //If the test is not claimed , then claim it .
                    //if (message.TestClientID.Length == 0)
                    //{
                    //    ClaimTest(message);
                    //}
                    //if (message.TestClientID.Length > 0 && message.TestClientID == ClientID)
                    //{
                    #endregion

                    SignalUI("MQConnectionStatus", "Pass");
                    response = new Message();
                    response.WhoSentMe = "SL";
                    response.Method = message.Method;
                    RemoteClient.Dispatch(message, response);
                    //}
                }
            }
            catch (Exception exception)
            {
                SignalUI("TestStatus", "Failed : " + exception.StackTrace.ToString());

                Message Response = new Message();
                Response.Method = message.Method;
                Response.WhoSentMe = "SL";
                PushResponseToQueue(message, Response);

                //DataServiceContext dsc = GetServiceContext();
                //Message response2 = new Message();
                //response.ReturnValue = exception.ToString();
                //response.WhoSentMe = "SL";
                //response.Method = message.Method;
                //dsc.AddObject("Messages", response2);

                //dsc.BeginSaveChanges(PostTestWorkRespose, dsc);
            }
            finally
            {

            }
        }

        private void SignalUI(string key, string value)
        {
            TextBlock messageBlock = this.FindName(key) as TextBlock;
            messageBlock.Text = value;
        }

        #region Test Connection to Message Queue
        private void TestMQConnection()
        {
            DataServiceContext testDataContext = GetServiceContext();
            testDataContext.BeginExecute<TestSL.Message>(new Uri("Messages", UriKind.RelativeOrAbsolute), MQCOnnectionCallback, testDataContext);
        }
        public void MQCOnnectionCallback(IAsyncResult asynchResult)
        {
            try
            {
                DataServiceContext testDataContext = asynchResult.AsyncState as DataServiceContext;
                var messages = testDataContext.EndExecute<TestSL.Message>(asynchResult);
                if (messages.Count<TestSL.Message>() >= 0)
                {
                    SignalUI("MQConnectionStatus", "Pass");
                }

            }
            catch (Exception exception)
            {
                SignalUI("MQConnectionStatus", "Fail with " + exception.Message);
            }

        }
        #endregion Test Connection to Message Queue

    }
}
