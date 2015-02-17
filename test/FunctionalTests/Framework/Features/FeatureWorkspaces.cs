//---------------------------------------------------------------------
// <copyright file="FeatureWorkspaces.cs" company="Microsoft">
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
    public class FeatureWorkspaces : WorkspacesBase
    {
        protected virtual bool WorkspacePredicate(WorkspaceAttribute attribute)
        {
            return true;
        }
        protected virtual void WorkspaceCallback(Workspace workspace)
        { }

        protected override void Filter()
        {
            Filter(WorkspacePredicate);
        }

        protected override void FilterAndCreate()
        {
            this.FilterAndCreate(WorkspaceCallback, WorkspacePredicate);
        }

        protected static Action<FeatureWorkspaces> PostWorkspaceCreation { get; set; }

        protected static Dictionary<Type, FeatureWorkspaces> createdWorkspaces = new Dictionary<Type, FeatureWorkspaces>();

        public static bool WorkspacesCreated<T>()
        {
            return createdWorkspaces.ContainsKey(typeof(T));
        }

        public static FeatureWorkspaces GetWorkspaces<T>() where T : FeatureWorkspaces, new()
        {
            FeatureWorkspaces workspaces;
            if (!createdWorkspaces.TryGetValue(typeof(T), out workspaces))
            {
                workspaces = new T();

                try
                {
                    workspaces.FilterAndCreate();
                    if (PostWorkspaceCreation != null)
                    {
                        PostWorkspaceCreation(workspaces);
                    }
                }
                catch (Exception)
                {
                    workspaces.Dispose();
                    workspaces = null;
                    throw;
                }
                finally
                {
                    createdWorkspaces[typeof(T)] = workspaces;
                }
            }

            // only way it could be null is if a previous failure stored the null value
            if (workspaces == null)
                throw new TestFailedException("Workspaces failed on creation previously");

            if (workspaces.Count == 0)
                throw new TestSkippedException("Test properties did not allow for the creation of any workspaces");
            return workspaces;
        }

        public static void ClearWorkspaces<T>() where T : FeatureWorkspaces
        {
            Type t = typeof(T);
            FeatureWorkspaces existing;
            if (createdWorkspaces.TryGetValue(t, out existing))
            {
                if(existing != null)
                    existing.Dispose();
                createdWorkspaces.Remove(t);
            }
        }
    }
}
