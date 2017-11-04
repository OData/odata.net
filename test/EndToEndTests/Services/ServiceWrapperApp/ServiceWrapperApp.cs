//---------------------------------------------------------------------
// <copyright file="ServiceWrapperApp.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ServiceWrapperApp
{
    using System;
    using System.Linq;
    using Microsoft.Test.OData.Framework.Server;
    using Microsoft.Test.OData.Services.TestServices;
    using ServiceCommand = Microsoft.Test.OData.Tests.Client.IPCCommandMap.ServiceCommands;
    using ServiceDescriptorType = Microsoft.Test.OData.Tests.Client.IPCCommandMap.ServiceDescriptorType;
    using ServiceType = Microsoft.Test.OData.Tests.Client.IPCCommandMap.ServiceType;

    /// <summary>
    /// Commandline tool to launch the test services for OData Client E2E tests. This tool must be run in admin mode to open ports.
    /// </summary>
    class ServiceWrapperApp
    {
        private static ServiceDescriptor serviceDescriptor;
        private static IServiceWrapper serviceWrapper;
        private static bool isServiceRunning = false;

        /// <summary>
        /// Enums representing return codes for the main program.
        /// </summary>
        private enum ReturnCode
        {
            Success,
            HelpPrinted,
            ServiceArgumentError,
            ServiceSetupError
        }

        /// <summary>
        /// App entry.
        /// </summary>
        /// <param name="args">Arguments passed into commandline.</param>
        static int Main(string[] args)
        {
            ServiceCommand serviceCommand;
            ServiceDescriptorType serviceDescriptorType;
            ServiceType serviceType;
            bool isAutomation = false;
            string commandText;

            if (args.Contains("h") || args.Contains("help") || args.Contains("?"))
            {
                PrintHelp();
                return (int)ReturnCode.HelpPrinted;
            }

            if (AttachDebugger(args))
            {
                Console.WriteLine("Attach a debugger. Press Enter to continue.");
                commandText = Console.ReadLine();
            }

            if (args.Contains("a") || args.Contains("automation"))
            {
                isAutomation = true;
            }

            if (!ParseServiceArguments(args, out serviceDescriptorType, out serviceType))
            {
                PrintHelp();
                return (int)ReturnCode.ServiceArgumentError;
            }

            // Initialize the service descriptor and wrapper
            try
            {
                SetServiceDescriptorAndWrapper(serviceDescriptorType, serviceType);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                PrintHelp();

                return (int)ReturnCode.ServiceSetupError;
            }

            if (!isAutomation)
            {
                Console.WriteLine("Service settings initialized.");
            }

            // Wait for a Start command to start the service
            do
            {
                if (!isAutomation)
                {
                    Console.WriteLine("Press {0} and enter to start service.", (int)ServiceCommand.StartService);
                }
                commandText = Console.ReadLine();
            } while (!ParseServiceCommand(commandText, out serviceCommand) || serviceCommand != ServiceCommand.StartService);
            StartService();

            // Console.WriteLine needs to happen to communicate the URI back to test client
            // At this point, the service URI should be up
            if (!isAutomation)
            {
                Console.WriteLine("Service Uri:");
            }
            Console.WriteLine("{0}", serviceWrapper.ServiceUri);

            // Wait for a Stop command to stop the service
            do
            {
                if (!isAutomation)
                {
                    Console.WriteLine("Press {0} and enter to stop service.", (int)ServiceCommand.StopService);
                }
                commandText = Console.ReadLine();
            } while (!ParseServiceCommand(commandText, out serviceCommand) || serviceCommand != ServiceCommand.StopService);
            StopService();

            return (int)ReturnCode.Success;
        }

        /// <summary>
        /// Hook for process exit. Ensure that we stop the service during exit.
        /// </summary>
        /// <param name="sender">Details about sender.</param>
        /// <param name="e">Details about event arguments.</param>
        static void OnProcessExit(object sender, EventArgs e)
        {
            StopService();
        }

        /// <summary>
        /// Determines whether to pause the app to let user attach debugger.
        /// </summary>
        /// <param name="args">String array to search for debug arguments.</param>
        /// <returns>True if pausing to attach debugger; false otherwise.</returns>
        private static bool AttachDebugger(string[] args)
        {
            // Last argument has to be "d" or "debug"
            if (args.Length < 1)
            {
                return false;
            }

            string argument = args.Last().ToLower();
            if (argument == "d" || argument == "debug")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Maps the service descriptor enum to the actual object.
        /// </summary>
        /// <param name="serviceDescriptorType">Service descriptor enum.</param>
        /// <returns>Corresponding service descriptor based on the provided enum.</returns>
        private static ServiceDescriptor GetServiceDescriptor(
            ServiceDescriptorType serviceDescriptorType)
        {
            switch (serviceDescriptorType)
            {
                case ServiceDescriptorType.AstoriaDefaultService:
                    return ServiceDescriptors.AstoriaDefaultService;
                case ServiceDescriptorType.PluggableFormatServiceDescriptor:
                    return ServiceDescriptors.PluggableFormatServiceDescriptor;
                case ServiceDescriptorType.TypeDefinitionServiceDescriptor:
                    return ServiceDescriptors.TypeDefinitionServiceDescriptor;
                case ServiceDescriptorType.ModelRefServiceDescriptor:
                    return ServiceDescriptors.ModelRefServiceDescriptor;
                case ServiceDescriptorType.OperationServiceDescriptor:
                    return ServiceDescriptors.OperationServiceDescriptor;
                case ServiceDescriptorType.TripPinServiceDescriptor:
                    return ServiceDescriptors.TripPinServiceDescriptor;
                case ServiceDescriptorType.ODataWCFServicePlusDescriptor:
                    return ServiceDescriptors.ODataWCFServicePlusDescriptor;
                case ServiceDescriptorType.ODataWCFServiceDescriptor:
                    return ServiceDescriptors.ODataWCFServiceDescriptor;
                case ServiceDescriptorType.AstoriaDefaultWithAccessRestrictions:
                    return ServiceDescriptors.AstoriaDefaultWithAccessRestrictions;
                case ServiceDescriptorType.PayloadValueConverterServiceDescriptor:
                    return ServiceDescriptors.PayloadValueConverterServiceDescriptor;
                case ServiceDescriptorType.PublicProviderEFService:
                    return ServiceDescriptors.PublicProviderEFService;
                case ServiceDescriptorType.PublicProviderHybridService:
                    return ServiceDescriptors.PublicProviderHybridService;
                case ServiceDescriptorType.ActionOverloadingService:
                    return ServiceDescriptors.ActionOverloadingService;
                case ServiceDescriptorType.OpenTypesService:
                    return ServiceDescriptors.OpenTypesService;
                case ServiceDescriptorType.UrlModifyingService:
                    return ServiceDescriptors.UrlModifyingService;
                case ServiceDescriptorType.ODataWriterService:
                    return ServiceDescriptors.ODataWriterService;
                case ServiceDescriptorType.PrimitiveKeysService:
                    return ServiceDescriptors.PrimitiveKeysService;
                case ServiceDescriptorType.KeyAsSegmentService:
                    return ServiceDescriptors.KeyAsSegmentService;
                case ServiceDescriptorType.AstoriaDefaultServiceModifiedClientTypes:
                    return ServiceDescriptors.AstoriaDefaultServiceModifiedClientTypes;
                case ServiceDescriptorType.PublicProviderReflectionService:
                    return ServiceDescriptors.PublicProviderReflectionService;
                case ServiceDescriptorType.ODataSimplifiedServiceDescriptor:
                    return ServiceDescriptors.ODataSimplifiedServiceDescriptor;
                default:
                    throw new Exception(string.Format("Unsupported service descriptor type: {0}",
                        serviceDescriptorType.ToString()));
            }
        }

        /// <summary>
        /// Maps the service wrapper enum to the actual object.
        /// </summary>
        /// <param name="serviceType">Service enum.</param>
        /// <returns>Corresponding service wrapper based on the provided enum.</returns>
        private static IServiceWrapper GetServiceWrapper(ServiceType serviceType)
        {
            switch (serviceType)
            {
                case ServiceType.Default:
                        return new DefaultServiceWrapper(serviceDescriptor);
                case ServiceType.WCF:
                        return new WCFServiceWrapper(serviceDescriptor);
                default:
                    throw new Exception(string.Format("Unsupported service type: {0}",
                        serviceType.ToString()));
            }
        }

        /// <summary>
        /// Verifies that the arguments are valid and populates the serviceDescriptorType and
        /// serviceType parameters.
        /// </summary>
        /// <param name="serviceDescriptorType">Service wrapper type used to create service wrapper.</param>
        /// <param name="serviceType">Service wrapper type used to create service wrapper.</param>
        /// <returns>True if there's nothing wrong with the provided arguemnts; false otherwise.</returns>
        private static bool ParseServiceArguments(
            string[] args, out ServiceDescriptorType serviceDescriptorType, out ServiceType serviceType)
        {
            serviceDescriptorType = ServiceDescriptorType.Unknown;
            serviceType = ServiceType.Unknown;

            // Expect the first two arguments:
            // 1) Microsoft.Test.OData.Tests.Client.IPCCommandMap.ServiceDescriptorType enum
            // 2) Microsoft.Test.OData.Tests.Client.IPCCommandMap.ServiceType enum
            if (args.Length < 2)
            {
                return false;
            }

            int serviceDescriptorTypeArg = 0;
            int serviceTypeArg = 0;

            if (!int.TryParse(args[0], out serviceDescriptorTypeArg) || !int.TryParse(args[1], out serviceTypeArg))
            {
                return false;
            }

            if (serviceDescriptorTypeArg >= (int)ServiceDescriptorType.Unknown ||
                serviceTypeArg >= (int)ServiceType.Unknown)
            {
                return false;
            }

            serviceDescriptorType = (ServiceDescriptorType)serviceDescriptorTypeArg;
            serviceType = (ServiceType)serviceTypeArg;

            return true;
        }

        /// <summary>
        /// Verifies that the service command text is valid and maps to corresponding command.
        /// </summary>
        /// <param name="commandText">Service command text to be mapped into the enum.</param>
        /// <param name="command">Service command enum to be returned to caller.</param>
        /// <returns>True if there's nothing wrong with the provided service command; false otherwise.</returns>
        private static bool ParseServiceCommand(string commandText, out ServiceCommand command)
        {
            command = ServiceCommand.Unknown;

            int commandNum;
            if (commandText == null || commandText.Length == 0 || !int.TryParse(commandText, out commandNum))
            {
                return false;
            }

            command = (ServiceCommand)commandNum;
            return true;
        }

        /// <summary>
        /// Prints the help dialog.
        /// </summary>
        private static void PrintHelp()
        {
            Console.WriteLine("--- HELP DIALOG ---");
            Console.WriteLine("This app helps launch test services for the OData Client E2E test cases.\n");

            Console.WriteLine("When launching the app, provide two arguments (in the following order): 1) the service descriptor and 2) the service type. " +
                "Use the following numbers to determine what types to launch and how to tell the service start/stop.\n");

            Console.WriteLine("Service descriptors:");
            for (int i = 0, length = (int)ServiceDescriptorType.Unknown; i < length; ++i)
            {
                Console.WriteLine("{0}: {1}", i, ((ServiceDescriptorType)i).ToString());
            }
            Console.WriteLine();

            Console.WriteLine("Service types:");
            for (int i = 0, length = (int)ServiceType.Unknown; i < length; ++i)
            {
                Console.WriteLine("{0}: {1}", i, ((ServiceType)i).ToString());
            }
            Console.WriteLine();

            Console.WriteLine("Service commands:");
            for (int i = 0, length = (int)ServiceCommand.Unknown; i < length; ++i)
            {
                Console.WriteLine("{0}: {1}", i, ((ServiceCommand)i).ToString());
            }
            Console.WriteLine();

            Console.WriteLine("To disable logs for automation, provide the argument 'a' or 'automation' after the service arguments (first two).");
            Console.WriteLine("To attach debugger, provide the argument 'd' or 'debug' at the end of all arguments.");
            Console.WriteLine();

            Console.WriteLine("Return codes:");
            for (int i = 0, length = Enum.GetNames(typeof(ReturnCode)).Length; i < length; ++i)
            {
                Console.WriteLine("{0}: {1}", i, ((ReturnCode)i).ToString());
            }
        }

        /// <summary>
        /// Sets the service descriptor based on the provided ServiceDescriptorType enum and instantiates the
        /// service wrapper based on the ServiceType enum. The service is also started.
        /// </summary>
        /// <param name="serviceDescriptorType">Service descriptor enum to create corresponding service descriptor.</param>
        /// <param name="serviceType">Service type enum to create corresponding service wrapper.</param>
        private static void SetServiceDescriptorAndWrapper(
            ServiceDescriptorType serviceDescriptorType, ServiceType serviceType)
        {
            TestServiceUtil.ServiceUriGenerator =
                Microsoft.Test.OData.Tests.Client.ServiceGeneratorFactory.CreateServiceUriGenerator();
            serviceDescriptor = GetServiceDescriptor(serviceDescriptorType);
            serviceWrapper = GetServiceWrapper(serviceType);
        }

        /// <summary>
        /// Starts the test service.
        /// </summary>
        private static void StartService()
        {
            serviceWrapper.StartService();
            isServiceRunning = true;
        }

        /// <summary>
        /// Stops the test service.
        /// </summary>
        private static void StopService()
        {
            if (isServiceRunning)
            {
                serviceWrapper.StopService();
                isServiceRunning = false;
            }
        }
    }
}
