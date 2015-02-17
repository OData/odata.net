//---------------------------------------------------------------------
// <copyright file="ServiceUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Diagnostics;
#if !ClientSKUFramework
    using System.ServiceProcess;
    using System.Security;
#endif


    /// <summary>This class provides utility methods and properties for managing Windows services.</summary>
    public static class ServiceUtil
    {
        /// <summary>Whether the local IIS server is known to be running.</summary>
        private static bool? _isLocalIisRunning;

        /// <summary>Whether the local SQL Server is known to be running.</summary>
        private static bool? _isLocalSqlServerRunning;

        /// <summary>Whether the local SQL Server Express is known to be running.</summary>
        private static bool? _isLocalSqlServerExpressRunning;

        /// <summary>Whether the local IIS server is known to be running.</summary>
        public static bool IsLocalIisRunning
        {
            get
            {
                if (!_isLocalIisRunning.HasValue)
                {
                    _isLocalIisRunning = CheckLocalRunningService("W3SVC");
                }

                return _isLocalIisRunning.Value;
            }
        }

        /// <summary>Whether the local SQL server is known to be running.</summary>
        public static bool IsLocalSqlServerRunning
        {
            get
            {
                if (!_isLocalSqlServerRunning.HasValue)
                {
                    _isLocalSqlServerRunning = CheckLocalRunningService("MSSQLSERVER");
                }

                return _isLocalSqlServerRunning.Value;
            }
        }

        /// <summary>Whether the local SQL server is known to be running.</summary>
        public static bool IsLocalSqlServerExpressRunning
        {
            get
            {
                if (!_isLocalSqlServerExpressRunning.HasValue)
                {
                    _isLocalSqlServerExpressRunning = CheckLocalRunningService("MSSQL$SQLEXPRESS");
                }

                return _isLocalSqlServerExpressRunning.Value;
            }
        }

#if !ClientSKUFramework
        [ServiceControllerPermission(Security.Permissions.SecurityAction.Assert, Unrestricted = true)]
#endif
        private static bool CheckLocalRunningService(string serviceName)
        {
            Debug.Assert(serviceName != null, "serviceName != null");

            bool result = false;
#if !ClientSKUFramework

            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (!result && service.ServiceName == serviceName && service.Status == ServiceControllerStatus.Running)
                {
                    result = true;
                }

                service.Dispose();
            }
#endif

            return result;
        }
    }
}
