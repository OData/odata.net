//Copyright 2010 Microsoft Corporation
//
//Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 
//
//http://www.apache.org/licenses/LICENSE-2.0 
//
//Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and limitations under the License.


namespace System.Data.Services.Client
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    #endregion Namespaces.

    [DataContract]
    public class DataServiceState
    {
        [DataMember]
        internal Dictionary<string, CollectionState> CollectionsState { get; set; } 

        [DataMember] 
        internal string ContextAsString { get; set; }

        [DataMember] 
        internal string ContextTypeName { get; set; }

        [DataMember] 
        internal List<string> KnownTypeNames { get; set; }

        static public DataServiceState Save(DataServiceContext context)
        {
            return Save(context, null);
        }

        static public DataServiceState Save(DataServiceContext context, Dictionary<string,object> collections)
        {
            List<Type> knownTypes = GetKnownTypes(context);

            Dictionary<EntityDescriptor, Guid> entityDescriptorToId;
            ContextState contextState = context.SaveState(out entityDescriptorToId);
            string contextAsString = SerializeContextToString(contextState, knownTypes);

            var collectionsState = new Dictionary<string, CollectionState>();
            if (collections != null)
            {
                foreach (KeyValuePair<string, object> kvp in collections)
                {
                    IDataServiceCollection collection = (IDataServiceCollection) kvp.Value;
                    CollectionState collectionState = collection.SaveState(context, entityDescriptorToId);                    
                    collectionsState.Add(kvp.Key, collectionState);
                }
            }

            DataServiceState state = new DataServiceState()
            {
                CollectionsState = collectionsState,
                ContextAsString = contextAsString,
                ContextTypeName = context.GetType().AssemblyQualifiedName,
                KnownTypeNames = knownTypes.Select(t => t.AssemblyQualifiedName).ToList()
            };

            return state;
        }

        public DataServiceContext Restore()
        {
            Dictionary<Guid, EntityDescriptor> idToEntityDescriptor;
            DataServiceContext context = this.RestoreContext(out idToEntityDescriptor);

            return context;
        }

        public DataServiceContext Restore(out Dictionary<string, object> collections)
        {
            Dictionary<Guid, EntityDescriptor> idToEntityDescriptor;
            DataServiceContext context = this.RestoreContext(out idToEntityDescriptor);

            collections = RestoreCollections(context, idToEntityDescriptor);

            return context;
        }

        static List<Type> GetKnownTypes(DataServiceContext context)
        {
            var knownTypes = new List<Type>();
            foreach (EntityDescriptor entityDescriptor in context.Entities)
            {
                Type type = entityDescriptor.Entity.GetType();
                if (!knownTypes.Contains(type))
                {
                    knownTypes.Add(type);
                }
            }

            return knownTypes;
        }

        static string SerializeContextToString(ContextState contextState, List<Type> knownTypes)
        {
            string contextAsString;
            using (var contextAsStream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(ContextState), knownTypes);
                serializer.WriteObject(contextAsStream, contextState);
                byte[] contextAsByteArray = contextAsStream.ToArray();
                contextAsString = Encoding.UTF8.GetString(contextAsByteArray, 0, contextAsByteArray.Length);
            }

            return contextAsString;
        }

        DataServiceContext RestoreContext(out Dictionary<Guid, EntityDescriptor> idToEntityDescriptor)
        {
            ContextState contextState;
            byte[] contextAsByteArray = Encoding.UTF8.GetBytes(ContextAsString);
            using (var contextAsStream = new MemoryStream(contextAsByteArray))
            {
                IEnumerable<Type> knownTypes = KnownTypeNames.Select(n => Type.GetType(n));
                var serializer = new DataContractSerializer(typeof(ContextState), knownTypes);
                contextState = (ContextState)serializer.ReadObject(contextAsStream);
            }

            Type contextType = Type.GetType(ContextTypeName);
            DataServiceContext context = (DataServiceContext)Activator.CreateInstance(
                contextType, new object[] { contextState.BaseUri });
            context.RestoreState(contextState, out idToEntityDescriptor);

            return context;
        }

        Dictionary<string, object> RestoreCollections(DataServiceContext context, Dictionary<Guid, EntityDescriptor> idToEntityDescriptor)
        {
            var collections = new Dictionary<string, object>(this.CollectionsState.Count);
            foreach (KeyValuePair<string, CollectionState> kvp in this.CollectionsState)
            {
                CollectionState collectionState = kvp.Value;

                Type entityType = Type.GetType(collectionState.EntityTypeName);
                Type collectionType = typeof(DataServiceCollection<>).MakeGenericType(new Type[] { entityType });
                MethodInfo restoreMethod = collectionType.GetMethod(
                    "RestoreState", BindingFlags.NonPublic | BindingFlags.Static);
                Debug.Assert(restoreMethod != null);

                object collection = restoreMethod.Invoke(null, new object[] { collectionState, context, idToEntityDescriptor });
                collections.Add(kvp.Key, collection);
            }

            return collections;
        }
    }

    [DataContract]
    internal class ContextState
    {
        [DataMember] 
        internal Uri BaseUri { get; set; }

        [DataMember] 
        internal Dictionary<LinkDescriptorState, LinkDescriptorState> Bindings { get; set; }

        [DataMember] 
        internal string DataNamespace { get; set; }

        [DataMember] 
        internal List<EntityDescriptorState> EntityDescriptors { get; set; }

#if ASTORIA_LIGHT
        [DataMember] 
        internal HttpStack HttpStack { get; set; }
#endif

        [DataMember] 
        internal bool IgnoreMissingProperties { get; set; }

        [DataMember] 
        internal bool IgnoreResourceNotFoundException { get; set; }

        [DataMember] 
        internal MergeOption MergeOption { get; set; }

        [DataMember] 
        internal uint NextChange { get; set; }

        [DataMember] 
        internal bool PostTunneling { get; set; }

        [DataMember] 
        internal SaveChangesOptions SaveChangesDefaultOptions { get; set; }

#if !ASTORIA_LIGHT        
        [DataMember] 
        internal int Timeout;
#endif

        [DataMember] 
        internal Uri TypeScheme { get; set; }
    }

    [DataContract]
    internal class DescriptorState
    {
        [DataMember] 
        internal uint ChangeOrder { get; set; }

        [DataMember] 
        internal bool SaveContentGenerated { get; set; }

        [DataMember] 
        internal EntityStates SaveResultProcessed { get; set; }

        [DataMember] 
        internal EntityStates State { get; set; }
    }

    [DataContract]
    internal class EntityDescriptorState : DescriptorState
    {
        [DataMember] 
        internal Uri EditLink { get; set; }

        [DataMember] 
        internal Uri EditMediaLink { get; set; }

        [DataMember] 
        internal Guid DescriptorId { get; set; }
        
        [DataMember] 
        internal object Entity { get; set; }

        [DataMember] 
        internal string EntitySetName { get; set; }

        [DataMember] 
        internal string Etag { get; set; }

        [DataMember] 
        internal string Identity { get; set; }

        [DataMember] 
        internal Guid? ParentDescriptorId { get; set; }

        [DataMember] 
        internal string ParentProperty { get; set; }

        [DataMember] 
        internal Uri ReadStreamLink { get; set; }

        [DataMember] 
        internal SaveStreamState SaveStream { get; set; }

        [DataMember] 
        internal Uri SelfLink { get; set; }

        [DataMember] 
        internal string ServerTypeName { get; set; }

        [DataMember] 
        internal string StreamETag { get; set; }

        [DataMember] 
        internal StreamStates StreamState { get; set; }
    }

    [DataContract]
    internal class SaveStreamState
    {
        [DataMember] 
        internal Dictionary<string, string> Args { get; set; }

        [DataMember] 
        internal bool Close { get; set; }

        [DataMember] 
        internal Stream Stream { get; set; }
    }

    [DataContract]
    internal class LinkDescriptorState : DescriptorState
    {
        [DataMember] 
        internal Guid SourceDescriptorId { get; set; }

        [DataMember] 
        internal string SourceProperty { get; set; }

        [DataMember] 
        internal Guid TargetDescriptorId { get; set; }
    }

    [DataContract]
    internal class CollectionState
    {
        [DataMember] 
        internal string EntitySetName { get; set; }

        [DataMember] 
        internal string EntityTypeName { get; set; }

        [DataMember] 
        internal List<Guid> DescriptorIds { get; set; }

        [DataMember] 
        internal bool RootCollection { get; set; }
    }
}
