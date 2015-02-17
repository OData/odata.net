//---------------------------------------------------------------------
// <copyright file="DDBasicsUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Service;
using System;
using System.Diagnostics;
using System.Linq;

namespace AstoriaUnitTests
{
    public partial class Utils
    {
        /// <summary>
        /// Random generator to generate random port numbers
        /// </summary>
        private static Random random = new Random();

        public static void CreateWorkspaceForType(Type serviceType, Type contextType, string name, out SimpleWorkspace workspace, out DataServiceHost host, bool isEFBased)
        {
            int retryCount = 0;
            string serviceEntryPointLocation = null;
            string partialEntryPointLocation = null;
            host = null;
            bool failed;

            do
            {
                failed = false;

                // Generate a random number between 20000 and 40000
                int LocalPort = random.Next(20000, 40000);

                Trace.WriteLine("Attempting to start service at port: " + LocalPort);
                try
                {
                    partialEntryPointLocation = "http://localhost:" + LocalPort + "/";
                    serviceEntryPointLocation = partialEntryPointLocation + name + ".svc";

                    host = new DataServiceHost(serviceType, new Uri[] { new Uri(serviceEntryPointLocation) });

                    Type implementedContract = typeof(IRequestHandler);
                    System.ServiceModel.Description.ServiceEndpoint endpoint = host.AddServiceEndpoint(implementedContract, new System.ServiceModel.WebHttpBinding(), "");
                    host.Open();
                    host.Faulted += delegate(object sender, EventArgs e)
                    {
                        Trace.WriteLine("WCF Host faulted.");
                    };
                }
                catch (System.ServiceModel.AddressAlreadyInUseException e)
                {
                    Trace.WriteLine(e.Message);
                    Trace.WriteLine("Failed to start service at: " + serviceEntryPointLocation);
                    retryCount++;
                    failed = true;
                }
            }
            while (retryCount < 10 && failed == true);

            Trace.WriteLine("Started " + name + " host at " + serviceEntryPointLocation);

            workspace = new SimpleWorkspace()
            {
                ServiceContainer = new SimpleServiceContainer()
                {
                    Name = name,
                    ResourceContainers = new ResourceContainerList(
                        from property in contextType.GetProperties()
                        where ((isEFBased && typeof(System.Data.Objects.ObjectQuery).IsAssignableFrom(property.PropertyType) ||
                               (!isEFBased && property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IQueryable<>))))
                        select new SimpleResourceContainer(property.Name,
                            from resouceType in contextType.Assembly.GetTypes()
                            where property.PropertyType.GetGenericArguments()[0].IsAssignableFrom(resouceType)
                            select new SimpleResourceType(resouceType.Name,
                                from typeProperty in resouceType.GetProperties()
                                select new SimpleProperty(typeProperty.Name)
                                )
                            )
                        )
                },
                ServiceEndPoint = partialEntryPointLocation,
            };
        }
    }
}