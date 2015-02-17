//---------------------------------------------------------------------
// <copyright file="EntityStateData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Data
{
    #region Namespaces

    using System;
    using Microsoft.OData.Client;
    using System.Diagnostics;

    #endregion Namespaces

    public class EntityStateData
    {
        private static EntityStateData[] values;
        private readonly EntityStates state;

        private EntityStateData(EntityStates state)
        {
            this.state = state;
        }

        public static EntityStateData[] Values
        {
            get
            {
                if (values == null)
                {
                    values = new EntityStateData[]
                        {
                            new EntityStateData(EntityStates.Added),
                            new EntityStateData(EntityStates.Deleted),
                            new EntityStateData(EntityStates.Detached),
                            new EntityStateData(EntityStates.Modified),
                            new EntityStateData(EntityStates.Unchanged),
                        };
                }

                return values;
            }
        }

        public bool HasIdentity
        {
            get { return this.state != EntityStates.Detached && this.state != EntityStates.Added; }
        }

        public EntityStates State
        {
            get { return this.state; }
        }

        public static EntityStates GetStateForEntity(DataServiceContext context, object target)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            EntityDescriptor descriptor = context.GetEntityDescriptor(target);
            EntityStates result = (descriptor == null) ? EntityStates.Detached : descriptor.State;
            return result;
        }

        /// <summary>
        /// Applies this state to the specfied <paramref name="target"/> such that after invocation, 
        /// the target in the given <paramref name="context"/> is in this state.
        /// </summary>
        /// <param name="context">Context to apply changes to.</param>
        /// <param name="target">Target to change state on.</param>
        /// <param name="entitySetName">Name of entity set (necessary for certain transitions).</param>
        public void ApplyToObject(DataServiceContext context, object target, string entitySetName)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            EntityStates current = GetStateForEntity(context, target);
            if (current == this.state)
            {
                return;
            }

            switch (this.state)
            {
                case EntityStates.Added:
                    if (current != EntityStates.Detached)
                    {
                        context.Detach(target);
                    }

                    context.AddObject(entitySetName, target);
                    break;
                case EntityStates.Detached:
                    context.Detach(target);
                    break;
                case EntityStates.Deleted:
                    if (current == EntityStates.Detached)
                    {
                        context.AttachTo(entitySetName, target);
                    }

                    context.DeleteObject(target);
                    break;
                case EntityStates.Modified:
                    if (current == EntityStates.Detached)
                    {
                        context.AttachTo(entitySetName, target);
                    }

                    context.UpdateObject(target);
                    break;
                case EntityStates.Unchanged:
                    if (current != EntityStates.Detached)
                    {
                        context.Detach(target);
                    }

                    context.AttachTo(entitySetName, target);
                    break;
            }
        }

        /// <summary>
        /// Gets the expected state after a refresh for an entity in this
        /// state with the specified <paramref name="option"/>.
        /// </summary>
        /// <param name="option">Option in effect during refresh.</param>
        /// <returns>The expected entity state.</returns>
        public EntityStates ExpectedStateAfterRefresh(MergeOption option)
        {
            EntityStates result;
            switch (option)
            {
                case MergeOption.AppendOnly:
                    if (this.state == EntityStates.Detached)
                    {
                        result = EntityStates.Unchanged;
                    }
                    else
                    {
                        result = this.state;
                    }

                    break;
                case MergeOption.NoTracking:
                    result = this.state;
                    break;
                case MergeOption.OverwriteChanges:
                    if (this.state == EntityStates.Added)
                    {
                        result = EntityStates.Added;
                    }
                    else
                    {
                        result = EntityStates.Unchanged;
                    }

                    break;
                default:
                    Debug.Assert(option == MergeOption.PreserveChanges, "option == MergeOption.PreserveChanges");
                    if (this.state == EntityStates.Detached)
                    {
                        result = EntityStates.Unchanged;
                    }
                    else 
                    {
                        result = this.state;
                    }

                    break;
            }

            return result;
        }

        public override string ToString()
        {
            return this.state.ToString();
        }
    }
}
