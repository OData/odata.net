//---------------------------------------------------------------------
// <copyright file="Workspaces.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ModuleCore;

namespace System.Data.Test.Astoria
{
    public abstract class WorkspacesBase : fxList<Workspace>
    {
        public void Dispose()
        {
            foreach (Workspace workspace in this)
            {
                workspace.Dispose();
            }
            this.Clear();
        }

        protected void Filter(Func<WorkspaceAttribute, bool> predicate)
        {
            this.Clear();

            foreach (Type t in typeof(Workspaces).Assembly.GetTypes())
            {
                object[] attributes = t.GetCustomAttributes(typeof(WorkspaceAttribute), true);

                if (attributes.Length > 0)
                {
                    WorkspaceAttribute workspaceAttribute = attributes[0] as WorkspaceAttribute;

                    if (predicate != null && !predicate(workspaceAttribute))
                        continue;

                    if (AstoriaTestProperties.DataLayerProviderKinds.Contains(workspaceAttribute.DataLayerProviderKind)
                        && (IgnoreWorkspacePriority || workspaceAttribute.Priority <= AstoriaTestProperties.MaxPriority))
                    {
                        foreach (Workspace w in Construct(t))
                            this.Add(w);
                    }
                }
            }
        }

        protected virtual IEnumerable<Workspace> Construct(Type t)
        {
            yield return (Workspace)Activator.CreateInstance(t);
        }

        protected virtual void Filter()
        {
            Filter(null);
        }

        protected void FilterAndCreate(Action<Workspace> callback, Func<WorkspaceAttribute, bool> predicate)
        {
            this.Filter(predicate);

            foreach (Workspace w in this)
            {
                if (callback != null)
                    callback(w);
                w.Create();
            }
        }

        protected virtual void FilterAndCreate()
        {
            FilterAndCreate(null, null);
        }

        public virtual bool IgnoreWorkspacePriority
        {
            get
            {
                return false;
            }
        }
    }

    public enum DataLayerProviderKind
    {
        Edm, LinqToSql, InMemoryLinq, NonClr, None
    }

    public class Workspaces : WorkspacesBase
    {
        public Workspace Item(string name)
        {
            foreach (Workspace w in this)
            {
                if (w.Name == name)
                {
                    return w;
                }
            }

            return null;
        }

        public Workspaces FilterByName(string name)
        {
            Workspaces result = new Workspaces();
            foreach (Workspace workspace in this)
            {
                if (workspace.Name == name)
                {
                    result.Add(workspace);
                }
            }

            return result;
        }



        private static Workspaces _allWorkspaces;
        /// <summary>Gets a list of all available providers.</summary>
        public static Workspaces AllWorkspaces
        {
            get
            {
                if (_allWorkspaces == null)
                {
                    List<DataLayerProviderKind> kinds = GetAllDataLayerProviderKinds();
                    _allWorkspaces = CreateFilteredWorkspaces(kinds.ToArray(), 5, false);
                }
                return _allWorkspaces;
            }
        }

        internal static List<DataLayerProviderKind> GetAllDataLayerProviderKinds()
        {
            List<DataLayerProviderKind> allKinds = new List<DataLayerProviderKind>();
            allKinds.Add(DataLayerProviderKind.Edm);
            allKinds.Add(DataLayerProviderKind.LinqToSql);
            return allKinds;
        }
        public static DataLayerProviderKind[] ConvertToArrayOfDataLayerProviderKinds(string val)
        {
            List<DataLayerProviderKind> kinds = new List<DataLayerProviderKind>();
            string[] segments = val.Split(',');
            foreach (string segment in segments)
            {
                kinds.Add(ConvertToDataLayerKind(segment));
            }
            return kinds.ToArray();
        }
        public static DataLayerProviderKind ConvertToDataLayerKind(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            string lowerCasedName = name.ToLower(System.Globalization.CultureInfo.InvariantCulture);

            if (lowerCasedName == "edm")
            {
                // Backwards compatiblity.
                return DataLayerProviderKind.Edm;
            }
            else if (lowerCasedName == "inmemorylinq")
            {
                return DataLayerProviderKind.InMemoryLinq;
            }
            //TODO:Change this to LinqToSql instead
            else if (lowerCasedName == "nonedm" || lowerCasedName == "linqtosql")
            {
                // Backwards compatiblity.
                return DataLayerProviderKind.LinqToSql;
            }
            else if (lowerCasedName == "nonclr")
            {
                // Backwards compatiblity.
                return DataLayerProviderKind.NonClr;
            }
            else
            {
                throw new ArgumentOutOfRangeException("ConvertToDataLayerKind", "Unknown DataLayerProviderKind:" + name);
            }
        }

        private static bool _workspaceCreationFailed = false;
        private static Workspaces _filteredWorkspaces;

        public static Workspaces GetWorkspacesFilteredByTestProperties()
        {
            return GetWorkspacesFilteredByTestProperties((Workspace w) => { });
        }
        internal static Workspaces GetWorkspacesFilteredByTestProperties(Action<Workspace> beforeWorkspaceCreate)
        {
            if (_workspaceCreationFailed)
                throw new TestFailedException("Workspaces failed on creation previously");
            if (_filteredWorkspaces == null)
            {
                try
                {
                    _filteredWorkspaces = CreateFilteredWorkspaces(AstoriaTestProperties.DataLayerProviderKinds, AstoriaTestProperties.MaxPriority, true, beforeWorkspaceCreate);
                }
                catch (System.Reflection.ReflectionTypeLoadException typeloadException)
                {
                    _workspaceCreationFailed = true;
                    AstoriaTestLog.WriteLine("LoaderExceptions:");
                    foreach (Exception exc in typeloadException.LoaderExceptions)
                    {
                        AstoriaTestLog.FailAndContinue(exc);
                    }
                    throw typeloadException;
                }
                catch (Exception exc)
                {
                    _workspaceCreationFailed = true;
                    throw;
                }
            }
            return _filteredWorkspaces;

        }

        struct WorkspaceCreation
        {
            public WorkspaceAttribute WorkspaceAttribute;
            public Type WorkspaceType;
        }

        // Returns list of workspaces matching test properties but does not create them
        public static Workspaces GetFilteredWorkspaces()
        {
            Workspaces workspaces = new Workspaces();
            workspaces.Filter(att => att.Standard);
            return workspaces;
        }
        internal static Workspaces CreateFilteredWorkspaces(DataLayerProviderKind[] kinds, int maxPriority, bool standardOnly)
        {
            return CreateFilteredWorkspaces(kinds, maxPriority, standardOnly, (Workspace w) =>{/*Doing nothing prior to Workspace creation*/});
        }
        internal static Workspaces CreateFilteredWorkspaces(DataLayerProviderKind[] kinds, int maxPriority, bool standardOnly, Action<Workspace> beforeWorkspaceCreate)
        {
            Workspaces workspaces = new Workspaces();
            //Find all workspaces
            List<WorkspaceCreation> workspacesToBeInitialized = new List<WorkspaceCreation>();
            //Filter by Test Property information
            foreach (Type t in typeof(Workspaces).Assembly.GetTypes())
            {
                object[] attributes = t.GetCustomAttributes(typeof(WorkspaceAttribute), true);
                if (attributes.Length > 0)
                {
                    WorkspaceAttribute workspaceAttribute = attributes[0] as WorkspaceAttribute;
                    if (kinds.Contains(workspaceAttribute.DataLayerProviderKind)
                        && workspaceAttribute.Priority <= maxPriority
                        && (!standardOnly || workspaceAttribute.Standard))
                    {
                        WorkspaceCreation creationType = new WorkspaceCreation();
                        creationType.WorkspaceAttribute = workspaceAttribute;
                        creationType.WorkspaceType = t;
                        workspacesToBeInitialized.Add(creationType);
                    }
                }
            }

            //Initialize Workspace types
            foreach (WorkspaceCreation wc in
                workspacesToBeInitialized)
            // some tests RELY on the arbitrary order of the workspaces
            //.OrderBy(wc => wc.WorkspaceAttribute.DataLayerProviderKind + "." + wc.WorkspaceAttribute.Name))
            {
                if (!wc.WorkspaceType.IsSubclassOf(typeof(Workspace)))
                {
                    //TODO: Should possibly error here
                    continue;
                }
                else
                {
                    Workspace workspace = (Workspace)wc.WorkspaceType.GetConstructor(new Type[] { }).Invoke(new object[] { });
                    workspaces.Add(workspace);
                }
            }



            //Now call create on all workspaces
            foreach (Workspace w in workspaces)
            {
                beforeWorkspaceCreate(w);
                w.Create();
            }

            return workspaces;
        }

    }
}
