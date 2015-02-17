//---------------------------------------------------------------------
// <copyright file="GraphBasedDataGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    public class GraphBasedDataGenerator : IDataGenerator
    {
        #region constructor
        public GraphBasedDataGenerator(Workspace w, IDataInserter inserter)
        {
#if !ClientSKUFramework

            if (!(w is InMemoryWorkspace || w is NonClrWorkspace))
                throw new NotSupportedException("GraphBasedDataGenerator is not supported for workspaces other than InMemory and NonClr");
#endif

            _workspace = w;
            Done = false;

            DataInserter = inserter;
            Done = false;
        }
        #endregion

        #region private fields
        private const int _maxEntitiesToInsert = 5;
        private const int _collectionSize = 5;

        private Workspace _workspace;

        private List<ResourceType> _usedTypes = new List<ResourceType>();
        private Dictionary<ResourceContainer, KeyExpressions> _existingKeyMap = new Dictionary<ResourceContainer, KeyExpressions>();
        private Dictionary<KeyExpression, KeyedResourceInstance> _existingEntityMap = new Dictionary<KeyExpression, KeyedResourceInstance>();
        #endregion

        #region IDataGenerator Members
        public bool Done
        {
            get;
            internal set;
        }

        public IDataInserter DataInserter
        {
            get;
            set;
        }

        public bool SkipAssociations
        { 
            get;
            set; 
        }

        public void Run()
        {
            if (_workspace.Settings.SkipDataPopulation)
            {
                AstoriaTestLog.WriteLineIgnore("Skipping GraphBasedDataGenerator.SendData() due to workspace settings");
                return;
            }

            bool oldLoggingValue = _workspace.Settings.SuppressTrivialLogging;
            _workspace.Settings.SuppressTrivialLogging = true;

            List<ResourceContainer> containers = _workspace.ServiceContainer.ResourceContainers.Where(rc => !(rc is ServiceOperation)).ToList();

            foreach (ResourceContainer container in containers)
            {
                if (!this.Skip(container))
                {
                    int addCount;
                    int graphDepth = this.GraphDepth(container.BaseType);

                    if (graphDepth > _maxEntitiesToInsert)
                        addCount = 1;
                    else
                        addCount = _maxEntitiesToInsert - graphDepth + 1;

                    System.Diagnostics.Debug.WriteLine(container.BaseType.Name + ": " + graphDepth.ToString(), ", " + addCount.ToString());

                    this.InsertData(container, addCount);
                }
            }

            DataInserter.Close();
            Done = true;

            _workspace.Settings.SuppressTrivialLogging = oldLoggingValue;
        }

        public KeyExpression GetRandomGeneratedKey(ResourceContainer container, ResourceType type)
        {
            KeyExpressions keys;
            if (!_existingKeyMap.TryGetValue(container, out keys))
                return null;

            if (type == null)
                return keys.Choose();

            return keys.Where(k => k.ResourceType == type).Choose();
        }

        public KeyExpressions GetAllGeneratedKeys(ResourceContainer container, ResourceType type)
        {
            KeyExpressions keys;
            if (!_existingKeyMap.TryGetValue(container, out keys))
                return new KeyExpressions();

            if (type == null)
                return keys;

            return new KeyExpressions(keys.Where(k => k.ResourceType == type));
        }
        #endregion

        #region private helper methods
        private int GraphDepth(ResourceType resourceType)
        {
            _usedTypes.Clear();
            return GraphDepth(resourceType, _usedTypes);
        }

        private int GraphDepth(ResourceType resourceType, List<ResourceType> usedTypes)
        {
            foreach (ResourceProperty property in resourceType.Properties.OfType<ResourceProperty>())
            {
                if (property.IsNavigation)
                {
                    ResourceType propertyType;

                    if( property.Type is CollectionType)
                        propertyType = (property.Type as CollectionType).SubType as ResourceType;
                    else
                        propertyType = property.Type as ResourceType;

                    if (!usedTypes.Contains(propertyType))
                    {
                        usedTypes.Add(propertyType);
                        GraphDepth(propertyType, usedTypes);
                    }
                }
            }

            if (usedTypes.Count() == 0) 
                return 1;
            
            return usedTypes.Count();
        }

        private void InsertData(ResourceContainer container, int count)
        {
            ResourceContainer currentContainer;
            ResourceType currentType;
            KeyExpression currentKey;
            KeyedResourceInstance currentEntity;
            KeyedResourceInstance parentEntity;

            Queue<object[]> queue = new Queue<object[]>();

            foreach (ResourceType resourceType in container.ResourceTypes)
            {
                if (resourceType.Facets.AbstractType)
                    continue;

                for (int i = 0; i < count; i++)
                {
                    if (CreateKeyedResourceInstance(container, resourceType, out currentKey, out currentEntity))
                    {
                        DataInserter.AddEntity(currentKey, currentEntity);
                        queue.Enqueue(new object[] { currentKey, currentEntity, null });
                    }
                }
            }

            HashSet<ResourceType> typesAdded = new HashSet<ResourceType>();
            while (queue.Count > 0)
            {
                object[] current = queue.Dequeue();
                currentKey = current[0] as KeyExpression;
                currentEntity = current[1] as KeyedResourceInstance;
                parentEntity = current[2] as KeyedResourceInstance;

                currentContainer = currentKey.ResourceContainer;
                currentType = currentKey.ResourceType;

                typesAdded.Add(currentType);

                foreach (ResourceProperty property in currentType.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation))
                {
                    ResourceContainer propertyResourceContainer = container.FindDefaultRelatedContainer(property);

                    ResourceType propertyResourceType = property.OtherAssociationEnd.ResourceType;
                    List<ResourceType> possibleTypes = new List<ResourceType>() { propertyResourceType };
                    possibleTypes.AddRange(propertyResourceType.DerivedTypes);

                    int typeOffset = AstoriaTestProperties.Random.Next(possibleTypes.Count);

                    bool collection = (property.Type is CollectionType);

                    int linked = 1;
                    if (collection)
                        linked = Math.Max(possibleTypes.Count, 2);

                    if (!collection && parentEntity != null && parentEntity.TypeName == propertyResourceType.Name)
                    {
                        DataInserter.AddAssociation(currentEntity, property, parentEntity);
                        continue;
                    }

                    for (int i = 0; i < linked; i++)
                    {
                        ResourceType typeToAdd = possibleTypes[(i + typeOffset) % possibleTypes.Count];
                        
                        if (!typesAdded.Contains(typeToAdd))
                        {
                            // add and link new entity
                            KeyedResourceInstance childEntity;
                            KeyExpression childKey;
                            if (CreateKeyedResourceInstance(propertyResourceContainer, typeToAdd, out childKey, out childEntity))
                            {
                                // add new entity
                                DataInserter.AddEntity(childKey, childEntity);

                                // add it to the queue
                                queue.Enqueue(new object[] { childKey, childEntity, currentEntity });

                                // add to collection
                                DataInserter.AddAssociation(currentEntity, property, childEntity);
                            }
                        }
                        else if (property.OtherAssociationEnd.Multiplicity == Multiplicity.Many)
                        {
                            // link existing entity
                            ResourceContainer otherContainer = container.FindDefaultRelatedContainer(property);
                            KeyExpression otherKey = this.GetRandomGeneratedKey(otherContainer, typeToAdd);
                            if (otherKey != null)
                            {
                                KeyedResourceInstance otherEntity;
                                if (_existingEntityMap.TryGetValue(otherKey, out otherEntity))
                                    DataInserter.AddAssociation(currentEntity, property, otherEntity);
                            }
                        }
                    }

                    // end of this property
                }

                // end of this entity
            }

            DataInserter.Flush();
        }

        private bool CreateKeyedResourceInstance(ResourceContainer container, ResourceType resourceType, out KeyExpression key, out KeyedResourceInstance newResource)
        {
            int retryCount = 5;

            if (!_existingKeyMap.Keys.Contains(container))
                _existingKeyMap.Add(container, new KeyExpressions());

            newResource = resourceType.CreateRandomResource(container, new KeyExpressions(), false); // should not make request
            key = newResource.ResourceInstanceKey.CreateKeyExpression(container, resourceType);

            while (_existingKeyMap[container].Contains(key) && retryCount > 0)
            {
                newResource.ResourceInstanceKey = ResourceInstanceUtil.CreateUniqueKey(container, resourceType, new KeyExpressions(), new KeyExpressions());
                key = newResource.ResourceInstanceKey.CreateKeyExpression(container, resourceType);
                retryCount--;
            }

            if (retryCount == 0)
            {
                newResource = null;
                key = null;
                return false;
            }

            _existingKeyMap[container].Add(key);
            _existingEntityMap[key] = newResource;
            return true;
        }

        private bool Skip(ResourceContainer container)
        {
            return false;
        }
        #endregion
    }
}
