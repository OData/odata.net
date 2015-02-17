//---------------------------------------------------------------------
// <copyright file="WorkspaceAttributes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Data.Test.Astoria.TestExecutionLayer;
    using System.Xml;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.Test.ModuleCore; //TestFailedException;
    using System.Reflection;
    using System.Text;


    public class WorkspaceAttribute : Attribute
    {
        public WorkspaceAttribute(string name, DataLayerProviderKind kind)
        {
            this.Name = name;
            this.DataLayerProviderKind = kind;
            
            Standard = false;
            Priority = 5;
        }

        public DataLayerProviderKind DataLayerProviderKind
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public int Priority
        {
            get;
            set;
        }

        public bool Standard
        {
            get;
            set;
        }
    }

    public enum UpdatableImplementation
    {
        None = 0,
        IUpdatable,
        DataServiceUpdateProvider
    }

    public enum StreamProviderImplementation
    {
        None = 0,
        DataServiceStreamProvider,
        DataServiceStreamProvider2
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class WorkspaceDefaultSettingsAttribute : Attribute
    {
        public WorkspaceDefaultSettingsAttribute()
        {
            this.MaxProtocolVersion = ODataProtocolVersion.V4;
        }

        public string ServiceBaseClass = null;
        public bool SuppressTrivialLogging = false;
        public bool SkipWorkspaceCheck = false;
        public bool SkipDataPopulation = false;
        public bool HasContainment = false;
        public UpdatableImplementation UpdatableImplementation = UpdatableImplementation.None;
        public bool UseLazyPropertyLoading = false;
        public ODataProtocolVersion? MaxProtocolVersion;
    }

    public class WorkspaceSettings
    {
        public WorkspaceSettings(Workspace w)
        {
            object[] attributes = w.GetType().GetCustomAttributes(typeof(WorkspaceDefaultSettingsAttribute), true);

            if (attributes.Length > 0)
            {
                WorkspaceDefaultSettingsAttribute att = attributes[0] as WorkspaceDefaultSettingsAttribute;
                foreach (FieldInfo p in att.GetType().GetFields())
                {
                    object val = p.GetValue(att);
                    this.GetType().GetProperty(p.Name).SetValue(this, val, null);
                }
            }
        }

        public string ServiceBaseClass { get; set; }
        public bool SuppressTrivialLogging { get; set; }
        public bool SkipWorkspaceCheck { get; set; }
        public bool SkipDataPopulation { get; set; }
        public bool HasContainment { get; set; }
        public UpdatableImplementation UpdatableImplementation { get; set; }
        public bool UseLazyPropertyLoading { get; set; }

        public StreamProviderImplementation StreamProviderImplementation { get; set; }
        public bool HasExpandProvider { get; set; }
        public bool HasPagingProvider { get; set; }

        public ODataProtocolVersion? MaxProtocolVersion { get; set; }

        public bool SupportsUpdate
        {
            get
            {
                return UpdatableImplementation != UpdatableImplementation.None;
            }
        }

        public bool SupportsMediaLinkEntries
        {
            get
            {
                return StreamProviderImplementation != StreamProviderImplementation.None;
            }
        }

        public bool SupportsNamedStreams
        {
            get
            {
                return StreamProviderImplementation != StreamProviderImplementation.None 
                    && StreamProviderImplementation != StreamProviderImplementation.DataServiceStreamProvider;
            }
        }
    }

    [global::System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class DataLayerProviderAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string positionalString;

        // This is a positional argument
        public DataLayerProviderAttribute(string positionalString)
        {
            this.positionalString = positionalString;

            // TODO: Implement code here
            throw new NotImplementedException();
        }

        public string PositionalString { get; private set; }

        // This is a named argument
        public int NamedInt { get; set; }
    }
}