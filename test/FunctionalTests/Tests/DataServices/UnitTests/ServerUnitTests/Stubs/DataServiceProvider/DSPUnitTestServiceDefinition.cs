//---------------------------------------------------------------------
// <copyright file="DSPUnitTestServiceDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using Microsoft.OData.Service.Providers;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Transactions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using test = System.Data.Test.Astoria;

    public class DSPUnitTestServiceDefinition : DSPServiceDefinition, IDisposable
    {
        /// <summary> Modify the generated code afterwards, for example to add anotations to properties.</summary>
        public static Func<StringBuilder, StringBuilder> ModifyGeneratedCode = null;

        /// <summary>What's the provider kind for the given service.</summary>
        public DSPDataProviderKind ProviderKind { get; private set; }

        public DSPUnitTestServiceDefinition(DSPMetadata metadata, DSPDataProviderKind kind, DSPContext context)
        {
            this.Metadata = metadata;

            if (kind == DSPDataProviderKind.CustomProvider)
            {
                this.DataServiceType = typeof(DSPDataService);
                this.ProviderKind = DSPDataProviderKind.CustomProvider;
                this.CreateDataSource = (m) => context;
            }
            else
            {
                Type contextType = GenerateAssemblyAndGetContextType(metadata, kind);
                this.ProviderKind = kind;
                this.DataServiceType = typeof(OpenWebDataService<>).MakeGenericType(contextType);

                if (kind == DSPDataProviderKind.EF || kind == DSPDataProviderKind.Reflection)
                {
                    PopulateContextWithDefaultData(contextType, context, kind);
                }
            }
        }

        public override string ToString()
        {
            return this.ProviderKind.ToString();
        }

        private static void PopulateContextWithDefaultData(Type contextType, DSPContext dspContext, DSPDataProviderKind providerKind)
        {
            Assert.IsTrue(providerKind == DSPDataProviderKind.EF || providerKind == DSPDataProviderKind.Reflection, "expecting only EF and reflection provider");
            Dictionary<DSPResource, object> entitiesAlreadyAdded = new Dictionary<DSPResource, object>();

            // push all the data to the context.
            // create a new instance of the context
            var context = Activator.CreateInstance(contextType);

            // Clear the database if the context is EF context
            if (providerKind == DSPDataProviderKind.EF)
            {
                if (((DbContext)context).Database.Exists())
                {
                    ((DbContext)context).Database.Delete();
                }
            }

            foreach (var set in dspContext.EntitySets)
            {
                string setName = set.Key;
                List<object> entities = set.Value;

                foreach (var entity in entities)
                {
                    DSPResource resource = (DSPResource)entity;
                    if (!entitiesAlreadyAdded.ContainsKey(resource))
                    {
                        Type entityType = contextType.Assembly.GetType(resource.ResourceType.FullName);
                        var entityInstance = Activator.CreateInstance(entityType);
                        if (providerKind == DSPDataProviderKind.EF)
                        {
                            ((DbContext)context).Set(entityType).Add(entityInstance);
                        }
                        else
                        {
                            IList list = (IList)context.GetType().GetField("_" + setName, BindingFlags.Public | BindingFlags.Static).GetValue(context);
                            list.Add(entityInstance);
                        }

                        entitiesAlreadyAdded.Add(resource, entityInstance);
                        PopulateProperties(context, entityInstance, resource, entitiesAlreadyAdded);
                    }
                    else if (providerKind == DSPDataProviderKind.Reflection)
                    {
                        // Since in reflection provider, adding to the collection does not add to the top level set, adding it explicitly now.
                        IList list = (IList)context.GetType().GetField("_" + setName, BindingFlags.Public | BindingFlags.Static).GetValue(context);
                        list.Add(entitiesAlreadyAdded[resource]);
                    }
                }
            }

            if (providerKind == DSPDataProviderKind.EF)
            {
                ((DbContext)context).SaveChanges();
            }
        }

        private static void PopulateProperties(object context, object entity, DSPResource resource, Dictionary<DSPResource, object> entitiesAlreadyAdded)
        {
            Assert.IsTrue(resource.ResourceType.ResourceTypeKind != ResourceTypeKind.EntityType || entitiesAlreadyAdded.ContainsKey(resource), "entitiesAlreadyAdded.ContainsKey(resource)");

            foreach (var property in resource.Properties)
            {
                ResourceProperty resourceProperty = resource.ResourceType.Properties.Single(p => p.Name == property.Key);
                if (resourceProperty.ResourceType.ResourceTypeKind == ResourceTypeKind.Primitive)
                {
                    SetPropertyValue(entity, resourceProperty.Name, property.Value);
                }
                else if (resourceProperty.Kind == ResourcePropertyKind.ResourceReference)
                {
                    DSPResource childResource = (DSPResource)property.Value;
                    object childEntity;
                    if (!entitiesAlreadyAdded.TryGetValue(childResource, out childEntity))
                    {
                        Type entityType = context.GetType().Assembly.GetType(childResource.ResourceType.FullName);
                        childEntity = Activator.CreateInstance(entityType);
                        entitiesAlreadyAdded.Add(childResource, childEntity);
                        PopulateProperties(context, childEntity, childResource, entitiesAlreadyAdded);
                    }

                    SetPropertyValue(entity, resourceProperty.Name, childEntity);
                }
                else if (resourceProperty.Kind == ResourcePropertyKind.ResourceSetReference)
                {
                    List<DSPResource> relatedEntities = (List<DSPResource>)property.Value;
                    object collection = GetPropertyValue(entity, resourceProperty.Name);
                    MethodInfo addMethod = collection.GetType().GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
                    foreach (DSPResource childResource in relatedEntities)
                    {
                        object childEntity;
                        if (!entitiesAlreadyAdded.TryGetValue(childResource, out childEntity))
                        {
                            Type entityType = context.GetType().Assembly.GetType(childResource.ResourceType.FullName);
                            childEntity = Activator.CreateInstance(entityType);
                            entitiesAlreadyAdded.Add(childResource, childEntity);
                            PopulateProperties(context, childEntity, childResource, entitiesAlreadyAdded);
                        }

                        addMethod.Invoke(collection, new object[] { childEntity });
                    }
                }
                else if (resourceProperty.Kind == ResourcePropertyKind.ComplexType)
                {
                    DSPResource complexResource = (DSPResource)property.Value;
                    Type complexType = context.GetType().Assembly.GetType(complexResource.ResourceType.FullName);
                    object complexValue = Activator.CreateInstance(complexType);
                    PopulateProperties(context, complexValue, complexResource, null);
                    SetPropertyValue(entity, resourceProperty.Name, complexValue);
                }
                else
                {
                    // TODO: support collection type properties.
                    throw new Exception("invalid property encountered");
                }
            }
        }

        private static void SetPropertyValue(object instance, string propertyName, object propertyValue)
        {
            PropertyInfo propertyInfo = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            propertyInfo.GetSetMethod().Invoke(instance, new object[] { propertyValue });
        }

        private static object GetPropertyValue(object instance, string propertyName)
        {
            PropertyInfo propertyInfo = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return propertyInfo.GetGetMethod().Invoke(instance, null);
        }

        private static Type GenerateAssemblyAndGetContextType(DSPMetadata metadata, DSPDataProviderKind providerKind)
        {
            Assert.IsTrue(providerKind == DSPDataProviderKind.EF || providerKind == DSPDataProviderKind.Reflection, "expecting only EF and reflection provider");
            StringBuilder stringBuilder = new StringBuilder();

            // Generate context class
            stringBuilder.Append("namespace " + metadata.ContainerNamespace);
            stringBuilder.AppendLine(" { ");
            stringBuilder.AppendLine("using System.Linq;");

            string contextBaseType = (providerKind == DSPDataProviderKind.EF) ? " : System.Data.Entity.DbContext" : String.Empty;
            stringBuilder.Append("public class " + metadata.ContainerName + contextBaseType);
            stringBuilder.AppendLine(" { ");

            foreach (var set in metadata.ResourceSets)
            {
                if (providerKind == DSPDataProviderKind.EF)
                {
                    stringBuilder.AppendLine(String.Format("public System.Data.Entity.DbSet<{0}> {1} {{ get; set; }}", set.ResourceType.FullName, set.Name));
                }
                else
                {
                    stringBuilder.AppendLine(String.Format("public static System.Collections.Generic.List<{0}> _{1} = new System.Collections.Generic.List<{0}>();", set.ResourceType.FullName, set.Name));
                    stringBuilder.AppendLine(String.Format("public System.Linq.IQueryable<{0}> {1} {{ get {{ return _{1}.AsQueryable(); }}}}", set.ResourceType.FullName, set.Name));
                }
            }

            stringBuilder.AppendLine(" } "); // context class end
            stringBuilder.AppendLine(" } "); // end namespace

            // Generate C# code for the given metadata
            foreach (var resource in metadata.Types)
            {
                stringBuilder.Append("namespace " + resource.Namespace);
                stringBuilder.AppendLine(" { ");

                if (resource.ETagProperties.Any())
                {
                    stringBuilder.AppendFormat("[global::Microsoft.OData.Service.ETag({0})]", String.Join(",", resource.ETagProperties.Where(etagProp => resource.PropertiesDeclaredOnThisType.Contains(etagProp)).Select(etagProperty => string.Format("\"{0}\"", etagProperty.Name))));
                }

                stringBuilder.Append("public class " + resource.Name);
                if (resource.BaseType != null)
                {
                    stringBuilder.Append(" : " + resource.BaseType.FullName);
                }

                stringBuilder.AppendLine(" { ");

                // write the default constructor
                stringBuilder.AppendLine("public " + resource.Name + "()");
                stringBuilder.AppendLine("{");
                foreach (var property in resource.PropertiesDeclaredOnThisType.Where(p => p.Kind == ResourcePropertyKind.ResourceSetReference))
                {
                    stringBuilder.AppendLine(String.Format("this.{0} = new System.Collections.Generic.List<{1}>();", property.Name, property.ResourceType.FullName));
                }
                stringBuilder.AppendLine(" } "); // constructor end

                // write all the properties
                foreach (var property in resource.PropertiesDeclaredOnThisType)
                {
                    if (IsPropertyKind(property, ResourcePropertyKind.Primitive))
                    {
                        if (providerKind == DSPDataProviderKind.EF && IsPropertyKind(property, ResourcePropertyKind.Key))
                        {
                            stringBuilder.AppendLine("[System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]");
                        }

                        // Entity Framework's spatial types are not currently the same as the type we use, so we need to use their type for the entity property
                        Type resourcePropertyType = property.ResourceType.InstanceType;
                        Type efPropertyType = resourcePropertyType;

                        stringBuilder.AppendLine(String.Format("public {0} {1} {{ get; set; }}", efPropertyType.ToCSharpString(), property.Name));
                    }
                    else if (IsPropertyKind(property, ResourcePropertyKind.ComplexType) ||
                        property.Kind == ResourcePropertyKind.ResourceReference)
                    {
                        stringBuilder.AppendLine(String.Format("public {0} {1} {{ get; set; }}", property.ResourceType.FullName, property.Name));
                    }
                    else if (property.Kind == ResourcePropertyKind.ResourceSetReference)
                    {
                        stringBuilder.AppendLine(String.Format("public System.Collections.Generic.List<{0}> {1} {{ get; set; }}", property.ResourceType.FullName, property.Name));
                    }
                    else
                    {
                        throw new Exception("Invalid Property encountered");
                    }
                }

                stringBuilder.AppendLine(" } "); // class end
                stringBuilder.AppendLine(" } "); // end namespace
            }

            string contextTypeName = metadata.ContainerNamespace + "." + metadata.ContainerName;
            string assemblyName = contextTypeName + (providerKind == DSPDataProviderKind.EF ? "_EF" : "_Reflection");
            string assemblyFullPath = Path.Combine(test.TestUtil.GeneratedFilesLocation, assemblyName + ".dll");

            // Modify the generated code
            if (DSPUnitTestServiceDefinition.ModifyGeneratedCode != null)
            {
                stringBuilder = ModifyGeneratedCode(stringBuilder);
            }

            // compile the assembly
            string path = Path.GetDirectoryName(typeof(System.Data.Test.Astoria.TestUtil).Assembly.Location);
            string[] dependentAssemblies = new string[]
            {
                    Path.Combine(test.TestUtil.GreenBitsReferenceAssembliesDirectory, "System.ComponentModel.DataAnnotations.dll"),
                    Path.Combine(path,"EntityFramework.dll")
            };

            test.TestUtil.GenerateAssembly(stringBuilder.ToString(), assemblyFullPath, dependentAssemblies);

            Assembly assembly = Assembly.LoadFrom(assemblyFullPath);
            return assembly.GetType(contextTypeName);
        }

        private static bool IsPropertyKind(ResourceProperty property, ResourcePropertyKind propertyKind)
        {
            return (property.Kind & propertyKind) == propertyKind;
        }

        public void Dispose()
        {
            // clean up the database for the EF provider
            if (this.ProviderKind == DSPDataProviderKind.EF)
            {
                Type contextType = this.DataServiceType.GetGenericArguments()[0];
                DbContext context = (DbContext)Activator.CreateInstance(contextType);
                context.Database.Delete();
            }
        }

        public IDisposable CreateChangeScope(DSPContext defaultData)
        {
            if (this.ProviderKind == DSPDataProviderKind.EF)
            {
                return new TransactionScope();
            }
            else
            {
                return new ChangeScope(this, defaultData);
            }
        }

        private class ChangeScope : IDisposable
        {
            private readonly DSPUnitTestServiceDefinition unitTestService;
            private readonly DSPContext defaultData;
            public ChangeScope(DSPUnitTestServiceDefinition service, DSPContext defaultData)
            {
                this.unitTestService = service;
                this.defaultData = defaultData;
            }

            public void Dispose()
            {
                this.unitTestService.ClearChanges();
                this.unitTestService.CreateDataSource = (m) => defaultData;
            }
        }
    }

    public enum DSPDataProviderKind
    {
        Reflection = 0,
        EF = 1,
        CustomProvider = 2
    }

    public static class TypeExtentions
    {
        public static string ToCSharpString(this Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var td = type.GetGenericTypeDefinition();
                if (td == typeof(Nullable<>))
                    return type.GetGenericArguments().Single().ToCSharpString() + "?";

                return td.Name.Substring(0, td.Name.IndexOf('`')) + "<" + string.Join(", ", type.GetGenericArguments().Select(ToCSharpString)) + ">";
            }
            else if (type.IsArray)
            {
                var rank = type.GetArrayRank();
                return type.GetElementType().ToCSharpString() + "[" + new string(',', rank - 1) + "]";
            }
            else
            {
                if (type == typeof(int)) return "int";
                if (type == typeof(uint)) return "uint";
                if (type == typeof(byte)) return "byte";
                if (type == typeof(char)) return "char";
                if (type == typeof(sbyte)) return "sbyte";
                if (type == typeof(long)) return "long";
                if (type == typeof(ulong)) return "ulong";
                if (type == typeof(short)) return "short";
                if (type == typeof(ushort)) return "ushort";
                if (type == typeof(double)) return "double";
                if (type == typeof(float)) return "float";
                if (type == typeof(string)) return "string";
                if (type == typeof(bool)) return "bool";
                if (type == typeof(decimal)) return "decimal";
                if (type == typeof(object)) return "object";

                return type.Name;
            }
        }
    }
}
