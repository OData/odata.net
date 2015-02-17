//---------------------------------------------------------------------
// <copyright file="TestServiceHost.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace TestServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string uriBaseAddress = args[0];
            string waitHandleName = null;
            if(args.Length > 1)
                 waitHandleName = args[1];

            Uri[] uriArray = { new Uri(uriBaseAddress) };

            Type serviceType = Assembly.GetExecutingAssembly().GetTypes().Single(t => !t.IsAbstract && typeof(Microsoft.OData.Service.IRequestHandler).IsAssignableFrom(t));

            using(WebServiceHost host = new WebServiceHost(serviceType, uriArray))
            {
                try
                {    
                    host.Open();
                    if(waitHandleName == "Debug" || string.IsNullOrEmpty(waitHandleName)) 
                    { 
                        Console.WriteLine("Running in Debug mode , please press any key to exit");
                        Console.Read();
                    }
                    else
                    {
                        EventWaitHandle waitHandle = EventWaitHandle.OpenExisting(waitHandleName);
                        waitHandle.WaitOne();
                    }
                    host.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An exception occurred:");
                    Console.WriteLine(ex.ToString());
                    host.Abort();
                    Console.ReadLine();
                }
            }
        }
    }

    //[[ServiceCode]]
}
