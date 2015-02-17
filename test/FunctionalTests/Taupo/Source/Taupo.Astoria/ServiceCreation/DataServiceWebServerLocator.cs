//---------------------------------------------------------------------
// <copyright file="DataServiceWebServerLocator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ServiceCreation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.DataServices.WebServerLocator;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Uses a DataService to find a machine to be used as a WebServer
    /// </summary>
    [ImplementationName(typeof(IWebServerLocator), "Default")]
    public class DataServiceWebServerLocator : IWebServerLocator
    {
        private IDataServiceWebServerLocatorStrategy strategy;
        private EventHandler<WebServerLocatorCompleteEventArgs> callBack;
        private string[] machinesToAvoid = new string[0] { };
        
        /// <summary>
        /// Initializes a new instance of the DataServiceWebServerLocator class
        /// </summary>
        /// <param name="strategy">Stategy used to find a set of webservers</param>
        public DataServiceWebServerLocator(IDataServiceWebServerLocatorStrategy strategy)
            : base()
        {
            this.strategy = strategy;
        }

        /// <summary>
        /// Gets a value indicating whether the BeginLookup is complete or not
        /// </summary>
        public bool IsComplete { get; private set; }

        /// <summary>
        /// Converts a boolean? value to null, "True", or "False"
        /// </summary>
        /// <param name="value">Nullable bool</param>
        /// <returns>a string that is a null value, "True", or "False"</returns>
        public static string MapBooleanValueToString(bool? value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value.Value)
            {
                return "True";
            }
            else
            {
                return "False";
            }
        }

        /// <summary>
        /// Map The Major release from a enum that makes sense to strings in the database that loosely make sense
        /// </summary>
        /// <param name="releaseVersion">Enumeration of the required release</param>
        /// <returns>a string that represents the version in the database</returns>
        public static string MapMajorReleaseVersionToString(MajorAstoriaReleaseVersion? releaseVersion)
        {
            if (releaseVersion == null)
            {
                return null;
            }
            else
            {
                return releaseVersion.ToString();
            }
        }

        /// <summary>
        /// Converts the enumeration to the proper WTT Language that is in the database or data service
        /// </summary>
        /// <param name="serverLanguage">Server Language</param>
        /// <returns>A String that is the name of the Language in WTT</returns>
        public static string MapLabServerLanguageToString(LabServerLanguage? serverLanguage)
        {
            if (serverLanguage == null)
            {
                return null;
            }
            else if (serverLanguage == LabServerLanguage.ChineseSimplified)
            {
                return "Chinese - Simplified";
            }
            else if (serverLanguage == LabServerLanguage.ChineseTraditional)
            {
                return "Chinese - Traditional";
            }

            return serverLanguage.ToString();
        }

        /// <summary>
        /// Converts a SKU to the appropriate string based on WTT items
        /// </summary>
        /// <param name="skuType">Parameter that is a SkuType</param>
        /// <returns>Returns a string that is a sku</returns>
        public static string MapLabSkuToString(LabSkuType? skuType)
        {
            if (skuType.HasValue == false)
            {
                return null;
            }
            else if (skuType == LabSkuType.EnterpriseIA64)
            {
                return "Enterprise IA64";
            }
            else if (skuType == LabSkuType.ProPremium)
            {
                return "Pro Premium";
            }
            else
            {
                return skuType.ToString();
            }
        }

        /// <summary>
        /// Converts a InetInfoServerMajorVersion to the appropriate string
        /// </summary>
        /// <param name="internetInformationServerMajorVersion">Parameter that is a IISVersion</param>
        /// <returns>Returns a string that is a sku</returns>
        public static string MapInternetInformationServerVersionToString(InternetInformationServerMajorVersion? internetInformationServerMajorVersion)
        {
            if (internetInformationServerMajorVersion.HasValue == false)
            {
                return null;
            }
            else
            {
                return internetInformationServerMajorVersion.ToString().Replace("V", "IIS");
            }
        }

        /// <summary>
        /// Begins looking for a WebServer
        /// </summary>
        /// <param name="machineSearchCriteria">Machine specific criteria</param>
        /// <param name="astoriaMachineSearchCriteria">Astoria specific criteria</param>
        /// <param name="callback">WebServer return results</param>
        public void BeginLookup(
            MachineSearchCriteria machineSearchCriteria, 
            AstoriaMachineSearchCriteria astoriaMachineSearchCriteria, 
            EventHandler<WebServerLocatorCompleteEventArgs> callback)
        {
            if (callback == null)
            {
                throw new TaupoArgumentNullException("callback");
            }

            this.IsComplete = false;
            this.callBack = callback;

            IQueryable<WebMachine> machinesQuery = this.strategy.WebServersQueryRoot;

            machinesQuery = this.ApplyMachineSearchCriteria(machinesQuery, machineSearchCriteria);

            machinesQuery = this.ApplyAstoriaMachineCriteria(machinesQuery, astoriaMachineSearchCriteria);

            machinesQuery = machinesQuery.OrderBy(m => m.LastUsed);
            this.strategy.BeginExecute(this.OnQueryComplete, machinesQuery);
        }

        private IQueryable<WebMachine> ApplyMachineSearchCriteria(IQueryable<WebMachine> query, MachineSearchCriteria machineSearchCriteria)
        {
            if (machineSearchCriteria != null)
            {
                string serverLanguage = null;
                string hostArch = null;
                string skuType = null;
                string ndpVersion = null;
                string iisVersion = null;
                string ready = null;

                serverLanguage = MapLabServerLanguageToString(machineSearchCriteria.ServerLanguage);
                skuType = MapLabSkuToString(machineSearchCriteria.SkuType);
                ready = MapBooleanValueToString(machineSearchCriteria.Ready);
                if (machineSearchCriteria.ArchType != null)
                {
                    hostArch = machineSearchCriteria.ArchType.ToString();
                }

                if (machineSearchCriteria.InternetInformationServerMajorVersion != null)
                {
                    iisVersion = MapInternetInformationServerVersionToString(machineSearchCriteria.InternetInformationServerMajorVersion);
                }

                ndpVersion = machineSearchCriteria.FrameworkVersion;

                this.machinesToAvoid = new string[0] { };
                if (machineSearchCriteria != null)
                {
                    this.machinesToAvoid = machineSearchCriteria.MachinesToAvoid.ToArray();
                }

                if (serverLanguage != null)
                {
                    query = query.Where(m => m.Locale == serverLanguage);
                }

                if (hostArch != null)
                {
                    query = query.Where(m => m.Arch == hostArch);
                }

                if (iisVersion != null)
                {
                    query = query.Where(m => m.InternetInformationMajorServerVersion == iisVersion);
                }

                if (ready != null)
                {
                    query = query.Where(m => m.Ready == ready);
                }

                if (skuType != null)
                {
                    query = query.Where(m => m.Sku == skuType);
                }

                if (ndpVersion != null)
                {
                    query = query.Where(m => m.Build == ndpVersion);
                }
            }

            return query;
        }

        private IQueryable<WebMachine> ApplyAstoriaMachineCriteria(IQueryable<WebMachine> query, AstoriaMachineSearchCriteria astoriaSpecificCriteria)
        {
            if (astoriaSpecificCriteria != null)
            {
                string majorReleaseVersion = null;
                string setupType = null;
                string versioning = null;
                string setupRunning = null;

                setupType = MapBooleanValueToString(astoriaSpecificCriteria.SetupType);
                majorReleaseVersion = MapMajorReleaseVersionToString(astoriaSpecificCriteria.MajorReleaseVersion);
                setupRunning = MapBooleanValueToString(astoriaSpecificCriteria.SetupRunning);
                versioning = MapBooleanValueToString(astoriaSpecificCriteria.Versioning);

                if (setupRunning != null)
                {
                    query = query.Where(m => m.SetupRunning == setupRunning);
                }

                if (versioning != null)
                {
                    query = query.Where(m => m.Versioning == astoriaSpecificCriteria.Versioning.Value);
                }

                if (majorReleaseVersion != null)
                {
                    query = query.Where(m => m.AstoriaVersion == majorReleaseVersion);
                }

                if (setupType != null)
                {
                    query = query.Where(m => m.SetupType == setupType);
                }
            }

            return query;
        }

        /// <summary>
        /// OnQueryComplete is called when BeginExecute method on the Strategy returns,
        /// It then removes any machines that from the list that need to be avoided and calls to Save the LastUsed Changes
        /// </summary>
        /// <param name="result">An IAsyncResult for when the BeginSaveChanges is complete on the LastUsed Update</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to catch all exceptions here.")]
        private void OnQueryComplete(IAsyncResult result)
        {
            try
            {
                List<WebMachine> queriedMachines = this.strategy.EndExecute(result).ToList();

                foreach (string machineName in this.machinesToAvoid)
                {
                    List<WebMachine> machinesToRemove = queriedMachines.Where(m => m.MachineName == machineName).ToList();
                    foreach (WebMachine machineToRemove in machinesToRemove)
                    {
                        queriedMachines.Remove(machineToRemove);
                    }
                }

                WebMachine selectedMachine = queriedMachines.FirstOrDefault();
                if (selectedMachine != null)
                {
                    this.strategy.UpdateMachineLastUsedProperty(DateTime.Now, this.LastUsedUpdated, selectedMachine);
                }
                else
                {
                    this.RaiseCallBack(null, new string[0] { }, new string[1] { "No IISMachine was able to be found with the given query" });
                }
            }
            catch (Exception exception)
            {
                this.RaiseCallBack(null, new string[0] { }, new string[1] { "Error occurred in querying for a WebServer:" + exception.ToString() });
            }
        }

        private void RaiseCallBack(string machineName, string[] traces, string[] errors)
        {
            this.callBack.Invoke(this, new WebServerLocatorCompleteEventArgs(machineName, traces, errors));
            this.IsComplete = true;
        }

        private void LastUsedUpdated(IAsyncResult result)
        {
            WebMachine selectedMachine = result.AsyncState as WebMachine;
            this.RaiseCallBack(selectedMachine.MachineName, new string[0] { }, new string[0] { });
        }
    }
}
