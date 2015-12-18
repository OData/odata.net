//---------------------------------------------------------------------
// <copyright file="InMemoryCLRTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    [Serializable]
    public abstract class ClrObject
    {
        protected ClrObject()
        {
            this.UpdatedTime = DateTime.Now;
        }

        public DateTime UpdatedTime { get; set; }

        public List<InstanceAnnotationType> Annotations { get; set; }

        public string EntitySetName { get; set; }
    }

    [Serializable]
    public abstract class OpenClrObject : ClrObject
    {
        //Open properties
        private readonly Dictionary<string, object> openProperties = new Dictionary<string, object>();

        public Dictionary<string, object> OpenProperties
        {
            get
            {
                return openProperties;
            }
        }
    }

    [Serializable]
    public class EntityCollection<T> : Collection<T> where T : ClrObject
    {
        public EntityCollection()
            : this(null)
        {
        }

        // TODO: we cannot serialize entity set, because the reference will be different from the real entity set during deserialization. The best way to fix this is to change the code in the CreateHandler.
        public EntityCollection(EntityCollection<T> entitySet)
        {
            this.EntitySet = entitySet;
        }

        public EntityCollection<T> EntitySet
        {
            get;
            private set;
        }

        public T GetEntity(Func<T, bool> func)
        {
            // TODO: GitHub Issue#424
            T entity = this.Where(func).FirstOrDefault();
            DeletionContext.Current.BindAction(entity, () => this.Remove(entity));

            return entity;
        }

        /// <summary>
        /// Remove all deleted item which is not in entity set.
        /// </summary>
        /// <returns></returns>
        public EntityCollection<T> Cleanup()
        {
            if (this.EntitySet != null)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (!this.EntitySet.Contains(this[i]))
                    {
                        this.Remove(this[i]);
                        i--;
                    }
                }
            }

            return this;
        }

        public void AddRange(IEnumerable<T> list)
        {
            foreach (T item in list)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Add the item to collection if it isnot in the collection.
        /// </summary>
        /// <param name="item">Item to be added</param>
        public void TryAdd(T item)
        {
            if (item != null && !this.Contains(item))
            {
                this.Add(item);
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            if (this.EntitySet != null)
            {
                this.EntitySet.TryAdd(item);
            }
        }
    }
}
