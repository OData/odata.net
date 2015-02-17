//---------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace SilverlightAstoriaTest
{
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;
    using System.Windows;
    using System.Windows.Browser;
    using Microsoft.OData.Client;
    using TestSL;
    using System.Runtime.Serialization;
    using System.IO;
    using System.Reflection;
    using System.Data.Test.Astoria;
    using System.Data.Test.Astoria.Tests;
    using WrapperTypes;

    public partial class App : Application
    {
        private static DataServiceContext AstoriaTestService;
        private static DataServiceContext AstoriaTestSubmitService;
        private static List<string> pendingMessages = new List<string>();
        public static List<object> Instances;
        private static Uri MessagesUri = new Uri("Messages?$filter=WhoSentMe eq 'Test'", UriKind.Relative);


        public App()
        {
            System.Diagnostics.Debug.WriteLine("App");
            this.Startup += this.Application_Startup;
            InitializeComponent();
        }


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_Startup");
            // Load the main control
            this.RootVisual = new Page();


        }
    }
}