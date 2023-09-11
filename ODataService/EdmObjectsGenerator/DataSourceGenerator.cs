namespace DataSourceGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;

    [Serializable]
    public class DataSourceGenerator
    {
        Dictionary<string, TypeBuilderInfo> _typeBuildersDict = new Dictionary<string, TypeBuilderInfo>();
        static Regex collectionRegex = new Regex(@"Collection\((.+)\)", RegexOptions.Compiled);
        Queue<TypeBuilderInfo> _builderQueue = new Queue<TypeBuilderInfo>();
        public class TypeBuilderInfo : MarshalByRefObject
        {
            public bool IsDerived { get; set; }
            public bool IsStructured { get; set; }
            public TypeInfo Builder { get; set; }
        }

        public static IEdmModel ReadModel(string fileName)
        {
            var edmxReaderSettings = new global::Microsoft.OData.Edm.Csdl.CsdlReaderSettings()
            {
                IgnoreUnexpectedAttributesAndElements = true
            };
            var references = global::System.Linq.Enumerable.Empty<global::Microsoft.OData.Edm.IEdmModel>();

            using (var reader = XmlReader.Create(fileName))
            {
                IEdmModel model;
                IEnumerable<EdmError> errors;
                if (CsdlReader.TryParse(reader, references, edmxReaderSettings, out model, out errors))
                {
                    return model;
                }
                return null;
            }
        }

        public void BuildModules(IEdmModel model, ModuleBuilder moduleBuilder, string dataSourceName)
        {
            //first create the basic types for the enums
            foreach (var modelSchemaElement in model.SchemaElements)
            {
                var declaredType = model.FindDeclaredType(modelSchemaElement.FullName());
                if (declaredType == null) continue;
                if (declaredType is IEdmEnumType)
                {
                    CreateType((IEdmEnumType)declaredType, moduleBuilder, declaredType.FullName());
                }
            }

            //next create the basic types for the types
            foreach (var modelSchemaElement in model.SchemaElements)
            {

                var declaredType = model.FindDeclaredType(modelSchemaElement.FullName());
                if (declaredType == null) continue;
                if (!(declaredType is IEdmEnumType))
                {
                    CreateType((IEdmStructuredType)declaredType, moduleBuilder, declaredType.FullName());
                }
                else
                {
                    Compile((IEdmEnumType)declaredType, moduleBuilder, declaredType.FullName());

                }
            }

            //go through and add all elements and their properties but not nav properties
            foreach (var modelSchemaElement in model.SchemaElements)
            {

                var one = model.FindDeclaredType(modelSchemaElement.FullName());
                if (one != null && !(modelSchemaElement is IEdmEnumType))
                {
                    Compile((IEdmStructuredType)one, moduleBuilder, one.FullName());
                }

            }

            //finally add the nav properties
            foreach (var modelSchemaElement in model.SchemaElements)
            {
                if ((modelSchemaElement is IEdmEnumType))
                {
                    continue;
                }
                var one = model.FindDeclaredType(modelSchemaElement.FullName());
                if (one != null)
                {
                    Compile((IEdmStructuredType)one, moduleBuilder, one.FullName(), true);
                }

            }

            //now go through the queue and create the types in dependency order
            while (_builderQueue.Count != 0)
            {
                var typeBuilder = _builderQueue.Dequeue();
                if (typeBuilder.Builder is TypeBuilder)
                {
                    ((TypeBuilder)typeBuilder.Builder).CreateType();

                }
                if (typeBuilder.Builder is EnumBuilder)
                {
                    ((EnumBuilder)typeBuilder.Builder).CreateType();

                }
            }

            // generate the in-memory dataSource
            var dataSource = moduleBuilder.DefineType(dataSourceName, TypeAttributes.Class | TypeAttributes.Public);
            foreach (var entitySet in model.EntityContainer.EntitySets())
            {
                TypeBuilderInfo entityType = _typeBuildersDict.FirstOrDefault(t => t.Key == entitySet.EntityType().FullName()).Value;
                if (entityType != null)
                {
                    Type listOf = typeof(List<>);
                    Type selfContained = listOf.MakeGenericType(entityType.Builder);
                    PropertyBuilderHelper.BuildProperty(dataSource, entitySet.Name, selfContained);
                }
            }

            foreach (var singleton in model.EntityContainer.Singletons())
            {
                TypeBuilderInfo entityType = _typeBuildersDict.FirstOrDefault(t => t.Key == singleton.EntityType().FullName()).Value;
                if (entityType != null)
                {
                    PropertyBuilderHelper.BuildProperty(dataSource, singleton.Name, entityType.Builder);
                }
            }

            dataSource.CreateType();

        }

        internal TypeBuilder CreateType(IEdmStructuredType targetType, ModuleBuilder moduleBuilder, string moduleName)
        {
            if (_typeBuildersDict.ContainsKey(moduleName))
            {
                return (TypeBuilder)_typeBuildersDict[moduleName].Builder;
            }
            if (targetType.BaseType != null)
            {
                TypeBuilder previouslyBuiltType = null;
                if (!_typeBuildersDict.ContainsKey(moduleName))
                {
                    previouslyBuiltType = CreateType(targetType.BaseType, moduleBuilder, targetType.BaseType.FullTypeName());
                }

                var typeBuilder = moduleBuilder.DefineType(moduleName, TypeAttributes.Class | TypeAttributes.Public, previouslyBuiltType);
                var typeBuilderInfo = new TypeBuilderInfo() { Builder = typeBuilder, IsDerived = true };
                _typeBuildersDict.Add(moduleName, typeBuilderInfo);
                _builderQueue.Enqueue(typeBuilderInfo);
                return typeBuilder;

            }
            else
            {
                var typeBuilder = moduleBuilder.DefineType(moduleName, TypeAttributes.Class | TypeAttributes.Public);
                var builderInfo = new TypeBuilderInfo() { Builder = typeBuilder, IsDerived = false };
                _typeBuildersDict.Add(moduleName, builderInfo);
                _builderQueue.Enqueue(builderInfo);

                return typeBuilder;
            }
        }

        internal void Compile(IEdmStructuredType type, ModuleBuilder moduleBuilder, string moduleName, bool navPass = false)
        {
            TypeBuilder typeBuilder = null;

            int iKey = 0;
            int keyCount = 0;
            IEdmEntityType entityType = type as IEdmEntityType;
            if (entityType != null)
            {
                IEnumerable<IEdmStructuralProperty> keyProperties = entityType.DeclaredKey;
                if (keyProperties != null)
                {
                    keyCount = keyProperties.Count();
                }
            }

            if (type.BaseType != null && !navPass)
            {
                typeBuilder = CreateType(type, moduleBuilder, moduleName);
            }

            if (!navPass)
            {
                if (typeBuilder == null)
                {
                    typeBuilder = CreateType(type, moduleBuilder, moduleName);
                }

                foreach (var property in type.DeclaredProperties)
                {
                    if (property.PropertyKind != EdmPropertyKind.Navigation)
                    {
                        GenerateProperty(property, typeBuilder, moduleBuilder, property.IsKey() ? keyCount == 1 ? -1 : iKey++ : (int?)null);
                    }
                }
            }
            else
            {
                typeBuilder = (TypeBuilder)_typeBuildersDict[moduleName].Builder;
                foreach (var property in type.DeclaredProperties)
                {
                    if (property.PropertyKind == EdmPropertyKind.Navigation)
                        GenerateProperty(property, typeBuilder, moduleBuilder);
                }
            }
        }

        internal EnumBuilder CreateType(IEdmEnumType targetType, ModuleBuilder moduleBuilder, string moduleName)
        {
            if (_typeBuildersDict.ContainsKey(moduleName))
            {
                return (EnumBuilder)_typeBuildersDict[moduleName].Builder;
            }

            EnumBuilder typeBuilder = moduleBuilder.DefineEnum(moduleName, TypeAttributes.Public, typeof(int));
            var builderInfo = new TypeBuilderInfo() { Builder = typeBuilder, IsDerived = false };
            _typeBuildersDict.Add(moduleName, builderInfo);
            _builderQueue.Enqueue(builderInfo);
            return typeBuilder;
        }

        internal void Compile(IEdmEnumType type, ModuleBuilder moduleBuilder, string moduleName)
        {
            var typeBuilder = CreateType(type, moduleBuilder, moduleName);
            foreach (var enumMember in type.Members)
            {
                GenerateEnum(enumMember, typeBuilder, moduleBuilder);
            }
        }

        internal void GenerateProperty(IEdmProperty property, TypeBuilder typeBuilder, ModuleBuilder moduleBuilder, int? keyIndex = null)
        {
            var propertyName = property.Name;
            var emdPropType = property.Type.PrimitiveKind();
            var propertyType = GetPrimitiveClrType(emdPropType, false);
            if (propertyType == null)
            {
                if (property.Type.FullName().ToLower().Contains("geography"))
                {
                    return;
                }

                if (property.PropertyKind == EdmPropertyKind.Navigation)
                {
                    if (property.Type.FullName().StartsWith("Collection"))
                    {
                        var typeName = collectionRegex.Match(property.Type.FullName()).Groups[1].Value;
                        Type listOf = typeof(List<>);
                        var baseType = _typeBuildersDict.ContainsKey(typeName) ? _typeBuildersDict[typeName].Builder : typeof(string);

                        var selfContained = listOf.MakeGenericType(baseType);
                        propertyType = selfContained;
                    }
                    else
                    {
                        var navProptype = _typeBuildersDict[property.Type.FullName()];
                        propertyType = navProptype.Builder;
                    }
                }
                else
                {
                    if (property.Type.FullName().StartsWith("Collection"))
                    {
                        var typeName = collectionRegex.Match(property.Type.FullName()).Groups[1].Value;
                        Type listOf = typeof(List<>);
                        var baseType = _typeBuildersDict.ContainsKey(typeName) ? _typeBuildersDict[typeName].Builder : typeof(string);
                        var selfContained = listOf.MakeGenericType(baseType);
                        propertyType = selfContained;
                    }
                    else
                    {
                        var previouslyBuiltType = _typeBuildersDict[property.Type.FullName()];

                        propertyType = previouslyBuiltType.Builder;
                    }
                }
            }

            if (property.Type.IsNullable && propertyType.IsValueType)
            {
                Type nullableOf = typeof(Nullable<>);
                Type selfContained = nullableOf.MakeGenericType(propertyType);
                propertyType = selfContained;
            }

            PropertyBuilderHelper.BuildProperty(typeBuilder, propertyName, propertyType, keyIndex);
        }

        internal static void GenerateEnum(IEdmEnumMember member, EnumBuilder enumBuilder, ModuleBuilder moduleBuilder)
        {
            var memberName = member.Name;

            var memberValue = Convert.ToInt32(member.Value.Value);
            enumBuilder.DefineLiteral(memberName, memberValue);
        }

        /// <summary>
        /// Get Clr type
        /// </summary>
        /// <param name="typeKind">Edm Primitive Type Kind</param>
        /// <param name="isNullable">Nullable value</param>
        /// <returns>CLR type</returns>
        private static Type GetPrimitiveClrType(EdmPrimitiveTypeKind typeKind, bool isNullable)
        {
            switch (typeKind)
            {
                case EdmPrimitiveTypeKind.Binary:
                    return typeof(byte[]);
                case EdmPrimitiveTypeKind.Boolean:
                    return isNullable ? typeof(Boolean?) : typeof(Boolean);
                case EdmPrimitiveTypeKind.Byte:
                    return isNullable ? typeof(Byte?) : typeof(Byte);
                case EdmPrimitiveTypeKind.Date:
                    return isNullable ? typeof(Date?) : typeof(Date);
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return isNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
                case EdmPrimitiveTypeKind.Decimal:
                    return isNullable ? typeof(Decimal?) : typeof(Decimal);
                case EdmPrimitiveTypeKind.Double:
                    return isNullable ? typeof(Double?) : typeof(Double);
                case EdmPrimitiveTypeKind.Guid:
                    return isNullable ? typeof(Guid?) : typeof(Guid);
                case EdmPrimitiveTypeKind.Int16:
                    return isNullable ? typeof(Int16?) : typeof(Int16);
                case EdmPrimitiveTypeKind.Int32:
                    return isNullable ? typeof(Int32?) : typeof(Int32);
                case EdmPrimitiveTypeKind.Int64:
                    return isNullable ? typeof(Int64?) : typeof(Int64);
                case EdmPrimitiveTypeKind.SByte:
                    return isNullable ? typeof(SByte?) : typeof(SByte);
                case EdmPrimitiveTypeKind.Single:
                    return isNullable ? typeof(Single?) : typeof(Single);
                case EdmPrimitiveTypeKind.Stream:
                    return typeof(Stream);
                case EdmPrimitiveTypeKind.String:
                    return typeof(String);
                case EdmPrimitiveTypeKind.Duration:
                    return isNullable ? typeof(TimeSpan?) : typeof(TimeSpan);
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return isNullable ? typeof(TimeOfDay?) : typeof(TimeOfDay);
                default:
                    return null;
            }
        }

        // used to pass the csdlFileName to a different app domain.
        // could also use Domain.SetData()/Domain.GetData() on the new domain
        public string csdlFileName { get; set; }

        public static Type GenerateDataSource(IEdmModel model, string dataSourceName, bool save=false)
        {
            // Get name for assembly
            string assemblyName = dataSourceName + "_Assembly";

            // See if assembly already exists
            AppDomain appDomain = AppDomain.CurrentDomain;
            Assembly assembly = appDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);

            if (assembly == null)
            {
                // Build Assembly
                AssemblyName assembly_Name = new AssemblyName(assemblyName);
                AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assembly_Name, AssemblyBuilderAccess.RunAndCollect);
                ModuleBuilder module = assemblyBuilder.DefineDynamicModule($"{assembly_Name.Name}");
                DataSourceGenerator generator = new DataSourceGenerator();
                generator.BuildModules(model, module, dataSourceName);

                // save for debugging purposes
                if (save)
                {
                    assemblyBuilder.Save($"{assembly_Name.Name}.dll");
                }

                assembly = appDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
            }


            //// Return generated DataSource
            //Type dataSource =  assembly.GetTypes().FirstOrDefault(t => t.Name == dataSourceName + "Context");
            //return dataSource;
            return null;
        }

        public void GenerateDataSourceInANewAppDomain(IEdmModel model, string dataSourceName)
        {
            // Get filename from static object; could also pass in Domain.SetData()
            string fileName = this.csdlFileName;

            // Call generator
            Type dataSourceType = GenerateDataSource(model, dataSourceName);

            // try to pass back to calling assembly. This does not work.
            AppDomain domain = AppDomain.CurrentDomain.GetData("domain") as AppDomain;
            domain.SetData("dataSourceType", dataSourceType);
        }
    }
}
