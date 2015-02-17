//---------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using Microsoft.Phone.Controls;
    using Microsoft.VisualStudio.TestPlatform.Core;
    using Microsoft.VisualStudio.TestPlatform.TestExecutor;
    using System.Threading;
    using vstest_executionengine_platformbridge;

    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            var wrapper = new TestExecutorServiceWrapper();
            new Thread(new ServiceMain((param0, param1) => wrapper.SendMessage((ContractName)param0, param1)).Run).Start();

        }
    }
}